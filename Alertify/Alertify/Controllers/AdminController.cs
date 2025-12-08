using Alertify.Data;
using Alertify.Models;
using Alertify.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alertify.Controllers
{
    public class AdminController : Controller
    {
        private readonly AlertifyDbContext _context;
        private readonly EmailService _emailService;

        public AdminController(AlertifyDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            var pendingCount = await _context.Emergencies
                .Where(e => e.EmergencyStatus == "Pendiente")
                .CountAsync();

            var unitsAvailable = await _context.Units
                .Where(u => u.UnitStatus == "Disponible" && u.Status == "Activo")
                .CountAsync();

            var stationsAvailable = await _context.Stations
                .Where(s => s.Status == "Activo")
                .CountAsync();

            ViewBag.ActiveEmergencies = activeEmergencies;
            ViewBag.PendingCount = pendingCount;
            ViewBag.UnitsAvailable = unitsAvailable;
            ViewBag.StationsAvailable = stationsAvailable;

            return View(); // ← Views/Admin/Dashboard.cshtml
        }

        // ============================================================
        // LISTADO DE EMERGENCIAS (Index)
        // ============================================================
        public async Task<IActionResult> Emergencies()
        {
            var list = await _context.Emergencies
                .Include(e => e.Citizen)
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            return View("Emergencies/Index", list);
            // ← carga Views/Admin/Emergencies/Index.cshtml
        }

        // ============================================================
        // ASIGNAR UNIDAD — GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> AssignUnit(int id)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == id);

            if (emergency == null)
                return NotFound();

            var stations = await _context.Stations
                .Include(s => s.Units)
                .ToListAsync();

            var units = await _context.Units
                .Include(u => u.Station)
                .Where(u => u.UnitStatus == "Disponible")
                .ToListAsync();

            foreach (var unit in units)
            {
                if (unit.Station == null)
                {
                    unit.CalculatedProximity = double.MaxValue;
                    continue;
                }

                double lat1 = (double)emergency.Latitude;
                double lon1 = (double)emergency.Longitude;

                double lat2 = (double)unit.Station.Latitude;
                double lon2 = (double)unit.Station.Longitude;

                unit.CalculatedProximity =
                    Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lon1 - lon2, 2));
            }

            ViewBag.Stations = stations;
            ViewBag.Units = units.OrderBy(u => u.CalculatedProximity).ToList();

            return View(emergency);
            // ← carga Views/Admin/Emergencies/AssignUnit.cshtml
        }

        // ============================================================
        // ASIGNAR UNIDAD — POST
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> AssignUnit(int emergencyId, int unitId)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == emergencyId);

            var unit = await _context.Units.FindAsync(unitId);

            if (emergency == null || unit == null)
                return NotFound();

            var assignment = new EmergencyAssignment
            {
                EmergencyID = emergencyId,
                UnitID = unitId,
                AssignedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                AssignmentDate = DateTime.Now,
                AssignmentStatus = "Asignado"
            };

            emergency.EmergencyStatus = "Asignado";
            unit.UnitStatus = "En Servicio";

            _context.EmergencyAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            await SendNotificationAsync(emergency, unit);

            return RedirectToAction("Emergencies");
        }

        // ============================================================
        // UPDATE STATUS — GET
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == id);

            if (emergency == null)
                return NotFound();

            ViewBag.StatusList = new List<string>
            {
                "Pendiente",
                "Asignado",
                "En Camino",
                "En Atención",
                "Resuelto",
                "Cancelado"
            };

            return View(emergency);
            // ← Views/Admin/Emergencies/UpdateStatus.cshtml
        }

        // ============================================================
        // UPDATE STATUS — POST
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int emergencyId, string newStatus, string comment)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == emergencyId);

            if (emergency == null)
                return NotFound();

            string previous = emergency.EmergencyStatus ?? "Pendiente";
            emergency.EmergencyStatus = newStatus;

            _context.EmergencyStatusHistories.Add(new EmergencyStatusHistory
            {
                EmergencyID = emergencyId,
                PreviousStatus = previous,
                NewStatus = newStatus,
                Comment = comment,
                ChangeDate = DateTime.Now,
                ChangedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
            });

            _context.Notifications.Add(new Notification
            {
                UserID = emergency.CitizenID,
                EmergencyID = emergencyId,
                Subject = "Estado actualizado",
                Message = $"Tu emergencia ha cambiado a: {newStatus}",
                SendDate = DateTime.Now,
                SendStatus = "OK"
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Emergencies");
        }

        // ============================================================
        // SEND NOTIFICATION
        // ============================================================
        private async Task SendNotificationAsync(Emergency emergency, Unit unit)
        {
            var notification = new Notification
            {
                UserID = emergency.CitizenID,
                EmergencyID = emergency.EmergencyID,
                RecipientEmail = emergency.Citizen.Email,
                RecipientPhone = emergency.Citizen.Phone,
                NotificationType = "Asignación",
                SendChannel = "Email + App",
                Subject = "Unidad Asignada",
                Message = $"La unidad {unit.Name} fue asignada a tu emergencia.",
                SendDate = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                emergency.Citizen.Email,
                "Alertify - Unidad Asignada",
                $"La unidad <b>{unit.Name}</b> está en camino a tu ubicación.");
        }
    }
}
