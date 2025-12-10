using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Citizen
{
    public class CreateEmergencyViewModel
    {
        [Required(ErrorMessage = "La categoría de emergencia es obligatoria")]
        [RegularExpression(@"^(Policia|Bomberos|Medico)$",
            ErrorMessage = "La categoría debe ser 'Policia', 'Bomberos' o 'Medico'")]
        [Display(Name = "Tipo de Emergencia")]
        public string EmergencyCategory { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, MinimumLength = 10,
            ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La latitud es obligatoria")]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        [Display(Name = "Latitud")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "La longitud es obligatoria")]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        [Display(Name = "Longitud")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria")]
        [RegularExpression(@"^(Baja|Media|Alta|Critica)$",
            ErrorMessage = "La prioridad debe ser 'Baja', 'Media', 'Alta' o 'Critica'")]
        [Display(Name = "Prioridad")]
        public string Priority { get; set; } = null!;

        [StringLength(255, ErrorMessage = "La dirección no debe exceder 255 caracteres")]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "La referencia no debe exceder 500 caracteres")]
        [Display(Name = "Referencia de Ubicación")]
        public string? LocationReference { get; set; }

        [Display(Name = "Imagen (Opcional)")]
        public IFormFile? Image { get; set; }
    }
}