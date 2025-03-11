using EXandIM.Web.Core.Models;

namespace EXandIM.Web.Core.ViewModels
{
    public class ItemInActivityViewModel
    {
        public int Id { get; set; }
        public string? Book { get; set; }
        public string? Reading { get; set; }
        public string? Procedure { get; set; }
        public DateTime? ProcedureDate { get; set; }

    }
}
