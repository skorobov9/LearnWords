namespace LearnForeignWords.Models
{
	public class Word
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CollectionId { get; set; }
		public string Meaning { get; set; }

		public Collection Collection { get; set; }

	}
}
