using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace EXandIM.Web.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف."), Display(Name = "الاسم بالكامل")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string FullName { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 20 حروف."), Display(Name = "اسم المستخدم")]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = "اسم المستخدم موجود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string UserName { get; set; } = null!;
        [Display(Name = "الدائره")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public int CircleId { get; set; }
        public IEnumerable<SelectListItem>? Circles { get; set; }
        [Display(Name = "القسم")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public int TeamId { get; set; }
        public IEnumerable<SelectListItem>? Teams { get; set; }

        [DataType(DataType.Password), Display(Name = "كلمه المرور")]
        [RequiredIf("Id==null", ErrorMessage = "الحقل مطلوب")]
        [MinLength(8, ErrorMessage = "يجب أن تتكون كلمات المرور من 8 أحرف على الأقل")]
        public string? Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "تأكيد كلمه المرور"),
            Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين.")]
        [RequiredIf("Id==null", ErrorMessage = "الحقل مطلوب")]
        public string? ConfirmPassword { get; set; } = null!;

        [Display(Name = "الرتبه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();

        public IEnumerable<SelectListItem>? Roles { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
    }
}
