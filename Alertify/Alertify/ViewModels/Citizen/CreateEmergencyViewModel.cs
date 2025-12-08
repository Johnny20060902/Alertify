using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Citizen
{
    public class CreateEmergencyViewModel
    {
        [Required(ErrorMessage = "La categoría de emergencia es obligatoria")]
        [Display(Name = "Tipo de Emergencia")]
        public string EmergencyCategory { get; set; } = null!;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no debe exceder 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La latitud es obligatoria")]
        [Display(Name = "Latitud")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "La longitud es obligatoria")]
        [Display(Name = "Longitud")]
        public decimal Longitude { get; set; }

        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [StringLength(500)]
        [Display(Name = "Referencia de Ubicación")]
        public string? LocationReference { get; set; }

        [Display(Name = "Imagen (Opcional)")]
        public IFormFile? Image { get; set; }
    }
}