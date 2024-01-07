using DOCUMENTMANAGER.Jwt;
using DOCUMENTMANAGER.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace DOCUMENTMANAGER.Controllers
{
   //[Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {

        private readonly DOCMANAGERContext _context;

        public AuthenticationController(DOCMANAGERContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        //[Authorize]
        [HttpPost]
        public  IActionResult Login(UserLogin model)
        {

            if (ModelState.IsValid)
            {
                var User = _context.UserRegistration.Where(c => c.UserName == model.username).FirstOrDefault();
                
                if (User != null)
                {
                    JwtTokenService jwtTokenService = new JwtTokenService();
                    var token = jwtTokenService.GenerateToken(User.UserName);

                    var loginList = _context.Login.Where(c => c.UserId == User.UserId && c.IsLogin == true).ToList();

                    if (loginList.Count <= 0)
                    {
                        Login login = new Login()
                        {
                            UserId = User.UserId,
                            LoginDate = DateTime.Now,
                            LoginToken = token,
                            IsLogin = true
                        };
                        _context.Add(login);
                        _context.SaveChanges();
                    }

                    CookieOptions cook= new CookieOptions();
                    cook.Secure = true;
                    cook.Expires = DateTime.Now.AddMinutes(5);
                    
                 
                     if (User.Password.Equals( model.password))
                     {
                        Response.Cookies.Append("Username", User.UserName , cook);
                        HttpContext.Session.SetString("User_Id",User.UserId.ToString());
                        Response.Cookies.Append("AuthToken", token, cook);
                        return RedirectToAction("Success");
                     }
                }
            }
            return RedirectToAction("Fail");
        }

        public IActionResult Logout()
        {
            long? UserId = Convert.ToInt64(HttpContext.Session.GetString("User_Id"));
            if(UserId != null)
            {
                
                var Login=_context.Login.FirstOrDefault(c=> c.UserId == UserId && c.IsLogin == true);
                if (Login != null)
                {
                    Login.IsLogin = false;
                    Login.LoginDate = DateTime.Now;
                }
                _context.Update(Login);
                _context.SaveChanges();

            }
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("AuthToken");
            // Sign out the user
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the home page or another page after logout
            return RedirectToAction("Login", "Authentication");
        }

        public IActionResult Success()
        {
            return RedirectToAction("Index","Dashboard");
        }

        public IActionResult Fail()
        {
            TempData["Error"] = "Authentication Failed";
            ViewBag.ErrorMessage = TempData["Error"];
            return RedirectToAction("Login", "Authentication"); 
        }
    }
}