using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pdfium2BMP.Internal
{
    internal static partial class NativeMethods
    {
        private const string LIBRARY_NAME = "pdfium";

        static NativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, PdfiumResolver.Resolve ?? _importResolver);
        }

        private static IntPtr _importResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr libHandle = IntPtr.Zero;
            if (libraryName == LIBRARY_NAME)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    NativeLibrary.TryLoad("pdfium.dll", assembly, DllImportSearchPath.SafeDirectories, out libHandle);
                else
                    NativeLibrary.TryLoad("pdfium.so", assembly, DllImportSearchPath.SafeDirectories, out libHandle);
            }
            return libHandle;
        }
    }
}
