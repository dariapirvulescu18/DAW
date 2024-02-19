using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using proiectDAW.Data;
using proiectDAW.Models;

namespace proiectDAW.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id)
        {
            Comment com = db.Comments.Find(Id);
            if (com.UserId == _userManager.GetUserId(User))
            {
                return View(com);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }

        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int Id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(Id);

            if (comm.UserId == _userManager.GetUserId(User))
            {
                if (ModelState.IsValid)
                {
                    comm.Text = requestComment.Text;

                    db.SaveChanges();

                    TempData["message"] = "Comentariul a fost editat cu succes";
                    TempData["messageType"] = "alert alert-success";

                    return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int Id)
        {
            Comment comm = db.Comments.Find(Id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters cu succes";
                TempData["messageType"] = "alert alert-success";
                return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
                TempData["messageType"] = "alert alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }
    }

}