using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace EXandIM.Web.Core.ViewModels
{
    public class UnAcceptedImportBookFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "رقم الوارد")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        public string ImportNumber { get; set; } = null!;

        [Display(Name = "تاريخ الورود")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [AssertThat("ImportDate <= Today()", ErrorMessage = "لا يمكن أن يكون التاريخ في المستقبل!")]
        public DateTime ImportDate { get; set; } = DateTime.Now;
    }
}
