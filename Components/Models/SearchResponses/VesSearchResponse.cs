namespace VehicleInformationChecker.Components.Models.SearchResponses
{
    public class VesSearchResponse
    {
        public string RegistrationNumber { get; set; } = String.Empty;
        public string TaxStatus { get; set; } = String.Empty;
        public string TaxDueDate { get; set; } = String.Empty;
        public string ArtEndDate { get; set; } = String.Empty;
        public string MotStatus { get; set; } = String.Empty;
        public string Make { get; set; } = String.Empty;
        public int YearOfManufacture { get; set; }
        public int EngineCapacity { get; set; }
        public int Co2Emissions { get; set; }
        public string FuelType { get; set; } = String.Empty;
        public bool MarkedForExport { get; set; }
        public string Colour { get; set; } = String.Empty;
        public string TypeApproval { get; set; } = String.Empty;
        public int RevenueWeight { get; set; }
        public string DateOfLastV5CIssued { get; set; } = String.Empty;
        public string MotExpiryDate { get; set; } = String.Empty;
        public string Wheelplan { get; set; } = String.Empty;
        public string MonthOfFirstRegistration { get; set; } = String.Empty;
        public string EuroStatus { get; set; } = String.Empty;
        public string RealDrivingEmissions { get; set; } = String.Empty;
    }
}
