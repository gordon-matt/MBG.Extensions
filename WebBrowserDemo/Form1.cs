using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MBG.Extensions.WinForms;
using System.Drawing.Imaging;

namespace WebBrowserDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //webBrowser.ScrollBarsEnabled = false;
            webBrowser.Navigate(txtUrl.Text.Trim());
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(txtUrl.Text);
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            //webBrowser.CreateScreenshot();
            //Application.DoEvents();
            //webBrowser.CreateScreenshot();
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                if (webBrowser.DocumentText == "<HTML></HTML>\0")
                { return; }

                txtUrl.Text = webBrowser.Url.ToString();
            }
        }
    }
}