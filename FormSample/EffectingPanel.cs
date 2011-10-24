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

namespace Effecing
{
    public partial class EffectingPanel : Panel
    {
        public enum EffectType { Fading, Rotating, L2RSliding, Random, None };
        public PictureBox pictureBox = null;

        private ArrayList effectList = null;
        private Random random;

        public EffectingPanel(Form form)
        {
            InitializeComponent();

            // PictureBoxの作成
            pictureBox = new PictureBox();

            pictureBox.AutoSize = true;
            pictureBox.BackColor = Color.Black;
            pictureBox.Location = new Point(0, 0);
            pictureBox.Size = new Size(this.Width, this.Height);
            pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(pictureBox);

            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Dock = System.Windows.Forms.DockStyle.Fill;            // 親コンテナにドッキング
            SetSize(form);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);                  // 背景を描画しない（ちらつきの抑制）
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);               // OSではなく独自で描画する
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);    // WM_ERASEBKGND を無視
            this.DoubleBuffered = true;

            this.BringToFront();
            this.Visible = false;

            // エフェクト効果を行うクラスのインスタンスを生成
            CreateEffectInstances();

            random = new Random();
        }

        private void CreateEffectInstances()
        {
            effectList = new ArrayList();

            effectList.Add(new EpFadingEffect());
            effectList.Add(new EpRotatingEffect());
            effectList.Add(new EpL2RSlidingEffect());
        }

        public void Transition(ref Panel current, ref Panel next)
        {
            Transition(ref current, ref next, EffectType.Random);
        }

        public void Transition(ref Panel current, ref Panel next, EffectType type)
        {
            Bitmap currentBitmap = null;
            Bitmap nextBitmap = null;
            EpDefaultEffect effect = null;

            try
            {
                currentBitmap = GetPreviousCapturedImage(current, current.Name + ".bmp", false);    // 遷移前Panelをキャプチャ
                nextBitmap = null;

                string nextBitmapPath = next.Name + ".bmp";

                if (System.IO.File.Exists(nextBitmapPath))
                {
                    nextBitmap = new Bitmap(nextBitmapPath);
                }
                else
                {
                    nextBitmap = GetPreviousCapturedImage(next, nextBitmapPath, true);              // 初回のみ
                }

                this.Visible = true;                                // effectスタート
                current.Visible = false;

                if (type == EffectType.Random)
                {
                    type = (EffectType)random.Next(effectList.Count);
                }

                if (type == EffectType.None)
                {
                    effect = new EpDefaultEffect();
                }
                else
                {
                    effect = effectList[(int)type] as EpDefaultEffect;
                }
                
                effect.DrawEffectImage(currentBitmap, nextBitmap, this);

                next.Visible = true;
                next.Refresh();

                this.Visible = false;                               // effect終わり

                currentBitmap.Dispose();
                nextBitmap.Dispose();
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Bitmap GetPreviousCapturedImage(Panel panel, string filePath, Boolean firstTime)
        {
            Rectangle rectangle;
            Bitmap bitmap = null;
            ArrayList controls = null;

            try
            {
                rectangle = RectangleToScreen(panel.Bounds);
                bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
                if (firstTime)
                {
                    panel.DrawToBitmap(bitmap, panel.Bounds);   // 再帰的にコンテナ及びコントロールをキャプチャ

                    controls = GetAllControls(panel);
                    controls.Reverse(); // 背面から
                    foreach (Control c in controls)
                    {
                        Rectangle rectangle2 = c.Bounds;
                        Control control = c;
                        while (control.Bounds.Location != panel.Bounds.Location)
                        {
                            rectangle2.X += control.Parent.Bounds.Location.X;
                            rectangle2.Y += control.Parent.Bounds.Location.Y;
                            control = control.Parent;
                        }
                        c.DrawToBitmap(bitmap, rectangle2);
                    }
                }
                else
                {
                    CaptureControls(panel, ref bitmap);
                }
                bitmap.Save(filePath, ImageFormat.Bmp);    // 保存する場合
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return bitmap;
        }

        private ArrayList GetAllControls(Control top)
        {
            ArrayList arrayList = new ArrayList();

            foreach (Control c in top.Controls)
            {
                arrayList.AddRange(GetAllControls(c));
                arrayList.Add(c);
            }
            return arrayList;
        }

        public void SetSize(Form form)
        {
            Rectangle rectangle = form.ClientRectangle;
            this.Location = rectangle.Location;
            this.Size = rectangle.Size;

            this.Update();
        }


        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private extern static bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        /// <summary>
        /// コントロールのイメージを取得する
        /// </summary>
        /// <param name="ctrl">キャプチャするコントロール</param>
        /// <returns>取得できたイメージ</returns>
        public Bitmap CaptureControls(Control control, ref Bitmap bitmap)
        {
            Graphics g;
            IntPtr hdc;
            
            g = Graphics.FromImage(bitmap);
            hdc = g.GetHdc();
            PrintWindow(control.Handle, hdc, 0);
            g.ReleaseHdc(hdc);
            g.Dispose();
            return bitmap;
        }
    }
}
