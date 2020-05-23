using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartFitness
{
    public partial class start : Form
    {
        ttsRead myTTS = new ttsRead();
        private Boolean ifClick = false;
        public start()
        {
            InitializeComponent();
            MycomboBox.Items.Add("哑铃飞鸟");
            MycomboBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // here is the tts test
            /*TTS.TTS myTTS = new TTS.TTS();
            myTTS.Read("hello world");
            
            ttsRead myTTS = new ttsRead();
            myTTS.Read("hello world");
            */

            //myTTS.Read("hello world, my name is your father");

            //MessageBox.Show("the item is " + MycomboBox.GetItemText(MycomboBox.Items[MycomboBox.SelectedIndex]) + MycomboBox.SelectedIndex);
            //myTTS.Read(" my name is your father hello world");
            //myTTS.Read("hello world");

            //this.Invoke(new EventHandler(delegate { iFitTest3.Program.Move();}));

            Task.Run(() => { iFitTest3.Program.Move(); });
            ifClick = true;
            //           Thread thread = new Thread(new ThreadStart(iFitTest3.Program.Move));
            //            thread.Start();
            //iFitTest3.Program.Move();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ifClick)
            {

                int tmp = iFitTest3.Program.getStatus();
                switch (tmp)
                {
                    case 0:
                        button2.Text = "多次训练";
                        iFitTest3.Program.setMore();
                        break;
                    case 3:
                        button2.Text = "单次锻炼";
                        iFitTest3.Program.setONE();
                        break;
                    case 1:
                        button2.Text = "停止";
                        iFitTest3.Program.setNone();
                        break;
                }
            }
        }
    }
}
