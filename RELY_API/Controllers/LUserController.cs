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
    public class LUserController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        [HttpGet]
        public IHttpActionResult TerminateUser(string Status,int Id,int LoggedInUserId)
        {
            var user = db.LUsers.Where(a => a.Id == Id).FirstOrDefault();
            user.WFStatus = Status;
            user.UpdatedById = LoggedInUserId; 
            user.UpdatedDateTime = DateTime.UtcNow;
            db.Entry(user).State = EntityState.Detached;
            db.SaveChanges();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

            // GET: api/LUsers
            public IHttpActionResult GetLUsers()
        {
            var xx = (from aa in db.LUsers
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.LoginEmail,
                          aa.FirstName,
                          aa.LastName,
                          aa.Phone,
                          aa.BlockNotification,
                          aa.Status,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.LockoutUntil
                      }).OrderBy(p => p.LoginEmail);
            return Ok(xx);
        }

        public IHttpActionResult GetLUsersByCompanyCode(string CompanyCode)
        {
            var xx = (from aa in db.LUsers
                      where aa.CompanyCode == CompanyCode
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.LoginEmail,
                          aa.FirstName,
                          aa.LastName,
                          aa.Phone,
                          aa.BlockNotification,
                          aa.Status,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.LockoutUntil
                      }).OrderBy(p => p.LoginEmail);
            return Ok(xx);
        }

        [HttpGet]
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> GetLUserByEmail(string Email, string UserName, string WorkFlow)
        {
            var LUser = db.LUsers.Where(p => p.LoginEmail == Email).Select(aa => new {
                aa.Id,
                aa.CompanyCode,
                aa.LoginEmail,
                aa.FirstName,
                aa.LastName,
                aa.Phone,
                aa.BlockNotification,
                aa.Status,
                aa.CreatedById,
                aa.CreatedDateTime,
                aa.UpdatedById,
                aa.UpdatedDateTime,
                aa.LockoutUntil,
                aa.OTP,
                aa.OTPValidUpto,
                aa.WFAnalystId,
                aa.WFComments,
                aa.WFCurrentOwnerId,
                aa.WFManagerId,
                aa.WFOrdinal,
                aa.WFRequesterId,
                aa.WFRequesterRoleId,
                aa.WFStatus,
                aa.WFType,
                aa.ChangePwdAtNextLogin
            }).FirstOrDefault();
            if (LUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
            }
            return Ok(LUser);
        }

        // GET: api/LUsers/5
        [HttpGet]
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> GetLUser(Nullable<int> id, string UserName, string WorkFlow)
        {
            var LUser = db.LUsers.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.CompanyCode,
                aa.LoginEmail,
                aa.FirstName,
                aa.LastName,
                aa.Phone,
                aa.BlockNotification,
                aa.Status,
                aa.CreatedById,
                aa.CreatedDateTime,
                aa.UpdatedById,
                aa.UpdatedDateTime,
                aa.LockoutUntil,
                aa.OTP,
                aa.OTPValidUpto,
                aa.WFAnalystId,
                aa.WFComments,
                aa.WFCurrentOwnerId,
                aa.WFManagerId,
                aa.WFOrdinal,
                aa.WFRequesterId,
                aa.WFRequesterRoleId,
                aa.WFStatus,
                aa.WFType,
                aa.ChangePwdAtNextLogin
            }).FirstOrDefault();
            if (LUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
            }
            return Ok(LUser);
        }

        // PUT: api/LUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLUser(int id, LUser LUser, string RolesList, string UserName, string WorkFlow, string FormType, int LoggedInUserId, int LoggedInRoleId)
        {
            
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "USER")));
            //}

            if (!LUserExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
            }

            if (id != LUser.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "USER")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //If form type is change then we will calculate ordinal of Logeed in user Id where he has create or we will setit to 1
                    //SG - 12 Feb 2019 - Users should not go in WF again. So keep Status,Ordinal values uncahnegd on Chnage Action
                    if (FormType == "Change")
                    {
                        var WorkflowDetails = db.RWorkFlows.Where(p => p.Name.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == LUser.CompanyCode).FirstOrDefault();
                        var StatusAndOrdinal = db.Database.SqlQuery<NameOrdinalViewModel>("select Name,Ordinal from dbo.FnGetWFColumnValues({0},{1},{2},'Create',{3})", LoggedInUserId, LoggedInRoleId, WorkflowDetails.Id, LUser.CompanyCode).FirstOrDefault();
                        if (StatusAndOrdinal != null)
                        {
                            LUser.WFOrdinal = StatusAndOrdinal.Ordinal;
                            // LUser.Status = StatusAndOrdinal.Name;
                        }
                        else
                        {
                            LUser.WFOrdinal = 1;
                            //  LUser.Status = "Initial";
                        }
                        LUser.WFStatus = "InProgress";
                    }
                    db.Entry(LUser).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //Add Roles and Delete previous roles
                    if (!string.IsNullOrEmpty(RolesList))
                    {
                        //check if user already has L2Admin role. If so, We need to add to RoleList as L2Admin role is not visible in forms
                        var userRole = (from aa in db.MUserRoles.Where(aa => aa.UserId == LUser.Id)
                                        join bb in db.LRoles on aa.RoleId equals bb.Id
                                        where bb.RoleName == "L2Admin"
                                        select new { aa.RoleId }).FirstOrDefault();
                        if (userRole != null)
                        {
                            RolesList = RolesList + "," + userRole.RoleId;
                        }
                        var ExistingRoles = db.MUserRoles.Where(p => p.UserId == LUser.Id).ToList();
                        db.MUserRoles.RemoveRange(ExistingRoles);//Delete previous roles
                        db.SaveChanges(); 
                        var RoleArray = RolesList.Split(',').ToList();
                        foreach (var roleid in RoleArray)
                        {
                            var RoleId = Convert.ToInt32(roleid);
                            var MUserRolesModel = new MUserRole { UserId = LUser.Id, RoleId = RoleId };
                            db.MUserRoles.Add(MUserRolesModel);
                            db.SaveChanges();
                        }
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

        // POST: api/LUsers
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> PostLUser(LUser LUser, string RolesList, string UserName, string WorkFlow)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == LUser.CompanyCode).FirstOrDefault();
                    var StatusAndOrdinal = db.Database.SqlQuery<NameOrdinalViewModel>("select Name,Ordinal from dbo.FnGetWFColumnValues({0},{1},{2},'Create',{3})", LUser.WFRequesterId, LUser.WFRequesterRoleId, WorkflowDetails.Id, LUser.CompanyCode).FirstOrDefault();
                    if (StatusAndOrdinal != null)
                    {
                        LUser.WFOrdinal = StatusAndOrdinal.Ordinal;
                        LUser.Status = StatusAndOrdinal.Name;
                    }
                    else
                    {
                        LUser.WFOrdinal = 1;
                        LUser.Status = "Initial";
                    }

                    db.LUsers.Add(LUser);
                    await db.SaveChangesAsync();
                    
                    //Add Roles
                    if (!string.IsNullOrEmpty(RolesList))
                    {
                        var RoleArray = RolesList.Split(',').ToList();
                        foreach (var roleid in RoleArray)
                        {
                            var RoleId = Convert.ToInt32(roleid);
                            var MUserRolesModel = new MUserRole { UserId = LUser.Id, RoleId = RoleId };
                            db.MUserRoles.Add(MUserRolesModel);
                            db.SaveChanges();
                        }
                    }
                    //Add Portfolios
                    if (db.LPortfolios.Where(m => m.CompanyCode == LUser.CompanyCode).Count() > 0)
                    {
                        MEntityPortfolio MEP = new MEntityPortfolio();
                        MEP.EntityId = LUser.Id;
                        MEP.EntityType = "LUsers";
                        int PortfolioId = db.LPortfolios.Where(m => m.CompanyCode == LUser.CompanyCode).FirstOrDefault().Id;
                        MEP.PortfolioId = PortfolioId;
                        db.MEntityPortfolios.Add(MEP);
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
            //return CreatedAtRoute("DefaultApi", new { id = LUser.Id }, LUser);
            return Ok();
        }

        // DELETE: api/LUsers/5
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> DeleteLUser(int id)
        {
            LUser LUser = await db.LUsers.FindAsync(id);
            if (LUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
            }
            try
            {
                db.LUsers.Remove(LUser);
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
            return Ok(LUser);
        }

        private bool LUserExists(int id)
        {
            return db.LUsers.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message

            if (SqEx.Message.IndexOf("FK_LUsers_GCopaDimensions_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "COPA DIMENSIONS"));
            else if (SqEx.Message.IndexOf("FK_LUsers_GcopaDimensions_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "COPA DIMENSIONS"));
            else if (SqEx.Message.IndexOf("FK_LUsers_GGlobalPobs_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "GLOBAL POBS"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LAccountingScenario_WFRequesterId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "ACCOUNTING SCENARIO"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LEmailBucket_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "EMAIL BUCKET"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LEmailBucket_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "EMAIL BUCKET"));

            //LFS TABLES
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSAnswerBank_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "ANSWER BANK"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSAnswerBank_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "ANSWER BANK"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSNextSteps_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "NEXT STEP"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSNextSteps_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "NEXT STEP"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSQuestionBank_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "QUESTION BANK"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSQuestionBank_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "QUESTION BANK"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSSurveyLevels_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "SURVEY LEVEL"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LFSSurveyLevels_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "SURVEY LEVEL"));


            //FK will work until name of FK will not change please delete once correct in db
            else if (SqEx.Message.IndexOf("FK_LUseers_LLocalPob_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            //FK with correct Name 
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_WFAnalystId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_WFCurrentOwnerId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_WFManagerId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LLocalPob_WFRequesterId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "LOCAL POB"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LPasswordHistory_LoginEmail", StringComparison.OrdinalIgnoreCase) >= 0)

                //return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "User's Email", "PASSWORD HISTORY"));
                return ("Cannot update user'email because it is being used as Login into the application");// fix for issue#41
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_WFAnalystId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_WFCurrentOwnerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_WFManagerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            //else if (SqEx.Message.IndexOf("FK_LUsers_LRequests_WFReauesterId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REQUESTS"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LReferenceTypes_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REFERENCE TYPE"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LReferenceTypes_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "REFERENCE TYPE"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LSupportingDocuments_CreatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "SUPPORTING DOCUMENT"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LSupportingDocuments_UpdatedById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "SUPPORTING DOCUMENT"));


            else if (SqEx.Message.IndexOf("FK_LUsers_LUserActivityLog_ActionById", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "USER ACTIVITYLOG"));
            else if (SqEx.Message.IndexOf("FK_LUsers_LUserActivityLog_ActionForId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "USER ACTIVITYLOG"));

            else if (SqEx.Message.IndexOf("FK_LUsers_MLUsersGSecurityQuestions_UserId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "USER SECURITYQUESTIONS"));

            else if (SqEx.Message.IndexOf("FK_LUsers_MUsersRoles_UserId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "USERS ROLES"));

            else if (SqEx.Message.IndexOf("UQ_LUsers_LoginEmail", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "USERS"));
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

    public class NameOrdinalViewModel
    {
        public string Name { get; set; }
        public int Ordinal { get; set; }
    }
}
