using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace SignalRemote
{
  public class RemoteHub : Hub
  {
    public void Ping()
    {
      this.Clients.Caller.pong();
    }

    public void SendInput(RemoteInput input)
    {
      new TouchScreen(ScreenCapture.GetScreenSize()).TouchAt(input.Left, input.Top);
    }
  }

  public class RemoteInput
  {
    [JsonProperty("left")]
    public int Left { get; set; }

    [JsonProperty("top")]
    public int Top { get; set; }
  }

  public class TouchScreen
  {
    private readonly Size size;

    public TouchScreen(Size screenSize)
    {
      this.size = screenSize;
    }

    public void TouchAt(int x, int y)
    {
      var normalized = Normalize(x, y);
      mouse_event((uint)(MouseEvent.Move | MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
      mouse_event((uint)(MouseEvent.LeftDown | MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
      mouse_event((uint)(MouseEvent.LeftUp | MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
    }

    private Point Normalize(int x, int y)
    {
      return new Point((int)(((float)x / size.Width) * 65535), (int)(((float)y / size.Height) * 65535));
    }

    [Flags]
    private enum MouseEvent
    {
      Move = 1,
      LeftDown = 2,
      LeftUp = 4,
      RightDown = 8,
      RightUp = 16,
      MiddleDown = 32,
      MiddleUp = 64,
      Absolute = 32768
    }

    [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
  }
}