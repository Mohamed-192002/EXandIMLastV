using EXandIM.Web.Core.Models;
using EXandIM.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EXandIM.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult GetCounts()
        {
            var counts = new
            {
                importNum = _context.Books.Where(x => !x.IsExport).Count(),
                importNumPassed = _context.Books.Where(x => !x.IsExport && x.Passed).Count(),
                importNumUnPassed = _context.Books.Where(x => !x.IsExport && !x.Passed).Count(),
                exportNum = _context.Books.Where(x => x.IsExport).Where(x => x.IsExport).Count(),
                exportNumPassed = _context.Books.Where(x => x.IsExport && x.Passed).Count(),
                exportNumUnPassed = _context.Books.Where(x => x.IsExport && !x.Passed).Count(),
                readingsNum = _context.Readings.Count(),
                readingsNumPassed = _context.Readings.Count(r => r.Passed),
                readingsNumUnPassed = _context.Readings.Count(r => !r.Passed),
                activityNum = _context.Activities.Count()
            };

            return Json(counts);
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "a")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
