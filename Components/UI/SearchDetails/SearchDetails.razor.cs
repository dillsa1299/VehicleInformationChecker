using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.UI.SearchDetails
{
    public partial class SearchDetails
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Parameter]
        public VehicleModel? Vehicle { get; set; }

        private readonly string _carPlaceholder = "images/placeholder-car.svg";
        private bool _isSearching;
        private bool _isAdditionalSearching;

        private Task OnSearchStarted(bool isAdditionalSearch)
        {
            _isSearching = !isAdditionalSearch;
            _isAdditionalSearching = isAdditionalSearch;

            return InvokeAsync(StateHasChanged);
        }

        private Task OnSearchCompleted(VehicleModel vehicle)
        {
            _isSearching = false;
            _isAdditionalSearching = false;
            return InvokeAsync(StateHasChanged);
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchStarted += OnSearchStarted;
            SearchRegistrationEventService.OnSearchCompleted += OnSearchCompleted;

            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchStarted -= OnSearchStarted;
            SearchRegistrationEventService.OnSearchCompleted -= OnSearchCompleted;
        }
    }
}
