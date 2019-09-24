using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;

namespace MyLeasing.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class PropertyTypesController : Controller
    {
        private readonly DataContext _context;

        public PropertyTypesController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PropertyTypes.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(propertyType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyType = await _context.PropertyTypes.FindAsync(id);
            if (propertyType == null)
            {
                return NotFound();
            }

            return View(propertyType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyTypeExists(propertyType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(propertyType);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyType = await _context.PropertyTypes
                .Include(pt => pt.Properties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (propertyType == null)
            {
                return NotFound();
            }

            if (propertyType.Properties.Count > 0)
            {
                //TODO: message
                return RedirectToAction(nameof(Index));
            }

            _context.PropertyTypes.Remove(propertyType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyTypeExists(int id)
        {
            return _context.PropertyTypes.Any(e => e.Id == id);
        }
    }
}
