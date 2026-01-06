using System.Net.Http.Json;
using leaseweb_downloader.Models;

namespace leaseweb_downloader.Services
{
    public class LeasewebClient
    {
        private readonly HttpClient _httpClient;

        public LeasewebClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.leaseweb.com");
            // clear headers just in case
            if (_httpClient.DefaultRequestHeaders.Contains("X-LSW-Auth"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-LSW-Auth");
            }
            _httpClient.DefaultRequestHeaders.Add("X-LSW-Auth", apiKey);
        }

        public async Task<InvoiceListResponse?> GetInvoicesAsync(int limit = 20, int offset = 0)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<InvoiceListResponse>($"/invoices/v1/invoices?limit={limit}&offset={offset}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching invoices list: {ex.Message}");
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceDetailAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Invoice>($"/invoices/v1/invoices/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching invoice detail for {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Stream> GetInvoicePdfAsync(string id)
        {
            return await _httpClient.GetStreamAsync($"/invoices/v1/invoices/{id}/pdf");
        }
        
        public async Task<Stream> GetCsvExportAsync(string? dateFrom = null, string? dateTo = null)
        {
             var query = "/invoices/v1/invoices/export/csv";
             var queryParams = new List<string>();
             if(!string.IsNullOrEmpty(dateFrom)) queryParams.Add($"dateFrom={dateFrom}");
             if(!string.IsNullOrEmpty(dateTo)) queryParams.Add($"dateTo={dateTo}");
             
             if(queryParams.Any()) query += "?" + string.Join("&", queryParams);
             
             return await _httpClient.GetStreamAsync(query);
        }
    }
}
