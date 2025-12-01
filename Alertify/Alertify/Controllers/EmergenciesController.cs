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
    public class EmergenciesController : Controller
    {
        private readonly AlertifyDbContext _context;

        public EmergenciesController(AlertifyDbContext context)
        {
            _context = context;
        }

        // GET: Emergencies
        public async Task<IActionResult> Index()
        {
            var alertifyDbContext = _context.Emergencies.Include(e => e.Citizen);
            return View(await alertifyDbContext.ToListAsync());
        }

        // GET: Emergencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(m => m.EmergencyID == id);
            if (emergency == null)
            {
                return NotFound();
            }

            return View(emergency);
        }

        // GET: Emergencies/Create
        public IActionResult Create()
        {
            ViewData["CitizenID"] = new SelectList(_context.Users, "UserID", "Email");
            return View();
        }

        // POST: Emergencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Emergency emergency)
        {
            if (ModelState.IsValid)
            {
                // Asigna automáticamente el usuario logueado
                emergency.CitizenID = int.Parse(User.FindFirst("UserID").Value);

                // Campos automáticos
                emergency.CreatedBy = emergency.CitizenID;
                emergency.CreationDate = DateTime.Now;
                emergency.EmergencyStatus = "Pending";
                emergency.Status = "Active";

                _context.Add(emergency);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(emergency);
        }


        // GET: Emergencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergency = await _context.Emergencies.FindAsync(id);
            if (emergency == null)
            {
                return NotFound();
            }
            ViewData["CitizenID"] = new SelectList(_context.Users, "UserID", "Email", emergency.CitizenID);
            return View(emergency);
        }

        // POST: Emergencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmergencyID,EmergencyCategory,Description,Latitude,Longitude,Address,LocationReference,ImageURL,EmergencyStatus,Priority,AssignmentDate,ResolutionDate,CitizenID,CreatedBy,CreationDate,ModifiedBy,ModificationDate,Status")] Emergency emergency)
        {
            if (id != emergency.EmergencyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emergency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmergencyExists(emergency.EmergencyID))
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
            ViewData["CitizenID"] = new SelectList(_context.Users, "UserID", "Email", emergency.CitizenID);
            return View(emergency);
        }

        // GET: Emergencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(m => m.EmergencyID == id);
            if (emergency == null)
            {
                return NotFound();
            }

            return View(emergency);
        }

        // POST: Emergencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emergency = await _context.Emergencies.FindAsync(id);
            if (emergency != null)
            {
                _context.Emergencies.Remove(emergency);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmergencyExists(int id)
        {
            return _context.Emergencies.Any(e => e.EmergencyID == id);
        }
    }
}
