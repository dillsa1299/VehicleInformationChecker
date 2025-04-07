using Microsoft.AspNetCore.Components;
using MudBlazor;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.UI.VehicleRegistrationInput
{
    public partial class VehicleRegistrationInput
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;
        
        private string _vehicleRegistration = string.Empty;
        private readonly IMask inputMask = new RegexMask(@"^[a-zA-Z0-9]{0,7}$");
        private bool _searching { get; set; }

        private async Task OnSearchClicked()
        {
            _searching = true;
            await SearchRegistrationEventService.NotifySearchRegistration(_vehicleRegistration);
            StateHasChanged();
        }

        private async Task OnSearchCompleted(string registration)
        {
            await Task.Delay(1);
            _searching = false;
            StateHasChanged();
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
    }
}
