using Microsoft.AspNetCore.Mvc;

namespace MediPlan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
