﻿using Microsoft.AspNetCore.Mvc;
using xyzpharmacy.Data.ViewModels;
using System.Net.Mail;

namespace xyzpharmacy.Controllers
{
    public class HomeController : Controller
    {
        
        
           
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
