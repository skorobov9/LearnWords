using LearnForeignWords.Models;
using System;
using System.Linq;

namespace LearnForeignWords.Data
{
    public class DbInitializer
    {
        public static void Initialize(WordTestContext context)
        {

            context.Database.EnsureCreated();

			// Look for any students.
			if (context.Collections.Any())
			{
				return;   // DB has been seeded
			}


			context.SaveChanges();
            var themes = new Theme[]
            {
                new Theme{Name="Еда",Image="images/pexels-dana-tentis-262959.jpg"},
                new Theme{Name="Дом", Image="images/pexels-terje-sollie-298842.jpg"},
                new Theme{Name="Одежда",Image="images/pexels-mentatdgt-1336873.jpg"},
                new Theme{Name="Человек",Image="images/pexels-pixabay-207920.jpg"},
                new Theme{Name="Наука и образование",Image="images/pexels-jeshootscom-714699.jpg"},
                new Theme{Name="Транспорт",Image="images/pexels-pixabay-358622.jpg"},
                 new Theme{Name="Спорт",Image="images/pexels-pixabay-235922.jpg"},
                 new Theme{Name="Глаголы",Image="images/pexels-pixabay-40751.jpg"},
                  new Theme{Name="Существительные",Image="images/pexels-pixabay-302769.jpg"},
                   new Theme{Name="Прилагательные",Image="images/pexels-miguel-á-padriñán-194098.jpg"},
                   new Theme{Name="Природа",Image="images/pexels-ben-jessop-1659689.jpg"}
            };
            foreach (Theme t in themes)
            {
                context.Themes.Add(t);
            }
            context.SaveChanges();
            var collections = new Collection[]
            {

            new Collection{Name="Дом", ThemeId=2},
            new Collection{Name="Строительство",  ThemeId=2},
            new Collection{Name="Дом",  ThemeId=2},

            new Collection{Name="Одежда",  ThemeId=3},
            new Collection{Name="Одежда",  ThemeId=3},
            new Collection{Name="Украшения",  ThemeId=3},

            new Collection{Name="Части тела",  ThemeId=4},
            new Collection{Name="Болезни",  ThemeId=4},

            new Collection{Name="Космос",  ThemeId=5},
            new Collection{Name="Математика",  ThemeId=5},
            new Collection{Name="Школьные предметы",  ThemeId=5},
            new Collection{Name="Образование и наука",  ThemeId=5},

            new Collection{Name="Автомобиль",  ThemeId=6},
            new Collection{Name="Аэропорт",  ThemeId=6},
            new Collection{Name="Корабль",  ThemeId=6},

            new Collection{Name="Футбол",  ThemeId=7},
            new Collection{Name="Спорт",  ThemeId=7},

            new Collection{Name="Глаголы",  ThemeId=8},
            new Collection{Name="Глаголы",  ThemeId=8},
            new Collection{Name="Глаголы",  ThemeId=8},


            new Collection{Name="Существительные",  ThemeId=9},
            new Collection{Name="Существительные",  ThemeId=9},
            new Collection{Name="Существительные",  ThemeId=9},

             new Collection{Name="Прилагательные",  ThemeId=10},
            new Collection{Name="Прилагательные",  ThemeId=10},
            new Collection{Name="Прилагательные",  ThemeId=10},

             new Collection{Name="Животные",  ThemeId=11},
            new Collection{Name="Погода",  ThemeId=11},
            new Collection{Name="Растения",  ThemeId=11},

            new Collection{Name="Овощи и фрукты", ThemeId=1},
            new Collection{Name="Кухонные принадлежности",  ThemeId=1},
            new Collection{Name="Напитки",  ThemeId=1},
            new Collection{Name="Сладости",  ThemeId=1},
            new Collection{Name="Продукты",  ThemeId=1}

            };
            foreach (Collection c in collections)
            {
                context.Collections.Add(c);
            }
            context.SaveChanges();
            var col = context.Collections.FirstOrDefault(x => x.Name == "Овощи и фрукты");
            var words = new Word[]
            {
                //new Word{ Collection=col, Name="Orange", Meaning="Апельсин"},
                //new Word{ Collection=col, Name="Apple", Meaning="Яблоко"},
                //new Word{ Collection=col, Name="Pineapple", Meaning="Ананас"},
                //new Word{ Collection=col, Name="Banana", Meaning="Банан"},
                //new Word{ Collection=col, Name="Kiwi", Meaning="Киви"},
                //new Word{ Collection=col, Name="Mango", Meaning="Манго"},
                //new Word{ Collection=col, Name="Fig", Meaning="Инжир"},
                //new Word{ Collection=col, Name="Grapes", Meaning="Виноград"},
                //new Word{ Collection=col, Name="Melon", Meaning="Дыня"},
                //new Word{ Collection=col, Name="Lemon", Meaning="Лемон"}
            };
            foreach (Word w in words)
            {
                context.Words.Add(w);
            }
            context.SaveChanges();
        }
    }
}
