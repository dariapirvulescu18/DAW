using System.ComponentModel.DataAnnotations.Schema;

namespace proiectDAW.Models
{
    public class BookmarkCategory
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? BookmarkId { get; set; }
        public int? CategoryId { get; set; }
        public virtual Bookmark? Bookmark { get; set; }
        public virtual Category? Category { get; set; }
        public DateTime BookmarkCategoryDate { get; set; }
    }
}
