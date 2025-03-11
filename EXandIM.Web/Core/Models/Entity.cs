namespace EXandIM.Web.Core.Models
{
    public class Entity:BaseModel
    {
        public bool IsInside { get; set; } = false;

        public ICollection<Book> Books{ get; set; } = new List<Book>();
        public ICollection<Reading> Readings { get; set; } = new List<Reading>();
        public ICollection<SubEntity> SubEntities { get; set; } = new List<SubEntity>();
    }
}
