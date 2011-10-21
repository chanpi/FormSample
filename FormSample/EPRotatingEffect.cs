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

namespace Effectable
{
    public class EpRotatingEffect : EpDefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, Control control)
        {
            Bitmap doubleBufferingBitmap;   // ダブルバッファリング用画面
            Graphics g;                                             // ダブルバッファリング用画面描画用Graphics
            SolidBrush solidBrush;
            Rectangle rectangle;
            System.Drawing.Drawing2D.Matrix matrix;

            try
            {                
                int deltaDegree = 10;
                
                doubleBufferingBitmap = new Bitmap(current);        // ダブルバッファリング用画面
                g = Graphics.FromImage(doubleBufferingBitmap);      // ダブルバッファリング用画面描画用Graphics

                solidBrush = new SolidBrush(System.Drawing.Color.Black);
                rectangle = new Rectangle(0, 0, current.Width, current.Height);
                                
                for (int angle = 0; angle <= 360; angle += deltaDegree)
                {
                    g.ResetTransform();                             // リセット座標変換
                    g.FillRectangle(solidBrush, rectangle);

                    matrix = new System.Drawing.Drawing2D.Matrix();
                    matrix.Translate(doubleBufferingBitmap.Width / 2, doubleBufferingBitmap.Height / 2, MatrixOrder.Append);   // 原点移動
                    matrix.Rotate((float)angle);
                    g.Transform = matrix;                           // 座標設定

                    g.DrawImage(current, -doubleBufferingBitmap.Width / 2, -doubleBufferingBitmap.Height / 2);  // 画像の中心が(0, 0)になるように描画

                    ((EffectablePanel)control).pictureBox.Image = doubleBufferingBitmap;

                    Application.DoEvents();
                    Thread.Sleep(20);
                    matrix.Dispose();
                }
                g.Dispose();
                doubleBufferingBitmap.Dispose();
            }
            catch(SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
