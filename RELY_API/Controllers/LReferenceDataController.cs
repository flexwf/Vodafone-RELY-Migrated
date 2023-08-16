using CsvHelper;
using RELY_API.Models;
using RELY_API.Utilities;
//using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LReferenceDataController : ApiController
    {


        private RELYDevDbEntities db = new RELYDevDbEntities();
        public IHttpActionResult GetReferenceDataGridCounts(int ReferenceId)
        {
            var sqlQuery = "select count(*) from LReferenceData where  ReferenceId = {0} ";
            int counts = db.Database.SqlQuery<int>(sqlQuery, ReferenceId).FirstOrDefault();
            return Ok(counts);
        }

        public IHttpActionResult GenerateReferenceDataGrid(int ReferenceId, string sortdatafield, string sortorder, string FilterQuery, int PageNumber, int PageSize)
        {
            var Query = "Exec [SpGenerateReferenceDataGrid] @ReferenceId,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@ReferenceId", ReferenceId);
            cmd.Parameters.AddWithValue("@pagesize", PageSize);
            cmd.Parameters.AddWithValue("@pagenum", PageNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            var dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

      
        [HttpGet]
        public IHttpActionResult DownloadReferenceDataGrid(int ReferenceId, string LRefType, string Tablename, string CompanyCode, string OutputFilename)
        {
            string Filename = null;
            DataTable dt = new DataTable();
            var Query = "Exec [SpDownloadReferenceDataGrid] @Tablename,@LRefType, @ReferenceId";           
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Tablename", Tablename);
            cmd.Parameters.AddWithValue("@LRefType", LRefType);
            cmd.Parameters.AddWithValue("@ReferenceId", ReferenceId);
            dt = Globals.GetDataTableUsingADO(cmd);
            if (dt.Columns.Count>0)
            {
                string path = ConfigurationManager.AppSettings["RelyTempPath"] + "/";
                if (OutputFilename == null || OutputFilename == "undefined" || OutputFilename == "")
                {
                    Filename = "ReferenceData-" + LRefType + ".xlsx";
                }
                else
                {
                    Filename = OutputFilename + ".xlsx";
                }

                Globals.ExportToExcelForRefGridNew(dt, path, Filename);

                if (!string.IsNullOrEmpty(Filename))
                {
                    string fullpath = path + "\\" + Filename;//
                    string localpath = fullpath;
                    string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
                    string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + Filename;
                    Globals.UploadFileToS3(localpath, S3TargetPath);
                }
               
            }
            GenericNameAndIdViewModel model = new GenericNameAndIdViewModel { Name = Filename };
            return Ok(model);

        }

        // GET: api/LReferenceData/5
        [ResponseType(typeof(LReferenceData))]
        public IHttpActionResult GetByReferenceId(int ReferenceId,string UserName,string WorkFlow)
        {
            var LReferenceData = db.LReferenceDatas.Where(p => p.ReferenceId == ReferenceId).Select(aa => new
            {
                aa.Id,
                aa.ReferenceId,
                aa.EffectiveStartDate,
                aa.EffectiveEndDate,

                aa.AttributeC01,aa.AttributeC02,aa.AttributeC03,aa.AttributeC04,aa.AttributeC05,  aa.AttributeC06,aa.AttributeC07,aa.AttributeC08,
                aa.AttributeC09, aa.AttributeC10,aa.AttributeC11,aa.AttributeC12,aa.AttributeC13,aa.AttributeC14,aa.AttributeC15,aa.AttributeC16,
                aa.AttributeC17,aa.AttributeC18,aa.AttributeC19,aa.AttributeC20, 
                aa.AttributeM01,aa.AttributeM02,aa.AttributeM03,aa.AttributeM04,aa.AttributeM05,
                
                aa.AttributeN01,aa.AttributeN02,aa.AttributeN03,aa.AttributeN04,aa.AttributeN05,aa.AttributeN06,aa.AttributeN07,aa.AttributeN08,aa.AttributeN09,aa.AttributeN10,
                aa.AttributeI01,aa.AttributeI02,aa.AttributeI03,aa.AttributeI04,aa.AttributeI05,aa.AttributeI06,aa.AttributeI07,aa.AttributeI08,aa.AttributeI09,aa.AttributeI10,
                aa.AttributeD01,aa.AttributeD02,aa.AttributeD03,aa.AttributeD04,aa.AttributeD05,
                aa.AttributeB01,aa.AttributeB02,aa.AttributeB03,aa.AttributeB04,aa.AttributeB05,aa.AttributeB06,aa.AttributeB07,aa.AttributeB08,aa.AttributeB09,aa.AttributeB10,

                aa.CreatedDateTime,
                aa.UpdatedById,
                aa.UpdatedDateTime
            }).OrderBy(aa => aa.EffectiveStartDate);
            if (LReferenceData == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Data")));
            }
            return Ok(LReferenceData);
        }

        

        public IHttpActionResult GetLReferenceDataForGrid(string CompanyCode, string TableName, int ReferenceTypeId,int ReferenceId,int PageNumber,int PageSize)
        {
            //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section
            TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

            //Get a list of labels defined in LCompanySpecificColumns table and add each of them as a property in the newly created TypeBuilder
            var ColListForNewType = db.LCompanySpecificColumns.Where(p=>p.TableName==TableName).Where(p => p.ReferenceTypeId == ReferenceTypeId).Select(p => new { p.ColumnName, p.DataType, p.OrdinalPosition }).OrderBy(p => p.OrdinalPosition).ToList();
            //var ColumnList = db.Database.SqlQuery<string>(ColumnQuery).ToList();
            if (ColListForNewType.Count() == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No LReferenceData column mapping found. Please contact L2 Support to fix"));
            }
            //Add Obivious columns (these columns are not available in the ColListForNewType)
            CreateAutoImplementedProperty(builder, "Id", typeof(int));
            


            //Add dynamic columns by looping through ColListForNewType and getting column details from there
            foreach (var item in ColListForNewType)
            {
                switch (item.DataType)
                {
                    case "varchar":
                    case "nvarchar":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(string));
                        break;
                    case "int":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(int?));
                        break;
                    case "date":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(DateTime?));
                        break;
                    case "datetime":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(DateTime?));
                        break;
                    case "bit":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(bool?));
                        break;
                    case "bigint":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(Int64?));
                        break;
                    case "decimal":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(decimal?));
                        break;
                    case "float":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(double?));
                        break;
                    case "numeric":
                        CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(double?));
                        break;
                }
            }

            Type resultType = builder.CreateType();

            //"For XML path('')" in this query is used to convert sql query result set into a single string.
            string ColumnQuerySingleRow = "select stuff((select '[' + [ColumnName] + '] AS [' +  [ColumnName] +'], ' FROM LCompanySpecificColumns WHERE ReferenceTypeID ={0} AND TableName = {1} Order by OrdinalPosition FOR XML PATH('') ),1,0,'')";
            var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow, ReferenceTypeId, TableName).ToList();

            

            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
            string Qry = "Select * From (Select Id, " + ColumnListSingleRow.ElementAt(0).ToString() + " ReferenceId, ";

            Qry = Qry + "ROW_NUMBER() OVER (ORDER BY Id) as row FROM LReferenceData "; 
            Qry = Qry + " Where ReferenceId = {0}) a ";
            Qry = Qry + " Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;

            //Execute the query and return the result 
            dynamic xx = db.Database.SqlQuery(resultType, Qry,ReferenceId);
            return Ok(xx);
        }

        public IHttpActionResult GetLReferenceDataCounts(string CompanyCode,string TableName)
        {
            //Get counts from database
           
            string Qry = "Select Count(*) as RowCounts from LCompanySpecificColumns Where CompanyCode = {0} And TableName = {1}"; 
            //Execute the query and return the result 
            var xx = db.Database.SqlQuery<int>(Qry,CompanyCode,TableName).FirstOrDefault();

            return Ok(xx);
        }

        // PUT: api/LReferenceData/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLReferenceData(int id, LReferenceData LReferenceData, string UserName, string WorkFlow)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Reference Data")));
            }

            if (!LReferenceDataExists(id))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Data")));
            }

            if (id != LReferenceData.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Reference Data")));
            }
            try
            {
                db.Entry(LReferenceData).State = EntityState.Modified;
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
            return Ok(LReferenceData);
        }


        [HttpPost]
        //[HttpGet]
        // POST: api/LReferenceData
        [ResponseType(typeof(LReferenceData))]
        public async Task<IHttpActionResult> PostLReferenceData(List<LReferenceData> LReferenceData,int Id, string UserName, string WorkFlow)
        {
           
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE","Reference")));
            }

            try
            {
                foreach (var model in LReferenceData)
                {
                    if (model.Id == 0)//add only when new record is there to insert.
                                      //insert date into LProduct table
                        db.LReferenceDatas.Add(model);
                }
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
            // return CreatedAtRoute("DefaultApi", new { id = LProduct.Id }, LProduct);
            return Ok();
        }



        // DELETE: api/LReferenceData/5
        [ResponseType(typeof(LReferenceData))]
        public async Task<IHttpActionResult> DeleteLReferenceData(int id, string UserName, string WorkFlow)
        {
            LReferenceData LReferenceData = await db.LReferenceDatas.FindAsync(id);
            if (LReferenceData == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Reference Data")));
            }

            try
            {
                db.LReferenceDatas.Remove(LReferenceData);
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
            return Ok(LReferenceData);
        }
        private bool LReferenceDataExists(int id)
        {
            return db.LReferences.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("FK_LReferenceTypes_LCompanySpecificColumns_ReferenceTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "LOCAL POB", "LOCALPOB SSP"));
            //else if (SqEx.Message.IndexOf("FK_LReferenceTypes_LReferences_ReferencyTypeId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "LOCALPOB SSP"));
            //else if (SqEx.Message.IndexOf("UQ_LReferenceTypes_CompanyCode_Name", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "LOCALPOB SSP"));
            //else
            //{
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
            //}
        }

        private TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(string.Concat(GetterPrefix, propertyName), propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(string.Concat(SetterPrefix, propertyName), propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}
