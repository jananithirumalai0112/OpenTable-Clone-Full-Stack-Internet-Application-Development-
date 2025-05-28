using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Data;
using OpenTableApp.Models;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace OpenTableApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RestaurantController> _logger;

        public RestaurantController(AppDbContext context, ILogger<RestaurantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private void PopulateViewData()
        {
            ViewBag.Metropolises = _context.Metropolises.ToList();
            ViewBag.PriceRanges = Enum.GetValues(typeof(PriceRange));
        }

        public IActionResult List(RestaurantFilterViewModel filters)
        {
            var query = _context.Restaurants.Include(r => r.Metropolis).AsQueryable();

            if (filters.MetropolisId.HasValue)
                query = query.Where(r => r.MetropolisId == filters.MetropolisId);

            if (filters.PriceRange.HasValue)
                query = query.Where(r => r.PriceRange == filters.PriceRange);

            if (!string.IsNullOrEmpty(filters.CuisineStyle))
                query = query.Where(r => r.CuisineStyle != null && r.CuisineStyle.Contains(filters.CuisineStyle));

            ViewBag.Filters = filters;
            PopulateViewData();
            return View(query.ToList());
        }

        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Restaurant model)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return View(model);
            }

            bool exists = _context.Restaurants.Any(r => r.Name == model.Name && r.MetropolisId == model.MetropolisId);
            if (exists)
            {
                ModelState.AddModelError("", "A restaurant with this name already exists in the selected metropolis.");
                PopulateViewData();
                return View(model);
            }

            _context.Restaurants.Add(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

        public IActionResult Edit(int id)
        {
            var restaurant = _context.Restaurants.Include(r => r.Metropolis).FirstOrDefault(r => r.Id == id);
            if (restaurant == null) return NotFound();

            PopulateViewData();
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult Edit(int id, Restaurant model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateViewData();
                return View(model);
            }

            try
            {
                _context.Restaurants.Update(model);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Restaurants.Any(r => r.Id == id))
                    return NotFound();

                _logger.LogError($"Concurrency conflict: {ex.Message}");
                ModelState.AddModelError("", "Another user has updated this record. Please reload and try again.");
                PopulateViewData();
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }
        public IActionResult Delete(int id)
        {
            var restaurant = _context.Restaurants.Include(r => r.Metropolis).FirstOrDefault(r => r.Id == id);
            if (restaurant == null) return NotFound();

            return View(restaurant);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(List));
        }
    }
}
