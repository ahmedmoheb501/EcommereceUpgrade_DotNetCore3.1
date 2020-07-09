using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;
using EcommCMS.Models.ViewModels.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommCMS.Controllers
{
    public class PagesController : Controller
    {
        private readonly Db _context;
      //  private readonly IProductRepository _productRepo;
        public PagesController(Db context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index(string id="")
        {
            //Declare ProductVM
            PageVM page;
            //Get The Product
            PageDTO dto = await _context.Pages.Where(x=>x.Slug == id).FirstOrDefaultAsync();
            //Make Sure Product exists
            if (dto == null)
            {
                return Content("That page doesn't exist");
            }
            //Init Model
            page = new PageVM(dto);
           
            //return Product
            return View(page);
        }
    }
}