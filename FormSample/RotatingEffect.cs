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
                Bitmap doubleBufferingBmp = new Bitmap(current);        // ダブルバッファリング用画面
                Graphics g = Graphics.FromImage(doubleBufferingBmp);    // ダブルバッファリング用画面描画用Graphics

                int deltaDegree = 10;
                SolidBrush blackBrush = new SolidBrush(System.Drawing.Color.Black);
                Rectangle rect = new Rectangle(0, 0, current.Width, current.Height);

                for (int angle = 0; angle <= 360; angle += deltaDegree)
                {
                    g.ResetTransform();     // リセット座標変換
                    g.FillRectangle(blackBrush, rect);

                    System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();

                    mat.Translate(doubleBufferingBmp.Width / 2, doubleBufferingBmp.Height / 2, MatrixOrder.Append);   // 原点移動
                    mat.Rotate((float)angle);
                    g.Transform = mat;      // 座標設定

                    // 画像の中心が(0, 0)になるように描画
                    g.DrawImage(current, -doubleBufferingBmp.Width / 2, -doubleBufferingBmp.Height / 2);

                    ((EffectablePanel)ctrl).pictureBox.Image = doubleBufferingBmp;
                    //ctrl.BackgroundImage = displayBmp;
                    ctrl.Refresh();

                    Thread.Sleep(10);
                    mat.Dispose();
                }
                g.Dispose();
                doubleBufferingBmp.Dispose();
            }
            catch(SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
