using Alertify.Data;
using Alertify.Models;
using Alertify.Services.FileUpload;
using Alertify.ViewModels.Citizen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alertify.Controllers
{
    [Authorize(Roles = "Ciudadano")]
    public class CitizenController : Controller
    {
        private readonly AlertifyDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public CitizenController(AlertifyDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var activeEmergencies = await _context.Emergencies
                .Where(e => e.CitizenID == userId
                    && e.EmergencyStatus != "Resuelta"
                    && e.EmergencyStatus != "Cancelada"
                    && e.Status == "Activo")
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserID == userId
                    && n.IsRead == false
                    && n.SendChannel == "InApp"
                    && n.Status == "Activo")
                .OrderByDescending(n => n.SendDate)
                .Take(5)
                .ToListAsync();

            ViewBag.ActiveEmergencies = activeEmergencies;
            ViewBag.UnreadNotifications = unreadNotifications;
            ViewBag.UnreadCount = unreadNotifications.Count;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateEmergency()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var hasActiveEmergency = await _context.Emergencies
                .AnyAsync(e => e.CitizenID == userId
                    && e.EmergencyStatus != "Resuelta"
                    && e.EmergencyStatus != "Cancelada"
                    && e.Status == "Activo");

            if (hasActiveEmergency)
            {
                TempData["ErrorMessage"] = "Ya tienes una emergencia activa. Espera a que sea resuelta antes de reportar una nueva.";
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmergency(CreateEmergencyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            string? imageUrl = null;

            if (model.Image != null)
            {
                var uploadResult = await _fileUploadService.UploadImageAsync(model.Image, "emergencies");

                if (!uploadResult.success)
                {
                    ViewBag.ErrorMessage = uploadResult.message;
                    return View(model);
                }

                imageUrl = uploadResult.filePath;
            }

            var emergency = new Emergency
            {
                CitizenID = userId,
                EmergencyCategory = model.EmergencyCategory,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Address = model.Address,
                LocationReference = model.LocationReference,
                ImageURL = imageUrl,
                EmergencyStatus = "Pendiente",
                Priority = model.Priority,
                CreatedBy = userId,
                Status = "Activo"
            };

            try
            {
                _context.Emergencies.Add(emergency);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Emergencia reportada exitosamente. Pronto recibirás ayuda.";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al reportar emergencia: {ex.Message}";
                return View(model);
            }
        }

        public async Task<IActionResult> MyEmergencies()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var emergencies = await _context.Emergencies
                .Where(e => e.CitizenID == userId && e.Status == "Activo")
                .OrderByDescending(e => e.CreationDate)
                .ToListAsync();

            return View(emergencies);
        }

        public async Task<IActionResult> EmergencyDetail(int id)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var emergency = await _context.Emergencies
                .Include(e => e.Citizen)
                .FirstOrDefaultAsync(e => e.EmergencyID == id && e.CitizenID == userId && e.Status == "Activo");

            if (emergency == null)
            {
                TempData["ErrorMessage"] = "Emergencia no encontrada o no tienes permiso para verla.";
                return RedirectToAction("MyEmergencies");
            }

            var assignments = await _context.EmergencyAssignments
                .Include(a => a.Unit)
                    .ThenInclude(u => u.Station)
                .Where(a => a.EmergencyID == id && a.Status == "Activo")
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();

            var statusHistory = await _context.EmergencyStatusHistories
                .Where(h => h.EmergencyID == id)
                .OrderByDescending(h => h.ChangeDate)
                .ToListAsync();

            ViewBag.Assignments = assignments;
            ViewBag.StatusHistory = statusHistory;

            return View(emergency);
        }

        [HttpPost]
        public async Task<IActionResult> CancelEmergency(int id, string? cancellationReason)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var emergency = await _context.Emergencies
                .FirstOrDefaultAsync(e => e.EmergencyID == id
                    && e.CitizenID == userId
                    && e.Status == "Activo");

            if (emergency == null)
            {
                TempData["ErrorMessage"] = "Emergencia no encontrada o no tienes permiso para cancelarla.";
                return RedirectToAction("MyEmergencies");
            }

            if (emergency.EmergencyStatus != "Pendiente" && emergency.EmergencyStatus != "Asignada")
            {
                TempData["ErrorMessage"] = "No puedes cancelar una emergencia que ya está en curso o finalizada.";
                return RedirectToAction("EmergencyDetail", new { id });
            }

            try
            {
                var previousStatus = emergency.EmergencyStatus;
                emergency.EmergencyStatus = "Cancelada";
                emergency.ModifiedBy = userId;
                emergency.ModificationDate = DateTime.Now;

                if (previousStatus == "Asignada")
                {
                    var assignments = await _context.EmergencyAssignments
                        .Include(a => a.Unit)
                        .Where(a => a.EmergencyID == id && a.Status == "Activo")
                        .ToListAsync();

                    foreach (var assignment in assignments)
                    {
                        assignment.AssignmentStatus = "Cancelada";
                        assignment.ModifiedBy = userId;
                        assignment.ModificationDate = DateTime.Now;

                        var hasOtherActiveEmergencies = await _context.EmergencyAssignments
                            .AnyAsync(a => a.UnitID == assignment.UnitID
                                && a.EmergencyAssignmentID != assignment.EmergencyAssignmentID
                                && a.AssignmentStatus != "Completada"
                                && a.AssignmentStatus != "Cancelada"
                                && a.Status == "Activo");

                        if (!hasOtherActiveEmergencies)
                        {
                            assignment.Unit.UnitStatus = "Disponible";
                            assignment.Unit.ModifiedBy = userId;
                            assignment.Unit.ModificationDate = DateTime.Now;
                        }
                    }
                }

                var statusHistory = new EmergencyStatusHistory
                {
                    EmergencyID = id,
                    PreviousStatus = previousStatus,
                    NewStatus = "Cancelada",
                    ChangedBy = userId,
                    Comment = !string.IsNullOrEmpty(cancellationReason)
                        ? $"Cancelada por ciudadano. Motivo: {cancellationReason}"
                        : "Cancelada por ciudadano"
                };
                _context.EmergencyStatusHistories.Add(statusHistory);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Emergencia cancelada exitosamente.";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cancelar emergencia: {ex.Message}";
                return RedirectToAction("EmergencyDetail", new { id });
            }
        }
    }
}