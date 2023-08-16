using RELY_API.Models;
using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RELY_API.Controllers
{
    public class LReconColumnMappingController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public object LProducts { get; private set; }

        //GetLReconColumnMapping/ByCompanyCode
        [HttpGet]
        public async Task<IHttpActionResult> GetLReconColumnMappingBySysCat(string CompanyCode, int FileFormatId, int SysCatId, string UserName, string WorkFlow)
        {
            //var xx = (from aa in db.LReconColumnMappings
            //          join bb in db.LReconFileFormats on aa.FileFormatId equals bb.Id
            //          where (aa.CompanyCode == CompanyCode && bb.SysCatId == SysCatId && bb.Id == FileFormatId)
            //          select new
            //          {
            //              aa.Id,
            //              aa.CompanyCode,
            //              aa.FileFormatId,
            //              bb.FormatName,
            //              aa.ColumnName,
            //              aa.Label,
            //              aa.DisplayOnForm,
            //              aa.OrdinalPosition,
            //              aa.IsProductCodeColumn
            //          }).OrderBy(p => p.OrdinalPosition);

            //var xx = (from aa in db.LReconColumnMappings
            //          join bb in db.LReconFileFormats on aa.FileFormatId equals bb.Id
            //          where (aa.CompanyCode == CompanyCode && bb.SysCatId == SysCatId && bb.Id == FileFormatId)
            //          select new
            //          {
            //              aa.Id,
            //              aa.CompanyCode,
            //              aa.FileFormatId,
            //              bb.FormatName,
            //              aa.ColumnName,
            //              aa.Label,
            //              aa.DisplayOnForm,
            //              aa.OrdinalPosition,
            //              aa.IsProductCodeColumn
            //          }).OrderByDescending(p => p.DisplayOnForm).OrderBy(p => p.OrdinalPosition).OrderBy(p => p.Label);

            var xx = await db.Database.SqlQuery<LReconColumnMapping>("select distinct ISC.COLUMN_NAME as ColumnName,LRCM.Id as Id,LRCM.CompanyCode,LRCM.FileFormatId,LRCM.Label as Label,LRCM.DisplayOnForm as DisplayOnForm,LRCM.OrdinalPosition as OrdinalPosition,LRCM.IsProductCodeColumn from INFORMATION_SCHEMA.COLUMNS ISC left outer join LReconColumnMapping LRCM on (ISC.COLUMN_NAME=LRCM.ColumnName  and LRCM.FileFormatId={0}) where ISC.TABLE_NAME='LReconBucket' and ISC.COLUMN_NAME like 'A%' order by LRCM.DisplayOnForm desc,LRCM.OrdinalPosition,LRCM.Label", FileFormatId).ToListAsync();


            return Ok(xx);
           
        }

        //GetFileFormatBySysCat
        [HttpGet]
        public IHttpActionResult GetFileFormatBySysCat(int SysCatId,string CompanyCode)
        {
            //List<string> ReconFileFormatList = new List<string>();
            //ReconFileFormatList =await db.LReconFileFormats.Where(p=>p.CompanyCode==CompanyCode).Select(p=>p.SysCatId==SysCatId).ToList();
            var ReconFileFormatList = (from aa in db.LReconFileFormats.Where(p => p.SysCatId == SysCatId).Where(p => p.CompanyCode == CompanyCode)
                                       select new
                                       {
                                           aa.Id,
                                           aa.FormatName
                                       }).ToList().OrderBy(p => p.FormatName);
            return Ok(ReconFileFormatList);
        }

        //POST:LReconColumnMapping
        [HttpPost]
        //[HttpGet]
        public IHttpActionResult POSTLReconColumnMapping(PostLReconColumnMappingViewModel model)
        {
            //LReconColumnMapping reconColumnMapping = new LReconColumnMapping();
            //reconColumnMapping.CompanyCode="DE";
            //reconColumnMapping.FileFormatId = 3;
            //reconColumnMapping.ColumnName = "ABC";

            //PostLReconColumnMappingViewModel LReconColumnMappingModel = new PostLReconColumnMappingViewModel();
            //LReconColumnMappingModel.CompanyCode = "DE";
            //LReconColumnMappingModel.FileFormatId = 3;
            //LReconColumnMappingModel.ColumnName = "ABC";
            //LReconColumnMappingModel.Label = "ABC";
            //LReconColumnMappingModel.DisplayOnForm = true;
            //LReconColumnMappingModel.OrdinalPosition = 0;
            //LReconColumnMappingModel.IsProductCodeColumn = true;
            using (RELYDevDbEntities db2 = new RELYDevDbEntities())
            {
                //var Arr = model.GridData.Split(',');
                var Arr = model.GridData.Split(',');
                //var ExistingReconColumnMapping = db2.LReconColumnMappings.Where(p => p.FileFormatId == model.FileFormatId).ToList();
                var xx=(from aa in db.LReconColumnMappings
                 join bb in db.LReconFileFormats on aa.FileFormatId equals bb.Id
                 where (aa.CompanyCode == model.CompanyCode && bb.SysCatId == model.SysCatId && bb.Id == model.FileFormatId)
                 select new
                 {
                     aa.Id,
                     aa.CompanyCode,
                     aa.FileFormatId,
                     bb.FormatName,
                     aa.ColumnName,
                     aa.Label,
                     aa.DisplayOnForm,
                     aa.OrdinalPosition,
                     aa.IsProductCodeColumn
                 }).ToList();
                //db2.LReconColumnMappings.RemoveRange(xx);
                foreach(var ColumnList in xx)
                {
                   var ExistingColumnList = db2.LReconColumnMappings.Where(P => P.Id == ColumnList.Id).FirstOrDefault();
                   db2.LReconColumnMappings.Remove(ExistingColumnList);
                   db2.SaveChanges();
                }
               
                for (var i = 0; i < Arr.Length; i = i + 5)
                {
                    try
                    {
                        var CompanyLabel = new LReconColumnMapping();
                        CompanyLabel.ColumnName = Arr[i].ToString();
                        CompanyLabel.Label = (string.IsNullOrEmpty(Arr[i + 1])) ? null : Arr[i + 1].ToString();
                        CompanyLabel.DisplayOnForm = Convert.ToBoolean(Arr[i + 2]);
                        CompanyLabel.OrdinalPosition = (string.IsNullOrEmpty(Arr[i + 3]) ? 0 : Convert.ToInt32(Arr[i + 3]));
                        CompanyLabel.IsProductCodeColumn = Convert.ToBoolean(Arr[i + 4]);
                        CompanyLabel.CompanyCode = model.CompanyCode;
                        CompanyLabel.FileFormatId = model.FileFormatId;
                        //CompanyLabel.Id = model.Id;

                        db2.LReconColumnMappings.Add(CompanyLabel);
                        db2.SaveChanges();
                    }
                    catch (DbEntityValidationException dbex)
                    {
                        //transaction.Rollback();
                        var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
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
            }
            return Ok();
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


        [HttpGet]
        public IHttpActionResult GetLReconColumnsByFormatId(string CompanyCode, int FileFormatId)
        {
            var ReconFileColumnsList = (from aa in db.LReconColumnMappings.Where(p => p.FileFormatId == FileFormatId).Where(p => p.CompanyCode == CompanyCode).Where((p => p.Label!=null))
                                        select new
                                       {

                                           aa.Id,
                                           aa.CompanyCode,
                                           aa.FileFormatId,
                                           aa.ColumnName,
                                           aa.Label,
                                           aa.DisplayOnForm,
                                           aa.OrdinalPosition,
                                           aa.IsProductCodeColumn
                                       }).OrderBy(p => p.OrdinalPosition);
            return Ok(ReconFileColumnsList);
           

        }




       

        
    }
}
