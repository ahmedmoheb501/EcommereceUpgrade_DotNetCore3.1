using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.Data
{
    [Table("tblOrderDetails")]
    public class OrderDetailsDTO
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("UserId")]
        public virtual UsersDTO Users { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrdersDTO Orders { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductsDTO Products { get; set; }

    }
}
