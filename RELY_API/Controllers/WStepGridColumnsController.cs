using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class WStepGridColumnGridColumnsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
       
        // GET: api/WStepGridColumnGridColumns/5
        [ResponseType(typeof(WStepGridColumn))]
        public async Task<IHttpActionResult> GetById(int id, string UserName, string WorkFlow)
        {

            var WStepGridColumn = db.WStepGridColumns.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.Ordinal,
                aa.WStepId,
                aa.ColumnName,aa.Label,aa.OrderByOrdinal,aa.ShouldBeVisible,aa.AscDesc,aa.FunctionName,aa.JoinTable,aa.JoinTableColumn,aa.BaseTableJoinColumn
            }).FirstOrDefault();
            if (WStepGridColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "GRID COLUMN")));
            }
            return Ok(WStepGridColumn);
        }
        [ResponseType(typeof(WStepGridColumn))]
        public async Task<IHttpActionResult> GetByStepId(int StepId, string UserName, string WorkFlow)
        {

            var WStepGridColumn = db.WStepGridColumns.Where(p => p.WStepId == StepId).Select(aa => new {
                aa.Id,
                aa.Ordinal,
                aa.WStepId,
                aa.ColumnName,
                aa.Label,
                aa.OrderByOrdinal,
                aa.ShouldBeVisible,
                aa.AscDesc,
                aa.FunctionName,
                aa.JoinTable,
                aa.JoinTableColumn,
                aa.BaseTableJoinColumn
            }).ToList();
            if (WStepGridColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "GRID COLUMN")));
            }
            return Ok(WStepGridColumn);
        }

        
        // PUT: api/WStepGridColumnGridColumns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWStepGridColumn(int id, WStepGridColumn WStepGridColumn, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            if (!WStepGridColumnExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }

            if (id != WStepGridColumn.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            try
            {
                db.Entry(WStepGridColumn).State = EntityState.Modified;
                await db.SaveChangesAsync();
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

            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(WStepGridColumn);
        }

        // POST: api/WStepGridColumnGridColumns
        [ResponseType(typeof(WStepGridColumn))]
        public async Task<IHttpActionResult> PostWStepGridColumn(WStepGridColumn model, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "STEP")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                if (db.WStepGridColumns.Where(p => p.Id == model.Id).Where(p => p.WStepId == model.WStepId).Count() > 0)
                {
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    model.Id = 0;//To override the Id generated by grid
                    db.WStepGridColumns.Add(model);
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

            //return CreatedAtRoute("DefaultApi", new { id = model.Id }, model);
            return Ok(model.Id);
        }

        // DELETE: api/WStepGridColumnGridColumns/5
        [ResponseType(typeof(WStepGridColumn))]
        public async Task<IHttpActionResult> DeleteById(int id, string UserName, string WorkFlow)
        {
            WStepGridColumn WStepGridColumn = await db.WStepGridColumns.FindAsync(id);
            if (WStepGridColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            try
            {
                db.WStepGridColumns.Remove(WStepGridColumn);
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
            return Ok(WStepGridColumn);
        }

        private bool WStepGridColumnExists(int id)
        {
            return db.WStepGridColumns.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message FK_WStepGridColumnGridColumns_MStepRoles_StepId
            if (SqEx.Message.IndexOf("FK_WSteps_WStepGridColumns_WStepId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "STEPS", "GRID COLUMNS"));
            else if (SqEx.Message.IndexOf("UQ_WStepId_Ordinal", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "GRID COLUMNS"));
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

