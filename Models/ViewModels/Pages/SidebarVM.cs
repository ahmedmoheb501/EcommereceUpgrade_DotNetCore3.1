using EcommCMS.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.ViewModels.Pages
{
    public class SidebarVM
    {
        public SidebarVM()
        {

        }
        public SidebarVM(SidebarDTO dto)
        {
            this.Id = dto.Id;
            this.Body = dto.Body;
        }
        public int Id { get; set; }
        public string Body { get; set; }
    }
}
