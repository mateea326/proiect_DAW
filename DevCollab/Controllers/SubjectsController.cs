using DevCollab.Controllers;
using DevCollab.Data;
using DevCollab.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;

namespace DevCollab.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public SubjectsController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;

        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var subjects = db.Subjects.Include("Category").Include("User");

            ViewBag.Subjects = subjects;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            Subject subject = db.Subjects.Include("Category")
                                         .Include("User")
                                         .Include("Answers")
                                         .Include("Answers.User")
                              .Where(sub => sub.Id == id)
                              .First();
            SetAccessRights();

            return View(subject);
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Answer answer)
        {
            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Subjects/Show/" + answer.SubjectId);
            }

            else
            {
                Subject sub = db.Subjects.Include("Category")
                                         .Include("User")
                                         .Include("Answers")
                                         .Include("Answers.User")
                              .Where(sub => sub.Id == answer.SubjectId)
                              .First();

                SetAccessRights();

                return View(sub);
            }
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            Subject subject = new Subject();

            subject.Categ = GetAllCategories();

            return View(subject);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpPost]
        public IActionResult New(Subject subject)
        {
            subject.Date = DateTime.Now;
            subject.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                db.SaveChanges();
                TempData["message"] = "Subiectul a fost adăugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                subject.Categ = GetAllCategories();
                return View(subject);
            }
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Subject subject = db.Subjects.Include("Category")
                                         .Where(sub => sub.Id == id)
                                         .First();

            subject.Categ = GetAllCategories();

            if(subject.UserId == _userManager.GetUserId(User) 
              || User.IsInRole("Admin"))
            {
                return View(subject);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui subiect care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Subject requestSubject)
        {
            Subject subject = db.Subjects.Find(id);
            

            if (ModelState.IsValid)
            {
                if (subject.UserId == _userManager.GetUserId(User)
                    || User.IsInRole("Admin"))
                {
                    subject.Title = requestSubject.Title;
                    subject.Content = requestSubject.Content;
                    subject.CategoryId = requestSubject.CategoryId;
                    TempData["message"] = "Subiectul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui subiect care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }

            else
            {
                requestSubject.Categ = GetAllCategories();
                return View(requestSubject);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Subject subject = db.Subjects.Include("Answers")
                                .Where(sub => sub.Id == id)
                                .First();

            if(subject.UserId == _userManager.GetUserId(User)
               || User.IsInRole("Admin"))
            {
                db.Subjects.Remove(subject);
                db.SaveChanges();
                TempData["message"] = "Subiectul a fost șters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un subiect care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }   
        }


        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor")
               || User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            
            return selectList;
        }


        public IActionResult IndexNou()
        {
            return View();
        }
    }
}
