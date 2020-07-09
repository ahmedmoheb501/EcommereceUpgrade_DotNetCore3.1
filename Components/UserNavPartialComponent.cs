using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EcommCMS.Components
{
    [ViewComponent]
    public class UserNavPartialComponent: ViewComponent
    {
        private readonly Db _context;

        public UserNavPartialComponent(Db context)
        {
            this._context = context;
        }
        public IViewComponentResult Invoke()
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
            return View(userNavModel);
        }
    }
}
