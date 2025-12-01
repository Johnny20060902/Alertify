using Alertify.Data;
using Alertify.Models;
using Alertify.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace Alertify.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AlertifyDbContext _context;

        public ProfileController(AlertifyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            var viewModel = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                FirstLastName = user.FirstLastName,
                SecondLastName = user.SecondLastName,
                Phone = user.Phone,
                NationalID = user.NationalID
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            user.FirstName = model.FirstName;
            user.FirstLastName = model.FirstLastName;
            user.SecondLastName = model.SecondLastName;
            user.Phone = model.Phone;
            user.NationalID = model.NationalID;

            user.ModifiedBy = userId;
            user.ModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Perfil actualizado exitosamente";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return Json(new { success = false, message = "Seleccione una imagen" });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
            if (!allowedTypes.Contains(photo.ContentType))
                return Json(new { success = false, message = "Solo se permiten imágenes JPG/PNG" });

            if (photo.Length > 2 * 1024 * 1024)
                return Json(new { success = false, message = "La imagen no debe superar 2MB" });

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return Json(new { success = false, message = "Usuario no encontrado" });

            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var uploadPath = Path.Combine("wwwroot", "uploads", "profiles");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            if (!string.IsNullOrEmpty(user.ProfilePhotoURL))
            {
                var oldPhotoPath = Path.Combine("wwwroot", user.ProfilePhotoURL.TrimStart('/'));
                if (System.IO.File.Exists(oldPhotoPath))
                    System.IO.File.Delete(oldPhotoPath);
            }

            // Guardar nueva foto
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            // Actualizar BD
            user.ProfilePhotoURL = $"/uploads/profiles/{fileName}";
            user.ModifiedBy = userId;
            user.ModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Foto actualizada exitosamente",
                newPhotoUrl = user.ProfilePhotoURL
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return Json(new { success = false, message = "Usuario no encontrado" });

            // Eliminar archivo físico
            if (!string.IsNullOrEmpty(user.ProfilePhotoURL))
            {
                var photoPath = Path.Combine("wwwroot", user.ProfilePhotoURL.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                    System.IO.File.Delete(photoPath);
            }

            // Actualizar BD
            user.ProfilePhotoURL = null;
            user.ModifiedBy = userId;
            user.ModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Foto eliminada exitosamente"
            });
        }
    }
}
