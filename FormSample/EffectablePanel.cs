using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;

namespace FormSample
{
    public partial class EffectablePanel : Panel
    {
        public EffectablePanel()
        {
            InitializeComponent();
            //this.BackColor = Color.Red;
        }

        public void StartScreenTransition(ref Panel before, Boolean doEffect)
        {
            Bitmap bmp = CapturePanel(ref before, true);
            Graphics g = this.CreateGraphics();

            //this.Visible = true;

            // フェードアウト
            for (int i = 10; i >= 1; i--)
            {
                this.Visible = true;

                // 半透明で画像を描画
                DrawFadedImage(g, bmp, i * 0.1F);
                Application.DoEvents();

                Thread.Sleep(50);
            }

            bmp.Dispose();
            g.Dispose();
        }

        public void EndScreenTransition(ref Panel after)
        {
            after.Visible = true;   // 先にVisibleにしておく

            Bitmap bmp = CapturePanel(ref after, false);
            Graphics g = this.CreateGraphics();

            // フェードイン
            for (int i = 0; i <= 9; i++)
            {
                // 半透明で画像を描画
                DrawFadedImage(g, bmp, i * 0.1F);
                Application.DoEvents();

                // 一時停止
                Thread.Sleep(50);
            }

            this.Visible = false;

            bmp.Dispose();
            g.Dispose();
        }

        //public void ScreenTransition(ref Panel before, ref Panel after, Boolean doEffect)
        //{
        //    CapturePanel(ref before, true);
        //    CapturePanel(ref after, false);

        //    // effect
        //    this.Visible = true;
        //    //before.SendToBack();
        //    //before.Visible = false;
        //    Thread.Sleep(300);
        //    after.Visible = true;
        //    this.Visible = false;
        //}

        private static void DrawFadedImage(Graphics g, Image img, float alpha)
        {
            // 背景を用意する
            Bitmap back = new Bitmap(img.Width, img.Height);
            // backのGraphicsオブジェクトを取得
            Graphics bg = Graphics.FromImage(back);
            // 白で塗りつぶす
            bg.Clear(Color.White);

            // ColorMatrixオブジェクトの作成
            ColorMatrix cm = new ColorMatrix();
            // ColorMatrixの行列の値を変更して、アルファ値がalphaに変更されるようにする
            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix33 = alpha;
            cm.Matrix44 = 1;

            // ImageAttributeオブジェクトの作成
            ImageAttributes ia = new ImageAttributes();
            // ColorMatrixを設定する
            ia.SetColorMatrix(cm);

            // ImageAttributesを使用して背景に描画
            bg.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
            // 合成された画像を表示
            g.DrawImage(back, 0, 0);

            // リソースを開放する
            bg.Dispose();
            back.Dispose();

        }

        private Bitmap CapturePanel(ref Panel panel, Boolean before)
        {
            Rectangle rect = RectangleToScreen(panel.Bounds);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            if (before)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                }
                bmp.Save(@"before.bmp", ImageFormat.Bmp);
            }
            else
            {
                panel.DrawToBitmap(bmp, panel.Bounds);
                //panel.DrawToBitmap(bmp, new Rectangle(0, 0, panel.Bounds.Width, panel.Bounds.Height));
                bmp.Save(@"after.bmp", ImageFormat.Bmp);
            }
            return bmp;
        }
    }
}
