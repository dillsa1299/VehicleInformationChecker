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
        public delegate void SearchCompletedEvent(VehicleModel vehicle);

        /// <summary>
        /// Event for when a registration is searched
        /// </summary>
        event SearchRegistrationEvent OnSearchRegistration;

        /// <summary>
        /// Event for when a search is completed
        /// </summary>
        event SearchCompletedEvent OnSearchCompleted;

        /// <summary>
        /// Triggers any event listeners when a registration search has started
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns></returns>
        Task NotifySearchRegistration(string registration);

        /// <summary>
        /// Triggers any event listeners when a registration search has completed
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns><see cref="VehicleModel"/></returns>
        void NotifySearchCompleted(VehicleModel vehicle);
    }
}
