using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class EmergencyAssignment : BaseModel
    {
        [Key]
        public int EmergencyAssignmentID { get; set; }
        public int EmergencyID { get; set; }
        public int UnitID { get; set; }
        public int AssignedBy { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? CalculatedDistance { get; set; }
        public string AssignmentStatus { get; set; }
        public string Notes { get; set; }

        public Emergency Emergency { get; set; }
        public Unit Unit { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}