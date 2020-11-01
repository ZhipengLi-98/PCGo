using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using System.Drawing;
using System.Threading;

using SocketUtils;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Interop;

using static CefSharp.Wpf.Internals.ImeNative;

namespace PCGo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INetworkCallback
    {
        public SocketClient pythonClient, androidClient;
        public Client anClient;

        private bool taskBar = false;
        [DllImport("User32")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bSCan, int dwFlags, int dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x0002;
        const int KEYEVENTF_KEYDOWN = 0x0000;
        const int KEYEVENTF_TAB = 0x09;
        const int KEYEVENTF_LEFTSHIFT = 0xA0;
        const int KEYEVENTF_LEFTCONTROL = 0xA2;
        const int KEYEVENTF_VOLUEMEMUTE = 173;
        const int KEYEVENTF_VOLUEMEDOWN = 174;
        const int KEYEVENTF_VOLUEMEUP = 175;

        [DllImport("user32 ")]
        public static extern bool LockWorkStation();

        [DllImport("user32")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        public int clickx, clicky;
        POINT lastCursorPos;
        private int mWidth = 3840;
        private int mHeight = 2160;

        private IntPtr hwnd;

        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine(SystemParameters.PrimaryScreenWidth);
            Console.WriteLine(SystemParameters.PrimaryScreenHeight);
            Console.WriteLine(SystemParameters.WorkArea);
            Console.WriteLine(SystemParameters.FullPrimaryScreenWidth);
            Console.WriteLine(SystemParameters.FullPrimaryScreenHeight);
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if (p.MainWindowTitle.Length > 0)
                {
                    Console.WriteLine(p.MainWindowTitle);
                }
            }
            pythonClient = new SocketClient(3000);
            pythonClient.StartClient();
            pythonClient.callback = this;
            // androidClient = new SocketClient("183.173.78.123", 4000);
            // androidClient.StartClient();
            // androidClient.callback = this;
            // anClient = new Client("183.173.77.113", 4000);

            hwnd = new WindowInteropHelper(this).Handle;
            this.WindowState = WindowState.Minimized;
        }

        public void startScreenRTC()
        {
            Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        this.Grid.Visibility = Visibility.Visible;
                        this.Left = 0;
                        this.Top = 0;
                        this.Width = 720;
                        this.Height = 1080;
                        this.WindowState = WindowState.Normal;
                        Browser.Reload();
                    }));
        }

        public void endScreenRTC()
        {
            Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        this.Grid.Visibility = Visibility.Hidden;
                        this.Left = 0;
                        this.Top = 0;
                        this.Width = 720;
                        this.Height = 1080;
                        this.WindowState = WindowState.Minimized;
                    }));
        }

        public void OnReceive(string data)
        {
            Console.WriteLine(data);
            switch (data)
            {
                case "phone":
                    startScreenRTC();
                    // clickx = 100;
                    // clicky = 100;
                    // mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, clickx, clicky, 0, 0);
                    // mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    break;
                case "endphone":
                    endScreenRTC();
                    break;
                case "image":
                    Console.WriteLine("Send Image");
                    anClient.Send("image");
                    break;
                case "file":
                    Console.WriteLine("Send File");
                    anClient.Send("file");
                    break;
                case "mission":
                    Console.WriteLine("Working Area");
                    GetCursorPos(out lastCursorPos);
                    clickx = mWidth / 3;
                    clicky = mHeight - 30;
                    taskBar = true;
                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    break;
                case "end":
                    Console.WriteLine("End");
                    taskBar = false;
                    clickx = lastCursorPos.X;
                    clicky = lastCursorPos.Y;
                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    break;
                case "left":
                    Console.WriteLine("Left");
                    keybd_event(KEYEVENTF_LEFTCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
                    keybd_event(KEYEVENTF_LEFTSHIFT, 0, KEYEVENTF_KEYDOWN, 0);
                    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYDOWN, 0);
                    keybd_event(KEYEVENTF_LEFTCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(KEYEVENTF_LEFTSHIFT, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYUP, 0);
                    break;
                case "right":
                    Console.WriteLine("Right");
                    keybd_event(KEYEVENTF_LEFTCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
                    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYDOWN, 0);
                    keybd_event(KEYEVENTF_LEFTCONTROL, 0, KEYEVENTF_KEYUP, 0);
                    keybd_event(KEYEVENTF_TAB, 0, KEYEVENTF_KEYUP, 0);
                    break;
                case "volup":
                    keybd_event(KEYEVENTF_VOLUEMEUP, 0, 0, 0);
                    break;
                case "voldown":
                    keybd_event(KEYEVENTF_VOLUEMEDOWN, 0, 0, 0);
                    break;
                case "black":
                    SendMessage(0xFFFF, 0x112, 0xF170, 2);
                    break;
                case "white":
                    SendMessage(0xFFFF, 0x112, 0xF170, -1);
                    break;
                case "screenshot":
                    // innerCanvas.GetSnapBitmap();
                    break;
                case "q":
                    break;
                default:
                    int temp = int.Parse(data);
                    if (temp < 0)

                    {
                        temp = 0;
                    }
                    else if (temp > mWidth)
                    {
                        temp = mWidth;
                    }
                    clickx = temp;
                    if (taskBar)
                    {
                        mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    }
                    break;
            }
        }
    }
}
