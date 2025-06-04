namespace VehicleInformationChecker.Components.Models.SearchResponses
{
    public class VesSearchResponse
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public string TaxStatus { get; set; } = string.Empty;
        public string TaxDueDate { get; set; } = string.Empty;
        public string ArtEndDate { get; set; } = string.Empty;
        public string MotStatus { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public int EngineCapacity { get; set; }
        public int Co2Emissions { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public bool MarkedForExport { get; set; }
        public string Colour { get; set; } = string.Empty;
        public string TypeApproval { get; set; } = string.Empty;
        public int RevenueWeight { get; set; }
        public string DateOfLastV5CIssued { get; set; } = string.Empty;
        public string MotExpiryDate { get; set; } = string.Empty;
        public string Wheelplan { get; set; } = string.Empty;
        public string MonthOfFirstRegistration { get; set; } = string.Empty;
        public string EuroStatus { get; set; } = string.Empty;
        public string RealDrivingEmissions { get; set; } = string.Empty;
    }
}
