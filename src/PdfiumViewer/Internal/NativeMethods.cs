using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pdfium2BMP.Internal
{
    internal static partial class NativeMethods
    {
        static NativeMethods()
        {
            // First try the custom resolving mechanism.

            string? fileName = PdfiumResolver.GetPdfiumFileName();
            if (fileName != null && File.Exists(fileName) && LoadLibrary(fileName) != IntPtr.Zero)
                return;

            // Load the platform dependent Pdfium.dll if it exists.

            if (!_tryLoadNativeLibrary(AppDomain.CurrentDomain.RelativeSearchPath))
                _tryLoadNativeLibrary(Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location));
        }

        private static bool _tryLoadNativeLibrary(string? path)
        {
            if (path == null)
                return false;

            path = Path.Combine(path, IntPtr.Size == 4 ? "x86" : "x64");
            path = Path.Combine(path, "Pdfium.dll");

            return File.Exists(path) && LoadLibrary(path) != IntPtr.Zero;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = Justifications.DLL_IMPORT_STYLE_MISMATCH)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);
    }
}
