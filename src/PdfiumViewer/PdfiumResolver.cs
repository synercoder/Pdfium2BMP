namespace PdfiumViewer
{
    public static class PdfiumResolver
    {
        public static event PdfiumResolveEventHandler? Resolve;

        internal static string? GetPdfiumFileName()
        {
            var e = new PdfiumResolveEventArgs();
            _onResolve(e);
            return e.PdfiumFileName;
        }

        private static void _onResolve(PdfiumResolveEventArgs e)
        {
            Resolve?.Invoke(null, e);
        }
    }
}
