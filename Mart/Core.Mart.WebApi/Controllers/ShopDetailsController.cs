using Core.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopDetailsController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/ShopDetails
        //public IQueryable<ShopDetail> GetShopDetails()
        //{
        //    return db.ShopDetails;
        //}

        // GET: api/ShopDetails/5
        [Route("GetShopDetails/{id}")]
        public async Task<IActionResult> GetShopDetail(string id)
        {
            ShopDetail shopDetail = await db.ShopDetails.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail1 = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);


            if (shopDetail == null || shopDetail.ShopCode != shopDetail1.ShopCode)
            {
                return NotFound();
            }

            return Ok(shopDetail);
        }

        // PUT: api/ShopDetails/5
        [Route("UpdateShopDetails/{id}")]
        public async Task<IActionResult> PutShopDetail(string id, ShopDetail shopDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ShopDetail shopDetail1 = await db.ShopDetails.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail2 = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != shopDetail.ShopId || shopDetail1.ShopCode != shopDetail2.ShopCode)
            {
                return BadRequest();
            }

            db.Entry(shopDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // POST: api/ShopDetails
        //[ResponseType(typeof(ShopDetail))]
        //public async Task<IHttpActionResult> PostShopDetail(ShopDetail shopDetail)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.ShopDetails.Add(shopDetail);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (ShopDetailExists(shopDetail.Shop_Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = shopDetail.Shop_Id }, shopDetail);
        //}

        // DELETE: api/ShopDetails/5
        //[ResponseType(typeof(ShopDetail))]
        //public async Task<IHttpActionResult> DeleteShopDetail(string id)
        //{
        //    ShopDetail shopDetail = await db.ShopDetails.FindAsync(id);
        //    if (shopDetail == null)
        //    {
        //        return NotFound();
        //    }

        //    db.ShopDetails.Remove(shopDetail);
        //    await db.SaveChangesAsync();

        //    return Ok(shopDetail);
        //}
        [Route("IsShopExists")]
        private bool ShopDetailExists(string id)
        {
            return db.ShopDetails.Count(e => e.ShopId == id) > 0;
        }
    }
}
