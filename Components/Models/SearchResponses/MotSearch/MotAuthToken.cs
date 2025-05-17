namespace VehicleInformationChecker.Components.Models.SearchResponses.MotSearch
{
    public class MotAuthToken
    {
        public string Type { get; set; } = string.Empty;
        public DateTime ExpireTime { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
