using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.Interfaces;
using EcommCMS.Models.ViewModels.Products;

namespace EcommCMS.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Db _context;

        public ProductRepository(Db context)
        {
            this._context = context;
        }
        public void CreateProduct(ProductVM productvm)
        {
            throw new NotImplementedException();
        }

        public List<ProductVM> GetProductsByCat(string cat)
        {
            //Find category 
            CategoryDTO category = _context.Categories.Where(x => x.Slug.Equals(cat)).FirstOrDefault();
            //Declare list of productVM
            List<ProductVM> productList;
            //search product by cat and populate into viewmodel
            productList = _context.Products.Where(x => x.CategoryId == category.Id).Select(x=> new ProductVM { 
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Description = x.Description,
                price = x.price,
                ImageName = x.ImageName,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            }).ToList();
            return productList;
        }
    }
}
