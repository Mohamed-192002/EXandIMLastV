using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class AddToActivityViewModel
    {
        public int Id { get; set; } // لو كتاب واحد
        public List<int> BookIds { get; set; } = new(); // لو كتب متعددة
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الجهه")]
        public int ActivityId { get; set; }
        public IEnumerable<SelectListItem>? Activities { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
