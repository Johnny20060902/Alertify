using Alertify.Data;
using Alertify.Models;
using Alertify.Services.Email;
using Alertify.ViewModels.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Alertify.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AlertifyDbContext _context;
        private readonly EmailService _emailService;
        private readonly EmailTemplateService _emailTemplateService;

        public AuthenticationController(
            AlertifyDbContext context,
            EmailService emailService,
            EmailTemplateService emailTemplateService)
        {
            _context = context;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }

        #region LOGIN/LOGOUT
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginCredentials credentials)
        {
            if (!ModelState.IsValid)
                return View(credentials);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == credentials.Email
                                                                   && u.Status == "Activo");

            if (user == null || !BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password))
            {
                ViewBag.ErrorMessage = "Credenciales inválidas";
                return View(credentials);
            }

            user.LastAccess = DateTime.Now;
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.FirstLastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = credentials.RememberMe,
                ExpiresUtc = credentials.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            if (user.MustChangePassword)
                return RedirectToAction("ChangePassword");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);

                if (user != null)
                {
                    user.LastAccess = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Login", "Authentication");
        }

        #endregion

        #region REGISTER

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerUser)
        {
            if (!ModelState.IsValid)
                return View(registerUser);

            bool isEmailValid = !_context.Users.Any(u => u.Email == registerUser.Email);
            if (!isEmailValid)
            {
                ViewBag.ErrorMessage = "Email ya esta registrado";
                return View(registerUser);
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);

            var newUser = new User
            {
                Email = registerUser.Email,
                Password = hashPassword,
                FirstName = registerUser.FirstName,
                FirstLastName = registerUser.FirstLastName,
                SecondLastName = registerUser.SecondLastName,
                Phone = registerUser.Phone,
                Role = "Ciudadano",
                CreatedBy = 1,
                Status = "Activo"
            };

            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registro exitoso. Por favor inicia sesión.";
                return RedirectToAction("Login", "Authentication");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"{ex.Message}";
                return View(registerUser);
            }
        }
        #endregion

        #region FORGOT PASSWORD
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (!ModelState.IsValid)
                return View(forgotPassword);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPassword.Email && u.Status == "Activo");

            if (user != null)
            {
                string temporaryPassword = GenerateTemporaryPassword();
                user.Password = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
                user.MustChangePassword = true;
                user.ModifiedBy = user.UserID;
                user.ModificationDate = DateTime.Now;
                await _context.SaveChangesAsync();

                string template = _emailTemplateService.GetForgotPasswordTemplate(user.FirstName, temporaryPassword);
                await _emailService.SendEmailAsync(user.Email, "Alertify - Recuperación de Contraseña", template);
            }

            TempData["SuccessMessage"] = "Se ha enviado una contraseña temporal a tu correo";
            return RedirectToAction("Login", "Authentication");
        }

        private string GenerateTemporaryPassword()
        {
            const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijkmnopqrstuvwxyz";
            const string numbers = "23456789";
            const string allChars = upperCase + lowerCase + numbers;

            var random = new Random();
            var password = new char[8];

            password[0] = upperCase[random.Next(upperCase.Length)];
            password[1] = lowerCase[random.Next(lowerCase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];

            for (int i = 3; i < password.Length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
        #endregion

        #region CHANGE PASSWORD
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return RedirectToAction("Login", "Authentication");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            ViewBag.MustChangePassword = user.MustChangePassword;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (!ModelState.IsValid)
                return View(changePassword);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, user.Password))
            {
                ViewBag.ErrorMessage = "La contraseña actual es incorrecta";
                return View(changePassword);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
            user.MustChangePassword = false;
            user.ModifiedBy = userId;
            user.ModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña actualizada exitosamente";
            return RedirectToAction("Index", "Home");
        }
        #endregion

    } 
}
