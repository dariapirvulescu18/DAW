using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using proiectDAW.Data;
using proiectDAW.Models;

namespace proiectDAW.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public VideosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New([FromForm] Video video)
        {
            Bookmark bmk = db.Bookmarks.Find(video.BookmarkId);
            if (_userManager.GetUserId(User) == video.UserId && bmk.Id == video.BookmarkId && bmk.UserId == video.UserId)
            //se poate adauga o fotografie doar daca User-ul este si proprietarul Bookmark-ului
            {
                if (ModelState.IsValid)
                {
                    db.Videos.Add(video);
                    db.SaveChanges();
                    TempData["message"] = "Videoclipul a fost adaugat";
                    TempData["messageType"] = "alert alert-success";
                    return Redirect("/Bookmarks/Show/" + video.BookmarkId);
                }
                else
                {
                    TempData["message"] = "Videoclipul nu a fost adaugat cu succes";
                    TempData["messageType"] = " alert alert-danger";
                    return Redirect("/Bookmarks/Show/" + video.BookmarkId);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a adauga un videoclip la un bookmark care nu va apartine";
                TempData["messageType"] = " alert alert-danger";
                return Redirect("/Bookmarks/Show/" + video.BookmarkId);
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id)
        {
            Video video = db.Videos.Find(Id);
            Bookmark bmk = db.Bookmarks.Find(video.BookmarkId);
            if (_userManager.GetUserId(User) == video.UserId && bmk.Id == video.BookmarkId && bmk.UserId == video.UserId)
            {
                return View(video);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati videoclipul";
                TempData["messageType"] = "alert alert-danger";
                return Redirect("/Bookmarks/Show/" + bmk.Id );
            }

        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id, Video requestVideo)
        {
            Video video = db.Videos.Find(Id);
            Bookmark bmk = db.Bookmarks.Find(video.BookmarkId);

            if (_userManager.GetUserId(User) == video.UserId && bmk.Id == video.BookmarkId && bmk.UserId == video.UserId)
            {
                if (ModelState.IsValid)
                {
                    video.URL = requestVideo.URL;
                        

                    db.SaveChanges();

                    TempData["message"] = "Videoclipul a fost editat cu succes";
                    TempData["messageType"] = "alert alert-success";

                    return Redirect("/Bookmarks/Show/" + bmk.Id);
                }
                else
                {
                    return View(requestVideo);
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
            Video video = db.Videos.Find(Id);

            if (video.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Videos.Remove(video);
                db.SaveChanges();
                TempData["message"] = "Videoclipul a fost stears cu succes";
                TempData["messageType"] = "alert alert-success";
                return Redirect("/Bookmarks/Show/" + video.BookmarkId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti videoclipul";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }
    }
}