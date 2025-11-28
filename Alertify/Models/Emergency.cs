using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class Emergency : BaseModel
    {
        [Key]
        public int EmergencyID { get; set; }
        public int CitizenID { get; set; }
        public string EmergencyCategory { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public string LocationReference { get; set; }
        public string ImageURL { get; set; }
        public string EmergencyStatus { get; set; }
        public string Priority { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? ResolutionDate { get; set; }

        public User Citizen { get; set; }
        public ICollection<EmergencyAssignment> EmergencyAssignments { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<EmergencyStatusHistory> StatusHistory { get; set; }
    }
}