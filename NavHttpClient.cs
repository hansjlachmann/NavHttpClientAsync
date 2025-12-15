using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NavHttpClientAsync
{
    [Guid("2fb45a77-9e85-431d-9ac9-d030107deebd")]
    [ComVisible(true)]
    public interface INavHttpClient
    {
        string HelloWorld();
        void SendHttpRequest(string url, string method, string headers, string body);
    }

    [Guid("79b05537-33c7-4f24-be8b-581373154e71")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ProgId("NavHttpClientAsync.NavHttpClient")]
    public class NavHttpClient : INavHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public string HelloWorld()
        {
            return "Hello World!";
        }

        public void SendHttpRequest(string url, string method, string headers, string body)
        {
            // Fire and forget - returns immediately without blocking NAV
            Task.Run(async () =>
            {
                try
                {
                    await ExecuteHttpRequestAsync(url, method, headers, body);
                }
                catch (Exception ex)
                {
                    // Log error silently (could write to Windows Event Log or file)
                    System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                }
            });
        }

        private async Task ExecuteHttpRequestAsync(string url, string method, string headers, string body)
        {
            using (var request = new HttpRequestMessage())
            {
                // Set HTTP method
                request.Method = new HttpMethod(method.ToUpper());
                request.RequestUri = new Uri(url);

                // Parse and add headers (format: "Header1:Value1|Header2:Value2")
                if (!string.IsNullOrEmpty(headers))
                {
                    var headerPairs = headers.Split('|');
                    foreach (var headerPair in headerPairs)
                    {
                        var parts = headerPair.Split(new[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            var headerName = parts[0].Trim();
                            var headerValue = parts[1].Trim();

                            // Try to add to request headers first
                            if (!request.Headers.TryAddWithoutValidation(headerName, headerValue))
                            {
                                // If it fails, it might be a content header
                                if (request.Content == null)
                                {
                                    request.Content = new StringContent(string.Empty);
                                }
                                request.Content.Headers.TryAddWithoutValidation(headerName, headerValue);
                            }
                        }
                    }
                }

                // Add body for POST, PUT, PATCH
                if (!string.IsNullOrEmpty(body) && (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                                                     method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
                                                     method.Equals("PATCH", StringComparison.OrdinalIgnoreCase)))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                // Send the request
                var response = await httpClient.SendAsync(request);

                // Optionally read response (for logging/debugging)
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");
            }
        }
    }
}
