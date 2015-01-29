using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SignalRemote.Controllers
{
  public class RemoteController : Controller
  {
    private static object jpegCodecLock = new object();
    private static ImageCodecInfo jpegCodec = null;

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Screen()
    {
      var imageBytes = new MemoryStream();

      using (var image = new ScreenCapture().GetScreenshot())
      {
        var codec = GetJpegCodec();
        var codecParams = new EncoderParameters();
        codecParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)50);
        image.Save(imageBytes, codec, codecParams);
      }

      imageBytes.Seek(0, SeekOrigin.Begin);
      return new FileStreamResult(imageBytes, "image/jpeg");
    }

    private ImageCodecInfo GetJpegCodec()
    {
      if (jpegCodec == null)
      {
        lock (jpegCodecLock)
        {
          if (jpegCodec == null)
          {
            jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
          }
        }
      }

      return jpegCodec;
    }
  }
}
