using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Authentication
{
    public class LoginCredentials
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email debe tener máximo {1} caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, ErrorMessage = "La contraseña debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}
