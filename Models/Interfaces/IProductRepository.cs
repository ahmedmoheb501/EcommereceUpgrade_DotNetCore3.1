using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.ViewModels.Products;

namespace EcommCMS.Models.Interfaces
{
    public interface IProductRepository
    {
        void CreateProduct(ProductVM productvm);
        List<ProductVM> GetProductsByCat(string cat);
    }
}
