using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarBuddy.Client.Services
{
    public interface IQRCodeService
    {
        Task<bool> IsValid(long reservationId, string qrCode);
    }

    public class QRCodeService : IQRCodeService
    {
        private readonly HttpClient _httpClient;

        public QRCodeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsValid(long reservationId, string qrCode)
        {
            return true;

            //var response = await _httpClient.PostAsJsonAsync("api/reservation/CheckQRCode", reservationId);
            //var content = await response.Content.ReadAsStringAsync();
            //if (!response.IsSuccessStatusCode)
            //{
            //    throw new ApplicationException(content);
            //}
            //return JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
