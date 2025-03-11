
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.Models
{
    public class ActivityBook
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "عنوان السلسله مطلوب")]
        [Display(Name = "عنوان السلسله")]
        public string Title { get; set; }
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<ItemInActivity> Books { get; set; }
        //[Required(ErrorMessage = "الاقسام مطلوب")]
        //[Display(Name = "الاقسام")]
        //public ICollection<Team> Teams { get; set; }

    }
}
