using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Authentication
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer apellido debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        public string FirstLastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido debe tener máximo {1} caracteres")]
        public string? SecondLastName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email debe tener máximo {1} caracteres")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[\d\s\+\-\(\)]+$", ErrorMessage = "El formato del teléfono no es válido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, ErrorMessage = "La contraseña debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}