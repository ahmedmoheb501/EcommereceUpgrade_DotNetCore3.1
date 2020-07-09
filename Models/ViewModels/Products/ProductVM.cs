using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommCMS.Models.ViewModels.Products
{
    public class ProductVM
    {

        public ProductVM()
        {
        }
        public ProductVM(ProductsDTO Obj)
        {
            Id = Obj.Id;
            Name = Obj.Name;
            Slug = Obj.Slug;
            Description = Obj.Description;
            price = Obj.price;
            CategoryName = Obj.CategoryName;
            CategoryId = Obj.CategoryId;
            ImageName = Obj.ImageName;
        }
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal price { get; set; }

        public string CategoryName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        [NotMapped]
        public IEnumerable<string> GalleryImages { get; set; }
        //public List<ProductsDTO> ProductList { get; set; }
    }
}
