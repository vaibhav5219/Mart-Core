using Core.EF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private CartDbcoreContext db = null;

        // Constructor Injection
        public AddressController(CartDbcoreContext _cartDbcoreContext)
        {
            db = _cartDbcoreContext;
        }

        // GET: api/Addresses
        //public IQueryable<Address> GetAddresses()
        //{
        //    return db.Addresses;
        //}

        // GET: api/Addresses/5
        [Route("GetAddress/{id:int}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            Address address = await db.Addresses.FindAsync(id);

            if (address == null || address.CustomerUserName != customer.UserName)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // PUT: api/Addresses/5
        [Route("UpdateAddress/{id:int}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != address.AddressId || address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }

            db.Entry(address).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        // POST: api/Addresses
        [Route("PostAddress/{id:int}")]
        public async Task<IActionResult> PostAddress(Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }

            db.Addresses.Add(address);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        [Route("RemoveAddress/{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (id != address.AddressId || address.CustomerUserName != customer.AspNetUserId)
            {
                return BadRequest();
            }


            db.Addresses.Remove(address);
            await db.SaveChangesAsync();

            return Ok(address);
        }

        private bool AddressExists(int id)
        {
            return db.Addresses.Count(e => e.AddressId == id) > 0;
        }
    }
}
