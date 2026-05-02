using CharaSimResource;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WzComparerR2.AvatarCommon;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;
using WzComparerR2.PluginBase;
using WzComparerR2.WzLib;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmStat : AlphaForm
    {
        public AfrmStat()
        {
            sec = new int[2];
            for (int i = 0; i < sec.Length; i++)
                sec[i] = 1 << i;

            hyperStatList = new int[] { 80000400, 80000401, 80000402, 80000403, 80000404, 80000405, 80000406, 80000409, 80000410, 80000412, 80000413, 80000414, 80000422, 80000416, 80000419, 80000420, 80000421 };
            hyperStatBitmapList = hyperStatList.Select(id => Resource.ResourceManager.GetObject("UICharacterInfo_img_common_detailStat_HyperStat_Window_statList_" + id) as Bitmap).ToArray();

            initCtrl();
        }

        private BitVector32 partVisible;
        private int[] sec;
        private Point baseOffset;
        private Point newLocation;
        private Character character;
        private List<TooltipHelpRect> helpList;
        private List<TooltipHelpRect> helpDetailList;
        private List<TooltipHelpRect> detailStatList;
        private int hyperStatScrollValue;
        private int[] hyperStatList;
        private List<int> GearList = new List<int>();
        private List<int> SlotIndexList = new List<int>();
        private List<int> SkillList = new List<int>();
        private List<string> SkillNames = new List<string>();
        private List<int> SkillLevels = new List<int>();
        private Bitmap[] hyperStatBitmapList;
        private Skill[] hyperStatSkillList;
        private Skill[] skillList;
        private Gear[] gearList;

        private ContextMenuStrip menu;
        private ACtrlVScroll vScroll;
        private ACtrlButton btnClose;
        private ACtrlButton btntoggleDetailOpen;
        private ACtrlButton btntoggleDetailClose;
        private ACtrlButton btnDetailOpen;
        private ACtrlButton btnDetailClose;
        private ACtrlButton btnHyperStatOpen;
        private ACtrlButton btnHyperStatClose;
        private ACtrlButton btnhelp;
        private ACtrlButton btnparty;
        private ACtrlButton btnguild;
        private ACtrlButton btnexchange;
        private ACtrlButton btnmyhome;
        private ACtrlButton btnpopularityUp;
        private ACtrlButton btnpopularityDown;
        private ACtrlButton btndetailTab;
        private ACtrlButton btndetailTab2;
        private ACtrlButton btndetailTab3;
        private ACtrlButton btndetailTab4;
        private ACtrlButton btnpresetPage1;
        private ACtrlButton btnpresetPage2;
        private ACtrlButton btnpresetPage3;
        private ACtrlButton btnStatFont;
        private ACtrlButton btnArc;
        private ACtrlButton btnAut;
        private ACtrlButton btnLinkSkill;
        private ACtrlButton btnVSkill;
        private ACtrlButton btnHexaSkill;
        private ACtrlButton btnCash;
        private ACtrlButton btnCashPreset;
        private ACtrlButton btnHpUp;
        private ACtrlButton btnLVUp1;
        private ACtrlButton btnLVUp2;
        private ACtrlButton btnLVUp3;
        private ACtrlButton btnLVUp4;
        private ACtrlButton btnLVUp5;
        private ACtrlButton btnLVUp6;
        private ACtrlButton btnLVUp7;
        private ACtrlButton btnLVUp8;
        private ACtrlButton btnLVUp9;
        private ACtrlButton btnLVUp10;
        private ACtrlButton btnLVUp11;
        private ACtrlButton btnLVUp12;
        private ACtrlButton btnReduce;
        private bool waitForRefresh;

        private AvatarCanvasManager avatar { get; set; }
        public JObject resultJson = null;//基础信息
        public JObject resultJson2 = null;//人气度信息
        public JObject resultJson3 = null;//角色属性信息
        public JObject resultJson4 = null;//超级属性信息
        public JObject resultJson5 = null;//内在能力信息
        public JObject resultJson6 = null;//V矩阵信息
        public JObject resultJson7 = null;//HEXA矩阵信息
        public JObject resultJson8 = null;//链接技能信息
        public JObject resultJson9 = null;//装备道具信息
        private string character_name = "WzComparerR2";
        private string character_class = "元素师";
        private string character_guild_name = "-";
        private string liberation_quest_clear = "0";
        private string damage = "0.00";
        private string bossDam = "0.00";
        private string finalDam = "0.00";
        private string ignoreDEF = "0.00";
        private string normalDam = "0.00";
        private string criRate = "0.00";
        private string criDam = "0.00";
        private string cooldownReduceSec = "5";
        private string cooldownReduce = "0";
        private string cooldownIgnore = "0";
        private string buffDuration = "0.00";
        private string elemResistance = "0.00";
        private string abnormalDam = "0.00";
        private string tamingmobDuration = "0";
        private string starforce = "0";
        private string arcforce = "0";
        private string autforce = "0";
        private string mesoRate = "0";
        private string dropRate = "0";
        private string expRate = "0.00";
        private string statusResistance = "0";
        private string stance = "0";
        private string defense = "0";
        private string movement = "100";
        private string jump = "100";
        private string attackSpeed = "1";
        private string ability_grade = "legendary";
        private int character_level = 281;
        public int union_level = 281;
        public int dojang_best_floor = 0;
        private int popularity = 0;
        private long attack_range = 49999999;
        private long combat_power = 49999999;
        private int STR = 2052;
        private int DEX = 1670;
        private int INT = 39263;
        private int LUK = 3011;
        private int HP = 50000;
        private int MP = 50000;
        private int ATT = 1082;
        private int MATT = 3095;

        public event ObjectMouseEventHandler ObjectMouseMove;
        public event EventHandler ObjectMouseLeave;

        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        public bool AbilityVisible
        {
            get { return partVisible[sec[0]]; }
            private set { partVisible[sec[0]] = value; }
        }

        public bool HyperStatVisible
        {
            get { return partVisible[sec[1]]; }
            private set { partVisible[sec[1]] = value; }
        }

        public bool DetailVisible = false;
        public bool StatVisible = false;
        public bool EquipVisible = false;
        public bool SkillVisible = false;
        public bool CashVisible = false;
        public bool RingSlotVisible = false;
        public int statFont = 1;
        public int presetPage = 1;
        public int ArcAut = 1;
        public int SkillTab = 1;
        public int CashPreset = 1;
        public List<int> hyperStats = Enumerable.Repeat(0, 17).ToList();
        public List<int> hyperStats2 = Enumerable.Repeat(0, 17).ToList();
        public List<int> hyperStats3 = Enumerable.Repeat(0, 17).ToList();
        public List<int> hyperStatsOnUse = new List<int>();
        public string use_preset_no = "1";
        public int scrollValue = 0;

        private Rectangle DetailRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X + Resource.UICharacterInfo_img_common_main_backgrnd.Width + 1, baseOffset.Y + 1),
                    Resource.UICharacterInfo_img_remote_detailStat_ability_backgrnd.Size);
            }
        }

        private Rectangle HyperStatRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_backgrnd.Width - 1, baseOffset.Y + 709 - Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_backgrnd.Height),
                    Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_backgrnd.Size);
            }
        }

        private Rectangle RingSlot
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.UICharacterInfo_img_remote_detailEquip_skillRingEquip_canvas_skillRing.Width - 1, baseOffset.Y + 351),
                    Resource.UICharacterInfo_img_remote_detailEquip_skillRingEquip_canvas_skillRing.Size);
            }
        }

        private void initCtrl()
        {
            this.menu = new ContextMenuStrip();
            this.menu.Items.Add(new ToolStripMenuItem("复制", null, tsmiCopy_Click));
            this.menu.Items.Add(new ToolStripMenuItem("保存PNG", null, tsmiSave_Click));
            this.MouseClick += AfrmStat_MouseClick;

            this.vScroll = new ACtrlVScroll();  //鼠标滑轮区域

            this.vScroll.PicBase.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_base);
            this.vScroll.PicBase.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_base);

            this.vScroll.BtnPrev.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_prev0);
            this.vScroll.BtnPrev.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_prev1);
            this.vScroll.BtnPrev.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_prev2);
            this.vScroll.BtnPrev.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_prev0);
            this.vScroll.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll.BtnPrev.Location = new Point(0, 0);

            this.vScroll.BtnNext.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_next0);
            this.vScroll.BtnNext.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_next1);
            this.vScroll.BtnNext.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_next2);
            this.vScroll.BtnNext.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_next0);
            this.vScroll.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll.BtnNext.Location = new Point(0, 344);

            this.vScroll.BtnThumb.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_thumb0);
            this.vScroll.BtnThumb.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_thumb1);
            this.vScroll.BtnThumb.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailSkill_scroll_slot_enabled_thumb2);
            this.vScroll.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll.Visible = false;
            this.vScroll.ValueChanged += new EventHandler(vScroll_ValueChanged);
            this.vScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton(); //主页关闭按钮
            this.btnClose.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_normal_0);
            this.btnClose.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_pressed_0);
            this.btnClose.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_mouseOver_0);
            this.btnClose.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_disabled_0);
            this.btnClose.Location = new Point(449, 12);
            this.btnClose.Size = new Size(11, 11);
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);

            this.btntoggleDetailOpen = new ACtrlButton();  //详情按钮开启
            this.btntoggleDetailOpen.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_normal_0);
            this.btntoggleDetailOpen.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_pressed_0);
            this.btntoggleDetailOpen.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_mouseOver_0);
            this.btntoggleDetailOpen.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_disabled_0);
            this.btntoggleDetailOpen.Location = new Point(10, 201);
            this.btntoggleDetailOpen.Size = new Size(452, 21);
            this.btntoggleDetailOpen.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btntoggleDetailOpen.MouseClick += new MouseEventHandler(btntoggleDetailOpen_MouseClick);

            this.btntoggleDetailClose = new ACtrlButton(); //详情按钮关闭
            this.btntoggleDetailClose.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_normal_0);
            this.btntoggleDetailClose.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_pressed_0);
            this.btntoggleDetailClose.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_mouseOver_0);
            this.btntoggleDetailClose.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttontoggleDetail_disabled_0);
            this.btntoggleDetailClose.Location = new Point(10, 201);
            this.btntoggleDetailClose.Size = new Size(452, 21);
            this.btntoggleDetailClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btntoggleDetailClose.MouseClick += new MouseEventHandler(btntoggleDetailClose_MouseClick);

            this.btnDetailOpen = new ACtrlButton();  //内在能力按钮开启
            this.btnDetailOpen.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_normal_0);
            this.btnDetailOpen.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_pressed_0);
            this.btnDetailOpen.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_mouseOver_0);
            this.btnDetailOpen.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_disabled_0);
            this.btnDetailOpen.Location = new Point(355, 679);
            this.btnDetailOpen.Size = new Size(106, 24);
            this.btnDetailOpen.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDetailOpen.MouseClick += new MouseEventHandler(btnDetailOpen_MouseClick);

            this.btnDetailClose = new ACtrlButton();  //内在能力按钮关闭
            this.btnDetailClose.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_normal_0);
            this.btnDetailClose.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_pressed_0);
            this.btnDetailClose.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_mouseOver_0);
            this.btnDetailClose.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonability_disabled_0);
            this.btnDetailClose.Location = new Point(355, 679);
            this.btnDetailClose.Size = new Size(106, 24);
            this.btnDetailClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDetailClose.MouseClick += new MouseEventHandler(btnDetailClose_MouseClick);

            this.btnHyperStatOpen = new ACtrlButton();  //超级属性按钮开启
            this.btnHyperStatOpen.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_normal_0);
            this.btnHyperStatOpen.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_pressed_0);
            this.btnHyperStatOpen.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_mouseOver_0);
            this.btnHyperStatOpen.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_disabled_0);
            this.btnHyperStatOpen.Location = new Point(12, 679);
            this.btnHyperStatOpen.Size = new Size(106, 24);
            this.btnHyperStatOpen.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHyperStatOpen.MouseClick += new MouseEventHandler(btnHyperStatOpen_MouseClick);

            this.btnHyperStatClose = new ACtrlButton();  //超级属性按钮关闭
            this.btnHyperStatClose.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_normal_0);
            this.btnHyperStatClose.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_pressed_0);
            this.btnHyperStatClose.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_mouseOver_0);
            this.btnHyperStatClose.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhyper_disabled_0);
            this.btnHyperStatClose.Location = new Point(12, 679);
            this.btnHyperStatClose.Size = new Size(106, 24);
            this.btnHyperStatClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHyperStatClose.MouseClick += new MouseEventHandler(btnHyperStatClose_MouseClick);

            this.btnHpUp = new ACtrlButton();  //内在能力按钮关闭(X)
            this.btnHpUp.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_normal_0);
            this.btnHpUp.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_pressed_0);
            this.btnHpUp.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_mouseOver_0);
            this.btnHpUp.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_disabled_0);
            this.btnHpUp.Location = new Point(222, Resource.UICharacterInfo_img_common_main_backgrnd.Height + Resource.UICharacterInfo_img_local_detail_backgrnd.Height - Resource.UICharacterInfo_img_remote_detailStat_ability_backgrnd.Height + 10);
            this.btnHpUp.Size = new Size(11, 11);
            this.btnHpUp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHpUp.MouseClick += new MouseEventHandler(btnDetailClose_MouseClick);

            this.btnReduce = new ACtrlButton();  //超级属性按钮关闭(X)
            this.btnReduce.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_normal_0);
            this.btnReduce.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_pressed_0);
            this.btnReduce.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_mouseOver_0);
            this.btnReduce.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonclose_disabled_0);
            this.btnReduce.Location = new Point(192, 11);
            this.btnReduce.Size = new Size(11, 11);
            this.btnReduce.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnReduce.MouseClick += new MouseEventHandler(btnHyperStatClose_MouseClick);

            this.btnhelp = new ACtrlButton();  //帮助按钮
            this.btnhelp.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhelp_normal_0);
            this.btnhelp.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhelp_pressed_0);
            this.btnhelp.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhelp_mouseOver_0);
            this.btnhelp.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_detailStat_buttonhelp_disabled_0);
            this.btnhelp.Location = new Point(430, 276);
            this.btnhelp.Size = new Size(112, 20);
            this.btnhelp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnparty = new ACtrlButton();  //组队邀请按钮
            this.btnparty.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonparty_normal_0);
            this.btnparty.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonparty_pressed_0);
            this.btnparty.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonparty_mouseOver_0);
            this.btnparty.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonparty_disabled_0);
            this.btnparty.Location = new Point(341, 102);
            this.btnparty.Size = new Size(112, 20);
            this.btnparty.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnguild = new ACtrlButton();  //家族按钮
            this.btnguild.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonguild_normal_0);
            this.btnguild.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonguild_pressed_0);
            this.btnguild.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonguild_mouseOver_0);
            this.btnguild.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_common_main_buttonguild_disabled_0);
            this.btnguild.Location = new Point(341, 124);
            this.btnguild.Size = new Size(112, 20);
            this.btnguild.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnexchange = new ACtrlButton();  //交换申请按钮
            this.btnexchange.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonexchange_normal_0);
            this.btnexchange.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonexchange_pressed_0);
            this.btnexchange.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonexchange_mouseOver_0);
            this.btnexchange.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonexchange_disabled_0);
            this.btnexchange.Location = new Point(19, 102);
            this.btnexchange.Size = new Size(112, 20);
            this.btnexchange.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnpopularityUp = new ACtrlButton();  //人气度增加按钮
            this.btnpopularityUp.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityUp_normal_0);
            this.btnpopularityUp.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityUp_pressed_0);
            this.btnpopularityUp.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityUp_mouseOver_0);
            this.btnpopularityUp.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityUp_disabled_0);
            this.btnpopularityUp.Location = new Point(63, 172);
            this.btnpopularityUp.Size = new Size(12, 12);
            this.btnpopularityUp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnpopularityDown = new ACtrlButton();  //人气度降低按钮
            this.btnpopularityDown.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityDown_normal_0);
            this.btnpopularityDown.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityDown_pressed_0);
            this.btnpopularityDown.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityDown_mouseOver_0);
            this.btnpopularityDown.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonpopularityDown_disabled_0);
            this.btnpopularityDown.Location = new Point(77, 172);
            this.btnpopularityDown.Size = new Size(12, 12);
            this.btnpopularityDown.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnmyhome = new ACtrlButton();  //我的小屋按钮
            this.btnmyhome.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonmyhome_normal_0);
            this.btnmyhome.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonmyhome_pressed_0);
            this.btnmyhome.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonmyhome_mouseOver_0);
            this.btnmyhome.Disabled = new BitmapOrigin(Resource.UICharacterInfo_img_remote_main_buttonmyhome_disabled_0);
            this.btnmyhome.Location = new Point(341, 80);
            this.btnmyhome.Size = new Size(112, 20);
            this.btnmyhome.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btndetailTab = new ACtrlButton();
            this.btndetailTab.Location = new Point(11, 242);
            this.btndetailTab.Size = new Size(111, 20);
            this.btndetailTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btndetailTab.MouseClick += new MouseEventHandler(btndetailTab_MouseClick);

            this.btndetailTab2 = new ACtrlButton();
            this.btndetailTab2.Location = new Point(124, 242);
            this.btndetailTab2.Size = new Size(111, 20);
            this.btndetailTab2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btndetailTab2.MouseClick += new MouseEventHandler(btndetailTab2_MouseClick);

            this.btndetailTab3 = new ACtrlButton();
            this.btndetailTab3.Location = new Point(237, 242);
            this.btndetailTab3.Size = new Size(111, 20);
            this.btndetailTab3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btndetailTab3.MouseClick += new MouseEventHandler(btndetailTab3_MouseClick);

            this.btndetailTab4 = new ACtrlButton();
            this.btndetailTab4.Location = new Point(350, 242);
            this.btndetailTab4.Size = new Size(111, 20);
            this.btndetailTab4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btndetailTab4.MouseClick += new MouseEventHandler(btndetailTab4_MouseClick);

            this.btnpresetPage1 = new ACtrlButton();
            this.btnpresetPage1.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage1_normal_0);
            this.btnpresetPage1.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage1_mouseOver_0);
            this.btnpresetPage1.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage1_pressed_0);
            this.btnpresetPage1.Location = new Point(136, 421);
            this.btnpresetPage1.Size = new Size(12, 12);
            this.btnpresetPage1.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnpresetPage1.MouseClick += new MouseEventHandler(btnpresetPage1_MouseClick);

            this.btnpresetPage2 = new ACtrlButton();
            this.btnpresetPage2.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage2_normal_0);
            this.btnpresetPage2.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage2_mouseOver_0);
            this.btnpresetPage2.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage2_pressed_0);
            this.btnpresetPage2.Location = new Point(156, 421);
            this.btnpresetPage2.Size = new Size(12, 12);
            this.btnpresetPage2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnpresetPage2.MouseClick += new MouseEventHandler(btnpresetPage2_MouseClick);

            this.btnpresetPage3 = new ACtrlButton();
            this.btnpresetPage3.Normal = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage3_normal_0);
            this.btnpresetPage3.MouseOver = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage3_mouseOver_0);
            this.btnpresetPage3.Pressed = new BitmapOrigin(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage3_pressed_0);
            this.btnpresetPage3.Location = new Point(176, 421);
            this.btnpresetPage3.Size = new Size(12, 12);
            this.btnpresetPage3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnpresetPage3.MouseClick += new MouseEventHandler(btnpresetPage3_MouseClick);

            this.btnStatFont = new ACtrlButton();
            this.btnStatFont.Location = new Point(220, 658);
            this.btnStatFont.Size = new Size(33, 8);
            this.btnStatFont.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnStatFont.MouseClick += new MouseEventHandler(btnStatFont_MouseClick);

            this.btnArc = new ACtrlButton();
            this.btnArc.Location = new Point(261, 280);
            this.btnArc.Size = new Size(92, 20);
            this.btnArc.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnArc.MouseClick += new MouseEventHandler(btnArc_MouseClick);

            this.btnAut = new ACtrlButton();
            this.btnAut.Location = new Point(353, 280);
            this.btnAut.Size = new Size(92, 20);
            this.btnAut.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnAut.MouseClick += new MouseEventHandler(btnAut_MouseClick);

            this.btnLinkSkill = new ACtrlButton();
            this.btnLinkSkill.Location = new Point(27, 285);
            this.btnLinkSkill.Size = new Size(136, 31);
            this.btnLinkSkill.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnLinkSkill.MouseClick += new MouseEventHandler(btnLinkSkill_MouseClick);

            this.btnVSkill = new ACtrlButton();
            this.btnVSkill.Location = new Point(168, 285);
            this.btnVSkill.Size = new Size(136, 31);
            this.btnVSkill.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnVSkill.MouseClick += new MouseEventHandler(btnVSkill_MouseClick);

            this.btnHexaSkill = new ACtrlButton();
            this.btnHexaSkill.Location = new Point(309, 285);
            this.btnHexaSkill.Size = new Size(136, 31);
            this.btnHexaSkill.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHexaSkill.MouseClick += new MouseEventHandler(btnHexaSkill_MouseClick);

            this.btnCash = new ACtrlButton();
            this.btnCash.Location = new Point(27, 280);
            this.btnCash.Size = new Size(89, 20);
            this.btnCash.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnCash.MouseClick += new MouseEventHandler(btnCash_MouseClick);

            this.btnCashPreset = new ACtrlButton();
            this.btnCashPreset.Location = new Point(116, 280);
            this.btnCashPreset.Size = new Size(89, 20);
            this.btnCashPreset.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnCashPreset.MouseClick += new MouseEventHandler(btnCashPreset_MouseClick);
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return vScroll;
                yield return btnClose;
                yield return btntoggleDetailOpen;
                yield return btntoggleDetailClose;
                yield return btnDetailOpen;
                yield return btnDetailClose;
                yield return btnHyperStatOpen;
                yield return btnHyperStatClose;
                yield return btnhelp;
                yield return btnparty;
                yield return btnguild;
                yield return btnexchange;
                yield return btnpopularityUp;
                yield return btnpopularityDown;
                yield return btnmyhome;
                yield return btndetailTab;
                yield return btndetailTab2;
                yield return btndetailTab3;
                yield return btndetailTab4;
                yield return btnStatFont;
                yield return btnArc;
                yield return btnAut;
                yield return btnLinkSkill;
                yield return btnVSkill;
                yield return btnHexaSkill;
                yield return btnCash;
                yield return btnCashPreset;
            }
        }

        private IEnumerable<AControl> aDetailControls
        {
            get
            {
                yield return btnHpUp;
            }
        }

        private IEnumerable<AControl> aHyperStatControls
        {
            get
            {
                yield return btnReduce;
                yield return btnpresetPage1;
                yield return btnpresetPage2;
                yield return btnpresetPage3;
            }
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(Resource.UICharacterInfo_img_common_main_backgrnd.Width, 24));
            this.Location = newLocation;
            base.Refresh();
        }

        protected override bool captionHitTest(Point point)
        {
            Rectangle rect = this.btnClose.Rectangle;
            rect.Offset(this.baseOffset);
            if (rect.Contains(point))
                return false;
            return base.captionHitTest(point);
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();

            setControlState();
            get_charinfo();

            Point baseOffsetnew = calcRenderBaseOffset();
            Size size = Resource.UICharacterInfo_img_common_main_backgrnd.Size;
            if (this.DetailVisible)
                size = new Size(size.Width, size.Height + Resource.UICharacterInfo_img_local_detail_backgrnd.Height);
            size.Width += baseOffsetnew.X + 2;
            size.Height += 1;
            if (this.AbilityVisible)
                size = new Size(size.Width + Resource.UICharacterInfo_img_remote_detailStat_ability_backgrnd.Width, size.Height);

            this.newLocation = new Point(this.Location.X + this.baseOffset.X - baseOffsetnew.X,
                this.Location.Y + this.baseOffset.Y - baseOffsetnew.Y);
            this.baseOffset = baseOffsetnew;

            //绘制背景
            Bitmap stat = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(stat);
            renderBase(g);
            if (this.DetailVisible)
                renderTab(g);
            if (this.StatVisible)
                renderDetail(g);
            if (this.EquipVisible)
                renderEquip(g);
            if (this.SkillVisible)
                renderSkill(g);
            if (this.CashVisible)
                renderCash(g);
            if (this.AbilityVisible)
                renderAbility(g);
            if (this.HyperStatVisible)
                renderHyperStat(g);

            //绘制按钮
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.Draw(g);
            }
            g.ResetTransform();

            g.TranslateTransform(this.DetailRect.X, this.DetailRect.Y);
            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.Draw(g);
            }
            g.ResetTransform();

            g.TranslateTransform(this.HyperStatRect.X, this.HyperStatRect.Y);
            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.Draw(g);
            }
            g.ResetTransform();

            g.Dispose();
            this.Bitmap = stat;

            if (detailStatList == null)
            {
                this.detailStatList = new List<TooltipHelpRect>();
                foreach (Wz_Node helpNode in PluginBase.PluginManager.FindWz("UI/UICharacterInfo.img/common/detailStat/Stat")?.Nodes ?? Enumerable.Empty<Wz_Node>())
                {
                    Wz_Vector lt = helpNode.Nodes["clickRangeLT"]?.Value as Wz_Vector ?? new Wz_Vector(0, 0);
                    Wz_Vector rb = helpNode.Nodes["clickRangeRB"]?.Value as Wz_Vector ?? new Wz_Vector(0, 0);
                    detailStatList.Add(new TooltipHelpRect(new Rectangle(lt.X + 12, lt.Y + 269, rb.X - lt.X, rb.Y - lt.Y), new TooltipHelp(helpNode.Nodes["Title"].GetValueEx<string>(null), helpNode.Nodes["Desc"].GetValueEx<string>(null))));
                }
                if (detailStatList.Count == 0)
                {
                    detailStatList = null;
                }
            }

            if (hyperStatSkillList == null)
            {
                try
                {
                    hyperStatSkillList = hyperStatList.Select(id => id.ToString().PadLeft(7, '0')).Select(id => Skill.CreateFromNode(PluginBase.PluginManager.FindWz("Skill/" + (Regex.IsMatch(id, @"80\d{6}") ? id.Substring(0, 6) : id.Substring(0, id.Length - 4)) + ".img/skill/" + id), PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz)).ToArray();
                }
                catch (Exception ex)
                {
                    hyperStatSkillList = null;
                }
            }
            if (skillList == null)
            {
                try
                {
                    skillList = this.SkillList.Select(id => Skill.CreateFromNode(PluginBase.PluginManager.FindWz("Skill/" + (Regex.IsMatch(id.ToString(), @"80\d{6}") ? id.ToString().PadLeft(7, '0').Substring(0, 6) : id.ToString().PadLeft(7, '0').Substring(0, id.ToString().Length - 4)) + ".img/skill/" + id.ToString()), PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz)).ToArray();
                }
                catch (Exception ex)
                {
                    skillList = null;
                }
            }
            if (gearList == null)
            {
                try
                {
                    gearList = this.GearList.Select(gearID =>
                    {
                        string nodePath = $@"{gearID:D8}.img";
                        foreach (Wz_Node category in PluginManager.FindWz("Character").Nodes)
                        {
                            if (category.Text.ToLower().Contains("canvas")) continue;

                            if (category.Text == nodePath)
                            {
                                var img = category.GetValueEx<Wz_Image>(null);
                                if (img != null && img.TryExtract())
                                {
                                    return Gear.CreateFromNode(img.Node, PluginManager.FindWz);
                                }
                            }

                            Wz_Node gearNode = category.FindNodeByPath(nodePath);
                            if (gearNode != null)
                            {
                                var img = gearNode.GetValueEx<Wz_Image>(null);
                                if (img != null)
                                {
                                    gearNode = img.TryExtract() ? img.Node : null;
                                }

                                return Gear.CreateFromNode(gearNode, PluginManager.FindWz);
                            }
                        }
                        return null;
                    }).ToArray();
                }
                catch (Exception ex) 
                { 
                    gearList = null;
                }
            }
        }

        private Point calcRenderBaseOffset()
        {
            if (this.HyperStatVisible)
                return new Point(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_backgrnd.Width, 0);
            else
                return new Point(0, 0);
        }

        private void setControlState()
        {
            this.btntoggleDetailOpen.Visible = !this.DetailVisible;
            this.btntoggleDetailClose.Visible = this.DetailVisible;
            this.btnDetailOpen.Visible = this.StatVisible && !this.AbilityVisible && this.DetailVisible;
            this.btnDetailClose.Visible = this.AbilityVisible && this.StatVisible;
            this.btnHpUp.Visible = this.AbilityVisible;
            this.btnhelp.Visible = this.StatVisible && this.DetailVisible;
            this.btnHyperStatClose.Visible = this.HyperStatVisible && this.StatVisible;
            this.btnHyperStatOpen.Visible = !this.HyperStatVisible && this.StatVisible;
            this.btnReduce.Visible = this.HyperStatVisible;
            this.vScroll.Visible = this.SkillVisible && this.DetailVisible;
            this.vScroll.Location = new Point(448, 323);
            this.vScroll.Size = new Size(7, 366);
            this.vScroll.ScrollableLocation = new Point(13, 323);
            this.vScroll.ScrollableSize = new Size(448, 366);

            if (this.character != null)
            {
                CharacterStatus charStat = this.character.Status;
                //setButtonEnabled(this.btnHPUp, charStat.Ap > 0 && charStat.MaxHP.BaseVal < charStat.MaxHP.TotalMax);
                //setButtonEnabled(this.btnMPUp, charStat.Ap > 0 && charStat.MaxMP.BaseVal < charStat.MaxMP.TotalMax);
                //setButtonEnabled(this.btnStrUp, charStat.Ap > 0/* && charStat.Strength.BaseVal <= 999*/);
                //setButtonEnabled(this.btnDexUp, charStat.Ap > 0/* && charStat.Dexterity.BaseVal <= 999*/);
                //setButtonEnabled(this.btnIntUp, charStat.Ap > 0/* && charStat.Intelligence.BaseVal <= 999*/);
                //setButtonEnabled(this.btnLukUp, charStat.Ap > 0/* && charStat.Luck.BaseVal <= 999*/);
                //setButtonEnabled(this.btnAuto, charStat.Ap > 0);
            }
            else
            {
                foreach (AControl ctrl in this.aControls)
                {
                    setButtonEnabled(ctrl as ACtrlButton, true);
                }
            }
        }

        private void get_charinfo()
        {
            if (resultJson != null)
            {
                this.character_name = resultJson["character_name"].ToString();
                this.character_class = resultJson["character_class"].ToString();
                this.character_guild_name = resultJson["character_guild_name"].ToString();
                this.liberation_quest_clear = resultJson["liberation_quest_clear"].ToString();
                this.character_level = resultJson["character_level"].ToObject<int>();
            }
            if (resultJson2 != null)
            {
                this.popularity = resultJson2["popularity"].ToObject<int>();
            }
            if (resultJson3 != null)
            {
                this.attack_range = resultJson3["final_stat"][0]["stat_value"].ToObject<long>();
                this.damage = resultJson3["final_stat"][2]["stat_value"].ToObject<string>();
                this.bossDam = resultJson3["final_stat"][3]["stat_value"].ToObject<string>();
                this.finalDam = resultJson3["final_stat"][4]["stat_value"].ToObject<string>();
                this.ignoreDEF = resultJson3["final_stat"][5]["stat_value"].ToObject<string>();
                this.criRate = resultJson3["final_stat"][6]["stat_value"].ToObject<string>();
                this.criDam = resultJson3["final_stat"][7]["stat_value"].ToObject<string>();
                this.statusResistance = resultJson3["final_stat"][8]["stat_value"].ToObject<string>();
                this.stance = resultJson3["final_stat"][9]["stat_value"].ToObject<string>();
                this.defense = resultJson3["final_stat"][10]["stat_value"].ToObject<string>();
                this.movement = resultJson3["final_stat"][11]["stat_value"].ToObject<string>();
                this.jump = resultJson3["final_stat"][12]["stat_value"].ToObject<string>();
                this.starforce = resultJson3["final_stat"][13]["stat_value"].ToObject<string>();
                this.arcforce = resultJson3["final_stat"][14]["stat_value"].ToObject<string>();
                this.autforce = resultJson3["final_stat"][15]["stat_value"].ToObject<string>();
                this.STR = resultJson3["final_stat"][16]["stat_value"].ToObject<int>();
                this.DEX = resultJson3["final_stat"][17]["stat_value"].ToObject<int>();
                this.INT = resultJson3["final_stat"][18]["stat_value"].ToObject<int>();
                this.LUK = resultJson3["final_stat"][19]["stat_value"].ToObject<int>();
                this.HP = resultJson3["final_stat"][20]["stat_value"].ToObject<int>();
                this.MP = resultJson3["final_stat"][21]["stat_value"].ToObject<int>();
                this.dropRate = resultJson3["final_stat"][28]["stat_value"].ToObject<string>();
                this.mesoRate = resultJson3["final_stat"][29]["stat_value"].ToObject<string>();
                this.buffDuration = resultJson3["final_stat"][30]["stat_value"].ToObject<string>();
                this.attackSpeed = resultJson3["final_stat"][31]["stat_value"].ToObject<string>();
                this.normalDam = resultJson3["final_stat"][32]["stat_value"].ToObject<string>();
                this.cooldownReduceSec = resultJson3["final_stat"][33]["stat_value"].ToObject<string>();
                this.cooldownReduce = resultJson3["final_stat"][34]["stat_value"].ToObject<string>();
                this.cooldownIgnore = resultJson3["final_stat"][35]["stat_value"].ToObject<string>();
                this.elemResistance = resultJson3["final_stat"][36]["stat_value"].ToObject<string>();
                this.abnormalDam = resultJson3["final_stat"][37]["stat_value"].ToObject<string>();
                this.expRate = resultJson3["final_stat"][39]["stat_value"].ToObject<string>();
                this.ATT = resultJson3["final_stat"][40]["stat_value"].ToObject<int>();
                this.MATT = resultJson3["final_stat"][41]["stat_value"].ToObject<int>();
                this.combat_power = resultJson3["final_stat"][42]["stat_value"].ToObject<long>();
                this.tamingmobDuration = resultJson3["final_stat"][43]["stat_value"].ToObject<string>();
            }
            if (resultJson4 != null)
            {
                use_preset_no = resultJson4["use_preset_no"].ToObject<string>();
                hyperStats = resultJson4["hyper_stat_preset_1"].Select(item => item["stat_level"]?.ToObject<int>() ?? 0).ToList();
                hyperStats2 = resultJson4["hyper_stat_preset_2"].Select(item => item["stat_level"]?.ToObject<int>() ?? 0).ToList();
                hyperStats3 = resultJson4["hyper_stat_preset_3"].Select(item => item["stat_level"]?.ToObject<int>() ?? 0).ToList();
                switch (presetPage)
                {
                    case 1: hyperStatsOnUse = hyperStats; break;
                    case 2: hyperStatsOnUse = hyperStats2; break;
                    case 3: hyperStatsOnUse = hyperStats3; break;
                }
            }
            if (resultJson5 != null)
            {
                ability_grade = ability_rank(resultJson5["ability_grade"].ToObject<String>());
            }
            if (resultJson6 != null && SkillTab == 2)
            {
                SkillList.Clear();
                SkillNames.Clear();
                SkillLevels.Clear();
                var character_skill = resultJson6["character_skill"] as JArray;
                foreach (var skill in character_skill)
                {
                    string skill_icon = skill["skill_icon"].ToString();
                    string skill_name = skill["skill_name"].ToString();
                    int skill_level = skill["skill_level"].Value<int>();
                    int convertedValue = indexConversion(skill_icon.Substring(skill_icon.LastIndexOf('/') + 1).Trim());
                    SkillList.Add(convertedValue);
                    SkillNames.Add(skill_name);
                    SkillLevels.Add(skill_level);
                }
                this.vScroll.Maximum = SkillList.Count > 12 ? SkillList.Count / 2 - 6 : 0;
            }
            else if (resultJson7 != null && SkillTab == 3)
            {
                SkillList.Clear();
                SkillNames.Clear();
                SkillLevels.Clear();
                var character_skill = resultJson7["character_skill"] as JArray;
                foreach (var skill in character_skill)
                {
                    string skill_icon = skill["skill_icon"].ToString();
                    string skill_name = skill["skill_name"].ToString();
                    int skill_level = skill["skill_level"].Value<int>();
                    int convertedValue = indexConversion(skill_icon.Substring(skill_icon.LastIndexOf('/') + 1).Trim());
                    SkillList.Add(convertedValue);
                    SkillNames.Add(skill_name);
                    SkillLevels.Add(skill_level);
                }
                this.vScroll.Maximum = SkillList.Count > 12 ? SkillList.Count / 2 - 6 : 0;
            }
            else if (resultJson8 != null && SkillTab == 1)
            {
                SkillList.Clear();
                SkillNames.Clear();
                SkillLevels.Clear();
                var character_link_skill = resultJson8["character_link_skill"] as JArray;
                foreach (var skill in character_link_skill)
                {
                    string skill_icon = skill["skill_icon"].ToString();
                    string skill_name = skill["skill_name"].ToString();
                    int skill_level = skill["skill_level"].Value<int>();
                    int convertedValue = indexConversion(skill_icon.Substring(skill_icon.LastIndexOf('/') + 1).Trim());
                    SkillList.Add(convertedValue);
                    SkillNames.Add(skill_name);
                    SkillLevels.Add(skill_level);
                }
                this.vScroll.Maximum = 0;
            }
            if (resultJson9 != null && EquipVisible)
            {
                GearList.Clear();
                SlotIndexList.Clear();
                var item_equipment = resultJson9["item_equipment"] as JArray;
                foreach ( var equip in item_equipment)
                {
                    string item_icon = equip["item_icon"].ToString();
                    string item_equipment_slot = equip["item_equipment_slot"].ToString();
                    int slotIndex = GetSlotIndex(item_equipment_slot);
                    int convertedValue = ItemIndexConversion(item_icon.Substring(item_icon.LastIndexOf('/') + 1).Trim());
                    GearList.Add(convertedValue);
                    SlotIndexList.Add(slotIndex);
                }
            }
        }

        private void setButtonEnabled(ACtrlButton button, bool enabled)
        {
            if (button == null)
                return;
            if (enabled)
            {
                if (button.State == ButtonState.Disabled)
                {
                    button.State = ButtonState.Normal;
                }
            }
            else
            {
                if (button.State != ButtonState.Disabled)
                {
                    button.State = ButtonState.Disabled;
                }
            }
        }

        private static string ToCJKNumberExpr(long value)
        {
            var sb = new StringBuilder(32);
            bool firstPart = true;
            if (value >= 1_0000_0000_0000_0000)
            {
                long part = value / 1_0000_0000_0000_0000;
                sb.AppendFormat("{0}京", part); // Korean: 교, Chinese+Japanese: 京
                value -= part * 1_0000_0000_0000_0000;
                firstPart = false;
            }
            if (value >= 1_0000_0000_0000)
            {
                long part = value / 1_0000_0000_0000;
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}兆", part); // Korean: 조, Chinese+Japanese: 兆
                value -= part * 1_0000_0000_0000;
                firstPart = false;
            }
            if (value >= 1_0000_0000)
            {
                long part = value / 1_0000_0000;
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}亿", part); // Korean: 억, TradChinese+Japanese: 億, SimpChinese: 亿
                value -= part * 1_0000_0000;
                firstPart = false;
            }
            if (value >= 1_0000)
            {
                long part = value / 1_0000;
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}万", part); // Korean: 만, TradChinese: 萬, SimpChinese+Japanese: 万
                value -= part * 1_0000;
                firstPart = false;
            }
            if (value > 0)
            {
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}", value);
            }

            return sb.Length > 0 ? sb.ToString() : "0";
        }

        private static string ability_rank(string grade)
        {
            switch (grade)
            {
                case "레전드리": case "Legendary": case "傳說": return "legendary"; 
                case "유니크": case "Unique": case "罕見": return "unique";
                case "에픽": case "Epic": case "稀有": return "epic";
                default: return "normal";
            }
        }

        private void renderBase(Graphics g) //绘制角色信息界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_common_main_backgrnd, 1, 0);
            //g.DrawImage(Resource.UICharacterInfo_img_customBackground_5_image_0, 141, 32);
            g.DrawImage(Resource.UICharacterInfo_img_common_main_layername, 183, 32);
            g.DrawImage(Resource.UICharacterInfo_img_common_main_canvasmasterDisciple, 437, 39);
            if (liberation_quest_clear == "2")
                g.DrawImage(Resource.UICharacterInfo_img_common_main_layer_genesisPass, 342, 39);
            if (this.character != null)
            {
                g.DrawString(character_name, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, (472 - g.MeasureString(character_name, GearGraphics.ItemDetailFont).Width) / 2, 174f);
                g.DrawString(character_class, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, (150 - g.MeasureString(character_class, GearGraphics.ItemDetailFont).Width) / 2, 44f);
                g.DrawString(character_guild_name, GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 444f - g.MeasureString(character_guild_name, GearGraphics.ItemDetailFont).Width, 150f);
                g.DrawString("-", GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 444f - g.MeasureString("-", GearGraphics.ItemDetailFont).Width, 173f);
                g.DrawString(popularity.ToString().PadLeft(5), GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 92f, 173f);
                g.DrawString(character_level.ToString().PadLeft(3), GearGraphics.LevelBoldFont, GearGraphics.WhiteBrush, 234f, 35f);
                g.DrawString(union_level.ToString().PadLeft(5), GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 92f, 129f);
                g.DrawString(dojang_best_floor.ToString().PadLeft(3) + "层", GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 92f, 151f);
            }
            g.ResetTransform();
        }

        private void renderTab(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_local_detail_backgrnd, 1, 231);  //属性背景
            // 四选一标签
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_normal_0, 12, 242);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_normal_1, 125, 242);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_normal_2, 238, 242);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_normal_3, 351, 242);
            g.ResetTransform();
        }

        private void renderDetail(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_selected_0, 11, 242);  //属性标签
            g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasattackBack, 12, 269);  //战斗力背景
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_canvasmainStatBack, 12, 307);  //主属性背景
            g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasutilityBack, 12, 583);  //三选一属性背景
            g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasmainStatFont, 24, 321);  //主属性文本
            g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasattackFont, 24, 400);   //属性文本

            if (this.character != null)
            {
                CharacterStatus charStat = this.character.Status;
                int brushSign;

                double max, min;
                this.character.CalcAttack(out max, out min, out brushSign);
                g.DrawString(HP.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 187f, 320f);
                switch (charStat.Job)
                {
                    case 3101:
                    case 3120:
                    case 3121:
                    case 3122:
                    case 3124:
                    case 14000:
                    case 14200:
                    case 14210:
                    case 14211:
                    case 14212:
                    case 14213:
                    case 14214:
                        break;
                    case 3100:
                    case 3110:
                    case 3111:
                    case 3112:
                    case 3114:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_Stat_1_titleImageDF, 244, 321);
                        g.DrawString(charStat.SpecialValue.GetSum().ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 406f, 320f);
                        break;
                    case 10000:
                    case 10100:
                    case 10110:
                    case 10111:
                    case 10112:
                    case 10114:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_Stat_1_titleImageTF, 244, 321);
                        g.DrawString(charStat.SpecialValue.GetSum().ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 406f, 320f);
                        break;
                    default:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_Stat_1_titleImage, 244, 321);
                        g.DrawString(MP.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 406f, 320f);
                        break;
                }
                string CombatPower = ToCJKNumberExpr(combat_power);
                int xPosition = 200;
                foreach (char c in CombatPower)
                {
                    if (c == ' ')
                    {
                        xPosition += 4; continue;
                    }
                    char replacementChar = c;
                    if (c == '万') replacementChar = 'x';
                    else if (c == '亿') replacementChar = 'y';
                    else if (c == '兆') replacementChar = 'z';
                    string imageName = "UICharacterInfo_img_common_detailStat_attackPowerFont_" + replacementChar;
                    DrawImage(g, imageName, xPosition, 279);
                    xPosition += GetImageWidth(imageName);
                }
                g.DrawString(STR.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 187f, 342f);
                g.DrawString(DEX.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 406f, 342f);
                g.DrawString(INT.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 187f, 364f);
                g.DrawString(LUK.ToString("N0").PadLeft(7), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 406f, 364f);

                float y = 401f;
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Far;
                g.DrawString(ToCJKNumberExpr(attack_range), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, y, format);
                g.DrawString(damage + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(finalDam + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(bossDam + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(ignoreDEF + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(normalDam + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(ATT.ToString("N0"), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(criRate + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(MATT.ToString("N0"), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(criDam + "%", GearGraphics.ItemDetailFont,GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(cooldownReduceSec + "秒/" + cooldownReduce + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(buffDuration + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(cooldownIgnore + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(elemResistance + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);
                g.DrawString(abnormalDam + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 234f, (y += 22f), format);
                g.DrawString(tamingmobDuration + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 451f, y, format);

                y = 594f;
                switch (statFont)
                {
                    case 1:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasutilityFont, 24, 594);  //三选一属性第1页
                        g.DrawString(mesoRate + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, y, format);
                        g.DrawString(starforce, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 453f, y, format);
                        g.DrawString(dropRate + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, (y += 22f), format);
                        g.DrawString(arcforce, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 453f, y, format);
                        g.DrawString(expRate + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, (y += 22f), format);
                        g.DrawString(autforce, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 453f, y, format);
                        break;
                    case 2:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvasdefenseFont, 24, 594);  //三选一属性第2页
                        g.DrawString(defense, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, y, format);
                        g.DrawString(statusResistance, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 453f, y, format);
                        g.DrawString(movement + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, (y += 22f), format);
                        g.DrawString(jump + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 453f, y, format);
                        g.DrawString(stance + "%", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, (y += 22f), format);
                        g.DrawString("第" + attackSpeed + "阶段", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 455f, y, format);
                        break;
                    case 3:
                        g.DrawImage(Resource.UICharacterInfo_img_common_detailStat_canvascnFont, 24, 594);  //三选一属性第3页
                        g.DrawString(ToCJKNumberExpr(700000000000), GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 235f, y, format);
                        break;
                    default: break;
                }
            }
            g.ResetTransform();
        }

        private void DrawImage(Graphics g, string imageName, int x, int y)
        {
            System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(imageName) as System.Drawing.Bitmap;
            g.DrawImage(image, new Rectangle(x, y, image.Width, image.Height));
        }

        private int GetImageWidth(string imageName)
        {
            System.Drawing.Bitmap image = Resource.ResourceManager.GetObject(imageName) as System.Drawing.Bitmap;
            return image != null ? image.Width : 0;
        }

        private void renderEquip(Graphics g) //绘制装备界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_selected_1, 125, 242);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_canvasequip, 13, 269);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_tabsymbolTab_normal_0, 262, 280);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_tabsymbolTab_normal_1, 354, 280);
            if (gearList != null)
            {
                foreach (Gear gear in gearList)
                {
                    if (gear == null) continue;
                    // 检查 IconRaw 和 Bitmap
                    if (gear.IconRaw.Bitmap == null)
                    {
                        // 尝试使用 Icon 属性
                        if (gear.Icon.Bitmap == null)
                            continue; // 跳过没有图标的装备
                    }
                    Bitmap gearIcon = gear.IconRaw.Bitmap ?? gear.Icon.Bitmap;
                    int index = Array.IndexOf(gearList, gear);
                    int slotIndex = SlotIndexList[index];
                    if (slotIndex < 0) continue;
                    Wz_Vector vector = get_vector(slotIndex);
                    int x = vector.X + 13 + 21 - gearIcon.Width / 2;
                    int y = vector.Y + 269 + 21 - gearIcon.Height / 2;
                    g.DrawImage(gearIcon, new Point(x, y));
                }
            }

            switch (this.ArcAut)
            {
                case 1:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_tabsymbolTab_selected_0, 262, 280);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_backgrnd, 262, 305);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 280, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 330, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 380, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 280, 476);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 330, 476);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_ArcEquip_slotVector1_canvasslot, 380, 476);
                    break;
                case 2:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_tabsymbolTab_selected_1, 354, 280);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_backgrnd, 262, 305);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 282, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 332, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 382, 384);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 282, 476);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 332, 476);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailEquip_AutEquip_slotVector1_canvasslot, 382, 476);
                    break;
                default: break;
            }
            g.ResetTransform();
        }

        private void renderSkill(Graphics g)  //绘制技能界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_selected_2, 238, 242);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_canvasskill, 13, 269);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_normal_0, 28, 285);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_normal_1, 169, 285);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_normal_2, 310, 285);
            switch (this.SkillTab)
            {
                case 1:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_selected_0, 28, 285);
                    renderSkillBack(g, SkillList);
                    break;
                case 2:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_selected_1, 169, 285);
                    renderSkillBack(g, SkillList);
                    break;
                case 3:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_tabtypeTab_selected_2, 310, 285);
                    renderSkillBack(g, SkillList);
                    break;
                default: break;
            }
            g.ResetTransform();
        }

        private void renderSkillBack(Graphics g, List<int> SkillList)
        {
            for (int i = 0; i < 12; i++)
            {
                int index = i + scrollValue * 2;
                if (index < 0 || index >= SkillList.Count)
                {
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_canvasskillBlank, 26 + (i % 2) * 208, 323 + (i / 2) * 61);
                    continue;
                }

                if (SkillList[index] != 0)
                {
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_canvasskill0, 26 + (i % 2) * 208, 323 + (i / 2) * 61);
                    if (skillList != null && index < skillList.Length)
                    {
                        Skill skill = skillList[index];
                        Bitmap skillIcon = skill.Icon.Bitmap;
                        string skillName = SkillNames[index];
                        string skillLevel = SkillLevels[index].ToString();
                        g.DrawImage(skillIcon, 41 + (i % 2) * 208, 335 + (i / 2) * 61);
                        g.DrawString(skillName, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 149 + (i % 2) * 208 - g.MeasureString(skillName, GearGraphics.ItemDetailFont).Width / 2, 335 + (i / 2) * 61);
                        g.DrawString(skillLevel, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 149 + (i % 2) * 208 - g.MeasureString(skillLevel, GearGraphics.ItemDetailFont).Width / 2, 355 + (i / 2) * 61);
                    }
                }
                else
                {
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailSkill_canvasskillBlank, 26 + (i % 2) * 208, 323 + (i / 2) * 61);
                }
            }
        }

        private void renderCash(Graphics g)  //绘制现金界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detail_tabdetailTab_selected_3, 351, 242);
            switch (this.CashPreset)
            {
                case 1:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_canvascash, 13, 269);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_tabcashTab_selected_0, 28, 280);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_tabcashTab_normal_1, 117, 280);
                    break;
                case 2:
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_canvaspreset, 13, 269);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_tabcashTab_normal_0, 28, 280);
                    g.DrawImage(Resource.UICharacterInfo_img_remote_detailCash_tabcashTab_selected_1, 117, 280); break;
                default: break;
            }
            g.ResetTransform();
        }

        private void renderAbility(Graphics g)  //绘制内在能力
        {
            Rectangle rect = this.DetailRect;
            g.TranslateTransform(rect.X, rect.Y);
            int AbilityYOffset = Resource.UICharacterInfo_img_common_main_backgrnd.Height + Resource.UICharacterInfo_img_local_detail_backgrnd.Height - Resource.UICharacterInfo_img_remote_detailStat_ability_backgrnd.Height;
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_ability_backgrnd, 1, AbilityYOffset);

            Bitmap abilityTitle = Resource.ResourceManager.GetObject("UICharacterInfo_img_remote_detailStat_ability_abilityTitle_" + ability_grade + "_0") as Bitmap;
            g.DrawImage(abilityTitle, 11, (AbilityYOffset + 30));
            if (resultJson5 != null)
            {
                int i = 0;
                foreach (var ability in resultJson5["ability_info"])
                {
                    string abilityGrade = ability_rank(ability["ability_grade"].ToString());
                    string abilityValue = ability["ability_value"].ToString();
                    Bitmap metierLine = Resource.ResourceManager.GetObject("UICharacterInfo_img_remote_detailStat_ability_metierLine_activated_0_" + abilityGrade + "_0") as Bitmap;
                    g.DrawImage(metierLine, 12, (AbilityYOffset + 62 + 20 * i));
                    g.DrawString(abilityValue, GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 14f, (AbilityYOffset + 70 + 20 * i));
                    i++;
                }
            }
            else
            {
                g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_ability_metierLine_activated_0_legendary_0, 12, (AbilityYOffset + 62));
                g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_ability_metierLine_activated_0_unique_0, 12, (AbilityYOffset + 82));
                g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_ability_metierLine_activated_0_unique_0, 12, (AbilityYOffset + 102));
            }
            g.ResetTransform();
        }

        private void renderHyperStat(Graphics g) //绘制超级属性
        {
            Rectangle rect = this.HyperStatRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_backgrnd, 1, 0); //超级属性背景
            g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_buttonpresetPage1_checked_0, 136, 421);
            switch (this.presetPage)
            {
                case 1: g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_presetSelected0_0, 135, 420); break;
                case 2: g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_presetSelected1_0, 155, 420); break;
                case 3: g.DrawImage(Resource.UICharacterInfo_img_remote_detailStat_HyperStat_Window_presetSelected2_0, 175, 420); break;
                default: break;
            }
            float ydistance = 22f;
            int count = 0;
            foreach (var statlevel in hyperStatsOnUse)
            {
                g.DrawString(statlevel.ToString(), GearGraphics.LevelBoldFont, GearGraphics.WhiteBrush, 190f - g.MeasureString(statlevel.ToString(), GearGraphics.LevelBoldFont).Width, 42f + ydistance * count);
                count++;
            }
            g.ResetTransform();
        }

        public TooltipHelp GetPairByPoint(Point point)
        {
            Point p = point;
            if (AbilityVisible && DetailRect.Contains(p))
            {
                p = Point.Subtract(point, new Size(DetailRect.X, DetailRect.Y));
                return helpDetailList?.FirstOrDefault(t => t.Rect.Contains(p))?.Help;
            }
            p = Point.Subtract(point, new Size(baseOffset.X, baseOffset.Y));
            return helpList?.FirstOrDefault(t => t.Rect.Contains(p))?.Help;
        }

        public int GetSlotIndexByPoint(Point point)
        {
            Point p = point;
            p.Offset(-11, -41);
            if (p.X < 0 || p.Y < 0)
                return -1;
            int idx = p.Y / 22;
            if (new Rectangle(new Point(0, idx * 22), new Size(71, 16)).Contains(p))
                return idx;
            else
                return -1;
        }

        public int GetHyperStatIndexByPoint(Point point)
        {
            int slotIdx = GetSlotIndexByPoint(point);
            if (slotIdx != -1)
            {
                slotIdx += this.hyperStatScrollValue;
            }
            return slotIdx;
        }

        private int GetSkillIndexByPoint(Point point)
        {
            // 技能区域起始位置 (13, 269) 是背景图位置
            // 实际技能图标起始位置 (39, 334) 是第一个技能图标
            // 每个技能图标大小和间距需要根据实际情况调整

            int startX = 26;  // 技能区域起始X
            int startY = 323; // 技能区域起始Y
            int cellWidth = 208;  // 每个技能单元宽度（2列）
            int cellHeight = 61;  // 每个技能单元高度（6行）

            // 计算列和行
            int col = (point.X - startX) / cellWidth;
            int row = (point.Y - startY) / cellHeight;

            // 检查是否在有效范围内
            if (col < 0 || col >= 2 || row < 0 || row >= 6)
                return -1;

            // 计算索引（考虑滚动值）
            int index = (row * 2 + col) + scrollValue * 2;

            return index;
        }

        public Skill GetSkillByPoint(Point point)
        {
            if (HyperStatVisible && HyperStatRect.Contains(point) && hyperStatSkillList != null)
            {
                int hyperStatIdx = GetHyperStatIndexByPoint(Point.Subtract(point, new Size(HyperStatRect.X, HyperStatRect.Y)));
                if (hyperStatIdx > -1 && hyperStatIdx < this.hyperStatSkillList.Length)
                    return this.hyperStatSkillList[hyperStatIdx];
                else
                    return null;
            }
            if (SkillVisible && skillList != null)
            {
                int skillIdx = GetSkillIndexByPoint(point);
                if (skillIdx > -1 && skillIdx < this.skillList.Length)
                {
                    this.skillList[skillIdx].Level = SkillLevels[skillIdx];
                    return this.skillList[skillIdx];
                }
                else
                    return null;
            }
            return null;
        }

        private int indexConversion(string text)
        {
            if (text.Length != 10)
                throw new ArgumentException("长度必须为10");

            var reverseTable = GetSkillIDReverseTable();

            char[] digits = new char[10];

            for (int i = 0; i < 10; i++)
            {
                char c = text[i];

                if (!reverseTable[i].TryGetValue(c, out int digit))
                    throw new Exception($"第{i + 1}位字符 {c} 无法解析");

                digits[i] = (char)('0' + digit);
            }
            return int.Parse(new string(digits));
        }

        private int ItemIndexConversion(string text)
        {
            if (text.Length != 8)
                throw new ArgumentException("长度必须为8");

            var reverseTable = GetItemIDReverseTable();

            char[] digits = new char[8];

            for (int i = 0; i < 8; i++)
            {
                char c = text[i];

                if (!reverseTable[i].TryGetValue(c, out int digit))
                    throw new Exception($"代码：{text} 第{i + 1}位字符 {c} 无法解析");

                digits[i] = (char)('0' + digit);
            }
            return int.Parse(new string(digits));
        }

        private Dictionary<char, int>[] GetSkillIDReverseTable()
        {
            var table = new Dictionary<char, int>[10];

            for (int i = 0; i < 10; i++)
                table[i] = new Dictionary<char, int>();

            // ===== 第1位 =====
            table[0]['K'] = 0;

            // ===== 第2位 =====
            table[1]['F'] = 0;
            table[1]['E'] = 1;
            table[1]['H'] = 2;
            table[1]['G'] = 3;
            table[1]['B'] = 4;
            table[1]['A'] = 5;

            // ===== 第3位 =====
            table[2]['P'] = 0;
            table[2]['O'] = 1;
            table[2]['N'] = 2;
            table[2]['M'] = 3;
            table[2]['L'] = 4;
            table[2]['K'] = 5;
            table[2]['J'] = 6;
            table[2]['I'] = 7;
            table[2]['H'] = 8;
            table[2]['G'] = 9;

            // ===== 第4位 =====
            table[3]['C'] = 0;
            table[3]['D'] = 1;
            table[3]['A'] = 2;
            table[3]['B'] = 3;
            table[3]['G'] = 4;
            table[3]['H'] = 5;
            table[3]['E'] = 6;
            table[3]['F'] = 7;
            table[3]['K'] = 8;
            table[3]['L'] = 9;

            // ===== 第5位 =====
            table[4]['L'] = 0;
            table[4]['K'] = 1;
            table[4]['J'] = 2;
            table[4]['I'] = 3;
            table[4]['D'] = 4;
            table[4]['O'] = 5;
            table[4]['N'] = 6;
            table[4]['M'] = 7;
            table[4]['P'] = 8;

            // ===== 第6位 =====
            table[5]['H'] = 0;
            table[5]['G'] = 1;
            table[5]['F'] = 2;
            table[5]['E'] = 3;
            table[5]['D'] = 4;
            table[5]['C'] = 5;
            table[5]['B'] = 6;
            table[5]['A'] = 7;
            table[5]['P'] = 8;
            table[5]['O'] = 9;

            // ===== 第7位=====
            table[6]['O'] = 0;
            table[6]['P'] = 1;
            table[6]['M'] = 2;
            table[6]['N'] = 3;
            table[6]['K'] = 4;
            table[6]['E'] = 5; // ⚠️ E/F 二选一
            table[6]['F'] = 6; // ⚠️ E/F 二选一
            table[6]['J'] = 7;
            table[6]['G'] = 8;
            table[6]['H'] = 9;

            // ===== 第8位 =====
            table[7]['B'] = 0;
            table[7]['A'] = 1;
            table[7]['D'] = 2;
            table[7]['C'] = 3;
            table[7]['F'] = 4;
            table[7]['E'] = 5;
            table[7]['H'] = 6;
            table[7]['G'] = 7;
            table[7]['J'] = 8;
            table[7]['I'] = 9;

            // ===== 第9位 =====
            table[8]['M'] = 0;
            table[8]['N'] = 1;
            table[8]['O'] = 2;
            table[8]['P'] = 3;
            table[8]['I'] = 4;
            table[8]['J'] = 5;
            table[8]['K'] = 6;
            table[8]['L'] = 7;
            table[8]['E'] = 8;
            table[8]['F'] = 9;

            // ===== 第10位 =====
            table[9]['A'] = 0;
            table[9]['B'] = 1;
            table[9]['C'] = 2;
            table[9]['D'] = 3;
            table[9]['E'] = 4;
            table[9]['F'] = 5;
            table[9]['G'] = 6;
            table[9]['H'] = 7;
            table[9]['I'] = 8;
            table[9]['J'] = 9;

            return table;
        }

        private Dictionary<char, int>[] GetItemIDReverseTable()
        {
            var table = new Dictionary<char, int>[8];

            for (int i = 0; i < 8; i++)
                table[i] = new Dictionary<char, int>();

            // ===== 第1位 =====
            table[0]['K'] = 0;

            // ===== 第2位 =====
            table[1]['E'] = 1;
            table[1]['H'] = 2;
            table[1]['G'] = 3;
            table[1]['B'] = 4;
            table[1]['A'] = 5;

            // ===== 第3位 =====
            table[2]['P'] = 0;
            table[2]['O'] = 1;
            table[2]['N'] = 2;
            table[2]['M'] = 3;
            table[2]['L'] = 4;
            table[2]['K'] = 5;
            table[2]['J'] = 6;
            table[2]['I'] = 7;
            table[2]['H'] = 8;
            table[2]['G'] = 9;

            // ===== 第4位 =====
            table[3]['C'] = 0;
            table[3]['D'] = 1;
            table[3]['A'] = 2;
            table[3]['B'] = 3;
            table[3]['G'] = 4;
            table[3]['H'] = 5;
            table[3]['E'] = 6;
            table[3]['F'] = 7;
            table[3]['K'] = 8;
            table[3]['L'] = 9;

            // ===== 第5位 =====
            table[4]['L'] = 0;
            table[4]['K'] = 1;
            table[4]['J'] = 2;
            table[4]['I'] = 3;
            table[4]['D'] = 4;
            table[4]['O'] = 5;
            table[4]['N'] = 6;
            table[4]['M'] = 7;
            table[4]['P'] = 8;
            table[4]['C'] = 9;

            // ===== 第6位 =====
            table[5]['H'] = 0;
            table[5]['G'] = 1;
            table[5]['F'] = 2;
            table[5]['E'] = 3;
            table[5]['D'] = 4;
            table[5]['C'] = 5;
            table[5]['B'] = 6;
            table[5]['A'] = 7;
            table[5]['P'] = 8;
            table[5]['O'] = 9;

            // ===== 第7位 =====
            table[6]['O'] = 0;
            table[6]['P'] = 1;
            table[6]['M'] = 2;
            table[6]['N'] = 3;
            table[6]['K'] = 4;
            table[6]['L'] = 5;
            table[6]['I'] = 6;
            table[6]['J'] = 7;
            table[6]['G'] = 8;
            table[6]['H'] = 9;

            // ===== 第8位 =====
            table[7]['B'] = 0;
            table[7]['A'] = 1;
            table[7]['D'] = 2;
            table[7]['C'] = 3;
            table[7]['F'] = 4;
            table[7]['E'] = 5;
            table[7]['H'] = 6;
            table[7]['G'] = 7;
            table[7]['J'] = 8;
            table[7]['I'] = 9;

            return table;
        }

        private int GetSlotIndex(string Category)
        {
            switch (Category)
            {
                case "모자": case "Hat": return 1;
                case "얼굴장식": case "Face Acc.": return 2;
                case "눈장식": case "Eye Acc.": return 3;
                case "귀고리": case "Earring": return 4;
                case "상의": case "Top": return 5;
                case "하의": case "Bottom": return 6;
                case "신발": case "Shoes": return 7;
                case "장갑": case "Glove": return 8;
                case "망토": case "Cape": return 9;
                case "보조무기": case "Secondary Weapons": return 10;
                case "무기": case "Weapon": return 11;
                case "반지1": case "Ring1": return 12;
                case "반지2": case "Ring2": return 13;
                case "반지3": case "Ring3": return 15;
                case "반지4": case "Ring4": return 16;
                case "펜던트": case "Pendant": return 17;
                case "훈장": case "Medal": return 26;
                case "벨트": case "Belt": return 29;
                case "어깨장식": case "Shoulder": return 30;
                case "펜던트2": case "Pendant 2": return 38;
                case "포켓 아이템": case "Pocket Item": return 33;
                case "기계 심장": case "M. Heart": return 35;
                case "뱃지": case "Badge": return 36;
                case "엠블렘": case "Emblem": return 37;
                case "예비 특수 반지": return -1;
                default: return -1;
            }
        }

        private Wz_Vector get_vector(int slotIndex)
        {
            switch (slotIndex)
            {
                case 12: return new Wz_Vector(15, 173);
                case 13: return new Wz_Vector(15, 128);
                case 15: return new Wz_Vector(15, 83);
                case 16: return new Wz_Vector(15, 38);
                case 26: return new Wz_Vector(195, 173);
                case 17: return new Wz_Vector(60, 218);
                case 11: return new Wz_Vector(105, 83);
                case 1: return new Wz_Vector(150, 38);
                case 2: return new Wz_Vector(60, 38);
                case 3: return new Wz_Vector(60, 83);
                case 5: return new Wz_Vector(150, 83);
                case 6: return new Wz_Vector(150, 128);
                case 7: return new Wz_Vector(195, 128);
                case 4: return new Wz_Vector(60, 128);
                case 8: return new Wz_Vector(195, 83);
                case 27: return new Wz_Vector(15, 218);
                case 30: return new Wz_Vector(150, 173);
                case 29: return new Wz_Vector(60, 263);
                case 10: return new Wz_Vector(105, 128);
                case 9: return new Wz_Vector(195, 38);
                case 28: return new Wz_Vector(15, 263);
                case 35: return new Wz_Vector(195, 218);
                case 33: return new Wz_Vector(105, 218);
                case 38: return new Wz_Vector(60, 173);
                case 34: return new Wz_Vector(150, 218);
                case 37: return new Wz_Vector(105, 173);
                case 36: return new Wz_Vector(195, 263);
                default: return new Wz_Vector(0, 0);
            }
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void btntoggleDetailOpen_MouseClick(object sender, MouseEventArgs e)
        {
            this.DetailVisible = true;
            this.StatVisible = true;
            this.EquipVisible = false;
            this.SkillVisible = false;
            this.CashVisible = false;
            this.AbilityVisible = false;
            this.HyperStatVisible = false;
            this.waitForRefresh = true;
        }

        private void btntoggleDetailClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.DetailVisible = false;
            this.StatVisible = false;
            this.EquipVisible = false;
            this.SkillVisible = false;
            this.CashVisible = false;
            this.AbilityVisible = false;
            this.HyperStatVisible = false;
            this.waitForRefresh = true;
        }

        private void btnDetailOpen_MouseClick(object sender, MouseEventArgs e)
        {
            this.AbilityVisible = true;
            this.waitForRefresh = true;
        }

        private void btnDetailClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.AbilityVisible = false;
            this.waitForRefresh = true;
        }

        private void btnHyperStatOpen_MouseClick(object sender, MouseEventArgs e)
        {
            this.HyperStatVisible = true;
            this.waitForRefresh = true;
        }

        private void btnHyperStatClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.HyperStatVisible = false;
            this.waitForRefresh = true;
        }

        private void btndetailTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.StatVisible = true;
            this.EquipVisible = false;
            this.SkillVisible = false;
            this.CashVisible = false;
        }

        private void btndetailTab2_MouseClick(object sender, MouseEventArgs e)
        {
            this.StatVisible = false;
            this.EquipVisible = true;
            this.SkillVisible = false;
            this.CashVisible = false;
            GearList.Clear();
            gearList = null;
        }

        private void btndetailTab3_MouseClick(object sender, MouseEventArgs e)
        {
            this.StatVisible = false;
            this.EquipVisible = false;
            this.SkillVisible = true;
            this.CashVisible = false;
        }

        private void btndetailTab4_MouseClick(object sender, MouseEventArgs e)
        {
            this.StatVisible = false;
            this.EquipVisible = false;
            this.SkillVisible = false;
            this.CashVisible = true;
        }

        private void btnpresetPage1_MouseClick(object sender, MouseEventArgs e)
        {
            this.presetPage = 1;
        }

        private void btnpresetPage2_MouseClick(object sender, MouseEventArgs e)
        {
            this.presetPage = 2;
        }

        private void btnpresetPage3_MouseClick(object sender, MouseEventArgs e)
        {
            this.presetPage = 3;
        }

        private void btnStatFont_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.StatVisible && this.DetailVisible)
            {
                switch (statFont)
                {
                    case 1: statFont = 2; break;
                    case 2: statFont = 3; break;
                    case 3: statFont = 1; break;
                    default: break;
                }
            }
        }

        private void btnArc_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.EquipVisible)
                this.ArcAut = 1;
        }

        private void btnAut_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.EquipVisible)
                this.ArcAut = 2;
        }

        private void btnLinkSkill_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.SkillVisible)
                this.SkillTab = 1;
            this.scrollValue = 0;
            this.vScroll.Maximum = 0;
            SkillList.Clear();
            SkillNames.Clear();
            SkillLevels.Clear();
            skillList = null;
            this.Refresh();
        }

        private void btnVSkill_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.SkillVisible)
                this.SkillTab = 2;
            this.scrollValue = 0;
            this.vScroll.Maximum = 0;
            SkillList.Clear();
            SkillNames.Clear();
            SkillLevels.Clear();
            skillList = null;
            this.Refresh();
        }

        private void btnHexaSkill_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.SkillVisible)
                this.SkillTab = 3;
            this.scrollValue = 0;
            this.vScroll.Maximum = 0;
            SkillList.Clear();
            SkillNames.Clear();
            SkillLevels.Clear();
            skillList = null;
            this.Refresh();
        }

        private void btnCash_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.CashVisible)
                this.CashPreset = 1;
        }

        private void btnCashPreset_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.CashVisible)
                this.CashPreset = 2;
        }

        private void vScroll_ValueChanged(object sneder, EventArgs e)
        {
            this.scrollValue = this.vScroll.Value;
            this.waitForRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            MouseEventArgs detailChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - DetailRect.X, e.Y - DetailRect.Y, e.Delta);

            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.OnMouseMove(detailChildArgs);
            }

            MouseEventArgs hyperStatChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperStatRect.X, e.Y - HyperStatRect.Y, e.Delta);

            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.OnMouseMove(hyperStatChildArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseMove(e);

            object obj = GetPairByPoint(e.Location);
            if (obj == null)
                obj = GetSkillByPoint(e.Location);
            if (obj != null)
                this.OnObjectMouseMove(new ObjectMouseEventArgs(e, obj));
            else
                this.OnObjectMouseLeave(EventArgs.Empty);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            MouseEventArgs detailChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - DetailRect.X, e.Y - DetailRect.Y, e.Delta);

            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.OnMouseDown(detailChildArgs);
            }

            MouseEventArgs hyperStatChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperStatRect.X, e.Y - HyperStatRect.Y, e.Delta);

            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.OnMouseDown(hyperStatChildArgs);
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

            MouseEventArgs detailChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - DetailRect.X, e.Y - DetailRect.Y, e.Delta);

            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.OnMouseUp(detailChildArgs);
            }

            MouseEventArgs hyperStatChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperStatRect.X, e.Y - HyperStatRect.Y, e.Delta);

            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.OnMouseUp(hyperStatChildArgs);
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

            MouseEventArgs detailChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - DetailRect.X, e.Y - DetailRect.Y, e.Delta);

            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.OnMouseClick(detailChildArgs);
            }

            MouseEventArgs hyperStatChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperStatRect.X, e.Y - HyperStatRect.Y, e.Delta);

            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.OnMouseClick(hyperStatChildArgs);
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

            MouseEventArgs detailChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - DetailRect.X, e.Y - DetailRect.Y, e.Delta);

            foreach (AControl ctrl in this.aDetailControls)
            {
                ctrl.OnMouseWheel(detailChildArgs);
            }

            MouseEventArgs hyperStatChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperStatRect.X, e.Y - HyperStatRect.Y, e.Delta);

            foreach (AControl ctrl in this.aHyperStatControls)
            {
                ctrl.OnMouseWheel(hyperStatChildArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseWheel(e);
        }

        protected virtual void OnObjectMouseMove(ObjectMouseEventArgs e)
        {
            if (this.ObjectMouseMove != null)
                this.ObjectMouseMove(this, e);
        }

        protected virtual void OnObjectMouseLeave(EventArgs e)
        {
            if (this.ObjectMouseLeave != null)
                this.ObjectMouseLeave(this, e);
        }

        public class TooltipHelpRect
        {
            public TooltipHelpRect(Rectangle rect, TooltipHelp pair)
            {
                this.rect = rect;
                this.help = pair;
            }

            private Rectangle rect;
            private TooltipHelp help;

            public Rectangle Rect
            {
                get { return rect; }
            }

            public TooltipHelp Help
            {
                get { return help; }
            }
        }

        void AfrmStat_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    dlg.FileName = "Stat UI Preview";

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