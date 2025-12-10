using Alertify.Data;
using Alertify.Models;
using Alertify.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alertify.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AlertifyDbContext _context;

        public AdminController(AlertifyDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // DASHBOARD
        // ============================================================
        public async Task<IActionResult> Dashboard()
        {
            var activeEmergencies = await _context.Emergencies
                .Where(e => e.EmergencyStatus != "Resuelta"
                         && e.EmergencyStatus != "Cancelada"
                         && e.Status == "Activo")
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            ViewBag.ActiveEmergencies = activeEmergencies;
            ViewBag.PendingCount = await _context.Emergencies.CountAsync(e => e.EmergencyStatus == "Pendiente");
            ViewBag.UnitsAvailable = await _context.Units.CountAsync(u => u.UnitStatus == "Disponible" && u.Status == "Activo");
            ViewBag.StationsAvailable = await _context.Stations.CountAsync(s => s.Status == "Activo");

            return View();
        }

        // ============================================================
        // LISTADO DE EMERGENCIAS
        // ============================================================
        public async Task<IActionResult> Emergencies()
        {
            var list = await _context.Emergencies
                .Include(e => e.Citizen)
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            return View("Emergencies/Index", list);
        }

        // ============================================================
        // DETALLE DE EMERGENCIA
        // ============================================================
        public async Task<IActionResult> Details(int id)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .Include(e => e.StatusHistory)
                .FirstOrDefaultAsync(e => e.EmergencyID == id);

            if (emergency == null)
                return NotFound();

            return View("Emergencies/Details", emergency);
        }

        // ============================================================
        // ASIGNAR UNIDAD (GET)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> AssignUnit(int id)
        {
            var emergency = await _context.Emergencies
                .FirstOrDefaultAsync(e => e.EmergencyID == id);

            if (emergency == null)
                return NotFound();

            // =======================
            // Estaciones (para el mapa)
            // =======================
            var stations = await _context.Stations
                .Include(s => s.Units)
                .ToListAsync();

            var stationDTOs = stations.Select(s => new StationDTO
            {
                StationID = s.StationID,
                Name = s.Name,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                UnitCount = s.Units.Count
            }).ToList();

            // =======================
            // Unidades disponibles
            // =======================
            var units = await _context.Units
                .Include(u => u.Station)
                .Where(u => u.UnitStatus == "Disponible")
                .ToListAsync();

            var unitDTOs = units.Select(u => new UnitDTO
            {
                UnitID = u.UnitID,
                UnitName = u.Name,
                StationName = u.Station?.Name ?? "Sin estación",
                Distance = Math.Sqrt(
                    Math.Pow((double)(u.Station?.Latitude ?? 0) - (double)emergency.Latitude, 2) +
                    Math.Pow((double)(u.Station?.Longitude ?? 0) - (double)emergency.Longitude, 2)
                )
            })
            .OrderBy(u => u.Distance)
            .ToList();

            // Modelo final para la vista
            var vm = new AdminAssignUnitVM
            {
                Emergency = emergency,
                Units = unitDTOs,
                Stations = stationDTOs
            };

            return View("Emergencies/AssignUnit", vm);
        }

        // ============================================================
        // ASIGNAR UNIDAD (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> AssignUnit(int emergencyId, int unitId)
        {
            var emergency = await _context.Emergencies.FindAsync(emergencyId);
            var unit = await _context.Units.FindAsync(unitId);

            if (emergency == null || unit == null)
                return NotFound();

            // Registrar asignación
            var assignment = new EmergencyAssignment
            {
                EmergencyID = emergencyId,
                UnitID = unitId,
                AssignedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                AssignmentDate = DateTime.Now,
                AssignmentStatus = "Asignada",
                Status = "Activo"
            };

            emergency.EmergencyStatus = "Asignada";
            unit.UnitStatus = "En misión";

            _context.EmergencyAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Unidad asignada correctamente.";
            return RedirectToAction("Emergencies");
        }

        // ============================================================
        // UPDATE STATUS (GET)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var emergency = await _context.Emergencies.FindAsync(id);
            if (emergency == null) return NotFound();

            ViewBag.StatusList = new List<string> {
                "Pendiente", "Asignada", "En Camino", "En Atención", "Resuelta", "Cancelada"
            };

            return View("Emergencies/UpdateStatus", emergency);
        }

        // ============================================================
        // UPDATE STATUS (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int emergencyId, string newStatus, string comment)
        {
            var emergency = await _context.Emergencies.FindAsync(emergencyId);
            if (emergency == null) return NotFound();

            string previousStatus = emergency.EmergencyStatus;

            emergency.EmergencyStatus = newStatus;

            _context.EmergencyStatusHistories.Add(new EmergencyStatusHistory
            {
                EmergencyID = emergencyId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                Comment = comment,
                ChangeDate = DateTime.Now,
                ChangedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Estado actualizado correctamente.";
            return RedirectToAction("Emergencies");
        }
    }
}
