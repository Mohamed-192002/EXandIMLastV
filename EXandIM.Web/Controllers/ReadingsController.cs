using AutoMapper;
using Dapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;

namespace EXandIM.Web.Controllers
{
    [Authorize]
    public class ReadingsController(IConfiguration configuration, ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        [Authorize(Roles = "CanViewReading,SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "CanCreateReading,SuperAdmin")]
        public IActionResult Create()
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var viewModel = new ReadingFormViewModel();
            var entities = _context.Entities.OrderBy(a => a.Name).ToList();
            var Subentities = _context.SubEntities.OrderBy(a => a.Name).ToList();
            var SecondSubEntities = _context.SecondSubEntities.OrderBy(a => a.Name).ToList();
            var ReferenceNumbers = _context.ReferenceNumbers.OrderBy(a => a.Name).ToList();

            viewModel.ReferenceNumbers = _mapper.Map<IEnumerable<SelectListItem>>(ReferenceNumbers);
            viewModel.Entities = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            viewModel.SubEntities = _mapper.Map<IEnumerable<SelectListItem>>(Subentities);
            viewModel.SecondSubEntities = _mapper.Map<IEnumerable<SelectListItem>>(SecondSubEntities);

            return View("Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ReadingFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form");

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");

            List<Entity> entities = new List<Entity>();
            foreach (var entityId in model.SelectedEntities)
            {
                var entity = await _context.Entities.FindAsync(entityId);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            List<SubEntity> subEntities = new List<SubEntity>();
            foreach (var subentityId in model.SelectedSubEntity)
            {
                var subentity = await _context.SubEntities.FindAsync(subentityId);
                if (subentity != null)
                {
                    subEntities.Add(subentity);
                }
            }
            List<SecondSubEntity> secendSubEntities = new List<SecondSubEntity>();
            foreach (var SecondentityId in model.SelectedSecondSubEntity)
            {
                var Secondentity = await _context.SecondSubEntities.FindAsync(SecondentityId);
                if (Secondentity != null)
                {
                    secendSubEntities.Add(Secondentity);
                }
            }

            var reading = _mapper.Map<Reading>(model);

            reading.UserId = userId;
            reading.Entities = entities;
            reading.SubEntities = subEntities;
            reading.SecondSubEntities = secendSubEntities;

            // Handle reading images
            if (model.ReadingFiles != null && model.ReadingFiles.Count > 0)
            {
                foreach (var image in model.ReadingFiles)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = "/images/files/reading";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await image.CopyToAsync(stream);

                        var readingImage = new ReadingFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = image.FileName,
                            Reading = reading
                        };

                        _context.ReadingFiles.Add(readingImage);
                    }
                }
            }

            _context.Readings.Add(reading);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "CanEditReading,SuperAdmin")]
        public IActionResult Edit(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var readings = _context.Readings
                 .Include(b => b.Entities)
                .Include(b => b.SubEntities)
                .Include(b => b.SecondSubEntities)
                .Include(r => r.ReadingImages).FirstOrDefault(b => b.Id == id);

            if (readings is null)
                return NotFound();
            var model = _mapper.Map<ReadingFormViewModel>(readings);
            model.ExistingFiles = _mapper.Map<List<BookFileDisplay>>(readings.ReadingImages);
            var entities = _context.Entities.OrderBy(a => a.Name).ToList();
            model.Entities = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            var ReferenceNumbers = _context.ReferenceNumbers.OrderBy(a => a.Name).ToList();

            model.ReferenceNumbers = _mapper.Map<IEnumerable<SelectListItem>>(ReferenceNumbers);
            model.SelectedEntities = readings.Entities.Select(t => t.Id).ToList();
            model.SelectedSubEntity = readings.SubEntities.Select(t => t.Id).ToList();
            model.SelectedSecondSubEntity = readings.SecondSubEntities.Select(t => t.Id).ToList();

            return View("Form", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReadingFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form");

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");
            var readings = _context.Readings
                .Include(b => b.Entities).Include(b => b.SubEntities).Include(b => b.SecondSubEntities)
                .Include(r => r.ReadingImages).Include(r => r.User).FirstOrDefault(b => b.Id == model.Id);

            if (readings is null)
                return NotFound();
            if (readings.User is null)
                return NotFound();

            var entities = readings.Entities;
            var Subentities = readings.SubEntities;
            var SecondSubentities = readings.SecondSubEntities;

            var currentEntities = readings.Entities.Select(t => t.Id).ToList();
            var currentSubEntities = readings.SubEntities.Select(t => t.Id).ToList();
            var currentSecondSubEntities = readings.SecondSubEntities.Select(t => t.Id).ToList();

            readings = _mapper.Map(model, readings);

            var orderedCurrentEntities = currentEntities.Order().ToList();
            var orderedSelectedEntities = model.SelectedEntities.Order().ToList();
            var entitiesUpdated = !orderedCurrentEntities.SequenceEqual(orderedSelectedEntities);
            if (entitiesUpdated)
            {
                readings.Entities.Clear();
                foreach (var entityId in model.SelectedEntities)
                {
                    var entity = await _context.Entities.FindAsync(entityId);
                    if (entity != null)
                    {
                        readings.Entities.Add(entity);
                    }
                }
            }
            else
                readings.Entities = entities;

            var orderedCurrentSubEntities = currentSubEntities.Order().ToList();
            var orderedSelectedSubEntities = model.SelectedSubEntity.Order().ToList();
            var SubentitiesUpdated = !orderedCurrentSubEntities.SequenceEqual(orderedSelectedSubEntities);
            if (SubentitiesUpdated)
            {
                readings.SubEntities.Clear();
                foreach (var teamId in model.SelectedSubEntity)
                {
                    var Subentity = await _context.SubEntities.FindAsync(teamId);
                    if (Subentity != null)
                    {
                        readings.SubEntities.Add(Subentity);
                    }
                }
            }
            else
                readings.SubEntities = Subentities;

            var orderedCurrentSecondSubEntities = currentSecondSubEntities.Order().ToList();
            var orderedSelectedSecondSubEntities = model.SelectedSecondSubEntity.Order().ToList();
            var SecondSubentitiesUpdated = !orderedCurrentSecondSubEntities.SequenceEqual(orderedSelectedSecondSubEntities);
            if (SecondSubentitiesUpdated)
            {
                readings.SecondSubEntities.Clear();
                foreach (var teamId in model.SelectedSecondSubEntity)
                {
                    var entity = await _context.SecondSubEntities.FindAsync(teamId);
                    if (entity != null)
                    {
                        readings.SecondSubEntities.Add(entity);
                    }
                }
            }
            else
                readings.SecondSubEntities = SecondSubentities;

            #region Files List
            // Handle file uploads
            if (model.ReadingFiles.Any())
            {
                foreach (var file in model.ReadingFiles)
                {
                    if (file.Length > 0)
                    {

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = "/images/files";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await file.CopyToAsync(stream);
                        stream.Dispose();

                        //    reading.FileUrl = $"{filePath}/{fileName}";
                        var readingImage = new ReadingFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = file.FileName,
                            Reading = readings
                        };

                        readings.ReadingImages.Add(readingImage);
                    }
                }
            }

            // Handle image deletions
            var deletedImageUrls = model.DeletedFileUrls?.Split(',') ?? Array.Empty<string>();
            foreach (var imageUrl in deletedImageUrls)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var file = _context.ReadingFiles.FirstOrDefault(r => r.FileUrl == imageUrl);
                    if (file != null)
                    {
                        // readings.ReadingImages.Remove(file);
                        _context.ReadingFiles.Remove(file);
                        _context.SaveChanges();  // Save changes to the database
                    }

                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            #endregion



            _context.SaveChanges();
            _context.Readings.Update(readings);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "CanDeleteReading,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var readings = _context.Readings.Include(r => r.ReadingImages).FirstOrDefault(r => r.Id == id);
            if (readings is null)
                return NotFound();

            if (readings.ReadingImages.Any())
            {
                var readingImagesCopy = readings.ReadingImages.ToList();

                foreach (var item in readingImagesCopy)
                {
                    var oldFilePath = $"{_webHostEnvironment.WebRootPath}{item.FileUrl}";
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    _context.ReadingFiles.Remove(item);
                    _context.SaveChanges();
                }

            }
            _context.Readings.Remove(readings);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = readings.Id });
        }
        public IActionResult Details(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var readings = _context.Readings
                .Include(r => r.ReadingImages)
                .Include(b => b.Entities)
                .Include(b => b.SubEntities)
                .Include(b => b.SecondSubEntities)
                 .Include(b => b.User)
                .SingleOrDefault(b => b.Id == id);

            if (readings is null)
                return NotFound();

            var viewModel = _mapper.Map<ReadingViewModel>(readings);

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> GetBooksAsync()
        {
            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);

            var searchValue = Request.Form["search[value]"];

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");

            var books = new ReadingResult();
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = await LoadReadings(skip, pageSize, searchValue, sortColumn, sortColumnDirection,null,null,true);
            }
            else
            {
                books = await LoadReadings(skip, pageSize, searchValue, sortColumn, sortColumnDirection, null, null, false);
            }
            var jsonData = new { recordsFiltered = books.RecordsTotal, books.RecordsTotal, data = books.Books };
            return Ok(jsonData);
        }

        public async Task<IActionResult> GetBooksAfterFilterDateAsync(DateTime? fromDate, DateTime? toDate)
        {
            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);

            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");

            var books = new ReadingResult();
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = await LoadReadings(skip, pageSize, searchValue, sortColumn, sortColumnDirection, fromDate, toDate, true);
            }
            else
            {
                books = await LoadReadings(skip, pageSize, searchValue, sortColumn, sortColumnDirection, fromDate, toDate, false);
            }
            var jsonData = new { recordsFiltered = books.RecordsTotal, books.RecordsTotal, data = books.Books };
            return Ok(jsonData);
        }
        private async Task<ReadingResult> LoadReadings(int skip, int pageSize, string search,
            string sortColumn, string sortDirection, DateTime? fromDate = null,
            DateTime? toDate = null, bool isSuperAdmin = false)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Skip = skip,
                    PageSize = pageSize,
                    Search = search ?? "",
                    SortColumn = sortColumn ?? "BookDate",
                    SortDirection = sortDirection ?? "DESC",
                    FromDate = fromDate,
                    ToDate = toDate,
                    IsSuperAdmin = isSuperAdmin
                };

                using var multi = await connection.QueryMultipleAsync(
                    "GetReadings",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // 1. Readings data
                var readings = (await multi.ReadAsync<ReadingViewModel>()).ToList();

                // 2. Entities
                var entities = (await multi.ReadAsync<(int ReadingId, string EntityName)>()).ToList();

                // 3. SubEntities
                var subEntities = (await multi.ReadAsync<(int ReadingId, string SubEntityName)>()).ToList();

                // 4. SecondSubEntities
                var secondSubEntities = (await multi.ReadAsync<(int ReadingId, string SecondSubEntityName)>()).ToList();

                // 6. Total Count
                var recordsTotal = (await multi.ReadAsync<int>()).FirstOrDefault();

                // Map related data to readings
                foreach (var reading in readings)
                {
                    reading.Entities = entities
                        .Where(e => e.ReadingId == reading.Id)
                        .Select(e => e.EntityName)
                        .ToList();

                    reading.SubEntities = subEntities
                        .Where(e => e.ReadingId == reading.Id)
                        .Select(e => e.SubEntityName)
                        .ToList();

                    reading.SecondSubEntities = secondSubEntities
                        .Where(e => e.ReadingId == reading.Id)
                        .Select(e => e.SecondSubEntityName)
                        .ToList();
                }

                return new ReadingResult
                {
                    Books = readings,
                    RecordsTotal = recordsTotal
                };
            }
        }
        public class ReadingResult
        {
            public List<ReadingViewModel> Books { get; set; } = new();
            public int RecordsTotal { get; set; }
        }
        public IActionResult AllowItem(ReadingFormViewModel model)
        {
            var reading = _context.Readings.SingleOrDefault(b => b.Title == model.Title && b.BookDate == model.BookDate);
            var isAllowed = reading is null || reading.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHidden(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null || !User.IsInRole(AppRoles.SuperAdmin))
                return BadRequest("يجب تسجيل الدخول اولا");

            var book = _context.Readings.Find(id);
            if (book is null) return NotFound();

            book.IsHidden = !book.IsHidden;

            _context.Update(book);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}