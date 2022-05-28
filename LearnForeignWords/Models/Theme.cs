using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnForeignWords.Models
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Image { get; set; }

        public List<Collection> Collections { get; set; } = new();

        public List<Word> AllWords()
        {
            var words = new List<Word>();
            foreach (var item in Collections)
            {
                words.AddRange(item.Words);
            }
            return words;
        }

        public Collection UnionCollections()
        {
            var collection = new Collection() { Name="Все слова", ThemeId=this.Id};
            foreach (var item in Collections)
            {
                collection.Words.AddRange(item.Words);
            }
            return collection;
        }
    }
}
