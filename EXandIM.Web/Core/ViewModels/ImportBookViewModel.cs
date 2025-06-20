namespace EXandIM.Web.Core.ViewModels
{
    public class ImportBookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime BookDate { get; set; }
        public string? BookNumber { get; set; }
        //    public string? FileUrl { get; set; }
        public IList<BookFileDisplay> ExistingFiles { get; set; } = new List<BookFileDisplay>();
        public List<string>? Entities { get; set; }
        public List<string>? SubEntities { get; set; }
        public List<string>? SecondSubEntities { get; set; }
        public string? SideEntity { get; set; }
        public string ImportNumber { get; set; } = null!;
        public DateTime ImportDate { get; set; }
        public string? Notes { get; set; }
        public bool IsAccepted { get; set; }
        public string? Circle { get; set; }
        public string? UserFullName { get; set; }

        public List<string> Teams { get; set; }
        public bool Passed { get; set; }
        public bool IsHidden { get; set; }

    }
}
