using Microsoft.EntityFrameworkCore;

namespace EXandIM.Web.Core.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string BookNumber { get; set; } = null!;
        public DateTime BookDate { get; set; }
        public ICollection<Entity> Entities { get; set; } = new List<Entity>();
        public bool Passed { get; set; } = default!;

        public ICollection<SubEntity> SubEntities { get; set; } = new List<SubEntity>();
        public ICollection<SecondSubEntity> SecondSubEntities { get; set; } = new List<SecondSubEntity>();

        public int SideEntityId { get; set; }
        public SideEntity SideEntity { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsAccepted { get; set; } = false;
        public string? ImportNumber { get; set; } = null!;
        public DateTime? ImportDate { get; set; }
        public string? Notes { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<BookFile> BookImages { get; set; } = new List<BookFile>();

        public bool IsReferTo { get; set; } = false;
        public string? ReferToBookNumber { get; set; }
        public DateTime? ReferDate { get; set; }

        public bool IsHidden { get; set; } = false;

    }
    public class BookFile
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
