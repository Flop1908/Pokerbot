﻿using System;
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
using System.IO;

using Emgu.CV;
using Emgu.CV.GPU;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;


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
        private static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        private Tesseract _ocr;
        const string tessractData = "tessdata";

        List<PokerCard> hand = new List<PokerCard>();
        List<PokerCard> table = new List<PokerCard>();
        List<CorrectionOCR> correction = new List<CorrectionOCR>();

        string pathimg = "imgPoker";
        string pathparam = "paramPoker";

        public Form1()
        {
            InitializeComponent();
            _ocr = new Tesseract(tessractData, "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED);
            CorrectionOCR();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            axShockwaveFlash1.Movie = "http://www.miniclip.com/games/bullfrog-poker/fr/gameloader.swf?mc_gamename=Bullfrog+Poker&mc_hsname=2717&mc_iconBig=bullfrogpokerv3medicon.jpg&mc_icon=bullfrogpokerv3smallicon.jpg&mc_negativescore=0&mc_players_site=1&mc_scoreistime=0&mc_lowscore=0&mc_width=738&mc_height=575&mc_v2=0&loggedin=0&mc_loggedin=0&mc_uid=0&mc_sessid=c4ihl2j5omridevohq204ca771&mc_shockwave=0&mc_gameUrl=%2Fgames%2Fbullfrog-poker%2Ffr%2F&mc_ua=3064a31&mc_geo=NapMia&mc_geoCode=FR&vid=1&m_vid=1&channel=miniclip.preroll&m_channel=miniclip.midroll&s_content=0&mc_webmaster=0&mc_lang=fr&c=1&fn=bfpoker1.9.1.swf";
            axShockwaveFlash1.Play();           
        }


        private void button1_Click(object sender, EventArgs e)
        {

            foreach (string filename in Directory.GetFiles(pathparam, "*.jpg", SearchOption.TopDirectoryOnly))
            {
                File.Delete(filename);
            }

            bool firstturn = true;
            List<string> hand_ocr = new List<string>();
            if (firstturn)
            {
                // recognition player hand's cards
                for (int i = 0; i < 2; i++)
                {
                    hand_ocr.Add(OCRDetection(pathparam + @"\hand" + i + ".jpg"));
                }
            }

            
            TakeImageMaster();
            CropMasterImg(1, "myturn", 295, 80);

           
            if (CompareImg(OCRDetection(pathparam + @"\myturn1.jpg").Trim(), OCRDetection(pathimg + @"\yourturn.jpg").Trim()))
            {
                TakeImageMaster();
                CropMasterImg(5, "table", 235, 327);
                CropMasterImg(2, "hand", 299, 223);
                
                if (firstturn)
                {
                    // recognition player hand's cards
                    foreach (string s_hand_ocr in hand_ocr)
                    {
                        foreach (CorrectionOCR c in correction)
                        {
                            if (c.ocr == s_hand_ocr)
                            {
                                hand.Add(new PokerCard(c.card));
                                label1.Text += c.card + "-";
                                break;
                            }
                        }
                        /*foreach (string filename in Directory.GetFiles(pathimg))
                        {
                            if (CompareImg(OCRDetection(pathparam + @"\hand" + i + ".jpg"), OCRDetection(filename)))
                            {
                                string[] file = filename.Split('\\');
                                //label1.Text += file[3].Split('.')[0];
                                hand.Add(new PokerCard(file[0]));
                                break;
                            }
                        }*/
                    }
                    firstturn = false;
                }
                // recognition table cards
                List<string> table_ocr = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    table_ocr.Add(OCRDetection(pathparam + @"\table" + i + ".jpg"));
                }
                foreach (string s_table_ocr in table_ocr)
                {
                    foreach (CorrectionOCR c in correction)
                    {
                        if (c.ocr == s_table_ocr)
                        {
                            table.Add(new PokerCard(c.card));
                            label2.Text += c.card + "-";
                            break;
                        }
                    }
                }
                    /*foreach (string filename in Directory.GetFiles(pathimg))
                    {
                        if (CompareImg(OCRDetection(pathparam + @"\table" + i + ".jpg"), OCRDetection(filename)))
                        {
                            string[] file = filename.Split('\\');
                            table.Add(new PokerCard(file[0]));
                            break;
                        }
                    }*/
                /*hand.Add(new PokerCard("as"));
                hand.Add(new PokerCard("ks"));
                table.Add(new PokerCard("10s"));
                table.Add(new PokerCard("js"));
                table.Add(new PokerCard("2d"));*/
                Situation s = new Situation();
                s.playerCards = hand;
                s.communityCards = table;
                //label1.Text = "Action à faire selon l'IA : " + Calculon.elCalculator(s);
                /*switch (Calculon.elCalculator(s))
                {
                    case "raise": 
                        Action.RaiseAction(this.Left, this.Top + 30);
                        break;
                    case "check": 
                        Action.CheckAction(this.Left, this.Top + 30);
                        break;
                    case "fold": 
                        Action.FoldAction(this.Left, this.Top + 30);
                        break;

                    default:
                        break;
                }*/

                
            }
        }

        // Compare images
        private bool CompareImg(string ocr1, string ocr2)
        {
            bool b = true;
            int max = 0;
            if (ocr1.Length > 1 && ocr2.Length > 1)
            {
                if (ocr1.Length > ocr2.Length) max = ocr2.Length;
                else max = ocr1.Length;
                for (int i = 0; i < max; i++)
                {
                    if (ocr1[i] != ocr2[i]) b = false;
                }
            }

            return b;
        }

        // Crop image on main image
        private void CropMasterImg(int nb, string name, int x, int y)
        {
            for (int i = 1; i <= nb; i++)
            {
                Image<Bgr, Byte> imageToCrop = new Image<Bgr, byte>(pathparam + @"\master.jpg");
                if (name == "myturn") imageToCrop.ROI = new Rectangle(x, y, 90, 25);
                else if (name == "check") imageToCrop.ROI = new Rectangle(x, y, 76, 30);
                else imageToCrop.ROI = new Rectangle(x, y, 38, 52);
                Image<Bgr, byte> crop = imageToCrop.Copy();
                crop.Save(pathparam + name + i + ".jpg");
                x += 45;
            }
        }

        private string OCRDetection(string filename)
        {
            Bgr drawColor = new Bgr(Color.Blue);
            try
            {
                Image<Bgr, Byte> image = new Image<Bgr, byte>(filename);
                
                using (Image<Gray, byte> gray = image.Convert<Gray, Byte>())
                {
                    _ocr.Recognize(gray);
                    Tesseract.Charactor[] charactors = _ocr.GetCharactors();
                    foreach (Tesseract.Charactor c in charactors)
                    {
                        image.Draw(c.Region, drawColor, 1);
                    }

                    return _ocr.GetText();
                }
            }
            catch { return ""; }
        }

        public void TakeImageMaster()
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
            bm.Save(pathparam + @"\master.jpg"); // now save the image
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void CorrectionOCR()
        {
            correction.Add(new CorrectionOCR("2s", "2Q"));
            correction.Add(new CorrectionOCR("2c", "V)"));
            correction.Add(new CorrectionOCR("2h", "2V"));
            correction.Add(new CorrectionOCR("2d", "2a."));

            correction.Add(new CorrectionOCR("3s", "5."));
            correction.Add(new CorrectionOCR("3c", "3."));
            correction.Add(new CorrectionOCR("3h", "'V"));
            correction.Add(new CorrectionOCR("3d", "s."));

            correction.Add(new CorrectionOCR("4s", "4Q"));
            correction.Add(new CorrectionOCR("4c", "4."));
            correction.Add(new CorrectionOCR("4h", "[4v"));
            correction.Add(new CorrectionOCR("4d", "'.1"));

            correction.Add(new CorrectionOCR("5s", "5."));
            correction.Add(new CorrectionOCR("5c", "5."));
            correction.Add(new CorrectionOCR("5h", "5V"));
            correction.Add(new CorrectionOCR("5d", "5."));

            correction.Add(new CorrectionOCR("6s", "6Q"));
            correction.Add(new CorrectionOCR("6c", "6."));
            correction.Add(new CorrectionOCR("6h", "6xV"));
            correction.Add(new CorrectionOCR("6d", "s."));

            correction.Add(new CorrectionOCR("7s", "7Q"));
            correction.Add(new CorrectionOCR("7c", "7Q"));
            correction.Add(new CorrectionOCR("7h", "7V"));
            correction.Add(new CorrectionOCR("7d", "7."));

            correction.Add(new CorrectionOCR("8s", "8Q"));
            correction.Add(new CorrectionOCR("8c", "8."));
            correction.Add(new CorrectionOCR("8h", "8V"));
            correction.Add(new CorrectionOCR("8d", "a."));

            correction.Add(new CorrectionOCR("9s", "9Q"));
            correction.Add(new CorrectionOCR("9c", "9Q"));
            correction.Add(new CorrectionOCR("9h", "9V"));
            correction.Add(new CorrectionOCR("9d", "9."));

            correction.Add(new CorrectionOCR("10s", "1D."));
            correction.Add(new CorrectionOCR("10c", "l0."));
            correction.Add(new CorrectionOCR("10h", "1DV"));
            correction.Add(new CorrectionOCR("10d", "10."));

            correction.Add(new CorrectionOCR("js", "JQ"));
            correction.Add(new CorrectionOCR("jc", "J."));
            correction.Add(new CorrectionOCR("jh", "JV"));
            correction.Add(new CorrectionOCR("jd", "J."));

            correction.Add(new CorrectionOCR("qs", "a I."));
            correction.Add(new CorrectionOCR("qc", "g."));
            correction.Add(new CorrectionOCR("qh", "(1V"));
            correction.Add(new CorrectionOCR("qd", "ut"));

            correction.Add(new CorrectionOCR("ks", "KQ"));
            correction.Add(new CorrectionOCR("kc", "K."));
            correction.Add(new CorrectionOCR("kh", "l"));
            correction.Add(new CorrectionOCR("kd", "K."));

            correction.Add(new CorrectionOCR("as", "l."));
            correction.Add(new CorrectionOCR("ac", "l .."));
            correction.Add(new CorrectionOCR("ah", "[ hV"));
            correction.Add(new CorrectionOCR("ad", "lt"));
        }
    }
}
