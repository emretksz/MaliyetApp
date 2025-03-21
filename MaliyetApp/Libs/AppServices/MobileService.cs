using MaliyetApp.Libs.AppSettings;
using MaliyetApp.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.AppServices
{
    public class MobileService
    {
        // private static string baseApiUrl = "https://localhost:7239/"; https://10.0.2.2:7239/
        private static string baseApiUrl = AppConst.HostAddress;



        public static async Task<T?> GetAsync<T>(string url)
        {
            try
            {

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                using (var httpClient = new HttpClient(handler)) {
                    var urltest = $"{baseApiUrl}{url}";
                var response = await httpClient.GetAsync(urltest);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Küçük/büyük harf farkını yok say
                    };


                    if (String.IsNullOrEmpty(responseData))
                    {
                        return default(T);
                    }
                    return JsonSerializer.Deserialize<T>(responseData, options);

                }
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                ExceptionAlert.ShowAlert("Hata", $"Hata oluştu: {ex.Message}");
                return default;
            }
        }

        public static async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                var testt = JsonSerializer.Serialize(data);
                using (var httpClient = new HttpClient(handler))
                {
                    var content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json"
                );
                    var urltest = $"{baseApiUrl}{url}";
                    var response = await httpClient.PostAsync(urltest, content);
                  
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        var responseData = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(responseData))
                        {
                              return default(TResponse);
                        }

                        return JsonSerializer.Deserialize<TResponse>(responseData);
                    }
                    else
                    {
                       
                            var errorMessage = await response.Content.ReadAsStringAsync();

                        ExceptionAlert.ShowAlert("Hata", $"Hata oluştu: {errorMessage}");
                        return default;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                ExceptionAlert.ShowAlert("Hata", $"Hata oluştu: {ex.Message}");
                return default;
            }
        }

    }
}
