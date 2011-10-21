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
                //Graphics g = ctrl.CreateGraphics();

                Bitmap displayBmp = new Bitmap(current);
                Graphics g = Graphics.FromImage(displayBmp);
                int deltaDegree = 10;
                SolidBrush bkBrush = new SolidBrush(System.Drawing.Color.Black);
                Rectangle rect = new Rectangle(0, 0, current.Width, current.Height);

                for (int degree = 0; degree <= 360; degree += deltaDegree)
                {
                    g.ResetTransform();     // リセット座標変換
                    g.FillRectangle(bkBrush, rect);

                    System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();

                    mat.Translate(displayBmp.Width / 2, displayBmp.Height / 2, MatrixOrder.Append);   // 原点移動
                    mat.Rotate((float)degree);
                    g.Transform = mat;      // 座標設定

                    // 画像の中心が(0, 0)になるように描画
                    g.DrawImage(current, -displayBmp.Width / 2, -displayBmp.Height / 2);

                    ((EffectablePanel)ctrl).pictureBox.Image = displayBmp;
                    //ctrl.BackgroundImage = displayBmp;
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

    }
}
