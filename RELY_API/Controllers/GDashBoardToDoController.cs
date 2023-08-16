using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace RELY_API.Controllers
{
    public class GDashBoardToDoController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        // GET: ProductHistoryGrid
        //public ActionResult Index()
        //{

        //    return View();
        //}


        //This Function will fetch data from stored Procedure to datatable
        public async Task<IHttpActionResult> GetDataForDashBoardToDo(int RoleId,int UserId)
        {
            string strQuery = "Exec SpGenerateDashBoardToDo @RoleId,@UserId";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }




    }
}