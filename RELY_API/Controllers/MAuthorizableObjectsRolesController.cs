using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class MAuthorizableObjectsRolesController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //few of the methods are commented out as  MAuthorizableObjectsRoles table has been renamed due to which After model update code was throwing error.
        //GetCount
        //public IHttpActionResult GetCount(int UserRoleId, string CurrentActionKey)
        //{

        //              var xxx = (from ma in db.MAuthorizableObjectsRoles.Where(p => p.RoleId == UserRoleId)
        //               join ga in db.GAuthorizableObjects.Where(q => q.ControllerName.Trim() + "-" + q.MethodName.Trim() == CurrentActionKey)
        //               on ma.AuthorizableObjectId equals ga.Id
        //               select new
        //               {
        //                   RoleId = ma.Id
        //               });
        //    return Ok(xxx.Count());

        //}
        [HttpGet]
        public IHttpActionResult GetRolesListbyControllerAction(string CurrentActionKey, string CompanyCode)
        {
            //retrieve AuthorizableObjectId based from Controller and Action Name
            int ObjectId = db.GAuthorizableObjects.Where(q => q.ControllerName.Trim() + "-" + q.MethodName.Trim() == CurrentActionKey).Select(a => a.Id).FirstOrDefault();

            //Retrieve MenuId from mapping table based on AuthorizableObjectId received
            // int MenuId = db.MMenusAuthorizableObjects.Where(a => a.AuthorizableObjectId == ObjectId).Select(a => a.MenuId).FirstOrDefault();
            string qry = "select MA.MenuId from MMenusAuthorizableObjects MA inner join GMenus GM on MA.MenuId=GM.Id where MA.AuthorizableObjectId={0} and GM.CompanyCode={1}";
           // int MenuId = db.Database.SqlQuery<int>(qry, ObjectId, CompanyCode).FirstOrDefault();
            List<int> MenuIdList = db.Database.SqlQuery<int>(qry, ObjectId, CompanyCode).ToList();

            //find out Roles who has access to the above Menu
            //var RoleIdList = db.MMenuRoles.Where(a => a.MenuId == MenuId).Select(a => a.RoleId).ToList();
            var RoleIdList = db.MMenuRoles.Where(a => MenuIdList.Contains(a.MenuId)).Select(a => a.RoleId).ToList();

            return Ok(RoleIdList);

        }


        //public IHttpActionResult GetMAuthorizableObjectsRoles(string UserName, string WorkFlow)
        //{
        //    var xx = (from aa in db.MAuthorizableObjectsRoles
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LRole,
        //                  aa.AuthorizableObjectId

        //              }).OrderBy(a => a.LRole);
        //    return Ok(xx);
        //}

        //// GET: api/MAuthorizableObjectsRoles/5
        //[ResponseType(typeof(MAuthorizableObjectsRole))]
        //public async Task<IHttpActionResult> GetMAuthorizableObjectsRoles(Nullable<int> Id, string UserName, string WorkFlow)
        //{
        //    var MAuthorizableObjectsRoles = db.MAuthorizableObjectsRoles.Where(p => p.Id == Id).Select(aa => new { aa.Id, aa.AuthorizableObjectId, aa.RoleId }).FirstOrDefault();
        //    if (MAuthorizableObjectsRoles == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "AuthorizableObject")));
        //    }
        //    return Ok(MAuthorizableObjectsRoles);
        //}

        //// PUT: api/MAuthorizableObjectsRoles/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutMAuthorizableObjectsRole( MAuthorizableObjectsRole MAuthorizableObjectsRole, int id, string UserName, string WorkFlow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
        //    }

        //    if (!MAuthorizableObjectsRoleExists(id))
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ROLE")));
        //    }

        //    if (id != MAuthorizableObjectsRole.Id)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ROLE")));
        //    }
        //    try
        //    {
        //        db.Entry(MAuthorizableObjectsRole).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //        {
        //            //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //        }
        //        else
        //        {
        //            throw ex;//This exception will be handled in FilterConfig's CustomHandler
        //        }
        //    }
        //    // return StatusCode(HttpStatusCode.NoContent);
        //    return Ok(MAuthorizableObjectsRole);
        //}

        //// POST: api/MAuthorizableObjectsRoles
        //[ResponseType(typeof(MMenuRole))]
        //public async Task<IHttpActionResult> PostMAuthorizableObjectsRole(MAuthorizableObjectsRole MAuthorizableObjectsRole, string UserName, string WorkFlow)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ROLE")));
        //    }
        //    try
        //    {
        //        db.MAuthorizableObjectsRoles.Add(MAuthorizableObjectsRole);
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //        {
        //            //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //        }
        //        else
        //        {
        //            throw ex;//This exception will be handled in FilterConfig's CustomHandler
        //        }
        //    }
        //    return CreatedAtRoute("DefaultApi", new { id = MAuthorizableObjectsRole.Id }, MAuthorizableObjectsRole);
        //}

        //// DELETE: api/MAuthorizableObjectsRoles/5
        //[ResponseType(typeof(MAuthorizableObjectsRole))]
        //public async Task<IHttpActionResult> DeleteMAuthorizableObjectsRole(int id, string UserName, string WorkFlow)
        //{
        //    MAuthorizableObjectsRole MAuthorizableObjectsRole = await db.MAuthorizableObjectsRoles.FindAsync(id);
        //    if (MAuthorizableObjectsRole == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "MMenuRole")));
        //    }

        //    try
        //    {
        //        db.MAuthorizableObjectsRoles.Remove(MAuthorizableObjectsRole);
        //        await db.SaveChangesAsync();

        //    }
        //    catch (DbEntityValidationException dbex)
        //    {
        //        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
        //        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
        //        {
        //            //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry. 
        //            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }
        //    return Ok(MAuthorizableObjectsRole);
        //}

        private bool MAuthorizableObjectsRoleExists(int id)
        {
            return db.MMenuRoles.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
               if (SqEx.Message.IndexOf("UQ_MAuthorizableObjectsRoles_AuthorizableObjectId_RoleId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "AuthorizableObject Role Mapping"));
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


        //public async Task<IHttpActionResult> GetDataForAuthorizableObjectRole()
        //{
        //    string strQuery = "Exec SpGetObjectRoleMapping";
        //    SqlCommand cmd = new SqlCommand(strQuery);
        //    DataTable dt = Globals.GetDataTableUsingADO(cmd);
        //    return Ok(dt);
        //}

        public async Task<IHttpActionResult> GetColumnForAuthorizableObjectRole()
        {
            //This query is same as used in SpGetObjectRoleMapping for getting list of Columns.Any Change in  this query should be implemented in Stored Procedure as well
            var xx = (from aa in db.LRoles
                     select new
                      {
                          aa.RoleName,
                         aa.Id,
                     }).OrderBy(p => p.Id);
            return Ok(xx);
        }

       // [HttpGet]

        //public async Task<IHttpActionResult> TestMultiThreading(int RequestId)
        //{
          
        //    Task objThisTask = Task.Factory.StartNew(() =>
        //    {
                
        //        for (int i = 0; i < 1000; ++i)
        //        {
        //            System.Threading.Thread.Sleep(5000);
        //            SqlConnection SqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        //        string sql = "insert into debug(DebugValue,DebugDateTime) values(@value,@DT)";
        //        SqlCommand cmd1 = new SqlCommand(sql, SqlConnection);
        //        cmd1.Parameters.AddWithValue("@value", "TestMultiThreading");
        //        cmd1.Parameters.AddWithValue("@DT", DateTime.UtcNow);
        //        SqlConnection.Open();
        //        cmd1.ExecuteNonQuery();
        //        SqlConnection.Close();
        //        // Task.Delay(2000000);
        //        // Task.Delay(TimeSpan.FromSeconds(60));
        //        }
        //    });
          
        //    return Ok();
        //}



            //public async Task<IHttpActionResult> SaveDataForAuthorizableObjectRole(List<UpdateMappingAuthorizableObjectRoleViewModel> ObjectRole)
            ////public async Task<IHttpActionResult> SaveDataForAuthorizableObjectRole()
            //{

            //    //List<UpdateMappingAuthorizableObjectRoleViewModel> ObjectRole = new List<UpdateMappingAuthorizableObjectRoleViewModel>();

            //    //var OR = new UpdateMappingAuthorizableObjectRoleViewModel();
            //    //OR.AuthorizableId = 640;OR.ColumnName = "Accountant";OR.NewResponse = true;OR.OldResponse = false;
            //    //ObjectRole.Add(OR);
            //    //var AOR = new UpdateMappingAuthorizableObjectRoleViewModel();
            //    //AOR.AuthorizableId = 639; AOR.ColumnName = "Controller"; AOR.NewResponse = true; AOR.OldResponse = false;
            //    //ObjectRole.Add(AOR);
            //    DataTable dt = new DataTable();
            //    dt.Columns.Add("AuthorizableId");
            //    dt.Columns.Add("ColumnName");
            //    dt.Columns.Add("NewResponse");
            //    dt.Columns.Add("OldResponse");
            //    foreach (var ObjectMapRole in ObjectRole)
            //    {
            //        //db.Entry(ObjectMapRole).State = EntityState.Modified;
            //        //await db.SaveChangesAsync();
            //        //DataColumn dc=
            //        DataRow dr = dt.NewRow(); // Creating a new Data Row

            //        dr[0] = ObjectMapRole.AuthorizableId;
            //        dr[1] = ObjectMapRole.ColumnName;
            //        dr[2] = ObjectMapRole.NewResponse;
            //        dr[3] = ObjectMapRole.OldResponse;
            //        dt.Rows.Add(dr);
            //        dt.AcceptChanges();
            //    }



            //    //db.Database.ExecuteSqlCommand("Exec SpUpdateObjectRoleMapping {0}", dt);
            //    SqlParameter param = new SqlParameter();
            //    param.SqlDbType = SqlDbType.Structured;
            //    param.ParameterName = "@tvp";
            //    param.TypeName = "dbo.CategoryTableType"; //This TableType is defined in database.It is a definition of table Type variable.
            //    param.Value = dt;
            //    db.Database.ExecuteSqlCommand("exec SpUpdateObjectRoleMapping @tvp", param);


            //    // return StatusCode(HttpStatusCode.NoContent);
            //    return Ok();

            //}


        }


}
