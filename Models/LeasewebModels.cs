using System.Text.Json.Serialization;

namespace leaseweb_downloader.Models
{
    public class InvoiceListResponse
    {
        [JsonPropertyName("invoices")]
        public List<Invoice>? Invoices { get; set; }

        [JsonPropertyName("_metadata")]
        public Metadata? Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }

    public class Invoice
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("taxAmount")]
        public decimal? TaxAmount { get; set; }

        [JsonPropertyName("netAmount")]
        public decimal? NetAmount { get; set; }

        [JsonPropertyName("total")]
        public decimal? Total { get; set; }

        [JsonPropertyName("openAmount")]
        public decimal? OpenAmount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("isPartialPaymentAllowed")]
        public bool? IsPartialPaymentAllowed { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("credits")]
        public List<object>? Credits { get; set; } // Keeping as object for now as structure isn't defined

        [JsonPropertyName("lineItems")]
        public List<object>? LineItems { get; set; } // Keeping as object for now
    }
}
