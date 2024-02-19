using System.ComponentModel.DataAnnotations.Schema;

namespace proiectDAW.Models
{
    public class UserLikesBookmark
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? BookmarkId { get; set; }
        public string? UserId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
