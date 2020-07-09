using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EcommCMS.Models.ViewModels.Pages;
using EcommCMS.Models.ViewModels.Category;
using Microsoft.AspNetCore.Identity;

using EcommCMS.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EcommCMS.Models.ViewModels.Products;

namespace EcommCMS.Models.Data
{
    public class Db : IdentityDbContext<IdentityUser>
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
     

        public DbSet<SidebarDTO> Sidebar { get; set; }
        public new DbSet<UsersDTO> Users { get; set; }
        public DbSet<ProductsDTO> Products{ get; set; }

        public DbSet<OrdersDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
