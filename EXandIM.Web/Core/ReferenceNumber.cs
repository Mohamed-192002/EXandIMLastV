using EXandIM.Web.Core.Models;

namespace EXandIM.Web.Core
{
    public class ReferenceNumber : BaseModel
    {
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Reading> Readings { get; set; } = new List<Reading>();
    }
}
