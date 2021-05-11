
using Intwenty;
using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Models;
using Intwenty.Entity;
using Intwenty.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IntwentyDemo.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        private IIntwentyDataService DataService { get; }
        private IIntwentyModelService ModelService { get; }
        private IntwentyUserManager UserManager { get; }

        public HomeController(IIntwentyDataService dataservice, IIntwentyModelService modelservice, IntwentyUserManager usermanager)
        {
            DataService = dataservice;
            ModelService = modelservice;
            UserManager = usermanager;
        }

       
        [Route("/")]
        public IActionResult Index()
        {
          
            return View();
        }

     


    }

}

  




