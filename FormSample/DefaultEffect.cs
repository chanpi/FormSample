﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FormSample
{
    public abstract class DefaultEffect
    {
        public abstract void DrawEffectImage(Bitmap before, Bitmap after, Graphics g);
    }
}
