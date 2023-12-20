using DevCollab.Data;
using DevCollab.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCollab.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public CategoriesController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;

        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            SetAccessRights();
            return View();
        }
        
       
        public IActionResult Show(int id, int? page = 1)
        {
            Category? category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound(); 
            }
            int _perPage = 3;
            var subjects = db.Subjects.Include("Category")
                                      .Include("User")
                                      .Where(a => a.CategoryId == id)
                                      .OrderBy(a => a.Date);
            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) !=null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<int> subjectIds = db.Subjects.Where
                         (
                         sb => sb.Title.Contains(search)
                         || sb.Content.Contains(search)
                         ).Select(s => s.Id).ToList();
                // Cautare in raspunsuri (Content)
                List<int> subjectIdsOfAnswersWithSearchString = db.Answers.Where
                    (
                    a => a.Content.Contains(search)
                    ).Select(a => (int)a.SubjectId).ToList();
                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds =subjectIds.Union(subjectIdsOfAnswersWithSearchString).ToList();
                // Lista subiectelor care contin cuvantul cautat
                // fie in subiect -> Title si Content
                // fie in raspunsuri -> Content
                subjects = db.Subjects.Where(subject =>
                        mergedIds.Contains(subject.Id))
                         .Include("Category")
                         .Include("User")
                         .OrderBy(a => a.Date);
            }

            ViewBag.SearchString = search;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            int totalItems = subjects.Count();
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedSubjects = subjects.Skip(offset).Take(_perPage);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Subjects = paginatedSubjects;
            SetAccessRights();

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Categories/Show/" + id + "/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Categories/Show/" + id + "/?page";
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category cat)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adăugată";
                return RedirectToAction("Index");
            }

            else
            {
                return View(cat);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Categories.Find(id);

            if (ModelState.IsValid)
            {

                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();
                TempData["message"] = "Categoria a fost modificată";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Include("Subjects")
                                             .Include("Subjects.Answers")
                                             .Where(cat => cat.Id == id)
                                             .First();
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost ștearsă";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
