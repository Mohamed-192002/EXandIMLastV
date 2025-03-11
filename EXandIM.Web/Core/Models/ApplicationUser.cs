using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        public bool IsDeleted { get; set; }
        [MaxLength(500)]
        public string ImageUrl { get; set; } = null!;
        [MaxLength(500)]
        public string ImageThumbnailUrl { get; set; } = null!;
        public int? CircleId { get; set; }
        public Circle? Circle { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public string password { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<Reading> Readings { get; set; }
        public ICollection<ActivityBook> ActivityBooks { get; set; }
    }
}
