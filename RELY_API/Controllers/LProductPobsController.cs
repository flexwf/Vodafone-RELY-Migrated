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
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;

namespace RELY_API.Controllers
{

    [CustomExceptionFilter]
    public class LProductPobsController : ApiController
    {
        private RELYDevDbEntities db = new RELYDevDbEntities();
        // GET: api/LProductPobs
        [ResponseType(typeof(LProductPob))]
        public IHttpActionResult GetById(int Id)
        {
            var LProductPob = (from aa in db.LProductPobs
                             where aa.Id == Id
                             select new {
                          aa.Id,aa.ProductId,aa.PobCatalogueId,
                          aa.AttributeC01,aa.AttributeC02,aa.AttributeC03,aa.AttributeC04,aa.AttributeC05,aa.AttributeC06,aa.AttributeC07,aa.AttributeC08,aa.AttributeC09,aa.AttributeC10,
                          aa.AttributeC11,aa.AttributeC12,aa.AttributeC13,aa.AttributeC14,aa.AttributeC15,aa.AttributeC16,aa.AttributeC17,aa.AttributeC18,aa.AttributeC19,aa.AttributeC20,
                          aa.AttributeB01,aa.AttributeB02,aa.AttributeB03,aa.AttributeB04,aa.AttributeB05,aa.AttributeB06,aa.AttributeB07,aa.AttributeB08,aa.AttributeB09,aa.AttributeB10,
                          aa.AttributeD01,aa.AttributeD02,aa.AttributeD03,aa.AttributeD04,aa.AttributeD05,aa.AttributeD06,aa.AttributeD07,aa.AttributeD08,aa.AttributeD09,aa.AttributeD10,
                          aa.AttributeI01,aa.AttributeI02,aa.AttributeI03,aa.AttributeI04,aa.AttributeI05,aa.AttributeI06,aa.AttributeI07,aa.AttributeI08,aa.AttributeI09,aa.AttributeI10,
                          aa.AttributeN01,aa.AttributeN02,aa.AttributeN03,aa.AttributeN04,aa.AttributeN05,aa.AttributeN06,aa.AttributeN07,aa.AttributeN08,aa.AttributeN09,aa.AttributeN10
                      }).FirstOrDefault();

            return Ok(LProductPob);
        }
        // GET: api/LProductPobs
        [ResponseType(typeof(LProductPob))]
        public IHttpActionResult GetByProductId(int ProductId)
        {
            var LProductPob = (from aa in db.LProductPobs
                               where aa.Id == ProductId
                               select new
                               {
                                   aa.Id,
                                   aa.ProductId,
                                   aa.PobCatalogueId,
                                   aa.AttributeC01,
                                   aa.AttributeC02,
                                   aa.AttributeC03,
                                   aa.AttributeC04,
                                   aa.AttributeC05,
                                   aa.AttributeC06,
                                   aa.AttributeC07,
                                   aa.AttributeC08,
                                   aa.AttributeC09,
                                   aa.AttributeC10,
                                   aa.AttributeC11,
                                   aa.AttributeC12,
                                   aa.AttributeC13,
                                   aa.AttributeC14,
                                   aa.AttributeC15,
                                   aa.AttributeC16,
                                   aa.AttributeC17,
                                   aa.AttributeC18,
                                   aa.AttributeC19,
                                   aa.AttributeC20,
                                   aa.AttributeB01,
                                   aa.AttributeB02,
                                   aa.AttributeB03,
                                   aa.AttributeB04,
                                   aa.AttributeB05,
                                   aa.AttributeB06,
                                   aa.AttributeB07,
                                   aa.AttributeB08,
                                   aa.AttributeB09,
                                   aa.AttributeB10,
                                   aa.AttributeD01,
                                   aa.AttributeD02,
                                   aa.AttributeD03,
                                   aa.AttributeD04,
                                   aa.AttributeD05,
                                   aa.AttributeD06,
                                   aa.AttributeD07,
                                   aa.AttributeD08,
                                   aa.AttributeD09,
                                   aa.AttributeD10,
                                   aa.AttributeI01,
                                   aa.AttributeI02,
                                   aa.AttributeI03,
                                   aa.AttributeI04,
                                   aa.AttributeI05,
                                   aa.AttributeI06,
                                   aa.AttributeI07,
                                   aa.AttributeI08,
                                   aa.AttributeI09,
                                   aa.AttributeI10,
                                   aa.AttributeN01,
                                   aa.AttributeN02,
                                   aa.AttributeN03,
                                   aa.AttributeN04,
                                   aa.AttributeN05,
                                   aa.AttributeN06,
                                   aa.AttributeN07,
                                   aa.AttributeN08,
                                   aa.AttributeN09,
                                   aa.AttributeN10
                               }).FirstOrDefault();

            return Ok(LProductPob);
        }

        [ResponseType(typeof(LProductPob))]
        public IHttpActionResult GetByProductIdForProductGrid(int ProductId)
        {
          
// string SqlQuery = "select aa.EffectiveStartDate,aa.EffectiveEndDate,aa.Id, aa.ProductId,LLocalPOB = lp.Name,aa.LocalPobId,POB1 = gp1.Name +'(' +gp1.Type + ' ' + gp1.Category + ' ' + gp1.IFRS15Account + ')',Type = gp1.Type,Retention =gp2.Name +'(' +gp2.Type + ' ' + gp2.Category + ' ' + gp2.IFRS15Account + ')',Type1 = gp2.Type,COPA2 = gc1.CopaValue + ' '  +gc1.Description + ' (' + gc1.Dimension + ')' ,COPA5 = gc2.CopaValue + ' '  +gc2.Description + ' (' + gc2.Dimension + ')',COPA22 = gc3.CopaValue + ' ' + gc3.Description + ' (' + gc3.Dimension + ')', "
//+ " COPA52 = gc4.CopaValue + ' ' + gc4.Description + ' (' + gc4.Dimension + ')',aa.AttributeC01,aa.AttributeC02,aa.AttributeC03,aa.AttributeC04,aa.AttributeC05,aa.AttributeC06,aa.AttributeC07,aa.AttributeC08,aa.AttributeC09,aa.AttributeC10,aa.AttributeC11," +
//"aa.AttributeC12,aa.AttributeC13,aa.AttributeC14,aa.AttributeC15,aa.AttributeC16,aa.AttributeC17,aa.AttributeC18,aa.AttributeC19,aa.AttributeC20,aa.AttributeB01,aa.AttributeB02,aa.AttributeB03,aa.AttributeB04,aa.AttributeB05,aa.AttributeB06,aa.AttributeB07," +
//"aa.AttributeB08,aa.AttributeB09,aa.AttributeB10,aa.AttributeD01,aa.AttributeD02,aa.AttributeD03,aa.AttributeD04,aa.AttributeD05,aa.AttributeD06,aa.AttributeD07,aa.AttributeD08,aa.AttributeD09,aa.AttributeD10,aa.AttributeI01, aa.AttributeI02,aa.AttributeI03,aa.AttributeI04,aa.AttributeI05,aa.AttributeI06,aa.AttributeI07,aa.AttributeI08,aa.AttributeI09,aa.AttributeI10,aa.AttributeN01,aa.AttributeN02,aa.AttributeN03,aa.AttributeN04,aa.AttributeN05,aa.AttributeN06,aa.AttributeN07,aa.AttributeN08,aa.AttributeN09,aa.AttributeN10 " +
//"from LProductPobs aa left outer join LLocalPobs lp on aa.LocalPobId = lp.Id "
//+ " left outer join GGlobalPobs gp1 on lp.GlobalPobId1 = gp1.Id  left outer join GGlobalPobs gp2 on lp.GlobalPobId2 = gp2.Id left outer join GCopaDimensions gc1 on lp.CopaId1 = gc1.Id left outer join GCopaDimensions gc2 on lp.CopaId2 = gc2.Id "
//+ " left outer join GCopaDimensions gc3 on lp.CopaId3 = gc3.Id left outer join GCopaDimensions gc4 on lp.CopaId4 = gc4.Id where aa.ProductId = {0}";
            string qry = "select aa.EffectiveStartDate,aa.EffectiveEndDate,aa.Id, aa.ProductId,LLocalPOB = lp.Name,aa.PobCatalogueId,LocalPobId = lp.Id, "
+ " aa.AttributeC01,aa.AttributeC02,aa.AttributeC03,aa.AttributeC04,aa.AttributeC05,aa.AttributeC06,aa.AttributeC07,aa.AttributeC08,aa.AttributeC09,aa.AttributeC10, "
+ " aa.AttributeC11,aa.AttributeC12,aa.AttributeC13,aa.AttributeC14,aa.AttributeC15,aa.AttributeC16,aa.AttributeC17,aa.AttributeC18,aa.AttributeC19,aa.AttributeC20, "
+ " aa.AttributeB01,aa.AttributeB02,aa.AttributeB03,aa.AttributeB04,aa.AttributeB05,aa.AttributeB06,aa.AttributeB07,aa.AttributeB08,aa.AttributeB09,aa.AttributeB10,"
+" aa.AttributeD01,aa.AttributeD02,aa.AttributeD03,aa.AttributeD04,aa.AttributeD05,aa.AttributeD06,aa.AttributeD07,aa.AttributeD08,aa.AttributeD09,aa.AttributeD10,"
+ " aa.AttributeI01, aa.AttributeI02,aa.AttributeI03,aa.AttributeI04,aa.AttributeI05,aa.AttributeI06,aa.AttributeI07,aa.AttributeI08,aa.AttributeI09,aa.AttributeI10,"
+" aa.AttributeN01,aa.AttributeN02,aa.AttributeN03,aa.AttributeN04,aa.AttributeN05,aa.AttributeN06,aa.AttributeN07,aa.AttributeN08,aa.AttributeN09,aa.AttributeN10"
 + " from LProductPobs aa left outer join LLocalPobs lp on aa.PobCatalogueId = lp.PobCatalogueId "
                + " where aa.ProductId = {0}  and lp.WFStatus in ('Completed','Parked')";
            var LProductPob = db.Database.SqlQuery<ProductObligationsViewModel>(qry, ProductId).ToList();

            return Ok(LProductPob);
        }


        [ResponseType(typeof(LProductPob))]
        public async Task<IHttpActionResult> PostLProductPob(List<LProductPob> LProductPob, string UserName, string WorkFlow,string TypeOfProduct)
        {
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "POB")));
            //}

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int ProductId = LProductPob[0].ProductId;
                    //delete existing Pobs when Type of Product is Marker SOC or Discount SOC
                    if (("Discount SOC".Equals(TypeOfProduct)) || ("Marker SOC".Equals(TypeOfProduct)))
                    {
                       var lstObligations =  db.LProductPobs.Where(a => a.ProductId == ProductId).ToList();
                        db.LProductPobs.RemoveRange(lstObligations);
                        db.SaveChanges();
                    }
                    foreach (var model in LProductPob)
                    {
                        if (model.Id == 0)//add only when new record is there to insert.
                        {               //insert date into LProduct table
                            db.LProductPobs.Add(model);

                            var product = db.LProducts.Find(model.ProductId);
                            if (product.SspId == null)
                            {
                                //we need to default it with LocalPob SSPID if Local Pob has SSPID
                                var LocalPob = db.LLocalPobs.Where(p => p.PobCatalogueId == model.PobCatalogueId).FirstOrDefault();
                                if (LocalPob.SspId != null)
                                {
                                    product.SspId = LocalPob.SspId;
                                    db.Entry(product).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            db.Entry(model).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                    db.SaveChanges();

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

                transaction.Commit();
            }

            // return CreatedAtRoute("DefaultApi", new { id = LProductPob.Id }, LProductPob);
            return Ok();
        }
        [HttpGet]
        // DELETE: api/LLocalPOBs/5
        [ResponseType(typeof(LProductPob))]
        public async Task<IHttpActionResult> DeleteLProductPob(int id)
        {
            LProductPob LProductPob = await db.LProductPobs.FindAsync(id);
            if (LProductPob == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "POB")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int PobCatalogueId = LProductPob.PobCatalogueId;
                    db.LProductPobs.Remove(LProductPob);
                    await db.SaveChangesAsync();
                        var lpob = db.LLocalPobs.Where(a => a.PobCatalogueId == PobCatalogueId).FirstOrDefault();
                        //remove the OnFly Pob when Product Obligation is deleted
                        if (lpob.WFStatus.Equals("Parked") && lpob.WFOrdinal == 0)
                        {
                            db.LLocalPobs.Remove(lpob);
                            await db.SaveChangesAsync();
                        }
                    transaction.Commit();
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
                        throw ex;
                    }
                }
                
            }
            return Ok(LProductPob);
        }

        private bool LProductPobExists(int id)
        {
            return db.LProductPobs.Count(e => e.Id == id) > 0;
        }


        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FK_LLocalPobs_LProductPobs_LocalPobId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PRODUCTS POB", "LOCALPOB"));
            
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



}
