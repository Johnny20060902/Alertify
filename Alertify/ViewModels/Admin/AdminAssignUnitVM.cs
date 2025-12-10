using Alertify.Models;

namespace Alertify.ViewModels
{
    public class AdminAssignUnitVM
    {
        public Emergency Emergency { get; set; }
        public List<UnitDTO> Units { get; set; }
        public List<StationDTO> Stations { get; set; }
    }

    public class UnitDTO
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; } = "";
        public string Plate { get; set; } = "";
        public string StationName { get; set; } = "";
        public double Distance { get; set; }
    }

    public class StationDTO
    {
        public int StationID { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int UnitCount { get; set; }
    }
}
