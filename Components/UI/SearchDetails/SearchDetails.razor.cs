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

        private readonly string _placeholderImage = "images/placeholder-car.svg";
        private bool _isSearchingDetails;
        private bool _isSearchingImages;
        private bool _isSearchingAiSummary;

        private Task OnSearchStarted(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Details:
                    _isSearchingDetails = true;
                    break;
                case SearchType.Images:
                    _isSearchingImages = true;
                    break;
                case SearchType.AiSummary:
                    _isSearchingAiSummary = true;
                    break;
            }

            return InvokeAsync(StateHasChanged);
        }

        private Task OnSearchCompleted(VehicleModel vehicle, SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Details:
                    _isSearchingDetails = false;
                    break;
                case SearchType.Images:
                    _isSearchingImages = false;
                    break;
                case SearchType.AiSummary:
                    _isSearchingAiSummary = false;
                    break;
            }

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
