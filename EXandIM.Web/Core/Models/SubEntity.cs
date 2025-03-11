using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.Models
{
    [Index(nameof(Name), nameof(EntityId), IsUnique = true)]
    public class SubEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public int EntityId { get; set; }
        public Entity? Entity { get; set; }
        public bool IsInside { get; set; } = false;

        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Reading> Readings { get; set; } = new List<Reading>();


    }
}
