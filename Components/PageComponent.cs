using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;

namespace EcommCMS.Components
{
    [ViewComponent]
    public class PageComponent : ViewComponent
    {
        private readonly Db _context;
        public PageComponent(Db context)
        {
            this._context = context;
        }

        public IViewComponentResult Invoke()
        {
            //Declare Model
            //  CategoryViewComponentVM categoryMenuList = new CategoryViewComponentVM();
            //Get the CategoryList         
            //List<CategoryViewComponentVM> categoryList = new List<CategoryViewComponentVM>();
            var pageList = _context.Pages.OrderBy(x => x.Sorting).Select(c => new PageViewComponentVM()
            {
                Name = c.Title,
                Slug = c.Slug
            }).ToList();
            return View(pageList);
        }
    }
}
