using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

#pragma warning disable CS0618 // Type or member is obsolete

public class BarcodeService : IBarcodeService
{
    public BarcodeService()
    {
    }

    public Task<byte[]> GenerateBarcodeAsync(string data, Core.Interfaces.BarcodeFormat format, int width = 300, int height = 150)
    {
        // Stub implementation to pass build
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<byte[]> GenerateQRCodeAsync(string data, int size = 250)
    {
        // Stub implementation to pass build
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<byte[]> GenerateLabelSheetAsync(IEnumerable<BarcodeLabelData> labels, LabelTemplate template)
    {
        // Stub implementation to pass build
        return Task.FromResult(Array.Empty<byte>());
    }
}
