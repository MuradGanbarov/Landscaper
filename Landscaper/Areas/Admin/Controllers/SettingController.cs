using Landscaper.Areas.Admin.ViewModels;
using Landscaper.DAL;
using Landscaper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Landscaper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [AutoValidateAntiforgeryToken]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Settings.CountAsync();
            List<Setting> settings = await _context.Settings.Skip(page*3).Take(3).ToListAsync();
            PaginationVM<Setting> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = settings
            };
            return View(vm);
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Setting existed = await _context.Settings.FirstOrDefaultAsync(s=>s.Id== id);
            if (existed is null) return NotFound();
            SettingUpdateVM vm = new()
            {
                Key = existed.Key,
                Value = existed.Value,
            };
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,SettingUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Setting existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            existed.Key= vm.Key;
            existed.Value = vm.Value;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
