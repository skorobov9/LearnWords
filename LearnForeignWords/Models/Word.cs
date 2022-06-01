using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnForeignWords.Models
{
	public class Word
	{
		public int Id { get; set; }

		[Column("Word")]
		public string Name { get; set; }
		public int CollectionId { get; set; }
		public string Meaning { get; set; }

		public string Image { get; set; }
		public Collection Collection { get; set; }

		public List<UserWords> UserWords { get; set; }

	}
}
