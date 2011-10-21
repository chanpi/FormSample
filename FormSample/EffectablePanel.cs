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
        public enum EffectType { Fading, L2RSliding, Rotating, None };
        private ArrayList effects = null;
        private Hashtable bmpContainer = null;

        public PictureBox pictureBox = null;

        public EffectablePanel(Form form)
        {
            InitializeComponent();

            pictureBox = new PictureBox();
            this.Controls.Add(pictureBox);
            pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;

            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Dock = System.Windows.Forms.DockStyle.Fill;            // 親コンテナにドッキング
            SetSize(form);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);                  // 背景を描画しない（ちらつきの抑制）
            this.SetStyle(ControlStyles.UserPaint, true);               // OSではなく独自で描画する
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);    // WM_ERASEBKGND を無視
            //this.SetStyle(ControlStyles.DoubleBuffer, true);          // 逆にちらつくのでコメントアウト
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);          // 逆にちらつくのでコメントアウト
            this.DoubleBuffered = true;

            this.BringToFront();
            this.Visible = false;

            // エフェクト効果を行うクラスのインスタンスを生成
            CreateEffectInstances();

            bmpContainer = new Hashtable();
        }

        private void CreateEffectInstances()
        {
            effects = new ArrayList();
            effects.Add(new FadingEffect());
            effects.Add(new L2RSlidingEffect());
            effects.Add(new RotatingEffect());
            effects.Add(new NoneEffect());
        }

        public void Transition(Panel current, Panel next, EffectType type)
        {
            try
            {
                // 遷移前Panelをキャプチャ
                Bitmap currentBmp = GetPreviousCapturedImage(current, current.Name + ".bmp", false);
                Bitmap nextBmp = null;

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

                // effectを実行
                DefaultEffect effect = effects[(int)type] as DefaultEffect;
                effect.DrawEffectImage(currentBmp, nextBmp, this);

                next.Visible = true;

                // effect終わり
                this.Visible = false;

                currentBmp.Dispose();
                nextBmp.Dispose();
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
                Application.DoEvents();

                // 再帰的にコンテナ及びコントロールをキャプチャ
                panel.DrawToBitmap(bmp, panel.Bounds);

                ArrayList controls = GetAllControls(panel);
                controls.Reverse(); // 背面から
                foreach (Control c in controls)
                {
                    Rectangle rc = c.Bounds;
                    Control tmp = c;
                    while (tmp.Bounds.Location != panel.Bounds.Location)
                    {
                        rc.X += tmp.Parent.Bounds.Location.X;
                        rc.Y += tmp.Parent.Bounds.Location.Y;
                        tmp = tmp.Parent;
                    }
                    c.DrawToBitmap(bmp, rc);
                }
            }
            else
            {
                CaptureControl(panel, ref bmp);
            }
            bmp.Save(filePath, ImageFormat.Bmp);    // 保存する場合
            return bmp;
        }

        private ArrayList GetAllControls(Control top)
        {
            ArrayList buf = new ArrayList();
            foreach (Control c in top.Controls)
            {
                buf.AddRange(GetAllControls(c));
                buf.Add(c);
            }
            return buf;
        }

        public void SetSize(Form form)
        {
            Rectangle clientRectangle = form.ClientRectangle;
            this.Location = clientRectangle.Location;
            this.Size = clientRectangle.Size;

            this.Update();
            Application.DoEvents();
        }


        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private extern static bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        /// <summary>
        /// コントロールのイメージを取得する
        /// </summary>
        /// <param name="ctrl">キャプチャするコントロール</param>
        /// <returns>取得できたイメージ</returns>
        public Bitmap CaptureControl(Control ctrl, ref Bitmap bmp)
        {
            Graphics memg = Graphics.FromImage(bmp);
            IntPtr dc = memg.GetHdc();
            PrintWindow(ctrl.Handle, dc, 0);
            memg.ReleaseHdc(dc);
            memg.Dispose();
            return bmp;
        }
    }
}
