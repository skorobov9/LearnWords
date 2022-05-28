namespace LearnForeignWords.Models
{
	public class UserWords
	{
		public int Id { get; set; }

		public string UserId { get; set; }
		public int WordId { get; set; }

		public bool IsLearned { get; set; }
		public bool IsFavorite { get; set; }

		public User User { get; set; }

		public int ErrorCount { get; set; }

		public int RightCount { get; set; }

		public Word Word { get; set; }

		
	}
}
