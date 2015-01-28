using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace SignalRemote.Controllers
{
  public class RemoteController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Screen()
    {
      var imageBytes = new MemoryStream();

      using (var image = new ScreenCapture().GetScreenshot())
      {
        image.Save(imageBytes, ImageFormat.Png);
      }

      imageBytes.Seek(0, SeekOrigin.Begin);
      return new FileStreamResult(imageBytes, "image/png");
    }
  }
}
