using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.ViewModels
{
    public class MyFolderViewModel
    {
        [Required(ErrorMessage ="هذا الحقل مطلوب")]
        public string FolderPath { get; set; }

    }
}
