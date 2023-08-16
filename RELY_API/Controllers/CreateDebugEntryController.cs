using RELY_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RELY_API.Utilities;

namespace RELY_API.Controllers
{
    public class CreateDebugEntryController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        // Get Error Message
        [HttpGet]
        public IHttpActionResult CreateDebugEntry(string Message)
        {
            db.Database.ExecuteSqlCommand("insert into Debug(DebugValue,DebugDateTime)values({0},'" + DateTime.Now.ToString() + "')",Message);
            return Ok();
        }
    }
}
