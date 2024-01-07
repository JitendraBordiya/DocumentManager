using DOCUMENTMANAGER.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOCUMENTMANAGER.Controllers
{
    [Route("[controller]/[action]")]
    public class RegistrationController : Controller
    {

        private readonly DOCMANAGERContext _context;

        public RegistrationController(DOCMANAGERContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserRegistration user)
        {
            var existingUser = _context.UserRegistration.Where(c=> c.UserName == user.UserName).FirstOrDefault();
            if(existingUser == null) {
                _context.Add(user);
                _context.SaveChanges();
                ViewBag.message = "The user " + user.UserName + " is saved successfully";
            }

            return RedirectToAction("Login","Authentication");
        }

    }
}
