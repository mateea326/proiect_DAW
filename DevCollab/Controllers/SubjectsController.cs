﻿using DevCollab.Controllers;
using DevCollab.Data;
using DevCollab.Models;
using Humanizer;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;   
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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

        public IActionResult Index()
        {
          
            int _perPage = 3;
            var subjects = db.Subjects.Include("Category")
                                      .Include("User")
                                      .Include("User")
                                      .OrderBy(a => a.Date);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            int totalItems = subjects.Count();
            var currentPage =Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedSubjects = subjects.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems /(float)_perPage);
            ViewBag.Subjects = paginatedSubjects;
            return View();
        }
        public IActionResult Show(int id)
        {
            Subject subject = db.Subjects.Include("Category")
                                         .Include("User")
                                         .Include("Answers")
                                         .Include("Answers.User")
                              .Where(sub => sub.Id == id)
                              .FirstOrDefault();

            if (subject == null)
            {
                return NotFound(); 
            }

            SetAccessRights();

            return View(subject);
        }


        [HttpPost]
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

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Subject subject = new Subject();

            subject.Categ = GetAllCategories();

            return View(subject);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Subject subject)
        {
            var sanitizer = new HtmlSanitizer();

            subject.Content = sanitizer.Sanitize(subject.Content);
            subject.Content = (subject.Content);


            subject.Date = DateTime.Now;
            subject.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Subjects.Add(subject);
                db.SaveChanges();
                TempData["message"] = "Subiectul a fost adăugat";
                return Redirect("/Categories/Show/" + subject.CategoryId);
            }

            else
            {
                subject.Categ = GetAllCategories();
                return View(subject);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Subject subject = db.Subjects.Include("Category")
                                         .Where(sub => sub.Id == id)
                                         .FirstOrDefault();

            if (subject == null)
            {
                return NotFound();
            }

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
                return Redirect("/Categories/Show/" + subject.CategoryId);
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Subject requestSubject)
        {
            var sanitizer = new HtmlSanitizer();

            Subject subject = db.Subjects.Find(id);
            
            if (ModelState.IsValid)
            {
                if (subject.UserId == _userManager.GetUserId(User)
                    || User.IsInRole("Admin"))
                {
                    subject.Title = requestSubject.Title;

                    requestSubject.Content =sanitizer.Sanitize(requestSubject.Content);

                    subject.Content = requestSubject.Content;
                    subject.CategoryId = requestSubject.CategoryId;
                    TempData["message"] = "Subiectul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return Redirect("/Categories/Show/" + subject.CategoryId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui subiect care nu va apartine";
                    return Redirect("/Categories/Show/" + subject.CategoryId);
                }
            }

            else
            {
                requestSubject.Categ = GetAllCategories();
                return View(requestSubject);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
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
                return Redirect("/Categories/Show/" + subject.CategoryId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un subiect care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Categories/Show/" + subject.CategoryId);
            }   
        }


        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
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
