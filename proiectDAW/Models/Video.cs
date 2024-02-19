using System.ComponentModel.DataAnnotations;

namespace proiectDAW.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string? URL { get; set; }
        public int? BookmarkId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
