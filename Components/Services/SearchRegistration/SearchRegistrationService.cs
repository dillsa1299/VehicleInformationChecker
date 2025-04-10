using System.Text.Json;
using VehicleInformationChecker.Components.Models;

namespace VehicleInformationChecker.Components.Services.SearchRegistrationService
{
    public sealed class SearchRegistrationService : ISearchRegistrationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _vesKey;
        private readonly string _vesURL;

        public SearchRegistrationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _vesKey = configuration["APIs:VES:Key"]
                      ?? throw new InvalidOperationException("API key not found in configuration.");
            _vesURL = configuration["APIs:VES:URL"]
                      ?? throw new InvalidOperationException("API URL not found in configuration.");
        }

        public async ValueTask<VehicleModel> SearchRegistration(string registration)
        {
            var vehicleData = new VehicleModel();
            vehicleData = await SearchVESAPI(registration);
            return vehicleData;
        }

        private async ValueTask<VehicleModel?> SearchVESAPI(string registration)
        {
            var vehicleData = new VehicleModel();

            var requestPayload = new
            {
                registrationNumber = registration
            };

            // Add required headers
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _vesKey);

            // Make HTTP POST request
            var response = await _httpClient.PostAsJsonAsync(_vesURL, requestPayload);

            if (!response.IsSuccessStatusCode)
            {
                return vehicleData;
            }

            // Parse API response
            var responseContent = await response.Content.ReadAsStringAsync();
            vehicleData = JsonSerializer.Deserialize<VehicleModel>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (vehicleData == null)
            {
                throw new InvalidOperationException("Failed to parse vehicle data.");
            }

            return vehicleData;
        }
    }
}
