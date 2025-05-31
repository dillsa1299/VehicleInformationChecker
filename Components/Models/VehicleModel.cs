using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;

namespace VehicleInformationChecker.Components.Models
{
    public class VehicleModel
    {
        public string RegistrationNumber { get; set; } = String.Empty;

        public int YearOfManufacture { get; set; }
        public string Make { get; set; } = String.Empty;
        public string Model { get; set; } = String.Empty;
        public string Colour { get; set; } = String.Empty;
        public string EngineCapacity { get; set; } = String.Empty;
        public string FuelType { get; set; } = String.Empty;

        public string TaxStatus { get; set; } = String.Empty;
        public DateOnly? TaxDueDate { get; set; }
        public string MotStatus { get; set; } = String.Empty;
        public DateOnly? MotExpiryDate { get; set; }
        public IEnumerable<MotModel> MotTests { get; set; } = [];

        public DateOnly DateOfLastV5CIssued { get; set; }
        public DateOnly MonthOfFirstRegistration { get; set; }

        public IEnumerable<ImageSearchItem> Images { get; set; } = [];
        public string AiOverview { get; set; } = String.Empty;
        public string AiCommonIssues { get; set; } = String.Empty;
        public string AiMotHistorySummary { get; set; } = String.Empty;
    }
}
