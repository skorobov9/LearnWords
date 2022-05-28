using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnForeignWords.Models
{
    public enum TestType
    {
        WordTranslation = 1,
        TranslationWord = 2,
        TextQuestion = 3
    }
    public class Test
    {

        public Collection Collection { get; set; }


        public List<Question> Questions { get; set; }

        public User User { get; set; }

        public TestType TestType { get; set; }

        public int CountRightAnswers { get; set; }

        public int CurrentQuestion { get; set; }
        public DateTime StartedOn { get; set; } = DateTime.UtcNow;

        public void CreateTest()
        {
            Questions = new List<Question>(); 
            List<Word> wlist = new List<Word>();
            int count = Collection.Words.Where(w => !w.UserWords.Where(w => w.UserId == User?.Id && w.IsLearned).Any()).Count();
            if (count < 10)
                return;
                    for (int i = 0; i < 10;)
                        {
                            var str = Collection.Words[new Random().Next(Collection.Words.Count)]; 
                        if (str.UserWords.Where(w => w.UserId == User?.Id && w.IsLearned).Any())
                            continue; 
                        if (!wlist.Contains(str))
                            {
                                wlist.Add(str);
                                i++;
                            }
                        }
            switch (TestType)
            {
                case TestType.TranslationWord:
                    foreach (var item in wlist)
                    {
                        
                        List<string> list = new List<string>();
                        list.Add(item.Name.ToString());
                        for (int i = 0; i < 3;)
                        {
                            var str = Collection.Words[new Random().Next(Collection.Words.Count)].Name;
                            if (!list.Contains(str))
                            {
                                list.Add(str);
                                i++;
                            }
                        }
                        Shuffle<string>(list);


                        Questions.Add(new Question() { Word = item, Answers = list });

                    }
                    Shuffle<Question>(Questions);
                    break;

                case TestType.WordTranslation:

                    foreach (var item in wlist)
                    {
                        List<string> list = new List<string>();
                        list.Add(item.Meaning.ToString());
                        for (int i = 0; i < 3;)
                        {
                            var str = Collection.Words[new Random().Next(Collection.Words.Count)].Meaning;
                            if (!list.Contains(str))
                            {
                                list.Add(str);
                                i++;
                            }
                        }
                        Shuffle<string>(list);


                        Questions.Add(new Question() { Word = item, Answers = list });

                    }
                    Shuffle<Question>(Questions);
                    break;
                case TestType.TextQuestion:
                    
                    foreach (var item in wlist)
                    {
                        Questions.Add(new Question() { Word = item});
                    }
                    Shuffle<Question>(Questions);
                    break;




            }


        }

        public bool CheckAnswer(string answer)
        {
            switch (this.TestType)
            {
                case TestType.WordTranslation:
                    if (Questions[CurrentQuestion - 1].Word.Meaning == answer)
                    {
                        Questions[CurrentQuestion - 1].IsRightAnswer = true;
                        CountRightAnswers++;
                        return true;
                    }
                    break;
                case TestType.TranslationWord:
                    if (Questions[CurrentQuestion - 1].Word.Name == answer)
                    {
                        Questions[CurrentQuestion - 1].IsRightAnswer = true;
                        CountRightAnswers++;
                        return true;
                    }
                    break;
                case TestType.TextQuestion:
                    if (Questions[CurrentQuestion - 1].Word.Name.ToLower() == answer.ToLower())
                    {
                        Questions[CurrentQuestion - 1].IsRightAnswer = true;
                        CountRightAnswers++;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public Question GetNextQuestion()
        {
            if (CurrentQuestion >= Questions.Count)
            {
                return null;
            }
            var result = Questions[CurrentQuestion++];
            return result;
        }

        public void Shuffle<T>(List<T> arr)
        {
            // создаем экземпляр класса Random для генерирования случайных чисел
            Random rand = new Random();

            for (int i = arr.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = arr[j];
                arr[j] = arr[i];
                arr[i] = tmp;
            }
        }


    }
}
