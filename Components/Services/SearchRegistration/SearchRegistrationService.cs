using System.Globalization;
using System.Text.Json;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Models.SearchResponses;
using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;
using VehicleInformationChecker.Components.Models.SearchResponses.MotSearch;

namespace VehicleInformationChecker.Components.Services.SearchRegistrationService
{
    public sealed class SearchRegistrationService : ISearchRegistrationService
    {
        private readonly HttpClient _httpClient;

        private readonly string _vesKey;
        private readonly string _vesURL;

        private readonly string _motUrl;
        private readonly string _motClientId;
        private readonly string _motClientSecret;
        private readonly string _motKey;
        private readonly string _motScopeUrl;
        private readonly string _motTokenUrl;
        private MotAuthToken _motAuthToken = new MotAuthToken();

        private readonly string _googleKey;
        private readonly string _googleCx;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SearchRegistrationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Get configuration values
            _vesKey = configuration["APIs:VES:Key"]
                      ?? throw new InvalidOperationException("VES API key not found in configuration.");
            _vesURL = configuration["APIs:VES:URL"]
                      ?? throw new InvalidOperationException("VES API URL not found in configuration.");

            _motUrl = configuration["APIs:MOT:URL"]
                      ?? throw new InvalidOperationException("MOT API URL not found in configuration.");
            _motClientId = configuration["APIs:MOT:ClientId"]
                           ?? throw new InvalidOperationException("MOT API ClientId not found in configuration.");
            _motClientSecret = configuration["APIs:MOT:ClientSecret"]
                               ?? throw new InvalidOperationException("MOT API ClientSecret not found in configuration.");
            _motKey = configuration["APIs:MOT:Key"]
                      ?? throw new InvalidOperationException("MOT API key not found in configuration.");
            _motScopeUrl = configuration["APIs:MOT:ScopeUrl"]
                           ?? throw new InvalidOperationException("MOT API ScopeUrl not found in configuration.");
            _motTokenUrl = configuration["APIs:MOT:TokenUrl"]
                           ?? throw new InvalidOperationException("MOT API TokenUrl not found in configuration.");

            _googleKey = configuration["APIs:Google:Key"]
                         ?? throw new InvalidOperationException("Google API key not found in configuration.");
            _googleCx = configuration["APIs:Google:Cx"]
                        ?? throw new InvalidOperationException("Google API cx not found in configuration.");

            // TODO: Should this be loaded from a different file?
        }

        public async ValueTask<VehicleModel> SearchRegistrationAsync(string registration)
        {
            // Setup search tasks
            var vesTask = SearchVesAsync(registration);
            var motTask = SearchMotAsync(registration);

            // Wait for all to complete
            await Task.WhenAll(vesTask.AsTask(), motTask.AsTask());

            // Get results
            var vesSearchResponse = await vesTask;
            var motSearchResponse = await motTask;

            if (string.IsNullOrEmpty(vesSearchResponse.RegistrationNumber) || string.IsNullOrEmpty(vesSearchResponse.RegistrationNumber))
                // TODO: Should use try catch instead for proper error handling
                return new VehicleModel();

            // Image search
            var imageSearchResponse = await SearchImagesAsync($"{vesSearchResponse.Colour} {vesSearchResponse.YearOfManufacture} {vesSearchResponse.Make} {motSearchResponse.Model}");
            if (imageSearchResponse?.Items == null || imageSearchResponse.Items.Count == 0)
                // TODO: Should have placeholder image if none found
                return new VehicleModel();

            return MapResponses(vesSearchResponse, motSearchResponse, imageSearchResponse); ;
        }

        private async ValueTask<VesSearchResponse> SearchVesAsync(string registration)
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
            if (!response.IsSuccessStatusCode)
                return parsedResponse;

            // Parse response
            var responseContent = await response.Content.ReadAsStringAsync();
            parsedResponse = JsonSerializer.Deserialize<VesSearchResponse>(responseContent, _jsonSerializerOptions);

            if (String.IsNullOrEmpty(parsedResponse?.RegistrationNumber))
            {
                throw new InvalidOperationException("Failed to parse VES search response.");
            }

            return parsedResponse;
        }

        private async ValueTask<MotSearchResponse> SearchMotAsync(string registration)
        {
            // Get authentication token
            if (!await GetMotTokenAsync()) return new MotSearchResponse();

            // Call the MOT API with the access token
            var motRequest = new HttpRequestMessage(HttpMethod.Get, $"{_motUrl}/{Uri.EscapeDataString(registration)}");
            motRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_motAuthToken.Type, _motAuthToken.Token);
            motRequest.Headers.Add("x-api-key", _motKey);

            using var motResponse = await _httpClient.SendAsync(motRequest);
            if (!motResponse.IsSuccessStatusCode)
                return new MotSearchResponse();

            var motContent = await motResponse.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<MotSearchResponse>(motContent, _jsonSerializerOptions);

            return parsedResponse ?? new MotSearchResponse();
        }

        public async Task<bool> GetMotTokenAsync()
        {
            // Check if token is still valid
            if (_motAuthToken.ExpireTime > DateTime.UtcNow) return true;

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _motTokenUrl)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = _motClientId,
                    ["client_secret"] = _motClientSecret,
                    ["scope"] = _motScopeUrl,
                    ["grant_type"] = "client_credentials"
                })
            };

            using var tokenResponse = await _httpClient.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
                return false;

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenContent);

            var tokenType = tokenDoc.RootElement.GetProperty("token_type").GetString();
            var expiresIn = tokenDoc.RootElement.GetProperty("expires_in").GetInt32();
            var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();

            if (string.IsNullOrEmpty(tokenType) || string.IsNullOrEmpty(accessToken))
                return false;

            _motAuthToken = new MotAuthToken
            {
                Type = tokenType,
                ExpireTime = DateTime.UtcNow.AddSeconds(expiresIn),
                Token = accessToken
            };

            return true;
        }

        public async Task<ImageSearchResponse?> SearchImagesAsync(string query)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(query)}&cx={_googleCx}&key={_googleKey}&searchType=image";
            var response = await _httpClient.GetFromJsonAsync<ImageSearchResponse>(url);

            return response;
        }

        private static VehicleModel MapResponses(VesSearchResponse vesSearchResponse, MotSearchResponse motSearchResponse, ImageSearchResponse imageSearchResponse)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            return new VehicleModel
            {
                RegistrationNumber = vesSearchResponse.RegistrationNumber,
                YearOfManufacture = vesSearchResponse.YearOfManufacture,
                Make = textInfo.ToTitleCase(vesSearchResponse.Make.ToLower()),
                Model = textInfo.ToTitleCase(motSearchResponse.Model.ToLower()),
                Colour = textInfo.ToTitleCase(vesSearchResponse.Colour.ToLower()),
                EngineCapacity = $"{vesSearchResponse.EngineCapacity} cc",
                FuelType = textInfo.ToTitleCase(vesSearchResponse.FuelType.ToLower()),
                TaxStatus = vesSearchResponse.TaxStatus,
                TaxDueDate = DateOnly.ParseExact(vesSearchResponse.TaxDueDate, "yyyy-MM-dd"),
                MotStatus = vesSearchResponse.MotStatus,
                MotExpiryDate = !String.IsNullOrEmpty(vesSearchResponse.MotExpiryDate) ?
                                    DateOnly.ParseExact(vesSearchResponse.MotExpiryDate, "yyyy-MM-dd") :
                                    null,
                DateOfLastV5CIssued = DateOnly.ParseExact(vesSearchResponse.DateOfLastV5CIssued, "yyyy-MM-dd"),
                MonthOfFirstRegistration = DateOnly.ParseExact(vesSearchResponse.MonthOfFirstRegistration, "yyyy-MM"),
                Images = imageSearchResponse.Items ?? []
            };
        }
    }
}
