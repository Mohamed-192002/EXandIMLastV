﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace EXandIM.Web.Core.ViewModels
{
    public class ReadingFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,BookDate", ErrorMessage = "هذه المطالعه موجوده فى نفس هذا التاريخ")]
        [Display(Name = " موضوع المطالعه او الاجتماع")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string Title { get; set; } = null!;


        [Display(Name = "التاريخ ")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,Title", ErrorMessage = "هذه المطالعه موجوده فى نفس هذا التاريخ")]
        public DateTime BookDate { get; set; } = DateTime.Now;

        [Display(Name = "صور المطالعه او الاجتماع")]
        public IList<IFormFile>? ReadingFiles { get; set; } = new List<IFormFile>();
        public IList<BookFileDisplay> ExistingFiles { get; set; } = new List<BookFileDisplay>();
        public string? DeletedFileUrls { get; set; }


        [Display(Name = "هل تم المراجعه؟")]
        public bool Passed { get; set; } = default!;



        [Display(Name = "اسم الجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<int>? SelectedEntities { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Entities { get; set; }

        [Display(Name = "المستوى الاول للجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<int>? SelectedSubEntity { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? SubEntities { get; set; }

        [Display(Name = "المستوى الثانى للجهه")]
        public IList<int>? SelectedSecondSubEntity { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? SecondSubEntities { get; set; }



        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
        

    }
}
