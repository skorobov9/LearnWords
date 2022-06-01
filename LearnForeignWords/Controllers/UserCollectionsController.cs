using Microsoft.AspNetCore.Mvc;
using LearnForeignWords.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LearnForeignWords.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace LearnForeignWords.Controllers
{
    public class UserCollectionsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly WordTestContext _context;
        IWebHostEnvironment _appEnvironment;

        public UserCollectionsController(UserManager<User> userManager, WordTestContext context, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { message = "Need to login!" });
            }
            var collections = await _context.Collections.Include(c=> c.Words).Where(c => c.OwnerId == user.Id).ToListAsync();
            return View(collections);
        }


        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { message = "Need to login!" });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string lang)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { message = "Need to login!" });
            }
            var col = _context.Collections.AddAsync(new Collection() { OwnerId = user.Id, Name = name, Language = lang }).Result;
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit",new { id=col.Entity.Id});
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string word, string mean, IFormFile uploadedFile)
        {
            var collection = await _context.Collections.Include(c => c.Words).Where(w => w.Id == id).FirstOrDefaultAsync();
            if (collection == null)
            {
                return NotFound();
            }
            if(collection.Words.Where(w => w.Name == word).Any())
			{
                ViewBag.Message = "Такое слово уже есть в коллекции";
                return View("Edit", collection);
            }
            string path=null;
            if (uploadedFile != null)
            {
                if (!Directory.Exists(_appEnvironment.WebRootPath + "/images/" + $"/{collection.Id}/"))
                {
                    Directory.CreateDirectory(_appEnvironment.WebRootPath + "/images/" + $"{collection.Id}/");
                }
                FileInfo fileInfo = new FileInfo(uploadedFile.FileName);
                // путь к папке Files
                path = "/images/" + $"{collection.Id}/" + word + fileInfo.Extension;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

            }  
            _context.Words.Add(new Word { Collection=collection, Meaning = mean, Name = word, Image=path});
            await _context.SaveChangesAsync();

            return View("Edit", collection);

        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { message = "Need to login!" });
            }
            var collection = await _context.Collections.Include(c => c.Words).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (collection == null)
            {
                return NotFound();
            }
            return View("Edit", collection);

        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { message = "Need to login!" });
            }
            var collection = await _context.Collections.Include(c => c.Words).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (collection == null)
            {
                return NotFound();
            }
            _context.Words.RemoveRange(collection.Words);
            _context.Collections.Remove(collection);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
