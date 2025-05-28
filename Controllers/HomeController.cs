using Microsoft.AspNetCore.Mvc;

namespace OpenTable.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    
    }
}
