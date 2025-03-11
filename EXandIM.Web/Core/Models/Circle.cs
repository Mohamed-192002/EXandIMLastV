using EXandIM.Web.Core.Models;

namespace EXandIM.Web.Core.Models
{
    public class Circle : BaseModel
    {
        public int EntityId { get; set; }
        public Entity? Entity { get; set; }
        public ICollection<Team> Teams { get; set; }
    }
}
