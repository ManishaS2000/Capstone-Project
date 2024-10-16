using Microsoft.AspNetCore.Mvc;
using recyclecollection.Models;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace recyclecollection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Logindb _dbop; // Declare _dbop for login operations

        // Constructor to accept ILogger and Db instances
        public HomeController(ILogger<HomeController> logger, Logindb db)
        {
            _logger = logger;
            _dbop = db; // Assign the db parameter to _dbop

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Index action for login page (GET)
        
        public IActionResult Loginpage()
        {
            return View("Loginpage"); // Return the login view
        }

        [HttpPost]
        public IActionResult Index([Bind] Login loginModel)
        {
            if (ModelState.IsValid) // Check if the login model is valid
            {
                int res = _dbop.Login(loginModel);
                if (res == 1)
                {
                    TempData["msg"] = "Welcome to the Admin Page";
                    return RedirectToAction("homepage"); // Redirect to homepage on success
                }
                else
                {
                    TempData["msg"] = "User ID or Password is wrong!";
                }
            }
            return View("Loginpage"); // Return the login view with validation errors
        }


        public IActionResult homepage()
        {
            return View(); // Return the dashboard view
        }
    }
}
