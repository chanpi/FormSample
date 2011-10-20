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
        private ArrayList panelList = null;
        private EffectablePanel myPanel = null;
        private int panelIndex = 0;
        private int panelCount = 0;

        public Form1()
        {
            InitializeComponent();

            panelList = new ArrayList();
            panelList.Add(panel1);
            panelList.Add(panel2);
            panelList.Add(panel3);

            // 最初に表示するPanel以外はあらかじめ非表示にしておく
            panel2.Visible = false;
            panel3.Visible = false;
            panelCount = panelList.Count;

            // エフェクト用のPanelを作成
            myPanel = new EffectablePanel(this);
            this.Controls.Add(myPanel);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 遷移前、遷移後のPanelとエフェクトのタイプを指定する
            Panel current = panelList[panelIndex] as Panel;
            if (++panelIndex >= panelCount) {
                panelIndex = 0;
            }
            Panel next = panelList[panelIndex] as Panel;
            myPanel.Transition(current, next, EffectablePanel.EffectType.Fading);
        }
    }
}
