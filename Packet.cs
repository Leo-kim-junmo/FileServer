using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    class Packet
    {
        public Packet()
        {

        }
        public string HereIsData(List<string> stringlist)
        {
            string str = null;

            str += "HereIsData" + "\a";
            str += stringlist.Count.ToString() + "@";
            for(int i=0; i<stringlist.Count; i++)
            {
                str += stringlist[i] + "#";
            }
            return str;
        }
    }
}
