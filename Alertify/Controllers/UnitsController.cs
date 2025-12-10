using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Alertify.Data;
using Alertify.Models;

namespace Alertify.Controllers
{
    public class UnitsController : Controller
    {
        private readonly AlertifyDbContext _context;

        public UnitsController(AlertifyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var alertifyDbContext = _context.Units.Include(u => u.Station);
            return View(await alertifyDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .Include(u => u.Station)
                .FirstOrDefaultAsync(m => m.UnitID == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        public IActionResult Create()
        {
            ViewData["StationID"] = new SelectList(_context.Stations, "StationID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,ServiceType,UnitStatus,ResponsiblePerson,ContactEmail,ContactPhone,StationID,Status")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                int parsedUserId = userId != null && int.TryParse(userId, out int uid) ? uid : 1;

                unit.CreatedBy = parsedUserId;
                unit.CreationDate = DateTime.Now;
                unit.ModifiedBy = parsedUserId;
                unit.ModificationDate = DateTime.Now;
                unit.Status = "Activo";

                _context.Add(unit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StationID"] = new SelectList(_context.Stations, "StationID", "Name", unit.StationID);
            return View(unit);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            ViewData["StationID"] = new SelectList(_context.Stations, "StationID", "Name", unit.StationID);
            return View(unit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UnitID,Code,Name,ServiceType,UnitStatus,ResponsiblePerson,ContactEmail,ContactPhone,StationID,Status")] Unit unit)
        {
            if (id != unit.UnitID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUnit = await _context.Units.FindAsync(id);
                    if (existingUnit == null)
                    {
                        return NotFound();
                    }

                    var duplicateCode = await _context.Units
                        .FirstOrDefaultAsync(u => u.Code == unit.Code && u.UnitID != id);
                    if (duplicateCode != null)
                    {
                        ModelState.AddModelError("Code", "El código de la unidad ya existe. Usa un código único.");
                        ViewData["StationID"] = new SelectList(_context.Stations, "StationID", "Name", unit.StationID);
                        return View(unit);
                    }

                    existingUnit.Code = unit.Code;
                    existingUnit.Name = unit.Name;
                    existingUnit.ServiceType = unit.ServiceType;
                    existingUnit.UnitStatus = unit.UnitStatus ?? "Disponible";
                    existingUnit.ResponsiblePerson = unit.ResponsiblePerson;
                    existingUnit.ContactEmail = unit.ContactEmail;
                    existingUnit.ContactPhone = unit.ContactPhone;
                    existingUnit.StationID = unit.StationID;
                    existingUnit.Status = unit.Status;

                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    int parsedUserId = userId != null && int.TryParse(userId, out int uid) ? uid : 1;
                    existingUnit.ModifiedBy = parsedUserId;
                    existingUnit.ModificationDate = DateTime.Now;

                    _context.Update(existingUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitExists(unit.UnitID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StationID"] = new SelectList(_context.Stations, "StationID", "Name", unit.StationID);
            return View(unit);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .Include(u => u.Station)
                .FirstOrDefaultAsync(m => m.UnitID == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit != null)
            {
                _context.Units.Remove(unit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.UnitID == id);
        }
    }
}
