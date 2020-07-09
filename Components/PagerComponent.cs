using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommCMS.Models.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace EcommCMS.Components
{
    public class PagerComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}
