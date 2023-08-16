using System;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RELY_API.Utilities;
using RELY_API.Models;
using System.Linq;
using System.Collections.Generic;
using static RELY_API.Utilities.Globals;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace RELY_API.Controllers
{
    //[RoutePrefix("api/Account")]
    [CustomExceptionFilter]
    public class AccountController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        //Method to Get user details by Email id
        [HttpGet]
        [OverrideAuthorization]
        public IHttpActionResult GetIdByEmailId(string Email)
        {
            var user = db.LUsers.Where(p => p.LoginEmail == Email).
                Select(p => new { p.LoginEmail, p.CompanyCode, p.Id ,p.FirstName,p.LastName,p.CreatedById,p.UpdatedById,p.CreatedDateTime,p.UpdatedDateTime,
                    p.BlockNotification,p.Phone,p.Status,p.LockoutUntil}).FirstOrDefault();
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid User"));
            }

        }


        //method used to get user details to web App and Authenticate user details
        // [Route("UserInfo")]
        [OverrideAuthorization]
        public IHttpActionResult GetUserInfo(string HostBrowserDetails, string HostIP, string HostTimeZone, string Email, string Password, string MFAOtp)
        {
            try
            {
                //check whether Email exists in db or not
                var UserDetails = db.LUsers.FirstOrDefault(p => p.LoginEmail == Email);
                if(UserDetails == null)
                {

                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "User Not registered."));
                }
                DateTime LockUnTilDateTime = DateTime.UtcNow;
                if (UserDetails.LockoutUntil.HasValue)
                {
                    LockUnTilDateTime = UserDetails.LockoutUntil.Value;
                }
                var xx = UserDetails;
                //whenever user is disabled/suspended,its Status values becomes Suspended.
                if (UserDetails.Status.Equals("Suspended"))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "User has been suspended."));
                }
                //if wfstatus is not completed, msg to user "User is not Approved."
                if (!UserDetails.WFStatus.Equals("Completed"))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "User is not approved."));
                }
                if (LockUnTilDateTime <= DateTime.UtcNow)
                {
                    //Authentication from AD, will be implemented later
                    ADModel model = new ADModel();
                    model.Email = Email;
                    model.Password = Password;
                    var result = Globals.SignIn(model);
                    if (!result.IsSuccess)
                    {
                        var LUserActivityModel = new LUserActivityLog();
                        //incorrect password
                        if (result.ErrorMessage.Contains("incorrect"))
                        {

                            // if (!Password.TrimEnd().Equals("Rely#123"))//testing for hardcoded password value
                            //{
                            //Add Entry in User Log for Failed Login
                            // var LUserActivityModel = new LUserActivityLog();
                            LUserActivityModel.ActionById = UserDetails.Id;
                            LUserActivityModel.Activity = "FailedLogin";
                            LUserActivityModel.Remarks = "Login Failed";
                            LUserActivityModel.ActivityDateTime = DateTime.UtcNow;
                            LUserActivityModel.CompanyCode = UserDetails.CompanyCode;
                            LUserActivityModel.IsActivitySucceeded = true;
                            LUserActivityModel.ActionForId = UserDetails.Id;
                            LUserActivityModel.HostBrowserDetails = HostBrowserDetails;
                            LUserActivityModel.HostIP = HostIP;
                            LUserActivityModel.HostTimeZone = HostTimeZone;
                            db.LUserActivityLogs.Add(LUserActivityModel);
                            db.SaveChanges();

                            //checking User Activity and LockAccount if consecutive UnSucessfull Attempts are made
                            var LPasswordPolicies = db.LPasswordPolicies.Where(p => p.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                            /*If failed login,  if  LockoutFailedAttempts  > 0,check if the number of consecutive failed exceeds LockoutFailedAttempts, 
                            If yes, update ASPNetUser table with GetDate()+LockoutMins in the column <LockoutEndDateUtc> for that user.*/
                            if (LPasswordPolicies != null)
                            {
                                if (LPasswordPolicies.LockoutFailedAttempts > 0)
                                {
                                    var UserActivityLog = db.LUserActivityLogs.Where(p => p.ActionForId == UserDetails.Id).OrderByDescending(p => p.ActivityDateTime).Take(LPasswordPolicies.LockoutFailedAttempts).ToList();
                                    var LockoutFailedAttempts = LPasswordPolicies.LockoutFailedAttempts;
                                    if (UserActivityLog.Where(p => p.Activity.Equals("FailedLogin")).Count() >= LockoutFailedAttempts)
                                    {
                                        UserDetails.LockoutUntil = DateTime.UtcNow.AddMinutes(LPasswordPolicies.LockoutMinutes);
                                        db.Entry(UserDetails).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Add Entry For User Lockout
                                        //checking User Activity and LockAccount if consecutive UnSucessfull Attempts are made
                                        //Add Entry For User Lockout
                                        LUserActivityModel = new LUserActivityLog();
                                        LUserActivityModel.ActionById = UserDetails.Id;
                                        LUserActivityModel.Activity = "Lockout";
                                        LUserActivityModel.Remarks = "LogIn Locked Out";
                                        LUserActivityModel.ActivityDateTime = DateTime.UtcNow;
                                        LUserActivityModel.CompanyCode = UserDetails.CompanyCode;
                                        LUserActivityModel.IsActivitySucceeded = true;
                                        LUserActivityModel.ActionForId = UserDetails.Id;
                                        LUserActivityModel.HostBrowserDetails = HostBrowserDetails;
                                        LUserActivityModel.HostIP = HostIP;
                                        LUserActivityModel.HostTimeZone = HostTimeZone;
                                        // LUserActivityModel.Status = "";
                                        db.LUserActivityLogs.Add(LUserActivityModel);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, "There is something wrong with Password Policy. Please contact L2Admin"));
                            }

                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "UserName/Password is incorrect"));
                        }
                        else if (result.ErrorMessage.Contains("reset"))
                        {
                            //redirect user to Login Page Only if He/She is Authenticated
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "Reset Password"));
                            //return Ok(result.ErrorMessage);//need to reset password
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
                        }
                    }
                    //if user is Authenticated check if his password is expired or not
                    if (xx.ChangePwdAtNextLogin)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "Reset Password"));
                    }
                    //check for MaxAgeDays from PwdPolicies,if reached, resest pwd
                    var LPasswordHistory = db.LPasswordHistories.Where(p => p.LoginEmail == xx.LoginEmail).ToList();
                    var PasswordPolicies = db.LPasswordPolicies.Where(p => p.CompanyCode == xx.CompanyCode).FirstOrDefault();
                    var PasswordExpiryDate = DateTime.UtcNow;
                    if (LPasswordHistory.Count() > 0 && PasswordPolicies != null)
                    {
                        Nullable<DateTime> MaxPasswordResetDate = LPasswordHistory.Max(p => p.CreatedDateTime);
                        PasswordExpiryDate = MaxPasswordResetDate.Value.AddDays(PasswordPolicies.MaxAgeDays);
                    }
                    /*If reached here check that the Expiry  date has exceeded today's date then redirect user to reset password screen
                     Either Password is expired or User is Logging in first time*/
                    if (PasswordExpiryDate < DateTime.UtcNow || LPasswordHistory.Count() == 0)
                    {
                        //return Ok("reset password");
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "Reset Password"));
                    }



                    //when user is authenticated,Get role list for the user
                    var RoleList = (from rr in db.LRoles
                                 join mur in db.MUserRoles.Where(u => u.UserId == UserDetails.Id) on rr.Id equals mur.RoleId
                                 where rr.CompanyCode == UserDetails.CompanyCode
                                 select new RoleViewModel
                                 {
                                     Id = rr.Id,
                                     RoleName = rr.RoleName,
                                     IsDefault = mur.IsDefault,
                                     IsMFAEnabled = rr.MFAEnabled //MFAEnabled
                                 }).OrderBy(a => a.RoleName).OrderByDescending(a => a.IsDefault).ToList();

                    /*  SG - 22Jan2019 - 2.5	Multiple Factor Authentication
                        For certain roles in certain OpCos, users will be asked to be authenticated by another mode (Token sent via SMS/Email). 
                        This is controlled by table LRoles (column MFA Enabled)
                        For all the roles for whom we want to enable 2FA, we will set MFAEnabled for the OpCo-Role in MUserRoles table.
                        When the user logs in, we check for MFAEnabled for the roles that user has. If any of the role has MFAEnabled, 
                        then SMS/EMAIL is sent to user with OTP and OTP screen is presented to user.
                        If user enters correct OTP, user is let into the system.
                     */
                    if (RoleList.Any(p => p.IsMFAEnabled) && string.IsNullOrEmpty(MFAOtp))
                    {
                        //Generate OTP and Send it to the User Via SMS
                        Random rd = new Random();
                        var RandomNumber = rd.Next(100000, 999999);
                        //Get PhoneNumber
                        string ReceiverPhoneNumber = db.LUsers.Where(p => p.Id == xx.Id).Select(p => p.Phone).FirstOrDefault();
                        //Update AspNetUser
                        UserDetails.OTP = RandomNumber.ToString();
                        UserDetails.OTPValidUpto = DateTime.UtcNow.AddMinutes(Convert.ToDouble(PasswordPolicies.MFAOTPValidMins));
                        db.Entry(UserDetails).State = EntityState.Modified;
                        db.SaveChanges();
                        //Manipulate Phone number
                        var ManipulatedPhoneNumber = db.Database.SqlQuery<string>("select [dbo].[FNReturnPhoneNumber]({0},{1})", ReceiverPhoneNumber, UserDetails.CompanyCode).FirstOrDefault();
                        //string ManipulatedPhoneNumber = "";

                        string SMSAccessKey = Globals.GetValue("sns_accesskey");  //ConfigurationManager.AppSettings["SMSAccessKey"];
                        string SMSSecretKey = Globals.GetValue("sns_secretkey"); // ConfigurationManager.AppSettings["SMSSecretKey"];
                        var snsClient = new AmazonSimpleNotificationServiceClient(SMSAccessKey, SMSSecretKey);
                        String message = "Your MFA token for login into RELY is " + RandomNumber + ". If you did not request this, please contact to support";
                        String phoneNumber = ManipulatedPhoneNumber;//"+919871073209";//"+61450280180";
                        string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a=>a.Value).FirstOrDefault();
                        if (!string.IsNullOrEmpty(ManipulatedPhoneNumber))
                        {
                            //If we are doing MFA Authentication in test(Non Prod) it should send SMS to Dummy Number(by concating test to mobile number).
                            if (!ProjectEnviournment.Equals("Prod",StringComparison.OrdinalIgnoreCase))
                            {
                                phoneNumber = "T" + phoneNumber;
                            }
                            var LSMS = new LSMSBucket
                            {
                                Recipient = phoneNumber,
                                Message = message,
                                Status = "Sent",
                                CreatedById = UserDetails.LoginEmail,
                                UpdatedById = UserDetails.LoginEmail,
                                CreatedDateTime = DateTime.UtcNow,
                                UpdatedDateTime = DateTime.UtcNow
                            };
                            db.LSMSBuckets.Add(LSMS);
                            db.SaveChanges();
                            try
                            {
                                Dictionary<System.String, MessageAttributeValue> smsAttributes = new Dictionary<string, MessageAttributeValue>();
                                snsClient.Publish(new PublishRequest()
                                {
                                    Message = message,
                                    PhoneNumber = phoneNumber,
                                    MessageAttributes = smsAttributes
                                });
                            }
                            catch (Exception ex)
                            {
                                //Add Error Code and Message in  Comments in SMS table. Also Update Status to failed.
                                LSMS.Comments = ex.Message + ex.StackTrace;
                                LSMS.Status = "Failed";
                                db.Entry(LSMS).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //Send Email
                        var SenderConfig = ConfigurationManager.AppSettings["SenderAccountName"];
                        db.SpLogEmail(UserDetails.LoginEmail, null, null, null, "MFA token for RELY", message, true, "Notifier", "High", null, "InQueue", null, UserDetails.Id, UserDetails.Id, SenderConfig);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "MFAScreen"));
                    }
                    //MFA Verifcation code moved to separate method.
                    //else if (RoleList.Any(p => p.IsMFAEnabled) && !string.IsNullOrEmpty(MFAOtp))
                    //{
                    //    //Valiadate The OTP and return Error if Invalid
                    //    if (UserDetails.OTP != MFAOtp)
                    //    {
                    //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "Invalid MFA code entered"));
                    //    }
                    //    else if (UserDetails.OTP == MFAOtp && UserDetails.OTPValidUpto < DateTime.UtcNow)
                    //    {
                    //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "MFA code expired.Please generate new code from login screen."));
                    //    }
                    //}

                    //Get Last Login Details
                    var LastLoginLog = db.LUserActivityLogs.Where(p => p.ActionForId == UserDetails.Id).OrderByDescending(p => p.ActivityDateTime).Select(p=>new {LastActionMessage="Your last login attempt made on "+p.ActivityDateTime+" was "+((p.Activity== "FailedLogin")?" not ":"")+ " successful." }).Select(p=>p.LastActionMessage).FirstOrDefault();
                    if (xx.Status.Equals("Active"))
                    {
                        return Ok(new LoginViewModel
                        {
                            Email = xx.LoginEmail,
                            CompanyCode = xx.CompanyCode,
                            Roles = RoleList,
                            Id = xx.Id,
                            FirstName = xx.FirstName,
                            LastName = xx.LastName,
                            FullName = xx.FirstName + " " + xx.LastName,
                            Phone = xx.Phone,
                            LastLoginMessage= LastLoginLog
                        });
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "User is Inactive"));
                        //return Ok("User is InActive");
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "Your account is locked temporarily, please try later"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IHttpActionResult VerifyMFAToken(string LoginEmail, string MFAOtp)
        {
            var UserDetails = db.LUsers.Where(a => a.LoginEmail == LoginEmail).FirstOrDefault();
            //when user is authenticated,Get role list for the user
            var RoleList = (from rr in db.LRoles
                            join mur in db.MUserRoles.Where(u => u.UserId == UserDetails.Id) on rr.Id equals mur.RoleId
                            where rr.CompanyCode == UserDetails.CompanyCode
                            select new RoleViewModel
                            {
                                Id = rr.Id,
                                RoleName = rr.RoleName,
                                IsDefault = mur.IsDefault,
                                IsMFAEnabled = rr.MFAEnabled //MFAEnabled
                            }).OrderByDescending(a => a.IsDefault).OrderBy(a => a.RoleName).ToList();

            if (string.IsNullOrEmpty(MFAOtp))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3,
                    "Please provide MFA Code"));
            }
            
            //Valiadate The OTP and return Error if Invalid
            if (UserDetails.OTP != MFAOtp)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "Invalid MFA code entered"));
            }
            else if (UserDetails.OTP == MFAOtp && UserDetails.OTPValidUpto < DateTime.UtcNow)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type3, "MFA code expired.Please generate new code from login screen."));
            }

            //Get Last Login Details
            var LastLoginLog = db.LUserActivityLogs.Where(p => p.ActionForId == UserDetails.Id).OrderByDescending(p => p.ActivityDateTime).Select(p => new { LastActionMessage = "Your last login attempt made on " + p.ActivityDateTime + " was " + ((p.Activity == "FailedLogin") ? " not " : "") + " successful." }).Select(p => p.LastActionMessage).FirstOrDefault();
            if (UserDetails.Status.Equals("Active"))
            {
                return Ok(new LoginViewModel
                {
                    Email = UserDetails.LoginEmail,
                    CompanyCode = UserDetails.CompanyCode,
                    Roles = RoleList,
                    Id = UserDetails.Id,
                    FirstName = UserDetails.FirstName,
                    LastName = UserDetails.LastName,
                    FullName = UserDetails.FirstName + " " + UserDetails.LastName,
                    Phone = UserDetails.Phone,
                    LastLoginMessage = LastLoginLog
                });
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type4, "User is Inactive"));
            }
        }

        [HttpPost]
        public IHttpActionResult SetPasswordViaAdmin(ADModel model,int LoggedInUserId,int LoggedInRoleId, string UserName, string WorkFlow)
        {
            string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
            switch (ProjectEnviournment)
            {
                case "Prod":
                    RandomPassword pwd = new RandomPassword();
                    string randompwd = pwd.Generate();
                    model.NewPassword = randompwd;
                    break;
                default:
                    model.NewPassword = ConfigurationManager.AppSettings["DefaultPassword"];
                    break;
            }
            AuthenticationResult result = new AuthenticationResult();
            result = Globals.SetUserPassword(model);
            if (result.IsSuccess)
            {
                var UserDetails = db.LUsers.Where(p => p.LoginEmail == model.Email).FirstOrDefault();
                var PasswordHistoryModel = new LPasswordHistory { CreatedDateTime = DateTime.UtcNow, LoginEmail = UserDetails.LoginEmail };
                db.LPasswordHistories.Add(PasswordHistoryModel);
                UserDetails.ChangePwdAtNextLogin = true;
                db.Entry(UserDetails).State = EntityState.Detached;
                db.SaveChanges();
                db.Entry(UserDetails).State = EntityState.Modified;
                db.SaveChanges();
                //Add entry in email bucket
                var SenderConfig = ConfigurationManager.AppSettings["SenderAccountName"];

                //Getting EmailSubject and EmailBody from database
                var EmailTemplate = db.LEmailTemplates.Where(p => p.TemplateName == "Password Reset by Admin").Where(aa => aa.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                var EmailSubject = EmailTemplate.EmailSubject;
                var EmailBody = EmailTemplate.EmailBody;

                //Replace the placeholders with actual values
                EmailBody = (EmailBody.Replace("{User-name}", UserDetails.FirstName + " " + UserDetails.LastName)).Replace("{Password}", model.NewPassword);

                db.SpLogEmail(UserDetails.LoginEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, SenderConfig);

                //db.SpLogEmail(UserDetails.LoginEmail, null, null, null, "Vodafone RELY", "Hi " + UserDetails.FirstName + " " + UserDetails.LastName +
                //    ",<br>Your Vodafone RELY Password is " + model.NewPassword, true, "Notifier", "Normal", null, "InQueue", null, LoggedInRoleId, LoggedInUserId, SenderConfig);

            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
            }

            return Ok();
        }
       [HttpPost]
        public IHttpActionResult ChangeUserPassword( ADModel model,string UserName, string WorkFlow)
        {
            
            AuthenticationResult result = new AuthenticationResult();
            if (String.IsNullOrEmpty(model.Password))
            {
                result = Globals.SetUserPassword(model);
            }
            else
            {
                var User = db.LUsers.Where(p => p.LoginEmail == model.Email).FirstOrDefault();
                // var PasswordHistory = db.LPasswordHistories.Where(p => p.LoginEmail == User.LoginEmail);
                // if (PasswordHistory.Count() > 0)/*Added logic to set password instead of changing it when user logs in for the first time. To ensure that history days validation is not applied .*/
                if(!User.ChangePwdAtNextLogin)
                {
                    result = Globals.ChangeMyPassword(model);
                }
                else
                {
                    result = Globals.SetUserPassword(model);
                }
            }

            if (result.IsSuccess)
            {
                /*If Password has changed sucessfully add an entryin LPassword History
                 We will first delete all older passwords (First In First Out method) more than number of LPasswordPolicies.PreventReuse for that companyID 
                 and then insert current password row for that user
                ToDo: PsuedoCode: Before you INSERT in this table, select count(*) from LPasswordHistory for login user
                If count >= LpasswordPolicy. PreventReuse, then DELETE oldest password (min CreatedDate) 
                Then INSERT “Current password”
                •	AspNetUserId nvarchar
                •	Password nvarchar 100 Nullable (Do not insert password – functionality parked)
                •	CreatedDate DateTime
                 */
                var UserDetail = db.LUsers.Where(p => p.LoginEmail.Equals(model.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (UserDetail != null)
                {
                    var PasswordHistory = db.LPasswordHistories.Where(p => p.LoginEmail.Equals(UserDetail.LoginEmail)).ToList();
                    var PasswordPolicy = db.LPasswordPolicies.Where(p => p.CompanyCode == UserDetail.CompanyCode).FirstOrDefault();
                    /*JS 01/08/2017-Since we are using AD inherited policy of preventing password re-use, we need to code it here (also help us not saving user passwords in our database)*/
                    if (PasswordHistory.Count() >= PasswordPolicy.PreventReuse)//If No of Password Hostory record exceeds Prevent reuse count then delte the oldest record
                    {
                        var OldestPassword = PasswordHistory.OrderBy(p => p.CreatedDateTime).FirstOrDefault();
                        if (OldestPassword != null)
                        {
                            db.LPasswordHistories.Remove(OldestPassword);
                            db.SaveChanges();
                        }
                    }

                    UserDetail.ChangePwdAtNextLogin = false;
                    db.Entry(UserDetail).State = EntityState.Modified;
                    var NewPasswordDetails = new LPasswordHistory { Id = UserDetail.Id, CreatedDateTime = DateTime.UtcNow, LoginEmail = UserDetail.LoginEmail };
                    db.LPasswordHistories.Add(NewPasswordDetails);
                    db.SaveChanges();
                }
                return Ok(true);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
            }

        }

        [HttpPost]
       // [OverrideAuthorization]
        public IHttpActionResult GenerateOTPnSendMail(MLUsersGSecurityQuestionViewModel model,string UserName, string WorkFlow)
        {
            string ans1 = (from aa in db.MLUsersGSecurityQuestions.Where(aa => aa.UserId == model.UserId).Where(aa => aa.QuestionId == model.Question1)
                           select aa.Answer).FirstOrDefault();

            //var ans1 = db.Database.SqlQuery<string>("select Answer from MLUsersGSecurityQuestions where UserId={0} and QuestionId={1}",model.UserId,model.Question1).FirstOrDefault();
            //need to check one question at a time.
            // var ans2 = db.Database.SqlQuery<string>("select MAugsqAnswer from MAspnetUsersGSecurityQuestions where MAuqsqUserId='" + model.MAuqsqUserId + "' and MAuqsqQuestionId=" + model.Question2).FirstOrDefault();
            if (ans1.ToLower().Equals((model.Answer1.ToLower())))//case-insensitive comparison
                                                                 //&& ans2.Equals(model.Answer2))
            {
                RandomPassword rnd = new RandomPassword();
                var OTP = rnd.Generate(8);
                var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
                var OTPValidUpto = DateTime.UtcNow.AddMinutes(Convert.ToInt32(OTPValidity));
                var AspUser = db.LUsers.Find(model.UserId);
                //this is hold until new columns are added in LUsers table 
                AspUser.OTP = OTP;
                AspUser.OTPValidUpto = OTPValidUpto;
                db.Entry(AspUser).State = EntityState.Modified;
                db.SaveChanges();
                string SenderAccountName = ConfigurationManager.AppSettings["SenderAccountName"];
                //send email to user
                //int result = db.SpLogEmail(UserName, null, null, null, OTP + "is the verification Code", OTP + "is your verifcation OTP code. Please use this OTP to reset your password.Expiration is  " + OTPValidUpto,
                //    true, "Notifier", "Normal", null, "InQueue", null, Convert.ToString(model.UserId), Convert.ToString(model.UserId), SenderAccountName);
                SendMail(AspUser.LoginEmail, OTP, model.UserId,AspUser.CompanyCode);
                return Ok();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, "Incorrect Answer. Please try again."));
            }
        }

       [HttpGet]
        public IHttpActionResult GetHelpUrl(int RoleId, int MenuId)
        {
            var url = "";
            //var Query = "Exec [spGetHelpUrl] @MenuId,@RoleId,@PageUrl output";
            //using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand(Query);
            //    cmd.Connection = conn;

            //    cmd.Parameters.AddWithValue("@MenuId", MenuId);
            //    cmd.Parameters.AddWithValue("@RoleId", RoleId);

            //    cmd.Parameters.Add("@PageUrl", SqlDbType.VarChar, 255);
            //    cmd.Parameters["@PageUrl"].Direction = ParameterDirection.Output;

            //    cmd.ExecuteNonQuery();
            //    url = cmd.Parameters["@PageUrl"].Value.ToString();               
            //    conn.Close();
            //}

            ObjectParameter PageUrl = new ObjectParameter("PageUrl", typeof(string));
           

            db.spGetHelpUrl(MenuId, RoleId, PageUrl).FirstOrDefault();
            url = (string)PageUrl.Value;
            return Ok(url);
        }
      

        [HttpGet]
        [OverrideAuthorization]
        public IHttpActionResult VerifyOTP(string OTP, int UserId, string UserName, string WorkFlow)
        {
            var validity = DateTime.UtcNow;
            var User = db.LUsers.Find(UserId);
            if (OTP.Equals(User.OTP) && validity < User.OTPValidUpto)
            {
                //Invalidate OTP once it is used.
                User.OTP = "Expired";
                var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
                User.OTPValidUpto = DateTime.UtcNow.AddHours(-(Convert.ToInt32(OTPValidity)));
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid OTP. Please Try again."));
            }

        }


        private int SendMail(string ToEmail, string OTP, int UserId,string CompanyCode)
        {
            //Getting info from LEmailTemplate so that we can use EmailSubject and EmailBody from the database
            var EmailTemplate = db.LEmailTemplates.Where(p => p.TemplateName == "Forgot Password OTP").Where(aa => aa.CompanyCode == CompanyCode).FirstOrDefault();
           
            var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
            var EmailSubject = EmailTemplate.EmailSubject;
            var EmailBody = EmailTemplate.EmailBody;
            EmailBody = (EmailBody.Replace("{Email-OTP}", OTP)).Replace("{Email-OTPValidity}", OTPValidity);
           // var EmailSubject = "RELY - Forgot Password OTP";
            //var EmailBody = "As per your request, a One Time Password (OTP) has been generated and the same is " + OTP
            //    + "<br/><br/>Please use this OTP to complete the forgot password process."
            //    + "<br/><br/>Note: OTP will expire in " + OTPValidity + "mins. If expired, the transaction would have to be re-initiated and a new OTP to be generated."
            //    + "<br/><br/>RELY Support Team"
            //    + "<br/><br/><b>** This is an auto-generated email. Please do not reply to this email.**<b>"
            //    + "<br/><br/>For any queries, please contact our local support";
            //call to Stored Procedure SpLogEmail which inserts the record in LEmailBucket for sending email.
            string SenderAccountName = ConfigurationManager.AppSettings["SenderAccountName"];
            //send email to user
            int result = db.SpLogEmail(ToEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, UserId, UserId, SenderAccountName);
            return result;
        }

        [HttpGet]
        public IHttpActionResult CreateNewUser( ADModel model,string UserName, string WorkFlow)
        {
            AuthenticationResult result = new AuthenticationResult();
            result = Globals.CreateUser(model);
            if (result.IsSuccess)
            {
                return Ok(true);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, result.ErrorMessage));
            }

        }
        [HttpGet]
        public IHttpActionResult TestMail()
        {
            Globals.SendExceptionEmail("This is just to test email");
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetSelectedLandingPage(int Userid)
        {
            string Rolename = string.Empty;
            var UserDetails = db.MUserRoles.Where(p => p.UserId==Userid & p.IsDefault == true).FirstOrDefault();
            if (UserDetails != null)
            {
                Rolename = db.LRoles.Where(p => p.Id == UserDetails.RoleId).Select(p => p.RoleName).FirstOrDefault();
            }
            return Ok(Rolename);


        }
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult UpdateSelectedLandingPage(int Userid, string RoleName, string CompanyCode)
        {

            var CompanyGcCode = db.GCompanies.Where(x => x.CompanyCode == CompanyCode).Select(x => x.CompanyCode).FirstOrDefault();
            var RoleID = db.LRoles.Where(x => x.RoleName == RoleName & x.CompanyCode == CompanyGcCode).Select(x => x.Id).FirstOrDefault();

            var RoleList = db.MUserRoles.Where(p => p.UserId== Userid).ToList();
            foreach (var obj in RoleList)
            {
                obj.IsDefault = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }

            var UserRoles = db.MUserRoles.Where(p => p.UserId.Equals(Userid) & p.RoleId == RoleID).FirstOrDefault();
            UserRoles.IsDefault = true;
            db.Entry(UserRoles).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

    }
}
