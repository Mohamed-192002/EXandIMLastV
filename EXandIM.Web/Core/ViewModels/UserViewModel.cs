namespace EXandIM.Web.Core.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Team { get; set; } = null!;
        public string Circle { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public string? Role { get; set; }
    }
}
