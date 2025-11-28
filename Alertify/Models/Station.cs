using System.ComponentModel.DataAnnotations;

namespace Alertify.Models
{
    public class Station : BaseModel
    {
        [Key]
        public int StationID { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ServiceType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }


        public ICollection<Unit> Units { get; set; }
    }
}