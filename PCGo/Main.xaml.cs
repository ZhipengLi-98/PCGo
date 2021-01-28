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
       

        public int clickx, clicky;
        POINT lastCursorPos;
        private int mWidth = 3840;
        private int mHeight = 2160;

        private IntPtr hwnd;

        public MainWindow()
        {
            InitializeComponent();
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
            anClient = new Client("183.173.206.238", 4000);

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
                    WinAPI.GetCursorPos(out lastCursorPos);
                    clickx = mWidth / 3;
                    clicky = mHeight - 30;
                    taskBar = true;
                    WinAPI.mouse_event(WinAPI.MOUSEEVENTF_MOVE | WinAPI.MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    break;
                case "endmisison":
                    Console.WriteLine("End Misison");
                    taskBar = false;
                    clickx = lastCursorPos.X;
                    clicky = lastCursorPos.Y;
                    WinAPI.mouse_event(WinAPI.MOUSEEVENTF_MOVE | WinAPI.MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    break;
                case "volup":
                    WinAPI.keybd_event(WinAPI.KEYEVENTF_VOLUEMEUP, 0, 0, 0);
                    break;
                case "voldown":
                    WinAPI.keybd_event(WinAPI.KEYEVENTF_VOLUEMEDOWN, 0, 0, 0);
                    break;
                case "black":
                    WinAPI.SendMessage(0xFFFF, 0x112, 0xF170, 2);
                    break;
                case "white":
                    WinAPI.SendMessage(0xFFFF, 0x112, 0xF170, -1);
                    break;
                case "close":
                    Console.WriteLine(WindowManager.GetWindowTitle(WindowManager.FindTopmostWindow()));
                    break;
                case "min":
                    WindowManager.MinimizeWindow(WindowManager.FindTopmostWindow());
                    break;
                case "max":
                    WindowManager.MaximizeWindow(WindowManager.FindTopmostWindow());
                    break;
                case "restore":
                    WindowManager.RestoreWindow(WindowManager.FindTopmostWindow());
                    break;
                case "screenshot":
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
                        WinAPI.mouse_event(WinAPI.MOUSEEVENTF_MOVE | WinAPI.MOUSEEVENTF_ABSOLUTE, clickx * 65535 / mWidth, clicky * 65535 / mHeight, 0, 0);
                    }
                    break;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Console.WriteLine("OnClosed");
            pythonClient.close();
        }
    }
}
