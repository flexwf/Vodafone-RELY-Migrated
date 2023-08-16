using RELY_API.Models;
using RELY_API.Utilities;
//using RELY_APP.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
//using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LReferencesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        //this method validates the cell value using db function FnValidateDataValue
        [HttpGet]
        public IHttpActionResult ValidateColumnValue(string TableName, string UserName, string WorkFlow, string ColumnName, string Value, string FieldLabel, string SelecterType, string CompanyCode)
        {
            /*String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            string strQuery = "select dbo.FnValidateDataValue(@TableName,@ColumnName,@Value,@FieldLabel,@SelecterType,@CompanyCode)";
            SqlCommand cmd = new SqlCommand(strQuery, con);
            cmd.Parameters.AddWithValue("@TableName", TableName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ColumnName", ColumnName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Value", Value ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FieldLabel", FieldLabel ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SelecterType", SelecterType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode ?? (object)DBNull.Value);
            con.Open();
            SqlDataReader dataReaderSql = cmd.ExecuteReader();
            string retunValue = "";
            while (dataReaderSql.Read())
            {
                retunValue = dataReaderSql[0].ToString();
                break;
            }
            dataReaderSql.Close();
            con.Close();
            con.Dispose();*/
            string SqlQuery = "select dbo.FnValidateDataValue({0},{1},{2},{3},{4},{5})";
            var retunValue =  db.Database.SqlQuery<string>(SqlQuery, TableName, ColumnName, Value, FieldLabel, SelecterType, CompanyCode).FirstOrDefault();
            //TODO: - Need to try below mechanism instead of ado
            // var result = db.Database.SqlQuery<GenericNameAndIdViewModel>("select dbo.FnValidateDataValue({0},{1},{2},{3},{4},{5})", TableName ?? (object)DBNull.Value, ColumnName ?? (object)DBNull.Value, Value ?? (object)DBNull.Value, FieldLabel ?? (object)DBNull.Value, SelecterType ?? (object)DBNull.Value, CompanyCode ?? (object)DBNull.Value).FirstOrDefault(); 
            //using this model as function is returning string value which needs to be sent back to APP
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = retunValue };
            return Ok(model);
        }

        // GET: api/LReferences/5
        [ResponseType(typeof(LReference))]
        public async Task<IHttpActionResult> GetLReferencesById(int ReferenceId)
        {
            var LReference = db.LReferences.Where(p => p.Id == ReferenceId).Select(aa => new
            {
                aa.Id,
                aa.ReferenceTypeId,
                aa.Name,
                aa.Description,
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
                aa.Status,
                aa.WFStatusDateTime,
                aa.WFIsReadyDateTime,
                aa.ExtractFileName,
                aa.Version,
                aa.CompanyCode
            }).FirstOrDefault();
            if (LReference == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "References")));
            }
            return Ok(LReference);
        }
        
        //public IHttpActionResult GetInformationSchema(string TableName, string ColumnName)
        //{

        //    string query = "select DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,COLUMN_DEFAULT from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME= {0} and  COLUMN_NAME={1}";
        //    var InformatonSchema = db.Database.SqlQuery<Models.InformationSchemaViewModel>(query, TableName, ColumnName).FirstOrDefault();
        //    return Ok(InformatonSchema);
        //}

        ////this method is used to get schema details for the given table.
        //public IHttpActionResult GetInformationSchemaForTable(string TableName)
        //{
        //    string query = "select COLUMN_NAME,ORDINAL_POSITION,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,COLUMN_DEFAULT from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME= {0} ";
        //    var InformatonSchema = db.Database.SqlQuery<Models.InformationSchemaViewModel>(query, TableName).ToList();
        //    return Ok(InformatonSchema);
        //}

        [HttpPut]
        // PUT: api/LReferences/5
        [ResponseType(typeof(void))]
       public IHttpActionResult PutLReference(int id, UploadReferenceViewModel model, string UserName, string WorkFlow, string CompanyCode, int LoggedInUserId, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, string DataUploadFileName, bool IsDataUploadedByFile, bool OverwriteExistingData, string ActionName)
        {
            //model = new UploadReferenceViewModel();
            //model.collength = 11;
            //model.GridArray = "AttributeC01:DEV_AppendTest,AttributeM01:Test12,AttributeC03:null,AttributeC04:null,AttributeC05:null,AttributeC06:null,AttributeC07:null,AttributeC08:null,AttributeD01:null,AttributeD02:null,Id:494648,";
            //model.Id = 455; model.ReferenceTypeId = 6;
            //model.CreatedById = 54;model.UpdatedById = 54; model.UpdatedDateTime = new DateTime(); model.CreatedDateTime = new DateTime();
            //model.WFStatus = "Saved"; model.WFStatusDateTime = new DateTime(); model.WFType = "LReferences";model.Name = "CREDIT_NOTES";
            //model.ExtractFileName = "CREDIT_NOTES";model.WFOrdinal = 1; model.CompanyCode = "DE";
            //model.Status = "Saved"; model.Version = 1; model.WFComments = "Updated in debugging";model.WFRequesterId = 54;model.WFRequesterRoleId = 1;

            if (!LReferenceExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "References")));
            }

            if (id != model.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "References")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {

                try
                {
                    var LReference = db.LReferences.Where(p => p.Id == id).FirstOrDefault();
                    LReference.ReferenceTypeId = model.ReferenceTypeId;
                    LReference.Name = model.Name;
                    LReference.ExtractFileName = model.ExtractFileName;
                    LReference.Description = model.Description;
                    LReference.UpdatedById = model.UpdatedById;
                    LReference.UpdatedDateTime = model.UpdatedDateTime;
                    LReference.WFComments = model.WFComments;
                    LReference.CompanyCode = model.CompanyCode;
                    //LReference.Version = model.Version;//not required to updae version in edit
                    db.Entry(LReference).State = EntityState.Modified;
                    db.SaveChanges();
                    //detach will free up the entity for further modification whenever required, otherwise, entity will not be available for modification.
                    db.Entry(LReference).State = EntityState.Detached;
                    db.SaveChanges();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                    //WF Change action is implemented here
                    if (ActionName.Equals("Change"))
                    {
                        //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                        string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
                        db.SPUpdateActionStatus(ActionName, WorkflowDetails.Name, id.ToString(), CompanyCode, LReference.CreatedById.ToString(), LReference.WFComments, LReference.WFRequesterRoleId.ToString(), ProjectEnviournment, string.Empty);
                    }

                    if (OverwriteExistingData)
                    {
                        //delete existing ReferenceData
                        var RefData = db.LReferenceDatas.Where(p => p.ReferenceId == LReference.Id).ToList();
                         db.LReferenceDatas.RemoveRange(RefData);
                         db.SaveChanges();
                    }
                    if (!IsDataUploadedByFile)
                    {
                        SaveRefDataFromGrid(model.GridArray, model.collength, LoggedInUserId, LReference.Id);
                    }
                    else
                    {//means ref data is uploaded thru file 
                     //SaveRefData(LReference.Id, LoggedInUserId, DataUploadFileName, CompanyCode);
                     //call SP for inserting data to LReferenceData table
                     /* var Query = "Exec SpSaveRefData @ReferenceId,@LoggedInUserId";
                      using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                      {
                          conn.Open();
                          SqlCommand cmd = new SqlCommand(Query);
                          cmd.Connection = conn;
                          cmd.Parameters.AddWithValue("@ReferenceId", LReference.Id);
                          cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                          cmd.ExecuteNonQuery();
                          conn.Close();
                      }*/
                        db.SpSaveRefData(LReference.Id, LoggedInUserId);
                        db.SaveChanges();
                    }

                    //Audit section starts
                    //string fixedComments = "";
                    //string AuditComments = fixedComments + " (Status " + LReference.WFStatus + ") " + LReference.WFComments;
                    //Comment format updated - 10Oct2018- Andre raised bug for duplicacy of information.
                    string AuditComments = LReference.WFComments;
                    var Action = db.WActions.Where(p => p.Name == "Edit").FirstOrDefault();

                    var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", LReference.WFOrdinal, CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                           "Update", LReference.UpdatedById, LReference.WFRequesterRoleId, DateTime.UtcNow, LReference.WFStatus, LReference.WFStatus,
                           "LReferences", LReference.Id, LReference.Name, WorkflowDetails.Id, CompanyCode, AuditComments, StepId, Action.Label, null);
                    db.SaveChanges();
                    //Audit section ends
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
                            var Destination = "/" + CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
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
                                EntityId = LReference.Id,
                                EntityType = "LReferences",
                                CreatedById = LReference.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = LReference.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                CreatedByRoleId = LReference.WFRequesterRoleId.Value,
                                CreatedDateTime = DateTime.UtcNow,
                                UpdatedDateTime = DateTime.UtcNow,
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
                           "Create", LReference.UpdatedById, LReference.WFRequesterRoleId, DateTime.UtcNow, LReference.WFStatus, LReference.WFStatus,
                           "LReferences", LReference.Id, LReference.Name, WorkflowDetails.Id, CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }

                    //supporting documents section ends
                    //everything looks good, so committing transaction
                    transaction.Commit();
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

            }

            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(model.Id);
        }

        [HttpPost]
        // [HttpGet]
        // POST: api/LReferences
        [ResponseType(typeof(LReference))]
        public IHttpActionResult PostLReference(UploadReferenceViewModel model, string UserName, string WorkFlow, string CompanyCode, int LoggedInUserId, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList, string DataUploadFileName, bool IsDataUploadedByFile)
        {
            
            var WorkflowDetails = db.RWorkFlows.Where(p => p.Name.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
            var StatusAndOrdinal = db.Database.SqlQuery<NameOrdinalViewModel>("select Name,Ordinal from dbo.FnGetWFColumnValues({0},{1},{2},'Create',{3})", model.WFRequesterId, model.WFRequesterRoleId, WorkflowDetails.Id, CompanyCode).FirstOrDefault();
            if (StatusAndOrdinal != null)
            {
                model.WFOrdinal = StatusAndOrdinal.Ordinal;
                model.Status = StatusAndOrdinal.Name;
            }
            else
            {
                model.WFOrdinal = 1;
                model.Status = "Saved";
            }
            var LReference = new LReference
            {
                ReferenceTypeId = model.ReferenceTypeId,
                Name = model.Name,
                Description = model.Description,
                WFOrdinal = model.WFOrdinal,
                WFStatus = model.WFStatus,
                WFType = model.WFType,
                WFComments = model.WFComments,
                WFRequesterId = model.WFRequesterId,
                WFAnalystId = model.WFAnalystId,
                WFManagerId = model.WFManagerId,
                WFCurrentOwnerId = model.WFCurrentOwnerId,
                WFRequesterRoleId = model.WFRequesterRoleId,
                CreatedById = model.CreatedById,
                CreatedDateTime = model.CreatedDateTime,
                UpdatedById = model.UpdatedById,
                UpdatedDateTime = model.UpdatedDateTime,
                Status = model.Status,
                WFStatusDateTime = model.WFStatusDateTime,
                WFIsReadyDateTime = model.WFIsReadyDateTime,
                ExtractFileName = model.ExtractFileName,
                Version = model.Version,
                CompanyCode=model.CompanyCode
            };
            //getting step and Action details for Audit log entries
            var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", LReference.WFOrdinal, CompanyCode, WorkflowDetails.Id).FirstOrDefault();
            var Action = db.WActions.Where(p => p.Name == "Create").FirstOrDefault();
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "References")));
            //}

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.LReferences.Add(LReference);
                    db.SaveChanges();
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                               "Update", LReference.UpdatedById, LReference.WFRequesterRoleId, DateTime.UtcNow, LReference.WFStatus, LReference.WFStatus,
                               "Lreferences", LReference.Id, LReference.Name, WorkflowDetails.Id, CompanyCode, LReference.WFComments, StepId,Action.Label, null);
                    db.SaveChanges();
                    //New Method to save Large file Data
                    if (IsDataUploadedByFile)
                    {
                        //SaveRefData(LReference.Id, LoggedInUserId, DataUploadFileName, CompanyCode);
                        //SaveBulkDataToDB(LReference.Id, LoggedInUserId, DataUploadFileName, CompanyCode, "LReferenceData");
                        //call SP for inserting data to LReferenceData table
                        /*var Query = "Exec SpSaveRefData @ReferenceId,@LoggedInUserId";
                        using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(Query);
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@ReferenceId", LReference.Id);
                            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }*/

                        db.SpSaveRefData(LReference.Id, LoggedInUserId);
                        db.SaveChanges();
                    }
                    else
                    {
                        //Data
                        //new method for storing reference data 
                        SaveRefDataFromGrid(model.GridArray, model.collength, LoggedInUserId, LReference.Id);
                    }

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
                            var Destination = "/" + CompanyCode.ToLower() + "/" + ConfigurationManager.AppSettings["S3BucketSupportingDocumentFolder"];
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
                                EntityId = LReference.Id,
                                EntityType = "LReferences",
                                CreatedById = LReference.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                UpdatedById = LReference.UpdatedById, //for the sake of consistency , CreatedById and UpdatedById of Attachment should be the LoggedInUsedId
                                CreatedByRoleId = LReference.WFRequesterRoleId.Value,
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
                           "Create", LReference.UpdatedById, LReference.WFRequesterRoleId, DateTime.UtcNow, LReference.WFStatus, LReference.WFStatus,
                           "LReferences", LReference.Id, LReference.Name, WorkflowDetails.Id, CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }


                   // Add Portfolios
                    if (db.LPortfolios.Where(m => m.CompanyCode == CompanyCode).Count() > 0)
                    {
                        MEntityPortfolio MEP = new MEntityPortfolio();
                        MEP.EntityId = LReference.Id;
                        MEP.EntityType = "LReferences";
                        int PortfolioId = db.LPortfolios.Where(m => m.CompanyCode == CompanyCode).FirstOrDefault().Id;
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
            return Ok(LReference.Id);
            //return CreatedAtRoute("DefaultApi", new { Id = LReference.Id }, LReference);
        }

        //private void SaveRefData(int ReferenceId, int LoggedInUserId, string filename, string CompanyCode)
        //{
        //    //First save the file in Rely Temp by getting it from bucket
        //    string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
        //    string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + filename;
        //    var bytedata = Globals.DownloadFromS3(S3TargetPath);
        //    string path = ConfigurationManager.AppSettings["RelyTempPath"];
        //    string fullpath = path + "\\" + filename;
        //    if (System.IO.File.Exists(fullpath))
        //    {
        //        System.IO.File.Delete(fullpath);
        //    }
        //    SqlDataAdapter adapter = new SqlDataAdapter();
        //    string sql = null;
        //    SqlCommand cmd1 = null;

        //    DataSet ds = new DataSet();
        //    try
        //    {
                
        //        System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
        //        DebugEntry("WriteBytes : " + fullpath);
        //        OleDbConnection con = null;
        //        //read large files 
        //        try
        //        {
        //            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
        //             con = new System.Data.OleDb.OleDbConnection(connectionString);
        //            DebugEntry("Before Connection to excel open ");
        //            con.Open();
        //            DebugEntry("Connection established ");
                    
        //            OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [SHEET1$]", con);
        //            cmd.Fill(ds);
        //            DebugEntry("Excel read successfully");
        //            DataTable dt = ds.Tables[0];
        //            con.Close();
        //            DebugEntry("Connection to excel closed");
        //        }
        //        catch (Exception ex)
        //        {
        //            DebugEntry("exception ocuured in inner try-catch block :" + ex.StackTrace.ToString());
        //            throw ex;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugEntry("exception ocuured in outer try-catch block:" + ex.StackTrace.ToString());
        //        throw ex;
        //    }
        //    //insert into debug
        //    //After filling the data read from excel sheet in a data set this for loop reads the data row by row 
        //    for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        DebugEntry("starting reading row no : " + i);
        //        //Loop through view model to read all respective records 
        //        var properties = typeof(LReferenceData).GetProperties();
        //        var ReferenceDataModel = new LReferenceData
        //        {
        //            ReferenceId = ReferenceId,
        //            CreatedDateTime = System.DateTime.Now,
        //            UpdatedDateTime = System.DateTime.Now,
        //            CreatedById = LoggedInUserId,
        //            UpdatedById = LoggedInUserId
        //        };
        //        foreach (var property in properties)
        //        {
                    
        //            var propertyName = property.Name;
        //            var PropertyType = property.PropertyType.Name;
        //            // DebugEntry("Reading Column " + propertyName +  " type : " + PropertyType);
        //            if (
        //                property.PropertyType == typeof(Nullable<int>))
        //            {
        //                PropertyType = "int";
        //            }
        //            else if (
        //               property.PropertyType == typeof(Nullable<DateTime>))
        //            {
        //                PropertyType = "datetime";
        //            }
        //            else if (
        //              property.PropertyType == typeof(Nullable<decimal>))
        //            {
        //                PropertyType = "decimal";
        //            }
        //            else if (
        //              property.PropertyType == typeof(Nullable<Boolean>))
        //            {
        //                PropertyType = "boolean";
        //            }
        //            else
        //            {
        //                PropertyType = property.PropertyType.Name;
        //            }

        //            if (ds.Tables[0].Rows[i].Table.Columns.Contains(propertyName))//As we do not want to read these properties which are not in excel from excel and assign them below
        //            {
        //                DebugEntry("Value of attibute" + propertyName + " = " + ds.Tables[0].Rows[i][propertyName].ToString());
        //                switch (PropertyType.ToLower())
        //                {
        //                    case "string":
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
        //                        {
        //                            property.SetValue(ReferenceDataModel, ds.Tables[0].Rows[i][propertyName].ToString());
        //                        }
        //                        break;
        //                    case "int":
        //                    case "int32":
        //                        // IsRequired=  Attribute.IsDefined(property, typeof(Required));
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
        //                        {
        //                            property.SetValue(ReferenceDataModel, Convert.ToInt32(ds.Tables[0].Rows[i][propertyName].ToString()));
        //                        }
        //                        break;
        //                    case "decimal":
        //                        // IsRequired=  Attribute.IsDefined(property, typeof(Required));
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
        //                        {
        //                            property.SetValue(ReferenceDataModel, Convert.ToDecimal(ds.Tables[0].Rows[i][propertyName].ToString()));
        //                        }
        //                        break;
        //                    case "datetime":
        //                        //IsRequired= Attribute.IsDefined(property, typeof(Required));
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
        //                        {
        //                            var TestData = ds.Tables[0].Rows[i][propertyName].ToString();
        //                            property.SetValue(ReferenceDataModel, Convert.ToDateTime(ds.Tables[0].Rows[i][propertyName].ToString()));
        //                        }
        //                        break;
        //                    case "boolean":
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
        //                        {
        //                            var TestData = ds.Tables[0].Rows[i][propertyName].ToString();
        //                            property.SetValue(ReferenceDataModel, Convert.ToBoolean(ds.Tables[0].Rows[i][propertyName].ToString()));
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //        //add in db
        //        db.LReferenceDatas.Add(ReferenceDataModel);
        //        DebugEntry("RefData model prepared i = " + i);
        //        db.SaveChanges();
        //        DebugEntry("RefData model saved in table i = " + i);
        //    }
        //   // return Ok();
        //}


        //method uses SQLBulkCopy functionality for saving data to DB directly.
        private void SaveBulkDataToDB(int ReferenceId, int LoggedInUserId, string filename, string CompanyCode,string TableName)
        {
            //First save the file in Rely Temp by getting it from bucket
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + filename;
            var bytedata = Globals.DownloadFromS3(S3TargetPath);
            string path = ConfigurationManager.AppSettings["RelyTempPath"];
            string fullpath = path + "\\" + filename;
            //if (System.IO.File.Exists(fullpath))
            //{
            //    System.IO.File.Delete(fullpath);
            //}
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = null;
            //try
            //{
            System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
            OleDbConnection con = null;
            //read large files 
            //try
            //{
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            con = new System.Data.OleDb.OleDbConnection(connectionString);
            con.Open();
            OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [SHEET1$]", con);
            cmd.Fill(ds);
            dt = ds.Tables[0];
            con.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //Since fixed columns were to be added in dt, using Copy() would not incorporate default value for fixed columns.
            //Using clone() just uses the table structure. 
            DataTable TempDt = dt.Clone();

            //Adding fixed columns to DataTable
            DataColumn ReferenceIdColumn = TempDt.Columns.Add("ReferenceId", typeof(int));
            ReferenceIdColumn.DefaultValue = ReferenceId;

            //DataColumn ESDColumn = TempDt.Columns.Add("EffectiveStartDate" );
            //DataColumn EEDColumn = TempDt.Columns.Add("EffectiveEndDate" );

            DataColumn CIdColumn = TempDt.Columns.Add("CreatedById", typeof(int));
            CIdColumn.DefaultValue = LoggedInUserId;

            DataColumn CDTColumn = TempDt.Columns.Add("CreatedDateTime");
            CDTColumn.DefaultValue = DateTime.UtcNow;

            DataColumn UIdColumn = TempDt.Columns.Add("UpdatedById", typeof(int));
            UIdColumn.DefaultValue = LoggedInUserId;

            DataColumn UDTColumn = TempDt.Columns.Add("UpdatedDateTime");
            UDTColumn.DefaultValue = DateTime.UtcNow;

            if (TableName.Equals("TRefDataValidation"))
            {
                DataColumn UserIdColumn = TempDt.Columns.Add("UserId");
                UserIdColumn.DefaultValue = LoggedInUserId;
                //adding SrNo column which will be auto incremented and starting value will 3.
                //This column will be used to show the row no in case of invalid records.
                DataColumn SrNo = TempDt.Columns.Add("SrNo");
                SrNo.DataType = System.Type.GetType("System.Int32");
                SrNo.AutoIncrement = true;
                SrNo.AutoIncrementSeed = 2;
                SrNo.AutoIncrementStep = 1;
            }

            //coping data to tempDt
            foreach (DataRow dr in dt.Rows)
            {
                TempDt.Rows.Add(dr.ItemArray);
            }
            TempDt.Rows.RemoveAt(0);//we dont need first row as it contains label information.

            var SQLConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(SQLConnString))
            {
                // make sure to enable triggers
                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                // set the destination table name
                bulkCopy.DestinationTableName = TableName;
                foreach (DataColumn col in TempDt.Columns)
                {
                    // Set up the column mappings by name.
                    SqlBulkCopyColumnMapping col1 =
                        new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName);
                    bulkCopy.ColumnMappings.Add(col1);
                }
                connection.Open();
                try
                {
                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(TempDt);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    dt.Clear();
                    TempDt.Clear();
                }
            }
            // reset

        }

        [HttpGet]
        public IHttpActionResult ReadAndValidateExcelData( int LoggedInUserId, string filename, string UserName, string WorkFlow, string CompanyCode,string SelecterType)
        {
            int ReferenceId = 0; //as we dont have refId at this momment, using 0 value
            ObjectParameter Resultforsp = new ObjectParameter("Result", typeof(int)); //return parameter is declared
            //delete existing data for the user id which is logged in.
           /* String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("delete from TRefDataValidation where UserId = @UserId", con))
                {
                    command.Parameters.AddWithValue("@UserId", LoggedInUserId);
                    command.ExecuteNonQuery();
                }
                db.SaveChanges();
                
            }
            finally
            {
                con.Dispose();
                con.Close();
            }*/
            var Qry = "delete from TRefDataValidation where UserId ={0}";
            db.Database.ExecuteSqlCommand(Qry, LoggedInUserId);
            db.SaveChanges();
            //save bulk data to temporary table.
            try
            {
               SaveBulkDataToDB(ReferenceId, LoggedInUserId, filename, CompanyCode, "TRefDataValidation");
            }
            catch(Exception ex)
            {
                //make entry in db.SpLogError
                //db.SpLogError("RELY-API", "LReferences", "ReadAndValidateExcelData", ex.StackTrace, "RELY", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New");
                //db.SaveChanges();
                db.SpLogError("RELY-API", "LReferences", "ReadAndValidateExcelData", ex.StackTrace, "RELY", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New", Resultforsp);
                db.SaveChanges();
                //send back error
                String ExceptionMsg = "Preliminary validation is failed.There could be several reasons for this. Please contact L2Admin.";
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, ExceptionMsg));
            }
            //Reaching here means Data is inserted into temp table and needs validation further. SpValidateTRefData does the same and returns the error row counts
            //var Result = "";
            //var Query = "Exec SpValidateTRefData @SelecterType,@LoggedInUserId,@Result output";
            //using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    conn.Open();
            //    SqlCommand cmd = new SqlCommand(Query);
            //    cmd.Connection = conn;
            //    cmd.Parameters.AddWithValue("@SelecterType", SelecterType ?? (object)DBNull.Value);
            //    cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            //    cmd.Parameters.Add("@Result", SqlDbType.Int);
            //    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            //    cmd.ExecuteNonQuery();
            //    Result = cmd.Parameters["@Result"].Value.ToString();
            //    conn.Close();
            //}
            //int ErrorRowsCount = 0;
            //if (!String.IsNullOrEmpty(Result))
            //{
            //    ErrorRowsCount = Convert.ToInt32(Result);
            //}
           // int ErrorRowsCount = 0;
            ObjectParameter Result = new ObjectParameter("errorRowCount", typeof(int)); //return parameter is declared
            db.SpValidateTRefData(SelecterType, LoggedInUserId, Result).FirstOrDefault();           
            int ErrorRowsCount = (int)Result.Value; //getting value of output parameter
            return Ok(ErrorRowsCount);
        }

        //this method calls Sp for getting Invalid records from TRefDataValidation table with server side paging
       public IHttpActionResult GetInValidDataRecords(int LoggedInUserId, string UserName, string WorkFlow, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize)
       {
            var Query = "Exec [SpGetInvalidRecordsTRefData] @LoggedInUserId,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

        // DELETE: api/LReferences/5
       // [ResponseType(typeof(LReference))]
        //public async Task<IHttpActionResult> DeleteLReference(int id, string UserName, string WorkFlow)
        //{
        //    LReference LReference = await db.LReferences.FindAsync(id);
        //    if (LReference == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "References")));
        //    }
        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            db.LReferences.Remove(LReference);
        //            await db.SaveChangesAsync();
        //            //delete reference data
        //            var RefDataList = db.LReferenceDatas.Where(a => a.ReferenceId == id).ToList();
        //            db.LReferenceDatas.RemoveRange(RefDataList);
        //            await db.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //            {
        //                //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry. 
        //                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //            }
        //            else
        //            {
        //                throw ex;
        //            }
        //        }
        //        transaction.Commit();
        //    }
        //    return Ok(LReference);
        //}


        private bool LReferenceExists(int id)
        {
            return db.LReferences.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LReferences_LReferenceData_ReferenceId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "References", "Reference Data"));
            else if (SqEx.Message.IndexOf("UQ_LReferences_ReferenceTypeId_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "References"));
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
                db.SpLogError("RELY-API", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New", Result);
                int errorid = (int)Result.Value; //getting value of output parameter
                //return Globals.SomethingElseFailedInDBErrorMessage;
                return (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid));
            }
        }
        
        private  void SaveRefDataFromGrid(string GridArray, int collength, int LoggedInUserId, int RefId)
        {
            if (!string.IsNullOrEmpty(GridArray))
            {
                //DebugEntry("Reading data from grid manual : " + GridArray);
                string GrdArray1 = GridArray.TrimEnd('}');
                string GrdArray2 = GrdArray1.TrimStart('{');
                string GrdAry = GrdArray2.TrimEnd(',');
                string[] keyValuePair = GrdAry.Split(',');
                for (var i = 0; i < keyValuePair.Length; i = i + collength)
                {
                    var LReferenceData = new LReferenceData
                    {
                        ReferenceId = RefId,
                        CreatedDateTime = DateTime.Now,
                        UpdatedDateTime = DateTime.Now,
                        CreatedById = LoggedInUserId,
                        UpdatedById = LoggedInUserId
                    };
                    for (var j = 0; j < collength; j++)
                    {
                        string[] dataList = keyValuePair[i + j].Split(':');
                        var AttributeName = dataList[0]; var AttributeValue = dataList[1];
                        PropertyInfo propertyInfo = LReferenceData.GetType().GetProperty(AttributeName);
                        var datatype = propertyInfo.PropertyType.Name;
                        if (datatype.Contains("Nullable") )
                        {
                            if (AttributeValue.Equals("undefined") || AttributeValue.Equals("null") || String.IsNullOrEmpty(AttributeValue))
                            {
                                //no need to set the value when null or undefined
                            }
                            else
                            {
                                propertyInfo.SetValue(LReferenceData, Convert.ChangeType(AttributeValue, Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
                            }
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(AttributeValue) || AttributeValue.Equals("undefined") || AttributeValue.Equals("null"))
                            {
                                //no need to set the value when null or undefined
                            }
                            else
                            {
                                propertyInfo.SetValue(LReferenceData, Convert.ChangeType(AttributeValue, propertyInfo.PropertyType), null);
                            }
                        }
                    }
                    if (db.LReferenceDatas.Where(p => p.Id == LReferenceData.Id).Count() > 0)
                    {
                        db.Entry(LReferenceData).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.LReferenceDatas.Add(LReferenceData);
                        db.SaveChanges();
                    }
                }
            }
        }

        [HttpGet]
        //this method is just for testing the SP returning values.
        public IHttpActionResult TestForConnection()
        {
            //sp returning Parameter, we need ObjectParameter.
            ObjectParameter PageUrl = new ObjectParameter("PageUrl", typeof(string));
             db.spGetHelpUrl(42,2,PageUrl).FirstOrDefault();
            string val = (string)PageUrl.Value;
            return Ok();
        }

        [HttpGet]
        //This Function will check the existence of ExtractFileName in the database
        public IHttpActionResult CheckExistenceOfExtractFileName(string ExtractFileName, string CompanyCode)
        {
            var qry = "select count(*) from LReferences where ExtractFileName={0} and CompanyCode={1}";
            var count = db.Database.SqlQuery<int>(qry, ExtractFileName, CompanyCode).FirstOrDefault();
            return Ok(count);
        }
        [HttpGet]
        public IHttpActionResult GetExtractFileNames(string CompanyCode, int Id)
        {
                List<string> Files = db.Database.SqlQuery<string>("select ExtractFileName from LReferences where  CompanyCode={0} and id not in ({1})", CompanyCode, Id).ToList();
                return Ok(Files);
            
        }
    }
}
