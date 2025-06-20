using AutoMapper;
using EXandIM.Web.Core;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXandIM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.Admin)]
    public class SideEntityController(ApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var SideEntity = _context.SideEntities.ToList();
            var viewModel = _mapper.Map<IEnumerable<SideEntityViewModel>>(SideEntity);
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
        public IActionResult Create(SideEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var SideEntity = _mapper.Map<SideEntity>(viewModel);
            _context.SideEntities.Add(SideEntity);
            _context.SaveChanges();
            return PartialView("_SideEntityRow", _mapper.Map<SideEntityViewModel>(SideEntity));
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var SideEntity = _context.SideEntities.Find(id);
            if (SideEntity == null)
                return NotFound();
            var viewModel = _mapper.Map<SideEntityFormViewModel>(SideEntity);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(SideEntityFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var SideEntity = _mapper.Map<SideEntity>(viewModel);
            if (SideEntity == null)
                return NotFound();
            SideEntity.Name = viewModel.Name;
            _context.SideEntities.Update(SideEntity);
            _context.SaveChanges();
            return PartialView("_SideEntityRow", _mapper.Map<SideEntityViewModel>(SideEntity));
        }

        public IActionResult Delete(int id)
        {
            var entity = _context.SideEntities.Find(id);
            if (entity == null)
                return NotFound();
            try
            {
                _context.SideEntities.Remove(entity);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(SideEntityFormViewModel model)
        {
            var SideEntity = _context.SideEntities.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = SideEntity is null || SideEntity.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }

}
