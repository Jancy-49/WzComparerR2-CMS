using CharaSimResource;
using DevComponents.AdvTree;
using SharpDX.Direct3D11;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;
using WzComparerR2.PluginBase;
using WzComparerR2.WzLib;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmArchive : AlphaForm
    {
        public AfrmArchive()
        {
            InitArchive();
        }

        private Point baseOffset;
        private Point newLocation;
        private bool waitForRefresh;

        private ACtrlVScroll vScroll;
        private ACtrlVScroll vScroll2;
        private ACtrlVScroll vScroll3;
        private ACtrlButton btnClose;
        private ACtrlButton btnClose2;
        private ACtrlButton btnClose3;
        private ACtrlButton btnClose4;
        private ACtrlButton btnReward;
        private ACtrlButton btnHelp;
        private ACtrlButton btnWorldSelect;
        private ACtrlButton btnWorldSelect2;
        private ACtrlButton btnWorldSelect3;
        private ACtrlButton btnSupplement;
        private ACtrlButton btnBack;
        private ACtrlButton btnRegion;
        private ACtrlButton btnCharacter;
        private ACtrlButton btnMonster;
        private ACtrlButton btnlistUp;
        private ACtrlButton btnlistDown;
        private ACtrlButton btnlistUp2;
        private ACtrlButton btnlistDown2;
        private ACtrlButton btnRight;
        private ACtrlButton btnLeft;
        private ACtrlButton btnHidden;
        private ACtrlButton btnTitle;
        private ACtrlButton btnBookMark;
        private ACtrlButton btnBookMark2;
        private ACtrlButton btnBookMark3;
        private ACtrlButton btnChapterPrev;
        private ACtrlButton btnChapterNext;
        private List<ACtrlButton> btnBooks = new List<ACtrlButton>();
        private List<ACtrlButton> btnMobs = new List<ACtrlButton>();
        private List<ACtrlButton> btnNpcs = new List<ACtrlButton>();
        private List<ACtrlButton> Titles = new List<ACtrlButton>();
        private List<ACtrlButton> Chapters = new List<ACtrlButton>();

        private DateTime lastPageSwitchTime = DateTime.MinValue;
        private const int CLICK_IGNORE_MS = 200; // 忽略点击的毫秒数

        private int scrollValue = 0;
        private int scrollValue2 = 0;
        private int scrollValue3 = 0;
        private int selectedIndex = -1;
        private int selectedIndex2 = 0;
        private int selectedIndex3 = 0;
        private int lastLoadedBookIndex = -1;
        private int lastLoadedNpcIndex = -1;
        private int lastLoadedMobIndex = -1;
        private int lastLoadedChapterIndex = -1;
        private int selectedTab = 0;
        private int selectedTitle = -1;
        private int selectedChapter = -1;
        private int bookArea = -1;
        private int page = 0;
        private int maxpage = 0;
        private int detailpage = 0;
        private int maxdetailpage = 0;
        private int maxcount = 0;
        private List<string> books = new List<String>();
        private List<string> mobs = new List<String>();
        private List<string> npcs = new List<String>();
        private List<string> npcNodeNames = new List<String>();
        private List<string> mobNodeNames = new List<String>();
        private List<string> specialNpcs = new List<String>();
        private List<string> imageNpcs = new List<String>();
        private List<string> imageMobs = new List<String>();
        private List<string> Infos = new List<String>();
        private bool mainPage = true;
        private bool bookPage = false;
        private bool detailPage = false;
        private bool extraPage = false;
        private bool showIllust = false;
        private bool tablePage = false;
        private bool tablePage2 = false;
        private bool detailInfo = false;


        private Rectangle BookRect
        {
            get
            {
                return new Rectangle(new Point(319, 79), new Size(626, 468));
            }
        }

        private void InitArchive()
        {
            this.vScroll = new ACtrlVScroll();  //书籍区域鼠标滚轮

            this.vScroll.PicBase.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/base"), PluginBase.PluginManager.FindWz);
            this.vScroll.PicBase.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/disabled/base"), PluginBase.PluginManager.FindWz);

            this.vScroll.BtnPrev.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/prev0"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnPrev.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/prev1"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnPrev.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/prev2"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnPrev.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/disabled/prev"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll.BtnPrev.Location = new Point(0, 0);

            this.vScroll.BtnNext.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/next0"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnNext.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/next1"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnNext.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/enabled/next2"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnNext.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr114/disabled/next"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll.BtnNext.Location = new Point(0, 440);

            this.vScroll.BtnThumb.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr114/enabled/thumb0"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnThumb.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr114/enabled/thumb0"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnThumb.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr114/enabled/thumb0"), PluginBase.PluginManager.FindWz);
            this.vScroll.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll.Location = new Point(613, 13);
            this.vScroll.Size = new Size(5, 445);
            this.vScroll.ScrollableLocation = new Point(319, 79);
            this.vScroll.ScrollableSize = new Size(626, 468);
            this.vScroll.Value = scrollValue;
            this.vScroll.Visible = false;
            this.vScroll.ValueChanged += new EventHandler(vScroll_ValueChanged);
            this.vScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.vScroll2 = new ACtrlVScroll();  //NPC和怪物区域鼠标滚轮

            this.vScroll2.PicBase.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/base"), PluginBase.PluginManager.FindWz);
            this.vScroll2.PicBase.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/disabled/base"), PluginBase.PluginManager.FindWz);

            this.vScroll2.BtnPrev.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/prev0"), PluginBase.PluginManager.FindWz);
            this.vScroll2.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll2.BtnPrev.Location = new Point(0, 0);

            this.vScroll2.BtnNext.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/next0"), PluginBase.PluginManager.FindWz);
            this.vScroll2.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll2.BtnNext.Location = new Point(0, 388);

            this.vScroll2.BtnThumb.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb0"), PluginBase.PluginManager.FindWz);
            this.vScroll2.BtnThumb.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb1"), PluginBase.PluginManager.FindWz);
            this.vScroll2.BtnThumb.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb2"), PluginBase.PluginManager.FindWz);
            this.vScroll2.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll2.Location = new Point(895, 111);
            this.vScroll2.Size = new Size(5, 388);
            this.vScroll2.ScrollableLocation = new Point(830, 111);
            this.vScroll2.ScrollableSize = new Size(92, 388);
            this.vScroll2.Value = scrollValue2;
            this.vScroll2.Visible = false;
            this.vScroll2.ValueChanged += new EventHandler(vScroll2_ValueChanged);
            this.vScroll2.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.vScroll3 = new ACtrlVScroll();  //文字区域滚轮

            this.vScroll3.PicBase.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/base"), PluginBase.PluginManager.FindWz);
            this.vScroll3.PicBase.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/disabled/base"), PluginBase.PluginManager.FindWz);

            this.vScroll3.BtnPrev.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/prev0"), PluginBase.PluginManager.FindWz);
            this.vScroll3.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll3.BtnPrev.Location = new Point(0, 0);

            this.vScroll3.BtnNext.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/Basic.img/VScr113/enabled/next0"), PluginBase.PluginManager.FindWz);
            this.vScroll3.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll3.BtnNext.Location = new Point(0, 388);

            this.vScroll3.BtnThumb.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb0"), PluginBase.PluginManager.FindWz);
            this.vScroll3.BtnThumb.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb1"), PluginBase.PluginManager.FindWz);
            this.vScroll3.BtnThumb.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/Basic.img/VScr113/enabled/thumb2"), PluginBase.PluginManager.FindWz);
            this.vScroll3.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll3.Location = new Point(800, 110);
            this.vScroll3.Size = new Size(5, 388);
            this.vScroll3.ScrollableLocation = new Point(493, 113);
            this.vScroll3.ScrollableSize = new Size(275, 386);
            this.vScroll3.Value = scrollValue3;
            this.vScroll3.Visible = false;
            this.vScroll3.ValueChanged += new EventHandler(vScroll3_ValueChanged);
            this.vScroll3.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton();//主页关闭按钮
            this.btnClose.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/ChatBalloon.img/popupSayEx/Button/close/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnClose.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/ChatBalloon.img/popupSayEx/Button/close/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnClose.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/ChatBalloon.img/popupSayEx/Button/close/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnClose.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/ChatBalloon.img/popupSayEx/Button/close/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnClose.Location = new Point(929, 12);
            this.btnClose.Size = new Size(11, 11);
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);

            this.btnClose2 = new ACtrlButton();//从Extra页面返回主页
            this.btnClose2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:close/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnClose2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:close/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnClose2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:close/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnClose2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:close/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnClose2.Location = new Point(949, 73);
            this.btnClose2.Size = new Size(12, 13);
            this.btnClose2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose2.MouseClick += new MouseEventHandler(btnClose2_MouseClick);

            this.btnClose3 = new ACtrlButton();//返回书籍选择页面
            this.btnClose3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:close/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnClose3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:close/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnClose3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:close/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnClose3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:close/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnClose3.Location = new Point(793, 65);
            this.btnClose3.Size = new Size(13, 13);
            this.btnClose3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose3.MouseClick += new MouseEventHandler(btnClose3_MouseClick);

            this.btnClose4 = new ACtrlButton();//插画返回详情页面
            this.btnClose4.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:close/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnClose4.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:close/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnClose4.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:close/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnClose4.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:close/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnClose4.Location = new Point(1304, 17);
            this.btnClose4.Size = new Size(18, 18);
            this.btnClose4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose4.MouseClick += new MouseEventHandler(btnClose4_MouseClick);

            this.btnReward = new ACtrlButton();
            this.btnReward.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:openRewardUI/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnReward.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:openRewardUI/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnReward.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:openRewardUI/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnReward.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:openRewardUI/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnReward.Location = new Point(803, 42);
            this.btnReward.Size = new Size(75, 27);
            this.btnReward.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnHelp = new ACtrlButton();
            this.btnHelp.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:help/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHelp.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:help/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHelp.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:help/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHelp.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:help/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHelp.Location = new Point(883, 42);
            this.btnHelp.Size = new Size(46, 27);
            this.btnHelp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnBack = new ACtrlButton();
            this.btnBack.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/main/button:back/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnBack.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/main/button:back/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnBack.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/main/button:back/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnBack.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/main/button:back/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnBack.Location = new Point(334, 42);
            this.btnBack.Size = new Size(46, 27);
            this.btnBack.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnBack.MouseClick += new MouseEventHandler(btnBack_MouseClick);

            this.btnWorldSelect = new ACtrlButton();
            this.btnWorldSelect.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_0/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_0/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_0/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_0/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect.Location = new Point(372, 356);
            this.btnWorldSelect.Size = new Size(255, 173);
            this.btnWorldSelect.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnWorldSelect.MouseClick += new MouseEventHandler(btnWorldSelect_MouseClick);
            //this.btnWorldSelect.MouseMove += new MouseEventHandler(btnWorldSelect_MouseMove);

            this.btnWorldSelect2 = new ACtrlButton();
            this.btnWorldSelect2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_1/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_1/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_1/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_1/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect2.Location = new Point(123, 115);
            this.btnWorldSelect2.Size = new Size(255, 173);
            this.btnWorldSelect2.Visible = false;
            this.btnWorldSelect2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnWorldSelect2.MouseClick += new MouseEventHandler(btnWorldSelect2_MouseClick);
            //this.btnWorldSelect2.MouseMove += new MouseEventHandler(btnWorldSelect_MouseMove);

            this.btnWorldSelect3 = new ACtrlButton();
            this.btnWorldSelect3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_2/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_2/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_2/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_2/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnWorldSelect3.Location = new Point(604, 164);
            this.btnWorldSelect3.Size = new Size(255, 173);
            this.btnWorldSelect3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnWorldSelect3.MouseClick += new MouseEventHandler(btnWorldSelect3_MouseClick);
            //this.btnWorldSelect3.MouseMove += new MouseEventHandler(btnWorldSelect_MouseMove);

            this.btnSupplement = new ACtrlButton();  //进入Extra页面
            this.btnSupplement.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:supplement/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSupplement.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:supplement/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSupplement.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:supplement/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSupplement.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/button:supplement/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSupplement.Location = new Point(49, 480);
            this.btnSupplement.Size = new Size(125, 46);
            this.btnSupplement.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSupplement.MouseClick += new MouseEventHandler(btnSupplement_MouseClick);

            this.btnRegion = new ACtrlButton();
            this.btnRegion.Location = new Point(38, 0);
            this.btnRegion.Size = new Size(96, 53);
            this.btnRegion.Visible = false;
            this.btnRegion.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnRegion.MouseClick += new MouseEventHandler(btnRegion_MouseClick);

            this.btnCharacter = new ACtrlButton();
            this.btnCharacter.Location = new Point(137, 0);
            this.btnCharacter.Size = new Size(96, 53);
            this.btnCharacter.Visible = false;
            this.btnCharacter.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnCharacter.MouseClick += new MouseEventHandler(btnCharacter_MouseClick);

            this.btnMonster = new ACtrlButton();
            this.btnMonster.Location = new Point(236, 0);
            this.btnMonster.Size = new Size(96, 53);
            this.btnMonster.Visible = false;
            this.btnMonster.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnMonster.MouseClick += new MouseEventHandler(btnMonster_MouseClick);

            this.btnlistUp = new ACtrlButton();
            this.btnlistUp.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listUp/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listUp/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listUp/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listUp/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp.Location = new Point(857, 97);
            this.btnlistUp.Size = new Size(19, 13);
            this.btnlistUp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnlistUp.MouseClick += new MouseEventHandler(btnlistUp_MouseClick);

            this.btnlistDown = new ACtrlButton();
            this.btnlistDown.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listDown/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listDown/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listDown/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/list/button:listDown/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown.Location = new Point(857, 505);
            this.btnlistDown.Size = new Size(19, 13);
            this.btnlistDown.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnlistDown.MouseClick += new MouseEventHandler(btnlistDown_MouseClick);

            this.btnlistUp2 = new ACtrlButton();
            this.btnlistUp2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listUp/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listUp/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listUp/mousOver/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listUp/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnlistUp2.Location = new Point(924, 194);
            this.btnlistUp2.Size = new Size(13, 9);
            this.btnlistUp2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnlistUp2.MouseClick += new MouseEventHandler(btnlistUp2_MouseClick);

            this.btnlistDown2 = new ACtrlButton();
            this.btnlistDown2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listDown/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listDown/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listDown/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/button:listDown/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnlistDown2.Location = new Point(924, 513);
            this.btnlistDown2.Size = new Size(13, 9);
            this.btnlistDown2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnlistDown2.MouseClick += new MouseEventHandler(btnlistDown2_MouseClick);

            this.btnRight = new ACtrlButton();
            this.btnRight.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:right/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnRight.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:right/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnRight.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:right/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnRight.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:right/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnRight.Location = new Point(1296, 370);
            this.btnRight.Size = new Size(38, 72);
            this.btnRight.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnRight.MouseClick += new MouseEventHandler(btnRight_MouseClick);

            this.btnLeft = new ACtrlButton();
            this.btnLeft.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:left/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnLeft.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:left/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnLeft.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:left/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnLeft.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/button:left/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnLeft.Location = new Point(52, 370);
            this.btnLeft.Size = new Size(38, 72);
            this.btnLeft.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnLeft.MouseClick += new MouseEventHandler(btnLeft_MouseClick);

            this.btnHidden = new ACtrlButton();
            this.btnHidden.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:hidden/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnHidden.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:hidden/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnHidden.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:hidden/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnHidden.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/button:hidden/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnHidden.Location = new Point(604, 537);
            this.btnHidden.Size = new Size(211, 120);
            this.btnHidden.Visible = false;
            this.btnHidden.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHidden.MouseClick += new MouseEventHandler(btnHidden_MouseClick);

            this.btnTitle = new ACtrlButton();
            this.btnTitle.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:title/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:title/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:title/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:title/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Location = new Point(130, 0);
            this.btnTitle.Size = new Size(196, 60);
            this.btnTitle.Visible = false;
            this.btnTitle.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTitle.MouseClick += new MouseEventHandler(btnTitle_MouseClick);

            this.btnBookMark = new ACtrlButton();
            this.btnBookMark.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/0/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/0/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/0/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/0/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark.Location = new Point(889, 101);
            this.btnBookMark.Size = new Size(121, 78);
            this.btnBookMark.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnBookMark.MouseClick += new MouseEventHandler(btnBookMark_MouseClick);

            this.btnBookMark2 = new ACtrlButton();
            this.btnBookMark2.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/1/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark2.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/1/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark2.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/1/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark2.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/1/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark2.Location = new Point(889, 101);
            this.btnBookMark2.Size = new Size(121, 78);
            this.btnBookMark2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnBookMark2.MouseClick += new MouseEventHandler(btnBookMark2_MouseClick);

            this.btnBookMark3 = new ACtrlButton();
            this.btnBookMark3.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/2/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark3.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/2/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark3.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/2/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark3.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/bookMark/2/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnBookMark3.Location = new Point(889, 101);
            this.btnBookMark3.Size = new Size(121, 78);
            this.btnBookMark3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnBookMark3.MouseClick += new MouseEventHandler(btnBookMark3_MouseClick);

            this.btnChapterPrev = new ACtrlButton();//章节上一页
            this.btnChapterPrev.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pagePrev/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterPrev.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pagePrev/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterPrev.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pagePrev/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterPrev.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pagePrev/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterPrev.Location = new Point(128, 517);
            this.btnChapterPrev.Size = new Size(16, 23);
            this.btnChapterPrev.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnChapterPrev.MouseClick += new MouseEventHandler(btnChapterPrev_MouseClick);

            this.btnChapterNext = new ACtrlButton();//章节下一页
            this.btnChapterNext.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pageNext/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterNext.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pageNext/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterNext.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pageNext/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterNext.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/button:pageNext/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnChapterNext.Location = new Point(862, 517);
            this.btnChapterNext.Size = new Size(16, 23);
            this.btnChapterNext.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnChapterNext.MouseClick += new MouseEventHandler(btnChapterNext_MouseClick);

            btns();
        }

        private void btns()
        {
            int i = 0;
            foreach (Wz_Node node in PluginManager.FindWz($@"UI/UIWorldArchiveBonusBook.img/info").Nodes)
            {
                string name = PluginManager.FindWz($"UI/_Canvas/UIWorldArchiveBonusBook.img/info/{node.Text}/name").GetValueEx<string>(null);
                if (!Infos.Contains(name)) Infos.Add(name);
                var Title = new ACtrlButton();
                Title.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/button/normal/0"), PluginBase.PluginManager.FindWz);
                Title.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/button/pressed/0"), PluginBase.PluginManager.FindWz);
                Title.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/button/pressed/0"), PluginBase.PluginManager.FindWz);
                Title.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/button/normal/0"), PluginBase.PluginManager.FindWz);
                Title.Size = new Size(273, 73);
                Title.Location = new Point(562, 144 + 80 * i);
                Title.Visible = false;
                Title.MouseClick += new MouseEventHandler(Title_MouseClick);
                Titles.Add(Title);
                i++;
            }
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, (mainPage || bookPage)? new Size(956, 30) : detailPage ? new Size(861, 40) : new Size(1044, 40));
            this.Location = newLocation;
            base.Refresh();
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            control_event();

            //计算图像大小
            Point baseOffsetnew = calcRenderBaseOffset();
            Size size = new Size(0, 0);
            if (mainPage)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/worldSelect/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (bookPage)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (detailPage && selectedIndex > -1)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (extraPage)
                size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            else if (showIllust)
                size = new Size(1344, 740);
            if (selectedTab > 0)
                size.Width += 67;
            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X - baseOffsetnew.X,
                this.Location.Y + this.baseOffset.Y - baseOffsetnew.Y);
            this.baseOffset = baseOffsetnew;

            //绘制图像
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bitmap);
            if (mainPage)
                render_base(g);
            else if (bookPage)
            {
                render_mapleWorld(g);
                render_book(g);
            }
            else if (detailPage && selectedIndex > -1)
                render_detail(g);
            else if (extraPage)
                render_extra(g);
            else if (showIllust)
                render_illust(g);
            g.Dispose();
            this.Bitmap = bitmap;
        }

        private Point calcRenderBaseOffset()
        {
            return new Point(0, 0);
        }

        private void control_event()
        {
            foreach (Wz_Node node in PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/illust/npc").Nodes)
            {
                string npcID = node.Text;
                if(!specialNpcs.Contains(npcID)) specialNpcs.Add(npcID);
            }
            foreach (Wz_Node node in PluginManager.FindWz("UI/UIworldArchive.img/image/npc").Nodes)
            {
                string npcID = node.Text;
                if(!imageNpcs.Contains(npcID)) imageNpcs.Add(npcID);
            }
            foreach (Wz_Node node in PluginManager.FindWz("UI/UIworldArchive.img/image/mob").Nodes)
            {
                string mobID = node.Text;
                if(!imageMobs.Contains(mobID)) imageMobs.Add(mobID);
            }
            maxcount = 0;
            foreach (Wz_Node node in PluginManager.FindWz("Etc/worldArchive.img/collectionInfo").Nodes)
            {
                foreach (Wz_Node node2 in node.Nodes)
                {
                    if (!Regex.Match(node2.Text, @"\d+$").Success) continue;
                    string bookID = node2.Text;
                    int mobcount = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{node.Text}/{bookID}/mob").Nodes.Count;
                    int npccount = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{node.Text}/{bookID}/npc").Nodes.Count;
                    maxcount += mobcount + npccount;
                }
            }
            this.btnWorldSelect.Visible = mainPage && PluginManager.FindWz("Etc/worldArchive.img/collectionInfo/0/0") != null;
            this.btnWorldSelect2.Visible = mainPage && PluginManager.FindWz("Etc/worldArchive.img/collectionInfo/1/0") != null;
            this.btnWorldSelect3.Visible = mainPage && PluginManager.FindWz("Etc/worldArchive.img/collectionInfo/2/0") != null;
            this.btnClose.Visible = mainPage;
            this.btnClose2.Visible = extraPage;
            this.btnClose3.Visible = detailPage;
            this.btnClose4.Visible = showIllust;
            this.btnReward.Visible = mainPage || bookPage;
            this.btnHelp.Visible = mainPage || bookPage;
            this.btnBack.Visible = bookPage;
            this.vScroll.Visible = bookPage;
            this.vScroll2.Visible = detailPage && selectedTab > 0;
            this.vScroll3.Visible = detailPage;
            this.btnlistUp.Visible = detailPage && selectedTab > 0;
            this.btnlistDown.Visible = detailPage && selectedTab > 0;
            this.btnlistUp2.Visible = extraPage && tablePage2;
            this.btnlistDown2.Visible = extraPage && tablePage2;
            this.btnRegion.Visible = detailPage;
            this.btnCharacter.Visible = detailPage && npcs.Count > 0;
            this.btnMonster.Visible = detailPage && mobs.Count > 0;
            this.btnHidden.Visible = detailPage && selectedTab == 1 && specialNpcs.Contains(npcs[selectedIndex2]);
            this.btnRight.Visible = showIllust && page < maxpage - 1;
            this.btnLeft.Visible = showIllust && page > 0 && maxpage > 0;
            this.btnTitle.Visible = extraPage && (tablePage2 || detailInfo);
            this.btnBookMark.Visible = extraPage && (tablePage2 || detailInfo) && selectedTitle == 0;
            this.btnBookMark2.Visible = extraPage && (tablePage2 || detailInfo) && selectedTitle == 1;
            this.btnBookMark3.Visible = extraPage && (tablePage2 || detailInfo) && selectedTitle == 2;
            this.btnChapterPrev.Visible = extraPage && detailInfo && detailpage > 0 && maxdetailpage > 0;
            this.btnChapterNext.Visible = extraPage && detailInfo && detailpage < maxdetailpage && maxdetailpage > 0;
            if (bookArea > -1 && bookArea != lastLoadedBookIndex)
            {
                lastLoadedBookIndex = bookArea;
                btnBooks.Clear();
                books.Clear();
                int i = 0;
                var nodes = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}").Nodes;
                foreach (Wz_Node node in nodes)
                {
                    if (!Regex.Match(node.Text, @"\d+$").Success) continue;
                    int buttonType = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{node.Text}/buttonType").GetValueEx<Int32>(1);
                    var btnBook = new ACtrlButton();
                    btnBook.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/regionSelect/list/button/{buttonType}/normal/0"), PluginBase.PluginManager.FindWz);
                    btnBook.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/regionSelect/list/button/{buttonType}/pressed/0"), PluginBase.PluginManager.FindWz);
                    btnBook.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/regionSelect/list/button/{buttonType}/mouseOver/0"), PluginBase.PluginManager.FindWz);
                    btnBook.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/regionSelect/list/button/{buttonType}/disabled/0"), PluginBase.PluginManager.FindWz);
                    btnBook.Size = new Size(133, 185);
                    btnBook.Location = new Point(26 + 146 * (i % 4), 2 + 185 * (i / 4));
                    btnBook.Visible = bookPage && 2 + 185 * (i / 4) >= 2;
                    btnBook.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
                    btnBook.MouseClick += new MouseEventHandler(btnBook_MouseClick);
                    btnBooks.Add(btnBook);
                    books.Add(node.Text);
                    i++;
                }
                this.vScroll.Maximum = i / 4 - 1;
            }
            else
            {
                for (int i = 0; i < btnBooks.Count; i++)
                {
                    int y = 2 + 185 * (i / 4 - scrollValue);
                    btnBooks[i].Visible = bookPage && y >= 2;
                    btnBooks[i].Location = new Point(26 + 146 * (i % 4), y);
                }
            }

            foreach (var title in Titles)
            {
                title.Visible = tablePage && extraPage;
            }
            if (selectedIndex != lastLoadedNpcIndex && selectedIndex > -1)
            {
                lastLoadedNpcIndex = selectedIndex;
                int i = 0;
                btnNpcs.Clear();
                npcs.Clear();
                var Npcs = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/npc")?.Nodes;
                foreach (Wz_Node node in Npcs)
                {
                    string npcID = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/npc/{node.Text}/id/0").GetValueEx<string>(null);
                    var btnNpc = new ACtrlButton();
                    btnNpc.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/detail/list/slotSelected"), PluginBase.PluginManager.FindWz);
                    btnNpc.Size = new Size(54, 54);
                    btnNpc.Location = new Point(840, 111 + 55 * (i - scrollValue2));
                    btnNpc.Visible = (i - scrollValue2) >= 0 && (i - scrollValue2) <= 6 && detailPage;
                    btnNpc.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
                    btnNpc.MouseClick += new MouseEventHandler(btnNpc_MouseClick);
                    btnNpcs.Add(btnNpc);
                    npcs.Add(npcID);
                    npcNodeNames.Add(node.Text);
                    i++;
                }
                this.vScroll2.Maximum = npcs.Count > 7 ? npcs.Count - 7 : 0;
            }
            else
            {
                for (int i = 0; i < btnNpcs.Count; i++)
                {
                    btnNpcs[i].Location = new Point(840, 111 + 55 * (i - scrollValue2));
                    btnNpcs[i].Visible = (i - scrollValue2) >= 0 && (i - scrollValue2) <= 6 && selectedTab == 1 && detailPage;
                }
            }
            if (selectedIndex != lastLoadedMobIndex && selectedIndex > -1)
            {
                lastLoadedMobIndex = selectedIndex;
                btnMobs.Clear();
                mobs.Clear();
                int i = 0;
                var Mobs = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/mob")?.Nodes;
                foreach (Wz_Node node in Mobs)
                {
                    string MobId = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/mob/{node.Text}/id/0").GetValueEx<string>(null);
                    var btnMob = new ACtrlButton();
                    btnMob.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/detail/list/slotSelected"), PluginBase.PluginManager.FindWz);
                    btnMob.Size = new Size(54, 54);
                    btnMob.Location = new Point(840, 111 + 55 * (i - scrollValue2));
                    btnMob.Visible = (i - scrollValue2) >= 0 && (i - scrollValue2) <= 6 && selectedTab == 2 && detailPage;
                    btnMob.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
                    btnMob.MouseClick += new MouseEventHandler(btnMob_MouseClick);
                    btnMobs.Add(btnMob);
                    mobs.Add(MobId);
                    mobNodeNames.Add(node.Text);  // 存储节点名称
                    i++;
                }
                this.vScroll2.Maximum = mobs.Count > 7 ? mobs.Count - 7 : 0;
            }
            else
            {
                for (int i = 0; i < btnMobs.Count; i++)
                {
                    btnMobs[i].Location = new Point(840, 111 + 55 * (i - scrollValue2));
                    btnMobs[i].Visible = (i - scrollValue2) >= 0 && (i - scrollValue2) <= 6 && selectedTab == 2 && detailPage;
                }
            }
            if (selectedTitle != lastLoadedChapterIndex && selectedTitle > -1)
            {
                lastLoadedChapterIndex = selectedTitle;
                Chapters.Clear();
                int i = 0;
                foreach (Wz_Node node in PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}").Nodes)
                {
                    if (node.Text == "name") continue;
                    var btnChapter = new ACtrlButton();
                    btnChapter.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/button/normal/0"), PluginBase.PluginManager.FindWz);
                    btnChapter.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/button/pressed/0"), PluginBase.PluginManager.FindWz);
                    btnChapter.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/button/pressed/0"), PluginBase.PluginManager.FindWz);
                    btnChapter.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/button/disabled/0"), PluginBase.PluginManager.FindWz);
                    btnChapter.Size = new Size(267, 52);
                    btnChapter.Location = new Point(564, 108 + 60 * i);
                    btnChapter.Visible = tablePage2;
                    btnChapter.MouseClick += new MouseEventHandler(btnChapter_MouseClick);
                    Chapters.Add(btnChapter);
                    i++;
                }
            }
            else
            {
                for (int i = 0; i < Chapters.Count; i++)
                {
                    Chapters[i].Location = new Point(564, 108 + 60 * i);
                    Chapters[i].Visible = tablePage2;
                }
            }
        }

        private void render_base(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/worldSelect/backgrnd", 0, 0);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/worldSelect/layer:rewardBase", 652, 42);
            if (PluginManager.FindWz("Etc/worldArchive.img/collectionInfo/1/0") == null)
                render_bitmap(g, "UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_1/disabled/0", 126, 159);
            if (PluginManager.FindWz("Etc/worldArchive.img/collectionInfo/2/0") == null)
                render_bitmap(g, "UI/_Canvas/UIworldArchive.img/worldSelect/button:worldSelect_2/disabled/0", 607, 208);
            g.DrawString(maxcount.ToString(), GearGraphics.ItemDetailFont, GearGraphics.numBrush, new Point(698, 49));
            g.DrawString(maxcount.ToString(), GearGraphics.ItemDetailFont, GearGraphics.numBrush, new Point(748, 49));
            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_mapleWorld(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/regionSelect/main/backgrnd", 0, 0);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/regionSelect/main/layer:cover", 319, 32);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/regionSelect/main/layer:rewardBase", 652, 42);
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/regionSelect/main/layer:light", 319, 32);
            render_bitmap(g, $"UI/_Canvas/UIworldArchive.img/regionSelect/main/world/{bookArea}", 43, 69);
            string worldDesc = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/worldDesc").GetValueEx<string>(null).Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
            int picH = 320;
            GearGraphics.DrawPlainText(g, worldDesc, GearGraphics.ArchiveNameFont2, Color.FromArgb(86, 76, 63), 49, 280, ref picH, 12, WzComparerR2.Text.TextAlignment.Center);
            g.DrawString(maxcount.ToString(), GearGraphics.ItemDetailFont, GearGraphics.numBrush, new Point(698, 49));
            g.DrawString(maxcount.ToString(), GearGraphics.ItemDetailFont, GearGraphics.numBrush, new Point(748, 49));
            foreach (AControl mapleCtrl in this.mapleControls)
            {
                mapleCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_book(Graphics g)
        {
            g.TranslateTransform(BookRect.X, BookRect.Y);
            Bitmap buffer = new Bitmap(623, 466, PixelFormat.Format32bppArgb);
            using (Graphics gBuffer = Graphics.FromImage(buffer))
            {
                gBuffer.Clear(Color.Transparent);
                gBuffer.TranslateTransform(0, 0);
                render_bitmap(gBuffer, "UI/_Canvas/UIworldArchive.img/regionSelect/list/basePattern", 0, 0);
                render_bitmap(gBuffer, "UI/_Canvas/UIworldArchive.img/regionSelect/list/blockPattern", 0, 117);
                render_bitmap(gBuffer, "UI/_Canvas/UIworldArchive.img/regionSelect/list/blockPattern", 0, 302);
                foreach (AControl btn in this.bookControls)
                {
                    if (!btn.Visible) continue;
                    btn.Draw(gBuffer);
                }
            }
            Bitmap mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/regionSelect/list/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
            Bitmap final = ApplyMaskWithOffset(buffer,mask, 0, 0, 0, 0);
            g.DrawImage(final, new Point(0, 0));

            var nodes = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}").Nodes;
            int count = nodes.Count;
            int i = 0;
            foreach (Wz_Node node in nodes)
            {
                if (!Regex.Match(node.Text, @"\d+$").Success) continue;
                int buttonType = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{node.Text}/buttonType").GetValueEx<Int32>(1);
                if (2 + 185 * (i / 4 - scrollValue) > 0)
                {
                    if (PluginBase.PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{node.Text}/regionName_MultiLine") != null)
                    {
                        string regionName_MultiLine = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{node.Text}/regionName_MultiLine").GetValueEx<string>(null);
                        string[] lines = regionName_MultiLine.Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                        float lineHeight = GearGraphics.ArchiveNameFont2.GetHeight(g);
                        float line1Width = g.MeasureString(lines[0], GearGraphics.ArchiveNameFont2).Width;
                        float line2Width = g.MeasureString(lines[1], GearGraphics.ArchiveNameFont2).Width;
                        g.DrawString(lines[0], GearGraphics.ArchiveNameFont2, GearGraphics.ArchiveNameBrush, 26 + 146 * (i % 4) + (133 - line1Width) / 2, 2 + 185 * (i / 4 - scrollValue) + 120);
                        g.DrawString(lines[1], GearGraphics.ArchiveNameFont2, GearGraphics.ArchiveNameBrush, 26 + 146 * (i % 4) + (133 - line2Width) / 2, 2 + 185 * (i / 4 - scrollValue) + 120 + lineHeight);
                    }
                    else
                    {
                        string regionName = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{node.Text}/regionName").GetValueEx<string>(null);
                        float nameWidth = g.MeasureString(regionName, GearGraphics.ArchiveNameFont).Width;
                        g.DrawString(regionName, GearGraphics.ArchiveNameFont, GearGraphics.ArchiveNameBrush, 26 + 146 * (i % 4) + (133 - nameWidth) / 2, 2 + 185 * (i / 4 - scrollValue) + 124);
                    }
                }
                i++;
            }
            vScroll.Draw(g);
            g.ResetTransform();
        }

        private void render_detail(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            switch (selectedTab)
            {
                case 0:
                    render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/selected/0", 38, 0);
                    if (npcs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/1", 137, 10);
                    if (npcs.Count > 0 && mobs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/2", 236, 10);
                    if (npcs.Count == 0 && mobs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/2", 137, 10);
                    break;
                case 1:
                    render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/0", 38, 10);
                    render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/selected/1", 137, 0);
                    if (npcs.Count > 0 && mobs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/2", 236, 10);
                    break;
                case 2:
                    render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/0", 38, 10);
                    if (npcs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/normal/1", 137, 10);
                    if (npcs.Count > 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/selected/2", 236, 0);
                    if (npcs.Count == 0) render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/category/selected/2", 137, 0);
                    break;
                default: break;
            }
            render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/main/backgrnd", 0, 0);
            switch (selectedTab)
            {
                case 0:
                    Bitmap mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/main/mask_illust"), PluginBase.PluginManager.FindWz).Bitmap;
                    Bitmap image = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/detail/main/regionillust/{bookArea}/{books[selectedIndex]}"), PluginBase.PluginManager.FindWz).Bitmap;
                    Bitmap final = ApplyMaskWithOffset(image, mask, 76, 91, 72, 86);
                    g.DrawImage(final, 76, 91);
                    //render_bitmap(g, $"UI/_Canvas/UIworldArchive.img/detail/main/regionillust/{bookArea}/{books[selectedIndex]}", 76, 91);
                    string regionName = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/regionName").GetValueEx<string>(null);
                    string regionDesc = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/regionDesc").GetValueEx<string>(null)?.Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
                    g.DrawString(regionName, GearGraphics.ArchiveNameFont3, GearGraphics.RegionBrush, 235 - g.MeasureString(regionName, GearGraphics.ArchiveNameFont3).Width / 2, 456);
                    int totalLines = CalculateTextLines(regionDesc, GearGraphics.ArchiveNameFont, 265);
                    int totalTextHeight = totalLines * 20;
                    this.vScroll3.Maximum = totalTextHeight > 386 ? (totalTextHeight - 386) / 20 : 0;
                    this.vScroll3.Visible = totalTextHeight > 386;
                    Bitmap textBuffer = new Bitmap(275, 386, PixelFormat.Format32bppArgb);
                    using (Graphics gText = Graphics.FromImage(textBuffer))
                    {
                        gText.Clear(Color.Transparent);
                        gText.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                        int picH = -this.scrollValue3 * 20;
                        GearGraphics.DrawFormattedDesc(gText, regionDesc, 0, 265, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(gText, regionDesc, GearGraphics.ArchiveNameFont, Color.FromArgb(255, 130, 102), 0, 265, ref picH, 20);
                        foreach (AControl scroll in this.scrollControls)
                        {
                            scroll.Draw(gText);
                        }
                    }
                    mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/desc/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
                    Bitmap finalText = ApplyMaskWithOffset(textBuffer, mask, 493, 113, 493, 113);
                    g.DrawImage(finalText, new Point(493, 113));
                    break;
                case 1: render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/list/backgrnd", 832, 84); 
                    string npcName = PluginManager.FindWz($@"String/Npc.img/{npcs[selectedIndex2]}/name").GetValueEx<string>(null);
                    string npcDesc = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/npc/{npcNodeNames[selectedIndex2]}/desc").GetValueEx<string>(null)?.Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
                    g.DrawString(npcName, GearGraphics.ArchiveNameFont3, GearGraphics.RegionBrush, 235 - g.MeasureString(npcName, GearGraphics.ArchiveNameFont3).Width / 2, 456);
                    totalLines = CalculateTextLines(npcDesc, GearGraphics.ArchiveNameFont, 265);
                    totalTextHeight = totalLines * 20;
                    this.vScroll3.Maximum = totalTextHeight > 386 ? (totalTextHeight - 386) / 20 : 0;
                    this.vScroll3.Visible = totalTextHeight > 386;
                    textBuffer = new Bitmap(275, 386, PixelFormat.Format32bppArgb);
                    using (Graphics gText = Graphics.FromImage(textBuffer))
                    {
                        gText.Clear(Color.Transparent);
                        gText.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                        int picH = - this.scrollValue3 * 20;
                        GearGraphics.DrawFormattedDesc(gText, npcDesc, 0, 265, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(gText, npcDesc, GearGraphics.ArchiveNameFont, Color.FromArgb(255, 130, 102), 0, 265, ref picH, 20);
                        vScroll3.Draw(gText);
                    }
                    mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/desc/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
                    finalText = ApplyMaskWithOffset(textBuffer, mask, 493, 113, 493, 113);
                    g.DrawImage(finalText, new Point(493, 113));
                    Wz_Node npcNode = PluginManager.FindWz($@"Npc/{npcs[selectedIndex2].PadLeft(7, '0')}.img");
                    if (npcNode != null)
                    {
                        Bitmap npcImage = imageNpcs.Contains(npcs[selectedIndex2]) ? BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/image/npc/{npcs[selectedIndex2]}"), PluginBase.PluginManager.FindWz).Bitmap :
                            Npc.CreateFromNode(npcNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz).Default.Bitmap;
                        int scale = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/npc/{npcNodeNames[selectedIndex2]}/scale").GetValueEx<Int32>(100);
                        int x = npcImage.Width * scale / 100;
                        int y = npcImage.Height * scale / 100;
                        if (x > 328 || y > 330)
                        {
                            double scaleRatio = x > y ? 328.0 / x: 330.0 / y;
                            x = (int)(x * scaleRatio);
                            y = (int)(y * scaleRatio);
                            Bitmap scaledImage = new Bitmap(x, y);
                            using (Graphics g2 = Graphics.FromImage(scaledImage))
                            {
                                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g2.DrawImage(npcImage, 0, 0, x, y);
                            }
                            npcImage = scaledImage;
                        }
                        g.DrawImage(npcImage, 236 - x/2, 251 - y/2, x, y);
                    }
                    textBuffer = new Bitmap(65, 388, PixelFormat.Format32bppArgb);
                    using (Graphics list = Graphics.FromImage(textBuffer))
                    {
                        foreach (string npcID in npcs)
                        {
                            int i = npcs.IndexOf(npcID);
                            if ((i - scrollValue2) >= 0 && (i - scrollValue2) <= 6) render_bitmap(list, "UI/_Canvas/UIworldArchive.img/detail/list/slotBase", 12, 1 + 55 * (i - scrollValue2));
                            npcNode = PluginManager.FindWz($@"Npc/{npcID.PadLeft(7, '0')}.img");
                            if (npcNode == null) continue;
                            Bitmap npcImage = imageNpcs.Contains(npcID) ? BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/image/npc/{npcID}"), PluginBase.PluginManager.FindWz).Bitmap :
                                Npc.CreateFromNode(npcNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz).Default.Bitmap;
                            if ((i - scrollValue2) >= 0 && (i - scrollValue2) <= 6)
                            {
                                int x = npcImage.Width > npcImage.Height ? 46 : npcImage.Width * 46 / npcImage.Height;
                                int y = npcImage.Width > npcImage.Height ? npcImage.Height * 46 / npcImage.Width : 46;
                                list.DrawImage(npcImage, 35 - x / 2, 24 - y / 2 + 55 * (i - scrollValue2), x, y);
                            }
                        }
                        vScroll2.Draw(list);
                    }
                    mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/desc/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
                    finalText = ApplyMaskWithOffset(textBuffer, mask, 832, 113, 832, 113);
                    g.DrawImage(finalText, new Point(832, 113));
                    break;
                case 2: render_bitmap(g, "UI/_Canvas/UIworldArchive.img/detail/list/backgrnd", 832, 84);
                    string mobName = PluginManager.FindWz($@"String/Mob.img/{mobs[selectedIndex3]}/name").GetValueEx<string>(null);
                    string mobDesc = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/mob/{mobNodeNames[selectedIndex3]}/desc").GetValueEx<string>(null)?.Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
                    g.DrawString(mobName, GearGraphics.ArchiveNameFont3, GearGraphics.RegionBrush, 235 - g.MeasureString(mobName, GearGraphics.ArchiveNameFont3).Width / 2, 456);
                    totalLines = CalculateTextLines(mobDesc, GearGraphics.ArchiveNameFont, 265);
                    totalTextHeight = totalLines * 20;
                    this.vScroll3.Maximum = totalTextHeight > 386 ? (totalTextHeight - 386) / 20 : 0;
                    this.vScroll3.Visible = totalTextHeight > 386;
                    textBuffer = new Bitmap(275, 386, PixelFormat.Format32bppArgb);
                    using (Graphics gText = Graphics.FromImage(textBuffer))
                    {
                        gText.Clear(Color.Transparent);
                        gText.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                        int picH = -this.scrollValue3 * 20;
                        GearGraphics.DrawFormattedDesc(gText, mobDesc, 0, 265, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(gText, mobDesc, GearGraphics.ArchiveNameFont, Color.FromArgb(255, 130, 102), 0, 265, ref picH, 20);
                        vScroll3.Draw(gText);
                    }
                    mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/desc/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
                    finalText = ApplyMaskWithOffset(textBuffer, mask, 493, 113, 493, 113);
                    g.DrawImage(finalText, new Point(493, 113));
                    var mobNode = PluginManager.FindWz($@"Mob/{mobs[selectedIndex3].PadLeft(7, '0')}.img");
                    if (mobNode != null)
                    {
                        Bitmap mobImage = imageMobs.Contains(mobs[selectedIndex3]) ? BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/image/mob/{mobs[selectedIndex3]}"), PluginBase.PluginManager.FindWz).Bitmap:
                            Mob.CreateFromNode(mobNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz).Default.Bitmap;
                        int scale = PluginManager.FindWz($@"Etc/worldArchive.img/collectionInfo/{bookArea}/{books[selectedIndex]}/mob/{mobNodeNames[selectedIndex3]}/scale").GetValueEx<Int32>(100);
                        int x = mobImage.Width * scale / 100;
                        int y = mobImage.Height * scale / 100;
                        if (x > 328 || y > 330)
                        {
                            double scaleRatio = (double)x > y ? 328.0 / x : 330.0 / y;
                            x = (int)(x * scaleRatio);
                            y = (int)(y * scaleRatio);
                            Bitmap scaledImage = new Bitmap(x, y);
                            using (Graphics g2 = Graphics.FromImage(scaledImage))
                            {
                                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g2.DrawImage(mobImage, 0, 0, x, y);
                            }
                            mobImage = scaledImage;
                        }
                        g.DrawImage(mobImage, 236 - x / 2, 251 - y / 2, x, y);
                    }
                    textBuffer = new Bitmap(65, 388, PixelFormat.Format32bppArgb);
                    using (Graphics list = Graphics.FromImage(textBuffer))
                    {
                        foreach (string mobID in mobs)
                        {
                            int i = mobs.IndexOf(mobID);
                            if ((i - scrollValue2) >= 0 && (i - scrollValue2) <= 6) render_bitmap(list, "UI/_Canvas/UIworldArchive.img/detail/list/slotBase", 12, 1 + 55 * (i - scrollValue2));
                            mobNode = PluginManager.FindWz($@"Mob/{mobID.PadLeft(7, '0')}.img");
                            if (mobNode == null) continue;
                            Bitmap mobImage = imageMobs.Contains(mobID) ? BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/image/mob/{mobID}"), PluginBase.PluginManager.FindWz).Bitmap :
                                Mob.CreateFromNode(mobNode, PluginBase.PluginManager.FindWz, PluginBase.PluginManager.FindWz).Default.Bitmap;
                            if ((i - scrollValue2) >= 0 && (i - scrollValue2) <= 6)
                            {
                                int x = mobImage.Width > mobImage.Height ? 46 : mobImage.Width * 46 / mobImage.Height;
                                int y = mobImage.Width > mobImage.Height ? mobImage.Height * 46 / mobImage.Width : 46;
                                list.DrawImage(mobImage, 35 - x / 2, 24 - y / 2 + 55 * (i - scrollValue2), x, y);
                            }
                        }
                        vScroll2.Draw(list);
                    }
                    mask = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIworldArchive.img/detail/desc/mask/shader/0/texture/1"), PluginBase.PluginManager.FindWz).Bitmap;
                    finalText = ApplyMaskWithOffset(textBuffer, mask, 832, 113, 832, 113);
                    g.DrawImage(finalText, new Point(832, 113));
                    break;
                default: break;
            }

            foreach (AControl detailCtrl in this.detailControls)
            {
                detailCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_extra(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/backgrnd", 0, 0);
            foreach (AControl extraCtrl in this.extraControls)
            {
                extraCtrl.Draw(g);
            }
            if (tablePage)
            {
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/layer:pageLeft", 140, 87);
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/layer:pageLeft", 528, 87);
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/title", 155, 219);
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/contents", 621, 114);
                for (int i = 0; i < 5; i++)
                {
                    if (i < Titles.Count)
                    {
                        string titleName = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{i}/name").GetValueEx<string>(null);
                        g.DrawString(titleName, GearGraphics.ArchiveNameFont3, GearGraphics.RegionBrush, 698 - g.MeasureString(titleName, GearGraphics.ArchiveNameFont3).Width / 2, 180 + 80 * i - g.MeasureString(titleName, GearGraphics.ArchiveNameFont3).Height / 2);
                        continue;
                    }
                    render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/title/button/vacant", 562, 144 + 80 * i);
                }
            }
            else if (tablePage2)
            {
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/layer:pageLeft", 140, 87);
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/layer:pageLeft", 528, 87);
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/backgrnd", 891, 180);
                render_bitmap(g, $"UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/title/{selectedTitle}", 155, 219);
                for (int i = 0; i < 7; i++)
                {
                    if (i < Chapters.Count)
                    {
                        string titleName = (i+1).ToString() + ". " + PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{i + 1}/name").GetValueEx<string>(null);
                        g.DrawString(titleName, GearGraphics.ArchiveNameFont, GearGraphics.RegionBrush, 698 - g.MeasureString(titleName, GearGraphics.ArchiveNameFont).Width / 2, 134 + 60 * i - g.MeasureString(titleName, GearGraphics.ArchiveNameFont).Height / 2);
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/slotBase", 911, 205 + 45 * i);
                        continue;
                    }
                    render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapter/button/vacant", 564, 108 + 60 * i);
                }
            }
            else if (detailInfo)
            {
                if (PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/disable") == null)
                {
                    //左页
                    int leftIndex = detailpage * 2;
                    string str = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{leftIndex}/str")?.GetValueEx<string>(null).Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
                    string url = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{leftIndex}/url").GetValueEx<string>(null);
                    string title = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{leftIndex}/title").GetValueEx<string>(null);
                    if (url != null)
                    {
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/layer:pageLeft", 140, 87);
                        render_bitmap(g, url, 138, 186);
                    }
                    if (str != null)
                    {
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/canvas:pageText", 136, 79);
                        int picH = 100;
                        GearGraphics.DrawFormattedDesc(g, str, 168, 432, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(g, str, GearGraphics.ArchiveNameFont, Color.FromArgb(86, 76, 63), 168, 432, ref picH, 20);
                    }
                    //右页
                    int rightIndex = 1 + detailpage * 2;
                    str = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{rightIndex}/str")?.GetValueEx<string>(null).Replace("\\r\\n", "\r\n").Replace("\\n", "\n");
                    url = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{rightIndex}/url").GetValueEx<string>(null);
                    title = PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/{rightIndex}/title").GetValueEx<string>(null);
                    if (title != null && str !=null)
                    {
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/canvas:pageText", 546, 79);
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/canvas:pageTitle", 575, 102);
                        g.DrawString(title, GearGraphics.ArchiveNameFont3, GearGraphics.RegionBrush, 708 - g.MeasureString(title, GearGraphics.ArchiveNameFont3).Width / 2, 129 - g.MeasureString(title, GearGraphics.ArchiveNameFont3).Height / 2);
                        int picH = 160;
                        GearGraphics.DrawFormattedDesc(g, str, 578, 832, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(g, str, GearGraphics.ArchiveNameFont, Color.FromArgb(86, 76, 63), 578, 842, ref picH, 20);
                    }
                    else if (str != null)
                    {
                        render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/canvas:pageText", 546, 79);
                        int picH = 100;
                        GearGraphics.DrawFormattedDesc(g, str, 578, 842, ref picH, 20, Color.FromArgb(86, 76, 63));
                        //GearGraphics.DrawPlainText(g, str, GearGraphics.ArchiveNameFont, Color.FromArgb(86, 76, 63), 578, 842, ref picH, 20);
                    }
                }
                render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/backgrnd", 891, 180);
                for (int i = 0; i < Chapters.Count; i++)
                {
                    render_bitmap(g, "UI/_Canvas/UIWorldArchiveBonusBook.img/main/chapterList/slotBase", 911, 205 + 45 * i);
                }
            }
            g.ResetTransform();
        }

        private void render_illust(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            maxpage = PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/illust/npc/{npcs[selectedIndex2].PadLeft(7, '0')}").Nodes.Count;
            if (maxpage > 0)
                render_bitmap(g, $"UI/_Canvas/UIworldArchive.img/illust/npc/{npcs[selectedIndex2].PadLeft(7, '0')}/{page}", 0, 0);
            else
                render_bitmap(g, $"UI/_Canvas/UIworldArchive.img/illust/npc/{npcs[selectedIndex2].PadLeft(7, '0')}", 0, 0);

            foreach (AControl illustCtrl in this.illustControls)
            {
                illustCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void render_bitmap(Graphics g, string nodepath, int x, int y)
        {
            Wz_Node Node = PluginBase.PluginManager.FindWz(nodepath);
            Bitmap image = BitmapOrigin.CreateFromNode(Node, PluginBase.PluginManager.FindWz).Bitmap;
            g.DrawImage(image, x, y);
        }

        private int CalculateTextLines(string text, System.Drawing.Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            if (font == null || maxWidth <= 0) return 0;

            // 移除富文本标签以获得纯文本长度
            string plainText = text.Replace("#e", "#c").Replace("#n", "").Replace("#", "");

            int lineCount = 0;
            string[] paragraphs = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            // 使用更大的位图来避免 GDI+ 问题
            using (Bitmap bmp = new Bitmap(Math.Max(maxWidth, 100), 100))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                foreach (string paragraph in paragraphs)
                {
                    if (string.IsNullOrEmpty(paragraph))
                    {
                        lineCount++;
                        continue;
                    }

                    try
                    {
                        SizeF size = g.MeasureString(paragraph, font);
                        if (size.Width <= maxWidth)
                        {
                            lineCount++;
                        }
                        else
                        {
                            int charsFitted, linesFilled;
                            g.MeasureString(paragraph, font, new SizeF(maxWidth, float.MaxValue),
                                StringFormat.GenericDefault, out charsFitted, out linesFilled);
                            lineCount += Math.Max(1, linesFilled);
                        }
                    }
                    catch (ExternalException)
                    {
                        lineCount += Math.Max(1, (int)Math.Ceiling(paragraph.Length / 20.0));
                    }
                    catch (ArgumentException)
                    {
                        lineCount += Math.Max(1, (int)Math.Ceiling(paragraph.Length / 20.0));
                    }
                }
            }

            return lineCount;
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
            selectedIndex = -1;
            lastLoadedBookIndex = -1;
            lastLoadedNpcIndex = -1;
            lastLoadedMobIndex = -1;
            lastLoadedChapterIndex = -1;
            this.books.Clear();
            this.npcs.Clear();
            this.mobs.Clear();
            this.npcNodeNames.Clear();  // 新增
            this.mobNodeNames.Clear();  // 新增
        }

        private void btnClose2_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex = -1;
            lastLoadedNpcIndex = -1;
            lastLoadedMobIndex = -1;
            lastLoadedChapterIndex = -1;
            detailpage = 0;
            maxdetailpage = 0;
            Chapters.Clear();
            mainPage = true;
            bookPage = false;
            detailPage = false;
            extraPage = false;
            tablePage = false;
            tablePage2 = false;
            showIllust = false;
            detailInfo = false;
            this.Refresh();
        }

        private void btnClose3_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex = -1;
            lastLoadedBookIndex = -1;
            lastLoadedNpcIndex = -1;
            lastLoadedMobIndex = -1;
            lastLoadedChapterIndex = -1;
            selectedTab = 1;
            scrollValue = 0;
            mainPage = false;
            bookPage = true;
            detailPage = false;
            extraPage = false;
            showIllust = false;
            this.Refresh();
        }

        private void btnClose4_MouseClick(object sender, MouseEventArgs e)
        {
            selectedTab = 1;
            scrollValue = 0;
            mainPage = false;
            bookPage = false;
            detailPage = true;
            extraPage = false;
            showIllust = false;
            this.Refresh();
        }

        private void btnWorldSelect_MouseClick(object sender, MouseEventArgs e)
        {
            lastLoadedBookIndex = -1;
            selectedIndex = -1;
            mainPage = false;
            bookPage = true;
            bookArea = 0;
            detailPage = false;
            extraPage = false;
            showIllust = false;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        private void btnWorldSelect2_MouseClick(object sender, MouseEventArgs e)
        {
            lastLoadedBookIndex = -1;
            selectedIndex = -1;
            mainPage = false;
            bookPage = true;
            bookArea = 1;
            detailPage = false;
            extraPage = false;
            showIllust = false;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        private void btnWorldSelect3_MouseClick(object sender, MouseEventArgs e)
        {
            lastLoadedBookIndex = -1;
            selectedIndex = -1;
            mainPage = false;
            bookPage = true;
            bookArea = 2;
            detailPage = false;
            extraPage = false;
            showIllust = false;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        //private void btnWorldSelect_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (btnWorldSelect.State == ButtonState.MouseOver)
        //        this.btnWorldSelect.Location = new Point(354, 296);
        //    else
        //        this.btnWorldSelect.Location = new Point(372, 356);
        //    this.Refresh();
        //}

        private void btnBack_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex = -1;
            lastLoadedBookIndex = -1;
            lastLoadedNpcIndex = -1;
            lastLoadedMobIndex = -1; 
            mainPage = true;
            bookPage = false;
            detailPage = false;
            extraPage = false;
            tablePage = false;
            showIllust = false;
            this.Refresh();
        }

        private void btnSupplement_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex = -1;
            lastLoadedNpcIndex = -1;  // 重置NPC缓存
            lastLoadedMobIndex = -1;  // 重置怪物缓存
            mainPage = false;
            bookPage = false;
            detailPage = false;
            extraPage = true;
            tablePage = true;
            showIllust = false;
            this.Refresh();
        }

        private void btnBook_MouseClick(object sender, MouseEventArgs e)
        {
            // 检查是否在页面切换后的忽略时间内
            if ((DateTime.Now - lastPageSwitchTime).TotalMilliseconds < CLICK_IGNORE_MS)
                return;
            selectedIndex = btnBooks.IndexOf(sender as ACtrlButton);
            lastLoadedNpcIndex = -1;
            lastLoadedMobIndex = -1;
            selectedTab = 0;
            selectedIndex2 = 0;
            selectedIndex3 = 0;
            npcNodeNames.Clear();  // 新增
            mobNodeNames.Clear();  // 新增
            mainPage = false;
            bookPage = false;
            detailPage = true;
            extraPage = false;
            showIllust = false;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        private void Title_MouseClick(object sender, MouseEventArgs e)
        {
            selectedTitle = Titles.IndexOf(sender as ACtrlButton);
            lastLoadedChapterIndex = -1;
            extraPage = true;
            tablePage = false;
            tablePage2 = true;
            detailInfo = true;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        private void btnNpc_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex2 = btnNpcs.IndexOf(sender as ACtrlButton);
            this.vScroll3.Value = 0;
            this.Refresh();
        }

        private void btnMob_MouseClick(object sender, MouseEventArgs e)
        {
            selectedIndex3 = btnMobs.IndexOf(sender as ACtrlButton);
            this.vScroll3.Value = 0;
            this.Refresh();
        }

        private void btnRegion_MouseClick(object sender, MouseEventArgs e)
        {
            selectedTab = 0;
            scrollValue2 = 0;
            scrollValue3 = 0;
            selectedIndex2 = 0;
            selectedIndex3 = 0;
            this.Refresh();
        }

        private void btnCharacter_MouseClick(object sender, MouseEventArgs e)
        {
            selectedTab = 1;
            scrollValue2 = 0;
            scrollValue3 = 0;
            selectedIndex3 = 0;
            this.Refresh();
        }

        private void btnMonster_MouseClick(object sender, MouseEventArgs e)
        {
            selectedTab = 2;
            scrollValue2 = 0;
            scrollValue3 = 0;
            selectedIndex2 = 0;
            this.Refresh();
        }

        private void btnlistUp_MouseClick(object sender, MouseEventArgs e)
        {
            if (scrollValue2 > 0)
                scrollValue2--;
            this.waitForRefresh = true;
        }

        private void btnHidden_MouseClick(object sender, MouseEventArgs e)
        {
            this.showIllust = true;
            mainPage = false;
            bookPage = false;
            detailPage = false;
            extraPage = false;
            this.waitForRefresh = true;
        }

        private void btnlistDown_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectedTab == 1 && scrollValue2 < npcs.Count - 7)
                scrollValue2++;
            else if (selectedTab == 2 && scrollValue2 < mobs.Count - 7)
                scrollValue2++;
            this.waitForRefresh = true;
        }

        private void btnRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (page < PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/illust/npc/{npcs[selectedIndex2].PadLeft(7, '0')}").Nodes.Count)
                page++;
            else
                btnRight.Visible = false;
            this.Refresh();
        }

        private void btnLeft_MouseClick(object sender, MouseEventArgs e)
        {
            if (page > 0 && PluginBase.PluginManager.FindWz($"UI/_Canvas/UIworldArchive.img/illust/npc/{npcs[selectedIndex2].PadLeft(7, '0')}").Nodes.Count > 0)
                page--;
            else
                btnLeft.Visible = false;
            this.Refresh();
        }

        private void btnTitle_MouseClick(Object sender, MouseEventArgs e)
        {
            extraPage = true;
            tablePage = true;
            tablePage2 = false;
            detailInfo = false;
            lastLoadedChapterIndex = -1;
            detailpage = 0;
            maxdetailpage = 0;
            this.Refresh();
        }

        private void btnChapter_MouseClick(Object sender, MouseEventArgs e)
        {
            // 检查是否在页面切换后的忽略时间内
            if ((DateTime.Now - lastPageSwitchTime).TotalMilliseconds < CLICK_IGNORE_MS)
                return;
            selectedChapter = Chapters.IndexOf(sender as ACtrlButton) + 1;
            if (PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}/disable") == null)
            {
                maxdetailpage = (PluginManager.FindWz($"UI/UIWorldArchiveBonusBook.img/info/{selectedTitle}/{selectedChapter}").Nodes.Count - 1) / 2 - 1;
            }
            else
                maxdetailpage = 0;
            extraPage = true;
            tablePage = false;
            tablePage2 = false;
            detailInfo = true;
            lastPageSwitchTime = DateTime.Now; // 记录页面切换时间
            this.Refresh();
        }

        private void btnChapterPrev_MouseClick(Object sender, MouseEventArgs e)
        {
            if (detailpage > 0)
                detailpage--;
            this.Refresh();
        }

        private void btnChapterNext_MouseClick(Object sender, MouseEventArgs e)
        {
            if (detailpage < maxdetailpage)
                detailpage++;
            this.Refresh();
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

        private void vScroll3_ValueChanged(object sender, EventArgs e)
        {
            this.scrollValue3 = this.vScroll3.Value;
            this.waitForRefresh = true;
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private Bitmap ApplyMaskWithOffset(Bitmap image, Bitmap mask, int imageX, int imageY, int maskX, int maskY)
        {
            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            var imgData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var resData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Rectangle maskRect = new Rectangle(0, 0, mask.Width, mask.Height);
            var maskData = mask.LockBits(maskRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int imgStride = imgData.Stride;
            int maskStride = maskData.Stride;

            int offsetX = imageX - maskX;
            int offsetY = imageY - maskY;
            unsafe
            {
                byte* imgPtr = (byte*)imgData.Scan0;
                byte* maskPtr = (byte*)maskData.Scan0;
                byte* resPtr = (byte*)resData.Scan0;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int imgIndex = y * imgStride + x * 4;

                        byte b = imgPtr[imgIndex];
                        byte g = imgPtr[imgIndex + 1];
                        byte r = imgPtr[imgIndex + 2];
                        byte a = imgPtr[imgIndex + 3];

                        int mx = x + offsetX;
                        int my = y + offsetY;

                        byte maskA = 0;
                        if (mx >= 0 && mx < mask.Width && my >= 0 && my < mask.Height)// 判断是否在 mask 范围内
                        {
                            int maskIndex = my * maskStride + mx * 4;
                            maskA = maskPtr[maskIndex + 3];
                        }

                        byte newA = (byte)(a * maskA / 255);

                        resPtr[imgIndex] = b;
                        resPtr[imgIndex + 1] = g;
                        resPtr[imgIndex + 2] = r;
                        resPtr[imgIndex + 3] = newA;
                    }
                }
            }
            image.UnlockBits(imgData);
            mask.UnlockBits(maskData);
            result.UnlockBits(resData);
            return result;
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return btnClose;
                yield return btnReward;
                yield return btnHelp;
                yield return btnWorldSelect;
                //yield return btnWorldSelect2;
                yield return btnWorldSelect3;
                yield return btnSupplement;
            }
        }

        private IEnumerable<AControl> mapleControls
        {
            get
            {
                yield return btnClose;
                yield return btnReward;
                yield return btnHelp;
                yield return btnBack;
            }
        }

        private IEnumerable<AControl> detailControls
        {
            get
            {
                yield return btnClose3;
                yield return btnRegion;
                if (npcs.Count > 0) yield return btnCharacter;
                if (mobs.Count > 0) yield return btnMonster;
                yield return btnlistUp;
                yield return btnlistDown;
                yield return btnHidden;
                if (selectedTab == 1)
                    foreach (var btnnpc in btnNpcs)
                        yield return btnnpc;
                else if (selectedTab == 2)
                    foreach (var btnmob in btnMobs)
                        yield return btnmob;

            }
        }

        private IEnumerable<AControl> scrollControls
        {
            get
            {
                yield return vScroll;
                yield return vScroll2;
                yield return vScroll3;
            }
        }

        private IEnumerable<AControl> extraControls
        {
            get
            {
                yield return btnClose2;
                yield return btnBookMark;
                yield return btnBookMark2;
                yield return btnBookMark3;
                if (tablePage)
                    foreach (var title in Titles)
                        yield return title;
                if (tablePage2)
                {
                    foreach (var Chapter in Chapters)
                        yield return Chapter;
                }
                if (tablePage2 || detailInfo)
                    yield return btnTitle;
                if (detailInfo)
                {
                    yield return btnChapterPrev;
                    yield return btnChapterNext;
                }
            }
        }

        private IEnumerable<AControl> illustControls
        {
            get
            {
                yield return btnClose4;
                yield return btnRight;
                yield return btnLeft;
            }
        }

        private IEnumerable<AControl> bookControls
        {
            get
            {
                if (bookPage)
                    foreach (var btn in btnBooks)
                        yield return btn;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseEventArgs childArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - baseOffset.X, e.Y - baseOffset.Y, e.Delta);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.mapleControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.detailControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.scrollControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.extraControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            foreach (AControl ctrl in this.illustControls)
            {
                ctrl.OnMouseMove(childArgs);
            }

            if (bookPage)
            {
                MouseEventArgs bookArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BookRect.X, e.Y - BookRect.Y, e.Delta);

                foreach (AControl ctrl in this.bookControls)
                {
                    ctrl.OnMouseMove(bookArgs);
                }
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

            foreach (AControl ctrl in this.mapleControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            foreach (AControl ctrl in this.detailControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            foreach (AControl ctrl in this.scrollControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            foreach (AControl ctrl in this.extraControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            foreach (AControl ctrl in this.illustControls)
            {
                ctrl.OnMouseDown(childArgs);
            }

            if (bookPage)
            {
                MouseEventArgs bookArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BookRect.X, e.Y - BookRect.Y, e.Delta);

                foreach (AControl ctrl in this.bookControls)
                {
                    ctrl.OnMouseDown(bookArgs);
                }
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

            foreach (AControl ctrl in this.mapleControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            foreach (AControl ctrl in this.detailControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            foreach (AControl ctrl in this.scrollControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            foreach (AControl ctrl in this.extraControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            foreach (AControl ctrl in this.illustControls)
            {
                ctrl.OnMouseUp(childArgs);
            }

            if (bookPage)
            {
                MouseEventArgs bookArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BookRect.X, e.Y - BookRect.Y, e.Delta);

                foreach (AControl ctrl in this.bookControls)
                {
                    ctrl.OnMouseUp(bookArgs);
                }
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

            foreach (AControl ctrl in this.mapleControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            foreach (AControl ctrl in this.detailControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            foreach (AControl ctrl in this.scrollControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            foreach (AControl ctrl in this.extraControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            foreach (AControl ctrl in this.illustControls)
            {
                ctrl.OnMouseClick(childArgs);
            }

            if (bookPage)
            {
                MouseEventArgs bookArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BookRect.X, e.Y - BookRect.Y, e.Delta);

                foreach (AControl ctrl in this.bookControls)
                {
                    ctrl.OnMouseClick(bookArgs);
                }
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
                ctrl.OnMouseWheel(e);
            }

            foreach (AControl ctrl in this.mapleControls)
            {
                ctrl.OnMouseWheel(e);
            }  

            foreach (AControl ctrl in this.detailControls)
            {
                ctrl.OnMouseWheel(e);
            }

            foreach (AControl ctrl in this.scrollControls)
            {
                ctrl.OnMouseWheel(e);
            }

            foreach (AControl ctrl in this.extraControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            foreach (AControl ctrl in this.illustControls)
            {
                ctrl.OnMouseWheel(childArgs);
            }

            if (bookPage)
            {
                MouseEventArgs bookArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BookRect.X, e.Y - BookRect.Y, e.Delta);

                foreach (AControl ctrl in this.bookControls)
                {
                    ctrl.OnMouseWheel(bookArgs);
                }
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
