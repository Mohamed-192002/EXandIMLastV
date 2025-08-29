using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace EXandIM.Web.Core.ViewModels
{
    public class ImportBookFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,BookDate,IsExport", ErrorMessage = "هذا الوارد موجود فى نفس هذا التاريخ")]
        [Display(Name = "عنوان الكتاب")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string Title { get; set; } = null!;

        [Display(Name = "اسم الجهة الواردة منها")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<int>? SelectedEntities { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Entities { get; set; }
        [Display(Name = "المستوى الاول للجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<int>? SelectedSubEntity { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? SubEntities { get; set; }
        [Display(Name = "المستوى الثانى للجهه")]
        //  [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<int>? SelectedSecondSubEntity { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? SecondSubEntities { get; set; }
        [Display(Name = "اسم الجهة المرسل إليها")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public int SideEntityId { get; set; }
        public IEnumerable<SelectListItem>? SideEntities { get; set; }
        [Display(Name = "القسم")]
        public IList<int>? SelectedTeams { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Teams { get; set; }
        [Display(Name = "تاريخ الكتاب")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        // [AssertThat("BookDate <= Today()", ErrorMessage = "لا يمكن أن يكون التاريخ في المستقبل!")]
        [NotFutureDate(ErrorMessage = "لا يمكن أن يكون التاريخ في المستقبل!")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,Title,IsExport", ErrorMessage = "هذا الوارد موجود فى نفس هذا التاريخ")]
        public DateTime BookDate { get; set; } = DateTime.Now;
        //[RequiredIf("Id == 0", ErrorMessage = "الحقل مطلوب")]
        //[Display(Name = "الكتاب")]
        //public IFormFile? BookFile { get; set; }
        //public string? FileUrl { get; set; }
        [Display(Name = "صور الكتاب")]
        public IList<IFormFile>? BookFiles { get; set; } = new List<IFormFile>();
        public IList<BookFileDisplay> ExistingFiles { get; set; } = new List<BookFileDisplay>();
        public string? DeletedFileUrls { get; set; }



        [Display(Name = "رقم الكتاب")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string BookNumber { get; set; } = null!;

        [Display(Name = "رقم الوارد")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string ImportNumber { get; set; } = null!;

        [Display(Name = "تاريخ الورود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        //   [AssertThat("ImportDate <= Today()", ErrorMessage = "لا يمكن أن يكون التاريخ في المستقبل!")]
        [NotFutureDate(ErrorMessage = "لا يمكن أن يكون التاريخ في المستقبل!")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        [Remote("AllowItem", null!, AdditionalFields = "Id,Title,BookDate", ErrorMessage = "هذا الصادر موجود فى نفس هذا التاريخ")]
        public bool IsExport { get; set; } = true;

        [Display(Name = "هل تم المراجعه؟")]
        public bool Passed { get; set; } = default!;
        [Display(Name = "هل يوجد كتاب مشار اليه؟")]
        public bool IsReferTo { get; set; } = default!;

        [Display(Name = "تاريخ الكتاب المشار اليه")]
        public DateTime? ReferDate { get; set; } = DateTime.Now;
        [Display(Name = "رقم الكتاب المشار اليه")]
        public string? ReferToBookNumber { get; set; } = null!;
    }
}
