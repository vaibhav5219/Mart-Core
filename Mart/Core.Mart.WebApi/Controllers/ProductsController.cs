using Core.EF.Models;
using Core.Mart.WebApi.ModelView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/Products
        // GET api/Products/GetProducts
        [HttpGet]
        [Route("GetProducts")]
        public IQueryable<Product> GetProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Products.Where(u => u.ShopCode == shopDetail.ShopCode);
        }

        // GET: api/Products/5
        [Route("GetProducts/{id:int}")]
        public async Task<IActionResult> GetProduct(int id, string Shop_Code)
        {
            Product product = await db.Products.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (product == null || shopDetail.ShopCode != Shop_Code)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [Route("UpdateProducts/{id:int}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != product.ProductId || shopDetail.ShopCode != product.ShopCode)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST:  api/Products/SetProduct
        [HttpPost]
        [Route("SetProduct")]
        public async Task<IActionResult> PostProduct(SetProductViewModel setProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Products.Add(product);
            //await db.SaveChangesAsync();
            //return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);

            using (CartDbcoreContext enteties = new CartDbcoreContext())
            {
                try
                {
                    //enteties.Configuration.ProxyCreationEnabled = false;
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);
                    Product product = new Product()
                    {
                        ProductName = setProductViewModel.Product.ProductName,
                        Description = setProductViewModel.Product.Description,
                        ImagePath = setProductViewModel.Product.ImagePath,      // Need to more work on it
                        UnitPrice = setProductViewModel.Product.UnitPrice,
                        //ShopCode = setProductViewModel.ShopCode,
                        ShopCode = shopDetail.ShopCode
                    };

                    enteties.Products.Add(product);
                    await enteties.SaveChangesAsync();

                    return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
                    //return Ok();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // DELETE: api/Products/5
        [Route("RemoveProducts/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (product == null || shopDetail.ShopCode != product.ShopCode)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        [Route("IsProductExists")]
        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}
