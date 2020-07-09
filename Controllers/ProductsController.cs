using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.Extensions;
using EcommCMS.Models.Interfaces;
using EcommCMS.Models.ViewModels.Cart;
using EcommCMS.Models.ViewModels.Category;
using EcommCMS.Models.ViewModels.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using ReflectionIT.Mvc.Paging;

namespace EcommCMS.Controllers
{
  
    public class ProductsController : Controller
    {
        private readonly Db _context;
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(Db context, IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
        {
            this._context = context;
            this._productRepo = productRepo;
            this._webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index( int page = 1)
        {
             ProductVM ProductVm = new ProductVM();
                    
            // for using paing, the list should be "IOrderedQueryable"
            IOrderedQueryable<ProductVM> productList = _context.Products.Select(x => new ProductVM()
            {
                Id = x.Id,
                Name = x.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                price = x.price,
                Slug = x.Slug,
                ImageName = x.ImageName,
                Description = x.Description
            }).OrderBy(p => p.Id); // Ordered the List
           //Paging Product List
            var model = await PagingList.CreateAsync(productList, 3, page);

            return View(model); // A view works only with view model so we convert dto list to Vm list
        }
        public IActionResult CategoryMenuViewComponent()
        {
            var categoryList = _context.Categories.ToArray().OrderBy(x => x.Sorting).ToList();

            //Return Partial View with model
            return View(categoryList);
        }

        //[Route("Products/Category/{cat?}")]
        //public IActionResult Category(string cat)
        //{
        //    //Get products by category
        //    return  View(_productRepo.GetProductsByCat(cat));
        //}
        
        [Route("Products/Category/{cat?}")]
        public ActionResult Category(int? page, string cat)
        {
            // Declare a list of ProductVM
            IQueryable<ProductVM> listOfProductVM;

            // Set page number
            var pageNumber = page ?? 1;

            // Init the list 
            listOfProductVM = _context.Products
                              .Where(x => cat == null || cat == "" || x.Category.Slug == cat)
                              .Select(x => new ProductVM(x)).AsQueryable();

            // Set pagination
            //ViewData["OnePageOfProducts"] = listOfProductVM.ToPagedList(pageNumber, 10);
            IPagedList<ProductVM> result = listOfProductVM.ToPagedList(pageNumber, 5);
            // Return view with list
            return View(result);
        }

        // GET: products/productDetails/name
        public ActionResult ProductDetails(string name)
        {
            //Declare the view model and Dto
            ProductVM model;
            ProductsDTO dto;
            //Get product Id
            int Id = 0;
            //Check if products exist
            if (!_context.Products.Any(x => x.Slug.Equals(name)))
            {
                return RedirectToAction("Index","Products");
            }
            //Get product Dto
            dto = _context.Products.Where(x => x.Slug.Equals(name)).FirstOrDefault();
            //Get inserted Id
            Id = dto.Id;
            //Init Model
            model = new ProductVM(dto);
            //Get Gallary images
            string webRootPath = _webHostEnvironment.WebRootPath + "\\";
            model.GalleryImages = Directory.EnumerateFiles(
                string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\Thumbs",
                webRootPath,Id)).Select(fn => Path.GetFileName(fn));
            //return view with model
            return View(model); 
        }


        //GET : Products/AddToCartVC
        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";
        public ActionResult AddToCartVC(int id)
        {
            //init CartVM list
            List<CartVM> cartList = (List<CartVM>)HttpContext.Session.Get<List<CartVM>>(SessionKeyName) ?? new List<CartVM>();
            //init cartVM
            CartVM cartModel = new CartVM();
            //Get the product
            ProductsDTO product = _context.Products.Find(id);
            //check if the product is already in the cart
            var productInCart = cartList.FirstOrDefault(x=>x.ProductId ==id);
            //if not . add new
            if (productInCart == null)
            {
                cartList.Add(new CartVM() { 
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = 1,
                    Price = product.price,
                    Image = product.ImageName
                });
            }
            else {
                //if it is increment
                productInCart.Quantity++;
            }
            //save cart back to session
            HttpContext.Session.Set<List<CartVM>>(SessionKeyName,cartList);

            //Get total qty and price and add to model
            int qty = 0;
            decimal price = 0m;
            foreach (var item in cartList)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }
            cartModel.Quantity = qty;
            cartModel.Price = price;
            

            //return partial view with model
            return ViewComponent("CartComponent");
        }

    }
}