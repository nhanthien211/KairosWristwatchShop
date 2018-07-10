using System.Drawing;
using System.IO;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;

namespace ProjectKairos.Utilities
{
    public class ImageProcessHelper
    {
        public static void ResizedImage(Stream stream, int width, int height, ResizeMode resizeMode, ref string path)
        {

            ISupportedImageFormat format = new PngFormat();
            format.Quality = 100;
            path = path + "." + format.DefaultExtension;
            Size size = new Size(width, height);
            using (var outStream = new MemoryStream())
            {
                using (ImageFactory imageFactory = new ImageFactory())
                {
                    ResizeLayer resizeLayer = new ResizeLayer(size, resizeMode);
                    imageFactory.Load(stream)
                        .Resize(resizeLayer)
                        .BackgroundColor(Color.White)
                        .Format(format)
                        .Save(path);
                }
            }
            path = MyCustomUtility.RelativeFromAbsolutePath(path);
        }
    }
}