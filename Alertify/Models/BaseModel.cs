namespace Alertify.Models
{
    public class BaseModel
    {
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Status { get; set; }
    }
}
