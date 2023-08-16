using RELY_API.Models;
using RELY_API.Utilities;
using System;
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
    public class GCopaDimensionsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public IHttpActionResult GetGCopaDimensions()
        {
            var xx = (from aa in db.GCopaDimensions
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Class,
                          aa.CopaValue,
                          aa.Description,
                          aa.Dimension,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.DimentionClassDescription
                      }).OrderBy(p => p.Class);
            return Ok(xx);
        }

        public IHttpActionResult GetGCopaDimensionsforDropDown(int Class, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.GCopaDimensions.Where(p => p.Class == Class)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.CopaValue,
                          aa.Description,
                          Label = aa.CopaValue + " " + aa.Description + " (" + aa.Dimension + ")" //CopaValue Description (Dimension)
                      }).OrderBy(p => p.CopaValue);
            return Ok(xx);
        }

        public IHttpActionResult GetGCopaDimensionsByClass(int Class,int PobCatalogueId,string UserName,string WorkFlow)
        {
            //var xx = (from aa in db.GCopaDimensions.Where(p=>p.Class==Class)
            //          select new
            //          {
            //              aa.Id,
            //              aa.Class,
            //              aa.CopaValue,
            //              aa.Description,
            //              aa.Dimension,
            //              aa.CreatedById,
            //              aa.CreatedDateTime,
            //              aa.UpdatedById,
            //              aa.UpdatedDateTime,
            //              aa.DimentionClassDescription,
            //              Label = aa.CopaValue + " "  +aa.Description + " (" + aa.Dimension + ")" //CopaValue Description (Dimension)
            //          }).OrderBy(p => p.Class);

            var Copalist = (from aa in db.GCopaDimensions.Where(aa => aa.Class == Class)
                            where !(from t2 in db.MPobCopaMappings
                            where t2.PobCatalogueId == PobCatalogueId
                            select t2.CopaId).Contains(aa.Id)
                            select new
                            {  
                                aa.Id,
                                aa.Class,
                                aa.CompanyCode,
                                aa.CopaValue,
                                aa.Description,
                                aa.Dimension,
                                aa.CreatedById,
                                aa.CreatedDateTime,
                                aa.UpdatedById,
                                aa.UpdatedDateTime,
                                aa.DimentionClassDescription,
                                Label = aa.CopaValue + " " + aa.Description + " (" + aa.Dimension + ")" //CopaValue Description (Dimension)
                            }).OrderBy(p => p.Class); ;
            return Ok(Copalist);
        }

       [HttpPost]
        [ResponseType(typeof(GCopaDimension))]
        public async Task<IHttpActionResult> PostGCopaDimension(GCopaDimension GCopaDimension, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "GCopaDimensions")));
            }
            try
            {//UQ is updated to Dimension so search is also updated to DImension
                if (db.GCopaDimensions.Where(p => p.Dimension == GCopaDimension.Dimension).Where(p => p.CompanyCode == GCopaDimension.CompanyCode).Count() > 0)
                {
                    //as we are not getting Id value from uploaded excel, need to explicitly setting Id Column.
                    int Id = db.GCopaDimensions.Where(p => p.Dimension == GCopaDimension.Dimension).Where(p => p.CompanyCode == GCopaDimension.CompanyCode).Select(a => a.Id).FirstOrDefault();
                    GCopaDimension.Id = Id;
                    db.Entry(GCopaDimension).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.GCopaDimensions.Add(GCopaDimension);
                    await db.SaveChangesAsync();
                }
            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
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
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
            return Ok(GCopaDimension.Id);


        }

        //[ResponseType(typeof(GCopaDimension))]
        //public IHttpActionResult GetById(int Id)
        //{
        //    var GCopaDimension = db.GCopaDimensions.Where(p => p.Id == Id).Select(aa => new {
        //        aa.Id,
        //        aa.Class,
        //        aa.CopaValue,
        //        aa.Description,
        //        aa.Dimension,
        //        aa.CreatedById,
        //        aa.CreatedDateTime,
        //        aa.UpdatedById,
        //        aa.UpdatedDateTime
        //    }).OrderBy(aa => aa.Id);
        //    if (GCopaDimension == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Dimension")));
        //    }
        //    return Ok(GCopaDimension);
        //}
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_GCopaDimensions_MMMLProductsLPobGPobCopa_CopaDimensionId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COPA DIMENSIONS", "PRODUCT OBLIGATIONS"));
            if (SqEx.Message.IndexOf("UQ_GCopaDimensions_Class_CopaValue", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "COPA DIMENSIONS"));
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
