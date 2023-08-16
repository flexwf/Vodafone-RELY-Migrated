using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

    [CustomExceptionFilter]
    public class LAccountingScenariosController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        /*a)	delete from LFSNextStepsActions (for that ReposnseId)
        b)	LFSResponse for that Id.
        JS 23 April 2018:- In ASM change, when you press ‘Set As Blank’, then in LFSResponses, 
        it should set set it as ‘Set As Blank’ (and delete the Next Steps as it is currently doing)
        So don’t delete the LFSResponses entry.
*/
        [HttpGet]
        public async Task<IHttpActionResult> DeleteResponse(int ResponseId)
        {
            var NextStepActionsOld = db.LFSNextStepActions.Where(p => p.ResponseId == ResponseId).ToList();
            db.LFSNextStepActions.RemoveRange(NextStepActionsOld);
            await db.SaveChangesAsync();
            var Response = db.LFSResponses.Where(p => p.Id == ResponseId).FirstOrDefault();
            if (Response != null)
            {
                Response.Response = "Blank";
                db.Entry(Response).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //db.LFSResponses.Remove(Response);
                //await db.SaveChangesAsync();
            }

            return Ok();
        }

        /*When [Save] pressed, 
a)	update LFSReposnses.Response for that ReposneId with the selected value.
b)	delete from LFSNextStepsActions (for that ReposnseId)
c)	Insert into LFSNextStepsActions (for that ReposnseId) from LFSNextSteps for QuestionCode = <questionCode> and AnswerCode = <new reponse> and CompanyCode = <Opco>
*/
        [HttpGet]
        public async Task<IHttpActionResult> SaveRecommendedAccountingScenario(int AnswerBankId,int ResponseId,string CompanyCode,int LoggedinUserId)
        {
            var AnswerBank = db.LFSAnswerBanks.Where(p => p.Id == AnswerBankId).FirstOrDefault();
            var CurrentResponse = db.LFSResponses.Where(p => p.Id == ResponseId).FirstOrDefault();
            if(CurrentResponse!=null)
            {
                CurrentResponse.Response = AnswerBank.AnswerOption;
                db.Entry(CurrentResponse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var NextStepActionsOld = db.LFSNextStepActions.Where(p => p.ResponseId == ResponseId).ToList();
                db.LFSNextStepActions.RemoveRange(NextStepActionsOld);
                await db.SaveChangesAsync();
                var ListOfSteps = db.LFSNextSteps.Where(p => p.QuestionCode == CurrentResponse.QuestionCode).Where(p => p.CompanyCode == CompanyCode).Where(p => p.AnswerOption == AnswerBank.AnswerOption).ToList();
                foreach(var NextStep in ListOfSteps)
                {
                    var NextStepAction = new LFSNextStepAction { ResponseId=ResponseId,NextStepId=NextStep.Id,IsDone=false,CreatedById=LoggedinUserId,UpdatedById=LoggedinUserId,CreatedDateTime=DateTime.UtcNow,UpdatedDateTime=DateTime.UtcNow};
                    db.LFSNextStepActions.Add(NextStepAction);
                    await db.SaveChangesAsync();
                }
            }
            return Ok();
        }

        /*When any ‘Save’ pressed, 
a)	update LFSReposnses.Response for that ReposneId with the selected value.
b)	delete from LFSNextStepsActions (for that ReposnseId)
c)	Insert into LFSNextStepsActions (for that ReposnseId) from LFSNextSteps for QuestionCode = <questionCode> and AnswerCode = <new reponse> and CompanyCode = <Opco>
*/
        [HttpGet]
        public async Task<IHttpActionResult> SaveAccountingScenario(int AccountingScenarioId, int ResponseId, string CompanyCode,int LoggedinUserId)
        {
            var AccountingScenarios = db.LAccountingScenarios.Where(p => p.Id == AccountingScenarioId).FirstOrDefault();
            var CurrentResponse = db.LFSResponses.Where(p => p.Id == ResponseId).FirstOrDefault();
            if (CurrentResponse != null)
            {
                CurrentResponse.Response = AccountingScenarios.Reference;
                db.Entry(CurrentResponse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var NextStepActionsOld = db.LFSNextStepActions.Where(p => p.ResponseId == ResponseId).ToList();
                db.LFSNextStepActions.RemoveRange(NextStepActionsOld);
                await db.SaveChangesAsync();
                var ListOfSteps = db.LFSNextSteps.Where(p => p.QuestionCode == CurrentResponse.QuestionCode).Where(p => p.CompanyCode == CompanyCode).Where(p => p.AnswerOption == AccountingScenarios.Reference).ToList();
                foreach (var NextStep in ListOfSteps)
                {
                    var NextStepAction = new LFSNextStepAction { ResponseId = ResponseId, NextStepId = NextStep.Id, IsDone = false, CreatedById = LoggedinUserId, UpdatedById = LoggedinUserId, CreatedDateTime = DateTime.UtcNow, UpdatedDateTime = DateTime.UtcNow };
                    db.LFSNextStepActions.Add(NextStepAction);
                    await db.SaveChangesAsync();
                }
            }
            return Ok();
        }

        /*1.	Basic/Special tab will come from LaccountingScenarios.Category: This tab will list all accounting scenario recorded against a product in LFSResponses (Answersheet) (for questions that are type AS_QUESTION)
         Select from LFSResponses 
inner join LFSSectionItems on QuestionCode
inner Join LFSOTEMTYPES on ItemTypeId
And LaccountingScenario (Inner join on Response = Reference) .
where ItemType = AS_QUESTION
and productId in <product list>
and EntityType = Lproducts
and  LaccountingScenario.category = ‘Basic’ (for Special tab, change it to ‘Special’ or Null)
             */
        /*Select 
-- LQ.QuestionName, --> Replace this with connected QuestionCode
LQR.QuestionName,
-- lq.ReadableName,--> Replace this with connected QuestionCode
LQR.ReadableName,
LR.Id as ResponseId,LAS.Category,LQ.QuestionCode,LR.ObjectType,
	RS.SysCat+'-'+lp.ProductCode as SysCat,
	LR.Response,LAS.ScenarioDescription,LR.Comments 
	from LFSResponses LR 
	inner join LFSQuestionBank LQ on (LQ.QuestionCode=LR.QuestionCode and LQ.CompanyCode='DE') -- For each response (accounting scenario), find the QuestionCode
	inner Join LFSItemTypes LIT on LIT.Id=LQ.ItemTypeId -- For the Question Code, find the ItemType
	inner join LProducts LP on (LP.Id=LR.EntityId and LR.EntityType='LProducts') -- For each response (accounting scenario), find the connected product
	inner join RSysCat RS on RS.Id=LP.SysCatId -- For each product, find the associated SysCat
	Inner join LAccountingScenarios LAS on LAS.Reference = LR.Response -- For each response (accounting scenario), find the accounting scenario details
	right Outer join LFSSectionItems LST on (LST.QuestionCode = LQ.QuestionCode and LST.SurveyId = LP.SurveyId) -- For each question, find the configuration (so you can find the related question). Outer Join just in case RelatedQuestionCode is not provided in config
    left Outer Join LFSQuestionBank LQR on (LQR.QuestionCode=LST.RelatedQuestionCode and LQR.CompanyCode='DE') -- For the related question, find its QuestionName, ReadableText that will be used in the Matrix instead. 
	where LIT.Name = 'AS_QUESTION' 
	and LP.Id in (select Id from LProducts where RequestId=1)*/
        public IHttpActionResult GetAccountingScenarioMatrix(int EntityId, string EntityType, string TabName,string CompanyCode)
        {
            //For Overview tab we need to call the stored procedure FnGenerateAccountingScenarioMatrix. Rest of the tabs are handled differently throughh direct queries.
            if (TabName == "Overview") 
            {
                //var ProductList = string.Join(",",db.LProducts.Where(p => p.RequestId == RequestId).Select(p => p.Id));
                //Call SP To Generate Overview Matrix
                db.Database.ExecuteSqlCommand("Exec SpGenerateAccountingScenarioMatrix {0},{1},{2},{3}", EntityId, EntityType,CompanyCode,TabName);
                //01-03-2018-SS-AS Disscussed with VG we are using ADO to get Data from LAccounting ScenarioMatrix table and we will switch to MVC once table comes in model
                var dataTable = new DataTable();
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "SELECT EntityType, EntityId, QuestionCode, Situation, ObjectType, Product, Scenario, ScenarioDescription, Comments,Category FROM LFSAccountingScenarioMatrix WHERE EntityId = @EntityId AND EntityType = @EntityType";
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@EntityId", System.Data.SqlDbType.Int);
                cmd.Parameters.Add("@EntityType", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EntityId"].Value = EntityId;
                cmd.Parameters["@EntityType"].Value = EntityType;
                conn.Open();
                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to  datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
                return Ok(dataTable);
            }
            else //It Means, the tab name is not "Overview" and ew don't need to call stored procedure.
            {
                var dataTable = new DataTable();
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "Exec SpGenerateAccountingScenarioMatrix @EntityId,@EntityType,@CompanyCode,@TabName";
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@EntityId", System.Data.SqlDbType.Int);
                cmd.Parameters.Add("@EntityType", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EntityId"].Value = EntityId;
                cmd.Parameters["@EntityType"].Value = EntityType;
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@TabName", TabName);
                conn.Open();
                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to  datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
                return Ok(dataTable);
                //code migrated to Stored procedure
                //var ACCQuery = "Select LR.Id as ResponseId,LAS.Category,LQ.QuestionCode,lq.ReadableName,LR.ObjectType,RS.SysCat+'-'+lp.ProductCode as SysCat,LR.Response,LAS.ScenarioDescription,LR.Comments from LFSResponses LR inner join LFSQuestionBank LQ on (LQ.QuestionCode=LR.QuestionCode and LQ.CompanyCode={0}) inner Join LFSItemTypes LIT on LIT.Id=LQ.ItemTypeId inner join LProducts LP on (LP.Id=LR.EntityId and LR.EntityType='LProducts') inner join RSysCat RS on RS.Id=LP.SysCatId Inner join LAccountingScenarios LAS on LAS.Reference = LR.Response where LIT.Name = 'AS_QUESTION' and LP.Id in (select Id from LProducts where RequestId={1}) ";
                /*var ACCQuery = "";
                if (EntityType == "LRequests")//removed LR.ObjectType from both the queries
                {
                    ACCQuery = "Select Lp.Id as ProductId ,LQR.QuestionName,LQR.ReadableName,LR.Id as ResponseId,LAS.Category,LQ.QuestionCode,LAS.ObjectType,lp.Name +'('+lp.ProductCode + ')'+ '-'+ RS.SysCat as SysCat,LR.Response,LAS.ScenarioDescription,LR.Comments from LFSResponses LR inner join LFSQuestionBank LQ on (LQ.QuestionCode=LR.QuestionCode and LQ.CompanyCode={0}) inner Join LFSItemTypes LIT on LIT.Id=LQ.ItemTypeId inner join LProducts LP on (LP.Id=LR.EntityId and LR.EntityType='LProducts') inner join RSysCat RS on RS.Id=LP.SysCatId Inner join LAccountingScenarios LAS on (LAS.Reference = LR.Response) right Outer join LFSSectionItems LST on (LST.QuestionCode = LQ.QuestionCode and LST.SurveyId = LP.SurveyId) left Outer Join LFSQuestionBank LQR on (LQR.QuestionCode=LST.RelatedQuestionCode and LQR.CompanyCode={0}) where LIT.Name = 'AS_QUESTION' and LP.Id in (select Id from LProducts where RequestId={1}) ";
                }
                else if (EntityType == "LProducts")
                {
                    ACCQuery = "Select Lp.Id as ProductId,LQR.QuestionName,LQR.ReadableName,LR.Id as ResponseId,LAS.Category,LQ.QuestionCode,LAS.ObjectType,lp.Name +'('+lp.ProductCode + ')'+ '-'+ RS.SysCat as SysCat,LR.Response,LAS.ScenarioDescription,LR.Comments from LFSResponses LR inner join LFSQuestionBank LQ on (LQ.QuestionCode=LR.QuestionCode and LQ.CompanyCode={0}) inner Join LFSItemTypes LIT on LIT.Id=LQ.ItemTypeId inner join LProducts LP on (LP.Id=LR.EntityId and LR.EntityType='LProducts') inner join RSysCat RS on RS.Id=LP.SysCatId Inner join LAccountingScenarios LAS  on (LAS.Reference = LR.Response) right Outer join LFSSectionItems LST on (LST.QuestionCode = LQ.QuestionCode and LST.SurveyId = LP.SurveyId) left Outer Join LFSQuestionBank LQR on (LQR.QuestionCode=LST.RelatedQuestionCode and LQR.CompanyCode={0}) where LIT.Name = 'AS_QUESTION' and LP.Id in ({1}) ";
                }
                if (TabName == "Special")
                {
                    ACCQuery += " and  (LAS.category = 'Special' or LAS.Category is null) and LAS.WFStatus='Completed' ";
                }
                else if (TabName == "Basic")
                {
                    ACCQuery += " and  (LAS.category = 'Basic') and LAS.WFStatus='Completed' ";
                }
                var AccScenarioMatrix = db.Database.SqlQuery<GetAccountingScenarioMatrixViewModel>(ACCQuery, CompanyCode, EntityId).ToList();
                return Ok(AccScenarioMatrix);*/

            }
        }

        public IHttpActionResult GetAccountingScenarioByResponseId(int ResponseId,string CompanyCode)
        {
            var ACCQuery = "Select LR.Id as ResponseId,LAS.Category,LQ.QuestionCode,LQ.QuestionName,lq.ReadableName,LAS.ObjectType,RS.SysCat+'-'+lp.ProductCode as SysCat,LR.Response,LAS.ScenarioDescription,LR.Comments from LFSResponses LR inner join LFSQuestionBank LQ on (LQ.QuestionCode=LR.QuestionCode and LQ.CompanyCode={0}) inner Join LFSItemTypes LIT on LIT.Id=LQ.ItemTypeId inner join LProducts LP on (LP.Id=LR.EntityId and LR.EntityType='LProducts') inner join RSysCat RS on RS.Id=LP.SysCatId Inner join LAccountingScenarios LAS on LAS.Reference = LR.Response where LIT.Name = 'AS_QUESTION' and LR.Id={1} ";
            var AccScenarioMatrix = db.Database.SqlQuery<GetAccountingScenarioMatrixViewModel>(ACCQuery, CompanyCode, ResponseId).FirstOrDefault();
            return Ok(AccScenarioMatrix);
        }

        [HttpGet]
        public IHttpActionResult GetRecommendedAccountingScenarioByResponseId(int ResponseId,string CompanyCode)
        {
            string QuestionCode = db.LFSResponses.Where(p => p.Id == ResponseId).Select(p => p.QuestionCode).FirstOrDefault();
            var AnswerList = db.LFSAnswerBanks.Where(p => p.CompanyCode == CompanyCode).Where(p => p.QuestionCode == QuestionCode).Select(p=>new { p.Id,p.AnswerOption,p.MemoText}).ToList();
            return Ok(AnswerList);
        }

        [HttpGet]
        public IHttpActionResult GetAccountingScenarioByCompanyCode(string CompanyCode)
        {
            var AccScenarioList = db.LAccountingScenarios.Where(p => p.CompanyCode == CompanyCode).Select(p => new { p.Id, p.BusinessAreaId,p.Category,p.ContractType,p.Dataset,p.PaymentMethod,p.Reference,
            p.ProductsIncluded,p.ScenarioDescription,p.Standards,p.Status,p.Subsidies}).ToList();
            return Ok(AccScenarioList);
        }

        [HttpGet]
        public IHttpActionResult SaveResponseComments(int ResponseId,string Comments)
        {
            var LFSResponse = db.LFSResponses.Find(ResponseId);
            if (LFSResponse != null)
            {
                LFSResponse.Comments = Comments;
                db.Entry(LFSResponse).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Ok();
        }


       // [HttpGet]
        // public IHttpActionResult SaveManualAccountingScenario()
        [HttpPost]
        public IHttpActionResult SaveManualAccountingScenario(object[] FormData,string CompanyCode)
           

        {
            //string[] FormData = new string[1];
            //FormData[0] = "0,manual_test,Product1207,RS-1,manual_test,sedc,,0,manual_test,Product1207,RS-1,manual_test,sedc,";
            //var CompanyCode = "DE";
            using (var transaction = db.Database.BeginTransaction())
            {
               // var ProductList = db.LProducts.Where(p => p.CompanyCode == CompanyCode).ToList();
                

                for (var i = 0; i < FormData.Length; i++)
                {
                    var ModelData = FormData[0].ToString().Split(',');

                   
                    
                    for (var j = 0; j < ModelData.Length; j += 7)
                    {
                        //delete from Previos records
                      
                        // var EntityId = (string.IsNullOrEmpty(ModelData[j + 2])) ? 0 : ProductList.Where(p => p.Name == ModelData[j + 2]).Select(p => p.Id).FirstOrDefault();
                       
                        //var ExistingData = db.LFSManualAccScenarios.Where(p => p.EntityType == "LProducts").Where(p => p.EntityId == EntityId).ToList();
                        //db.LFSManualAccScenarios.RemoveRange(ExistingData);
                        //db.SaveChanges();

                        var LManualAccScenario = new LFSManualAccScenario();
                        LManualAccScenario.Id = (string.IsNullOrEmpty(ModelData[j])) ? 0 : Convert.ToInt32(ModelData[j]);
                        LManualAccScenario.ObjectType = (string.IsNullOrEmpty(ModelData[j + 1])) ? null : ModelData[j + 1];
                        LManualAccScenario.EntityId = (string.IsNullOrEmpty(ModelData[j+2])) ? 0 : Convert.ToInt32(ModelData[j+2]);
                        // LManualAccScenario.EntityId = EntityId;
                        LManualAccScenario.Reference = (string.IsNullOrEmpty(ModelData[j + 3])) ? null : ModelData[j + 3];
                        LManualAccScenario.Situation = (string.IsNullOrEmpty(ModelData[j + 4])) ? null : ModelData[j + 4];
                        LManualAccScenario.Description = (string.IsNullOrEmpty(ModelData[j + 5])) ? null : ModelData[j + 5];
                        LManualAccScenario.Comments = (string.IsNullOrEmpty(ModelData[j + 6])) ? null : ModelData[j + 6];
                        LManualAccScenario.EntityType = "LProducts";

                        //In case of new rows,row should be added and in case of editing, rows should be modified
                        if (db.LFSManualAccScenarios.Where(p => p.Id == LManualAccScenario.Id).Count() > 0)
                        {
                            db.Entry(LManualAccScenario).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.LFSManualAccScenarios.Add(LManualAccScenario);

                            db.SaveChanges();
                        }
                    }
                }
                transaction.Commit();
            }
            return Ok();
        }

        //[HttpGet]
        //public IHttpActionResult GetOverviewAccountingScenario(int EntityId, string EntityType, int LoggedInUserId,string CompanyCode)
        //{
        //    //var ProductList = string.Join(",",db.LProducts.Where(p => p.RequestId == RequestId).Select(p => p.Id));
        //    //Call SP To Generate Overview Matrix
        //    db.Database.ExecuteSqlCommand("Exec SpGenerateAccountingScenarioMatrix "+LoggedInUserId+","+EntityId+",'"+EntityType+"','"+CompanyCode+"'");
        //    //01-03-2018-SS-AS Disscussed with VG we are using ADO to get Data from LAccounting ScenarioMatrix table and we will switch to MVC once table comes in model
        //    var dataTable = new DataTable();
        //    string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    string query = "select * from LFSAccountingScenarioMatrix";
        //    SqlConnection conn = new SqlConnection(connString);
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    conn.Open();
        //    // create data adapter
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    // this will query your database and return the result to  datatable
        //    da.Fill(dataTable);
        //    conn.Close();
        //    da.Dispose();
        //    return Ok(dataTable);
                
        //}
        /*Shows editable Grid from LFSManualAccScenarios (where EntityId in (list of productId where RequestId = <RequestId passed from form> and EntityType = ‘Lproducts’)) with Action tab with a Bin Gymph to delete any manually added scenarios. And ability to add row*/
        public IHttpActionResult GetManualAccountingScenario(int EntityId, string EntityType)
        {
            if (EntityType == "LRequests")
            {
                var ProductList = db.LProducts.Where(p => p.RequestId == EntityId).Select(p => p.Id).ToList();
                var ManualAcc = from aa in db.LFSManualAccScenarios.Where(p => p.EntityType == "LProducts" && ProductList.Contains(p.EntityId))
                                join bb in db.LProducts on aa.EntityId equals bb.Id
                                select new { aa.Comments, aa.Description, aa.EntityId, aa.EntityType, aa.Id, aa.ObjectType, aa.Reference, aa.Situation, Product = bb.Name };
                return Ok(ManualAcc);
            }
            else if (EntityType == "LProducts")
            {
                var ManualAcc = from aa in db.LFSManualAccScenarios.Where(p => p.EntityType == "LProducts" && p.EntityId==EntityId)
                                join bb in db.LProducts on aa.EntityId equals bb.Id
                                select new { aa.Comments, aa.Description, aa.EntityId, aa.EntityType, aa.Id, aa.ObjectType, aa.Reference, aa.Situation, Product = bb.Name };
                return Ok(ManualAcc);
            }
            return Ok();
        }

        public IHttpActionResult GetByCompanyCode(string CompanyCode)
        {
            var xx = db.LAccountingScenarios.Where(p => p.CompanyCode == CompanyCode).Select(p=>new {p.BusinessAreaId,p.Category,p.CommentInbound,p.ContractType,p.ObjectType,p.PaymentMethod,p.ProductsIncluded,p.Reference,p.ScenarioDescription }).ToList();
            return Ok(xx);
        }
        public IHttpActionResult GetCompletedList(string CompanyCode)
        {
            var xx = db.LAccountingScenarios.Where(p => p.WFStatus == "Completed").Where(p => p.CompanyCode == CompanyCode).Select(p => new { p.Id,p.BusinessAreaId, p.Category, p.CommentInbound, p.ContractType, p.ObjectType, p.PaymentMethod, p.ProductsIncluded, p.Reference, p.ScenarioDescription }).ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetByQuestionCodeCompanyCode(string QuestionCode, string CompanyCode)
        {
            var AnswerOptions = db.LFSAnswerBanks.Where(p => p.QuestionCode == QuestionCode).Where(p => p.CompanyCode == CompanyCode).Select(a=> a.AnswerOption).ToList();
            var xx = (from aa in db.LAccountingScenarios where AnswerOptions.Contains(aa.Reference)
                      join bb in db.LFSAnswerBanks on aa.Reference equals bb.AnswerOption
                      select new
                      {
                          aa.Id,aa.Reference,aa.ScenarioDescription,aa.Standards,aa.PaymentMethod,aa.ProductsIncluded,aa.Status,aa.Subsidies,aa.CreatedById,aa.UpdatedById,aa.CreatedDateTime,aa.UpdatedDateTime,
                          ScenarioTrigger = bb.ScenarioTrigger,
                          ScenarioCategory = bb.ScenarioCategory
                      }).ToList();
            return Ok(xx);
        }


        public IHttpActionResult GetById(int Id)
        {
            string sqlQuery = "Select * from LAccountingScenarios where Id = {0}";
            var LAccountingScen = db.Database.SqlQuery<LAccountingScenario>(sqlQuery, Id).FirstOrDefault();
            //var xx = db.LAccountingScenarios.Where(p => p.Id == Id).FirstOrDefault();
            //return Ok(xx);
            if (LAccountingScen == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AccountingScenario")));
            }
            return Ok(LAccountingScen);
        }
        public IHttpActionResult GetByReference(string Reference)
        {
            var xx = db.LAccountingScenarios.Where(p => p.Reference == Reference).FirstOrDefault();
            return Ok(xx);
        }

        public async Task<IHttpActionResult> PostData(LAccountingScenario model, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    
                    var Action = db.WActions.Where(p => p.Name == "Create").FirstOrDefault();
                    var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                    model.Id = 0;
                    db.LAccountingScenarios.Add(model);
                    await db.SaveChangesAsync();
                    //string fixedComments = "";
                    //if (!string.IsNullOrEmpty(model.AttributeC20))
                    //{
                    //    if (model.AttributeC20.Equals("Change"))
                    //    {
                    //        fixedComments = "Opened for Changes";
                    //    }
                    //    else if (model.AttributeC20.Equals("Create"))
                    //    {
                    //        fixedComments = "Created";
                    //    }
                    //}

                    //string AuditComments = fixedComments + " (Status " + model.WFStatus + ") " + model.WFComments;
                    //Comment format updated - 10Oct2018- Andre raised bug for duplicacy of information.
                    string AuditComments = model.WFComments;
                    var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", model.WFOrdinal, model.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                    db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Create",
                           "Create", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                           "LAccountingScenarios", model.Id, model.Reference, WorkflowDetails.Id, model.CompanyCode, AuditComments, StepId, Action.Label, null);
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
                                EntityType = "LAccountingScenarios",
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
                           "LAccountingScenarios", model.Id, model.Reference, WorkflowDetails.Id, model.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
                            db.SaveChanges();
                        }
                    }
                    MEntityPortfolio MEP = new MEntityPortfolio();
                    MEP.EntityId = model.Id;
                    MEP.EntityType = "LAccountingScenarios";
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
        public async Task<IHttpActionResult> PutData(int id, LAccountingScenario model, string UserName, string WorkFlow, string FileList, string SupportingDocumentsDescription, string FilePath, string OriginalFileNameList)
        {
           
            if (!LAccountingScenarioExists(id))
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
                //if (!string.IsNullOrEmpty(model.AttributeC20))
                //{

                //    if (model.AttributeC20.Equals("Edit"))
                //    {
                //        fixedComments = "Edited";
                //    }
                //}

                //string AuditComments = fixedComments + " (Status " + model.WFStatus + ") " + model.WFComments;
                //Comment format updated - 10Oct2018- Andre raised bug for duplicacy of information.
                string AuditComments = model.WFComments;
                var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == WorkFlow).Where(p => p.CompanyCode == model.CompanyCode).FirstOrDefault();
                var StepId = db.Database.SqlQuery<int?>("select Id from WSteps where Ordinal={0} and CompanyCode={1} and WorkFlowId={2}", model.WFOrdinal, model.CompanyCode, WorkflowDetails.Id).FirstOrDefault();
                db.SpLogAudit(WorkFlow, WorkFlow, "Audit", string.Empty, "Update",
                       "Update", model.UpdatedById, model.WFRequesterRoleId, DateTime.UtcNow, model.WFStatus, model.WFStatus,
                       "LAccountingScenarios", model.Id, model.Reference, WorkflowDetails.Id, model.CompanyCode, AuditComments, StepId, Action.Label, null);
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
                            EntityType = "LAccountingScenarios",
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
                       "LAccountingScenarios", model.Id, model.Reference, WorkflowDetails.Id, model.CompanyCode, SupportingDocument.Description, StepId, Action.Label, SupportingDocument.Id);
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

        private bool LAccountingScenarioExists(int id)
        {
            return db.LAccountingScenarios.Count(e => e.Id == id) > 0;
        }


        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LAccountingScenarios_LASLifeCycleEvents_AccountingScenarioId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ACCOUNTING SCENARIOS", "LIFE CYCLE EVENTS"));
            else if (SqEx.Message.IndexOf("UQ_LAccountingScenario_Reference", StringComparison.OrdinalIgnoreCase) >= 0)
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