using System;
using System.Collections.Generic;

namespace LearnForeignWords.Models
{
    public class Question: ICloneable
    {
        public Word Word { get; set; }

        public List<string> Answers { get; set; }

        public bool IsRightAnswer { get; set; }

        public string UserAnswer { get; set; }
        public object Clone()
        {
            return new Question() { Word = Word, Answers = Answers };
        }
    }
}
