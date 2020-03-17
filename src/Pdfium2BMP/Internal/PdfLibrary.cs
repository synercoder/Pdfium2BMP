using System;

namespace Pdfium2BMP.Internal
{
    internal class PdfLibrary : IDisposable
    {
        private static readonly object _syncRoot = new object();
        private static PdfLibrary? _library;

        public static void EnsureLoaded()
        {
            lock (_syncRoot)
            {
                if (_library == null)
                    _library = new PdfLibrary();
            }
        }

        private bool _disposed;

        private PdfLibrary()
        {
            NativeMethods.FPDF_AddRef();
        }

        ~PdfLibrary()
        {
            _dispose();
        }

        public void Dispose()
        {
            _dispose();

            GC.SuppressFinalize(this);
        }

        private void _dispose()
        {
            if (!_disposed)
            {
                NativeMethods.FPDF_Release();

                _disposed = true;
            }
        }
    }
}
