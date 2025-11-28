using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class EmergencyStatusHistory
    {
        [Key]
        public int HistoryID { get; set; }
        public int EmergencyID { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public int ChangedBy { get; set; }
        public string Comment { get; set; }

        public Emergency Emergency { get; set; }
    }
}