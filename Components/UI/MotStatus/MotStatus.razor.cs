using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.UI.MotStatus
{
    public partial class MotStatus
    {
        [Parameter]
        public VehicleModel? Vehicle { get; set; }

        private string _style = string.Empty;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Set _style based on the MOT status
            switch (Vehicle?.MotStatus?.ToLowerInvariant())
            {
                case "valid":
                    _style = "background-color: var(--mud-palette-success);";
                    break;
                case "not valid":
                    _style = "background-color: var(--mud-palette-error);";
                    break;
                default:
                    _style = "background-color: var(--mud-palette-gray-default);";
                    break;
            }
        }
    }
}
