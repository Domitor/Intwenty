﻿
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

      
    }
}
