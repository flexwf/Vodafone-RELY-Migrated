using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
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
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class MMenuRolesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public IHttpActionResult GetMMenuRoles(string UserName, string WorkFlow)
        {
            var xx = (from aa in db.MMenuRoles
                             select new
                             {
                                 aa.Id,
                                 aa.MenuId,
                                 aa.RoleId
                             }).OrderBy(a =>a.Id);

            return Ok(xx);                   
        }

        // GET: api/MMenuRoles/5
        [ResponseType(typeof(MMenuRole))]
        public async Task<IHttpActionResult> GetMMenuRoles(Nullable<int> Id, string UserName, string WorkFlow)
        {
            var MMenuRoles = db.MMenuRoles.Where(p => p.Id == Id).Select(aa => new { aa.Id, aa.MenuId, aa.RoleId }).FirstOrDefault();
            if (MMenuRoles == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
            }
            return Ok(MMenuRoles);
        }

        // PUT: api/MMenuRoles/5
        [ResponseType(typeof(MMenuRole))]
        public async Task<IHttpActionResult> PutMMenuRole(int id, MMenuRole MMenuRole, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
            }

            if (!MMenuRoleExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
            }

            if (id != MMenuRole.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
            }
            try
            {
                db.Entry(MMenuRole).State = EntityState.Modified;
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
            return Ok(MMenuRole);
        }

        // POST: api/MMenuRoles
        [ResponseType(typeof(MMenuRole))]
        public async Task<IHttpActionResult> PostMMenuRole(MMenuRole MMenuRole, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ROLE")));
            }
            try
            {
                db.MMenuRoles.Add(MMenuRole);
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
            return CreatedAtRoute("DefaultApi", new { id = MMenuRole.Id }, MMenuRole);
        }

        // DELETE: api/MMenuRoles/5
        [ResponseType(typeof(MMenuRole))]
        public async Task<IHttpActionResult> DeleteMMenuRole(int id, string UserName, string WorkFlow)
        {
            MMenuRole MMenuRole = await db.MMenuRoles.FindAsync(id);
            if (MMenuRole == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MMenuRole")));
            }

            try
            {
                db.MMenuRoles.Remove(MMenuRole);
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
                    throw ex;
                }
            }
            return Ok(MMenuRole);
        }
            private bool MMenuRoleExists(int id)
        {
            return db.MMenuRoles.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            
           if (SqEx.Message.IndexOf("UQ_MMenuRoles_MenuId_RoleId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "MENU ROLES"));
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

        public async Task<IHttpActionResult> GetDataForMenuRoleMapping(string CompanyCode)
        {
            string strQuery = "Exec SpGetMenuRoleMapping @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            DataTable dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

        public async Task<IHttpActionResult> GetColumnForMenuRoles(string CompanyCode)
        {
            
            //This query is same as used in SpGetMenuRoleMapping for getting list of Columns.Any Change in  this query should be implemented in Stored Procedure as well
            var xx = (from aa in db.LRoles.Where(r => r.RoleName != "L2Admin" && r.CompanyCode==CompanyCode)
                      select new
                      {
                          aa.RoleName,
                          aa.Id,
                      }).OrderBy(p => p.Id);
            return Ok(xx);
        }

      // [HttpGet]
       public async Task<IHttpActionResult> SaveDataForMenuRole(List<UpdateMappingMenuRoleViewModel> ObjectRole, string CompanyCode)

        { 
            //Following commented portion can be used to make this method Get for testing point of view

            //List<UpdateMappingMenuRoleViewModel> ObjectRole = new List<UpdateMappingMenuRoleViewModel>();

            //var OR = new UpdateMappingMenuRoleViewModel();
            //OR.MenuId = 24; OR.ColumnName = "Admin"; OR.NewResponse = true; OR.OldResponse = false;
            //ObjectRole.Add(OR);
            // var AOR = new UpdateMappingMenuRoleViewModel();
            // AOR.MenuId = 6; AOR.ColumnName = "Controller"; AOR.NewResponse = false; AOR.OldResponse = true;
            //ObjectRole.Add(AOR);
            DataTable dt = new DataTable();
            dt.Columns.Add("MenuId");
            dt.Columns.Add("ColumnName");
            dt.Columns.Add("NewResponse");
            dt.Columns.Add("OldResponse");
            dt.Columns.Add("CompanyCode");
            foreach (var ObjectMapRole in ObjectRole)
            {
               
                DataRow dr = dt.NewRow(); // Creating a new Data Row

                dr[0] = ObjectMapRole.MenuId;
                dr[1] = ObjectMapRole.ColumnName;
                dr[2] = ObjectMapRole.NewResponse;
                dr[3] = ObjectMapRole.OldResponse;
                dr[4] = CompanyCode;
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }

             SqlParameter param = new SqlParameter();
            param.SqlDbType = SqlDbType.Structured;
            param.ParameterName = "@tvp";
            param.TypeName = "dbo.CategoryTableType"; //This TableType is defined in database.It is a definition of table Type variable.
            param.Value = dt;
            db.Database.ExecuteSqlCommand("exec SpUpdateMenuRoleMapping @tvp", param);
            return Ok();

        }


    }
}
