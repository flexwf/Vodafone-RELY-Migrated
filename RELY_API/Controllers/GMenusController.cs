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
using System.Data.Entity.Core.Objects;
using System.Data;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class GMenusController : ApiController
    {

        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/GMenus
        public IHttpActionResult GetGMenus(string CompanyCode)
        {
            //string strGenerateQuery = "select id,MenuName,ParentId,MenuURL,OrdinalPosition from GMenus_Copy where CompanyCode={0} order by OrdinalPosition";
            //var MenuList = db.Database.SqlQuery<GMenuViewModel>(strGenerateQuery, CompanyCode).ToList();//For testing I created GMenuViewModel,So delete it after tested
            //return Ok(MenuList);
            var xx = (from aa in db.GMenus.Where(p => p.CompanyCode == CompanyCode)
                      select new { aa.Id, aa.MenuName, aa.ParentId, aa.MenuURL, aa.OrdinalPosition }).OrderBy(p => p.OrdinalPosition);//.OrderBy(p => p.OrdinalPosition);
            return Ok(xx);
        }

        
        public IHttpActionResult GetGMenusRolesByUserRole(string UserRole, string CompanyCode, string UserName, string WorkFlow)
        {
            var xx = (from aa in db.MMenuRoles.Include(p => p.GMenu).Include(p => p.LRole).Where(p => p.LRole.RoleName.Equals(UserRole)).Where(p => p.LRole.CompanyCode == CompanyCode).Where(aa => aa.GMenu.CompanyCode == CompanyCode)
                      select new { aa.Id, aa.MenuId, aa.RoleId, aa.GMenu.MenuName, aa.LRole.RoleName, aa.GMenu.ParentId, aa.GMenu.MenuURL, aa.GMenu.OrdinalPosition }).ToList().OrderBy(p => p.OrdinalPosition);
            return Ok(xx);
        }


        // GET: api/GMenus/5
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> GetGMenu(Nullable<int> id, string UserName, string WorkFlow)
        {
            var GMenu = db.GMenus.Where(p => p.Id == id).Select(aa => new { aa.Id, aa.MenuName, aa.ParentId, aa.MenuURL, aa.OrdinalPosition }).FirstOrDefault();
            if (GMenu == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            return Ok(GMenu);
        }

        // PUT: api/GMenus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGMenu(int id, GMenu GMenu, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
            }
                if (!GMenuExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
                }

                if (id != GMenu.Id)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "MENU")));
                }
                try
                {
                    db.Entry(GMenu).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
            return Ok(GMenu);
        }

        // POST: api/GMenus
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> PostGMenu(GMenu GMenu, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "MENU")));
            }
                try
                {
                    db.GMenus.Add(GMenu);
                    await db.SaveChangesAsync();
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
               
            return CreatedAtRoute("DefaultApi", new { id = GMenu.Id }, GMenu);
        }

        // DELETE: api/GMenus/5
        [ResponseType(typeof(GMenu))]
        public async Task<IHttpActionResult> DeleteGMenu(int id, string UserName, string WorkFlow)
        {
            GMenu GMenu = await db.GMenus.FindAsync(id);
            if (GMenu == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
                try
                {
                    db.GMenus.Remove(GMenu);
                    await db.SaveChangesAsync();
                    
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
            return Ok(GMenu);
        } 

        private bool GMenuExists(int id)
        {
            return db.GMenus.Count(e => e.Id == id) > 0;
        }

        [HttpPost]
        
        public async Task<IHttpActionResult> ProcessMenuItems(GMenu GMenu, string MenuLabel, int MenuId, int ParentMenuId, string CompanyCode)
        
        {
            try
            {
                //GMenu GMenu = new GMenu();
                //GMenu.MenuName = null;
                //GMenu.MenuURL = null;

                string NewMenuName = (string.IsNullOrEmpty(GMenu.MenuName))?"": (GMenu.MenuName).ToString();
                string NewMenuURL = (string.IsNullOrEmpty(GMenu.MenuURL))?"": (GMenu.MenuURL).ToString();
               
                string strGenerateQuery = "Exec SpProcessMenuItems @MenuLabel,@MenuId,@ParentMenuId,@CompanyCode,@NewMenuName,@NewMenuURL";
                SqlCommand cmd = new SqlCommand(strGenerateQuery);
                cmd.Parameters.AddWithValue("@MenuLabel", MenuLabel);
                cmd.Parameters.AddWithValue("@MenuId", MenuId);
                cmd.Parameters.AddWithValue("@ParentMenuId", ParentMenuId);
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@NewMenuName", NewMenuName);
                cmd.Parameters.AddWithValue("@NewMenuURL", NewMenuURL);
                Globals.GetDataTableUsingADO(cmd);
                return Ok();
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

        }
        [HttpGet]
        public async Task<IHttpActionResult> GetMenuById(int id)
        {
            var GMenu = db.GMenus.Where(p => p.Id == id).Select(aa => new { aa.Id, aa.MenuName, aa.ParentId, aa.MenuURL, aa.OrdinalPosition }).FirstOrDefault();
            if (GMenu == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MENU")));
            }
            return Ok(GMenu);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message FK_GMenus_MMenuRoles_MenuId
            if (SqEx.Message.IndexOf("FK_GMenus_MMenuRoles_MenuId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "MENUS", "MENU ROLES"));
            //UNIQUE KEY MUST BE COME
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
    }
}
