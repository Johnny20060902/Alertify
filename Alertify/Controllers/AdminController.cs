using Alertify.Data;
using Alertify.Models;
using Alertify.Services.Email;
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
            ViewBag.UnitsAvailable = await _context.Units.CountAsync(u => (u.UnitStatus == "Disponible" || u.UnitStatus == "Available") && u.Status == "Activo");
            ViewBag.StationsAvailable = await _context.Stations.CountAsync(s => s.Status == "Activo");

            return View();
        }

        public async Task<IActionResult> Emergencies(string? category, string? status, string? priority, string? search)
        {
            var query = _context.Emergencies
                .Include(e => e.Citizen)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(e => e.EmergencyCategory == category);

            if (!string.IsNullOrEmpty(status))
            {
                string normalized = status switch
                {
                    "En Camino" => "En Camino",
                    "En Atención" => "En Atención",
                    _ => status
                };

                query = query.Where(e => e.EmergencyStatus == normalized);
            }

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(e => e.Priority == priority);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e =>
                    e.Description.Contains(search) ||
                    e.Address.Contains(search));

            var list = await query
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            return View("Emergencies/Index", list);
        }

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

        [HttpGet]
        public async Task<IActionResult> AssignUnit(int id)
        {
            var emergency = await _context.Emergencies
                .FirstOrDefaultAsync(e => e.EmergencyID == id);

            if (emergency == null)
                return NotFound();

            string Normalize(string value)
            {
                if (value == null) return "";
                return value.ToLower()
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u");
            }

            string emergencyType = Normalize(emergency.EmergencyCategory);


            var stations = await _context.Stations
                .Include(s => s.Units)
                .Where(s => s.Status == "Activo")
                .ToListAsync();

            var stationDTOs = stations.Select(s => new StationDTO
            {
                StationID = s.StationID,
                Name = s.Name ?? "",
                Latitude = (double)s.Latitude,     
                Longitude = (double)s.Longitude, 
                UnitCount = s.Units.Count,
                ServiceType = s.ServiceType ?? ""
            }).ToList();

            var units = await _context.Units
                .Include(u => u.Station)
                .Where(u =>
                    (u.UnitStatus == "Disponible" || u.UnitStatus == "Available")
                    && u.Status == "Activo"
                )
                .ToListAsync();

            units = units.Where(u =>
                Normalize(u.ServiceType) == emergencyType
            ).ToList();

            var unitDTOs = units.Select(u => new UnitDTO
            {
                UnitID = u.UnitID,
                UnitName = u.Name ?? "",
                Plate = u.Code ?? "",
                StationName = u.Station?.Name ?? "Sin estación",
                StationID = u.StationID,

                Distance = Math.Sqrt(
                    Math.Pow((double)u.Station!.Latitude - (double)emergency.Latitude, 2) +
                    Math.Pow((double)u.Station!.Longitude - (double)emergency.Longitude, 2)
                )
            })
            .OrderBy(u => u.Distance)
            .ToList();

            var vm = new AdminAssignUnitVM
            {
                Emergency = emergency,
                Stations = stationDTOs,
                Units = unitDTOs
            };

            return View("Emergencies/AssignUnit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUnit(
            int emergencyId,
            int unitId,
            [FromServices] UnitEmailService emailService)
        {

            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == emergencyId);

            var unit = await _context.Units
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UnitID == unitId);

            if (emergency == null || unit == null)
                return NotFound();

            var assignment = new EmergencyAssignment
            {
                EmergencyID = emergencyId,
                UnitID = unitId,
                AssignedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                AssignmentDate = DateTime.Now,
                AssignmentStatus = "Asignada",
                Status = "Activo"
            };

            _context.EmergencyAssignments.Add(assignment);

            emergency.EmergencyStatus = "Asignada";
            unit.UnitStatus = "En misión";

            await _context.SaveChangesAsync();

            bool emailSent = false;
            string? emailError = null;

            if (!string.IsNullOrWhiteSpace(unit.ContactEmail))
            {
                try
                {
                    await emailService.SendUnitAssignedEmail(
                        unit.ContactEmail!,
                        emergency,
                        unit
                    );
                    emailSent = true;
                }
                catch (Exception ex)
                {
                    emailSent = false;
                    emailError = ex.Message;
                }
            }

            var notificationUnit = new Notification
            {
                EmergencyID = emergencyId,
                EmergencyAssignmentID = assignment.EmergencyAssignmentID,
                UserID = null,
                RecipientEmail = unit.ContactEmail,
                NotificationType = "Asignación de Emergencia",
                SendChannel = "Email",
                Subject = $"Nueva emergencia asignada - {emergency.EmergencyCategory}",
                Message = $"Se asignó la emergencia '{emergency.Description}' a la unidad {unit.Name}.",
                SendDate = DateTime.Now,
                SendStatus = emailSent ? "Enviado" : "Fallido",
                ErrorMessage = emailError,
                IsRead = false
            };

            _context.Notifications.Add(notificationUnit);

            var citizenNotification = new Notification
            {
                EmergencyID = emergencyId,
                UserID = emergency.CitizenID,         
                RecipientEmail = emergency.Citizen?.Email,
                NotificationType = "Estado de Emergencia",
                SendChannel = "Sistema",
                Subject = "Tu emergencia ha sido asignada",
                Message = $"Tu emergencia '{emergency.Description}' ha sido asignada a la unidad {unit.Name}.",
                SendDate = DateTime.Now,
                SendStatus = "Enviado",
                IsRead = false
            };

            _context.Notifications.Add(citizenNotification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = emailSent
                ? "Unidad asignada y correo enviado correctamente."
                : "Unidad asignada, pero ocurrió un error al enviar el correo.";

            return RedirectToAction("Emergencies");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var emergency = await _context.Emergencies.FindAsync(id);
            if (emergency == null) return NotFound();

            ViewBag.StatusList = new List<string> {
                "Pendiente",
                "Asignada",
                "En Camino",
                "En Atención",
                "Resuelta",
                "Cancelada"
            };

            return View("Emergencies/UpdateStatus", emergency);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int emergencyId, string newStatus, string comment)
        {
            var emergency = await _context.Emergencies
                .Include(e => e.EmergencyAssignments)
                .FirstOrDefaultAsync(e => e.EmergencyID == emergencyId);

            if (emergency == null)
                return NotFound();

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

            var lastAssignment = emergency.EmergencyAssignments?
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            if (lastAssignment != null)
            {
                var unit = await _context.Units
                    .FirstOrDefaultAsync(u => u.UnitID == lastAssignment.UnitID);

                if (unit != null)
                {
                    if (newStatus == "Resuelta" || newStatus == "Cancelada")
                        unit.UnitStatus = "Disponible";

                    if (previousStatus == "Asignada" && newStatus == "Pendiente")
                        unit.UnitStatus = "Disponible";
                }
            }

            var notify = new Notification
            {
                EmergencyID = emergencyId,
                UserID = emergency.CitizenID,
                NotificationType = "Cambio de Estado",
                SendChannel = "Sistema",
                Subject = "Actualización de estado de tu emergencia",
                Message = $"El estado de tu emergencia '{emergency.Description}' cambió a '{newStatus}'.",
                SendDate = DateTime.Now,
                SendStatus = "Enviado",
                IsRead = false
            };

            _context.Notifications.Add(notify);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Estado actualizado correctamente.";
            return RedirectToAction("Emergencies");
        }



    }
}
