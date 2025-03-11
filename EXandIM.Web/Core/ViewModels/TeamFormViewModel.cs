using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class TeamFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الدائره")]
        public int CircleId { get; set; }
        public IEnumerable<SelectListItem>? Circles { get; set; }
        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم القسم")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,CircleId", ErrorMessage = "اسم الدائرة او المديرية موجوده")]
        public string Name { get; set; } = null!;

        public bool AcceptArchive { get; set; }

    }
}
