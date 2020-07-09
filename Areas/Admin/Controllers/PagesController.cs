using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommCMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly Db _context;
        public PagesController(Db context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            TempData["Pagemsg"] = null;
            List<PageVM> Pages;
            Pages = _context.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            return View(Pages);
        }
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Declare slug
            string slug;

            //init PageDTO
            PageDTO dto = new PageDTO();

            //DTO Title
            dto.Title = model.Title;

            //check for and set slug if need be
            if (string.IsNullOrEmpty(model.Slug))
            {
                slug = model.Title.Replace(" ", "-").ToLower();
            }
            else
            {
                slug = model.Slug.Replace(" ", "-").ToLower();
            }

            //Make sure title and slug are unique
            if (_context.Pages.Any(x => x.Title == model.Title || _context.Pages.Any(x => x.Slug == model.Slug)))
            {
                ModelState.AddModelError("", "That title or slug already exists");
                return View(model);
            }

            //Set the rest
            dto.Slug = slug;
            dto.Body = model.Body;
            dto.HasSidebar = model.HasSidebar;
            dto.Sorting = 100;

            //Save
            _context.Pages.Add(dto);
            _context.SaveChanges();

            //Set message
            TempData["msg"] = "You have added a new page.";


            //Redirect
            return RedirectToAction("AddPage");
        }

        [HttpGet]
        public async Task<IActionResult> EditPage(int? id)
        {          
            if (id == null)
            {
                return NotFound();
            }
            PageDTO dto = await _context.Pages.FindAsync(id);
            if (dto != null)
            {
                PageVM vM = new PageVM();
                vM.Id = dto.Id;
                vM.Body = dto.Body;
                vM.Title = dto.Title;
                vM.Slug = dto.Slug;
                vM.HasSidebar = dto.HasSidebar;

                TempData["Title"] = dto.Title;
            
                return View(vM);

            }
            return NotFound();

        }
        [HttpPost]
        public async Task<IActionResult> EditPage(PageVM model, string Title, string Slug)
        {
            if (model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {               
                try
                {
                    //Declare slug
                    string slug;

                    //init PageDTO
                    PageDTO dto = new PageDTO();

                    //DTO Title
                    dto.Title = model.Title;

                    //check for and set slug if need be
                    if (string.IsNullOrEmpty(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                    //Make sure title and slug are unique

                    if (!_context.Pages.Any(x => x.Title == model.Title && x.Id == model.Id))
                    {
                        //DTO Title
                        dto.Title = model.Title;
                    }
                    else
                    {


                        dto.Title = Title;

                    }

                    if (!_context.Pages.Any(x => x.Slug == model.Slug && x.Id == model.Id))
                    {
                        dto.Slug = slug;
                    }
                    else
                    {
                        dto.Slug = Slug;
                    }

                    //Set the rest
                    //  dto.Slug = slug;
                    dto.Body = model.Body;
                    dto.HasSidebar = model.HasSidebar;
                    dto.Sorting = 100;
                    dto.Id = model.Id;

                    //Save

                    _context.Pages.Update(dto);                 
                    await _context.SaveChangesAsync();

                    //Set message
                    TempData["Pagemsg"] = "You have Edited the selected page.";

                    //Redirect
                    return RedirectToAction("EditPage");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (model.Id <= 0)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Pages/Delete/Id
        public async Task<IActionResult> DeletePage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PageDTO dto = await _context.Pages.FindAsync(id);
            if (dto == null)
            {
                return NotFound();
            }
            PageVM vM = new PageVM();
            vM.Id = dto.Id;
            vM.Body = dto.Body;
            vM.Title = dto.Title;
            vM.Slug = dto.Slug;
            vM.HasSidebar = dto.HasSidebar;
            vM.Sorting = dto.Sorting;
            return View(vM);
        }

        // POST: Pages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePage(int id)
        {
            PageDTO dto = await _context.Pages.FindAsync(id);
            _context.Pages.Remove(dto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DetailsPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PageDTO dto = await _context.Pages.FindAsync(id);
            if (dto != null)
            {
                PageVM vM = new PageVM();
                vM.Id = dto.Id;
                vM.Body = dto.Body;
                vM.Title = dto.Title;
                vM.Slug = dto.Slug;
                vM.HasSidebar = dto.HasSidebar;
                vM.Sorting = dto.Sorting;
                return View(vM);

            }
            return NotFound();
        }

        // Ajax Post
        /// <summary>
        /// Sort the Pages  when the user drag & drop them in the list
        /// </summary>
        /// <param name="data"></param>
        // Ajax Post
        [HttpPost]
        public IActionResult UpdateSort(string data)
        {
            //Split "page[]=" the serilized data and put them in the array
            string[] arrItemsPlanner = data.Split(new string[] { "page[]=" }, StringSplitOptions.None);
            // a variable for holding string
            string str = "";
            // Get the arrItemsPlanner and convert to str
            for (int i = 0; i < arrItemsPlanner.Length; i++)
            {
                str += arrItemsPlanner[i].ToString();
            }
            //Split "&" in str and put the numbers in arrItemsPlanner as string
            arrItemsPlanner = str.Split(new string[] { "&" }, StringSplitOptions.None);
            try
            {
                // update Sorting Field
                for (int i = 0; i < arrItemsPlanner.Length; i++)
                {
                    int id = Convert.ToInt32(arrItemsPlanner[i]);
                    var dto1 = _context.Pages.Find(id);
                    if (dto1 != null)
                    {
                        dto1.Sorting = i + 1;
                    }
                    _context.Pages.Update(dto1);
                    _context.SaveChanges();
                }
                //Redirect
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare Model
            SidebarVM model;
            //Declare DTO
            SidebarDTO dto = _context.Sidebar.Find(1);
            //Init model
            model = new SidebarVM(dto);
            //Return view with model
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            //Get the dto
            SidebarDTO dto = _context.Sidebar.Find(1);
            //set the body
            dto.Body = model.Body;
            //save
            _context.SaveChanges();
            //set message
            TempData["msg"] = "Sidebar has been edited successfully";
            //Redirect
            return View();
        }    
    }
}