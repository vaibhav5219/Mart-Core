using Core.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace Core.Mart.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private CartDbcoreContext db = new CartDbcoreContext();

        // GET: api/Orders
        [HttpGet]
        [Route("GetOrders")]
        [Authorize(Roles = "IsAShop")]
        public IQueryable<Order> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            return db.Orders.Where(u => u.ShopCode == shopDetail.ShopCode
                                   && u.OrderStatus != 5
                                   && u.OrderStatus != 4
                                   && u.OrderStatus != 3);
        }

        // GET: api/Orders/5
        [HttpGet]
        [Route("FetchOrders/{id:int}")]
        [Authorize(Roles = "IsAShop")]
        public async Task<IActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == userId);

            if (order == null || order.ShopCode != shopDetail.ShopCode)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        //[Route("UpdateOrder/{id}")]
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutOrder(int id, Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != order.Order_Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrderExists(id))
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

        // POST: api/Orders  
        [Authorize(Roles = "IsACustomer")]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PostOrder(Order order = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.ShopCode == order.ShopCode);
            Customer customer = db.Customers.SingleOrDefault(c => c.AspNetUserId == userId);

            // We can remove the Item from cart
            // Then Change Order_Status
            //   Order_Status = 1 => Order Placed
            //   Order_Status = 2 => Order Confirmed By Shop Keeper
            //   Order_Status = 3 => Onthe Way
            //   Order_Status = 4 => Delivered
            //   Order_Status = 5 => Cancled

            List<OrderStatusTbl> orderStatus_Tbl = db.OrderStatusTbls.ToList();

            Order order1 = new Order()
            {
                OrderDate = DateTime.Now,
                OrderStatus = orderStatus_Tbl[0].OrderStatusId,     //    Order Placed => 1
                OrderTotal = order.OrderTotal,
                CustomerId = customer.AspNetUserId,
                ProductId = order.ProductId,
                ShopCode = order.ShopCode,
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5  =>  Cancel Order
        [Route("CancelOrder/{id:int}")]
        public async Task<IActionResult> DeleteOrder(int id, string Shop_Code)
        {
            Order order = await db.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.ShopCode != Shop_Code)
            {
                return NotFound();
            }
            List<OrderStatusTbl> orderStatus_Tbl = db.OrderStatusTbls.ToList();

            order.OrderStatus = orderStatus_Tbl[4].OrderStatusId;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [Authorize(Roles = "IsAShop")]
        [Route("ConfirmOrder")]
        public async Task<IActionResult> PostConfirmOrder(int id, string Shop_Code)
        {
            Order order = await db.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.ShopCode != Shop_Code)
            {
                return NotFound();
            }
            order.OrderStatus = 2;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [Authorize(Roles = "IsAShop")]
        [Route("OrderOnTheWay")]
        public async Task<IActionResult> PostOrderOnTheWay(int id, string Shop_Code)
        {
            Order order = await db.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.ShopCode != Shop_Code)
            {
                return NotFound();
            }
            order.OrderStatus = 3;

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        [Authorize(Roles = "IsAShop")]
        [Route("OrderDelivered")]
        public async Task<IActionResult> PostOrderDelivered(int id, string Shop_Code)
        {
            Order order = await db.Orders.FindAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Customer customer = db.Customers.FirstOrDefault(u => u.AspNetUserId == userId);

            if (order == null || order.ShopCode != Shop_Code)
            {
                return NotFound();
            }

            List<OrderStatusTbl> orderStatus_Tbl = db.OrderStatusTbls.ToList();

            order.DeliveredDate = DateTime.Now;
            order.OrderStatus = orderStatus_Tbl[3].OrderStatusId;    // Order Delivered  => 4

            //db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }
        [Route("IsOrderExists")]
        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}
