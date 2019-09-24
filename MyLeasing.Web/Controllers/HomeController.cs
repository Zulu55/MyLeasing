using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;

        public HomeController(
            DataContext dataContext,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult SearchProperties()
        {
            return View(_dataContext.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyImages)
                .Where(p => p.IsAvailable));
        }

        public async Task<IActionResult> DetailsProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(o => o.PropertyType)
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> MyProperties()
        {
            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Contracts)
                .Include(o => o.Properties)
                .ThenInclude(p => p.PropertyType)
                .Include(o => o.Properties)
                .ThenInclude(p => p.PropertyImages)
                .FirstOrDefaultAsync(o => o.User.UserName.ToLower().Equals(User.Identity.Name.ToLower()));
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AddProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new PropertyViewModel
            {
                OwnerId = owner.Id,
                PropertyTypes = _combosHelper.GetComboPropertyTypes()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = await _converterHelper.ToPropertyAsync(model, true);
                _dataContext.Properties.Add(property);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(MyProperties));
            }

            model.PropertyTypes = _combosHelper.GetComboPropertyTypes();
            return View(model);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> EditProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToPropertyViewModel(property);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProperty(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = await _converterHelper.ToPropertyAsync(model, false);
                _dataContext.Properties.Update(property);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(MyProperties));
            }

            model.PropertyTypes = _combosHelper.GetComboPropertyTypes();
            return View(model);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DetailsPropertyOwner(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(o => o.Owner)
                .ThenInclude(o => o.User)
                .Include(o => o.Contracts)
                .ThenInclude(c => c.Lessee)
                .ThenInclude(l => l.User)
                .Include(o => o.PropertyType)
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties.FindAsync(id.Value);
            if (property == null)
            {
                return NotFound();
            }

            var model = new PropertyImageViewModel
            {
                Id = property.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddImage(PropertyImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Properties",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Properties/{file}";
                }

                var propertyImage = new PropertyImage
                {
                    ImageUrl = path,
                    Property = await _dataContext.Properties.FindAsync(model.Id)
                };

                _dataContext.PropertyImages.Add(propertyImage);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsPropertyOwner)}/{model.Id}");
            }

            return View(model);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyImage = await _dataContext.PropertyImages
                .Include(pi => pi.Property)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (propertyImage == null)
            {
                return NotFound();
            }

            _dataContext.PropertyImages.Remove(propertyImage);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsPropertyOwner)}/{propertyImage.Property.Id}");
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(p => p.Owner)
                .Include(p => p.Contracts)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }

            if (property.Contracts?.Count > 0)
            {
                return RedirectToAction(nameof(MyProperties));
            }

            _dataContext.Properties.Remove(property);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(MyProperties));
        }

        [Authorize(Roles = "Owner, Lessee")]
        public IActionResult MyContracts()
        {
            return View(_dataContext.Contracts
                .Include(c => c.Owner)
                .ThenInclude(o => o.User)
                .Include(c => c.Lessee)
                .ThenInclude(l => l.User)
                .Include(c => c.Property)
                .ThenInclude(p => p.PropertyType)
                .Where(c => c.Owner.User.UserName.ToLower().Equals(User.Identity.Name.ToLower()) ||
                            c.Lessee.User.UserName.ToLower().Equals(User.Identity.Name.ToLower())));
        }

        [Authorize(Roles = "Owner, Lessee")]
        public async Task<IActionResult> DetailsContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _dataContext.Contracts
                .Include(c => c.Owner)
                .ThenInclude(o => o.User)
                .Include(c => c.Lessee)
                .ThenInclude(l => l.User)
                .Include(c => c.Property)
                .ThenInclude(p => p.PropertyType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

    }
}
