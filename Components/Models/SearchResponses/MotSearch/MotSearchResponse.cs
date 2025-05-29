namespace VehicleInformationChecker.Components.Models.SearchResponses.MotSearch
{
    public class MotSearchResponse
    {
        public string Registration { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string FirstUsedDate { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public string PrimaryColour { get; set; } = string.Empty;
        public string RegistrationDate { get; set; } = string.Empty;
        public string ManufactureDate { get; set; } = string.Empty;
        public string HasOutstandingRecall { get; set; } = string.Empty;
        public IEnumerable<MotTestResponse> MotTests { get; set; } = [];
    }
}
