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
    public class SecondSubEntityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SecondSubEntityController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var SecondSubEntity = _context.SecondSubEntities.Include(x => x.Entity).Include(e => e.SubEntity).ToList();
            var viewModel = _mapper.Map<IEnumerable<SecondSubEntityViewModel>>(SecondSubEntity);
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
        public IActionResult Create(SecondSubEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));

            var entity = _context.Entities.Find(viewModel.EntityId);
            if (entity == null)
                return NotFound();

            var subEntity = _context.SubEntities.Find(viewModel.SubEntityId);
            if (subEntity == null)
                return NotFound();

            if (entity.IsInside)
            {
                var side = _context.SideEntities.FirstOrDefault(x => x.Name == subEntity.Name);
                var circle = _context.Circles.FirstOrDefault(x => x.Name == subEntity.Name);
                var team = new Team
                {
                    SideEntityId = side.Id,
                    CircleId = circle.Id,
                    Name = viewModel.Name,
                    AcceptArchive = viewModel.AcceptArchive,
                };
                    _context.Teams.Add(team);
                    _context.SaveChanges();
                
            }

            var secondSubEntity = new SecondSubEntity
            {
                EntityId = viewModel.EntityId,
                SubEntityId = viewModel.SubEntityId,
                Name = viewModel.Name,
                IsInside = entity.IsInside
            };

            _context.SecondSubEntities.Add(secondSubEntity);
            _context.SaveChanges();

            var savedSubEntity = _context.SecondSubEntities
                .Include(x => x.Entity)
                .Include(x => x.SubEntity)
                .SingleOrDefault(s => s.Id == secondSubEntity.Id);

            if (savedSubEntity == null)
                return NotFound();

            return PartialView("_SecondSubEntityRow", _mapper.Map<SecondSubEntityViewModel>(savedSubEntity));
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var SecondSubEntity = _context.SecondSubEntities.Include(x => x.Entity).Include(e => e.SubEntity).SingleOrDefault(s => s.Id == id);
            if (SecondSubEntity == null)
                return NotFound();
            var model = _mapper.Map<SecondSubEntityFormViewModel>(SecondSubEntity);
            var viewModel = PopulateViewModel(model);
            var subEntities = _context.SubEntities.OrderBy(a => a.Name).Where(s => s.EntityId == viewModel.EntityId).ToList();
            var team = _context.Teams.AsNoTracking().FirstOrDefault(c => c.Name == SecondSubEntity.Name);
            if (team != null)
                viewModel.AcceptArchive = team.AcceptArchive;
            viewModel.SubEntities = _mapper.Map<IEnumerable<SelectListItem>>(subEntities);
            viewModel.IsInside = SecondSubEntity.IsInside;
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(SecondSubEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));

            var existingSecondSubEntities = _context.SecondSubEntities.AsNoTracking().FirstOrDefault(s => s.Id == viewModel.Id);
            if (existingSecondSubEntities == null)
                return NotFound();

            var SecondSubEntity = _mapper.Map<SecondSubEntity>(viewModel);
            if (SecondSubEntity == null)
                return NotFound();

            var entity = _context.Entities.Find(viewModel.EntityId);
            if (entity == null)
                return NotFound();

            var subEntity = _context.SubEntities.Find(viewModel.SubEntityId);
            if (subEntity == null)
                return NotFound();

            if (entity.IsInside)
            {
                var team = _context.Teams.AsNoTracking().FirstOrDefault(c => c.Name == existingSecondSubEntities.Name);
                var side = _context.SideEntities.FirstOrDefault(x => x.Name == entity.Name);
                var circle = _context.Circles.FirstOrDefault(x => x.Name == subEntity.Name);
                if (team != null)
                {

                    team.SideEntityId = side.Id;
                    team.CircleId = circle.Id;
                    team.Name = viewModel.Name;
                    team.AcceptArchive = viewModel.AcceptArchive;

                    _context.Teams.Update(team);
                }
                else
                {
                    var NewTeam = new Team()
                    {
                        SideEntityId = side.Id,
                        CircleId = circle.Id,
                        Name = viewModel.Name
                    };
                    _context.Teams.Add(NewTeam);
                    _context.SaveChanges();
                };

            }
            else
            {
                if (existingSecondSubEntities.IsInside)
                {
                    var team = _context.Teams.AsNoTracking().FirstOrDefault(c => c.Name == existingSecondSubEntities.Name);
                    _context.Teams.Remove(team);
                }

            }
            SecondSubEntity.IsInside = entity.IsInside;
            SecondSubEntity.Name = viewModel.Name;
            SecondSubEntity.EntityId = viewModel.EntityId;
            SecondSubEntity.SubEntityId = viewModel.SubEntityId;
            _context.SecondSubEntities.Update(SecondSubEntity);
            _context.SaveChanges();
            var x = _context.SecondSubEntities.Include(x => x.Entity).Include(e => e.SubEntity).SingleOrDefault(s => s.Id == SecondSubEntity.Id);
            return PartialView("_SecondSubEntityRow", _mapper.Map<SecondSubEntityViewModel>(x));
        }

        public IActionResult Delete(int id)
        {
            var SecondSubEntity = _context.SecondSubEntities.Find(id);
            if (SecondSubEntity == null)
                return NotFound();
            try
            {
                if (SecondSubEntity.IsInside)
                {
                    var team = _context.Teams.FirstOrDefault(x => x.Name == SecondSubEntity.Name);
                    if(team != null)
                    {
                        _context.Teams.Remove(team);
                        _context.SaveChanges();
                    }
                  
                }
                _context.SecondSubEntities.Remove(SecondSubEntity);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(SecondSubEntityFormViewModel model)
        {
            var book = _context.SecondSubEntities.SingleOrDefault(b => b.Name == model.Name && b.EntityId == model.EntityId && b.SubEntityId == model.SubEntityId);
            var isAllowed = book is null || book.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        private SecondSubEntityFormViewModel PopulateViewModel(SecondSubEntityFormViewModel? model = null)
        {
            SecondSubEntityFormViewModel viewModel = model is null ? new SecondSubEntityFormViewModel() : model;

            var entities = _context.Entities.OrderBy(a => a.Name).ToList();
            viewModel.Entities = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            var subEntities = _context.SubEntities.OrderBy(a => a.Name).ToList();
            viewModel.SubEntities = _mapper.Map<IEnumerable<SelectListItem>>(subEntities);


            return viewModel;
        }
    }
}
