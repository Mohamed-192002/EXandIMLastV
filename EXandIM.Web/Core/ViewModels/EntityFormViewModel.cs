using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class EntityFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "الجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = "الجهه موجوده")]

        public string Name { get; set; } = null!;
        public bool IsInside { get; set; } = false;

    }
}
