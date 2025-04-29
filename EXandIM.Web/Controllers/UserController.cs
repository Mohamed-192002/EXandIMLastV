using AutoMapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using EXandIM.Web.Helpers;
using EXandIM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;

namespace EXandIM.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ApplicationDbContext context, IImageService imageService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
            _imageService = imageService;
        }
        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            List<ApplicationUser> users;
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                users = await _userManager.Users.Include(u => u.Team).ThenInclude(t => t.Circle).ToListAsync();
            }
            else if (User.IsInRole(AppRoles.Admin))
            {
                users = await _userManager.Users.Include(u => u.Team).ThenInclude(t => t.Circle)
              .Where(b => b.Team!.Id == user.TeamId).ToListAsync();
            }
            else
            {
                users = await _userManager.Users.Include(u => u.Team).ThenInclude(t => t.Circle)
             .Where(b => b.Id == userId).ToListAsync();
            }

            var viewModel = _mapper.Map<IList<UserViewModel>>(users);

            return View(viewModel);
        }
        public async Task<IActionResult> GetAllAdmins()
        {
            var allAdmins = await _userManager.Users.ToListAsync();
            var AdminsInRole = new List<ApplicationUser>();

            foreach (var admin in allAdmins)
            {
                var isInRole = await _userManager.IsInRoleAsync(admin, AppRoles.Admin);
                if (isInRole)
                {
                    AdminsInRole.Add(admin);
                }
            }

            var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(AdminsInRole);
            return View("Index", viewModel);
        }
        public async Task<IActionResult> GetAllSuberAdmins()
        {
            var allSuberAdmins = await _userManager.Users.ToListAsync();
            var SuberAdminsInRole = new List<ApplicationUser>();

            foreach (var suberAdmin in allSuberAdmins)
            {
                var isInRole = await _userManager.IsInRoleAsync(suberAdmin, AppRoles.SuperAdmin);
                if (isInRole)
                {
                    SuberAdminsInRole.Add(suberAdmin);
                }
            }
            var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(SuberAdminsInRole);
            return View("Index", viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = PopulateViewModel();
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = new()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                TeamId = model.TeamId,
                CircleId = model.CircleId,
                password = model.Password!
            };
            if (model.Image is not null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var imagePath = "/images/Users";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);
                if (!isUploaded)
                {
                    ModelState.AddModelError("Image", errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }
                user.ImageUrl = $"{imagePath}/{imageName}";
                user.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            }
            else
            {
                user.ImageUrl = "/assets/images/avatar.png";
                user.ImageThumbnailUrl = "/assets/images/avatar.png";
            }

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                var viewModel = _mapper.Map<UserViewModel>(user);
                return RedirectToAction("Index");
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var model = _mapper.Map<UserFormViewModel>(user);
            var viewModel = PopulateViewModel(model);
            viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(user.ImageUrl))
                    _imageService.Delete(user.ImageUrl, user.ImageThumbnailUrl);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var imagePath = "/images/Users";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError("Image", errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }

                model.ImageUrl = $"{imagePath}/{imageName}";
                model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            }
            else if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                model.ImageUrl = user.ImageUrl;
                model.ImageThumbnailUrl = user.ImageThumbnailUrl;
            }
            user = _mapper.Map(model, user);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                var currentRoles = await _userManager.GetRolesAsync(user);
                var filteredRoles = currentRoles.Where(role => !role.StartsWith("Can")).ToList();

                var rolesUpdated = !filteredRoles.SequenceEqual(model.SelectedRoles);

                if (rolesUpdated)
                {
                    await _userManager.RemoveFromRolesAsync(user, filteredRoles);
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                var viewModel = _mapper.Map<UserViewModel>(user);
                return RedirectToAction("Index");
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }
        [HttpGet]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        public async Task<IActionResult> EditPermissions(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();
            var Permissions = new PermissionsViewModel();
            Permissions.FullName = user.FullName;
            Permissions.UserName = user.UserName;
            Permissions.SelectedRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles
                .Where(r => r.Name.StartsWith("can"))
                .OrderBy(a => a.Name)
                .ToList();
            Permissions.Roles = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();

            return View(Permissions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPermissions(PermissionsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var filteredRoles = currentRoles.Where(role => role.StartsWith("Can")).ToList();

            var rolesUpdated = !filteredRoles.SequenceEqual(model.SelectedRoles);

            if (rolesUpdated)
            {
                await _userManager.RemoveFromRolesAsync(user, filteredRoles);
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var isLocked = await _userManager.IsLockedOutAsync(user);

            if (isLocked)
                await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok();
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = new ResetPasswordFormViewModel { Id = user.Id };

            return PartialView("_ResetPasswordForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user is null)
                return NotFound();

            var currentPasswordHash = user.PasswordHash;

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, model.Password);

            if (result.Succeeded)
            {

                await _userManager.UpdateAsync(user);

                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }

            user.PasswordHash = currentPasswordHash;
            await _userManager.UpdateAsync(user);

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            user.IsDeleted = !user.IsDeleted;

            await _userManager.UpdateAsync(user);
            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();
            try
            {
                await _userManager.DeleteAsync(user);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        [AjaxOnly]
        public IActionResult GetTeams(int circleId)
        {
            var userId = GetAuthenticatedUser();
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            List<Team> teams;
            if (User.IsInRole(AppRoles.SuperAdmin))
                teams = _context.Teams
                   .Where(a => a.CircleId == circleId)
                   .OrderBy(g => g.Name)
                   .ToList();
            else
                teams = _context.Teams
                   .Where(a => a.Id == user.TeamId)
                   .OrderBy(g => g.Name)
                   .ToList();

            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(teams));
        }
        private UserFormViewModel PopulateViewModel(UserFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            UserFormViewModel viewModel = model is null ? new UserFormViewModel() : model;
            List<IdentityRole> roles;
            if (User.IsInRole(AppRoles.SuperAdmin))
                roles = [.. _roleManager.Roles
                   .Where(r => !r.Name.StartsWith("can"))
                   .OrderBy(a => a.Name)];
            else
                roles = [.. _roleManager.Roles
                  .Where(r => !r.Name.StartsWith("can")&&r.Name!=AppRoles.SuperAdmin)
                  .OrderBy(a => a.Name)];

            viewModel.Roles = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();
            List<Circle> circles;
            if (User.IsInRole(AppRoles.SuperAdmin))
                circles = _context.Circles.OrderBy(a => a.Name).ToList();
            else
                circles = _context.Circles.Where(c => c.Id == user.CircleId).OrderBy(a => a.Name).ToList();


            var teams = _context.Teams.OrderBy(a => a.Name).ToList();
            viewModel.Teams = _mapper.Map<IEnumerable<SelectListItem>>(teams);
            viewModel.Circles = _mapper.Map<IEnumerable<SelectListItem>>(circles);

            return viewModel;
        }

        [AjaxOnly]
        public IActionResult GetSubEntities(int entityId)
        {
            var subEntities = _context.SubEntities
                    .Where(a => a.EntityId == entityId)
                    .OrderBy(g => g.Name)
                    .ToList();
            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(subEntities));
        }

    }
}
