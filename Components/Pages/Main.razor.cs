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
            // Details Search
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.Details);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(new VehicleModel { RegistrationNumber = registration }, SearchType.Details);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.Details);
            StateHasChanged();

            // Don't waste further API calls on failed search
            if (String.IsNullOrEmpty(_vehicle.RegistrationNumber)) return;

            // TODO - Do Image and AI at same time.

            // Image Search
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.Images);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(_vehicle, SearchType.Images);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.Images);
            StateHasChanged();

            // AI Summary search
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.AiSummary);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(_vehicle, SearchType.AiSummary);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.AiSummary);
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchVehicle += SearchRegistration;
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchVehicle -= SearchRegistration;
            GC.SuppressFinalize(this);
        }
    }
}
