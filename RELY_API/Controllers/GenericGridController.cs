using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GenericGridController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //This method loads the buttons on the bottom of grid based on StepId,transactionid,loggedinroleid passed
        public IHttpActionResult GetGridBottomActionItems(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId)
        {
            var ButtonsList = new List<string>();
            var WFDetails = db.RWorkFlows.Where(p => p.Name == Workflow).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var DistinctStepsAvailableToParticipant = db.WStepParticipants.Where(p => (p.ParticipantId == LoggedInRoleId && p.Type.Equals("ROLE", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInUserId && p.Type.Equals("USER", StringComparison.OrdinalIgnoreCase))).Select(p => new { p.IsDefault, p.WStepId }).Distinct().ToList();
            var AssignedStepIdList = DistinctStepsAvailableToParticipant.Select(p => p.WStepId).ToList();
            var StepList = db.WSteps.Where(p => p.RWorkFlow.Name.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).Where(p => AssignedStepIdList.Contains(p.Id))
                .Where(p=> p.Ordinal !=0) //Step with Ordinal 0 = Parked one, we dont want to include that in thelist.
                .OrderBy(p => p.Ordinal).Select(p =>new { p.Id ,p.Name}).ToList();
            for (var j = 0; j < StepList.Count(); j++)
            {
                var StepId = Convert.ToInt32(StepList[j].Id);
                // 1.Get the action Items for the Bottom of Grid by using show in WFStep Id Concept  (Get buttons in Tabs Where Click is made)
                var ActionItemsList = db.WStepParticipantActions.Where(p => p.ShowInStepId == StepId).Where(p => p.ButtonOnWfGrid == true).Where(p=>p.IsLinkOverWFGrid==false).Where(p => (p.ParticipantId == LoggedInRoleId && p.ParticipantType.Equals("ROLE", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInUserId && p.ParticipantType.Equals("USER", StringComparison.OrdinalIgnoreCase))).Select(p => new {p.Id, p.Label,p.Glymph, p.ButtonOnWfGrid, p.WActionId,p.ActionUrl }).ToList();
                // 2.Loop through action Items and for each item get the list of parameters (List of Parameters : Parameter Name,Parameter Type,Parameter Value)
                string HtmlTemplate = "";
                foreach (var ActionItem in ActionItemsList)
                {
                    var StepParticipantActionId = ActionItem.Id;
                    var Label = ActionItem.Label;
                    var Glymph = ActionItem.Glymph;
                    //Get Action Details
                    int ActionId = ActionItem.WActionId;
                    var ActionDetails = db.WActions.Where(p => p.Id == ActionId).FirstOrDefault();
                    if (String.IsNullOrEmpty(Label))
                    {
                        Label = ActionDetails.Label;
                    }
                    if (String.IsNullOrEmpty(Glymph))
                    {
                        Glymph = ActionDetails.Glymph;
                    }
                    /*The below string is used to frame the buttons html part in secondary form   window.location.href='" + UrlString + "';*/
                    HtmlTemplate += "<button type=\"button\" class=\"btn btn-red btn-cons\" onclick = \"FnClickBottomButtons('" + ActionDetails.Name + "','"+StepList[j].Name+"',"
                        +StepId+",0,"+WFDetails.Id  + "," + StepParticipantActionId + ",'"+ ActionItem.ActionUrl + "')\" > " + Label 
                        + "&ensp;<i class=\"fa "+Glymph+"\" aria-hidden=\"true\" style=\"color:white;\" ></i></button>&nbsp;&nbsp;";
                }
                ButtonsList.Add(HtmlTemplate);
            }
            return Ok(ButtonsList);
        }

        //This method loads the buttons on the bottom of form based on StepId,transactionid,loggedinroleid passed
        public IHttpActionResult GetFormBottomActionItems(string Workflow, string CompanyCode, int LoggedInRoleId, int LoggedInUserId,int StepId,string FormType)
        {
            var WFDetails = db.RWorkFlows.Where(p => p.Name == Workflow).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var ButtonsList = new List<string>();
            var DistinctStepsAvailableToParticipant = db.WStepParticipants.Where(p => (p.ParticipantId == LoggedInRoleId && p.Type.Equals("ROLE", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInUserId && p.Type.Equals("USER", StringComparison.OrdinalIgnoreCase))).Select(p => new { p.IsDefault, p.WStepId }).Distinct().ToList();
            var AssignedStepIdList = DistinctStepsAvailableToParticipant.Select(p => p.WStepId).ToList();
            var StepList = db.WSteps.Where(p => p.RWorkFlow.Name.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).Where(p => AssignedStepIdList.Contains(p.Id)).Where(p => p.Id ==StepId).OrderBy(p => p.Ordinal).Select(p => new { p.Id, p.Name }).FirstOrDefault();
          //  for (var j = 0; j < StepList.Count(); j++)
            //{
                //var StepId = Convert.ToInt32(StepList[j].Id);
                // 1.Get the action Items for the Bottom of Grid by using show in WFStep Id Concept  (Get buttons in Tabs Where Click is made)
                var ActionItemsList = db.WStepParticipantActions.Where(p => p.ShowInStepId == StepId).Where(p => p.ButtonOnForm == true).Where(p => p.IsLinkOverWFGrid == false).Where(p => (p.ParticipantId == LoggedInRoleId && p.ParticipantType.Equals("ROLE", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInUserId && p.ParticipantType.Equals("USER", StringComparison.OrdinalIgnoreCase))).Select(p => new { p.Id, p.Label, p.Glymph, p.ButtonOnWfGrid, p.WActionId, p.ButtonOnForm,p.ActionUrl }).ToList();
                // 2.Loop through action Items and for each item get the list of parameters (List of Parameters : Parameter Name,Parameter Type,Parameter Value)
                string HtmlTemplate = "";
            foreach (var ActionItem in ActionItemsList)
            {
                var StepParticipantActionId = ActionItem.Id;
                var Label = ActionItem.Label;
                var Glymph = ActionItem.Glymph;
                //Get Action Details
                int ActionId = ActionItem.WActionId;
                var ActionDetails = db.WActions.Where(p => p.Id == ActionId).FirstOrDefault();
                if (String.IsNullOrEmpty(Label))
                {
                    Label = ActionDetails.Label;
                }
                if (String.IsNullOrEmpty(Glymph))
                {
                    Glymph = ActionDetails.Glymph;
                }
                if (ActionDetails.Name != FormType)//EG:- We do not want to display Edit on Edit Form
                {
                    /*The below string is used to frame the buttons html part in secondary form   window.location.href='" + UrlString + "';*/
                    HtmlTemplate += "<button type=\"button\" class=\"btn btn-red-SideLayout btn-cons\" onclick = \"FnClickBottomButtons('" + ActionDetails.Name + "'"+ "," + StepParticipantActionId + ",'" + ActionItem.ActionUrl + "')\" > " + Label + "&ensp;<i class=\"fa " + Glymph + "\" aria-hidden=\"true\" style=\"color:white;\" ></i></button>";
                    //UrlString += "<a href=\"" + TopActionList[j].ActionUrl + "?WorkflowId=" + WFDetails.Id + "&ActionName=" + ActionDetails.Name + "&TransactionId=null&Source=null" + "&StepParticipantActionId=" + TopActionList[j].Id + "&StepId=" + TopActionList[j].ShowInStepId + "\">" + Label + "</a>&ensp;";
                }
              }
                ButtonsList.Add(HtmlTemplate);
           // }
            return Ok(HtmlTemplate);
        }

        //Method to actions which will be displayed on the top of the page
        [HttpGet]
        public IHttpActionResult GetTopActionLinks(string Workflow,string CompanyCode,int LoggedInUserId,int LoggedInRoleId)
        {
            var WFDetails = db.RWorkFlows.Where(p => p.Name == Workflow).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var TopActionList = db.WStepParticipantActions.Where(p => p.IsLinkOverWFGrid == true).Where(p=>p.WStep.WorkFlowId==WFDetails.Id).Where(p => (p.ParticipantId == LoggedInUserId && p.ParticipantType.Equals("User", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInRoleId && p.ParticipantType.Equals("Role", StringComparison.OrdinalIgnoreCase))).ToList();
            var UrlString = "";
            for (var j = 0; j < TopActionList.Count(); j++)
            {
                var Label = TopActionList[j].Label;
                var Glymph = TopActionList[j].Glymph;
                //Get Action Details
                int ActionId = TopActionList[j].WActionId;
                var ActionDetails = db.WActions.Where(p => p.Id == ActionId).FirstOrDefault();
                if (String.IsNullOrEmpty(Label))
                {
                    Label = ActionDetails.Label;
                }
                if (String.IsNullOrEmpty(Glymph))
                {
                    Glymph = ActionDetails.Glymph;
                }
                //Passing Extra  Parameter of WFConfigId and TransactionId to display buttons in secondary Forms in all Workflows
                //UrlString += "<a href=\"" + TopActionList[j].ActionUrl + "?WorkflowId=" + WFDetails.Id + "&ActionName=" + ActionDetails.Name + "&TransactionId=null&Source=null" + "&StepParticipantActionId=" + TopActionList[j].Id + "&StepId=" + TopActionList[j].ShowInStepId + "\">" + Label+"</a>&ensp;";
                UrlString += "<a href=\"" + TopActionList[j].ActionUrl + "?FormType=" + ActionDetails.Name + "&Source=null" + "\">" + Label + "</a>&ensp;";
            }
            return Ok(UrlString);
        }

        //Method to Get List of Columns based on StepId
        public IHttpActionResult GetWorkflowGridColumnsByStepId(string WorkFlow, int StepId, string UserName)
        {
            //Avoiding SQL injection
            //Get List of Columns based on StepId
            var xx = db.Database.SqlQuery<GenericGridColumnsViewModel>("select [ColumnName],[Label],[ShouldBeVisible],[OrderByOrdinal],[AscDesc],[Ordinal],[FunctionName],[JoinTable],[JoinTableColumn],[BaseTableJoinColumn] from WStepGridColumns where WStepId={0} order by Ordinal", StepId).ToList();

            //The following columns are put on loop to alter column Name as per logic for joining tables
            for (var i = 0; i < xx.Count(); i++)
            {
                if (!string.IsNullOrEmpty(xx[i].JoinTable))
                {
                    //DataType = "nvarchar"; For Join with Table column Name will be changed to tableName+Ordinal.ColumnName
                    xx[i].ColumnName = xx[i].JoinTable + xx[i].Ordinal + "." + xx[i].ColumnName + "";
                }
            }
            return Ok(xx);
        }

        //Method to Get List of Columns based on workflow
        public IHttpActionResult GetWorkflowGridColumnsByWorkflow(string WorkFlow, string UserName, string CompanyCode)
        {
            var WFDetails = db.RWorkFlows.Where(p => p.Name.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            //Avoiding SQL injection
            //GetAll columns for that Workflow for all steps
            //NOTE:- The below query has been added due to lack of time with a known issue that as the column names are same between multiple tables join may fail with Information Schema in some cases. It will be handled once caught during execution.
            var xx = db.Database.SqlQuery<GenericGridColumnsViewModel>("select distinct C.ColumnName,C.WStepId,C.Ordinal,C.JoinTable,C.JoinTableColumn, C.BaseTableJoinColumn, WF.BaseTableName,C.Label,C.AscDesc,C.FunctionName, ISC.DATA_TYPE as DataType from WStepGridColumns C inner join WSteps WS on C.[WStepId] = WS.Id inner join RWorkflows WF on WS.[WorkFlowId] = WF.Id inner join INFORMATION_SCHEMA.COLUMNS ISC on (ISC.Table_Name in(C.JoinTable,WF.BaseTableName) and C.ColumnName=ISC.COLUMN_NAME)  Where WF.Name={0} order by C.Ordinal", WorkFlow).ToList();

            //The following columns are put on loop to alter column Name as per logic for joining tables
            for (var i = 0; i < xx.Count(); i++)
            {
                if (!string.IsNullOrEmpty(xx[i].JoinTable))
                {
                    //DataType = "nvarchar"; For Join with Table column Name will be changed to tableName+Ordinal.ColumnName
                    xx[i].ColumnName = xx[i].JoinTable + xx[i].Ordinal + "." + xx[i].ColumnName + "";
                }
                else
                {
                    //SS added this line on 3Apr 2018 as filter stopped working in Generic grid
                    xx[i].ColumnName ="BT." + xx[i].ColumnName + "";
                }
            }
            return Ok(xx);
        }

        //Method to Get List of Tabs Based on Workflow
        public IHttpActionResult GetStepsByWorkflow(string WorkFlow, string UserName, string CompanyCode,int LoggedInRoleId, int LoggedInUserId)
        {
            var DistinctStepsAvailableToParticipant = db.WStepParticipants.Where(p => (p.ParticipantId == LoggedInRoleId && p.Type.Equals("ROLE", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInUserId && p.Type.Equals("USER", StringComparison.OrdinalIgnoreCase))).Select(p => new { p.IsDefault, p.WStepId }).Distinct().ToList();
            var DefaultStepIdList = DistinctStepsAvailableToParticipant.Where(p => p.IsDefault).Select(p => p.WStepId).ToList();
            var AssignedStepIdList = DistinctStepsAvailableToParticipant.Select(p => p.WStepId).ToList();
            var WFSteps = db.WSteps.Where(p => p.RWorkFlow.Name.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).Where(p=> AssignedStepIdList.Contains(p.Id))
                .Where(p => p.Ordinal > 0)
                .Select(p=>new {IsDefault=DefaultStepIdList.Contains(p.Id), p.Banner,p.CompanyCode,p.Description,p.DoNotNotify,p.Id,p.IsReady,p.Label,p.Name,p.Ordinal,p.Skip,p.SkipFunctionName,p.WorkFlowId}).OrderBy(p=>p.Ordinal).ToList();
            return Ok(WFSteps);
        }

        //Method to Get Data from Base Table and Load it in Generic Grid
        [HttpGet]
        public IHttpActionResult GetGenericGridData(string Workflow, string UserName, int StepId, int LoggedInUserId, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize, int LoggedInRoleId,string CompanyCode)
        {
            //GetAll columns for that Workflow for all steps
            //NOTE:- The below query has been added due to lack of time with a known issue that as the column names are same between multiple tables join may fail with Information Schema in some cases. It will be handled once caught during execution.
            var ColumnDetails = db.Database.SqlQuery<GenericGridColumnsViewModel>("select distinct isnull(C.JoinTable,'')+'.'+C.ColumnName as CompleteColumnName,C.ColumnName,C.WStepId,C.Ordinal,C.JoinTable,C.JoinTableColumn, C.BaseTableJoinColumn, WF.BaseTableName,C.Label,C.AscDesc,C.FunctionName, ISC.DATA_TYPE as DataType from WStepGridColumns C inner join WSteps WS on C.[WStepId] = WS.Id inner join RWorkflows WF on WS.[WorkFlowId] = WF.Id inner join INFORMATION_SCHEMA.COLUMNS ISC on (ISC.Table_Name in(C.JoinTable,WF.BaseTableName) and C.ColumnName=ISC.COLUMN_NAME)  Where C.WStepId={0} order by C.Ordinal", StepId).ToList();

            //Add dynamic columns by looping through list of columns returned from api to be displayed in generic grid and getting column details from there
            string ColumnList = null;//ColumnDetails.Select(p => p.ColumnName).ToList();
            String JoiningTableQuery = "";
            string SortQuery = null;
            if (!string.IsNullOrEmpty(sortorder))//code for server side sorting
            {
                if (sortorder == "asc")
                {
                    SortQuery += " order by " + sortdatafield + " ";
                }
                else
                {
                    SortQuery += " order by " + sortdatafield + " desc ";
                }
            }

            foreach (var item in ColumnDetails)
            {
                var ColumnName = string.IsNullOrEmpty(item.JoinTable) ? " BT." + item.ColumnName + " as [BT." + item.ColumnName + "]" : item.JoinTable + item.Ordinal + "." + item.ColumnName + " as [" + item.JoinTable + item.Ordinal + "." + item.ColumnName + "]";
                if (string.IsNullOrEmpty(SortQuery))
                {
                    if (!string.IsNullOrEmpty(item.AscDesc) && item.ColumnName != sortdatafield)
                        SortQuery += " order by " + (string.IsNullOrEmpty(item.JoinTable) ? item.ColumnName : item.JoinTable + item.Ordinal + "." + item.ColumnName + " ") + " " + item.AscDesc;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.AscDesc) && item.ColumnName != sortdatafield)
                        SortQuery += " , " + (string.IsNullOrEmpty(item.JoinTable) ? item.ColumnName : item.JoinTable + item.Ordinal + "." + item.ColumnName + " ") + " " + item.AscDesc;
                }
                string DataType = item.DataType;

                //This is done to convert Id into Name
                if (!string.IsNullOrEmpty(item.FunctionName))
                {
                    //DataType = "nvarchar";
                    ColumnList += item.FunctionName + " as [BT." + item.ColumnName + "] , ";
                }
                else
                {
                    ColumnList +=ColumnName + " , ";
                }

                if (!string.IsNullOrEmpty(item.JoinTable))
                {
                    //Doing inner join if joining table is defined and if Base Table joining column is null then will also display the record
                   
               if(item.JoinTable == "SSPDimensions") 
                {
                        JoiningTableQuery += " left outer join " + item.JoinTable + " " + item.JoinTable + item.Ordinal + " on " + item.JoinTable + item.Ordinal + "." + item.JoinTableColumn + "=BT." + item.BaseTableJoinColumn + " and(GETDATE() between " + item.JoinTable + item.Ordinal + ".EffectiveStartDate and " + item.JoinTable + item.Ordinal + ".EffectiveEndDate)";
                    }
               else
                {
                   JoiningTableQuery += " left outer join " + item.JoinTable + " " + item.JoinTable + item.Ordinal + " on " + item.JoinTable + item.Ordinal + "." + item.JoinTableColumn + "=BT." + item.BaseTableJoinColumn + " ";
                }
                
                }

            }
            //If no sorting is there add default statement
            if (string.IsNullOrEmpty(SortQuery))
            {
                SortQuery = " order by BT.Id desc ";
            }
            //Get Workflow and Step Details
            var WFDetails = db.RWorkFlows.Where(p => p.Name.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var WstepDetails = db.WSteps.Where(p => p.Id == StepId).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            //Run DB function to get list of Ids for Latest version
            //no need to include the completed version list
            //var ListOfIds = db.Database.SqlQuery<string>("select dbo.[FnGetLatestCompletedVersionList]({0},{1},{2})", WstepDetails.Ordinal, WFDetails.BaseTableName, WstepDetails.CompanyCode).FirstOrDefault();
            var Query = "Select * From(select BT.Id, " + ColumnList + " '' as Actions, ROW_NUMBER() OVER (" + SortQuery + ") as row from " + WFDetails.BaseTableName + " BT ";
            Query += JoiningTableQuery;

            //Adding Company code in query
           //Query += " where BT.WFOrdinal=" + WstepDetails.Ordinal+" And BT.WFStatus in ('Saved', 'InProgress','Completed')  ";
            Query += " where BT.WFOrdinal=" + WstepDetails.Ordinal + " AND BT.WFType = '" + WFDetails.WFType + "' And BT.WFStatus in ('Saved', 'InProgress','Completed') And BT.CompanyCode='" + CompanyCode + "'";
            //Filter for getting Maximum version in Completed status of the transaction 
            //if (WFDetails.BaseTableName.Equals("LProducts"))
            //{
            //    Query += " and BT.Version = (select max(lp1.Version) from " + WFDetails.BaseTableName + "  lp1 where BT.SourceProductId = lp1.SourceProductId)  ";
            //}
            //else if (WFDetails.BaseTableName.Equals("LLocalPobs"))
            //{
            //    Query += " and BT.Version = (select max(lp1.Version) from " + WFDetails.BaseTableName + "  lp1 where BT.SourceLocalPobId = lp1.SourceLocalPobId)  ";
            //}
            //code to add portfolio filter in the records got from above query
            //Get the list of all portfolios which belongs to the logged in user
            var PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.EntityId == LoggedInUserId).Where(p => p.EntityType == "LUsers").Select(p => p.PortfolioId));
            //Get the List of All Entity Items which matchs with logged in user's portfolios 
            var PortfolioEntityListQuery = " select EntityId from MEntityPortfolios where EntityType='" + WFDetails.BaseTableName + "' and PortfolioId in (" + PortfolioList + ")";
            //no need to include the completed version list
            //If Latest version Ids are returned then add it in query and clause.
            // PortfolioEntityListQuery += (string.IsNullOrEmpty(ListOfIds)) ? string.Empty : " and EntityId in (" + ListOfIds + ") ";
            Query += " AND BT.Id in (" + PortfolioEntityListQuery + ")";

            //The below line checks that the Workflow Type of Base table should be equal to WfType of current workflow in RWorkflows 
            Query += "  AND BT.WFType='" + WFDetails.WFType + "' " + FilterQuery + ") a  Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;


            if (!string.IsNullOrEmpty(sortorder))//code for server side sorting
            {
                if (sortorder == "asc")
                {
                    Query += " order by [" + sortdatafield + "]";
                }
                else
                {
                    Query += " order by [" + sortdatafield + "] desc";
                }
            }

            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fetch the data from database
            //Execute the query and return the result 
            //dynamic xx = db.Database.SqlQuery(resultType, Query);

            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();

            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            //Now add Actions to Each TransactionId
            var WStepParticipantAct = db.WStepParticipantActions.Where(p=>p.IsLinkOverWFGrid==false).Where(p => p.ShowInStepId == StepId).Where(p => (p.ParticipantId == LoggedInUserId && p.ParticipantType.Equals("User", StringComparison.OrdinalIgnoreCase)) || (p.ParticipantId == LoggedInRoleId && p.ParticipantType.Equals("Role", StringComparison.OrdinalIgnoreCase))).ToList();

            if (WStepParticipantAct.Count() > 0)
            {
                for (var i = 0; i < tb.Rows.Count; i++)
                {
                    var UrlString = "";
                    for (var j = 0; j < WStepParticipantAct.Count(); j++)
                    {
                        var StepParticipantActionId = WStepParticipantAct[j].Id;
                        var Label = WStepParticipantAct[j].Label;
                        var Glymph = WStepParticipantAct[j].Glymph;
                        //Get Action Details
                        int ActionId = WStepParticipantAct[j].WActionId;
                        var ActionDetails = db.WActions.Where(p => p.Id == ActionId).FirstOrDefault();
                        if (String.IsNullOrEmpty(Label))
                        {
                            Label = ActionDetails.Label;
                        }
                        if (String.IsNullOrEmpty(Glymph))
                        {
                            Glymph = ActionDetails.Glymph;
                        }
                        //Passing Extra  Parameter of WFConfigId and TransactionId to display buttons in secondary Forms in all Workflows
                        //VG advised to use if condition to display dialog box in caseof cancel action
                        //SS07Mar2018 has changed method to add actions so as to add comments with individual actions
                        //UrlString += "<a href=\"#\" onclick = \"FnClickBottomButtons('" + ActionDetails.Name + "','" + WstepDetails.Name + "',"+StepId+","+ tb.Rows[i].Field<dynamic>("Id") + ","+WFDetails.Id+ ","+ StepParticipantActionId+")\" > <i class=\"fa " + Glymph + "\" aria-hidden=\"true\" style=\"color:red;\" title=\""+Label+"\" ></i></a>&ensp;";
                        UrlString += "<a href=\"#\" onclick = \"FnClickBottomButtons('" + ActionDetails.Name + "','" + WstepDetails.Name + "'," + StepId + "," + tb.Rows[i].Field<dynamic>("Id") + "," + WFDetails.Id + "," + StepParticipantActionId + ",'"+ WStepParticipantAct[j].ActionUrl + "')\" > <i class=\"fa " + Glymph + "\" aria-hidden=\"true\" style=\"color:red;\" title=\"" + Label + "\" ></i></a>&ensp;";
                        /*if (ActionDetails.Name == "Cancel")
                        {
                            UrlString += "<a href=\"#\" onclick=\"if(confirm('Once cancelled this item will be permanently removed from workflow. Are you sure you want to proceed?')==true){window.location.href='" + ActionDetails.ActionURL + "?StepId=" + StepId + "&TransactionId=" + tb.Rows[i].Field<dynamic>("Id") + "&ActionName=" + ActionDetails.Name + "&WorkflowId=" + WFDetails.Id + "'}\"><i class=\"fa " + Glymph + "\" aria-hidden=\"true\" style=\"color:red;\" title=\"" + Label + "\"></i></a>&ensp;";
                        }
                        else
                        {
                            UrlString += "<a href=\"" + ActionDetails.ActionURL + "?StepId=" + StepId + "&TransactionId=" + tb.Rows[i].Field<dynamic>("Id") + "&ActionName=" + ActionDetails.Name + "&WorkflowId=" + WFDetails.Id + "\"><i class=\"fa " + Glymph + "\" aria-hidden=\"true\" style=\"color:red;\" title=\"" + Label + "\"></i></a>&ensp;";
                        }*/
                    }
                    //Update Action Items column with the links obtained from above process
                    tb.Rows[i]["Actions"] = UrlString;
                }
            }


            return Ok(tb);
        }
        

        //
        [HttpGet]
        //Method to Get Data from Base Table and Load it in Generic Grid
        public IHttpActionResult GetGenericGridCounts(string Workflow, string UserName, int StepId, int LoggedInUserId,string CompanyCode)
        {
            //we have one Stored procedure [SpGetGenericGridCounts] which is doing same thing, but not currently beig used.In future, we may migrate to SP if required.

            //Get Workflow and Step Details
            var WFDetails = db.RWorkFlows.Where(p => p.Name.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var WstepDetails = db.WSteps.Where(p => p.Id == StepId).FirstOrDefault();
            //no need to include the completed version list
            //Run DB function to get list of Ids for Latest version
            // var ListOfIds = db.Database.SqlQuery<string>("select dbo.[FnGetLatestCompletedVersionList]({0},{1},{2})", WstepDetails.Ordinal,WFDetails.BaseTableName,WstepDetails.CompanyCode).FirstOrDefault();
            var Query = "select count(*) from " + WFDetails.BaseTableName + " BT where BT.WFOrdinal=" + WstepDetails.Ordinal;
            Query += " AND WFType = '" + WFDetails.WFType + "' And WFStatus in ('Saved', 'InProgress','Completed') And BT.CompanyCode='" + CompanyCode + "' ";

            //Filter for getting Maximum version in Completed status of the transaction 
            //if (WFDetails.BaseTableName.Equals("LProducts"))
            //{
            //    Query += " and BT.Version = (select max(lp1.Version) from " + WFDetails.BaseTableName + "  lp1 where BT.SourceProductId = lp1.SourceProductId)  ";
            //}
            //else if (WFDetails.BaseTableName.Equals("LLocalPobs"))
            //{
            //    Query += " and BT.Version = (select max(lp1.Version) from " + WFDetails.BaseTableName + "  lp1 where BT.SourceLocalPobId = lp1.SourceLocalPobId)  ";
            //}

            //code to add portfolio filter in the records got from above query
            //Get the list of all portfolios which belongs to the logged in user
            var PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.EntityId == LoggedInUserId).Where(p => p.EntityType == "LUsers").Select(p => p.PortfolioId));
            //Get the List of All Entity Items which matchs with logged in user's portfolios 
            var PortfolioEntityListQuery = " select EntityId from MEntityPortfolios where EntityType='" + WFDetails.BaseTableName + "' and PortfolioId in (" + PortfolioList + ")";
            //If Latest version Ids are returned then add it in query and clause.
            //no need to include the completed version list
            //PortfolioEntityListQuery += (string.IsNullOrEmpty(ListOfIds)) ?string.Empty: " and EntityId in ("+ListOfIds+") ";
            Query += " AND BT.Id in (" + PortfolioEntityListQuery + ")";
            //Get the counts for Grid
            var RowCounts = db.Database.SqlQuery<int>(Query).FirstOrDefault();

            return Ok(RowCounts);
        }

        //Get Workflow details by WorkflowName
        [HttpGet]
        public IHttpActionResult GetWorkFlowDetails(string Workflow, string CompanyCode)
        {
            var WorkflowDetails = db.RWorkFlows.Where(p => p.Name == Workflow).Where(p => p.CompanyCode == CompanyCode).Select(p=>new { p.Name,p.UILabel,p.Id,p.WFType,p.CRAllowed,p.CRWFName,p.BaseTableName}).FirstOrDefault();
            return Ok(WorkflowDetails);
        }

        //This method is called when we update WfSatatus in any Workflow against any transaction
        [HttpPost]
        // public IHttpActionResult UpdateActionStatus(string Action, string WorkFlowName, int TransactionId, string CompanyCode, int LoggedInUserId, string Comments, int LoggedInRoleId, string AssigneeId)
        public IHttpActionResult UpdateActionStatus(OtherAPIData objTrans, string Action, string WorkFlowName, string CompanyCode, int LoggedInUserId, string Comments, int LoggedInRoleId, string AssigneeId)
        {
            //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            string ProjectEnviournment = db.GKeyValues.Where(a => a.Key == "ProjectEnviournment").Select(a => a.Value).FirstOrDefault();
            //db.SPUpdateActionStatus(Action, WorkFlowName, TransactionId, CompanyCode, LoggedInUserId.ToString(), Comments, LoggedInRoleId.ToString(), ProjectEnviournment, AssigneeId);
            db.Database.ExecuteSqlCommand("Exec dbo.SPUpdateActionStatus {0},{1},{2},{3},{4},{5},{6},{7},{8}"
                ,Action,WorkFlowName,objTrans.TransactionID,CompanyCode,LoggedInUserId, Comments,LoggedInRoleId, ProjectEnviournment, AssigneeId);
            //Now Perform Actions which are required after Workflow is Ready
            //get WStep Details
            var WFDetails = db.RWorkFlows.Where(p => p.Name == WorkFlowName).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault();
            var TranIdList = objTrans.TransactionID.Split(',').ToList();
            foreach (var TId in TranIdList)
            {
                var TransactionId = Convert.ToInt32(TId);
                switch (Action)
                {
                    case "SetCompleted":
                    case "Approve":
                        var BaseTableDetails = db.Database.SqlQuery<WFColumnsViewModel>("select WFOrdinal,WFStatus from " + WFDetails.BaseTableName + " where Id={0}", TransactionId).FirstOrDefault();
                        if (BaseTableDetails != null)
                        {
                            // var IsReady = db.Database.SqlQuery<bool>("select dbo.fnIsready({0},{1},{2})", WorkFlowName, CompanyCode, (BaseTableDetails.WFOrdinal-1)).FirstOrDefault();
                            if (BaseTableDetails.WFStatus == "Completed")
                            {
                                switch (WorkFlowName)
                                {
                                    case "Users":
                                        //create AD Account
                                        var UserDetails = db.LUsers.Where(p => p.Id == TransactionId).FirstOrDefault();
                                        //Random password generator code used for creating AD account ---- commenting as now
                                        RandomPassword pwd = new RandomPassword();
                                        string randompwd = pwd.Generate();
                                        ADModel model = new ADModel();
                                        model.Email = UserDetails.LoginEmail;
                                        //ConfigurationManager.AppSettings["ProjectEnviournment"];

                                        //JS directed Choose password as per the Project Enviournment
                                        switch (ProjectEnviournment)
                                        {
                                            case "Prod":
                                                model.Password = randompwd;
                                                break;
                                            default:
                                                model.Password = ConfigurationManager.AppSettings["DefaultPassword"];
                                                break;

                                        }
                                        var result = Globals.CreateUser(model);
                                        if (!result.IsSuccess)
                                        {
                                            if (result.ErrorMessage.ToLower().Contains("already"))
                                            {
                                                //this is the case when user exists in AD and Part of EUG
                                                //This might happen due to Change request.
                                                //do nothing

                                            }
                                            if (result.ErrorMessage.ToLower().Contains("added to eug"))
                                            {
                                                //Add entry in email bucket
                                                var SenderConfig = ConfigurationManager.AppSettings["SenderAccountName"];
                                                //Getting EmailSubject and EmailBody from database for Password for Welcome Email
                                                var EmailTemplateWelcome = db.LEmailTemplates.Where(p => p.TemplateName == "WelcomeUser").Where(aa => aa.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                                                if (EmailTemplateWelcome != null)
                                                {
                                                    var EmailBodyWelcome = EmailTemplateWelcome.EmailBody;
                                                    EmailBodyWelcome = EmailBodyWelcome.Replace("###EmailAddress###", model.Email);
                                                    var EmailSubjectWelcome = EmailTemplateWelcome.EmailSubject;
                                                    db.SpLogEmail(UserDetails.LoginEmail, null, null, null, EmailSubjectWelcome, EmailBodyWelcome, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, SenderConfig);
                                                }
                                                //User Added to EUG and if ENV is PROD pwd is reser
                                                //Therefore,Set Pwd History and force to ChangePwdAtNextLogin and Send Email.
                                                if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    //Add entry in LPasswordHistory
                                                    var PasswordHistoryModel = new LPasswordHistory { CreatedDateTime = DateTime.UtcNow, LoginEmail = UserDetails.LoginEmail };
                                                    db.LPasswordHistories.Add(PasswordHistoryModel);
                                                    UserDetails.ChangePwdAtNextLogin = true;
                                                    db.Entry(UserDetails).State = EntityState.Detached;
                                                    db.SaveChanges();
                                                    db.Entry(UserDetails).State = EntityState.Modified;
                                                    db.SaveChanges();


                                                    //Getting EmailSubject and EmailBody from database
                                                    var EmailTemplate = db.LEmailTemplates.Where(p => p.TemplateName == "Password Reset by Admin").Where(aa => aa.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                                                    var EmailSubject = EmailTemplate.EmailSubject;
                                                    var EmailBody = EmailTemplate.EmailBody;
                                                    //Replace the placeholders with actual values
                                                    EmailBody = (EmailBody.Replace("{User-name}", UserDetails.FirstName + " " + UserDetails.LastName)).Replace("{Password}", model.Password);
                                                    db.SpLogEmail(UserDetails.LoginEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, SenderConfig);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //New user created in AD and added to EUG. Set Pwd History and force to ChangePwdAtNextLogin and Send Email.
                                            //Add entry in LPasswordHistory
                                            var PasswordHistoryModel = new LPasswordHistory { CreatedDateTime = DateTime.UtcNow, LoginEmail = UserDetails.LoginEmail };
                                            db.LPasswordHistories.Add(PasswordHistoryModel);
                                            UserDetails.ChangePwdAtNextLogin = true;
                                            db.Entry(UserDetails).State = EntityState.Detached;
                                            db.SaveChanges();
                                            db.Entry(UserDetails).State = EntityState.Modified;
                                            db.SaveChanges();
                                            //Add entry in email bucket
                                            var SenderConfig = ConfigurationManager.AppSettings["SenderAccountName"];
                                            //Getting EmailSubject and EmailBody from database for Password for Welcome Email

                                            var EmailTemplateWelcome = db.LEmailTemplates.Where(p => p.TemplateName == "WelcomeUser").Where(aa => aa.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                                            if (EmailTemplateWelcome != null)
                                            {
                                                var EmailBodyWelcome = EmailTemplateWelcome.EmailBody;
                                                EmailBodyWelcome = EmailBodyWelcome.Replace("###EmailAddress###", model.Email);
                                                var EmailSubjectWelcome = EmailTemplateWelcome.EmailSubject;
                                                db.SpLogEmail(UserDetails.LoginEmail, null, null, null, EmailSubjectWelcome, EmailBodyWelcome, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, SenderConfig);
                                            }



                                            //Getting EmailSubject and EmailBody from database for Password
                                            var EmailTemplate = db.LEmailTemplates.Where(p => p.TemplateName == "Password Reset by Admin").Where(aa => aa.CompanyCode == UserDetails.CompanyCode).FirstOrDefault();
                                            if (EmailTemplate != null)
                                            {
                                                var EmailSubject = EmailTemplate.EmailSubject;
                                                var EmailBody = EmailTemplate.EmailBody;
                                                //Replace the placeholders with actual values
                                                EmailBody = (EmailBody.Replace("{User-name}", UserDetails.FirstName + " " + UserDetails.LastName)).Replace("{Password}", model.Password);
                                                db.SpLogEmail(UserDetails.LoginEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, SenderConfig);
                                            }
                                        }

                                        break;
                                }
                            }
                        }
                        break;
                    case "ResetPassword":
                        switch (WorkFlowName)
                        {
                            case "Users":
                                var UserDetail = db.LUsers.Where(p => p.Id == TransactionId).FirstOrDefault();
                                if (UserDetail != null)
                                {
                                    //Generate a random password and set the password for the user
                                    RandomPassword pwd = new RandomPassword();
                                    string randompwd = pwd.Generate();
                                    ADModel model = new ADModel();
                                    model.Email = UserDetail.LoginEmail;
                                    model.NewPassword = randompwd;
                                    Globals.SetUserPassword(model);

                                    //Getting EmailSubject and EmailBody from database
                                    var EmailTemplate = db.LEmailTemplates.Where(p => p.TemplateName == "Password Reset by User").Where(aa => aa.CompanyCode == UserDetail.CompanyCode).FirstOrDefault();
                                    var EmailSubject = EmailTemplate.EmailSubject;
                                    var EmailBody = EmailTemplate.EmailBody;

                                    //Replace the placeholders with actual values
                                    EmailBody = (EmailBody.Replace("{User-name}", UserDetail.FirstName + " " + UserDetail.LastName)).Replace("{Password}", model.NewPassword);

                                    db.SpLogEmail(UserDetail.LoginEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "DEV Vodafone RELY");
                                    //Make entry in email bucket for the new password
                                    //db.SpLogEmail(UserDetail.LoginEmail, null, null, null, "Vodafone RELY", "Hi " + UserDetail.FirstName + " " + UserDetail.LastName + ",<br>Your Vodafone RELY Password is changed to " + model.NewPassword, true, "Notifier", "Normal", null, "InQueue", null, LoggedInRoleId, LoggedInUserId, "DEV Vodafone RELY");
                                }
                                break;
                        }
                        break;
                }
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult ValidateActionResult(int StepParticipantActionId,int EntityId, string CompanyCode)
        {

            ObjectParameter ValidationMessage = new ObjectParameter("ValidationMessage", typeof(string)); //return parameter is declared
            db.SpValidateParticipantAction(StepParticipantActionId, EntityId, CompanyCode, ValidationMessage);
            string OutputMessage = (string)ValidationMessage.Value; //getting value of output parameter
           //var Response = db.Database.SqlQuery<string>("select dbo.[FnValidateParticipantAction]({0},{1},{2})", StepParticipantActionId, EntityId, CompanyCode).FirstOrDefault();
            return Ok(OutputMessage);
        }

        /// <summary>
        /// Author: Rakhi Singh
        /// Created Date: 4th july
        /// Description: The method is used to Upload Generic Grid Data file to the targeted path
        /// </summary>
        /// <param name="Workflow"></param>
        /// <param name="UserName"></param>
        /// <param name="StepId"></param>
        /// <param name="LoggedInUserId"></param>
        /// <param name="LoggedInRoleId"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DownloadGenericDataGrid(string Workflow, string UserName, int StepId, int LoggedInUserId, int LoggedInRoleId, string CompanyCode, string TabName)
        {
            var Query = "Exec [SpDownloadGenericGridData] @Workflow,@UserName, @StepId,@LoggedInUserId,@LoggedInRoleId,@CompanyCode";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Workflow", Workflow);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@StepId", StepId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", LoggedInRoleId);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = Globals.GetDataTableUsingADO(cmd);

            string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
            string Filename = Workflow + "_" + TabName + "_" + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
            if (dt.Columns.Count > 0)
            {
                Globals.ExportToExcel(dt, path, Filename);
            }
           
            if (!string.IsNullOrEmpty(Filename))
            {
                string fullpath = path + "\\" + Filename;//
                string localpath = fullpath;
                string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
                string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                Globals.UploadFileToS3(localpath, S3TargetPath);
            }
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = Filename };
            return Ok(model);
        }

    }


}
