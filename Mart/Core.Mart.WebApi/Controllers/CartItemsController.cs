using Core.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/CartItems
        [HttpGet]
        [Route("GetCartItems")]
        public IQueryable<CartItem> GetCartItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.CartItems.Where(u => u.ShopCode == shopDetail.ShopCode);
        }

        // GET: api/CartItems/5
        [HttpGet]
        [Route("GetOrders/{id}")]
        [Authorize(Roles = "IsACustomer")]
        public async Task<IActionResult> GetCartItem(string id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (cartItem == null || shopDetail.ShopCode != cartItem.ShopCode)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }

        // PUT: api/CartItems/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutCartItem(string id, CartItem cartItem)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != cartItem.Cart_Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(cartItem).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CartItemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/CartItems  =>  Add to cart
        //[Authorize(Roles = "IsACustomer")]
        [Route("AddToCart")]
        public async Task<IActionResult> PostCartItem(CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(c => c.AspNetUserId == userId);

            var cartItemExists = db.CartItems.SingleOrDefault(
               c => c.CartId == cartItem.CartId
               && c.ProductId == cartItem.ProductId);

            if (cartItemExists == null)
            {
                // Create a new cart item if no cart item exists.                 
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = cartItem.ProductId,
                    //Product = db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now,
                    ShopCode = cartItem.ShopCode
                };

                db.CartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity++;
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartItemExists(cartItem.CartId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cartItem.CartId }, cartItem);
        }

        // DELETE: api/CartItems/5  => Remove Cart Item
        [Authorize(Roles = "IsACustomer")]
        [Route("RemoveCartItem/{id:int}")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            CartItem cartItem = await db.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(c => c.AspNetUserId == userId);

            var cartItemExists = db.CartItems.SingleOrDefault(
               c => c.CartId == cartItem.CartId
               && c.ProductId == cartItem.ProductId);

            if (cartItemExists != null && cartItem.Quantity > 0)
            {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity--;
            }

            db.CartItems.Remove(cartItem);
            await db.SaveChangesAsync();

            return Ok(cartItem);
        }

        [Route("IsCartItemExists")]
        private bool CartItemExists(string id)
        {
            return db.CartItems.Count(e => e.CartId == id) > 0;
        }
    }
}
