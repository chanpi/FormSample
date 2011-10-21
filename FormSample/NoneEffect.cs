using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FormSample
{
    public class NoneEffect : DefaultEffect
    {
        public override void DrawEffectImage(Bitmap before, Bitmap after, Control ctrl)
        {
            // 処理なし
        }
    }
}
