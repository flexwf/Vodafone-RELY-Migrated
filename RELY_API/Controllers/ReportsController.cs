using Newtonsoft.Json;
using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class ReportsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();


        // GET: api/GetWftype? BaseTableName
        //Method to Get Data in Grid
        public IHttpActionResult GetWftype(string BaseTableName, string UserName, string WorkFlow, string CompanyCode)
        {
            var xx = (from aa in db.RWorkFlows.Where(p => p.BaseTableName == BaseTableName).Where(p => p.CompanyCode == CompanyCode)
                      select new
                      {
                          aa.WFType
                      }).OrderBy(p => p.WFType);
            return Ok(xx);
        }


        // GET: api/GetRequestName? CompanyCode
        //Method to Get Data in Grid
        public IHttpActionResult GetRequestName(string CompanyCode)
        {
            var query = "select Id,Name from LRequests where CompanyCode={0} order by Name";
            var RequestName = db.Database.SqlQuery<LRequestsViewModel>(query, CompanyCode).ToList();
            return Ok(RequestName);
        }

        // GET: api/GetRequestPerStatus? CompanyCode,WFType
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetRequestPerStatus(string CompanyCode,string WFType)
        {

            string strQuery = "Exec SPRPTRequestPerStatus @CompanyCode,@WFType";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@WFType",WFType);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }

        // GET: api/GetRequestsPerDateInterval? CompanyCode
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetRequestsPerDateInterval(string CompanyCode)
        {
            string strQuery = "Exec SPRPTRequestsPerDateInterval @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }

        // GET: api/GetRequestDetails? CompanyCode,Id
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetRequestDetails(string CompanyCode,int Id)
        {
            string strQuery = "Exec SPRPTRequestDetails @CompanyCode,@RequestId";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@RequestId", Id);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }

        // GET: api/GetRequestsInProgress? CompanyCode
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetRequestsInProgress(string CompanyCode,int Interval,int NumberofBuckets)
        {
            string strQuery = "Exec SPRPTRequestsInProgress @CompanyCode,@Interval,@NumberofBuckets";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@Interval", Interval);
            cmd.Parameters.AddWithValue("@NumberofBuckets", NumberofBuckets);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }

        // GET: api/GetReportOfProducts? CompanyCode
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetReportOfProducts(string CompanyCode, string FilterType,string FilterValue)
        {
            string strQuery = "Exec [SpReportOfProduct] @CompanyCode,@FilterType,@FilterValue";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@FilterType", FilterType);
            cmd.Parameters.AddWithValue("@FilterValue", FilterValue);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);           
        }     
        
        [HttpGet]
        public IHttpActionResult DownloadReportOfProducts(string CompanyCode, string FilterType, string FilterValue)
        {           
            string strQuery = "Exec [SpReportOfProduct] @CompanyCode,@FilterType,@FilterValue";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@FilterType", FilterType);
            cmd.Parameters.AddWithValue("@FilterValue", FilterValue);
            var dt = new DataTable();
            dt = GetData(cmd);  
            
            string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
            string Filename = "ReportOfProducts_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            if (dt != null)
            {
                if (dt.Columns.Count > 0)
                {
                    /*renaming of the columns in downloaded file as to match with the front end.*/
                    dt.Columns.Remove("SysCatId");
                    dt.Columns.Remove("ProductId");
                    dt.Columns.Remove("ProductPobId");
                    //dt.Columns.Remove("LocalpobId");
                    //dt.Columns.Remove("Copa2Id");
                    //dt.Columns.Remove("Copa5Id");
                    //dt.Columns.Remove("Copa2Id(Ret)");
                    //dt.Columns.Remove("Copa5Id(Ret)");
                    dt.Columns.Remove("RequestId");
                    //dt.Columns.Remove("GlobalPob1");
                    //dt.Columns.Remove("GlobalPob2");

                    //dt.Columns["Copa2Ret"].ColumnName = "Copa2(Ret)";
                    //dt.Columns["Copa5Ret"].ColumnName = "Copa5(Ret)";

                    dt.Columns["ProductName"].SetOrdinal(0);
                    dt.Columns["ProductCode"].SetOrdinal(1);
                    dt.Columns["SysCat"].SetOrdinal(2);
                    dt.Columns["RequestName"].SetOrdinal(3);
                    dt.Columns["LocalPOBName"].SetOrdinal(4);
                    //SG - 9 Jan 2019 - hardcoded columns are removed as raised exception in PROD

                    //dt.Columns["Article Number(LocalPobs)"].SetOrdinal(5);
                    //dt.Columns["Base Price Id(Products)"].SetOrdinal(6);
                    //dt.Columns["Bundle Indicator(LocalPobs)"].SetOrdinal(7);
                    //dt.Columns["Business Category(Products)"].SetOrdinal(8);
                    //dt.Columns["Ctr Duration(Products)"].SetOrdinal(9);
                    //dt.Columns["Demo(LocalPobs)"].SetOrdinal(10);
                    //dt.Columns["paym. %(ProductPobs)"].SetOrdinal(11);
                    //dt.Columns["Pob Indicator(LocalPobs)"].SetOrdinal(12);
                    //dt.Columns["Special Indicator(LocalPobs)"].SetOrdinal(13);
                    //dt.Columns["SSP Amount(LocalPobs)"].SetOrdinal(14);
                    //dt.Columns["SSP Amount(Products)"].SetOrdinal(15);
                    //dt.Columns["Start Date(LocalPobs)"].SetOrdinal(16);
                    //dt.Columns["Start Date(Products)"].SetOrdinal(17);
                    //dt.Columns["Usage Indicator(LocalPobs)"].SetOrdinal(18);
                    dt.Columns["GlobalPobName"].SetOrdinal(5);
                    //dt.Columns["GlobalPobNameRet"].SetOrdinal(6);
                    dt.Columns["CopaDimension"].SetOrdinal(6);
                    //dt.Columns["Copa5"].SetOrdinal(7);
                    //dt.Columns["Copa2(Ret)"].SetOrdinal(9);
                    //dt.Columns["Copa5(Ret)"].SetOrdinal(10);

                    Globals.ExportToExcel(dt, path, Filename);
                }
            }//End of (if (dt != null))
            if (!string.IsNullOrEmpty(Filename))
            {
                string fullpath = path + "\\" + Filename;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            DownloadFileNameViewModel model = new DownloadFileNameViewModel { FileName = Filename }; /*As Restclient throws error while returning string to return filename only,so returning filename in new viewmodel */
            return Ok(model);           
        }



        // GET: api/GetNewScenarioDemand? CompanyCode
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetNewScenarioDemand(string CompanyCode, int Interval, int NumberofBuckets)
        {
            string strQuery = "Exec SpRPTScenarioDemand @CompanyCode,@Interval,@NumberofBuckets";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@Interval", Interval);
            cmd.Parameters.AddWithValue("@NumberofBuckets", NumberofBuckets);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }


        // GET: api/GetStatus? BaseTableName and CompanyCode
        //Method to Get Data in Grid
        public IHttpActionResult GetStatus(string BaseTableName, string CompanyCode,string UserName, string WorkFlow)
        {
            var query = "select distinct WS.Name from LRequests as LR inner join RWorkFlows as RWF on LR.WFType = RWF.WFType inner join WSteps as WS on RWF.Id = WS.WorkFlowId and LR.WFOrdinal = WS.Ordinal where RWF.BaseTableName = {0} and WS.CompanyCode = {1}";
            var Status = db.Database.SqlQuery<WFColumnsViewModel>(query, BaseTableName, CompanyCode).ToList();
            return Ok(Status);
        }

        // GET: api/GetAccountingScenarios? CompanyCode
        //Method to Get Data in Grid
        [HttpGet]
        public IHttpActionResult GetAccountingScenarios(string CompanyCode,string StartDate,string EndDate)
        {
            string strQuery = "Exec SpRPTAccountingScenarios @CompanyCode,@StartDate,@EndDate";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            var dt = new DataTable();
            // dt.Load(dataReader);
            dt = GetData(cmd);
            return Ok(dt);
        }
        [HttpGet]
        public IHttpActionResult DownloadAccountingScenariosList(string CompanyCode)
        {
            string strQuery1 = "Exec SpRPTAccountingScenarioList @CompanyCode";
            string strQuery2 = "Exec SpRPTLifeCycleEvent";
            SqlCommand cmd1 = new SqlCommand(strQuery1);
            SqlCommand cmd2 = new SqlCommand(strQuery2);
            cmd1.Parameters.AddWithValue("@CompanyCode", CompanyCode);

            var dtAccountingScenario = new DataTable();
            var dtLifeCycleEvent = new DataTable();
            // dt.Load(dataReader);
            dtAccountingScenario = GetData(cmd1);
            dtLifeCycleEvent = GetData(cmd2);
            
            string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
            
            string FileName= "AccountingScenarioList__" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";

            if (dtAccountingScenario.Rows.Count > 0 && dtLifeCycleEvent.Rows.Count == 0)
            {
                DataTable newdtAccountingScenario = dtAccountingScenario.DefaultView.ToTable(false, "CompanyCode", "Reference", "BusinessArea", "ScenarioDescription", "Standards", "Dataset", "ContractType", "PaymentMethod", "ProductsIncluded", "Subsidies", "Status", "Categorization", "Version", "InboundFlow", "CoreD03Ref", "Notes", "PWCReview", "Wave", "Completed", "OutboundFlow", "IncludedinPOC", "SAPRARResponse");
                Globals.ExportToExcelforAccountingscenariolist(newdtAccountingScenario, dtLifeCycleEvent, path, FileName,"Sheet1","Sheet2");

            }

            if (dtLifeCycleEvent.Rows.Count > 0 && dtAccountingScenario.Rows.Count > 0)
            {
                DataTable newdtLifeCycleEvent = dtLifeCycleEvent.DefaultView.ToTable(false, "AccountingScenario", "Ordinal", "Nature", "Event", "Notes");
                DataTable newdtAccountingScenario = dtAccountingScenario.DefaultView.ToTable(false, "CompanyCode", "Reference", "BusinessArea", "ScenarioDescription", "Standards", "Dataset", "ContractType", "PaymentMethod", "ProductsIncluded", "Subsidies", "Status", "Categorization", "Version", "InboundFlow", "CoreD03Ref", "Notes", "PWCReview", "Wave", "Completed", "OutboundFlow", "IncludedinPOC", "SAPRARResponse");

                Globals.ExportToExcelforAccountingscenariolist(newdtAccountingScenario, newdtLifeCycleEvent, path, FileName, "Sheet1", "Sheet2");


            }
            else
            {

                Globals.ExportToExcelforAccountingscenariolist(dtAccountingScenario, dtLifeCycleEvent, path, FileName, "Sheet1", "Sheet2");



            }


            if (!string.IsNullOrEmpty(FileName))
            {
                string fullpath = path + "\\" + FileName;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = FileName };
            return Ok(model);

            
               
        }




        public IHttpActionResult GetAccountingScenarioList(string CompanyCode)
        {
            var xx = (from aa in db.LAccountingScenarios.Where(p => p.CompanyCode == CompanyCode)
                      join bb in db.LDropDownValues on aa.BusinessAreaId equals bb.Id
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.Reference,
                          BusinessArea = bb.Value,
                         aa.BusinessAreaId,
                          aa.ScenarioDescription,
                          aa.Standards,
                          aa.Dataset,
                          aa.ContractType,
                          aa.PaymentMethod,
                          aa.ProductsIncluded,
                          aa.Subsidies,
                          aa.WFStatus,
                          aa.CreatedDateTime,
                          aa.WFStatusDateTime,
                          aa.WFIsReadyDateTime

                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }


        private DataTable GetData(SqlCommand cmd)
        {
            DataTable dt = new DataTable();

            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            SqlConnection con = new SqlConnection(strConnString);

            SqlDataAdapter sda = new SqlDataAdapter();
           
            cmd.CommandType = CommandType.Text;

            cmd.Connection = con;

            try

            {

                con.Open();
                //cmd.CommandTimeout = 600;
                sda.SelectCommand = cmd;

                sda.Fill(dt);

                return dt;

            }
            //Lock Exception in GErrorLog
            catch (Exception ex)
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
                ObjectParameter Result = new ObjectParameter("Result", typeof(int)); //return parameter is declared
                db.SpLogError("RELY-API", "Reports", "GetData", ex.Message, "RELY_API", "Type1", ex.StackTrace, "resolution", "L2Admin", "field", 0, "New", Result);
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult DownloadAuditReports(string CompanyCode)
        {
            string strQuery1 = "Exec P_GetRequestAuditReports @CompanyCode";
            string strQuery2 = "Exec P_GetProductLevelAuditReport @CompanyCode";
            SqlCommand cmd1 = new SqlCommand(strQuery1);
            SqlCommand cmd2 = new SqlCommand(strQuery2);
            cmd1.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd2.Parameters.AddWithValue("@CompanyCode", CompanyCode);

            //Product audit
            string qryProduct = "select   Req.Name as RequestName,AuthorizationNumber,lp.ProductCode,lp.Name as ProductDescription," +
                "StepName = ss.Name ,Action = la.ActionLabel,ActionBy = lu.LoginEmail,Role = lr.RoleName,ActionOn = la.ActionDateTime,Comments = la.Comments" +
                " from LRequests req inner join LProducts lp on lp.RequestId = req.Id inner join LAudit la on la.EntityId = lp.Id and la.EntityType = 'LProducts'" +
                " inner join LUsers lu on lu.Id = la.ActionedById inner join LRoles lr on lr.Id = la.ActionedByRoleId left outer join WSteps ss on la.StepId = ss.Id " +
                " where req.CompanyCode = @CompanyCode order by RequestName,ProductCode,ActionOn";
            SqlCommand cmd3 = new SqlCommand(qryProduct);
            cmd3.Parameters.AddWithValue("@CompanyCode", CompanyCode);

            //LocalPob audit
            string qryLPob = "select   Req.Name as RequestName,AuthorizationNumber,lp.ProductCode,lp.Name as ProductDescription,ll.Name as LPobName, " +
                "StepName = ss.Name ,Action = la.ActionLabel,ActionBy = lu.LoginEmail,Role = lr.RoleName,ActionOn = la.ActionDateTime,Comments = la.Comments from LRequests req  " +
                " inner join  LProducts lp on lp.RequestId = req.Id inner join  LProductPobs pob on pob.ProductId = lp.Id inner join  LLocalPobs ll on pob.PobCatalogueId = ll.PobCatalogueId inner join LAudit la on la.EntityId = ll.Id and la.EntityType = 'LLocalPobs'  " +
                " inner join LUsers lu on lu.Id = la.ActionedById inner join LRoles lr on lr.Id = la.ActionedByRoleId left outer join WSteps ss on la.StepId = ss.Id where req.CompanyCode = @CompanyCode order by RequestName,ProductCode,LPobName,ActionOn";
            SqlCommand cmd4 = new SqlCommand(qryLPob);
            cmd4.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                        
            var dtRequestLevel = GetData(cmd1);
            var dtProductLevel = GetData(cmd2);
            var dtProduct = GetData(cmd3);
            var dtLPob = GetData(cmd4);

            string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
            string FileName = "AuditReport_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            DataSet DSExcel = new DataSet();
            DSExcel.Tables.Add(dtRequestLevel ); DSExcel.Tables[0].TableName = "Request Audit"; 
            DSExcel.Tables.Add(dtProduct); DSExcel.Tables[1].TableName = " Product Audit";
            DSExcel.Tables.Add(dtLPob); DSExcel.Tables[2].TableName = "LPob Audit";
            DSExcel.Tables.Add(dtProductLevel); DSExcel.Tables[3].TableName = "Product Attachments";
            Globals.GenericMethodForExportToExcel(DSExcel, path, FileName, "all text", "dd.MM.yyyy");
            //Globals.ExportToExcelforAccountingscenariolist(dtRequestLevel, dtProductLevel, path, FileName,"Audit Trail","Product Level");
            if (!string.IsNullOrEmpty(FileName))
            {
                string fullpath = path + "\\" + FileName;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            return Ok(FileName);
        }
    }
}
