using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Effectable
{
    public class EpL2RSlidingEffect : EpDefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, EffectablePanel effectablePanel)
        {
            int step = 1;
            Graphics bg;
            Bitmap doubleBufferingBitmap;
            SolidBrush solidBrush;
            Rectangle rectangle;
            Matrix matrix = null;

            try
            {
                doubleBufferingBitmap = new Bitmap(current);        // ダブルバッファリング用画面
                bg = Graphics.FromImage(doubleBufferingBitmap);     // ダブルバッファリング用画面描画用Graphics

                solidBrush = new SolidBrush(System.Drawing.Color.Black);
                rectangle = new Rectangle(0, 0, current.Width, current.Height);

                step = doubleBufferingBitmap.Width / 200;
                if (step < 1)
                {
                    step = 1;
                }

                for (int x = 0; x < doubleBufferingBitmap.Width; x += step)
                {
                    bg.ResetTransform();                        // リセット座標変換
                    bg.FillRectangle(solidBrush, rectangle);

                    matrix = new Matrix();
                    matrix.Translate(x, 0, MatrixOrder.Append);    // 原点移動
                    bg.Transform = matrix;                         // 座標設定

                    bg.DrawImage(current, 0, 0);
                    effectablePanel.pictureBox.Image = doubleBufferingBitmap;
                    effectablePanel.Refresh();

                    Application.DoEvents();
                    matrix.Dispose();
                }

                bg.Dispose();
                doubleBufferingBitmap.Dispose();
                effectablePanel.pictureBox.Image = next;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
