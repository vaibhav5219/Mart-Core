using Mart.EF.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mart.Webapi.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        // GET: api/<CustomersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CustomersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private CartDbContext db = new CartDbContext();

        // GET: api/Customers/5
        [Route("GetCustomer/{id:int}")]
        //[ProducesResponseType(Customer)]
        public async Task<IActionResult> GetCustomer(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//User.Identity.GetUserId();
            //db.Configuration.ProxyCreationEnabled = false;
            Customer customer1 = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null || customer1.UserName != customer.UserName)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [Route("UpdateCustomer/{id:int}")]
        //[ResponseType(typeof(void))]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //db.Configuration.ProxyCreationEnabled = false;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//User.Identity.GetUserId();
            Customer customer1 = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != customer.CustomerId || customer.UserName != customer1.UserName)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NotFound(); // StatusCode(HttpStatusCode.NoContent);
        }

        //POST: api/Customers
        //[ResponseType(typeof(Customer))]
        public async Task<IActionResult> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        //[ResponseType(typeof(Customer))]
        //public async Task<IHttpActionResult> DeleteCustomer(int id)
        //{
        //    Customer customer = await db.Customers.FindAsync(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Customers.Remove(customer);
        //    await db.SaveChangesAsync();

        //    return Ok(customer);
        //}

        
        [Route("IsCustomerExists")]
        private bool CustomerExists(int id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}

