using BarBuddy.DTOs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarBuddy.Client.Services
{
    public interface IReservationService
    {
        Task<bool> CheckCurrentReservation();

        Task<ReservationResult> GetReservationById(long id);

        Task<ReservationResult> CreateReservation(NewReservation newReservation);

        Task<ReservationResult> CheckIn(long id);

        Task<ReservationResult> CheckOut(long id);

        Task<bool> CancelReservation(long id);
    }

    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public ReservationService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<bool> CheckCurrentReservation()
        {
            var reservationId = await _localStorage.GetItemAsync<long>(AppSettings.LocalStorage_ReservationId);
            if (reservationId > 0)
            {
                try
                {
                    var reservation = await GetReservationById(reservationId);
                    if (reservation.IsExpired && !reservation.IsRunning)
                    {
                        await _localStorage.RemoveItemAsync(AppSettings.LocalStorage_ReservationId);
                    }
                    else if (!reservation.IsExpired && !reservation.IsRunning)
                    {
                        _navigationManager.NavigateTo($"/reservation/{reservationId}");
                        return true;
                    }
                    else if (reservation.IsRunning)
                    {
                        _navigationManager.NavigateTo($"/checkin/{reservationId}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    await _localStorage.RemoveItemAsync(AppSettings.LocalStorage_ReservationId);
                }
            }

            return false;
        }

        public async Task<ReservationResult> GetReservationById(long id)
        {
            var response = await _httpClient.GetAsync($"api/reservation/GetReservationById/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            return JsonSerializer.Deserialize<ReservationResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ReservationResult> CreateReservation(NewReservation newReservation)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservation/CreateReservation", newReservation);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var reservationResult = JsonSerializer.Deserialize<ReservationResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            await _localStorage.SetItemAsync(AppSettings.LocalStorage_ReservationId, reservationResult.Id);
            return reservationResult;
        }

        public async Task<ReservationResult> CheckIn(long id)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservation/CheckIn", id);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return JsonSerializer.Deserialize<ReservationResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<ReservationResult> CheckOut(long id)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservation/CheckOut", id);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            return JsonSerializer.Deserialize<ReservationResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CancelReservation(long id)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reservation/CancelReservation", id);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var cancelResult = JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (cancelResult)
            {
                await _localStorage.RemoveItemAsync(AppSettings.LocalStorage_ReservationId);
            }
            return cancelResult;
        }
    }
}
