using Microsoft.AspNetCore.Mvc;

namespace _VEHRSv1.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult DatabaseConnection()
        {
            return View();
        }
    }
}
