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
        private ACtrlButton btnClose;
        private ACtrlButton btnAttacker;
        private ACtrlButton btnArtifact;
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
        private bool UIdeploy = true;
        private bool UIapply = false;
        public string union_level = "9500";
        public string union_grade = "그랜드 마스터 유니온 4";
        private string attackers = "39";
        public string union_attackpower = "999999999";
        private string union_coin = "0";
        public int union_artifact_level = 10;
        public int union_artifact_exp = 500;
        public string union_artifact_point = "10000";
        public string available_coin = "0";
        public string artifact_ap = "0";
        public JObject resultJson = null;
        public UnionBlock[] union_block = Array.Empty<UnionBlock>();
        public UnionInnerStat[] union_inner_stat = Array.Empty<UnionInnerStat>();
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
            this.btnAttacker.Visible = false;
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

            this.btnstartRaid = new ACtrlButton(); //参与战斗按钮
            this.btnstartRaid.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_normal_0);
            this.btnstartRaid.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_pressed_0);
            this.btnstartRaid.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_mouseOver_0);
            this.btnstartRaid.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonstartRaid_disabled_0);
            this.btnstartRaid.Location = new Point(640, 423);
            this.btnstartRaid.Size = new Size(121, 46);
            this.btnstartRaid.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btncoin = new ACtrlButton(); //领取纪念币按钮
            this.btncoin.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_normal_0);
            this.btncoin.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_pressed_0);
            this.btncoin.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_mouseOver_0);
            this.btncoin.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttoncoin_disabled_0);
            this.btncoin.Location = new Point(762, 423);
            this.btncoin.Size = new Size(121, 46);
            this.btncoin.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnartifactMission = new ACtrlButton(); //神器任务按钮
            this.btnartifactMission.Normal = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_normal_0);
            this.btnartifactMission.Pressed = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_pressed_0);
            this.btnartifactMission.MouseOver = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_mouseOver_0);
            this.btnartifactMission.Disabled = new BitmapOrigin(Resource.mapleUnion_unionInfo_buttonartifactMission_disabled_0);
            this.btnartifactMission.Location = new Point(884, 423);
            this.btnartifactMission.Size = new Size(121, 46);
            this.btnartifactMission.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

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
                yield return this.btnClose;
                yield return this.btnAttacker;
                yield return this.btnArtifact;
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

        private void renderattacker(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.mapleUnion_buttonattackerSetting_checked_0, 0, 12);
            g.DrawImage(Resource.mapleUnion_buttonartifactSetting_normal_0, 0, 136);
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
                case 1: 
                    this.btnPresetPage1.Visible = false;
                    this.btnPresetPage2.Visible = true;
                    this.btnPresetPage3.Visible = true;
                    this.btnPresetPage4.Visible = true;
                    this.btnPresetPage5.Visible = true;
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage1_checked_0, 256, 490);
                    break;
                case 2:
                    this.btnPresetPage1.Visible = true;
                    this.btnPresetPage2.Visible = false;
                    this.btnPresetPage3.Visible = true;
                    this.btnPresetPage4.Visible = true;
                    this.btnPresetPage5.Visible = true;
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage2_checked_0, 276, 490); 
                    break;
                case 3:
                    this.btnPresetPage1.Visible = true;
                    this.btnPresetPage2.Visible = true;
                    this.btnPresetPage3.Visible = false;
                    this.btnPresetPage4.Visible = true;
                    this.btnPresetPage5.Visible = true;
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage3_checked_0, 296, 490); 
                    break;
                case 4:
                    this.btnPresetPage1.Visible = true;
                    this.btnPresetPage2.Visible = true;
                    this.btnPresetPage3.Visible = true;
                    this.btnPresetPage4.Visible = false;
                    this.btnPresetPage5.Visible = true;
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage4_checked_0, 316, 490); 
                    break;
                case 5:
                    this.btnPresetPage1.Visible = true;
                    this.btnPresetPage2.Visible = true;
                    this.btnPresetPage3.Visible = true;
                    this.btnPresetPage4.Visible = true;
                    this.btnPresetPage5.Visible = false;
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetPage5_checked_0, 336, 490); 
                    break;
                default: break;
            }
            if (UIdeploy)
            { 
                if (UIapply)
                {
                    this.btnresetApply.Visible = true;
                }
                else
                {
                    g.DrawImage(Resource.mapleUnion_attackerSetting_buttonpresetApplication_disabled_0, 366, 483);
                    this.btnpresetEdit.Visible = true;
                    this.btnresetApply.Visible = false;
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
                            case "전사": block_subfix = "main_0_0"; break;
                            case "마법사": block_subfix = "main_2_0"; break;
                            case "궁수": block_subfix = "main_3_0"; break;
                            case "도적": block_subfix = "main_4_0"; break;
                            case "해적": block_subfix = "main_5_0"; break;
                            case "메이플 M 캐릭터": block_subfix = "main_M_0"; break;
                            default: block_subfix = "sub_0"; break;
                        }
                        int center_x = block.block_control_point.x;
                        int center_y = block.block_control_point.y * (-1);
                        string blockName = "mapleUnion_attackerSetting_Board_sector_char_" + block_subfix;
                        System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(blockName) as System.Drawing.Bitmap;
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
                            case "유니온 STR": this.btnSTR.Location = areaPos[stat_field_id]; this.btnSTR.Visible = true; break;
                            case "유니온 DEX": this.btnDEX.Location = areaPos[stat_field_id]; this.btnDEX.Visible = true; break;
                            case "유니온 INT": this.btnINT.Location = areaPos[stat_field_id]; this.btnINT.Visible = true; break;
                            case "유니온 LUK": this.btnLUK.Location = areaPos[stat_field_id]; this.btnLUK.Visible = true; break;
                            case "유니온 최대 HP": this.btnHP.Location = areaPos[stat_field_id]; this.btnHP.Visible = true; break;
                            case "유니온 최대 MP": this.btnMP.Location = areaPos[stat_field_id]; this.btnMP.Visible = true; break;
                            case "유니온 공격력": this.btnPAD.Location = areaPos[stat_field_id]; this.btnPAD.Visible = true; break;
                            case "유니온 마력": this.btnMAD.Location = areaPos[stat_field_id]; this.btnMAD.Visible = true; break;
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
                this.btnpresetEdit.Visible = false;
                this.btnresetApply.Visible = false;
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
            g.ResetTransform();
        }

        private void renderartifact(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.mapleUnion_buttonattackerSetting_normal_0, 0, 12);
            g.DrawImage(Resource.mapleUnion_buttonartifactSetting_checked_0, 0, 136);
            g.DrawImage(Resource.mapleUnion_artifactSetting_backgrnd, 29, 0);
            for (int i = 0; i <= 8; i++)
            {
                g.DrawImage(Resource.mapleUnion_artifactSetting_canvasdisabledSlot, 49 + 188 * (i % 3), 20 + 218 * (i / 3));
                string artifact_disabled = "artifact_artifacts_" + i.ToString() + "_gradeInfo_0_disabled";
                System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(artifact_disabled) as System.Drawing.Bitmap;
                g.DrawImage(image, new Rectangle(49 + 188 * (i % 3) + 89 - image.Width / 2, 20 + 218 * (i / 3) + 105 - image.Height / 2, image.Width, image.Height));
                g.DrawImage(Resource.mapleUnion_artifactSetting_canvasdisabled, 50 + 188 * (i % 3), 22 + 218 * (i / 3));
            }
            g.DrawImage(Resource.mapleUnion_artifactSetting_scrollslot_enabled_base, 608, 20);
            g.DrawString(artifact_ap, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 590f, 682f);
            this.btnslotPointReset.Visible = true;
            this.btnartifactSync.Visible = true;
            g.ResetTransform();
        }

        private void rendervalue(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            switch (union_grade)
            {
                case "그랜드 마스터 유니온 1":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_0, 670, 17);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_0, 657, 95); attackers = "36"; break;
                case "그랜드 마스터 유니온 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_1, 670, 17);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_2, 657, 95); attackers = "37"; break;
                case "그랜드 마스터 유니온 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_3, 670, 17);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_3, 657, 95); attackers = "38"; break;
                case "그랜드 마스터 유니온 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_3, 670, 17);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_3, 657, 95); attackers = "39"; break;
                case "그랜드 마스터 유니온 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_3_4, 670, 17);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_3_4, 657, 95); attackers = "40"; break;
                case "슈프림 유니온 1": 
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_0, 671, 11);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_0, 657, 92); attackers = "41"; break;
                case "슈프림 유니온 2":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_1, 671, 11);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_1, 657, 92); attackers = "42"; break;
                case "슈프림 유니온 3":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_2, 671, 11);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_2, 657, 92); attackers = "43"; break;
                case "슈프림 유니온 4":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_3, 671, 11);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_3, 657, 92); attackers = "44"; break;
                case "슈프림 유니온 5":
                    g.DrawImage(Resource.mapleUnion_unionInfo_title_4_4, 671, 11);
                    g.DrawImage(Resource.mapleUnion_unionInfo_symbol_4_4, 657, 92); attackers = "45"; break;
                default: break;
            }
            string union_num = union_block.Count(block => block.block_type != "메이플 M 캐릭터").ToString();
            int artifact_max_ap = union_artifact_level <= 30 ? 10000 + 100 * union_artifact_level : union_artifact_level <= 50 ? 13000 + 200 * union_artifact_level : 17000 + 300 * union_artifact_level;
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", union_level, 987, 114, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", union_coin, 987, 184, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_large_", union_artifact_level.ToString(), 987, 260, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", union_num, 952, 311, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", attackers, 987, 311, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(union_attackpower), 987, 336, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(artifact_max_ap.ToString()), 987, 361, true);
            g.DrawImage(Resource.mapleUnion_unionInfo_numberSources_small_d, 921, 361);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(union_artifact_point), 917, 361, true);
            DrawText(g, "mapleUnion_unionInfo_numberSources_small_", ThousandSeparator(available_coin), 987, 384, true);
            g.DrawImage(Resource.mapleUnion_unionInfo_layerartifactExpGaugeBlank, 879, 250);
            int artifact_max_exp = artifact_exp_list[union_artifact_level - 1];
            System.Drawing.Bitmap layer = Resource.ResourceManager.GetObject("mapleUnion_unionInfo_layerartifactExpGauge") as System.Drawing.Bitmap;
            g.DrawImage(layer, new Rectangle(881, 251, layer.Width * union_artifact_exp / artifact_max_exp, layer.Height));
            g.ResetTransform();
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

        private void btnAttacker_MouseClick(object sender, EventArgs e)
        {
            this.UIattacker = true;
            this.UIartifact = false;
            this.btnAttacker.Visible = false;
            this.btnArtifact.Visible = true;
            this.btnLeft.Visible = true;
            this.btnRight.Visible = true;
            this.btnslotPointReset.Visible = false;
            this.btnartifactSync.Visible = false;
            this.UIdeploy = true;
            this.UIapply = false;
        }
        
        private void btnArtifact_MouseClick(object sender, EventArgs e)
        {
            this.UIattacker = false;
            this.UIartifact = true;
            this.btnAttacker.Visible = true;
            this.btnArtifact.Visible = false;
            this.btnLeft.Visible = false;
            this.btnRight.Visible = false;
            this.btnpresetEdit.Visible = false;
            this.btnresetApply.Visible = false;
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
            this.btnPresetPage1.Visible = false;
            this.btnPresetPage2.Visible = false;
            this.btnPresetPage3.Visible = false;
            this.btnPresetPage4.Visible = false;
            this.btnPresetPage5.Visible = false;
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
    }
}
