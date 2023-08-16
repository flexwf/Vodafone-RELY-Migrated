using RELY_APP.Helper;
using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LFSUserResponseController : Controller
    {
        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        //int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
        //string loggedInUser = System.Web.HttpContext.Current.Session["LoginEmail"].ToString();
        //string UserRoleName = System.Web.HttpContext.Current.Session["CurrentRoleName"].ToString();
        //int UserRoleId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentRoleId"].ToString());
        //string WorkFlowName = System.Web.HttpContext.Current.Session["WorkFlow"].ToString();

        IRLFSResponsRestClient RestClient = new LFSResponsRestClient();
        ILFSTableConfigRestClient TCRC = new LFSTableConfigRestClient();
        ILProductsRestClient PRC = new LProductsRestClient();
        ILFSTablesRestClient TRC = new LFSTablesRestClient();
        // GET: LFSUserResponse
        [CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet][CustomAuthorize]
        public ActionResult CaptureUserResponse(int SurveyId, string ChapterCode,string SectionCode,int EntityId,string EntityType,string SectionName,string ChapterName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //for displaying Products details with the Section Name and ChapterName
            var Product = PRC.GetById(EntityId);
            string ProductName = ""; string ProductCode = "";
            if(Product!= null)
            {
                //Finding Name and code for the Product
                ProductName = Product.Name;
                ProductCode = Product.ProductCode;
                //fetch substring of length 20
                if (!string.IsNullOrEmpty(ProductName) && ProductName.Length > 20)
                {
                    ProductName = ProductName.Substring(0, 20);
                }
                if (!string.IsNullOrEmpty(ProductCode) && ProductCode.Length > 20)
                {
                    ProductCode = ProductCode.Substring(0, 20);
                }
            }
            ViewBag.SectionName = ChapterName + " - " + SectionName + " (" + ProductName + " - " + ProductCode + ")" ;
            //Below line will decide how to display User Response Partial View
            ViewBag.CaptureUserResponse = new CaptureUserResponseViewModel { SurveyId = SurveyId, ChapterCode = ChapterCode, SectionCode = SectionCode,EntityId = EntityId,EntityType = EntityType };
            ILFSSectionItemRestClient SIRC = new LFSSectionItemRestClient();
            var ApiData = SIRC.GetBySectionCode(SectionCode, ChapterCode, EntityId, EntityType, CompanyCode,SurveyId);
            //select count(*) as VTableCount  INTO where ItemType = TABLE(from within a section).
            int TableCount = SIRC.GetTableItemsCountForSection(SectionCode, ChapterCode, SurveyId);
            
            if(TableCount == 0)// show Normal grid
            {
                if(ApiData!= null && ApiData.Count() > 0)
                  ViewBag.FirstQuestionToRender = ApiData.ElementAt(0);
                //tooltip manipulation to replace LF with <br> tags
                for (int i = 0; i < ApiData.Count(); i++)
                {
                    //tooltip manipulation for LF characters
                    string tooltip = ApiData.ElementAt(i).ToolTip;
                    if (tooltip != null)
                    {
                        tooltip = tooltip.Replace("\r\n", "<br>");
                        tooltip = tooltip.Replace("!!", "<br/>");
                    }
                    ApiData.ElementAt(i).ToolTip = tooltip;

                    //response manipulation for LF characters
                    string response = ApiData.ElementAt(i).Response;
                    if (response != null)
                    {
                        response = response.Replace("\n\r", "");
                    }
                    ApiData.ElementAt(i).Response = response;
                }
                ViewBag.GridSectionItems = ApiData;
                return View();
            }
            else if(TableCount == 1)//Else If VTableCount = 1-- there is 1 table in thios section
            {

                int ItemCount = SIRC.GetItemsCountExcludingTable(SectionCode, ChapterCode, SurveyId);
                if (ItemCount == 0)//show Table Grid
                {
                    List<LFSSectionItemViewModel> OtherSectionItems = new List<LFSSectionItemViewModel>();
                    foreach(var item in ApiData) { //get the item having TableCode populated
                        if (item.TableCode != null)//retrieve the details for that SectionItem
                        {
                            var TableData = TRC.GetByTableCode(item.TableCode, CompanyCode);
                            int ColumnsCount = TableData.NoOfCols;
                            int RowsCount = TableData.NoOfRows;
                            ViewBag.ColumnsCount = ColumnsCount;
                            ViewBag.TableData = TableData;
                        }
                        else if(item.ItemTypeName == "TEXT")
                        {
                            OtherSectionItems.Add(item);
                        }
                        
                    }
                    ViewBag.OtherSectionItems = OtherSectionItems;
                    return View("LFSTableResponse");
                }
                else
                {
                    //Error message 'A Section with Table can have only one table and conclusion text Items'
                    TempData["Error"] = "A Section with Table can have only one table and conclusion text Items";
                    return View("LFSTableResponse");
                }
            }
            else//--there are multiple tables in the section,  Error message 'Multiple Tables cannot exist in one section'
            {
                TempData["Error"] = "Multiple Tables cannot exist in one section";
                return View("LFSTableResponse");
            }
           /* 
            if (ApiData == null) { }
            else if(ApiData.Count() == 1 && ApiData.ElementAt(0).TableCode != null)
            {
                //Getting Table data for the TableCode. As There will be only one table associated to one section, we are considering only first element
                
                var TableData = TRC.GetByTableCode(ApiData.ElementAt(0).TableCode, CompanyCode);
                int ColumnsCount = TableData.NoOfCols; 
                int RowsCount = TableData.NoOfRows;
                ViewBag.ColumnsCount = ColumnsCount;
                //var ColumnData= TCRC.GetByAxis("col", ApiData.ElementAt(0).TableCode, CompanyCode);
                ViewBag.TableData = TableData;
                //ViewBag.ColumnData = ColumnData;
                return View("LFSTableResponse");
            }
            else if (ApiData != null && ApiData.Count() > 0)
            {
                ViewBag.FirstQuestionToRender = ApiData.ElementAt(0);
            }
            ViewBag.GridSectionItems = ApiData;
            return View();*/
        }

        //public JsonResult GetConfigByAxis(string Axis,string TableCode)
        //{
        //    var ApiData =TCRC.GetByAxis(Axis,TableCode,CompanyCode);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}GetByColRow

        [CustomAuthorize]
        public JsonResult GetConfigByColRow(int Col,int Row, string TableCode)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = TCRC.GetByColRow(Col,Row, TableCode, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost][CustomAuthorize]
        public ActionResult SaveData(string QuestionCode, string Response, int EntityId, string EntityType)
        {
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            var model = new LFSResponsViewModel
            {
                QuestionCode = QuestionCode,
                Response = Response,
                EntityId = EntityId,
                EntityType = EntityType,
                IsReponseAutoGenerated = false,
                CreatedById = LoggedInUserId,
                UpdatedById = LoggedInUserId,
                CreatedDateTime = DateTime.Now,
                UpdatedDateTime = DateTime.Now
            };
            // RestClient.Add(model, null);
            RestClient.Add(model, EntityId, EntityType, CompanyCode,null);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost][CustomAuthorize]
        public ActionResult SaveTableResponse(string TableCode, string Response, int EntityId, string EntityType,int Col,int Row)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());

            ILFSTableResponseRestClient TRRC = new LFSTableResponseRestClient();
            var model = new LFSTableResponseViewModel
            {
                TableCode = TableCode,
                Response = Response,
                EntityId = EntityId,
                EntityType = EntityType,
                Col = Col,
                Row = Row
            };
            TRRC.Add(model,CompanyCode,LoggedInUserId, null);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize]
        public JsonResult GetTableGridData( string TableCode,int EntityId, string EntityType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            var ApiData = TRC.GetSurveyTableLeftGrid(CompanyCode,TableCode,EntityId,EntityType);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


    }
}