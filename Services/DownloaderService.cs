using System.Text.Json;
using leaseweb_downloader.Models;

namespace leaseweb_downloader.Services
{
    public class DownloaderService
    {
        private readonly LeasewebClient _client;
        private readonly string _basePath;

        public DownloaderService(LeasewebClient client, string basePath)
        {
            _client = client;
            _basePath = basePath;
        }

        public async Task DownloadAllInvoicesAsync(bool includePdf, bool includeJson, bool includeCsv)
        {
            Console.WriteLine("Fetching invoice list...");
            var allInvoices = new List<Invoice>();
            int offset = 0;
            int limit = 50;
            int total = 0;

            do
            {
                var response = await _client.GetInvoicesAsync(limit, offset);
                if (response?.Invoices == null || response.Invoices.Count == 0)
                    break;

                allInvoices.AddRange(response.Invoices);
                total = response.Metadata?.TotalCount ?? 0;
                offset += limit;
                Console.WriteLine($"  Fetched {allInvoices.Count} of {total} invoices...");
            } while (allInvoices.Count < total);

            Console.WriteLine($"Found {allInvoices.Count} invoices. Starting download...");

            foreach (var invoice in allInvoices)
            {
                if (invoice.Id == null || invoice.Date == null)
                {
                    Console.WriteLine($"  Skipping invoice with missing id or date.");
                    continue;
                }

                var date = invoice.Date.Value;
                var year = date.Year.ToString();
                var month = date.Month.ToString("D2");
                var day = date.Day.ToString("D2");
                var invoiceId = invoice.Id;

                var invoiceDir = Path.Combine(_basePath, "downloaded", "invoices", year);
                Directory.CreateDirectory(invoiceDir);

                var fileBaseName = $"{year}_{month}_{day}_{invoiceId}";

                if (includePdf)
                {
                    var pdfPath = Path.Combine(invoiceDir, $"{fileBaseName}.pdf");
                    if (!File.Exists(pdfPath))
                    {
                        try
                        {
                            Console.WriteLine($"  Downloading PDF: {pdfPath}");
                            await using var pdfStream = await _client.GetInvoicePdfAsync(invoiceId);
                            await using var fileStream = File.Create(pdfPath);
                            await pdfStream.CopyToAsync(fileStream);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"    Error downloading PDF for {invoiceId}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  Skipping (exists): {pdfPath}");
                    }
                }

                if (includeJson)
                {
                    var jsonPath = Path.Combine(invoiceDir, $"{fileBaseName}.json");
                    if (!File.Exists(jsonPath))
                    {
                        try
                        {
                            Console.WriteLine($"  Downloading JSON: {jsonPath}");
                            var detail = await _client.GetInvoiceDetailAsync(invoiceId);
                            if (detail != null)
                            {
                                var json = JsonSerializer.Serialize(detail, new JsonSerializerOptions { WriteIndented = true });
                                await File.WriteAllTextAsync(jsonPath, json);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"    Error downloading JSON for {invoiceId}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  Skipping (exists): {jsonPath}");
                    }
                }
            }

            // CSV export (bulk)
            if (includeCsv)
            {
                var csvDir = Path.Combine(_basePath, "downloaded", "invoices");
                Directory.CreateDirectory(csvDir);
                var csvPath = Path.Combine(csvDir, "invoices_export.csv");
                try
                {
                    Console.WriteLine($"  Downloading CSV export: {csvPath}");
                    await using var csvStream = await _client.GetCsvExportAsync();
                    await using var fileStream = File.Create(csvPath);
                    await csvStream.CopyToAsync(fileStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    Error downloading CSV export: {ex.Message}");
                }
            }

            Console.WriteLine("Download complete.");
        }
    }
}
