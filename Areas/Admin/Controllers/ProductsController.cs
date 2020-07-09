using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using PagedList.Core;
using EcommCMS.Areas.Admin.Models.ViewModels;
using EcommCMS.Models.ViewModels.Orders;

namespace EcommCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly Db _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(Db context, IWebHostEnvironment hostingEnvironment)
        {
            this._context = context;
            this._hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Admin/Shop/Products
        public ActionResult Products(int? page, int? catId)
        {
            // Declare a list of ProductVM
            IQueryable<ProductVM> listOfProductVM;

            // Set page number
            var pageNumber = page ?? 1;

            // Init the list 
            listOfProductVM = _context.Products
                              .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                              .Select(x => new ProductVM(x)).AsQueryable();

            // Populate categories select list
            ViewData["Categories"] = new SelectList(_context.Categories.ToList(), "Id", "Name");

            // Set selected category
            ViewData["SelectedCat"] = catId.ToString();

            // Set pagination
            //ViewData["OnePageOfProducts"] = listOfProductVM.ToPagedList(pageNumber, 10);
            IPagedList<ProductVM> result = listOfProductVM.ToPagedList(pageNumber, 5);
            // Return view with list
            return View(result);
        }
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Init model
            ProductVM model = new ProductVM();

            // Add select list of categories to model

            model.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");

            // Return view with model
            return View(model);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public IActionResult AddProduct(ProductVM model, IFormFile file)
        {
            // Check model state
            if (!ModelState.IsValid)
            {

                model.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(model);

            }

            // Make sure product name is unique

            if (_context.Products.Any(x => x.Name == model.Name))
            {
                model.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                ModelState.AddModelError("", "That product name is taken!");
                return View(model);
            }
            // Declare product id
            int id;
            // Init and save productDTO
            ProductsDTO product = new ProductsDTO();
            product.Name = model.Name;
            product.Slug = model.Name.Replace(" ", "-").ToLower();
            product.Description = model.Description;
            product.price = model.price;
            product.CategoryId = model.CategoryId;
            product.ImageName = file.FileName;

            CategoryDTO catDTO = _context.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            product.CategoryName = catDTO.Name;

            _context.Products.Add(product);
            _context.SaveChanges();

            // Get the id
            id = product.Id;

            // Set TempData message
            TempData["SM"] = "Product added successfully";

            #region Upload Image

            // Create necessary directories
            string webRootPath = _hostingEnvironment.WebRootPath + "\\";
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", webRootPath));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {

                    model.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                    return View(model);
                }

                // Init image name
                string imageName = file.FileName;

                // Save image name to DTO

                ProductsDTO dto = _context.Products.Find(id);
                dto.ImageName = imageName;

                _context.SaveChanges();

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                //file.SaveAs(path);
                //using (var fileStream = new FileStream(path, FileMode.Create))
                //{
                //    await file.CopyToAsync(fileStream);
                //}
                using var mainImg = Image.Load(file.OpenReadStream());
                mainImg.Mutate(x => x.Resize(210, 210));
                mainImg.Save(path);

                //Create and save thumb
                using var image = Image.Load(file.OpenReadStream());
                image.Mutate(x => x.Resize(50, 50));
                image.Save(path2);
            }
            #endregion
            // Redirect
            return RedirectToAction("AddProduct");
        }

        //GET: Admin/Products/EditProduct
        public async Task<IActionResult> EditProduct(int Id)
        {
            //Declare ProductVM
            ProductVM product;
            //Get The Product
            ProductsDTO dto = await _context.Products.FindAsync(Id);
            //Make Sure Product exists
            if (dto == null)
            {
                return Content("That product doesn't exist");
            }
            //Init Model
            product = new ProductVM(dto);
            //Make a Select List
            product.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            //Get All gallery Images
            string webRootPath = _hostingEnvironment.WebRootPath + "\\";
            product.GalleryImages = Directory.EnumerateFiles(string.Format("{0}Images\\Uploads\\Products\\{1}\\Gallery\\Thumbs", webRootPath, Id))
                 .Select(fn => Path.GetFileName(fn));
            //return Product
            return View(product);
        }

        //POST: Admin/Products/EditProduct
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductVM model, IFormFile file)
        {
            //Get Producct ID
            int Id = model.Id;
            //Populate categories
            model.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            //Populate Callergy Images
            string WebRootPath = _hostingEnvironment.WebRootPath;
            model.GalleryImages = Directory.EnumerateFiles(string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\Thumbs", WebRootPath, Id))
                .Select(fn => Path.GetFileName(fn));
            //Check Model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Make sure product name is Unique
            if (_context.Products.Where(x => x.Id != Id).Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "That Product Name is taken!");
                return View(model);
            }
            //update Product
            ProductsDTO dto = _context.Products.Find(Id);
            dto.Name = model.Name;
            dto.Description = model.Description;
            dto.price = model.price;
            dto.Slug = model.Slug;
            dto.CategoryId = model.CategoryId;
            CategoryDTO category = _context.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
            dto.CategoryName = category.Name;
            dto.Category = category;


            #region ImageUpload
            //Check for file Upload
            if (file != null && file.Length > 0)
            {
                //Get Extension
                string extension = file.ContentType.ToLower();
                //Verify extension
                if (extension != "image/jpg" &&
                    extension != "image/jpeg" &&
                    extension != "image/pjpeg" &&
                    extension != "image/gif" &&
                    extension != "image/x-png" &&
                    extension != "image/png")
                {
                    ModelState.AddModelError("", "Image was not uploaded, Wrong Image extension.");
                    return View(model);
                }
                //set upload directory path
                string webRootPath = _hostingEnvironment.WebRootPath;
                DirectoryInfo originalPath = new DirectoryInfo(string.Format("{0}\\Images\\Uploads\\Products\\{1}", webRootPath, model.Id));
                DirectoryInfo thumbPath = new DirectoryInfo(string.Format("{0}\\Thumbs", originalPath));
                //Delete Files from directories
                foreach (FileInfo f in originalPath.GetFiles())
                    f.Delete();
                foreach (FileInfo f in thumbPath.GetFiles())
                    f.Delete();
                //Save Image name
                dto.ImageName = file.FileName;
                //Save original and thumb Images
                string path1 = string.Format("{0}\\{1}", originalPath.FullName, file.FileName);
                string path2 = string.Format("{0}\\{1}", thumbPath.FullName, file.FileName);
                using (var fileStream = new FileStream(path1, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                using var image = Image.Load(file.OpenReadStream());
                image.Mutate(x => x.Resize(50, 50));
                image.Save(path2);
            }
            #endregion
            //commit changes
            await _context.SaveChangesAsync();
            //set message
            ViewData["msg"] = "Product edited successfully";
            return View(model);
        }

        //POST: Admin/Products/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            //loop through files
            foreach (FormFile file in Request.Form.Files)
            {
                //Check itis not null
                if (file != null && file.Length > 0)
                {
                    //Set directory path
                    var originalPath = string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\{2}", webRootPath, id, file.FileName);
                    var ThumbsPath = string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\Thumbs\\{2}", webRootPath, id, file.FileName);
                    //save original and Thumb   
                    using var image1 = Image.Load(file.OpenReadStream());
                    image1.Mutate(x => x.Resize(210, 210));
                    image1.Save(originalPath);
                    //Create and save thumb
                    using var image = Image.Load(file.OpenReadStream());
                    image.Mutate(x => x.Resize(50, 50));
                    image.Save(ThumbsPath);
                }
            }
        }


        // POST: Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string fullPath1 = string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\{2}", webRootPath, id.ToString(), imageName);
            string fullPath2 = string.Format("{0}\\Images\\Uploads\\Products\\{1}\\Gallery\\Thumbs\\{2}", webRootPath, id.ToString(), imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

        // GET: Admin/Shop/Orders
        public ActionResult Orders()
        {
            // Init list of OrdersForAdminVM
            List<OrdersForAdmin> ordersForAdmin = new List<OrdersForAdmin>();

                // Init list of OrderVM
                List<OrderVM> orders = _context.Orders.ToArray().Select(x => new OrderVM(x)).ToList();

                // Loop through list of OrderVM
                foreach (var order in orders)
                {
                    // Init product dict
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                    // Declare total
                    decimal total = 0m;

                    // Init list of OrderDetailsDTO
                    List<OrderDetailsDTO> orderDetailsList = _context.OrderDetails.Where(X => X.OrderId == order.OrderId).ToList();

                    // Get username
                    UsersDTO user = _context.Users.Where(x => x.Id == order.UserId).FirstOrDefault();
                    string username = user.Username;

                    // Loop through list of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsList)
                    {
                        // Get product
                        ProductsDTO product = _context.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();

                        // Get product price
                        decimal price = product.price;

                        // Get product name
                        string productName = product.Name;

                        // Add to product dict
                        productsAndQty.Add(productName, orderDetails.Quantity);

                        // Get total
                        total += orderDetails.Quantity * price;
                    }

                    // Add to ordersForAdminVM list
                    ordersForAdmin.Add(new OrdersForAdmin()
                    {
                        OrderNumber = order.OrderId,
                        Username = username,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }

            // Return view with OrdersForAdminVM list
            return View(ordersForAdmin);
        }

    }
}