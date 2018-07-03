using ExtractLib;
using HttpToolsLib;
using PControlsLib;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
namespace 参数化Http请求
{
    public partial class Form1 : Form
    {
        #region 全局变量
        String[] httpvarr = { "1.0","1.1"};
        HttpInfo info;
        ConcurrentDictionary<String, String> HeadDic = new ConcurrentDictionary<string, string>();
        String html = String.Empty;
        String JsHtml = String.Empty;

        #endregion

        #region 初始化相关
        public Form1()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            InitializeComponent();
            Exe_Path = Application.ExecutablePath;
            Root_Path = AppDomain.CurrentDomain.BaseDirectory;
            Xml_Path = Path.Combine(Root_Path, Config_Name);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            String Exe_Path = Application.ExecutablePath;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(Exe_Path);
            this.Text = Path.GetFileNameWithoutExtension(Exe_Path) + "V" + info.FileVersion;
            this.MaximizeBox = false;
            toolStripStatusLabel1.Alignment = ToolStripItemAlignment.Right;
            toolStripStatusLabel2.Alignment = ToolStripItemAlignment.Right;
            comboBox1.Items.AddRange(httpvarr);
            comboBox1.SelectedIndex = 0;

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {

        }
        #endregion
 
        #region 请求相关

        public HttpInfo CreateHttp()
        {
            HttpInfo info = new HttpInfo();
            #region 请求头配置
            info = new HttpInfo(textBox1.Text);
            if (!String.IsNullOrEmpty(richTextBox4.Text))
            {
                info.PostData = richTextBox4.Text;
            }
            info.UseSystemProxy = checkBox10.Checked;
            info.Host = textBox17.Text;
            info.IgnoreWebException = checkBox9.Checked;
            info.User_Agent = textBox3.Text;
            info.Referer = textBox4.Text;
            info.ContentType = textBox5.Text;
            info.Accept = textBox6.Text;
            info.AcceptEncoding = textBox7.Text;
            if(!String.IsNullOrEmpty(textBox19.Text))
            {
                var arr = textBox19.Text.Split('|');
                if(arr.Length>0)
                {
                    info.Ip = arr[0];
                }
                if (arr.Length > 1)
                {
                    info.Proxy_UserName = arr[1];
                }
                if (arr.Length > 2)
                {
                    info.Proxy_PassWord = arr[2];
                }
            }

            info.CheckUrl = checkBox7.Checked;
            if(checkBox8.Checked)
            {
                info.Expect100Continue = true;
            }
            String httpversion = Convert.ToString(comboBox1.SelectedItem);
            switch (httpversion)
            {
                case "1.0":
                    info.ProtocolVersion =    ProtocolVersionEnum.V10;
                    break;
                case "1.1":
                    info.ProtocolVersion = ProtocolVersionEnum.V11;
                    break;
            }
            if (!String.IsNullOrEmpty(textBox8.Text))
            {
                info.Encoding = Encoding.GetEncoding(textBox8.Text);
            }
            if (!String.IsNullOrEmpty(textBox11.Text))
            {
                info.Cookie = new CookieString(textBox11.Text, true);
            }
            else
            {
                info.CC = new CookieContainer();
            }
            info.AllowAutoRedirect = checkBox1.Checked;
            info.KeepLive = checkBox2.Checked;
            if (checkBox3.Checked)
            {
                info.Header.Add("X-Requested-With", "XMLHttpRequest");
            }
            foreach (var item in HeadDic)
            {
                info.Header.Add(item.Key, item.Value);
            }
            #endregion

            return info;
            
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            button4_Click(null, null);
            textBox1.Clear();
            richTextBox4.Clear();
            textBox3.Text = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.04";
            textBox4.Clear();
            textBox5.Text = "application/x-www-form-urlencoded; charset=UTF-8";
            textBox6.Text = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            textBox7.Text = "gzip, deflate";
            textBox8.Clear();
            textBox11.Clear();
            checkBox1.Checked = false;
            checkBox2.Checked = true;
            checkBox3.Checked = false;
            textBox9.Clear();
            textBox10.Clear();
            textBox13.Clear();
            textBox14.Text = "0";
            textBox15.Clear();
            textBox16.Clear();
            textBox18.Clear();
            richTextBox1.Clear();
            richTextBox2.Clear();
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            HeadDic.Clear();
        }
        /// <summary>
        /// 常规提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            GC.Collect();

            #region 请求前检查
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请求地址不合法");
                return;
            }
            #endregion

            #region 清空上一次请求内容
            tabPage1.Controls.Clear();
            tabPage4.Controls.Clear();
            tabPage5.Controls.Clear();
            html = String.Empty;
            JsHtml = String.Empty;
            #endregion

            #region 状态显示
            toolStripStatusLabel1.Text = String.Format("以{0}方式请求{1}......", String.IsNullOrEmpty(richTextBox4.Text) ? "GET" : "POST", textBox1.Text);
            
            #endregion

            #region 请求头配置
            info = CreateHttp();
          
            #endregion

     
            //发送请求
            html = HttpMethod.HttpWork(ref info);

            //显示
            RichTextBox box = new RichTextBox();
            textBox18.Text = info.Cookie.ConventToString();
            box.Text = html;
            box.Dock = DockStyle.Fill;
            tabControl1.TabPages[0].Controls.Add(box);


            //渲染
            EWebBrowser browser = new EWebBrowser();
            /* browser.Navigate("about:blank");
             browser.Document.Write(html);*/
            browser.DocumentText = html;
            browser.Dock = DockStyle.Fill;
            tabPage4.Controls.Add(browser);
            RichTextBox box1 = new RichTextBox();
            if (EWebBrowser.WaitWebPageLoad(browser))
            {
                JsHtml = browser.Document.Body.OuterHtml;
            }
            box1.Text = JsHtml;
            box1.Dock = DockStyle.Fill;
            tabPage5.Controls.Add(box1);
            toolStripStatusLabel1.Text = String.Format("以{0}方式请求{1}完毕", String.IsNullOrEmpty(richTextBox4.Text) ? "GET" : "POST", textBox1.Text);
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            GC.Collect();
            tabPage1.Controls.Clear();
            String checkurl = ".+\\..+";
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请求地址不合法");
                toolStripStatusLabel1.Text = "取消图片请求";
                return;
            }
            if (!RegexMethod.CheckRegex(checkurl, textBox1.Text))
            {
                DialogResult result = MessageBox.Show("未检测到常规格式的下载地址,是否继续", "警告", MessageBoxButtons.OKCancel);
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    toolStripStatusLabel1.Text = "取消图片请求";
                    return;
                }
            }
            toolStripStatusLabel1.Text = "下载图片:" + Path.GetFileName(textBox1.Text);
            tabPage1.Controls.Clear();
            tabPage4.Controls.Clear();
            tabPage5.Controls.Clear();
            html = String.Empty;
            JsHtml = String.Empty;


            #region 请求头配置
            info = CreateHttp();

            #endregion

            try
            {
                Image img = HttpMethod.DownPic(info);
                if (img == null)
                {
                    MessageBox.Show("下载失败");
                }
                PictureBox box = new PictureBox();
                box.Dock = DockStyle.Fill;
                box.Image = img;
                tabPage1.Controls.Add(box);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            toolStripStatusLabel1.Text = "下载图片:" + Path.GetFileName(textBox1.Text) + "完毕";
        }
        /// <summary>
        /// 连续请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            this.tabPage1.Controls.Clear();
            Form2 form2 = new Form2();
            form2.ConfigFinish += new  Form2.ConfigFinishEventHandler(ConfigFinishFunc);
            form2.FormClosed += new FormClosedEventHandler(ChildFormClosed);
            this.Enabled = false;
            form2.Show();
        }

        private void ConfigFinishFunc(HttpConfig config)
        {
            throw new NotImplementedException();
        }

        private void ChildFormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
        }

        /// <summary>
        /// 测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            //  String Url = "https://www.starwoodhotels.com/preferredguest/account/sign_in.html";
            //  String Cookie = "TLTSID=8833BA6621AF10219EEA86079AFF7586; list_view=G; _sdsat_landing_page=https://www.starwoodhotels.com/preferredguest/index.html|1492242343014; _sdsat_session_count=1; s_vnum=1493568000255%26vn%3D1; CoreID6=18390397374820365930416; BTgroupEMEA=Turn; ttc=1492242349269; _sdsat_traffic_source=https://www.starwoodhotels.com/preferredguest/index.html; P=invalid; autoLogin=invalid; s_lastvisit=1492246538366; JSESSIONID=0000pkk1DHBu_7J9jhyZIBVyE8E:16jpvppou; _sdsat_signin=N; TLTHID=52171D7821BA1021E3FB83822C9E232A; LOCALECOOKIE=zh_CN; mbox=PC#1492242342122-493724.24_4#1500022977|session#1492244733325-622058#1492248837|check#true#1492247037; aOmIj1jm=uniqueStateKey%3DAGvl2XBbAQAAXL7ODaEYd5-Di7Buy1ME-zgNf5rjL5PzAoiJ46GPc7KZsoKz%26DifAThqJQ%3DRqEs81hs1PwY1DZj81pH7yc-NKqdwW1NNNN; _sdsat_lt_pages_viewed=9; _sdsat_pages_viewed=9; s_cc=true; __ampt=469782191098451267; __amps=||1|true; _ga=GA1.2.1338517566.1492242350; s_tps=146; s_pvs=292; __ampa=423578561174891045; __ampb=423578571044188354; s_fid=7FDFE3E48A37458E-089C00DEAD6EF8FD; s_nr=1492247124690-New; SPG_nr=1492247124690-New; s_invisit=true; gpv_pn=zh_CN%3ASPG%3AnoID%3ALogin%3AError; s_visit=1; tp=1583; s_ppv=-%2C60%2C60%2C944; s_vi=[CS]v1|2C78E7D7854889ED-40000104E0000260[CE]";
            //  String Postdata = "ctok=5882424347264608841&actualPath=%2Fpreferredguest%2Faccount%2Fsign_in.html&stayId=&successPath=https%3A%2F%2Fwww.starwoodhotels.com%2Fpreferredguest%2Faccount%2Fprofile%2FupdateSecurityInformation.html&login=a2667055388&passwordIE=Password&password=a123456&X-uniqueStateKey=AGvl2XBbAQAAXL7ODaEYd5-Di7Buy1ME-zgNf5rjL5PzAoiJ46GPc7KZsoKz&X-DifAThqJQ=RW3o1Bmy1Yhf1PUm_zGbZZU27Hp4OBZ0n7Ze7x4irZw_a2pf1Pr41XelnBZlnHpHLPHghprF7Da-a3tghPZj7YZ0hYtW13d4apmgO6pxL2GlazwPZz4mhPZ0hQ4xn2H41YrWO7cta7a4O6rzk7d4yDHW1Pe4aimyZ3tH1Yr4aQGya7mHa2GlazGHO2t41Xayh2ten7UBhBmjaDZxj2GWhP46L7rf1XW4k3r41PGWOQa6OBwsL2yBL6ZxhBw0ABGWO22M8XeVa7tPOYteL2Gla1hyn2h0L2AygCeJODHcODHVOBZxahWJODHcODGxa2GxUBwWaBZJr7a4O6rmOPUnaBwebDw0hBZ0hiAfL2r4aiZDa2GxzYrW16UuaBwe_2Gxa7tWLYrghP79aBweUBwWaB40aCAja7mHa7pxzYrW16Up1PZy1Bw01DZUhBmjh953L6tfhYp41XrH1DZjTXGs1PwY1DZjzBAHaD401CGln3tfO2Z7a2tyhBwja1Hl1PZWhBZmOBZea2GxjPrWhBZUh3tgOPFtn2H6zYtl1DZxjDgWhPmUh7dVOYtxy2H4aB4WrBZDn2p4_2U_1BZjaPwjO2m0LDZUh7dVOYtxy7djOYd416rGbDW4LDfu13tf1BZjh342L2AHa7Ku1Dpja2Z0UYa416tgaB791YrWLDez1Pmla1pH7yc9kBWjzYZV1Bwjh9K7nBmyrBZPL7ZXhitjOYhya7toa2AVa75rnBmyZD4Ja7agOPZbO3Z6n2%3D31BAHaD401MK5nBmyZBwUOYZjLD791DwH1Pp4_BmynQgyh3tgOPhoL7p8g1WJODpHO2Z0hQAJODpHO2Z0hitfa36ycFQuCVjfYErlaBpFL7pJnPaXL7pHhBwVaPWDLHgQO2pPOmRyNfQiHZwFhDZsa3tghPZj7Ypl1P4VhmwPOXpWOByQLDWW1PmlhBZjzDZxADp8L7tya7U9LDwe1BmxU2wJa1AJODpHO2Z0hiHfaB7uaDZxr2A4O2Z0hitG_2U2aDZxr2A4O2Z0h3pEkzpXL7pyUPmea1H8L7pdh3rjn2tHhBZyAP4eL2h41CaXL74416Kp17Z4164Ua2A4LYrf1vF5LDw0hBZ-hiH4O67tn2G0a7toZiHQj240OPZjZBZ-hpreOYg_a7mHa7pxr6ZXOmpl1PZ4Oemja7mHa7pxr6ZXOmpl1PZ4OX4Ya2tJ1P4Da757hDZsnD4xzPZAh2ZyhiaHOBAULYt4a2MWjZp417Z4O6rHOFbPyzmlhB4DaZWKLPg4LYUKb7dVOBZbL74Ua7pyn2w0AiagOB7t_2Gxuimj1PmGxiHHhBmxn2w0UDtya7tDa75QUPwxn2agLDmxn2w0jHdH1DWpL2GWaDZjymp8L7t4amhf1Pe41XgzOYZlniZDa2Gxy4WiODHWn2G_a7mHa7pxCV5ydQWF1BWWO6rfOhdWaBrmhPZ0hiAg1Yr4OPZjjDmxhBmlniZDa2GxjDpWOBAbnBm0hBwejDpja2mxaZdf13ZVjDr4hBmlniZDa2Gxy2rg1YdWhBp8r7a4O6Uma7a4O6Uyc14Pn7t4r7a4O6UBa6tWO2ZyxBh4hipfO7dHhBZJzYrGOB7paDAfLPmXzYrf1Pm6a1AXODpWOmpxOYtWaD7zO2wMz4rczBZ41JpfOPG4LYrgOD%3DLO2wMzPZAh2Zyhim0n2HWhB4fOJajL2H4AYd8L2GxODY51BwyhiH41YpWaD771PZ6n7pxa7tb1PwxODpfOiWWOPrXa75U1PZeOYa4r7a4O6rQn7pxa2G41eZja7mHa7pxb2GgO2mxn2w0r6tWO27u1DZy1D4fO4pxOYtWaD731D4Ja2tW1ehYa2tTn7r_Zipba2ZjbDw0OPZlhB4fOeeYa2tTn7r_a7mHa7pxb2GgO2mxn2w0r6tWO27FhDZsnD4xzPZyODADazAfLDmXrP4XaZpG1Yr4OZZ_U933hP4s1Pmxan7ycXeXODpWhB4fOPtW1fQQA6pl1PZ4OXhxODwXLPmjgCAP1PmeazZXa2H4O6U5n2G0a7toa246n3U9n2G0a7t7n2rxnQefh7r41JW4n2h8hQgfh7r414hga3r8AYpl1PZ4O4l31Dpja2Z02ny5L7dVbDwJazGWO273L7dVUPmea1gW13d2a7tyn2w0jPrfUPwxZ3tWLDfUnBmja3hW1PZcODGlh7tja2Glk1GeL7WzOYZlnmdfn2Gx1CWVOBmxaPwjO1hV1PwJh2pxj6djODrHLYrUh25th7p41Jm6a2GxA6a4OPrf1X4Da2GJOYtUh29PjDmDL24X_BZgaDWxjPmDL24XZD4JhBl9LDwXOYtia7dxnQa8a246n3U91B4-a2Aia7dxnQZYn2rxn9O5aBZyLYtg13rgOD%3D3O2Zy1Dm6aFQbVDGHO1ZyhBmlnCgyhBmlnYrjL2p4gQGTa74sODmjaiZDa2Gx1CeeOYZyazZDa2Gx1wmf1P44O6rWhB4fOJZDa2Gx1CexOYZlniZDa2Gx1MK3nDZGrBwYOXWTa74b1PZy1CZTa74Z19fBL2Ax_DZGADpx1PA5a76ydRQmADe4kzpfaB7yEQhea7rW_DZGCV6o1DWga6r5a76yERQBg1a8n2rJa2%3DEn2UyiQhxL2huL2H4j3rG1BZdh3rjgQgeOYZyazpXn2pTj2Hfh7p4rBwYOX4eOYZyazHfhP73O2wH1DZZ19fycRQ3CV7yEQtf2Qtf2FQtCVfydXm-V7P8CVRydC4PL2e4r7a4O6UyERQBy6Z0h3tH1Yr4aiZDa2GxV7ldkn6oL7a6b2AVnB33L7a6bPZxL1WWhPh3L2HeL1eWhPhtO6r416aWOpr0h2HK1P44O6rWhB4fOJZDa2Gx1CeyhBria7adO3d8L1gyhBria7aEa7rWjYpxair4hJhWO2HWy6pxair4hJ40hBZjhPmX8CWxOYZlniZ0aQ4xOYZlniHfhP79hBwHLDWUhBmjhKqNSN36NNNd2YcaY_f6NNNd2YcaYrK6NNNd2YcaY5F6NNNd2YcaY5F6NNNd2YcaHvF6NNNd2Ycay4T6NNNd2YcaHg%3D-NfqcSNUq-n7vac5py08_ZT3_MP1B_GYvLmXhX0LKOGEF-Fqm-NcvZKlqOFdpOD-qb7Z6ocNDociGpczqQc1MQULMQcNqrxHz9yN-QcNq9u_-Tk2OfknqWS23W0n7e0P7eEJN-kcPOgjiQS3-dfJ3cDTGENcGEbcGElSiux71Sb0NSNy-cktcXinb3iEqM7xNM7xN-FquAxHfkP4XOB3oUPZx1DpW1B7Vp_-VoEW7n2GJOYhyoiGzocL0QyXqZxw7plbgoimV1BA4ZDZs_D4x5yzypj-ypsN8_xWzUzVXoBAgnDzqrDZlnDCgoip81Pwea_CHpj-V5loGuc10QUQyompWaPmjn_CHQy10QyLN-qqNAZhgOlQjAzh4LDefjcoVQcQVQUNYCiHfkP4XOBifp_-VoEW7n2GJOYhyoiGzocL0QyXqZxw7plbgoimV1BA4ZDZs_D4x5yzypj-ypsN8_xWzUzVXoBAgnDzqrDZlnDCgoip81Pwea_CHpj-V5loGuc10QUQyompWaPmjn_CHQy10QyLNjxhfODhXa_dtOPQ0VKqKbRtBJdWiBWWBJu3-dkNNMPg5A77-iuKVbDm0OPwxo3t4L2bq13tf1BZjh3JqtyN6oBwPoBGHOBVNjZrG1BZm16tf10KVZ34VazZj1PwjusdcL2G0OYbq1PZWaEdV1PwVa7txk_N6QE1qODLqO6ZXON8qoENqL7bqUDtva2px56p4h3ZVoEWZzJVgEsNqoEdWhEd42_GPLjN8ZZtQ9b8qoENqL7bqa2x0a7bq9mZ_UEJ9oENqoBmxoiwsnPZlhEGVhsdOL7Qqn2Gghmxq9mZ_UEJ9oENqoBmxomZ_UNclwsqNNNNNNu3-iFq_gRqU-uNd-N3W-NcqSdUqCVcyNKQdCVc4NlUUSdKq-N3qNk3qN0c-mucyNKQNCV3yNuzEpW7-iScqNkNd-kNi-Kqz-KQNCVcyNFQNGboDAFqU-uNd-N3W-NOqSdUqCVcyNKQdCVc4Nlk5SdKq-N3qNk3qEuc-mucyNKQNCV3yNuzEuBU-iScqNkNd-kN9-Kqz-KQNCVcyNFQNGboGmFqU-uNd-N3W-NyqSdUqCVcyNKQdCVc4Nl6ZgRqU-uNc-N3W-N3qSdUqCVcyNKQdCVc4NlUUSdKq-NKqNk3qNSc-mucyNKQNCV3yNuzEpW7-iScqNSNd-kNm-Kqz-KQNCVcyNFQNGboDAFqU-uNc-N3W-NFqSdUqCVcyNKQdCVc4Nlk5SdKq-NKqNk3qEkc-mucyNKQNCV3yNuzEuBU-iScqNSNd-kN5-Kqz-KQNCVcyNFQNGboGmFqU-uNc-N3W-NYqSdUqCVcyNKQdCVc4Nl6ZgRqU-uNE-N3W-NcqSdUqCVcyNKQdCVc4NlZ9SdKq-N5qNk3qN0c-mucyNKQNCV3yNuzEpPl-iScqN0Nd-kNi-Kqz-KQNCVcyNFQNGboYcfqU-uNE-N3W-NOqSdUqCVcyNKQdCVc4NlFLSdKq-N5qNk3qEuc-mucyNKQNCV3yNuzEu5c-iScqN0Nd-kN9-Kqz-KQNCVcyNFQNGboG2KqU-uNE-N3W-NyqSdUqCVcyNKQdCVc4Nl8VSd2WSdLNNScW%3Di3%3DbNNONfqz-KQcVKQdCVT4NlSdbVrENn3-mqNd-u3XbFeNNdXESdUqCVKNCV3yE0zEKjacdiod0Rq7-qUl-0zEKOFlbAmdyfq7-qUl-0zEKOflbVGdxFq7-qUl-0zEK1KlbVAdxfq7-qUl-0zEK1flbVgdHFq7-qUl-0zEK1flbVgdHRq7-qUl-0zEKWYlbVWdDKq7-qUl-0zEKsOlbVWdDFq7-qUl-0zEKsYlbVhdDfq7-qUl-0zEKl7lbVhdDRq7-qUl-0zEKlylbVhdYKq7-qUl-0zEKJylbVhdYfq7-qUl-0zEK4flbVhdYRq7-qUl-0zEKPflbVhd-Kq7-qUl-0zEK6TlbVhd-Fq7-qUl-0zEK6TlbVhd-Rq7-qUl-0zEK8TlbVhdGFq7-qUl-0zEKgOlbVhdMFq7-qUl-0zEKgYlbVhdMRq7-qUl-0zEKv7lbVhdCKq7-qUl-0zEKv%3DlbVhdwKq7-qUl-0zEKT7lbVhdwRq7-qUl-0zEKTYlbVhdSFq7-qUl-0zEKX3lbVhdRKq7-qUl-0zEKX3lbVZdRfq7-qUl-0zEKe3lbVrdRRq7-qUl-0zEKe3lbVrENKq7-qUl-0zEK03lbVrENn3-mqNE-u3XbFeNNdXESdUqCVKNCV3yE0zEKYGcdiodSdls-0oNNbNNNNc-BnEq8N";
            //  HttpWin.HttpWin http = new HttpWin.HttpWin();
            //  Dictionary<String,String> dic = new Dictionary<string,string>();
            //  dic.Add("Cookie",Cookie);
            //  dic.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            ////  dic.Add("Accept-Encoding", "gzip, deflate, sdch, br");
            //  dic.Add("Accept-Language", "zh-CN,zh;q=0.8");
            //  dic.Add("Connection", "keep-alive");
            //  dic.Add("Cache-Control", "max-age=0");
            //  dic.Add("Host", "www.starwoodhotels.com");
            //  dic.Add("Referer", "https://www.starwoodhotels.com/preferredguest/account/sign_in.html");
            //  dic.Add("Upgrade-Insecure-Request", "1");
            //  dic.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");

            //  RichTextBox box1 = new RichTextBox();
            //  String html = http.GetHtml(Url,dic,"POST",Postdata);

            //  box1.Text = html;
            //  box1.Dock = DockStyle.Fill;
            //  tabControl1.TabPages[0].Controls.Add(box1);
            //  //Console.WriteLine(html);
            HttpHelper helper = new HttpHelper();
            HttpItem item = new HttpItem();
            item.URL = "https://www.ihg.com/gs-json/cn/zh/login";
            item.Method = "Post";
            item.ContentType = "application/json";
            item.Cookie = "datacenternode=us-ca-sjcd1; X-IHG-SRV=sjcd1plb2cwb021; BlueStripe.PVN=5460000017ad;";
            item.Header.Add("Accept-Encoding", "gzip,  deflate,  br");
            item.Postdata = "{\"username\":\"741031442\",\"password\":\"1111\",\"cookieFlag\":true}";

            HttpResult result = helper.GetHtml(item);

            Console.WriteLine(result.Html);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            GC.Collect();

            #region 请求头配置
            info = CreateHttp();
            #endregion

            tabPage1.Controls.Clear();
            String checkurl = ".+\\..+";
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请求地址不合法");
                toolStripStatusLabel1.Text = "取消下载请求";
                return;
            }
            if (!RegexMethod.CheckRegex(checkurl, textBox1.Text))
            {
                DialogResult result = MessageBox.Show("未检测到常规格式的下载地址,是否继续", "警告", MessageBoxButtons.OKCancel);
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    toolStripStatusLabel1.Text = "取消下载请求";
                    return;
                }
            }
            SaveFileDialog sf = new SaveFileDialog();
            var arr = textBox1.Text.Split('/');
            String filename = arr[arr.Length - 1];
            String type = filename.Split('.')[1];
            String name = filename.Split('.')[0];
            String filter = String.Format("{0}(*.{0})|*.{0}|所有文件(*.*)|*.*", type, type, type);
            sf.Filter = filter;//可以保存的格式
            sf.FileName = arr[arr.Length - 1];
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (HttpMethod.DownLoadFile_ABPath(info, sf.FileName))
                {
                    MessageBox.Show("下载成功");
                }
                else
                {
                    MessageBox.Show("下载失败");
                }
            }
            toolStripStatusLabel1.Text = "完毕";
        }
        #endregion

        #region 自定义头配置
        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox9.Text) || String.IsNullOrEmpty(textBox10.Text))
            {
                MessageBox.Show("参数不完整");
                return;
            }
            String Key = textBox9.Text;
            String Value = textBox10.Text;
            AddItem(Key, Value);
            toolStripStatusLabel1.Text = "新增自定义请求头:" + textBox9.Text + "=" + textBox10.Text;
        }

        private void AddItem(string Key, string Value)
        {
            if (HeadDic.TryAdd(Key, Value))
            {
                ListViewItem item = new ListViewItem();
                item.Text = Key;
                item.SubItems.Add(Value);
                listViewNF1.Items.Add(item);
                listViewNF1.Items[listViewNF1.Items.Count - 1].EnsureVisible();
            }
            else
            {
                MessageBox.Show("存在相同的Key");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("请输入需要删除的Key");
            }
            String temp = String.Empty;
            if (HeadDic.TryRemove(textBox9.Text, out temp))
            {
                RemoveListView();
                FillListView();
            }
            toolStripStatusLabel1.Text = "删除自定义请求头:" + textBox9.Text;
        }
        private void RemoveListView()
        {
            Application.DoEvents();
            listViewNF1.BeginUpdate();
            while (listViewNF1.Items.Count > 0)
            {
                listViewNF1.Items.RemoveAt(listViewNF1.Items.Count - 1);
            }
            listViewNF1.EndUpdate();
            Application.DoEvents();
        }
        private void FillListView()
        {
            foreach (var item in HeadDic)
            {
                ListViewItem listitem = new ListViewItem();
                listitem.Text = item.Key;
                listitem.SubItems.Add(item.Value);
                listViewNF1.Items.Add(listitem);
            }
            toolStripStatusLabel1.Text = "清空自定义请求头";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RemoveListView();
            HeadDic.Clear();
        }
        #endregion

        #region 抽取相关
        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(html))
            {
                MessageBox.Show("网页源码为空");
                return;
            }
            if (String.IsNullOrEmpty(textBox13.Text))
            {
                MessageBox.Show("正则表达式为空");
                return;
            }
            if (String.IsNullOrEmpty(textBox14.Text))
            {
                MessageBox.Show("层数为空,将使用默认值0");
                textBox14.Text = "0";
            }
            int lay = Convert.ToInt32(textBox14.Text);
            if (checkBox4.Checked)
            {
                try
                {
                    var list = RegexMethod.GetMutResult(textBox13.Text, html, lay);
                    richTextBox1.Text = String.Join("\n", list);
                }
                catch
                {
                    richTextBox1.Text = "未找到匹配结果";
                }
            }
            else
            {
                try
                {
                    richTextBox1.Text = RegexMethod.GetSingleResult(textBox13.Text, html, lay);
                }
                catch
                {
                    richTextBox1.Text = "未找到匹配结果";
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox15.Text))
            {
                MessageBox.Show("Xpath为空");
                return;
            }
            richTextBox2.Clear();
            String text = checkBox6.Checked ? JsHtml : html;
            //标识匹配或抽取
            int flag = 1;
            if (radioButton1.Checked)
            {
                flag = 0;
            }
            else if (radioButton2.Checked)
            {
                flag = 1;
            }
            if (checkBox5.Checked)
            {
                try
                {
                    var list = String.IsNullOrEmpty(textBox16.Text) ? XpathMethod.GetMutResult(textBox15.Text, text, flag) : XpathMethod.GetMutResult(textBox15.Text, text, textBox16.Text);
                    richTextBox2.Text = String.Join("\n", list);
                }
                catch
                {
                    richTextBox2.Text = "抽取失败";
                }
            }
            else
            {
                try
                {
                    richTextBox2.Text = String.IsNullOrEmpty(textBox16.Text) ? XpathMethod.GetSingleResult(textBox15.Text, text, flag) : XpathMethod.GetSingleResult(textBox15.Text, text, textBox16.Text);
                }
                catch
                {
                    richTextBox2.Text = "抽取失败";
                }
            }
        }


        #endregion

        #region 更新相关  更新服务器已迁移

        public string Exe_Path { get; set; }

        public string Root_Path { get; set; }

        public string Xml_Path { get; set; }

        public string Config_Name = "config_201708032235.config";

        public String Root_Id = "201708032235";
        #endregion

        #region 底部状态栏字段显示
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                groupBox13.Height = 0;
            }
            else
            {
                groupBox13.Height = 220;

            }
            if (tabControl1.SelectedIndex == 3)
            {
                richTextBox3.Clear();
                richTextBox3.AppendText("Method:" + (String.IsNullOrEmpty(richTextBox4.Text) ? "GET" : "POST") + "\n");
                richTextBox3.AppendText("RequesrUrl:" + textBox1.Text + "\n");
                richTextBox3.AppendText("Postdata:" + richTextBox4.Text + "\n");
                richTextBox3.AppendText("UA:" + textBox3.Text + "\n");
                richTextBox3.AppendText("Referer:" + textBox4.Text + "\n");
                richTextBox3.AppendText("Content-Tyep:" + textBox5.Text + "\n");
                richTextBox3.AppendText("Accept:" + textBox6.Text + "\n");
                richTextBox3.AppendText("Accept-Encoding:" + textBox7.Text + "\n");
                richTextBox3.AppendText("Encoding:" + textBox8.Text + "\n");
                if (info != null)
                {
                    richTextBox3.AppendText("Cookie:" + info.Cookie.ConventToString() + "\n");
                    richTextBox3.AppendText("Host:" + info.Host + "\n");
                    richTextBox3.AppendText("Proxy:" + info.Ip + "\n");
                    richTextBox3.AppendText("AllowAutoRedirect:" + info.AllowAutoRedirect + "\n");
                    richTextBox3.AppendText("Keep_Alive:" + info.KeepLive + "\n");
                    richTextBox3.AppendText("XMLHttpRequest:" + checkBox3.Checked + "\n");
                    richTextBox3.AppendText("Expect100Continue:" + info.Expect100Continue + "\n");
                    richTextBox3.AppendText("ContentLength:" + info.ContentLength + "\n");
                    richTextBox3.AppendText("Timeout:" + info.Timeout + "\n");
                    richTextBox3.AppendText("WriteTimeout:" + info.ReadWriteTimeout + "\n");
                    richTextBox3.AppendText("AllowWriteStreamBuffering:" + info.AllowWriteStreamBuffering + "\n");
                    richTextBox3.AppendText("ConnectionLimit:" + HttpInfo.ConnectionLimit + "\n");
                    richTextBox3.AppendText("ProtocolVersion:" + info.ProtocolVersion.ToString() + "\n");
                }
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置请求地址";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置PostData";
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置UA";
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置来源页";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置Content-Type";
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置Accept";
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置Accept-Encoding";
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置返回编码";
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置Cookie";
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置域名";
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "设置代理IP";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = checkBox1.Checked ? "允许重定向" : "禁止重定向";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = checkBox2.Checked ? "设置保持连接" : "不保持连接";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = checkBox3.Checked ? "XML-Request开" : "XML-Request关";
        }
        #endregion

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void textBox19_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.textBox19.Text))
            {
                this.textBox19.Text = "IP地址:端口号|代理IP用户名|代理IP密码(用户名和密码选填)";
                this.textBox19.SelectAll();
            }
        }
    }
}
