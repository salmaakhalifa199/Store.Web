using AutoMapper;
using StackExchange.Redis;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.Basket;
using Store.Repository.Interfaces;
using Store.Repository.Specification.OrderSpecs;
using Store.Services.Services.BasketService;
using Store.Services.Services.OrderService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Store.Data.Entities.OrderEntities.Order;

namespace Store.Services.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketService _basketService;
        private readonly IUnitWork _unitWork;
        private readonly IMapper _mapper;

        public OrderService(IBasketService basketService , IUnitWork unitWork , IMapper mapper)
        {
            _basketService = basketService;
            _unitWork = unitWork;
            _mapper = mapper;
        }
        public async Task<OrderDetailsDto> CreateOrderAsync(OrderDto input)
        {
            // Get basket

            var basket = await _basketService.GetBasketAsync(input.BasketId);
            if (basket is null)
                throw new Exception("Basket Not Exist");


            #region Fill order item list with items in the basket 

            var orderItems = new List<OrderItemDto>();

            foreach (var basketitem in basket.BasketItems)
            {
                var productItem = await _unitWork.Repository<Product, int>().GetByIdAsync(basketitem.ProductId);

                if (productItem is null)
                    throw new Exception($"Product With Id : {basketitem.ProductId} Not Exist");

                var itemOrdered = new ProductItem
                {
                    ProductId = productItem.Id,
                    ProductName = productItem.Name,
                    PictureUrl = productItem.PictureUrl,
                };

                var orderItem = new OrderItem
                {
                    Price = productItem.Price,
                    Quantity = basketitem.Quantity,
                    ItemOrdered = itemOrdered,
                };

                var mapperOrderItem = _mapper.Map<OrderItemDto>(orderItem);
                orderItems.Add(mapperOrderItem);

            }

            #endregion

            #region Get Delivery Method

            var deliveryMethod = await _unitWork.Repository<DeliveryMethods, int>().GetByIdAsync(input.DeliveryMethodId);

            if (deliveryMethod is null)
                throw new Exception("Delivery Method Not Provided");

            #endregion

            #region Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Quantity * item.Price);
            #endregion

            #region To Do => Payment



            #endregion

            #region Create Order

            var mappedShippingAddress = _mapper.Map<ShippingAddress>(input.ShippingAddress);

            var mappedOrderItems = _mapper.Map<List<OrderItem>>(orderItems);

            var order = new Order
            {
                DeliveryMethodId = deliveryMethod.Id,   
                ShippingAddress = mappedShippingAddress,
                BuyerEmail = input.BuyerEmail,
                BasketId = input.BasketId,  
                OrderItems = mappedOrderItems,
                SubTotal = subTotal
            };

            await _unitWork.Repository<Order , Guid>().AddAsync(order);

            await _unitWork.CompleteAsync();

            var mappedOrder = _mapper.Map<OrderDetailsDto>(order);
            return mappedOrder;

            #endregion

        }

        public  async Task<IReadOnlyList<DeliveryMethods>> GetAllDeliveryMethodsAsync()
        => await _unitWork.Repository<DeliveryMethods, int>().GetAllAsync();
        

        public async Task<IReadOnlyList<OrderDetailsDto>> GetAllOrdersForUserAsync(string buyerEmail)
        {
            var specs = new OrderWithItemSpecification(buyerEmail);

            var orders = await _unitWork.Repository<Order, Guid>().GetAllWithSpecificationAsync(specs);

            if (!orders.Any())
                throw new Exception("You do not have any orders yet!");

            var mappedOrders = _mapper.Map<List<OrderDetailsDto>>(orders);

            return mappedOrders;
        }

        public async Task<OrderDetailsDto> GetOrderByIdAsync(Guid id)
        {
            var specs = new OrderWithItemSpecification(id);

            var order = await _unitWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception($"There is no order with id {id}");

            var mappedOrder = _mapper.Map<OrderDetailsDto>(order);

            return mappedOrder;
        }
    }
}
