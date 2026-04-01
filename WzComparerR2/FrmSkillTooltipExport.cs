using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WzComparerR2.WzLib;

namespace WzComparerR2
{
    public partial class FrmSkillTooltipExport : DevComponents.DotNetBar.Office2007Form
    {
        public FrmSkillTooltipExport()
        {
            InitializeComponent();
#if NET6_0_OR_GREATER
            // https://learn.microsoft.com/en-us/dotnet/core/compatibility/fx-core#controldefaultfont-changed-to-segoe-ui-9pt
            this.Font = new Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
#endif
            this.UpdateList();
        }

        public string ExportFolderPath { get; private set; }
        public List<int> SelectedJobCodes { get; private set; }
        public Wz_Node skillNode { get; set; }
        private bool sorted = false;

        private static Dictionary<string, int[]> jobNameToCode = new Dictionary<string, int[]>()
        {
            { "英雄", new int[] { 100, 110, 111, 112, 114 } }, 
            { "圣骑士", new int[] { 100, 120, 121, 122, 124 } }, 
            { "黑骑士", new int[] { 100, 130, 131, 132, 134 } }, 
            { "魔导师（火/毒）", new int[] { 200, 210, 211, 212, 214 } }, 
            { "魔导师（冰/雷）", new int[] { 200, 220, 221, 222, 224 } }, 
            { "主教", new int[] { 200, 230, 231, 232, 234 } }, 
            { "神射手", new int[] { 300, 310, 311, 312, 314 } }, 
            { "箭神", new int[] { 300, 320, 321, 322, 324 } }, 
            { "古迹猎人", new int[] { 301, 330, 331, 332, 334 } }, 
            { "隐士", new int[] { 400, 410, 411, 412, 414 } }, 
            { "侠盗", new int[] { 400, 420, 421, 422, 424 } }, 
            { "暗影双刀", new int[] { 400, 430, 431, 432, 433, 434, 436 } }, 
            { "冲锋队长", new int[] { 500, 510, 511, 512, 514 } }, 
            { "船长", new int[] { 500, 520, 521, 522, 524 } }, 
            { "火炮手", new int[] { 501, 530, 531, 532, 534 } }, 
            // { "용의 전인 && 제트", new int[] { 508, 570, 571, 572, 574 } }, 
            { "魂骑士", new int[] { 1000, 1100, 1110, 1111, 1112, 1114 } }, 
            { "炎术士", new int[] { 1000, 1200, 1210, 1211, 1212, 1214 } }, 
            { "风灵使者", new int[] { 1000, 1300, 1310, 1311, 1312, 1314 } }, 
            { "夜行者", new int[] { 1000, 1400, 1410, 1411, 1412, 1414 } }, 
            { "奇袭者", new int[] { 1000, 1500, 1510, 1511, 1512, 1514 } }, 
            { "战神", new int[] { 2000, 2100, 2110, 2111, 2112, 2114 } }, 
            { "龙神", new int[] { 2001, 2200, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220 } }, 
            { "双弩精灵", new int[] { 2002, 2300, 2310, 2311, 2312, 2314 } }, 
            { "幻影", new int[] { 2003, 2400, 2410, 2411, 2412, 2414 } }, 
            { "夜光法师", new int[] { 2004, 2700, 2710, 2711, 2712, 2714 } }, 
            { "隐月", new int[] { 2005, 2500, 2510, 2511, 2512, 2514 } }, 
            { "恶魔猎手", new int[] { 3001, 3100, 3110, 3111, 3112, 3114 } }, 
            { "恶魔复仇者", new int[] { 3001, 3101, 3120, 3121, 3122, 3124 } }, 
            { "爆破手", new int[] { 3000, 3700, 3710, 3711, 3712, 3714 } }, 
            { "唤灵斗师", new int[] { 3000, 3200, 3210, 3211, 3212, 3214 } }, 
            { "豹弩游侠", new int[] { 3000, 3300, 3310, 3311, 3312, 3314 } }, 
            { "机械师", new int[] { 3000, 3500, 3510, 3511, 3512, 3514 } }, 
            { "尖兵", new int[] { 3002, 3600, 3610, 3611, 3612, 3614 } }, 
            { "剑豪", new int[] { 4001, 4100, 4110, 4111, 4112, 4114 } }, 
            { "阴阳师", new int[] { 4002, 4200, 4210, 4211, 4212, 4214 } }, 
            { "米哈尔", new int[] { 5000, 5100, 5110, 5111, 5112, 5114 } }, 
            { "狂龙战士", new int[] { 6000, 6100, 6110, 6111, 6112, 6114 } }, 
            { "炼狱黑客", new int[] { 6003, 6300, 6310, 6311, 6312, 6314 } }, 
            { "魔链影士", new int[] { 6002, 6400, 6410, 6411, 6412, 6414 } }, 
            { "爆莉萌天使", new int[] { 6001, 6500, 6510, 6511, 6512, 6514 } }, 
            // { "内在能力", new int[] { 7000 } }, 
            // { "联盟", new int[] { 7100 } }, 
            // { "怪物农庄", new int[] { 7200 } }, 
            // { "公会", new int[] { 9100 } }, 
            // { "专业技术", new int[] { 9200, 9201, 9202, 9203, 9204 } }, 
            { "神之子", new int[] { 10000, 10100, 10110, 10111, 10112, 10114 } }, 
            // { "林之灵", new int[] { 11000, 11200, 11210, 11211, 11212 } }, 
            { "灶门炭治郎", new int[] { 12000, 12005, 12100 } }, 
            { "琦玉", new int[] { 12006, 12200 } }, 
            { "品克缤", new int[] { 13000, 13100 } }, 
            { "白雪人", new int[] { 13001, 13500 } }, 
            { "超能力者", new int[] { 14000, 14200, 14210, 14211, 14212, 14214 } }, 
            { "御剑骑士", new int[] { 15002, 15100, 15110, 15111, 15112, 15114 } }, 
            { "圣晶使徒", new int[] { 15000, 15200, 15210, 15211, 15212, 15214 } }, 
            { "飞刃沙士", new int[] { 15003, 15400, 15410, 15411, 15412, 15414 } }, 
            { "影魂异人", new int[] { 15001, 15500, 15510, 15511, 15512, 15514 } }, 
            { "莲", new int[] { 16002, 16100, 16110, 16111, 16112, 16114 } }, 
            { "元素师", new int[] { 16001, 16200, 16210, 16211, 16212, 16214 } }, 
            { "虎影", new int[] { 16000, 16400, 16410, 16411, 16412, 16414 } }, 
            { "墨玄", new int[] { 17000, 17500, 17510, 17511, 17512, 17514 } }, 
            { "琳", new int[] { 17001, 17200, 17210, 17211, 17212, 17214 } }, 
            // { "叶里莱特", new int[] { 18001, 18100, 18110, 18111, 18112, 18114 } }, 
            { "施亚阿斯特", new int[] { 18000, 18200, 18210, 18211, 18212, 18214 } }, 
            // { "艾伊尔", new int[] { 18002, 18300, 18310, 18311, 18312, 18314 } }, 
            { "5转(其它)", new int[] { 40000, 40001, 40002, 40003, 40004, 40005 } }, 
            { "6转(其它)", new int[] { 50000, 50006, 50007 } },
        };

        private static HashSet<int> AllClassesCode()
        {
            HashSet<int> hsClassCode = new HashSet<int>() { };
            foreach (var i in jobNameToCode.Values)
            {
                foreach (var j in i)
                {
                    hsClassCode.Add(j);
                }
            }
            return hsClassCode;
        }

        private void clbJobName_MouseDown(object sender, MouseEventArgs e)
        {
            return;
            var clb = sender as CheckedListBox;
            int index = clb.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                bool isChecked = clb.GetItemChecked(index);
                clb.SetItemChecked(index, !isChecked);
            }
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            this.sorted = !this.sorted;
            this.btnSort.Text = this.sorted ? "字母顺序" : "默认顺序";
            this.UpdateList();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            int checkedCount = this.clbJobName.CheckedItems.Contains("其它") ? this.clbJobName.CheckedItems.Count - 1 : this.clbJobName.CheckedItems.Count;
            bool checkedStatus = (checkedCount < this.clbJobName.Items.Count - 1);
            for (int i = 1; i < this.clbJobName.Items.Count; i++)
            {
                this.clbJobName.SetItemChecked(i, checkedStatus);
            }
        }

        private void btnReverseSelect_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < this.clbJobName.Items.Count; i++)
            {
                this.clbJobName.SetItemChecked(i, !this.clbJobName.GetItemChecked(i));
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.clbJobName.CheckedItems.Count == 0)
            {
                MessageBoxEx.Show("请选择导出职业。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择导出文件夹。";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                bool allSelected = this.clbJobName.CheckedItems.Count == this.clbJobName.Items.Count;

                List<int> skillImg = new List<int>() { };
                foreach (Wz_Node node in skillNode.Nodes)
                {
                    Wz_Image currentImg = node.GetValue<Wz_Image>();
                    if (currentImg != null && Int32.TryParse(currentImg.Name.Replace(".img", ""), out int jobCode))
                    {
                        skillImg.Add(jobCode);
                    }
                }
                List<int> selectedJob = skillImg.Intersect(this.clbJobName.CheckedItems.Cast<string>().SelectMany(name => jobNameToCode.ContainsKey(name) ? jobNameToCode[name] : new int[] { }).ToList()).ToList();
                if (this.clbJobName.CheckedItems.Contains("其它"))
                {
                    selectedJob.AddRange(skillImg.Except(AllClassesCode()));
                }
                ExportFolderPath = dlg.SelectedPath;
                SelectedJobCodes = allSelected ? skillImg : selectedJob;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void UpdateList()
        {
            var checkedList = this.clbJobName.CheckedItems.Cast<object>().ToList();
            this.clbJobName.Items.Clear();
            this.clbJobName.Items.Add("其它", checkedList.Contains("其它"));
            if (!this.sorted)
            {
                foreach (var i in jobNameToCode.Keys)
                {
                    this.clbJobName.Items.Add(i, checkedList.Contains(i));
                }
            }
            else
            {
                foreach (var i in jobNameToCode.Keys.OrderBy(kv => kv))
                {
                    this.clbJobName.Items.Add(i, checkedList.Contains(i));
                }
            }
        }
    }
}
