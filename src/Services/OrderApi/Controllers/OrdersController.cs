using Common.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShoesOnContainers.Services.OrderApi.Data;
using ShoesOnContainers.Services.OrderApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ShoesOnContainers.Services.OrderApi.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrdersContext _ordersContext;
        private readonly IOptionsSnapshot<OrderSettings> _settings;
        private IBus _bus;

        private readonly ILogger<OrdersController> _logger;

        public OrdersController(OrdersContext ordersContext, ILogger<OrdersController> logger, IOptionsSnapshot<OrderSettings> settings, IBus bus)
        {
            _settings = settings;
            // _ordersContext = ordersContext;
            _ordersContext = ordersContext ?? throw new ArgumentNullException(nameof(ordersContext));

            ((DbContext)ordersContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _bus = bus;
            _logger = logger;
        }

        // POST api/Order
        [Route("new")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var envs = Environment.GetEnvironmentVariables();
            var conString = _settings.Value.ConnectionString;
            _logger.LogInformation($"{conString}");

            order.OrderStatus = OrderStatus.Preparing;
            order.OrderDate = DateTime.UtcNow;

            _logger.LogInformation(" In Create Order");
            _logger.LogInformation(" Order" + order.UserName);

            _ordersContext.Orders.Add(order);
            _ordersContext.OrderItems.AddRange(order.OrderItems);

            _logger.LogInformation(" Order added to context");
            _logger.LogInformation(" Saving........");
            try
            {
                await _ordersContext.SaveChangesAsync();
                _logger.LogWarning("BuyerId is: " + order.BuyerId);
                _bus.Publish(new OrderCompletedEvent(order.BuyerId)).Wait();
                return CreatedAtRoute("GetOrder", new { id = order.OrderId }, order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("An error occored during Order saving .." + ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}", Name = "GetOrder")]
        //  [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var item = await _ordersContext.Orders
                .Include(x => x.OrderItems)
                .SingleOrDefaultAsync(ci => ci.OrderId == id);
            if (item != null)
            {
                return Ok(item);
            }

            return NotFound();
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _ordersContext.Orders.ToListAsync();

            // var orders = await orderTask;

            return Ok(orders);
        }

        //[HttpGet]
        //[Route("userOrders")]
        //[ProducesResponseType((int)HttpStatusCode.Accepted)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> GetOrdersByUser([FromQuery] string userName)
        //{
        //    _caller.Claims.Select(
        //        c => new { c.Type, c.Value });

        //    var items = await _ordersContext.Orders
        //        .Where(ci => ci.UserName.Equals(userName))
        //        .Include(x => x.OrderItems)
        //        .ToListAsync();
        //    if (items != null)
        //    {
        //        return Ok(items);
        //    }

        //    return NotFound();

        //}
    }
}