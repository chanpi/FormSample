using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Effectable
{
    public class EpFadingEffect : EpDefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, Control control)
        {
            Graphics g = control.CreateGraphics();

            try
            {
                // フェードアウト
                for (int i = 10; i >= 1; i--)
                {
                    // 半透明で画像を描画
                    DrawFadedImage(g, current, i * 0.1F);
                    Application.DoEvents();

                    Thread.Sleep(50);
                }

                // フェードイン
                for (int i = 0; i <= 9; i++)
                {
                    // 半透明で画像を描画
                    DrawFadedImage(g, next, i * 0.1F);
                    Application.DoEvents();

                    // 一時停止
                    Thread.Sleep(50);
                }


                current.Dispose();
                next.Dispose();
                g.Dispose();
            }
            catch(SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DrawFadedImage(Graphics g, Image img, float alpha)
        {
            try
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
            catch(SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
