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
        public SubjectsController(ApplicationDbContext context)
        {
            db = context;
        }


        public IActionResult Index()
        {
            var subjects = db.Subjects.Include("Category");

            ViewBag.Subjects = subjects;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }


        public IActionResult Show(int id)
        {
            Subject subject = db.Subjects.Include("Category").Include("Answers")
                                         .Where(art => art.Id == id)
                                         .First();

            return View(subject);
        }


        [HttpPost]
        public IActionResult Show([FromForm] Answer answer)
        {
            answer.Date = DateTime.Now;

            try
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Subjects/Show/" + answer.SubjectId);
            }

            catch (Exception ex)
            {
                Subject sub = db.Subjects.Include("Category").Include("Answers")
                                         .Where(sub => sub.Id == answer.SubjectId)
                                         .First();

                return View(sub);
            }
        }


        public IActionResult New()
        {
            Subject subject = new Subject();

            subject.Categ = GetAllCategories();

            return View(subject);
        }


        [HttpPost]
        public IActionResult New(Subject subject)
        {
            subject.Date = DateTime.Now;
            subject.Categ = GetAllCategories();

            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                db.SaveChanges();
                TempData["message"] = "Subiectul a fost adaugat";
                return RedirectToAction("Index");
            }

            else
            {
                return View(subject);
            }
        }


        public IActionResult Edit(int id)
        {
            Subject subject = db.Subjects.Include("Category")
                                         .Where(sub => sub.Id == id)
                                         .First();

            subject.Categ = GetAllCategories();

            return View(subject);
        }


        [HttpPost]
        public IActionResult Edit(int id, Subject requestSubject)
        {
            Subject subject = db.Subjects.Find(id);
            requestSubject.Categ = GetAllCategories();

            if (ModelState.IsValid)
            {
                subject.Title = requestSubject.Title;
                subject.Content = requestSubject.Content;
                subject.CategoryId = requestSubject.CategoryId;
                TempData["message"] = "Subiectul a fost modificat";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            else
            {
                return View(requestSubject);
            }
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Subject subject = db.Subjects.Find(id);
            db.Subjects.Remove(subject);
            db.SaveChanges();
            TempData["message"] = "Subiectul a fost sters";
            return RedirectToAction("Index");
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
