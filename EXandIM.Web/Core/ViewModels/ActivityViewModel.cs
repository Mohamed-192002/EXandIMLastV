using EXandIM.Web.Core.Models;

namespace EXandIM.Web.Core.ViewModels
{
    public class ActivityViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<ItemInActivity>? Books { get; set; }
        public ICollection<ItemInActivity>? newBooks { get; set; }
      //  public List<int>? SelectedTeams { get; set; }
    }
}
