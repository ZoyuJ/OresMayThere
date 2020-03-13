using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GTOresMayHere {
  public static class Kits {
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
      public int Left;                             //最左坐标
      public int Top;                             //最上坐标
      public int Right;                           //最右坐标
      public int Bottom;                        //最下坐标
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern int FindWindow(
            string lpClassName,
            string lpWindowName
           );

    [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    public static IntPtr FindWindowHandle(IntPtr ParentWindow, IntPtr ChildWindowAfter, string WindowClass, string WindowTitle) { return FindWindowEx(ParentWindow, ChildWindowAfter, WindowClass, WindowTitle); }

    public static RECT GetWindowRect(IntPtr hWnd) {
      RECT _RECT = new RECT();
      GetWindowRect(hWnd, ref _RECT);
      return _RECT;
    }




  }
}
