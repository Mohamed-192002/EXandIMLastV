namespace EXandIM.Web.Core.ViewModels
{
    public class ReadingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime BookDate { get; set; }
        public bool Passed { get; set; }
        public string? UserFullName { get; set; }
        public string? Circle { get; set; }
        public string? Notes { get; set; }
        public List<string>? Entities { get; set; }
        public List<string>? SubEntities { get; set; }
        public List<string>? SecondSubEntities { get; set; }
        public IList<BookFileDisplay> ExistingFiles { get; set; } = new List<BookFileDisplay>();
    }
}
