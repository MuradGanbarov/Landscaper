using Landscaper.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Landscaper.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }

    }
}