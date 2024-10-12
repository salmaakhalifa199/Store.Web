using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.Interfaces;
using Store.Repository.Specification.OrderSpecs;
using Store.Services.Services.BasketService;
using Store.Services.Services.BasketService.Dtos;
using Store.Services.Services.OrderService.Dtos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Store.Data.Entities.Product;

namespace Store.Services.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitWork _unitWork;
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;

        public PaymentService(IConfiguration configuration , IUnitWork unitWork , IBasketService basketService , IMapper mapper)
        {
            _configuration = configuration;
            _unitWork = unitWork;
            _basketService = basketService;
            _mapper = mapper;
        }
        public async Task<CustomerBasketDto> CreateOrUpdatePaymentIntent(CustomerBasketDto basket)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            if (basket is null)
                throw new Exception("Basket is Empty");


            var deliveryMethod = await _unitWork.Repository<DeliveryMethods, int>().GetByIdAsync(basket.DeliveryMethodId.Value);

            if (deliveryMethod is null)
                throw new Exception("Delivery Method Not Provided");

            decimal shippingPrice = deliveryMethod.Price;

            foreach (var item in basket.BasketItems) 
            {
                var product = await _unitWork.Repository<Product, int>().GetByIdAsync(item.ProductId);

                if(item.Price != product.Price)
                    item.Price = product.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }

                };

                paymentIntent = await service.CreateAsync(options);

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                };

                await service.UpdateAsync(basket.PaymentIntentId , options);
            }
            await _basketService.UpdateBasketAsync(basket);

            return basket;

        }

        public async Task<OrderDetailsDto> UpdateOrderPaymentFailed(string paymentIntentId)
        { 
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await _unitWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception(" order does not exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Failed;

            _unitWork.Repository<Order, Guid>().UpdateAsync(order);

            await _unitWork.CompleteAsync();

            var mapperOrder = _mapper.Map<OrderDetailsDto>(order);

            return mapperOrder;

        }

        public async Task<OrderDetailsDto> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await _unitWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception(" order does not exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Received;

            _unitWork.Repository<Order, Guid>().UpdateAsync(order);

            await _unitWork.CompleteAsync();

            await _basketService.DeleteBasketAsync(order.BasketId);

            var mapperOrder = _mapper.Map<OrderDetailsDto>(order);

            return mapperOrder;
        }
    }
}
