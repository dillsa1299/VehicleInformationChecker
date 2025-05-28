using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public interface ISearchRegistrationEventService
    {
        /// <summary>
        /// Search vehicle details event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchVehicleEvent(string registration);

        /// <summary>
        /// Search completed event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchCompletedEvent(VehicleModel vehicle, SearchType searchType);

        /// <summary>
        /// Search completed event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchStartedEvent(SearchType searchType);

        /// <summary>
        /// Event for searching for vehicle details
        /// </summary>
        event SearchVehicleEvent OnSearchVehicle;

        /// <summary>
        /// Event for when a search is started
        /// </summary>
        event SearchStartedEvent OnSearchStarted;

        /// <summary>
        /// Event for when a search is completed
        /// </summary>
        event SearchCompletedEvent OnSearchCompleted;

        /// <summary>
        /// Triggers any event listeners to begin a search
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns><see cref="Task"/></returns>
        Task NotifySearchVehicleAsync(string registration);

        /// <summary>
        /// Triggers any event listeners when a search has started
        /// </summary>
        Task NotifySearchStarted(SearchType searchType);

        /// <summary>
        /// Triggers any event listeners when a search has completed
        /// </summary>
        /// <param name="vehicle"><see cref="VehicleModel"/></param>
        Task NotifySearchCompleted(VehicleModel vehicle, SearchType searchType);
    }
}
