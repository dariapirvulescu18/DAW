using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proiectDAW.Data;
using proiectDAW.Models;

namespace proiectDAW.Controllers
{
   
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {   
            SetAccessRights();
            var cat = db.Categories.Where(x => x.UserId == _userManager.GetUserId(User));
            foreach(Category c in cat)
            {
                c.NrBookmarks = db.BookmarkCategories.Where(a => a.CategoryId == c.Id).Count();
            }
            ViewBag.Categories = cat;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }


            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        { 
            Category cat = new Category();
            return View(cat);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Category cat)
        {
      
            cat.NrBookmarks = 0;

            cat.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                TempData["messageType"] = "alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Categoria nu a fost adaugata cu succes";
                TempData["messageType"] = " alert alert-danger";
                return View(cat);
            }
        }
       
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var cat = db.Categories.Include("BookmarkCategories.Bookmark")
                                   .Include("BookmarkCategories.Bookmark.User")
                                   .Include("User")
                                   .Where(a=> a.Id == id).First();

            cat.NrBookmarks = db.BookmarkCategories.Where(a=>a.CategoryId == cat.Id).Count();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            return View(cat);

               
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id) 
        { 
            Category cat = db.Categories.Where(a =>a.Id == id).First();

            SetAccessRights();

            if (cat.UserId == _userManager.GetUserId(User))
            {
                return View(cat);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei categorii care nu va apartine";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
         
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Category reqcat)
        {   
            Category cat = db.Categories.Where(a =>a.Id==id).First();

            if (ModelState.IsValid)
            {
                if(cat.UserId == _userManager.GetUserId(User))
                {
                    cat.CategoryName = reqcat.CategoryName;
                    cat.Description = reqcat.Description;
                    cat.NrBookmarks = db.BookmarkCategories.Where(a =>a.CategoryId==cat.Id).Count();

                    TempData["message"] = "Categoria a fost modificata cu succes";
                    TempData["messageType"] = "alert alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei categorii care nu va apartine";
                    TempData["messageType"] = "alert alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(reqcat);
            }
        

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {   
            Category cat = db.Categories.Include("BookmarkCategories")
                                        .Where(a => a.Id==id).First();
            if(_userManager.GetUserId(User) == cat.UserId || User.IsInRole("Admin"))
            {   
                if(cat.BookmarkCategories.Count > 0)
                {
                    foreach(var bmkcat in cat.BookmarkCategories)
                    {
                        db.BookmarkCategories.Remove(bmkcat);
                    }
                }


                db.Categories.Remove(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost stearsa";
                TempData["messageType"] = " alert alert-success";
                return RedirectToAction("Index");
            } 
            else 
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o categorie care nu va apartine";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult RemoveBookmark(int CategoryId, int BookmarkId)
        {
            /* ViewBag.CategoryId = CategoryId;
             ViewBag.BookmarkId = BookmarkId;

             return Redirect("/Bookmarks/Show/" + BookmarkId);*/
            Category cat = db.Categories.Include("BookmarkCategories")
                                      .Where(a => a.Id == CategoryId).First();

            if (_userManager.GetUserId(User) == cat.UserId)
            {

                foreach (var bmkcat in cat.BookmarkCategories)
                {
                    if (bmkcat.BookmarkId == BookmarkId)
                    {
                        db.BookmarkCategories.Remove(bmkcat);
                    }

                }


                db.SaveChanges();
                TempData["message"] = "Bookmark-ul a fost scos din categorie";
                TempData["messageType"] = " alert alert-success";
                return Redirect("/Categories/Show/" + cat.Id);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa scoateti acest bookmark din categorie";
                TempData["messageType"] = "alert alert-danger";
                return Redirect("/Categories/Show/" + cat.Id);
            }

        }
        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }


    }



}
