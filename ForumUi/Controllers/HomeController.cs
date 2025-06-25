using Microsoft.AspNetCore.Mvc;

namespace ForumUi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            return RedirectToAction("Index", "Topics");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}