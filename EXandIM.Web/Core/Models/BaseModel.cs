using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EXandIM.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class BaseModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
