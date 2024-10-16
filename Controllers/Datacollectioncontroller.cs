using Microsoft.AspNetCore.Mvc;
using recyclecollection.Models;

namespace recyclecollection.Controllers
{
    public class Datacollectioncontroller : Controller
    {
    
        

        public IActionResult Datacollection()
        {
            return View("~/Views/Home/Datacollection.cshtml"); // Return the waste collection form view
        }

        Datacollectiondb dl = new Datacollectiondb();
        string msg;

        [HttpPost]
        public IActionResult Create([Bind] Datacollection dc)
        {
            try
            {
               
                dl.InsertCollection(dc, out msg);
                TempData["msg"] = msg;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("Datacollection");
        }
    }
}
