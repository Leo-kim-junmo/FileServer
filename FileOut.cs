using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace FileServer
{
    class FileOut
    {
        private string dirPath = "";
        private Socket sock;
        private IPEndPoint ip;
        public FileOut(string str, Socket socket, string path)
        {
            dirPath = path + "\\";
            dirPath = dirPath + str;
            sock = socket;
            ip = (IPEndPoint)sock.RemoteEndPoint;
            Run(dirPath);
        }

        public void Run(string filepath)
        {
            Thread t = new Thread(new ParameterizedThreadStart(sendthread));
            t.IsBackground = true;
            t.Start(filepath);
        }

        private void sendthread(object o)
        {
            try
            {
                string filepath = (string)o;
                string[] p = filepath.Split('\\');
                string filename = p[p.Count() - 1];
                Socket socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(IPAddress.Parse(ip.Address.ToString()), 12345);

                FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

                int fileLength = (int)fileStream.Length;
                byte[] fileBuffer = BitConverter.GetBytes(fileLength);
                socket.Send(fileBuffer);

                int fileNameLength = (int)filename.Length;
                fileBuffer = BitConverter.GetBytes(fileNameLength);
                socket.Send(fileBuffer);

                fileBuffer = Encoding.UTF8.GetBytes(filename);
                socket.Send(fileBuffer);

                int count = fileLength / 1024 + 1;

                BinaryReader reader = new BinaryReader(fileStream);

                for(int i=0; i<count; i++)
                {
                    fileBuffer = reader.ReadBytes(1024);

                    socket.Send(fileBuffer);
                }
                reader.Close();
                socket.Close();
            }
            catch(Exception)
            {
                Console.WriteLine("파일 전송 오류");
            }
        }
    }
}
