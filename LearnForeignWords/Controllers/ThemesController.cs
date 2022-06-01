using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearnForeignWords.Data;
using LearnForeignWords.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace LearnForeignWords.Controllers
{
    public class ThemesController : Controller
    {
        private readonly WordTestContext _context;

        IWebHostEnvironment _appEnvironment;

        public ThemesController(WordTestContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        // GET: Themes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Themes.ToListAsync());
        }

        public async Task<IActionResult> Additionally()
        {
            return View(await _context.Themes.ToListAsync());
        }


        // GET: Themes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theme = await _context.Themes.Include(x => x.Collections).ThenInclude(x => x.Words)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (theme == null)
            {
                return NotFound();
            }

            return View(theme);
        }
    }
}
