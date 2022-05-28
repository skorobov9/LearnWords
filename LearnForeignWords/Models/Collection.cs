using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnForeignWords.Models
{
	public class Collection
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string OwnerId { get; set; }
		public int? ThemeId { get; set; }

		public string Language { get; set; } = "en";

		[NotMapped]
		public int local { get; set; }
		
		public Theme Theme { get; set; }	
		public List<Word> Words { get; set; } = new();

	}
}
