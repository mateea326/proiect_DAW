using Microsoft.AspNetCore.Mvc;

namespace DevCollab.Controllers
{
    public class SubjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
