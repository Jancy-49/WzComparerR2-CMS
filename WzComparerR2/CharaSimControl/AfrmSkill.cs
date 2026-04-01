using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using CharaSimResource;
using WzComparerR2.WzLib;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Comparer;
using WzComparerR2.Controls;
using WzComparerR2.CharaSimControl;
using WzComparerR2.PluginBase;
using System.Security.Cryptography;
using System.Security.Policy;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmSkill : AlphaForm
    {
        public AfrmSkill()
        {
            initCtrl();
        }

        private Point baseOffset;
        private Point newLocation;
        private bool waitForRefresh;
        private ACtrlVScroll vScroll;
        private ACtrlVScroll vScroll2;//超级技能
        private Character character;

        private ContextMenuStrip menu;
        private ACtrlButton btnClose;
        private ACtrlButton btnTab0;
        private ACtrlButton btnTab1;
        private ACtrlButton btnTab2;
        private ACtrlButton btnTab3;
        private ACtrlButton btnTab4;
        private ACtrlButton btnTab5;
        private ACtrlButton btnTab6;
        private ACtrlButton btnTab7;
        private ACtrlButton btnTab8;
        private ACtrlButton btnVMatrix;//普通模式
        private ACtrlButton btnVMatrix2;//龙神模式
        private ACtrlButton btnVMatrix3;//神之子模式
        private ACtrlButton btnHexaMatrix;//普通模式
        private ACtrlButton btnHexaMatrix2;//龙神模式
        private ACtrlButton btnHexaMatrix3;//神之子模式
        private ACtrlButton btnHyper;//普通模式
        private ACtrlButton btnHyper2;//龙神模式
        private ACtrlButton btnGuildSkill;
        private ACtrlButton btnRide;//普通模式
        private ACtrlButton btnRide2;//龙神模式
        private ACtrlButton btnRide3;//神之子模式
        private ACtrlButton btnSkillSkin;
        private ACtrlButton btnSequence;//普通模式
        private ACtrlButton btnSequence2;//龙神模式
        private ACtrlButton btnSequence3;//神之子模式
        private ACtrlButton btnMacro;//普通模式
        private ACtrlButton btnMacro2;//龙神模式
        private ACtrlButton btnMacro3;//神之子模式
        private ACtrlButton btnCooltimeAlarm;
        private ACtrlButton btnDlg;//龙神对话
        private ACtrlButton btnEx;//神之子超越者技能
        private ACtrlButton btnPassive;
        private ACtrlButton btnActive;
        private ACtrlButton btnReset;

        public Skill Skill { get; set; }
        public int selectJob;
        private int selectedTab = 0;
        private int hyperselectedTab = 1;
        private int ScrollMaxValue = 0;
        private int ScrollMaxValue2 = 0;
        private int scrollValue = 0;
        private int scrollValue2 = 0;
        private bool normalmode = true;
        private bool dualblademode = false;
        private bool evanmode = false;
        private bool zeromode = false;
        private bool yetipbmode = false;
        private bool collabmode = false;
        private bool HyperStatVisible = false;
        private bool ZeroexVisible = false;
        BitmapOrigin icon = new BitmapOrigin();
        private List<string> tab_list = new List<string>();
        private List<Tuple<BitmapOrigin, string , string, string>> skillList = new List<Tuple<BitmapOrigin, string, string, string>>();
        private List<Tuple<BitmapOrigin, string , string, string>> skillList2 = new List<Tuple<BitmapOrigin, string, string, string>>();//超级技能
        private List<Tuple<BitmapOrigin, string , string, string>> skillList3 = new List<Tuple<BitmapOrigin, string, string, string>>();//左列神之子技能
        private List<Tuple<BitmapOrigin, string , string, string>> skillList4 = new List<Tuple<BitmapOrigin, string, string, string>>();//右列神之子技能
        private List<Tuple<BitmapOrigin, string, string, string>> scrollList = new List<Tuple<BitmapOrigin, string, string, string>>();
        private List<Tuple<BitmapOrigin, string, string, string>> scrollList2 = new List<Tuple<BitmapOrigin, string, string, string>>();//超级技能
        private string jobtab;
        public event ObjectMouseEventHandler ObjectMouseMove;
        public event EventHandler ObjectMouseLeave;

        private Rectangle HyperSkillRect
        {
            get
            {
                return new Rectangle(new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width - 1, 
                    baseOffset.Y),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private Rectangle ZeroexRect
        {
            get
            {
                return new Rectangle(new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width - 1, 
                    baseOffset.Y),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private void initCtrl()
        {
            this.menu = new ContextMenuStrip();
            this.menu.Items.Add(new ToolStripMenuItem("复制", null, tsmiCopy_Click));
            this.menu.Items.Add(new ToolStripMenuItem("保存PNG", null, tsmiSave_Click));
            this.MouseClick += AfrmJob_MouseClick;

            this.vScroll = new ACtrlVScroll();  //鼠标滑轮区域

            this.vScroll.PicBase.Normal = new BitmapOrigin(Resource.VScr9_enabled_base);
            this.vScroll.PicBase.Disabled = new BitmapOrigin(Resource.VScr9_disabled_base);

            this.vScroll.BtnPrev.Normal = new BitmapOrigin(Resource.VScr9_enabled_prev0);
            this.vScroll.BtnPrev.Pressed = new BitmapOrigin(Resource.VScr9_enabled_prev1);
            this.vScroll.BtnPrev.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_prev2);
            this.vScroll.BtnPrev.Disabled = new BitmapOrigin(Resource.VScr9_enabled_prev0);
            this.vScroll.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll.BtnPrev.Location = new Point(0, 0);

            this.vScroll.BtnNext.Normal = new BitmapOrigin(Resource.VScr9_enabled_next0);
            this.vScroll.BtnNext.Pressed = new BitmapOrigin(Resource.VScr9_enabled_next1);
            this.vScroll.BtnNext.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_next2);
            this.vScroll.BtnNext.Disabled = new BitmapOrigin(Resource.VScr9_enabled_next0);
            this.vScroll.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;

            this.vScroll.BtnThumb.Normal = new BitmapOrigin(Resource.VScr9_enabled_thumb0);
            this.vScroll.BtnThumb.Pressed = new BitmapOrigin(Resource.VScr9_enabled_thumb1);
            this.vScroll.BtnThumb.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_thumb2);
            this.vScroll.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll.Visible = true;
            this.vScroll.ValueChanged += new EventHandler(vScroll_ValueChanged);
            this.vScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.vScroll2 = new ACtrlVScroll();  //超级技能鼠标滑轮区域

            this.vScroll2.PicBase.Normal = new BitmapOrigin(Resource.VScr9_enabled_base);
            this.vScroll2.PicBase.Disabled = new BitmapOrigin(Resource.VScr9_disabled_base);

            this.vScroll2.BtnPrev.Normal = new BitmapOrigin(Resource.VScr9_enabled_prev0);
            this.vScroll2.BtnPrev.Pressed = new BitmapOrigin(Resource.VScr9_enabled_prev1);
            this.vScroll2.BtnPrev.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_prev2);
            this.vScroll2.BtnPrev.Disabled = new BitmapOrigin(Resource.VScr9_enabled_prev0);
            this.vScroll2.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll2.BtnPrev.Location = new Point(0, 0);

            this.vScroll2.BtnNext.Normal = new BitmapOrigin(Resource.VScr9_enabled_next0);
            this.vScroll2.BtnNext.Pressed = new BitmapOrigin(Resource.VScr9_enabled_next1);
            this.vScroll2.BtnNext.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_next2);
            this.vScroll2.BtnNext.Disabled = new BitmapOrigin(Resource.VScr9_enabled_next0);
            this.vScroll2.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;

            this.vScroll2.BtnThumb.Normal = new BitmapOrigin(Resource.VScr9_enabled_thumb0);
            this.vScroll2.BtnThumb.Pressed = new BitmapOrigin(Resource.VScr9_enabled_thumb1);
            this.vScroll2.BtnThumb.MouseOver = new BitmapOrigin(Resource.VScr9_enabled_thumb2);
            this.vScroll2.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll2.Visible = true;
            this.vScroll2.ValueChanged += new EventHandler(vScroll2_ValueChanged);
            this.vScroll2.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton();
            this.btnClose.Normal = new BitmapOrigin(Resource.BtClose3_normal_0);
            this.btnClose.Pressed = new BitmapOrigin(Resource.BtClose3_pressed_0);
            this.btnClose.MouseOver = new BitmapOrigin(Resource.BtClose3_mouseOver_0);
            this.btnClose.Disabled = new BitmapOrigin(Resource.BtClose3_disabled_0);
            this.btnClose.Size = new Size(13, 13);
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);

            this.btnVMatrix = new ACtrlButton();
            this.btnVMatrix.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtVMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtVMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtVMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtVMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix.Location = new Point(10, 52);
            this.btnVMatrix.Size = new Size(297, 35);
            this.btnVMatrix.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnVMatrix.MouseClick += new MouseEventHandler(btnVMatrix_MouseClick);

            this.btnVMatrix2 = new ACtrlButton();
            this.btnVMatrix2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtVMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtVMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtVMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtVMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix2.Location = new Point(12, 163);
            this.btnVMatrix2.Size = new Size(104, 25);
            this.btnVMatrix2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnVMatrix2.MouseClick += new MouseEventHandler(btnVMatrix2_MouseClick);

            this.btnVMatrix3 = new ACtrlButton();
            this.btnVMatrix3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtVMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtVMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtVMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtVMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnVMatrix3.Location = new Point(12, 54);
            this.btnVMatrix3.Size = new Size(320, 35);
            this.btnVMatrix3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnVMatrix3.MouseClick += new MouseEventHandler(btnVMatrix2_MouseClick);

            this.btnHexaMatrix = new ACtrlButton();
            this.btnHexaMatrix.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHexaMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHexaMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHexaMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHexaMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix.Location = new Point(10, 52);
            this.btnHexaMatrix.Size = new Size(297, 35);
            this.btnHexaMatrix.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnHexaMatrix.MouseClick += new MouseEventHandler(btnHexaMatrix_MouseClick);

            this.btnHexaMatrix2 = new ACtrlButton();
            this.btnHexaMatrix2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHexaMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHexaMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHexaMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHexaMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix2.Location = new Point(12, 163);
            this.btnHexaMatrix2.Size = new Size(104, 25);
            this.btnHexaMatrix2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnHexaMatrix2.MouseClick += new MouseEventHandler(btnHexaMatrix2_MouseClick);

            this.btnHexaMatrix3 = new ACtrlButton();
            this.btnHexaMatrix3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtHexaMatrix/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtHexaMatrix/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtHexaMatrix/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtHexaMatrix/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHexaMatrix3.Location = new Point(12, 54);
            this.btnHexaMatrix3.Size = new Size(320, 35);
            this.btnHexaMatrix3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnHexaMatrix3.MouseClick += new MouseEventHandler(btnHexaMatrix3_MouseClick);

            this.btnHyper = new ACtrlButton();
            this.btnHyper.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHyper/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHyper/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHyper/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtHyper/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper.Location = new Point(8, 335);
            this.btnHyper.Size = new Size(50, 16);
            this.btnHyper.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHyper.MouseClick += new MouseEventHandler(btnHyper_MouseClick);

            this.btnHyper2 = new ACtrlButton();
            this.btnHyper2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHyper/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHyper/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHyper/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtHyper/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHyper2.Location = new Point(10, 387);
            this.btnHyper2.Size = new Size(44, 16);
            this.btnHyper2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHyper2.MouseClick += new MouseEventHandler(btnHyper_MouseClick);

            this.btnGuildSkill = new ACtrlButton();
            this.btnGuildSkill.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtGuildSkill/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnGuildSkill.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtGuildSkill/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnGuildSkill.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtGuildSkill/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnGuildSkill.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtGuildSkill/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnGuildSkill.Size = new Size(50, 16);
            this.btnGuildSkill.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnRide = new ACtrlButton();
            this.btnRide.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnRide.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnRide.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnRide.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnRide.Location = new Point(110, 335);
            this.btnRide.Size = new Size(34, 16);
            this.btnRide.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnRide.MouseClick += new MouseEventHandler(btnRide_MouseClick);

            this.btnRide2 = new ACtrlButton();
            this.btnRide2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtRide/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnRide2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtRide/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnRide2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtRide/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnRide2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtRide/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnRide2.Location = new Point(108, 387);
            this.btnRide2.Size = new Size(44, 16);
            this.btnRide2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnRide2.MouseClick += new MouseEventHandler(btnRide2_MouseClick);

            this.btnRide3 = new ACtrlButton();
            this.btnRide3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnRide3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnRide3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnRide3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtRide/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnRide3.Location = new Point(140, 387);
            this.btnRide3.Size = new Size(50, 16);
            this.btnRide3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnRide3.MouseClick += new MouseEventHandler(btnRide3_MouseClick);

            this.btnSkillSkin = new ACtrlButton();
            this.btnSkillSkin.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSkillSkin/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSkillSkin.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSkillSkin/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSkillSkin.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSkillSkin/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSkillSkin.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSkillSkin/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSkillSkin.Location = new Point(145, 335);
            this.btnSkillSkin.Size = new Size(50, 16);
            this.btnSkillSkin.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnSkillSkin.MouseClick += new MouseEventHandler(btnSkillSkin_MouseClick);

            this.btnSequence = new ACtrlButton();
            this.btnSequence.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSequence/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSequence/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSequence/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtSequence/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence.Location = new Point(196, 335);
            this.btnSequence.Size = new Size(40, 16);
            this.btnSequence.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnSequence.MouseClick += new MouseEventHandler(btnSequence_MouseClick);

            this.btnSequence2 = new ACtrlButton();
            this.btnSequence2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtSequence/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtSequence/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtSequence/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtSequence/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence2.Location = new Point(154, 387);
            this.btnSequence2.Size = new Size(44, 16);
            this.btnSequence2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnSequence2.MouseClick += new MouseEventHandler(btnSequence2_MouseClick);

            this.btnSequence3 = new ACtrlButton();
            this.btnSequence3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtSequence/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtSequence/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtSequence/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtSequence/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSequence3.Location = new Point(192, 387);
            this.btnSequence3.Size = new Size(51, 16);
            this.btnSequence3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnSequence3.MouseClick += new MouseEventHandler(btnSequence3_MouseClick);

            this.btnMacro = new ACtrlButton();
            this.btnMacro.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtMacro/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtMacro/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtMacro/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/Skill/main/BtMacro/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro.Location = new Point(237, 335);
            this.btnMacro.Size = new Size(71, 16);
            this.btnMacro.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnMacro.MouseClick += new MouseEventHandler(btnMacro_MouseClick);

            this.btnMacro2 = new ACtrlButton();
            this.btnMacro2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtMacro/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtMacro/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtMacro/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtMacro/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro2.Location = new Point(200, 387);
            this.btnMacro2.Size = new Size(76, 16);
            this.btnMacro2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnMacro2.MouseClick += new MouseEventHandler(btnMacro2_MouseClick);

            this.btnMacro3 = new ACtrlButton();
            this.btnMacro3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtMacro/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtMacro/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtMacro/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtMacro/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnMacro3.Location = new Point(244, 387);
            this.btnMacro3.Size = new Size(88, 16);
            this.btnMacro3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnMacro3.MouseClick += new MouseEventHandler(btnMacro_MouseClick);

            this.btnDlg = new ACtrlButton();
            this.btnDlg.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtDlg/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnDlg.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtDlg/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnDlg.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtDlg/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnDlg.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillEx/main/BtDlg/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnDlg.Location = new Point(12, 28);
            this.btnDlg.Size = new Size(48, 16);
            this.btnDlg.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnEx = new ACtrlButton();
            this.btnEx.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtEx/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnEx.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtEx/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnEx.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtEx/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnEx.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZero/main/BtEx/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnEx.Location = new Point(12, 387);
            this.btnEx.Size = new Size(72, 16);
            this.btnEx.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEx.MouseClick += new MouseEventHandler(btnEx_MouseClick);

            this.btnReset = new ACtrlButton();
            this.btnReset.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/BtReset/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnReset.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/BtReset/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnReset.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/BtReset/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnReset.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/BtReset/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnReset.Size = new Size(50, 16);
            this.btnReset.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnTab0 = new ACtrlButton();
            this.btnTab0.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab0.MouseClick += new MouseEventHandler(btnTab0_MouseClick);

            this.btnTab1 = new ACtrlButton();
            this.btnTab1.Size = new Size(25, 18);
            this.btnTab1.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab1.MouseClick += new MouseEventHandler(btnTab1_MouseClick);

            this.btnTab2 = new ACtrlButton();
            this.btnTab2.Size = new Size(25, 18);
            this.btnTab2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab2.MouseClick += new MouseEventHandler(btnTab2_MouseClick);

            this.btnTab3 = new ACtrlButton();
            this.btnTab3.Size = new Size(25, 18);
            this.btnTab3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab3.MouseClick += new MouseEventHandler(btnTab3_MouseClick);

            this.btnTab4 = new ACtrlButton();
            this.btnTab4.Size = new Size(25, 18);
            this.btnTab4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab4.MouseClick += new MouseEventHandler(btnTab4_MouseClick);

            this.btnTab5 = new ACtrlButton();
            this.btnTab5.Size = new Size(25, 18);
            this.btnTab5.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab5.MouseClick += new MouseEventHandler(btnTab5_MouseClick);

            this.btnTab6 = new ACtrlButton();
            this.btnTab6.Size = new Size(25, 18);
            this.btnTab6.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab6.MouseClick += new MouseEventHandler(btnTab6_MouseClick);

            this.btnTab7 = new ACtrlButton();
            this.btnTab7.Size = new Size(25, 18);
            this.btnTab7.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab7.MouseClick += new MouseEventHandler(btnTab7_MouseClick);

            this.btnTab8 = new ACtrlButton();
            this.btnTab8.Size = new Size(25, 18);
            this.btnTab8.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTab8.MouseClick += new MouseEventHandler(btnTab8_MouseClick);

            this.btnPassive = new ACtrlButton();
            this.btnPassive.Location = new Point(10, 27);
            this.btnPassive.Size = new Size(77, 23);
            this.btnPassive.MouseClick += new MouseEventHandler(btnPassive_MouseClick);

            this.btnActive = new ACtrlButton();
            this.btnActive.Location = new Point(88, 29);
            this.btnActive.Size = new Size(77, 23);
            this.btnActive.MouseClick += new MouseEventHandler(btnActive_MouseClick);
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/UIWindow2.img/Skill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width, 24));
            this.Location = newLocation;
            base.Refresh();
        }

        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            control_event();
            //计算图像大小
            Point baseOffsetnew = calcRenderBaseOffset();
            Size size = new Size(0, 0);
            size.Width += baseOffsetnew.X;
            if (normalmode || dualblademode || yetipbmode || collabmode)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/UIWindow2.img/Skill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (evanmode)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/UIWindow2.img/SkillEx/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (zeromode)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/UIWindow2.img/SkillZero/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            if (HyperStatVisible)
                size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
            else if (ZeroexVisible)
                size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X,
                this.Location.Y + this.baseOffset.Y);

            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X - baseOffsetnew.X,
                this.Location.Y + this.baseOffset.Y - baseOffsetnew.Y);
            this.baseOffset = baseOffsetnew;

            //绘制图像
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bitmap);
            if (normalmode || dualblademode || yetipbmode || collabmode)
                renderBase(g);
            else if (evanmode)
                render_evan(g);
            else if (zeromode)
                render_zero(g);
            if (HyperStatVisible)
                render_hyperskill(g);
            else if (ZeroexVisible)
                render_zeroex(g);

            g.Dispose();
            this.Bitmap = bitmap;
        }

        private Point calcRenderBaseOffset()
        {
            if (HyperStatVisible)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width, 0);
            else if (ZeroexVisible)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width, 0);
            else
                return new Point(0, 0);
        }

        private void control_event()
        {
            get_tablist(selectJob);
            if (normalmode || dualblademode || yetipbmode || collabmode)
            {
                this.btnClose.Location = new Point(298, 5);
                this.btnGuildSkill.Location = new Point(59, 335);
                this.btnTab0.Location = new Point(10, 29); this.btnTab0.Size = new Size(25, 18);
                this.btnTab1.Location = new Point(36, 29); this.btnTab1.Size = new Size(25, 18);
                this.btnTab2.Location = new Point(62, 29); this.btnTab2.Size = new Size(25, 18);
                this.btnTab3.Location = new Point(88, 29); this.btnTab3.Size = new Size(25, 18);
                this.btnTab4.Location = new Point(114, 29); this.btnTab4.Size = new Size(25, 18);
                this.btnTab5.Location = new Point(140, 29); this.btnTab5.Size = new Size(25, 18);
                this.btnTab6.Location = new Point(166, 29); this.btnTab6.Size = new Size(25, 18);
                this.btnTab7.Location = new Point(192, 29); this.btnTab7.Size = new Size(25, 18);
                this.btnTab8.Location = new Point(218, 29); this.btnTab8.Size = new Size(25, 18);
                if (normalmode)
                {
                    switch (selectedTab)
                    {
                        case 5: btnVMatrix.Visible = true; btnHexaMatrix.Visible = false; break;
                        case 6: btnVMatrix.Visible = false; btnHexaMatrix.Visible = true; break;
                        default: btnVMatrix.Visible = false; btnHexaMatrix.Visible = false; break;
                    }
                }
                else if (dualblademode)
                {
                    switch (selectedTab)
                    {
                        case 7: btnVMatrix.Visible = true; btnHexaMatrix.Visible = false; break;
                        case 8: btnVMatrix.Visible = false; btnHexaMatrix.Visible = true; break;
                        default: btnVMatrix.Visible = false; btnHexaMatrix.Visible = false; break;
                    }
                }
                this.vScroll.BtnNext.Location = new Point(0, 223);
                this.vScroll.Location = new Point(295, 93);
                this.vScroll.Size = new Size(11, 235);
                this.vScroll.ScrollableLocation = new Point(0, 93);
                this.vScroll.ScrollableSize = new Size(318, 235);
                this.vScroll2.BtnNext.Location = new Point(0, 223);
                this.vScroll2.Location = new Point(153, 93);
                this.vScroll2.Size = new Size(11, 235);
                this.vScroll2.ScrollableLocation = new Point(5, 93);
                this.vScroll2.ScrollableSize = new Size(175, 235);
                this.btnReset.Location = new Point(10, 335);
                this.btnHyper.Visible = true;
                this.btnHyper2.Visible = false;
                this.btnEx.Visible = false;
            }
            else if (evanmode)
            {
                this.btnClose.Location = new Point(266, 5);
                this.btnGuildSkill.Location = new Point(56, 387);
                this.btnTab0.Location = new Point(11, 196); this.btnTab0.Size = new Size(25, 18);
                this.btnTab1.Location = new Point(37, 196); this.btnTab1.Size = new Size(25, 18);
                this.btnTab2.Location = new Point(63, 196); this.btnTab2.Size = new Size(25, 18);
                this.btnTab3.Location = new Point(89, 196); this.btnTab3.Size = new Size(25, 18);
                this.btnTab4.Location = new Point(115, 196); this.btnTab4.Size = new Size(25, 18);
                this.btnTab5.Location = new Point(141, 196); this.btnTab5.Size = new Size(25, 18);
                this.btnTab6.Location = new Point(167, 196); this.btnTab6.Size = new Size(25, 18);
                switch (selectedTab)
                {
                    case 5: btnVMatrix2.Visible = true; btnHexaMatrix2.Visible = false; break;
                    case 6: btnVMatrix2.Visible = false; btnHexaMatrix2.Visible = true; break;
                    default: btnVMatrix2.Visible = false; btnHexaMatrix2.Visible = false; break;
                }
                this.vScroll.BtnNext.Location = new Point(0, 151);
                this.vScroll.Location = new Point(265, 218);
                this.vScroll.Size = new Size(11, 162);
                this.vScroll.ScrollableLocation = new Point(6, 218);
                this.vScroll.ScrollableSize = new Size(278, 162);
                this.vScroll2.BtnNext.Location = new Point(0, 276);
                this.vScroll2.Location = new Point(153, 93);
                this.vScroll2.Size = new Size(11, 287);
                this.vScroll2.ScrollableLocation = new Point(5, 93);
                this.vScroll2.ScrollableSize = new Size(163, 287);
                this.btnReset.Location = new Point(10, 387);
                this.btnHyper.Visible = false;
                this.btnHyper2.Visible = true;
                this.btnEx.Visible = false;
            }
            else if (zeromode)
            {
                this.btnClose.Location = new Point(325, 5);
                this.btnGuildSkill.Location = new Point(88, 387);
                this.btnTab0.Location = new Point(12, 29); this.btnTab0.Size = new Size(73, 20);
                this.btnTab1.Location = new Point(85, 29); this.btnTab1.Size = new Size(73, 20);
                this.btnTab2.Location = new Point(158, 29); this.btnTab2.Size = new Size(72, 18);
                this.btnTab3.Location = new Point(231, 29); this.btnTab3.Size = new Size(72, 18);
                switch (selectedTab)
                {
                    case 2: btnVMatrix3.Visible = true; btnHexaMatrix3.Visible = false; break;
                    case 3: btnVMatrix3.Visible = false; btnHexaMatrix3.Visible = true; break;
                    default: btnVMatrix3.Visible = false; btnHexaMatrix3.Visible = false; break;
                }
                this.vScroll.BtnNext.Location = new Point(0, 227);
                this.vScroll.Location = new Point(322, 114);
                this.vScroll.Size = new Size(11, 238);
                this.vScroll.ScrollableLocation = new Point(8, 114);
                this.vScroll.ScrollableSize = new Size(328, 238);
                this.vScroll2.BtnNext.Location = new Point(0, 148);
                this.vScroll2.Location = new Point(153, 93);
                this.vScroll2.Size = new Size(11, 160);
                this.vScroll2.ScrollableLocation = new Point(6, 93);
                this.vScroll2.ScrollableSize = new Size(162, 160);
                this.btnHyper.Visible = false;
                this.btnHyper2.Visible = false;
                this.btnEx.Visible = true;
            }
            this.vScroll.BtnThumb.Visible = (this.ScrollMaxValue > 0);
            this.vScroll2.BtnThumb.Visible = (this.ScrollMaxValue2 > 0);
        }

        private void renderBase(Graphics g) //绘制普通模式技能界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/UIWindow2.img/Skill/main/backgrnd", 0, 0);
            render_bitmap(g, "UI/UIWindow2.img/Skill/main/backgrnd2", 5, 22);
            render_bitmap(g, "UI/UIWindow2.img/Skill/main/backgrnd3", 7, 47);
            if ((normalmode || yetipbmode || collabmode) && selectedTab < 5)//右上角技能点数
            {
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/Skill/main/skillPoint", 212, 29); g.DrawString("0", GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 296, 31);
            }
            if (dualblademode && selectedTab < 7)
            {
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/Skill/main/bg_dual/skillPoint", 245, 29); g.DrawString("0", GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 296, 31);
            }
            string jobtab = tab_list[selectedTab];
            for (int i = 0; i < (dualblademode ? 9 : (yetipbmode || collabmode) ? 2 : 7); i++) //绘制技能栏标签
            {
                if (normalmode || yetipbmode || collabmode)
                {
                    var tabSelObj = Resource.ResourceManager.GetObject(i == selectedTab ? "Skill_main_Tab_selected_" + i : "Skill_main_Tab_enabled_" + i);
                    if (tabSelObj is Bitmap selBitmap) g.DrawImage(selBitmap, 10 + 26 * i, i == selectedTab ? 27 : 29);
                }
                else if (dualblademode)
                {
                    var tabSelObj = Resource.ResourceManager.GetObject(i == selectedTab ? "Skill_main_Tab_DualTab_selected_" + i : "Skill_main_Tab_DualTab_enabled_" + i);
                    if (tabSelObj is Bitmap selBitmap) g.DrawImage(selBitmap, 10 + 26 * i, i == selectedTab ? 27 : 29);
                }
            }
            if (selectedTab < 5 || selectedTab == 6)//技能册名称
            {
                string bookName = PluginManager.FindWz($@"String/Skill.img/{jobtab}/bookName").GetValueEx<string>(null);
                g.DrawString(bookName, GearGraphics.ItemDetailFont2, GearGraphics.WhiteBrush, (318f - g.MeasureString(bookName, GearGraphics.ItemDetailFont2).Width) / 2, 65f);
            }
            if ((selectedTab < 5 && normalmode) || (selectedTab < 7 && dualblademode)) render_bitmap(g, PluginManager.FindWz($"Skill/{jobtab}.img/info/icon/_outlink").GetValue<string>(null), 15, 55); //绘制技能册
            get_skilllist(jobtab);
            for (int i = 0; i < 12; i++) //绘制技能栏
            {
                if (scrollList[i].Item2 != string.Empty)
                {
                    render_bitmap(g, "UI/UIWindow2.img/Skill/main/skill1", 10 + 143 * (i % 2), 93 + 40 * (i / 2));
                    if (i < 10) render_bitmap(g, "UI/UIWindow2.img/Skill/main/line", 10 + 143 * (i % 2), 130 + 40 * (i / 2));
                    Bitmap skillIcon = scrollList[i].Item1.Bitmap;
                    string skillname = scrollList[i].Item2;
                    string maxLevel = scrollList[i].Item3;
                    if (skillIcon != null)
                        g.DrawImage(skillIcon, 12 + 143 * (i % 2), 94 + 40 * (i / 2), 32, 32);
                    g.DrawString(skillname, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48 + 143 * (i % 2), 96 + 40 * (i / 2));
                    g.DrawString(maxLevel, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48 + 143 * (i % 2), 114 + 40 * (i / 2));
                }
                else
                {
                    render_bitmap(g, "UI/UIWindow2.img/Skill/main/skillBlank", 10 + 143 * (i % 2), 93 + 40 * (i / 2));
                }
            }

            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_evan(Graphics g) //绘制龙神模式技能界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/backgrnd", 0, 0);
            render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/backgrnd2", 6, 19);
            render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/Dragon/4/0", 52, 44);
            if (selectedTab < 5) render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/skillPoint", 12, 171);
            for (int i = 0; i < 7; i++) //绘制技能栏标签
            {
                var tabSelObj = Resource.ResourceManager.GetObject(i == selectedTab ? "SkillEx_main_Tab_selected_" + i : "SkillEx_main_Tab_enabled_" + i);
                if (tabSelObj is Bitmap selBitmap) g.DrawImage(selBitmap, 11 + 26 * i, i == selectedTab ? 194 : 196);
            }
            get_skilllist(tab_list[selectedTab]);
            for (int i = 0; i < 8; i++) //绘制技能栏
            {
                if (scrollList[i].Item2 != string.Empty)
                {
                    render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/skill1", 10 + 128 * (i % 2), 219 + 42 * (i / 2));
                    if (i < 6) render_bitmap(g, "UI/UIWindow2.img/SkillEx/main/line", 10 + 128 * (i % 2), 257 + 42 * (i / 2));
                    Bitmap skillIcon = scrollList[i].Item1.Bitmap;
                    string skillname = scrollList[i].Item2;
                    string maxLevel = scrollList[i].Item3;
                    if (skillIcon != null)
                        g.DrawImage(skillIcon, 12 + 128 * (i % 2), 221 + 42 * (i / 2), 32, 32);
                    g.DrawString(skillname, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48 + 128 * (i % 2), 222 + 42 * (i / 2));
                    g.DrawString(maxLevel, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48 + 128 * (i % 2), 240 + 42 * (i / 2));
                }
            }
            foreach (AControl evanCtrl in this.evanControls)
            {
                evanCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_zero(Graphics g)//绘制神之子模式技能界面
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd", 0, 0);
            render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd2", 8, 23);
            render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/tabLine", 9, 47);
            switch (selectedTab)
            {
                case 0: render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd5", 15, 91); render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/skillPoint", 63, 355); break;
                case 1: render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd4", 15, 91); render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/skillPoint", 63, 355); break;
                case 2: 
                case 3: render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd6", 15, 91); break;
                default: break;
            }
            for (int i = 0; i < 4; i++) //绘制技能栏标签
            {
                render_bitmap(g, String.Format(@"UI/_Canvas/UIWindow2.img/SkillZero/main/Tab/{0}/{1}", i == selectedTab ? "selected" : "enabled", i), 12 + 73 * i, i == selectedTab ? (i > 1 ? 29 : 27) : (i > 1 ? 31 : 29));
            }
            if (selectedTab < 2)
            {
                render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/backgrnd3", 12, 52);
                render_bitmap(g, "Skill/_Canvas/000.img/info/icon", 17, 57);
                string bookName = selectedTab == 0 ? "通用" : "神之子";
                g.DrawString(bookName, GearGraphics.ItemDetailFont2, GearGraphics.WhiteBrush, (345f - g.MeasureString(bookName, GearGraphics.ItemDetailFont2).Width) / 2, 67f);
                g.DrawString("0", GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 144, 357);
                g.DrawString("0", GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 300, 357);
                get_zeroskill();
            }                
            else if (selectedTab > 1)
            {
                get_skilllist(tab_list[selectedTab]);
            }
            for (int i = 0; i < 12; i++) //绘制技能栏
            {
                if (scrollList[i].Item2 != string.Empty)
                {
                    render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/skill1", 19 + 154 * (i % 2), 114 + 40 * (i / 2));
                    if (i < 10) render_bitmap(g, "UI/UIWindow2.img/SkillZero/main/line", 19 + 154 * (i % 2), 151 + 40 * (i / 2));
                    Bitmap skillIcon = scrollList[i].Item1.Bitmap;
                    string skillname = scrollList[i].Item2;
                    string maxLevel = scrollList[i].Item3;
                    if (skillIcon != null)
                        g.DrawImage(skillIcon, 21 + 154 * (i % 2), 115 + 40 * (i / 2), 32, 32);
                    g.DrawString(skillname, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 57 + 154 * (i % 2), 117 + 40 * (i / 2));
                    g.DrawString(maxLevel, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 57 + 154 * (i % 2), 135 + 40 * (i / 2));
                }
            }
            foreach (AControl zeroCtrl in this.zeroControls)
            {
                zeroCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_hyperskill(Graphics g)
        {
            Rectangle rect = this.HyperSkillRect;
            g.TranslateTransform(rect.X, rect.Y);
            if (normalmode || dualblademode)
            {
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd", 1, 0);
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/backgrnd2", 5, 22);
                g.DrawString("0", GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 150, 337);
            }
            if (evanmode)
            {
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/bg_evan/backgrnd", 1, 0);
                render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/bg_evan/backgrnd2", 5, 22);
                g.DrawString("0", GearGraphics.ItemDetailFont, GearGraphics.GrayBrush, 150, 390);
            }
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd3", 7, 47);
            switch (hyperselectedTab)
            {
                case 1:
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/Tab/enabled/0", 10, 25);
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/Tab/disabled/1", 88, 27);
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/TypeIcon/1", 15, 55);
                    g.DrawString("技能强化(被动)", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 104 - g.MeasureString("技能强化(被动)", GearGraphics.ItemDetailFont2).Width / 2, 65);
                    break;
                case 2:
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/Tab/disabled/0", 10, 27);
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/Tab/enabled/1", 88, 25);
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/HyperSkill/main/TypeIcon/2", 15, 55);
                    g.DrawString("攻击/增益(主动)", GearGraphics.ItemDetailFont, GearGraphics.WhiteBrush, 104 - g.MeasureString("攻击/增益(主动)", GearGraphics.ItemDetailFont2).Width / 2, 65);
                    break;
            }
            if (normalmode || evanmode)
                get_hyperskill(tab_list[4]);
            else if (dualblademode)
                get_hyperskill(tab_list[6]);
            for (int i = 0; i < (evanmode ? 7 : 6); i++)
            {
                if (scrollList2[i].Item2 != string.Empty)
                {
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/skill1", 10, 93 + 40 * i);
                    Bitmap skillIcon = scrollList2[i].Item1.Bitmap;
                    string skillname = scrollList2[i].Item2;
                    string maxLevel = scrollList2[i].Item3;
                    if (skillIcon != null)
                        g.DrawImage(skillIcon, 12, 94 + 40 * i, 32, 32);
                    g.DrawString(skillname, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48, 96 + 40 * i);
                    g.DrawString(maxLevel, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48, 114 + 40 * i);
                }
            }
            foreach (AControl hyperCtrl in this.hyperControls)
            {
                hyperCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_zeroex(Graphics g)
        {
            Rectangle rect = this.ZeroexRect;
            g.TranslateTransform(rect.X, rect.Y);
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd", 1, 0);
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd2", 7, 22);
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/backgrnd3", 8, 47);
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/Tab/enabled/0", 11, 27);
            render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/TypeIcon/0", 16, 55);
            g.DrawString("超越者", GearGraphics.ItemDetailFont2, GearGraphics.WhiteBrush, 24f + (160f - g.MeasureString("超越者", GearGraphics.ItemDetailFont2).Width) / 2, 65f);
            get_transcendentskill();
            for (int i = 0; i < 4; i++)
            {
                if (scrollList2[i].Item2 != string.Empty)
                {
                    render_bitmap(g, "UI/_Canvas/UIWindow2.img/SkillZeroEx/main/skill1", 10, 93 + 40 * i);
                    Bitmap skillIcon = scrollList2[i].Item1.Bitmap;
                    string skillname = scrollList2[i].Item2;
                    string maxLevel = scrollList2[i].Item3;
                    if (skillIcon != null)
                        g.DrawImage(skillIcon, 12, 94 + 40 * i, 32, 32);
                    g.DrawString(skillname, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48, 96 + 40 * i);
                    g.DrawString(maxLevel, GearGraphics.ItemDetailFont2, GearGraphics.GrayBrush, 48, 114 + 40 * i);
                }
            }
            foreach (AControl zeroexrCtrl in this.zeroexControls)
            {
                zeroexrCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_bitmap(Graphics g, string nodepath, int x, int y)
        {
            Wz_Node Node = PluginBase.PluginManager.FindWz(nodepath);
            Bitmap image = BitmapOrigin.CreateFromNode(Node, PluginBase.PluginManager.FindWz).Bitmap;
            g.DrawImage(image, x, y);
        }

        private void get_skilllist(string jobtab)
        {
            skillList.Clear();
            skillList3.Clear();
            skillList4.Clear();
            if (((normalmode || evanmode || yetipbmode || collabmode) && selectedTab < 5) || (dualblademode && selectedTab < 7)) //绘制非5转技能
            {
                foreach (Wz_Node wz_Node in PluginManager.FindWz($@"Skill/{jobtab}.img/skill").Nodes)
                {
                    Skill skill = Skill.CreateFromNode(wz_Node, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                    if (skill.Invisible || skill.Hyper != HyperSkillType.None || skill.TimeLimited) continue;
                    load_list(skill, wz_Node.Text);
                }
            }
            if (((normalmode || evanmode) && selectedTab == 5) || (dualblademode && selectedTab == 7) || (zeromode && selectedTab == 2)) //绘制5转技能
            {
                string tab = zeromode ? tab_list[1] : (dualblademode ? tab_list[6] : tab_list[4]);
                try
                {
                    foreach (Wz_Node wz_Node in PluginManager.FindWz($@"Etc/VCore.img/JobSkill/{tab}").Nodes)
                    {
                        string skillid = PluginManager.FindWz($@"Etc/VCore.img/JobSkill/{tab}/{wz_Node.Text}/id").GetValueEx<Int32>(400001000).ToString();
                        Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\40000.img\skill\{0}", skillid));
                        Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                        if (skill.Invisible || skillList.Exists(t => t.Item4 == skillid)) continue;
                        load_list(skill, skillid);
                    }
                    foreach (Wz_Node wz_Node in PluginManager.FindWz($@"Etc/VCore.img/CoreData").Nodes)
                    {
                        foreach (Wz_Node wz_Node2 in PluginManager.FindWz($@"Etc/VCore.img/CoreData/{wz_Node.Text}/job").Nodes)
                        {
                            string jobreq = wz_Node2.GetValueEx<string>(null);
                            if (jobreq == tab)
                            {
                                string skillid = PluginManager.FindWz($@"Etc/VCore.img/CoreData/{wz_Node.Text}/connectSkill/0").GetValueEx<Int32>(400001000).ToString();
                                Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", skillid.Remove(skillid.Length - 4), skillid));
                                Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                                if (skill.Invisible || skillList.Exists(t => t.Item4 == skillid)) continue;
                                load_list(skill, skillid);
                            }
                        }
                    }
                }
                catch
                {
                    Wz_Node Vcore = PluginManager.FindWz($@"Etc/VcoreNew.img/vSkill/coreGroup/{tab}") ?? PluginManager.FindWz($@"Etc/VCore.img/vSkill/coreGroup/{tab}");
                    foreach (Wz_Node wz_Node in Vcore.Nodes)
                    {
                        foreach (Wz_Node wz_Node2 in wz_Node.Nodes)
                        {
                            string tabnode = wz_Node2.GetValueEx<Int32>(400001000).ToString();
                            string skillid = PluginManager.FindWz(string.Format(@"Etc/{0}.img/vSkill/CoreData/{1}/connectSkill/0", Vcore.FullPath.Contains("VcoreNew") ? "VcoreNew" : "VCore", tabnode)).GetValueEx<Int32>(400001000).ToString();
                            Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", skillid.Remove(skillid.Length - 4), skillid));
                            Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                            if (skill.Invisible || skillList.Exists(t => t.Item4 == skillid)) continue;
                            load_list(skill, skillid);
                        }

                    }
                }
            }
            if (((normalmode || evanmode) && selectedTab == 6) || (dualblademode && selectedTab == 8) || (zeromode && selectedTab == 3))//绘制6转技能
            {
                string tab = zeromode ? tab_list[1] : (dualblademode ? tab_list[6] : tab_list[4]);
                foreach (Wz_Node wz_Node in PluginManager.FindWz($@"Etc/HexaCore.img/hexaSkill/jobCore/{tab}").Nodes)
                {
                    foreach (Wz_Node wz_Node2 in wz_Node.Nodes)
                    {
                        string tabnode = wz_Node2.GetValueEx<Int32>(400001000).ToString();
                        string skillid = PluginManager.FindWz($"Etc/HexaCore.img/hexaSkill/coreData/{tabnode}/connectSkill/0").GetValueEx<Int32>(400001000).ToString();
                        Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", skillid.Remove(skillid.Length - 4), skillid));
                        Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                        if (skill.Invisible || skillList.Exists(t => t.Item4 == skillid)) continue;
                        load_list(skill, skillid);
                    }
                }
            }
            // 计算最大滚动值
            int pageSize = evanmode ? 8 : 12;
            int maxScroll = skillList.Count > pageSize ? skillList.Count / 2 - (pageSize / 2) : 0;
            this.ScrollMaxValue = Math.Max(0, maxScroll);
            this.vScroll.Maximum = maxScroll;
            if (this.scrollValue > this.ScrollMaxValue)
            {
                this.scrollValue = this.ScrollMaxValue;
            }
            // 按scrollValue截取技能列表
            scrollList.Clear();
            int startIdx = scrollValue * 2;
            for (int i = 0; i < pageSize; i++)
            {
                int idx = startIdx + i;
                if (idx < skillList.Count)
                {
                    scrollList.Add(skillList[idx]);
                }
                else
                {
                    scrollList.Add(new Tuple<BitmapOrigin, string, string, string>(new BitmapOrigin(), string.Empty, string.Empty, string.Empty));
                }
            }
        }

        private void get_zeroskill()
        {
            skillList.Clear();
            skillList3.Clear();
            skillList4.Clear();
            List<string> zeroimg = new List<string>(){"10100", "10110", "10111", "10112"};
            if (selectedTab == 0)
            {
                foreach (Wz_Node wz_Node in PluginManager.FindWz("Skill/10000.img/skill").Nodes)
                {
                    string skillid = wz_Node.Text;
                    Skill skill = Skill.CreateFromNode(wz_Node, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                    if (skill.Invisible || (skill.Hasreq && !skill.HasreqLevel)) continue;
                    string skillname = PluginManager.FindWz($@"String/Skill.img/{skillid}/name").GetValueEx<string>(null);
                    if (skill.categoryIndex == 1 && skill.tabIndex == 0)
                        skillList4.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
                    else
                        skillList3.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
                }
            }
            else if (selectedTab == 1)
            {
                foreach (string node in zeroimg)
                {
                    foreach (Wz_Node wz_Node in PluginManager.FindWz($"Skill/{node}.img/skill").Nodes)
                    {
                        string skillid = wz_Node.Text;
                        Skill skill = Skill.CreateFromNode(wz_Node, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                        if (skill.Invisible) continue;
                        string skillname = PluginManager.FindWz($@"String/Skill.img/{skillid}/name").GetValueEx<string>(null);
                        if (skill.categoryIndex == 1 && skill.tabIndex == 1)
                            skillList4.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
                        else if (skill.categoryIndex == 0 && skill.tabIndex == 1)
                            skillList3.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
                    }
                }
            }
            int pageSize = 6;
            int maxScroll = Math.Max(skillList3.Count, skillList4.Count) > pageSize ? Math.Max(skillList3.Count, skillList4.Count) - pageSize : 0;
            this.ScrollMaxValue = Math.Max(0, maxScroll);
            this.vScroll.Maximum = maxScroll;
            int startIdx = scrollValue;
            if (this.scrollValue > this.ScrollMaxValue)
            {
                this.scrollValue = this.ScrollMaxValue;
            }
            scrollList.Clear();
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0) // 奇数位置：从skillList3取
                {
                    int index3 = startIdx + i / 2;
                    if (index3 < skillList3.Count)
                    {
                        scrollList.Add(skillList3[index3]);
                    }
                    else
                    {
                        scrollList.Add(new Tuple<BitmapOrigin, string, string, string>(new BitmapOrigin(), string.Empty, string.Empty, string.Empty));
                    }
                }
                else// 偶数位置：从skillList4取
                {
                    int index4 = startIdx + i / 2;
                    if (index4 < skillList4.Count)
                    {
                        scrollList.Add(skillList4[index4]);
                    }
                    else
                    {
                        scrollList.Add(new Tuple<BitmapOrigin, string, string, string>(new BitmapOrigin(), string.Empty, string.Empty, string.Empty));
                    }
                }
            }
        }

        private void get_hyperskill(string jobtab)
        {
            skillList2.Clear();
            foreach (Wz_Node wz_Node in PluginManager.FindWz($@"Skill/{jobtab}.img/skill").Nodes)
            {
                string skillid = wz_Node.Text;
                Skill skill = Skill.CreateFromNode(wz_Node, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                if (skill.Invisible || skill.Hyper == HyperSkillType.None || skill.HyperSkill != hyperselectedTab) continue;
                string skillname = PluginManager.FindWz($@"String/Skill.img/{wz_Node.Text}/name").GetValueEx<string>(null);
                skillList2.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
            }
            int pageSize = evanmode ? 7 : 6;
            int maxScroll = skillList2.Count > pageSize ? skillList2.Count - pageSize : 0;
            this.ScrollMaxValue2 = Math.Max(0, maxScroll);
            this.vScroll2.Maximum = maxScroll;
            if (this.scrollValue2 > this.ScrollMaxValue2)
            {
                this.scrollValue2 = this.ScrollMaxValue2;
            }
            scrollList2.Clear();
            int startIdx = scrollValue2;
            for (int i = 0; i < pageSize; i++)
            {
                int idx = startIdx + i;
                if (idx < skillList2.Count)
                {
                    scrollList2.Add(skillList2[idx]);
                }
                else
                {
                    scrollList2.Add(new Tuple<BitmapOrigin, string, string, string>(new BitmapOrigin(), string.Empty, string.Empty, string.Empty));
                }
            }
        }

        private void get_transcendentskill()
        {
            skillList2.Clear();
            foreach (Wz_Node wz_Node in PluginManager.FindWz("Skill/10000.img/skill").Nodes)
            {
                string skillid = wz_Node.Text;
                Skill skill = Skill.CreateFromNode(wz_Node, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                if (skill.Invisible) continue;
                string skillname = PluginManager.FindWz($@"String/Skill.img/{skillid}/name").GetValueEx<string>(null);
                if (skill.Hasreq && !skill.HasreqLevel)
                    skillList2.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
            }
            int pageSize = 4;
            int maxScroll = skillList2.Count > pageSize ? skillList2.Count - pageSize : 0;
            this.ScrollMaxValue2 = Math.Max(0, maxScroll);
            this.vScroll2.Maximum = maxScroll;
            if (this.scrollValue2 > this.ScrollMaxValue2)
            {
                this.scrollValue2 = this.ScrollMaxValue2;
            }
            scrollList2.Clear();
            int startIdx = scrollValue2;
            for (int i = 0; i < pageSize; i++)
            {
                int idx = startIdx + i;
                if (idx < skillList2.Count)
                {
                    scrollList2.Add(skillList2[idx]);
                }
                else
                {
                    scrollList2.Add(new Tuple<BitmapOrigin, string, string, string>(new BitmapOrigin(), string.Empty, string.Empty, string.Empty));
                }
            }
        }

        private void load_list(Skill skill, string skillid)
        {
            string skillname = PluginManager.FindWz($@"String/Skill.img/{skillid}/name").GetValueEx<string>(null);
            skillList.Add(new Tuple<BitmapOrigin, string, string, string>(skill.Icon, skillname, skill.MaxLevel.ToString(), skillid));
        }

        public int GetSlotIndexByPoint(Point point)
        {
            // 技能栏左上角起点
            int baseX = zeromode ? 19 : 10;
            int baseY = evanmode? 219 : zeromode ? 114 : 93;
            int slotWidth = evanmode ? 124 : zeromode ? 141 : 140;
            int slotHeight = (evanmode) ? 35 : 40;
            int columns = 2;
            int rows = evanmode? 4 : 6;

            Point p = new Point(point.X - baseX, point.Y - baseY);
            if (p.X < 0 || p.Y < 0)
                return -1;

            int col = p.X / slotWidth;
            int row = p.Y / slotHeight;
            if (col < 0 || col >= columns || row < 0 || row >= rows)
                return -1;

            int idx = row * columns + col;
            if (idx >= 0 && idx < (evanmode ? 8 :12))
                return idx;
            return -1;
        }

        public int GetSlotIndexByPoint2(Point point)
        {
            int baseX = 10 - baseOffset.X;
            int baseY = 93 - baseOffset.Y;
            int slotWidth = 140;
            int slotHeight = 40;
            int rows = evanmode ? 7 : ZeroexVisible ? 4 : 6;

            // 先减去偏移
            Point p = new Point(point.X - baseX, point.Y - baseY);
            if (p.X < 0 || p.Y < 0)
                return -1;

            int col = p.X / slotWidth;
            int row = p.Y / slotHeight;
            if (col < 0 || col >= 1 || row < 0 || row >= rows)
                return -1;

            int idx = row + col;
            if (idx >= 0 && idx < rows)
                return idx;
            return -1;
        }

        public int GetSkillIndexByPoint(Point point)
        {
            int slotIdx = GetSlotIndexByPoint(point);
            return slotIdx;
        }

        public int GetSkillIndexByPoint2(Point point)
        {
            int slotIdx = GetSlotIndexByPoint2(point);
            return slotIdx;
        }

        public Skill GetSkillByPoint(Point point)
        {
            int idx = GetSkillIndexByPoint(point);
            if (idx >= 0 && idx < scrollList.Count && scrollList[idx].Item2 != string.Empty)
            {
                string skillId = scrollList[idx].Item4;
                Wz_Node skillNode = PluginBase.PluginManager.FindWz($"Skill/{skillId.Substring(0, skillId.Length - 4)}.img/skill/{skillId}");
                if (skillNode != null)
                {
                    var skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                    skill.Level = skill.MaxLevel;
                    return skill;
                }
            }
            int idx2 = GetSkillIndexByPoint2(point);
            if (idx2 >= 0 && idx2 < scrollList2.Count && scrollList2[idx2].Item2 != string.Empty)
            {
                string skillId = scrollList2[idx2].Item4;
                Wz_Node skillNode = PluginBase.PluginManager.FindWz($"Skill/{skillId.Substring(0, skillId.Length - 4)}.img/skill/{skillId}");
                if (skillNode != null)
                {
                    var skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz);
                    skill.Level = skill.MaxLevel;
                    return skill;
                }
            }
            return null;
        }

        private void get_tablist(int selectJob)
        {
            switch (selectJob)
            {
                case 110: tab_list = ["000", "100", "110", "111", "112", "40001", "114"]; break;
                case 120: tab_list = ["000", "100", "120", "121", "122", "40001", "124"]; break;
                case 130: tab_list = ["000", "100", "130", "131", "132", "40001", "134"]; break;
                case 210: tab_list = ["000", "200", "210", "211", "212", "40002", "214"]; break;
                case 220: tab_list = ["000", "200", "220", "221", "222", "40002", "224"]; break;
                case 230: tab_list = ["000", "200", "230", "231", "232", "40002", "234"]; break;
                case 310: tab_list = ["000", "300", "310", "311", "312", "40003", "314"]; break;
                case 320: tab_list = ["000", "300", "320", "321", "322", "40003", "324"]; break;
                case 330: tab_list = ["000", "300", "330", "331", "332", "40003", "334"]; break;
                case 410: tab_list = ["000", "400", "410", "411", "412", "40004", "414"]; break;
                case 420: tab_list = ["000", "400", "420", "421", "422", "40004", "424"]; break;
                case 430: tab_list = ["000", "400", "430", "431", "432", "433", "434", "40004", "436"]; break;
                case 510: tab_list = ["000", "500", "510", "511", "512", "40005", "514"]; break;
                case 520: tab_list = ["000", "500", "520", "521", "522", "40005", "524"]; break;
                case 530: tab_list = ["000", "500", "530", "531", "532", "40005", "534"]; break;
                case 1100: tab_list = ["1000", "1100", "1110", "1111", "1112", "40001", "1114"]; break;
                case 1200: tab_list = ["1000", "1200", "1210", "1211", "1212", "40002", "1214"]; break;
                case 1300: tab_list = ["1000", "1300", "1310", "1311", "1312", "40003", "1314"]; break;
                case 1400: tab_list = ["1000", "1400", "1410", "1411", "1412", "40004", "1414"]; break;
                case 1500: tab_list = ["1000", "1500", "1510", "1511", "1512", "40005", "1514"]; break;
                case 2100: tab_list = ["2000", "2100", "2110", "2111", "2112", "40001", "2114"]; break;
                case 2200: tab_list = ["2001", "2200", "2211", "2214", "2217", "40002", "2220"]; break;
                case 2300: tab_list = ["2002", "2300", "2310", "2311", "2312", "40003", "2314"]; break;
                case 2400: tab_list = ["2003", "2400", "2410", "2411", "2412", "40004", "2414"]; break;
                case 2500: tab_list = ["2005", "2500", "2510", "2511", "2512", "40005", "2514"]; break;
                case 2700: tab_list = ["2004", "2700", "2710", "2711", "2712", "40002", "2714"]; break;
                case 3100: tab_list = ["3001", "3100", "3110", "3111", "3112", "40001", "3114"]; break;
                case 3101: tab_list = ["3001", "3101", "3120", "3124", "3122", "40001", "3124"]; break;
                case 3200: tab_list = ["3000", "3200", "3210", "3211", "3212", "40002", "3214"]; break;
                case 3300: tab_list = ["3000", "3300", "3310", "3311", "3312", "40003", "3314"]; break;
                case 3500: tab_list = ["3000", "3500", "3510", "3511", "3512", "40005", "3514"]; break;
                case 3600: tab_list = ["3002", "3600", "3610", "3611", "3612", "40004", "3614"]; break;
                case 3700: tab_list = ["3000", "3700", "3710", "3711", "3712", "40001", "3714"]; break;
                case 4100: tab_list = ["4001", "4100", "4110", "4111", "4112", "40001", "4114"]; break;
                case 4200: tab_list = ["4002", "4200", "4210", "4211", "4212", "40002", "4214"]; break;
                case 5100: tab_list = ["5000", "5100", "5110", "5111", "5112", "40001", "5114"]; break;
                case 6100: tab_list = ["6000", "6100", "6110", "6111", "6112", "40001", "6114"]; break;
                case 6300: tab_list = ["6003", "6300", "6310", "6311", "6312", "40003", "6314"]; break;
                case 6400: tab_list = ["6002", "6400", "6410", "6411", "6412", "40004", "6414"]; break;
                case 6500: tab_list = ["6001", "6500", "6510", "6511", "6512", "40005", "6514"]; break;
                case 10100: tab_list = ["", "10112", "40001", "10114"]; break;
                case 12100: tab_list = ["12005", "12100", "40001", ""]; break;
                case 12200: tab_list = ["12006", "12200", "40005", ""]; break;
                case 13100: tab_list = ["13000", "13100"]; break;
                case 13500: tab_list = ["13001", "13500"]; break;
                case 14200: tab_list = ["14000", "14200", "14210", "14211", "14212", "40002", "14214"]; break;
                case 15100: tab_list = ["15002", "15100", "15110", "15111", "15112", "40001", "15114"]; break;
                case 15200: tab_list = ["15000", "15200", "15210", "15211", "15212", "40002", "15214"]; break;
                case 15400: tab_list = ["15003", "15400", "15410", "15411", "15412", "40003", "15414"]; break;
                case 15500: tab_list = ["15001", "15500", "15510", "15511", "15512", "40005", "15514"]; break;
                case 16100: tab_list = ["16002", "16100", "16110", "16111", "16112", "40001", "16114"]; break;
                case 16200: tab_list = ["16001", "16200", "16210", "16211", "16212", "40002", "16214"]; break;
                case 16400: tab_list = ["16000", "16400", "16410", "16411", "16412", "40004", "16414"]; break;
                case 17200: tab_list = ["17001", "17200", "17210", "17211", "17212", "40002", "17214"]; break;
                case 17500: tab_list = ["17000", "17500", "17510", "17511", "17512", "40005", "17514"]; break;
                case 18200: tab_list = ["18000", "18200", "18210", "18211", "18212", "40002", "18214"]; break;
                default: break;
            }
            switch (selectJob)
            {
                case 430: normalmode = false; dualblademode = true; evanmode = false; zeromode = false; yetipbmode = false; collabmode = false; break;
                case 2200: normalmode = false; dualblademode = false; evanmode = true; zeromode = false; yetipbmode = false; collabmode = false; break;
                case 10100: normalmode = false; dualblademode = false; evanmode = false; zeromode = true; yetipbmode = false; collabmode = false; break;
                case 12100:
                case 12200: normalmode = false; dualblademode = false; evanmode = false; zeromode = false; yetipbmode = false; collabmode = true; break;
                case 13100:
                case 13500: normalmode = false; dualblademode = false; evanmode = false; zeromode = false; yetipbmode = true; collabmode = false; break;
                default: normalmode = true; dualblademode = false; evanmode = false; zeromode = false; yetipbmode = false; collabmode = false; break;
            }
        }

        private IEnumerable<AControl> aControls //普通模式专用控件
        {
            get
            {
                yield return vScroll;
                yield return btnClose;
                yield return btnGuildSkill;
                yield return btnRide;
                yield return btnSkillSkin;
                yield return btnSequence;
                yield return btnMacro;
                yield return btnTab0;
                yield return btnTab1;
                if (normalmode || dualblademode)
                {
                    yield return btnTab2;
                    yield return btnTab3;
                    yield return btnTab4;
                    yield return btnTab5;
                    yield return btnTab6;
                    yield return btnVMatrix;
                    yield return btnHexaMatrix;
                    yield return btnHyper;
                }
                if (dualblademode)
                {
                    yield return btnTab7;
                    yield return btnTab8;
                }
            }
        }

        private IEnumerable<AControl> evanControls //龙神模式专用控件
        {
            get
            {
                yield return vScroll;
                yield return btnClose;
                yield return btnVMatrix2;
                yield return btnHexaMatrix2;
                yield return btnHyper2;
                yield return btnGuildSkill;
                yield return btnRide2;
                yield return btnSequence2;
                yield return btnMacro2;
                yield return btnDlg;
                yield return btnTab0;
                yield return btnTab1;
                yield return btnTab2;
                yield return btnTab3;
                yield return btnTab4;
                yield return btnTab5;
                yield return btnTab6;
            }
        }

        private IEnumerable<AControl> zeroControls //神之子模式专用控件
        {
            get
            {
                yield return vScroll;
                yield return btnClose;
                yield return btnVMatrix3;
                yield return btnHexaMatrix3;
                yield return btnEx;
                yield return btnGuildSkill;
                yield return btnRide3;
                yield return btnSequence3;
                yield return btnMacro3;
                yield return btnTab0;
                yield return btnTab1;
                yield return btnTab2;
                yield return btnTab3;
            }
        }

        private IEnumerable<AControl> hyperControls //超级技能专用控件
        {
            get
            {
                yield return vScroll2;
                yield return btnPassive;
                yield return btnActive;
                yield return btnReset;
            }
        }

        private IEnumerable <AControl> zeroexControls //神之子女神技能专用控件
        {
            get
            {
                yield return vScroll2;
            }
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void vScroll_ValueChanged(object sender, EventArgs e)
        {
            this.scrollValue = this.vScroll.Value;
            this.waitForRefresh = true;
        }

        private void vScroll2_ValueChanged(object sender, EventArgs e)
        {
            this.scrollValue2 = this.vScroll2.Value;
            this.waitForRefresh = true;
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
            if (this.Owner is MainForm mainForm)
            {
                mainForm.buttonSkill.Checked = false;
            }
        }
        private void btnTab0_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 0;
            this.waitForRefresh = true;
        }

        private void btnTab1_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 1;
            this.waitForRefresh = true;
        }

        private void btnTab2_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 2;
            this.waitForRefresh = true;
        }

        private void btnTab3_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 3;
            this.waitForRefresh = true;
        }

        private void btnTab4_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 4;
            this.waitForRefresh = true;
        }

        private void btnTab5_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 5;
            this.waitForRefresh = true;
        }

        private void btnTab6_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 6;
            this.waitForRefresh = true;
        }

        private void btnTab7_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 7;
            this.waitForRefresh = true;
        }

        private void btnTab8_MouseClick(object sender, MouseEventArgs e)
        {
            this.selectedTab = 8;
            this.waitForRefresh = true;
        }

        private void btnHyper_MouseClick(object sender, MouseEventArgs e)
        {
            this.HyperStatVisible = !this.HyperStatVisible;
            this.ZeroexVisible = false;
            this.waitForRefresh = true;
        }

        private void btnPassive_MouseClick(object sender, MouseEventArgs e)
        {
            this.hyperselectedTab = 1;
            this.waitForRefresh = true;
        }

        private void btnActive_MouseClick(object sender, MouseEventArgs e)
        {
            this.hyperselectedTab = 2;
            this.waitForRefresh = true;
        }

        private void btnEx_MouseClick(object sender, MouseEventArgs e)
        {
            this.ZeroexVisible = !this.ZeroexVisible;
            this.HyperStatVisible = false;
            this.waitForRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.evanControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            MouseEventArgs hyperSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperSkillRect.X, e.Y - HyperSkillRect.Y, e.Delta);

            foreach (AControl ctrl in this.hyperControls)
            {
                ctrl.OnMouseMove(hyperSkillChildArgs);
            }

            MouseEventArgs zeroexSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - ZeroexRect.X, e.Y - ZeroexRect.Y, e.Delta);

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseMove(zeroexSkillChildArgs);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseMove(e);

            object obj = GetSkillByPoint(new Point(e.Location.X - baseOffset.X, e.Location.Y - baseOffset.Y));
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

            foreach (AControl ctrl in this.evanControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            MouseEventArgs hyperSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperSkillRect.X, e.Y - HyperSkillRect.Y, e.Delta);

            foreach (AControl ctrl in this.hyperControls)
            {
                ctrl.OnMouseDown(hyperSkillChildArgs);
            }

            MouseEventArgs zeroexSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - ZeroexRect.X, e.Y - ZeroexRect.Y, e.Delta);

            foreach (AControl ctrl in this.zeroexControls)
            {
                ctrl.OnMouseDown(zeroexSkillChildArgs);
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

            foreach (AControl ctrl in this.evanControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            MouseEventArgs hyperSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperSkillRect.X, e.Y - HyperSkillRect.Y, e.Delta);

            foreach (AControl ctrl in this.hyperControls)
            {
                ctrl.OnMouseUp(hyperSkillChildArgs);
            }

            MouseEventArgs zeroexSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - ZeroexRect.X, e.Y - ZeroexRect.Y, e.Delta);

            foreach (AControl ctrl in this.zeroexControls)
            {
                ctrl.OnMouseUp(zeroexSkillChildArgs);
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

            foreach (AControl ctrl in this.evanControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            MouseEventArgs hyperSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperSkillRect.X, e.Y - HyperSkillRect.Y, e.Delta);

            foreach (AControl ctrl in this.hyperControls)
            {
                ctrl.OnMouseClick(hyperSkillChildArgs);
            }

            MouseEventArgs zeroexSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - ZeroexRect.X, e.Y - ZeroexRect.Y, e.Delta);

            foreach (AControl ctrl in this.zeroexControls)
            {
                ctrl.OnMouseClick(zeroexSkillChildArgs);
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

            foreach (AControl ctrl in this.evanControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            foreach (AControl ctrl in this.zeroControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            MouseEventArgs hyperSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - HyperSkillRect.X, e.Y - HyperSkillRect.Y, e.Delta);

            foreach (AControl ctrl in this.hyperControls)
            {
                ctrl.OnMouseWheel(hyperSkillChildArgs);
            }

            MouseEventArgs zeroexSkillChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - ZeroexRect.X, e.Y - ZeroexRect.Y, e.Delta);

            foreach (AControl ctrl in this.zeroexControls)
            {
                ctrl.OnMouseWheel(zeroexSkillChildArgs);
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

        void AfrmJob_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
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
                    dlg.FileName = "Skill UI Preview";

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
