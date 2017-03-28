using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WebKit;

namespace YashiWebpageScreenshot
{
    public partial class Form1 : Form
    {
        string[] args = null;
        WebKitBrowser webKitBrowser = null;
        WebBrowser ieBrowser = null;
        string purl = "https://github.com/cxchope/YashiWebpageScreenshot";
        string pout = "WebpageScreenshot.png";
        int ptime = 1;
        int ntime = 0;
        int pwidth = 1024;
        int pheight = 768;
        bool usewebkit = true;
        int scrollto = 0;

        public Form1()
        {
            InitializeComponent();
            this.Text = "No parameters - " + this.Text;
            this.webKitBrowser = new WebKitBrowser();
            this.webKitBrowser.Width = 1024;
            this.webKitBrowser.Height = 768;
            this.Controls.Add(this.webKitBrowser);
            this.webKitBrowser.Navigate(this.purl);
        }
        public Form1(string[] args)
        {
            InitializeComponent();
            this.args = args;
            viewDidLoad();
        }
        // /b webkit /w 1024 /h 768 /u https://www.baidu.com/ /o Z:\1.png /t 10
        private void viewDidLoad()
        {
            List<string> argskey = new List<string> { };
            List<string> argsval = new List<string> { };
            bool nowvk = true;
            foreach (string t in this.args)
            {
                if (nowvk)
                {
                    argskey.Add(t);
                }
                else
                {
                    argsval.Add(t);
                }
                nowvk = !nowvk;
            }
            for (int i = 0; i < argskey.Count; i++)
            {
                string nowkey = argskey[i];
                string nowval = argsval[i];
                if (nowkey == "/w")
                {
                    this.pwidth = Convert.ToInt32(nowval);
                }
                else if (nowkey == "/h")
                {
                    this.pheight = Convert.ToInt32(nowval);
                }
                else if (nowkey == "/t")
                {
                    this.ptime = Convert.ToInt32(nowval);
                }
                else if (nowkey == "/u")
                {
                    this.purl = nowval;
                }
                else if (nowkey == "/o")
                {
                    this.pout = nowval;
                }
                else if (nowkey == "/b" && nowval == "ie")
                {
                    this.usewebkit = false;
                }
                else if (nowkey == "/p")
                {
                    this.scrollto = Convert.ToInt32(nowval);
                }
            }
            this.startp();
        }

        private void startp()
        {
            this.Text = "[Loading...] " + this.purl;
            this.ntime = ptime;
            if (this.usewebkit)
            {
                this.webKitBrowser = new WebKitBrowser();
                this.webKitBrowser.Width = this.pwidth;
                this.webKitBrowser.Height = this.pheight;
                this.Controls.Add(this.webKitBrowser);
                this.webKitBrowser.Navigate(this.purl);
                this.webKitBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(documentCompleted);
            }
            else
            {
                this.ieBrowser = new WebBrowser();
                this.ieBrowser.Width = this.pwidth;
                this.ieBrowser.Height = this.pheight;
                this.Controls.Add(this.ieBrowser);
                this.ieBrowser.Navigate(this.purl);
                this.ieBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(documentCompleted);
                this.ieBrowser.ScrollBarsEnabled = false;
            }
        }

        private void documentCompleted(object sender, EventArgs e)
        {
            string js = "document.body.scrollTop=" + this.scrollto + ";document.body.style.overflow='hidden';";
            if (this.usewebkit)
            {
                this.webKitBrowser.StringByEvaluatingJavaScriptFromString(js);
            }
            else
            {
                this.ieBrowser.Document.InvokeScript(js);
            }
            if (this.timer1.Enabled)
            {
                return;
            }
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.ntime--;
            this.Text = "[" + this.ntime + "] " + this.purl;
            if (this.ntime < 0)
            {
                this.Text = "[screenshot...] " + this.purl;
                this.timer1.Enabled = false;
                this.screenshot();
            }
        }

        private void screenshot()
        {
            Bitmap bit = new Bitmap(this.pwidth, this.pheight);
            Graphics g = Graphics.FromImage(bit);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            if (this.webKitBrowser != null)
            {
                g.CopyFromScreen(this.webKitBrowser.PointToScreen(Point.Empty), Point.Empty, this.webKitBrowser.Size);
            }
            else if (this.ieBrowser != null)
            {
                g.CopyFromScreen(this.ieBrowser.PointToScreen(Point.Empty), Point.Empty, this.ieBrowser.Size);
            }
            try
            {
                bit.Save(this.pout);
            }
            catch
            {
                MessageBox.Show("Save failed:\n" + this.pout, "YashiWebpageScreenshot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Application.Exit();
        }
    }
}
