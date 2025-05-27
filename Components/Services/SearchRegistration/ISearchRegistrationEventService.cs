using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public interface ISearchRegistrationEventService
    {
        /// <summary>
        /// Search with registration event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchRegistrationEvent(string registration);

        /// <summary>
        /// Search completed event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchCompletedEvent(VehicleModel vehicle);

        /// <summary>
        /// Search completed event delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchStartedEvent(bool isAdditionalSearch);

        /// <summary>
        /// Event for searching a registration
        /// </summary>
        event SearchRegistrationEvent OnSearchRegistration;

        /// <summary>
        /// Event for when a search is started
        /// </summary>
        event SearchStartedEvent OnSearchStarted;

        /// <summary>
        /// Event for when a search is completed
        /// </summary>
        event SearchCompletedEvent OnSearchCompleted;

        /// <summary>
        /// Triggers any event listeners to begin a registration search
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns><see cref="Task"/></returns>
        Task NotifySearchRegistrationAsync(string registration);

        /// <summary>
        /// Triggers any event listeners when a search has started
        /// </summary>
        Task NotifySearchStarted(bool isAdditionalSearch);

        /// <summary>
        /// Triggers any event listeners when a search has completed
        /// </summary>
        /// <param name="vehicle"><see cref="VehicleModel"/></param>
        Task NotifySearchCompleted(VehicleModel vehicle);
    }
}
