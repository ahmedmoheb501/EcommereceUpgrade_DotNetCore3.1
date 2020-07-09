using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Account;
using EcommCMS.Models.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly Db _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(Db context, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IStringLocalizer<AccountController> localizer)
        {
            this._context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["asd"];
            return View();
        }

        [HttpGet]
        public ActionResult AddAccount()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAccount(ProfileVM model)//use userVM instead of profilevm, also apply in addaccount view
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //TODO change code to use identity
            //code sample
            var user = new IdentityUser() { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                //init UsersDTO
                UsersDTO dto = new UsersDTO();
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.Username = model.Username;
                dto.Password = model.Password;
                dto.EmailAddress = model.EmailAddress;


                //Save
                _context.Users.Add(dto);
                _context.SaveChanges();
                //Set message
                TempData["Accountmsg"] = "You have added a new user.";

                //Redirect
                return View("~/Views/Account/AddAccount.cshtml");
            }
            else
            {
                return View(model);
            }
        }
        //Show Login Page
        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }
        //Select User
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserVM model)
        {
            //Select User by username and password
            var user = await _userManager.FindByNameAsync(model.Username);
            //var dto = _context.Users.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
            if (user != null) //"Login Success";
            {
                // TempData["Accountmsg"] = "Login Success";
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            // if "Login Faile"
            //Set message
            TempData["Accountmsg"] = "Login Fai";
            return View("~/Views/Account/Login.cshtml");
        }
        public IActionResult userNavPartial()
        {
            //Get username
            string username = User.Identity.Name;
            //Declare Model
            UserNavPartialVM userNavModel;
            //Get the user
            UsersDTO userDto = _context.Users.FirstOrDefault(x => x.Username == username);
            //Build the model
            userNavModel = new UserNavPartialVM()
            {
                FirstName = userDto.FirstName,
                Lastname = userDto.LastName
            };
            //Return Partial View with model
            return PartialView(userNavModel);
        }
        //userprofile {get,post} -> Ahmed
        [HttpGet]
        //[Authorize]
        public IActionResult userprofile()
        {
            //Get username
            string username = User.Identity.Name;
            //Declare Model
            ProfileVM profileModel;
            //Get User
            UsersDTO user = _context.Users.FirstOrDefault(x => x.Username == username);
            //Build model
            profileModel = new ProfileVM(user);
            //Return view with model
            return View(profileModel);
        }
        //Post user profile
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> userprofile(ProfileVM profile)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(profile);
            }
            //Check if passwords match
            if (!string.IsNullOrWhiteSpace(profile.Password))
            {
                if (profile.Password.Equals(profile.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password don't match");
                    return View(profile);
                }
            }
            //Get Username
            string username = User.Identity.Name;
            //Make sure username is unique
            if (_context.Users.Where(x => x.Id != profile.Id).Any(x => x.Username == username))
            {
                ModelState.AddModelError("", "username " + profile.Username + " already exists.");
                profile.Username = "";
                return View(profile);
            }
            //Edit dto
            UsersDTO user = _context.Users.Find(profile.Id);
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            user.EmailAddress = profile.EmailAddress;
            user.Username = profile.Username;
            //Edit password if it was changed only
            if (!string.IsNullOrWhiteSpace(profile.Password))
            {
                user.Password = profile.Password;
            }
            //save
            await _context.SaveChangesAsync();
            //set message
            ViewData["msg"] = "Your profile is updated successfully";
            //Redirect
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login");
        }

        // GET: /account/Orders
        //[Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            // Init list of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            // Get user id
            UsersDTO user = _context.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            int userId = user.Id;

            // Init list of OrderVM
            List<OrderVM> orders = _context.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();

            // Loop through list of OrderVM
            foreach (var order in orders)
            {
                // Init products dict
                Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                // Declare total
                decimal total = 0m;

                // Init list of OrderDetailsDTO
                List<OrderDetailsDTO> orderDetailsDTO = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                // Loop though list of OrderDetailsDTO
                foreach (var orderDetails in orderDetailsDTO)
                {
                    // Get product
                    ProductsDTO product = _context.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();

                    // Get product price
                    decimal price = product.price;

                    // Get product name
                    string productName = product.Name;

                    // Add to products dict
                    productsAndQty.Add(productName, orderDetails.Quantity);

                    // Get total
                    total += orderDetails.Quantity * price;
                }

                // Add to OrdersForUserVM list
                ordersForUser.Add(new OrdersForUserVM()
                {
                    OrderNumber = order.OrderId,
                    Total = total,
                    ProductsAndQty = productsAndQty,
                    CreatedAt = order.CreatedAt
                });
            }

            // Return view with list of OrdersForUserVM
            return View(ordersForUser);
        }
    }
}
