using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proiectDAW.Data;
using proiectDAW.Models;
using System.Text.RegularExpressions;

namespace proiectDAW.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public BookmarksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            var search = "";
            var bmk = db.Bookmarks.Include("User")
                                  .Include("UserLikesBookmarks")
                                  .OrderByDescending(a => a.UserLikesBookmarks.Count)
                                  .ThenByDescending(a => a.Date);

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<int> bookmarkIds = db.Bookmarks
                                          .Where(bmk => bmk.Title.Contains(search) || bmk.Description.Contains(search))
                                          .Select(bmk => bmk.Id)
                                          .ToList();

                bmk = db.Bookmarks.Where(bmk =>
                bookmarkIds.Contains(bmk.Id))
                .Include("User")
                .Include("UserLikesBookmarks")
                .OrderBy(a => a.Title);
            }

            int _perPage = 9;
            ViewBag.SearchString = search;
            int totalItems = bmk.Count();


            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            ViewBag.PaginaCurenta = currentPage;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedBookmarks = bmk.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.Bookmarks = paginatedBookmarks;
            ViewBag.UserLikes = db.UserLikesBookmarks.Where(ab => ab.UserId == _userManager.GetUserId(User));

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?page";
            }

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int Id)
        {
            Bookmark bmk = db.Bookmarks
                                        .Include("User")
                                        .Include("Comments")
                                        .Include("Comments.User")
                                        .Include("Photos")
                                        .Include("Videos")
                                        .Where(bm => bm.Id == Id)
                                        .First();
            ViewBag.UserCategories = db.Categories
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }

            return View(bmk);

        }

        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int Id)
        {

            Bookmark bmk = db.Bookmarks.Where(bm => bm.Id == Id)
                                       .First();


            if (bmk.UserId == _userManager.GetUserId(User))
            {
                return View(bmk);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui bookmark care nu va apartine";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int Id, Bookmark reqbmk)
        {
            Bookmark bmk = db.Bookmarks.Where(bm => bm.Id == Id)
                                       .First();

            if (ModelState.IsValid)
            {
                if (bmk.UserId == _userManager.GetUserId(User))
                {
                    bmk.Title = reqbmk.Title;
                    bmk.Description = reqbmk.Description;
                    bmk.Photo_Cover = reqbmk.Photo_Cover;

                    TempData["message"] = "Bookmark-ul a fost modificat cu succes";
                    TempData["messageType"] = "alert alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui bookmark care nu va apartine";
                    TempData["messageType"] = "alert alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(reqbmk);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost adaugat";
                TempData["messageType"] = " alert alert-success";
                return Redirect("/Bookmarks/Show/" + comment.BookmarkId);
            }

            else
            {
                Bookmark bmk = db.Bookmarks.Include("Photos")
                                          .Include("Videos")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(b => b.Id == comment.BookmarkId)
                                         .First();

                ViewBag.UserCategories = db.Categories
                                          .Where(b => b.UserId == _userManager.GetUserId(User))
                                          .ToList();

                SetAccessRights();

                return View(bmk);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int Id)
        {
            Bookmark bmk = db.Bookmarks.Include("Comments")
                                         .Where(art => art.Id == Id)
                                         .First();

            if (bmk.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Bookmarks.Remove(bmk);
                db.SaveChanges();
                TempData["message"] = "Bookmark-ul a fost sters";
                TempData["messageType"] = " alert alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un bookmark care nu va apartine";
                TempData["messageType"] = " alert alert-danger";
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {

            Bookmark bmk = new Bookmark();

            return View(bmk);
        }



        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Bookmark bmk)
        {

            bmk.Date = DateTime.Now;

            bmk.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Bookmarks.Add(bmk);
                db.SaveChanges();
                TempData["message"] = "Bookmark-ul a fost adaugat";
                TempData["messageType"] = "alert alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Bookmark-ul nu a fost adaugat cu succes";
                TempData["messageType"] = " alert alert-danger";
                return View(bmk);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Like([FromForm] UserLikesBookmark ulb)
        {
            if (ModelState.IsValid)
            {
                if (db.UserLikesBookmarks
                    .Where(ab => ab.UserId == ulb.UserId)
                    .Where(ab => ab.BookmarkId == ulb.BookmarkId)
                    .Count() > 0)
                {
                    var like = db.UserLikesBookmarks.Where(ab => ab.UserId == ulb.UserId)
                                                    .Where(ab => ab.BookmarkId == ulb.BookmarkId)
                                                    .First();
                    db.UserLikesBookmarks.Remove(like);
                    db.SaveChanges();
                }
                else
                {
                    db.UserLikesBookmarks.Add(ulb);
                    db.SaveChanges();
                }
            }
            else
            {
                TempData["message"] = "Nu s-a putut efectua actiunea";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Bookmarks/Index");
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult AddCategory([FromForm] BookmarkCategory bmkcat)
        {

            if (ModelState.IsValid)
            {

                if (db.BookmarkCategories
                    .Where(ab => ab.CategoryId == bmkcat.CategoryId)
                    .Where(ab => ab.BookmarkId == bmkcat.BookmarkId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest bookmark este deja adaugat in categorie";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.BookmarkCategories.Add(bmkcat);

                    var cat = db.Categories.Where(a => a.Id == bmkcat.CategoryId).First();
                    cat.NrBookmarks = db.BookmarkCategories.Where(a => a.CategoryId == cat.Id).Count();



                    db.SaveChanges();


                    TempData["message"] = "Bookmark-ul a fost adaugat in categoria selectata";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga bookmarkul in categorie";
                TempData["messageType"] = "alert-danger";
            }


            return Redirect("/Bookmarks/Show/" + bmkcat.BookmarkId);
        }



        private void SetAccessRights()
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteInregistrat = User.IsInRole("User");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}