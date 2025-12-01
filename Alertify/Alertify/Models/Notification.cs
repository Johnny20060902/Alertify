using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class Notification : BaseModel
    {
        [Key]
        public int NotificationID { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientPhone { get; set; }
        public int? EmergencyAssignmentID { get; set; }
        public string? NotificationType { get; set; }
        public string? SendChannel { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public DateTime SendDate { get; set; }
        public string? SendStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool IsRead { get; set; }

        public int? UserID { get; set; }
        public int? EmergencyID { get; set; }

        public User? User { get; set; }
        public Emergency? Emergency { get; set; }
        public EmergencyAssignment? EmergencyAssignment { get; set; }
    }
}