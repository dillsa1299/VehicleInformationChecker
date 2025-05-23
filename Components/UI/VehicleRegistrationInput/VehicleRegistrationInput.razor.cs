using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.UI.VehicleRegistrationInput
{
    public partial class VehicleRegistrationInput
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Parameter]
        public VehicleModel Vehicle { get; set; } = default!;

        RegistrationInputModel _registrationInput = new();
        private readonly IMask _inputMask = new RegexMask(@"^[a-zA-Z0-9]{0,7}$");
        private bool _searchFailed;

        private async Task SearchRegistrationInput()
        {
            await SearchRegistrationEventService.NotifySearchRegistrationAsync(_registrationInput.Input);
            StateHasChanged();
        }

        private Task OnSearchCompleted(VehicleModel vehicle)
        {
            _searchFailed = vehicle.RegistrationNumber == string.Empty;
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
