using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alertify.Models
{
    public class Emergency : BaseModel
    {
        [Key]
        public int EmergencyID { get; set; }

        [Required(ErrorMessage = "La categoría de emergencia es obligatoria")]
        [StringLength(50, ErrorMessage = "La categoría no debe exceder 50 caracteres")]
        [Display(Name = "Categoría de Emergencia")]
        public string EmergencyCategory { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La latitud es obligatoria")]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        [Column(TypeName = "decimal(10, 8)")]
        [Display(Name = "Latitud")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "La longitud es obligatoria")]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        [Column(TypeName = "decimal(11, 8)")]
        [Display(Name = "Longitud")]
        public decimal Longitude { get; set; }

        [StringLength(255, ErrorMessage = "La dirección no debe exceder 255 caracteres")]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "La referencia no debe exceder 500 caracteres")]
        [Display(Name = "Referencia de Ubicación")]
        public string? LocationReference { get; set; }

        [StringLength(500, ErrorMessage = "La URL de imagen no debe exceder 500 caracteres")]
        [Display(Name = "Imagen")]
        public string? ImageURL { get; set; }

        [Required(ErrorMessage = "El estado de emergencia es obligatorio")]
        [StringLength(50, ErrorMessage = "El estado no debe exceder 50 caracteres")]
        [Display(Name = "Estado de Emergencia")]
        public string EmergencyStatus { get; set; } = "Pendiente";

        [Required(ErrorMessage = "La prioridad es obligatoria")]
        [StringLength(20, ErrorMessage = "La prioridad no debe exceder 20 caracteres")]
        [Display(Name = "Prioridad")]
        public string Priority { get; set; } = "Media";

        [Display(Name = "Fecha de Asignación")]
        public DateTime? AssignmentDate { get; set; }

        [Display(Name = "Fecha de Resolución")]
        public DateTime? ResolutionDate { get; set; }

        [Required(ErrorMessage = "El ID del ciudadano es obligatorio")]
        [Display(Name = "Ciudadano")]
        public int CitizenID { get; set; }

        public User? Citizen { get; set; }
        public ICollection<EmergencyAssignment>? EmergencyAssignments { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<EmergencyStatusHistory>? StatusHistory { get; set; }
    }
}