using DevCollab.Models;
using DevCollab.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevCollab.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // stergerea unui raspuns asociat unei intrebari din baza de date
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Answer ans = db.Answers.Find(id);
            db.Answers.Remove(ans);
            db.SaveChanges();
            return Redirect("/Subjects/Show/" + ans.SubjectId);
        }

        // implementarea editarii intr-o pagina View separata
        // se editeaza un raspuns existent

        public IActionResult Edit(int id)
        {
            Answer ans = db.Answer.Find(id);

            return View(ans);
        }

        [HttpPost]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            Answer ans = db.Answers.Find(id);

            if (ModelState.IsValid)
            {

                ans.Content = requestAnswer.Content;

                db.SaveChanges();

                return Redirect("/Articles/Show/" + ans.SubjectId);
            }
            else
            {
                return View(requestAnswer);
            }

        }
    }
}