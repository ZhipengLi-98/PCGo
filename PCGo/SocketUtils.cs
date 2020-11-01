using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PCGo;

namespace SocketUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class SocketClient
    {
        private string _ip = string.Empty;
        private int _port = 0;
        public Socket _socket = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];

        public SocketClient(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        public SocketClient(int port)
        {
            this._ip = "127.0.0.1";
            this._port = port;
        }

        public void StartClient()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(_ip);
                IPEndPoint endPoint = new IPEndPoint(address, _port);
                _socket.Connect(endPoint);
                Console.WriteLine("Connect to Server {0}", _socket.LocalEndPoint.ToString());
                Thread receiveThread = new Thread(ReceiveMsg);
                receiveThread.Start(_socket);
            }
            catch (Exception e)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                Console.WriteLine("Socket Shutdown");
                Console.WriteLine(e.Message);
            }
        }

        public INetworkCallback callback;

        private void ReceiveMsg(object obj)
        {
            Console.WriteLine("Start Listening");
            Socket receiveSocket = obj as Socket;
            while (true)
            {
                int length = receiveSocket.Receive(buffer);
                if (length == 0)
                {
                    continue;
                }
                // Console.WriteLine("Receive {1} from {0}", receiveSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                if (length == 1 && buffer[0].ToString() == "q")
                {
                    Console.WriteLine("End Connection with {0}", receiveSocket.RemoteEndPoint.ToString());
                    break;
                }
                else
                {
                    string msg = Encoding.UTF8.GetString(buffer, 0, length);

                    if (callback != null)
                    {
                        callback.OnReceive(msg);
                    }
                }
            }
        }

        
    }

    public interface INetworkCallback
    {
        void OnReceive(string data);
    }
}
