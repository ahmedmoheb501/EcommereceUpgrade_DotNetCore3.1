using EcommCMS.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommCMS.Models.Interfaces
{
    public interface IPageRepository 
    {
        void CreatePage(PageVM pagevm);

    }
}
