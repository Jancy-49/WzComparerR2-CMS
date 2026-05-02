using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CharaSimResource;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;
using WzComparerR2.WzLib;
using Newtonsoft.Json.Linq;
using SharpDX.Win32;
using System.Globalization;
using DevComponents.DotNetBar;
using SharpDX.DirectWrite;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmUnion: AlphaForm
    {
        public AfrmUnion()
        {
            InitUnion();
        }

        private Point baseOffset;
        private Point newLocation;
        private Character character;
        private ContextMenuStrip menu;
        private ACtrlVScroll vScroll;
        private ACtrlVScroll vScroll2;
        private ACtrlVScroll vScroll3;
        private ACtrlButton btnClose;
        private ACtrlButton btnAttacker;
        private ACtrlButton btnArtifact;
        private ACtrlButton btnChampion;
        private ACtrlButton btnstartRaid;
        private ACtrlButton btncoin;
        private ACtrlButton btnartifactMission;
        private ACtrlButton btnhelp;
        private ACtrlButton btnLeft;
        private ACtrlButton btnRight;
        private ACtrlButton btnresetApply;
        private ACtrlButton btnpresetEdit;
        private ACtrlButton btnpresetSave;
        private ACtrlButton btnrollback;
        private ACtrlButton btnclear;
        private ACtrlButton btnslotPointReset;
        private ACtrlButton btnartifactSync;
        private ACtrlButton btnSTR;
        private ACtrlButton btnDEX;
        private ACtrlButton btnINT;
        private ACtrlButton btnLUK;
        private ACtrlButton btnPAD;
        private ACtrlButton btnMAD;
        private ACtrlButton btnHP;
        private ACtrlButton btnMP;
        private ACtrlButton btnPresetPage1;
        private ACtrlButton btnPresetPage2;
        private ACtrlButton btnPresetPage3;
        private ACtrlButton btnPresetPage4;
        private ACtrlButton btnPresetPage5;
        private bool UIattacker = true;
        private bool UIartifact = false;
        private bool UIchampion = false;
        private bool UIdeploy = true;
        private bool UIapply = false;
        private int scrollValue = 0;
        private int scrollValue2 = 0;
        private int scrollValue3 = 0;
        public string union_level = "9500";
        public string union_grade = "그랜드 마스터 유니온 4";
        private string attackers = "39";
        public string union_attackpower = "999999999";
        public string champion_power = "0";
        private string union_coin = "0";
        private string champion_coin = "0";
        public int union_artifact_level = 10;
        public int union_artifact_exp = 500;
        public string union_artifact_point = "10000";
        public string available_coin = "0";
        public string artifact_ap = "0";
        public JObject resultJson = null;
        public JObject resultJson2 = null;
        public JObject resultJson3 = null;
        public UnionBlock[] union_block = Array.Empty<UnionBlock>();
        public UnionInnerStat[] union_inner_stat = Array.Empty<UnionInnerStat>();
        public UnionChampion[] union_champion = Array.Empty<UnionChampion>();
        public UnionBadgeInfo[] champion_badge_total_info = Array.Empty<UnionBadgeInfo>();
        public UnionArtifactEffect[] union_artifact_effect = Array.Empty<UnionArtifactEffect>();
        public UnionArtifactCrystal[] union_artifact_crystal = Array.Empty<UnionArtifactCrystal>();
        public List<string> union_raider_stat = new List<string>();
        public List<string> union_occupied_stat = new List<string>();
        public int union_preset = 1;
        private bool waitForRefresh;

        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        public class BlockControlPoint
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class BlockPosition
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class UnionBlock
        {
            public string block_type { get; set; }
            public string block_class { get; set; }
            public string block_level { get; set; }
            public BlockControlPoint block_control_point { get; set; }
            public BlockPosition[] block_position { get; set; }
        }

        public class UnionInnerStat
        {
            public int stat_field_id { get; set; }
            public string stat_field_effect { get; set; }
        }

        public class UnionChampion
        {
            public int champion_slot { get; set; }
            public string champion_grade { get; set; }
        }

        public class UnionBadgeInfo
        {
            public string stat { get; set; }
        }

        public class UnionArtifactEffect
        {
            public string name { get; set; }
            public int level { get; set; }
        }

        public class UnionArtifactCrystal
        {
            public string name { get; set; }
            public string validity_flag { get; set; }
            public int level { get; set; }
            public string date_expire { get; set; }
            public string crystal_option_name_1 { get; set; }
            public string crystal_option_name_2 { get; set; }
            public string crystal_option_name_3 { get; set; }
        }

        private List<Point> areaPos = new List<Point>
        {
            new Point(287, 183), new Point(344, 183), new Point(392, 215), new Point(392, 260),
            new Point(344, 293), new Point(287, 293), new Point(239, 260), new Point(239, 215)
        };

        private List<int> artifact_exp_list = new List<int>
        {
            2500, 2550, 2600, 2650, 2700, 2750, 2800, 2850, 2900, 2950, 3000, 3050, 3100, 3150, 3200, 3250, 3300, 3350, 3400, 3450,
            3500, 3550, 3600, 3700, 3800, 3900, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000, 12000,
            14000, 16000, 18000, 20000, 22000, 24000, 26000, 28000, 30000, 50000, 55000, 60000, 65000, 70000, 100000, 110000, 120000, 130000, 500000, 0
        };

        private void InitUnion()
        {
            this.menu = new ContextMenuStrip();
            this.menu.Items.Add(new ToolStripMenuItem("复制", null, tsmiCopy_Click));
            this.menu.Items.Add(new ToolStripMenuItem("保存PNG", null, tsmiSave_Click));
            this.MouseClick += AfrmUnion_MouseClick;

            this.vScroll = new ACtrlVScroll();  //攻击队员效果滚轮

            this.vScroll.PicBase.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_base);
            this.vScroll.PicBase.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_disabled_base);

            this.vScroll.BtnPrev.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll.BtnPrev.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev1);
            this.vScroll.BtnPrev.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev2);
            this.vScroll.BtnPrev.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll.BtnPrev.Location = new Point(0, 0);

            this.vScroll.BtnNext.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll.BtnNext.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next1);
            this.vScroll.BtnNext.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next2);
            this.vScroll.BtnNext.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll.BtnNext.Location = new Point(0, 117);

            this.vScroll.BtnThumb.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb0);
            this.vScroll.BtnThumb.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb1);
            this.vScroll.BtnThumb.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb2);
            this.vScroll.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll.Location = new Point(805, 544);
            this.vScroll.Size = new Size(5, 145);
            this.vScroll.ScrollableLocation = new Point(640, 544);
            this.vScroll.ScrollableSize = new Size(175, 145);
            this.vScroll.Value = 0;
            this.vScroll.ValueChanged += new EventHandler(vScroll_ValueChanged);
            this.vScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.vScroll2 = new ACtrlVScroll();  //攻击队占领效果滚轮

            this.vScroll2.PicBase.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_base);
            this.vScroll2.PicBase.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_disabled_base);

            this.vScroll2.BtnPrev.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll2.BtnPrev.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev1);
            this.vScroll2.BtnPrev.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev2);
            this.vScroll2.BtnPrev.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll2.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll2.BtnPrev.Location = new Point(0, 0);

            this.vScroll2.BtnNext.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll2.BtnNext.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next1);
            this.vScroll2.BtnNext.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next2);
            this.vScroll2.BtnNext.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll2.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll2.BtnNext.Location = new Point(0, 117);

            this.vScroll2.BtnThumb.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb0);
            this.vScroll2.BtnThumb.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb1);
            this.vScroll2.BtnThumb.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb2);
            this.vScroll2.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll2.Location = new Point(990, 544);
            this.vScroll2.Size = new Size(5, 145);
            this.vScroll2.ScrollableLocation = new Point(825, 544);
            this.vScroll2.ScrollableSize = new Size(175, 145);
            this.vScroll2.Value = 0;
            this.vScroll2.ValueChanged += new EventHandler(vScroll2_ValueChanged);
            this.vScroll2.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.vScroll3 = new ACtrlVScroll();  //神器效果滚轮

            this.vScroll3.PicBase.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_base);
            this.vScroll3.PicBase.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_disabled_base);

            this.vScroll3.BtnPrev.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll3.BtnPrev.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev1);
            this.vScroll3.BtnPrev.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev2);
            this.vScroll3.BtnPrev.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_prev0);
            this.vScroll3.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll3.BtnPrev.Location = new Point(0, 0);

            this.vScroll3.BtnNext.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll3.BtnNext.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next1);
            this.vScroll3.BtnNext.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next2);
            this.vScroll3.BtnNext.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_next0);
            this.vScroll3.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll3.BtnNext.Location = new Point(0, 117);

            this.vScroll3.BtnThumb.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb0);
            this.vScroll3.BtnThumb.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb1);
            this.vScroll3.BtnThumb.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_scroll_attackerScroll_enabled_thumb2);
            this.vScroll3.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll3.Location = new Point(990, 544);
            this.vScroll3.Size = new Size(5, 145);
            this.vScroll3.ScrollableLocation = new Point(640, 544);
            this.vScroll3.ScrollableSize = new Size(360, 145);
            this.vScroll3.Value = 0;
            this.vScroll3.ValueChanged += new EventHandler(vScroll3_ValueChanged);
            this.vScroll3.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton(); //主页关闭按钮
            this.btnClose.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclose_normal_0);
            this.btnClose.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclose_pressed_0);
            this.btnClose.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclose_mouseOver_0);
            this.btnClose.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclose_disabled_0);
            this.btnClose.Location = new Point(994, 18);
            this.btnClose.Size = new Size(11, 11);
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);

            this.btnAttacker = new ACtrlButton(); //攻击者按钮
            this.btnAttacker.Normal = new BitmapOrigin(Resource.mapleUnion_buttonattackerSetting_normal_0);
            this.btnAttacker.Pressed = new BitmapOrigin(Resource.mapleUnion_buttonattackerSetting_pressed_0);
            this.btnAttacker.MouseOver = new BitmapOrigin(Resource.mapleUnion_buttonattackerSetting_mouseOver_0);
            this.btnAttacker.Disabled = new BitmapOrigin(Resource.mapleUnion_buttonattackerSetting_disabled_0);
            this.btnAttacker.Location = new Point(0, 12);
            this.btnAttacker.Size = new Size(29, 122);
            this.btnAttacker.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnAttacker.MouseClick += new MouseEventHandler(btnAttacker_MouseClick);

            this.btnArtifact = new ACtrlButton(); //神器按钮
            this.btnArtifact.Normal = new BitmapOrigin(Resource.mapleUnion_buttonartifactSetting_normal_0);
            this.btnArtifact.Pressed = new BitmapOrigin(Resource.mapleUnion_buttonartifactSetting_pressed_0);
            this.btnArtifact.MouseOver = new BitmapOrigin(Resource.mapleUnion_buttonartifactSetting_mouseOver_0);
            this.btnArtifact.Disabled = new BitmapOrigin(Resource.mapleUnion_buttonartifactSetting_disabled_0);
            this.btnArtifact.Location = new Point(0, 136);
            this.btnArtifact.Size = new Size(29, 122);
            this.btnArtifact.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnArtifact.MouseClick += new MouseEventHandler(btnArtifact_MouseClick);
            
            this.btnChampion = new ACtrlButton(); //神器按钮
            this.btnChampion.Normal = new BitmapOrigin(Resource.mapleUnion_button_championSetting_normal_0);
            this.btnChampion.Pressed = new BitmapOrigin(Resource.mapleUnion_button_championSetting_disabled_0);
            this.btnChampion.MouseOver = new BitmapOrigin(Resource.mapleUnion_button_championSetting_mouseOver_0);
            this.btnChampion.Disabled = new BitmapOrigin(Resource.mapleUnion_button_championSetting_disabled_0);
            this.btnChampion.Location = new Point(0, 260);
            this.btnChampion.Size = new Size(29, 122);
            this.btnChampion.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnChampion.MouseClick += new MouseEventHandler(btnChampion_MouseClick);

            this.btncoin = new ACtrlButton(); //领取纪念币按钮
            this.btncoin.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_normal_0);
            this.btncoin.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_pressed_0);
            this.btncoin.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_mouseOver_0);
            this.btncoin.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_disabled_0);
            this.btncoin.Location = new Point(640, 423);
            this.btncoin.Size = new Size(121, 46);
            this.btncoin.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnartifactMission = new ACtrlButton(); //神器任务按钮
            this.btnartifactMission.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_normal_0);
            this.btnartifactMission.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_pressed_0);
            this.btnartifactMission.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_mouseOver_0);
            this.btnartifactMission.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_disabled_0);
            this.btnartifactMission.Location = new Point(762, 423);
            this.btnartifactMission.Size = new Size(121, 46);
            this.btnartifactMission.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnstartRaid = new ACtrlButton(); //冠军战场按钮
            this.btnstartRaid.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_normal_0);
            this.btnstartRaid.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_pressed_0);
            this.btnstartRaid.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_mouseOver_0);
            this.btnstartRaid.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_disabled_0);
            this.btnstartRaid.Location = new Point(884, 423);
            this.btnstartRaid.Size = new Size(121, 46);
            this.btnstartRaid.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnhelp = new ACtrlButton(); //帮助按钮
            this.btnhelp.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonhelp_normal_0);
            this.btnhelp.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonhelp_pressed_0);
            this.btnhelp.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonhelp_mouseOver_0);
            this.btnhelp.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonhelp_disabled_0);
            this.btnhelp.Location = new Point(49, 677);
            this.btnhelp.Size = new Size(32, 24);
            this.btnhelp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnLeft = new ACtrlButton(); //左页按钮
            this.btnLeft.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonLeft_normal_0);
            this.btnLeft.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonLeft_pressed_0);
            this.btnLeft.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonLeft_mouseOver_0);
            this.btnLeft.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonLeft_disabled_0);
            this.btnLeft.Location = new Point(51, 583);
            this.btnLeft.Size = new Size(31, 62);
            this.btnLeft.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnRight = new ACtrlButton(); //左页按钮
            this.btnRight.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonRight_normal_0);
            this.btnRight.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonRight_pressed_0);
            this.btnRight.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonRight_mouseOver_0);
            this.btnRight.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonRight_disabled_0);
            this.btnRight.Location = new Point(580, 587);
            this.btnRight.Size = new Size(31, 62);
            this.btnRight.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnresetApply = new ACtrlButton(); //应用按钮
            this.btnresetApply.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetApplication_normal_0);
            this.btnresetApply.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetApplication_pressed_0);
            this.btnresetApply.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetApplication_mouseOver_0);
            this.btnresetApply.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetApplication_disabled_0);
            this.btnresetApply.Location = new Point(366, 483);
            this.btnresetApply.Size = new Size(120, 28);
            this.btnresetApply.Visible = false;
            this.btnresetApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnresetApply.MouseClick += new MouseEventHandler(btnresetApply_MouseClick);
            
            this.btnpresetEdit = new ACtrlButton(); //部署攻击队按钮
            this.btnpresetEdit.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetEdit_normal_0);
            this.btnpresetEdit.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetEdit_pressed_0);
            this.btnpresetEdit.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetEdit_mouseOver_0);
            this.btnpresetEdit.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetEdit_disabled_0);
            this.btnpresetEdit.Location = new Point(490, 483);
            this.btnpresetEdit.Size = new Size(131, 28);
            this.btnpresetEdit.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnpresetEdit.MouseClick += new MouseEventHandler(btnpresetEdit_MouseClick);
            
            this.btnpresetSave = new ACtrlButton(); //部署完毕按钮
            this.btnpresetSave.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetSave_normal_0);
            this.btnpresetSave.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetSave_pressed_0);
            this.btnpresetSave.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetSave_mouseOver_0);
            this.btnpresetSave.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetSave_disabled_0);
            this.btnpresetSave.Location = new Point(490, 483);
            this.btnpresetSave.Size = new Size(131, 28);
            this.btnpresetSave.Visible = false;
            this.btnpresetSave.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnpresetSave.MouseClick += new MouseEventHandler(btnpresetSave_MouseClick);
            
            this.btnrollback = new ACtrlButton(); //撤回按钮
            this.btnrollback.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonrollback_normal_0);
            this.btnrollback.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonrollback_pressed_0);
            this.btnrollback.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonrollback_mouseOver_0);
            this.btnrollback.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonrollback_disabled_0);
            this.btnrollback.Location = new Point(366, 483);
            this.btnrollback.Size = new Size(46, 28);
            this.btnrollback.Visible = false;
            this.btnrollback.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnclear = new ACtrlButton(); //初始化按钮
            this.btnclear.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclear_normal_0);
            this.btnclear.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclear_pressed_0);
            this.btnclear.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclear_mouseOver_0);
            this.btnclear.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonclear_disabled_0);
            this.btnclear.Location = new Point(416, 483);
            this.btnclear.Size = new Size(70, 28);
            this.btnclear.Visible = false;
            this.btnclear.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnslotPointReset = new ACtrlButton(); //特殊能力点初始化按钮
            this.btnslotPointReset.Normal = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonslotPointReset_normal_0);
            this.btnslotPointReset.Pressed = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonslotPointReset_pressed_0);
            this.btnslotPointReset.MouseOver = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonslotPointReset_mouseOver_0);
            this.btnslotPointReset.Disabled = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonslotPointReset_disabled_0);
            this.btnslotPointReset.Location = new Point(193, 677);
            this.btnslotPointReset.Size = new Size(106, 24);
            this.btnslotPointReset.Visible = false;
            this.btnslotPointReset.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnartifactSync = new ACtrlButton(); //批量延长按钮
            this.btnartifactSync.Normal = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonartifactSync_normal_0);
            this.btnartifactSync.Pressed = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonartifactSync_pressed_0);
            this.btnartifactSync.MouseOver = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonartifactSync_mouseOver_0);
            this.btnartifactSync.Disabled = new BitmapOrigin(Resource.mapleUnion_artifactSetting_buttonartifactSync_disabled_0);
            this.btnartifactSync.Location = new Point(84, 677);
            this.btnartifactSync.Size = new Size(106, 24);
            this.btnartifactSync.Visible = false;
            this.btnartifactSync.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnSTR = new ACtrlButton(); //力量按钮
            this.btnSTR.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonSTR_normal_0);
            this.btnSTR.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonSTR_normal_0);
            this.btnSTR.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonSTR_mouseOver_0);
            this.btnSTR.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonSTR_normal_0);
            this.btnSTR.Size = new Size(35, 16);
            this.btnSTR.Visible = false;
            this.btnSTR.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnDEX = new ACtrlButton(); //敏捷按钮
            this.btnDEX.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonDEX_normal_0);
            this.btnDEX.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonDEX_normal_0);
            this.btnDEX.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonDEX_mouseOver_0);
            this.btnDEX.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonDEX_disabled_0);
            this.btnDEX.Size = new Size(35, 16);
            this.btnDEX.Visible = false;
            this.btnDEX.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnINT = new ACtrlButton(); //智力按钮
            this.btnINT.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonINT_normal_0);
            this.btnINT.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonINT_normal_0);
            this.btnINT.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonINT_mouseOver_0);
            this.btnINT.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonINT_normal_0);
            this.btnINT.Size = new Size(35, 16);
            this.btnINT.Visible = false;
            this.btnINT.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnLUK = new ACtrlButton(); //运气按钮
            this.btnLUK.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonLUK_normal_0);
            this.btnLUK.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonLUK_normal_0);
            this.btnLUK.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonLUK_mouseOver_0);
            this.btnLUK.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonLUK_normal_0);
            this.btnLUK.Size = new Size(35, 16);
            this.btnLUK.Visible = false;
            this.btnLUK.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnPAD = new ACtrlButton(); //攻击力按钮
            this.btnPAD.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonPAD_normal_0);
            this.btnPAD.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonPAD_pressed_0);
            this.btnPAD.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonPAD_mouseOver_0);
            this.btnPAD.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonPAD_pressed_0);
            this.btnPAD.Size = new Size(43, 15);
            this.btnPAD.Visible = false;
            this.btnPAD.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnMAD = new ACtrlButton(); //魔力按钮
            this.btnMAD.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMAD_normal_0);
            this.btnMAD.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMAD_normal_0);
            this.btnMAD.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMAD_mouseOver_0);
            this.btnMAD.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMAD_normal_0);
            this.btnMAD.Size = new Size(34, 16);
            this.btnMAD.Visible = false;
            this.btnMAD.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnHP = new ACtrlButton(); //血量按钮
            this.btnHP.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonHP_normal_0);
            this.btnHP.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonHP_normal_0);
            this.btnHP.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonHP_mouseOver_0);
            this.btnHP.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonHP_normal_0);
            this.btnHP.Size = new Size(35, 16);
            this.btnHP.Visible = false;
            this.btnHP.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            
            this.btnMP = new ACtrlButton(); //魔量按钮
            this.btnMP.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMP_normal_0);
            this.btnMP.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMP_pressed_0);
            this.btnMP.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMP_mouseOver_0);
            this.btnMP.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_AreaButtons_buttonMP_normal_0);
            this.btnMP.Size = new Size(35, 16);
            this.btnMP.Visible = false;
            this.btnMP.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall); 
            
            this.btnPresetPage1 = new ACtrlButton(); //联盟预设1按钮
            this.btnPresetPage1.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage1_normal_0);
            this.btnPresetPage1.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage1_pressed_0);
            this.btnPresetPage1.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage1_mouseOver_0);
            this.btnPresetPage1.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage1_normal_0);
            this.btnPresetPage1.Location = new Point(256, 490);
            this.btnPresetPage1.Size = new Size(12, 12);
            this.btnPresetPage1.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall); 
            this.btnPresetPage1.MouseClick += new MouseEventHandler(btnPresetPage1_MouseClick); 
            
            this.btnPresetPage2 = new ACtrlButton(); //联盟预设2按钮
            this.btnPresetPage2.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage2_normal_0);
            this.btnPresetPage2.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage2_pressed_0);
            this.btnPresetPage2.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage2_mouseOver_0);
            this.btnPresetPage2.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage2_normal_0);
            this.btnPresetPage2.Location = new Point(276, 490);
            this.btnPresetPage2.Size = new Size(12, 12);
            this.btnPresetPage2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetPage2.MouseClick += new MouseEventHandler(btnPresetPage2_MouseClick);


            this.btnPresetPage3 = new ACtrlButton(); //联盟预设3按钮
            this.btnPresetPage3.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage3_normal_0);
            this.btnPresetPage3.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage3_pressed_0);
            this.btnPresetPage3.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage3_mouseOver_0);
            this.btnPresetPage3.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage3_normal_0);
            this.btnPresetPage3.Location = new Point(296, 490);
            this.btnPresetPage3.Size = new Size(12, 12);
            this.btnPresetPage3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetPage3.MouseClick += new MouseEventHandler(btnPresetPage3_MouseClick);

            this.btnPresetPage4 = new ACtrlButton(); //联盟预设4按钮
            this.btnPresetPage4.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage4_normal_0);
            this.btnPresetPage4.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage4_pressed_0);
            this.btnPresetPage4.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage4_mouseOver_0);
            this.btnPresetPage4.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage4_normal_0);
            this.btnPresetPage4.Location = new Point(316, 490);
            this.btnPresetPage4.Size = new Size(12, 12);
            this.btnPresetPage4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetPage4.MouseClick += new MouseEventHandler(btnPresetPage4_MouseClick);

            this.btnPresetPage5 = new ACtrlButton(); //联盟预设5按钮
            this.btnPresetPage5.Normal = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage5_normal_0);
            this.btnPresetPage5.Pressed = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage5_pressed_0);
            this.btnPresetPage5.MouseOver = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage5_mouseOver_0);
            this.btnPresetPage5.Disabled = new BitmapOrigin(Resource.mapleUnion_attackerSetting_buttonpresetPage5_normal_0);
            this.btnPresetPage5.Location = new Point(336, 490);
            this.btnPresetPage5.Size = new Size(12, 12);
            this.btnPresetPage5.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetPage5.MouseClick += new MouseEventHandler(btnPresetPage5_MouseClick);
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return this.vScroll;
                yield return this.vScroll2;
                yield return this.vScroll3;
                yield return this.btnClose;
                yield return this.btnAttacker;
                yield return this.btnArtifact;
                yield return this.btnChampion;
                yield return this.btnstartRaid;
                yield return this.btncoin;
                yield return this.btnartifactMission;
                yield return this.btnhelp;
                yield return this.btnLeft;
                yield return this.btnRight;
                yield return this.btnresetApply;
                yield return this.btnpresetEdit;
                yield return this.btnpresetSave;
                yield return this.btnrollback;
                yield return this.btnclear;
                yield return this.btnslotPointReset;
                yield return this.btnartifactSync;
                yield return this.btnSTR;
                yield return this.btnDEX;
                yield return this.btnINT;
                yield return this.btnLUK;
                yield return this.btnPAD;
                yield return this.btnMAD;
                yield return this.btnHP;
                yield return this.btnMP;
                yield return this.btnPresetPage1;
                yield return this.btnPresetPage2;
                yield return this.btnPresetPage3;
                yield return this.btnPresetPage4;
                yield return this.btnPresetPage5;
            }
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(Resource.mapleUnion_attackerSetting_backgrnd.Width, 24));
            this.Location = newLocation;
            base.Refresh();
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();

            control_event();
            Point baseOffsetnew = new Point(0, 0);
            Size size = Resource.mapleUnion_attackerSetting_backgrnd.Size;
            this.newLocation = new Point(this.Location.X + this.baseOffset.X - baseOffsetnew.X,
                this.Location.Y + this.baseOffset.Y - baseOffsetnew.Y);
            this.baseOffset = baseOffsetnew;

            //绘制背景
            Bitmap union = new Bitmap(size.Width + 29, size.Height);
            Graphics g = Graphics.FromImage(union);
            if (UIattacker)
                renderattacker(g);
            else if (UIartifact)
                renderartifact(g);
            else if (UIchampion)
                renderchampion(g);
            rendervalue(g);

            //绘制按钮
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.Draw(g);
            }
            g.ResetTransform();

            g.Dispose();
            this.Bitmap = union;
        }

        private void control_event()
        {
            this.btnAttacker.Visible = UIartifact || UIchampion;
            this.btnArtifact.Visible = UIattacker || UIchampion;
            this.btnChampion.Visible = UIattacker || UIartifact;
            this.btnLeft.Visible = UIattacker;
            this.btnRight.Visible = UIattacker;
            this.btnpresetEdit.Visible = UIattacker && UIdeploy && !UIapply;
            this.btnresetApply.Visible = UIattacker && UIdeploy && !UIapply;
            //this.btnpresetSave.Visible = UIattacker && !UIdeploy;
            //this.btnrollback.Visible = false;
            //this.btnclear.Visible = false;
            //this.btnSTR.Visible = UIattacker && !UIdeploy;
            //this.btnDEX.Visible = UIattacker && !UIdeploy;
            //this.btnINT.Visible = UIattacker && !UIdeploy;
            //this.btnLUK.Visible = UIattacker && !UIdeploy;
            //this.btnPAD.Visible = UIattacker && !UIdeploy;
            //this.btnMAD.Visible = UIattacker && !UIdeploy;
            //this.btnHP.Visible = UIattacker && !UIdeploy;
            //this.btnMP.Visible = UIattacker && !UIdeploy;
            this.btnPresetPage1.Visible = UIattacker && union_preset != 1;
            this.btnPresetPage2.Visible = UIattacker && union_preset != 2;
            this.btnPresetPage3.Visible = UIattacker && union_preset != 3;
            this.btnPresetPage4.Visible = UIattacker && union_preset != 4;
            this.btnPresetPage5.Visible = UIattacker && union_preset != 5;
            this.btnslotPointReset.Visible = UIartifact;
            this.btnartifactSync.Visible = UIartifact;
            this.btnhelp.Visible = UIattacker || UIartifact;
            this.vScroll.Visible = UIattacker;
            this.vScroll2.Visible = UIattacker;
            this.vScroll3.Visible = UIartifact;
            this.vScroll.Maximum = Math.Max(0, union_raider_stat.Count - 10);
            this.vScroll2.Maximum = Math.Max(0, union_occupied_stat.Count - 10);
            this.vScroll3.Maximum = Math.Max(0, union_artifact_effect.Length - 10);
        }

        private void renderattacker(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.mapleUnion_buttonattackerSetting_checked_0, 0, 12);
            g.DrawImage(Resource.mapleUnion_buttonartifactSetting_normal_0, 0, 136);
            g.DrawImage(Resource.mapleUnion_button_championSetting_normal_0, 0, 260);
            g.DrawImage(Resource.mapleUnion_attackerSetting_backgrnd, 29, 0);
            for (int i = 0; i < 4; i ++)
            {
                g.DrawImage(Resource.mapleUnion_attackerSetting_CardBackgrnd_0_0, 90 + (118 + 4) * i, 524);
            }
            if (resultJson != null)
            {
                union_block = resultJson["union_raider_preset_" + union_preset.ToString()]["union_block"].ToObject<AfrmUnion.UnionBlock[]>();
                union_inner_stat = resultJson["union_raider_preset_" + union_preset.ToString()]["union_inner_stat"].ToObject<AfrmUnion.UnionInnerStat[]>();
            }
            switch (union_preset)
            {
                case 1: g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage1_checked_0, 256, 490); break;
                case 2: g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage2_checked_0, 276, 490); break;
                case 3: g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage3_checked_0, 296, 490); break;
                case 4: g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage4_checked_0, 316, 490); break;
                case 5: g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage5_checked_0, 336, 490); break;
                default: break;
            }
            if (UIdeploy)
            { 
                if (UIapply)
                {
                }
                else
                {
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetApplication_disabled_0, 366, 483);
                    this.btnpresetSave.Visible = false;
                    this.btnrollback.Visible = false;
                    this.btnclear.Visible = false;
                }
                if (union_block != null && union_block.Length >0)
                {
                    List<BlockPosition> allPositions = new List<BlockPosition>();
                    foreach (var block in union_block)
                    {
                        string block_type = block.block_type;
                        BlockPosition[] block_position = block.block_position;
                        allPositions.AddRange(block.block_position);
                        string block_subfix = "sub_0";
                        switch(block_type)
                        {
                            case "劍士": case "Warrior": case "전사": block_subfix = "main_0_0"; break;
                            case "法師": case "Magician": case "마법사": block_subfix = "main_2_0"; break;
                            case "弓箭手": case "Bowman ": case "궁수": block_subfix = "main_3_0"; break;
                            case "盜賊": case "Thief": case "도적": block_subfix = "main_4_0"; break;
                            case "海盜": case "Pirate": case "해적": block_subfix = "main_5_0"; break;
                            case "米納爾森林": block_subfix = "main_-1002_0"; break;
                            case "冰原雪域": block_subfix = "main_-1003_0"; break;
                            case "메이플 M 캐릭터": block_subfix = "main_M_0"; break;
                            default: block_subfix = "sub_0"; break;
                        }
                        int center_x = block.block_control_point.x;
                        int center_y = block.block_control_point.y * (-1);
                        string blockName = "mapleUnion_attackerSetting_Board_sector_char_" + block_subfix;
                        Bitmap image = Resource.ResourceManager.GetObject(blockName) as Bitmap;
                        g.DrawImage(image, new Rectangle(327 + center_x * 22, 241 + center_y * 22, image.Width, image.Height));
                        foreach (var pos in block_position)
                        {
                            int x = pos.x;
                            int y = pos.y * (-1);
                            if (x == center_x && y == center_y)
                                continue;
                            g.DrawImage(Resource.mapleUnion_attackerSetting_Board_sector_char_sub_0, 327 + x * 22, 241 + y * 22);
                        }
                    }
                    var unionPositions = allPositions.GroupBy(pos => new { pos.x, pos.y }).Select(g => g.First()).ToList();
                    foreach (var pos in unionPositions)
                    {
                        int x = pos.x;
                        int y = pos.y * (-1);
                        bool hasLeft = unionPositions.Any(other => other.x == pos.x - 1 && other.y == pos.y);
                        bool hasRight = unionPositions.Any(other => other.x == pos.x + 1 && other.y == pos.y);
                        bool hasUp = unionPositions.Any(other => other.x == pos.x && other.y == pos.y + 1);
                        bool hasDown = unionPositions.Any(other => other.x == pos.x && other.y == pos.y - 1);
                        if (hasRight)
                            g.DrawImage(Resource.mapleUnion_attackerSetting_Board_sector_fill_verti, 348 + x * 22, 243 + y * 22);
                        if (hasDown)
                            g.DrawImage(Resource.mapleUnion_attackerSetting_Board_sector_fill_hori, 329 + x * 22, 262 + y * 22);
                    }
                }
                
                if (union_inner_stat != null && union_inner_stat.Length > 0) //绘制属性按钮
                {
                    foreach (var stat_field in union_inner_stat)
                    {
                        int stat_field_id = stat_field.stat_field_id;
                        string stat_field_effect = stat_field.stat_field_effect;
                        switch (stat_field_effect)
                        {
                            case "聯盟STR": case "Union STR": case "유니온 STR": this.btnSTR.Location = areaPos[stat_field_id]; this.btnSTR.Visible = true; break;
                            case "聯盟DEX": case "Union DEX": case "유니온 DEX": this.btnDEX.Location = areaPos[stat_field_id]; this.btnDEX.Visible = true; break;
                            case "聯盟INT": case "Union INT": case "유니온 INT": this.btnINT.Location = areaPos[stat_field_id]; this.btnINT.Visible = true; break;
                            case "聯盟LUK": case "Union LUK": case "유니온 LUK": this.btnLUK.Location = areaPos[stat_field_id]; this.btnLUK.Visible = true; break;
                            case "聯盟最大HP": case "Union Max HP": case "유니온 최대 HP": this.btnHP.Location = areaPos[stat_field_id]; this.btnHP.Visible = true; break;
                            case "聯盟最大MP": case "Union Max MP":  case "유니온 최대 MP": this.btnMP.Location = areaPos[stat_field_id]; this.btnMP.Visible = true; break;
                            case "聯盟攻擊力": case "Union ATT":  case "유니온 공격력": this.btnPAD.Location = areaPos[stat_field_id]; this.btnPAD.Visible = true; break;
                            case "聯盟魔力": case "Union MATT": case "유니온 마력": this.btnMAD.Location = areaPos[stat_field_id]; this.btnMAD.Visible = true; break;
                            default: break;
                        }
                    }
                }
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_8_default_0, 113, 164);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_9_default_0, 210, 65);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_10_default_0, 380, 65);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_11_default_0, 496, 164);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_12_default_0, 490, 310);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_13_default_0, 377, 404);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_14_default_0, 214, 404);
                g.DrawImage(Resource.mapleUnion_attackerSetting_AreaButtons_AreaMouseOver_15_default_0, 109, 310);
                g.DrawImage(Resource.mapleUnion_attackerSetting_Board_modeOverlay_default, 590, 18);
            }
            else
            {
                this.btnpresetSave.Visible = true;
                this.btnrollback.Visible = true;
                this.btnclear.Visible = true;
                this.btnSTR.Visible = false;
                this.btnDEX.Visible = false;
                this.btnINT.Visible = false;
                this.btnLUK.Visible = false;
                this.btnPAD.Visible = false;
                this.btnMAD.Visible = false;
                this.btnHP.Visible = false;
                this.btnMP.Visible = false;
                g.DrawImage(Resource.mapleUnion_attackerSetting_Board_modeOverlay_setting, 47, 18);
            }
            for (int i = 0; i < 10; i++)
            {
                string option = i + scrollValue < union_raider_stat.Count ? union_raider_stat[i + scrollValue] : "";
                g.DrawString(option, GearGraphics.EquipDetailFont, GearGraphics.GrayBrush, 650, 544 + 14 * i);
            }
            for (int i = 0; i < 10; i++)
            {
                string option2 = i + scrollValue2 < union_occupied_stat.Count ? union_occupied_stat[i + scrollValue2] : "";
                g.DrawString(option2, GearGraphics.EquipDetailFont, GearGraphics.GrayBrush, 835, 544 + 14 * i);
            }
            g.ResetTransform();
        }

        private void renderartifact(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.mapleUnion_buttonattackerSetting_normal_0, 0, 12);
            g.DrawImage(Resource.mapleUnion_buttonartifactSetting_checked_0, 0, 136);
            g.DrawImage(Resource.mapleUnion_button_championSetting_normal_0, 0, 260);
            g.DrawImage(Resource.mapleUnion_artifactSetting_backgrnd, 29, 0);
            if (resultJson2 != null)
            {
                union_artifact_crystal = resultJson2["union_artifact_crystal"].ToObject<AfrmUnion.UnionArtifactCrystal[]>();
                union_artifact_effect = resultJson2["union_artifact_effect"].ToObject<AfrmUnion.UnionArtifactEffect[]>();
            }
            for (int i = 0; i <= 8; i++)
            {
                if (i >= union_artifact_crystal.Length)
                {
                    g.DrawImage(Resource.mapleUnion_artifactSetting_canvasdisabledSlot, 49 + 188 * (i % 3), 20 + 218 * (i / 3));
                    string artifact_disabled = "artifact_artifacts_" + i.ToString() + "_gradeInfo_0_disabled";
                    Bitmap image = Resource.ResourceManager.GetObject(artifact_disabled) as Bitmap;
                    g.DrawImage(image, new Rectangle(49 + 188 * (i % 3) + 89 - image.Width / 2, 20 + 218 * (i / 3) + 105 - image.Height / 2, image.Width, image.Height));
                    g.DrawImage(Resource.mapleUnion_artifactSetting_canvasdisabled, 50 + 188 * (i % 3), 22 + 218 * (i / 3));
                }
                else
                {
                    var crystal = union_artifact_crystal[i];
                    int artifact_level = crystal.level;
                    if (artifact_level < 5)
                    {
                        g.DrawImage(Resource.artifact_slot_0_0, 49 + 188 * (i % 3), 20 + 218 * (i / 3));
                        if (artifact_level == 1)
                        {
                            string artifact = "artifact_artifacts_"+ i.ToString() + "_gradeInfo_0_icon";
                            System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(artifact) as System.Drawing.Bitmap;
                            g.DrawImage(image, new Rectangle(49 + 188 * (i % 3) + 89 - image.Width / 2, 20 + 218 * (i / 3) + 105 - image.Height / 2, image.Width, image.Height));
                        }
                        else if (artifact_level < 5)
                        {
                            string artifact = "artifact_artifacts_" + i.ToString() + "_gradeInfo_1_icon";
                            System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(artifact) as System.Drawing.Bitmap;
                            g.DrawImage(image, new Rectangle(49 + 188 * (i % 3) + 89 - image.Width / 2, 20 + 218 * (i / 3) + 105 - image.Height / 2, image.Width, image.Height));
                        }
                    }
                    else
                    {
                        g.DrawImage(Resource.artifact_slot_4_0, 49 + 188 * (i % 3), 20 + 218 * (i / 3));
                        string artifact = "artifact_artifacts_" + i.ToString() + "_gradeInfo_4_icon";
                        System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(artifact) as System.Drawing.Bitmap;
                        g.DrawImage(image, new Rectangle(49 + 188 * (i % 3) + 89 - image.Width / 2, 20 + 218 * (i / 3) + 105 - image.Height / 2, image.Width, image.Height));
                    }
                    switch (artifact_level)
                    {
                        case 1: g.DrawImage(Resource.mapleUnion_artifactSetting_artifactGrade_enabled_0, 49 + 188 * (i % 3) + 77, 218 * (i / 3) + 33); break;
                        case 2: g.DrawImage(Resource.mapleUnion_artifactSetting_artifactGrade_enabled_1, 49 + 188 * (i % 3) + 68, 218 * (i / 3) + 33); break;
                        case 3: g.DrawImage(Resource.mapleUnion_artifactSetting_artifactGrade_enabled_2, 49 + 188 * (i % 3) + 59, 218 * (i / 3) + 33); break;
                        case 4: g.DrawImage(Resource.mapleUnion_artifactSetting_artifactGrade_enabled_3, 49 + 188 * (i % 3) + 49, 218 * (i / 3) + 33); break;
                        case 5: g.DrawImage(Resource.mapleUnion_artifactSetting_artifactGrade_enabled_4, 49 + 188 * (i % 3) + 41, 218 * (i / 3) + 33); break;
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (i + scrollValue3 < union_artifact_effect.Length)
                {
                    var effect = union_artifact_effect[i + scrollValue3];
                    int effect_level = effect.level;
                    string effect_name = effect.name;
                    g.DrawString("Lv. " + effect_level.ToString(), GearGraphics.EquipDetailFont, GearGraphics.WhiteBrush, 653, 524 + 16 * i);
                    g.DrawString(effect_name, GearGraphics.EquipDetailFont, GearGraphics.GrayBrush, 695, 524 + 16 * i);
                }
            }
            g.DrawImage(Resource.mapleUnion_artifactSetting_scrollslot_enabled_base, 608, 20);
            g.DrawString(artifact_ap, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 590f, 682f);
            g.ResetTransform();
        }

        private void rendervalue(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            rendersymbol(g);
            string union_num = union_block.Count(block => block.block_type != "메이플 M 캐릭터").ToString();
            int artifact_max_ap = union_artifact_level <= 30 ? 10000 + 100 * union_artifact_level : union_artifact_level <= 50 ? 13000 + 200 * union_artifact_level : 17000 + 300 * union_artifact_level;
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", union_level, 987, 114, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", champion_power, 987, 184, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", union_artifact_level.ToString(), 987, 260, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", union_num, 952, 311, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", attackers, 987, 311, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(union_attackpower), 987, 336, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", champion_coin, 987, 361, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(available_coin), 987, 361, true);
            g.DrawString(union_coin, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 725f, 390f);
            g.DrawString(union_artifact_point, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 840f, 390f);
            g.DrawImage(Resource.mapleUnion_unionInfo_layerartifactExpGaugeBlank, 879, 250);
            int artifact_max_exp = artifact_exp_list[union_artifact_level - 1];
            System.Drawing.Bitmap layer = Resource.ResourceManager.GetObject("mapleUnion_unionInfo_layerartifactExpGauge") as System.Drawing.Bitmap;
            g.DrawImage(layer, new Rectangle(881, 251, artifact_max_exp > 0 ? layer.Width * union_artifact_exp / artifact_max_exp : 0, layer.Height));
            g.ResetTransform();
        }

        private void renderchampion(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.mapleUnion_buttonattackerSetting_normal_0, 0, 12);
            g.DrawImage(Resource.mapleUnion_buttonartifactSetting_normal_0, 0, 136);
            g.DrawImage(Resource.mapleUnion_button_championSetting_check_0, 0, 260);
            g.DrawImage(Resource.mapleUnion_ChampionSetting_backgrnd, 29, 0);
            union_champion = resultJson3 != null ? resultJson3["union_champion"].ToObject<AfrmUnion.UnionChampion[]>() : null;
            for (int i = 0; i <= 5; i++)
            {
                if (union_champion != null)
                {
                    var champion = union_champion.FirstOrDefault(c => c.champion_slot == i + 1);
                    if (champion != null)
                    {
                        g.DrawImage(Resource.mapleUnion_championSetting_slot_canvasregistered, 49 + 190 * (i % 3), 20 + 344 * (i / 3));
                        string grade = champion.champion_grade;
                        render_signia(g, grade, i);
                    }
                    else
                    {
                        g.DrawImage(Resource.mapleUnion_championSetting_slot_canvasdisabled, 49 + 190 * (i % 3), 20 + 344 * (i / 3));
                    }
                }
                else
                {
                    g.DrawImage(Resource.mapleUnion_championSetting_slot_canvasdisabled, 49 + 190 * (i % 3), 20 + 344 * (i / 3));
                }
            }
            champion_badge_total_info = resultJson3 != null ? resultJson3["champion_badge_total_info"].ToObject<AfrmUnion.UnionBadgeInfo[]>() : null;
            if (champion_badge_total_info != null)
            {
                float y_offset = 0;
                foreach (var badgeInfo in champion_badge_total_info)
                {
                    string stat = badgeInfo.stat;
                    g.DrawString(stat, GearGraphics.KMSItemDetailFont2, GearGraphics.GrayBrush, 653f, 524f + y_offset);
                    y_offset += 16f;
                }
            }
            g.ResetTransform();
        }

        private void rendersymbol(Graphics g)
        {
            switch (union_grade)
            {
                case "新手戰地聯盟 1":
                case "Novice Union 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_0_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_0_0, 674, 109); attackers = "9"; break;
                case "新手戰地聯盟 2":
                case "Novice Union 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_0_1, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_0_1, 674, 109); attackers = "10"; break;
                case "新手戰地聯盟 3":
                case "Novice Union 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_0_2, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_0_2, 674, 109); attackers = "11"; break;
                case "新手戰地聯盟 4":
                case "Novice Union 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_0_3, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_0_3, 674, 109); attackers = "12"; break;
                case "新手戰地聯盟 5":
                case "Novice Union 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_0_4, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_0_4, 674, 109); attackers = "13"; break;
                case "Veteran Union 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 665, 109); attackers = "18"; break;
                case "Veteran Union 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 665, 109); attackers = "19"; break;
                case "Veteran Union 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 665, 109); attackers = "20"; break;
                case "Veteran Union 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 665, 109); attackers = "21"; break;
                case "Veteran Union 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 665, 109); attackers = "22"; break;
                case "Master Union 1":
                case "마스터 유니온 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 655, 109); attackers = "27"; break;
                case "Master Union 2":
                case "마스터 유니온 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 655, 109); attackers = "28"; break;
                case "Master Union 3":
                case "마스터 유니온 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 655, 109); attackers = "29"; break;
                case "Master Union 4":
                case "마스터 유니온 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 655, 109); attackers = "30"; break;
                case "Master Union 5":
                case "마스터 유니온 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_1_0, 670, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_1_0, 655, 109); attackers = "31"; break;
                case "宗師戰地聯盟 1":
                case "Grand Master Union 1":
                case "그랜드 마스터 유니온 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_0, 670, 17); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_0, 657, 95); attackers = "36"; break;
                case "宗師戰地聯盟 2":
                case "Grand Master Union 2":
                case "그랜드 마스터 유니온 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_1, 670, 17); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_2, 657, 95); attackers = "37"; break;
                case "宗師戰地聯盟 3":
                case "Grand Master Union 3":
                case "그랜드 마스터 유니온 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_3, 670, 17); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_3, 657, 95); attackers = "38"; break;
                case "宗師戰地聯盟 4":
                case "Grand Master Union 4":
                case "그랜드 마스터 유니온 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_3, 670, 17); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_3, 657, 95); attackers = "39"; break;
                case "宗師戰地聯盟 5":
                case "Grand Master Union 5":
                case "그랜드 마스터 유니온 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_4, 670, 17); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_4, 657, 95); attackers = "40"; break;
                case "總司令聯盟 1":
                case "Supreme Union 1":
                case "슈프림 유니온 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_0, 671, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_0, 657, 92); attackers = "41"; break;
                case "總司令聯盟 2":
                case "Supreme Union 2":
                case "슈프림 유니온 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_1, 671, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_1, 657, 92); attackers = "42"; break;
                case "總司令聯盟 3":
                case "Supreme Union 3":
                case "슈프림 유니온 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_2, 671, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_2, 657, 92); attackers = "43"; break;
                case "總司令聯盟 4":
                case "Supreme Union 4":
                case "슈프림 유니온 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_3, 671, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_3, 657, 92); attackers = "44"; break;
                case "總司令聯盟 5":
                case "Supreme Union 5":
                case "슈프림 유니온 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_4, 671, 11); g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_4, 657, 92); attackers = "45"; break;
                default: break;
            }
        }

        private void DrawText(Graphics g, string resourceName, string text, int x, int y, bool reverse)
        {
            if (reverse)
                text = new string(text.Reverse().ToArray());
            foreach (char c in text)
            {
                char Char = c == ',' ? 'c' : c;
                string imageName = resourceName + Char;
                System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(imageName) as System.Drawing.Bitmap;
                if (reverse)
                {
                    x -= image.Width + 1;
                    g.DrawImage(image, new Rectangle(x, (c == ',') ? y + 8 : y, image.Width, image.Height));
                }
                else
                {
                    g.DrawImage(image, new Rectangle(x, (c == ',') ? y + 8 : y, image.Width, image.Height));
                    x += image.Width + 1;
                }
            }
        }

        private string ThousandSeparator(string numericString)
        {
            long number = long.Parse(numericString);
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        private void render_signia(Graphics g, string grade, int i)
        {
            int baseX = 49 + 190 * (i % 3);
            int baseY = 20 + 344 * (i / 3);
            int gradeX = baseX + 14;
            int gradeY = baseY + 12;
            int[] badgeTypes = new int[5];

            switch (grade)
            {
                case "C":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_0, gradeX, gradeY);
                    badgeTypes = new int[] { 0, 0, 0, 0, 0 };
                    break;
                case "B":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_1, gradeX, gradeY);
                    badgeTypes = new int[] { 1, 0, 0, 0, 0 };
                    break;
                case "A":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_2, gradeX, gradeY);
                    badgeTypes = new int[] { 1, 1, 0, 0, 0 };
                    break;
                case "S":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_3, gradeX, gradeY);
                    badgeTypes = new int[] { 1, 1, 1, 0, 0 };
                    break;
                case "SS":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_4, gradeX, gradeY);
                    badgeTypes = new int[] { 1, 1, 1, 1, 0 };
                    break;
                case "SSS":
                    g.DrawImage(Resource.mapleUnion_championSetting_dlgChampionAdvancement_championGrade_5, gradeX, gradeY);
                    badgeTypes = new int[] { 1, 1, 1, 1, 1 };
                    break;
                default:
                    return;
            }

            int[] badgeOffsetX = { 10, 43, 76, 109, 142 };
            int badgeY = baseY + 266;

            for (int j = 0; j < 5; j++)
            {
                string imgName = $"mapleUnion_championSetting_slot_insignia_{j}_{badgeTypes[j]}";
                var img = Resource.ResourceManager.GetObject(imgName) as Bitmap;
                if (img != null)
                {
                    g.DrawImage(img, baseX + badgeOffsetX[j], badgeY);
                }
            }
        }

        private void render_bitmap(Graphics g, string nodepath, int x, int y)
        {
            Wz_Node Node = PluginBase.PluginManager.FindWz(nodepath);
            Bitmap image = BitmapOrigin.CreateFromNode(Node, PluginBase.PluginManager.FindWz).Bitmap;
            g.DrawImage(image, x, y);
        }

        private void btnAttacker_MouseClick(object sender, EventArgs e)
        {
            this.UIattacker = true;
            this.UIartifact = false;
            this.UIchampion = false;
            this.UIdeploy = true;
            this.UIapply = false;
        }
        
        private void btnArtifact_MouseClick(object sender, EventArgs e)
        {
            this.UIattacker = false;
            this.UIartifact = true;
            this.UIchampion = false;
            this.btnpresetSave.Visible = false;
            this.btnrollback.Visible = false;
            this.btnclear.Visible = false;
            this.UIdeploy = false;
            this.UIapply = false;
            this.btnSTR.Visible = false;
            this.btnDEX.Visible = false;
            this.btnINT.Visible = false;
            this.btnLUK.Visible = false;
            this.btnPAD.Visible = false;
            this.btnMAD.Visible = false;
            this.btnHP.Visible = false;
            this.btnMP.Visible = false;
        }

        private void btnChampion_MouseClick(object sender, MouseEventArgs e)
        {
            this.UIattacker = false;
            this.UIartifact = false;
            this.UIchampion = true;
            this.btnpresetSave.Visible = false;
            this.btnrollback.Visible = false;
            this.btnclear.Visible = false;
            this.UIdeploy = false;
            this.UIapply = false;
            this.btnSTR.Visible = false;
            this.btnDEX.Visible = false;
            this.btnINT.Visible = false;
            this.btnLUK.Visible = false;
            this.btnPAD.Visible = false;
            this.btnMAD.Visible = false;
            this.btnHP.Visible = false;
            this.btnMP.Visible = false;
        }

        private void btnpresetEdit_MouseClick(object sender, MouseEventArgs e)
        {
            this.UIdeploy = false;
            this.UIapply = false;
        }

        private void btnpresetSave_MouseClick(object sender, MouseEventArgs e)
        {
            this.UIdeploy = true;
            this.UIapply = false;
        }

        private void btnPresetPage1_MouseClick(object sender, MouseEventArgs e)
        {
            this.union_preset = 1;
        }

        private void btnPresetPage2_MouseClick(object sender, MouseEventArgs e)
        {
            this.union_preset = 2;
        }

        private void btnPresetPage3_MouseClick(object sender, MouseEventArgs e)
        {
            this.union_preset = 3;
        }

        private void btnPresetPage4_MouseClick(object sender, MouseEventArgs e)
        {
            this.union_preset = 4;
        }

        private void btnPresetPage5_MouseClick(object sender, MouseEventArgs e)
        {
            this.union_preset = 5;
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }

        private void vScroll_ValueChanged(object sender, EventArgs e)
        {
            scrollValue = vScroll.Value;
            this.waitForRefresh = true;
        }

        private void vScroll2_ValueChanged(object sender, EventArgs e)
        {
            scrollValue2 = vScroll2.Value;
            this.waitForRefresh = true;
        }

        private void vScroll3_ValueChanged(object sender, EventArgs e)
        {
            scrollValue3 = vScroll3.Value;
            this.waitForRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseWheel(e);
        }

        void AfrmUnion_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.menu.Show(this, e.Location);
            }
        }

        void tsmiCopy_Click(object sender, EventArgs e)
        {
            if (this.Bitmap != null)
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    var dataObj = new DataObject();
                    dataObj.SetData(DataFormats.Bitmap, this.Bitmap);
                    Byte[] dibData = ConvertToDib(this.Bitmap);
                    stream.Write(dibData, 0, dibData.Length);
                    dataObj.SetData(DataFormats.Dib, stream);
                    Clipboard.SetDataObject(dataObj, true);
                }
            }
        }

        void tsmiSave_Click(object sender, EventArgs e)
        {
            if (this.Bitmap != null)
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "PNG (*.png)|*.png|*.*|*.*";
                    dlg.FileName = "Union UI Preview";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        this.Bitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }

        private Byte[] ConvertToDib(Image image) // https://stackoverflow.com/a/46424800
        {
            Byte[] bm32bData;
            Int32 width = image.Width;
            Int32 height = image.Height;
            // Ensure image is 32bppARGB by painting it on a new 32bppARGB image.
            using (Bitmap bm32b = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics gr = Graphics.FromImage(bm32b))
                    gr.DrawImage(image, new Rectangle(0, 0, bm32b.Width, bm32b.Height));
                // Bitmap format has its lines reversed.
                bm32b.RotateFlip(RotateFlipType.Rotate180FlipX);
                Int32 stride;
                bm32bData = GetImageData(bm32b, out stride);
            }
            // BITMAPINFOHEADER struct for DIB.
            Int32 hdrSize = 0x28;
            Byte[] fullImage = new Byte[hdrSize + 12 + bm32bData.Length];
            //Int32 biSize;
            WriteIntToByteArray(fullImage, 0x00, 4, true, (UInt32)hdrSize);
            //Int32 biWidth;
            WriteIntToByteArray(fullImage, 0x04, 4, true, (UInt32)width);
            //Int32 biHeight;
            WriteIntToByteArray(fullImage, 0x08, 4, true, (UInt32)height);
            //Int16 biPlanes;
            WriteIntToByteArray(fullImage, 0x0C, 2, true, 1);
            //Int16 biBitCount;
            WriteIntToByteArray(fullImage, 0x0E, 2, true, 32);
            //BITMAPCOMPRESSION biCompression = BITMAPCOMPRESSION.BITFIELDS;
            WriteIntToByteArray(fullImage, 0x10, 4, true, 3);
            //Int32 biSizeImage;
            WriteIntToByteArray(fullImage, 0x14, 4, true, (UInt32)bm32bData.Length);
            // These are all 0. Since .net clears new arrays, don't bother writing them.
            //Int32 biXPelsPerMeter = 0;
            //Int32 biYPelsPerMeter = 0;
            //Int32 biClrUsed = 0;
            //Int32 biClrImportant = 0;

            // The aforementioned "BITFIELDS": colour masks applied to the Int32 pixel value to get the R, G and B values.
            WriteIntToByteArray(fullImage, hdrSize + 0, 4, true, 0x00FF0000);
            WriteIntToByteArray(fullImage, hdrSize + 4, 4, true, 0x0000FF00);
            WriteIntToByteArray(fullImage, hdrSize + 8, 4, true, 0x000000FF);
            Array.Copy(bm32bData, 0, fullImage, hdrSize + 12, bm32bData.Length);
            return fullImage;
        }

        private void WriteIntToByteArray(Byte[] data, Int32 startIndex, Int32 bytes, Boolean littleEndian, UInt32 value) // https://stackoverflow.com/a/46424800
        {
            Int32 lastByte = bytes - 1;
            if (data.Length < startIndex + bytes)
                throw new ArgumentOutOfRangeException("startIndex", "Data array is too small to write a " + bytes + "-byte value at offset " + startIndex + ".");
            for (Int32 index = 0; index < bytes; index++)
            {
                Int32 offs = startIndex + (littleEndian ? index : lastByte - index);
                data[offs] = (Byte)(value >> (8 * index) & 0xFF);
            }
        }

        private Byte[] GetImageData(Bitmap sourceImage, out Int32 stride) // https://stackoverflow.com/a/43706643
        {
            System.Drawing.Imaging.BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            stride = sourceData.Stride;
            Byte[] data = new Byte[stride * sourceImage.Height];
            System.Runtime.InteropServices.Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
            sourceImage.UnlockBits(sourceData);
            return data;
        }
    }
}
