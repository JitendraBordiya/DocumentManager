using Microsoft.AspNetCore.Mvc;

namespace DOCUMENTMANAGER.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
