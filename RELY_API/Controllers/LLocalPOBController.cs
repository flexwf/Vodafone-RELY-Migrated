
using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LLocalPOBController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        [HttpGet]
        public IHttpActionResult DeleteMappingRow(int RowId,string Type)
        {
            db.Database.ExecuteSqlCommand("Exec P_DeleteEffectiveDatedRow {0},{1}", RowId, Type);
            return Ok();
        }

        public IHttpActionResult GetGPOBDataForGrid(int PobCatalogueId,string GpobType, string CompanyCode)
        {
            if (GpobType.Equals("OTHERS"))
            {
                var xx = (from m in db.MLocalGlobalUsecaseMappings
                          where m.PobCatalogueId == PobCatalogueId
                          where m.CompanyCode == CompanyCode
                          join gp in db.GGlobalPobs on m.GPobId equals gp.Id
                          where  !(gp.Type == "RETENTION" || gp.Type == "ACQUISIT")
                          select new
                          {
                              m.Id,
                              m.PobCatalogueId,
                              m.GPobId,
                              m.EffectiveStartDate,
                              m.EffectiveEndDate,
                              Name = gp.Name,
                              m.UseCaseIndicator
                          }).OrderByDescending(m => m.Id).ToList();
                return Ok(xx);
            }
            else
            {
                var xx = (from m in db.MLocalGlobalUsecaseMappings
                          where m.PobCatalogueId == PobCatalogueId
                          where m.CompanyCode == CompanyCode
                          join gp in db.GGlobalPobs on m.GPobId equals gp.Id
                          where gp.Type == GpobType
                          select new
                          {
                              m.Id,
                              m.PobCatalogueId,
                              m.GPobId,
                              m.EffectiveStartDate,
                              m.EffectiveEndDate,
                              Name = gp.Name,
                              m.UseCaseIndicator
                          }).OrderByDescending(m => m.Id).ToList();
                return Ok(xx);
            }
        }
        [HttpGet]
        public IHttpActionResult AttachGpob(int PobCatalogueId, string GpobMappingStartDate, int GpobId, string CompanyCode, string GpobType)
        {
            //if ("null".Equals(UCIndicator))
            //{
            //    UCIndicator = " ";
            //}
            string UCIndicator = "";
            switch (GpobType)
            {
                case "ACQUISIT":
                    UCIndicator = "1";
                    break;
                case "RETENTION":
                    UCIndicator = "2";
                    break;
                default:
                    UCIndicator = "0";
                    break;
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Date manipulations for Effective Start and End Date
                    DateTime StartDate = new DateTime();
                    if (!string.IsNullOrEmpty(GpobMappingStartDate))
                    {
                        GpobMappingStartDate = GpobMappingStartDate + " 13:00:00";//This is just a workaround. due to some time/offset difference, db was saving dates with 2hrs difference. 
                        StartDate = DateTime.ParseExact(GpobMappingStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    DateTime EndDate = new DateTime(2099, 12, 31);
                    MLocalGlobalUsecaseMapping model = new MLocalGlobalUsecaseMapping()
                    {
                        PobCatalogueId = PobCatalogueId,
                        GPobId = GpobId,
                        CompanyCode = CompanyCode,
                        EffectiveStartDate = StartDate,
                        EffectiveEndDate = EndDate ,
                        UseCaseIndicator = string.IsNullOrEmpty(UCIndicator) ?" ": UCIndicator
                    };
                    //EnDt of previous will be -1 StDt of new version.
                    var PreviousVersionEndDate = model.EffectiveStartDate.AddDays(-1);
                    if (GpobType.Equals("OTHERS"))
                    {
                        string qry11 = "update MLocalGlobalUsecaseMapping set EffectiveEndDate= {1} from MLocalGlobalUsecaseMapping t1 " 
                            +" inner join(select PobCatalogueId, max(EffectiveStartDate) as MaxDate from MLocalGlobalUsecaseMapping mm "
                        + " join GGlobalPobs gc on gc.Id = mm.GPobId where PobCatalogueId = {0} and gc.Type NOT IN ('ACQUISIT','RETENTION') group by PobCatalogueId) t2 "
                       + " on t1.PobCatalogueId = t2.PobCatalogueId and t1.EffectiveStartDate = t2.MaxDate where t1.PobCatalogueId = {0} " +
                       "and t1.UseCaseIndicator ={2} ";
                        db.Database.ExecuteSqlCommand(qry11, model.PobCatalogueId, PreviousVersionEndDate, UCIndicator);
                    }
                    else
                    {
                        string qry = "update MLocalGlobalUsecaseMapping set EffectiveEndDate= {1} from MLocalGlobalUsecaseMapping t1 " 
                            +" inner join(select PobCatalogueId, max(EffectiveStartDate) as MaxDate from MLocalGlobalUsecaseMapping mm "
                            + " join GGlobalPobs gc on gc.Id = mm.GPobId where PobCatalogueId = {0} and gc.Type = {3} group by PobCatalogueId) t2 "
                           + " on t1.PobCatalogueId = t2.PobCatalogueId and t1.EffectiveStartDate = t2.MaxDate where t1.PobCatalogueId = {0} " +
                           "and t1.UseCaseIndicator ={2} ";
                        db.Database.ExecuteSqlCommand(qry, model.PobCatalogueId, PreviousVersionEndDate, UCIndicator, GpobType);
                    }
                    db.MLocalGlobalUsecaseMappings.Add(model);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            //DateTime LatestMappedStartDate = db.MLocalGlobalUsecaseMappings.Where(a => a.PobCatalogueId == PobCatalogueId).Select(a => a.EffectiveStartDate).FirstOrDefault();
            string qry1 = "";
            if (GpobType.Equals("OTHERS"))
            {
                qry1 = "select EffectiveStartDate from MLocalGlobalUsecaseMapping mm join GGlobalPobs gp on gp.id = mm.GPobId" +
                       " where PobCatalogueId = {0}  and gp.Type NOT IN ('ACQUISIT','RETENTION') order by mm.id desc ";
                var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId).FirstOrDefault();
                return Ok(LatestMappedStartDate);
            }
            else
            {
                qry1 = "select EffectiveStartDate from MLocalGlobalUsecaseMapping mm join GGlobalPobs gp on gp.id = mm.GPobId" +
                       " where PobCatalogueId = {0}  and gp.Type = {1} order by mm.id desc ";
                var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId, GpobType).FirstOrDefault();
                return Ok(LatestMappedStartDate);
            }
        }
        public IHttpActionResult GetCopaDataForGrid(int PobCatalogueId, string CompanyCode,int CopaClass)
        {
            var xx = (from m in db.MPobCopaMappings
                      where m.PobCatalogueId == PobCatalogueId 
                      where m.CompanyCode == CompanyCode
                      join gp in db.GCopaDimensions on m.CopaId equals gp.Id
                      where gp.Class == CopaClass
                      select new
                      {
                          m.Id,
                          m.PobCatalogueId,
                          m.CopaId,
                          m.EffectiveStartDate,
                          m.EffectiveEndDate,
                          Dimension = gp.Dimension,
                          Class = gp.Class,
                          Description = gp.Description,
                          DimentionClassDescription = gp.DimentionClassDescription,
                          CopaValue = gp.CopaValue
                      }).OrderByDescending(m => m.Id).ToList();
            return Ok(xx);
        }
        [HttpGet]
        public IHttpActionResult AttachCopa(int PobCatalogueId, string CopaMappingStartDate, int CopaId, string CompanyCode,int CopaClass)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Date manipulations for Effective Start and End Date
                    DateTime StartDate = new DateTime();
                    if (!string.IsNullOrEmpty(CopaMappingStartDate))
                    {
                        CopaMappingStartDate = CopaMappingStartDate + " 13:00:00";//This is just a workaround. due to some time/offset difference, db was saving dates with 2hrs difference. 
                        StartDate = DateTime.ParseExact(CopaMappingStartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    DateTime EndDate = new DateTime(2099, 12, 31);
                    MPobCopaMapping model = new MPobCopaMapping()
                    { PobCatalogueId = PobCatalogueId, CopaId = CopaId, CompanyCode = CompanyCode, EffectiveStartDate = StartDate, EffectiveEndDate = EndDate };
                    //EnDt of previous will be -1 StDt of new version.
                    var PreviousVersionEndDate = model.EffectiveStartDate.AddDays(-1);
                    string qry = "update MPobCopaMapping set EffectiveEndDate= {1} from MPobCopaMapping t1 "
                        + " inner join(select PobCatalogueId, max(EffectiveStartDate) as MaxDate from MPobCopaMapping mp join" +
                        " GCopaDimensions gc on gc.Id = mp.CopaId where PobCatalogueId = {0} and gc.class={2} group by PobCatalogueId  ) t2 "
                       + " on t1.PobCatalogueId = t2.PobCatalogueId and t1.EffectiveStartDate = t2.MaxDate " +
                       " inner join GCopaDimensions gc1 on gc1.id = t1.CopaId where t1.PobCatalogueId = {0}  and gc1.Class={2} ";
                    db.Database.ExecuteSqlCommand(qry, model.PobCatalogueId, PreviousVersionEndDate,CopaClass);

                    db.MPobCopaMappings.Add(model);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            string qry1 = "select EffectiveStartDate from MPobCopaMapping mp join GCopaDimensions gc on gc.Id = mp.CopaId " +
                "where PobCatalogueId = {0} and gc.class={1} order by mp.id desc ";
            var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId,CopaClass).FirstOrDefault();
            return Ok(LatestMappedStartDate);
        }
        public IHttpActionResult GetLatestMappedCopaStartDate(int PobCatalogueId,int CopaClass)
        {
            string qry1 = "select EffectiveStartDate from MPobCopaMapping mp join GCopaDimensions gc on gc.Id = mp.CopaId " +
                "where PobCatalogueId = {0} and gc.class={1} order by mp.id desc ";
            var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId, CopaClass).FirstOrDefault();
            return Ok(LatestMappedStartDate);
        }
        public IHttpActionResult GetLatestMappedGpobStartDate(int PobCatalogueId,string GpobType)
        {
            string qry1 = "";
            if (GpobType.Equals("OTHERS"))
            {
                qry1 = "select EffectiveStartDate from MLocalGlobalUsecaseMapping mm join GGlobalPobs gp on gp.id = mm.GPobId" +
                       " where PobCatalogueId = {0}  and gp.Type NOT IN ('ACQUISIT','RETENTION') order by mm.id desc ";
                var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId).FirstOrDefault();
                return Ok(LatestMappedStartDate);
            }
            else
            {
                qry1 = "select EffectiveStartDate from MLocalGlobalUsecaseMapping mm join GGlobalPobs gp on gp.id = mm.GPobId" +
                       " where PobCatalogueId = {0}  and gp.Type = {1} order by mm.id desc ";
                var LatestMappedStartDate = db.Database.SqlQuery<DateTime>(qry1, PobCatalogueId, GpobType).FirstOrDefault();
                return Ok(LatestMappedStartDate);
            }
            
        }
        [HttpGet]
        public IHttpActionResult CloneLPOB(int LPobId, int loggedInUserId, int LoggedInUserRoleId, string CompanyCode, string Source)
        {
            var OldPob = db.LLocalPobs.Where(a => a.Id == LPobId).FirstOrDefault();
            LLocalPob ClonedLPob = null;
            //clone for Completed Lpobs
            if ("Completed".Equals(OldPob.WFStatus))
            {
                ObjectParameter ClonedPobId = new ObjectParameter("ClonedPobId", typeof(int)); //return parameter is declared
                db.SPCloneLPob(LPobId, loggedInUserId, LoggedInUserRoleId, CompanyCode, Source, ClonedPobId);
                int ClonedId = (int)ClonedPobId.Value; //getting value of output parameter
                ClonedLPob = db.LLocalPobs.Where(a => a.Id == ClonedId).FirstOrDefault();
            }
            else
            {
                ClonedLPob = OldPob;
            }
            
            return Ok(ClonedLPob);
        }
        [HttpGet]
        public IHttpActionResult GetPreviousVersionStartDate(int Id)
        {
            var pob = db.LLocalPobs.Where(a => a.Id == Id).FirstOrDefault();
            int CurrentVersion = pob.Version;
            int? SourcePId = pob.PobCatalogueId;
            DateTime? PreviousVersionStartDate = null;
            if (CurrentVersion > 1)
            {
                //PreviousVersionStartDate = db.LLocalPobs.Where(a => a.Version == CurrentVersion - 1).Where(a => a.PobCatalogueId == SourcePId).Select(a => a.AttributeD01).FirstOrDefault();
                PreviousVersionStartDate = db.LLocalPobs.Where(a => a.Version == CurrentVersion - 1).Where(a => a.PobCatalogueId == SourcePId).Select(a => a.EffectiveStartDate).FirstOrDefault();
            }
            else
            {
                PreviousVersionStartDate = db.LLocalPobs.Where(a => a.Version == CurrentVersion).Where(a => a.PobCatalogueId == SourcePId).Select(a => a.EffectiveStartDate).FirstOrDefault();
            }
            return Ok(PreviousVersionStartDate);
        }
        public async Task<IHttpActionResult> GetLatestPOBCreatedOnFlyForProduct(string CompanyCode, int ProductId)
        {
            var SqlQuery = "select TOP(1) aa.Id, aa.LocalPobTypeId, aa.Name,LLocalPOBType = lp.Name,aa.Description,aa.GlobalPobId1, POB1 = gp1.Name + '(' + gp1.Type + ' ' + gp1.Category + ' ' + gp1.IFRS15Account + ')',aa.GlobalPobId2, Retention = gp2.Name + '(' + gp2.Type + ' ' + gp2.Category + ' ' + gp2.IFRS15Account + ')', "
                          +" aa.CopaId1,COPA2 = gc1.CopaValue + ' ' + gc1.Description + ' (' + gc1.Dimension + ')' ,aa.CopaId2, COPA5 = gc2.CopaValue + ' ' + gc2.Description + ' (' + gc2.Dimension + ')',aa.CopaId3,COPA22 = gc3.CopaValue + ' ' + gc3.Description + ' (' + gc3.Dimension + ')', "
                           + " aa.CopaId4,COPA52 = gc4.CopaValue + ' ' + gc4.Description + ' (' + gc4.Dimension + ')'  from LLocalPobs aa left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id left outer join GGlobalPobs gp1 on aa.GlobalPobId1 = gp1.Id left outer join GGlobalPobs gp2 on aa.GlobalPobId2 = gp2.Id left outer join GCopaDimensions gc1 on aa.CopaId1 = gc1.Id left outer join GCopaDimensions gc2 on aa.CopaId2 = gc2.Id"
                            + " left outer join GCopaDimensions gc3 on aa.CopaId3 = gc3.Id left outer join GCopaDimensions gc4 on aa.CopaId4 = gc4.Id "
                            + " left outer join LProductPobs pp on pp.LocalPobId = lp.Id "
                            + " where  aa.WFOrdinal=0 and aa.WFStatus='Parked' and aa.CompanyCode={0} and  pp.ProductId != {1} Order by aa.Id desc";
            //As the following query includes sub query, writing it direct SQL with placeholders instead of LINQ.
            var POB = await db.Database.SqlQuery<LocalPobForProductViewModel>(SqlQuery, CompanyCode, ProductId).FirstOrDefaultAsync();
            return Ok(POB);
        }
        public IHttpActionResult GetLLocalPOBs()
        {//more columns need to be added
            var xx = (from aa in db.LLocalPobs
                      join bb in db.RLocalPobTypes on aa.LocalPobTypeId equals bb.Id
                      select new { aa.Id, aa.CompanyCode, aa.Name,aa.LocalPobTypeId, aa.Description,aa.Version,aa.PobCatalogueId,
                          aa.AttributeC01, aa.AttributeC02, aa.AttributeC03, aa.AttributeC04, aa.AttributeC05,aa.AttributeC06,
                          aa.AttributeC07,aa.AttributeC08,aa.AttributeC09,aa.AttributeC10,aa.AttributeC11,aa.AttributeC12,
                          aa.AttributeC13,aa.AttributeC14,aa.AttributeC15,aa.AttributeC16,aa.AttributeC17,aa.AttributeC18,aa.AttributeC19,aa.AttributeC20,
                          aa.AttributeI01,aa.AttributeI02,aa.AttributeI03,aa.AttributeI04,aa.AttributeI05,aa.AttributeI06,aa.AttributeI07,aa.AttributeI08,aa.AttributeI09,aa.AttributeI10,
                          aa.AttributeN01,aa.AttributeN02,aa.AttributeN03,aa.AttributeN04,aa.AttributeN05,aa.AttributeN06,aa.AttributeN07,aa.AttributeN08,aa.AttributeN09,aa.AttributeN10,
                          aa.AttributeD01,aa.AttributeD02,aa.AttributeD03,aa.AttributeD04,aa.AttributeD05,aa.AttributeD06,aa.AttributeD07,aa.AttributeD08,aa.AttributeD09,aa.AttributeD10,
                          aa.AttributeB01,aa.AttributeB02,aa.AttributeB03,aa.AttributeB04,aa.AttributeB05,aa.AttributeB06,aa.AttributeB07,aa.AttributeB08,aa.AttributeB09,aa.AttributeB10, 
                          aa.WFOrdinal,aa.WFStatus,aa.WFType,aa.WFComments,aa.WFRequesterId,aa.WFAnalystId,aa.WFManagerId,aa.WFCurrentOwnerId,aa.WFRequesterRoleId,
                          aa.CreatedById,aa.UpdatedById,aa.CreatedDateTime,aa.UpdatedDateTime,
                          LocalPobTypeName = bb.Name,
                          aa.SspId
                      }).OrderBy(p => p.Name);
            return Ok(xx);
        }

        /// <summary>
        /// Method to get the counts of data from the base table
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public IHttpActionResult GetAllLocalPOBCountsByCompanyCode(string CompanyCode)
        {
            var counts = db.Database.SqlQuery<int>("select count(*) from LLocalPobs lp where lp.CompanyCode={0}", CompanyCode).FirstOrDefault();
            return Ok(counts);
        }

        /// <summary>
        /// Method to call StoreProcedure to get data
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <param name="sortdatafield"></param>
        /// <param name="sortorder"></param>
        /// <param name="FilterQuery"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public IHttpActionResult GetAllLPobsByCompanyCode(string CompanyCode, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize)
        {
            //[SpGenerateLocalPOBLibrary]
            var Query = "Exec SpGenerateLocalPOBLibrary @CompanyCode,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value :(object) FilterQuery);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
            //var SqlQuery = "select aa.Id, aa.LocalPobTypeId, aa.Name,LLocalPOBType = lp.Name,aa.Description," +
            //    "aa.GlobalPobId1, POB1 = gp1.Name + '(' + gp1.Type + ' ' + gp1.Category + ' ' + gp1.IFRS15Account + ')'," +
            //    "aa.GlobalPobId2, Retention = gp2.Name + '(' + gp2.Type + ' ' + gp2.Category + ' ' + gp2.IFRS15Account + ')', "
            //    + " CASE When aa.WFStatus = 'Parked' Then  'On the Fly' when aa.WFStatus ='Saved' OR aa.WFStatus='InProgress' Then 'InProgress' else aa.WFStatus END Status, aa.Version, "
            //     + " aa.CopaId1,COPA2 = gc1.CopaValue + ' ' + gc1.Description + ' (' + gc1.Dimension + ')' ,aa.CopaId2, COPA5 = gc2.CopaValue + ' ' + gc2.Description + ' (' + gc2.Dimension + ')',aa.CopaId3,COPA22 = gc3.CopaValue + ' ' + gc3.Description + ' (' + gc3.Dimension + ')', "
            //               + " aa.CopaId4,COPA52 = gc4.CopaValue + ' ' + gc4.Description + ' (' + gc4.Dimension + ')'  from LLocalPobs aa left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id left outer join GGlobalPobs gp1 on aa.GlobalPobId1 = gp1.Id left outer join GGlobalPobs gp2 on aa.GlobalPobId2 = gp2.Id left outer join GCopaDimensions gc1 on aa.CopaId1 = gc1.Id left outer join GCopaDimensions gc2 on aa.CopaId2 = gc2.Id "
            //               + " left outer join GCopaDimensions gc3 on aa.CopaId3 = gc3.Id left outer join GCopaDimensions gc4 on aa.CopaId4 = gc4.Id where aa.CompanyCode = {0}";

            //var LLocalPOB = db.Database.SqlQuery<LocalPobForProductViewModel>(SqlQuery, CompanyCode).ToList();
            ////return Ok(LLocalPOB);

            //return Ok(LLocalPOB);
        }
    [ResponseType(typeof(LocalPobForProductViewModel))]
        public async Task<IHttpActionResult> GetLLocalPOBForProduct(int PobCatalogueId,int ProductId)
        {
            //var SqlQuery = "select pp.EffectiveStartDate,pp.EffectiveEndDate ,aa.Id, aa.LocalPobTypeId,aa.WFStatus, aa.Name,LLocalPOBType = lp.Name,aa.Description,aa.GlobalPobId1, POB1 = gp1.Name + '(' + gp1.Type + ' ' + gp1.Category + ' ' + gp1.IFRS15Account + ')',aa.GlobalPobId2, Retention = gp2.Name + '(' + gp2.Type + ' ' + gp2.Category + ' ' + gp2.IFRS15Account + ')', "
            //               + " aa.CopaId1,COPA2 = gc1.CopaValue + ' ' + gc1.Description + ' (' + gc1.Dimension + ')' ,aa.CopaId2, COPA5 = gc2.CopaValue + ' ' + gc2.Description + ' (' + gc2.Dimension + ')',aa.CopaId3,COPA22 = gc3.CopaValue + ' ' + gc3.Description + ' (' + gc3.Dimension + ')', "
            //               + " aa.CopaId4,COPA52 = gc4.CopaValue + ' ' + gc4.Description + ' (' + gc4.Dimension + ')'  " +
            //               "from LLocalPobs aa left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id " +
            //               "left outer join GGlobalPobs gp1 on aa.GlobalPobId1 = gp1.Id left outer join GGlobalPobs gp2 " +
            //               "on aa.GlobalPobId2 = gp2.Id left outer join GCopaDimensions gc1 on aa.CopaId1 = gc1.Id " +
            //               "left outer join GCopaDimensions gc2 on aa.CopaId2 = gc2.Id "
            //               + " left outer join GCopaDimensions gc3 on aa.CopaId3 = gc3.Id " +
            //               " left outer join GCopaDimensions gc4 on aa.CopaId4 = gc4.Id " +
            //               " left outer join LProductPobs pp on aa.Id = pp.LocalPobId where aa.Id = {0}";
            string qry = "select pp.PobCatalogueId, pp.EffectiveStartDate,pp.EffectiveEndDate ,aa.Id, aa.LocalPobTypeId,aa.WFStatus, aa.Name,LLocalPOBType = lp.Name,aa.Description,aa.Version from LLocalPobs aa  "
                        + " left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id  left outer join LProductPobs pp on aa.PobCatalogueId = pp.PobCatalogueId"
                        + " where aa.PobCatalogueId = {0} and pp.ProductId = {1} and aa.WFStatus in ( 'Completed','Parked')";
                        
            var LLocalPOB = db.Database.SqlQuery<LocalPobForProductViewModel>(qry, PobCatalogueId,ProductId).FirstOrDefault();
            return Ok(LLocalPOB);
        }

        // GET: api/LLocalPOBs/5
        [ResponseType(typeof(LLocalPob))]
        public async Task<IHttpActionResult> GetLLocalPOB(int id)
        {
            var LLocalPOB = (from aa in db.LLocalPobs
                             join bb in db.RLocalPobTypes on aa.LocalPobTypeId equals bb.Id where aa.Id == id
                select  new {
                aa.Id,
                aa.PobCatalogueId,
                aa.CompanyCode,
                aa.Name,
                aa.LocalPobTypeId,
                aa.Description,
                aa.Version,
                aa.AttributeC01,
                aa.AttributeC02,
                aa.AttributeC03,
                aa.AttributeC04,
                aa.AttributeC05,
                aa.AttributeC06,
                aa.AttributeC07,
                aa.AttributeC08,
                aa.AttributeC09,
                aa.AttributeC10,
                aa.AttributeC11,
                aa.AttributeC12,
                aa.AttributeC13,
                aa.AttributeC14,
                aa.AttributeC15,
                aa.AttributeC16,
                aa.AttributeC17,
                aa.AttributeC18,
                aa.AttributeC19,
                aa.AttributeC20,
                aa.AttributeI01,
                aa.AttributeI02,
                aa.AttributeI03,
                aa.AttributeI04,
                aa.AttributeI05,
                aa.AttributeI06,
                aa.AttributeI07,
                aa.AttributeI08,
                aa.AttributeI09,
                aa.AttributeI10,
                aa.AttributeN01,
                aa.AttributeN02,
                aa.AttributeN03,
                aa.AttributeN04,
                aa.AttributeN05,
                aa.AttributeN06,
                aa.AttributeN07,
                aa.AttributeN08,
                aa.AttributeN09,
                aa.AttributeN10,
                aa.AttributeD01,
                aa.AttributeD02,
                aa.AttributeD03,
                aa.AttributeD04,
                aa.AttributeD05,
                aa.AttributeD06,
                aa.AttributeD07,
                aa.AttributeD08,
                aa.AttributeD09,
                aa.AttributeD10,
                aa.AttributeB01,
                aa.AttributeB02,
                aa.AttributeB03,
                aa.AttributeB04,
                aa.AttributeB05,
                aa.AttributeB06,
                aa.AttributeB07,
                aa.AttributeB08,
                aa.AttributeB09,
                aa.AttributeB10,
                aa.WFOrdinal,
                aa.WFStatus,
                aa.WFType,
                aa.WFComments,
                aa.WFRequesterId,
                aa.WFAnalystId,
                aa.WFManagerId,
                aa.WFCurrentOwnerId,
                aa.WFRequesterRoleId,
                aa.CreatedById,
                aa.UpdatedById,
                aa.CreatedDateTime,
                aa.UpdatedDateTime,
                LocalPobTypeName = bb.Name,
                aa.EffectiveStartDate,
                aa.EffectiveEndDate,
                //aa.CopaId1,
                //aa.CopaId2,
                //aa.CopaId3,aa.CopaId4,
                //aa.GlobalPobId1,aa.GlobalPobId2,
                    aa.SspId
                }).FirstOrDefault();
            if (LLocalPOB == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
            }
            return Ok(LLocalPOB);
        }

        [ResponseType(typeof(LLocalPob))]
        public async Task<IHttpActionResult> GetLLocalPOBVersions(string Name,string CompanyCode,int TypeId,string UserName,string WorkFlow)
        {
            var LLocalPOB = (from aa in db.LLocalPobs
                             join bb in db.RLocalPobTypes on aa.LocalPobTypeId equals bb.Id
                             where aa.Name == Name && aa.CompanyCode == CompanyCode && aa.LocalPobTypeId == TypeId
                             select new
                             {
                                 aa.Id,
                                 aa.PobCatalogueId,
                                 aa.CompanyCode,
                                 aa.Name,
                                 aa.LocalPobTypeId,
                                 aa.Description,
                                 aa.Version,
                                 aa.AttributeC01,
                                 aa.AttributeC02,
                                 aa.AttributeC03,
                                 aa.AttributeC04,
                                 aa.AttributeC05,
                                 aa.AttributeC06,
                                 aa.AttributeC07,
                                 aa.AttributeC08,
                                 aa.AttributeC09,
                                 aa.AttributeC10,
                                 aa.AttributeC11,
                                 aa.AttributeC12,
                                 aa.AttributeC13,
                                 aa.AttributeC14,
                                 aa.AttributeC15,
                                 aa.AttributeC16,
                                 aa.AttributeC17,
                                 aa.AttributeC18,
                                 aa.AttributeC19,
                                 aa.AttributeC20,
                                 aa.AttributeI01,
                                 aa.AttributeI02,
                                 aa.AttributeI03,
                                 aa.AttributeI04,
                                 aa.AttributeI05,
                                 aa.AttributeI06,
                                 aa.AttributeI07,
                                 aa.AttributeI08,
                                 aa.AttributeI09,
                                 aa.AttributeI10,
                                 aa.AttributeN01,
                                 aa.AttributeN02,
                                 aa.AttributeN03,
                                 aa.AttributeN04,
                                 aa.AttributeN05,
                                 aa.AttributeN06,
                                 aa.AttributeN07,
                                 aa.AttributeN08,
                                 aa.AttributeN09,
                                 aa.AttributeN10,
                                 aa.AttributeD01,
                                 aa.AttributeD02,
                                 aa.AttributeD03,
                                 aa.AttributeD04,
                                 aa.AttributeD05,
                                 aa.AttributeD06,
                                 aa.AttributeD07,
                                 aa.AttributeD08,
                                 aa.AttributeD09,
                                 aa.AttributeD10,
                                 aa.AttributeB01,
                                 aa.AttributeB02,
                                 aa.AttributeB03,
                                 aa.AttributeB04,
                                 aa.AttributeB05,
                                 aa.AttributeB06,
                                 aa.AttributeB07,
                                 aa.AttributeB08,
                                 aa.AttributeB09,
                                 aa.AttributeB10,
                                 aa.WFOrdinal,
                                 aa.WFStatus,
                                 aa.WFType,
                                 aa.WFComments,
                                 aa.WFRequesterId,
                                 aa.WFAnalystId,
                                 aa.WFManagerId,
                                 aa.WFCurrentOwnerId,
                                 aa.WFRequesterRoleId,
                                 aa.CreatedById,
                                 aa.UpdatedById,
                                 aa.CreatedDateTime,
                                 aa.UpdatedDateTime,
                                 LocalPobTypeName = bb.Name,
                                 aa.SspId
                             }).ToList().OrderByDescending(p => p.Version);
            return Ok(LLocalPOB);
        }
        //// GET: api/LLocalPOBs/5
        //[ResponseType(typeof(LLocalPob))]
        //public async Task<IHttpActionResult> GetLLocalPOBWithCurrentSSP(int id)
        //{

        //    var LLocalPOB = (from aa in db.LLocalPobs.Where(p => p.Id == id)
        //                    join yy in db.LLocalPobSsps on aa.Id equals yy.LocalPobId
        //                    where yy.EffectiveStartDate <= DateTime.Now && yy.EffectiveEndDate >= DateTime.Now
        //                    select new {
        //                    aa.Id,
        //                    aa.CompanyCode,
        //                    aa.ArticleNumber,
        //                    aa.Name,
        //                    aa.Description,
        //                    aa.PobIndicator,
        //                    aa.BundleIndicator,
        //                    aa.UsageIndicator,
        //                    aa.SpecialIndicator,
        //                    aa.IsHardwareType,
        //                    aa.AttributeC01,
        //                    aa.AttributeC02,
        //                    aa.AttributeC03,
        //                    aa.AttributeC04,
        //                    aa.AttributeC05,
        //                    aa.AttributeC06,
        //                    aa.AttributeC07,
        //                    aa.AttributeC08,
        //                    aa.AttributeC09,
        //                    aa.AttributeC10,
        //                    aa.AttributeN01,
        //                    aa.AttributeN02,
        //                    aa.AttributeN03,
        //                    aa.AttributeN04,
        //                    aa.AttributeN05,
        //                    aa.AttributeN06,
        //                    aa.AttributeN07,
        //                    aa.AttributeN08,
        //                    aa.AttributeN09,
        //                    aa.AttributeN10,
        //                    aa.WFOrdinal,
        //                    aa.WFStatus,
        //                    aa.WFType,
        //                    aa.WFComments,
        //                    aa.WFRequesterId,
        //                    aa.WFAnalystId,
        //                    aa.WFManagerId ,
        //                    aa.WFCurrentOwnerId,
        //                    aa.WFRequesterRoleId,
        //                    aa.CreatedById,
        //                    aa.UpdatedById,
        //                    aa.CreatedDateTime,
        //                    aa.UpdatedDateTime,
        //                    SSP = yy.Amount,
        //                    EffectiveStartDate = yy.EffectiveStartDate,
        //                    EffectiveEndDate = yy.EffectiveEndDate
        //    }).FirstOrDefault();
        //    if (LLocalPOB == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
        //    }
        //    return Ok(LLocalPOB);
        //}


        public IHttpActionResult GetCompletedListCount(string CompanyCode)
        {
            //var sqlQuery = "select Count(*) from LLocalPobs aa inner join ( select Name as nm, max(version) as ver from LLocalPobs where WFStatus in ('Completed', 'Parked') "
            //                    + " and CompanyCode = {0}  group by name) B on aa.Name = B.nm and aa.Version = B.ver "
            //                 + " left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id left outer join GGlobalPobs gp1 on aa.GlobalPobId1 = gp1.Id"
            //                + " left outer join GGlobalPobs gp2 on aa.GlobalPobId2 = gp2.Id"
            //                + " left outer join GCopaDimensions gc1 on aa.CopaId1 = gc1.Id"
            //                + " left outer join GCopaDimensions gc2 on aa.CopaId2 = gc2.Id"
            //              + " left outer join GCopaDimensions gc3 on aa.CopaId3 = gc3.Id left outer join GCopaDimensions gc4 on aa.CopaId4 = gc4.Id "
            //              + " left outer join LProductPobs pp on pp.LocalPobId = lp.Id";
            string sqlQuery = "select Count(*) from LLocalPobs aa inner join (select Name as nm, max(version) as ver from LLocalPobs where WFStatus in ('Completed', 'Parked')" +
                " and CompanyCode ={0} group by name) B on aa.Name = B.nm and aa.Version = B.ver  left outer join RLocalPobTypes lp on aa.LocalPobTypeId = lp.Id  " +
                "left outer join LProductPobs pp on pp.PobCatalogueId = aa.PobCatalogueId";
            int counts = db.Database.SqlQuery<int>(sqlQuery, CompanyCode).FirstOrDefault();
            return Ok(counts);
        }

        
        public IHttpActionResult GetCompletedList(string CompanyCode, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize,string UnderlyingProductId)
        {
            var Query = "Exec [SpGetLocalPobCompletedList] @CompanyCode,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@UnderlyingProductId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@UnderlyingProductId", string.IsNullOrEmpty(UnderlyingProductId) ? (object)System.DBNull.Value : UnderlyingProductId);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);

            //string query = "SELECT Id,Name,Version from LLocalPobs A inner join( select Name as nm, max(version) as ver from LLocalPobs where WFStatus in ('Completed','Parked' )and CompanyCode = {0}"
            //                     + " group by name) B on A.Name = B.nm and A.Version = B.ver ";
            //var LLocalPOB = db.Database.SqlQuery<LocalPobForDropDown>(query,CompanyCode).ToList();
            //return Ok(LLocalPOB);
        }

            

        // GET: api/LLocalPOBs/5
        [ResponseType(typeof(LLocalPob))]
        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var LLocalPOB = (from aa in db.LLocalPobs
                             join bb in db.RLocalPobTypes on aa.LocalPobTypeId equals bb.Id
                             where aa.CompanyCode == CompanyCode
                             select new { aa.Id,
                                 aa.PobCatalogueId,
                                 aa.CompanyCode,
                aa.Name,
                aa.Description,
               // aa.IsHardwareType,
                aa.AttributeC01,
                aa.AttributeC02,
                aa.AttributeC03,
                aa.AttributeC04,
                aa.AttributeC05,
                aa.AttributeC06,
                aa.AttributeC07,
                aa.AttributeC08,
                aa.AttributeC09,
                aa.AttributeC10,
                aa.AttributeN01,
                aa.AttributeN02,
                aa.AttributeN03,
                aa.AttributeN04,
                aa.AttributeN05,
                aa.AttributeN06,
                aa.AttributeN07,
                aa.AttributeN08,
                aa.AttributeN09,
                aa.AttributeN10,
                aa.WFOrdinal,
                aa.WFStatus,
                aa.WFType,
                aa.WFComments,
                aa.WFRequesterId,
                aa.WFAnalystId,
                aa.WFManagerId,
                aa.WFCurrentOwnerId,
                aa.WFRequesterRoleId,
                aa.CreatedById,
                aa.UpdatedById,
                aa.CreatedDateTime,
                aa.UpdatedDateTime,
                LocalPobTypeName = bb.Name,
                aa.SspId
            }).OrderByDescending(aa => aa.CreatedDateTime);
            if (LLocalPOB == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
            }
            return Ok(LLocalPOB);
        }

        [HttpPut]
        // PUT: api/LLocalPOBs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLLocalPOB(int id, LLocalPob LLocalPob, string UserName,string WorkFlow,string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,string ActionName,int? ProductId)
        {
            
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "POB")));
            //}
            if (!LLocalPOBExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
                }

                if (id != LLocalPob.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "POB")));
                }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var xx = db.LLocalPobs.Find(id);
                    if (xx != null)
                    {
                        int PobCatalogueId = xx.PobCatalogueId;
                        var PobList = db.LLocalPobs.Where(aa => aa.PobCatalogueId == PobCatalogueId).Where(aa => aa.CompanyCode == xx.CompanyCode).ToList();

                        foreach (var pob in PobList)
                        {//updating Name & PC with new Name & PC (of all the existing versions) 
                            pob.Name = LLocalPob.Name;
                            db.Entry(pob).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            db.Entry(pob).State = EntityState.Detached;
                            await db.SaveChangesAsync();
                        }
                    }

                    //Update EffecticeEndDate of previous version
                    var PreviousVersionEndDate = LLocalPob.EffectiveStartDate.AddDays(-1);
                    string qry = "update LLocalPobs set EffectiveEndDate= {0} " +
                        " where PobCatalogueId={1} and Version={2}";
                    db.Database.ExecuteSqlCommand(qry, PreviousVersionEndDate, LLocalPob.PobCatalogueId,LLocalPob.Version-1);



                    LLocalPob.SspId = xx.SspId;
                    db.Entry(LLocalPob).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == LLocalPob.CompanyCode).FirstOrDefault();
                    if (ActionName.Equals("Edit"))
                    {

                        var Action = db.WActions.Where(p => p.Name == "Edit").FirstOrDefault();

                        var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", LLocalPob.WFOrdinal, LLocalPob.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                        string CompanyCode = LLocalPob.CompanyCode;
                        //when LPOB is created on fly, calculate its step
                        if (ProductId != null && ProductId != -9999) //-9999 is self manufactured as null causes issue while HTTP call
                        {
                            var product = db.LProducts.Where(a => a.Id == ProductId).FirstOrDefault();
                            if (product.WFStatus.Equals("Parked"))// use step of request
                            {
                                //get stepId of the request
                                int reqId = (int)product.RequestId;
                                var request = db.LRequests.Where(a => a.Id == reqId).FirstOrDefault();
                                int reqOrdinal = (int)request.WFOrdinal;
                                int wfId = db.RWorkFlows.Where(a => a.WFType == request.WFType).FirstOrDefault().Id;
                                StepId = db.WSteps.Where(p => p.WorkFlowId == wfId).Where(p => p.CompanyCode == CompanyCode).Where(p => p.Ordinal == reqOrdinal).FirstOrDefault().Id;
                            }
                            else//use step of product
                            {
                                //get stepId of the request 
                                int wfId = db.RWorkFlows.Where(a => a.WFType == product.WFType).FirstOrDefault().Id;
                                StepId = db.WSteps.Where(p => p.WorkFlowId == wfId).Where(p => p.CompanyCode == CompanyCode).Where(p => p.Ordinal == product.WFOrdinal).FirstOrDefault().Id;
                            }
                        }

                        db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                               "Update", LLocalPob.UpdatedById, LLocalPob.WFRequesterRoleId, DateTime.UtcNow, LLocalPob.WFStatus, LLocalPob.WFStatus,
                               "LLocalPobs", LLocalPob.Id, String.IsNullOrEmpty(LLocalPob.Name) ? "blank" : LLocalPob.Name, WorkflowDetails.Id, LLocalPob.CompanyCode, LLocalPob.WFComments, StepId, Action.Label, null);
                        db.SaveChanges();



                        //add supporting documents
                        if (!string.IsNullOrEmpty(FileList))
                        {
                            var OriginalFileArray = OriginalFileNameList.Split(',').ToList();
                            var FileArray = FileList.Split(',').ToList();
                            List<string> DescriptionArray = null;
                            if (!string.IsNullOrEmpty(SupportingDocumentsDescription))
                            {
                                DescriptionArray = SupportingDocumentsDescription.Split(',').ToList();
                            }
                            for (var i = 0; i < FileArray.Count(); i++)
                            {
                                //Move File Over S3
                                var Source = FilePath + "/" + FileArray[i];
                                var Destination = "/" + LLocalPob.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                                var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                                var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                                if (sucess)
                                    FilePath = Destination;

                                var SupportingDocument = new LSupportingDocument
                                {
                                    StepId = StepId,
                                    FileName = FileArray[i],
                                    OriginalFileName = OriginalFileArray[i],
                                    FilePath = FilePath,
                                    EntityId = LLocalPob.Id,
                                    EntityType = "LLocalPobs",
                                    CreatedById = LLocalPob.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                    UpdatedById = LLocalPob.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                    CreatedByRoleId = LLocalPob.WFRequesterRoleId.Value,
                                    CreatedDateTime = DateTime.UtcNow,
                                    UpdatedDateTime = DateTime.UtcNow,
                                    //Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                                    Description = (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                                };
                                db.LSupportingDocuments.Add(SupportingDocument);
                                db.SaveChanges();
                                /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                                 * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                                 * NewStatus should be same as current status of the entry, comments = 
                                 * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                                = LSupportingDocument.Id, rest of the columns are obvious.*/
                                db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                               "Create", LLocalPob.UpdatedById, LLocalPob.WFRequesterRoleId, DateTime.UtcNow, LLocalPob.WFStatus, LLocalPob.WFStatus,
                               "LLocalPobs", LLocalPob.Id, String.IsNullOrEmpty(LLocalPob.Name) ? "blank" : LLocalPob.Name, WorkflowDetails.Id, LLocalPob.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                                db.SaveChanges();
                            }
                        }
                    }
                    else if (ActionName.Equals("Change"))
                    {
                        //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                        string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
                        db.SPUpdateActionStatus(ActionName, WorkflowDetails.Name, id.ToString(), LLocalPob.CompanyCode, LLocalPob.CreatedById.ToString(), LLocalPob.WFComments, LLocalPob.WFRequesterRoleId.ToString(), ProjectEnviournment, string.Empty);
                    }

                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            return Ok(LLocalPob);
        }

        [HttpPost]
        //[HttpGet]
        // POST: api/LLocalPOBs
        [ResponseType(typeof(LLocalPob))]
        public async Task<IHttpActionResult> PostLLocalPOB(LLocalPob LLocalPOB, string UserName,string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, int? ProductId, string PobStDt, string PobEnDt)
        {
            //-9999 means null.
            if(ProductId == -9999)
            {
                ProductId = null;
            }
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "POB")));
            //}
            //LLocalPob LLocalPOB = new LLocalPob();
            //LLocalPOB.CompanyCode = "DE"; LLocalPOB.CreatedById = 1; LLocalPOB.UpdatedById = 1; LLocalPOB.CreatedDateTime = DateTime.Now;LLocalPOB.UpdatedDateTime = DateTime.Now;
            //LLocalPOB.Version = 1; LLocalPOB.Status = "Initial"; LLocalPOB.LocalPobTypeId = 1;LLocalPOB.WFOrdinal = 1;LLocalPOB.WFRequesterId=1;LLocalPOB.WFRequesterRoleId = 1;
            //LLocalPOB.Name = "LP21"; LLocalPOB.WFStatus = "Saved"; LLocalPOB.WFType = "LLocalPobs";LLocalPOB.WFCurrentOwnerId = 1;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //SG Commenting Sequence code, no more required. SSPDimension table is created
                    ////getting sequence value
                    //var Sequence_qry = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_SspId");
                    //var Task = Sequence_qry.SingleAsync();
                    //int sequenceValue = Task.Result;
                    //LLocalPOB.SspId = sequenceValue;

                    db.LLocalPobs.Add(LLocalPOB);
                    db.SaveChanges();

                    //Set End date for LocalPob of version=1, for greater than 1 version, there is trigger(TrCalculateEndDate)
                   // LLocalPOB.AttributeD10 = DateTime.ParseExact("31/12/3500 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    //CR1.5 - SourceLocalPobId is removed
                    ////after LOcalPob is created successfully, update its SourceLocalPobId with the Id value.
                    //LLocalPOB.SourceLocalPobId = LLocalPOB.Id;
                    //db.Entry(LLocalPOB).State = EntityState.Modified;
                    //db.SaveChanges();

                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == LLocalPOB.CompanyCode).FirstOrDefault();
                    var Action = db.WActions.Where(p => p.Name == "Create").FirstOrDefault();
                    /*independent lpob -> stepname of LPOB
                    attached with Product-> check status of Product -> if parked, use step of request
                    if not parked, use step of product*/
                    int? StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1}" +
                        " and WorkFlowId={2}", LLocalPOB.WFOrdinal, LLocalPOB.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    string CompanyCode = LLocalPOB.CompanyCode;
                    //when LPOB is created on fly, calculate its step
                    if (ProductId != null)
                    {
                        var product = db.LProducts.Where(a => a.Id == ProductId).FirstOrDefault();
                        if(product.WFStatus.Equals("Parked"))// use step of request
                        {
                            //get stepId of the request
                            int reqId = (int)product.RequestId;
                            var request =  db.LRequests.Where(a => a.Id == reqId).FirstOrDefault() ;
                            int reqOrdinal = (int)request.WFOrdinal;
                            int wfId= db.RWorkFlows.Where(a=>a.WFType== request.WFType).FirstOrDefault().Id;
                            StepId = db.WSteps.Where(p => p.WorkFlowId == wfId).Where(p => p.CompanyCode == CompanyCode).Where(p => p.Ordinal == reqOrdinal).FirstOrDefault().Id;
                        }
                        else//use step of product
                        {
                            //get stepId of the request 
                            int wfId = db.RWorkFlows.Where(a => a.WFType == product.WFType).FirstOrDefault().Id;
                            StepId = db.WSteps.Where(p => p.WorkFlowId == wfId).Where(p => p.CompanyCode == CompanyCode).Where(p => p.Ordinal == product.WFOrdinal).FirstOrDefault().Id;
                        }
                    }
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Create",
                           "Create", LLocalPOB.UpdatedById, LLocalPOB.WFRequesterRoleId, DateTime.UtcNow, LLocalPOB.WFStatus, LLocalPOB.WFStatus,
                           "LLocalPobs", LLocalPOB.Id, String.IsNullOrEmpty(LLocalPOB.Name) ? "blank" : LLocalPOB.Name, WorkflowDetails.Id, LLocalPOB.CompanyCode, LLocalPOB.WFComments, StepId, Action.Label, null);
                    db.SaveChanges();

                    //add supporting documents
                    if (!string.IsNullOrEmpty(FileList))
                    {
                        var OriginalFileArray = OriginalFileNameList.Split(',').ToList();
                        var FileArray = FileList.Split(',').ToList();
                        List<string> DescriptionArray = null;
                        if (!string.IsNullOrEmpty(SupportingDocumentsDescription))
                        {
                             DescriptionArray = SupportingDocumentsDescription.Split(',').ToList();
                        }
                        for (var i = 0; i < FileArray.Count(); i++)
                        {
                            //Move File Over S3
                            var Source = FilePath + "/" + FileArray[i];
                            var Destination = "/" + LLocalPOB.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                            var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                            var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                            if (sucess)
                                FilePath = Destination;

                            var SupportingDocument = new LSupportingDocument
                            {
                                StepId = StepId,
                                FileName = FileArray[i],
                                OriginalFileName = OriginalFileArray[i],
                                FilePath = FilePath,
                                EntityId = LLocalPOB.Id,
                                EntityType = "LLocalPobs",
                                CreatedById = LLocalPOB.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = LLocalPOB.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                CreatedByRoleId = LLocalPOB.WFRequesterRoleId.Value,
                                CreatedDateTime = DateTime.UtcNow,
                                UpdatedDateTime = DateTime.UtcNow,
                                //Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription)? DescriptionArray[i] : null)
                                Description = (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                            };
                            db.LSupportingDocuments.Add(SupportingDocument);
                            db.SaveChanges();
                            /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                             * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                             * NewStatus should be same as current status of the entry, comments = 
                             * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                            = LSupportingDocument.Id, rest of the columns are obvious.*/
                            db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                           "Create", LLocalPOB.UpdatedById, LLocalPOB.WFRequesterRoleId, DateTime.UtcNow, LLocalPOB.WFStatus, LLocalPOB.WFStatus,
                           "LLocalPobs", LLocalPOB.Id, String.IsNullOrEmpty(LLocalPOB.Name) ? "blank" : LLocalPOB.Name, WorkflowDetails.Id, LLocalPOB.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }


                    MEntityPortfolio MEP = new MEntityPortfolio();
                    MEP.EntityId = LLocalPOB.Id;
                    MEP.EntityType = "LLocalPobs";
                    int PortfolioId = db.LPortfolios.Where(m=>m.CompanyCode == LLocalPOB.CompanyCode).FirstOrDefault().Id;
                    MEP.PortfolioId = PortfolioId;
                    db.MEntityPortfolios.Add(MEP);
                    db.SaveChanges();
                    //Make entry in ProductPob as the Pob is created on Fly
                    if(ProductId!= null)
                    {
                        LProductPob ProductPob = new LProductPob
                        {
                            ProductId = Convert.ToInt32(ProductId)/*,LocalPobId = LLocalPOB.Id */,
                            PobCatalogueId = LLocalPOB.PobCatalogueId,
                            EffectiveStartDate = DateTime.ParseExact((string.IsNullOrEmpty(PobStDt)?"01/01/2010" : PobStDt) + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            EffectiveEndDate = DateTime.ParseExact((string.IsNullOrEmpty(PobEnDt) ? "31/12/2099" : PobEnDt) + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        };
                    
                        db.LProductPobs.Add(ProductPob);
                        db.SaveChanges();
                    }

                   
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
               
                transaction.Commit();
            }

            // return CreatedAtRoute("DefaultApi", new { id = LLocalPOB.Id }, LLocalPOB);
            return Ok(LLocalPOB.Id);
        }

        public IHttpActionResult GetMaxPobCatelogueId(string CompanyCode)
        {
            string qry = "select IsNull(max(PobCatalogueId) +1,1) from LLocalPobs where CompanyCode = {0} ";
            int PobCatalogueId = db.Database.SqlQuery<int>(qry, CompanyCode).FirstOrDefault();
            return Ok(PobCatalogueId);
        }
        // DELETE: api/LLocalPOBs/5
        [ResponseType(typeof(LLocalPob))]
        public async Task<IHttpActionResult> DeleteLLocalPOB(int id)
        {
            LLocalPob LLocalPOB = await db.LLocalPobs.FindAsync(id);
            if (LLocalPOB == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
            }
            
                try
                {
                    db.LLocalPobs.Remove(LLocalPOB);
                    await db.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry. 
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;
                    }
                }
            return Ok(LLocalPOB);
        }

        private bool LLocalPOBExists(int id)
        {
            return db.LLocalPobs.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LLocalPobs_LProductPobs_LocalPobId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCTS POB", "LOCALPOB"));
            else if (SqEx.Message.IndexOf("UQ_LLocalPob_CompanyCode_LocalPobTypeId_Name_Version", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "LOCALPOB "));
            else if (SqEx.Message.IndexOf("UQ_MLocalGlobalUsecaseMapping_GPobId_PobCatalogueId_CompanyCode", StringComparison.OrdinalIgnoreCase) >=0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "GLobal POB"));
            else if (SqEx.Message.IndexOf("UQ_MPobCopaMapping_CopaId_PobCatalogueId_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "Copa Dimension"));
            else
            {
                //Something else failed return original error message as retrieved from database
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                    ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3rd index position
                                                                        //db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                                                                        //return Globals.SomethingElseFailedInDBErrorMessage;

                ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result).FirstOrDefault();
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }

    }
}
