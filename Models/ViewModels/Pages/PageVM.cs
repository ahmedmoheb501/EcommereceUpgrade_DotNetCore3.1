using EcommCMS.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.ViewModels.Pages
{
    public class PageVM
    {
        //This is a constructor
        public PageVM()
        {

        }
        public PageVM(PageDTO PageObj)
        {
            Id = PageObj.Id;
            Title = PageObj.Title;
            Slug = PageObj.Slug;
            Body = PageObj.Body;
            Sorting = PageObj.Sorting;
            HasSidebar = PageObj.HasSidebar;
        }
        public int Id { get; set; }
        [Required]
        [StringLength(50,MinimumLength =3)]
        [Display(Name ="Title")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        //[AllowHtml]
        public string Body { get; set; }
        public int Sorting { get; set; }
        public bool HasSidebar { get; set; }
    }
}
