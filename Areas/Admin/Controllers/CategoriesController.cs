using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.Interfaces;
using EcommCMS.Models.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace EcommCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly Db _context;
        private readonly IPageRepository _pageRepo;
        public CategoriesController(Db context, IPageRepository pageRepo)
        {
            this._context = context;
            this._pageRepo = pageRepo;
        }
        // Get: Categories/Index
        public IActionResult Index()
        {
            CategoryVM CategoryVm = new CategoryVM();
            //Category List in Index View
            //this code will go in the ??? do you know  where ??
            CategoryVm.CategoryList = _context.Categories.ToArray().OrderBy(x => x.Sorting).ToList();//.Select(x => new CategoryVM(x)).ToList();
            return View(CategoryVm);
        }
        [HttpPost]
        // Post Ajax: Categories/Index
        public async Task<IActionResult> Index(CategoryVM model, string catName, int catId = 0)
        {

            if (catName != null & catId != 0)//Edit Category
            {
                CategoryDTO dto = _context.Categories.Find(catId);
                if (dto != null)
                {
                    // CategoryVM vM = new CategoryVM();
                    dto.Id = catId;
                    dto.Name = catName;
                    dto.Slug = catName;
                    dto.Sorting = dto.Sorting;
                    //Save
                    _context.Categories.Update(dto);
                    await _context.SaveChangesAsync();
                }
            }
            else if (model != null)//Add Category
            {
                //check model state
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //init CategoryDTO
                CategoryDTO dto = new CategoryDTO();

                //DTO Name & Slug
                dto.Name = dto.Slug = model.Name;

                //Make sure Name and slug are unique
                if (_context.Categories.Any(x => x.Name == model.Name || _context.Categories.Any(x => x.Slug == model.Slug)))
                {
                    ModelState.AddModelError("", "That name or slug already exists");

                    return View(model);
                }

                dto.Sorting = 100;

                //Save
                _context.Categories.Add(dto);
                await _context.SaveChangesAsync();

                //Set message
                TempData["msg"] = "You have added a new Categoy.";
            }
            //List Categories

            model.CategoryList = _context.Categories.ToArray().OrderBy(x => x.Sorting).ToList();//.Select(x => new CategoryVM(x)).ToList();
            return RedirectToAction(nameof(Index));
            //return RedirectToAction("Index");
        }

        // Get: Categories/Delete/5

        public async Task<IActionResult> DeleteCategory(int id = 0)
        {
            CategoryDTO dto = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(dto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Partial CategoryList View for Add, Edit and Delete Categories
        /// </summary>
        /// <returns></returns>
        //Partial CategoryList View Show
        public IActionResult GetPageCategories()
        {
            List<CategoryVM> Categories;
            Categories = _context.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            // return PartialView(Categories);
            return PartialView(@"~/Views/Shared/_CategoryList.cshtml", Categories);
        }

        [HttpPost]
        public void ReorderCats(string[] data)
        {
            //set initial count
            int count = 1;
            //Declare Cat dto
            CategoryDTO dto;
            //Get sorting for each caegory and update it
            string id;
            foreach (var catId in data)
            {
                id = catId.Substring(3, catId.Length - 3);
                dto = _context.Categories.Find(Int32.Parse(id));
                dto.Sorting = count;
                _context.SaveChanges();
                count++;
            }
        }
        
    }
}