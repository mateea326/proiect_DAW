using DevCollab.Models;
using DevCollab.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevCollab.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public AnswersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();
            return Redirect("/Subjects/Show/" + answer.SubjectId);
        }

        

        public IActionResult Edit(int id)
        {
            Answer answer = db.Answers.Find(id);

            return View(answer);
        }

        [HttpPost]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            Answer answer = db.Answers.Find(id);

            if (ModelState.IsValid)
            {

                answer.Content = requestAnswer.Content;

                db.SaveChanges();

                return Redirect("/Subjects/Show/" + answer.SubjectId);
            }
            else
            {
                return View(requestAnswer);
            }

        }
    }
}