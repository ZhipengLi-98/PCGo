using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace PCGo
{
    class WindowManager
    {

        #region Window Controls
        public static void CloseWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Close Window:" + GetWindowTitle(hwnd));
                WinAPI.SendMessage(hwnd, WinAPI.WM_CLOSE, 0, IntPtr.Zero);
            }
        }

        public static void MinimizeWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Minimize Window: " + GetWindowTitle(hwnd));
                WinAPI.SendMessage(hwnd, WinAPI.WM_SYSCOMMAND, WinAPI.SC_MINIMIZE, IntPtr.Zero);
            }
        }

        public static void MaximizeWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Maximize Window: " + GetWindowTitle(hwnd));
                WinAPI.SendMessage(hwnd, WinAPI.WM_SYSCOMMAND, WinAPI.SC_MAXIMIZE, IntPtr.Zero);
            }
        }

        public static void RestoreWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Restore Window: " + GetWindowTitle(hwnd));
                WinAPI.SendMessage(hwnd, WinAPI.WM_SYSCOMMAND, WinAPI.SC_RESTORE, IntPtr.Zero);
            }
        }

        public static void ShowWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Show Window: " + GetWindowTitle(hwnd));
                WinAPI.ShowWindow(hwnd, WinAPI.SW_RESTORE);
            }
        }

        public static void MoveWindow(IntPtr hwnd, int xOffset, int yOffset)
        {
            if (hwnd != IntPtr.Zero)
            {
                WinAPI.RECT rect = new WinAPI.RECT();
                WinAPI.GetWindowRect(hwnd, out rect);
                int width = Math.Abs(rect.Right - rect.Left);
                int height = Math.Abs(rect.Bottom - rect.Top);
                int x = Math.Max(0 - width / 2, Math.Min((int)GetScreenWidth() + width / 2, rect.Left + xOffset));
                int y = Math.Max(0, Math.Min((int)GetScreenHeight(), rect.Top + yOffset));
                WinAPI.SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOACTIVATE | WinAPI.SWP_NOZORDER | WinAPI.SWP_SHOWWINDOW);
            }
        }

        public static void ScaleWindow(IntPtr hwnd, double xRatio, double yRatio)
        {
            if (hwnd != IntPtr.Zero)
            {
                WinAPI.RECT rect = new WinAPI.RECT();
                WinAPI.GetWindowRect(hwnd, out rect);
                int width = Math.Abs(rect.Right - rect.Left);
                int height = Math.Abs(rect.Bottom - rect.Top);
                int x = rect.Left;
                int y = rect.Top;
                int cx = x + width / 2;
                int cy = y + height / 2;
                int nWidth = (int)((double)width * xRatio);
                int nHeight = (int)((double)height * yRatio);
                int nx = cx - nWidth / 2;
                int ny = cy - nHeight / 2;
                WinAPI.SetWindowPos(hwnd, IntPtr.Zero, x, y, nWidth, nHeight, WinAPI.SWP_NOACTIVATE | WinAPI.SWP_NOZORDER | WinAPI.SWP_SHOWWINDOW);
            }
        }

        //public static void ReturnToDesktop()
        //{
        //    Shell32.Shell shell = new Shell32.Shell();
        //    shell.ToggleDesktop();
        //    //foreach(Window window in Application.Current.Windows)
        //    //{
        //    //    if(window.Title == "Menu" && window.WindowState == WindowState.Minimized)
        //    //    {
        //    //        window.WindowState = WindowState.Normal;
        //    //    }
        //    //}
        //}
        #endregion

        public static String GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder Buff = new StringBuilder(256);
            if (WinAPI.GetWindowText(hWnd, Buff, 256) > 0)
                return Buff.ToString();
            return null;
        }

        #region Screen Parameters
        public static double GetScreenWidth()
        {
            return SystemParameters.PrimaryScreenWidth;
        }

        public static double GetScreenHeight()
        {
            return SystemParameters.PrimaryScreenHeight;
        }

        public static System.Windows.Size GetScreenSize()
        {
            return new System.Windows.Size(SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
        }

        #endregion

        #region Find Active/Topmost Windows

        public static bool IsAltTabWindow(IntPtr hwnd)
        {
            const uint WS_EX_TOOLWINDOW = 0x00000080;
            const uint DWMWA_CLOAKED = 14;

            if (!WinAPI.IsWindowVisible(hwnd)) return false;

            string title = GetWindowTitle(hwnd);
            if (title == null || title == "Menu" || title == "GestureWindow" || title == "BlockingWindow" || title == "AppList") return false;

            WinAPI.WINDOWINFO winInfo = new WinAPI.WINDOWINFO(true);
            WinAPI.GetWindowInfo(hwnd, ref winInfo);
            if ((winInfo.dwExStyle & WS_EX_TOOLWINDOW) != 0) return false;

            uint CloakedVal;
            WinAPI.DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, out CloakedVal, sizeof(uint));
            return CloakedVal == 0;
        }

        private static int GetWindowZOrder(IntPtr hWnd)
        {
            var zOrder = -1;
            while ((hWnd = WinAPI.GetWindow(hWnd, 2)) != IntPtr.Zero) zOrder++;
            return zOrder;
        }


        public static IntPtr FindTopmostWindow()
        {
            List<IntPtr> hwnds = EnumerateWindow();
            if (hwnds.Count != 0)
            {
                int max = -1;
                IntPtr result = IntPtr.Zero;
                foreach (IntPtr hwnd in hwnds)
                {
                    var z = GetWindowZOrder(hwnd);
                    if (z > max)
                    {
                        max = z;
                        result = hwnd;
                    }
                }
                return result;
            }

            return IntPtr.Zero;
        }

        private static IntPtr GetLastVisibleActivePopUpOfWindow(IntPtr window)
        {
            IntPtr lastPopUp = WinAPI.GetLastActivePopup(window);
            if (WinAPI.IsWindowVisible(lastPopUp))
                return lastPopUp;
            else if (lastPopUp == window)
                return IntPtr.Zero;
            else
                return GetLastVisibleActivePopUpOfWindow(lastPopUp);
        }

        public static List<IntPtr> EnumerateWindow()
        {
            List<IntPtr> windows = new List<IntPtr>();
            WinAPI.EnumWindows(delegate (IntPtr hwnd, int param)
            {
                if (!IsAltTabWindow(hwnd))
                    return true;
                windows.Add(hwnd);
                return true;
            }, 0);


            //Console.WriteLine(windows.Count);
            //foreach (IntPtr hWnd in windows)
            //{
            //    Console.WriteLine(GetWindowTitle(hWnd));
            //}
            return windows;
        }
        #endregion
    }
}
