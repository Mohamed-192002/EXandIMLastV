using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class SecondSubEntityFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "الجهه")]
        public int EntityId { get; set; }
        public IEnumerable<SelectListItem>? Entities { get; set; }
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Display(Name = "المستوى الاول للجهه")]
        public int SubEntityId { get; set; }
        public IEnumerable<SelectListItem>? SubEntities { get; set; }

        [MaxLength(100, ErrorMessage = "لا يمكن أن يكون الحد الأقصى للطول أكثر من 100 حروف.")]
        [Display(Name = "اسم المستوى الثانى للجهه")]
        [Required(ErrorMessage = "الحقل مطلوب")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,EntityId", ErrorMessage = "المستوى الاول للجهه موجوده")]
        public string Name { get; set; } = null!;

        public bool AcceptArchive { get; set; } = false;

        public bool IsInside { get; set; }

    }
}
