using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Core.EF.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Core.Mart.WebApi.ModelView;
using Microsoft.AspNetCore.Mvc.Abstractions;

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
                return Ok(customer);
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

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        //POST: api/Customers
        [Authorize(Roles = UserRoles.IsACustomer)]
        [Route("CreateCustomer")]
        public async Task<IActionResult> PostCustomer(CustomerModel customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var UserObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == userId);
            
            if(UserObj == null || UserObj.Email == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Customer IsCustomerExist = db.Customers.FirstOrDefault(a=> a.AspNetUserId == userId);
            if(IsCustomerExist != null)
            {
                return Ok(IsCustomerExist);
            }

            Customer customer1 = new Customer()
            {
                CustomerName = customer.CustomerName,
                Mobile = customer.Mobile,
                Email = UserObj.Email,
                UserName = customer.UserName,
                Address = customer.Address,
                AspNetUserId = UserObj.Id,
            };

            db.Customers.Add(customer1);

            try
            {
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
                    return StatusCode(StatusCodes.Status500InternalServerError);    //throw;
                }
            }
            return Ok(customer1);
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

        [Authorize(Roles = UserRoles.IsACustomer)]
        [Route("IsCustomerExists")]
        [HttpGet]
        public IActionResult CustomerExists()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var UserObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == username);

                if (UserObj == null || UserObj.Email == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                var customer = db.Customers.FirstOrDefault(a => a.AspNetUserId == UserObj.Id);
                return Ok(new
                {
                    IsCustomerRegistered = customer != null ? true : false
                });
            }
            catch(DbUpdateException) 
            {
                return BadRequest();
            }
        }
    }
}

