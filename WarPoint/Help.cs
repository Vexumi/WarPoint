using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WarPoint
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
            InitText();
        }

        public void InitText()
        {
            StreamReader sr = new StreamReader(@"instructions.txt");
            richTextBox1.Text = sr.ReadToEnd();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
