using Landscaper.Areas.Admin.Models.Utilities.Enums;
using Landscaper.Areas.Admin.Models.Utilities.Extentions;
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
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Services.CountAsync();
            List<Service> services = await _context.Services.Skip(page * 3).Take(3).ToListAsync();
            PaginationVM<Service> vm = new()
            {
                CurrentPage= page+1,
                TotalPage= Math.Ceiling(count/3),
                Items = services
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ServiceCreateVM vm)
        {
            if(!ModelState.IsValid) return View();
            bool check = await _context.Services.AnyAsync(s=>s.Name.ToLower().Trim()==vm.Name.ToLower().Trim());
            if(check)
            {
                ModelState.AddModelError("Name", "This service already exists");
                return View();
            }
            if (!vm.Photo.IsValidType(FileType.Image))
            {
                ModelState.AddModelError("Photo", "Photo should be image type");
                return View();
            }
            if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
            {
                ModelState.AddModelError("Photo", "Photo can be less or equal 5mb");
                return View();
            }
            Service service = new()
            {
                Name = vm.Name,
                Description = vm.Description,
                ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets","img","services"),
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Service existed = await _context.Services.FirstOrDefaultAsync(s=>s.Id == id);
            if (existed is null) return NotFound();
            ServiceUpdateVM vm = new()
            {
                Name = existed.Name,
                Description = existed.Description,
                ImageURL = existed.ImageURL,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,ServiceUpdateVM vm)
        {
            if (!ModelState.IsValid) return View();
            Service existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            if(vm.Photo is not null)
            {
                if (!vm.Photo.IsValidType(FileType.Image))
                {
                    ModelState.AddModelError("Photo", "Photo should be image type");
                    return View();
                }
                if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
                {
                    ModelState.AddModelError("Photo", "Photo can be less or equal 5mb");
                    return View();
                }
                existed.ImageURL.Delete(_env.WebRootPath, "assets", "img", "services");
                existed.ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets", "img", "services");
            }
            existed.Name = vm.Name;
            existed.Description = vm.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Service existed = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            existed.ImageURL.Delete(_env.WebRootPath, "assets", "img", "services");
            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
    }
}
