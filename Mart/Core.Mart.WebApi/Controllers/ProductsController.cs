﻿using Core.EF.Models;
using Core.Mart.WebApi.ModelView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Propertie
        private CartDbcoreContext db = null;

        // Constructor Injection
        public ProductsController(CartDbcoreContext _cartDbcoreContext)
        {
            db = _cartDbcoreContext;
        }

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
        [Authorize(Roles = UserRoles.IsAShop)]
        public async Task<IActionResult> PostProduct(SetProductViewModel setProductViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (CartDbcoreContext enteties = new CartDbcoreContext())
            {
                try
                {
                    var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userObj = enteties.AspNetUsers.FirstOrDefault(u => u.Email == userName);
                    if (userObj == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });
                    }

                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userObj.Id);

                    if(shopDetail == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Shop does not exists!" });
                    }
                    int? categoryId = db.Categories.FirstOrDefault(c => c.CategoryName == setProductViewModel.CategoryName).CategoryId;
                    if (categoryId == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Category does not exists!" });
                    }

                    Product product = new Product()
                    {
                        ProductName = setProductViewModel.ProductName,
                        Description = setProductViewModel.Description,
                        ImagePath = setProductViewModel.ImagePath,      // Need to more work on it
                        UnitPrice = setProductViewModel.UnitPrice,
                        ShopCode = shopDetail.ShopCode,
                        CategoryId = categoryId
                    };

                    enteties.Products.Add(product);
                    await enteties.SaveChangesAsync();

                    //return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
                    return Ok();
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

        [HttpGet]
        [Route("GetProductsList")]
        public async Task<IActionResult> GetProductsList()
        {
            List<Product> products = await db.Products.Include(a => a.Category).ToListAsync();

            List<GetProductsModelView> getProductsModelView = new List<GetProductsModelView>();

            foreach(var product in products)
            {
                GetProductsModelView getProductsModelView1 = new GetProductsModelView()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    ImagePath = product.ImagePath,
                    UnitPrice = Convert.ToSingle(product.UnitPrice),
                    CategoryID = product.CategoryId,
                    ShopCode = product.ShopCode == null ? "" : product.ShopCode
                };

                getProductsModelView.Add(getProductsModelView1);
            }

            return Ok(getProductsModelView);
        }
    }
}
