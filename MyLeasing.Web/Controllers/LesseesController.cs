using System.Collections.Generic;
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
    public class LesseesController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public LesseesController(
            DataContext dataContext,
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_dataContext.Lessees
                .Include(o => o.User)
                .Include(o => o.Contracts));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessee = await _dataContext.Lessees
                .Include(l => l.User)
                .Include(l => l.Contracts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessee == null)
            {
                return NotFound();
            }

            return View(lessee);
        }

        public IActionResult Create()
        {
            var view = new AddUserViewModel { RoleId = 1 };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.AddUser(view, "Lessee");
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    return View(view);
                }

                var lessee = new Lessee
                {
                    Contracts = new List<Contract>(),
                    User = user,
                };

                _dataContext.Lessees.Add(lessee);
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(view.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                    $"To allow the user, " +
                    $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessee = await _dataContext.Lessees
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (lessee == null)
            {
                return NotFound();
            }

            var view = new EditUserViewModel
            {
                Address = lessee.User.Address,
                Document = lessee.User.Document,
                FirstName = lessee.User.FirstName,
                Id = lessee.Id,
                LastName = lessee.User.LastName,
                PhoneNumber = lessee.User.PhoneNumber
            };

            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel view)
        {
            if (ModelState.IsValid)
            {
                var lessee = await _dataContext.Lessees
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == view.Id);

                lessee.User.Document = view.Document;
                lessee.User.FirstName = view.FirstName;
                lessee.User.LastName = view.LastName;
                lessee.User.Address = view.Address;
                lessee.User.PhoneNumber = view.PhoneNumber;

                await _userHelper.UpdateUserAsync(lessee.User);
                return RedirectToAction(nameof(Index));
            }

            return View(view);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessee = await _dataContext.Lessees
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessee == null)
            {
                return NotFound();
            }

            _dataContext.Lessees.Remove(lessee);
            await _dataContext.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(lessee.User.Email);
            return RedirectToAction(nameof(Index));
        }
    }
}
