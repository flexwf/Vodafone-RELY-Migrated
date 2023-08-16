using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LRequestsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //Database function is created for the same purpose
        //public IHttpActionResult GetSurveyCompletionStatus(int RequestId , string CompanyCode)
        //{

        //    int ProductCount = 0;
        //    int totalProductPercentage = 0;
        //    //find out all the Products associated with the Request
        //    List<LProduct> ProductList = db.LProducts.Where(a => a.RequestId == RequestId).Where(a => a.CompanyCode == CompanyCode).ToList();
        //    //loop through the Products associated and calculate the % completed for that Product
        //    foreach(var Product in ProductList)
        //    {
        //        int ProductCompletionPercentage = 0;
        //        //int Id = Convert.ToInt32(ProductId);
        //        ProductCount++;
        //        //find out Scetion details so as to get SectionCode and ChapterCode
        //        int? SurveyId = Product.SurveyId;
        //        if (SurveyId != null)
        //        {
        //            var SectionCount = 0;
        //            var SectionData = db.Database.SqlQuery<LFSSection>("Select * from LFSSections where SurveyId ={0}", SurveyId).ToList();
        //            //looping through the multiple Sections
        //            foreach (var Section in SectionData)
        //            {
        //                //call to function FnGetProductCompletionStatus(@SurveyId,@EntityId,@EntityType,@CompanyCode,@ChapterCode,@SectionCode)
        //                var result = db.Database.SqlQuery<int>("select dbo.FnGetProductCompletionStatus({0},{1},{2},{3},{4},{5})", SurveyId,Product.Id,"LProducts",CompanyCode, Section.ChapterCode,Section.SectionCode).FirstOrDefault();
        //                ProductCompletionPercentage+= result;
        //                SectionCount++;
        //            }
        //            int ProductPercentage = ProductCompletionPercentage / SectionCount;
        //            totalProductPercentage += ProductPercentage;
        //        }
        //    }

        //    //Calculate overall %
        //    int PercentageStatus = totalProductPercentage / ProductCount;

        //    return Ok(PercentageStatus);
        //}

        public IHttpActionResult GetSurveySummary(int? SurveyId, int EntityId, string EntityType)
        {
            string strQuery = "Exec SpGenerateSurveySummary @SurveyId,@EntityId,@EntityType";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@SurveyId", SurveyId);
            cmd.Parameters.AddWithValue("@EntityId", EntityId);
            cmd.Parameters.AddWithValue("@EntityType", EntityType);
            DataTable dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }



        public IHttpActionResult GetRequestNamesByOpco(string CompanyCode, string UserName, string WorkFlow)
        {
            List<string> names = (from aa in db.LRequests.Where(r => r.CompanyCode == CompanyCode).ToList()
                                  select aa.Name).ToList();
            return Ok(names);
        }

        public IHttpActionResult GetWFDetails(int WorkFlowId, string CompanyCode)
        {
            var WorkflowDetails = (from aa in db.RWorkFlows.Where(r => r.Id == WorkFlowId).Where(r => r.CompanyCode == CompanyCode)
                                   select new { aa.Id, aa.WFType, aa.UILabel, aa.Name, aa.CRAllowed, aa.CRWFName, aa.BaseTableName })
                .FirstOrDefault();
            return Ok(WorkflowDetails);
        }
        public IHttpActionResult GetByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var sLRequest = (from aa in db.LRequests.Where(r => r.CompanyCode == CompanyCode)
                             join yy in db.RRequestSystems on aa.SystemId equals yy.Id
                             select new
                             {
                                 aa.Id,
                                 aa.CompanyCode,
                                 aa.SystemId,
                                 aa.AuthorizationNumber,
                                 //aa.Date,
                                 aa.Description,
                                 //aa.Notes,
                                 aa.Name,
                                 aa.SurveyId,
                                 aa.AttributeC01,
                                 aa.AttributeC02,
                                 aa.AttributeC03,
                                 aa.AttributeC04,
                                 aa.AttributeC05,
                                 aa.AttributeC06,
                                 aa.AttributeC07,
                                 aa.AttributeC08,
                                 aa.AttributeC09,
                                 aa.AttributeC10,
                                 aa.AttributeN01,
                                 aa.AttributeN02,
                                 aa.AttributeN03,
                                 aa.AttributeN04,
                                 aa.AttributeN05,
                                 aa.AttributeN06,
                                 aa.AttributeN07,
                                 aa.AttributeN08,
                                 aa.AttributeN09,
                                 aa.AttributeN10,
                                 aa.AttributeB01,
                                 aa.AttributeB02,
                                 aa.AttributeB03,
                                 aa.AttributeB04,
                                 aa.AttributeB05,
                                 aa.AttributeD01,
                                 aa.AttributeD02,
                                 aa.AttributeD03,
                                 aa.AttributeD04,
                                 aa.AttributeD05,
                                 aa.WFOrdinal,
                                 aa.WFStatus,
                                 aa.WFType,
                                 aa.WFComments,
                                 aa.WFRequesterId,
                                 aa.WFAnalystId,
                                 aa.WFManagerId,
                                 aa.WFCurrentOwnerId,
                                 aa.WFRequesterRoleId,
                                 aa.CreatedById,
                                 aa.CreatedDateTime,
                                 aa.UpdatedById,
                                 aa.UpdatedDateTime,
                                 SystemName = yy.Name,
                                 aa.WFStatusDateTime
                             }).OrderByDescending(aa => aa.UpdatedDateTime);
            if (sLRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LREQUEST")));
            }
            return Ok(sLRequest);
        }

        [ResponseType(typeof(LRequest))]
        public async Task<IHttpActionResult> GetById(int Id, string UserName, string WorkFlow)
        {
            var LRequest = (from aa in db.LRequests.Where(r => r.Id == Id)
                            join yy in db.RRequestSystems on aa.SystemId equals yy.Id into tmpRSys
                            from yy in tmpRSys.DefaultIfEmpty()
                            select new
                            {
                                aa.Id,
                                aa.Name,
                                aa.CompanyCode,
                                aa.SystemId,
                                aa.SurveyId,
                                aa.AuthorizationNumber,
                                SystemName = yy.Name,
                                aa.Description,
                                aa.AttributeC01,
                                aa.AttributeC02,
                                aa.AttributeC03,
                                aa.AttributeC04,
                                aa.AttributeC05,
                                aa.AttributeC06,
                                aa.AttributeC07,
                                aa.AttributeC08,
                                aa.AttributeC09,
                                aa.AttributeC10,
                                aa.AttributeI01,
                                aa.AttributeI02,
                                aa.AttributeI03,
                                aa.AttributeI04,
                                aa.AttributeI05,
                                aa.AttributeN01,
                                aa.AttributeN02,
                                aa.AttributeN03,
                                aa.AttributeN04,
                                aa.AttributeN05,
                                aa.AttributeN06,
                                aa.AttributeN07,
                                aa.AttributeN08,
                                aa.AttributeN09,
                                aa.AttributeN10,
                                aa.AttributeB01,
                                aa.AttributeB02,
                                aa.AttributeB03,
                                aa.AttributeB04,
                                aa.AttributeB05,
                                aa.AttributeD01,
                                aa.AttributeD02,
                                aa.AttributeD03,
                                aa.AttributeD04,
                                aa.AttributeD05,
                                aa.WFOrdinal,
                                aa.WFStatus,
                                aa.WFType,
                                aa.WFComments,
                                aa.WFRequesterId,
                                aa.WFAnalystId,
                                aa.WFManagerId,
                                aa.WFCurrentOwnerId,
                                aa.WFRequesterRoleId,
                                aa.CreatedById,
                                aa.CreatedDateTime,
                                aa.UpdatedById,
                                aa.UpdatedDateTime,
                                aa.WFStatusDateTime
                            }).FirstOrDefault();
            if (LRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Request")));
            }
            return Ok(LRequest);
        }

        // PUT: api/LRequests/5
        [ResponseType(typeof(LRequest))]
        public async Task<IHttpActionResult> PutLRequest(LRequest LRequest, int Id, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {

            //getting and setting CreatedDateTime for the model as this was conflicting with the JS date
            var req = db.LRequests.Find(Id);
            var CreatedDateTime = req.CreatedDateTime;
            db.Entry(req).State = EntityState.Detached;
            await db.SaveChangesAsync();
            LRequest.CreatedDateTime = CreatedDateTime;
            //This will convert String.Empty comment to null again as we cannot send null in restclient
            if (String.IsNullOrEmpty(SupportingDocumentsDescription))
            {
                SupportingDocumentsDescription = null;
            }
            if (!LRequestExists(Id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REQUEST")));
            }

            if (Id != LRequest.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "REQUEST")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    //Get Created DateTime
                    // var ExistingCreatedDateTime = db.LRequests.Where(p => p.Id == Id).FirstOrDefault().CreatedDateTime;//This is a workaround to resolve issue in which created DateTime is not obtained from post method in form
                    var Action = db.WActions.Where(p => p.Name == "Edit").FirstOrDefault();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == LRequest.CompanyCode).FirstOrDefault();
                    //  LRequest.CreatedDateTime = ExistingCreatedDateTime;
                    db.Entry(LRequest).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //Updating #(if exist) with Request Id 
                    db.Entry(req).State = EntityState.Detached;
                    await db.SaveChangesAsync();
                    if (LRequest.Name.Contains("#"))
                    {
                        LRequest.Name = LRequest.Name.Replace("#", LRequest.Id.ToString());
                    }
                    if (!string.IsNullOrEmpty(LRequest.AuthorizationNumber))
                    {
                        if (LRequest.AuthorizationNumber.EndsWith("#"))
                        {
                            LRequest.AuthorizationNumber = LRequest.AuthorizationNumber.Replace("#", LRequest.Id.ToString());
                        }
                    }
                    db.Entry(LRequest).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", LRequest.WFOrdinal, LRequest.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    //Comments settings
                    //string fixedComments = "";
                    //if (!string.IsNullOrEmpty(LRequest.AttributeC20))
                    //{

                    //    if (LRequest.AttributeC20.Equals("Edit"))
                    //    {
                    //        fixedComments = "Edited";
                    //    }
                    //}
                    //string AuditComments = fixedComments + " (Status " + LRequest.WFStatus + ") " + LRequest.WFComments;
                    string AuditComments = LRequest.WFComments;
                    //Comment foromat updated - 10Oct2018- Andre raised bug for duplicacy of information.
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                           "Update", LRequest.UpdatedById, LRequest.WFRequesterRoleId, DateTime.UtcNow, LRequest.WFStatus, LRequest.WFStatus,
                           "LRequests", LRequest.Id, LRequest.Name, WorkflowDetails.Id, LRequest.CompanyCode, AuditComments, StepId, Action.Label, null);
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
                            var Destination = "/" + LRequest.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
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
                                EntityId = LRequest.Id,
                                EntityType = "LRequests",
                                CreatedById = LRequest.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = LRequest.UpdatedById,
                                CreatedByRoleId = LRequest.WFRequesterRoleId.Value,
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
                            var StepIdForDoc = db.Database.SqlQuery<int>("select Id from WSteps where Ordinal=(select WFOrdinal from " + SupportingDocument.EntityType + " where Id={0}) and CompanyCode={1} and WorkFlowId={2}", SupportingDocument.EntityId, LRequest.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                            db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                           "Create", LRequest.UpdatedById, LRequest.WFRequesterRoleId, DateTime.UtcNow, LRequest.WFStatus, LRequest.WFStatus,
                           "LRequests", LRequest.Id, LRequest.Name, WorkflowDetails.Id, LRequest.CompanyCode, SupportingDocument.Description, StepIdForDoc, Action.Label, SupportingDocument.Id);
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
            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(LRequest.Id);
        }

        [HttpPost]
        // POST: api/LRequests
        [ResponseType(typeof(LRequest))]
        public async Task<IHttpActionResult> PostLRequest(LRequest LRequest, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                //This will convert String.Empty comment to null again as we cannot send null in restclient
                if (String.IsNullOrEmpty(SupportingDocumentsDescription))
                {
                    SupportingDocumentsDescription = null;
                }
                try
                {
                    var Action = db.WActions.Where(p => p.Name == "Create").FirstOrDefault();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == LRequest.CompanyCode).FirstOrDefault();

                    db.LRequests.Add(LRequest);
                    await db.SaveChangesAsync();
                    if (LRequest.Name.EndsWith("#"))
                    {
                        LRequest.Name = LRequest.Name.Replace("#", LRequest.Id.ToString());
                    }
                    if (!string.IsNullOrEmpty(LRequest.AuthorizationNumber))
                    {
                        if (LRequest.AuthorizationNumber.EndsWith("#"))
                        {
                            LRequest.AuthorizationNumber = LRequest.AuthorizationNumber.Replace("#", LRequest.Id.ToString());
                        }
                    }
                    db.Entry(LRequest).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", LRequest.WFOrdinal, LRequest.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    //string fixedComments = "";
                    //if (!string.IsNullOrEmpty(LRequest.AttributeC20))
                    //{

                    //    if (LRequest.AttributeC20.Equals("Change"))
                    //    {
                    //        fixedComments = "Opened for Changes";
                    //    }
                    //    else if (LRequest.AttributeC20.Equals("Create"))
                    //    {
                    //        fixedComments = "Created the Request";
                    //    }
                    //}
                    //string AuditComments = fixedComments + " (Status " + LRequest.WFStatus + ") " + LRequest.WFComments;
                    //Comment foromat updated - 10Oct2018- Andre raised bug for duplicacy of information.
                    string AuditComments = LRequest.WFComments;
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Create",
                           "Create", LRequest.UpdatedById, LRequest.WFRequesterRoleId, DateTime.UtcNow, LRequest.WFStatus, LRequest.WFStatus,
                           "LRequests", LRequest.Id, LRequest.Name, WorkflowDetails.Id, LRequest.CompanyCode, AuditComments, StepId, Action.Label, null);
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
                            var Destination = "/" + LRequest.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
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
                                EntityId = LRequest.Id,
                                EntityType = "LRequests",
                                CreatedById = LRequest.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = LRequest.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                CreatedByRoleId = LRequest.WFRequesterRoleId.Value,
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
                            var StepIdForDoc = db.Database.SqlQuery<int>("select Id from WSteps where Ordinal=(select WFOrdinal from " + SupportingDocument.EntityType + " where Id={0}) and CompanyCode={1} and WorkFlowId={2}", SupportingDocument.EntityId, LRequest.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                            db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                           "Create", LRequest.UpdatedById, LRequest.WFRequesterRoleId, DateTime.UtcNow, LRequest.WFStatus, LRequest.WFStatus,
                           "LRequests", LRequest.Id, LRequest.Name, WorkflowDetails.Id, LRequest.CompanyCode, SupportingDocument.Description, StepIdForDoc, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }

                    MEntityPortfolio MEP = new MEntityPortfolio();
                    MEP.EntityId = LRequest.Id;
                    MEP.EntityType = "LRequests";
                    int PortfolioId = db.LPortfolios.Where(m => m.CompanyCode == LRequest.CompanyCode).FirstOrDefault().Id;
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
            return Ok(LRequest.Id);
        }

        // DELETE: api/LRequests/5
        [ResponseType(typeof(LProduct))]
        public async Task<IHttpActionResult> DeleteLRequest(int Id, string UserName, string WorkFlow)
        {
            LRequest LRequest = await db.LRequests.FindAsync(Id);
            if (LRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Request")));
            }

            try
            {
                db.LRequests.Remove(LRequest);
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
                    //Question for Shivani - why not systematic error handling here?
                    throw ex;
                }
            }
            return Ok(LRequest);
        }


        private bool LRequestExists(int Id)
        {
            return db.LRequests.Count(e => e.Id == Id) > 0;
        }

        [HttpGet]
        //This Method will check the feasibility of Accounting Memo
        public IHttpActionResult CheckFeasibilityOfAccMemo(int RequestId)
        {
            var Response = db.Database.SqlQuery<string>("select dbo.[FnCheckAccountingMemoFeasibility]({0})", RequestId).FirstOrDefault();
            return Ok(Response);
        }

        [HttpGet]
        public IHttpActionResult GetRequestLevelAccMemo(int RequestId, int LoggedInUserId, string CompanyCode)
        {
            try
            {
                //Task objThisTask = Task.Factory.StartNew(() =>
                //{
                string FileName = "";
                DataTable dt = new DataTable();
                string strQuery = "Exec SpConsolidatedAccountingMemo @RequestId,@LoggedInUserId,@IsConsolidatedAccountingMemo";
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.AddWithValue("@RequestId", RequestId);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@IsConsolidatedAccountingMemo", 1);
                dt = GetData(cmd);

                //We get ProductList here to show it in header of the document but later we made a arrangement in the Stored Procedure. So currnetly not in use
                // string ProductList= GetProductListByRequestId(RequestId);

                //Getting RequestType,AuthorizationNumber and RequestName from database for making the filename
                GetRequestDataViewModel RequestData = GetDataByRequestId(RequestId);
                var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"];

                string TemplateName = "WDTemplate.docx";
                string path = ConfigurationManager.AppSettings["RelyTempPath"];
                string WordTemplatePath = path + "\\consolidatedaccountingmemo\\" + TemplateName;

                if (!String.IsNullOrEmpty(RequestData.AuthorizationNumber))
                {
                    FileName = "AM " + RequestData.SystemName + " " + RequestData.AuthorizationNumber + " " + RequestData.Name + " " + DateTime.UtcNow.ToString("yyyy-MM-dd - hhmmss") + ".docx";
                }
                else
                {
                    FileName = "AM " + RequestData.SystemName + " " + RequestData.Name + " " + DateTime.UtcNow.ToString("yyyy-MM-dd - hhmmss") + ".docx";
                }


                string Result = Globals.ExportWordSample(dt, TempFileFolder, FileName, WordTemplatePath);
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/downloads/consolidatedaccountingmemo/" + FileName;



                Globals.UploadFileToS3(TempFileFolder + "/" + FileName, S3TargetPath);

                // });
                return Ok(FileName);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, ex.Message));//type 1 error
            }
        }
        private GetRequestDataViewModel GetDataByRequestId(int RequestId)
        {

            var LRequest = (from aa in db.LRequests.Where(r => r.Id == RequestId)
                            join yy in db.RRequestSystems on aa.SystemId equals yy.Id into tmpRSys
                            from yy in tmpRSys.DefaultIfEmpty()
                            select new
                            {
                                aa.Name,
                                aa.AuthorizationNumber,
                                SystemName = yy.Name


                            }).FirstOrDefault();


            GetRequestDataViewModel obj = new GetRequestDataViewModel();
            obj.Name = LRequest.Name;
            obj.AuthorizationNumber = LRequest.AuthorizationNumber;
            obj.SystemName = LRequest.SystemName;
            return (obj);
        }


        //private string GetProductListByRequestId(int RequestId)
        //{

        //    var LProducts = (from aa in db.LProducts.Where(r => r.RequestId == RequestId)

        //                     select new
        //                     {
        //                         aa.Name

        //                     }).ToList();

        //    string ProductNames="";
        //    foreach (var name in LProducts)
        //    {
        //        ProductNames = ProductNames + "," + name;
        //    }
        //    ProductNames = ProductNames.Substring(1, ProductNames.Length - 1);
        //    return (ProductNames);
        //}

        //Thios function will fill DataTable by cmd
        private DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].
            ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.CommandTimeout = 600;
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                //Globals.DebugEntry("Consolidated RowCount from C# code=" + (dt.Rows.Count).ToString());
                con.Close();
                sda.Dispose();
                con.Dispose();
                return dt;
            }
            catch (Exception ex)
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
                ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", "LRequests", "GetData", ex.Message, "RELY_API", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New", Result);
                throw ex;
            }
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message 
            if (SqEx.Message.IndexOf("FK_LRequests_LProducts_Requestid", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "REQUESTS", "PRODUCTS"));
            //else if (SqEx.Message.IndexOf("UQ_LRequests_CompanyCodeRequestId", StringComparison.OrdinalIgnoreCase) >= 0)
            //     return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "REQUESTS"));
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

        [HttpGet]
        public async Task<IHttpActionResult> GetUploadPPM(string FileName, string UserName, string LoggedInRoleId, string iCompanyCode, string WorkflowName, string UpdatedBy)
        {
            //async  Task<IHttpActionResult>

            var BatchModel = new XBatch();
            var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            var Task = RawQuery.SingleAsync();
            var BatchNumber = Task.Result;
            Task.Dispose();
            using (var transaction = db.Database.BeginTransaction())
            {
                BatchModel = new XBatch
                {
                    XCompanyCode = iCompanyCode,
                    XUpdatedBy = UpdatedBy,
                    XStatus = "Saved",
                    XBatchNumber = BatchNumber,
                    //XBatchNumber = 10,
                    XBatchType = "LRequests",
                    XRawDataType = null,
                    XUploadFinishDateTime = DateTime.UtcNow,
                    XAlteryxBatchNumber = null,
                    XComments = null,
                    BatchName = null,
                    XUploadStartDateTime = DateTime.UtcNow
                };
                db.XBatches.Add(BatchModel);
                await db.SaveChangesAsync();
                db.SaveChanges();
                var LBatchFiles = new XBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
                db.XBatchFiles.Add(LBatchFiles);
                await db.SaveChangesAsync();
                try
                {
                    #region 
                    string excelConnectionString = string.Empty;

                    #region File reading
                    //CreateDebugEntry("Start reading file");
                    var CompanyDetails = db.GCompanies.Where(p => p.CompanyCode == iCompanyCode).FirstOrDefault();
                    //string S3BucketPPMDataFilesFolder = ConfigurationManager.AppSettings["S3BucketPPMFilesFolder"];
                    //string S3TargetPath = "/" + iCompanyCode.ToLower() + "/" + S3BucketPPMDataFilesFolder + "/" + FileName;
                    //byte[] bytedata = Globals.DownloadFromS3(S3TargetPath, "");


                    string S3BucketPPMDataFilesFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                    string S3TargetPath = "/" + iCompanyCode.ToLower() + "/" + S3BucketPPMDataFilesFolder + "/" + FileName;
                    byte[] bytedata = Globals.DownloadFromS3(S3TargetPath);

                    string fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["LocalTempUploadFolder"], FileName);



                    ////byte[] bytedata = null;
                    ////FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read);
                    ////BinaryReader binaryReader = new BinaryReader(fs);
                    ////long byteLength = new FileInfo(fileLocation).Length;
                    ////bytedata = binaryReader.ReadBytes((Int32)byteLength);
                    ////fs.Close();
                    ////fs.Dispose();
                    ////binaryReader.Close();

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet dsSheet = new DataSet();
                    DataTable dtdataSheet = null;
                    try
                    {
                        try
                        {
                            string fileExtension = System.IO.Path.GetExtension(FileName);
                            string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                            string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + "_UPLOAD" + fileExtension;

                            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
                            string fullpath = path + "\\" + FileName_New;
                            System.IO.File.WriteAllBytes(fullpath, bytedata);
                            #region SSC changes
                            string connectionString = string.Empty;
                            excelConnectionString = ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"].Replace("{0}", fullpath);
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Rely$]", excelConnection);
                            cmd2.Fill(dsSheet);
                            dtdataSheet = dsSheet.Tables[0];
                            excelConnection.Close();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            DataTable dtE = new DataTable();
                            dtE.Columns.Add("ExceptionMessage");
                            dtE.Rows.Add(ex.ToString());
                            return Ok(dtE);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { UserName = "Rely", Controller = "LRequests", Method = "UploadPPM", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-Rely WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                    }
                    #endregion

                    //dtdataSheet.Rows.RemoveAt(0);//Removing the 1st row of data table as it contains the column alias.
                    #region Prepare data for bulk insert FOR Xschema.Xppm
                    System.Data.DataColumn newColumn1 = new System.Data.DataColumn("XCompanyId", typeof(System.String));
                    newColumn1.DefaultValue = iCompanyCode.ToString();
                    dtdataSheet.Columns.Add(newColumn1);
                    //System.Data.DataColumn newColumn2 = new System.Data.DataColumn("XProductId", typeof(System.String));
                    //dtdataSheet.Columns.Add(newColumn2);
                    //System.Data.DataColumn newColumn3 = new System.Data.DataColumn("XAutherzationNumber", typeof(System.String));
                    //dtdataSheet.Columns.Add(newColumn3);
                    //XFileNames
                    System.Data.DataColumn newColumn4 = new System.Data.DataColumn("XFileNames", typeof(System.String));
                    newColumn4.DefaultValue = FileName;
                    dtdataSheet.Columns.Add(newColumn4);
                    //XUserFriendlyFileNames
                    System.Data.DataColumn newColumn5 = new System.Data.DataColumn("XUserFriendlyFileNames", typeof(System.String));
                    newColumn5.DefaultValue = FileName;
                    dtdataSheet.Columns.Add(newColumn5);
                    //XCreatedDateTime
                    System.Data.DataColumn newColumn6 = new System.Data.DataColumn("XCreatedDateTime", typeof(System.DateTime));
                    newColumn6.DefaultValue = DateTime.UtcNow;
                    dtdataSheet.Columns.Add(newColumn6);
                    System.Data.DataColumn newColumn7 = new System.Data.DataColumn("XUpdatedDateTime", typeof(System.DateTime));
                    newColumn7.DefaultValue = DateTime.UtcNow;
                    dtdataSheet.Columns.Add(newColumn7);
                    //System.Data.DataColumn newColumn8 = new System.Data.DataColumn("XBatchNumber", typeof(System.String));
                    //newColumn8.DefaultValue = BatchNumber;
                    //dtdataSheet.Columns.Add(newColumn8);
                    //AlreadyExists
                    //System.Data.DataColumn newColumn8 = new System.Data.DataColumn("AlreadyExists", typeof(System.Boolean));
                    //newColumn8.DefaultValue = false;
                    //dtdataSheet.Columns.Add(newColumn8);
                    System.Data.DataColumn newColumn9 = new System.Data.DataColumn("XBatchNumber", typeof(System.String));
                    newColumn9.DefaultValue = BatchNumber;
                    dtdataSheet.Columns.Add(newColumn9);

                    System.Data.DataColumn newColumn10 = new System.Data.DataColumn("Action", typeof(System.String));
                    newColumn10.DefaultValue = "Create";
                    dtdataSheet.Columns.Add(newColumn10);

                    System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);


                    sqlBulk.ColumnMappings.Add("XCompanyId", "XCompanyId"); //
                    sqlBulk.ColumnMappings.Add("Product ID", "XProductId"); //
                    sqlBulk.ColumnMappings.Add("Authorization#", "XAutherzationNumber");//
                    sqlBulk.ColumnMappings.Add("XFileNames", "XFileNames"); //
                    sqlBulk.ColumnMappings.Add("XUserFriendlyFileNames", "XUserFriendlyFileNames"); //
                    sqlBulk.ColumnMappings.Add("Segment", "XSegment"); //
                    sqlBulk.ColumnMappings.Add("Gate", "XGate"); //
                    sqlBulk.ColumnMappings.Add("System", "XSystem"); //
                    sqlBulk.ColumnMappings.Add("Survey", "XSurvey");//
                    sqlBulk.ColumnMappings.Add("Business Category", "XBusinessCategory"); //
                    sqlBulk.ColumnMappings.Add("S15 impact", "XS15impact");//
                    sqlBulk.ColumnMappings.Add("Supported by existing systems", "XSupportedbyExistingSystems");//
                    sqlBulk.ColumnMappings.Add("Type of Product", "XTypeofProduct");//
                    sqlBulk.ColumnMappings.Add("Create new Local POB Name", "XCreateNewLocalPOBName");//
                    sqlBulk.ColumnMappings.Add("Global POB 1: Acquisit", "XGlobalPOB1Acquisit");//
                    sqlBulk.ColumnMappings.Add("Global POB 2: Retention", "XGlobalPOB2Retention");//
                    sqlBulk.ColumnMappings.Add("Copa Dimesion - Call Origin/Destination", "XCopaDimesionCallOriginDestination"); //
                    sqlBulk.ColumnMappings.Add("Copa Dimesion : Bearer Technology", "XCopaDimesionBearerTechnology"); //
                    sqlBulk.ColumnMappings.Add("XBatchNumber", "XBatchNumber");
                    sqlBulk.ColumnMappings.Add("XCreatedDateTime", "XCreatedDateTime"); //
                    sqlBulk.ColumnMappings.Add("XUpdatedDateTime", "XUpdatedDateTime"); //
                    sqlBulk.ColumnMappings.Add("SSP", "XSspAmount");//
                    sqlBulk.ColumnMappings.Add("Special Indicator", "XSpecialIndicator");//
                    sqlBulk.ColumnMappings.Add("Action", "Action");//
                    sqlBulk.ColumnMappings.Add("POB Indicator", "XPOBIndicator");//
                    sqlBulk.ColumnMappings.Add("Usage indicator", "XUsageIndicator"); //
                    sqlBulk.ColumnMappings.Add("Bundle indicator", "XBundleIndicator"); //                
                    sqlBulk.ColumnMappings.Add("Article number", "XArticleNumber");//                  
                    sqlBulk.ColumnMappings.Add("Global POB 3 : Others", "XGlobalPOBOthers");//


                    sqlBulk.DestinationTableName = "XRequestUpload";
                    #endregion
                    try
                    {
                        sqlBulk.WriteToServer(dtdataSheet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { UserName = "RELY", Controller = "LRequests", Method = "UploadPPM", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-RELY WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                        DataTable dtE = new DataTable();
                        dtE.Columns.Add("ExceptionMessage");
                        return Ok(dtE);
                    }

                    transaction.Commit();

                    #endregion
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var models = new GErrorLog { UserName = "RELY", Controller = "LClaims", Method = "UploadClaims", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.ToString(), SourceProject = "[Vodafone-Rely WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("ExceptionMessage");
                    return Ok(dtE);
                }
                //transaction.Commit();
            }

            #region commented by shivani
            var Query = "Exec dbo.USPValidateSheetUploadData @UserID,@CompanyCode,@BatchNo";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserID", UpdatedBy);
            cmd.Parameters.AddWithValue("@CompanyCode", iCompanyCode.ToString());
            cmd.Parameters.AddWithValue("@BatchNo", BatchNumber);
            DataTable dtErrors = new DataTable();
            dtErrors = Globals.GetDataTableUsingADO(cmd);
            return Ok();
            #endregion
        }

        [HttpGet]
        public IHttpActionResult GetGridDataFields(int CompanyId)
        {
            var Query = "Exec dbo.[SPGetUploadPayeeColumnHeaders] @CompanyID";
            DataTable dt = new DataTable();
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            conn.Close();
            //The Ado.Net code ends here

            return Ok(dt);
        }

        public IHttpActionResult GetXUploadLRequestCountByBatchNumber(int CompanyId, int BatchNumber)
        {
            var Query = "select count(*) from XRequestUpload where XCompanyId={0} and XBatchNumber={1}";
            var count = db.Database.SqlQuery<int>(Query, CompanyId, BatchNumber).FirstOrDefault();
            return Ok(count);
        }

        [HttpGet]
        public IHttpActionResult GetByUserForLRequestUploadGrid(string CompanyCode, string AspnetUserid)
        {
            string Qry = "select Case lb.XStatus WHEN 'ValidationFailed' Then 0 else 1 END as IsImport,lb.Id,lb.XStatus,lb.XBatchNumber,isnull(lb.XRecordCount,0) as XRecordCount,lb.XUploadStartDateTime,lbf.LbfFileName from XBatches lb join XBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.XBatchType='LRequests' and lb.XCompanyCode = {0} and lb.XUpdatedBy={1} and lb.XStatus <> 'Deleted' order by lb.Id desc ";
            var xx = db.Database.SqlQuery<LBatchViewModelGrid>(Qry, CompanyCode, AspnetUserid).ToList();
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult GetXUploadLRequestByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum, string FilterQuery)
        {
            DataTable dt = new DataTable();
            var Query = "exec SPGetXUploadPayeeByBatchNumber @CompanyId,@BatchNumber,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@BatchNumber", BatchNumber);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            conn.Close();
            //The Ado.Net code ends here

            return Ok(dt);
        }

        [HttpGet]
        public IHttpActionResult DeleteLRequestUploadBatch(int Id)
        {
            var batch = db.XBatches.Where(a => a.Id == Id).FirstOrDefault();
            batch.XStatus = "Deleted";
            db.Entry(batch).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetById(string CompanyCode, int Id)
        {
            string Qry = "select lb.Id,lb.XStatus,lb.XBatchNumber,lb.XBatchNumber,lb.XUploadStartDateTime,lbf.LbfFileName from XBatches lb " +
                " join XBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.XBatchType='LRequests' and lb.XCompanyCode = {0} and lb.Id={1}";
            var xx = db.Database.SqlQuery<LBatchViewModelForRequestGrid>(Qry, CompanyCode, Id).FirstOrDefault();
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult UploadValidatedRequestBatch(string CompanyCode, int BatchNumber, string AspNetUserId, int LoggedinRoleId, string Workflow)
        {
            string Query = "exec SpUploadValidatedRequests {0},{1},{2},{3},{4}";
            //due to exception, using List<Object>
            //The data reader has more than one field. Multiple fields are not valid for EDM primitive or enumeration types.
            db.Database.SqlQuery<List<Object>>(Query, CompanyCode, BatchNumber, AspNetUserId, LoggedinRoleId, Workflow).FirstOrDefault();

            //RK
            //Query = "exec SpPushUploadedPayeesToNextOrdinal {0},{1},{2},{3},{4}";
            ////due to exception, using List<Object>
            ////The data reader has more than one field. Multiple fields are not valid for EDM primitive or enumeration types.
            //db.Database.SqlQuery<List<Object>>(Query, CompanyId, BatchNumber, AspNetUserId, LoggedinRoleId, Workflow).FirstOrDefault();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult DownloadRequestUploadErrors(string CompanyCode, int BatchNumber)
        {
            //var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            string Filename = null;
            string Query = "select XProductId as [Product Id],XValidationMessage as [Validation Message] from XRequestUpload where XCompanyId='" + CompanyCode + "' and XBatchNumber=" + BatchNumber + " and XValidationMessage is not null";
            DataSet ds = new DataSet();
            DataTable dtPayee = Globals.GetDdatainDataTable(Query);
            //R3.1 - SG 18082020 - Portfolio errors included in sheet as well.
            ds.Tables.Add(dtPayee);
            
            ds.Tables[0].TableName = "LRequests"; 
            Filename = BatchNumber + "_LRequestUploadErrors" + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".xlsx";
            var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/"; // + CompanyDetails.CompanyCode + "/upload/payees/";
            var OutPutMessage = Globals.ExportDataSetToExcel(ds, TempPath, Filename, "all text", "dd.mm.yyyy");
            //Globals.ExportToExcel(dtPayee, TempPath, Filename);

            if (!string.IsNullOrEmpty(Filename))
            {
                string fullpath = TempPath + "\\" + Filename;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            return Ok(Filename);
        }
    }
}