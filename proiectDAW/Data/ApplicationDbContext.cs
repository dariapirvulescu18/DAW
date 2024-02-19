using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using proiectDAW.Models;

namespace proiectDAW.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Video> Videos { get; set; }

        public DbSet<BookmarkCategory> BookmarkCategories { get; set; }
        public DbSet<UserLikesBookmark> UserLikesBookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookmarkCategory>()
            .HasKey(ab => new
            {
                ab.Id,
                ab.BookmarkId,
                ab.CategoryId
            });

            modelBuilder.Entity<UserLikesBookmark>()
           .HasKey(ab => new
           {
               ab.Id,
               ab.BookmarkId,
               ab.UserId
           });

            modelBuilder.Entity<BookmarkCategory>()
               .HasOne(ab => ab.Bookmark)
               .WithMany(ab => ab.BookmarkCategories)
               .HasForeignKey(ab => ab.BookmarkId);

            modelBuilder.Entity<BookmarkCategory>()
                .HasOne(ab => ab.Category)
                .WithMany(ab => ab.BookmarkCategories)
                .HasForeignKey(ab => ab.CategoryId);

            modelBuilder.Entity<UserLikesBookmark>()
               .HasOne(ab => ab.Bookmark)
               .WithMany(ab => ab.UserLikesBookmarks)
               .HasForeignKey(ab => ab.BookmarkId);

            modelBuilder.Entity<UserLikesBookmark>()
                .HasOne(ab => ab.User)
                .WithMany(ab => ab.UserLikesBookmarks)
                .HasForeignKey(ab => ab.UserId);

            modelBuilder.Entity<Bookmark>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Bookmark)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bookmark>()
            .HasMany(p => p.Photos)
            .WithOne(c => c.Bookmark)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bookmark>()
            .HasMany(p => p.Videos)
            .WithOne(c => c.Bookmark)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bookmark>()
            .HasMany(p => p.UserLikesBookmarks)
            .WithOne(c => c.Bookmark)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(p => p.UserLikesBookmarks)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}