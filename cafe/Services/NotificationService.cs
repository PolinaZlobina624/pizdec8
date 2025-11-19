using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace RestaurantApp.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Отправка Push-уведомления сотруднику
        public async Task NotifyEmployeeAsync(string deviceToken, string title, string body)
        {
            // Отправляем уведомление через Firebase Cloud Messaging (FCM)
            var fcmUrl = "https://fcm.googleapis.com/fcm/send";
            var serverKey = _configuration["FCM_SERVER_KEY"]; // ключ для доступа к FCM

            // Формируем payload уведомления
            var notificationPayload = new
            {
                to = deviceToken,
                notification = new
                {
                    title = title,
                    body = body
                },
                priority = "high"
            };

            // Отправляем POST-запрос на сервер FCM
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", serverKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var content = JsonSerializer.Serialize(notificationPayload);
                var httpResponse = await client.PostAsync(fcmUrl, new StringContent(content));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Ошибка отправки уведомления.");
                }
            }
        }
    }
}