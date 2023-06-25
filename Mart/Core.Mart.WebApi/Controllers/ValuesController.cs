using Core.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IEnumerable<Category> Get()
        {
            using (CartDbcoreContext enteties = new CartDbcoreContext())
            {
                try
                {
                    List<Category> categories = enteties.Categories.ToList();
                    return categories;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        //public HttpResponseMessage Get(int id)
        //{
        //    using (CartDbcoreContext ctDB = new CartDbcoreContext())
        //    {
        //        // List<Categories> categories = ctDB.Categories.ToList();
        //        var entity = ctDB.Categories.FirstOrDefault(c => c.CategoryId == id);

        //        if (entity != null)
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.OK, entity);
        //        }
        //        //return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Category with id : " + id.ToString() + " not fount");
        //    }
        //}

        // POST api/values
        //public HttpResponseMessage Post([FromBody] Category category)
        //{
        //    try
        //    {
        //        using (CartDbcoreContext ctDB = new CartDbcoreContext())
        //        {
        //            // List<Categories> categories = ctDB.Categories.ToList();
        //            ctDB.Categories.Add(category);
        //            ctDB.SaveChanges();

        //            //var message = Request.CreateResponse(HttpStatusCode.Created, category);

        //            //message.Headers.Location = new Uri(Request.RequestUri + category.CategoryID.ToString());
        //            //return message;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return Request.HttpContext(HttpStatusCode.BadRequest, ex);
        //    }
        //}

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
