using Microsoft.AspNetCore.Mvc;

namespace WebApiBD.Controllers
{
    public class BazaDan : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
