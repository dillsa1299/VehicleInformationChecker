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
        Task<VehicleModel> SearchVehicleAsync(VehicleModel vehicle, SearchType searchType);
    }
}
