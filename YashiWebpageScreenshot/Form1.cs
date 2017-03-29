using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
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
        string proxy = String.Empty;

        struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        public Form1()
        {
            InitializeComponent();

            Text = "No parameters - " + Text;
            webKitBrowser = new WebKitBrowser();
            webKitBrowser.Width = 1024;
            webKitBrowser.Height = 768;
            Controls.Add(webKitBrowser);
            webKitBrowser.Navigate(purl);
            /*
            ieBrowser = new WebBrowser();
            ieBrowser.Width = 1024;
            ieBrowser.Height = 768;
            Controls.Add(ieBrowser);
            proxy = "http://127.0.0.1:8080";
            if (httpProxy(proxy)) Console.Write("PROXY");
            ieBrowser.Navigate("http://ipip.net");
            */
        }

        public Form1(string[] args)
        {
            InitializeComponent();
            this.args = args;
            viewDidLoad();
        }
        
        private void viewDidLoad()
        {
            List<string> argskey = new List<string> { };
            List<string> argsval = new List<string> { };
            bool nowvk = true;
            foreach (string t in args)
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
                    if (pwidth > 0)
                    {
                        pwidth = Convert.ToInt32(nowval);
                    }
                }
                else if (nowkey == "/h")
                {
                    if (pheight > 0)
                    {
                        pheight = Convert.ToInt32(nowval);
                    }
                }
                else if (nowkey == "/t")
                {
                    ptime = Convert.ToInt32(nowval);
                    if (ptime == 0)
                    {
                        TopMost = true;
                    }
                    else if (ptime < 0)
                    {
                        ptime = 0;
                    }
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
                else if (nowkey == "/p")
                {
                    if (scrollto > 0)
                    {
                        scrollto = Convert.ToInt32(nowval);
                    }
                }
                else if (nowkey == "/e")
                {
                    proxy = nowval;
                }
            }
            startp();
        }

        private void startp()
        {
            Text = "[Loading...] " + purl;
            ntime = ptime;
            if (usewebkit)
            {
                webKitBrowser = new WebKitBrowser();
                webKitBrowser.Width = pwidth;
                webKitBrowser.Height = pheight;
                Controls.Add(webKitBrowser);
                webKitBrowser.Navigate(purl);
                webKitBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(documentCompleted);
            }
            else
            {
                ieBrowser = new WebBrowser();
                ieBrowser.Width = pwidth;
                ieBrowser.Height = pheight;
                Controls.Add(ieBrowser);
                ieBrowser.Navigate(purl);
                ieBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(documentCompleted);
                ieBrowser.ScrollBarsEnabled = false;
            }
        }

        private void documentCompleted(object sender, EventArgs e)
        {
            string js = "document.body.scrollTop=" + scrollto + ";document.body.style.overflow='hidden';";
            if (usewebkit)
            {
                webKitBrowser.StringByEvaluatingJavaScriptFromString(js);
            }
            else
            {
                ieBrowser.Document.InvokeScript(js);
            }
            if (timer1.Enabled)
            {
                return;
            }
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ntime--;
            Text = "[" + ntime + "] " + purl;
            if (ntime == 0)
            {
                TopMost = true;
            }
            else if (ntime < 0)
            {
                Text = "[screenshot...] " + purl;
                timer1.Enabled = false;
                screenshot();
            }
        }

        private void screenshot()
        {
            Bitmap bit = new Bitmap(pwidth, pheight);
            Graphics g = Graphics.FromImage(bit);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            if (webKitBrowser != null)
            {
                g.CopyFromScreen(webKitBrowser.PointToScreen(Point.Empty), Point.Empty, webKitBrowser.Size);
            }
            else if (ieBrowser != null)
            {
                g.CopyFromScreen(ieBrowser.PointToScreen(Point.Empty), Point.Empty, ieBrowser.Size);
            }
            try
            {
                bit.Save(pout);
            }
            catch
            {
                MessageBox.Show("Save failed:\n" + pout, "YashiWebpageScreenshot", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            exit();
        }

        private bool httpProxy(string strProxy = "")
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;
            Struct_INTERNET_PROXY_INFO struct_IPI;
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));
            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }

        private void exit()
        {
            if (!string.IsNullOrEmpty(proxy) && proxy.Trim().Length != 0)
            {
                httpProxy(string.Empty);
            }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit();
        }
    }
}
