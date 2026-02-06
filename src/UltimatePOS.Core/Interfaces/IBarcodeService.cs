using System.Threading.Tasks;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for barcode and QR code generation
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// Generate a barcode image from the given data
    /// </summary>
    /// <param name="data">The data to encode</param>
    /// <param name="format">Barcode format (Code128, EAN13, etc.)</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <returns>Barcode image as byte array (PNG)</returns>
    Task<byte[]> GenerateBarcodeAsync(string data, BarcodeFormat format, int width = 300, int height = 150);

    /// <summary>
    /// Generate a QR code image from the given data
    /// </summary>
    /// <param name="data">The data to encode</param>
    /// <param name="size">QR code size in pixels (square)</param>
    /// <returns>QR code image as byte array (PNG)</returns>
    Task<byte[]> GenerateQRCodeAsync(string data, int size = 250);

    /// <summary>
    /// Generate barcode labels for multiple products
    /// </summary>
    /// <param name="labels">List of label data to print</param>
    /// <param name="template">Label template to use</param>
    /// <returns>Label sheet image as byte array (PNG)</returns>
    Task<byte[]> GenerateLabelSheetAsync(IEnumerable<BarcodeLabelData> labels, LabelTemplate template);
}

public class BarcodeLabelData
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
}

/// <summary>
/// Barcode format enumeration
/// </summary>
public enum BarcodeFormat
{
    Code128,
    Code39,
    EAN13,
    EAN8,
    UPCA,
    UPCE,
    QRCode
}

/// <summary>
/// Label template definition
/// </summary>
public class LabelTemplate
{
    public string Name { get; set; } = string.Empty;
    public int LabelsPerRow { get; set; }
    public int LabelsPerColumn { get; set; }
    public double LabelWidth { get; set; } // in mm
    public double LabelHeight { get; set; } // in mm
    public bool ShowProductName { get; set; }
    public bool ShowPrice { get; set; }
    public bool ShowSKU { get; set; }
    
    // Common templates
    public static LabelTemplate A4_40Up => new()
    {
        Name = "A4 40-up (52.5x29.7mm)",
        LabelsPerRow = 2,
        LabelsPerColumn = 20,
        LabelWidth = 52.5,
        LabelHeight = 29.7,
        ShowProductName = true,
        ShowPrice = true,
        ShowSKU = false
    };

    public static LabelTemplate A4_24Up => new()
    {
        Name = "A4 24-up (63.5x33.9mm)",
        LabelsPerRow = 3,
        LabelsPerColumn = 8,
        LabelWidth = 63.5,
        LabelHeight = 33.9,
        ShowProductName = true,
        ShowPrice = true,
        ShowSKU = true
    };
}
