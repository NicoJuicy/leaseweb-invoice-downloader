using CommandLine;
using leaseweb_downloader.Services;

namespace leaseweb_downloader
{
    public class Options
    {
        [Option('k', "api-key", Required = true, HelpText = "Leaseweb API Key (X-LSW-Auth).")]
        public string ApiKey { get; set; } = string.Empty;

        [Option('o', "output", Required = false, Default = ".", HelpText = "Base output directory.")]
        public string OutputPath { get; set; } = ".";

        [Option("pdf", Required = false, Default = true, HelpText = "Download invoices as PDF.")]
        public bool IncludePdf { get; set; } = true;

        [Option("json", Required = false, Default = false, HelpText = "Download invoice details as JSON.")]
        public bool IncludeJson { get; set; }

        [Option("csv", Required = false, Default = false, HelpText = "Export invoices summary as CSV.")]
        public bool IncludeCsv { get; set; }

        [Option('y', "year", Required = false, Default = null, HelpText = "Filter invoices by year (e.g., 2025).")]
        public int? Year { get; set; }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    async opts => await RunAsync(opts),
                    errs => Task.FromResult(1)
                );
        }

        static async Task<int> RunAsync(Options opts)
        {
            Console.WriteLine("=== Leaseweb Invoice Downloader ===");
            Console.WriteLine($"Output directory: {Path.GetFullPath(opts.OutputPath)}");
            Console.WriteLine($"Formats: PDF={opts.IncludePdf}, JSON={opts.IncludeJson}, CSV={opts.IncludeCsv}");
            if (opts.Year.HasValue)
            {
                Console.WriteLine($"Filtering by year: {opts.Year.Value}");
            }
            Console.WriteLine();

            var httpClient = new HttpClient();
            var leasewebClient = new LeasewebClient(httpClient, opts.ApiKey);
            var downloader = new DownloaderService(leasewebClient, opts.OutputPath, opts.Year);

            try
            {
                await downloader.DownloadAllInvoicesAsync(opts.IncludePdf, opts.IncludeJson, opts.IncludeCsv);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                return 1;
            }
        }
    }
}
