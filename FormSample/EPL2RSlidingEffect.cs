using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Effecting
{
    public class EpL2RSlidingEffect : EpDefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, EffectingPanel effecingPanel)
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
				matrix = new Matrix();

                step = doubleBufferingBitmap.Width / 50;
                if (step < 1)
                {
                    step = 1;
                }
					
				ResetInterval();

                for (int x = 0; x < doubleBufferingBitmap.Width; x += step)
                {
                    bg.ResetTransform();                        // リセット座標変換
                    bg.FillRectangle(solidBrush, rectangle);

                    // current画像
                    matrix.Reset();
                    matrix.Translate(x, 0, MatrixOrder.Append);    // 原点移動
                    bg.Transform = matrix;                         // 座標設定
                    bg.DrawImage(current, 0, 0);
                    
                    // next画像
					matrix.Reset();
					matrix.Translate(x - doubleBufferingBitmap.Width, 0, MatrixOrder.Append);
                    bg.Transform = matrix;
                    bg.DrawImage(next, 0, 0);
                    
                    effecingPanel.pictureBox.Image = doubleBufferingBitmap;
                    effecingPanel.pictureBox.Refresh();
					
					DoEventAtIntervals();
                }

				matrix.Dispose();
                bg.Dispose();
                doubleBufferingBitmap.Dispose();

                effecingPanel.pictureBox.Image = next;
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
