using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.Extensions;
using EcommCMS.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommCMS.Controllers
{
    public class CartController : Controller
    {
        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";
        private readonly Db _context;

        //CTOR
        public CartController(Db context)
        {
            this._context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            //init the cart list
            var cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName) ?? new List<CartVM>();
            //Check if the cart is empty
            if (cart == null || cart.Count == 0)
            {
                ViewData["msg"] = "Your cart is empty";
                return View();
            }
            //calculate total and save to viewData
            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;
            }
            ViewData["GrandTotal"] = total;
            //Return View with list
            return View(cart);
        }


        public ActionResult CartPartial()
        {
            //Init CartVm
            CartVM cartModel = new CartVM();
            //Init quantity, price
            cartModel.Quantity = 0;
            cartModel.Price = 0m;
            //Check for cart session
            if (HttpContext.Session.Get<List<CartVM>>(SessionKeyName) != null)
            {
                //HttpContext.Session.SetString(SessionKeyName, "cart");
                //HttpContext.Session.SetInt32(SessionKeyAge, 773);
                var list = (List<CartVM>)HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
                //Get total quanitity & price
                foreach (var item in list)
                {
                    cartModel.Quantity += item.Quantity;
                    cartModel.Price += item.Quantity * item.Price;
                }
            }
            else
            {
                //Or set quantity and price to 0
                cartModel.Quantity = 0;
                cartModel.Price = 0m;
            }

            //Return partial with model
            return View(cartModel);
        }

        //GET: Products/incproduct
        public JsonResult incProduct(int productId)
        {
            //init cart list
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            //Get product from list
            CartVM productInCart = cart.FirstOrDefault(x => x.ProductId == productId);
            //inc quantity
            productInCart.Quantity++;
            //update session
            HttpContext.Session.Set<List<CartVM>>(SessionKeyName, cart);
            //store in json object
            var result = new { qty = productInCart.Quantity, price = productInCart.Price };
            //return json with data
            return Json(result);
        }

        //GET: Products/decproduct
        public JsonResult decProduct(int productId)
        {
            //init cart list
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            //Get product from list
            CartVM productInCart = cart.FirstOrDefault(x => x.ProductId == productId);
            //dec quantity
            if (productInCart.Quantity > 1)
            { productInCart.Quantity--; }
            else
            {
                productInCart.Quantity = 0;
                cart.Remove(productInCart);
            }
            //update session
            HttpContext.Session.Set<List<CartVM>>(SessionKeyName, cart);
            //store in json object
            var result = new { qty = productInCart.Quantity, price = productInCart.Price };
            //return json with data
            return Json(result);
        }

        //GET: Products/decproduct
        public JsonResult RemoveProduct(int productId)
        {
            //init cart list
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            //Get product from list
            CartVM productInCart = cart.FirstOrDefault(x => x.ProductId == productId);
            //remove from cart
            cart.Remove(productInCart);
            //update session
            HttpContext.Session.Set<List<CartVM>>(SessionKeyName, cart);
            //store in json object
            var result = new { total = productInCart.Quantity * productInCart.Price };
            //return json with data
            return Json(result);
        }
        // POST: /Cart/PlaceOrder
        public ActionResult PaypalComponent()
        {
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            return View(cart);
        }
        // GET: /Cart/updatePaypal Component
        public ActionResult UpdatePaypalComponent()
        {
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            return ViewComponent("PaypalComponent");
        }

        // POST: /Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Get cart list
            List<CartVM> cart = HttpContext.Session.Get<List<CartVM>>(SessionKeyName);

            // Get username
            string username = User.Identity.Name;

            int orderId = 0;

            #region order
            // Init OrderDTO
            OrdersDTO orderDTO = new OrdersDTO();

            // Get user id
            var q = _context.Users.FirstOrDefault(x => x.Username == username);
            int userId = q.Id;

            // Add to OrderDTO and save
            orderDTO.UserId = userId;
            orderDTO.CreatedAt = DateTime.Now;

            _context.Orders.Add(orderDTO);

            _context.SaveChanges();

            // Get inserted id
            orderId = orderDTO.OrderId;

            //Declare OrderDetailDTO
            OrderDetailsDTO orderDetailsDTO;
            // Add to OrderDetailsDTO
            foreach (var item in cart)
            {
                // Init OrderDetailsDTO
                orderDetailsDTO = new OrderDetailsDTO();
                orderDetailsDTO.OrderId = orderId;
                orderDetailsDTO.UserId = userId;
                orderDetailsDTO.ProductId = item.ProductId;
                orderDetailsDTO.Quantity = item.Quantity;

                _context.OrderDetails.Add(orderDetailsDTO);
            }
            _context.SaveChanges();
            #endregion

            // Email admin
            //var client = new SmtpClient("mailtrap.io", 2525)
            //{
            //    Credentials = new NetworkCredential("654dd088804a68", "629a0b20d6de99"),
            //    EnableSsl = true
            //};

            //client.Send("admin@example.com", "admin@example.com", "New Order", "You have a new order. Order number " + orderId);
            // Reset session
            HttpContext.Session.Set<List<CartVM>>(SessionKeyName, null);
        }
    }
}
