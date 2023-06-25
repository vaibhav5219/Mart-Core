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
    public class CategoriesController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/Categories
        [HttpGet]
        [Route("GetCategories")]
        public IQueryable<Category> GetCategories()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Categories.Where(u => u.ShopCode == shopDetail.ShopCode);
        }

        // GET: api/Categories/5
        [HttpGet]
        [Route("GetCategories/{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            Category category = await db.Categories.SingleAsync(u => u.ShopCode == shopDetail.ShopCode && u.CategoryId == id);
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
        public async Task<IActionResult> PostCategory(SetCategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Categories.Add(category);
            //await db.SaveChangesAsync();

            using (CartDbcoreContext enteties = new CartDbcoreContext())
            {
                try
                {
                    //enteties.Configuration.ProxyCreationEnabled = false;
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ShopDetail shopDetail = enteties.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

                    Category category = new Category()
                    {
                        CategoryName = categoryModel.CategoryName,
                        Description = categoryModel.Description,
                        ShopCode = shopDetail.ShopCode
                    };

                    enteties.Categories.Add(category);
                    await enteties.SaveChangesAsync();

                    return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, category);
                    //return Ok();
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
