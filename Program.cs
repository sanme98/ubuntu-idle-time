using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("libX11.so")]
    public static extern IntPtr XOpenDisplay(string displayName);

    [DllImport("libX11.so")]
    public static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libXss.so.1")]
    public static extern IntPtr XScreenSaverAllocInfo();

    [DllImport("libXss.so.1")]
    public static extern void XScreenSaverQueryInfo(IntPtr display, IntPtr window, IntPtr xssInfo);

    [StructLayout(LayoutKind.Sequential)]
    public struct XScreenSaverInfo
    {
        public IntPtr window;
        public int state;
        public int kind;
        public ulong since;
        public ulong idle;
        public ulong event_mask;
    }

    static void Main()
    {
        var displayName = Environment.GetEnvironmentVariable("DISPLAY");
        if (string.IsNullOrEmpty(displayName))
        {
            Console.WriteLine("No display found, unable to detect idle time.");
            return;
        }

        IntPtr dpy = XOpenDisplay(displayName);
        IntPtr root = XDefaultRootWindow(dpy);
        IntPtr xssInfo = XScreenSaverAllocInfo();

        XScreenSaverQueryInfo(dpy, root, xssInfo);

        XScreenSaverInfo xssInfoData = Marshal.PtrToStructure<XScreenSaverInfo>(xssInfo);
        Console.WriteLine("Idle time in milliseconds: {0}", xssInfoData.idle);

        // Remember to free the allocated memory
        Marshal.FreeHGlobal(xssInfo);
    }
}
