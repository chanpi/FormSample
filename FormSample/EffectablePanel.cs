using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;

namespace FormSample
{
    public partial class EffectablePanel : Panel
    {
        public enum EffectType { Fading, NONE };

        private ArrayList effectType = null;

        public EffectablePanel()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            CreateEffectInstance();
        }

        private void CreateEffectInstance()
        {
            effectType = new ArrayList();
            effectType.Add(new FadingEffect());     // Fading
            effectType.Add(new DefaultEffect());    // NONE
        }

        public void TransPanel(Panel current, Panel next, EffectType type)
        {
            try
            {
                Console.WriteLine(current.Name + " -> " + next.Name);

                // 遷移前Panelをキャプチャ
                Bitmap currentBmp = GetPreviousCapturedImage(current, current.Name + ".bmp", false);
                Bitmap nextBmp = null;
                Graphics g = this.CreateGraphics();

                // 遷移後画面を取得
                string nextBmpPath = next.Name + ".bmp";
                if (System.IO.File.Exists(nextBmpPath))
                {
                    Console.WriteLine("NOT first time.");
                    nextBmp = new Bitmap(nextBmpPath);
                }
                else
                {
                    Console.WriteLine("first time.");
                    nextBmp = GetPreviousCapturedImage(next, nextBmpPath, true); // 初回のみ
                }

                // effectスタート
                this.Visible = true;

                current.Visible = false;

                FadingEffect effectInstance = effectType[(int)type] as FadingEffect;
                effectInstance.DrawEffectImage(currentBmp, nextBmp, g);

                next.Visible = true;

                // effect終わり
                this.Visible = false;

                currentBmp.Dispose();
                nextBmp.Dispose();
                g.Dispose();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Bitmap GetPreviousCapturedImage(Panel panel, string filePath, Boolean firstTime)
        {
            Rectangle rect = RectangleToScreen(panel.Bounds);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            if (firstTime)
            {
                panel.DrawToBitmap(bmp, panel.Bounds);
            }
            else
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                }
            }
            bmp.Save(filePath, ImageFormat.Bmp);    // 保存する場合
            return bmp;
        }

    }
}
