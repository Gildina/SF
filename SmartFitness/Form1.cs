using System;
using System.Windows.Forms;

namespace SmartFitness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            start start = new start();
            start.ShowDialog();
        }
    }
}
