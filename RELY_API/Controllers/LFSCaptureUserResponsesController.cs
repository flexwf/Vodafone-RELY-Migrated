using RELY_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class LFSCaptureUserResponsesController : ApiController
    {
       private RELYDevDbEntities db=new RELYDevDbEntities();
      public IHttpActionResult GetSurveyItems(int SurveyId,string ChapterCode,string SectionCode,string CompanyCode)
        {
            /*Notes: While loading this screen SurveyId, ChapterCode, SectionCode will be passed from backend to identify that which section items are to be displayed here.
            2. Order By Ordinal of SectionItems table*/
            var SectionItems = from aa in db.LFSSectionItems.Where(p => p.SectionCode == SectionCode).Where(p => p.ChapterCode == ChapterCode).Where(p => p.SurveyId == SurveyId)
                               join bb in db.LFSQuestionBanks.Where(p=>p.CompanyCode==CompanyCode) on aa.ItemTypeId equals bb.ItemTypeId
                               select new {aa.Id,aa.ItemText,aa.Ordinal,aa.LFSItemType.IsQuestion,aa.LFSItemType.Name };
            return Ok(SectionItems);
        }
    }
}