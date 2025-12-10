using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alertify.Models
{
    [Table("User")]
    public class User : BaseModel
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email debe tener máximo {1} caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, ErrorMessage = "La contraseña debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer apellido debe tener al menos {2} caracteres y máximo {1}", MinimumLength = 3)]
        public string FirstLastName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido debe tener máximo {1} caracteres")]
        public string? SecondLastName { get; set; }

        [StringLength(20, ErrorMessage = "El CI debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El CI solo puede contener números")]
        public string? NationalID { get; set; } // CI

        [StringLength(20, ErrorMessage = "El teléfono debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[\d\s\+\-\(\)]+$", ErrorMessage = "El formato del teléfono no es válido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El rol debe tener máximo {1} caracteres")]
        [RegularExpression(@"^(Admin|Ciudadano)$", ErrorMessage = "El rol debe ser 'Admin' o 'Ciudadano'")]
        public string Role { get; set; } = "Ciudadano";

        [Required]
        [Display(Name = "Debe Cambiar Contraseña")]
        public bool MustChangePassword { get; set; } = false;

        public DateTime? LastAccess { get; set; }

        [StringLength(500, ErrorMessage = "La URL de la foto debe tener máximo {1} caracteres")]
        [DataType(DataType.Url)]
        public string? ProfilePhotoURL { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Emergency>? Emergencies { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
    }
}