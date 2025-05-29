namespace VehicleInformationChecker.Components.Models
{
    public class MotDefectModel
    {
        public enum MotDefectType
        {
            Advisory,
            Dangerous,
            Fail,
            Major,
            Minor,
            NonSpecific,
            SystemGenerated,
            UserEntered
        }

        public string Description { get; set; } = string.Empty;
        public MotDefectType Type { get; set; }
        public bool Dangerous { get; set; }
    }
}
