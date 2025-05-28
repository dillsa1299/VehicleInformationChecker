using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.UI.RegistrationInput
{
    public partial class RegistrationInput
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Parameter]
        public VehicleModel Vehicle { get; set; } = default!;

        RegistrationInputModel _registrationInput = new();
        private bool _searchFailed;

        private async Task SearchRegistrationInput()
        {
            if (!_registrationInput.Input.Equals(Vehicle.RegistrationNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                await SearchRegistrationEventService.NotifySearchVehicleAsync(_registrationInput.Input);
                StateHasChanged();
            }
        }

        private Task OnSearchCompleted(VehicleModel vehicle, SearchType searchType)
        {
            _searchFailed = String.IsNullOrEmpty(vehicle.RegistrationNumber);
            return InvokeAsync(StateHasChanged);
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchCompleted += OnSearchCompleted;

            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchCompleted -= OnSearchCompleted;
        }

        private class RegistrationInputModel
        {
            public string Input { get; set; } = string.Empty;
        }
    }
}
