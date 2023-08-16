using RELY_API.Models;
using System.Linq;
using System.Web.Http;

namespace RELY_API.Controllers
{
    [CustomExceptionFilter]
    public class UseCaseIndicatorController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();

        public IHttpActionResult GetAll()
        {
            var GlobalPob = (from aa in db.UseCaseIndicators
                             select new
                             {
                                 aa.Id,
                                 aa.Indicatior,
                                 aa.Description,
                                 aa.EffectiveStartDate,
                                 aa.EffectiveEndDate
                             }).OrderBy(P => P.Id).ToList();

            return Ok(GlobalPob);
        }
        
    }
}
