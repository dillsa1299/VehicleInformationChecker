using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Pages
{
    public partial class Main : IDisposable
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        private VehicleModel _vehicle = new VehicleModel();
        private bool _searching = false;

        private async Task SearchRegistration(string registration)
        {
            await Task.Delay(5000);
            await SearchRegistrationEventService.NotifySearchCompleted(registration);
        }

        private async Task OnSearchCompleted(string registration)
        {
            await Task.Delay(1);
            _vehicle = new VehicleModel
            {
                Registration = registration
            };
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
