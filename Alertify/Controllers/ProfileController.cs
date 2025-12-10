using Alertify.Data;
using Alertify.Models;
using Alertify.Services.FileUpload;
using Alertify.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Alertify.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AlertifyDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public ProfileController(AlertifyDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
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

            if (model.Photo != null)
            {
                var uploadResult = await _fileUploadService.UploadImageAsync(model.Photo, "profiles", userId);

                if (!uploadResult.success)
                {
                    ViewBag.ErrorMessage = uploadResult.message;
                    return View(model);
                }

                if (!string.IsNullOrEmpty(user.ProfilePhotoURL))
                {
                    _fileUploadService.DeleteImage(user.ProfilePhotoURL);
                }

                user.ProfilePhotoURL = uploadResult.filePath;
            }

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
    }
}