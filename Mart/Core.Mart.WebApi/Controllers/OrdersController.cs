﻿using Core.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using Core.Mart.WebApi.ModelView;
using Microsoft.EntityFrameworkCore;

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

            return db.Orders.Where(u => u.ShopCode == shopDetail.ShopCode);
        }

        [HttpGet]
        [Route("GetCustomerOrders")]
        [Authorize(Roles = UserRoles.IsACustomer)]
        public async Task<IActionResult> GetCustomerOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var UserObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == userId);

                if (UserObj == null || UserObj.Email == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                Customer customer = db.Customers.SingleOrDefault(c => c.AspNetUserId == UserObj.Id);
                if (customer == null)
                {
                    return StatusCode(StatusCodes.Status303SeeOther);
                }

                var orders = await db.Orders
                    .Include(o => o.Product)
                    .Where(o => o.CustomerId == customer.CustomerId)
                    .Select(o => new { o.OrderId, o.OrderNumber, o.OrderDate, o.CustomerId, o.ShopCode, o.PaymentId, o.ShipDate,
                                        o.RequiredDate, o.ShipperId, o.Freight, o.SalesTax, o.TransactStatus, o.IsCancled, o.Paid,
                                        o.PaymentDate, o.IsOrderConfimed, o.IsShipped, o.IsOutForDelivery, o.IsDelivered, o.OutForDeliveryDate,
                                        o.DeliveredDate, o.RefundId, o.OrderQuantity, o.Product.ProductName, o.Product.UnitPrice, o.Product.ImagePath
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
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
        public async Task<IActionResult> PostOrder(List<OrderModel> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var orders = new List<OrderModel>();
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                var UserObj = db.AspNetUsers.FirstOrDefault(a => a.UserName == userId);

                if (UserObj == null || UserObj.Email == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                Customer customer = db.Customers.SingleOrDefault(c => c.AspNetUserId == UserObj.Id);
                if (customer == null)
                {
                    return StatusCode(StatusCodes.Status303SeeOther);
                }

                // Does product exists
                var IsProductExists = false;
                foreach (var item in items)
                {
                    using (CartDbcoreContext db = new CartDbcoreContext())
                    {
                        if (db.Products.Count(e => e.ProductId == item.ProductId) < 0)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError);
                        }
                    }
                }
                ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.AspNetUsersId == UserObj.Id);

                List<string> OrderList = new List<string>();

                // Place order in order table
                foreach (var order in items)
                {
                    //ShopDetail shopDetail = db.ShopDetails.FirstOrDefault(u => u.ShopCode == order.ShopCode);
                    var productUnitPrice = db.Products.SingleOrDefault(c => c.ProductId == order.ProductId).UnitPrice;

                    List<OrderStatusTbl> orderStatus_Tbl = db.OrderStatusTbls.ToList();

                    var guid = Guid.NewGuid().ToString();
                    Order order1 = new Order()
                    {

                        //OrderStatus = orderStatus_Tbl[0].OrderStatusId,     //    Order Placed => 1
                        CustomerId = customer.CustomerId,
                        ProductId = order.ProductId,
                        ShopCode = order.ShopCode,
                        OrderDate = DateTime.Now,
                        //  1 product ka price X it's quantity
                        OrderTotal = Convert.ToInt32(order.Quantity * productUnitPrice), 
                        OrderQuantity = order.Quantity,
                        // uniq identifier, unique order number
                        OrderNumber = guid,
                        PaymentId = null,
                        ShipDate = null,
                        RequiredDate = null,
                        ShipperId = null,
                        Freight = null,
                        SalesTax = null,
                        TransactStatus = null,
                        IsCancled = false,
                        Paid = null,
                        PaymentDate = null,
                        IsOrderConfimed = false,
                        IsShipped = null,
                        IsOutForDelivery = null,
                        IsDelivered = null,
                        OutForDeliveryDate = null,
                        DeliveredDate = null,
                        RefundId = null,
                    };

                    db.Orders.Add(order1);
                    await db.SaveChangesAsync();

                    OrderList.Add(order1.OrderNumber);

                    // Add Order Details for this order
                    //OrderDetail orderDetail = new OrderDetail()
                    //{
                          //   No Use of now
                    //}
                }
                return Ok(OrderList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
            //order.OrderStatus = orderStatus_Tbl[3].OrderStatusId;    // Order Delivered  => 4

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
