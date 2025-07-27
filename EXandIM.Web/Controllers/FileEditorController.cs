using EXandIM.Web.Data;
using EXandIM.Web.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;

namespace EXandIM.Web.Controllers
{
    public class FileEditorController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public FileEditorController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult ViewFile(string fileUrl)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, fileUrl.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound("الملف غير موجود");

            using var document = PdfReader.Open(fullPath, PdfDocumentOpenMode.ReadOnly);
            ViewBag.PageCount = document.PageCount;
            ViewBag.FileName = Path.GetFileNameWithoutExtension(fileUrl);

            var response = new ViewFileResponse
            {
                FileUrl = fileUrl,
            };
            return View(response);
        }
        public class ViewFileResponse
        {
            public string FileUrl { get; set; }
        }
        [HttpPost]
        public IActionResult RemovePageAndRedirect(string fileUrl, int pageNumber)
        {
            var rootPath = _webHostEnvironment.WebRootPath;
            var inputPath = Path.Combine(rootPath, fileUrl.TrimStart('/'));

            if (!System.IO.File.Exists(inputPath))
                return NotFound("الملف غير موجود");
         
            PdfPageEditor.RemovePage(inputPath, pageNumber - 1); // 0-based index
            return RedirectToAction("ViewFile", new { fileUrl = fileUrl });
        }
        public class RemovePageDto
        {
            public string FileUrl { get; set; }
            public int PageToRemove { get; set; }
        }
        [HttpPost]
        public IActionResult AddImageToPage(string fileUrl, int pageNumber, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("الصورة غير صالحة");

            var rootPath = _webHostEnvironment.WebRootPath;
            var inputPath = Path.Combine(rootPath, fileUrl.TrimStart('/'));

            if (!System.IO.File.Exists(inputPath))
                return NotFound("الملف غير موجود");

            // استخدم المسار المحفوظ للدالة
            PdfPageEditor.InsertFileAsPage(inputPath, pageNumber - 1, imageFile);

          

            return RedirectToAction("ViewFile", new { fileUrl = fileUrl});
        }


    }
}
