using Microsoft.AspNetCore.Mvc;

namespace DevCollab.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
