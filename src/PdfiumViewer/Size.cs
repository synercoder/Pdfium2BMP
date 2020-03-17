using System;

namespace Pdfium2BMP
{
    public struct Size : IEquatable<Size>
    {
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; }
        public double Height { get; }

        public override int GetHashCode()
        {
            var hashCode = 859600377;
            hashCode = (hashCode * -1521134295) + Width.GetHashCode();
            hashCode = (hashCode * -1521134295) + Height.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object? obj) 
            => obj is Size size ? Equals(size) : false;

        public bool Equals(Size other) 
            => other.Width == Width && other.Height == Height;

        public static bool operator ==(Size left, Size right) 
            => left.Equals(right);

        public static bool operator !=(Size left, Size right) 
            => !(left == right);
    }
}
