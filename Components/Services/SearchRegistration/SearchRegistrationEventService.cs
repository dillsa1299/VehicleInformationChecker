using System.Threading.Tasks;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public sealed class SearchRegistrationEventService : ISearchRegistrationEventService
    {
        private event ISearchRegistrationEventService.SearchVehicleEvent? _onSearchVehicle;
        private event ISearchRegistrationEventService.SearchStartedEvent? _onSearchStarted;
        private event ISearchRegistrationEventService.SearchCompletedEvent? _onSearchComplete;

        event ISearchRegistrationEventService.SearchVehicleEvent ISearchRegistrationEventService.OnSearchVehicle
        {
            add => _onSearchVehicle += value;
            remove => _onSearchVehicle -= value;
        }

        event ISearchRegistrationEventService.SearchStartedEvent ISearchRegistrationEventService.OnSearchStarted
        {
            add => _onSearchStarted += value;
            remove => _onSearchStarted -= value;
        }

        event ISearchRegistrationEventService.SearchCompletedEvent ISearchRegistrationEventService.OnSearchCompleted
        {
            add => _onSearchComplete += value;
            remove => _onSearchComplete -= value;
        }

        public Task NotifySearchVehicleAsync(string registration)
        {
            var task = _onSearchVehicle?.Invoke(registration);

            return task ?? Task.CompletedTask;
        }

        public Task NotifySearchStarted(SearchType searchType)
        {
            _onSearchStarted?.Invoke(searchType);
            return Task.CompletedTask;
        }

        public Task NotifySearchCompleted(VehicleModel vehicle, SearchType searchType)
        {
            var task = _onSearchComplete?.Invoke(vehicle, searchType);

            return task ?? Task.CompletedTask;
        }

        public void Dispose()
        {
            _onSearchVehicle = null;
            _onSearchStarted = null;
            _onSearchComplete = null;
        }
    }
}
