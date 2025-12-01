using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Authentication
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email debe tener máximo {1} caracteres")]
        public string Email { get; set; } = string.Empty;
    }
}
