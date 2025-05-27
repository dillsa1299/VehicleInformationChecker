using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Pages
{
    public partial class Main : IDisposable
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Inject]
        private ISearchRegistrationService SearchRegistrationService { get; set; } = default!;

        private VehicleModel _vehicle = new();

        private async Task SearchRegistration(string registration)
        {
            _vehicle = await SearchRegistrationService.SearchVehicleDetailsAsync(registration);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle);
            StateHasChanged();
            await AdditionalSearch(_vehicle);
        }

        private async Task AdditionalSearch(VehicleModel vehicle)
        {
            await SearchRegistrationEventService.NotifySearchStarted(true);
            _vehicle = await SearchRegistrationService.SearchVehicleAdditionalDetailsAsync(vehicle);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle);
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchRegistration += SearchRegistration;
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchRegistration -= SearchRegistration;
            GC.SuppressFinalize(this);
        }
    }
}
