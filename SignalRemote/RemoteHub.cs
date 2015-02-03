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

    public void MoveMouse(MoveInput input)
    {
      new TouchPad().Move(input.DeltaX, input.DeltaY);
    }

    public void Click()
    {
      new TouchPad().Click();
    }
  }

  public class RemoteInput
  {
    [JsonProperty("left")]
    public int Left { get; set; }

    [JsonProperty("top")]
    public int Top { get; set; }
  }

  public class MoveInput
  {
    [JsonProperty("dx")]
    public int DeltaX { get; set; }

    [JsonProperty("dy")]
    public int DeltaY { get; set; }
  }

  public static class NativeMouseMethods
  {
    [Flags]
    public enum MouseEvent
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
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
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
      NativeMouseMethods.mouse_event((uint)(NativeMouseMethods.MouseEvent.Move | NativeMouseMethods.MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
      NativeMouseMethods.mouse_event((uint)(NativeMouseMethods.MouseEvent.LeftDown | NativeMouseMethods.MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
      NativeMouseMethods.mouse_event((uint)(NativeMouseMethods.MouseEvent.LeftUp | NativeMouseMethods.MouseEvent.Absolute), normalized.X, normalized.Y, 0, 0);
    }

    private Point Normalize(int x, int y)
    {
      return new Point((int)(((float)x / size.Width) * 65535), (int)(((float)y / size.Height) * 65535));
    }
  }

  public class TouchPad
  {
    public void Move(int dx, int dy)
    {
      NativeMouseMethods.mouse_event((uint)NativeMouseMethods.MouseEvent.Move, dx, dy, 0, 0);
    }

    public void Click()
    {
      NativeMouseMethods.mouse_event((uint)NativeMouseMethods.MouseEvent.LeftDown, 0, 0, 0, 0);
      NativeMouseMethods.mouse_event((uint)NativeMouseMethods.MouseEvent.LeftUp, 0, 0, 0, 0);
    }
  }
}