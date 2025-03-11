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
    public class SubEntityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SubEntityController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var SubEntity = _context.SubEntities.Include(x => x.Entity).ToList();
            var viewModel = _mapper.Map<IEnumerable<SubEntityViewModel>>(SubEntity);
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
        public IActionResult Create(SubEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));
            var entity = _context.Entities.Find(viewModel.EntityId);
            if (entity.IsInside)
            {
                var circle = new Circle()
                {
                    EntityId = viewModel.EntityId,
                    Name = viewModel.Name,
                };
                _context.Circles.Add(circle);
                _context.SaveChanges();

                var side = new SideEntity
                {
                    Name = viewModel.Name,
                };
                _context.SideEntities.Add(side);
                _context.SaveChanges();
            }
            var SubEntity = _mapper.Map<SubEntity>(viewModel);
            SubEntity.IsInside = entity.IsInside;
            _context.SubEntities.Add(SubEntity);
            _context.SaveChanges();

            var x = _context.SubEntities.Include(x => x.Entity).SingleOrDefault(s => s.Id == SubEntity.Id);
            return PartialView("_SubEntityRow", _mapper.Map<SubEntityViewModel>(x));
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var SubEntity = _context.SubEntities.Include(s => s.Entity).SingleOrDefault(s => s.Id == id);
            if (SubEntity == null)
                return NotFound();
            var model = _mapper.Map<SubEntityFormViewModel>(SubEntity);
            var viewModel = PopulateViewModel(model);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(SubEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return PartialView("_Form", PopulateViewModel(viewModel));

            var existingSubEntity = _context.SubEntities.AsNoTracking().FirstOrDefault(s => s.Id == viewModel.Id);
            if (existingSubEntity == null)
                return NotFound();

            var subEntity = _mapper.Map<SubEntity>(viewModel);
            if (subEntity == null)
                return NotFound();

            var entity = _context.Entities.Find(viewModel.EntityId);
            if (entity == null)
                return NotFound();

            if (entity.IsInside)
            {
                var Sidentity = _context.SideEntities.FirstOrDefault(x => x.Name == entity.Name);
                if (Sidentity != null)
                {
                    Sidentity.Name = viewModel.Name;
                    _context.SideEntities.Update(Sidentity);
                }
                else
                {
                    var side = new SideEntity
                    {
                        Name = viewModel.Name,
                    };
                    _context.SideEntities.Add(side);
                    _context.SaveChanges();
                }
                var circle = _context.Circles.AsNoTracking().FirstOrDefault(c => c.Name == existingSubEntity.Name);
                if (circle != null)
                {
                    circle.EntityId = viewModel.EntityId;
                    circle.Name = viewModel.Name;
                    _context.Circles.Update(circle);
                }
                else
                {
                    var Newcircle = new Circle()
                    {
                        EntityId = viewModel.EntityId,
                        Name = viewModel.Name,
                    };
                    _context.Circles.Add(Newcircle);
                    _context.SaveChanges();
                }
            }
            //else
            //{
            //    //var circle = _context.Circles.AsNoTracking().FirstOrDefault(c => c.Name == existingSubEntity.Name);
            //    //_context.Circles.Remove(circle);
            //    var Sidentity = _context.SideEntities.FirstOrDefault(x => x.Name == existingSubEntity.Name);
            //    _context.SideEntities.Remove(Sidentity);

            //}


            _context.Entry(existingSubEntity).State = EntityState.Detached; // Detach the existing entity

            subEntity.Name = viewModel.Name;
            subEntity.EntityId = viewModel.EntityId;

            _context.SubEntities.Update(subEntity);
            _context.SaveChanges();

            var updatedSubEntity = _context.SubEntities
                .Include(x => x.Entity)
                .SingleOrDefault(s => s.Id == subEntity.Id);

            if (updatedSubEntity == null)
                return NotFound();

            return PartialView("_SubEntityRow", _mapper.Map<SubEntityViewModel>(updatedSubEntity));
        }

        public IActionResult Delete(int id)
        {
            var subEntity = _context.SubEntities.Include(s => s.Entity).FirstOrDefault(s => s.Id == id);
            if (subEntity == null)
                return NotFound();


            var circle = _context.Circles.FirstOrDefault(x => x.Name == subEntity.Name);
            if (circle != null)
            {
                _context.Circles.Remove(circle);
                _context.SaveChanges();
            }

            var SideEntities = _context.SideEntities.FirstOrDefault(x => x.Name == subEntity.Name);
            if (SideEntities != null)
            {
                _context.SideEntities.Remove(SideEntities);
                _context.SaveChanges();
            }

            _context.SubEntities.Remove(subEntity);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(SubEntityFormViewModel model)
        {
            var book = _context.SubEntities.SingleOrDefault(b => b.Name == model.Name && b.EntityId == model.EntityId);
            var isAllowed = book is null || book.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        private SubEntityFormViewModel PopulateViewModel(SubEntityFormViewModel? model = null)
        {
            SubEntityFormViewModel viewModel = model is null ? new SubEntityFormViewModel() : model;

            var entities = _context.Entities.OrderBy(a => a.Name).ToList();

            viewModel.Entities = _mapper.Map<IEnumerable<SelectListItem>>(entities);

            return viewModel;
        }
    }
}
