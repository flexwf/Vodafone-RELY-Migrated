using RELY_APP.Helper;
using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static RELY_APP.Utilities.Globals;

namespace RELY_APP.Controllers
{

    [SessionExpire]
    [HandleCustomError]
    public class WStepParticipantActionsController : Controller
    {
        IWStepParticipantActionsRestClient RestClient = new WStepParticipantActionsRestClient();
               
        public JsonResult GetByParticipantIdStepId(int ParticipantId,int StepId)
        {
            var ApiData = RestClient.GetByParticipantIdStepId(ParticipantId,StepId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActions()
        {
            var ApiData = RestClient.GetAllActions();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ControllerActionFilter]
        public JsonResult SaveData(object[] GridData, int StepId,int ParticipantId)
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            try
            {
                IWStepParticipantRestClient SPRC = new WStepParticipantRestClient();
                var WfDetails = SPRC.GetById(ParticipantId);
                if (WfDetails == null)
                {
                    var OutputJson2 = new { ErrorMessage = "Participant Not Found", PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                }
                if (GridData != null && GridData.Length > 0)
                {
                    foreach (string[] Data in GridData)
                    {
                        var GridArray = Data;
                        var model = new WStepParticipantActionViewModel();
                        //As SendToStepId column is nullable int, so converting GridArray string value to Int is converting it to 0 when null.
                        //Could not find other way out to set NULL, so setting model properties like this.
                            model.Id = Convert.ToInt32(GridArray[0]);
                            model.WActionId = Convert.ToInt32(GridArray[1]);
                            model.Label = String.IsNullOrEmpty(GridArray[2]) ? null : GridArray[2];
                            model.Glymph = String.IsNullOrEmpty(GridArray[3]) ? null : GridArray[3];
                            model.ShowInStepId = StepId;
                            model.ButtonOnWfGrid = Convert.ToBoolean(GridArray[5]);
                            model.ButtonOnForm = Convert.ToBoolean(GridArray[6]);
                            model.VisibilityFunction = String.IsNullOrEmpty(GridArray[7]) ? null : GridArray[7];
                            model.ParticipantId = ParticipantId;
                            model.ParticipantType = GridArray[9];
                            model.Description = String.IsNullOrEmpty(GridArray[10]) ? null : GridArray[10];
                            model.IsLinkOverWFGrid = Convert.ToBoolean(GridArray[11]);
                            model.ValidationFunction = String.IsNullOrEmpty(GridArray[12]) ? null : GridArray[12];
                            model.SendToStepId = Convert.ToInt32(String.IsNullOrEmpty(GridArray[13]) ? null : GridArray[13]);
                            if(model.SendToStepId == 0)
                            {
                                model.SendToStepId = null;
                            }
                            model.ActionUrl = String.IsNullOrEmpty(GridArray[14]) ? null : GridArray[14];
                        RestClient.Add(model, null);
                    }

                }
                var OutputJson = new { Success = "Create Participant Actions", ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/RWorkFlows/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }

        public JsonResult DeleteById(int Id)
        {
            try
            {
                RestClient.Delete(Id, null);
                var OutputJson = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = "/RWorkFlows/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be updated";
                    throw ex;
                }
            }
        }

    }
}