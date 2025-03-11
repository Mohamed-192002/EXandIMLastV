using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class SideEntityFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "الجهه الداخليه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = "الجهه موجوده")]

        public string Name { get; set; } = null!;
    }
}
