using Microsoft.AspNetCore.Mvc;
using recyclecollection.Models;

namespace recyclecollection.Controllers
{
    public class landfillController : Controller
    {
        lanfilldb dbop = new lanfilldb();
        string msg;
        public IActionResult landfill()
        {
            return View("~/Views/Home/landfill.cshtml");
        }


        [HttpPost]
        public IActionResult Create([Bind] landfill gf)
        {
            try
            {
                
                dbop.Insertlandfill(gf, out msg);
                TempData["msg"] = msg;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("landfill");
        }

    }
}
