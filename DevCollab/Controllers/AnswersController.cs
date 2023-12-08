using Microsoft.AspNetCore.Mvc;

namespace DevCollab.Controllers
{
    public class AnswersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
