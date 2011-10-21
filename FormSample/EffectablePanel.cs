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

namespace Effectable
{
    public partial class EffectablePanel : Panel
    {
        public enum EffectType { Fading, Rotating, L2RSliding, None, Random };
        private ArrayList effectList = null;
        private Hashtable bitmapTable = null;

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

            bitmapTable = new Hashtable();
        }

        private void CreateEffectInstances()
        {
            effectList = new ArrayList();
            effectList.Add(new EpFadingEffect());
       //     effectList.Add(new EpL2RSlidingEffect());
            effectList.Add(new EpRotatingEffect());
        //    effectList.Add(new EpDefaultEffect());
        }

        public void Transition(Panel current, Panel next)
        {
            Transition(current, next, EffectType.Random);
        }

        public void Transition(Panel current, Panel next, EffectType type)
        {
            Bitmap currentBitmap;
            Bitmap nextBitmap;
            EpDefaultEffect effect;

            try{
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
                
                this.Visible = true;        // effectスタート
                current.Visible = false;

                if (type == EffectType.Random) {
                    type = (EffectType)new System.Random().Next(effectList.Count);
                }
                effect = effectList[(int)type] as EpDefaultEffect;                  // effectを実行
                effect.DrawEffectImage(currentBitmap, nextBitmap, this);

                next.Visible = true;
                this.Visible = false;       // effect終わり

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
            Bitmap bitmap;
            ArrayList controls;

            rectangle = RectangleToScreen(panel.Bounds);
            bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
            if (firstTime)
            {
                rectangle = RectangleToScreen(panel.Bounds);
                
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
                    control.DrawToBitmap(bitmap, rectangle2);
                }
            }
            else
            {
                CaptureControl(panel, ref bitmap);
            }
            bitmap.Save(filePath, ImageFormat.Bmp);    // 保存する場合
            return bitmap;
        }

        private ArrayList GetAllControls(Control top)
        {
            ArrayList arrayList;

            arrayList = new ArrayList();
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
        public Bitmap CaptureControl(Control control, ref Bitmap bitmap)
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
