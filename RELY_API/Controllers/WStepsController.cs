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
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class WStepsController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/WSteps
        public IHttpActionResult GetWSteps(string UserName, string WorkFlow)
        {
            var xx = (from aa in db.WSteps
                      select new
                      {
                          aa.Id,
                          aa.Ordinal,
                          aa.CompanyCode,
                          aa.WorkFlowId,
                          aa.Name,
                          aa.Description,
                          aa.Label,
                          aa.Skip,
                          aa.SkipFunctionName,
                          aa.Banner,
                          aa.DoNotNotify,
                          aa.IsReady
                      }).OrderBy(p => p.Ordinal);
            return Ok(xx);
        }

        // GET: api/LandingStepsName
        public IHttpActionResult GetLandingStepsName(string CompanyCode,string UserName,string WorkFlow)
        {
            var xx = (from WS in db.WSteps.Where(p => p.CompanyCode == CompanyCode)
                      join RWF in db.RWorkFlows on WS.WorkFlowId equals RWF.Id
                      select new
                      {
                          WS.Id,
                          Name=RWF.Name + "-" + WS.Name 
                      }).ToList().OrderBy(p => p.Id);
            return Ok(xx);
        }
        public IHttpActionResult GetMaxOrdinalValue(int WorkFlowId, string CompanyCode, string UserName, string WorkFlow)
        {
            var maxOrdinal = db.Database.SqlQuery<int?>("Select MAX(Ordinal) from WSteps where WorkflowId={0} and CompanyCode={1}", WorkFlowId, CompanyCode).FirstOrDefault();
            return Ok(maxOrdinal);
        }

        public IHttpActionResult GetStepIdByWFIdAndOrdinal(int WFId, int OrdinalValue,string CompanyCode)
        {
            string SqlString = "select Id from WSteps where WorkFlowId={0} and Ordinal = {1} and CompanyCode = {2}";
            int StepId = db.Database.SqlQuery<int>(SqlString,WFId,OrdinalValue,CompanyCode).FirstOrDefault();
            return Ok(StepId);
        }
        // GET: api/WSteps/5
        [ResponseType(typeof(WStep))]
        public async Task<IHttpActionResult> GetWStepById(int id, string UserName, string WorkFlow)
        {

            var WStep = db.WSteps.Where(p => p.Id == id).Select(aa => new {
                aa.Id,
                aa.Ordinal,
                aa.CompanyCode,
                aa.WorkFlowId,
                aa.Name,
                aa.Description,
                aa.Label,
                aa.Skip,
                aa.SkipFunctionName,
                aa.Banner,
                aa.DoNotNotify,
                aa.IsReady
            }).FirstOrDefault();
            if (WStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            return Ok(WStep);
        }

        public async Task<IHttpActionResult> GetStepDetailsForWorkFlowInSession(int id, string UserName, string WorkFlow)
        {

            var WStep = (from WS in db.WSteps.Where(p => p.Id == id)
                         join RWF in db.RWorkFlows on WS.WorkFlowId equals RWF.Id
                         select new
                         { WS.Id,WS.WorkFlowId,WorkFlowName = RWF.Name }).FirstOrDefault();
            if (WStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            return Ok(WStep);
        }
        public async Task<IHttpActionResult> GetWStepByWFIdForDropDown(int WorkflowId, string UserName, string WorkFlow, string CompanyCode)
        {

            var WStep = db.WSteps.Where(p => p.WorkFlowId == WorkflowId && p.CompanyCode == CompanyCode).Select(aa => new {
                aa.Id,
                aa.Name,
            }).OrderBy(p => p.Name).ToList();
            if (WStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            return Ok(WStep);
        }


        [ResponseType(typeof(WStep))]
        public async Task<IHttpActionResult> GetWStepByWFId(int WorkflowId, string UserName, string WorkFlow, string CompanyCode)
        {

            var WStep = db.WSteps.Where(p => p.WorkFlowId == WorkflowId && p.CompanyCode == CompanyCode).Select(aa => new {
                aa.Id,
                aa.Ordinal,
                aa.CompanyCode,
                aa.WorkFlowId,
                aa.Name,
                aa.Description,
                aa.Label,
                aa.Skip,
                aa.SkipFunctionName,
                aa.Banner,
                aa.DoNotNotify,
                aa.IsReady
            }).OrderBy(p => p.Ordinal).ToList();
            if (WStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            return Ok(WStep);
        }

        // GET: api/WSteps/5
        [ResponseType(typeof(WStep))]
        public async Task<IHttpActionResult> GetWFColumnsFromWStep(int LoggedInUserId, int LoggedInUserRoleId, int WFId, string Action, string CompanyCode, string UserName, string WorkFlow)
        {
            var result = db.FnGetWFColumnValues(LoggedInUserId, LoggedInUserRoleId, WFId, Action, CompanyCode).
                Select(aa => new { aa.Ordinal, aa.Name }).FirstOrDefault();

            if (result == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            return Ok(result);
        }
        // PUT: api/WSteps/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWStep(int id, WStep WStep, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            if (!WStepExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }

            if (id != WStep.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "STEP")));
            }
            try
            {
                db.Entry(WStep).State = EntityState.Modified;
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
            return Ok(WStep);
        }
        
        // POST: api/WSteps
       // [ResponseType(typeof(WStep))]
       [HttpPost]     
        public async Task<IHttpActionResult> PostWStep(WStep model, string UserName,string WorkFlow)
        {     
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "STEP")));
            }
            try
            {
                if (db.WSteps.Where(p => p.Id == model.Id).Where(p => p.WorkFlowId == model.WorkFlowId).Count() > 0)
                {
                    //model might have been updated while inserting new entry, so updated Ordinal value is preserved in model.
                    var xx = db.WSteps.Where(p => p.Id == model.Id).Where(p=>p.WorkFlowId == model.WorkFlowId).FirstOrDefault();
                    var ExistingOrdinal = xx.Ordinal;
                    db.Entry(xx).State = EntityState.Detached;
                    await db.SaveChangesAsync();
                    model.Ordinal = ExistingOrdinal;
                    db.Entry(model).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    // var ExistingOrdinal = db.WSteps.Find(model.Id).Ordinal;


                    /*===============Commented by RS ON 25TH SEPT 2018 as per requiremnt==========================================================================*/
                    /*var Query = "Exec [SpInsertDeleteWFStep] @CompanyCode,@WorkFlowId, @InsertOrdinal,@DeleteOrdinal,@OperationMode,@IsReady,@Name,@Label,@dNotNotify,@Banner,@Description,@Result output";
                    using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(Query);
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@CompanyCode", model.CompanyCode);
                        cmd.Parameters.AddWithValue("@WorkFlowId", model.WorkFlowId);
                        cmd.Parameters.AddWithValue("@InsertOrdinal", model.Ordinal);
                        cmd.Parameters.AddWithValue("@DeleteOrdinal", 0);
                        cmd.Parameters.AddWithValue("@OperationMode", "Insert");
                        cmd.Parameters.AddWithValue("@IsReady", model.IsReady);
                        cmd.Parameters.AddWithValue("@Name", model.Name);
                        cmd.Parameters.AddWithValue("@label", model.Label);
                        cmd.Parameters.AddWithValue("@dNotNotify", model.DoNotNotify);
                        cmd.Parameters.AddWithValue("@Banner", model.Banner ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);

                        cmd.Parameters.Add("@Result", SqlDbType.VarChar,255);
                        cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        var  message = cmd.Parameters["@Result"].Value.ToString();
                        if (message.Contains("Id="))
                        {
                            model.Id = Convert.ToInt32(message.Substring(3));
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error
                        }
                        conn.Close();

                    }
                    */

                    ObjectParameter Result = new ObjectParameter("Result", typeof(string)); //return parameter is declared                   
                    db.SpInsertDeleteWFStep(model.CompanyCode, model.WorkFlowId, model.Ordinal, 0, "Insert", Convert.ToInt16(model.IsReady), model.Name, model.Label, Convert.ToInt16(model.DoNotNotify), model.Banner, model.Description, Result).FirstOrDefault();
                    var message = (string)Result.Value;
                    if (message.Contains("Id="))
                    {
                        model.Id = Convert.ToInt32(message.Substring(3));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error
                    }

                    
                    //model.Id = 0;//To override the Id generated by grid
                    ////find out Steps which are eligible for updation, whose Ordinal value is greater than equal to new ordinal value
                    //var StepsForUpdations =  db.WSteps.Where(aa => aa.WorkFlowId == model.WorkFlowId ).Where(aa=>aa.Ordinal >= model.Ordinal ).ToList();
                    ////update ordinal value to +1
                    //foreach (var step in StepsForUpdations)
                    //{
                    //    step.Ordinal++;
                    //    db.Entry(step).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                    //db.WSteps.Add(model);
                    //await db.SaveChangesAsync();
                    //WStep model = new WStep {
                    //    Name ="t",
                    //    Label = "t",
                    //    Id =285,
                    //    Ordinal =1,
                    //    Skip =false,
                    //    IsReady =false,
                    //    Description ="des",
                    //    DoNotNotify =false,
                    //    CompanyCode ="DE",
                    //    WorkFlowId =12,
                    //    Banner= "t"
                    //};
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

        // DELETE: api/WSteps/5


       
        [ResponseType(typeof(WStep))]
        public async Task<IHttpActionResult> DeleteById(int id,int WorkFlowId, string UserName, string WorkFlow)
        {
            WStep WStep = db.WSteps.Where(a=>a.Id == id).Where(a=>a.WorkFlowId == WorkFlowId).FirstOrDefault();
            var message = "";
            if (WStep == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "STEP")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    // var Query = "Exec [SpInsertDeleteWFStep] @CompanyCode,@WorkFlowId, @InsertOrdinal,@DeleteOrdinal,@OperationMode,@IsReady,@Name,@Label,@Result output";


                    //---------------------------------------Below code is commented by RS as per requirement on 25th sept 2018----------------------------------------
                    var Query = "Exec [SpInsertDeleteWFStep] @CompanyCode,@WorkFlowId, @InsertOrdinal,@DeleteOrdinal,@OperationMode,@IsReady,@Name,@Label,@dNotNotify,@Banner,@Description,@Result output";
                    using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(Query);
                        cmd.Connection = conn;
                        cmd.Parameters.AddWithValue("@CompanyCode", WStep.CompanyCode);
                        cmd.Parameters.AddWithValue("@WorkFlowId", WStep.WorkFlowId);
                        cmd.Parameters.AddWithValue("@InsertOrdinal", 0);
                        cmd.Parameters.AddWithValue("@DeleteOrdinal", WStep.Ordinal);
                        cmd.Parameters.AddWithValue("@OperationMode", "Delete");
                        cmd.Parameters.AddWithValue("@IsReady", WStep.IsReady);
                        cmd.Parameters.AddWithValue("@Name", WStep.Name);
                        cmd.Parameters.AddWithValue("@label", WStep.Label);
                        cmd.Parameters.AddWithValue("@dNotNotify", WStep.DoNotNotify);
                        cmd.Parameters.AddWithValue("@Banner", WStep.Banner ?? (object)DBNull.Value);                       
                        cmd.Parameters.AddWithValue("@Description", WStep.Description ?? (object)DBNull.Value);
                        cmd.Parameters.Add("@Result", SqlDbType.VarChar, 255);
                        cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        message = cmd.Parameters["@Result"].Value.ToString();

                        if (message == "Successfully Deleted")
                        {
                            message = "Successfully Deleted";
                        }
                        else
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error
                        }
                        conn.Close();

                    }
                    

                    //List<WStepParticipant> wStepParticipant = db.WStepParticipants.Where(a => a.WStepId == id).ToList();
                    ////1. deleting all the WStepParticipantActions
                    //foreach (var SP in wStepParticipant)
                    //{
                    //    List<WStepParticipantAction> wStepParticipantAction =
                    //        db.WStepParticipantActions.Where(a => a.ParticipantId == SP.ParticipantId).Where(a => a.ShowInStepId == SP.WStepId).ToList();
                    //    db.WStepParticipantActions.RemoveRange(wStepParticipantAction);
                    //    await db.SaveChangesAsync();
                    //}

                    ////2. deleting all the WStepParticipants
                    //db.WStepParticipants.RemoveRange(wStepParticipant);
                    //await db.SaveChangesAsync();

                    ////3. deleting all the WStepGridColumns
                    //List<WStepGridColumn> wStepGridColumn = db.WStepGridColumns.Where(a => a.WStepId == id).ToList();
                    //db.WStepGridColumns.RemoveRange(wStepGridColumn);
                    //await db.SaveChangesAsync();


                    ////find out Steps which are eligible for updation, whose Ordinal value is greater than equal to deleting ordinal value
                    //var StepsForUpdations = db.WSteps.Where(aa => aa.WorkFlowId == WStep.WorkFlowId).Where(aa => aa.Ordinal >= WStep.Ordinal).ToList();
                    ////update ordinal value to -1
                    //foreach (var step in StepsForUpdations)
                    //{
                    //    step.Ordinal--;
                    //    db.Entry(step).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}
                    ////4. deleting the WStep
                    //db.WSteps.Remove(WStep);
                    //await db.SaveChangesAsync();

                    //transaction.Commit();                   
                    /*ObjectParameter Result = new ObjectParameter("Result", typeof(string)); //return parameter is declared
                    db.SpInsertDeleteWFStep(WStep.CompanyCode, WStep.WorkFlowId,0, WStep.Ordinal, "Delete", Convert.ToInt16(WStep.IsReady), WStep.Name, WStep.Label, Convert.ToInt16(WStep.DoNotNotify), WStep.Banner, WStep.Description, Result).FirstOrDefault();
                    message = (string)Result.Value;
                    if (message == "Successfully Deleted")
                    {
                        message = "Successfully Deleted";
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error
                    }*/

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
                        throw ex;
                    }
                }
            }
            return Ok(message);          
        }
        [HttpGet]
        public IHttpActionResult SwapStepOrdinals( int WorkFlowId,int Ordinal1,int Ordinal2,string CompanyCode, string UserName, string WorkFlow)
        {
            string message = "";
                try 
                {
   //---------------------------------------Below code is commented by RS as per requirement on 25th sept 2018---------------------------------------
                /*  var Query = "Exec [SpSwapStepOrdinals] @CompanyCode,@WorkFlowId, @Ordinal1,@Ordinal2,@Result Output";
                  using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                  {
                      conn.Open();
                      SqlCommand cmd = new SqlCommand(Query);
                      cmd.Connection = conn;
                      cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                      cmd.Parameters.AddWithValue("@WorkFlowId", WorkFlowId);
                      cmd.Parameters.AddWithValue("@Ordinal1", Ordinal1);
                      cmd.Parameters.AddWithValue("@Ordinal2", Ordinal2);
                      cmd.Parameters.Add("@Result", SqlDbType.VarChar, 255);
                      cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
                      cmd.ExecuteNonQuery();
                      message = cmd.Parameters["@Result"].Value.ToString();//getting value of output parameter

                      if (message != "Success")
                          throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error

                      conn.Close();
                  }*/

                    ObjectParameter Result = new ObjectParameter("Result", typeof(string)); //return parameter is declared
                    db.SpSwapStepOrdinals(CompanyCode, WorkFlowId, Ordinal1, Ordinal2, Result).FirstOrDefault();
                    message = (string)Result.Value; //getting value of output parameter
                    if (message != "Success")
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, message));//type 2 error

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
            return Ok(message);
        }

        private bool WStepExists(int id)
        {
            return db.WSteps.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message FK_WSteps_MStepRoles_StepId
            if (SqEx.Message.IndexOf("FK_WSteps_WStepParticipants_WStepId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "STEPS", "STEP PARTICIPANTS"));
            else if (SqEx.Message.IndexOf("UQ_WSteps_CompanyCode_Name_WorkFlowId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "STEPS"));
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
