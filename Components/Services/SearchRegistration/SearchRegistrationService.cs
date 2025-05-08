using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Models.SearchResponses;
using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;

namespace VehicleInformationChecker.Components.Services.SearchRegistrationService
{
    public sealed class SearchRegistrationService : ISearchRegistrationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _vesKey;
        private readonly string _vesURL;
        private readonly string _googleKey;
        private readonly string _googleCx;

        public SearchRegistrationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _vesKey = configuration["APIs:VES:Key"]
                      ?? throw new InvalidOperationException("VES API key not found in configuration.");
            _vesURL = configuration["APIs:VES:URL"]
                      ?? throw new InvalidOperationException("VES API URL not found in configuration.");
            _googleKey = configuration["APIs:Google:Key"]
                      ?? throw new InvalidOperationException("Google API key not found in configuration.");
            _googleCx = configuration["APIs:Google:Cx"]
                      ?? throw new InvalidOperationException("Google API cx not found in configuration.");
        }

        public async ValueTask<VehicleModel> SearchRegistrationAsync(string registration)
        {
            var vehicleData = new VehicleModel();

            var vesSearchResponse = await SearchVESAsync(registration);
            if (vesSearchResponse == null || string.IsNullOrEmpty(vesSearchResponse.RegistrationNumber))
            {
                return vehicleData;
            }

            var imageSearchResponse = await SearchImagesAsync($"{vesSearchResponse.Colour} {vesSearchResponse.YearOfManufacture} {vesSearchResponse.Make}");
            if (imageSearchResponse == null || imageSearchResponse.Items == null || !imageSearchResponse.Items.Any())
            {
                return vehicleData;
            }

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            vehicleData = new VehicleModel
            {
                RegistrationNumber = vesSearchResponse.RegistrationNumber,
                YearOfManufacture = vesSearchResponse.YearOfManufacture,
                Make = textInfo.ToTitleCase(vesSearchResponse.Make.ToLower()),
                Colour = textInfo.ToTitleCase(vesSearchResponse.Colour.ToLower()),
                EngineCapacity = $"{vesSearchResponse.EngineCapacity} cc",
                FuelType = textInfo.ToTitleCase(vesSearchResponse.FuelType.ToLower()),
                TaxStatus = vesSearchResponse.TaxStatus,
                TaxDueDate = DateOnly.ParseExact(vesSearchResponse.TaxDueDate, "yyyy-MM-dd"),
                MotStatus = vesSearchResponse.MotStatus,
                MotExpiryDate = DateOnly.ParseExact(vesSearchResponse.MotExpiryDate, "yyyy-MM-dd"),
                DateOfLastV5CIssued = DateOnly.ParseExact(vesSearchResponse.DateOfLastV5CIssued, "yyyy-MM-dd"),
                MonthOfFirstRegistration = DateOnly.ParseExact(vesSearchResponse.MonthOfFirstRegistration, "yyyy-MM"),
                Images = imageSearchResponse.Items
            };

            await Task.Delay(1000); // Simulate a delay for demonstration purposes

            return vehicleData;
        }

        private async ValueTask<VesSearchResponse> SearchVESAsync(string registration)
        {
            var parsedResponse = new VesSearchResponse();

            using var request = new HttpRequestMessage(HttpMethod.Post, _vesURL)
            {
                Content = JsonContent.Create(new
                {
                    registrationNumber = registration
                })
            };
            request.Headers.Add("x-api-key", _vesKey);
            using var response = await _httpClient.SendAsync(request);

            // Check if the response is successful
            if (!response.IsSuccessStatusCode) return parsedResponse;

            // Parse response
            var responseContent = await response.Content.ReadAsStringAsync();
            parsedResponse = JsonSerializer.Deserialize<VesSearchResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (parsedResponse == null)
            {
                throw new InvalidOperationException("Failed to parse VES search response.");
            }

            return parsedResponse;
        }

        public async Task<ImageSearchResponse?> SearchImagesAsync(string query)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(query)}&cx={_googleCx}&key={_googleKey}&searchType=image";
            var response = await _httpClient.GetFromJsonAsync<ImageSearchResponse>(url);

            return response;
        }
    }
}
