using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LSupportingDocumentsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        public IHttpActionResult GetSupportingDocumentsByEntityType(string EntityType, int EntityId)
        {
            var SupportingDocument = from aa in db.LSupportingDocuments.Where(p => p.EntityType == EntityType).Where(p => p.EntityId == EntityId).Where(p => p.IsDeleted == false)
                                     join bb in db.LUsers on aa.CreatedById equals bb.Id
                                     join SS in db.WSteps on aa.StepId equals SS.Id into yG
                                     from Step in yG.DefaultIfEmpty()
                                     select new { aa.Description, aa.FileName, aa.OriginalFileName, aa.FilePath, aa.Id, bb.LoginEmail, Step.Label, aa.CreatedDateTime };
            return Ok(SupportingDocument);
        }

        public IHttpActionResult GetLSupportingDocumentById(int id)
        {
            var SupportingDocument = db.LSupportingDocuments.Where(p => p.Id == id).Where(p => p.IsDeleted == false).Select(p => new { p.Description, p.FileName, p.OriginalFileName, p.FilePath, p.Id }).FirstOrDefault();
            return Ok(SupportingDocument);
        }

        /*8. (f)  When action Delete is pressed, an audit entry is to be made when executing this action. 
         * (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit', Action = 'DeleteAttachment', ActionType = Create/Edit 
         * depending upon the mode (create/edit) in which the form is open, OldStatus, NewStatus should be same as current status of the entry, 
         * comments = "Deleted file <FileName> ", SupportindDocumentId = LSupportingDocument.Id, rest of the columns are obvious. 
           (g) When action Delete is pressed, LSupportingDocuments.IsDeleted should be set to 1*/
        public IHttpActionResult DeleteSupportingDocument(int id, string WorkFlow, int LoggedInUserId, int LoggedInRoleId, string CompanyCode)
        {
            try
            {
                var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
                var SupportingDocument = db.LSupportingDocuments.Where(p => p.Id == id).FirstOrDefault();
                var StepId = db.Database.SqlQuery<int>("select Id from WSteps where Ordinal=(select WFOrdinal from " + SupportingDocument.EntityType + " where Id={0}) and CompanyCode={1} and WorkFlowId={2}", SupportingDocument.EntityId, CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                var sqlqry = "select WFOrdinal,WFStatus,Name from " + SupportingDocument.EntityType + " where Id={0}";
                if (SupportingDocument.EntityType.Equals("LAccountingScenarios"))
                {
                    sqlqry = "select WFOrdinal,WFStatus,Reference as Name from " + SupportingDocument.EntityType + " where Id={0}";
                }
                else if (SupportingDocument.EntityType.Equals("LScenarioDemand"))
                {
                    sqlqry = "select WFOrdinal,WFStatus,QuestionCode as Name from " + SupportingDocument.EntityType + " where Id={0}";
                }
                var BaseTableColumns = db.Database.SqlQuery<WFColumnsViewModel>(sqlqry, SupportingDocument.EntityId).FirstOrDefault();
                SupportingDocument.IsDeleted = true;
                db.Entry(SupportingDocument).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "DeleteAttachment",
                             "Delete", LoggedInUserId, LoggedInRoleId, DateTime.UtcNow, BaseTableColumns.WFStatus, BaseTableColumns.WFStatus,
                             SupportingDocument.EntityType, SupportingDocument.EntityId, BaseTableColumns.Name, WorkflowDetails.Id, CompanyCode, "Deleted: " + SupportingDocument.FileName, StepId, "Delete", SupportingDocument.Id);
                db.SaveChanges();
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
            return Ok();
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message 
            if (SqEx.Message.IndexOf("FK_LSupportingDocuments_LAudit_SupportingDocumentId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "SUPPORTING DOCUMENTS", "AUDIT"));
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
