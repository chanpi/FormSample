﻿using System;
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
        public override void DrawEffectImage(Bitmap current, Bitmap next, Control control)
        {
            //try
            //{
            //    int step = 1;
            //    Graphics g = ctrl.CreateGraphics();
            //    Bitmap displayBmp = new Bitmap(current);

            //    //SolidBrush bkBrush

            //    step = displayBmp.Width / 20;
            //    if (step < 1)
            //    {
            //        step = 1;
            //    }

            //    for (int x = 0; x < displayBmp.Width; x += step)
            //    {
            //        g.ResetTransform();
            //        Matrix mat = new Matrix();
            //        g.FillRectangle(
            //    }
            //}
            //catch(SystemException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
