using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;

namespace EcommCMS.Models.ViewModels.Category
{
    public class CategoryVM
    {
        public CategoryVM()
        {
        }
        public CategoryVM(CategoryDTO Obj)
        {
            Id = Obj.Id;
            Name = Obj.Name;
            Slug = Obj.Slug;
            Sorting = Obj.Sorting;  
            
        }
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Slug { get; set; } 
        public int Sorting { get; set; }

        //Add CategoryList (for show the list of categories Index view)
        public List<CategoryDTO> CategoryList;

    }
}
