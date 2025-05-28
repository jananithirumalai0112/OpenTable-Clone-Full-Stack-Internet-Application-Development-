using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTableApp.Data;
using OpenTableApp.Models;
using System;
using System.Linq;

namespace OpenTableApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class MetropolisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MetropolisController> _logger;

        public MetropolisController(AppDbContext context, ILogger<MetropolisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult List()
        {
            _logger.LogInformation("DEBUG: Fetching list of Metropolises...");
            var metropolises = _context.Metropolises.ToList();
            return View(metropolises);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("DEBUG: Entering Create (GET) method.");
            return View(new Metropolis());
        }

        [HttpPost]
        public IActionResult Create(Metropolis metropolis)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("DEBUG: ModelState is invalid.");
                    return View(metropolis);
                }

                _context.Add(metropolis);
                _context.SaveChanges();

                _logger.LogInformation("DEBUG: Metropolis successfully created.");
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the metropolis.");
                return View(metropolis);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("WARNING: Edit ID is null.");
                return NotFound();
            }

            var metropolis = _context.Metropolises.Find(id);
            if (metropolis == null)
            {
                _logger.LogWarning("WARNING: Metropolis not found.");
                return NotFound();
            }

            return View(metropolis);
        }

        [HttpPost]
        public IActionResult Edit(int id, Metropolis metropolis)
        {
            if (id != metropolis.Id)
            {
                _logger.LogWarning("WARNING: Edit ID mismatch.");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("WARNING: ModelState is invalid for Edit.");
                return View(metropolis);
            }

            try
            {
                _context.Update(metropolis);
                _context.SaveChanges();

                _logger.LogInformation("DEBUG: Metropolis successfully updated.");
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the metropolis.");
                return View(metropolis);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("WARNING: Delete ID is null.");
                return NotFound();
            }

            var metropolis = _context.Metropolises.Find(id);
            if (metropolis == null)
            {
                _logger.LogWarning("WARNING: Metropolis not found.");
                return NotFound();
            }

            return View(metropolis);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var metropolis = _context.Metropolises.Find(id);
            if (metropolis != null)
            {
                _context.Metropolises.Remove(metropolis);
                _context.SaveChanges();

                _logger.LogInformation("DEBUG: Metropolis successfully deleted.");
            }

            return RedirectToAction(nameof(List));
        }
    }
}
