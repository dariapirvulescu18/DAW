using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace proiectDAW.Models
{
    public class ApplicationUser : IdentityUser
    {

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Bookmark>? Bookmarks { get; set; }
        public virtual ICollection<Category>? Categories { get; set; }
        public virtual ICollection<UserLikesBookmark>? UserLikesBookmarks { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }
        public virtual ICollection<Video>? Videos { get; set; }
    }
}
