using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
{
    public interface ISearchRegistrationService
    {
        /// <summary>
        /// Searches for vehicle details based on the provided registration number.
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns><see cref="VehicleModel"/></returns>
        Task<VehicleModel> SearchVehicleDetailsAsync(string registration);

        /// <summary>
        /// Searches for additional vehicle details which can be loaded after the initial search.
        /// </summary>
        /// <param name="vehicle"><see cref="VehicleModel"/></param>
        /// <returns><see cref="VehicleModel"/></returns>
        Task<VehicleModel> SearchVehicleAdditionalDetailsAsync(VehicleModel vehicle);
    }
}
