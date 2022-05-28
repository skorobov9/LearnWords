using System.Collections.Generic;

namespace LearnForeignWords.Models
{
    public class Question
    {
        public Word Word { get; set; }

        public List<string> Answers { get; set; }

        public bool IsRightAnswer { get; set; }


    }
}
