using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class BaseModel
    {
        [Required(ErrorMessage = "El usuario creador es obligatorio")]
        public int CreatedBy { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public DateTime? ModificationDate { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(20, ErrorMessage = "El estado debe tener máximo {1} caracteres")]
        [RegularExpression(@"^(Activo|Inactivo)$", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'")]
        public string Status { get; set; } = "Activo";
    }
}