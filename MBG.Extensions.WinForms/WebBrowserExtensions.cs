using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MBG.Extensions.IO;
using MBG.Extensions.Win32;

namespace MBG.Extensions.WinForms
{
    public static class WebBrowserExtensions
    {
        //Working for most websites now, but not all
        //Many thanks to mariscn: http://www.codeproject.com/KB/graphics/html2image.aspx
        private static Bitmap GenerateScreenshot(this WebBrowser webBrowser, Size? size)
        {
            Application.DoEvents();

            Control parent = webBrowser.Parent;
            DockStyle dockStyle = webBrowser.Dock;
            bool scrollbarsEnabled = webBrowser.ScrollBarsEnabled;

            if (parent != null)
            {
                parent.Controls.Remove(webBrowser);
            }

            Rectangle screen = Screen.PrimaryScreen.Bounds;
            Size? imageSize = null;

            Rectangle body = webBrowser.Document.Body.ScrollRectangle;

            //check if the document width/height is greater than screen width/height
            Rectangle docRectangle = new Rectangle()
            {
                Location = new Point(0, 0),
                Size = new Size(
                    body.Width > screen.Width ? body.Width : screen.Width,
                    body.Height > screen.Height ? body.Height : screen.Height)
            };
            //set the width and height of the WebBrowser object
            webBrowser.ScrollBarsEnabled = false;
            webBrowser.Size = new Size(docRectangle.Width, docRectangle.Height);

            //Cannot update scrollrectangle. Wehn web browser size changed, only width of body changed; not height
            //webBrowser.Document.Body.ScrollRectangle.Width = webBrowser.Width;
            //webBrowser.Document.Body.ScrollRectangle.Height = webBrowser.Height;

            //if the imgsize is null, the size of the image will 
            //be the same as the size of webbrowser object
            //otherwise  set the image size to imgsize
            Rectangle imgRectangle;
            if (imageSize == null)
            { imgRectangle = docRectangle; }
            else
                imgRectangle = new Rectangle()
                {
                    Location = new Point(0, 0),
                    Size = imageSize.Value
                };
            //create a bitmap object 
            Bitmap bitmap = new Bitmap(imgRectangle.Width, imgRectangle.Height);
            //get the viewobject of the WebBrowser
            IViewObject viewObject = webBrowser.Document.DomDocument as IViewObject;

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //get the handle to the device context and draw
                IntPtr hdc = g.GetHdc();
                viewObject.Draw(1, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, hdc, ref imgRectangle, ref docRectangle, IntPtr.Zero, 0);
                g.ReleaseHdc(hdc);
            }

            if (parent != null)
            {
                parent.Controls.Add(webBrowser);
                webBrowser.Dock = dockStyle;
            }
            webBrowser.ScrollBarsEnabled = scrollbarsEnabled;

            return bitmap;
        }

        public static Bitmap GetScreenshot(this WebBrowser webBrowser)
        {
            return webBrowser.GetScreenshot(null);
        }
        public static Bitmap GetScreenshot(this WebBrowser webBrowser, Size? size)
        {
            Bitmap screenshot = webBrowser.GenerateScreenshot(size);
            return screenshot;
        }

        public static void SaveScreenshot(this WebBrowser webBrowser, string fileName)
        {
            new DirectoryInfo(Path.GetDirectoryName(fileName)).Ensure();
            webBrowser.GetScreenshot(null).Save(fileName);
        }
        public static void SaveScreenshot(this WebBrowser webBrowser, string fileName, ImageFormat imageFormat)
        {
            new DirectoryInfo(Path.GetDirectoryName(fileName)).Ensure();
            webBrowser.GetScreenshot(null).Save(fileName, imageFormat);
        }
        public static void SaveScreenshot(this WebBrowser webBrowser, string fileName, Size? size)
        {
            new DirectoryInfo(Path.GetDirectoryName(fileName)).Ensure();
            webBrowser.GetScreenshot(size).Save(fileName);
        }
        public static void SaveScreenshot(this WebBrowser webBrowser, string fileName, Size? size, ImageFormat imageFormat)
        {
            new DirectoryInfo(Path.GetDirectoryName(fileName)).Ensure();
            webBrowser.GetScreenshot(size).Save(fileName, imageFormat);
        }
    }
}