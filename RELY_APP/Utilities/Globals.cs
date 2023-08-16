using RELY_APP.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RELY_APP.Helper;
using RestSharp.Helper;
using RestSharp;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.IO.Packaging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Net.Mail;
using NPOI.SS.Util;
using ExportWord = NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Text;
//using NPOI.XWPF.UserModel;

namespace RELY_APP.Utilities
{
    public struct SessionData
    {
        public SessionData(string userID, string UserSessionID)
        {
            _UserID = userID;
            _UserSessionID = UserSessionID;
        }

        public string _UserID { get; private set; }
        public string _UserSessionID { get; private set; }

    }

    public static class Globals
    {
        public static List<SessionData> LstSessionIDs = new List<SessionData>();

        public static string ErrorPageUrl = "/Home/Error";

        //This methods checks if the Url contains RoleId, Set it in session. If StepId is also there, Workflow details are also updated in session.
        public static void GetRoleIdStepIdFromUrlAndUpdateSession(string LastActiveUrl,string CompanyCode)
        {
            //Assuming that LastActiveUrl is in the specified format. "RoleId=Value&StepId=Value" will be at the end of the url
            //Check whether RoleId exists in URL. If so, then Set RoleId and RoleName in session.
            //Also, set WorkFlowid and WorkFlow name is set in session.
            if (LastActiveUrl.Contains("RoleId"))
            {
                //calculating StepId and RoleId from LastActiveUrl
                var urlParts = LastActiveUrl.Split('/');
                string ParameterStr = urlParts[5].Split('?')[1];
                string[] paramList = ParameterStr.Split('&');
                int length = paramList.Length;
                var RoleId = paramList[length - 2].Split('=')[1];
                ILRolesRestClient RRC = new LRolesRestClient();
                var Role = RRC.GetById(Convert.ToInt32(RoleId), CompanyCode);
                if (Role != null)
                {
                    System.Web.HttpContext.Current.Session["CurrentRoleName"] = Role.RoleName;
                    System.Web.HttpContext.Current.Session["CurrentRoleId"] = Role.Id;

                }
                if (LastActiveUrl.Contains("StepId"))
                {
                    var StepId = paramList[length - 1].Split('=')[1];
                    IWStepRestClient SRC = new WStepRestClient();
                    var StepDetails = SRC.GetStepDetailsForWorkFlow(Convert.ToInt32(StepId));
                    string WorkFlowName = StepDetails.WorkFlowName;
                    System.Web.HttpContext.Current.Session["WorkFlow"] = WorkFlowName;
                    System.Web.HttpContext.Current.Session["WorkFlowId"] = StepDetails.WorkFlowId;
                }
            }
        }

        public static string A2SS3AccessKey = GetValue("rely_accesskey");
        public static string A2SS3SecretKey = GetValue("rely_secretkey");


        public static string GetValue(string Key)
        {
            IGKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            var Policy = KVRC.GetByName(Key);
            if (Policy != null)
                return WebUtility.HtmlDecode(Policy.Value);
            else
                return "";
        }


        //This method is used to check whether user has access to the Action at particular step
        public static bool CheckActionAuthorization(string ActionName, int LoggedInRoleId, int LoggedInUserId, int WorkFlowId, int StepId)
        {
            IWStepParticipantActionsRestClient SPRC = new WStepParticipantActionsRestClient();
            int Count = SPRC.GetCount(ActionName, LoggedInRoleId, LoggedInUserId, WorkFlowId, StepId);
            //count 0 means there is no mapping for Action and User, user is not authorized to perform this action.
            if (Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //RK Added following method to get session data from one place so that exceptions can be handled accordingly
        public static string GetSessionData(string strTypeOfSessionData)
        {
            string strReturnVal = "";
            try
            {
                //Due to issue raised on 26 March 2018, we have used this if condition as non string values are not returned as null
                if (strTypeOfSessionData == "CurrentRoleId")
                    strReturnVal = Convert.ToString(System.Web.HttpContext.Current.Session[strTypeOfSessionData]);
                else
                    strReturnVal = System.Web.HttpContext.Current.Session[strTypeOfSessionData] as string;
            }
            catch (Exception ex)
            {
                CreateDebugEntryRestClient CDERC = new CreateDebugEntryRestClient();
                CDERC.CreateDebugEntry("Getting session data: " + ex.ToString());
            }

            //Note For Rajesh (Added by VG) - This method is returning NULL values and that again os creating same issues. This method should never return NULLs instead
            //If session variable evaluates to NULL we need to gracefully redirect the user to session timeout page.

            return strReturnVal;
        }
        //new HTTP Status types has been defined.
        public enum ExceptionType
        {
            /*An exception/error condition in the application which is unhandled in the system and the system don’t have any predefined mechanism to recover*/
            Type1 = 551,
            /*- A server-side validation failure, such as unique key constraint failure while trying inserting data in a unique column*/
            Type2 = 552,
            /*In some cases, we will need to display a popup message to the user (maybe with some relevant information about the process in which error occurred) and then redirect user to another page*/
            Type3 = 553,
            /*When some custom validations(such as "user is not registered in the system") fails, user can be presented with a popup message describing the failed validation, and then keep user on the same page*/
            Type4 = 554
        };

        //Methods introduced by Shubham for server side filtering
        public static string BuildQuery(System.Collections.Specialized.NameValueCollection FilterQuery)
        {
            var filtercount = FilterQuery.GetValues("filterscount");
            //that means no filter, 
            if(filtercount == null)
            {
                return null;
            }
            var filtersCount = int.Parse(FilterQuery.GetValues("filterscount")[0]);
            var queryString = "";
            var tmpDataField = "";
            var tmpFilterOperator = "";
            var where = "";
            if (filtersCount > 0)
            {
                where = " AND (";
            }
            for (var i = 0; i < filtersCount; i += 1)
            {
                var filterValue = FilterQuery.GetValues("filtervalue" + i)[0];
                var filterCondition = FilterQuery.GetValues("filtercondition" + i)[0];
                var filterDataField = FilterQuery.GetValues("filterdatafield" + i)[0];
                var filterOperator = FilterQuery.GetValues("filteroperator" + i)[0];
                if (tmpDataField == "")
                {
                    tmpDataField = filterDataField;
                }
                else if (tmpDataField != filterDataField)
                {
                    where += ") AND (";
                }
                else if (tmpDataField == filterDataField)
                {
                    if (tmpFilterOperator == "" || tmpFilterOperator == "0")
                    {
                        where += " AND ";
                    }
                    else
                    {
                        where += " OR ";
                    }
                }
                // build the "WHERE" clause depending on the filter's condition, value and datafield.
                where += GetFilterCondition(filterCondition, filterDataField, filterValue);
                if (i == filtersCount - 1)
                {
                    where += ")";
                }
                tmpFilterOperator = filterOperator;
                tmpDataField = filterDataField;
            }
            queryString += where;
            return queryString;
        }


        //convert the filter condtion in a sql statement
        public static string GetFilterCondition(string filterCondition, string filterDataField, string filterValue)
        {
            //List of available filters present in JqxGrid are present in case statements to form Sql Query
            switch (filterCondition)
            {
                case "NOT_EMPTY":
                case "NOT_NULL":
                    return " " + filterDataField + " NOT LIKE '" + "" + "'";
                case "EMPTY":
                case "NULL":
                    return " " + filterDataField + " LIKE '" + "" + "'";
                case "CONTAINS_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS";
                case "CONTAINS":
                    return " " + filterDataField + " LIKE '%" + filterValue + "%'";
                case "DOES_NOT_CONTAIN_CASE_SENSITIVE":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "DOES_NOT_CONTAIN":
                    return " " + filterDataField + " NOT LIKE '%" + filterValue + "%'";
                case "EQUAL_CASE_SENSITIVE":
                    return " " + filterDataField + " = '" + filterValue + "'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "EQUAL":
                    return " " + filterDataField + " = '" + filterValue + "'";
                case "NOT_EQUAL_CASE_SENSITIVE":
                    return " BINARY " + filterDataField + " <> '" + filterValue + "'";
                case "NOT_EQUAL":
                    return " " + filterDataField + " <> '" + filterValue + "'";
                case "GREATER_THAN":
                    return " " + filterDataField + " > '" + filterValue + "'";
                case "LESS_THAN":
                    return " " + filterDataField + " < '" + filterValue + "'";
                case "GREATER_THAN_OR_EQUAL":
                    return " " + filterDataField + " >= '" + filterValue + "'";
                case "LESS_THAN_OR_EQUAL":
                    return " " + filterDataField + " <= '" + filterValue + "'";
                case "STARTS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "STARTS_WITH":
                    return " " + filterDataField + " LIKE '" + filterValue + "%'";
                case "ENDS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "ENDS_WITH":
                    return " " + filterDataField + " LIKE '%" + filterValue + "'";
            }
            return "";
        }

        //The below method will contain the code section in which we will be generating exception from the response recived from api project
        public static void GenerateExceptionFromResponse(IRestResponse response, string RedirectToUrl)
        {
            if (string.IsNullOrEmpty(RedirectToUrl))
            {
                RedirectToUrl = "/Home/ErrorPage";
            }
            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            ex.Data.Add("ErrorCode", (int)response.StatusCode);
            ex.Data.Add("RedirectToUrl", RedirectToUrl);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            string xx = data.Message;
            ex.Data.Add("ErrorMessage", xx);
            throw ex;
        }
        //To set variables in session  variable
        public static void SetSessionVariable(string CompanyCode, List<LRoleViewModel> Roles, string UserId, string UserName, string FirstName, string LastName, string PhoneNumber)
        {
            for (int i = 0; i < LstSessionIDs.Count; i++)
            {
                if (LstSessionIDs[i]._UserID == UserId)
                {
                    LstSessionIDs.RemoveAt(i);
                }
            }
            LstSessionIDs.Add(new SessionData(UserId, System.Web.HttpContext.Current.Session.SessionID));
            System.Web.HttpContext.Current.Session["CompanyCode"] = (CompanyCode != null) ? CompanyCode : "";
            //When RoleName is already stored in session, no need to reset. This happens in case of URLRedirection.
            //Otherwise, Set RoleName and Id in session.
            if (System.Web.HttpContext.Current.Session["CurrentRoleName"] as string == null)
            {
                System.Web.HttpContext.Current.Session["CurrentRoleName"] = (Roles.Count != 0) ? Roles.ElementAt(0).RoleName : "";//Role Name currently selected for the user(UserRole)
                System.Web.HttpContext.Current.Session["CurrentRoleId"] = (Roles.Count > 0) ? Convert.ToString(Roles.ElementAt(0).Id) : "0"; //RoleId for the current Role Name(UserRoleId)
            }
            System.Web.HttpContext.Current.Session["Roles"] = (Roles != null) ? Roles : null;//list of roles assigned to the user
            System.Web.HttpContext.Current.Session["UserId"] = (UserId != null) ? UserId : "";
            System.Web.HttpContext.Current.Session["LoginEmail"] = (UserName != null) ? UserName : "";//Login Email Id
                                                                                                      //LoginEmail session variables will be used only, this is set to overcome the exception occuring due to the use of UserName session variable, 
                                                                                                      //ultimately UserName needs to be replaced with LoginEmail
            System.Web.HttpContext.Current.Session["UserName"] = (UserName != null) ? UserName : "";
            //commenting untill We get the database connectivity
            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            var Company = GCRC.GetByComapnyCode(CompanyCode);
            System.Web.HttpContext.Current.Session["CompanyName"] = (Company != null) ? Company.CompanyName : "";
            System.Web.HttpContext.Current.Session["CompanyCode"] = (Company != null) ? Company.CompanyCode : "";

            //we are copy the LogoImage from S3BucketPath to Content folder of Project because system pick the images only from there
            if (Company != null)
            {
                string LogoImageVirtualPath = SaveLogoImageInSolution(Company.LogoPath);
                System.Web.HttpContext.Current.Session["LogoPath"] = LogoImageVirtualPath;
            }
            else
            {
                System.Web.HttpContext.Current.Session["LogoPath"] = "";
            }
            
           
            //System.Web.HttpContext.Current.Session["LogoPath"] = (Company != null) ? Company.LogoPath : "";
            System.Web.HttpContext.Current.Session["PunchLine"] = (Company != null) ? Company.PunchLine : "";
            //Getting Value of Version Key
            GKeyValueViewModel KeyValue = GetKeyvalue("Version");
            //This session will store the value of a perticular Key from GKeyValues table. In the following case Key=Version
            System.Web.HttpContext.Current.Session["KeyValue"] = KeyValue.Value;


        }


        public static void ClearSessionVariable(string AttributesList)
        {
            HttpContext.Current.Session["UploadedFilesList"] = "";
            HttpContext.Current.Session["CurrentMenuId"] = "";
            if (!String.IsNullOrEmpty(AttributesList))
            {
               string[] AttributesToBeCleared = AttributesList.Split(',');
            }
        }

        internal static void GenerateException(IRestResponse<GCompanyViewModel> response, object redirectToUrl)
        {
            throw new NotImplementedException();
        }

        public static void LogUserEvent(string ActionForId, string strActivity, string strRemarks, Boolean blnIsSuccess, string ActionById, string CompanyCode, string ClientIPAddress)
        {
            //Get Host name and IP Address
            string hostName = Dns.GetHostName();
            var HostEntry = Dns.GetHostEntry(hostName);
            string strHostIP = ClientIPAddress;
            //Below method is obsolate now, used GetHostEntry instead of GetHostByName as its obsolate now.
            // string strHostIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            //Getting Timezone info
            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            string strHostTimeZone = curTimeZone.StandardName;
            //Browser details
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string strBrowserDetails =
            "Name = " + browser.Browser + ", " + "Type = " + browser.Type + ", " + "Version = " + browser.Version + ", " + "Major Version = " + browser.MajorVersion + ", "
            + "Minor Version = " + browser.MinorVersion + ", " + "Platform = " + browser.Platform + ", " + "Is Beta = " + browser.Beta + ", " + "Is Crawler = " + browser.Crawler + ", "
            + "Is AOL = " + browser.AOL + ", " + "Is Win16 = " + browser.Win16 + ", " + "Is Win32 = " + browser.Win32 + ", " + "Supports Frames = " + browser.Frames + ", " + "Supports Tables = " + browser.Tables + ", "
            + "Supports Cookies = " + browser.Cookies + ", " + "Supports VBScript = " + browser.VBScript + ", " + "Supports JavaScript = " + ", " + browser.EcmaScriptVersion.ToString() + ", "
            + "Supports Java Applets = " + browser.JavaApplets + ", " + "Supports ActiveX Controls = " + browser.ActiveXControls + ", " + "Supports JavaScript Version = " + browser["JavaScriptVersion"];

            LUserActivityLogViewModel UserLog = new LUserActivityLogViewModel();
            UserLog.ActionById = Convert.ToInt32(ActionById);
            UserLog.Activity = strActivity;
            UserLog.CompanyCode = CompanyCode;
            UserLog.HostBrowserDetails = strBrowserDetails;
            UserLog.HostIP = strHostIP;
            UserLog.HostTimeZone = strHostTimeZone;
            UserLog.IsActivitySucceeded = blnIsSuccess;
            UserLog.Remarks = strRemarks;
            UserLog.ActionForId = Convert.ToInt32(ActionForId);
            UserLog.ActivityDateTime = DateTime.UtcNow;
            //Commented as we dont have restclient as of  now
            ILUserActivityLogRestClient RestClient = new LUserActivityLogRestClient();
            RestClient.Add(UserLog, null);
        }

        public static LUserActivityLogViewModel GetUserEvents()
        {
            //Get Host name and IP Address
            string hostName = Dns.GetHostName();
            var HostEntry = Dns.GetHostEntry(hostName); //used instead of GetHostByName as its obsolate now.
            string strHostIP = HostEntry.AddressList.ToString();
            //Below method is obsolate now, used GetHostEntry instead of GetHostByName as its obsolate now.
            // string strHostIP = Dns.GetHostByName(hostName).AddressList[0].ToString();//Getting Timezone info
            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            string strHostTimeZone = curTimeZone.StandardName;
            //Browser details
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string strBrowserDetails =
            "Name = " + browser.Browser + ", " + "Type = " + browser.Type + ", " + "Version = " + browser.Version + ", " + "Major Version = " + browser.MajorVersion + ", "
            + "Minor Version = " + browser.MinorVersion + ", " + "Platform = " + browser.Platform + ", " + "Is Beta = " + browser.Beta + ", " + "Is Crawler = " + browser.Crawler + ", "
            + "Is AOL = " + browser.AOL + ", " + "Is Win16 = " + browser.Win16 + ", " + "Is Win32 = " + browser.Win32 + ", " + "Supports Frames = " + browser.Frames + ", " + "Supports Tables = " + browser.Tables + ", "
            + "Supports Cookies = " + browser.Cookies + ", " + "Supports VBScript = " + browser.VBScript + ", " + "Supports JavaScript = " + ", " + browser.EcmaScriptVersion.ToString() + ", "
            + "Supports Java Applets = " + browser.JavaApplets + ", " + "Supports ActiveX Controls = " + browser.ActiveXControls + ", " + "Supports JavaScript Version = " + browser["JavaScriptVersion"];

            LUserActivityLogViewModel UserLog = new LUserActivityLogViewModel();
            UserLog.HostBrowserDetails = strBrowserDetails;
            UserLog.HostIP = strHostIP;
            UserLog.HostTimeZone = strHostTimeZone;
            UserLog.ActivityDateTime = DateTime.UtcNow;
            return UserLog;
        }

        //S3 Section Starts
        //Move local file across location in S3
        public static bool MoveFileinS3(string SourcePath, string DestinationPath)
        {
            string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            AmazonS3Config cfg = new AmazonS3Config();
            try
            {
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    CopyObjectRequest request = new CopyObjectRequest
                    {
                        SourceBucket = _bucketName,
                        SourceKey = ConfigurationManager.AppSettings["S3BucketRootFolder"] + SourcePath,
                        DestinationBucket = _bucketName,
                        DestinationKey = ConfigurationManager.AppSettings["S3BucketRootFolder"] + DestinationPath
                    };
                    CopyObjectResponse response = client.CopyObject(request);
                }
                return true;
            }
            catch (Exception ex)
            {
                //log error entry to DB.
                var model = new GErrorLogViewModel { UserName = System.Web.HttpContext.Current.Session["LoginEmail"] as string, Controller = "",
                    Method = "MoveFileinS3", ErrorDateTime = DateTime.UtcNow, StackTrace = ex.StackTrace.ToString(), ErrorDescription = "",
                    SourceProject = "[RELY WebApp]", Status = "New" };
                IErrorLogsRestClient ERC = new ErrorLogsRestClient();
                ERC.Add(model,null);
                return false;
            }
        }

        //upload file to S3
        public static bool UploadFileToS3(string LocalFilePath, string S3TargetPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            //using (StreamReader sr = new StreamReader(LocalFilePath))
            //{

            using (IAmazonS3 client = new AmazonS3Client(A2SS3AccessKey, A2SS3SecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], S3TargetPath),
                    // InputStream = sr.BaseStream//SEND THE FILE STREAM
                    FilePath = LocalFilePath,
                    // ContentType = "text/plain"
                };

                PutObjectResponse response2 = client.PutObject(request);
            }

            // }
            return true;

        }
        //S3 Upload uaing stream of file
        public static bool UploadStreamToS3(Stream stream, string S3TargetPath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
            using (IAmazonS3 client = new AmazonS3Client(A2SS3AccessKey, A2SS3SecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], S3TargetPath),
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
            }
            return true;

        }

        /*
       this method has been added to convert filestream to byte array because the Amazon file stream connection get closed as we move
      out of DownloadFromS3 method
           */
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //section to download file from S3 drive drectly 
        public static byte[] DownloadFromS3(string FilePath)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
           // string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];
          //  string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
            byte[] FileData;
            using (IAmazonS3 client = new AmazonS3Client(A2SS3AccessKey, A2SS3SecretKey))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = string.Format("{0}{1}", ConfigurationManager.AppSettings["S3BucketRootFolder"], FilePath),
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    FileData = ReadFully(response.ResponseStream);
                }
            }
            return FileData;
        }


        //S3 Section ends


        //public static bool UploadToS3(Stream stream, string FileName)
        //{
        //    string _awsAccessKey = ConfigurationManager.AppSettings["AWSS3AccessKey"];
        //    string _awsSecretKey = ConfigurationManager.AppSettings["AWSS3SecretKey"];
        //    string _bucketName = ConfigurationManager.AppSettings["S3Bucketname"];

        //    using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
        //    {
        //        var request = new PutObjectRequest()
        //        {
        //            BucketName = _bucketName,
        //            CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
        //            Key = FileName,  //string.Format("{0}/{1}", ConfigurationManager.AppSettings["S15ReportsBucketPath"],FileName),
        //            InputStream = stream//SEND THE FILE STREAM
        //        };

        //        client.PutObject(request);
        //    }
        //    return true;
        //}


        //public static byte[] DownloadFromS3(string FileName)
        //{
        //    string _awsAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
        //    string _awsSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        //    string _bucketName = ConfigurationManager.AppSettings["Bucketname"];
        //    byte[] FileData;
        //    using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
        //    {
        //        GetObjectRequest request = new GetObjectRequest
        //        {
        //            BucketName = _bucketName,
        //            Key = FileName //string.Format("{0}/{1}", ConfigurationManager.AppSettings["S15ReportsBucketPath"], FileName),
        //        };

        //        using (GetObjectResponse response = client.GetObject(request))
        //        {

        //            FileData = ReadFully(response.ResponseStream);

        //        }
        //    }
        //    return FileData;
        //}

        // //This method has been added to convert filestream to byte array because the Amazon file stream connection get closed 
        // //as we move out of DownloadFromS3 method 
        //public static byte[] ReadFully(Stream input)
        //{
        //    byte[] buffer = new byte[16 * 1024];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        public static DataTable ImportDataFromExcel(string ExcelFileName)
        {
            string str = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + ExcelFileName + @"; Extended properties=""Excel 8.0;HDR=Yes;IMEX=1;ImportMixedTypes=Text;TypeGuessRows=0""";
            OleDbConnection con = new OleDbConnection(str);
            OleDbCommand cmd = new OleDbCommand("Select * from [Sheet1$]", con);
            DataSet ds = new DataSet();
            OleDbDataAdapter adp = new OleDbDataAdapter(cmd);
            adp.Fill(ds);
            return ds.Tables[0];
        }


        public static string ExportToExcel(DataTable dt, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            // ICellStyle _TextCellStyle = workbook.CreateCellStyle();
            // _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("text");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();

                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();
            }
           
            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    //getting datatype for dataclumn. If it is datatime, convert its value to contain only date in MM/dd/yyyy format.
                    var type = dt.Columns[j].DataType.Name.ToString();
                    if (type == "DateTime")
                    {
                        if (!String.IsNullOrEmpty(value))
                            value = DateTime.Parse(value).ToString("dd/MM/yyyy");
                    }

                    cell.SetCellValue(value);
                    //GC.Collect();
                }
            }
            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);


            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");
        }

        /// <summary>
        /// Generic Method to get the dynamic columns on Excel Sheet
        /// </summary>
        /// <returns></returns>
        public static string DynamicColumnsExportToExcel(DataTable dt, string TempPath, string Filename,string Tablename, string SelectorType)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            
            ILCompanySpecificColumnsRestClient LCSRDC = new LCompanySpecificColumnsRestClient();
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();          
            var CompanySpecificColumnData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, Tablename, SelectorType).ToList();

            for (int j = 0; j < CompanySpecificColumnData.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = CompanySpecificColumnData[j].Label;
                cell.SetCellValue(columnName);              
               // GC.Collect();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                 //   GC.Collect();
                }
            }
            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);


            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");
        }

        public static string ExportTemplateToExcelForLFSSectionItems(DataTable dt, DataTable dtSurveys,DataTable dtItemTypeName, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            string extension = System.IO.Path.GetExtension(Filename);
            string sheetName = Filename.Substring(0, Filename.Length - extension.Length);
            XSSFSheet sheet1 = (XSSFSheet)workbook.CreateSheet(sheetName);

            
            //ISheet sheet2 = workbook.CreateSheet("Surveys");


            IRow row1 = sheet1.CreateRow(0);


            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
               // GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    //GC.Collect();
                }
            }

            //Survey List for SurveyListDropdown
            IList<string> surveyList = dtSurveys.AsEnumerable()
                           .Select(r => r.Field<string>("SurveyName"))
                           .ToList();
            //While generating Template we dont know the no of rows to be included in excel, so 65535 is used as value of lastrow parameter
            CreateDropDownListForExcel(sheet1, surveyList, 1, 65535, 0);

            //ItemTypeName List for ItemTypeNameDropdown
            IList<string> itemTypeNameList = dtItemTypeName.AsEnumerable()
                           .Select(r => r.Field<string>("Name"))
                           .ToList();
            //While generating Template we dont know the no of rows to be included in excel, so 65535 is used as value of lastrow parameter
            CreateDDLItemTypeNameForExcel(sheet1, itemTypeNameList, 1, 65535, 5);



            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);


            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");

       
        }

        //Export Template for Question Bank with ItemTypeName Dropdown
        public static string ExportTemplateToExcelForLFSQuestionBank(DataTable dt, DataTable dtItemTypeName, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            string extension = System.IO.Path.GetExtension(Filename);
            string sheetName = Filename.Substring(0, Filename.Length - extension.Length);
            XSSFSheet sheet1 = (XSSFSheet)workbook.CreateSheet(sheetName);

            //ISheet sheet2 = workbook.CreateSheet("Surveys");


            IRow row1 = sheet1.CreateRow(0);


            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                   // GC.Collect();
                }
            }

            IList<string> target = dtItemTypeName.AsEnumerable()
                           .Select(r => r.Field<string>("Name"))
                           .ToList();

            //Create ItemTypeName dropdown for Question Bank
            CreateDDLItemTypeNameForExcelForQuestionBank(sheet1, target, 1, 65535, 1);//While generating Template we dont know the no of rows to be included in excel, so 65535 is used as value of lastrow parameter

            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);


            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");
        }
        //Export Template for Surveyes
        public static string ExportTemplateToExcel(DataTable dt, DataTable dtSurveys, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            string extension = System.IO.Path.GetExtension(Filename);
            string sheetName = Filename.Substring(0,Filename.Length - extension.Length);
            XSSFSheet sheet1 = (XSSFSheet)workbook.CreateSheet(sheetName);
            IRow row1 = sheet1.CreateRow(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();
            }
            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                   // GC.Collect();
                }
            }
            
            IList<string> target = dtSurveys.AsEnumerable()
                           .Select(r => r.Field<string>("SurveyName"))
                           .ToList();

            CreateDropDownListForExcel(sheet1, target, 1,65535, 0);//While generating Template we dont know the no of rows to be included in excel, so 65535 is used as value of lastrow parameter

            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);


            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");
        }

        public static void CreateDDLItemTypeNameForExcelForQuestionBank(this XSSFSheet sheet, IList<string> dropDownValues, int startRow, int lastRow, int column)
        {
            if (sheet == null)
            {
                return;
            }

            //Create a hidden sheet on the workbook (using the column as an id) with the dropdown values
            IWorkbook workbook = sheet.Workbook;
            string dropDownName = sheet.SheetName + "_ItemTypeName";
            ISheet hiddenSheet = workbook.CreateSheet(dropDownName);
            for (int i = 0, length = dropDownValues.Count; i < length; i++)
            {
                string name = dropDownValues[i];
                IRow row = hiddenSheet.CreateRow(i);
                ICell cell = row.CreateCell(1);
                cell.SetCellValue(name);
            }


            //Create the dropdown using the fields of the hidden sheet
            IName namedCell = workbook.CreateName();
            namedCell.NameName = dropDownName;
            namedCell.RefersToFormula = (dropDownName + "!$B$1:$B$" + dropDownValues.Count);
            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sheet);
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(dropDownName);
            CellRangeAddressList addressList = new CellRangeAddressList(startRow, lastRow, column, column);
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);
            sheet.AddValidationData(validation);
        }

        public static void CreateDropDownListForExcel(this XSSFSheet sheet, IList<string> dropDownValues, int startRow, int lastRow, int column)
        {
            if (sheet == null)
            {
                return;
            }

            //Create a hidden sheet on the workbook (using the column as an id) with the dropdown values
            IWorkbook workbook = sheet.Workbook;
            string dropDownName = sheet.SheetName + "_Surveys";
            ISheet hiddenSheet = workbook.CreateSheet(dropDownName);
            for (int i = 0, length = dropDownValues.Count; i < length; i++)
            {
                string name = dropDownValues[i];
                IRow row = hiddenSheet.CreateRow(i);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(name);
            }

            
            //Create the dropdown using the fields of the hidden sheet
            IName namedCell = workbook.CreateName();
            namedCell.NameName = dropDownName;
            namedCell.RefersToFormula = (dropDownName + "!$A$1:$A$" + dropDownValues.Count);
            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sheet);
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(dropDownName);
            CellRangeAddressList addressList = new CellRangeAddressList(startRow,lastRow, column, column);
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);
            sheet.AddValidationData(validation);
        }

        public static void CreateDDLItemTypeNameForExcel(this XSSFSheet sheet, IList<string> dropDownValues, int startRow, int lastRow, int column)
        {
            if (sheet == null)
            {
                return;
            }

            //Create a hidden sheet on the workbook (using the column as an id) with the dropdown values
            IWorkbook workbook = sheet.Workbook;
            string dropDownName = sheet.SheetName + "_ItemTypeName";
            ISheet hiddenSheet = workbook.CreateSheet(dropDownName);
            for (int i = 0, length = dropDownValues.Count; i < length; i++)
            {
                string name = dropDownValues[i];
                IRow row = hiddenSheet.CreateRow(i);
                ICell cell = row.CreateCell(5);
                cell.SetCellValue(name);
            }


            //Create the dropdown using the fields of the hidden sheet
            IName namedCell = workbook.CreateName();
            namedCell.NameName = dropDownName;
            namedCell.RefersToFormula = (dropDownName + "!$F$1:$F$" + dropDownValues.Count);
            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sheet);
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(dropDownName);
            CellRangeAddressList addressList = new CellRangeAddressList(startRow, lastRow, column, column);
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);
            sheet.AddValidationData(validation);
        }
        //This method for use only for Accounting Memo this not Generic Method. please do not use any other.
        public static string ExportAccountingMemoToExcel(DataTable dt, DataTable dtNextSteps, DataTable dtAccountingScenarioMatrix, string TempPath, string Filename)
        {

            
            
            //Create Excel for AccountingMemo
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("AccountingMemo");
            ISheet sheet2 = workbook.CreateSheet("NextSteps");
            ISheet sheet3 = workbook.CreateSheet("AccountingScenarioMatrix");

            //Apply Sheet level formatting on the AccountingMemo Sheet (As of now, we don't need to apply formatting on any of the other sheets)
            //IFont font1 = workbook.CreateFont();
            //ICellStyle style1 = workbook.CreateCellStyle();
            //style1.FillPattern = FillPattern.SolidForeground;
            //style1.VerticalAlignment = VerticalAlignment.Top;
            //style1.WrapText = true;

            //Create the header row here
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string columnName = dt.Columns[j].ToString();
                string columnId;
                if (columnName.Equals("QuestionName"))
                {
                    columnId = "Id";
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(columnId);
                }
                // GC is used to avoid error System.argument exception
               // GC.Collect();
            }

            //loops through data and populate the cell data in the sheet
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var ItemType = dt.Rows[i]["ItemType"].ToString();

                int MaxUsableColumns = 0;
                if (ItemType == "TABLE")
                {
                    MaxUsableColumns = 11;
                }
                else
                {
                    MaxUsableColumns = 3;
                }
               

                IRow row = sheet1.CreateRow(i + 1);

                var OutputFormat = dt.Rows[i]["OutputFormat"].ToString();
                string[] strOutPutFormatval = OutputFormat.Split(',');

                int HCellPosition = 0;
                
                for (int j = 0; j < MaxUsableColumns; j++) //In the event when item type is not table there will be at the max 3 columns (Question_Name, MemoCol1, MemoCol2) will be populated.
                {
                    String columnName = dt.Columns[j].ToString();
                    if (columnName.Equals("QuestionName") || columnName.Equals("MemoCol1") || columnName.Equals("MemoCol2") || columnName.Equals("MemoCol3") || columnName.Equals("MemoCol4") || columnName.Equals("MemoCol5") || columnName.Equals("MemoCol6") || columnName.Equals("MemoCol7") || columnName.Equals("MemoCol8") || columnName.Equals("MemoCol9") || columnName.Equals("MemoCol10") || columnName.Equals("MemoCol11") || columnName.Equals("MemoCol12") || columnName.Equals("MemoCol13") || columnName.Equals("MemoCol14") || columnName.Equals("MemoCol15"))
                    {
                        ICell cell = row.CreateCell(HCellPosition);
                        cell.SetCellValue(dt.Rows[i][columnName].ToString());


 
                        IFont font1 = workbook.CreateFont();
                        ICellStyle style1 = workbook.CreateCellStyle();


                        //Apply cell specific formatting
                        //
                        int MergeStartPosition = 0;
                        int MergeCount = 0;
                        for (int k = 0; k < strOutPutFormatval.Count(); k++)
                        {
                            string[] Keyvalue = strOutPutFormatval[k].Split('=');
                            switch (Keyvalue[0])
                            {
                                case "WordStyle":
                                    font1.Boldweight = (short)GetNPOIWordStyle(Keyvalue[1]);
                                    break;
                                case "BgColor":
                                    style1.FillForegroundColor = (short)GetNPOIColorIndex(Keyvalue[1]);
                                    style1.FillPattern = FillPattern.SolidForeground;
                                    break;
                                case "TextColor":
                                    font1.Color = (short)GetNPOIColorIndex(Keyvalue[1]);
                                    break;
                                case "Font":
                                    font1.FontName = Keyvalue[1];
                                    break;
                                case "FontSize":
                                    font1.FontHeightInPoints = Convert.ToInt16(Keyvalue[1]);
                                    break;
                                case "MergeCount":
                                    //font1.FontHeightInPoints = Convert.ToInt16(Keyvalue[1]);
                                    MergeCount = Convert.ToInt16(Keyvalue[1]);
                                    break;
                                case "MergeStartPosition":
                                    MergeStartPosition = Convert.ToInt16(Keyvalue[1]);
                                    break;

                                    
                            }

                        }
                        if (ItemType == "TABLE")
                        {
                            style1.BorderRight = BorderStyle.Thin;
                            style1.BorderBottom = BorderStyle.Thin;
                            style1.BorderTop = BorderStyle.Thin;
                            style1.BorderLeft = BorderStyle.Thin;
                            sheet1.SetColumnWidth(j + 1, 21 * 256);
                        }

                        //Merging Cell Code
                        if (MergeCount > 0 && j == (MergeStartPosition - 1))
                        {

                            CellRangeAddress MergedCellRange = new CellRangeAddress(i + 1, i + 1, MergeStartPosition - 1, MergeCount);
                            sheet1.AddMergedRegion(MergedCellRange);
                            HCellPosition = HCellPosition + MergeCount - 1;
                            if (HCellPosition < 10)
                            {

                                MergedCellRange = new CellRangeAddress(i + 1, i + 1, HCellPosition + 1, (10));
                                sheet1.AddMergedRegion(MergedCellRange);
                            }

                            
                            var ExcelRowHeight = Convert.ToInt32(dt.Rows[i]["ExcelRowHeight"]);
                            if (ItemType == "L1_TITLE" || ItemType == "L2_TITLE" || ItemType == "L3_TITLE")
                            {
                                ExcelRowHeight = 2;
                            }
                            var defaultrowheight = sheet1.DefaultRowHeightInPoints;
                            sheet1.GetRow(i + 1).HeightInPoints = ExcelRowHeight * defaultrowheight;

                        }

                        style1.VerticalAlignment = VerticalAlignment.Top;
                        style1.WrapText = true;

                        style1.SetFont(font1);
                        cell.CellStyle = style1;

                    }

                    HCellPosition = HCellPosition + 1;
                    //GC.Collect();
                }

                
            }
            

            //This is for NextSteps Export
            if (dtNextSteps.Rows.Count == 0)
            {
                // This is for Blank column display if data are not exist in table. 
                dtNextSteps.Columns.Add("QuestionCode");
                dtNextSteps.Columns.Add("QuestionText");
                dtNextSteps.Columns.Add("Response");
                dtNextSteps.Columns.Add("IsDone");
                dtNextSteps.Columns.Add("ActionTaken");
                dtNextSteps.Columns.Add("NextStepText");
                IRow row2 = sheet2.CreateRow(0);
                for (int j = 0; j < dtNextSteps.Columns.Count; j++)
                {
                    ICell cell = row2.CreateCell(j);
                    string columnName = dtNextSteps.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                    // GC is used to avoid error System.argument exception
                    //GC.Collect();
                }
            }
            else
            {
                IRow row2 = sheet2.CreateRow(0);
                ICellStyle style2 = workbook.CreateCellStyle();
                for (int j = 0; j < dtNextSteps.Columns.Count; j++)
                {
                    ICell cell = row2.CreateCell(j);
                    string columnName = dtNextSteps.Columns[j].ToString();
                    //setting column width and WrapText property for Columns excluding IsDone column
                    switch (columnName) {
                        case "ActionTaken":
                            sheet2.SetColumnWidth(j, 39 * 256);
                            style2.WrapText = true; 
                            cell.CellStyle = style2;
                            break;
                        case "QuestionCode":
                        case "QuestionText":
                        case "NextStepText":
                        case "Response":
                            sheet2.SetColumnWidth(j, 15 * 256);
                            style2.WrapText = true;
                            cell.CellStyle = style2;
                            break;
                    }

                    cell.SetCellValue(columnName);
                    // GC is used to avoid error System.argument exception
                   // GC.Collect();
                }
                //loops through data  
                for (int i = 0; i < dtNextSteps.Rows.Count; i++)
                {
                    IRow row = sheet2.CreateRow(i + 1);
                    for (int j = 0; j < dtNextSteps.Columns.Count; j++)
                    {
                        ICell cell = row.CreateCell(j);
                        String columnName = dtNextSteps.Columns[j].ToString();
                        //setting column width and WrapText property for Columns excluding IsDone column
                        switch (columnName)
                        {
                            case "ActionTaken":
                                sheet2.SetColumnWidth(j, 39 * 256);
                                style2.WrapText = true;
                                cell.CellStyle = style2;
                                break;
                            case "QuestionCode":
                            case "QuestionText":
                            case "NextStepText":
                            case "Response":
                                sheet2.SetColumnWidth(j, 15 * 256);
                                style2.WrapText = true;
                                cell.CellStyle = style2;
                                break;
                        }
                        cell.SetCellValue(dtNextSteps.Rows[i][columnName].ToString());
                       // GC.Collect();
                    }
                }
            }
            //This is for AccountingScenarioMatrix Export
            if (dtAccountingScenarioMatrix.Rows.Count == 0)
            {
                // This is for Blank column display if data are not exist in table.
                dtAccountingScenarioMatrix.Columns.Add("QuestionCode");
                dtAccountingScenarioMatrix.Columns.Add("Situation");
                dtAccountingScenarioMatrix.Columns.Add("ObjectType");
                dtAccountingScenarioMatrix.Columns.Add("Product");
                dtAccountingScenarioMatrix.Columns.Add("Scenario");
                dtAccountingScenarioMatrix.Columns.Add("ScenarioDescription");
                dtAccountingScenarioMatrix.Columns.Add("Comments");

                IRow row3 = sheet3.CreateRow(0);
                for (int j = 0; j < dtAccountingScenarioMatrix.Columns.Count; j++)
                {

                    ICell cell = row3.CreateCell(j);
                    string columnName = dtAccountingScenarioMatrix.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                    // GC is used to avoid error System.argument exception
                   // GC.Collect();
                }
            }
            else
            {
                IRow row3 = sheet3.CreateRow(0);
                for (int j = 0; j < dtAccountingScenarioMatrix.Columns.Count; j++)
                {

                    ICell cell = row3.CreateCell(j);
                    string columnName = dtAccountingScenarioMatrix.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                    // GC is used to avoid error System.argument exception
                    //GC.Collect();
                }

                //loops through data  
                for (int i = 0; i < dtAccountingScenarioMatrix.Rows.Count; i++)
                {
                    IRow row = sheet3.CreateRow(i + 1);
                    for (int j = 0; j < dtAccountingScenarioMatrix.Columns.Count; j++)
                    {

                        ICell cell = row.CreateCell(j);
                        String columnName = dtAccountingScenarioMatrix.Columns[j].ToString();
                        cell.SetCellValue(dtAccountingScenarioMatrix.Rows[i][columnName].ToString());
                        //GC.Collect();
                    }
                }
            }
            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
            //obook.SaveAs(FilePath);

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            

            return ("Success");
        }


        //Commented By Namita--03 October 2018,Function has moved in Globals of API therefore this is not in use
        //public static string ExportWordSample(DataTable dt, string TempPath, string Filename)
        //{
        //    //Create Word Document
        //    var WDTemplate = @"D:\Namita\Test\WDTemplate.docx";
        //    Stream stream = new FileStream(WDTemplate, FileMode.Open);
        //    ExportWord.XWPFDocument doc = new ExportWord.XWPFDocument(stream);
            
        //    //Id should displayed instead of Question Name
        //    for (int j = 0; j < dt.Columns.Count; j++)
        //    {
        //        string columnName = dt.Columns[j].ToString();
        //        string columnId;
        //        if (columnName.Equals("QuestionName"))
        //        {
        //            columnId = "Id";

        //            //ExportWord.XWPFTableCell c1 = table.GetRow(0).GetCell(0);

        //            ExportWord.XWPFParagraph pdt = doc.CreateParagraph();
        //            ExportWord.XWPFRun rdt = pdt.CreateRun();
        //            rdt.SetText(columnId);
        //            break;
        //        }

        //    }

        //    ExportWord.XWPFTableCell Wordcell;
        //    ExportWord.XWPFParagraph Wordpdt;
        //    ExportWord.XWPFRun Wordrdt;

        //    var BGColor = "";
        //    var TextColor = "";
        //    var Font = "";
        //    var FontSize = 0;
        //    var tableColumnCount = 0;
        //    int count = 0;
        //    DataTable dtTable = new DataTable();
            
        //    dtTable = dt.Clone();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        //Function will be creating later on to avoid rewriting the code
        //        var OutputFormat = dt.Rows[i]["OutputFormat"].ToString();
        //        string[] strOutPutFormatval = OutputFormat.Split(',');
        //        for (int k = 0; k < strOutPutFormatval.Count(); k++)
        //        {
        //            string[] Keyvalue = strOutPutFormatval[k].Split('=');
        //            switch (Keyvalue[0])
        //            {
        //                //case "WordStyle":

        //                //    break;
        //                case "BgColor":
        //                    BGColor = Keyvalue[1];
        //                    break;
        //                case "TextColor":
        //                    TextColor = Keyvalue[1];
        //                    break;
        //                case "Font":
        //                    Font = Keyvalue[1];
        //                    break;
        //                case "FontSize":
        //                    FontSize = Convert.ToInt16(Keyvalue[1]);
        //                    break;
        //            }

        //        }

        //        var ItemType = dt.Rows[i]["ItemType"].ToString();
                
        //        //Getting Row count for Table Item Types for creating doc table
        //        if (dt.Rows[i]["ItemType"].ToString() == "TABLE")
        //        {
        //            count = count + 1;

        //            var Rowdata = dt.Rows[i];
        //            dtTable.ImportRow(Rowdata);
        //            goto nextIteration;
        //        }
        //        else
        //        {

        //            goto next;
        //        }


        //        next:


        //        if (i > 0)
        //        {
             
        //            if (dt.Rows[i - 1]["ItemType"].ToString() == "TABLE")
        //            {
        //                //this is table column counting to avoid nullable columns.
        //                for (int a = 0; a < dtTable.Rows.Count; a++)
        //                {
        //                    for (int b = 0; b < 16; b++)
        //                    {
        //                        var values = dtTable.Rows[a][b].ToString();
        //                        if (values != "" && values != null)
        //                        {
        //                            if (tableColumnCount <= b)
        //                            {
        //                                tableColumnCount = b+1;
        //                            }
        //                        }
        //                    }
        //                }
                        
        //                var tableBGColor = "";
        //                var tableTextColor = "";
        //                var tableFont = "";
        //                var tableFontSize = 0;
        //                OutputFormat = dt.Rows[i - 1]["OutputFormat"].ToString();
        //                strOutPutFormatval = OutputFormat.Split(',');
        //                for (int k = 0; k < strOutPutFormatval.Count(); k++)
        //                {
        //                    string[] Keyvalue = strOutPutFormatval[k].Split('=');
        //                    switch (Keyvalue[0])
        //                    {
        //                        //case "WordStyle":

        //                        //    break;
        //                        case "BgColor":
        //                            tableBGColor = Keyvalue[1];
        //                            break;
        //                        case "TextColor":
        //                            tableTextColor = Keyvalue[1];
        //                            break;
        //                        case "Font":
        //                            tableFont = Keyvalue[1];
        //                            break;
        //                        case "FontSize":
        //                            tableFontSize = Convert.ToInt16(Keyvalue[1]);
        //                            break;
        //                    }

        //                }
                        
        //                ExportWord.XWPFTable table = doc.CreateTable(count, tableColumnCount);
        //                for (int RowIndex = 0; RowIndex < count; RowIndex++)
        //                {
        //                    for (int columnIndex = 0; columnIndex < tableColumnCount; columnIndex++)
        //                    {
        //                        // table.GetRow(RowIndex).GetCell(columnIndex).SetText(dtTable.Rows[RowIndex][columnIndex].ToString());
        //                        Wordcell = table.GetRow(RowIndex).GetCell(columnIndex);
        //                        Wordpdt = Wordcell.AddParagraph();
        //                        Wordrdt = Wordpdt.CreateRun();
        //                        Wordrdt.SetText(dtTable.Rows[RowIndex][columnIndex].ToString());
        //                        Wordrdt.SetColor(tableTextColor);
        //                        Wordcell.SetColor(tableBGColor);
        //                        Wordrdt.FontFamily = tableFont;
        //                        Wordrdt.FontSize = tableFontSize;
        //                        Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
        //                    }

        //                }

        //                count = 0;
        //                dtTable.Clear();
        //                tableColumnCount = 0;
        //            }
        //        }
        //        //For all ItemType except table.
        //        for (int l = 0; l < 3; l++)
        //        {
                    
        //            Wordpdt = doc.CreateParagraph();
        //            Wordrdt = Wordpdt.CreateRun();

        //            //////////////////applying styles///////////////////////////

        //            NPOI.XWPF.UserModel.XWPFStyles styles = doc.CreateStyles();

        //            String strStyleName = "subtitle";
        //            CT_Style ctStyle = new CT_Style();

                   

        //            ctStyle.styleId = (strStyleName);
        //            NPOI.XWPF.UserModel.XWPFStyle s = new NPOI.XWPF.UserModel.XWPFStyle(ctStyle);
        //            styles.AddStyle(s);

        //            ///////////////////////////////////////////

        //            if (l==0)
        //            {
        //                Wordrdt.SetText(dt.Rows[i][l].ToString() + "  " + dt.Rows[i][l + 1].ToString());
        //                l=l + 1;
        //            }
        //            else
        //            {
        //                Wordrdt.SetText(dt.Rows[i][l].ToString());
        //            }

                

        //            Wordrdt.SetColor(TextColor);
                    
        //            Wordpdt.FillBackgroundColor = BGColor;
        //            Wordpdt.FillPattern = ST_Shd.clear;
        //            Wordrdt.FontFamily = Font;
        //            Wordrdt.FontSize = FontSize;
        //            Wordpdt.Alignment = ExportWord.ParagraphAlignment.BOTH;
                    
        //        }
            
        //      //  }

        //        nextIteration:
        //        bool flag;



        //    }

        //    string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
        //                                           //obook.SaveAs(FilePath);

        //    FileStream out1 = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);

        //    doc.Write(out1);


        //    out1.Close();

        //    doc.Close();
        //    stream.Close();
        //    return ("Success");
        //    //} 
        //}

        //private static void changeOrientation(ExportWord.XWPFDocument doc, String orientation)
        //{
        //    CT_Document document = doc.Document;
        //    CT_Body body = document.body;
        //    CT_SectPr section = body.sectPr;
        //    ExportWord.XWPFParagraph para = doc.CreateParagraph();
           
        //    CT_P ctp = new CT_P();
        //    CT_PPr br = ctp.AddNewPPr();
        //    br.sectPr = section;
        //    CT_PageSz pageSize = section.pgSz;
        //    if (orientation.Equals("Landscape"))
        //    {
        //        pageSize.orient = ST_PageOrientation.landscape;
        //    }
        //    else
        //    {
        //        pageSize.orient = ST_PageOrientation.portrait;
        //    }
        //}


        public static int GetNPOIWordStyle(string WordStyle)
        {
            if (WordStyle == "Bold")
            {
                var headerLabelFont = (short)FontBoldWeight.Bold;
                return headerLabelFont;
            }
            else if (WordStyle == "Normal")
            {
                var headerLabelFont = (short)FontBoldWeight.Normal;
                return headerLabelFont;
            }
            else if (WordStyle == "None")
            {
                var headerLabelFont = (short)FontBoldWeight.None;
                return headerLabelFont;
            }
            return 0;
        }
        public static int GetNPOIColorIndex(string BGColor)
        {
            //int ColorIndex;
            if (BGColor == "Aqua")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Aqua.Index;
                return ColorIndex;
            }
            else if (BGColor == "Black")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Black.Index;
                return ColorIndex;
            }
            else if (BGColor == "Blue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                return ColorIndex;
            }
            else if (BGColor == "BlueGrey")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.BlueGrey.Index;
                return ColorIndex;
            }
            else if (BGColor == "BrightGreen")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.BrightGreen.Index;
                return ColorIndex;
            }
            else if (BGColor == "Brown")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Brown.Index;
                return ColorIndex;
            }
            else if (BGColor == "Coral")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Coral.Index;
                return ColorIndex;
            }
            else if (BGColor == "CornflowerBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.CornflowerBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "DarkGreen")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.DarkGreen.Index;
                return ColorIndex;
            }
            else if (BGColor == "DarkRed")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.DarkRed.Index;
                return ColorIndex;
            }
            else if (BGColor == "DarkTeal")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.DarkTeal.Index;
                return ColorIndex;
            }
            else if (BGColor == "DarkYellow")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.DarkYellow.Index;
                return ColorIndex;
            }
            else if (BGColor == "Gold")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Gold.Index;
                return ColorIndex;
            }
            else if (BGColor == "Green")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Green.Index;
                return ColorIndex;
            }
            else if (BGColor == "Indigo")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Indigo.Index;
                return ColorIndex;
            }
            else if (BGColor == "Lavender")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Lavender.Index;
                return ColorIndex;
            }
            else if (BGColor == "LemonChiffon")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightCornflowerBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightGreen")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightOrange")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightTurquoise")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightTurquoise.Index;
                return ColorIndex;
            }
            else if (BGColor == "LightYellow")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
                return ColorIndex;
            }
            else if (BGColor == "Lime")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Lime.Index;
                return ColorIndex;
            }
            else if (BGColor == "Maroon")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Maroon.Index;
                return ColorIndex;
            }
            else if (BGColor == "OliveGreen")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.OliveGreen.Index;
                return ColorIndex;
            }
            else if (BGColor == "Orange")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Orange.Index;
                return ColorIndex;
            }
            else if (BGColor == "Orchid")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Orchid.Index;
                return ColorIndex;
            }
            else if (BGColor == "PaleBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "Pink")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Pink.Index;
                return ColorIndex;
            }
            else if (BGColor == "Plum")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Plum.Index;
                return ColorIndex;
            }
            else if (BGColor == "Rose")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Rose.Index;
                return ColorIndex;
            }
            else if (BGColor == "RoyalBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.RoyalBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "SeaGreen")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.SeaGreen.Index;
                return ColorIndex;
            }
            else if (BGColor == "SkyBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "Tan")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Tan.Index;
                return ColorIndex;
            }
            else if (BGColor == "Teal")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Teal.Index;
                return ColorIndex;
            }
            else if (BGColor == "Turquoise")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
                return ColorIndex;
            }
            else if (BGColor == "Violet")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Violet.Index;
                return ColorIndex;
            }
            else if (BGColor == "White")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.White.Index;
                return ColorIndex;
            }
            else if (BGColor == "Yellow")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                return ColorIndex;
            }
            else if (BGColor == "DarkBlue")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.DarkBlue.Index;
                return ColorIndex;
            }
            else if (BGColor == "Red")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Red.Index;
                return ColorIndex;
            }
            else if (BGColor == "Grey")
            {
                int ColorIndex = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
                return ColorIndex;
            }
            return 0;
        }

        //public static string ExportToExcel(DataTable dt, string TempPath, string Filename)
        //{
        //    /*Set up work book, work sheets, and excel application*/
        //    Microsoft.Office.Interop.Excel.Application oexcel = new Microsoft.Office.Interop.Excel.Application();
        //    try
        //    {
        //        string path = AppDomain.CurrentDomain.BaseDirectory;
        //        object misValue = System.Reflection.Missing.Value;
        //        Microsoft.Office.Interop.Excel.Workbook obook = oexcel.Workbooks.Add(misValue);
        //        Microsoft.Office.Interop.Excel.Worksheet osheet = new Microsoft.Office.Interop.Excel.Worksheet();
        //        osheet = (Microsoft.Office.Interop.Excel.Worksheet)obook.Sheets["Sheet1"];
        //        int colIndex = 0;
        //        int rowIndex = 1;
        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            colIndex++;
        //            osheet.Cells[1, colIndex] = dc.ColumnName;
        //        }
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            rowIndex++;
        //            colIndex = 0;

        //            foreach (DataColumn dc in dt.Columns)
        //            {
        //                colIndex++;
        //                osheet.Cells[rowIndex, colIndex] = dr[dc.ColumnName];
        //            }
        //        }

        //        osheet.Columns.AutoFit();
        //        string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
        //        obook.SaveAs(FilePath);

        //        //string[] csvData = System.IO.File.ReadAllLines(TempPath);
        //        //string result = new StreamReader(file.InputStream).ReadToEnd();
        //        //File saved at temporary path, now we need to move it to S3 bucket using AWS SDK 
        //        // string FileUploadSucceeded = UploadToS3(System.IO.StreamReader(TempPath.InputStream).ReadToEnd(), Filename);

        //        obook.Close();
        //        oexcel.Quit();
        //        // releaseObject(osheet);
        //        // releaseObject(obook);
        //        //releaseObject(oexcel);
        //        GC.Collect();
        //        using (StreamReader sr = new StreamReader(FilePath))
        //        {

        //            UploadStreamToS3(sr.BaseStream, Filename);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oexcel.Quit();
        //        throw (ex); //this will be caught in another enclosing try/catch or in FilterConfig exception handler.
        //    }

        //    return ("Success");
        //}

        //This method returns the WFComments in the desired format
        public static string GenerateWFComments(string UserName, string RoleName, string NewComments, string PreviousComments, string Action, string Step)
        {
            string WFComments = null;
            if (string.IsNullOrEmpty(Action)) { Action = ""; }
            // if (!string.IsNullOrEmpty(NewComments))
            // {
            if (string.IsNullOrEmpty(PreviousComments))//when previous commments do not exist
            {
                WFComments = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy") + "] [" + UserName + "] [" + RoleName + "] [" + Action + "] [" + Step + "]" + NewComments;
            }
            else//when previous commments exist
            {
                WFComments = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy") + "] [" + UserName + "] [" + RoleName + "] [" + Action + "] [" + Step + "]" + NewComments + Environment.NewLine + PreviousComments;
            }

            // }
            return WFComments;
        }
       
        //This method calculates the WorkflowId based on the Workflow stored in session.
        public static int GetWFId()
        {
            string WorkFlowName = HttpContext.Current.Session["WorkFlow"] as string;
            string CompanyCode = HttpContext.Current.Session["CompanyCode"] as string;
            IRWorkFlowsRestClient RRC = new RWorkFlowsRestClient();
            var WorkFlowData = RRC.GetByName(WorkFlowName, CompanyCode);
            int WFId = 0;
            if (WorkFlowData != null)
                WFId = WorkFlowData.Id;
            return WFId;
        }
        //This method calculates the StepId based on WorkflowId and Ordinal
        public static int GetStepId(int OridnalValue,int WorkFlowId)
        {
            string CompanyCode = HttpContext.Current.Session["CompanyCode"] as string;
            //int WorkFlowId = Convert.ToInt32(System.Web.HttpContext.Current.Session["WorkFlowId"]);
            IWStepRestClient SRC = new WStepRestClient();
            int StepId = SRC.GetStepIdByWFIdAndOrdinal(WorkFlowId, OridnalValue, CompanyCode);
            return StepId;
        }
        //Thismethod validates the cell values against Nullability,min length,max length and others
        public static string ValidateCellData(string TableName, string ColumnName, object value, string FormLabel, string SelecterType, string CompanyCode)
        {
            if (value == null || String.IsNullOrEmpty(Convert.ToString(value)) || "null".Equals(Convert.ToString(value)))
            {
                value = "blank";
            }
            ILReferencesRestClient RRC = new LReferencesRestClient();
            var result = RRC.ValidateColumnValue(TableName, ColumnName, Convert.ToString(value), FormLabel, SelecterType, CompanyCode, null);
            if (String.IsNullOrEmpty(result.Name))
            {
                result.Name = "Success";
            }
            return result.Name;
        }

        //This function will return value corresponding to a Key from GKeyValues Table
        public static GKeyValueViewModel GetKeyvalue(string Key)
        {
            GKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            string GetCompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();

            GKeyValueViewModel KeyValue = KVRC.GetKeyValue(Key, GetCompanyCode);
            //System.Web.HttpContext.Current.Session["KeyValue"] = KeyValue;
            return (KeyValue);
        }


        //This method is retired and currently not being referenced anywhere. For cell validations above method is being used
        public static string ValidateData(string TableName, string ColumnName, object value)
        {

            //var ValidData = new List<InformationSchemaViewModel>();
            //var ModelList = new List<InformationSchemaViewModel>();
            //var ErroredList = new List<InformationSchemaViewModel>();
            //string ErrorList = null;
            //ILReferencesRestClient RestClient = new LReferencesRestClient();
            //var model = new InformationSchemaViewModel();

            //var InformationSchema = RestClient.GetInformationSchema(TableName, ColumnName);
            //var datatype = InformationSchema.DATA_TYPE;
            //string CellValue = value.ToString();

            //if (string.IsNullOrEmpty(CellValue))
            //{
            //    if (InformationSchema.IS_NULLABLE == "YES" || InformationSchema.COLUMN_DEFAULT != null)
            //    {
            //    }
            //    else
            //    {
            //        ErrorList= "This is mandatory field.";
            //    }
            //}
            //else
            //{

            //    if (CellValue.Length <=5)
            //    {

            //    }
            //    else
            //    {
            //        ErrorList += "| Length should be less than " + InformationSchema.CHARACTER_MAXIMUM_LENGTH;
            //    }

            //}

            //return ErrorList;

            return "Success";
        }

        //This method is using SMTP client to send emails directly, without db interaction(In some cases, when DB might fail)
        public static bool SendExceptionEmail(string EmailBody)
        {
            try
            {
                String FROM = ConfigurationManager.AppSettings["SenderEmailId"];   //This address must be verified.

                // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
                String SMTP_USERNAME = ConfigurationManager.AppSettings["SmtpLoginId"];  //SMTP username. 
                String SMTP_PASSWORD = ConfigurationManager.AppSettings["SmtpPassword"];  //SMTP password.

                // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
                String HOST = ConfigurationManager.AppSettings["SmtpHost"];

                // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
                // STARTTLS to encrypt the connection.
                int PORT = Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"]);

                // Create an SMTP client with the specified host name and port.
                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Create a network credential with your SMTP user name and password.
                    client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    client.EnableSsl = true;

                    string ToAddress = ConfigurationManager.AppSettings["ExceptionEmailTo"];
                    string EmailSubject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
                    // Send the email. 
                    MailMessage message = new MailMessage(FROM, ToAddress, EmailSubject, EmailBody);
                    message.IsBodyHtml = true;
                    //client.Send(FROM, ToAddress, EmailSubject, EmailBody);
                    client.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public string GetColumnType(object Value)
        //{
        //    var datatype = Value.GetType();
        //    switch (datatype.ToString())
        //    {
        //            case "varchar":
        //            case "nvarchar":
        //            return typeof(string);
        //    break;
        //            case "int":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(int?));
        //    break;
        //            case "date":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(DateTime?));
        //    break;
        //            case "datetime":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(DateTime?));
        //    break;
        //            case "bit":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(bool?));
        //    break;
        //            case "bigint":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(Int64?));
        //    break;
        //            case "decimal":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(decimal?));
        //    break;
        //            case "float":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(double?));
        //    break;
        //            case "numeric":
        //                CreateAutoImplementedProperty(builder, item.ColumnName.ToString(), typeof(double?));
        //    break;
        //}
        //    return "";
        //}

        #region Upload/download for Excel sheet
        //This method is used for Generating Template for the excel depending on Type and Selecter
        //it is being used for Copa, GPOB, ref data
        public static string GenerateTemplate(string TemplateType, string SelecterType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
            DataTable dt = new DataTable();
            string filename = null;
            ILReferencesRestClient RRC = new LReferencesRestClient();
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            //asterisk(*) is implemented with the mandatory columns
            switch (TemplateType)
            {
                case "GCopaDimensions":
                    dt.Columns.Add("Class");
                    dt.Columns.Add("CopaValue");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Dimension");
                    dt.Columns.Add("DimentionClassDescription");
                    filename = "CopaDimension.xlsx";
                    break;
                case "GGlobalPobs":
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("Category");
                    dt.Columns.Add("IFRS15Account");
                    filename = "GGLobalPOB.xlsx";
                    break;
                case "LReferences":
                    // string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
                    List<LReferenceDataViewModel> list = new List<LReferenceDataViewModel>();
                    ILCompanySpecificColumnsRestClient LCSRDC = new LCompanySpecificColumnsRestClient();
                    ILReferenceTypesRestClient TypesRestClient = new LReferenceTypesRestClient();
                    //Effective Dating
                    bool IsEffectiveDated = TypesRestClient.GetByReferenceTypeByName(SelecterType).IsEffectiveDated;
                    var CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, "LReferenceData", SelecterType).ToList();
                    if (IsEffectiveDated)
                    {
                        CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveStartDate", Label = "StartDate", DataType = "datetime", IsMandatory = true });
                        CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "EffectiveEndDate", Label = "EndDate", DataType = "datetime", IsMandatory = true });
                    }
                    var LReferenceDataColumnList = CompanySpecificData;
                    DataRow dr = dt.NewRow();
                    int i = 0;
                    foreach (LCompanySpecificColumnViewModel Lbl in LReferenceDataColumnList)
                    {
                        
                        dt.Columns.Add(Lbl.ColumnName);
                        //if (Lbl.IsMandatory)
                        //{
                        //    dr[i] = Lbl.Label+"*";
                        //}
                        //else
                        {
                            dr[i] = Lbl.Label;
                        }
                        i++;
                    }
                    dt.Rows.Add(dr);
                    filename = "RefData_" + SelecterType + ".xlsx";
                    break;
            }
            string Result = Globals.ExportToExcel(dt, path, filename);
            return Result;
        }

        //This method is used for Generating Template for the excel depending on Type and Selecter
        //it is being used for Sections, Chapters
        public static string GenerateTemplateForSurvey(string TemplateType,string FileName)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            DataTable dt = new DataTable();
            switch (TemplateType)
            {
                //defining columns for Excel which are fixed and reference is taken from DB
                case "LFSSections":
                    dt.Columns.Add("Survey");
                    dt.Columns.Add("ChapterCode");
                    dt.Columns.Add("SectionName");
                    dt.Columns.Add("SectionCode");
                    dt.Columns.Add("Ordinal");
                    dt.Columns.Add("InternalNotes");
                    break;
                case "LFSChapters":
                    dt.Columns.Add("Survey");
                    dt.Columns.Add("ChapterCode");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("Ordinal");
                    dt.Columns.Add("InternalNotes");
                    break;
            }
            //Survey Name Display
            DataTable dtSurvey = new DataTable();
            dtSurvey.Columns.Add("Id");
            dtSurvey.Columns.Add("SurveyName");
            IRLFSSectionRestClient SRC = new LFSSectionRestClient();
            var ApiSurveyName = SRC.GetSurveyName(CompanyCode);
            //calculating for Survey list which is to be used in second sheet of generated template. And this sheet will be responsible for Survey Dropdown 
            for (var i = 0; i < ApiSurveyName.Count(); i++)
            {
                DataRow dr = dtSurvey.NewRow();
                dr[0] = ApiSurveyName.ElementAt(i).Id;
                dr[1] = ApiSurveyName.ElementAt(i).SurveyName;
                dtSurvey.Rows.Add(dr);
                dtSurvey.AcceptChanges();
            }

            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"] + "\\";
            string Result = Globals.ExportTemplateToExcel(dt, dtSurvey, path, FileName);
            return Result;
        }

        public static ReadAndValidateViewModel ReadAndValidateExcel(string TableName, string SelecterType)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            string filename = System.Web.HttpContext.Current.Session["FileName"].ToString();
            string fullpath = path + "\\" + filename;
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            ReadAndValidateViewModel model = new ReadAndValidateViewModel();
            OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            con.Open();
            //get the sheets/tables in uploaded excel 
            var tables = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            var sheet1 = tables.Rows[0]["TABLE_NAME"].ToString(); // fetching name of first sheet as we upload data from first sheet only.
            OleDbDataAdapter cmd = null;
            if (TableName.Equals("GGlobalPobs"))
            {
                cmd = new OleDbDataAdapter("Select Name from [" + sheet1 + "] group by name having count(*) > 1", con);
                DataSet ds1 = new DataSet();
                cmd.Fill(ds1);
                DataTable dt1 = ds1.Tables[0];
                int cnt = dt1.Rows.Count;
                if (cnt > 0)//duplicate rows
                {
                    model.PopUpErrorMessage = "Sorry, File could not be processed. Uploaded file contains duplicate records.";
                    return model;
                }
            }
            else if (TableName.Equals("GCopaDimensions"))
            {
                cmd = new OleDbDataAdapter("Select Dimension from [" + sheet1 + "] group by Dimension having count(*) > 1", con);
                DataSet ds1 = new DataSet();
                cmd.Fill(ds1);
                DataTable dt1 = ds1.Tables[0];
                int cnt = dt1.Rows.Count;
                if (cnt > 0)//duplicate rows
                {
                    model.PopUpErrorMessage = "Sorry, File could not be processed. Uploaded file contains duplicate records.";
                    return model;
                }
            }
            cmd = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [" + sheet1 + "]", con);
            //OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("select * from [SHEET1$]", con);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            
             DataTable dt = ds.Tables[0];
            con.Close();
            List<string> list = new List<string>();
            //validating templates
            bool IsRowOk = true;
            string RowLevelValidationMessage = null;
            DataTable ValidationOK = dt.Clone();
            DataTable ValidationError = dt.Clone();
            var InvalidRowIndex = 0;
            var ValidRowIndex = 0;
            DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            Col.SetOrdinal(0);// to put the column in position 0;
            DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            Col.SetOrdinal(1);// to put the column in position 0;
           
            //bool MatchFound = false;
            var LCSRDC = new LCompanySpecificColumnsRestClient();
            var ColumnsList = dt.Columns;
            string returnMessage = ValidateUploadFileHeaderRow(CompanyCode, TableName, SelecterType, ColumnsList);
            if (returnMessage != null)
            {
                model.PopUpErrorMessage = returnMessage;
                return model;
            }
            //as Ref data upload(template) file contains first two informational rows, setting value of i to 1
            int startIndex = 0;
            if (TableName.Equals("LReferenceData"))
            {
                startIndex = 1;
            }
            LFinancialSurveysRestClient LFSRC = new LFinancialSurveysRestClient();
            for (int i = startIndex; i < dt.Rows.Count; i++)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    string ValidationResult = "";
                    //When the columnn Name will be Survey, we need to get validate cell value against SurveyId. Therefore, calculating SurveyId Value for the Survey Name provided in the row.
                    if (col.ColumnName == "Survey")
                    {
                        string value = dt.Rows[i].Field<string>(col);
                        int SurveyId = LFSRC.GetSurveyIdbyName(value);
                        col.ColumnName = "SurveyId";
                        ValidationResult = Globals.ValidateCellData(TableName, col.ColumnName, SurveyId, col.ColumnName, "", CompanyCode);
                        col.ColumnName = "Survey";
                    }
                    else
                    {
                         ValidationResult = Globals.ValidateCellData(TableName, col.ColumnName, dt.Rows[i].Field<object>(col), col.ColumnName, SelecterType, CompanyCode);
                    }
                    if (ValidationResult != "Success")
                    {
                        RowLevelValidationMessage = ValidationResult;
                        IsRowOk = false;
                        break;
                    }
                    else
                    {
                        IsRowOk = true;
                    }
                }
                if (IsRowOk)
                {
                    //Add Row to ValidationOK Table
                    ValidationOK.ImportRow(dt.Rows[i]);
                    ValidRowIndex = i - 1;
                }
                else
                {
                    ValidationError.ImportRow(dt.Rows[i]);
                    InvalidRowIndex = ValidationError.Rows.Count - 1;
                    ValidationError.Rows[InvalidRowIndex]["ValidationMessage"] = RowLevelValidationMessage;//setting cell value for the invalid record
                    ValidationError.Rows[InvalidRowIndex]["SrNo"] = i;
                    IsRowOk = false;
                }
            }
            model.ValidData = ValidationOK;
            model.ErrorData = ValidationError;
            if (ValidationError.Rows.Count == 0)
                model.HideButton = true;
            else
                model.HideButton = false;
            model.ErrorMessage = RowLevelValidationMessage;
            return model;
        }
        

        //This function validates the header of uploaded excel  against the desired template.
        public static string ValidateUploadFileHeaderRow(string CompanyCode, string TableName, string SelecterType, DataColumnCollection ColumnsList)
        {
            bool MatchFound = false;
            string returnMessage = null;
            var LCSRDC = new LCompanySpecificColumnsRestClient();
            List<LCompanySpecificColumnViewModel> CompanySpecificData = new List<LCompanySpecificColumnViewModel>();
            switch(TableName)
            {
                case "GCopaDimensions":
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Class" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "CopaValue" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Description" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Dimension" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "DimentionClassDescription" });
                    break;
                case "GGlobalPobs":
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Type" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Name" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Description" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Category" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "IFRS15Account" });
                    break;
                case "LFSSections":
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Survey" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "ChapterCode" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "SectionName" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "SectionCode" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Ordinal" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "InternalNotes" });
                    break;
                case "LFSChapters":
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Survey" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "ChapterCode" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Name" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Ordinal" });
                    CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "InternalNotes" });
                    break;

                default:
                    CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, TableName, SelecterType).ToList();
                    break;
            }
            /*if (TableName.Equals("GCopaDimensions"))
            {
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Class" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "CopaValue" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Description" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Dimension" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "DimentionClassDescription" });
            }
            else if (TableName.Equals("GGlobalPobs"))
            {
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Type" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Name" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Description" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "Category" });
                CompanySpecificData.Add(new LCompanySpecificColumnViewModel { ColumnName = "IFRS15Account" });
            }
            else
            {
                CompanySpecificData = LCSRDC.GetLCompanySpecificColumnsByCompanyCode(CompanyCode, TableName, SelecterType).ToList();
            } */
            foreach (var xx in CompanySpecificData)
            {
                foreach (DataColumn col in ColumnsList)
                {
                    if (col.ColumnName.Equals(xx.ColumnName))
                    {
                        MatchFound = true;
                        break;
                    }
                    else
                    {
                        MatchFound = false;
                    }
                }
                if (!MatchFound)
                {
                    returnMessage = "Sorry, File could not be processed. Uplaoded file Struture does not match with the template.";
                    return returnMessage;
                }
            }
            return returnMessage;
        }



       public static string ReadAndValidateCSV(int FileFormatId, int SysCatId)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            string filename = System.Web.HttpContext.Current.Session["FileName"].ToString();
            string fullpath = path + "\\" + filename;
           
             DataTable dtFile = ReadCSVAndGenerateDataTable(path, filename);
            var columnsList = dtFile.Columns;

            string returnMessage = ValidateCSVFileHeaderRow(CompanyCode, FileFormatId, columnsList);
            if (returnMessage != null)
            {
               return returnMessage;
            }

            string localpath = fullpath;
            string S3BucketReconBucketFolder = ConfigurationManager.AppSettings["S3BucketReconBucketFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReconBucketFolder + "/" + filename;
            Globals.UploadFileToS3(localpath, S3TargetPath);

            ILReconBucketRestclient RBRC = new LReconBucketRestclient();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            var Result = RBRC.ReadAndValidateCSVData(LoggedInUserId, filename, CompanyCode, FileFormatId,SysCatId, null);
            return Result.ToString();//as we are receiving int value from RestClient, so converting it to string as return type is sstring here.

           // FileInfo file = new FileInfo(filename);
           // DataTable dt = new DataTable();
           // bool IsRowOk = true;
           // string RowLevelValidationMessage = null;

           // string ValidationResult = null;
           // var InvalidRowIndex = 0;
           // var ValidRowIndex = 0;
           // string[] columns;
           // int ProductCodeIndex = 0; ;
           // string ProductCodelabel = "";
           // DataTable dt2 = new DataTable();
           // DataTable ValidationOK = new DataTable();
           // DataTable ValidationError = new DataTable();
           // //DataTable ValidationOK = dt2.Clone();
           // //DataTable ValidationError = dt2.Clone();

           // using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "\\;Extended Properties = 'text;HDR=Yes;FMT=Delimited(^)'; "))
           // {
           //     using (OleDbCommand cmd = new OleDbCommand(string.Format
           //                               ("SELECT * FROM [{0}]", file.Name), con))
           //     {
           //         con.Open();

           //         //CsvReader csv = new CsvReader(File.OpenText(fullpath));
           //         //csv.Read();
           //         //csv.ReadHeader();

           //         //List<string> headerList = csv.Context.HeaderRecord.ToList();

           //         // string[] columns= headerList[0].Split('^');

           //         // Using a DataReader to process the data
           //         using (OleDbDataReader reader = cmd.ExecuteReader())
           //         {

           //             //getting columns from file
           //             var columnNames = Enumerable.Range(0, reader.FieldCount)
           //             .Select(reader.GetName) //OR .Select("\""+  reader.GetName"\"") 
           //            .ToList();
           //             // var columnNames = (string.Join("^", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList()));


           //             string strcolumnNames = columnNames[0].ToString();
           //             //For getting value of each row
           //             //while (reader.Read())
           //             //{
           //             //    //string value= reader.GetValue(0).ToString();
           //             //}
           //             columns = strcolumnNames.Split('^');

           //         }

           //         // Populating DataTable with CSV file data
           //         using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
           //         {

           //             // DataTable tbl = new DataTable();
           //             adp.Fill(dt);



           //         }

           //        // Getting Label for ProductCode from LReconMapping for getting its Index
           //         foreach (var xx in ReconColumnData)
           //                 {
           //                     if (xx.IsProductCodeColumn)
           //                     {
           //                         ProductCodelabel = xx.Label;
           //                     }
           //                 }

           //         //This loop will add columns in DataTable dt2
           //         for (int j = 0; j < columns.Count(); j++)
           //         {
           //             dt2.Columns.Add(columns[j]);
           //             //Set the Index of Product Code field
           //             if (columns[j] == ProductCodelabel)
           //             {
           //                 ProductCodeIndex = j;
           //             }

           //         }

           //        // This loop will Values in datatable dt2
           //       for (int i = 0; i < dt.Rows.Count; i++)
           //         {

           //             DataRow OriginalRow = dt.Rows[i];
           //             string OriginalRowdata = OriginalRow.ItemArray[0].ToString();
           //             string[] OriginalRowdataArray = OriginalRowdata.Split('^');
           //             DataRow dr = dt2.NewRow();

           //             for (int c = 0; c < dt2.Columns.Count; c++)
           //             {
           //                 // int length = OriginalRowdataArray[c].Length;
           //                 if (OriginalRowdataArray[c].Length > 255)
           //                 {
           //                     // int CharToRemove = length - 255;

           //                     OriginalRowdataArray[c] = OriginalRowdataArray[c].Substring(0, 254);
           //                 }

           //                 dr[c] = OriginalRowdataArray[c];


           //             }

           //             dt2.Rows.Add(dr);
           //         }
           //     }
           // }
           // ValidationOK = dt2.Clone();
           //ValidationError = dt2.Clone();
           // DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
           // Col.SetOrdinal(0);// to put the column in position 0;
           // DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
           // Col.SetOrdinal(1);// to put the column in position 0;

          
            
           




           // ReadAndValidateViewModel model = new ReadAndValidateViewModel();

           // //Checking Valid and Invalid Records Row wise
           // for (int i = 0; i < dt2.Rows.Count;i++)
           // {

           //     DataRow dataRow = dt2.Rows[i];

           //     string ProductCode = dataRow.ItemArray[ProductCodeIndex].ToString();
           //  // string[] Rowdata = strRowdata.Split('^');
           //   // string ProductCode = Rowdata[ProductCodeIndex];
           //     //    // ProductCode = "P1";

           //     if (ProductCode != null)
           //     {
           //         // Check Existence of ProductCode
           //         //int ProductCount = LRCM.CheckExistenceOfProductCode(CompanyCode, ProductCode);
           //         //if (ProductCount == 0)
           //         //{
           //         //    RowLevelValidationMessage = "Product Code does not exist";
           //         //    IsRowOk = false;

           //         //}
           //         //else
           //         //{
           //         //    IsRowOk = true;
           //         //}
           //         IsRowOk = true;//SG commented this Code after discussion with VG. ProductCode existem=nce check not required here

           //     }
           //     else
           //     {
           //         RowLevelValidationMessage = "Product Code can not be blank";
           //         IsRowOk = false;
                   
                   
           //     }



           //     if (IsRowOk)
           //     {
           //         //Add Row to ValidationOK Table
           //         ValidationOK.ImportRow(dt2.Rows[i]);
           //         ValidRowIndex = i - 1;
           //     }
           //     else
           //     {
           //         ValidationError.ImportRow(dt2.Rows[i]);
           //         InvalidRowIndex = ValidationError.Rows.Count - 1;
           //         ValidationError.Rows[InvalidRowIndex]["ValidationMessage"] = RowLevelValidationMessage;//setting cell value for the invalid record
           //         ValidationError.Rows[InvalidRowIndex]["SrNo"] = i;
           //         IsRowOk = false;
           //     }
           // }
           //   model.ValidData = ValidationOK;
           //   model.ErrorData = ValidationError;
           //// return model;
           // return returnMessage;
        }

        public static string ValidateCSVFileHeaderRow(string CompanyCode, int FileFormatId, DataColumnCollection ColumnsList)
        {
            bool MatchFound = false;
            string returnMessage = null;
            //getting Column List from database corresponding to FileFormatId
            var LRCM = new LReconColumnMappingRestClient();
          
            var ReconColumnData = LRCM.GetLReconColumnsByFormatId(CompanyCode, FileFormatId);
            foreach (var xx in ReconColumnData)
            {
                foreach (DataColumn col in ColumnsList)
                {
                    if (col.ColumnName.Equals(xx.Label))
                    {
                        MatchFound = true;
                        break;
                    }
                    else
                    {
                        MatchFound = false;
                    }
                }
                if (!MatchFound)
                {
                    returnMessage = "Sorry, File could not be processed. Uplaoded file Struture does not match with the template.";
                    return returnMessage;
                }
            }
            return returnMessage;
        }


        public static DataTable ReadExcelAndGenerateDataTable(String ExcelFileNameWithPath, String Query)
        {
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ExcelFileNameWithPath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            OleDbConnection con = new System.Data.OleDb.OleDbConnection(connectionString);
            con.Open();

            //Here we are assuming that the first row of the data will automatically be converted into Column Headers as it is the default setting.
            //Need to find the exact property to set this default value explicitly to make it double sure in case somehow defaults are changed automatically

            OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter(Query, con);
            DataSet ds = new DataSet();
            cmd.Fill(ds);
            DataTable dt = ds.Tables[0];
            con.Close();
            return dt;
        }

        public static DataTable ReadCSVAndGenerateDataTable(String path, String filename)
        {
            //Here I am getting Encoding of the received file
            FileStream textfileStream = File.OpenRead(path + "\\" + filename);
            textfileStream.Position = 0;
            byte[] bomBytes = new byte[textfileStream.Length > 4 ? 4 : textfileStream.Length];
            textfileStream.Read(bomBytes, 0, bomBytes.Length);
            Encoding encodingFound = null;
            string EnCodeValue = "";
            encodingFound = DetectBOMBytes(bomBytes);

            //In some cases, It might possible that value of encodingFound variable is null that getting value from this variable is throwing error, so this condition is applied
            if (encodingFound == null)
            {
                EnCodeValue = "";
            }
            else
            {
                EnCodeValue = encodingFound.BodyName;
            }
            textfileStream.Close();

            //We used StreamReader instead of OleDbConnection because OleDb was not supporting Encoding and Error was coming in received file as encoded in UTF8.Therefore to get DataTable we use streamReader and commented OleDbConnection code
            DataTable dt = new DataTable();
            bool headerFlag = true;
            Encoding FileCodeValue = null;

            //StreamReader supports the following UniCode, In future more cases can be added to this condition
            switch (EnCodeValue)
            {
                case "utf-8":
                    FileCodeValue = Encoding.UTF8;
                    break;
                case "utf-7":
                    FileCodeValue = Encoding.UTF7;
                    break;
                case "utf-32":
                    FileCodeValue = Encoding.UTF32;
                    break;
                default:
                    FileCodeValue = Encoding.Default;
                    break;
            }

            //StreamReader sr = new StreamReader(path + "\\" + filename, Encoding.UTF8, true);
            
            using (StreamReader sr = new StreamReader(path + "\\" + filename, FileCodeValue, true))
            {
                String line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    if (headerFlag)
                    {
                        dt.Columns.Add(line);
                        headerFlag = false;
                    }
                    else
                    {
                        dt.Rows.Add(line);
                    }
                    
                }
            }


            //Commented because it was not supporting Encoding or CHARSET parameter
            //FileInfo file = new FileInfo(filename);
            ////using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "\\;Extended Properties = 'text;HDR=Yes;FMT=Delimited(^)'; "))

            //using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + "\\;Extended Properties = 'text;HDR=Yes;FMT=Delimited(^);CHARSET=65001'"))
            //{
            //    using (OleDbCommand cmd = new OleDbCommand(string.Format
            //                              ("SELECT * FROM [{0}]", file.Name), con))
            //    {
            //        con.Open();
            //        using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
            //        {
            //            adp.Fill(dt);
            //        }

            //    }
            //}

            //Following code will return datatable in table form because csv was coming in singla row
            DataTable dt2 = new DataTable();
            var strcolumnNames = dt.Columns;
            var columns = (strcolumnNames[0].ToString()).Split('^');
       
            for (int j = 0; j < columns.Count(); j++)
            {
                dt2.Columns.Add(columns[j]);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                DataRow OriginalRow = dt.Rows[i];
                string OriginalRowdata = OriginalRow.ItemArray[0].ToString();
                string[] OriginalRowdataArray = OriginalRowdata.Split('^');
                DataRow dr = dt2.NewRow();

                for (int c = 0; c < dt2.Columns.Count; c++)
                {
                    // int length = OriginalRowdataArray[c].Length;
                    

                    if (OriginalRowdataArray.Length < c+1)
                    {
                        dr[c] = null;

                    }
                    else
                    {
                        if (OriginalRowdataArray[c].Length > 255)
                        {
                            // int CharToRemove = length - 255;

                            OriginalRowdataArray[c] = OriginalRowdataArray[c].Substring(0, 254);
                        }
                        dr[c] = OriginalRowdataArray[c];
                    }


                }

                dt2.Rows.Add(dr);
            }

            return dt2;
        }

        //This function will return the received file , Encoding
        public static Encoding DetectBOMBytes(byte[] BOMBytes)
        {
            if (BOMBytes == null)
                throw new ArgumentNullException("Must provide a valid BOM byte array!", "BOMBytes");

            if (BOMBytes.Length < 2)
                return null;

            if (BOMBytes[0] == 0xff
                && BOMBytes[1] == 0xfe
                && (BOMBytes.Length < 4
                    || BOMBytes[2] != 0
                    || BOMBytes[3] != 0
                    )
                )
                return Encoding.Unicode;

            if (BOMBytes[0] == 0xfe
                && BOMBytes[1] == 0xff
                )
                return Encoding.BigEndianUnicode;

            if (BOMBytes.Length < 3)
                return null;

            if (BOMBytes[0] == 0xef && BOMBytes[1] == 0xbb && BOMBytes[2] == 0xbf)
                return Encoding.UTF8;

            if (BOMBytes[0] == 0x2b && BOMBytes[1] == 0x2f && BOMBytes[2] == 0x76)
                return Encoding.UTF7;

            if (BOMBytes.Length < 4)
                return null;

            if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe && BOMBytes[2] == 0 && BOMBytes[3] == 0)
                return Encoding.UTF32;

            if (BOMBytes[0] == 0 && BOMBytes[1] == 0 && BOMBytes[2] == 0xfe && BOMBytes[3] == 0xff)
                return Encoding.GetEncoding(12001);

            return null;
        }


        //Method to validate data uploaded using excel file. 
        //Earlier this was a generic method, but later it got converted only to validate reference data.
        //Still kept in this file to see the possibility of still being used as a generic validator

        public static string ReadAndValidateExcelColumnVise(string TableName, string SelecterType)
        {
            /* Read the header (first row of the sheet) and compare it with the required header. If it matches then only upload the file to S3 for further validation 
             * by API, Otherwise throw the error to user saying headers not matching.
            */
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            string path = ConfigurationManager.AppSettings["LocalTempUploadFolder"];
            string filename = System.Web.HttpContext.Current.Session["FileName"].ToString();
            string fullpath = path + "\\" + filename;

            DataTable dt = Globals.ReadExcelAndGenerateDataTable(fullpath, "select * from [SHEET1$]");
            var ColumnsList = dt.Columns;
            string returnMessage = ValidateUploadFileHeaderRow(CompanyCode, TableName, SelecterType, ColumnsList);
            if (returnMessage != null)
            {
                return returnMessage;
            }

            //Reaching here means header is valid, now we can move further to upload the file to S3 for further validation
            string localpath = fullpath;
            string S3BucketReferenceDataFolder = ConfigurationManager.AppSettings["S3BucketReferenceDataFolder"];
            string S3TargetPath = "/" + CompanyCode.ToLower() + "/" + S3BucketReferenceDataFolder + "/" + filename;
            Globals.UploadFileToS3(localpath, S3TargetPath);

            ILReferencesRestClient RRC = new LReferencesRestClient();
            int LoggedInUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"].ToString());
            var Result = RRC.ReadAndValidateExcelData(LoggedInUserId, filename, CompanyCode, SelecterType, null);
            return Result.ToString();//as we are receiving int value from RestClient, so converting it to string as return type is sstring here.

            //List<string> list = new List<string>();
            //DataTable ValidationOK = dt.Clone();
            //DataTable ValidationError = dt.Clone();
            //DataColumn Col = ValidationError.Columns.Add("ValidationMessage", System.Type.GetType("System.String"));
            //Col.SetOrdinal(0);// to put the column in position 0;
            //DataColumn RowNo = ValidationError.Columns.Add("SrNo", typeof(int));
            //Col.SetOrdinal(1);// to put the column in position 0;
            //ReadAndValidateViewModel model = new ReadAndValidateViewModel();
            ////bool MatchFound = false;
            //var LCSRDC = new LCompanySpecificColumnsRestClient();


        }
        #endregion


        public static string SaveLogoImageInSolution(string S3Path)
        {
            try
            {
                //Storing S3 Bucket path in session because we need this path in post method. To save in database
                System.Web.HttpContext.Current.Session["LogoImagePath"] = S3Path;
                string ImagePath = "/Content/temp/";
                var ServerPath = System.Web.HttpContext.Current.Server.MapPath(ImagePath);
                if (!System.IO.Directory.Exists(ServerPath))
                {
                    System.IO.Directory.CreateDirectory(ServerPath);
                }
                var fileArray = S3Path.Split('/');
                int count = fileArray.Count();
                var filename = fileArray[count - 1];
                var ImageFileData = Globals.DownloadFromS3(S3Path);
                System.IO.File.WriteAllBytes(ServerPath + filename, ImageFileData); // Requires System.IO  
                return ImagePath + filename;
            }
            catch (Exception ex)
            {
                return "";
                //throw ex;
            }
        }


    }
    


    //This Attribute is used for restricting special characters in input string 
    public class RestrictSpecialCharAttribute : RegularExpressionAttribute
    {
        static RestrictSpecialCharAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RestrictSpecialCharAttribute), typeof(RegularExpressionAttributeAdapter));
        }
        //Allowing single quotes after JS discussion
        public RestrictSpecialCharAttribute() : base("[^<>]*$") { ErrorMessage = "Special characters are not allowed."; }
    }
    //Thisclass contains the required methods of System.IO.Compression to create zip files
    public static class ZipHelper
    {
        public static string OK { get; private set; }

        public static void ZipFiles(string path, IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                ZipHelper.ZipFilesToStream(fileStream, files, compressionLevel);
            }
        }

        public static byte[] ZipFilesToByteArray(IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            byte[] zipBytes = default(byte[]);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ZipHelper.ZipFilesToStream(memoryStream, files, compressionLevel);
                memoryStream.Flush();
                zipBytes = memoryStream.ToArray();
            }

            return zipBytes;
        }

        private static void ZipFilesToStream(Stream destination,
          IEnumerable<string> files, CompressionOption compressionLevel)
        {
            using (Package package = Package.Open(destination, FileMode.Create))
            {
                foreach (string path in files)
                {
                    // fix for white spaces in file names (by ErrCode)
                    Uri fileUri = PackUriHelper.CreatePartUri(new Uri(@"/" +
                                  Path.GetFileName(path), UriKind.Relative));
                    string contentType = @"data/" + ZipHelper.GetFileExtentionName(path);

                    using (Stream zipStream =
                            package.CreatePart(fileUri, contentType, compressionLevel).GetStream())
                    {
                        using (FileStream fileStream = new FileStream(path, FileMode.Open))
                        {
                            fileStream.CopyTo(zipStream);
                        }
                    }
                }
            }
        }

        private static string GetFileExtentionName(string path)
        {
            string extention = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extention) && extention.StartsWith("."))
            {
                extention = extention.Substring(1);
            }

            return extention;
        }


        


    }


}


