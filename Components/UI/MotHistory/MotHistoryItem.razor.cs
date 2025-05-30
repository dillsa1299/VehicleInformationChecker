using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using VehicleInformationChecker.Components.Models;
using static VehicleInformationChecker.Components.Models.MotDefectModel;

namespace VehicleInformationChecker.Components.UI.MotHistory;
public partial class MotHistoryItem
{
    [Inject]
    private IJSRuntime? JsRuntime { get; set; }

    [Parameter]
    public MotModel? Mot { get; set; }

    private IEnumerable<MotDefectModel> _dangerousDefects =>
        Mot?.Defects.Where(d => d.Type == MotDefectType.Dangerous || d.Dangerous)
        ?? Enumerable.Empty<MotDefectModel>();

    private IEnumerable<MotDefectModel> _majorDefects =>
        Mot?.Defects.Where(d => d.Type == MotDefectType.Fail || d.Type == MotDefectType.Major)
        ?? Enumerable.Empty<MotDefectModel>();

    private IEnumerable<MotDefectModel> _otherDefects =>
        Mot?.Defects.Where(d => !(d.Type == MotDefectType.Dangerous || d.Dangerous || d.Type == MotDefectType.Fail || d.Type == MotDefectType.Major))
        ?? Enumerable.Empty<MotDefectModel>();

    private Size _iconSize = Size.Large;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && JsRuntime != null)
        {
            var size = await JsRuntime.InvokeAsync<int>("getWindowWidth");

            switch (size)
            {
                case < 600:
                    _iconSize = Size.Small;
                    break;
                case < 960:
                    _iconSize = Size.Medium;
                    break;
                default:
                    _iconSize = Size.Large;
                    break;
            }

            StateHasChanged();
        }
    }
}
