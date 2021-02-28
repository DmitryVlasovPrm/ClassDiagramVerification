using Emgu.CV;
using Emgu.CV.Structure;

namespace ClassDiagrammVerification
{
    public static class MinCoordinates
    {
        public static (int, int) Compute(Image<Bgra, byte> image)
        {
            var grayImage = image.Convert<Gray, byte>();
            var imageHeight = grayImage.Height;
            var imageWidth = grayImage.Width;

            var minX = imageWidth;
            for (var i = 0; i < imageHeight; i++)
                for (var j = 0; j < imageWidth / 4; j++)
                    if (grayImage.Data[i, j, 0] != 0 && j < minX)
                        minX = j;

            var minY = imageHeight;
            for (var i = 0; i < imageWidth; i++)
                for (var j = 0; j < imageHeight / 4; j++)
                    if (grayImage.Data[j, i, 0] != 0 && j < minY)
                        minY = j;

            return (minX, minY);
        }
    }
}
