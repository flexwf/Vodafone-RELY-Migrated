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
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LEmailTemplatesController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LEmailTemplates
        [HttpGet]
        public IHttpActionResult GetLEmailTemplates(string CompanyCode)
        {
            var xx = (from aa in db.LEmailTemplates.Where(p => p.CompanyCode == CompanyCode)
                      select new { aa.Id, aa.TemplateName, aa.EmailSubject, aa.EmailBody, aa.Signature, aa.CompanyCode }).OrderBy(p => p.TemplateName);
            return Ok(xx);
        }

        // GET: api/LEmailTemplate/GetEmailTemplateByCompanyCode
        [ResponseType(typeof(LEmailTemplate))]
        public IHttpActionResult GetEmailTemplateByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.LEmailTemplates.Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.Id,
                          aa.TemplateName
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

        // GET: api/LEmailTemplates/5
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> GetLEmailTemplate(Nullable<int> id)
        {
            var LEmailTemplate = db.LEmailTemplates.Where(p => p.Id == id).Select(aa => new { aa.Id, aa.TemplateName, aa.EmailSubject, aa.EmailBody, aa.Signature, aa.CompanyCode }).FirstOrDefault();
            if (LEmailTemplate == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TEMPLATE")));
            }
            return Ok(LEmailTemplate);
        }

        // PUT: api/LEmailTemplates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLEmailTemplate(int id, LEmailTemplate LEmailTemplate, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, int LoggedInUserId, int UserRoleId)
        {
            
            
                if (!LEmailTemplateExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TEMPLATE")));
                }

                if (id != LEmailTemplate.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TEMPLATE")));
                }
                try
                {
                    db.Entry(LEmailTemplate).State = EntityState.Modified;
                    await db.SaveChangesAsync();


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
                        var Destination = "/" + LEmailTemplate.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                        var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                        var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                        if (sucess)
                            FilePath = Destination;

                        var SupportingDocument = new LSupportingDocument
                        {

                            FileName = FileArray[i],
                            OriginalFileName = OriginalFileArray[i],
                            FilePath = FilePath,
                            EntityId = LEmailTemplate.Id,
                            EntityType = "LEmailTemplates",
                            CreatedById = LoggedInUserId, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                            UpdatedById = LoggedInUserId,
                            CreatedByRoleId = UserRoleId,
                            CreatedDateTime = DateTime.UtcNow,
                            UpdatedDateTime = DateTime.UtcNow,
                            Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                        };
                        db.LSupportingDocuments.Add(SupportingDocument);
                        db.SaveChanges();
                        /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                         * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                         * NewStatus should be same as current status of the entry, comments = 
                         * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                        = LSupportingDocument.Id, rest of the columns are obvious.*/
                        // db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                        //"Create", LProduct.UpdatedById, LProduct.WFRequesterRoleId, DateTime.UtcNow, LProduct.WFStatus, LProduct.WFStatus,
                        //"LProducts", LProduct.Id, LProduct.Name, WorkflowDetails.Id, LProduct.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                        // db.SaveChanges();
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
            return Ok(LEmailTemplate);
        }

        // POST: api/LEmailTemplates
    
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> PostLEmailTemplate(LEmailTemplate LEmailTemplate, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList,int LoggedInUserId,int UserRoleId)
        {
            //LEmailTemplate.Id = 1;LEmailTemplate.EmailSubject = "Subject";LEmailTemplate.EmailBody = "<div></div>";
            //LEmailTemplate.Signature = "Namita Singla";LEmailTemplate.CompanyCode = "DE";LEmailTemplate.TemplateName = "Template_name";
            
            
                try
                {
                    db.LEmailTemplates.Add(LEmailTemplate);
                  await db.SaveChangesAsync();
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
                        var Destination = "/" + LEmailTemplate.CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
                        var DestinationCompleteFilePath = Destination + "/" + FileArray[i];
                        var sucess = Globals.MoveFileinS3(Source, DestinationCompleteFilePath);
                        if (sucess)
                            FilePath = Destination;

                        var SupportingDocument = new LSupportingDocument
                        {
                           
                            FileName = FileArray[i],
                            OriginalFileName = OriginalFileArray[i],
                            FilePath = FilePath,
                            EntityId = LEmailTemplate.Id,
                            EntityType = "LEmailTemplates",
                            CreatedById = LoggedInUserId, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                           UpdatedById = LoggedInUserId,
                            CreatedByRoleId = UserRoleId,
                            CreatedDateTime = DateTime.UtcNow,
                            UpdatedDateTime = DateTime.UtcNow,
                            Description = "Uploaded " + OriginalFileArray[i] + " :" + "User Description: " + (!string.IsNullOrEmpty(SupportingDocumentsDescription) ? DescriptionArray[i] : null)
                        };
                        db.LSupportingDocuments.Add(SupportingDocument);
                        db.SaveChanges();
                        /*(b) An entry is to be made in audit table when a file is attached. (RelyProcessName, VFProcessName = WFName, ControlCode = 'Audiit',
                         * Action = 'AddAttachment',ActionType = Create/Edit depending upon the mode (create/edit) in which the form is open, OldStatus,
                         * NewStatus should be same as current status of the entry, comments = 
                         * "Uploaded <FileName> :" + "User Description: " + <Description entered by user in FileUploadUtility>, SupportindDocumentId
                        = LSupportingDocument.Id, rest of the columns are obvious.*/
                       // db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "AddAttachment",
                       //"Create", LProduct.UpdatedById, LProduct.WFRequesterRoleId, DateTime.UtcNow, LProduct.WFStatus, LProduct.WFStatus,
                       //"LProducts", LProduct.Id, LProduct.Name, WorkflowDetails.Id, LProduct.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                       // db.SaveChanges();
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
            return CreatedAtRoute("DefaultApi", new { id = LEmailTemplate.Id }, LEmailTemplate);
        }

        // DELETE: api/LEmailTemplates/5
        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> DeleteLEmailTemplate(int id,string CompanyCode )
        {
            LEmailTemplate LEmailTemplate = await db.LEmailTemplates.FindAsync(id);
            if (LEmailTemplate == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "TEMPLATE")));
            }
                try
                {
                //Before deleting LEmailTemplate, will check whether a Notification is associated with this Email Template or not
                var Notification = db.LNotifications.Where(p => p.TemplateId == id).Where(p => p.CompanyCode == CompanyCode).ToList();
                if (Notification.Count != 0)
                {
                   throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Email Template", "Notification")));
                }
                else
                {
                    db.LEmailTemplates.Remove(LEmailTemplate);
                    await db.SaveChangesAsync();
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

        private bool LEmailTemplateExists(int id)
        {
            return db.LEmailTemplates.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message

            if (SqEx.Message.IndexOf("UQ_LEmailTemplates_CompanyCode_TemplateName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "EMAIL TEMPLATES"));
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

        [ResponseType(typeof(LEmailTemplate))]
        public async Task<IHttpActionResult> GetLEmailTemplateById(int id)
        {

            //this needs to be updated after Model update as SurveyId will be nullable<int>
            string sqlQuery = "select * from LEmailTemplates where id = {0}";
            var LEmailTemplate = db.Database.SqlQuery<LEmailTemplateViewModel>(sqlQuery, id).FirstOrDefault();
           
            if (LEmailTemplate == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PRODUCT")));
            }
            return Ok(LEmailTemplate);
        }


    }
}
