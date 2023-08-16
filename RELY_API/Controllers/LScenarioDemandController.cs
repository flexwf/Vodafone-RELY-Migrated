using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RELY_API.Controllers
{
    public class LScenarioDemandController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        public IHttpActionResult GetById(int Id)
        {
            var xx = db.LScenarioDemands.Where(p => p.Id == Id).FirstOrDefault();
            return Ok(xx);
        }
        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var xx = db.LScenarioDemands.Where(p => p.CompanyCode == CompanyCode).ToList();
            return Ok(xx);
        }
        public async Task<IHttpActionResult> PostData(LScenarioDemand model, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    
                    var Action = db.WActions.Where(p => p.Name == "Create").FirstOrDefault();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                    model.Id = 0;
                    db.LScenarioDemands.Add(model);
                    await db.SaveChangesAsync();
                    //string fixedComments = "";
                    //string AuditComments = fixedComments + " (Status " + model.WFStatus + ") " + model.WFComments;

                    //Comment format updated - 10Oct2018- Andre raised bug for duplicacy of information.
                    string AuditComments = model.WFComments;
                    var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", model.WFOrdinal, model.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Create",
                           "Create", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                           "LScenarioDemand", model.Id, model.QuestionCode, WorkflowDetails.Id, model.CompanyCode, AuditComments, StepId, Action.Label, null);
                    db.SaveChanges();

                    //add supporting documents
                    if (!string.IsNullOrEmpty(FileList))
                    {
                        var OriginalFileArray = OriginalFileNameList.Split(',').ToList();
                        var FileArray = FileList.Split(',').ToList();
                        List<string> DescriptionArray = null;
                        if (!string.IsNullOrEmpty(SupportingDocumentsDescription))
                        {
                            DescriptionArray = SupportingDocumentsDescription.Split(',').ToList();
                        }
                        for (var i = 0; i < FileArray.Count(); i++)
                        {//Move File Over S3
                            var Source = FilePath + "/" + FileArray[i];
                            var Destination = "/" + model.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                            var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                            var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                            if (sucess)
                                FilePath = Destination;

                            var SupportingDocument = new LSupportingDocument
                            {
                                StepId = StepId,
                                FileName = FileArray[i],
                                OriginalFileName = OriginalFileArray[i],
                                FilePath = FilePath,
                                EntityId = model.Id,
                                EntityType = "LScenarioDemand",
                                CreatedById = (int)model.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = (int)model.UpdatedById,
                                CreatedByRoleId = model.WFRequesterRoleId.Value,
                                CreatedDateTime = DateTime.UtcNow,
                                UpdatedDateTime = DateTime.UtcNow,
                                //Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                                Description = (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                            };
                            db.LSupportingDocuments.Add(SupportingDocument);
                            db.SaveChanges();
                            /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                             * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                             * NewStatus should be same as current status of the entry, comments = 
                             * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                            = LSupportingDocument.Id, rest of the columns are obvious.*/
                            db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                           "Create", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                           "LScenarioDemand", model.Id, model.QuestionCode, WorkflowDetails.Id, model.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }
                    MEntityPortfolio MEP = new MEntityPortfolio();
                    MEP.EntityId = model.Id;
                    MEP.EntityType = "LScenarioDemand";
                    int PortfolioId = db.LPortfolios.Where(m => m.CompanyCode == model.CompanyCode).FirstOrDefault().Id;
                    MEP.PortfolioId = PortfolioId;
                    db.MEntityPortfolios.Add(MEP);
                    db.SaveChanges();
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
            return Ok(model.Id);

        }


        [HttpPut]
        public async Task<IHttpActionResult> PutData(int id, LScenarioDemand model, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {

            if (!LScenarioDemandExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AccountingScenario")));
            }

            if (id != model.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "AccountingScenario")));
            }
            try
            {

                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var Action = db.WActions.Where(p => p.Name == "Edit").FirstOrDefault();

                //Comments settings
                //string fixedComments = "";
                //Comment format updated - 10Oct2018- Andre raised bug for duplicacy of information.
                //string AuditComments = fixedComments + " (Status " + model.WFStatus + ") " + model.WFComments;
                string AuditComments = model.WFComments;
                var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", model.WFOrdinal, model.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                       "Update", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                       "LScenarioDemand", model.Id, model.QuestionCode, WorkflowDetails.Id, model.CompanyCode, AuditComments, StepId, Action.Label, null);
                db.SaveChanges();

                //add supporting documents
                if (!string.IsNullOrEmpty(FileList))
                {
                    var OriginalFileArray = OriginalFileNameList.Split(',').ToList();
                    var FileArray = FileList.Split(',').ToList();
                    List<string> DescriptionArray = null;
                    if (!string.IsNullOrEmpty(SupportingDocumentsDescription))
                    {
                        DescriptionArray = SupportingDocumentsDescription.Split(',').ToList();
                    }
                    for (var i = 0; i < FileArray.Count(); i++)
                    {
                        //Move File Over S3
                        var Source = FilePath + "/" + FileArray[i];
                        var Destination = "/" + model.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                        var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                        var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                        if (sucess)
                            FilePath = Destination;

                        var SupportingDocument = new LSupportingDocument
                        {
                            StepId = StepId,
                            FileName = FileArray[i],
                            OriginalFileName = OriginalFileArray[i],
                            FilePath = FilePath,
                            EntityId = model.Id,
                            EntityType = "LScenarioDemand",
                            CreatedById = (int)model.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                            UpdatedById = (int)model.UpdatedById,
                            CreatedByRoleId = model.WFRequesterRoleId.Value,
                            CreatedDateTime = DateTime.UtcNow,
                            UpdatedDateTime = DateTime.UtcNow,
                            //Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                            Description = (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                        };
                        db.LSupportingDocuments.Add(SupportingDocument);
                        db.SaveChanges();
                        /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                         * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                         * NewStatus should be same as current status of the entry, comments = 
                         * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                        = LSupportingDocument.Id, rest of the columns are obvious.*/
                        db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                       "Create", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                       "LScenarioDemand", model.Id, model.QuestionCode, WorkflowDetails.Id, model.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                        db.SaveChanges();
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

            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(model);
        }

        private bool LScenarioDemandExists(int id)
        {
            return db.LScenarioDemands.Count(e => e.Id == id) > 0;
        }


        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LScenarioDemands_LASLifeCycleEvents_AccountingScenarioId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ACCOUNTING SCENARIOS", "LIFE CYCLE EVENTS"));
            else if (SqEx.Message.IndexOf("[UQ_LScenarioDemand_Reference]", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "ACCOUNTING SCENARIO"));
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
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3rd index position
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
