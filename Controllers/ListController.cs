using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Data;
using OpenTableApp.Models;
using OpenTableApp.ExtensionMethods;
using System.Linq;

namespace OpenTable.Controllers
{
    public class ListController : Controller
    {
        private readonly AppDbContext _context;

        public ListController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult List(RestaurantFilterViewModel? filters)
        {
            if (filters == null || (string.IsNullOrEmpty(filters.CuisineStyle) && !filters.MetropolisId.HasValue && !filters.PriceRange.HasValue))
            {
                filters = HttpContext.Session.GetObject<RestaurantFilterViewModel>("Filter") ?? new RestaurantFilterViewModel();
            }
            else
            {
                HttpContext.Session.SetObject("Filter", filters);
            }

            var query = _context.Restaurants.Include(r => r.Metropolis).AsQueryable();

            if (filters.MetropolisId.HasValue)
                query = query.Where(r => r.MetropolisId == filters.MetropolisId);

            if (filters.PriceRange.HasValue)
                query = query.Where(r => r.PriceRange == filters.PriceRange);

            if (!string.IsNullOrEmpty(filters.CuisineStyle))
                query = query.Where(r => r.CuisineStyle != null && r.CuisineStyle.Contains(filters.CuisineStyle));

            filters.Restaurants = query.ToList();

            filters.Metropolises = _context.Metropolises.ToList();
            filters.CuisineStyles = _context.Restaurants
                .Where(r => r.CuisineStyle != null)
                .Select(r => r.CuisineStyle)
                .Distinct()
                .ToList();

            // ✅ Updated: Get cart count using SessionWrapper extension
            ViewBag.CartCount = HttpContext.Session.GetCartCount();

            return View(filters);
        }

        [HttpPost]
        public IActionResult ClearFilter()
        {
            HttpContext.Session.Remove("Filter");
            return RedirectToAction("List");
        }
    }
}
