using Microsoft.AspNetCore.Mvc;
using recyclecollection.Models;

namespace recyclecollection.Controllers
{
    public class Usercontroller : Controller
    {
        public IActionResult AddUser()
        {
            return View("~/Views/Home/AddUser.cshtml");
        }


        AddUserdb ad = new AddUserdb();
        string msg;
        [HttpPost]
        public IActionResult ADDUSER([Bind] Adduser ar)
        {
            try
            {
                
                ad.ADDUSER(ar, out msg);
                TempData["msg"] = msg;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("Adduser");
        }


    }
}
