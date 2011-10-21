using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;

namespace FormSample
{
    public class RotatingEffect : DefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, Control ctrl)
        {
            try
            {
                //ctrl.SetStyle(ControlStyles.DoubleBuffer, true);
                //ctrl.SetStyle(ControlStyles.UserPaint, true);
                //ctrl.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

                Graphics g = ctrl.CreateGraphics();

                Bitmap displayBmp = new Bitmap(current);
                int deltaDegree = 10;
                SolidBrush bkBrush = new SolidBrush(System.Drawing.Color.Black);
                Rectangle rect = new Rectangle(0, 0, current.Width, current.Height);

                for (int degree = 0; degree <= 360; degree += deltaDegree)
                {
                    //Bitmap bmp = RotateBitmap(current, degree, current.Width / 2, current.Height / 2);
                    //g.DrawImage(current, new Point(0, 0));
                    g.ResetTransform();     // リセット座標変換
                    g.FillRectangle(bkBrush, rect);

                    System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();

                    mat.Translate(displayBmp.Width / 2, displayBmp.Height / 2, MatrixOrder.Append);   // 原点移動
                    mat.Rotate((float)degree);
                    g.Transform = mat;      // 座標設定

                    // 画像の中心が(0, 0)になるように描画
                    g.DrawImage(current, -displayBmp.Width / 2, -displayBmp.Height / 2);
                    ctrl.BackgroundImage = displayBmp;
                    ctrl.Refresh();

                    Thread.Sleep(10);
                    mat.Dispose();
                }
                g.Dispose();
                displayBmp.Dispose();
            }
            catch(SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //private Bitmap RotateBitmap(Bitmap bmp, float angle, int originX, int originY)
        //{
        //    Bitmap bmp2 = new Bitmap(bmp);
        //    Graphics g = Graphics.FromImage(bmp2);
        //    g.Clear(System.Drawing.Color.Black);

        //    g.TranslateTransform(-originX, -originY);
        //    g.RotateTransform(angle, MatrixOrder.Append);
        //    g.TranslateTransform(originX, originY, MatrixOrder.Append);

        //    g.InterpolationMode = InterpolationMode.HighQualityBilinear;

        //    g.DrawImageUnscaled(bmp, 0, 0);
        //    g.Dispose();
        //    return bmp2;
        //}
    }
}
