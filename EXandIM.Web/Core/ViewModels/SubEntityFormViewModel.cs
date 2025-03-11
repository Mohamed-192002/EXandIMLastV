using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class SubEntityFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الجهه")]
        public int EntityId { get; set; }
        public IEnumerable<SelectListItem>? Entities { get; set; }

        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم المستوى الاول للجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,EntityId", ErrorMessage = "المستوى الاول للجهه موجوده")]
        public string Name { get; set; } = null!;
        public bool IsInside { get; set; } = false;

    }
}
