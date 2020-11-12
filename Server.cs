using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics.Tracing;

namespace FileServer
{
    class Server
    {
        private Socket server;
        private Thread Thread;
        private Control con;

        private List<Socket> socklist = new List<Socket>();

        string dirPath = "";

        public Server(Control conn)
        {
            con = conn;
        }

        public void ServerStart(int port)
        {
            SocketInit(port);
            Thread = new Thread(new ThreadStart(AcceptThread));
            Thread.Start();
        }

        private void SocketInit(int port)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipep);
            server.Listen(20);

            Console.WriteLine("서버시작");

            using (var dialog = new FolderBrowserDialog())
            {
                if(dialog.ShowDialog()==DialogResult.OK)
                {
                    dirPath = dialog.SelectedPath;
                }
            }
        }

        private void AcceptThread()
        {
            while(true)
            {
                // 1. 클라이언트 수신
                Socket client = server.Accept();

                IPEndPoint ip = (IPEndPoint)client.RemoteEndPoint;
                Console.WriteLine("{0}주소, {1}포트 접속", ip.Address, ip.Port);

                // 2. 소켓 리스트에 저장
                socklist.Add(client);

                // 3. 통신 쓰레드
                Thread thread = new Thread(new ParameterizedThreadStart(WorkThread));
                thread.Start(client);
            }
        }

        private void WorkThread(object obj)
        {
            Socket sock = (Socket)obj;
            int sendtype = 0;
            while(true)
            {
                byte[] msg = ReceiveData(sock);
                if (msg == null)
                    break;

                byte[] str = con.RecvData(msg, ref sendtype, sock, dirPath);
                if(str!=null)
                {
                    if(sendtype==0)
                    {
                        foreach(Socket s in socklist)
                        {
                            SendData(s, str);
                        }
                    }
                    else if(sendtype==1)
                    {
                        SendData(sock, str);
                    }
                }
            }

            IPEndPoint ip = (IPEndPoint)sock.RemoteEndPoint;
            Console.WriteLine("{0}주소, {1}퐅, 접속 종료", ip.Address, ip.Port);

            socklist.Remove(sock);
        }

        private void SendData(Socket sock, byte[] data)
        {
            try
            {
                int total = 0;
                int size = data.Length;
                int left_data = size;
                int send_data = 0;

                byte[] data_size = new byte[4];
                data_size = BitConverter.GetBytes(size);
                send_data = sock.Send(data_size);

                while(total<size)
                {
                    send_data = sock.Send(data, total, left_data, SocketFlags.None);
                    total += send_data;
                    left_data -= send_data;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private byte[] ReceiveData(Socket sock)
        {
            try
            {
                int total = 0;
                int size = 0;
                int left_data = 0;
                int recv_data = 0;

                byte[] data_size = new byte[4];
                recv_data = sock.Receive(data_size, 0, 4, SocketFlags.None);
                size = BitConverter.ToInt32(data_size, 0);

                left_data = size;
                byte[] data = new byte[size];

                while(total<size)
                {
                    recv_data = sock.Receive(data, total, left_data, 0);
                    if (recv_data == 0) break;
                    total += recv_data;
                    left_data -= recv_data;
                }

                return data;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
