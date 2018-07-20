using HttpToolsLib;
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
            String proxy_api = textBox3.Text;
            String regstr = textBox1.Text;
            String threadnum = textBox2.Text;
            String xpathstr = textBox4.Text;
            this.Close();
            HttpConfig config = new HttpConfig();
            config.Check = radioButton2.Checked;
            config.CheckXpath = radioButton3.Checked;
            //textBox1.Enabled = config.Check;
            if(config.Check)
            {
                config.RegStr = regstr;
            }
            if (config.CheckXpath)
            {
                config.XpathStr = xpathstr;
            }
            config.TimerEnable = checkBox1.Checked;
            if (!String.IsNullOrEmpty(threadnum))
            {
                try
                {
                    config.ThreadNum = Convert.ToInt32(threadnum);
                }
                catch
                {
                    MessageBox.Show("请输入正确的整数");
                    return;
                }
            }
            config.ProxyEnable = checkBox2.Checked;
            config.LoopGet = checkBox3.Checked;
            if (config.ProxyEnable)
            {
                if (!String.IsNullOrEmpty(proxy_api))
                {
                    config.ProxyAPI = proxy_api;
                }
                else
                {
                    MessageBox.Show("请输入代理ip的api");
                }
            }  
            ConfigFinish.Invoke(config);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.Enabled = checkBox2.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Clear();
            }
            if (!String.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Clear();
            }
            if (!String.IsNullOrEmpty(textBox3.Text))
            {
                textBox3.Clear();
            }
            textBox1.Enabled = false;
            if (radioButton2.Checked||radioButton3.Checked)
            {
                radioButton1.Checked = true;
            }
            if (!checkBox1.Checked)
            {
                checkBox1.Checked = true;
            }
            if (checkBox2.Checked)
            {
                checkBox2.Checked = false;
            }
            if (checkBox3.Checked)
            {
                checkBox3.Checked = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = radioButton2.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = radioButton3.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 form1 = new Form1();
            form1.button1.Enabled = true;
            form1.button5.Enabled = true;
            form1.button8.Enabled = true;
            form1.button10.Enabled = true;
            form1.button11.Enabled = true;
        }
    }
}
