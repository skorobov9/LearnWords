using LearnForeignWords.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LearnForeignWords.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<User> _userManager;

        public GameController(UserManager<User> userManager)
        {
            _userManager = userManager;

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
            return View(user);
        }
    }
}
