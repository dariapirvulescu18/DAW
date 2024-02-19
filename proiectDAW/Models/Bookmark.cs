using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proiectDAW.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(50, ErrorMessage = "Titlul nu poate avea mai mult de 50 de caractere")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [StringLength(100, ErrorMessage = "Descrierea nu poate avea mai mult de 100 de caractere")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fotografia de coperta este obligatorie")]
        public string Photo_Cover { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }
        public virtual ICollection<Video>? Videos { get; set; }
        public virtual ICollection<BookmarkCategory>? BookmarkCategories { get; set; }
        public virtual ICollection<UserLikesBookmark>? UserLikesBookmarks { get; set; }


    }
}