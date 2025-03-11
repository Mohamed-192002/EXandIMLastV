using AutoMapper;
using EXandIM.Web.Data;
using EXandIM.Web.Filters;
using EXandIM.Web.Core.Models;
using EXandIM.Web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EXandIM.Web.Controllers
{
    public class CircleController(ApplicationDbContext context, IMapper mapper) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public IActionResult Index()
        {
            var Circle = _context.Circles.ToList();
            var viewModel = _mapper.Map<IEnumerable<CircleViewModel>>(Circle);
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
        public IActionResult Create(CircleFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Circle = _mapper.Map<Circle>(viewModel);
            _context.Circles.Add(Circle);
            _context.SaveChanges();
            return PartialView("_CircleRow", _mapper.Map<CircleViewModel>(Circle));
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var Circle = _context.Circles.Find(id);
            if (Circle == null)
                return NotFound();
            var viewModel = _mapper.Map<CircleFormViewModel>(Circle);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(CircleFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var Circle = _mapper.Map<Circle>(viewModel);
            if (Circle == null)
                return NotFound();
            Circle.Name = viewModel.Name;
            _context.Circles.Update(Circle);
            _context.SaveChanges();
            return PartialView("_CircleRow", _mapper.Map<CircleViewModel>(Circle));
        }
        public IActionResult Delete(int id)
        {
            var Circle = _context.Circles.Find(id);
            if (Circle == null)
                return NotFound();
            try
            {
                _context.Circles.Remove(Circle);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public IActionResult AllowItem(CircleFormViewModel model)
        {
            var Circle = _context.Circles.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = Circle is null || Circle.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }
}
