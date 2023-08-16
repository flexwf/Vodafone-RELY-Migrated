using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GSecurityQuestionsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        //GET: Method to Get Reset password Questions
        public IHttpActionResult GetGSecurityQuestions()
        {

            var xx = (from aa in db.GSecurityQuestions
                      select new
                      {
                          aa.Id,
                          aa.Question
                      });

            return Ok(xx);
        }
        ////GET: Method to Get Saved Question Answers
        public IHttpActionResult GetQuestionAnswersByUser(int userid)
        {
            var xx = (from aa in db.MLUsersGSecurityQuestions.Where(p => p.UserId == userid)
                      select new
                      {
                          aa.QuestionId,
                          aa.Answer,
                          aa.GSecurityQuestion.Question

                      }).OrderBy(p => p.QuestionId);
            return Ok(xx);
        }
        [HttpPost]
        //POST: Method to post Data into db
        public IHttpActionResult PostQuestionAnswers(MLUsersGSecurityQuestionViewModel model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var MAUGSQ = new MLUsersGSecurityQuestion();
                    MAUGSQ.UserId = model.UserId;
                    MAUGSQ.QuestionId = model.Question1;
                    MAUGSQ.Answer = model.Answer1;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.QuestionId = model.Question2;
                    MAUGSQ.Answer = model.Answer2;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.QuestionId = model.Question3;
                    MAUGSQ.Answer = model.Answer3;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }


            return Ok();



        }
        //PUT: Update Question Answer
        public IHttpActionResult PutQuestionAnswers(int userid, MLUsersGSecurityQuestionViewModel model)

        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var update = db.MLUsersGSecurityQuestions.Where(p => p.UserId == userid).ToList();


                    db.MLUsersGSecurityQuestions.RemoveRange(update);
                    db.SaveChanges();
                    var MAUGSQ = new MLUsersGSecurityQuestion();
                    MAUGSQ.UserId = model.UserId;
                    MAUGSQ.QuestionId = model.Question1;
                    MAUGSQ.Answer = model.Answer1;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.QuestionId = model.Question2;
                    MAUGSQ.Answer = model.Answer2;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.QuestionId = model.Question3;
                    MAUGSQ.Answer = model.Answer3;
                    db.MLUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }

            }
            return Ok();
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_GSecurityQuestions_MLUsersGSecurityQuestions_QuestionId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SECURITY QUESTIONS", "USERS SECURITY QUESTIONS"));
            else if (SqEx.Message.IndexOf("UQ_GSecurityQuestions_Question", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "SECURITY QUESTIONS"));
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
