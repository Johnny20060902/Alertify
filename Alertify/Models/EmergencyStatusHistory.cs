using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class EmergencyStatusHistory
    {
        [Key]
        public int HistoryID { get; set; }

        [Required(ErrorMessage = "El ID de emergencia es obligatorio")]
        [Display(Name = "ID de Emergencia")]
        public int EmergencyID { get; set; }

        [StringLength(50, ErrorMessage = "El estado anterior no debe exceder 50 caracteres")]
        [RegularExpression(@"^(Pendiente|Asignada|EnCamino|EnSitio|Resuelta|Cancelada)$",
            ErrorMessage = "El estado anterior debe ser 'Pendiente', 'Asignada', 'EnCamino', 'EnSitio', 'Resuelta' o 'Cancelada'")]
        [Display(Name = "Estado Anterior")]
        public string? PreviousStatus { get; set; }

        [Required(ErrorMessage = "El estado nuevo es obligatorio")]
        [StringLength(50, ErrorMessage = "El estado nuevo no debe exceder 50 caracteres")]
        [RegularExpression(@"^(Pendiente|Asignada|EnCamino|EnSitio|Resuelta|Cancelada)$",
            ErrorMessage = "El estado nuevo debe ser 'Pendiente', 'Asignada', 'EnCamino', 'EnSitio', 'Resuelta' o 'Cancelada'")]
        [Display(Name = "Estado Nuevo")]
        public string NewStatus { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de cambio es obligatoria")]
        [Display(Name = "Fecha de Cambio")]
        public DateTime ChangeDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El ID del usuario que realizó el cambio es obligatorio")]
        [Display(Name = "Cambiado Por")]
        public int ChangedBy { get; set; }

        [StringLength(2000, ErrorMessage = "El comentario no debe exceder 2000 caracteres")]
        [Display(Name = "Comentario")]
        public string? Comment { get; set; }


        public Emergency? Emergency { get; set; }
    }
}