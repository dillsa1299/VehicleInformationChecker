namespace VehicleInformationChecker.Components.Models
{
    public class MotModel
    {
        public DateOnly CompletedDate { get; set; }
        public bool Passed { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public long OdometerValue { get; set; }
        public string OdometerUnit { get; set; } = string.Empty;
        public IEnumerable<MotDefectModel> Defects { get; set; } = [];
    }
}
