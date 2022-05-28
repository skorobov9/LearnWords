using LearnForeignWords.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LearnForeignWords.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LearnForeignWords.Controllers
{
	public class AdditionallyCollectionsController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly WordTestContext _context;

		public AdditionallyCollectionsController(UserManager<User> userManager, WordTestContext context)
		{
			_userManager = userManager;
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			if (user == null)
			{
				return RedirectToAction("Login", "User",new { message="Need to login!" });
			}
			List<Word> words = _context.Words.Include(w => w.Collection).Include(w => w.UserWords).Where(w => w.Collection.OwnerId == user.Id || w.Collection.OwnerId == null).ToList();
			List<Collection> collections = new List<Collection>();
			collections.Add(new Collection() { Name = "Все слова", Id=1,Words=words});
			words = _context.UserWords.Include(w => w.Word).ThenInclude(x => x.UserWords).Where(w => w.User.Id == user.Id && w.IsFavorite).Select(w => w.Word).ToList();
			collections.Add(new Collection() { Name = "Избранные", Id=2,  Words = words });
			words = _context.UserWords.Include(w => w.Word).ThenInclude(x => x.UserWords).Where(w => w.User.Id == user.Id && w.IsLearned).Select(w => w.Word).ToList();
			collections.Add(new Collection() { Name = "Выученные слова", Id=3, Words = words });
			words = _context.Words.Include(x => x.UserWords).Include(x => x.Collection).Where(w => !(w.UserWords.Where(x => (x.User.Id == user.Id && x.IsLearned)).Any()) && w.Collection.OwnerId == user.Id || w.Collection.OwnerId == null).ToList();
			collections.Add(new Collection() { Name = "Невыученные слова", Id=4, Words = words });
			return View(collections);
		}
		public async Task<IActionResult> IndexLanguages(int id)
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			if (user == null)
			{
				return RedirectToAction("Login", "User", new { message = "Need to login!" });
			}
			return View(new Collection() { Id=id});
		}
	}
}
