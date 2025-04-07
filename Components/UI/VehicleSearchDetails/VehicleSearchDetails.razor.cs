using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.UI.VehicleSearchDetails
{
    public partial class VehicleSearchDetails
    {
        [Parameter]
        public VehicleModel? Vehicle { get; set; }
    }
}
