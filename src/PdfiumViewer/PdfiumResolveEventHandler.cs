using System;

namespace Pdfium2BMP
{
    public class PdfiumResolveEventArgs : EventArgs
    {
        public string? PdfiumFileName { get; set; }
    }

    public delegate void PdfiumResolveEventHandler(object? sender, PdfiumResolveEventArgs e);
}
