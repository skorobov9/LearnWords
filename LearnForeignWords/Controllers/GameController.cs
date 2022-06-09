using LearnForeignWords.Data;
using LearnForeignWords.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LearnForeignWords.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly WordTestContext _context;

        public GameController(WordTestContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return RedirectToAction("Login", "User", new { message = "Для доступа к разделу нужно войти/зарегистрироваться!" });
            return View(user);
        }
        public async Task<IActionResult> Start(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return RedirectToAction("Login", "User", new { message = "Для доступа к разделу нужно войти/зарегистрироваться!" });
            ViewBag.Type = id;
            var departmentsQuery = _context.Collections.Where(c => c.OwnerId ==null || c.OwnerId == user.Id).Where(c => c.Words.Count>20).ToList();
            SelectList coll = new SelectList(departmentsQuery, "Id", "Name");
            ViewBag.Select = coll;
            return View(user);
        }
    }
}
