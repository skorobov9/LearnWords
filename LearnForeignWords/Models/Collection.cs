namespace LearnForeignWords.Models
{
	public class Collection
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int ColLevel { get; set; }
		public int LanguageId { get; set; }
		public int? OwnerId { get; set; }

		public Language Language { get; set; }

	}
}
