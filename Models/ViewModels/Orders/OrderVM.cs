using EcommCMS.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.ViewModels.Orders
{
    public class OrderVM
    {
        public OrderVM()
        {

        }
        public OrderVM(OrdersDTO orderdto)
        {
            OrderId = orderdto.OrderId;
            UserId = orderdto.UserId;
            CreatedAt = orderdto.CreatedAt;
        }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
