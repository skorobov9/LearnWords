using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearnForeignWords.Models;
using LearnForeignWords.ViewModels;
using Microsoft.AspNetCore.Identity;
using LearnForeignWords.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LearnForeignWords.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly WordTestContext _context;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, WordTestContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> AddWordToUser(int? id,string type)
		{
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if(user == null)
                return Json(new { result = false });
            var word = await _context.Words.Include(x => x.Collection).ThenInclude(x => x.Words)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (type == "f")
            {
                var userword = await _context.UserWords.Where(x => x.UserId == user.Id && x.WordId == word.Id).FirstOrDefaultAsync();
                if (userword == null)
                {
                    _context.UserWords.Add(new UserWords() { User = user, Word = word, IsFavorite = true, IsLearned=false });
                }
                else
                {
                    if(userword.IsFavorite)
                    userword.IsFavorite = false;
                    else userword.IsFavorite = true;
                }
                await _context.SaveChangesAsync();
            }
            else
            {

                var userword = await _context.UserWords.Where(x => x.UserId == user.Id && x.WordId == word.Id).FirstOrDefaultAsync();
                if (userword == null)
                {
                    _context.UserWords.Add(new UserWords() { User = user, Word = word, IsFavorite = false, IsLearned=true });
                }
                else
                {
                    if (userword.IsLearned)
                        userword.IsLearned = false;
                    else userword.IsLearned = true;
                }
                await _context.SaveChangesAsync();

            }

            return Json(new { result = true }); 
		}

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email};
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string message,string returnUrl = null)
        {
            ViewBag.Message = message;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
