using RELY_API.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSAccountingMemoGeneratorController : ApiController
    {
        [HttpGet]
        public IHttpActionResult DownloadAccountingMemo(string EntityType,int EntityId,int SurveyId,int UserId, string UserName, string WorkFlow)
        {
            //Generate AccountingMemo
           
            string strGenerateQuery = "Exec SpGenerateAccountingMemo @SurveyId,@EntityId,@EntityType,@UserId";
            SqlCommand cmdGenerate = new SqlCommand(strGenerateQuery);
            cmdGenerate.Parameters.AddWithValue("@SurveyId", SurveyId);
            cmdGenerate.Parameters.AddWithValue("@EntityId", EntityId);
            cmdGenerate.Parameters.AddWithValue("@EntityType", EntityType);
            cmdGenerate.Parameters.AddWithValue("@UserId", UserId);
            Globals.GetDataTableUsingADO(cmdGenerate);

            //Download Accounting Memo
            string strQuery = "Exec SpDownloadAccountingMemo @EntityType,@EntityId,@SurveyId,@UserId";
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.Parameters.AddWithValue("@EntityType", EntityType);
            cmd.Parameters.AddWithValue("@EntityId", EntityId);
            cmd.Parameters.AddWithValue("@SurveyId", SurveyId);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmd);
            return Ok(dt);
        }

        [HttpGet]
        public IHttpActionResult DownloadAccountingScenarioMatrix(int EntityId, string EntityType, string CompanyCode, string UserName, string WorkFlow)
        {
            //Generate Accounting Scenario Matrix
            string strGenerateQuery = "Exec SpGenerateAccountingScenarioMatrix @EntityId,@EntityType,@CompanyCode";
            SqlCommand cmdGenerate = new SqlCommand(strGenerateQuery);
            cmdGenerate.Parameters.AddWithValue("@EntityId", EntityId);
            cmdGenerate.Parameters.AddWithValue("@EntityType", EntityType);
            cmdGenerate.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            Globals.GetDataTableUsingADO(cmdGenerate);

            //Download Accounting Scenario Matrix
            string strDownloadQuery = "Exec SpDownloadAccountingScenarioMatrix @EntityId,@EntityType";
            SqlCommand cmdDownload = new SqlCommand(strDownloadQuery);
            cmdDownload.Parameters.AddWithValue("@EntityId", EntityId);
            cmdDownload.Parameters.AddWithValue("@EntityType", EntityType);
            var dt = new DataTable();
            dt = Globals.GetDataTableUsingADO(cmdDownload);
            return Ok(dt);
        }


    }
}
