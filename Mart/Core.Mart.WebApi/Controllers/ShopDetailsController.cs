using Core.EF.Models;
using Core.Mart.WebApi.ModelView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
                if (true)
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
        [HttpPost]
        [Route("CreateShop")]
        [Authorize(Roles = UserRoles.IsAShop)]
        public async Task<IActionResult> PostShopDetail(ShopDetailsModel shopDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var UserObj = await db.AspNetUsers.FirstOrDefaultAsync(a => a.UserName == username);

                if (UserObj == null || UserObj.Email == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                ShopDetail shopDetail1 = new ShopDetail()
                {
                    ShopId = Guid.NewGuid().ToString(),
                    ShopCode = shopDetail.ShopCode,
                    ShopName = shopDetail.ShopName,
                    ShopDomainName = shopDetail.ShopDomainName,
                    ShopKeeperName = shopDetail.ShopKeeperName,
                    Mobile = shopDetail.Mobile,
                    Address = shopDetail.Address,
                    PinCode = shopDetail.PinCode,
                    AspNetUsersId = UserObj.Id
                };

                db.ShopDetails.Add(shopDetail1);

                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (true)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new Response() 
            {
                Status = "200",
                Message="Shop has been successfully created"
            });
            //return CreatedAtRoute("DefaultApi", new { id = shopDetail.ShopId }, shopDetail);
        }

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
        //[Route("IsShopExists")]
        //private bool ShopDetailExists(string id)
        //{
        //    return db.ShopDetails.Count(e => e.ShopId == id) > 0;
        //}

        [Authorize(Roles = UserRoles.IsAShop)]
        [Route("IsShopExists")]
        [HttpGet]
        public async Task<IActionResult> ShopDetailExists()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserObj = await db.AspNetUsers.FirstOrDefaultAsync(a => a.UserName == username);

            if (UserObj == null || UserObj.Email == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var customer = db.ShopDetails.FirstOrDefault(a => a.AspNetUsersId == UserObj.Id);
            return Ok(new
            {
                IsShopRegistered = customer != null ? true : false
            });
        }
    }
}
