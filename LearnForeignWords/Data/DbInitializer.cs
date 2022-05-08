using LearnForeignWords.Models;
using System;
using System.Linq;

namespace LearnForeignWords.Data
{
	public class DbInitializer
	{
        public static void Initialize(WordTestContext context)
        {
            //context.Database.EnsureCreated();

            // Look for any students.
            if (context.Collections.Any())
            {
                return;   // DB has been seeded
            }
            
            var languages = new Language[]
            {
            new Language{Name="English"}    
            };
            foreach (Language l in languages)
            {
                context.Languages.Add(l);
            }
            context.SaveChanges();

            var collections = new Collection[]
            {
            new Collection{Name="Города", ColLevel=0,  LanguageId=1},
            new Collection{Name="Дом", ColLevel=0,  LanguageId=1},
            new Collection{Name="Еда", ColLevel=0,  LanguageId=1}
            };
            foreach (Collection c in collections)
            {
                context.Collections.Add(c);
            }
            context.SaveChanges();

            
            

          
        }
    }
}
