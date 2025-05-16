namespace VehicleInformationChecker.Components.Models.SearchResponses.MotSearch
{
    public class Defect
    {
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Dangerous { get; set; }
    }
}