using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.Data
{
    [Table("tblOrders")]
    public class OrdersDTO
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual UsersDTO Users { get; set; }

    }
}
