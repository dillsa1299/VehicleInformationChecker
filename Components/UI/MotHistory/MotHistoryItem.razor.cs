using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using static VehicleInformationChecker.Components.Models.MotDefectModel;

namespace VehicleInformationChecker.Components.UI.MotHistory;
public partial class MotHistoryItem
{
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
}
