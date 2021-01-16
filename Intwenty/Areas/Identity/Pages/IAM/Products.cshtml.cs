using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Pages.IAM
{
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class ProductsModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IIntwentyProductManager ProductManager { get; }

        public ProductsModel(IIntwentyDataService ms, IIntwentyModelService sr, IIntwentyProductManager prodmanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            ProductManager = prodmanager;
        }

        public void OnGet()
        {
           
        }

        public async Task<IActionResult> OnGetLoad()
        {
            var list = await ProductManager.GetProducts();
            return new JsonResult(list);
        }

        public async Task<IActionResult> Load()
        {
            var list = await ProductManager.GetProducts();
            return new JsonResult(list);
        }

    

    }
}
