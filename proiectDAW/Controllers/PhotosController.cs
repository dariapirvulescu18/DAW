using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using proiectDAW.Data;
using proiectDAW.Models;

namespace proiectDAW.Controllers
{
    public class PhotosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PhotosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New([FromForm] Photo photo)
        {
            Bookmark bmk = db.Bookmarks.Find(photo.BookmarkId);
            if (_userManager.GetUserId(User) == photo.UserId && bmk.Id == photo.BookmarkId && bmk.UserId == photo.UserId) 
                //se poate adauga o fotografie doar daca User-ul este si proprietarul Bookmark-ului
            {
                if (ModelState.IsValid)
                {
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    TempData["message"] = "Fotografia a fost adaugata";
                    TempData["messageType"] = "alert alert-success";
                    return Redirect("/Bookmarks/Show/" + photo.BookmarkId);
                }
                else
                {
                    TempData["message"] = "Fotografia nu a fost adaugata cu succes";
                    TempData["messageType"] = " alert alert-danger";
                    return Redirect("/Bookmarks/Show/" + photo.BookmarkId);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a adauga o fotografie la un bookmark care nu va apartine";
                TempData["messageType"] = " alert alert-danger";
                return Redirect("/Bookmarks/Show/" + photo.BookmarkId);
            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id)
        {
            Photo photo = db.Photos.Find(Id);
            Bookmark bmk = db.Bookmarks.Find(photo.BookmarkId);
            if (_userManager.GetUserId(User) == photo.UserId && bmk.Id == photo.BookmarkId && bmk.UserId == photo.UserId)
            {
                return View(photo);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati fotografia";
                TempData["messageType"] = "alert alert-danger";
                return Redirect("/Bookmarks/Show/" + bmk.Id);
            }

        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id, Photo requestPhoto)
        {
            Photo photo = db.Photos.Find(Id);
            Bookmark bmk = db.Bookmarks.Find(photo.BookmarkId);

            if (_userManager.GetUserId(User) == photo.UserId && bmk.Id == photo.BookmarkId && bmk.UserId == photo.UserId)
            {
                if (ModelState.IsValid)
                {
                    photo.URL = requestPhoto.URL;


                    db.SaveChanges();

                    TempData["message"] = "Imaginea a fost editata cu succes";
                    TempData["messageType"] = "alert alert-success";

                    return Redirect("/Bookmarks/Show/" + bmk.Id);
                }
                else
                {
                    return View(requestPhoto);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                TempData["messageType"] = "alert alert-danger";
                return Redirect("/Bookmarks/Show/" + bmk.Id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int Id)
        {
            Photo photo = db.Photos.Find(Id);

            if (photo.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Photos.Remove(photo);
                db.SaveChanges();
                TempData["message"] = "Fotografia a fost stearsa cu succes";
                TempData["messageType"] = "alert alert-success";
                return Redirect("/Bookmarks/Show/" + photo.BookmarkId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti fotografia";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }
    }
}