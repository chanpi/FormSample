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
        public override void DrawEffectImage(Bitmap current, Bitmap next, EffectablePanel effectablePanel)
        {
            Bitmap doubleBufferingBitmap = null;    // ダブルバッファリング用画面
            Graphics bg = null;                     // ダブルバッファリング用画面描画用Graphics

            SolidBrush solidBrush = null;
            Rectangle rectangle;

            ColorMatrix colorMatrix = null;
            ImageAttributes imageAttributes = null;

            try
            {
                doubleBufferingBitmap = new Bitmap(current);
                bg = Graphics.FromImage(doubleBufferingBitmap);

                solidBrush = new SolidBrush(System.Drawing.Color.Black);
                rectangle = new Rectangle(0, 0, current.Width, current.Height);

                colorMatrix = new ColorMatrix();
                imageAttributes = new ImageAttributes();

                // フェードアウト
                for (float alpha = 0.9f; alpha >= 0.0f; alpha -= 0.05f)
                {
                    bg.FillRectangle(solidBrush, rectangle);
                    colorMatrix.Matrix33 = alpha;
                    imageAttributes.SetColorMatrix(colorMatrix);
                    bg.DrawImage(current, rectangle, 0, 0, doubleBufferingBitmap.Width, doubleBufferingBitmap.Height, GraphicsUnit.Pixel, imageAttributes);

                    Thread.Sleep(10);
                    effectablePanel.pictureBox.Image = doubleBufferingBitmap;
                    effectablePanel.pictureBox.Refresh();
                }
                for (float alpha = 0.0f; alpha <= 1.0f; alpha += 0.05f)
                {
                    bg.FillRectangle(solidBrush, rectangle);
                    colorMatrix.Matrix33 = alpha;
                    imageAttributes.SetColorMatrix(colorMatrix);
                    bg.DrawImage(next, rectangle, 0, 0, doubleBufferingBitmap.Width, doubleBufferingBitmap.Height, GraphicsUnit.Pixel, imageAttributes);

                    Thread.Sleep(10);
                    effectablePanel.pictureBox.Image = doubleBufferingBitmap;
                    effectablePanel.pictureBox.Refresh();
                }
                bg.Dispose();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //public override void DrawEffectImage(Bitmap current, Bitmap next, EffectablePanel effectablePanel)
        //{
        //    Graphics g = null;
        //    //effectablePanel.pictureBox.Visible = false;

        //    try
        //    {
        //        g = effectablePanel.CreateGraphics();

        //        // フェードアウト
        //        for (int i = 10; i >= 1; i--)
        //        {
        //            DrawFadedImage(g, current, i * 0.1F);   // 半透明で画像を描画
        //            Thread.Sleep(50);
        //        }

        //        Application.DoEvents();

        //        // フェードイン
        //        for (int i = 0; i <= 9; i++)
        //        {
        //            DrawFadedImage(g, next, i * 0.1F);  // 半透明で画像を描画
        //            Thread.Sleep(50);
        //        }

        //        current.Dispose();
        //        next.Dispose();
        //        g.Dispose();
        //    }
        //    catch (SystemException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    //effectablePanel.pictureBox.Visible = true;
        //}

        //private static void DrawFadedImage(Graphics g, Image image, float alpha)
        //{
        //    Bitmap doubleBufferingBitmap = null;    // ダブルバッファリング用画面
        //    Graphics bg = null;                     // ダブルバッファリング用画面描画用Graphics
        //    ColorMatrix colorMatrix = null;
        //    ImageAttributes imageAttributes = null;

        //    try
        //    {
        //        doubleBufferingBitmap = new Bitmap(image.Width, image.Height);
        //        bg = Graphics.FromImage(doubleBufferingBitmap);
        //        bg.Clear(Color.White);  // 白で塗りつぶす

        //        colorMatrix = new ColorMatrix();    // ColorMatrixオブジェクトの作成
        //        // ColorMatrixの行列の値を変更して、アルファ値がalphaに変更されるようにする
        //        colorMatrix.Matrix00 = 1;
        //        colorMatrix.Matrix11 = 1;
        //        colorMatrix.Matrix22 = 1;
        //        colorMatrix.Matrix33 = alpha;
        //        colorMatrix.Matrix44 = 1;

        //        imageAttributes = new ImageAttributes();
        //        imageAttributes.SetColorMatrix(colorMatrix);    // ColorMatrixを設定する

        //        // ImageAttributesを使用して背景に描画
        //        bg.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
        //            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
        //        g.DrawImage(doubleBufferingBitmap, 0, 0);

        //        bg.Dispose();
        //        doubleBufferingBitmap.Dispose();
        //    }
        //    catch (SystemException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }
}
