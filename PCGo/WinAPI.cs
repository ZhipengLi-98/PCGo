using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static CefSharp.Wpf.Internals.ImeNative;

namespace PCGo
{
    class WinAPI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filter) : this()
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, uint dwAttribute, out uint pvAttribute, uint cbAttribute);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, uint flags);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bSCan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #region MouseEvent Message Code
        public const int MOUSEEVENTF_MOVE = 0x0001;
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        #endregion

        #region KeyEvent Message Code
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int KEYEVENTF_KEYDOWN = 0x0000;
        public const int KEYEVENTF_TAB = 0x09;
        public const int KEYEVENTF_LEFTSHIFT = 0xA0;
        public const int KEYEVENTF_LEFTCONTROL = 0xA2;
        public const int KEYEVENTF_VOLUEMEMUTE = 173;
        public const int KEYEVENTF_VOLUEMEDOWN = 174;
        public const int KEYEVENTF_VOLUEMEUP = 175;
        #endregion

        #region Windows Message Code
        public static UInt32 WM_CLOSE = 0x0010;
        public static UInt32 WM_MOVE = 0x0003;
        public static UInt32 WM_SYSCOMMAND = 0x0112;
        public static UInt32 SC_MINIMIZE = 0xF020;
        public static UInt32 SC_MAXIMIZE = 0xF030;
        public static UInt32 SC_RESTORE = 0xF120;
        public static UInt32 SC_NEXTWINDOW = 0xF040;
        public static UInt32 SC_LASTWINDOW = 0xF050;
        public static UInt32 SC_MOVE = 0xF010;
        public static UInt32 SC_SIZE = 0xF000;
        public static uint SW_RESTORE = 0x09;
        public static int SWP_NOMOVE = 0x0002;
        public static int SWP_NOSIZE = 0x0001;
        public static int SWP_NOACTIVATE = 0x0010;
        public static int SWP_NOZORDER = 0x0004;
        public static int SWP_SHOWWINDOW = 0x0040;
        public static IntPtr ICON_SMALL = new IntPtr(0);
        public static IntPtr ICON_BIG = new IntPtr(1);
        public static IntPtr ICON_SMALL2 = new IntPtr(2);
        public static uint PerIconTimeoutMilliseconds = 50;
        public static uint WM_GETICON = 0x7F;
        public static uint SMTO_ABORTIFHUNG = 0x0002;
        #endregion
    }
}
