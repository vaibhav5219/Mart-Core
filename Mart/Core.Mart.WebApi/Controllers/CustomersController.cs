using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Core.EF.Models;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/Customers
        //[Route("GetCustomers")]
        //public IQueryable<Customer> GetCustomers()
        //{
        //    return db.Customers;
        //}

        // GET: api/Customers/5
        [Route("{id:int}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer1 = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null || customer1.UserName != customer.UserName)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [Route("{id:int}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        //POST: api/Customers
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
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}

