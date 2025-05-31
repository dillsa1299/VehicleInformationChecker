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
            if (expanded && !_isSearchingAiCommonIssues && Vehicle != null && String.IsNullOrEmpty(Vehicle.AiCommonIssues))
            {
                await SearchRegistrationEventService.NotifySearchStarted(SearchType.AiCommonIssues);
                Vehicle = await SearchRegistrationService.SearchVehicleAsync(Vehicle, SearchType.AiCommonIssues);
                await SearchRegistrationEventService.NotifySearchCompleted(Vehicle, SearchType.AiCommonIssues);
            }
        }

        private async Task OnMotHistoryExpandedAsync(bool expanded)
        {
            if (expanded && !_isSearchingAiMotHistorySummary && Vehicle != null && String.IsNullOrEmpty(Vehicle.AiMotHistorySummary))
            {
                await SearchRegistrationEventService.NotifySearchStarted(SearchType.AiMotHistorySummary);
                Vehicle = await SearchRegistrationService.SearchVehicleAsync(Vehicle, SearchType.AiMotHistorySummary);
                await SearchRegistrationEventService.NotifySearchCompleted(Vehicle, SearchType.AiMotHistorySummary);
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
