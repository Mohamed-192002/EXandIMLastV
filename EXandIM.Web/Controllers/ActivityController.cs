using AutoMapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace EXandIM.Web.Controllers
{
    public class ActivityController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        [Authorize(Roles = "CanViewActivity,SuperAdmin")]
        public IActionResult Index()
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            List<ActivityBook> Activities;
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                Activities = [.. _context.Activities.Include(b => b.User)];
            }
            else
            {
                Activities = [.. _context.Activities.Include(b => b.User).Where(b => b.User!.Id == userId && !b.IsHidden)];
            }
            return View(Activities);
        }
        public IActionResult Details(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var activity = _context.Activities
                .Include(a => a.Books).ThenInclude(x => x.Book)
                .Include(a => a.Books).ThenInclude(x => x.Reading)
                 //  .Include(b => b.Teams).ThenInclude(t => t.Circle)
                 .Include(b => b.User)
                .FirstOrDefault(a => a.Id == id);
            if (activity == null)
                return BadRequest();

            return View(activity);
        }

        [HttpGet]
        [Authorize(Roles = "CanCreateActivity,SuperAdmin")]
        public IActionResult Create()
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).ThenInclude(t => t.Circle).SingleOrDefault(u => u.Id == userId);

            var activity = new ActivityBook
            {
                Books = [],
                UserId = userId
            };
            ViewBag.ExportBooks = GetExportBooks(user);
            ViewBag.ImportBooks = GetImportBooks(user);
            ViewBag.ReadingsBooks = GetReadingBooks(user);
            //  ViewBag.Teams = GetTeams(user);
            return View(activity);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ActivityBook activity, List<int> SelectedTeams)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");
            //activity.Teams = [];
            //foreach (var teamId in SelectedTeams)
            //{
            //    var team = await _context.Teams.FindAsync(teamId);
            //    if (team != null)
            //    {
            //        activity.Teams.Add(team);
            //    }
            //}
            //if (user.Team is not null)
            //{
            //    var userTeamId = user.TeamId;
            //    if (!activity.Teams.Any(team => team.Id == userTeamId))
            //    {
            //        var team = await _context.Teams.FindAsync(userTeamId);
            //        activity.Teams.Add(team);
            //    }
            //}
            _context.Activities.Add(activity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "CanEditActivity,SuperAdmin")]
        public IActionResult Edit(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
            if (user is null)
                return BadRequest();
            var activity = _context.Activities
               .Include(a => a.Books)
               // .Include(b => b.Teams)
               .FirstOrDefault(a => a.Id == id);

            if (activity is null)
                return NotFound();

            var viewModel = new ActivityViewModel
            {
                Id = id,
                Title = activity.Title,
                UserId = userId,
                newBooks = [],
                Books = activity.Books.OrderBy(x => x.SortOrder).ToList(),
                //    SelectedTeams = activity.Teams.Select(t => t.Id).ToList()
            };
            ViewBag.ExportBooks = GetExportBooks(user);
            ViewBag.ImportBooks = GetImportBooks(user);
            ViewBag.ReadingsBooks = GetReadingBooks(user);
            // ViewBag.Teams = GetTeams(user);

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.Include(u => u.Team).SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return BadRequest("لا يمكن العثور على المستخدم");

            var activity = await _context.Activities.Include(a => a.Books).ThenInclude(x => x.Book)
               .Include(a => a.Books).ThenInclude(x => x.Reading)
               .Include(b => b.User).ThenInclude(u => u.Team).FirstOrDefaultAsync(a => a.Id == model.Id);

            if (activity is null)
                return NotFound();
            if (activity.User is null)
                return NotFound();

            var orderSort = new List<double>();
            foreach (var book in activity.Books)
            {
                orderSort.Add(book.SortOrder);
                _context.ItemsInActivity.Remove(book);
            }

            activity.Title = model.Title;
            if (model.newBooks != null && model.newBooks.Count > 0)
            {
                if (model.Books != null && model.Books.Count > 0)
                    activity.Books = [.. model.Books, .. model.newBooks];
                else
                    activity.Books = model.newBooks;
            }
            else
            {
                activity.Books = model.Books;
            }

            var lastCount = orderSort.Count();
            for (int i = 0; i < activity.Books.Count; i++)
            {
                if (i < orderSort.Count)
                {
                    activity.Books.ToList()[i].SortOrder = orderSort[i];
                }
                else
                {
                    activity.Books.ToList()[i].SortOrder = i;
                }
            }
            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult AddBookToActivity(List<int> bookIds)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");

            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            List<ActivityBook> Activities;
            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                Activities = [.. _context.Activities.Include(b => b.User)];
            }
            else
            {
                Activities = [.. _context.Activities
            .Include(b => b.User)
            .Where(b => b.User!.Id == userId && !b.IsHidden)];
            }

            var viewModel = new AddToActivityViewModel
            {
                BookIds = bookIds,
                Activities = _mapper.Map<IEnumerable<SelectListItem>>(Activities)
            };

            return PartialView("_AddBookToActivity", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookToActivityAsync(List<int> bookIds, int ActivityId)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest(new { errorMessage = "يجب تسجيل الدخول اولا" });

            var activity = await _context.Activities
                .Include(a => a.Books).ThenInclude(x => x.Book)
                .FirstOrDefaultAsync(a => a.Id == ActivityId);

            if (activity is null)
                return NotFound();

            // جيب كل الكتب الموجودة في النشاط
            var orderedItems = activity.Books.OrderBy(b => b.SortOrder).ToList();

            foreach (var bookId in bookIds)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || activity.Books.Any(x => x.BookId == book.Id))
                    continue;

                DateTime? newItemDate = book.BookDate;
                if (newItemDate == null)
                    return BadRequest(new { errorMessage = "لا يوجد تاريخ للعنصر المراد إضافته" });

                var itemInActivity = new ItemInActivity
                {
                    BookId = book.Id,
                    ActivityBookId = activity.Id
                };

                // احسب مكان الكتاب الجديد بناءً على orderedItems
                double newSortOrder = CalculateSortOrder(orderedItems, newItemDate.Value);

                itemInActivity.SortOrder = newSortOrder;

                // ضيفه في الليست عشان الكتب اللي بعده تاخده في الاعتبار
                orderedItems.Add(itemInActivity);
                orderedItems = orderedItems.OrderBy(b => b.SortOrder).ToList();

                activity.Books.Add(itemInActivity);
            }

            _context.Update(activity);
            await _context.SaveChangesAsync();

            return Ok();
        }
        private double CalculateSortOrder(List<ItemInActivity> orderedItems, DateTime newItemDate)
        {
            for (int i = 0; i < orderedItems.Count - 1; i++)
            {
                var currentDate = orderedItems[i].Book?.BookDate ?? orderedItems[i].Reading?.BookDate;
                var nextDate = orderedItems[i + 1].Book?.BookDate ?? orderedItems[i + 1].Reading?.BookDate;

                if (currentDate <= newItemDate && newItemDate <= nextDate)
                {
                    return (orderedItems[i].SortOrder + orderedItems[i + 1].SortOrder) / 2.0;
                }
            }

            if (orderedItems.Count > 0 && newItemDate < (orderedItems[0].Book?.BookDate ?? orderedItems[0].Reading?.BookDate))
            {
                return orderedItems[0].SortOrder - 1;
            }

            return (orderedItems.LastOrDefault()?.SortOrder ?? 0) + 1;
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult AddReadingToActivity(List<int> readingIds)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest(new { errorMessage = "يجب تسجيل الدخول اولا" });

            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            List<ActivityBook> Activities;

            if (User.IsInRole(AppRoles.SuperAdmin))
            {
                Activities = [.. _context.Activities.Include(b => b.User)];
            }
            else
            {
                Activities = [.. _context.Activities
            .Include(b => b.User)
            .Where(b => b.User!.Id == userId && !b.IsHidden)];
            }

            var viewModel = new AddToActivityViewModel
            {
                BookIds = readingIds, // نخزن الـ IDs كلها
                Activities = _mapper.Map<IEnumerable<SelectListItem>>(Activities)
            };

            return PartialView("_AddReadingToActivity", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReadingToActivityAsync(List<int> bookIds, int ActivityId)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest(new { errorMessage = "يجب تسجيل الدخول اولا" });

            var activity = await _context.Activities
                .Include(a => a.Books).ThenInclude(x => x.Book)
                .Include(a => a.Books).ThenInclude(x => x.Reading)
                .Include(b => b.User).ThenInclude(u => u.Team)
                .FirstOrDefaultAsync(a => a.Id == ActivityId);

            if (activity is null)
                return NotFound(new { errorMessage = "غير موجود" });

            foreach (var bookId in bookIds)
            {
                var reading = await _context.Readings.FindAsync(bookId);
                if (reading == null || activity.Books.Any(x => x.ReadingId == reading.Id))
                    continue;

                DateTime? newItemDate = reading.BookDate;
                if (newItemDate == null)
                    return BadRequest(new { errorMessage = "لا يوجد تاريخ للعنصر المراد إضافته" });

                var itemInActivity = new ItemInActivity();
                itemInActivity.ReadingId = reading.Id;
                itemInActivity.ActivityBookId = activity.Id;

                // ترتيب القائمة الحالية بناء على التاريخ
                var orderedItems = activity.Books.OrderBy(b => b.SortOrder).ToList();


                // العثور على الموضع المناسب حسب التاريخ
                double? newSortOrder = null;

                for (int i = 0; i < orderedItems.Count - 1; i++)
                {
                    var currentDate = orderedItems[i].Book?.BookDate ?? orderedItems[i].Reading?.BookDate;
                    var nextDate = orderedItems[i + 1].Book?.BookDate ?? orderedItems[i + 1].Reading?.BookDate;

                    if (currentDate <= newItemDate && newItemDate <= nextDate)
                    {
                        var currentSort = orderedItems[i].SortOrder;
                        var nextSort = orderedItems[i + 1].SortOrder;

                        // اختر قيمة بين الاثنين
                        newSortOrder = (currentSort + nextSort) / 2.0;
                        break;
                    }
                }

                if (newSortOrder == null)
                {
                    // التاريخ أقدم من كل العناصر
                    if (orderedItems.Count > 0 && newItemDate < (orderedItems[0].Book?.BookDate ?? orderedItems[0].Reading?.BookDate))
                    {
                        newSortOrder = orderedItems[0].SortOrder - 1;
                    }
                    else
                    {
                        // أضفه في النهاية
                        newSortOrder = (orderedItems.LastOrDefault()?.SortOrder ?? 0) + 1;
                    }
                }

                itemInActivity.SortOrder = newSortOrder.Value;

                // الإضافة دون تعديل الباقي
                activity.Books.Add(itemInActivity);
            }

            _context.Update(activity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "CanDeleteActivity,SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null)
                return BadRequest("يجب تسجيل الدخول اولا");
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var activity = _context.Activities.Find(id);
            if (activity is null) return NotFound();
            _context.Remove(activity);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        private List<Book> GetExportBooks(ApplicationUser user)
        {
            List<Book> Books;
            if (User.IsInRole(AppRoles.SuperAdmin))
                Books = _context.Books.Where(b => b.IsExport).OrderBy(a => a.Title).ToList();
            else
                Books = _context.Books.Where(b => b.IsExport && b.Teams.Any(team => team.Id == user.Team!.Id) && !b.IsHidden).ToList();
            return Books;
        }

        private List<Book> GetImportBooks(ApplicationUser user)
        {
            List<Book> Books;
            if (User.IsInRole(AppRoles.SuperAdmin))
                Books = _context.Books.Where(b => !b.IsExport).OrderBy(a => a.Title).ToList();
            else
                Books = _context.Books.Where(b => !b.IsExport && b.Teams.Any(team => team.Id == user.Team!.Id) && !b.IsHidden).ToList();
            return Books;
        }

        private List<Reading> GetReadingBooks(ApplicationUser user)
        {
            List<Reading> readings;
            if (User.IsInRole(AppRoles.SuperAdmin))
                readings = _context.Readings.OrderBy(a => a.Title).ToList();
            else
                readings = _context.Readings.Where(r => !r.IsHidden).ToList();
            return readings;
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder(List<string> itemOrder, int activityBookId)
        {
            if (itemOrder == null || itemOrder.Count == 0)
            {
                return BadRequest(new { message = "No order data provided." });
            }
            List<string> numbers = [];
            foreach (string item in itemOrder)
            {
                string cleanedItem = item.Replace("??", "").Trim();
                numbers.Add(cleanedItem);
            }

            try
            {
                var activityBook = _context.Activities
                                      .Include(ab => ab.Books)
                                       .FirstOrDefault(ab => ab.Id == activityBookId);


                if (activityBook == null)
                {
                    return NotFound("ActivityBook not found.");
                }

                for (int i = 0; i < numbers.Count; i++)
                {
                    int item;
                    bool isInt = int.TryParse(numbers[i], out item);

                    var itemInActivity = activityBook.Books.FirstOrDefault(ia => ia.Id == item);

                    if (itemInActivity != null)
                    {
                        itemInActivity.SortOrder = i;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Order saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the order.", details = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHidden(int id)
        {
            var userId = GetAuthenticatedUser();
            if (userId == null || !User.IsInRole(AppRoles.SuperAdmin))
                return BadRequest("يجب تسجيل الدخول اولا");

            var activity = _context.Activities.Find(id);
            if (activity is null) return NotFound();

            activity.IsHidden = !activity.IsHidden;

            _context.Update(activity);
            await _context.SaveChangesAsync();
            return Ok();
        }



    }
}
