using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public sealed class SearchRegistrationEventService : ISearchRegistrationEventService
    {
        private event ISearchRegistrationEventService.SearchRegistrationEvent? _onSearchRegistration;
        private event ISearchRegistrationEventService.SearchRegistrationEvent? _onSearchComplete;

        event ISearchRegistrationEventService.SearchRegistrationEvent ISearchRegistrationEventService.OnSearchRegistration
        {
            add => _onSearchRegistration += value;
            remove => _onSearchRegistration -= value;
        }

        event ISearchRegistrationEventService.SearchRegistrationEvent ISearchRegistrationEventService.OnSearchCompleted
        {
            add => _onSearchComplete += value;
            remove => _onSearchComplete -= value;
        }

        public Task NotifySearchRegistration(string registration)
        {
            var task = _onSearchRegistration?.Invoke(registration);

            return task ?? Task.CompletedTask;
        }

        public Task NotifySearchCompleted(string registration)
        {
            var task = _onSearchComplete?.Invoke(registration);
            return task ?? Task.CompletedTask;
        }

        public void Dispose()
        {
            _onSearchRegistration = null;
            _onSearchComplete = null;
        }
    }
}
