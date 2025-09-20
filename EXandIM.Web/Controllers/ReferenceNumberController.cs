using AutoMapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EXandIM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.Admin)]
    public class ReferenceNumberController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpGet]
        public IActionResult Index()
        {
            var ReferenceNumber = _context.ReferenceNumbers.ToList();
            var viewModel = _mapper.Map<IEnumerable<ReferenceNumberViewModel>>(ReferenceNumber);
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ReferenceNumberFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var ReferenceNumber = _mapper.Map<ReferenceNumber>(viewModel);
            _context.ReferenceNumbers.Add(ReferenceNumber);
            _context.SaveChanges();
            return PartialView("_ReferenceNumberRow", _mapper.Map<ReferenceNumberViewModel>(ReferenceNumber));

        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var ReferenceNumber = _context.ReferenceNumbers.Find(id);
            if (ReferenceNumber == null)
                return NotFound();
            var viewModel = _mapper.Map<ReferenceNumberFormViewModel>(ReferenceNumber);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(ReferenceNumberFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var ExitReferenceNumber = _context.ReferenceNumbers.AsNoTracking().FirstOrDefault(x => x.Id == viewModel.Id);
            var ReferenceNumber = _mapper.Map<ReferenceNumber>(viewModel);
            if (ReferenceNumber == null)
                return NotFound();
            ReferenceNumber.Name = viewModel.Name;
            _context.ReferenceNumbers.Update(ReferenceNumber);
            _context.SaveChanges();

            return PartialView("_ReferenceNumberRow", _mapper.Map<ReferenceNumberViewModel>(ReferenceNumber));
        }
        public IActionResult Delete(int id)
        {
            var ReferenceNumber = _context.ReferenceNumbers.Find(id);
            if (ReferenceNumber == null)
                return NotFound();
            try
            {
                _context.ReferenceNumbers.Remove(ReferenceNumber);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(ReferenceNumberFormViewModel model)
        {
            var ReferenceNumber = _context.ReferenceNumbers.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = ReferenceNumber is null || ReferenceNumber.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        public IActionResult Details(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");

            var referenceNumber = _context.ReferenceNumbers
                .Include(a => a.Books)
                .Include(a => a.Readings)
                .FirstOrDefault(a => a.Id == id);

            if (referenceNumber == null)
                return BadRequest();

            var items = referenceNumber.Books
                .Select(b => new ReferenceNumberTimelineItem
                {
                    Id = b.Id,
                    Date = b.BookDate,
                    Book = b
                })
                .Concat(referenceNumber.Readings.Select(r => new ReferenceNumberTimelineItem
                {
                    Id = r.Id,
                    Date = r.BookDate,
                    Reading = r
                }))
                .OrderBy(i => i.Date)
                .ToList();

            var viewModel = new ReferenceNumberDetailsViewModel
            {
                Id = referenceNumber.Id,
                Name = referenceNumber.Name,
                Items = items
            };

            return View(viewModel);
        }

    }


}

