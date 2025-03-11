using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.Models
{
    public class Team
    {
		public int Id { get; set; }

		[MaxLength(100)]
		public string Name { get; set; } = null!;
		public bool AcceptArchive { get; set; } = false;
        public int SideEntityId { get; set; }
        public SideEntity? SideEntity { get; set; }
        public int CircleId { get; set; }
        public Circle Circle { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Book>? Books { get; set; }
      //  public ICollection<ActivityBook>? Activities { get; set; }
    }
}
