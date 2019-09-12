﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;

namespace MyLeasing.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class OwnersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public OwnersController(
            DataContext dataContext,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper, 
            IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Properties)
                .Include(o => o.Contracts));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Properties)
                .ThenInclude(p => p.PropertyImages)
                .Include(o => o.Contracts)
                .ThenInclude(c => c.Lessee)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public IActionResult Create()
        {
            var view = new AddUserViewModel { RoleId = 2 };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await CreateUserAsync(model);
                if (user != null)
                {
                    var owner = new Owner
                    {
                        Contracts = new List<Contract>(),
                        Properties = new List<Property>(),
                        User = user
                    };

                    _dataContext.Owners.Add(owner);
                    await _dataContext.SaveChangesAsync();

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Username, "Email confirmation",
                        $"<table style = 'max-width: 600px; padding: 10px; margin:0 auto; border-collapse: collapse;'>" +
                        $"  <tr>" +
                        $"    <td style = 'background-color: #34495e; text-align: center; padding: 0'>" +
                        $"       <a href = 'https://www.facebook.com/NuskeCIV/' >" +
                        $"         <img width = '20%' style = 'display:block; margin: 1.5% 3%' src= 'https://veterinarianuske.com/wp-content/uploads/2016/10/line_separator.png'>" +
                        $"       </a>" +
                        $"  </td>" +
                        $"  </tr>" +
                        $"  <tr>" +
                        $"  <td style = 'padding: 0'>" +
                        $"     <img style = 'padding: 0; display: block' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/logo-nnske-blanck.jpg' width = '100%'>" +
                        $"  </td>" +
                        $"</tr>" +
                        $"<tr>" +
                        $" <td style = 'background-color: #ecf0f1'>" +
                        $"      <div style = 'color: #34495e; margin: 4% 10% 2%; text-align: justify;font-family: sans-serif'>" +
                        $"            <h1 style = 'color: #e67e22; margin: 0 0 7px' > Hola </h1>" +
                        $"                    <p style = 'margin: 2px; font-size: 15px'>" +
                        $"                      El mejor Hospital Veterinario Especializado de la Ciudad de Morelia enfocado a brindar servicios médicos y quirúrgicos<br>" +
                        $"                      aplicando las técnicas más actuales y equipo de vanguardia para diagnósticos precisos y tratamientos oportunos..<br>" +
                        $"                      Entre los servicios tenemos:</p>" +
                        $"      <ul style = 'font-size: 15px;  margin: 10px 0'>" +
                        $"        <li> Urgencias.</li>" +
                        $"        <li> Medicina Interna.</li>" +
                        $"        <li> Imagenologia.</li>" +
                        $"        <li> Pruebas de laboratorio y gabinete.</li>" +
                        $"        <li> Estetica canina.</li>" +
                        $"      </ul>" +
                        $"  <div style = 'width: 100%;margin:20px 0; display: inline-block;text-align: center'>" +
                        $"    <img style = 'padding: 0; width: 200px; margin: 5px' src = 'https://veterinarianuske.com/wp-content/uploads/2018/07/tarjetas.png'>" +
                        $"  </div>" +
                        $"  <div style = 'width: 100%; text-align: center'>" +
                        $"    <h2 style = 'color: #e67e22; margin: 0 0 7px' >Email Confirmation </h2>" +
                        $"    To allow the user,plase click in this link:</ br ></ br > " +
                        $"    <a style ='text-decoration: none; border-radius: 5px; padding: 11px 23px; color: white; background-color: #3498db' href = \"{tokenLink}\">Confirm Email</a>" +
                        $"    <p style = 'color: #b3b3b3; font-size: 12px; text-align: center;margin: 30px 0 0' > Nuskë Clinica Integral Veterinaria 2019 </p>" +
                        $"  </div>" +
                        $" </td >" +
                        $"</tr>" +
                        $"</table>");

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "User with this eamil already exists.");
            }

            return View(model);
        }

        private async Task<User> CreateUserAsync(AddUserViewModel model)
        {
            var user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                user = await _userHelper.GetUserByEmailAsync(model.Username);
                await _userHelper.AddUserToRoleAsync(user, "Owner");
                return user;
            }

            return null;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = owner.User.Address,
                Document = owner.User.Document,
                FirstName = owner.User.FirstName,
                Id = owner.Id,
                LastName = owner.User.LastName,
                PhoneNumber = owner.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var owner = await _dataContext.Owners
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                owner.User.Document = model.Document;
                owner.User.FirstName = model.FirstName;
                owner.User.LastName = model.LastName;
                owner.User.Address = model.Address;
                owner.User.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners
                .Include(o => o.User)
                .Include(o => o.Properties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            if (owner.Properties.Count != 0)
            {
                ModelState.AddModelError(string.Empty, "Owner can't be delete because it has properties.");
                return RedirectToAction(nameof(Index));
            }

            _dataContext.Owners.Remove(owner);
            await _dataContext.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(owner.User.Email);
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _dataContext.Owners.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Owners.FindAsync(id);
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
                return RedirectToAction($"Details/{model.OwnerId}");
            }

            model.PropertyTypes = _combosHelper.GetComboPropertyTypes();
            return View(model);
        }

        public async Task<IActionResult> EditProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .FirstOrDefaultAsync(p => p.Id == id);
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
                return RedirectToAction($"Details/{model.OwnerId}");
            }

            return View(model);
        }

        public async Task<IActionResult> DetailsProperty(int? id)
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
                .FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

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

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var propertyImage = new PropertyImage
                {
                    ImageUrl = path,
                    Property = await _dataContext.Properties.FindAsync(model.Id)
                };

                _dataContext.PropertyImages.Add(propertyImage);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.Id}");
            }

            return View(model);
        }

        public async Task<IActionResult> AddContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }

            var model = new ContractViewModel
            {
                OwnerId = property.Owner.Id,
                PropertyId = property.Id,
                Lessees = _combosHelper.GetComboLessees(),
                Price = property.Price,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contract = await _converterHelper.ToContractAsync(model, true);
                _dataContext.Contracts.Add(contract);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.PropertyId}");
            }

            model.Lessees = _combosHelper.GetComboLessees();
            return View(model);
        }

        public async Task<IActionResult> EditContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _dataContext.Contracts
                .Include(p => p.Owner)
                .Include(p => p.Lessee)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToContractViewModel(contract));
        }

        [HttpPost]
        public async Task<IActionResult> EditContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contract = await _converterHelper.ToContractAsync(model, false);
                _dataContext.Contracts.Update(contract);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.PropertyId}");
            }

            return View(model);
        }

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
            return RedirectToAction($"{nameof(DetailsProperty)}/{propertyImage.Property.Id}");
        }

        public async Task<IActionResult> DeleteContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _dataContext.Contracts
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            _dataContext.Contracts.Remove(contract);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsProperty)}/{contract.Property.Id}");
        }

        public async Task<IActionResult> DeleteProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _dataContext.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .Include(p => p.Contracts)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }

            if (property.Contracts.Count != 0)
            {
                ModelState.AddModelError(string.Empty, "The property can't be deleted because it has contracts.");
                return RedirectToAction($"{nameof(Details)}/{property.Owner.Id}");
            }

            _dataContext.PropertyImages.RemoveRange(property.PropertyImages);
            _dataContext.Properties.Remove(property);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{property.Owner.Id}");
        }

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
                .ThenInclude(o => o.User)
                .Include(c => c.Property)
                .ThenInclude(p => p.PropertyType)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
    }
}
