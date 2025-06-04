using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;

namespace VehicleInformationChecker.Components.UI.ImageViewer;
public partial class ImageViewer
{
    [Parameter]
    public bool IsSearching { get; set; }

    [Parameter]
    public IEnumerable<ImageSearchItem> Images { get; set; } = [];

    [Parameter]
    public string? PlaceholderImage { get; set; } = string.Empty;
}
