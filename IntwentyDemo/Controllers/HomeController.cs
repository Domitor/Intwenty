
using Intwenty;
using Intwenty.Entity;
using Microsoft.AspNetCore.Mvc;


namespace IntwentyDemo.Controllers
{
    public class HomeController : Controller
    {
    

        public HomeController()
        {
            
        }

        public IActionResult Index()
        {
          
            return View();
        }

        [HttpGet("UserPermission/{id}")]
        public IActionResult UserPermission(string id)
        {
            return View();
        }


    }
}
