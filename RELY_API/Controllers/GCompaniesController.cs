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
using System.Data.Entity.Core.Objects;
using System.Data;
using static RELY_API.Utilities.Globals;
using System.Configuration;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GCompaniesController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/GCompanies
        public IHttpActionResult GetGCompanies()
        {
            var xx = (from aa in db.GCompanies
                      select new { aa.Id, aa.CompanyCode, aa.CompanyName,aa.LogoPath,aa.PunchLine}).OrderBy(p => p.CompanyName);
            return Ok(xx);
        }


        // GET: api/GCompanies/5
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> GetGCompany(string CompanyCode)
        {
            var GCompany = db.GCompanies.Where(p => p.CompanyCode == CompanyCode).Select(aa => new { aa.Id, aa.CompanyCode, aa.CompanyName,aa.LogoPath,aa.PunchLine }).FirstOrDefault();
            if (GCompany == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
            }
            return Ok(GCompany);
        }

        // PUT: api/GCompanies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGCompany(int id, GCompany GCompany)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Company")));
            }
            if (!GCompanyExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
            }

            if (id != GCompany.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Company")));
            }
            try
            {
                db.Entry(GCompany).State = EntityState.Modified;
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
            return Ok(GCompany);
        }

        // POST: api/GCompanies
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> PostGCompany(GCompany GCompany)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Company")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                db.GCompanies.Add(GCompany);
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

            return CreatedAtRoute("DefaultApi", new { id = GCompany.Id }, GCompany);
        }

        // DELETE: api/GCompanies/5
        [ResponseType(typeof(GCompany))]
        public async Task<IHttpActionResult> DeleteGCompany(int id)
        {
            GCompany GCompany = await db.GCompanies.FindAsync(id);
            if (GCompany == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
            }
            try
            {
                db.GCompanies.Remove(GCompany);
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
            return Ok(GCompany);
        }

        private bool GCompanyExists(int id)
        {
            return db.GCompanies.Count(e => e.Id == id) > 0;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetCompanyInfo(string CompanyCode)
        {
            string Domain = "";
            var GCompany = db.GCompanies.Where(p => p.CompanyCode == CompanyCode).Select(aa => new { aa.Id, aa.CompanyCode, aa.CompanyName, aa.LogoPath, aa.PunchLine }).FirstOrDefault();

            var LPasswordPolicy = db.LPasswordPolicies.Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();

            if (GCompany == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
            }

            if (LPasswordPolicy == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PasswordPolicy")));
            }

            var LUser = db.LUsers.Where(p => p.CompanyCode == CompanyCode).Select(p => new { p.LoginEmail }).FirstOrDefault();
            string LoginEmail = (LUser.LoginEmail).ToString();

            if (LUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "User")));

            }
            else
            {
                var Login = LoginEmail.Split('@').ToList();
                Domain = Login[1];
            }

            var CompanyInfo = new CreateCompanyViewModel
            {
                CompanyCode=(GCompany.CompanyCode).ToString(),
                CompanyName=(GCompany.CompanyName).ToString(),
                LogoPath=(GCompany.LogoPath).ToString(),
                PunchLine=(GCompany.PunchLine).ToString(),
                DomainAddress=Domain.ToString(),
                MinLength= Convert.ToInt32(LPasswordPolicy.MinLength),
                MinUpperCase= Convert.ToInt32(LPasswordPolicy.MinUpperCase),
                MinLowerCase= Convert.ToInt32(LPasswordPolicy.MinLowerCase),
                MinNumbers= Convert.ToInt32(LPasswordPolicy.MinNumbers),
                MinSpecialChars= Convert.ToInt32(LPasswordPolicy.MinSpecialChars),
                MinAgeDays= Convert.ToInt32(LPasswordPolicy.MinAgeDays),
                MaxAgeDays= Convert.ToInt32(LPasswordPolicy.MaxAgeDays),
                ReminderDays= Convert.ToInt32(LPasswordPolicy.ReminderDays),
                PreventReuse= Convert.ToInt32(LPasswordPolicy.PreventReuse),
                LockoutFailedAttempts= Convert.ToInt32(LPasswordPolicy.LockoutFailedAttempts),
                LockoutMinutes= Convert.ToInt32(LPasswordPolicy.LockoutMinutes)
            };
            return Ok(CompanyInfo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateCompany(CreateCompanyViewModel model)
        //public async Task<IHttpActionResult> CreateCompany()
        {
            //var model = new CreateCompanyViewModel
            //{
            //    CompanyCode="TC",
            //    CompanyName="TestCompany",
            //    LogoPath= "../Content/VodafoneThemes/images/CicadaLogo.png",
            //    PunchLine="Testing",
            //    DomainAddress="testcompany.com",
            //    MinLength=8,
            //    MinUpperCase=2,
            //    MinLowerCase=2,
            //    MinNumbers=2,
            //    MinSpecialChars=1,
            //    MinAgeDays=30,
            //    MaxAgeDays=60,
            //    ReminderDays=45,
            //    PreventReuse=1,
            //    LockoutFailedAttempts=3,
            //    LockoutMinutes=15

            //};
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Company")));
            }
            //need to remove transactions, as its not required in this scenario
            try
            {
                var Query = "Exec [SpCreateCompany] @CompanyCode,@ComapnyName, @LogoPath,@PunchLine,@DomainAddress,@MinLength,@MinUpperCase,@MinLowerCase,@MinNumbers,@MinSpecialChars,@MinAgeDays,@MaxAgeDays,@ReminderDays,@PreventReuse,@LockoutFailedAttempts,@LockoutMinutes,@Result Output";
               using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(Query);
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@CompanyCode", model.CompanyCode);
                    cmd.Parameters.AddWithValue("@ComapnyName", model.CompanyName);
                    cmd.Parameters.AddWithValue("@LogoPath", model.LogoPath);
                    cmd.Parameters.AddWithValue("@PunchLine", model.PunchLine);
                    cmd.Parameters.AddWithValue("@DomainAddress", model.DomainAddress);
                    cmd.Parameters.AddWithValue("@MinLength", model.MinLength);
                    cmd.Parameters.AddWithValue("@MinUpperCase", model.MinUpperCase);
                    cmd.Parameters.AddWithValue("@MinLowerCase", model.MinLowerCase);
                    cmd.Parameters.AddWithValue("@MinNumbers", model.MinNumbers);
                    cmd.Parameters.AddWithValue("@MinSpecialChars", model.MinSpecialChars);
                    cmd.Parameters.AddWithValue("@MinAgeDays", model.MinAgeDays);
                    cmd.Parameters.AddWithValue("@MaxAgeDays", model.MaxAgeDays);
                    cmd.Parameters.AddWithValue("@ReminderDays", model.ReminderDays);
                    cmd.Parameters.AddWithValue("@PreventReuse", model.PreventReuse);
                    cmd.Parameters.AddWithValue("@LockoutFailedAttempts", model.LockoutFailedAttempts);
                    cmd.Parameters.AddWithValue("@LockoutMinutes", model.LockoutMinutes);
                    cmd.Parameters.Add("@Result", SqlDbType.VarChar, 255);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    string LoginEmail = cmd.Parameters["@Result"].Value.ToString();//getting value of output parameter

                    if (LoginEmail == "")
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, LoginEmail));//type 2 error

                    conn.Close();

                    // Create Entry in AD

                    var ADmodel = new ADModel
                    {
                        Email= LoginEmail,
                        Password= ConfigurationManager.AppSettings["DefaultPassword"]
                     };
                    AuthenticationResult result = new AuthenticationResult();
                    result = Globals.CreateUser(ADmodel);
                    if (result.IsSuccess)
                    {
                        return Ok(true);
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
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

           // return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateData(CreateCompanyViewModel model)
       
        {
            //var model = new CreateCompanyViewModel
            //{
            //    CompanyCode = "TC",
            //    CompanyName = "TestCompany",
            //    LogoPath = "../Content/VodafoneThemes/images/CicadaLogo.png",
            //    PunchLine = "Testing Purpose",
            //    DomainAddress = "testcompany.com",
            //    MinLength = 9,
            //    MinUpperCase = 2,
            //    MinLowerCase = 2,
            //    MinNumbers = 2,
            //    MinSpecialChars = 1,
            //    MinAgeDays = 30,
            //    MaxAgeDays = 60,
            //    ReminderDays = 45,
            //    PreventReuse = 1,
            //    LockoutFailedAttempts = 3,
            //    LockoutMinutes = 15

            //};
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Company")));
            }
            
            try
            {
                var GCompany = db.GCompanies.Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                if (GCompany == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
                }

                else
                {
                    GCompany.CompanyName = (model.CompanyName).ToString();
                    GCompany.PunchLine = (model.PunchLine).ToString();
                    GCompany.LogoPath = (model.LogoPath).ToString();
                    db.Entry(GCompany).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }

               
                var LPasswordPolicy = db.LPasswordPolicies.Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                if (LPasswordPolicy == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Company")));
                }
                else
                {
                    LPasswordPolicy.MinLength = Convert.ToInt32(model.MinLength);
                    LPasswordPolicy.MinUpperCase = Convert.ToInt32(model.MinUpperCase);
                    LPasswordPolicy.MinLowerCase = Convert.ToInt32(model.MinLowerCase);
                    LPasswordPolicy.MinNumbers = Convert.ToInt32(model.MinNumbers);
                    LPasswordPolicy.MinSpecialChars = Convert.ToInt32(model.MinSpecialChars);
                    LPasswordPolicy.MinAgeDays = Convert.ToInt32(model.MinAgeDays);
                    LPasswordPolicy.MaxAgeDays = Convert.ToInt32(model.MaxAgeDays);
                    LPasswordPolicy.ReminderDays = Convert.ToInt32(model.ReminderDays);
                    LPasswordPolicy.PreventReuse = Convert.ToInt32(model.PreventReuse);
                    LPasswordPolicy.LockoutFailedAttempts = Convert.ToInt32(model.LockoutFailedAttempts);
                    LPasswordPolicy.LockoutMinutes = Convert.ToInt32(model.LockoutMinutes);



                    db.Entry(LPasswordPolicy).State = EntityState.Modified;
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

            return Ok();
        }

       
        public async Task<IHttpActionResult> DeleteCompany(string CompanyCode)
        {
            
            try
              {
                
                //Before calling SpDeleteCompany, we need to delete all users for this company from AD
                var LUser = db.LUsers.Where(p => p.CompanyCode == CompanyCode).Select(p => new { p.LoginEmail }).ToList();
                foreach (var user in LUser)
                {
                    var ADmodel = new ADModel
                    {
                        Email = user.LoginEmail,

                    };
                    AuthenticationResult result = new AuthenticationResult();
                    result = Globals.DeleteUser(ADmodel);
                    if (!result.IsSuccess)
                    {
                        
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
                    }
               }
                var Query = "Exec [SpDeleteCompany] @CompanyCode";
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(Query);
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                  
                }
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
            return Ok();
        }


        public string GetCompanyCodeByCompanyId(int id)
        {
            var xx = (from aa in db.GCompanies where aa.Id == id
                      select new { aa.CompanyCode}).FirstOrDefault()  ;
            return xx.CompanyCode;
        }

        private string GetCustomizedErrorMessage(Exception ex)
             {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_GCompanies_LFSItemTemplates_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "ITEM TEMPLATES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_GKeyValues_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "KEY VALUES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LDropDowns_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "DROP DOWNS"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LEmailTemplates_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "EMAIL TEMPLATES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LFSItemTypes_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "ITEM TYPES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LPasswordPolicies_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "PASSWORD POLICIES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LPortfolios_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "PORTFOLIOS"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LRoles_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "ROLES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_LUsers_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "USERS"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_RLocalPobTypes_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "LOCAL POB TYPES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_RProductCategories_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "PRODUCT CATEGORIES"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_RProductSystems_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "PRODUCT SYSTEMS"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_RRequestSystems_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "COMPANY", "REQUESTS SYSTEMS"));

            else if (SqEx.Message.IndexOf("FK_GCompanies_RUseCaseIndicators_CompanyCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Company", "USECASE INDICATOR"));

            else if (SqEx.Message.IndexOf("UQ_GCompanies_CompanyName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "COMPANIES"));
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
