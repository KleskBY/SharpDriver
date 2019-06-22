using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orion
{
    public partial class Loader : Form
    {
        public Loader()
        {
            InitializeComponent();
        }
        public static IntPtr Client;
        public static IntPtr Engine;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                Define.EnsureLoaded(textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Define.Unload();
        }
        bool active = false;
        private void button3_Click(object sender, EventArgs e)
        {
            active = true;
            timer1.Start();
        }
 
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (active == true)
            {
                float v = 49;
                int ICVAR = MemDrv.ReadMemory<int>(0x1B42E6C);
                MessageBox.Show(ICVAR.ToString());

                if (ICVAR != 0)
                {
                    MessageBox.Show(ICVAR.ToString());

                    MemDrv.WriteMemory<int>(ICVAR + 0x118, v);
                }
            }
        }

        int gam = Process.GetProcessesByName("Game")[0].Id;
        private void Loader_Load(object sender, EventArgs e)
        {
            MessageBox.Show( gam.ToString());
        }
    }
}
