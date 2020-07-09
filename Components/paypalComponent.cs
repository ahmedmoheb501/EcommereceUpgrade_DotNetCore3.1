using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Extensions;
using EcommCMS.Models.ViewModels.Cart;

namespace EcommCMS.Components
{
    [ViewComponent]
    public class PaypalComponent : ViewComponent
    {
        public const string SessionKeyName = "_Name";
        public PaypalComponent()
        {

        }

        public IViewComponentResult Invoke()
        {
            List<CartVM> cart= HttpContext.Session.Get<List<CartVM>>(SessionKeyName);
            return View(cart);
        }
    }
}
