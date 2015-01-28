using System.Drawing;
using System.Runtime.InteropServices;

namespace SignalRemote
{
  public class ScreenCapture
  {
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int smIndex);

    public static Size GetScreenSize()
    {
      return new Size(GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN));
    }

    public Image GetScreenshot()
    {
      var size = GetScreenSize();
      var image = new Bitmap(size.Width, size.Height);
      using (var g = Graphics.FromImage(image))
      {
        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), size, CopyPixelOperation.SourceCopy);

        //var temp = Path.GetTempPath();
        //image.Save(Path.Combine(temp, "test.bmp"));

        return image;
      }
    }
  }
}