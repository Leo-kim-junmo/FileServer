using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace FileServer
{
    class Control
    {
        private Server server; // 서버 클래스
        private Packet packet; // 패킷화 클래스
        private FileOut fileout; // 파일 전송 클래스

        private List<string> itemlist = new List<string>(); // 파일 목록 리스트

        string dirPath = ""; // 파일 경로

        public Control()
        {
            server = new Server(this);
            server.ServerStart(12345);
        }

        public byte[] RecvData(byte[] msg, ref int type, Socket sock, string path)
        {
            string str = Encoding.Default.GetString(msg);
            string[] token = str.Split('\a');
            switch(token[0].Trim())
            {
                case "GiveMeData":
                    dirPath = path;
                    type = 1;
                    str = FileScanSend();
                    Console.WriteLine("파일스캔");
                    break;
                case "GiveMeFile":
                    type = 1;
                    fileout = new FileOut(token[1], sock, dirPath);
                    return null;
            }
            return Encoding.Default.GetBytes(str);
        }

        public string FileScanSend()
        {
            itemlist.Clear();
            if(System.IO.Directory.Exists(dirPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);
                foreach(var item in di.GetFiles())
                {
                    itemlist.Add(item.Name);
                }
            }
            packet = new Packet();
            return packet.HereIsData(itemlist);
        }
    }
}
