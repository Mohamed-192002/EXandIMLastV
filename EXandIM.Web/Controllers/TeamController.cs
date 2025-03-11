using AutoMapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EXandIM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class TeamController(ApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var Team = _context.Teams.Include(x => x.Circle).ToList();
            var viewModel = _mapper.Map<IEnumerable<TeamViewModel>>(Team);
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form", PopulateViewModel());
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(TeamFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));
            var Team = new Team
            {
                CircleId = viewModel.CircleId,
                Name = viewModel.Name,
                AcceptArchive = viewModel.AcceptArchive,
            };
            _context.Teams.Add(Team);
            _context.SaveChanges();
            var x = _context.Teams.Include(t=>t.Circle).SingleOrDefault(t => t.Id == Team.Id);
            return PartialView("_TeamRow", _mapper.Map<TeamViewModel>(x));
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var Team = _context.Teams.Include(t => t.Circle).SingleOrDefault(s => s.Id == id);
            if (Team == null)
                return NotFound();
            var model = _mapper.Map<TeamFormViewModel>(Team);
            var viewModel = PopulateViewModel(model);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(TeamFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));
            var Team = _mapper.Map<Team>(viewModel);
            if (Team == null)
                return NotFound();
            Team.Name = viewModel.Name;
            Team.CircleId = viewModel.CircleId;
            Team.AcceptArchive = viewModel.AcceptArchive;
            _context.Teams.Update(Team);
            _context.SaveChanges();
            var x = _context.Teams.Include(t => t.Circle).SingleOrDefault(t => t.Id == Team.Id);
            return PartialView("_TeamRow", _mapper.Map<TeamViewModel>(x));
        }
        public IActionResult Delete(int id)
        {
            var Team = _context.Teams.Include(t => t.Users).FirstOrDefault(t => t.Id == id);
            if (Team == null)
                return NotFound();

            if (Team.Users.All(u => u.IsDeleted))
            {
                _context.Users.RemoveRange(Team.Users);
                _context.Teams.Remove(Team);
                _context.SaveChanges();
            }
            else
                return BadRequest();

            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(TeamFormViewModel model)
        {
            var team = _context.Teams.SingleOrDefault(b => b.Name == model.Name && b.CircleId == model.CircleId);
            var isAllowed = team is null || team.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        private TeamFormViewModel PopulateViewModel(TeamFormViewModel? model = null)
        {
            TeamFormViewModel viewModel = model is null ? new TeamFormViewModel() : model;

            var circles = _context.Circles.OrderBy(a => a.Name).ToList();

            viewModel.Circles = _mapper.Map<IEnumerable<SelectListItem>>(circles);

            return viewModel;
        }
    }
}
