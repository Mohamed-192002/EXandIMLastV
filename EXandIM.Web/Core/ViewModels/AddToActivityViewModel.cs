using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class AddToActivityViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الجهه")]
        public int ActivityId { get; set; }
        public IEnumerable<SelectListItem>? Activities { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
