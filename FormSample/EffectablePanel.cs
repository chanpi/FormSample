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
        public enum EffectType { Fading, None };
        private ArrayList effects = null;
        //private Hashtable bmpContainer = null;

        public EffectablePanel(Form form)
        {
            InitializeComponent();
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            SetSize(form);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);                  // 背景を描画しない（ちらつきの抑制）
            this.SetStyle(ControlStyles.UserPaint, true);               // OSではなく独自で描画する
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);    // WM_ERASEBKGND を無視
            //this.SetStyle(ControlStyles.DoubleBuffer, true);          // 逆にちらつくのでコメントアウト

            this.BringToFront();
            this.Visible = false;

            // エフェクト効果を行うクラスのインスタンスを生成
            CreateEffectInstances();
        }

        private void CreateEffectInstances()
        {
            effects = new ArrayList();
            effects.Add(new FadingEffect());
            effects.Add(new NoneEffect());
        }

        public void Transition(Panel current, Panel next, EffectType type)
        {
            try
            {
                Bitmap nextBmp = null;
                Graphics g = this.CreateGraphics();

                // 遷移前Panelをキャプチャ
                Bitmap currentBmp = GetPreviousCapturedImage(current, current.Name + ".bmp", false);

                string nextBmpPath = next.Name + ".bmp";
                if (System.IO.File.Exists(nextBmpPath))
                {
                    nextBmp = new Bitmap(nextBmpPath);
                }
                else
                {
                    nextBmp = GetPreviousCapturedImage(next, nextBmpPath, true); // 初回のみ
                }

                // effectスタート
                this.Visible = true;

                current.Visible = false;

                DefaultEffect effect = effects[(int)type] as DefaultEffect;

                // effectを実行
                effect.DrawEffectImage(currentBmp, nextBmp, g);

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
                panel.Refresh();
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

        public void SetSize(Form form)
        {
            Rectangle clientRectangle = form.ClientRectangle;
            this.Location = clientRectangle.Location;
            this.Size = clientRectangle.Size;

            this.Update();
            Application.DoEvents();
        }
    }
}
