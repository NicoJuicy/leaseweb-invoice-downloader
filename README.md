# Leaseweb Invoice Downloader

A vibe-coded .NET CLI tool to download invoices from the Leaseweb API.

## Features

- Download invoices as **PDF**
- Export invoice details as **JSON**
- Export all invoices summary as **CSV**
- **Filter by year** (e.g., `--year 2025`)
- Organized folder structure by year
- Skip already downloaded files

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later
- Leaseweb API Key (generate one in the [Customer Portal](https://my.leaseweb.com))

## Installation

```bash
git clone <repository-url>
cd leaseweb-downloader
dotnet build
```

## Usage

```bash
# Basic usage (downloads PDFs only)
dotnet run -- -k YOUR_API_KEY

# Download to specific folder
dotnet run -- -k YOUR_API_KEY -o ./downloads

# Download all formats (PDF, JSON, CSV)
dotnet run -- -k YOUR_API_KEY -o ./downloads --pdf --json --csv

# JSON and CSV only (no PDF)
dotnet run -- -k YOUR_API_KEY --pdf false --json --csv

# Download only invoices from a specific year
dotnet run -- -k YOUR_API_KEY --year 2025

# Combine year filter with output directory
dotnet run -- -k YOUR_API_KEY -o ./downloads -y 2024
```

### CLI Options

| Option | Default | Description |
|--------|---------|-------------|
| `-k`, `--api-key` | *(required)* | Leaseweb API Key (`X-LSW-Auth`) |
| `-o`, `--output` | `.` | Base output directory |
| `-y`, `--year` | *(all years)* | Filter invoices by year (e.g., 2025) |
| `--pdf` | `true` | Download invoices as PDF |
| `--json` | `false` | Download invoice details as JSON |
| `--csv` | `false` | Export invoices summary as CSV |
| `--help` | | Display help screen |
| `--version` | | Display version information |

## Output Structure

```
{output}/downloaded/
└── invoices/
    ├── 2024/
    │   ├── 2024_01_15_00000001.pdf
    │   ├── 2024_01_15_00000001.json
    │   └── ...
    ├── 2025/
    │   └── ...
    └── invoices_export.csv
```

## API Documentation

This tool uses the [Leaseweb Invoices API](https://developer.leaseweb.com/):

- `GET /invoices/v1/invoices` - List all invoices
- `GET /invoices/v1/invoices/{id}` - Get invoice details
- `GET /invoices/v1/invoices/{id}/pdf` - Download invoice PDF
- `GET /invoices/v1/invoices/export/csv` - Export invoices as CSV

## License

MIT
