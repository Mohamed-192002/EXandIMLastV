using Microsoft.EntityFrameworkCore;

namespace EXandIM.Web.Core.Models
{
    [Index(nameof(Title), nameof(BookDate), IsUnique = true)]
    public class Reading
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime BookDate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public bool Passed { get; set; } = default!;

        public string? Notes { get; set; }
        public ICollection<Entity> Entities { get; set; } = new List<Entity>();

        public ICollection<SubEntity> SubEntities { get; set; } = new List<SubEntity>();
        public ICollection<SecondSubEntity> SecondSubEntities { get; set; } = new List<SecondSubEntity>();

        public ICollection<ReadingFile> ReadingImages { get; set; } = new List<ReadingFile>();
        public bool IsHidden { get; set; } = false;

    }
    public class ReadingFile
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int ReadingId { get; set; }
        public Reading Reading { get; set; } = null!;
    }
}
