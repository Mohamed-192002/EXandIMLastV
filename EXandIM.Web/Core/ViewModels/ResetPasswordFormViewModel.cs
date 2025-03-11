using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {
        public string Id { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "كلمه المرور")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [MinLength(8, ErrorMessage = "يجب أن تتكون كلمات المرور من 8 أحرف على الأقل")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "تأكيد كلمه المرور"),
           Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين.")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
