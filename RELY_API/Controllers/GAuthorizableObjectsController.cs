
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
    public class GAuthorizableObjectsController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();
        // GET: api/GAuthorizableObjects
        public IHttpActionResult GetGAuthorizableObjects(string UserName, string WorkFlow)
        {          
            var xx = (from x in db.GAuthorizableObjects
                      select new { x.Id, x.ControllerName, x.MethodName, x.Description }).OrderBy(p => p.ControllerName);
            return Ok(xx);
            
        }


        // GET: api/GAuthorizableObjects/5
        [ResponseType(typeof(GAuthorizableObject))]
        public async Task<IHttpActionResult> GetGAuthorizableObject(Nullable<int> id, string UserName, string WorkFlow)
        {
            var GAuthorizableObject = db.GAuthorizableObjects.Where(p => p.Id == id).Select(x => new { x.Id, x.ControllerName, x.MethodName, x.Description }).FirstOrDefault();
            if (GAuthorizableObject == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            return Ok(GAuthorizableObject);
        }

        // PUT: api/GAuthorizableObjects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGAuthorizableObject(int id, GAuthorizableObject GAuthorizableObject, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }
                if (!GAuthorizableObjectExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
                }

                if (id != GAuthorizableObject.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
                }
                try
                {
                    db.Entry(GAuthorizableObject).State = EntityState.Modified;
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
            return Ok(GAuthorizableObject);
        }

        // POST: api/GAuthorizableObjects
        [ResponseType(typeof(GAuthorizableObject))]
        public async Task<IHttpActionResult> PostGAuthorizableObject(string[] DataArray)
        {
          // string [][] DataArray = new string[][] { new string []{"1", "Dummy Test", "Dummy Method", "Description"} };
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "AuthorizableObject")));
            }
            try
            {
                foreach (string Data in DataArray)
                {
                    var data = Data;
                    var GridArray = data.Split(',');
                    var model = new GAuthorizableObject
                    {
                        Id = Convert.ToInt32(GridArray[0]),
                        ControllerName = GridArray[1],
                        MethodName = GridArray[2],
                        Description = GridArray[3]

                    };
                    if (db.GAuthorizableObjects.Where(p => p.Id == model.Id).Count() > 0)
                    {
                        db.Entry(model).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        model.Id = 0;//To override the Id generated by grid
                        db.GAuthorizableObjects.Add(model);
                        await db.SaveChangesAsync();
                    }
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

            // return CreatedAtRoute("DefaultApi", new { id = GAuthorizableObject.Id }, GAuthorizableObject);
            return Ok();
        }

        // DELETE: api/GAuthorizableObjects/5
        [ResponseType(typeof(GAuthorizableObject))]
        public async Task<IHttpActionResult> DeleteGAuthorizableObject(int id, string UserName, string WorkFlow)
        {
            GAuthorizableObject GAuthorizableObject = await db.GAuthorizableObjects.FindAsync(id);
            if (GAuthorizableObject == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
                try
                {
                    db.GAuthorizableObjects.Remove(GAuthorizableObject);
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
                        throw ex;
                    }
                }
                
            return Ok(GAuthorizableObject);
        }

        private bool GAuthorizableObjectExists(int id)
        {
            return db.GAuthorizableObjects.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message 
            if (SqEx.Message.IndexOf("FK_GAuthorizableObjects_MAuthorizableObjectsRoles_AuthorizableObjectId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "AUTHORIZABLE OBJECTS", "PRODUCT OBLIGATIONS"));
            else if (SqEx.Message.IndexOf("UQ_GAuthorizableObjects_ControllerName_MethodName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "AUTHORIZABLE OBJECTS"));
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
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New",Result).FirstOrDefault();
                int errorid = (int)Result.Value; //getting value of output parameter                
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }
    }
}
