using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class Unit : BaseModel
    {
        [Key]
        public int UnitID { get; set; }

        [Required(ErrorMessage = "El código de la unidad es obligatorio")]
        [StringLength(20, ErrorMessage = "El código debe tener máximo {1} caracteres")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "El código solo puede contener letras mayúsculas y números")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "El nombre de la unidad es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener máximo {1} caracteres")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
        public string? ServiceType { get; set; }

        [Required(ErrorMessage = "El estado de la unidad es obligatorio")]
        [RegularExpression(@"^(Disponible|EnServicio|Mantenimiento)$", 
            ErrorMessage = "El estado debe ser 'Disponible', 'EnServicio' o 'Mantenimiento'")]
        public string? UnitStatus { get; set; } = "Disponible";

        [StringLength(150, ErrorMessage = "El nombre del responsable debe tener máximo {1} caracteres")]
        public string? ResponsiblePerson { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo debe tener máximo {1} caracteres")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "El teléfono no es válido")]
        [StringLength(20, ErrorMessage = "El teléfono debe tener máximo {1} caracteres")]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "La estación es obligatoria")]
        public int StationID { get; set; }
        public Station? Station { get; set; }
        public ICollection<EmergencyAssignment>? EmergencyAssignments { get; set; }
    }
}