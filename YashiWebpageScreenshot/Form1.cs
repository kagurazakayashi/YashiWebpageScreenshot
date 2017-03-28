using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebKit;

namespace YashiWebpageScreenshot
{
    public partial class Form1 : Form
    {
        string[] args = null;
        WebKitBrowser webKitBrowser = null;
        WebBrowser ieBrowser = null;

        public Form1()
        {
            InitializeComponent();
            Application.Exit();
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
            int pwidth = 1024;
            int pheight = 768;
            int ptime = 5;
            string purl = "http://127.0.0.1";
            string pout = "1.png";
            bool usewebkit = true;
            for (int i = 0; i < argskey.Count; i++)
            {
                string nowkey = argskey[i];
                string nowval = argsval[i];
                if (nowkey == "/w")
                {
                    pwidth = Convert.ToInt32(nowval);
                }
                else if (nowkey == "/h")
                {
                    pheight = Convert.ToInt32(nowval);
                }
                else if (nowkey == "/t")
                {
                    ptime = Convert.ToInt32(nowval) * 1000;
                }
                else if (nowkey == "/u")
                {
                    purl = nowval;
                }
                else if (nowkey == "/o")
                {
                    pout = nowval;
                }
                else if (nowkey == "/b" && nowval == "ie")
                {
                    usewebkit = false;
                }
            }
            this.startp(usewebkit, pwidth, pheight, ptime, purl, pout);
        }
        private void startp(bool usewebkit, int pwidth, int pheight, int ptime, string purl, string pout)
        {
            this.Text = purl;
            if (usewebkit)
            {
                this.webKitBrowser = new WebKitBrowser();
                this.webKitBrowser.Width = pwidth;
                this.webKitBrowser.Height = pheight;
                this.Controls.Add(webKitBrowser);
                this.webKitBrowser.Navigate(purl);
            }
            else
            {
                this.ieBrowser = new WebBrowser();
                this.ieBrowser.Width = pwidth;
                this.ieBrowser.Height = pheight;
                this.Controls.Add(ieBrowser);
                this.ieBrowser.Navigate(purl);
            }
        }
    }
}
