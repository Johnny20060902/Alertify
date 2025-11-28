using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class Unit : BaseModel
    {
        [Key]
        public int UnitID { get; set; }
        public int StationID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ServiceType { get; set; }
        public string UnitStatus { get; set; }
        public string ResponsiblePerson { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        public Station Station { get; set; }
        public ICollection<EmergencyAssignment> EmergencyAssignments { get; set; }
    }
}