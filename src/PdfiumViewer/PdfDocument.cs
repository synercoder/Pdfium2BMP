using PdfiumViewer.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Pdfium2BMP
{
    /// <summary>
    /// Provides functionality to render a PDF document.
    /// </summary>
    public sealed class PdfDocument : IPdfDocument
    {
        private bool _disposed;
        private readonly PdfFile _file;
        private readonly List<Size> _pageSizes;

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path">Path to the PDF document.</param>
        public static PdfDocument Load(string path)
        {
            return Load(path, null);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path">Path to the PDF document.</param>
        /// <param name="password">Password for the PDF document.</param>
        public static PdfDocument Load(string path, string? password)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Load(File.OpenRead(path), password);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream">Stream for the PDF document.</param>
        public static PdfDocument Load(Stream stream)
        {
            return Load(stream, null);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream">Stream for the PDF document.</param>
        /// <param name="password">Password for the PDF document.</param>
        public static PdfDocument Load(Stream stream, string? password)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return new PdfDocument(stream, password);
        }

        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        public int PageCount
            => PageSizes.Count;

        /// <summary>
        /// Size of each page in the PDF document.
        /// </summary>
        public IReadOnlyList<Size> PageSizes { get; private set; }

        private PdfDocument(Stream stream, string? password)
        {
            _file = new PdfFile(stream, password);

            _pageSizes = _file.GetPDFDocInfo();
            if (_pageSizes == null)
                throw new Win32Exception();

            PageSizes = new ReadOnlyCollection<Size>(_pageSizes);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        public RawRenderData Render(int page, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            var size = PageSizes[page];

            return Render(page, (int)size.Width, (int)size.Height, dpiX, dpiY, flags);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        public RawRenderData Render(int page, int width, int height, float dpiX, float dpiY, PdfRenderFlags flags)
        {
            return Render(page, width, height, dpiX, dpiY, 0, flags);
        }

        public RawRenderData Render(int page, int width, int height, float dpiX, float dpiY, PdfRotation rotate, PdfRenderFlags flags)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (( flags & PdfRenderFlags.CorrectFromDpi ) != 0)
            {
                width = width * (int)dpiX / 72;
                height = height * (int)dpiY / 72;
            }

            var byteArray = new byte[4 * width * height];

            var gcHandle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);

            try
            {
                var handle = NativeMethods.FPDFBitmap_CreateEx(width, height, 4, gcHandle.AddrOfPinnedObject(), width * 4);

                try
                {
                    uint background = ( flags & PdfRenderFlags.Transparent ) == 0 ? 0xFFFFFFFF : 0x00FFFFFF;

                    NativeMethods.FPDFBitmap_FillRect(handle, 0, 0, width, height, background);

                    bool success = _file.RenderPDFPageToBitmap(
                        page,
                        handle,
                        0, 0, width, height,
                        (int)rotate,
                        _flagsToFPDFFlags(flags),
                        ( flags & PdfRenderFlags.Annotations ) != 0
                    );

                    if (!success)
                        throw new Win32Exception();
                }
                finally
                {
                    NativeMethods.FPDFBitmap_Destroy(handle);
                }
            }
            finally
            {
                gcHandle.Free();
            }

            return new RawRenderData(byteArray, width, height);
        }

        private NativeMethods.FPDF _flagsToFPDFFlags(PdfRenderFlags flags)
        {
            return (NativeMethods.FPDF)( flags & ~( PdfRenderFlags.Transparent | PdfRenderFlags.CorrectFromDpi ) );
        }

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="stream">Stream to save the PDF document to.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _file.Save(stream);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">Whether this method is called from Dispose.</param>
        private void _dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _file?.Dispose();

                _disposed = true;
            }
        }
    }
}
