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
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.Admin)]
    public class EntityController(ApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var Entity = _context.Entities.ToList();
            var viewModel = _mapper.Map<IEnumerable<EntityViewModel>>(Entity);
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(EntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Entity = _mapper.Map<Entity>(viewModel);
            Entity.IsInside = viewModel.IsInside;
            _context.Entities.Add(Entity);
            _context.SaveChanges();
            return PartialView("_EntityRow", _mapper.Map<EntityViewModel>(Entity));

        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var Entity = _context.Entities.Find(id);
            if (Entity == null)
                return NotFound();
            var viewModel = _mapper.Map<EntityFormViewModel>(Entity);
            viewModel.IsInside = Entity.IsInside;
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(EntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var ExitEntity = _context.Entities.AsNoTracking().FirstOrDefault(x => x.Id == viewModel.Id);
            var Entity = _mapper.Map<Entity>(viewModel);
            if (Entity == null)
                return NotFound();
            Entity.Name = viewModel.Name;
            _context.Entities.Update(Entity);
            _context.SaveChanges();

            return PartialView("_EntityRow", _mapper.Map<EntityViewModel>(Entity));
        }
        public IActionResult Delete(int id)
        {
            var entity = _context.Entities.Find(id);
            if (entity == null)
                return NotFound();
            try
            {
                _context.Entities.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(EntityFormViewModel model)
        {
            var Entity = _context.Entities.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = Entity is null || Entity.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        [AjaxOnly]
        public IActionResult GetEntity(int entityId)
        {
            var entity = _context.Entities
                    .OrderBy(g => g.Name)
                    .Where(e => e.IsInside)
                    .FirstOrDefault(entity => entity.Id == entityId);
            if (entity == null) return Ok(true);
            else return Ok(false);
        }
        [AllowAnonymous]
        [AjaxOnly]
        public IActionResult GetSearchTitle(int titleFilterId)
        {
            IEnumerable<SelectListItem> titleSearchs;
            if (titleFilterId == 1)
            {
                var entities = _context.Entities.ToList();
                titleSearchs = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            }
            else if (titleFilterId == 2)
            {
                var entities = _context.SubEntities.ToList();
                titleSearchs = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            }
            else if (titleFilterId == 3)
            {
                var entities = _context.SideEntities.ToList();
                titleSearchs = _mapper.Map<IEnumerable<SelectListItem>>(entities);
            }
            else
            {
                titleSearchs = null;
            }

            return Json(titleSearchs);
        }
    }
}

