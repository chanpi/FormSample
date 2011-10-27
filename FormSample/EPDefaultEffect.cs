using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Effecting
{
    public class EpDefaultEffect
    {
		private int intervalTime;
		private int previousTime;

		public EpDefaultEffect() {
			intervalTime = 200;
			previousTime = System.Environment.TickCount;
		}

        public virtual void DrawEffectImage(Bitmap current, Bitmap next, EffectingPanel effecingPanel)
        {
        }

		public void ResetInterval()
		{
			previousTime = System.Environment.TickCount;
		}

		public void DoEventAtIntervals()
		{
			if (System.Environment.TickCount - previousTime > intervalTime)
			{
				Application.DoEvents();
				previousTime = System.Environment.TickCount;

				// System.Diagnostics.Debug.WriteLine("DoEvents");
			}
		}
    }
}
