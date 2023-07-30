using Core.EF.Models;
using Core.Mart.WebApi.ModelView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/Categories
        [HttpGet]
        [Route("GetCategories")]
        public string GetCategories()
        {
            try
            {
                    var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userObj = db.AspNetUsers.FirstOrDefault(u => u.Email == userName);
                    if (userObj == null)
                    {
                        IQueryable<Category> categories1 = null; //new List<Category>();
                        return null;
                    }
                    ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userObj.Id);
                    if (shopDetail == null)
                    {
                        IQueryable<Category> categories2 = null; // new IQueryable<Category>();
                        return null;
                    }
                    IQueryable<Category> categories =  db.Categories.Where(u => u.ShopCode == shopDetail.ShopCode);
                    
                    List<string> categoriesName = categories.Select(u => u.CategoryName).ToList();
                string res = string.Empty;
                foreach (var cat in categoriesName)
                {
                    res = res + cat + ',';
                }    
                return res;
            }
            catch (Exception ex)
            {
                return null; //StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Shop already exists!" }); ;
            }
        }

        // GET: api/Categories/5
        [HttpGet]
        [Route("GetCategories/{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            Category category = null;// await db.Categories.SingleAsync(u => u.ShopCode == shopDetail.ShopCode && u.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut]
        [Route("EditCategories/{id:int}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (id != category.CategoryId || shopDetail.ShopCode != category.ShopCode)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        [Route("SetCategory")]
        [Authorize(Roles = UserRoles.IsAShop)]
        public async Task<IActionResult> PostCategory(SetCategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (CartDbcoreContext enteties = new CartDbcoreContext())
            {
                try
                {
                    //enteties.Configuration.ProxyCreationEnabled = false;
                    var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userObj = enteties.AspNetUsers.FirstOrDefault(u => u.Email == userName);
                    if (userObj == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User doesn't exists!" });
                    }
                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userObj.Id);
                    if (shopDetail == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Shop already exists!" });
                    }

                    Category category = new Category()
                    {
                        CategoryName = categoryModel.CategoryName,
                        Description = categoryModel.Description,
                        ShopCode = shopDetail.ShopCode
                    };

                    enteties.Categories.Add(category);
                    await enteties.SaveChangesAsync();

                    //return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, category);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // DELETE: api/Categories/5
        [Route("RemoveCategory/{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = await db.Categories.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (category == null || shopDetail.ShopCode != category.ShopCode)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync();

            return Ok(category);
        }

        [Route("IsCategoryExists")]
        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CategoryId == id) > 0;
        }
    }
}
