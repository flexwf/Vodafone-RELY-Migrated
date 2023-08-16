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
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LCompanySpecificColumnsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        //SG commneted this code on 18/01/2018 - As it seems to be not in use
        /* public string GetColumnListAsSingleRow(string CompanyCode,string TableName,string SelecterType)
         {
             //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section
             TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

             if (SelecterType != null)
             {
                 //Get a list of labels defined in LCompanySpecificColumns table and add each of them as a property in the newly created TypeBuilder
                 var ColListForNewType = db.LCompanySpecificColumns.Where(p => p.TableName == TableName).Where(st => st.SelecterType == SelecterType).Where(d => d.DisplayOnForms == true).Select(p => new { p.ColumnName, p.Label, p.DataType, p.OrdinalPosition }).OrderBy(p => p.OrdinalPosition).ToList();


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

             }
             string ColumnQuerySingleRow = "select stuff((select '[' + [ColumnName] + '] AS [' +  [ColumnName] +'], ' FROM LCompanySpecificColumns WHERE TableName = '" + TableName + "' and SelecterType= '" + SelecterType  +"' and DisplayOnForms = 1 Order by OrdinalPosition FOR XML PATH('') ),1,0,'')";
             var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow).FirstOrDefault();
             return (ColumnListSingleRow);

             //Execute the query and return the result 
             //dynamic xx = db.Database.SqlQuery(resultType, Qry);

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
         }*/
        
        public IHttpActionResult GetAttributesForGrid(string CompanyCode, string TableName, string SelecterType, string UserName, string WorkFlow)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).Where(aa => aa.TableName == TableName).Where(aa => aa.SelecterType == SelecterType)
                      .Where(aa => aa.DisplayInGrid == true)
                      select new
                      {
                          aa.OrdinalPosition,
                          aa.ColumnName,
                          aa.Label,
                          aa.DataType
                      }).OrderBy(p => p.OrdinalPosition);
            return Ok(yy);
        }
        public IHttpActionResult GetLCompanySpecificColumnsByCompanyCode(string CompanyCode, string TableName, string SelecterType, string UserName, string WorkFlow)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).Where(aa => aa.TableName == TableName).Where(aa => aa.SelecterType == SelecterType).Where(aa => aa.DisplayOnForms == true)
                      select new
                      {
                          aa.Id,aa.CompanyCode,aa.TableName,aa.ColumnName,aa.Label,aa.DisplayOnForms,aa.OrdinalPosition,
                          aa.ExportHeader,aa.DefaultValue,aa.IsReportParameter,aa.DataType,
                          aa.MaximumLength,aa.DigitsAfterDecimal,aa.IsMultiline,aa.DropDownId,aa.IsMandatory,aa.SelecterType,aa.DisplayInGrid,aa.AuditEnabled
                      }).OrderBy(p => p.OrdinalPosition);
            return Ok(yy);
        }

        public IHttpActionResult GetColumnNameByLabel(string CompanyCode, string TableName, int SysCatId, string Label)
        {
            string SelecterType = db.RSysCats.Where(a => a.Id == SysCatId).Select(a => a.SysCatCode).FirstOrDefault();
            string qry = "select dbo.[FnGetColumnNameFromLcompanySpecificColumns]({0},{1},{2},{3})";
            string ColumnName = db.Database.SqlQuery<string>(qry, CompanyCode, TableName, Label, SelecterType).FirstOrDefault();
            return Ok(ColumnName);
        }

        public IHttpActionResult GetColumnDetails(string CompanyCode, string TableName, string SelecterType,string ColumnName, string UserName, string WorkFlow)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).Where(aa => aa.TableName == TableName).Where(aa => aa.SelecterType == SelecterType).Where(aa => aa.DisplayOnForms == true).Where(aa => aa.ColumnName == ColumnName)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.TableName,
                          aa.ColumnName,
                          aa.Label,
                          aa.DisplayOnForms,
                          aa.OrdinalPosition,
                          aa.ExportHeader,
                          aa.DefaultValue,
                          aa.IsReportParameter,
                          aa.DataType,
                          aa.MaximumLength,
                          aa.DigitsAfterDecimal,
                          aa.IsMultiline,
                          aa.DropDownId,
                          aa.IsMandatory,
                          aa.SelecterType,
                          aa.DisplayInGrid,
                          aa.AuditEnabled
                      }).FirstOrDefault();
            return Ok(yy);
        }
        public IHttpActionResult GetColumnsForProductHistory(string CompanyCode,string SelecterType, string UserName, string WorkFlow)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).Where(aa => aa.TableName == "LProducts" || aa.TableName == "LProductPobs")
                      .Where(aa => aa.SelecterType == SelecterType).Where(aa => aa.DisplayOnForms == true)
                      select new
                      {
                          aa.Id,
                          aa.CompanyCode,
                          aa.TableName,
                          aa.ColumnName,
                          aa.Label,
                          aa.DisplayOnForms,
                          aa.OrdinalPosition,
                          aa.ExportHeader,
                          aa.DefaultValue,
                          aa.IsReportParameter,
                          aa.DataType,
                          aa.MaximumLength,
                          aa.DigitsAfterDecimal,
                          aa.IsMultiline,
                          aa.DropDownId,
                          aa.IsMandatory,
                          aa.SelecterType,
                          aa.DisplayInGrid,
                          aa.AuditEnabled
                      }).OrderBy(p => p.OrdinalPosition);
            return Ok(yy);
        }
        public IHttpActionResult GetColumnsForProductReprt(string CompanyCode, string SelecterType1,string SelecterType2)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).
                      Where(aa => ((aa.TableName == "LProducts" || aa.TableName == "LProductPobs") && aa.SelecterType == SelecterType1 && aa.DisplayOnForms == true)
                      || (aa.TableName == "LLocalPobs" && aa.SelecterType == SelecterType2 && aa.DisplayOnForms == true))

                      select new
                      {
                          aa.DataType,
                          aa.ColumnName,
                          Label = aa.Label + "(" + aa.TableName.Substring(1) + ")"
                      }).OrderBy(p => p.Label).ToList();
            return Ok(yy);
            //var xx = await db.Database.SqlQuery<string>("Select  Label from LCompanySpecificColumns where TableName IN ('LProducts','LProductPobs') and SelecterType={0} and DisplayOnForms=1 or TableName='LLocalPobs' and SelecterType={1} and DisplayOnForms=1", , SelecterType1,SelecterType2).ToListAsync();
            //return Ok(xx);
        }
       /* public IHttpActionResult GetLocalPobsLCompanySpecificColumnsByCompanyCode(string CompanyCode, string UserName, string WorkFlow)
        {
            var yy = (from aa in db.LCompanySpecificColumns.Where(aa => aa.CompanyCode == CompanyCode).Where(aa => aa.TableName == "LLocalPobs").Where(aa => aa.DisplayOnForms == true)
                      select new
                      {
                          aa.SelecterType,
                          aa.Id,
                          aa.CompanyCode,
                          aa.TableName,
                          aa.ColumnName,
                          aa.Label,
                          aa.DisplayOnForms,
                          aa.OrdinalPosition,
                          aa.ExportHeader,
                          aa.DefaultValue,
                          aa.IsReportParameter,
                          aa.DataType,
                          aa.MaximumLength,
                          aa.DigitsAfterDecimal,
                          aa.IsMultiline,
                          aa.DropDownId,
                          aa.IsMandatory
                      }).OrderBy(p => p.OrdinalPosition);

            return Ok(yy);
        }*/

        [HttpGet]
        public async Task<IHttpActionResult> GetCompanySpecificColumnsByTableName(string TableName,string SelecterType)
        {
            //Select ColumnNames on the basis of SysCatCode instead of syscat name in the cases of Lproducts,LProductPob
            if (TableName.Equals("LProducts") || TableName.Equals("LProductPobs"))
            {
                var SelecterCode = db.Database.SqlQuery<string>("select SysCatCode from RSysCat where SysCat={0}", SelecterType).FirstOrDefault();
                SelecterType = SelecterCode;

            }
            

            if (db.LCompanySpecificColumns.Where(p => p.TableName == TableName && p.SelecterType == SelecterType.ToString()).Count() > 0)
            {
                
            
               var xx = await db.Database.SqlQuery<CompanySpecificColumnViewModel>("select distinct ISC.COLUMN_NAME as ColumnName,(select Name from LDropDowns where Id=LCSC.DropDownId) as LdName,LCSC.DropDownId,LCSC.Id as Id,LCSC.OrdinalPosition as OrdinalPosition,LCSC.Label as Label,LCSC.DisplayOnForms as DisplayOnForms,LCSC.AuditEnabled as AuditEnabled,LCSC.DisplayInGrid as DisplayInGrid,LCSC.IsMandatory,LCSC.DefaultValue as DefaultValue,LCSC.MaximumLength,DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC left outer join LCompanySpecificColumns LCSC on (ISC.COLUMN_NAME=LCSC.ColumnName  and Lcsc.TableName=ISC.TABLE_NAME and LCSC.SelecterType={0}) where ISC.TABLE_NAME={1} and ISC.COLUMN_NAME like 'Attribute%' order by LCSC.DisplayOnForms desc,LCSC.OrdinalPosition,LCSC.Label", SelecterType.ToString(), TableName).ToListAsync();
                    return Ok(xx);
                
            }
            else
            {
                var xx = await db.Database.SqlQuery<CompanySpecificColumnViewModel>("select 0 as Id,0 as OrdinalPosition,'' as Label,ISC.COLUMN_NAME as ColumnName,CONVERT(BIT,case IS_NULLABLE when 'YES' then 0 else 1 end)  as IsMandatory,LCSC.DefaultValue as DefaultValue,LCSC.AuditEnabled as AuditEnabled,LCSC.DisplayInGrid as DisplayInGrid,LCSC.MaximumLength,DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC where ISC.TABLE_NAME={0} and ISC.Column_Name like 'Attribute%' order by ISC.COLUMN_NAME ", TableName).ToListAsync();
                return Ok(xx);
            }

            //if (db.LCompanySpecificColumns.Where(p => p.TableName == TableName && p.SelecterType == SelecterType.ToString()).Count() > 0)
            //{
            //    var xx=await db.Database.SqlQuery<CompanySpecificColumnViewModel>("select distinct ISC.COLUMN_NAME as ColumnName,(select Name from LDropDowns where Id=LCSC.DropDownId) as LdName,LCSC.DropDownId,LCSC.Id as Id,LCSC.OrdinalPosition as OrdinalPosition,LCSC.Label as Label,LCSC.DisplayOnForms as DisplayOnForms,LCSC.IsMandatory,DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC left outer join LCompanySpecificColumns LCSC on (ISC.COLUMN_NAME=LCSC.ColumnName  and Lcsc.TableName=ISC.TABLE_NAME and LCSC.SelecterType={0}) where ISC.TABLE_NAME={1} and ISC.COLUMN_NAME like 'Attribute%' order by LCSC.DisplayOnForms desc,LCSC.OrdinalPosition,LCSC.Label", SelecterType.ToString(), TableName).ToListAsync();
            //    return Ok(xx);
            //}
            //else
            //{
            //    var xx =await db.Database.SqlQuery<CompanySpecificColumnViewModel>("select 0 as Id,0 as OrdinalPosition,'' as Label,ISC.COLUMN_NAME as ColumnName,CONVERT(BIT,case IS_NULLABLE when 'YES' then 0 else 1 end)  as IsMandatory,DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC where ISC.TABLE_NAME={0} and ISC.Column_Name like 'Attribute%' order by ISC.COLUMN_NAME ", TableName).ToListAsync();
            //    return Ok(xx);
            //}
        }

        [HttpPost]
        public IHttpActionResult PostLCompanySpecificColumns(PostLCompanySpecificFormViewModel model)
        {
            //PostLCompanySpecificFormViewModel model = new PostLCompanySpecificFormViewModel();

            //model.CompanyCode = "DE";
            //model.TableName = "LProducts";
            //model.SelecterType = "demo-hdw";
            //model.GridData = "AttributeB01,newAttrib1,true,false,,1,,,bit,,,false,true,AttributeB02,newAttrib1,true,false,,2,,,bit,,,false,false,AttributeC01,Base Price Id,true,false,,3,,,nvarchar,,,false,false,AttributeC02,Demo dropdown,true,false,41,4,,,nvarchar,,,false,false,AttributeC03,Business Category,true,false,2,5,,,nvarchar,,,false,false,AttributeN01,SSP Amount,true,false,,6,,,numeric,,,false,false,AttributeB03,,false,false,,0,,,bit,,,false,false,AttributeB04,,false,false,,0,,,bit,,,false,false,AttributeB05,,false,false,,0,,,bit,,,false,false,AttributeB06,,false,false,,0,,,bit,,,false,false,AttributeB07,,false,false,,0,,,bit,,,false,false,AttributeB08,,false,false,,0,,,bit,,,false,false,AttributeB09,,false,false,,0,,,bit,,,false,false,AttributeB10,,false,false,,0,,,bit,,,false,false,AttributeC04,,false,false,,0,,,nvarchar,,,false,false,AttributeC05,,false,false,,0,,,nvarchar,,,false,false,AttributeC06,,false,false,,0,,,nvarchar,,,false,false,AttributeC07,,false,false,,0,,,nvarchar,,,false,false,AttributeC08,,false,false,,0,,,nvarchar,,,false,false,AttributeC09,,false,false,,0,,,nvarchar,,,false,false,AttributeC10,,false,false,,0,,,nvarchar,,,false,false,AttributeC11,,false,false,,0,,,nvarchar,,,false,false,AttributeC12,,false,false,,0,,,nvarchar,,,false,false,AttributeC13,,false,false,,0,,,nvarchar,,,false,false,AttributeC14,,false,false,,0,,,nvarchar,,,false,false,AttributeC15,,false,false,,0,,,nvarchar,,,false,false,AttributeC16,,false,false,,0,,,nvarchar,,,false,false,AttributeC17,,false,false,,0,,,nvarchar,,,false,false,AttributeC18,,false,false,,0,,,nvarchar,,,false,false,AttributeC19,,false,false,,0,,,nvarchar,,,false,false,AttributeC20,,false,false,,0,,,nvarchar,,,false,false,AttributeD01,,false,false,,0,,,datetime,,,false,false,AttributeD02,,false,false,,0,,,datetime,,,false,false,AttributeD03,,false,false,,0,,,datetime,,,false,false,AttributeD04,,false,false,,0,,,datetime,,,false,false,AttributeD05,,false,false,,0,,,datetime,,,false,false,AttributeD06,,false,false,,0,,,datetime,,,false,false,AttributeD07,,false,false,,0,,,datetime,,,false,false,AttributeD08,,false,false,,0,,,datetime,,,false,false,AttributeD09,,false,false,,0,,,datetime,,,false,false,AttributeD10,,false,false,,0,,,datetime,,,false,false,AttributeI01,,false,false,,0,,,int,,,false,false,AttributeI02,,false,false,,0,,,int,,,false,false,AttributeI03,,false,false,,0,,,int,,,false,false,AttributeI04,,false,false,,0,,,int,,,false,false,AttributeI05,,false,false,,0,,,int,,,false,false,AttributeI06,,false,false,,0,,,int,,,false,false,AttributeI07,,false,false,,0,,,int,,,false,false,AttributeI08,,false,false,,0,,,int,,,false,false,AttributeI09,,false,false,,0,,,int,,,false,false,AttributeI10,,false,false,,0,,,int,,,false,false,AttributeM01,,false,false,,0,,,nvarchar,,,false,false,AttributeM02,,false,false,,0,,,nvarchar,,,false,false,AttributeM03,,false,false,,0,,,nvarchar,,,false,false,AttributeM04,,false,false,,0,,,nvarchar,,,false,false,AttributeM05,,false,false,,0,,,nvarchar,,,false,false,AttributeN02,,false,false,,0,,,numeric,,,false,false,AttributeN03,,false,false,,0,,,numeric,,,false,false,AttributeN04,,false,false,,0,,,numeric,,,false,false,AttributeN05,,false,false,,0,,,numeric,,,false,false,AttributeN06,,false,false,,0,,,numeric,,,false,false,AttributeN07,,false,false,,0,,,numeric,,,false,false,AttributeN08,,false,false,,0,,,numeric,,,false,false,AttributeN09,,false,false,,0,,,numeric,,,false,false,AttributeN10,,false,false,,0,,,numeric,,,false,false";

            using (RELYDevDbEntities db2 = new RELYDevDbEntities())
            {
                // In LProducts and LProductPobs, SysCatCode is to be used as Selecter Type , so overriding value of SelecterType with SysCatCode
                //From HTML page SysCat name and its Id are used, therefore calculating value of SysCatCode for the same.
                if (model.TableName.Equals("LProducts") || model.TableName.Equals("LProductPobs"))
                {
                  var  SysCatCode = db.Database.SqlQuery<string>("select SysCatCode from RSysCat where SysCat={0}", model.SelecterType).FirstOrDefault();
                    model.SelecterType = SysCatCode;
                }
                 

                using (var transaction = db2.Database.BeginTransaction())//SS Commented transaction as commit statement is throwing error
                {
                var Arr = model.GridData.Split(',');
                    //delete all existing records for Selected Table
                    var ExistingCompanySpecificColumn = db2.LCompanySpecificColumns.Where(p => p.TableName == model.TableName).Where(p => p.SelecterType == model.SelecterType.ToString()).ToList();
                    db2.LCompanySpecificColumns.RemoveRange(ExistingCompanySpecificColumn);
                    db2.SaveChanges();
                    for (var i = 0; i < Arr.Length; i = i + 13)
                    {
                        try
                        {
                            var CompanyLabel = new LCompanySpecificColumn();
                            CompanyLabel.ColumnName = Arr[i].ToString();
                            CompanyLabel.Label = (string.IsNullOrEmpty(Arr[i + 1])) ? null : Arr[i + 1].ToString();
                            CompanyLabel.DisplayOnForms = Convert.ToBoolean(Arr[i + 2]);
                            CompanyLabel.IsMandatory = Convert.ToBoolean(Arr[i + 3]);
                            //JS added a constraint that do not save DropDownID where Display On Form = 0
                            if (CompanyLabel.DisplayOnForms)
                            {
                                if (!string.IsNullOrEmpty(Arr[i + 4]))
                                    CompanyLabel.DropDownId = Convert.ToInt32(Arr[i + 4]);
                            }
                            CompanyLabel.OrdinalPosition = (string.IsNullOrEmpty(Arr[i + 5])?0:Convert.ToInt32(Arr[i + 5]));
                            if (!string.IsNullOrEmpty(Arr[i + 6]))
                                CompanyLabel.IsReportParameter = Convert.ToBoolean(Arr[i + 6]);
                            if (!string.IsNullOrEmpty(Arr[i + 7]))
                                CompanyLabel.ReportParameterOrdinal = Convert.ToInt32(Arr[i + 7]);
                        if (!string.IsNullOrEmpty(Arr[i + 8]))
                            CompanyLabel.DataType = Convert.ToString(Arr[i + 8]);//ConvertSQLDataTypeToCSharp(Convert.ToString(Arr[i + 8]));
                        if (!string.IsNullOrEmpty(Arr[i + 9]))
                            CompanyLabel.MaximumLength = Convert.ToInt32(Arr[i + 9]);
                        if (!string.IsNullOrEmpty(Arr[i + 10]))
                            CompanyLabel.DefaultValue = Convert.ToString(Arr[i + 10]);

                            CompanyLabel.CompanyCode = model.CompanyCode;
                            CompanyLabel.TableName = model.TableName;
                            CompanyLabel.DisplayInGrid = Convert.ToBoolean(Arr[i + 11]);
                            CompanyLabel.AuditEnabled = Convert.ToBoolean(Arr[i + 12]);

                            //Getting Syscat Code instead of Syscat name
                            CompanyLabel.SelecterType =(model.SelecterType).ToString();


                        db2.LCompanySpecificColumns.Add(CompanyLabel);
                            db2.SaveChanges();
                        }
                        catch (DbEntityValidationException dbex)
                        {
                            transaction.Rollback();
                            var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
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
                    transaction.Commit();
                }
                return Ok();
            }
        }
      
        /*Base Table -> Selector
LRequests -> Show current OpCo code, store OpCo code in selector)
LProducts -> SysCatId (from RSysCat: Show Syscat, store Syscat in selector)
LLocalPOB -> LocalPOBTypeID (from RLocalPOBTypes: Show Name, store Name in selector)
LProductPOB -> Same as Products
LReferenceData -> RefTypeID (from LReferenceType: Show Name, store Name in selector)*/
[HttpGet]
        public async Task<IHttpActionResult> GetSelecterTypeByTableName(string TableName,string CompanyCode)
        {
            List<string> SelecterList = new List<string>();
            switch(TableName)
            {
                case "LRequests":
                    SelecterList = await db.RRequestSystems.Where(p => p.CompanyCode == CompanyCode).OrderBy(aa => aa.Name).Select(p => p.Name).ToListAsync();
                    break;
                case "LAccountingScenarios":                
                    SelecterList.Add(CompanyCode);
                    break;
                case "LProductPobs":
                case "LProducts":
                    SelecterList =await db.RSysCats.Where(p => p.CompanyCode == CompanyCode).OrderBy(aa => aa.SysCat).Select(p => p.SysCat).ToListAsync();
                    break;
                case "LLocalPobs":
                    SelecterList =await db.RLocalPobTypes.Where(p=>p.CompanyCode==CompanyCode).OrderBy(aa => aa.Name).Select(p => p.Name).ToListAsync();
                    break;
                case "LReferenceData":
                    SelecterList =await db.LReferenceTypes.Where(p => p.CompanyCode == CompanyCode).OrderBy(aa => aa.Name).Select(p => p.Name).ToListAsync();
                    break;
            }
            return Ok(SelecterList);
        }

        public async Task<IHttpActionResult> GetTableNamesByCompanyCode(string CompanyCode)
        {
            string strQuery = "Exec SpGetTableNameForColumnMapping @CompanyCode";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

        private string ConvertSQLDataTypeToCSharp(string SqlDataType)
        {
            switch (SqlDataType)
            {
                case "varchar":
                case "nvarchar":
                    return "string";
                case "int":
                case "bigint":
                    return "int";
                case "date":
                case "datetime":
                    return "DateTime";
                case "bit":
                    return "bool";
                case "decimal":
                    return "decimal";
                case "float":
                case "numeric":
                    return "double";
            }
            return null;
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

        public IHttpActionResult GetLRequestLCompanySpecificColumnsByCompanyCodeForGrid(string CompanyCode)
        {
            //RK 14122018 this method will return all the columns from lpayees when configuration is not saved for any company
            //Any exclusion will be mentioned in the not in clause of query
            //RK 14122018: added LpBlockNotifiation to the exclusion list
            var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select (select LdName from LDropDowns where Id=Lcsc.LcscDropDownId) as LdName,Lcsc.LcscDataType, LCSC.LcscIsMandatory,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, Replace(ISC.COLUMN_NAME ,'Lp','') as ColumnName,ISC.IS_NULLABLE as IsNullable,LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscTooltip,LCSC.LcscLabel from INFORMATION_SCHEMA.COLUMNS ISC inner join LCompanySpecificColumns LCSC on Replace(ISC.COLUMN_NAME ,'Lp','')=LCSC.LcscColumnName where LCSC.LcscTableName='LPayees' and LCSC.LcscCompanyId={0} and  ISC.TABLE_NAME='LPayees'and ISC.Column_Name not in ('Id','LpUserId','LpUserId','LpCompanyId','LpStatusId','LpCreatedById','LpUpdatedById','LpBatchId','LpComments','LpFileNames','LpUserFriendlyFileNames','LpCreatedDateTime','LpUpdatedDateTime','WFRequesterId','WFAnalystId','WFManagerId','WFOrdinal','WFCurrentOwnerId','WFStatus','WFType','WFRequesterRoleId','WFCompanyId','WFComments','LpBusinessUnit','LpSubChannelId','LpChannelId','LpPrimaryChannel','LpCreatedByForm','ParameterCarrier','LpBlockNotification') order by LCSC.LcscDisplayOnForm desc, LcscOrdinalPosition", CompanyCode).ToList();
            return Ok(yy);
        }

    }
}
