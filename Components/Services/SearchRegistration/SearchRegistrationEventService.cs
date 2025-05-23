using System.Threading.Tasks;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public sealed class SearchRegistrationEventService : ISearchRegistrationEventService
    {
        private event ISearchRegistrationEventService.SearchRegistrationEvent? _onSearchRegistration;
        private event Action? _onSearchStarted;
        private event ISearchRegistrationEventService.SearchCompletedEvent? _onSearchComplete;

        event ISearchRegistrationEventService.SearchRegistrationEvent ISearchRegistrationEventService.OnSearchRegistration
        {
            add => _onSearchRegistration += value;
            remove => _onSearchRegistration -= value;
        }

        event Action ISearchRegistrationEventService.OnSearchStarted
        {
            add => _onSearchStarted += value;
            remove => _onSearchStarted -= value;
        }

        event ISearchRegistrationEventService.SearchCompletedEvent ISearchRegistrationEventService.OnSearchCompleted
        {
            add => _onSearchComplete += value;
            remove => _onSearchComplete -= value;
        }

        public Task NotifySearchRegistrationAsync(string registration)
        {
            _onSearchStarted?.Invoke();
            var task = _onSearchRegistration?.Invoke(registration);

            return task ?? Task.CompletedTask;
        }

        public Task NotifySearchStarted()
        {
            _onSearchStarted?.Invoke();

            return Task.CompletedTask;
        }

        public Task NotifySearchCompleted(VehicleModel vehicle)
        {
            var task = _onSearchComplete?.Invoke(vehicle);

            return task ?? Task.CompletedTask;
        }

        public void Dispose()
        {
            _onSearchRegistration = null;
            _onSearchStarted = null;
            _onSearchComplete = null;
        }
    }
}
