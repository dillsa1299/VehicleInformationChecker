using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.Services.SearchRegistrationService
{
    public interface ISearchRegistrationService
    {
        /// <summary>
        /// Searches for a vehicle registration
        /// </summary>
        /// <param name="registration"><see cref="string"/></param>
        /// <returns><see cref="VehicleModel"/></returns>
        ValueTask<VehicleModel> SearchRegistrationAsync(string registration);
    }
}
