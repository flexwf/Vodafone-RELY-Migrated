using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class LAuditController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        // GET: api/LAudit/5
        [ResponseType(typeof(LAudit))]
        public async Task<IHttpActionResult> GetByEntityTypeEntityId(string EntityType,string EntityIdList)
        {
            var EntityIds = EntityIdList.Split(',');
            var xx = (from aa in db.LAudits
                      join bb in db.LUsers on aa.ActionedById equals bb.Id 
                      join cc in db.LRoles on aa.ActionedByRoleId equals cc.Id
                             where EntityIds.Contains(aa.EntityId.ToString()) && aa.EntityType == EntityType
                             select new
                             {
                                 aa.Id,aa.RelyProcessName,aa.VFProcessName,aa.ControlCode,aa.ControlDescription,
                                 aa.Action,
                                 aa.ActionedById,
                                 UserName = bb.LoginEmail,
                                 aa.ActionedByRoleId,
                                 UserRole = cc.RoleName,
                                 aa.ActionType,
                                 aa.ActionDateTime,
                                 aa.OldStatus,aa.NewStatus,aa.EntityType,aa.EntityId,aa.EntityName,aa.WorkFlowId,
                                 aa.CompanyCode,aa.Comments,aa.ActionLabel
                             }).ToList().OrderByDescending(p => p.ActionDateTime);

            return Ok(xx);
        }

        // GET: api/LAudit/5
        [ResponseType(typeof(LAudit))]
        public async Task<IHttpActionResult> GetByTypeEntityId(string EntityType, int EntityId)
        {
            //var xx = (from aa in db.LAudits
            //          join bb in db.LUsers on aa.ActionedById equals bb.Id
            //          join cc in db.LRoles on aa.ActionedByRoleId equals cc.Id
            //          join ss in db.WSteps on aa.StepId equals ss.Id
            //          where aa.EntityId == EntityId && aa.EntityType == EntityType
            //          select new
            //          {
            //              aa.Id,aa.ActionLabel,
            //              aa.RelyProcessName,
            //              aa.VFProcessName,
            //              aa.ControlCode,
            //              aa.ControlDescription,
            //              aa.Action,
            //              aa.ActionedById,
            //              UserName = bb.LoginEmail,
            //              aa.ActionedByRoleId,
            //              UserRole = cc.RoleName,
            //              aa.ActionType,
            //              aa.ActionDateTime,
            //              aa.OldStatus,
            //              aa.NewStatus,
            //              aa.EntityType,
            //              aa.EntityId,
            //              aa.EntityName,
            //              aa.WorkFlowId,
            //              aa.CompanyCode,
            //              aa.Comments,
            //              StepName = ss.Name
            //          }).ToList().OrderByDescending(p => p.ActionDateTime);
            string SqlString = "select aa.Id,aa.ActionLabel,aa.RelyProcessName,aa.VFProcessName,aa.ControlCode,aa.ControlDescription,aa.Action,aa.ActionedById,"
                          + " UserName = bb.LoginEmail,aa.ActionedByRoleId,UserRole = cc.RoleName,aa.ActionType,aa.ActionDateTime,aa.OldStatus,aa.NewStatus,aa.EntityType,"
                          + " aa.EntityId,aa.EntityName,aa.WorkFlowId,aa.CompanyCode,aa.Comments,StepName = ss.Name from LAudit aa inner join LUsers bb on aa.ActionedById = bb.Id "
                             + " inner join LRoles cc on aa.ActionedByRoleId = cc.Id left outer join WSteps ss on aa.StepId = ss.Id "
                             + " where aa.EntityId = {0} and aa.EntityType = {1} order by aa.ActionDateTime desc";
            var xx = db.Database.SqlQuery<LAuditViewModel>(SqlString, EntityId, EntityType).ToList();

            return Ok(xx);
        }
        [ResponseType(typeof(LAudit))]
        public async Task<IHttpActionResult> GetByEntityName(string EntityName, string UserName,string WorkFlow)
        {
            var xx = (from aa in db.LAudits
                      where aa.EntityName == EntityName
                      select new
                      {
                          aa.Id,aa.ActionLabel,
                          aa.RelyProcessName,
                          aa.VFProcessName,
                          aa.ControlCode,
                          aa.ControlDescription,
                          aa.Action,
                          aa.ActionedById,
                          aa.ActionedByRoleId,
                          aa.ActionType,
                          aa.ActionDateTime,
                          aa.OldStatus,
                          aa.NewStatus,
                          aa.EntityType,
                          aa.EntityId,
                          aa.EntityName,
                          aa.WorkFlowId,
                          aa.CompanyCode,
                          aa.Comments
                      }).ToList();

            return Ok(xx);
        }


        [HttpPost]
        public IHttpActionResult PostLAudit(LAudit model,string UserName,string WorkFlow)
        {
                try { 
                    db.SpLogAudit(model.RelyProcessName,model.VFProcessName,model.ControlCode,model.ControlDescription,model.Action,
                        model.ActionType,model.ActionedById,model.ActionedByRoleId,model.ActionDateTime,model.OldStatus,model.NewStatus,
                        model.EntityType,model.EntityId,model.EntityName,model.WorkFlowId,model.CompanyCode,model.Comments,model.StepId,model.ActionLabel,model.SupportingDocumentId);
                    db.SaveChanges();
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
                return Ok();
            }

        public async Task<IHttpActionResult> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid)
        {

            var Query = "Exec [spGetNewItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@Intervalid";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);           
            cmd.Parameters.AddWithValue("@Intervalid", Intervalid);
            var dt = Globals.GetDataTableUsingADO(cmd);
            dt.Columns.Remove("datacount");           
            return Ok(dt);
        }

        public async Task<IHttpActionResult> GetCountsForNewItems()
        {    
            var Query = "Exec [spGetNewItemsCounts]";
            SqlCommand cmd = new SqlCommand(Query);
            var dt = Globals.GetDataTableUsingADO(cmd);
            var count = dt.Rows.Count;           
            return Ok(count);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid)
        {
            var Query = "Exec [spGetNewItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@Intervalid";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@Intervalid", Intervalid);
            var dt = Globals.GetDataTableUsingADO(cmd);
            dt.Columns.Remove("datacount");
          
            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
          
            return Ok(columnNames);
        }


        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Something else failed return original error message as retrieved from database
            //Add complete Url in description
            var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
            string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
            var ErrorDesc = "";
            var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            if (Desc.Count() > 0)
                ErrorDesc = string.Join(",", Desc);
            string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3rd index position
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
