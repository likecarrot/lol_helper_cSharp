using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class LeagueOfLegendsWindow
{
    private IntPtr _hWnd;

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public LeagueOfLegendsWindow()
    {
        _hWnd = FindWindow(null, "League of Legends");
    }

    public bool IsFound()
    {
        return _hWnd != IntPtr.Zero;
    }

    public RECT GetWindowRect()
    {
        RECT rect;
        GetWindowRect(_hWnd, out rect);
        return rect;
    }
}