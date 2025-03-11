using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class PermissionsViewModel
    {
        public string? Id { get; set; }

        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف."), Display(Name = "الاسم بالكامل")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string FullName { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 20 حروف."), Display(Name = "اسم المستخدم")]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = "اسم المستخدم موجود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string UserName { get; set; } = null!;
        [Display(Name = "الرتبه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();

        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
