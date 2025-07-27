using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace EXandIM.Web.Helpers
{
    public static class PdfPageEditor
    {
        public static void RemovePage(string inputPath, string outputPath, int pageToRemove)
        {
            using (PdfDocument document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify))
            {
                if (pageToRemove > 0 && pageToRemove <= document.PageCount)
                {
                    document.Pages.RemoveAt(pageToRemove);
                    document.Save(outputPath);
                    Console.WriteLine($"تم حذف الصفحة رقم {pageToRemove}");
                }
                else
                {
                    Console.WriteLine("رقم الصفحة غير صالح.");
                }
            }
        }
        public static void RemovePage(string inputPath, int pageToRemove)
        {
            using (PdfDocument document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify))
            {
                if (pageToRemove >= 0 && pageToRemove < document.PageCount)
                {
                    document.Pages.RemoveAt(pageToRemove);
                    document.Save(inputPath); // حفظ فوق الملف نفسه
                    Console.WriteLine($"✅ تم حذف الصفحة رقم {pageToRemove + 1}");
                }
                else
                {
                    Console.WriteLine("❌ رقم الصفحة غير صالح.");
                }
            }
        }

        public static void InsertImagePage(string inputPath, string outputPath, int insertAtIndex, string imagePath)
        {
            using (PdfDocument document = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import)) // ✅ استخدم Import
            {
                if (insertAtIndex < 0 || insertAtIndex > document.PageCount)
                {
                    Console.WriteLine("❌ رقم الصفحة غير صالح.");
                    return;
                }

                PdfDocument newDoc = new PdfDocument();

                // نسخ الصفحات قبل الصفحة اللي هنضيف فيها الصورة
                for (int i = 0; i < insertAtIndex; i++)
                {
                    newDoc.AddPage(document.Pages[i]);
                }

                // إنشاء صفحة جديدة فيها صورة
                PdfPage imagePage = newDoc.AddPage();
                imagePage.Width = XUnit.FromMillimeter(210);
                imagePage.Height = XUnit.FromMillimeter(297);

                using (XGraphics gfx = XGraphics.FromPdfPage(imagePage))
                {
                    using (XImage image = XImage.FromFile(imagePath))
                    {
                        double imgWidth = image.PixelWidth * 72 / image.HorizontalResolution;
                        double imgHeight = image.PixelHeight * 72 / image.VerticalResolution;

                        double x = (imagePage.Width - imgWidth) / 2;
                        double y = (imagePage.Height - imgHeight) / 2;

                        gfx.DrawImage(image, x, y, imgWidth, imgHeight);
                    }
                }

                // نسخ بقية الصفحات
                for (int i = insertAtIndex; i < document.PageCount; i++)
                {
                    newDoc.AddPage(document.Pages[i]);
                }

                newDoc.Save(outputPath);
                Console.WriteLine($"✅ تم إدراج صفحة جديدة بالصورة في الموضع رقم {insertAtIndex + 1}");
            }
        }

        public static void InsertImagePage(string pdfPath, int insertAtPage, IFormFile imageFile)
        {
            // إعداد مسارات الملفات المؤقتة
            string tempDir = Path.Combine(Path.GetTempPath(), "PdfTemp");
            Directory.CreateDirectory(tempDir);

            string originalImagePath = Path.Combine(tempDir, Guid.NewGuid() + Path.GetExtension(imageFile.FileName));
            using (var stream = new FileStream(originalImagePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            // تحويل الصورة إلى PNG (إن لم تكن PNG أو JPG)
            string finalImagePath = Path.GetExtension(originalImagePath).ToLower() switch
            {
                ".jpg" or ".jpeg" or ".png" => originalImagePath,
                _ => ConvertImageToPng(originalImagePath, tempDir)
            };

            // تحميل الصورة
            using var xImage = XImage.FromFile(finalImagePath);

            // تحميل ملف PDF
            using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Modify);

            // إنشاء صفحة جديدة
            PdfPage page = new PdfPage();
            document.Pages.Insert(insertAtPage, page);

            // إعداد الجرافيكس ورسم الصورة
            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
            }

            // حفظ الملف بعد التعديل
            document.Save(pdfPath);

            // حذف الصور المؤقتة
            File.Delete(originalImagePath);
            if (finalImagePath != originalImagePath)
                File.Delete(finalImagePath);
        }

        public static string ConvertImageToPng(string originalImagePath, string tempDir)
        {
            var extension = Path.GetExtension(originalImagePath).ToLower();
            var finalImagePath = Path.Combine(tempDir, Guid.NewGuid() + ".png");

            // نحول فقط إذا لم تكن PNG بالفعل
            if (extension != ".png")
            {
                using var image = SixLabors.ImageSharp.Image.Load(originalImagePath);
                image.Save(finalImagePath, new PngEncoder());

                File.Delete(originalImagePath); // حذف الأصل
            }
            else
            {
                finalImagePath = originalImagePath; // لا حاجة للتحويل
            }

            return finalImagePath;
        }
        public static void InsertPdfIntoPdf(string mainPdfPath, int insertAtPage, IFormFile pdfFile)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "PdfTemp");
            Directory.CreateDirectory(tempDir);

            // حفظ ملف الـ PDF المؤقت
            string tempPdfPath = Path.Combine(tempDir, Guid.NewGuid() + ".pdf");
            using (var stream = new FileStream(tempPdfPath, FileMode.Create))
            {
                pdfFile.CopyTo(stream);
            }

            // فتح الملف الرئيسي والملف اللي هيتم إدراجه
            using var mainDoc = PdfReader.Open(mainPdfPath, PdfDocumentOpenMode.Modify);
            using var insertDoc = PdfReader.Open(tempPdfPath, PdfDocumentOpenMode.Import);

            // إدراج كل الصفحات من الملف الثاني
            for (int i = 0; i < insertDoc.PageCount; i++)
            {
                var page = insertDoc.Pages[i];
                mainDoc.Pages.Insert(insertAtPage + i, page);
            }

            // حفظ التغييرات
            mainDoc.Save(mainPdfPath);

            // حذف الملف المؤقت
            File.Delete(tempPdfPath);
        }

        public static void InsertFileAsPage(string filePath, int insertAtPage, IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();
            string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff", ".webp" };

            if (imageExtensions.Contains(extension))
            {
                InsertImagePage(filePath, insertAtPage, file);
            }
            else if (extension == ".pdf")
            {
                InsertPdfIntoPdf(filePath, insertAtPage, file);
            }
            else
            {
                throw new NotSupportedException("الملف المدخل ليس صورة أو PDF");
            }
        }

    }

}
