using LearnForeignWords.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnForeignWords.Data
{
	public class WordTestContext: DbContext
	{
		public WordTestContext(DbContextOptions<WordTestContext> options) : base(options)
		{
		}

		public DbSet<Collection> Collections { get; set; }
		public DbSet<Language> Languages { get; set; }
		public DbSet<Word> Words { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Collection>().ToTable("Collection");
			modelBuilder.Entity<Language>().ToTable("Language");
			modelBuilder.Entity<Word>().ToTable("Word");
		}
	}
}
