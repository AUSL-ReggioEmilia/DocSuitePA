using iTextSharp.text;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class RectangleExtensions
    {

        public static bool IsLandscape(this Rectangle source)
        {
            return source.Height < source.Width;
        }
        public static bool IsSquare(this Rectangle source)
        {
            return source.Height == source.Width;
        }

        public static Rectangle ToPageSizeWithRotation(this Rectangle source, Rectangle pageSize)
        {
            var result = new Rectangle(pageSize);
            if (source.IsLandscape() != pageSize.IsLandscape())
                result = pageSize.Rotate();
            result.Rotation = source.Rotation;
                
            return result;
        }
        public static Rectangle ToA4WithRotation(this Rectangle source)
        {
            return source.ToPageSizeWithRotation(PageSize.A4);
        }
        public static Rectangle Replicate(this Rectangle source)
        {
            return source.ToPageSizeWithRotation(source);
        }

    }
}
