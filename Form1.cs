using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileServer
{
    enum State
    {
        STATE,
        FILENAMEZISE,
        FILENAME,
        FILESIZE,
        FILEDOWNLOAD
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }

    class File
    {
        // 상태
        protected State state = State.STATE;
        // 파일 이름
        public byte[] FileName { get; set; }
        // 파일
        public byte[] Binary { get; set; }
    }
}
