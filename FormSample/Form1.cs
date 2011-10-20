using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormSample
{
    public partial class Form1 : Form
    {
        private ArrayList panelList = new ArrayList();
        private EffectablePanel myPanel = new EffectablePanel();
        private int panelIndex = 0;
        private int panelCount = 0;

        public Form1()
        {
            InitializeComponent();

            panelList.Add(panel1);
            panelList.Add(panel2);
            panelList.Add(panel3);

            // 最初に表示するPanel以外はあらかじめ非表示にしておく
            panel2.Visible = false;
            panel3.Visible = false;
            panelCount = panelList.Count;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Panel current = panelList[panelIndex] as Panel;
            if (++panelIndex >= panelCount) {
                panelIndex = 0;
            }
            Panel next = panelList[panelIndex] as Panel;
            myPanel.TransPanel(current, next, EffectablePanel.EffectType.Fading);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            myPanel.Location = new System.Drawing.Point(0, 0);
            myPanel.Width = this.Bounds.Width;
            myPanel.Height = this.Bounds.Height;
            myPanel.Visible = false;
            this.Controls.Add(myPanel);
            myPanel.BringToFront();
        }
    }
}
