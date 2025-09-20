using EXandIM.Web.Core.Models;

namespace EXandIM.Web.Core.ViewModels
{
    public class ReferenceNumberDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<ReferenceNumberTimelineItem> Items { get; set; } = new List<ReferenceNumberTimelineItem>();

    }
    public class ReferenceNumberTimelineItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public Book? Book { get; set; }
        public Reading? Reading { get; set; }
    }


}
