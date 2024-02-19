using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proiectDAW.Data;
using proiectDAW.Models;



namespace proiectDAW.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
                ViewBag.MsgType = TempData["messageType"];
            }
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> Show(string id)
        {

            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {

            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user);

            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First();
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.PhoneNumber = newData.PhoneNumber;


                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost modificat cu succes";
                TempData["messageType"] = "alert alert-success";

            }
            else
            {
                TempData["message"] = "A aparut o eroare la modificarea utilizatorului";
                TempData["messageType"] = "alert alert-danger";
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include("Categories")
                         .Include("Comments")
                         .Include("Bookmarks")
                         .Include("Photos")
                         .Include("Videos")
                         .Where(u => u.Id == id)
                         .First();

            if (user.Comments.Count > 0)
            {
                foreach (var comment in user.Comments)
                {
                    db.Comments.Remove(comment);
                }
            }

            if (user.Bookmarks.Count > 0)
            {
                foreach (var bookmark in user.Bookmarks)
                {
                    db.Bookmarks.Remove(bookmark);
                }
            }

            if (user.Categories.Count > 0)
            {
                foreach (var category in user.Categories)
                {
                    db.Categories.Remove(category);
                }
            }

            if (user.Photos.Count > 0)
            {
                foreach (var photo in user.Photos)
                {
                    db.Photos.Remove(photo);
                }
            }

            if (user.Videos.Count > 0)
            {
                foreach (var video in user.Videos)
                {
                    db.Videos.Remove(video);
                }
            }

            TempData["message"] = "Utilizatorul a fost sters cu succes";
            TempData["messageType"] = "alert alert-success";
            db.ApplicationUsers.Remove(user);

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ShowBookmarks(string id)
        {
            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteInregistrat = User.IsInRole("User");

            ViewBag.UserCurent = _userManager.GetUserId(User);

            var bmks = db.Bookmarks.Include("User")
                                   .Include("UserLikesBookmarks")
                                   .Where(bmk => bmk.UserId == id);

            ViewBag.UserLikes = db.UserLikesBookmarks.Where(ab => ab.UserId == _userManager.GetUserId(User));

            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.Bookmarks = bmks;

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ShowCategories(string id)
        {
            var cat = db.Categories.Include("User")
                                   .Where(x => x.UserId == id);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.Categories = cat;
            return View();
        }
    }
}