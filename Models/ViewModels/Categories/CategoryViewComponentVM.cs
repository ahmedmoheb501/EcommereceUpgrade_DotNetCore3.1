using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Data;

namespace EcommCMS.Models.ViewModels.Category
{
    public class CategoryViewComponentVM
    {
        // public List<CategoryDTO> CategoryList;
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}
