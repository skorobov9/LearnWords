using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnForeignWords.Data;
using LearnForeignWords.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace LearnForeignWords.Controllers
{
    public class TestController : Controller
    {
        private readonly WordTestContext _context;
        private readonly UserManager<User> _userManager;

        public TestController(UserManager<User> userManager, WordTestContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .Include(c => c.Words)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }
        public async Task<IActionResult> IndexAll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theme = await _context.Themes
                .Include(c => c.Collections).ThenInclude(c => c.Words)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (theme == null)
            {
                return NotFound();
            }
            var col = theme.UnionCollections();
            col.local = 1;
            //HttpContext.Session.Set<Collection>("Collection", col);

            return View("Index", col);
        }
        public async Task<IActionResult> IndexAdditionally(int? id,string lang)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return NotFound();
            var lwords = _context.Words.Include(w => w.Collection).Where(w => w.Collection.Language == lang).ToList();
            switch (id)
            {
                case 1:
                  List<Word> words = _context.Words.Include(w => w.Collection).Include(w=> w.UserWords).Where(w=> (w.Collection.OwnerId==user.Id|| w.Collection.OwnerId==null)&& lwords.Contains(w)).ToList();
                    Collection collection = new Collection() { Name="Все слова", Words = words, local=2, Id=1};
                    HttpContext.Session.Set<Collection>("Collection", collection);
                    return View("Index", collection);
                case 2:
                    words = _context.UserWords.Include(w => w.Word).ThenInclude(x=> x.UserWords).Where(w => w.User.Id == user.Id && w.IsFavorite && lwords.Contains(w.Word)).Select(w => w.Word).ToList();
                    collection = new Collection() { Name = "Избранные", Words = words, local = 2, Id=2 };
                    HttpContext.Session.Set<Collection>("Collection", collection);
                    return View("Index", collection);
                case 3:
                    words = _context.UserWords.Include(w => w.Word).ThenInclude(x => x.UserWords).Where(w => w.User.Id == user.Id && w.IsLearned && lwords.Contains(w.Word)).Select(w => w.Word).ToList();
                    collection = new Collection() { Name = "Выученные слова", Words = words, local = 2, Id = 2 };
                    HttpContext.Session.Set<Collection>("Collection", collection);
                    return View("Index", collection);
                case 4:
                    words = _context.Words.Include(x => x.UserWords).Include(x=> x.Collection).Where(w => !(w.UserWords.Where(x=> (x.User.Id == user.Id && x.IsLearned)).Any()) && (w.Collection.OwnerId == user.Id || w.Collection.OwnerId == null) && lwords.Contains(w)).ToList();
                    collection = new Collection() { Name = "Невыученные слова", Words = words, local = 2, Id = 2 };
                    HttpContext.Session.Set<Collection>("Collection", collection);
                    return View("Index", collection);
            }

            return NotFound();
        }
        public async Task<IActionResult> Start(int? id, int type, int local, int themeid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Collection collection;
            if (local == 1)
            {
                var theme = await _context.Themes
             .Include(c => c.Collections).ThenInclude(c => c.Words).ThenInclude(w=> w.UserWords)
             .FirstOrDefaultAsync(m => m.Id == themeid);

                if (theme == null)
                {
                    return NotFound();
                }
                collection = theme.UnionCollections();
                collection.local = 1;
            }else if(local==2){
                collection = HttpContext.Session.Get<Collection>("Collection");
            }
            else
            {
                collection = await _context.Collections
                                .Include(c => c.Words).ThenInclude(w => w.UserWords)
                                .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (collection == null)
            {
                return NotFound();
            }
           
            Test test = new Test() { Collection = collection, TestType = (TestType)type, User = user };
            test.CreateTest();
            if (test.Questions.Count < 10)
            {
                ViewBag.Message = "Необходимо как минимум 10 слов в коллекции на изучении для тренировки";
                return View("Index", collection);
            }
            HttpContext.Session.Set<Test>("Test", test);
            return View("Test");
        }

       


        [HttpPost]
        public async Task<ActionResult> Answer(string answer)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var test = HttpContext.Session.Get<Test>("Test");
            var result = test.CheckAnswer(answer); 
            Question q = test.Questions[test.CurrentQuestion - 1];
            if (user != null)
            {
                UserWords uw = _context.UserWords.FirstOrDefault(w => w.UserId == user.Id && w.WordId == q.Word.Id);
                if (uw ==null)
                {
                     uw = _context.UserWords.AddAsync(new UserWords() { UserId = user.Id, WordId = q.Word.Id }).Result.Entity;
                   await _context.SaveChangesAsync();
                }
                if (q.IsRightAnswer)
                    uw.RightCount++;
                else uw.ErrorCount++;
                await _context.SaveChangesAsync();
            }  
            HttpContext.Session.Set<Test>("Test", test);
            string rightAnswer = q.Word.Name;
            if (test.TestType == TestType.WordTranslation)
                rightAnswer = q.Word.Meaning;
            return Json(new { result = result, rightAnswer = rightAnswer});
        }


        public async Task<ActionResult> GetQuestion()
        {
            var test = HttpContext.Session.Get<Test>("Test");
            Question q = test.GetNextQuestion();
            if (q == null)
                return PartialView("TestResult", test);
            HttpContext.Session.Set<Test>("Test", test);
            ViewBag.Number = test.CurrentQuestion-1;
            switch (test.TestType)
            {
                case TestType.WordTranslation:
                    return PartialView("QuestionWordTranslation", q);
                case TestType.TranslationWord:
                    return PartialView("QuestionTranslationWord", q);
                case TestType.TextQuestion:
                    return PartialView("TextQuestion", q);
                default: return PartialView("QuestionWordTranslation", q);

            }
        }

        public async Task<ActionResult> GetWords(int? id, int local, int themeid)
        {
            if (id == null)
            {
                return NotFound();
            }
            Collection collection;
            if (local == 1)
            {
                var theme = await _context.Themes
               .Include(c => c.Collections).ThenInclude(c => c.Words).ThenInclude(x => x.UserWords)
               .FirstOrDefaultAsync(m => m.Id == themeid);

                if (theme == null)
                {
                    return NotFound();
                }
                collection = theme.UnionCollections();
                collection.local = 1;
            }else  if(local == 2)
            {
              collection = HttpContext.Session.Get<Collection>("Collection");

            }
            else
            {
                collection = await _context.Collections
                                .Include(c => c.Words).ThenInclude(x=> x.UserWords)
                                .FirstOrDefaultAsync(m => m.Id == id);
            }
            
            if (collection == null)
            {
                return NotFound();
            }
           
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(user != null)
            ViewBag.id = user.Id;
            collection.Words = collection.Words.OrderBy(w => w.Name).ToList();
            return View("Words", collection);

        }
        public async Task<ActionResult> GetCards(int? id, int local, int themeid, int page)
        {
           ViewData["PageNumber"] = page;
            
            Collection collection;
            if (local == 1)
            {
                var theme = await _context.Themes
              .Include(c => c.Collections).ThenInclude(c => c.Words)
              .FirstOrDefaultAsync(m => m.Id == themeid);

                if (theme == null)
                {
                    return NotFound();
                }
                collection = theme.UnionCollections();
                collection.local = 1;
            }
            else if(local == 2)
            {
                collection = HttpContext.Session.Get<Collection>("Collection");
            }
            else
            {
                if (id == null)
            {
                return NotFound();
            }
                collection = await _context.Collections
                                .Include(c => c.Words)
                                .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (collection == null)
            {
                return NotFound();
            } 
            if (collection.Words.Count == 0)
            {
                ViewBag.Message = "В наборе нет ни одного слова";
                return View("Index", collection);
            }

                return View("Cards", collection);


        }
        public async Task<ActionResult> GetCard(int? id, int local, int themeid, int page)
        {
            ViewData["PageNumber"] = page;

            Collection collection;
            if (local == 1)
            {
                var theme = await _context.Themes
              .Include(c => c.Collections).ThenInclude(c => c.Words)
              .FirstOrDefaultAsync(m => m.Id == themeid);

                if (theme == null)
                {
                    return NotFound();
                }
                collection = theme.UnionCollections();
                collection.local = 1;
            }
            else if (local == 2)
            {
                collection = HttpContext.Session.Get<Collection>("Collection");
            }
            else
            {
                if (id == null)
                {
                    return NotFound();
                }
                collection = await _context.Collections
                                .Include(c => c.Words)
                                .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (collection == null)
            {
                return NotFound();
            }
            if (collection.Words.Count == 0)
            {
                ViewBag.Message = "В наборе нет ни одного слова";
                return View("Index", collection);
            }

                return PartialView("Card", collection);

        }

    }
}