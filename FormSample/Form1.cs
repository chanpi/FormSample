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
        ArrayList panelList;
        int panelCount;
        int panelIndex = 0;
        //bool animationPanelIsTopOrder = false;
        static EffectablePanel myPanel = new EffectablePanel();

        public Form1()
        {
            InitializeComponent();
            InitializePanels();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // panelの切り替え
            Panel panel, before, after;
            before = panelList[panelIndex] as Panel;

            // Effectスタート
            myPanel.StartScreenTransition(ref before, true);

            panelIndex++;
            if (panelCount <= panelIndex)
            {
                for (int i = 1; i < panelCount; i++)
                {
                    panel = panelList[i] as Panel;
                    panel.Visible = false;
                }

                panelIndex = 0;
            }
            after = panelList[panelIndex] as Panel;

            // Effectストップ
            myPanel.EndScreenTransition(ref after);
        }

        private void InitializePanels()
        {
            panelList = new ArrayList();
            panelList.Add(panel1);
            panelList.Add(panel2);
            panelList.Add(panel3);

            panel2.Visible = false;
            panel3.Visible = false;
            panelCount = panelList.Count;
            panelIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            myPanel.Location = new System.Drawing.Point(0, 0);
            myPanel.Width = 700;
            myPanel.Height = 300;
            myPanel.Visible = false;
            this.Controls.Add(myPanel);
            myPanel.BringToFront();
        }
    }
}
