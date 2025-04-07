namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public interface ISearchRegistrationEventService
    {
        /// <summary>
        /// Grid Line With Section Number Event Delegate
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public delegate Task SearchRegistrationEvent(string registration);

        /// <summary>
        /// Event for when a registration is searched
        /// </summary>
        event SearchRegistrationEvent OnSearchRegistration;

        /// <summary>
        /// Event for when a search is completed
        /// </summary>
        event SearchRegistrationEvent OnSearchCompleted;

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
        /// <returns></returns>
        Task NotifySearchCompleted(string registration);
    }
}
