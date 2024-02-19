using proiectDAW.Models;
using System.ComponentModel.DataAnnotations;
namespace proiectDAW.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Textul nu poate avea mai mult de 100 de caractere")]
        public string? Text { get; set; }

        public DateTime Date { get; set; }
        public int? BookmarkId { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual Bookmark? Bookmark { get; set; }
    }
}