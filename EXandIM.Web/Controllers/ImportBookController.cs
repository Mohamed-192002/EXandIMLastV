using AutoMapper;
using Dapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using static EXandIM.Web.Controllers.ExportBookController;

namespace EXandIM.Web.Controllers
{
    [Authorize]
    public class ImportBookController(IConfiguration configuration, ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        [Authorize(Roles = "CanViewImport,SuperAdmin")]
        public IActionResult Index() => View();
        public IActionResult AllImportUnAccepted()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetImportUnAcceptedCount()
        {
            var userId = GetAuthenticatedUser();
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            int books;
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = _context.Books.Where(b => !b.IsExport && !b.IsAccepted).Count();
            }
            else
            {
                books = _context.Books.Where(b => !b.IsExport && b.Teams.Any(t => t.CircleId == user.CircleId) && !b.IsAccepted).Count();
            }

            return Json(new { count = books });
        }
        [Authorize(Roles = "CanCreateImport,SuperAdmin")]
        public IActionResult Create()
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            return View("Form", PopulateViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ImportBookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
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

            var book = _mapper.Map<Book>(model);

            #region BookFile
            //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.BookFile.FileName)}";
            //var filePath = "/images/files";
            //if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
            //    Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

            //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

            //using var stream = System.IO.File.Create(path);
            //await model.BookFile.CopyToAsync(stream);
            //stream.Dispose();

            //book.FileUrl = $"{filePath}/{fileName}";
            #endregion

            book.IsExport = false;
            book.UserId = userId;
            book.IsAccepted = true;
            book.Entities = entities;
            book.SubEntities = subEntities;
            book.SecondSubEntities = secendSubEntities;
            foreach (var teamId in model.SelectedTeams)
            {
                var team = await _context.Teams.FindAsync(teamId);
                if (team != null)
                {
                    book.Teams.Add(team);
                }
            }
            //if (user.Team is not null)
            //{
            //    var userTeamId = user.TeamId;
            //    if (!book.Teams.Any(team => team.Id == userTeamId))
            //    {
            //        var team = await _context.Teams.FindAsync(userTeamId);
            //        book.Teams.Add(team);
            //    }
            //}
            // Handle Book images
            if (model.BookFiles != null && model.BookFiles.Count > 0)
            {
                foreach (var image in model.BookFiles)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = "/images/files/Book";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await image.CopyToAsync(stream);

                        var bookImage = new BookFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = image.FileName,
                            Book = book
                        };

                        _context.BookFiles.Add(bookImage);
                    }
                }
            }

            _context.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "CanEditImport,SuperAdmin")]
        public IActionResult Edit(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var book = _context.Books.Include(r => r.BookImages).Include(b => b.Teams).Include(b => b.Entities)
                .Include(b => b.SubEntities)
                .Include(b => b.SecondSubEntities).FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var model = _mapper.Map<ImportBookFormViewModel>(book);
            model.ExistingFiles = _mapper.Map<List<BookFileDisplay>>(book.BookImages);
            var viewModel = PopulateViewModel(model);
            viewModel.SelectedTeams = book.Teams.Select(t => t.Id).ToList();
            viewModel.SelectedEntities = book.Entities.Select(t => t.Id).ToList();
            viewModel.SelectedSubEntity = book.SubEntities.Select(t => t.Id).ToList();
            viewModel.SelectedSecondSubEntity = book.SecondSubEntities.Select(t => t.Id).ToList();
            return View("Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ImportBookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");

            var book = _context.Books.Include(b => b.Teams).Include(r => r.BookImages)
                           .Include(b => b.Entities).Include(b => b.SubEntities).Include(b => b.SecondSubEntities)
                           .Include(b => b.User).ThenInclude(u => u.Team).FirstOrDefault(b => b.Id == model.Id);

            if (book is null)
                return NotFound();
            if (book.User is null)
                return NotFound();

            var entities = book.Entities;
            var Subentities = book.SubEntities;
            var SecondSubentities = book.SecondSubEntities;
            var teams = book.Teams;
            int userTeam = 0;
            if (book.User.Team is not null)
                userTeam = book.User.Team.Id;

            #region BookFile
            //if (model.BookFile is not null)
            //{
            //    if (!string.IsNullOrEmpty(book.FileUrl))
            //    {
            //        var oldFilePath = $"{_webHostEnvironment.WebRootPath}{book.FileUrl}";

            //        if (System.IO.File.Exists(oldFilePath))
            //            System.IO.File.Delete(oldFilePath);
            //    }
            //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.BookFile.FileName)}";
            //    var filePath = "/images/files";

            //    if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
            //        Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

            //    var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

            //    using var stream = System.IO.File.Create(path);
            //    await model.BookFile.CopyToAsync(stream);
            //    stream.Dispose();

            //    model.FileUrl = $"{filePath}/{fileName}";
            //}

            //else if (!string.IsNullOrEmpty(book.FileUrl))
            //{
            //    model.FileUrl = book.FileUrl;
            //}
            #endregion

            var currentEntities = book.Entities.Select(t => t.Id).ToList();
            var currentSubEntities = book.SubEntities.Select(t => t.Id).ToList();
            var currentSecondSubEntities = book.SecondSubEntities.Select(t => t.Id).ToList();
            var currentTeams = book.Teams.Select(t => t.Id).ToList();

            book = _mapper.Map(model, book);

            var orderedCurrentEntities = currentEntities.Order().ToList();
            var orderedSelectedEntities = model.SelectedEntities.Order().ToList();
            var entitiesUpdated = !orderedCurrentEntities.SequenceEqual(orderedSelectedEntities);
            if (entitiesUpdated)
            {
                book.Entities.Clear();
                foreach (var teamId in model.SelectedEntities)
                {
                    var entity = await _context.Entities.FindAsync(teamId);
                    if (entity != null)
                    {
                        book.Entities.Add(entity);
                    }
                }
            }
            else
                book.Entities = entities;

            var orderedCurrentSubEntities = currentSubEntities.Order().ToList();
            var orderedSelectedSubEntities = model.SelectedSubEntity.Order().ToList();
            var SubentitiesUpdated = !orderedCurrentSubEntities.SequenceEqual(orderedSelectedSubEntities);
            if (SubentitiesUpdated)
            {
                book.SubEntities.Clear();
                foreach (var teamId in model.SelectedSubEntity)
                {
                    var Subentity = await _context.SubEntities.FindAsync(teamId);
                    if (Subentity != null)
                    {
                        book.SubEntities.Add(Subentity);
                    }
                }
            }
            else
                book.SubEntities = Subentities;

            var orderedCurrentSecondSubEntities = currentSecondSubEntities.Order().ToList();
            var orderedSelectedSecondSubEntities = model.SelectedSecondSubEntity.Order().ToList();
            var SecondSubentitiesUpdated = !orderedCurrentSecondSubEntities.SequenceEqual(orderedSelectedSecondSubEntities);
            if (SecondSubentitiesUpdated)
            {
                book.Entities.Clear();
                foreach (var teamId in model.SelectedSecondSubEntity)
                {
                    var entity = await _context.SecondSubEntities.FindAsync(teamId);
                    if (entity != null)
                    {
                        book.SecondSubEntities.Add(entity);
                    }
                }
            }
            else
                book.SecondSubEntities = SecondSubentities;


            var orderedCurrentTeams = currentTeams.Order().ToList();
            var orderedSelectedTeams = model.SelectedTeams.Order().ToList();

            var teamsUpdated = !orderedCurrentTeams.SequenceEqual(orderedSelectedTeams);
            if (teamsUpdated)
            {
                book.Teams.Clear();
                foreach (var teamId in model.SelectedTeams)
                {
                    var team = await _context.Teams.FindAsync(teamId);
                    if (team != null)
                    {
                        book.Teams.Add(team);
                    }
                }
                //if (userTeam > 0)
                //{
                //    if (!model.SelectedTeams.Contains(userTeam))
                //    {
                //        var UserTeam = await _context.Teams.FindAsync(userTeam);
                //        book.Teams.Add(UserTeam!);
                //    }
                //}

            }
            else
                book.Teams = teams;

            #region Files List
            // Handle file uploads
            if (model.BookFiles.Any())
            {
                foreach (var file in model.BookFiles)
                {
                    if (file.Length > 0)
                    {

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = "/images/files/Book";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await file.CopyToAsync(stream);
                        stream.Dispose();

                        var bookImage = new BookFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = file.FileName,
                            Book = book
                        };

                        book.BookImages.Add(bookImage);
                    }
                }
            }

            // Handle image deletions
            var deletedImageUrls = model.DeletedFileUrls?.Split(',') ?? Array.Empty<string>();
            foreach (var imageUrl in deletedImageUrls)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var file = _context.BookFiles.FirstOrDefault(r => r.FileUrl == imageUrl);
                    if (file != null)
                    {
                        _context.BookFiles.Remove(file);
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

            _context.Books.Update(book);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult EditUnAccept(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var model = _mapper.Map<UnAcceptedImportBookFormViewModel>(book);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnAccept(UnAcceptedImportBookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");

            var book = _context.Books.Include(b => b.Teams).FirstOrDefault(b => b.Id == model.Id);

            if (book is null)
                return NotFound();

            book = _mapper.Map(model, book);
            book.UserId = userId;
            book.IsAccepted = true;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "CanDeleteImport,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var book = _context.Books
                 .Include(r => r.BookImages).FirstOrDefault(x => x.Id == id);
            if (book is null)
                return NotFound();

            if (book.BookImages.Any())
            {
                var bookImagesCopy = book.BookImages.ToList();

                foreach (var item in bookImagesCopy)
                {
                    var oldFilePath = $"{_webHostEnvironment.WebRootPath}{item.FileUrl}";
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    _context.BookFiles.Remove(item);
                    _context.SaveChanges();
                }
            }


            _context.Books.Remove(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { id = book.Id });
        }
        public IActionResult Details(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var book = _context.Books
                .Include(b => b.BookImages)
                .Include(b => b.Entities)
               .Include(b => b.SubEntities)
               .Include(b => b.SecondSubEntities)
                .Include(b => b.User)
                 .Include(b => b.Teams).ThenInclude(t => t.Circle)
                .SingleOrDefault(b => b.Id == id);

            if (book is null)
                return NotFound();

            var viewModel = _mapper.Map<ImportBookViewModel>(book);

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

            var books = new ImportBookResult();
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = await LoadBooks(skip, pageSize, searchValue, sortColumn, sortColumnDirection, null, null, true);
            }
            else
            {
                books = await LoadBooks(skip, pageSize, searchValue, sortColumn, sortColumnDirection, null, null, false);
            }

            var jsonData = new { recordsFiltered = books.RecordsTotal, books.RecordsTotal, data = books.Books };

            return Ok(jsonData);
        }

        [HttpPost]
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
            var books = new ImportBookResult();
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = await LoadBooks(skip, pageSize, searchValue, sortColumn, sortColumnDirection, fromDate, toDate, true);
            }
            else
            {
                books = await LoadBooks(skip, pageSize, searchValue, sortColumn, sortColumnDirection, fromDate, toDate, false);
            }
            var jsonData = new { recordsFiltered = books.RecordsTotal, books.RecordsTotal, data = books.Books };

            return Ok(jsonData);
        }
        private async Task<ImportBookResult> LoadBooks(int skip, int pageSize, string search, string sortColumn, string sortDirection,
            DateTime? fromDate = null, DateTime? toDate = null, bool isSuperAdmin = false)
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
                    FromDate = fromDate, // يمكن أن تكون null
                    ToDate = toDate,     // يمكن أن تكون null
                    IsSuperAdmin = isSuperAdmin
                };

                using var multi = await connection.QueryMultipleAsync(
                    "GetFullAcceptedImportBooks",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // 1. الكتب
                var books = (await multi.ReadAsync<ImportBookViewModel>()).ToList();

                // 2. Entities
                var entities = (await multi.ReadAsync<(int BookId, string EntityName)>()).ToList();

                // 3. SubEntities
                var subEntities = (await multi.ReadAsync<(int BookId, string SubEntityName)>()).ToList();

                // 4. SecondSubEntities
                var secondSubEntities = (await multi.ReadAsync<(int BookId, string SecondSubEntityName)>()).ToList();
                var teams = (await multi.ReadAsync<(int BookId, string TeamName)>()).ToList();

                // 5. Total Count
                var recordsTotal = (await multi.ReadAsync<int>()).FirstOrDefault();

                // ربط الكيانات الفرعية بالكتب
                foreach (var book in books)
                {
                    book.Entities = entities.Where(e => e.BookId == book.Id).Select(e => e.EntityName).ToList();
                    book.SubEntities = subEntities.Where(e => e.BookId == book.Id).Select(e => e.SubEntityName).ToList();
                    book.SecondSubEntities = secondSubEntities.Where(e => e.BookId == book.Id).Select(e => e.SecondSubEntityName).ToList();
                    book.Circle = books.FirstOrDefault(b => b.Id == book.Id)?.Circle;
                    book.Teams = teams.Where(t => t.BookId == book.Id).Select(t => t.TeamName).ToList();

                }

                return new ImportBookResult
                {
                    Books = books,
                    RecordsTotal = recordsTotal
                };
            }
        }
        public class ImportBookResult
        {
            public List<ImportBookViewModel> Books { get; set; } = new();
            public int RecordsTotal { get; set; }
        }
        [HttpPost]
        public IActionResult GetImportBooksUnAccepted()
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
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            IQueryable<Book> books;
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                books = _context.Books
             .Include(b => b.Entities)
               .Include(b => b.SubEntities)
               .Include(b => b.SecondSubEntities)
              .Include(b => b.SideEntity)
             .Where(b => !b.IsExport && !b.IsAccepted);
            }
            else
            {
                books = _context.Books
             .Include(b => b.Entities)
               .Include(b => b.SubEntities)
               .Include(b => b.SecondSubEntities)
               .Include(b => b.SideEntity)
               .Include(b => b.Teams).ThenInclude(t => t.Circle)
              .Where(b => !b.IsExport && b.Teams.Any(t => t.CircleId == user.CircleId) && !b.IsAccepted && !b.IsHidden);
            }


            if (!string.IsNullOrEmpty(searchValue))
                books = books.Where(b => b.Title.Contains(searchValue)
                   || b.Entities.Any(e => e.Name.Contains(searchValue))
                   || b.SubEntities.Any(se => se.Name.Contains(searchValue))
                   //  || b.SideEntity.Name.Contains(searchValue)
                   || b.BookNumber.Contains(searchValue));

            books = books.OrderBy($"{sortColumn} {sortColumnDirection}");

            var data = books.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<ImportBookViewModel>>(data);

            var recordsTotal = books.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
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
        private ImportBookFormViewModel PopulateViewModel(ImportBookFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            var user = _userManager.Users.Include(u => u.Team).ThenInclude(t => t.Circle).SingleOrDefault(u => u.Id == userId);

            ImportBookFormViewModel viewModel = model is null ? new ImportBookFormViewModel() : model;

            var entities = _context.Entities.OrderBy(a => a.Name).ToList();
            var Subentities = _context.SubEntities.OrderBy(a => a.Name).ToList();
            var Sideentities = _context.Circles.OrderBy(a => a.Name).ToList();
            var SecondSubEntities = _context.SecondSubEntities.OrderBy(a => a.Name).ToList();

            List<Team> Teams;
            if (User.IsInRole(AppRoles.SuperAdmin))
                Teams = _context.Teams.OrderBy(a => a.Name).ToList();
            else
                Teams = _context.Teams.Where(t => t.CircleId == user!.Team!.CircleId).OrderBy(a => a.Name).ToList();

            viewModel.Teams = _mapper.Map<IEnumerable<SelectListItem>>(Teams);

            viewModel.Entities = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            viewModel.SubEntities = _mapper.Map<IEnumerable<SelectListItem>>(Subentities);
            viewModel.SecondSubEntities = _mapper.Map<IEnumerable<SelectListItem>>(SecondSubEntities);
            viewModel.SideEntities = _mapper.Map<IEnumerable<SelectListItem>>(Sideentities);
            viewModel.IsExport = false;
            return viewModel;
        }

        public IActionResult AllowItem(ImportBookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.BookDate == model.BookDate && !b.IsExport);
            var isAllowed = book is null || book.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHidden(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null || !User.IsInRole(AppRoles.SuperAdmin))
                return BadRequest("يجب تسجيل الدخول اولا");

            var book = _context.Books.Find(id);
            if (book is null) return NotFound();

            book.IsHidden = !book.IsHidden;

            _context.Update(book);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
