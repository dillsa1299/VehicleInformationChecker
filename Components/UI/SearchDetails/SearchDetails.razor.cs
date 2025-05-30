using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.UI.SearchDetails
{
    public partial class SearchDetails
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Inject]
        private ISearchRegistrationService SearchRegistrationService { get; set; } = default!;


        [Parameter]
        public VehicleModel? Vehicle { get; set; }

        private readonly string _placeholderImage = "images/placeholder-car.svg";
        private bool _isSearchingDetails;
        private bool _isSearchingImages;
        private bool _isSearchingAiSummary;
        private bool _isSearchingAiCommonIssues;

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
                case SearchType.AiCommonIssues:
                    _isSearchingAiCommonIssues = true;
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
                case SearchType.AiCommonIssues:
                    _isSearchingAiCommonIssues = false;
                    break;
            }

            return InvokeAsync(StateHasChanged);
        }

        private async Task OnCommonIssuesExpandedAsync(bool expanded)
        {
            if (expanded && !_isSearchingAiCommonIssues && Vehicle != null && String.IsNullOrEmpty(Vehicle.AiCommonIssues))
            {
                await SearchRegistrationEventService.NotifySearchStarted(SearchType.AiCommonIssues);
                Vehicle = await SearchRegistrationService.SearchVehicleAsync(Vehicle, SearchType.AiCommonIssues);
                await SearchRegistrationEventService.NotifySearchCompleted(Vehicle, SearchType.AiCommonIssues);
            }
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
