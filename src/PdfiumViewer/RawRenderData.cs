namespace PdfiumViewer
{
    public class RawRenderData
    {
        public RawRenderData(byte[] datainBgra, int width, int height)
        {
            DataInBgra = datainBgra;
            Width = width;
            Height = height;
        }

        public byte[] DataInBgra { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
