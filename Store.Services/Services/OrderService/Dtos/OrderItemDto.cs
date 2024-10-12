﻿using Store.Data.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.OrderService.Dtos
{
    public class OrderItemDto
    {
        public Guid OrderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public  int ProductItemId  { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }


    }
}
