using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;
using VehicleInformationChecker.Components.Services.SearchRegistrationService;

namespace VehicleInformationChecker.Components.Pages
{
    public partial class Main : IDisposable
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Inject]
        private ISearchRegistrationService SearchRegistrationService { get; set; } = default!;

        private VehicleModel _vehicle = new VehicleModel();

        private async Task SearchRegistration(string registration)
        {
            var vehicle = await SearchRegistrationService.SearchRegistrationAsync(registration);
            SearchRegistrationEventService.NotifySearchCompleted(vehicle);
        }

        private void OnSearchCompleted(VehicleModel vehicle)
        {
            _vehicle = vehicle;
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchRegistration += SearchRegistration;
            SearchRegistrationEventService.OnSearchCompleted += OnSearchCompleted;

            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchRegistration -= SearchRegistration;
            SearchRegistrationEventService.OnSearchCompleted -= OnSearchCompleted;
        }
    }
}
