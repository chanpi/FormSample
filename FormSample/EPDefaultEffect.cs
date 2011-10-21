using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormSample
{
    public abstract class EPDefaultEffect
    {
        public abstract void DrawEffectImage(Bitmap current, Bitmap next, Control ctrl);
    }
}
