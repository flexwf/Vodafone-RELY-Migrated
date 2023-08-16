using RELY_APP.Utilities;
using RELY_APP.ViewModel;
using RELY_APP.Helper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RELY_APP.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class TestingController : Controller
    {       
         OleDbConnection con = new OleDbConnection(@"Data Source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com; Initial Catalog = RELYDevDb; User ID=euitdsadbmuser; Password=d-4nATAk&gezac; Provider=SQLOLEDB");
         DataTable dt = new DataTable();
        // GET: Testing
        public ActionResult Index()
        {
            //string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
            //string query = null;
            //string connString = "";
            //string ExcelFileName = "";

            //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            dt = Globals.ImportDataFromExcel(@"C:\Users\arvkum\Desktop\EXCELPOC.xlsx");
            ViewBag.Data = dt;
            //dt = Globals.ImportDataFromExcel(@"C:\Users\arvkum\Desktop\EXCELPOC.xlsx");

            //string query = "select * From GCopaDimensions";
            //DataTable dt = new DataTable();
            //con.Open();
            //OleDbDataAdapter da = new OleDbDataAdapter(query, con);
            //da.Fill(dt);
            //string Result = Globals.ExportToExcel(dt, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Arvind" + System.DateTime.Now.ToString("ddMMyyyyhhmmss"));
            //if (Result.IndexOf("success", StringComparison.OrdinalIgnoreCase) >= 0)
            //{
            //    ViewBag.Result = "succeeded";
            //}
            //else
            //{
            //    ViewBag.Result = "Failed";
            //}
            //con.Close();
            return View();
        }

        // [HttpPost]
        //public ActionResult Import()
        //{
        //    //    string ExcelFileName = "";

        //    //    //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //    DataTable dt = Globals.ImportDataFromExcel(@"C:\Users\arvkum\Desktop\EXCELPOC.xlsx");
        //    ViewBag.Data = dt;
        //    return View();

        //}

    }

}