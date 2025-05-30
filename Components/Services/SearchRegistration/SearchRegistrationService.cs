using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using VehicleInformationChecker.Components.Models;
using VehicleInformationChecker.Components.Models.SearchResponses;
using VehicleInformationChecker.Components.Models.SearchResponses.ImageSearch;
using VehicleInformationChecker.Components.Models.SearchResponses.MotSearch;
using static VehicleInformationChecker.Components.Models.MotDefectModel;

namespace VehicleInformationChecker.Components.Services.SearchRegistration
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
        private MotAuthToken _motAuthToken = new();

        private readonly string _googleKey;
        private readonly string _googleCx;

        private readonly string _geminiUrl;
        private readonly string _geminiKey;

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

            _geminiUrl = configuration["APIs:Gemini:URL"]
                         ?? throw new InvalidOperationException("Gemini API URL not found in configuration.");
            _geminiKey = configuration["APIs:Gemini:Key"]
                         ?? throw new InvalidOperationException("Gemini API key not found in configuration.");
        }

        public async Task<VehicleModel> SearchVehicleAsync(VehicleModel vehicle, SearchType searchType)
        {
            if (string.IsNullOrEmpty(vehicle.RegistrationNumber) || !Regex.IsMatch(vehicle.RegistrationNumber, @"^[a-zA-Z0-9]{0,7}$"))
                return new VehicleModel();

            VesSearchResponse? vesSearchResponse = null;
            MotSearchResponse? motSearchResponse = null;
            ImageSearchResponse? imageSearchResponse = null;
            string? aiSummary = null;
            string? aiCommonIssues = null;

            string aiCarDetails = $"Year={vehicle.YearOfManufacture}, Make={vehicle.Make}, Model={vehicle.Model}, Fuel Type={vehicle.FuelType}, Engine Capacity={vehicle.EngineCapacity}";

            switch (searchType)
            {
                case SearchType.Details:
                    // Setup search tasks
                    var vesTask = SearchVesAsync(vehicle.RegistrationNumber);
                    var motTask = SearchMotAsync(vehicle.RegistrationNumber);

                    // Wait for all to complete
                    await Task.WhenAll(vesTask.AsTask(), motTask.AsTask());

                    // Get results
                    vesSearchResponse = await vesTask;
                    motSearchResponse = await motTask;

                    if (string.IsNullOrEmpty(vesSearchResponse.RegistrationNumber) || string.IsNullOrEmpty(vesSearchResponse.RegistrationNumber))
                        // Failed to get data from one of the APIs
                        return new VehicleModel();
                    break;

                case SearchType.Images:
                    var query = $"{vehicle.Colour} {vehicle.YearOfManufacture} {vehicle.Make} {vehicle.Model}";
                    imageSearchResponse = await SearchImagesAsync(query);
                    break;

                case SearchType.AiSummary:
                    var prompt = $"Provide a summary of the following UK specification vehicle, including performance metrics and providing additional details," +
                        $"but not using any markup. This information is already displayed so should not be repeated: " + aiCarDetails;
                    aiSummary = await SearchGeminiAsync(prompt);
                    break;

                case SearchType.AiCommonIssues:
                    // AI Common Issues search
                    var commonIssuesPrompt = $"List the common issues with the UK specification of the following vehicle with no introduction: " + aiCarDetails;
                    aiCommonIssues = await SearchGeminiAsync(commonIssuesPrompt);
                    break;
            }

            return MapResponses(vehicle, vesSearchResponse, motSearchResponse, imageSearchResponse, aiSummary, aiCommonIssues);
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

        private async Task<bool> GetMotTokenAsync()
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

        private async Task<ImageSearchResponse?> SearchImagesAsync(string query)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?q={Uri.EscapeDataString(query)}&cx={_googleCx}&key={_googleKey}&searchType=image";
            var response = await _httpClient.GetFromJsonAsync<ImageSearchResponse>(url);

            if (response?.Items != null)
            {
                int index = 1; // Initialize index starting from 1
                foreach (var item in response.Items)
                {
                    item.Index = index++; // Increment index for each item
                }
            }

            return response;
        }

        private async Task<string> SearchGeminiAsync(string prompt)
        {
            var geminiRequest = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };
            var jsonBody = JsonSerializer.Serialize(geminiRequest);

            var requestContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_geminiUrl + _geminiKey, requestContent);

            if (!response.IsSuccessStatusCode)
                return "Unable to generate AI summary. Please try again.";

            string responseString = await response.Content.ReadAsStringAsync();

            // responseString contains the JSON from the API
            using var doc = JsonDocument.Parse(responseString);

            // Grab only the text from the response, the rest is information we don't need
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? String.Empty;
        }

        private static VehicleModel MapResponses(
            VehicleModel vehicleModel,
            VesSearchResponse? vesSearchResponse,
            MotSearchResponse? motSearchResponse,
            ImageSearchResponse? imageSearchResponse,
            string? aiSummary,
            string? aiCommonIssues)
        {

            if (vesSearchResponse != null)
            {
                vehicleModel.RegistrationNumber = vesSearchResponse.RegistrationNumber ?? string.Empty;
                vehicleModel.YearOfManufacture = vesSearchResponse.YearOfManufacture;
                vehicleModel.Make = FormatName(vesSearchResponse.Make, 3);
                vehicleModel.Colour = FormatName(vesSearchResponse.Colour, 0);
                vehicleModel.EngineCapacity = $"{vesSearchResponse.EngineCapacity} cc";
                vehicleModel.FuelType = FormatName(vesSearchResponse.FuelType, 0);
                vehicleModel.TaxStatus = vesSearchResponse.TaxStatus ?? string.Empty;
                vehicleModel.MotStatus = vesSearchResponse.MotStatus ?? string.Empty;
                vehicleModel.MotExpiryDate = DateOnlyTryParse(vesSearchResponse.MotExpiryDate, "yyyy-MM-dd");
                vehicleModel.DateOfLastV5CIssued = DateOnlyTryParse(vesSearchResponse.DateOfLastV5CIssued, "yyyy-MM-dd") ?? default;
                vehicleModel.MonthOfFirstRegistration = DateOnlyTryParse(vesSearchResponse.MonthOfFirstRegistration, "yyyy-MM") ?? default;
                vehicleModel.TaxDueDate = DateOnlyTryParse(vesSearchResponse.TaxDueDate, "yyyy-MM-dd");
            }

            if (motSearchResponse != null)
            {
                vehicleModel.Model = FormatName(motSearchResponse.Model, 3);

                if (motSearchResponse.MotTests != null)
                {
                    vehicleModel.MotTests = [.. motSearchResponse.MotTests.Select(test => new MotModel
                    {
                        CompletedDate = DateOnlyTryParseIso(test.CompletedDate) ?? default,
                        Passed = test.TestResult == "PASSED",
                        ExpiryDate = DateOnlyTryParse(test.ExpiryDate, "yyyy-MM-dd") ?? default,
                        OdometerValue = long.TryParse(test.OdometerValue, out var odo) ? odo : -1,
                        OdometerUnit = test.OdometerUnit == "KM" ? "Kilometers" : "Miles",
                        Defects = test.Defects?.Select(defect => new MotDefectModel
                        {
                            Type = GetDefectType(defect.Type),
                            Description = defect.Text,
                            Dangerous = defect.Dangerous
                        }).ToList() ?? []
                    })];
                }
            }

            if (imageSearchResponse != null)
            {
                vehicleModel.Images = imageSearchResponse?.Items ?? [];
            }

            if (aiSummary != null)
            {
                vehicleModel.AiSummary = aiSummary;
            }

            if (aiCommonIssues != null)
            {
                vehicleModel.AiCommonIssues = aiCommonIssues;
            }

            return vehicleModel;

            // Helper for title-casing with length check
            static string FormatName(string? value, int maxLength) =>
                !string.IsNullOrWhiteSpace(value)
                    ? (value.Length > maxLength ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant()) : value)
                    : string.Empty;

            // Local helper for safe date parsing
            static DateOnly? DateOnlyTryParse(string? value, string format)
                => !string.IsNullOrWhiteSpace(value) && DateOnly.TryParseExact(value, format, out var date)
                    ? date
                    : null;

            // Local helper to convert MOT defect type string to enum
            static MotDefectType GetDefectType(string? type)
            {
                return type switch
                {
                    "ADVISORY" => MotDefectType.Advisory,
                    "DANGEROUS" => MotDefectType.Dangerous,
                    "FAIL" => MotDefectType.Fail,
                    "MAJOR" => MotDefectType.Major,
                    "MINOR" => MotDefectType.Minor,
                    "NON SPECIFIC" => MotDefectType.NonSpecific,
                    "SYSTEM GENERATED" => MotDefectType.SystemGenerated,
                    "USER ENTERED" => MotDefectType.UserEntered,
                    _ => MotDefectType.UserEntered
                };
            }
        }
        static DateOnly? DateOnlyTryParseIso(string? value)
            => DateTimeOffset.TryParse(value, out var dt) ? DateOnly.FromDateTime(dt.DateTime) : null;
    }
}
