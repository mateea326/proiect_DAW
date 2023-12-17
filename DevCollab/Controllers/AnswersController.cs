using DevCollab.Models;
using DevCollab.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DevCollab.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public AnswersController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;

        }



        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Delete(int id)
        {
            Answer answer = db.Answers.Find(id);

            if (answer.UserId == _userManager.GetUserId(User)
              || User.IsInRole("Admin"))
            {
                db.Answers.Remove(answer);
                db.SaveChanges();
                return Redirect("/Subjects/Show/" + answer.SubjectId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Subjects");
            }
        }


        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Answer answer = db.Answers.Find(id);

            if (answer != null && (answer.UserId == _userManager.GetUserId(User))
              || User.IsInRole("Admin"))
            {
                return View(answer);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Subjects");
            }
                
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            Answer answer = db.Answers.Find(id);

            if (answer.UserId == _userManager.GetUserId(User)
              || User.IsInRole("Admin"))
            {
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
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Subjects");
            }

        }
    }
}