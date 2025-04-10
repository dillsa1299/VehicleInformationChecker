namespace VehicleInformationChecker.Components.Models
{
    public class VehicleModel
    {
        public string? RegistrationNumber { get; set; }
        public string? TaxStatus { get; set; }
        public string? TaxDueDate { get; set; }
        public string? MotStatus { get; set; }
        public string? Make { get; set; }
        public int YearOfManufacture { get; set; }
        public int EngineCapacity { get; set; }
        public int Co2Emissions { get; set; }
        public string? FuelType { get; set; }
        public bool MarkedForExport { get; set; }
        public string? Colour { get; set; }
        public string? TypeApproval { get; set; }
        public int RevenueWeight { get; set; }
        public string? DateOfLastV5CIssued { get; set; }
        public string? MotExpiryDate { get; set; }
        public string? Wheelplan { get; set; }
        public string? MonthOfFirstRegistration { get; set; }
    }
}
