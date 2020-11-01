using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using SocketUtils;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace PCGo
{
    public class Client
    {
        #region Private Field
        private Int32 port = 4000;
        private String IP = "183.173.77.113";
        private TcpClient client;
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor. Try to connect to a TCP server. 
        /// </summary>
        public Client(string _IP, int _port)
        {
            try
            {
                IP = _IP;
                port = _port;
                client = new TcpClient(IP, port);
                Console.WriteLine("Connected to a server!");
                Thread thread = new Thread(Receive);
                thread.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Send message to TCP server
        /// Always add a '\n' character on tail to avoid blocking.
        /// </summary>
        /// <param name="msg"> message to send </param>
        public void Send(String msg)
        {
            if (!client.Connected)
                return;
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg + "\n");
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Receive message from TCP server. 
        /// </summary>
        /// 
        public void Receive()
        {
            while (true)
            {
                if (!client.Connected)
                    continue;

                try
                {
                    var data = new Byte[1024];
                    String response = String.Empty;
                    NetworkStream stream = client.GetStream();
                    String header = System.Text.Encoding.UTF8.GetString(data, 0, stream.Read(data, 0, data.Length));
                    int len = int.Parse(header.Split(',')[0]);
                    var data2 = new Byte[1024];
                     Console.WriteLine(header);
                    // Console.WriteLine(name);
                    string name = header.Split(',')[1];
                    //var data2 = new Byte[int.Parse(len)];
                    //stream.Read(data2, 0, data2.Length);
                    string replacement = Regex.Replace(name, @"\t|\n|\r", "");
                    string path = "D:\\\\" + replacement;
                    Console.WriteLine(path);
                    FileStream fs = new FileStream(path, FileMode.Create);
                    //fs.Write(data2);
                    int cnt = 0;
                    int recv;
                    while (cnt < len)
                    {
                        if (len - cnt <= data2.Length)
                            recv = stream.Read(data2, 0, len - cnt);
                        else
                            recv = stream.Read(data2, 0, data2.Length);
                        fs.Write(data2, 0, recv);
                        cnt += recv;
                        Console.WriteLine(cnt);
                    }
                    fs.Dispose();
                    Console.WriteLine("Receive Complete");
                    Console.WriteLine(response);
                    // TaskManager.getInstance().Execute(response);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Close the network connection.
        /// </summary>
        public void Close()
        {
            client.Close();
        }
        #endregion
    }
}
