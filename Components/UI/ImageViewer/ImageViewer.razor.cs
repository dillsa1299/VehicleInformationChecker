using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;

namespace VehicleInformationChecker.Components.UI.ImageViewer;
public partial class ImageViewer
{
    [Parameter]
    public bool IsSearching { get; set; }

    [Parameter]
    public IEnumerable<ImageSearchItem> Images { get; set; } = [];

    [Parameter]
    public string? PlaceholderImage { get; set; } = String.Empty;
}
