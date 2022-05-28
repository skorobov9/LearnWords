using LearnForeignWords.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace LearnForeignWords.Data
{
	public class WordTestContext: IdentityDbContext<User>
	{
		public WordTestContext(DbContextOptions<WordTestContext> options) : base(options)
		{
		}

		public DbSet<Collection> Collections { get; set; }
		public DbSet<Word> Words { get; set; }
		public DbSet<Theme> Themes { get; set; }
		public new DbSet<User> Users { get; set; }

		public DbSet<UserWords> UserWords { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<Collection>().ToTable("Collections");
			modelBuilder.Entity<Language>().ToTable("Languages");
			modelBuilder.Entity<Word>().ToTable("Words");
			modelBuilder.Entity<Theme>().ToTable("Themes");
		}
	}
}
