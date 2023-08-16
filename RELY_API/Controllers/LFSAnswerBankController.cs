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
    public class LFSAnswerBankController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        /*-------------not required any more date 25/09/18-------------------------------*/
        //public IHttpActionResult GetConclusionText(string CompanyCode,int EntityId,string EntityType,string QuestionCode)
        //{
        //    /* String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //     SqlConnection con = new SqlConnection(strConnString);

        //     string strQuery = "select dbo.[FnGenerateConclusionText](@CompanyCode,@EntityId,@EntityType,@QuestionCode)";
        //     SqlCommand cmd = new SqlCommand(strQuery, con);
        //     cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode ?? (object)DBNull.Value);
        //     cmd.Parameters.AddWithValue("@EntityId", EntityId );
        //     cmd.Parameters.AddWithValue("@EntityType", EntityType ?? (object)DBNull.Value);
        //     cmd.Parameters.AddWithValue("@QuestionCode", QuestionCode ?? (object)DBNull.Value);
        //     con.Open();
        //     SqlDataReader dataReaderSql = cmd.ExecuteReader();
        //     string retunValue = "";
        //     while (dataReaderSql.Read())
        //     {
        //         retunValue = dataReaderSql[0].ToString();
        //         break;
        //     }
        //     dataReaderSql.Close();
        //     con.Dispose();
        //     con.Close();*/

        //    string SqlQuery = "select dbo.FnGenerateConclusionText({0},{1},{2},{3})";
        //    var retunValue = db.Database.SqlQuery<string>(SqlQuery, CompanyCode, EntityId, EntityType, QuestionCode).FirstOrDefault();
        //    //using this model as function is returning string value which needs to be sent back to APP
        //    GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = retunValue };
        //    return Ok(model);
        //}

        // GET: api/LFSAnswerBank? CompanyCode = DE
        //Method to Get Data in Grid
        public IHttpActionResult GetLFSAnswerBanksByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSAnswerBanks.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.MemoText,
                          aa.Comments,                        
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }
            // GET: api/GetLFSAnswerBank? Id
            //Method to Get Data in Grid
        public IHttpActionResult GetLFSAnswerBanksById(int Id, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LFSAnswerBanks.Where(p => p.Id == Id)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.MemoText,
                          aa.Comments,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        // GET: api/GetLFSAnswerBank? QuestionCode
        //Method to Get Data in Grid
        public IHttpActionResult GetLFSAnswerBanksByQuestionCode(string QuestionCode,string ItemTypeName, string CompanyCode, string UserName, string WorkFlow)
        {
            //if(ItemTypeName == "AS_QUESTION")
            //{
            //    string qry = "select aa.Id,aa.CompanyCode,aa.QuestionCode,AnswerOption =aa.AnswerOption + ', ' + bb.ScenarioDescription ,aa.MemoText,aa.Comments,aa.CreatedById,aa.CreatedDateTime,aa.UpdatedById,aa.UpdatedDateTime"
            //        + " from LFSAnswerBank aa left outer join LAccountingScenarios bb on bb.Reference= aa.AnswerOption where aa.QuestionCode = {0} and aa.CompanyCode = {1}";
            //    var Apidata = db.Database.SqlQuery<LFSAnswerBank>(qry,QuestionCode,CompanyCode).ToList();
            //    return Ok(Apidata);
            //}
            var xx = (from aa in db.LFSAnswerBanks.Where(p => p.QuestionCode == QuestionCode).Where(q => q.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.MemoText,
                          aa.Comments,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).ToList().OrderBy(a =>a.Id);
            return Ok(xx);
        }

        // GET: api/GetLFSAnswerBank
        //Method to Get Data in Grid
        public IHttpActionResult GetLFSAnswerBank()
        {
            var xx = (from aa in db.LFSAnswerBanks
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.QuestionCode,
                          aa.AnswerOption,
                          aa.MemoText,
                          aa.Comments,
                          aa.ScenarioCategory,
                          aa.ScenarioTrigger,
                          aa.InternalNotes,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        // POST: api/LFSAnswerBank
        //Method to Post Data from app to db
        [ResponseType(typeof(LFSAnswerBank))]
        public async Task<IHttpActionResult> PostLFSAnswerBanks(LFSAnswerBank LFSAnswerBank, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "AnswerBank")));
            }

            try
            {
                if (db.LFSAnswerBanks.Where(p => p.Id == LFSAnswerBank.Id).Count() > 0)
                {
                    db.Entry(LFSAnswerBank).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    LFSAnswerBank.Id = 0;//To override the Id generated by grid
                    db.LFSAnswerBanks.Add(LFSAnswerBank);
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
            return CreatedAtRoute("DefaultApi", new { id = LFSAnswerBank.Id }, LFSAnswerBank);
        }

        [HttpPost]
        [ResponseType(typeof(LFSAnswerBank))]
        public async Task<IHttpActionResult> POSTLFSAnswerBank(List<LFSAnswerBank> LFSAnswerBank, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LFSAnswerBank")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int LoggedInUserId = 0;
                    foreach (var model in LFSAnswerBank)
                    {
                        if (model.Id == 0)//add only when new record is there to insert.
                            db.LFSAnswerBanks.Add(model);
                        LoggedInUserId = model.UpdatedById;
                        await db.SaveChangesAsync();
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

        

        // PUT: api/LFSAnswerBank?Id
        //Method to update Requested Data by User in db
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLFSAnswerBanks(int Id, LFSAnswerBank LFSAnswerBank, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ANSWERBANK")));
            }

            if (!LFSAnswerBankExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ANSWERBANK")));
            }

            if (Id != LFSAnswerBank.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "QUESTIONBANK")));
            }
            try
            {
                db.Entry(LFSAnswerBank).State = EntityState.Modified;
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
            return Ok(LFSAnswerBank);
        }

        // Delete: api/LFSAnswerBank?id
        [ResponseType(typeof(LFSAnswerBank))]
        public async Task<IHttpActionResult> DeleteLFSAnswerBanks(int id, string UserName, string WorkFlow)
        {
            LFSAnswerBank LFSAnswerBank = await db.LFSAnswerBanks.FindAsync(id);
            if (LFSAnswerBank == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Answer Bank")));
            }

            try
            {
                db.LFSAnswerBanks.Remove(LFSAnswerBank);
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
            return Ok(LFSAnswerBank);
        }

        private bool LFSAnswerBankExists(int Id)
        {
            return db.LFSAnswerBanks.Count(e => e.Id == Id) > 0;
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LFSAnswerBank_LFSNextSteps_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "NEXT STEPS"));
            //else if (SqEx.Message.IndexOf("FK_LFSAnswerBank_MLFSAnswerBookLAccountingScenarios_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "ACCOUNTING SCENARIO"));
            //else if (SqEx.Message.IndexOf("FK_LFSAnswerBank_MLFSAnswerBankLFSSurveyLevels_AnswerId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ANSWER BANK", "SURVEY LEVELS"));
            //else if
            if
                (SqEx.Message.IndexOf("UQ_LFSAnswerBankCompanyCode_QuestionCode_AnswerOption", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "ANSWER BANK"));
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
