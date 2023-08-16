using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GGlobalPOBController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        //GetAllGGlobalPOB
        public IHttpActionResult GetAllGGlobalPOB(string CompanyCode)
        {
            var list1 = (from aa in db.GGlobalPobs
                         select new
                         {
                             aa.Id,
                             aa.Type,
                             aa.Name,
                             aa.Description,
                             aa.Category,
                             aa.IFRS15Account,
                             aa.CreatedById,
                             aa.CreatedDateTime,
                             aa.UpdatedById,aa.CompanyCode,
                             aa.UpdatedDateTime,
                             Label = aa.Name + "(" + aa.Type + ", " + aa.Category + ", " + aa.IFRS15Account + ")"  //Name(Type, Category, Account)
                         }).Where(aa=>aa.CompanyCode == CompanyCode).OrderBy(p => p.Id).ToList();
            return Ok(list1);
        }
            public IHttpActionResult GetGGlobalPOBforDropDown(string CompanyCode)
        {
            var GlobalPob = (from aa in db.GGlobalPobs
                         select new
                         {
                             aa.Id,
                             aa.CompanyCode,
                             aa.Name,
                             aa.Type,
                             aa.Category,
                             aa.IFRS15Account,
                             Label = aa.Name + "(" + aa.Type + ", " + aa.Category + ", " + aa.IFRS15Account + ")"  //Name(Type, Category, Account)
                         }).Where(aa=>aa.CompanyCode == CompanyCode).OrderBy(P => P.Name).ToList();
    
            return Ok(GlobalPob);
        }

        public IHttpActionResult GetGGlobalPOB(int PobCatalogueId,string GpobType)
        {
            //var list1 = (from aa in db.GGlobalPobs
            //          select new
            //          {
            //              aa.Id,
            //              aa.Type,
            //              aa.Name,
            //              aa.Description,
            //              aa.Category,
            //              aa.IFRS15Account,
            //              aa.CreatedById,
            //              aa.CreatedDateTime,
            //              aa.UpdatedById,
            //              aa.UpdatedDateTime,
            //              Label =   aa.Name + "(" +aa.Type + ", " + aa.Category + ", " + aa.IFRS15Account + ")"  //Name(Type, Category, Account)
            //          }).OrderBy(p => p.Id).ToList();
            if (GpobType.Equals("OTHERS"))
            {
                var results1 = (from aa in db.GGlobalPobs
                                where !(from t2 in db.MLocalGlobalUsecaseMappings
                                        where t2.PobCatalogueId == PobCatalogueId
                                        select t2.GPobId).Contains(aa.Id)
                                        && !(aa.Type == "RETENTION" || aa.Type == "ACQUISIT")
                                select new
                                {
                                    aa.Id,
                                    aa.Type,
                                    aa.Name,
                                    aa.CompanyCode,
                                    aa.Description,
                                    aa.Category,
                                    aa.IFRS15Account,
                                    aa.CreatedById,
                                    aa.CreatedDateTime,
                                    aa.UpdatedById,
                                    aa.UpdatedDateTime,
                                    Label = aa.Name + "(" + aa.Type + ", " + aa.Category + ", " + aa.IFRS15Account + ")"  //Name(Type, Category, Account)
                                }).OrderBy(p => p.Id).ToList();
                return Ok(results1);
            }
            else
            {
                var results = (from aa in db.GGlobalPobs
                               where !(from t2 in db.MLocalGlobalUsecaseMappings
                                       where t2.PobCatalogueId == PobCatalogueId
                                       select t2.GPobId).Contains(aa.Id)
                                       && aa.Type == GpobType
                               select new
                               {
                                   aa.Id,
                                   aa.Type,
                                   aa.Name,
                                   aa.CompanyCode,
                                   aa.Description,
                                   aa.Category,
                                   aa.IFRS15Account,
                                   aa.CreatedById,
                                   aa.CreatedDateTime,
                                   aa.UpdatedById,
                                   aa.UpdatedDateTime,
                                   Label = aa.Name + "(" + aa.Type + ", " + aa.Category + ", " + aa.IFRS15Account + ")"  //Name(Type, Category, Account)
                               }).OrderBy(p => p.Id).ToList();
                //var list2 = new[]{
                //    new {
                //        Id = 0, Type = "",Name="", Description ="", Category ="", IFRS15Account ="", CreatedById =0,  CreatedDateTime=DateTime.Now,UpdatedById =0,
                //        UpdatedDateTime=DateTime.Now
                //        ,Label="Please Choose"
                //    } };
                //var FinalList = list2.Union(list1);
                return Ok(results);
            }
        }

        [HttpPost]
        [ResponseType(typeof(GGlobalPob))]
        public async Task<IHttpActionResult> PostGGlobalPob(List<GGlobalPob> GGlobalPob, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "GGlobalPOBs")));
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var model in GGlobalPob)
                    {
                        if (db.GGlobalPobs.Where(p => p.Name == model.Name).Count() > 0)
                        {
                            //as we are not getting Id value from uploaded excel, need to explicitly setting Id Column.
                            int Id = db.GGlobalPobs.Where(p => p.Name == model.Name).Where(p=>p.CompanyCode == model.CompanyCode).Select(a => a.Id).FirstOrDefault();
                            model.Id = Id;
                            db.Entry(model).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            model.Id = 0;
                            db.GGlobalPobs.Add(model);
                            await db.SaveChangesAsync();
                        }
                            //if (model.Id == 0)//add only when new record is there to insert.
                            //db.GGlobalPobs.Add(model);
                       // LoggedInUserId = model.UpdatedById;
                       // await db.SaveChangesAsync();
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

            return Ok();
        }



        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_GGlobalPOBs_MMLProductsLPobGPob_GPobId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "GLOBAL POBS", "PRODUCT OBLIGATIONS"));
            if (SqEx.Message.IndexOf("UQ_GGlobalPOBs_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "GLOBAL POBS"));
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
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
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
