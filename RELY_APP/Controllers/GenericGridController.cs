//using Newtonsoft.Json;
using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
//using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
//using System.IO.Compression;
using System.Linq;
//using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class GenericGridController : Controller
    {
        IGenericGridRestClient RestClient = new GenericGridRestClient();
        // GET: GenericGrid for all workflows
        //Method to display Grid on the Page//
        [CustomAuthorize]
        public ActionResult Index(string WorkFlow)
        { 
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            string CompanyCode = Globals.GetSessionData("CompanyCode");
            if (string.IsNullOrEmpty(WorkFlow))
            {
                WorkFlow = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            }
            else
            {
                System.Web.HttpContext.Current.Session["WorkFlow"] = WorkFlow;
            }
            System.Web.HttpContext.Current.Session["Title"] = "Manage " + WorkFlow;
            ViewBag.TopLinks = RestClient.GetGenericGridTopLinks(LoggedInRoleId,LoggedInUserId,WorkFlow,CompanyCode);
            var WFDeatails = RestClient.GetWorkflowDetails(WorkFlow, CompanyCode); //stored in a variable here because vairable is used muyltiple times later in this method
            ViewBag.WorkflowDetails = WFDeatails;
            ViewBag.Title = "Manage " + WFDeatails.UILabel;
            System.Web.HttpContext.Current.Session["WorkFlowBaseTable"] = WFDeatails.BaseTableName;
            System.Web.HttpContext.Current.Session["WorkFlowId"] = WFDeatails.Id;
            ViewBag.GridColumns = RestClient.GetGenericGridColumnsByWorkflow(CompanyCode,WorkFlow);
            ViewBag.GenericGridTabs = RestClient.GetTabsByWorkflow(CompanyCode,WorkFlow,LoggedInRoleId,LoggedInUserId);
            ViewBag.BottomButtons = RestClient.GetGridBottomButtons(WorkFlow, CompanyCode, LoggedInRoleId, LoggedInUserId);
            return View();
        }

      //  //create method for all workflows
      //  [CustomAuthorize]
      //  public ActionResult Create(int WorkflowId, string Source,int StepId)
      //  {
      //      var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
      //      //ViewBag.Title = "Create" + WFName;
      //      switch (WFName)
      //      {
      //          case "LocalPobs":
      //              return RedirectToAction("Create","LLocalPOB",new { /*WorkflowId= WorkflowId ,*/Source = Source , FormType = "Create" }); //--commented by RS as per requirement to hide workflowid
      //          case "RequestsContract":
      //          case "RequestsPPM":
      //              return RedirectToAction("Create", "LRequests"); /*new { WorkflowId = WorkflowId , StepId = StepId }*/
      //          case "Products":
      //              return RedirectToAction("Create", "LProducts", new {Source = Source , FormType = "Create"/*WorkflowId = WorkflowId, StepId = StepId*/ });
      //          case "Users":
      //              return RedirectToAction("Create", "LUsers", new { FormType = "Create"/*, WorkflowId = WorkflowId, StepId = StepId*/ });
      //          case "References":
      //              return RedirectToAction("Create","LReferences",new { FormType = "Create" ,/* WorkflowId = WorkflowId, StepId = StepId*/ });
      //          case "AccountingScenario":
      //              return RedirectToAction("Create", "LAccountingScenarios", new {/* WorkflowId = WorkflowId ,*/ FormType = "Create" });
      //      }
      //      TempData["Error"] = "Functionality coming soon";
      //      return RedirectToAction("Index");
      //  }

      //  //Edit method for all workflows
      //  [CustomAuthorize]
      //  public ActionResult Edit(int WorkflowId, int StepId, int TransactionId, string ActionName, string Source)
      //  {
      //      int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
      //      int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
      //      if (string.IsNullOrEmpty(ActionName))
      //          ActionName = "Edit";
      //      var isAuthorized = Globals.CheckActionAuthorization(ActionName, LoggedInRoleId, LoggedInUserId, WorkflowId, StepId);
      //      if (isAuthorized)
      //      {
      //          var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
      //          System.Web.HttpContext.Current.Session["Title"] = "Edit" + WFName;
      //          switch (WFName)
      //          {
      //              case "LocalPobs":
      //                  ///LLocalPOB/Edit/11?WorkflowId=1&ActionName=Edit
      //                  return RedirectToAction("Edit", "LLocalPOB", new { /*WorkflowId = WorkflowId,*/ ActionName = ActionName, id = TransactionId, Source = Source/*, StepId = StepId*/ });
      //              case "RequestsContract":
      //              case "RequestsPPM":
      //                  return RedirectToAction("Edit", "LRequests", new { /*WorkflowId = WorkflowId,*/ id = TransactionId/*, StepId = StepId */});
      //              case "Products":
      //                  return RedirectToAction("Edit", "LProducts", new { /*WorkflowId = WorkflowId, */ ActionName = ActionName, id = TransactionId, Source = Source /*, StepId = StepId */});
      //              case "Users":
      //                  return RedirectToAction("Create", "LUsers", new { FormType = "Edit", TransactionId = TransactionId/*, StepId = StepId*/ });
      //              case "References":
      //                  return RedirectToAction("Edit", "LReferences", new { id = TransactionId /*,WorkflowId = WorkflowId, StepId = StepId*/ });
      //              case "AccountingScenario":
      //                  return RedirectToAction("Edit", "LAccountingScenarios", new { Id = TransactionId/*, WorkflowId = WorkflowId, StepId = StepId*/ });
      //              case "ScenarioDemand":
      //                  return RedirectToAction("Edit", "LScenarioDemand", new { Id = TransactionId, /*WorkflowId = WorkflowId, StepId = StepId, */FormType = "Edit" });
      //          }
      //      TempData["Error"] = "Functionality coming soon";
      //      return RedirectToAction("Index");
      //  }
      //      else
      //      {
      //          TempData["Error"] = "Sorry , You are not authorised to perform this action. Please contact L2 Admin";
      //          return RedirectToAction("Index");
      //      }
      //  }

      ///*  //This method will Reset Password for User WorkFlow
      //  [CustomAuthorize]
      //  public ActionResult ResetPassword(int WorkflowId, int StepId, string TransactionId, string ActionName, string Comments, int StepParticipantActionId)
      //  {
      //      int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
      //      int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
      //      if (string.IsNullOrEmpty(ActionName))
      //          ActionName = "Reset Password ";
      //      var isAuthorized = Globals.CheckActionAuthorization(ActionName, LoggedInRoleId, LoggedInUserId, WorkflowId, StepId);
      //      if (isAuthorized)
      //      {
      //          ILUsersRestClient URC = new LUsersRestClient();
      //          var user = URC.GetById(Convert.ToInt32(TransactionId));
      //          string UserName = user.LoginEmail;
      //          return RedirectToAction("SetPasswordViaAdmin", new { Email = UserName, LoggedInUserId = LoggedInUserId, LoggedInRoleId= LoggedInRoleId });
      //      }
      //      else
      //      {
      //          TempData["Error"] = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
      //          return RedirectToAction("Index");
      //      }


      //  }
      //  public ActionResult SetPasswordViaAdmin(string Email, int LoggedInUserId, int LoggedInRoleId)
      //  {
      //      ChangePasswordBindingModel model = new ChangePasswordBindingModel { Email = Email };
      //      try
      //      {
      //          IAccountsRestClient ARC = new AccountsRestClient(); 
      //          var result = ARC.SetPasswordViaAdmin(model, LoggedInUserId, LoggedInRoleId, null);
      //          @TempData["Message"] = "New password has been sent to the user : " + Email ;
      //          return RedirectToAction("Index");
      //      }
      //      catch (Exception ex)
      //      {
      //          TempData["Error"] = ex.Data["ErrorMessage"];
      //          return RedirectToAction("Index");
      //      }
      //  }
      //  */
      //  //Review method for all workflows
      //  [CustomAuthorize]
      //  public ActionResult Review(int WorkflowId, int StepId, int TransactionId, string ActionName, string Source)
      //  {
      //      if (string.IsNullOrEmpty(ActionName))
      //          ActionName = "Review";
      //          var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
      //      System.Web.HttpContext.Current.Session["Title"] = "Review" + WFName;
      //      switch (WFName)
      //          {
      //              case "LocalPobs":
      //                  ///LLocalPOB/Edit/11?WorkflowId=1&ActionName=Edit
      //                  return RedirectToAction("Review", "LLocalPOB", new { /*WorkflowId = WorkflowId,*/ ActionName = ActionName, id = TransactionId, Source = Source /*, StepId = StepId*/ });
      //          case "RequestsContract":
      //          case "RequestsPPM":
      //                  return RedirectToAction("Review", "LRequests", new {/* WorkflowId = WorkflowId,, StepId = StepId */id = TransactionId});
      //              case "Products":
      //                  return RedirectToAction("Review", "LProducts", new {/* WorkflowId = WorkflowId,*/ ActionName = ActionName, id = TransactionId, Source = Source/*, StepId = StepId */});
      //              case "Users":
      //                  return RedirectToAction("Create", "LUsers", new { FormType = "Review", TransactionId = TransactionId/*, StepId = StepId*/ });
      //              case "References":
      //                  return RedirectToAction("Review", "LReferences", new { id = TransactionId , /*WorkflowId = WorkflowId, StepId = StepId */});
      //              case "AccountingScenario":
      //                  return RedirectToAction("Review", "LAccountingScenarios", new { Id = TransactionId/*, WorkflowId = WorkflowId, StepId = StepId*/ });
      //              case "ScenarioDemand":
      //                  return RedirectToAction("Review", "LScenarioDemand", new { Id = TransactionId/*, WorkflowId = WorkflowId, StepId = StepId*/, FormType = "Review" });
      //          }
      //          TempData["Error"] = "Functionality coming soon";
      //          return RedirectToAction("Index");
      //          // TempData["Error"] = "Functionality coming soon";
      //          //return RedirectToAction("Index");//to be implemented
      //  }

        //Download method for all workflows//
        //[CustomAuthorize]
        //public ActionResult Download(int WorkflowId, int StepId, int TransactionId, string ActionName)
        //{
        //    //TempData["Error"] = "Functionality coming soon";
        //    ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
        //    var BaseTable = Convert.ToString(System.Web.HttpContext.Current.Session["WorkFlowBaseTable"]);
        //    var LSupportingDocs = LSDRC.GetByEntityType(BaseTable, TransactionId);
        //    if (LSupportingDocs.Count() > 0)
        //    {
        //        for(var i=0;i<LSupportingDocs.Count();i++)
        //        {
        //            var FileName = LSupportingDocs.ElementAt(i).OriginalFileName;
        //            var CompleteFileName = LSupportingDocs.ElementAt(i).FilePath + "/" + LSupportingDocs.ElementAt(i).FileName;
        //            var bytedata = Globals.DownloadFromS3(CompleteFileName);
        //            var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + FileName;
        //            System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
        //        }
        //        var FilesToBezipped = LSupportingDocs.Select(p => new { CompleteFileName= (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + p.OriginalFileName) }).Select(p=>p.CompleteFileName).ToList();
        //        var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
        //        System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"]);//Now Delete all files
        //        foreach (FileInfo file in di.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //        return File(ZippedData, "application/zip", "SupportingDocument_"+WorkflowId+".zip");
        //    }
        //    TempData["Error"] = "No File/Data Found";
        //    return RedirectToAction("Index");//to be implemented
        //}

        //Change method for all workflows
        //[CustomAuthorize]
        //public ActionResult Change(int WorkflowId, int StepId, int TransactionId, string ActionName, string Source)
        //{
        //    int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
        //    int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
        //    if (string.IsNullOrEmpty(ActionName))
        //        ActionName = "Change";
        //      var isAuthorized = Globals.CheckActionAuthorization(ActionName, LoggedInRoleId, LoggedInUserId, WorkflowId, StepId);
        //    if (isAuthorized)
        //    {
        //        var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
        //        System.Web.HttpContext.Current.Session["Title"] = "Change" + WFName;
        //        switch (WFName)
        //        {
        //            case "LocalPobs":
        //                ///LLocalPOB/Edit/11?WorkflowId=1&ActionName=Create
        //                // TempData["Error"] = "Functionality coming soon";
        //                // break;
        //                return RedirectToAction("Change", "LLocalPOB", new { /*WorkflowId = WorkflowId,*/ ActionName = ActionName, id = TransactionId, Source = Source /*, StepId = StepId*/ });
        //            case "RequestsContract":
        //            case "RequestsPPM":
        //                return RedirectToAction("Change", "LRequests", new { /*WorkflowId = WorkflowId, StepId = StepId*/ id = TransactionId });
        //            case "Products":
        //                return RedirectToAction("Change", "LProducts", new {/* WorkflowId = WorkflowId,*/ ActionName = ActionName, id = TransactionId, Source = Source/*, StepId = StepId */});
        //            case "Users":
        //                return RedirectToAction("Create", "LUsers", new { FormType = ActionName, TransactionId = TransactionId/*, StepId = StepId */});
        //            case "References":
        //                return RedirectToAction("Change", "LReferences", new { id = TransactionId , /*WorkflowId = WorkflowId, StepId = StepId */});
        //            case "AccountingScenario":
        //                return RedirectToAction("Change", "LAccountingScenarios", new { Id = TransactionId/*, WorkflowId = WorkflowId, StepId = StepId*/ });
        //            case "ScenarioDemand":
        //                return RedirectToAction("Edit", "LScenarioDemand", new { Id = TransactionId/*, WorkflowId = WorkflowId, StepId = StepId*/, FormType = ActionName });
        //        }
        //        return RedirectToAction("Index");//to be implemented
        //    }
        //    else
        //    {
        //        TempData["Error"] = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
        //        return RedirectToAction("Index");
        //    }
        //}

        //Get Grid Counts used for server side filtering and Tab Heading//
        [CustomAuthorize]
        public JsonResult GetGenericGridCounts(Nullable<int> WStepID, string TabName)
        {
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var counts=RestClient.GetGenericGridCounts(WStepID.Value,LoggedInUserId,WFName);
            return Json(counts, JsonRequestBehavior.AllowGet);
        }
        //
        //Generic method to get grid data//
        //The variable names a pagesize and pagenum are default for generic grid       
        [CustomAuthorize]
        public JsonResult GetGridData(Nullable<int> WStepID, string sortdatafield, string sortorder, int pagesize, int pagenum, string TabName)
        {
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            var WFName = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetGridDataByWorkflowId(WStepID.Value, LoggedInUserId, WFName, pagesize, pagenum, sortdatafield, sortorder, FilterQuery, LoggedInRoleId);

            return Json(ApiData, JsonRequestBehavior.AllowGet);

        }

        ////This method will be called when we click on any action in GenericGrid
        //[HttpGet]
        //[CustomAuthorize]
        ////public ActionResult UpdateActionStatus(int WorkflowId, int StepId, string TransactionId, string ActionName, string Comments,int StepParticipantActionId)
        //public ActionResult UpdateActionStatus(string TransactionId, string Comments, int StepParticipantActionId)
        //{
        //    Comments = Comments.Replace('@', '\n');//for newline character @ is being used in javascript. Replacing back it to newline character
        //    int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
        //    int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
        //    string CompanyCode = Globals.GetSessionData("CompanyCode");
        //    //Calculating WrkFlowId,StepId,ActionName from StepParticipantId
        //    IWStepParticipantActionsRestClient WPARC = new WStepParticipantActionsRestClient();
        //    var ParticipantActionData = WPARC.GetWFIdStepIdById(StepParticipantActionId);

        //    var isAuthorized = Globals.CheckActionAuthorization(ParticipantActionData.ActionName, LoggedInRoleId, LoggedInUserId, ParticipantActionData.WorkFlowId, ParticipantActionData.StepId);
        //    if (isAuthorized)
        //    {
        //        string WorkflowName = null;
        //        try
        //        {
        //            if (String.IsNullOrEmpty(TransactionId))
        //                return RedirectToAction("Index");
        //            var TransactionArray = TransactionId.Split(',').ToList();
        //            //foreach (var TranId in TransactionArray)
        //            //{
        //            //    var Id = Convert.ToInt32(TranId);

        //                if (string.IsNullOrEmpty(Comments))
        //                {
        //                    Comments = "";
        //                }
        //                //WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
        //                //WFName is calculatedon the basis of ParticipantAction
        //                WorkflowName = ParticipantActionData.WorkflowName;
        //                RestClient.UpdateActionStatus(WorkflowName, TransactionId, CompanyCode, ParticipantActionData.ActionName, LoggedInUserId, Comments, LoggedInRoleId, string.Empty);
        //            //}
        //            //Product is not versioned any more.
        //            if (ParticipantActionData.ActionName.Equals("Duplicate")) {
        //                if (WorkflowName.Equals("LocalPobs"))
        //                    @TempData["Message"] = "New version has been created and placed in Initial tab";
        //                else if (WorkflowName.Equals("Products"))
        //                    @TempData["Message"] = "New copy of Product(s) created with ProductCode appended with _dup# and placed in initial tab. Please remember to update Product Code as needed.";
        //            }
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    else
        //    {
        //        TempData["Error"] = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
        //        //return View("UnAuthorized");
        //        return RedirectToAction("Index");
        //    }
        //}
        //This method will be called when we click on any action in GenericGrid
        //[HttpPost]
        [CustomAuthorize]
        public JsonResult UpdateActionStatus(string TransactionId, string Comments, int StepParticipantActionId)
        {
            Comments = Comments.Replace('@', '\n');//for newline character @ is being used in javascript. Replacing back it to newline character
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            string CompanyCode = Globals.GetSessionData("CompanyCode");
            //Calculating WrkFlowId,StepId,ActionName from StepParticipantId
            IWStepParticipantActionsRestClient WPARC = new WStepParticipantActionsRestClient();
            var ParticipantActionData = WPARC.GetWFIdStepIdById(StepParticipantActionId);

            var isAuthorized = Globals.CheckActionAuthorization(ParticipantActionData.ActionName, LoggedInRoleId, LoggedInUserId, ParticipantActionData.WorkFlowId, ParticipantActionData.StepId);
            if (isAuthorized)
            {
                string WorkflowName = null;
                try
                {
                    if (String.IsNullOrEmpty(TransactionId))
                        return Json(new { success = false, message = "Transaction is required.", redirectUrl = Url.Action("Index") });
                    //return RedirectToAction("Index");
                    var TransactionArray = TransactionId.Split(',').ToList();

                    if (string.IsNullOrEmpty(Comments))
                    {
                        Comments = "";
                    }
                    //WFName is calculatedon the basis of ParticipantAction
                    WorkflowName = ParticipantActionData.WorkflowName;
                    RestClient.UpdateActionStatus(WorkflowName, TransactionId, CompanyCode, ParticipantActionData.ActionName, LoggedInUserId, Comments, LoggedInRoleId, string.Empty);

                    //Product is not versioned any more.
                    if (ParticipantActionData.ActionName.Equals("Duplicate"))
                    {
                        if (WorkflowName.Equals("LocalPobs"))
                            @TempData["Message"] = "New version has been created and placed in Initial tab";
                        else if (WorkflowName.Equals("Products"))
                            @TempData["Message"] = "New copy of Product(s) created with ProductCode appended with _dup# and placed in initial tab. Please remember to update Product Code as needed.";
                    }
                    //return RedirectToAction("Index");
                    return Json(new { success = true, message = "Action status updated successfully.", redirectUrl = Url.Action("Index") });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    //return RedirectToAction("Index");
                    return Json(new { success = false, message = ex.Data["ErrorMessage"].ToString(), redirectUrl = Url.Action("Index") });
                }
            }
            else
            {
                TempData["Error"] = "Sorry , You are not authorised to perform this action .Please contact L2 Admin";
                //return View("UnAuthorized");
                //return RedirectToAction("Index");
                return Json(new { success = false, message = TempData["Error"].ToString(), redirectUrl = Url.Action("Index") });
            }
        }

        public ActionResult DownloadGridData(Nullable<int> WStepID, string TabName)
        {
            int LoggedInUserId = Convert.ToInt32(Globals.GetSessionData("UserId"));
            int LoggedInRoleId = Convert.ToInt32(Globals.GetSessionData("CurrentRoleId"));
            var Workflow = System.Web.HttpContext.Current.Session["WorkFlow"] as string;
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            string FileName = "";
            var model = RestClient.DownloadGetGridDataByWorkflowId(WStepID.Value, LoggedInUserId, Workflow, LoggedInRoleId, CompanyCode, TabName);
            if (model != null)
            {
                FileName = model.Name;
            }
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketTempUploadFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + FileName;
            var fileData = Globals.DownloadFromS3(S3TargetPath);           
            var newfileName = Workflow + "_" + TabName + "_" + DateTime.UtcNow.ToString("dd-MM-yyyy") + ".xlsx";
            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", newfileName);
        }
    }
}