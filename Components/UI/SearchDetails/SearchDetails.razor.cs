using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        private readonly string _aiFailedMessage = "Unable to generate AI response. Please try again.";
        private bool _isSearchingDetails;
        private bool _isSearchingImages;
        private bool _isSearchingAiOverview;
        private bool _isSearchingAiCommonIssues;
        private bool _isSearchingAiMotHistorySummary;

        private MarkupString? AiOverviewHtml =>
            string.IsNullOrWhiteSpace(Vehicle?.AiOverview)
                ? null
                : (MarkupString)Markdig.Markdown.ToHtml(Vehicle.AiOverview);

        private MarkupString? AiCommonIssuesHtml =>
            string.IsNullOrWhiteSpace(Vehicle?.AiCommonIssues)
                ? null
                : (MarkupString)Markdig.Markdown.ToHtml(Vehicle.AiCommonIssues);

        private MarkupString? AiMotHistorySummaryHtml =>
            string.IsNullOrWhiteSpace(Vehicle?.AiMotHistorySummary)
                ? null
                : (MarkupString)Markdig.Markdown.ToHtml(Vehicle.AiMotHistorySummary);

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
                case SearchType.AiOverview:
                    _isSearchingAiOverview = true;
                    break;
                case SearchType.AiCommonIssues:
                    _isSearchingAiCommonIssues = true;
                    break;
                case SearchType.AiMotHistorySummary:
                    _isSearchingAiMotHistorySummary = true;
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
                case SearchType.AiOverview:
                    _isSearchingAiOverview = false;
                    break;
                case SearchType.AiCommonIssues:
                    _isSearchingAiCommonIssues = false;
                    break;
                case SearchType.AiMotHistorySummary:
                    _isSearchingAiMotHistorySummary = false;
                    break;
            }

            return InvokeAsync(StateHasChanged);
        }

        private async Task OnCommonIssuesExpandedAsync(bool expanded)
        {
            if (!_isSearchingAiCommonIssues && Vehicle != null && String.IsNullOrEmpty(Vehicle.AiCommonIssues))
            {
                await StartSearch(SearchType.AiCommonIssues);
            }
        }

        private async Task OnMotHistoryExpandedAsync(bool expanded)
        {
            if (!_isSearchingAiMotHistorySummary && Vehicle != null && String.IsNullOrEmpty(Vehicle.AiMotHistorySummary))
            {
                await StartSearch(SearchType.AiMotHistorySummary);
            }
        }

        private async Task StartSearch(SearchType searchType)
        {
            if (Vehicle != null)
            {
                await SearchRegistrationEventService.NotifySearchStarted(searchType);
                Vehicle = await SearchRegistrationService.SearchVehicleAsync(Vehicle, searchType);
                await SearchRegistrationEventService.NotifySearchCompleted(Vehicle, searchType);
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
