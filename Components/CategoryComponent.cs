using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Account;
using EcommCMS.Models.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Components
{
    [ViewComponent]
    public class CategoryComponent : ViewComponent
    {
        private readonly Db _context;

        public CategoryComponent(Db context)
        {
            this._context = context;
        }

        // CategoryDTO category = new CategoryDTO();
     //   async Task<IActionResult>
        public IViewComponentResult Invoke()
        {
            //Declare Model
            //  CategoryViewComponentVM categoryMenuList = new CategoryViewComponentVM();
            //Get the CategoryList                    
           
           //IOrderedQueryable<CategoryViewComponentVM> categoryList = _context.Categories.Select(c => new CategoryViewComponentVM()

            var categoryList = _context.Categories.OrderBy(x => x.Sorting).Select(c => new CategoryViewComponentVM()
            {

                Name = c.Name,
                Slug = c.Slug,
                Sorting = c.Sorting
            }).ToList() ;//.OrderBy(p => p.Sorting);// Ordered the List//
           // var model = await PagingList.CreateAsync(categoryList, 3, page);
            return View(categoryList);
        }
    }
}
