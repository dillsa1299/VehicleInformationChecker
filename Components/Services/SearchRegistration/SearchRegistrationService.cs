using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Services.SearchRegistration;

namespace VehicleInformationChecker.Components.Services.SearchRegistrationService
{
    public sealed class SearchRegistrationService:ISearchRegistrationService
    {
        // Todo - Listen for event of searching registration

        // Call API w/ registration

        // Parse API response

        // Notify listeners of search completion w/ response

        public async ValueTask<VehicleModel> SearchRegistration(string registration)
        {
            var VehicleModel = new VehicleModel();

            // Simulate a delay for the search
            await Task.Delay(5000);

            return VehicleModel;
        }
    }
}
