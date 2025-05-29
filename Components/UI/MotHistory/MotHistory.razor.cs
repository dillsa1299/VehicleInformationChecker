using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.UI.MotHistory
{
    public partial class MotHistory
    {
        [Parameter]
        public VehicleModel? Vehicle { get; set; }
    }
}
