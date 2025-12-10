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
    public class StationsController : Controller
    {
        private readonly AlertifyDbContext _context;

        public StationsController(AlertifyDbContext context)
        {
            _context = context;
        }

        // GET: Stations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stations.ToListAsync());
        }

        // GET: Stations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .FirstOrDefaultAsync(m => m.StationID == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // GET: Stations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string latitude, string longitude, string serviceType, string address, string phone, string status)
        {
            var station = new Station
            {
                Name = name,
                ServiceType = serviceType,
                Address = address,
                Phone = phone,
                Status = status ?? "Activo"
            };

            // Parsear Latitude y Longitude como strings, reemplazando comas con puntos
            if (!string.IsNullOrWhiteSpace(latitude))
            {
                latitude = latitude.Replace(",", ".");
                if (decimal.TryParse(latitude, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out decimal lat))
                {
                    station.Latitude = lat;
                }
            }

            if (!string.IsNullOrWhiteSpace(longitude))
            {
                longitude = longitude.Replace(",", ".");
                if (decimal.TryParse(longitude, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out decimal lng))
                {
                    station.Longitude = lng;
                }
            }

            // Asignar automáticamente campos de auditoría
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = userId != null && int.TryParse(userId, out int uid) ? uid : 1;

            station.CreatedBy = parsedUserId;
            station.CreationDate = DateTime.Now;
            station.ModifiedBy = parsedUserId;
            station.ModificationDate = DateTime.Now;
            station.Status = "Activo";

            _context.Add(station);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Stations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return View(station);
        }

        // POST: Stations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, string latitude, string longitude, string serviceType, string address, string phone, string status)
        {
            // Obtener la estación existente
            var existingStation = await _context.Stations.FindAsync(id);
            if (existingStation == null)
            {
                return NotFound();
            }

            // Actualizar campos editables
            existingStation.Name = name;
            existingStation.ServiceType = serviceType;
            existingStation.Address = address;
            existingStation.Phone = phone;
            existingStation.Status = status;

            // Parsear Latitude y Longitude como strings, reemplazando comas con puntos
            if (!string.IsNullOrWhiteSpace(latitude))
            {
                latitude = latitude.Replace(",", ".");
                if (decimal.TryParse(latitude, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out decimal lat))
                {
                    existingStation.Latitude = lat;
                }
            }
            else
            {
                existingStation.Latitude = 00;
            }

            if (!string.IsNullOrWhiteSpace(longitude))
            {
                longitude = longitude.Replace(",", ".");
                if (decimal.TryParse(longitude, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out decimal lng))
                {
                    existingStation.Longitude = lng;
                }
            }
            else
            {
                existingStation.Longitude = 00;
            }

            // Asignar campos de auditoría de modificación
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int parsedUserId = userId != null && int.TryParse(userId, out int uid) ? uid : 1;
            existingStation.ModifiedBy = parsedUserId;
            existingStation.ModificationDate = DateTime.Now;

            try
            {
                _context.Update(existingStation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return View();
        }

        // GET: Stations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .FirstOrDefaultAsync(m => m.StationID == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station != null)
            {
                _context.Stations.Remove(station);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StationExists(int id)
        {
            return _context.Stations.Any(e => e.StationID == id);
        }
    }
}
