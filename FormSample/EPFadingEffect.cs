﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Effecting
{
    public class EpFadingEffect : EpDefaultEffect
    {
        public override void DrawEffectImage(Bitmap current, Bitmap next, EffectingPanel effecingPanel)
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

				ResetInterval();

                // フェードアウト
                for (float alpha = 0.9f; alpha >= 0.0f; alpha -= 0.05f)
                {
                    bg.FillRectangle(solidBrush, rectangle);
                    colorMatrix.Matrix33 = alpha;
                    imageAttributes.SetColorMatrix(colorMatrix);
                    bg.DrawImage(current, rectangle, 0, 0, doubleBufferingBitmap.Width, doubleBufferingBitmap.Height, GraphicsUnit.Pixel, imageAttributes);

                    Thread.Sleep(10);

                    effecingPanel.pictureBox.Image = doubleBufferingBitmap;
                    effecingPanel.pictureBox.Refresh();
				
					DoEventAtIntervals();
				}
                for (float alpha = 0.0f; alpha <= 1.0f; alpha += 0.05f)
                {
                    bg.FillRectangle(solidBrush, rectangle);
                    colorMatrix.Matrix33 = alpha;
                    imageAttributes.SetColorMatrix(colorMatrix);
                    bg.DrawImage(next, rectangle, 0, 0, doubleBufferingBitmap.Width, doubleBufferingBitmap.Height, GraphicsUnit.Pixel, imageAttributes);

                    Thread.Sleep(10);
					
					effecingPanel.pictureBox.Image = doubleBufferingBitmap;
                    effecingPanel.pictureBox.Refresh();

					DoEventAtIntervals();
                }
                bg.Dispose();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
