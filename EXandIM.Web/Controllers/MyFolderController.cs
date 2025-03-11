using AutoMapper;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;

namespace EXandIM.Web.Controllers
{
    public class MyFolderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public MyFolderController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.MyFolders.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateFolder()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFolder([Bind("FolderPath")] MyFolderViewModel folderViewModel)
        {
            if (ModelState.IsValid)
            {
                var folder = new MyFolder()
                {
                    FolderPath = folderViewModel.FolderPath,
                };
                folder.FolderName = Path.GetFileName(folderViewModel.FolderPath.TrimEnd(Path.DirectorySeparatorChar));
                _context.Add(folder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(folderViewModel);
        }
        [HttpGet]
        public IActionResult EditFolder(int id)
        {
            var folder = _context.MyFolders.Find(id);
            return View(new MyFolderViewModel
            {
                FolderPath = folder.FolderPath
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFolder(int id, [Bind("FolderPath")] MyFolderViewModel folderViewModel)
        {
            if (ModelState.IsValid)
            {
                var folder = _context.MyFolders.Find(id);
                folder.FolderPath = folderViewModel.FolderPath;
                folder.FolderName = Path.GetFileName(folderViewModel.FolderPath.TrimEnd(Path.DirectorySeparatorChar));
                _context.Update(folder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(folderViewModel);
        }
        [AjaxOnly]
        public async Task<IActionResult> Open(int? id)
        {
            if (id == null)
                return NotFound();

            var folder = await _context.MyFolders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (folder == null)
                return NotFound();

            try
            {
                #region source
                if (Directory.Exists(folder.FolderPath))
                {
                    Process.Start("explorer.exe", folder.FolderPath);
                }

                else
                    throw new Exception();
                #endregion


            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var entity = _context.MyFolders.Find(id);
            if (entity == null)
                return NotFound();
            try
            {
                _context.MyFolders.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
    }
}
