using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnForeignWords.Models
{
    public class Answer
    {
        public string UserAnswer { get; set; }
        public bool IsRightAnswer { get; set; }
    }
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

        public void CreateGameTest(List<Word> words)
        {
            Questions = new List<Question>();
            switch (TestType)
            {
                case TestType.TranslationWord:
                    foreach (var item in words)
                    {

                        List<string> list = new List<string>();
                        list.Add(item.Name.ToString());
                        for (int i = 0; i < 3;)
                        {
                            var str = words[new Random().Next(words.Count)].Name;
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

                    foreach (var item in words)
                    {
                        List<string> list = new List<string>();
                        list.Add(item.Meaning.ToString());
                        for (int i = 0; i < 3;)
                        {
                            var str = words[new Random().Next(words.Count)].Meaning;
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

                    foreach (var item in words)
                    {
                        Questions.Add(new Question() { Word = item });
                    }
                    Shuffle<Question>(Questions);
                    break;

            }

        }
        public bool CheckAnswer(string answer)
        {
            Questions[CurrentQuestion - 1].UserAnswer = answer;
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
                    default:
                    if (Questions[CurrentQuestion - 1].Word.Meaning == answer)
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

        public List<string> GetRightAnswers()
        {
            List<string> answer = new List<string>();
            foreach(Question question in Questions)
            {
                if (question.IsRightAnswer)
                    answer.Add(question.Word.Name+"-"+question.Word.Meaning);
            }
            return answer;
        }
        public List<Answer> GetAnswers()
        {
            List<Answer> answer = new List<Answer>();
            for (int i = 0; i < CurrentQuestion-1; i++)
            {
                if (this.TestType == TestType.WordTranslation)
                {
                    answer.Add(new Answer() {UserAnswer = $"{Questions[i].Word.Name} - {Questions[i].UserAnswer}", IsRightAnswer=Questions[i].IsRightAnswer} );
                }
                if (this.TestType == TestType.TranslationWord)
                {
                    answer.Add(new Answer() { UserAnswer = $"{Questions[i].Word.Meaning} - {Questions[i].UserAnswer}", IsRightAnswer = Questions[i].IsRightAnswer });
                }
                if (this.TestType == TestType.TextQuestion)
                {
                    answer.Add(new Answer() { UserAnswer = $"{Questions[i].Word.Name} - {Questions[i].UserAnswer}", IsRightAnswer = Questions[i].IsRightAnswer });
                }
            }
            return answer;
        }
        public List<string> GetWrongAnswers()
        {
            List<string> answer = new List<string>();
            for(int i=0;i<CurrentQuestion;i++)
            {
                if (!Questions[i].IsRightAnswer)
                    answer.Add(Questions[i].Word.Name + "-" + Questions[i].Word.Meaning);
            }
            return answer;
        }



    }
}
