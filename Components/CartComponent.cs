using EcommCMS.Models.Data;
using EcommCMS.Models.Extensions;
using EcommCMS.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Components
{
    [ViewComponent]
    public class CartComponent : ViewComponent
    {
        private readonly Db _context;
        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";

        public CartComponent(Db context)
        {
            this._context = context;
        }

        public IViewComponentResult Invoke()
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
    }
}
