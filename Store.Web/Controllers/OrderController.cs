using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Data.Entities;
using Store.Services.HandlerResponses;
using Store.Services.Services.OrderService;
using Store.Services.Services.OrderService.Dtos;
using Store.Web.Controllers;
using System.Security.Claims;

namespace Store.API.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService )
        {
            _orderService = orderService;
        }

        [HttpPost]

        public async Task<ActionResult<OrderDetailsDto>> CreatOrderAsync(OrderDto input)
        {
            var order = await _orderService.CreateOrderAsync(input);

            if (order is null)
                return BadRequest(new Response (400, " Error while creating your order "));
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<IReadOnlyList<OrderDetailsDto>>> GetAllOrdersForUserAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetAllOrdersForUserAsync(email);

            return Ok(orders);

        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderByIdAsync(Guid id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var order = await _orderService.GetOrderByIdAsync(id , email);

            return Ok(order);

        }

        [HttpPost]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethods>>> GetAllDeliveryMethodsAsync()
         => Ok(await _orderService.GetAllDeliveryMethodsAsync());
    }
}
