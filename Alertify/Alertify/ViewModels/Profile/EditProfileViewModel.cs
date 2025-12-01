using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer apellido debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        [Display(Name = "Primer Apellido")]
        public string FirstLastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido debe tener máximo {1} caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string? SecondLastName { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[\d\s\+\-\(\)]+$", ErrorMessage = "El formato del teléfono no es válido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [StringLength(20, ErrorMessage = "El CI debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El CI solo puede contener números")]
        [Display(Name = "Cédula de Identidad")]
        public string? NationalID { get; set; }
    }
}
