using proiectDAW.Models;
using System.ComponentModel.DataAnnotations;

namespace proiectDAW.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Descrierea categoriei este obligatorie")]
        public string Description { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? NrBookmarks { get; set; }
        public virtual ICollection<BookmarkCategory>? BookmarkCategories { get; set; }
    }
}