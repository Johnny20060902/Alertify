using System.ComponentModel.DataAnnotations;

namespace Alertify.ViewModels.Citizen
{
    public class CancelEmergencyViewModel
    {
        [Required(ErrorMessage = "El ID de emergencia es requerido")]
        public int EmergencyID { get; set; }

        [MaxLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
        [Display(Name = "Motivo de cancelación")]
        public string? CancellationReason { get; set; }
    }
}