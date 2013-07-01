using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShockwaveFlashObjects;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.GPU;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;


namespace BotPoker
{
    public partial class Form1 : Form
    {
        //imports the GDI BitBlt function that enables the background of the window 
        //to be captured 
        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(
            IntPtr hdcDest,          // handle to destination DC 
            int nXDest,                // x-coord of destination upper-left corner 
            int nYDest,                // y-coord of destination upper-left corner 
            int nWidth,               // width of destination rectangle 
            int nHeight,              // height of destination rectangle 
            IntPtr hdcSrc,            // handle to source DC 
            int nXSrc,                  // x-coordinate of source upper-left corner 
            int nYSrc,                  // y-coordinate of source upper-left corner 
            int nSrcWidth,
            int nSrcHeight,
            System.Int32 dwRop  // raster operation code 
        );
        const Int32 SRCCOPY = 0x00CC0020;
        [DllImport("User32.dll")]
        public extern static System.IntPtr GetDC(System.IntPtr hWnd);
        [DllImport("User32.dll")]
        public extern static int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC); //modified to include hWnd

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }


        int formWidth, formHeight;      //original form size
        //int imageWidth, imageHeight;    // resize image box when form is resized
        Image<Bgr, Byte> imgMasterColor;
        Image<Bgr, Byte> imgToFindColor;
        Image<Bgr, Byte> imgCopyToFind;
        Image<Bgr, Byte> imgResult;

        Bgr bgrKeyPointsColor = new Bgr(Color.Blue);
        Bgr bgrMatchingLinesColor = new Bgr(Color.Green);
        Bgr bgrFoundImageColor = new Bgr(Color.Red);

        bool masterLoaded = false;
        bool toFindLoaded = false;

        Stopwatch stopwatch = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            formWidth = this.Width;
            formHeight = this.Height;
            //imageWidth = pbox.Width;
            //imageHeight = pbox.Height;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axShockwaveFlash1.Movie = "http://www.miniclip.com/games/bullfrog-poker/fr/gameloader.swf?mc_gamename=Bullfrog+Poker&mc_hsname=2717&mc_iconBig=bullfrogpokerv3medicon.jpg&mc_icon=bullfrogpokerv3smallicon.jpg&mc_negativescore=0&mc_players_site=1&mc_scoreistime=0&mc_lowscore=0&mc_width=738&mc_height=575&mc_v2=0&loggedin=0&mc_loggedin=0&mc_uid=0&mc_sessid=c4ihl2j5omridevohq204ca771&mc_shockwave=0&mc_gameUrl=%2Fgames%2Fbullfrog-poker%2Ffr%2F&mc_ua=3064a31&mc_geo=NapMia&mc_geoCode=FR&vid=1&m_vid=1&channel=miniclip.preroll&m_channel=miniclip.midroll&s_content=0&mc_webmaster=0&mc_lang=fr&c=1&fn=bfpoker1.9.1.swf";
            axShockwaveFlash1.Play();
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(this.axShockwaveFlash1.Width, this.axShockwaveFlash1.Height);
            Graphics g = Graphics.FromImage(bm);
            System.IntPtr bmDC = g.GetHdc();
            System.IntPtr srcDC = GetDC(this.axShockwaveFlash1.Handle);
            int xOffset = 0,
                yOffset = 0,
                width = bm.Width,         // Fill the entire picture box.
                height = bm.Height;
            StretchBlt(bmDC, xOffset, yOffset, width, height, srcDC, 0, 0, axShockwaveFlash1.Width, axShockwaveFlash1.Height, SRCCOPY);
            ReleaseDC(this.axShockwaveFlash1.Handle, srcDC);
            g.ReleaseHdc(bmDC);
            g.Dispose();
            bm.Save(@"C:\temp\master.jpg"); // now save the image
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Convertimg the master image to a bitmap
            /*Bitmap masterImage = new Bitmap(@"C:\temp\master.jpg");

            // Normalazing it to the grayscal mode
            Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>(masterImage);

            // Searching for the sample pictures in the master image
            ImageList imagesList1 = new ImageList();
            Image myImage = Image.FromFile(@"C:\temp\play.png");
            imagesList1.Images.Add(myImage);*/

            imgMasterColor = new Image<Bgr, byte>(@"C:\temp\master.jpg");
            masterLoaded = true;

            imgToFindColor = new Image<Bgr, byte>(@"C:\temp\play.png");
            toFindLoaded = true;

            imgCopyToFind = imgToFindColor.Copy();
            imgCopyToFind.Draw(new Rectangle(1, 1, imgCopyToFind.Width - 3, imgCopyToFind.Height - 3), bgrFoundImageColor, 2);

            PerformSurfDetection(new object(), new EventArgs());

        }

        private void PerformSurfDetection(object sender, EventArgs e)
        {
            
            this.Text = "working...";
            Application.DoEvents();
            stopwatch.Restart();

            HomographyMatrix homographyMatrix = null;
            SURFDetector surfDetector = new SURFDetector(500, false);
            Image<Gray, Byte> imgMasterGray;
            Image<Gray, Byte> imgToFindGray;
            VectorOfKeyPoint vkpMasterKeyPoints;
            VectorOfKeyPoint vkpToFindKeyPoints;
            Matrix<float> mtxMasterDescriptors;
            Matrix<float> mtxToFindDescriptors;
            Matrix<int> mtxMatchIndices;
            Matrix<float> mtxDistance;
            Matrix<Byte> mtxMask;
            BruteForceMatcher<float> bruteForceMatcher;


            int neighbors = 2;
            double ratioUnique = 0.5;
            int nonZeroElements;
            double scaleIncrement = 1.5;
            int rotationBin = 20;
            double maxReprojectionError = 2.0;


            //PointF[] ptfPointsF;
            //Point ptPoints;

            imgMasterGray = new Image<Gray, byte>(imgMasterColor.ToBitmap());
            imgToFindGray = new Image<Gray, byte>(imgToFindColor.ToBitmap());

            vkpMasterKeyPoints = surfDetector.DetectKeyPointsRaw(imgMasterGray, null);
            mtxMasterDescriptors = surfDetector.ComputeDescriptorsRaw(imgMasterGray, null, vkpMasterKeyPoints);

            vkpToFindKeyPoints = surfDetector.DetectKeyPointsRaw(imgToFindGray, null);
            mtxToFindDescriptors = surfDetector.ComputeDescriptorsRaw(imgToFindGray, null, vkpToFindKeyPoints);

            bruteForceMatcher = new BruteForceMatcher<float>(DistanceType.L2);
            bruteForceMatcher.Add(mtxToFindDescriptors);

            mtxMatchIndices = new Matrix<int>(mtxMasterDescriptors.Rows, neighbors);
            mtxDistance = new Matrix<float>(mtxMasterDescriptors.Rows, neighbors);

            bruteForceMatcher.KnnMatch(mtxMasterDescriptors, mtxMatchIndices, mtxDistance, neighbors, null);

            mtxMask = new Matrix<byte>(mtxDistance.Rows, 1);
            mtxMask.SetValue(255);

            Features2DToolbox.VoteForUniqueness(mtxDistance, ratioUnique, mtxMask);

            nonZeroElements = CvInvoke.cvCountNonZero(mtxMask);
            if (nonZeroElements >= 4)
            {
                nonZeroElements = Features2DToolbox.VoteForSizeAndOrientation(vkpToFindKeyPoints, vkpMasterKeyPoints, mtxMatchIndices, mtxMask, scaleIncrement, rotationBin);
                if (nonZeroElements >= 4)
                {
                    homographyMatrix = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(vkpToFindKeyPoints, vkpMasterKeyPoints, mtxMatchIndices, mtxMask, maxReprojectionError);
                }
            }

            imgCopyToFind = imgToFindColor.Copy();
            imgCopyToFind.Draw(new Rectangle(1, 1, imgCopyToFind.Width - 3, imgCopyToFind.Height - 3), bgrFoundImageColor, 2);

            imgResult = imgMasterColor;
            imgResult = imgResult.ConcateHorizontal(imgCopyToFind);

            if (homographyMatrix != null)
            {
                // draw a rectangle along the projected model
                Rectangle rect = imgCopyToFind.ROI;
                PointF[] pts = new PointF[] { 
                    new PointF(rect.Left, rect.Bottom),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Right, rect.Top),
                    new PointF(rect.Left, rect.Top)
                };

                homographyMatrix.ProjectPoints(pts);
                
                Point[] ptPoints = { Point.Round(pts[0]), Point.Round(pts[1]), Point.Round(pts[2]), Point.Round(pts[3]) };
                
                imgResult.DrawPolyline(ptPoints, true, bgrFoundImageColor, 2);

                int X = Convert.ToInt16((pts[0].X + pts[1].X) / 2) + this.Left;
                int Y = Convert.ToInt16((pts[1].Y + pts[2].Y) / 2) + this.Top + 30;
                
                LeftClick(X, Y);
            }

            stopwatch.Stop();
            //this.Text = "working time = " + stopwatch.Elapsed.TotalSeconds.ToString() + "sec, done ! ";
        }

        public void LeftClick(int x, int y)
        {
            Cursor.Position = new System.Drawing.Point(x, y);
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            this.Text = x.ToString() + " ----- " + y.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            imgMasterColor = new Image<Bgr, byte>(@"C:\temp\master.jpg");
            masterLoaded = true;

            imgToFindColor = new Image<Bgr, byte>(@"C:\temp\tournament.png");
            toFindLoaded = true;

            imgCopyToFind = imgToFindColor.Copy();
            imgCopyToFind.Draw(new Rectangle(1, 1, imgCopyToFind.Width - 3, imgCopyToFind.Height - 3), bgrFoundImageColor, 2);

            PerformSurfDetection(new object(), new EventArgs());
        }
    }
}
