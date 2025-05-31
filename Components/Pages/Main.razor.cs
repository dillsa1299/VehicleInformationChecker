using Microsoft.AspNetCore.Components;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Pages
{
    public partial class Main : IDisposable
    {
        [Inject]
        private ISearchRegistrationEventService SearchRegistrationEventService { get; set; } = default!;

        [Inject]
        private ISearchRegistrationService SearchRegistrationService { get; set; } = default!;

        private VehicleModel _vehicle = new();

        private async Task SearchRegistration(string registration)
        {
            // Details Search
            _vehicle = new VehicleModel { RegistrationNumber = registration };
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.Details);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(_vehicle, SearchType.Details);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.Details);
            StateHasChanged();

            // Don't waste further API calls on failed search
            if (String.IsNullOrEmpty(_vehicle.RegistrationNumber)) return;

            // Perform parallel searches for images and AI overview
            var imagesTask = SearchImages();
            var aiOverviewTask = SearchAiOverview();
            await Task.WhenAll(imagesTask, aiOverviewTask);
        }

        private async Task SearchImages()
        {
            // Image Search
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.Images);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(_vehicle, SearchType.Images);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.Images);
            StateHasChanged();
        }

        private async Task SearchAiOverview()
        {
            // AI Overview search
            await SearchRegistrationEventService.NotifySearchStarted(SearchType.AiOverview);
            _vehicle = await SearchRegistrationService.SearchVehicleAsync(_vehicle, SearchType.AiOverview);
            await SearchRegistrationEventService.NotifySearchCompleted(_vehicle, SearchType.AiOverview);
            StateHasChanged();
        }

        private Task OnSearchCompleted(VehicleModel vehicle, SearchType searchType)
        {
            _vehicle = vehicle;
            return InvokeAsync(StateHasChanged);
        }

        protected override Task OnInitializedAsync()
        {
            SearchRegistrationEventService.OnSearchVehicle += SearchRegistration;
            SearchRegistrationEventService.OnSearchCompleted += OnSearchCompleted;
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            SearchRegistrationEventService.OnSearchVehicle -= SearchRegistration;
            SearchRegistrationEventService.OnSearchCompleted -= OnSearchCompleted;
            GC.SuppressFinalize(this);
        }
    }
}
