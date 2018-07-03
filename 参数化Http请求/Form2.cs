using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 参数化Http请求
{
    public partial class Form2 : Form
    {
        public delegate void ConfigFinishEventHandler(HttpConfig config);
        public event ConfigFinishEventHandler ConfigFinish;

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            HttpConfig config = new HttpConfig();
            config.Check = radioButton2.Checked;
            textBox1.Enabled = config.Check;
            if(config.Check)
            {
                config.RegStr = textBox1.Text;
            }
            config.TimerEnable = checkBox1.Checked;
            config.ThreadNum = Convert.ToInt32(textBox2.Text);
            config.ProxyEnable = checkBox2.Checked;
            if (config.ProxyEnable)
            {
                config.ProxyAPI = textBox3.Text;
            }
            ConfigFinish.Invoke(config);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = checkBox2.Checked;
        }
    }
}
