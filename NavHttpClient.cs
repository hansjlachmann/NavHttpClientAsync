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
        void SendHttpRequest(string url, string method, string headers, string body, string filePath);
    }

    [Guid("79b05537-33c7-4f24-be8b-581373154e71")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    [ProgId("NavHttpClientAsync.NavHttpClient")]
    public class NavHttpClient : INavHttpClient
    {
        private static readonly HttpClient httpClient;

        static NavHttpClient()
        {
            // Enable TLS 1.2 and TLS 1.3 for older Windows servers
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls;

            httpClient = new HttpClient();
        }

        public string HelloWorld()
        {
            return "Hello World!";
        }

        public void SendHttpRequest(string url, string method, string headers, string body, string filePath)
        {
            // Fire and forget - returns immediately without blocking NAV
            Task.Run(async () =>
            {
                try
                {
                    await ExecuteHttpRequestAsync(url, method, headers, body, filePath);
                }
                catch (Exception ex)
                {
                    // Log detailed error to file if path provided
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        var errorLog = new StringBuilder();
                        errorLog.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR");
                        errorLog.AppendLine($"URL: {url}");
                        errorLog.AppendLine($"Method: {method}");
                        errorLog.AppendLine($"Error Type: {ex.GetType().Name}");
                        errorLog.AppendLine($"Error Message: {ex.Message}");

                        // Log inner exceptions
                        var innerEx = ex.InnerException;
                        int level = 1;
                        while (innerEx != null)
                        {
                            errorLog.AppendLine($"Inner Exception {level}: {innerEx.GetType().Name}");
                            errorLog.AppendLine($"Inner Message {level}: {innerEx.Message}");
                            innerEx = innerEx.InnerException;
                            level++;
                        }

                        errorLog.AppendLine($"Stack Trace: {ex.StackTrace}");
                        errorLog.AppendLine(new string('-', 80));

                        LogToFile(filePath, errorLog.ToString());
                    }
                    System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                }
            });
        }

        private async Task ExecuteHttpRequestAsync(string url, string method, string headers, string body, string filePath)
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

                // Read response
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");

                // Log to file if path provided
                if (!string.IsNullOrEmpty(filePath))
                {
                    var logEntry = new StringBuilder();
                    logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                    logEntry.AppendLine($"URL: {url}");
                    logEntry.AppendLine($"Method: {method}");
                    logEntry.AppendLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
                    logEntry.AppendLine($"Response: {responseContent}");
                    logEntry.AppendLine(new string('-', 80));

                    LogToFile(filePath, logEntry.ToString());
                }
            }
        }

        private void LogToFile(string filePath, string content)
        {
            try
            {
                System.IO.File.AppendAllText(filePath, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
