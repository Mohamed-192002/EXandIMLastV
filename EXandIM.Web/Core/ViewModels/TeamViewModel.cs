namespace EXandIM.Web.Core.ViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string? Circle { get; set; }
        public string Name { get; set; } = null!;
        public bool AcceptArchive { get; set; } = false;

    }
}
