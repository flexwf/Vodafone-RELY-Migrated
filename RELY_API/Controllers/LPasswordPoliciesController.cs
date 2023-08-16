using System;
using System.Web.Http;
using RELY_API.Models;
using System.Linq;
using System.Data.SqlClient;
using RELY_API.Utilities;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Net;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    //[RoutePrefix("api/Account")]
    [CustomExceptionFilter]
    public class LPasswordPoliciesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        public IHttpActionResult GetLPasswordPolicies(string CompanyCode, int UserId,string UserName, string WorkFlow)
        {
            
            var xx = (from aa in db.LPasswordPolicies.Where(p => p.CompanyCode == CompanyCode)
                      select new PasswordPolicyViewModel
                      {
                          Id = aa.Id,
                          MaxAgeDays = aa.MaxAgeDays,
                          MinLength = aa.MinLength,
                          MinLowerCase = aa.MinLowerCase,
                          MinNumbers = aa.MinNumbers,
                          MinSpecialChars = aa.MinSpecialChars,
                          MinUpperCase = aa.MinUpperCase,
                          ReminderDays = aa.ReminderDays,
                          MinAgeDays = aa.MinAgeDays,
                          PreventReuse = aa.PreventReuse,
                          LockoutFailedAttempts = aa.LockoutFailedAttempts,
                          LockoutMinutes = aa.LockoutMinutes,
                          DaysToExpirePassword = aa.MaxAgeDays
                      }).FirstOrDefault();
            if (xx == null)
            {

                return Ok(xx);
            }
            //else
            //{
                string LoginEmail = db.LUsers.Find(UserId).LoginEmail;
                if (db.LPasswordHistories.Where(p => p.LoginEmail == LoginEmail).Count() > 0)
                {
                    var LastCreatedDate = db.LPasswordHistories.Where(p => p.LoginEmail == LoginEmail).Max(p => p.CreatedDateTime);
                    xx.DaysToExpirePassword = Convert.ToInt32((LastCreatedDate.AddDays(xx.MaxAgeDays) - DateTime.UtcNow).TotalDays);

                }
                return Ok(xx);
           // }
           
        }

       
        public async Task<IHttpActionResult> SaveData(LPasswordPolicy model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Password Policy")));
            }

            try
            {
                db.LPasswordPolicies.Add(model);
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
            return CreatedAtRoute("DefaultApi", new { id = model.Id }, model);
        }


        public IHttpActionResult GetPasswordPolicyById(int id , string CompanyCode)
        {

            var xx = (from aa in db.LPasswordPolicies.Where(p => p.CompanyCode == CompanyCode && p.Id == id)
                      select new PasswordPolicyViewModel
                      {
                          Id = aa.Id,
                          CompanyCode = aa.CompanyCode,
                          MaxAgeDays = aa.MaxAgeDays,
                          MinLength = aa.MinLength,
                          MinLowerCase = aa.MinLowerCase,
                          MinNumbers = aa.MinNumbers,
                          MinSpecialChars = aa.MinSpecialChars,
                          MinUpperCase = aa.MinUpperCase,
                          ReminderDays = aa.ReminderDays,
                          MinAgeDays = aa.MinAgeDays,
                          PreventReuse = aa.PreventReuse,
                          LockoutFailedAttempts = aa.LockoutFailedAttempts,
                          LockoutMinutes = aa.LockoutMinutes,
                          DaysToExpirePassword = aa.MaxAgeDays
                      }).FirstOrDefault();
           
            return Ok(xx);
        }


        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPasswordPolicy(int id, LPasswordPolicy PasswordPolicy )
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PasswordPolicy")));
            }
            if (!LPasswordPolicyExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PasswordPolicy")));
            }

            if (id != PasswordPolicy.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PasswordPolicy")));
            }
            try
            {
                db.Entry(PasswordPolicy).State = EntityState.Modified;
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
            return Ok(PasswordPolicy);
        }

        [ResponseType(typeof(LPasswordPolicy))]
        public async Task<IHttpActionResult> DeletePasswordPolicy(int id)
        {
            LPasswordPolicy passwordPolicy = await db.LPasswordPolicies.FindAsync(id);
            if (passwordPolicy == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Password Policy")));
            }
            try
            {
                db.LPasswordPolicies.Remove(passwordPolicy);
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
            return Ok(passwordPolicy);
        }

        private bool LPasswordPolicyExists(int id)
        {
            return db.LPasswordPolicies.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_LPasswordPolicies_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "PASSWORD POLICIES"));
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
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result);
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }

    }
}
