using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class ReferenceNumberFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "الرقم المرجعى")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = "الرقم المرجعى موجود")]

        public string Name { get; set; } = null!;
    }
}
