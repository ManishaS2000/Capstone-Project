using Microsoft.AspNetCore.Mvc;
using recyclecollection.Models;

namespace recyclecollection.Controllers
{
    public class RevenueController : Controller
    {
        Revenuedb rm = new Revenuedb();

        public IActionResult Revenue()
        {
            return View("~/Views/Home/Revenue.cshtml"); // Return the waste collection form view
        }

        string msg;


        [HttpPost]
        public IActionResult Create([Bind] Revenue dt)
        {
            try
            {
                
                rm.InsertRevenue(dt, out msg);
                TempData["msg"] = msg;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("Revenue");
        }

    }
}
