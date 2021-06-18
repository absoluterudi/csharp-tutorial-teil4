using BarBuddy.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarBuddy.Client.Services
{
    public interface ILocationService
    {
        Task<List<BarResult>> GetLocationsWithinRadius(CurrentPosition currentPosition);

        Task<BarResult> GetLocationWithSpotInfos(long id);
    }

    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BarResult>> GetLocationsWithinRadius(CurrentPosition currentPosition)
        {
            var response = await _httpClient.PostAsJsonAsync("api/location/GetLocationsWithinRadius", currentPosition);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return JsonSerializer.Deserialize<List<BarResult>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<BarResult> GetLocationWithSpotInfos(long id)
        {
            var response = await _httpClient.GetAsync($"api/location/GetLocationWithSpotInfos/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<BarResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
