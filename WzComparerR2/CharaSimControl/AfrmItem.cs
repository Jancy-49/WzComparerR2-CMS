using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using CharaSimResource;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmItem : AlphaForm
    {
        public AfrmItem()
        {
            this.AllowDrop = true;
            initCtrl();
        }

        private ItemTab[] itemTabs;
        private bool smallMode = true;
        private bool fullMode = false;
        private bool fullmaxMode = false;
        private bool slotLock = false;
        private bool slotLockFull = false;
        private bool slotLockFullMax = false;
        private bool itemLock = false;
        private bool itemLockFull = false;
        private bool itemLockFullMax = false;
        private bool filter = false;
        private bool selectedIndexChanging;
        private ACtrlVScroll vScroll;
        private Character character;

        private ACtrlButton btnFull;
        private ACtrlButton btnFullMax;
        private ACtrlButton btnSmall;
        private ACtrlButton btnSmallMax;
        private ACtrlButton btnCoin3;
        private ACtrlButton btnCoin4;
        private ACtrlButton btnPoint;
        private ACtrlButton btnGather;
        private ACtrlButton btnSort;
        private ACtrlButton btnSortFull;
        private ACtrlButton btnSortFullMax;
        private ACtrlButton btnSlotLock;
        private ACtrlButton btnSlotActive;
        private ACtrlButton btnSlotLockFull;
        private ACtrlButton btnSlotActiveFull;
        private ACtrlButton btnSlotLockFullMax;
        private ACtrlButton btnSlotActiveFullMax;
        private ACtrlButton btnItemLock;
        private ACtrlButton btnItemActive;
        private ACtrlButton btnItemLockFull;
        private ACtrlButton btnItemActiveFull;
        private ACtrlButton btnItemLockFullMax;
        private ACtrlButton btnItemActiveFullMax;
        private ACtrlButton btnDisassemble3;
        private ACtrlButton btnDisassemble4;
        private ACtrlButton btnExtract3;
        private ACtrlButton btnExtract4;
        private ACtrlButton btnAppraise3;
        private ACtrlButton btnAppraise4;
        private ACtrlButton btnBits3;
        private ACtrlButton btnBits4;
        private ACtrlButton btnPot3;
        private ACtrlButton btnPot4;
        private ACtrlButton btnUpgrade3;
        private ACtrlButton btnUpgrade4;
        private ACtrlButton btnToad3;
        private ACtrlButton btnToad4;
        private ACtrlButton btnCashshop;
        private ACtrlButton btnClose;
        private ACtrlButton btnHelp;
        private ACtrlButton btnEquip;
        private ACtrlButton btnShop;
        private ACtrlButton btnBag;
        private ACtrlButton btnFilter;
        private ACtrlButton btnFilterApply;
        private ACtrlButton btnFilterFull;
        private ACtrlButton btnFilterFullApply;
        private ACtrlButton btnFilterFullMax;
        private ACtrlButton btnFilterFullMaxApply;
        private ACtrlButton btnEquipFull;
        private ACtrlButton btnShopFull;
        private ACtrlButton btnBagFull;
        private ACtrlButton btnSearchFull;
        private bool waitForRefresh;

        public event ItemMouseEventHandler ItemMouseDown;
        public event ItemMouseEventHandler ItemMouseUp;
        public event ItemMouseEventHandler ItemMouseClick;
        public event ItemMouseEventHandler ItemMouseMove;
        public event EventHandler ItemMouseLeave;
        CharaSimControlGroup charaSimCtrl;
        public void updateitemTabs()
        {
            for (int i = 0; i < itemTabs.Length; i++)
            {
                if (this.fullmaxMode && this.itemTabs[5].Selected)
                {
                    int enable_x = -14;
                    int disabled_x = enable_x;
                    int enabale_y = -30 - 33 * i;
                    int disabled_y = enabale_y;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullMaxAutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullMaxAutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);                    
                }
                else if (this.fullMode || (this.fullmaxMode && !this.itemTabs[5].Selected))
                {
                    int enable_x = -11 - 130 * i;
                    int disabled_x = enable_x;
                    int enabale_y = -31;
                    int disabled_y = -31;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullAutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullAutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);
                }
                else if (this.smallMode)
                {
                    int enable_x = -11 - 37 * i;
                    int disabled_x = enable_x;
                    int enabale_y = -31;
                    int disabled_y = -31;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_AutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_AutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);
                }
            }
        }

        public Character Character
        {
            get { return character; }
            set
            {
                character = value;

                for (int i = 0; i < this.itemTabs.Length; i++)
                {
                    if (this.character == null || !this.itemTabs[i].SetItemSource(character.ItemSlots[i]))
                    {
                        this.itemTabs[i].ClearItems();
                    }
                }
            }
        }

        private void initCtrl()
        {
            charaSimCtrl = new CharaSimControlGroup();
            this.itemTabs = new ItemTab[6];
            for (int i = 0; i < itemTabs.Length; i++)
            {
                this.itemTabs[i] = new ItemTab(this);
                if (this.fullmaxMode && this.itemTabs[5].Selected)
                {
                    int enable_x = -14;
                    int disabled_x = enable_x;
                    int enabale_y = -30 - 33 * i;
                    int disabled_y = enabale_y;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullMaxAutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullMaxAutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);
                }
                else if (this.fullMode || (this.fullmaxMode && !this.itemTabs[5].Selected))
                {
                    int enable_x = -11 - 130 * i;
                    int disabled_x = enable_x;
                    int enabale_y = -31;
                    int disabled_y = -31;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullAutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_FullAutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);
                }
                else if (this.smallMode)
                {
                    int enable_x = -11 - 37 * i;
                    int disabled_x = enable_x;
                    int enabale_y = -31;
                    int disabled_y = -31;
                    this.itemTabs[i].TabEnabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_AutoBuild_tabcategory_selected_" + i), enable_x, enabale_y);
                    this.itemTabs[i].TabDisabled = new BitmapOrigin((Bitmap)Resource.ResourceManager.GetObject("UIInventory_img_Inventory_AutoBuild_tabcategory_normal_" + i), enable_x, disabled_y);
                }
            }
            this.itemTabs[0].Selected = true;
            this.vScroll = new ACtrlVScroll();  //小屏鼠标滑轮区域

            this.vScroll.PicBase.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_base);
            this.vScroll.PicBase.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_base);

            this.vScroll.BtnPrev.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_prev0);
            this.vScroll.BtnPrev.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_prev1);
            this.vScroll.BtnPrev.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_prev2);
            this.vScroll.BtnPrev.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_prev0);
            this.vScroll.BtnPrev.Size = this.vScroll.BtnPrev.Normal.Bitmap.Size;
            this.vScroll.BtnPrev.Location = new Point(0, 0);

            this.vScroll.BtnNext.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_next0);
            this.vScroll.BtnNext.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_next1);
            this.vScroll.BtnNext.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_next2);
            this.vScroll.BtnNext.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_next0);
            this.vScroll.BtnNext.Size = this.vScroll.BtnNext.Normal.Bitmap.Size;
            this.vScroll.BtnNext.Location = new Point(0, 368);

            this.vScroll.BtnThumb.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_thumb0);
            this.vScroll.BtnThumb.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_thumb1);
            this.vScroll.BtnThumb.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_scrollslot_enabled_thumb2);
            this.vScroll.BtnThumb.Size = this.vScroll.BtnThumb.Normal.Bitmap.Size;

            this.vScroll.Location = new Point(214, 99);  
            this.vScroll.Size = new Size(5, 368);
            this.vScroll.ScrollableLocation = new Point(5, 51);
            this.vScroll.ScrollableSize = new Size(153, 244);
            this.vScroll.ValueChanged += new EventHandler(vScroll_ValueChanged);
            this.vScroll.ChildButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnFull = new ACtrlButton();  //小屏转大屏
            this.btnFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_normal_0);
            this.btnFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_pressed_0);
            this.btnFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_mouseOver_0);
            this.btnFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_disabled_0);
            this.btnFull.Location = new Point(193, 8);
            this.btnFull.Size = new Size(19, 19);
            this.btnFull.MouseClick += new MouseEventHandler(btnFull_MouseClick);
            this.btnFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnFullMax = new ACtrlButton();  //大屏转最大屏
            this.btnFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfullMax_avatar_normal_0);
            this.btnFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfullMax_avatar_pressed_0);
            this.btnFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfullMax_avatar_mouseOver_0);
            this.btnFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfullMax_avatar_disabled_0);
            this.btnFullMax.Location = new Point(357, 474);
            this.btnFullMax.Size = new Size(80, 17);
            this.btnFullMax.MouseClick += new MouseEventHandler(btnFullMax_MouseClick);
            this.btnFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnSmall = new ACtrlButton();  //大屏转小屏
            this.btnSmall.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmin_normal_0);
            this.btnSmall.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmin_pressed_0);
            this.btnSmall.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmin_mouseOver_0);
            this.btnSmall.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmin_disabled_0);
            this.btnSmall.Location = new Point(171, 8);
            this.btnSmall.Size = new Size(19, 19);
            this.btnSmall.MouseClick += new MouseEventHandler(btnSmall_MouseClick);
            this.btnSmall.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnSmallMax = new ACtrlButton();  //最大屏转大屏
            this.btnSmallMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttoncontract_avatar_normal_0);
            this.btnSmallMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttoncontract_avatar_pressed_0);
            this.btnSmallMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttoncontract_avatar_mouseOver_0);
            this.btnSmallMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttoncontract_avatar_disabled_0);
            this.btnSmallMax.Location = new Point(452, 758);
            this.btnSmallMax.Size = new Size(80, 9);
            this.btnSmallMax.MouseClick += new MouseEventHandler(btnSmallMax_MouseClick);
            this.btnSmallMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnCoin3 = new ACtrlButton();  //小屏金币
            this.btnCoin3.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_normal_0);
            this.btnCoin3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_pressed_0);
            this.btnCoin3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_mouseOver_0);
            this.btnCoin3.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_normal_0);
            this.btnCoin3.Location = new Point(25, 485);
            this.btnCoin3.Size = new Size(40, 17);
            this.btnCoin3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnCoin4 = new ACtrlButton(); //全屏金币
            this.btnCoin4.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_normal_0);
            this.btnCoin4.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_pressed_0);
            this.btnCoin4.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_mouseOver_0);
            this.btnCoin4.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonmeso_normal_0);
            this.btnCoin4.Location = new Point(25, 485);
            this.btnCoin4.Size = new Size(40, 17);
            this.btnCoin4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnPoint = new ACtrlButton();  //小屏抵用券
            //this.btnPoint.Normal = new BitmapOrigin(Resource.Item_BtPoint_normal_0);
            //this.btnPoint.Pressed = new BitmapOrigin(Resource.Item_BtPoint_pressed_0);
            //this.btnPoint.MouseOver = new BitmapOrigin(Resource.Item_BtPoint_mouseOver_0);
            //this.btnPoint.Disabled = new BitmapOrigin(Resource.Item_BtPoint_disabled_0);
            //this.btnPoint.Location = new Point(8, 328);
            //this.btnPoint.Size = new Size(82, 17);
            //this.btnPoint.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnGather = new ACtrlButton();  //小屏整理
            //this.btnGather.Normal = new BitmapOrigin(Resource.Item_BtGather_normal_0);
            //this.btnGather.Pressed = new BitmapOrigin(Resource.Item_BtGather_pressed_0);
            //this.btnGather.MouseOver = new BitmapOrigin(Resource.Item_BtGather_mouseOver_0);
            //this.btnGather.Disabled = new BitmapOrigin(Resource.Item_BtGather_disabled_0);
            //this.btnGather.Location = new Point(153, 310);
            //this.btnGather.Size = new Size(16, 16);
            //this.btnGather.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnGather.MouseClick += new MouseEventHandler(btnGather_MouseClick);

            this.btnSort = new ACtrlButton();  //小屏排列
            this.btnSort.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsort_normal_0);
            this.btnSort.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsort_pressed_0);
            this.btnSort.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsort_mouseOver_0);
            this.btnSort.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsort_disabled_0);
            this.btnSort.Location = new Point(19, 66);
            this.btnSort.Size = new Size(22, 23);
            this.btnSort.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnSortFull = new ACtrlButton();  //大屏排列
            this.btnSortFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsort_normal_0);
            this.btnSortFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsort_normal_0);
            this.btnSortFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsort_normal_0);
            this.btnSortFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsort_normal_0);
            this.btnSortFull.Location = new Point(21, 67);
            this.btnSortFull.Size = new Size(68, 22);
            this.btnSortFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnSortFullMax = new ACtrlButton();  //最大屏排列
            this.btnSortFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonsort_normal_0);
            this.btnSortFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonsort_pressed_0);
            this.btnSortFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonsort_mouseOver_0);
            this.btnSortFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonsort_disabled_0);
            this.btnSortFullMax.Location = new Point(10, 639);
            this.btnSortFullMax.Size = new Size(97, 24);
            this.btnSortFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnSlotLock = new ACtrlButton();  //小屏栏位锁定
            this.btnSlotLock.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLock_normal_0);
            this.btnSlotLock.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLock_pressed_0);
            this.btnSlotLock.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLock_mouseOver_0);
            this.btnSlotLock.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLock_disabled_0);
            this.btnSlotLock.Location = new Point(43, 66);
            this.btnSlotLock.Size = new Size(22, 23);
            this.btnSlotLock.Visible = true;
            this.btnSlotLock.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotLock.MouseClick += new MouseEventHandler(btnSlotLock_MouseClick);

            this.btnSlotActive = new ACtrlButton();  //小屏栏位激活
            this.btnSlotActive.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLockActive_normal_0);
            this.btnSlotActive.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLockActive_pressed_0);
            this.btnSlotActive.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLockActive_mouseOver_0);
            this.btnSlotActive.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonslotLockActive_disabled_0);
            this.btnSlotActive.Location = new Point(43, 66);
            this.btnSlotActive.Size = new Size(22, 23);
            this.btnSlotActive.Visible = false;
            this.btnSlotActive.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotActive.MouseClick += new MouseEventHandler(btnSlotActive_MouseClick);

            this.btnSlotLockFull = new ACtrlButton();  //大屏栏位锁定
            this.btnSlotLockFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLock_normal_0);
            this.btnSlotLockFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLock_pressed_0);
            this.btnSlotLockFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLock_mouseOver_0);
            this.btnSlotLockFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLock_disabled_0);
            this.btnSlotLockFull.Location = new Point(95, 67);
            this.btnSlotLockFull.Size = new Size(68, 22);
            this.btnSlotLockFull.Visible = true;
            this.btnSlotLockFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotLockFull.MouseClick += new MouseEventHandler(btnSlotLockFull_MouseClick);

            this.btnSlotActiveFull = new ACtrlButton();  //大屏栏位激活
            this.btnSlotActiveFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLockActive_normal_0);
            this.btnSlotActiveFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLockActive_pressed_0);
            this.btnSlotActiveFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLockActive_mouseOver_0);
            this.btnSlotActiveFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonslotLockActive_disabled_0);
            this.btnSlotActiveFull.Location = new Point(95, 67);
            this.btnSlotActiveFull.Size = new Size(68, 22);
            this.btnSlotActiveFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotActiveFull.MouseClick += new MouseEventHandler(btnSlotActiveFull_MouseClick);

            this.btnSlotLockFullMax = new ACtrlButton();  //最大屏栏位固定
            this.btnSlotLockFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLock_normal_0);
            this.btnSlotLockFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLock_pressed_0);
            this.btnSlotLockFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLock_mouseOver_0);
            this.btnSlotLockFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLock_disabled_0);
            this.btnSlotLockFullMax.Location = new Point(10, 666);
            this.btnSlotLockFullMax.Size = new Size(97, 24);
            this.btnSlotLockFullMax.Visible = true;
            this.btnSlotLockFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotLockFullMax.MouseClick += new MouseEventHandler(btnSlotLockFullMax_MouseClick);

            this.btnSlotActiveFullMax = new ACtrlButton();  //最大屏栏位激活
            this.btnSlotActiveFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLockActive_normal_0);
            this.btnSlotActiveFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLockActive_pressed_0);
            this.btnSlotActiveFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLockActive_mouseOver_0);
            this.btnSlotActiveFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonslotLockActive_disabled_0);
            this.btnSlotActiveFullMax.Location = new Point(10, 666);
            this.btnSlotActiveFullMax.Size = new Size(97, 24);
            this.btnSlotActiveFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSlotActiveFullMax.MouseClick += new MouseEventHandler(btnSlotActiveFullMax_MouseClick);

            //this.btnItemLock = new ACtrlButton();  //小屏道具锁定
            //this.btnItemLock.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLock_normal_0);
            //this.btnItemLock.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLock_pressed_0);
            //this.btnItemLock.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLock_mouseOver_0);
            //this.btnItemLock.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLock_disabled_0);
            //this.btnItemLock.Location = new Point(67, 66);
            //this.btnItemLock.Size = new Size(22, 23);
            //this.btnItemLock.Visible = true;
            //this.btnItemLock.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemLock.MouseClick += new MouseEventHandler(btnItemLock_MouseClick);


            //this.btnItemActive = new ACtrlButton();  //小屏栏位锁定
            //this.btnItemActive.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLockActive_normal_0);
            //this.btnItemActive.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLockActive_pressed_0);
            //this.btnItemActive.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLockActive_mouseOver_0);
            //this.btnItemActive.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemLockActive_disabled_0);
            //this.btnItemActive.Location = new Point(67, 66);
            //this.btnItemActive.Size = new Size(22, 23);
            //this.btnItemActive.Visible = false;
            //this.btnItemActive.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemActive.MouseClick += new MouseEventHandler(btnItemActive_MouseClick);

            //this.btnItemLockFull = new ACtrlButton();  //大屏道具锁定
            //this.btnItemLockFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLock_normal_0);
            //this.btnItemLockFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLock_pressed_0);
            //this.btnItemLockFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLock_mouseOver_0);
            //this.btnItemLockFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLock_disabled_0);
            //this.btnItemLockFull.Location = new Point(169, 67);
            //this.btnItemLockFull.Size = new Size(68, 22);
            //this.btnItemLockFull.Visible = true;
            //this.btnItemLockFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemLockFull.MouseClick += new MouseEventHandler(btnItemLockFull_MouseClick);


            //this.btnItemActiveFull = new ACtrlButton();  //大屏栏位锁定
            //this.btnItemActiveFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLockActive_normal_0);
            //this.btnItemActiveFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLockActive_pressed_0);
            //this.btnItemActiveFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLockActive_mouseOver_0);
            //this.btnItemActiveFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemLockActive_disabled_0);
            //this.btnItemActiveFull.Location = new Point(169, 67);
            //this.btnItemActiveFull.Size = new Size(68, 22);
            //this.btnItemActiveFull.Visible = false;
            //this.btnItemActiveFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemActiveFull.MouseClick += new MouseEventHandler(btnItemActiveFull_MouseClick);

            //this.btnItemLockFullMax = new ACtrlButton();  //最大屏道具锁定
            //this.btnItemLockFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLock_normal_0);
            //this.btnItemLockFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLock_pressed_0);
            //this.btnItemLockFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLock_mouseOver_0);
            //this.btnItemLockFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLock_disabled_0);
            //this.btnItemLockFullMax.Location = new Point(10, 666);
            //this.btnItemLockFullMax.Size = new Size(97, 24);
            //this.btnItemLockFullMax.Visible = true;
            //this.btnItemLockFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemLockFullMax.MouseClick += new MouseEventHandler(btnItemLockFullMax_MouseClick);


            //this.btnItemActiveFullMax = new ACtrlButton();  //最大屏栏位锁定
            //this.btnItemActiveFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLockActive_normal_0);
            //this.btnItemActiveFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLockActive_pressed_0);
            //this.btnItemActiveFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLockActive_mouseOver_0);
            //this.btnItemActiveFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonitemLockActive_disabled_0);
            //this.btnItemActiveFullMax.Location = new Point(10, 666);
            //this.btnItemActiveFullMax.Size = new Size(97, 24);
            //this.btnItemActiveFullMax.Visible = false;
            //this.btnItemActiveFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            //this.btnItemActiveFullMax.MouseClick += new MouseEventHandler(btnItemActiveFullMax_MouseClick);

            //this.btnDisassemble3 = new ACtrlButton();  //小屏分解
            //this.btnDisassemble3.Normal = new BitmapOrigin(Resource.Item_BtDisassemble3_normal_0);
            //this.btnDisassemble3.Pressed = new BitmapOrigin(Resource.Item_BtDisassemble3_pressed_0);
            //this.btnDisassemble3.MouseOver = new BitmapOrigin(Resource.Item_BtDisassemble3_mouseOver_0);
            //this.btnDisassemble3.Disabled = new BitmapOrigin(Resource.Item_BtDisassemble3_disabled_0);
            //this.btnDisassemble3.Location = new Point(9, 347);
            //this.btnDisassemble3.Size = new Size(23, 23);
            //this.btnDisassemble3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnDisassemble4 = new ACtrlButton();  //大屏分解
            //this.btnDisassemble4.Normal = new BitmapOrigin(Resource.Item_BtDisassemble4_normal_0);
            //this.btnDisassemble4.Pressed = new BitmapOrigin(Resource.Item_BtDisassemble4_pressed_0);
            //this.btnDisassemble4.MouseOver = new BitmapOrigin(Resource.Item_BtDisassemble4_mouseOver_0);
            //this.btnDisassemble4.Disabled = new BitmapOrigin(Resource.Item_BtDisassemble4_disabled_0);
            //this.btnDisassemble4.Location = new Point(471, 391);
            //this.btnDisassemble4.Size = new Size(16, 16);
            //this.btnDisassemble4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnExtract3 = new ACtrlButton();  //小屏合成
            this.btnExtract3.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemAlchemy_normal_0);
            this.btnExtract3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemAlchemy_pressed_0);
            this.btnExtract3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemAlchemy_mouseOver_0);
            this.btnExtract3.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonitemAlchemy_disabled_0);
            this.btnExtract3.Location = new Point(145, 530);
            this.btnExtract3.Size = new Size(24, 25);
            this.btnExtract3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnExtract4 = new ACtrlButton();  //大屏合成
            this.btnExtract4.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemAlchemy_normal_0);
            this.btnExtract4.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemAlchemy_pressed_0);
            this.btnExtract4.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemAlchemy_mouseOver_0);
            this.btnExtract4.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonitemAlchemy_disabled_0);
            this.btnExtract4.Location = new Point(496, 479);
            this.btnExtract4.Size = new Size(92, 27);
            this.btnExtract4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnAppraise3 = new ACtrlButton();  //小屏鉴定
            //this.btnAppraise3.Normal = new BitmapOrigin(Resource.Item_BtAppraise3_normal_0);
            //this.btnAppraise3.Pressed = new BitmapOrigin(Resource.Item_BtAppraise3_pressed_0);
            //this.btnAppraise3.MouseOver = new BitmapOrigin(Resource.Item_BtAppraise3_mouseOver_0);
            //this.btnAppraise3.Disabled = new BitmapOrigin(Resource.Item_BtAppraise3_disabled_0);
            //this.btnAppraise3.Location = new Point(61, 347);
            //this.btnAppraise3.Size = new Size(23, 23);
            //this.btnAppraise3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnAppraise4 = new ACtrlButton();  //大屏鉴定
            //this.btnAppraise4.Normal = new BitmapOrigin(Resource.Item_BtAppraise4_normal_0);
            //this.btnAppraise4.Pressed = new BitmapOrigin(Resource.Item_BtAppraise4_pressed_0);
            //this.btnAppraise4.MouseOver = new BitmapOrigin(Resource.Item_BtAppraise4_mouseOver_0);
            //this.btnAppraise4.Disabled = new BitmapOrigin(Resource.Item_BtAppraise4_disabled_0);
            //this.btnAppraise4.Location = new Point(507, 391);
            //this.btnAppraise4.Size = new Size(16, 16);
            //this.btnAppraise4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnBits3 = new ACtrlButton();
            this.btnBits3.Normal = new BitmapOrigin(Resource.Item_BtBits3_normal_0);
            this.btnBits3.Pressed = new BitmapOrigin(Resource.Item_BtBits3_pressed_0);
            this.btnBits3.MouseOver = new BitmapOrigin(Resource.Item_BtBits3_mouseOver_0);
            this.btnBits3.Disabled = new BitmapOrigin(Resource.Item_BtBits3_disabled_0);
            this.btnBits3.Location = new Point(113, 391);
            this.btnBits3.Size = new Size(23, 23);
            this.btnBits3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnBits4 = new ACtrlButton();
            this.btnBits4.Normal = new BitmapOrigin(Resource.Item_BtBits4_normal_0);
            this.btnBits4.Pressed = new BitmapOrigin(Resource.Item_BtBits4_pressed_0);
            this.btnBits4.MouseOver = new BitmapOrigin(Resource.Item_BtBits4_mouseOver_0);
            this.btnBits4.Disabled = new BitmapOrigin(Resource.Item_BtBits4_disabled_0);
            this.btnBits4.Location = new Point(484, 337);
            this.btnBits4.Size = new Size(16, 16);
            this.btnBits4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnPot3 = new ACtrlButton();
            this.btnPot3.Normal = new BitmapOrigin(Resource.Item_BtPot3_normal_0);
            this.btnPot3.Pressed = new BitmapOrigin(Resource.Item_BtPot3_pressed_0);
            this.btnPot3.MouseOver = new BitmapOrigin(Resource.Item_BtPot3_mouseOver_0);
            this.btnPot3.Disabled = new BitmapOrigin(Resource.Item_BtPot3_disabled_0);
            this.btnPot3.Location = new Point(87, 303);
            this.btnPot3.Size = new Size(23, 23);
            this.btnPot3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnPot4 = new ACtrlButton();
            this.btnPot4.Normal = new BitmapOrigin(Resource.Item_BtPot4_normal_0);
            this.btnPot4.Pressed = new BitmapOrigin(Resource.Item_BtPot4_pressed_0);
            this.btnPot4.MouseOver = new BitmapOrigin(Resource.Item_BtPot4_mouseOver_0);
            this.btnPot4.Disabled = new BitmapOrigin(Resource.Item_BtPot4_disabled_0);
            this.btnPot4.Location = new Point(466, 337);
            this.btnPot4.Size = new Size(16, 16);
            this.btnPot4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnUpgrade3 = new ACtrlButton();  //小屏强化
            this.btnUpgrade3.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonupgrade_normal_0);
            this.btnUpgrade3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonupgrade_pressed_0);
            this.btnUpgrade3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonupgrade_mouseOver_0);
            this.btnUpgrade3.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonupgrade_disabled_0);
            this.btnUpgrade3.Location = new Point(199, 530);
            this.btnUpgrade3.Size = new Size(24, 25);
            this.btnUpgrade3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnUpgrade4 = new ACtrlButton();  //大屏强化
            this.btnUpgrade4.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonupgrade_normal_0);
            this.btnUpgrade4.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonupgrade_pressed_0);
            this.btnUpgrade4.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonupgrade_mouseOver_0);
            this.btnUpgrade4.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonupgrade_disabled_0);
            this.btnUpgrade4.Location = new Point(688, 479);
            this.btnUpgrade4.Size = new Size(92, 27);
            this.btnUpgrade4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnToad3 = new ACtrlButton();  //小屏托德之锤
            this.btnToad3.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsuccession_normal_0);
            this.btnToad3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsuccession_pressed_0);
            this.btnToad3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsuccession_mouseOver_0);
            this.btnToad3.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonsuccession_disabled_0);
            this.btnToad3.Location = new Point(172, 530);
            this.btnToad3.Size = new Size(24, 25);
            this.btnToad3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnToad4 = new ACtrlButton();  //大屏托德之锤
            this.btnToad4.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsuccession_normal_0);
            this.btnToad4.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsuccession_pressed_0);
            this.btnToad4.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsuccession_mouseOver_0);
            this.btnToad4.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsuccession_disabled_0);
            this.btnToad4.Location = new Point(592, 479);
            this.btnToad4.Size = new Size(92, 27);
            this.btnToad4.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            //this.btnCashshop = new ACtrlButton();  //大屏扩展背包
            //this.btnCashshop.Normal = new BitmapOrigin(Resource.Item_BtCashshop_normal_0);
            //this.btnCashshop.Pressed = new BitmapOrigin(Resource.Item_BtCashshop_pressed_0);
            //this.btnCashshop.MouseOver = new BitmapOrigin(Resource.Item_BtCashshop_mouseOver_0);
            //this.btnCashshop.Disabled = new BitmapOrigin(Resource.Item_BtCashshop_disabled_0);
            //this.btnCashshop.Location = new Point(597, 391);
            //this.btnCashshop.Size = new Size(82, 16);
            //this.btnCashshop.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton();  //关闭按钮
            this.btnClose.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonclose_normal_0);
            this.btnClose.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonclose_pressed_0);
            this.btnClose.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonclose_mouseOver_0);
            this.btnClose.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonclose_disabled_0);
            this.btnClose.Size = new Size(19, 19);
            this.btnClose.Visible = true;
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);

            this.btnHelp = new ACtrlButton();  //帮助按钮
            this.btnHelp.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonhelp_normal_0);
            this.btnHelp.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonhelp_pressed_0);
            this.btnHelp.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonhelp_mouseOver_0);
            this.btnHelp.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonhelp_disabled_0);
            this.btnHelp.Size = new Size(36, 24);
            this.btnHelp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnEquip = new ACtrlButton();  //小屏装备
            this.btnEquip.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonequip_normal_0);
            this.btnEquip.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonequip_pressed_0);
            this.btnEquip.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonequip_mouseOver_0);
            this.btnEquip.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonequip_disabled_0);
            this.btnEquip.Location = new Point(155, 563);
            this.btnEquip.Size = new Size(24, 24);
            this.btnEquip.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquip.MouseClick += new MouseEventHandler(btnEquip_MouseClick);

            this.btnEquipFull = new ACtrlButton();  //大屏装备
            this.btnEquipFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonequip_normal_0);
            this.btnEquipFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonequip_pressed_0);
            this.btnEquipFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonequip_mouseOver_0);
            this.btnEquipFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonequip_disabled_0);
            this.btnEquipFull.Location = new Point(551, 515);
            this.btnEquipFull.Size = new Size(76, 24);
            this.btnEquipFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquipFull.MouseClick += new MouseEventHandler(btnEquip_MouseClick);

            this.btnShop = new ACtrlButton();  //小屏商店
            this.btnShop.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonshop_normal_0);
            this.btnShop.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonshop_pressed_0);
            this.btnShop.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonshop_mouseOver_0);
            this.btnShop.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonshop_disabled_0);
            this.btnShop.Location = new Point(181, 563);
            this.btnShop.Size = new Size(24, 24);
            this.btnShop.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnShopFull = new ACtrlButton();  //大屏商店
            this.btnShopFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonshop_normal_0);
            this.btnShopFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonshop_pressed_0);
            this.btnShopFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonshop_mouseOver_0);
            this.btnShopFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonshop_disabled_0);
            this.btnShopFull.Location = new Point(631, 515);
            this.btnShopFull.Size = new Size(76, 24);
            this.btnShopFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnBag = new ACtrlButton();  //小屏卡包
            this.btnBag.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonbag_normal_0);
            this.btnBag.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonbag_pressed_0);
            this.btnBag.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonbag_mouseOver_0);
            this.btnBag.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonbag_disabled_0);
            this.btnBag.Location = new Point(207, 563);
            this.btnBag.Size = new Size(24, 24);
            this.btnBag.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnBagFull = new ACtrlButton();  //大屏卡包
            this.btnBagFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonbag_normal_0);
            this.btnBagFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonbag_pressed_0);
            this.btnBagFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonbag_mouseOver_0);
            this.btnBagFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonbag_disabled_0);
            this.btnBagFull.Location = new Point(711, 515);
            this.btnBagFull.Size = new Size(76, 24);
            this.btnBagFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnFilter = new ACtrlButton();  //小屏筛选
            this.btnFilter.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilter_normal_0);
            this.btnFilter.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilter_pressed_0);
            this.btnFilter.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilter_mouseOver_0);
            this.btnFilter.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilter_disabled_0);
            this.btnFilter.Location = new Point(67, 66);
            this.btnFilter.Size = new Size(22, 23);
            this.btnFilter.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilter.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnFilterApply = new ACtrlButton();  //小屏筛选应用
            this.btnFilterApply.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilterApplied_normal_0);
            this.btnFilterApply.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilterApplied_pressed_0);
            this.btnFilterApply.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilterApplied_mouseOver_0);
            this.btnFilterApply.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_AutoBuild_buttonfilterApplied_disabled_0);
            this.btnFilterApply.Location = new Point(67, 66);
            this.btnFilterApply.Size = new Size(22, 23);
            this.btnFilterApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilterApply.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnFilterFull = new ACtrlButton();  //大屏筛选
            this.btnFilterFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilter_normal_0);
            this.btnFilterFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilter_pressed_0);
            this.btnFilterFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilter_mouseOver_0);
            this.btnFilterFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilter_disabled_0);
            this.btnFilterFull.Location = new Point(167, 67);
            this.btnFilterFull.Size = new Size(68, 22);
            this.btnFilterFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilterFull.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnFilterFullApply = new ACtrlButton();  //大屏筛选应用
            this.btnFilterFullApply.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilterApplied_normal_0);
            this.btnFilterFullApply.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilterApplied_pressed_0);
            this.btnFilterFullApply.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilterApplied_mouseOver_0);
            this.btnFilterFullApply.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonfilterApplied_disabled_0);
            this.btnFilterFullApply.Location = new Point(167, 67);
            this.btnFilterFullApply.Size = new Size(68, 22);
            this.btnFilterFullApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilterFullApply.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnFilterFullMax = new ACtrlButton();  //最大屏筛选
            this.btnFilterFullMax.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilter_normal_0);
            this.btnFilterFullMax.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilter_pressed_0);
            this.btnFilterFullMax.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilter_mouseOver_0);
            this.btnFilterFullMax.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilter_disabled_0);
            this.btnFilterFullMax.Location = new Point(10, 693);
            this.btnFilterFullMax.Size = new Size(97, 24);
            this.btnFilterFullMax.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilterFullMax.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnFilterFullMaxApply = new ACtrlButton();  //最大屏筛选应用
            this.btnFilterFullMaxApply.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilterApplied_normal_0);
            this.btnFilterFullMaxApply.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilterApplied_pressed_0);
            this.btnFilterFullMaxApply.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilterApplied_mouseOver_0);
            this.btnFilterFullMaxApply.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_buttonfilterApplied_disabled_0);
            this.btnFilterFullMaxApply.Location = new Point(10, 693);
            this.btnFilterFullMaxApply.Size = new Size(97, 24);
            this.btnFilterFullMaxApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFilterFullMaxApply.MouseClick += new MouseEventHandler(btnfilter_MouseClick);

            this.btnSearchFull = new ACtrlButton();  //大屏搜索
            this.btnSearchFull.Normal = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsearch_normal_0);
            this.btnSearchFull.Pressed = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsearch_pressed_0);
            this.btnSearchFull.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsearch_mouseOver_0);
            this.btnSearchFull.Disabled = new BitmapOrigin(Resource.UIInventory_img_Inventory_FullAutoBuild_buttonsearch_disabled_0);
            this.btnSearchFull.Size = new Size(16, 15);
            this.btnSearchFull.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(0, 0, this.Bitmap.Width, 24);
            base.Refresh();
        }

        protected override bool captionHitTest(Point point)
        {
            if (this.btnClose.Rectangle.Contains(point))
                return false;
            return base.captionHitTest(point);
        }

        private void preRender()
        {
            if (Bitmap != null)
                Bitmap.Dispose();

            if (this.fullmaxMode && this.itemTabs[5].Selected)
            {
                this.btnFull.Visible = false;
                this.btnFullMax.Visible = false;
                this.btnSmall.Visible = true;
                this.btnSmallMax.Visible = true;
                this.vScroll.Visible = false;
                this.btnCoin3.Visible = false;
                this.btnCoin4.Visible = false;
                this.btnSort.Visible = false;
                this.btnSortFull.Visible = false;
                this.btnSlotLock.Visible = false;
                this.btnSlotActive.Visible = false;
                this.btnSlotLockFull.Visible = false;
                this.btnSlotActiveFull.Visible = false;
                if (this.slotLockFullMax)
                {
                    this.btnSlotLockFullMax.Visible = false;
                    this.btnSlotActiveFullMax.Visible = true;
                }
                else
                {
                    this.btnSlotLockFullMax.Visible = true;
                    this.btnSlotActiveFullMax.Visible = false;
                }
                this.btnExtract3.Visible = false;
                this.btnExtract4.Visible = false;
                this.btnBits3.Visible = false;
                this.btnBits4.Visible = false;
                this.btnPot3.Visible = false;
                this.btnPot4.Visible = false;
                this.btnUpgrade3.Visible = false;
                this.btnUpgrade4.Visible = false;
                this.btnToad3.Visible = false;
                this.btnToad4.Visible = false;
                this.btnHelp.Visible = false;
                this.btnEquip.Visible = false;
                this.btnShop.Visible = false;
                this.btnBag.Visible = false;
                this.btnFilter.Visible = false;
                this.btnFilterApply.Visible = false;
                this.btnEquipFull.Visible = false;
                this.btnShopFull.Visible = false;
                this.btnBagFull.Visible = false;
                if (this.filter)
                {
                    this.btnFilterFullMaxApply.Visible = true;
                    this.btnFilterFullMax.Visible = false;
                }
                else
                {
                    this.btnFilterFullMaxApply.Visible = false;
                    this.btnFilterFullMax.Visible = true;
                }
                this.btnFilter.Visible = false;
                this.btnFilterApply.Visible = false;
                this.btnFilterFull.Visible = false;
                this.btnFilterFullApply.Visible = false;
                this.btnSearchFull.Visible = true;
                this.btnClose.Location = new Point(871, 4);
                this.btnSmall.Location = new Point(871, 48);
                this.btnSearchFull.Location = new Point(83, 735);
                renderFullMax();
            }
            else if (this.fullMode || (this.fullmaxMode && !this.itemTabs[5].Selected))
            {
                if (this.itemTabs[5].Selected && !filter)
                {
                    this.btnFilterFull.Visible = true;
                    this.btnFilterFullApply.Visible = false;
                    this.btnFullMax.Visible = true;
                }
                else if (this.itemTabs[5].Selected && filter)
                {
                    this.btnFilterFull.Visible = false;
                    this.btnFilterFullApply.Visible = true;
                    this.btnFullMax.Visible = true;
                }
                else if (!this.itemTabs[5].Selected)
                {
                    this.btnFilterFull.Visible = false;
                    this.btnFilterFullApply.Visible = false;
                    this.btnFullMax.Visible = false;
                }
                this.btnFull.Visible = false;
                this.btnSmall.Visible = true;
                this.btnSmallMax.Visible = false;
                this.vScroll.Visible = false;
                this.btnCoin3.Visible = false;
                this.btnCoin4.Visible = true;
                //this.btnPoint.Location = new Point(229, 391);
                //this.btnGather.Location = new Point(154, 391);
                this.btnSort.Visible = false;
                this.btnSortFull.Visible = true;
                this.btnSlotLock.Visible = false;
                this.btnSlotActive.Visible = false;
                this.btnSlotLockFullMax.Visible = false;
                this.btnSlotActiveFullMax.Visible = false;
                if (this.slotLockFull)
                {
                    this.btnSlotLockFull.Visible = false;
                    this.btnSlotActiveFull.Visible = true;
                }
                else
                {
                    this.btnSlotLockFull.Visible = true;
                    this.btnSlotActiveFull.Visible = false;
                }
                //this.btnDisassemble3.Visible = false;
                //this.btnDisassemble4.Visible = true;
                this.btnExtract3.Visible = false;
                this.btnExtract4.Visible = true;
                //this.btnAppraise3.Visible = false;
                //this.btnAppraise4.Visible = true;
                this.btnBits3.Visible = false;
                this.btnBits4.Visible = false;
                this.btnPot3.Visible = false;
                this.btnPot4.Visible = false;
                this.btnUpgrade3.Visible = false;
                this.btnUpgrade4.Visible = true;
                this.btnToad3.Visible = false;
                this.btnToad4.Visible = true;
                //this.btnCashshop.Visible = true;
                this.btnClose.Location = new Point(773, 8);
                this.btnSmall.Location = new Point(729, 8);
                this.btnHelp.Location = new Point(11, 515);
                this.btnHelp.Visible = true;
                this.btnEquip.Visible = false;
                this.btnShop.Visible = false;
                this.btnBag.Visible = false;
                this.btnFilter.Visible = false;
                this.btnFilterApply.Visible = false;
                this.btnFilterFullMax.Visible = false;
                this.btnFilterFullMaxApply.Visible = false;
                this.btnEquipFull.Visible = true;
                this.btnShopFull.Visible = true;
                this.btnBagFull.Visible = true;
                this.btnSearchFull.Location = new Point(246, 519);
                this.btnSearchFull.Visible = true;
                renderFull();
            }
            else if (this.smallMode)
            {
                if (this.itemTabs[5].Selected && !filter)
                {
                    this.btnFilter.Visible = true;
                    this.btnFilterApply.Visible = false;
                }
                else if (this.itemTabs[5].Selected && filter)
                {
                    this.btnFilter.Visible = false;
                    this.btnFilterApply.Visible = true;
                }
                else if (!this.itemTabs[5].Selected)
                {
                    this.btnFilterApply.Visible = false;
                    this.btnFilter.Visible = false;
                }
                this.btnFull.Visible = true;
                this.btnFullMax.Visible = false;
                this.btnSmall.Visible = false;
                this.btnSmallMax.Visible = false;
                this.vScroll.Visible = true;
                this.vScroll.Maximum = this.SelectedTab.ScrollMaxValue - 6;
                this.vScroll.Value = this.SelectedTab.ScrollValue;
                this.btnCoin3.Visible = true;
                this.btnCoin4.Visible = false;
                //this.btnPoint.Location = new Point(8, 328);
                //this.btnGather.Location = new Point(153, 310);
                this.btnSort.Visible = true;
                this.btnSortFull.Visible = false;
                if (this.slotLock)
                {
                    this.btnSlotLock.Visible = false;
                    this.btnSlotActive.Visible = true;
                }
                else
                {
                    this.btnSlotLock.Visible = true;
                    this.btnSlotActive.Visible = false;
                }
                this.btnSlotLockFull.Visible = false;
                this.btnSlotActiveFull.Visible = false;
                this.btnSlotLockFullMax.Visible = false;
                this.btnSlotActiveFullMax.Visible = false;
                //this.btnDisassemble3.Visible = true;
                //this.btnDisassemble4.Visible = false;
                this.btnExtract3.Visible = true;
                this.btnExtract4.Visible = false;
                //this.btnAppraise3.Visible = true;
                //this.btnAppraise4.Visible = false;
                this.btnBits3.Visible = false;
                this.btnBits4.Visible = false;
                this.btnPot3.Visible = false;
                this.btnPot4.Visible = false;
                this.btnUpgrade3.Visible = true;
                this.btnUpgrade4.Visible = false;
                this.btnToad3.Visible = true;
                this.btnToad4.Visible = false;
                //this.btnCashshop.Visible = false;
                this.btnClose.Location = new Point(215, 8);
                this.btnFull.Location = new Point(193, 8);
                this.btnHelp.Location = new Point(11, 563);
                this.btnHelp.Visible = true;
                this.btnEquip.Visible = true;
                this.btnShop.Visible = true;
                this.btnBag.Visible = true;
                this.btnEquipFull.Visible = false;
                this.btnShopFull.Visible = false;
                this.btnBagFull.Visible = false;
                this.btnFilterFull.Visible = false;
                this.btnFilterFullApply.Visible = false;
                this.btnFilterFullMax.Visible = false;
                this.btnFilterFullMaxApply.Visible = false;
                this.btnSearchFull.Visible = false;
                renderSmall();
            }
        }

        private void renderSmall()
        {
            this.Bitmap = new Bitmap(Resource.UIInventory_img_Inventory_backgrnd);
            Graphics g = Graphics.FromImage(this.Bitmap);
            g.DrawImage(Resource.UIInventory_img_Inventory_AutoBuild_buttonmin_disabled_0, 171, 8);
            renderTabs(g);
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.Draw(g);
            }
            ItemBase[] itemArray = this.SelectedTab.Items;
            int idxOffset = 4 * this.SelectedTab.ScrollValue;
            for (int i = 0; i < 32; i++)
            {
                Point origin = getItemIconOrigin(i);
                origin.Offset(28, 101);
                renderItemBase(g, itemArray[i + idxOffset], origin);
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            g.DrawString("0", GearGraphics.EquipDetailFont, Brushes.Black, 213f, 484f, format);  //金币为0
            g.DrawString("0", GearGraphics.EquipDetailFont, Brushes.Black, 213f, 508f, format);  //抵用券为0

            g.Dispose();
        }

        private void renderFull()
        {
            this.Bitmap = new Bitmap(Resource.UIInventory_img_Inventory_FullBackgrnd);
            Graphics g = Graphics.FromImage(this.Bitmap);
            g.DrawImage(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_disabled_0, 751, 8);
            renderTabs(g);
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.Draw(g);
            }

            ItemBase[] itemArray = this.SelectedTab.Items;
            for (int i = 0; i < itemArray.Length; i++)
            {
                int idx = i % 32, group = i / 32;
                Point origin = getItemIconOrigin(i);
                origin.Offset(28, 101);
                renderItemBase(g, itemArray[i], origin);
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            g.DrawString("0", GearGraphics.EquipDetailFont, Brushes.Black, 185f, 484f, format); //金币为0
            g.DrawString("0", GearGraphics.EquipDetailFont, Brushes.Black, 343f, 484f, format); //抵用券为0
            g.DrawImage(Resource.UIInventory_img_Inventory_FullAutoBuild_layersearch, 71, 521);
            g.Dispose();
        }

        private void renderFullMax()
        {
            this.Bitmap = new Bitmap(Resource.UIInventory_img_Inventory_FullMaxBackgrnd);
            Graphics g = Graphics.FromImage(this.Bitmap);
            g.DrawImage(Resource.UIInventory_img_Inventory_AutoBuild_buttonfull_disabled_0, 871, 26);
            renderTabs(g);
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.Draw(g);
            }

            ItemBase[] itemArray = this.SelectedTab.Items;
            for (int i = 0; i < itemArray.Length; i++)
            {
                int idx = i % 32, group = i / 32, zone = i / 128;
                Point origin = getItemIconOrigin(i);
                origin.Offset(123, 17);
                renderItemBase(g, itemArray[i], origin);
            }
            g.DrawImage(Resource.UIInventory_img_Inventory_FullMaxAutoBuild_layersearch, 21, 737);
            g.Dispose();
        }

        private void renderTabs(Graphics g)
        {
            updateitemTabs();
            for (int i = 0; i < this.itemTabs.Length; i++)
            {
                if (this.itemTabs[i].Selected)
                {
                    Point pos = this.itemTabs[i].TabEnabled.OpOrigin;
                    // if (this.fullMode)
                    pos.Offset(0, 0);
                    g.DrawImage(this.itemTabs[i].TabEnabled.Bitmap, pos);
                }
                else
                {
                    g.DrawImage(this.itemTabs[i].TabDisabled.Bitmap, this.itemTabs[i].TabEnabled.OpOrigin);
                }
            }
        }

        private void renderItemBase(Graphics g, ItemBase itemBase, Point origin)
        {
            if (itemBase is Gear)
                renderGear(g, itemBase as Gear, origin);
            else if (itemBase is Item)
                renderItem(g, itemBase as Item, origin);
        }

        private void renderGear(Graphics g, Gear gear, Point origin)
        {
            if (g == null || gear == null)
                return;
            Pen pen = GearGraphics.GetGearItemBorderPen(gear.Grade);
            if (pen != null)
            {
                Point[] path = GearGraphics.GetIconBorderPath(origin.X, origin.Y - 32);
                g.DrawLines(pen, path);
            }
            g.DrawImage(Resource.Item_shadow, origin.X + 3, origin.Y - 6);
            if (gear.IconRaw.Bitmap != null)
            {
                g.DrawImage(gear.IconRaw.Bitmap, origin.X - gear.IconRaw.Origin.X, origin.Y - gear.IconRaw.Origin.Y);
            }
            if (gear.Cash)
            {
                Bitmap cashImg = null;
                Point cashOrigin = new Point(12, 12);

                int value;
                if (gear.Props.TryGetValue(GearPropType.royalSpecial, out value) && value > 0)
                {
                    string resKey = $"CashShop_img_CashItem_label_{value - 1}";
                    cashImg = Resource.ResourceManager.GetObject(resKey) as Bitmap;
                }
                else if (gear.Props.TryGetValue(GearPropType.masterSpecial, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_3;
                }
                else if (gear.Props.TryGetValue(GearPropType.BTSLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_10;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                else if (gear.Props.TryGetValue(GearPropType.BLACKPINKLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_11;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                else if (gear.Props.TryGetValue(GearPropType.illusionGrade, out value) && value > 0)
                {
                    switch (value)
                    {
                        case 1:
                            cashImg = Resource.CashShop_img_CashItem_label_12;
                            cashOrigin = new Point(cashImg.Width, cashImg.Height);
                            break;
                        case 2:
                            cashImg = Resource.CashShop_img_CashItem_label_13;
                            cashOrigin = new Point(cashImg.Width, cashImg.Height);
                            break;
                        case 3:
                            cashImg = Resource.CashShop_img_CashItem_label_14;
                            cashOrigin = new Point(cashImg.Width, cashImg.Height);
                            break;
                        case 4:
                            cashImg = Resource.CashShop_img_CashItem_label_16;
                            cashOrigin = new Point(cashImg.Width, cashImg.Height);
                            break;
                    }
                }
                else if (gear.Props.TryGetValue(GearPropType.limitedLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_15;
                    cashOrigin = new Point(12, 12);
                }
                else if (gear.Props.TryGetValue(GearPropType.magicLayerWz2, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_100;
                    cashOrigin = new Point(12, 12);
                }
                if (cashImg == null) //default cashImg
                {
                    cashImg = Resource.CashItem_0;
                }

                //g.DrawImage(cashImg, origin.X + 20, origin.Y - 12);
                g.DrawImage(cashImg, origin.X + 32 - cashOrigin.X, origin.Y - cashOrigin.Y);
            }
            if (gear.TimeLimited)
            {
                g.DrawImage(Resource.Item_timeLimit_0, origin.X, origin.Y - 32);
            }
        }

        private void renderItem(Graphics g, Item item, Point origin)
        {
            if (g == null || item == null)
                return;
            g.DrawImage(Resource.Item_shadow, origin.X + 3, origin.Y - 6);
            if (item.IconRaw.Bitmap != null)
            {
                g.DrawImage(item.IconRaw.Bitmap, origin.X - item.IconRaw.Origin.X, origin.Y - item.IconRaw.Origin.Y);
            }
            if (item.Cash)
            {
                Bitmap cashImg = null;
                Point cashOrigin = new Point(12, 12);

                if (item.Props.TryGetValue(ItemPropType.wonderGrade, out long value) && value > 0)
                {
                    string resKey = $"CashShop_img_CashItem_label_{value + 3}";
                    cashImg = Resource.ResourceManager.GetObject(resKey) as Bitmap;
                }
                else if (item.Props.TryGetValue(ItemPropType.BTSLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_10;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                else if (item.Props.TryGetValue(ItemPropType.BLACKPINKLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_11;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                if (cashImg == null) //default cashImg
                {
                    cashImg = Resource.CashItem_0;
                }

                g.DrawImage(cashImg, origin.X + 32 - cashOrigin.X, origin.Y - cashOrigin.Y);
            }
            if (item.TimeLimited)
            {
                g.DrawImage(Resource.Item_timeLimit_0, origin.X, origin.Y - 32);
            }
            if (item.ItemID / 1000 == 3017)
            {
                g.DrawImage(Resource.Item_monsterCollection_0, origin.X, origin.Y - 32);
            }
        }

        private Point getItemIconOrigin(int index) //index指栏位下道具索引值
        {
            int zone = index / 128;
            int indexInZone = index % 128;   // 区域内的索引（0-127）
            int group = indexInZone / 32; // 区域内的组索引（0-3）
            int idxInGroup = indexInZone % 32; // 组内索引（0-31）

            int rowInGroup = idxInGroup / 4;   // 组内行号（0-7）
            int colInGroup = idxInGroup % 4;   // 组内列号（0-3）

            int absoluteRow = zone * 8 + rowInGroup;  // 全局行号（0-31）
            int absoluteCol = group * 4 + colInGroup; // 全局列号（0-15）

            Point p = new Point(absoluteCol * 46, absoluteRow * 46);
            p.Offset(5, 38);
            return p;
        }

        public int GetSlotIndexByPoint(Point point)
        {
            Point p = point;
            p.Offset(fullmaxMode ? -28 : -28, fullmaxMode ? 0 : -51);
            if (p.X < 0 || p.Y < 0)
                return -1;
            int x = p.X / 46, y = p.Y / 46;
            if ((fullmaxMode ? y >= 16 : y >= 8) || (smallMode ? x >= 4 : x >= 16))
                return -1;
            int idx = y * 4 + x % 4 + x / 4 * 32;
            if (new Rectangle(getItemIconOrigin(idx), new Size(42, 42)).Contains(point))
                return idx;
            else
                return -1;
        }

        public int GetItemIndexByPoint(Point point)
        {
            int slotIdx = GetSlotIndexByPoint(point);
            if (slotIdx != -1 && !this.fullMode)
            {
                slotIdx += 4 * this.SelectedTab.ScrollValue;
            }
            return slotIdx;
        }

        public ItemBase GetItemByPoint(Point point)
        {
            int itemIdx = GetItemIndexByPoint(point);
            if (itemIdx > -1 && itemIdx < this.SelectedTab.Items.Length)
                return this.SelectedTab.Items[itemIdx];
            else
                return null;
        }

        #region 重写和响应事件
        protected override void OnMouseMove(MouseEventArgs e)
        {
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseMove(e);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseMove(e);

            ItemBase item = GetItemByPoint(e.Location);
            if (item != null)
                this.OnItemMouseMove(new ItemMouseEventArgs(e, item));
            else
                this.OnItemMouseLeave(EventArgs.Empty);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseDown(e);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseDown(e);

            ItemBase item = GetItemByPoint(e.Location);
            if (item != null)
                this.OnItemMouseDown(new ItemMouseEventArgs(e, item));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseUp(e);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseUp(e);

            ItemBase item = GetItemByPoint(e.Location);
            if (item != null)
                this.OnItemMouseUp(new ItemMouseEventArgs(e, item));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            //处理选择选项卡
            tab_OnMouseClick(e);

            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseClick(e);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseClick(e);

            ItemBase item = GetItemByPoint(e.Location);
            if (item != null)
                this.OnItemMouseClick(new ItemMouseEventArgs(e, item));
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            foreach (AControl ctrl in this.aControls)
            {
                ctrl.OnMouseWheel(e);
            }

            if (this.waitForRefresh)
            {
                this.Refresh();
                waitForRefresh = false;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                this.SelectedIndex = (this.SelectedIndex + 1) % this.itemTabs.Length;
                this.Refresh();
            }

            base.OnKeyDown(e);
        }

        protected virtual void OnItemMouseDown(ItemMouseEventArgs e)
        {
            if (this.ItemMouseDown != null)
                this.ItemMouseDown(this, e);
        }

        protected virtual void OnItemMouseUp(ItemMouseEventArgs e)
        {
            if (this.ItemMouseUp != null)
                this.ItemMouseUp(this, e);
        }

        protected virtual void OnItemMouseClick(ItemMouseEventArgs e)
        {
            if (this.ItemMouseClick != null)
                this.ItemMouseClick(this, e);
        }

        protected virtual void OnItemMouseMove(ItemMouseEventArgs e)
        {
            if (this.ItemMouseMove != null)
                this.ItemMouseMove(this, e);
        }

        protected virtual void OnItemMouseLeave(EventArgs e)
        {
            if (this.ItemMouseLeave != null)
                this.ItemMouseLeave(this, e);
        }
        #endregion

        private void tab_OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < this.itemTabs.Length; i++)
                {
                    Rectangle rect;
                    if (this.itemTabs[i].Selected)
                    {
                        rect = this.itemTabs[i].TabEnabled.Rectangle;
                    }
                    else
                    {
                        rect = this.itemTabs[i].TabDisabled.Rectangle;
                    }
                    if (rect.Contains(e.Location))
                    {
                        if (this.SelectedIndex != i)
                        {
                            this.SelectedIndex = i;
                            //this.btnGather.Visible = true; //切换tab时重置btnGather状态
                            this.waitForRefresh = true;
                        }
                        break;
                    }
                }
            }
        }

        private void btnSmall_MouseClick(object sender, MouseEventArgs e)
        {
            this.smallMode = true;
            this.fullMode = false;
            this.fullmaxMode = false;
            this.waitForRefresh = true;
        }

        private void btnSmallMax_MouseClick(object sender, MouseEventArgs e)
        {
            this.smallMode = false;
            this.fullMode = true;
            this.fullmaxMode = false;
            this.waitForRefresh = true;
        }

        private void btnFull_MouseClick(object sender, MouseEventArgs e)
        {
            this.smallMode = false;
            this.fullMode = true;
            this.fullmaxMode = false;
            this.waitForRefresh = true;
        }

        private void btnFullMax_MouseClick(object sender, MouseEventArgs e)
        {
            this.smallMode = false;
            this.fullMode = false;
            this.fullmaxMode = true;
            this.waitForRefresh = true;
        }

        private void btnfilter_MouseClick(object sender, MouseEventArgs e)
        {
            this.filter = !this.filter;
        }

        private void vScroll_ValueChanged(object sender, EventArgs e)
        {
            this.SelectedTab.ScrollValue = this.vScroll.Value;
            this.waitForRefresh = true;
        }

        //private void btnGather_MouseClick(object sender, MouseEventArgs e)
        //{
        //    this.btnGather.Visible = !this.btnGather.Visible;
        //    this.gather();
        //    this.waitForRefresh = true;
        //}

        private void btnSort_MouseClick(object sender, MouseEventArgs e)
        {
            this.sort();
            this.waitForRefresh = true;
        }

        private void btnSlotLock_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLock = true;
            this.waitForRefresh = true;
        }

        private void btnSlotActive_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLock = false;
            this.waitForRefresh = true;
        }
        
        private void btnSlotLockFull_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLockFull = true;
            this.waitForRefresh = true;
        }

        private void btnSlotActiveFull_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLockFull = false;
            this.waitForRefresh = true;
        }
        
        private void btnSlotLockFullMax_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLockFullMax = true;
            this.waitForRefresh = true;
        }

        private void btnSlotActiveFullMax_MouseClick(object sender, MouseEventArgs e)
        {
            this.slotLockFullMax = false;
            this.waitForRefresh = true;
        }

        private void btnClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = false;
        }

        private void btnEquip_MouseClick(object sender, MouseEventArgs e)
        {
            charaSimCtrl.UIEquip.Visible = true;
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void gather()
        {
            ItemBase[] itemArray = this.SelectedTab.Items;
            Queue<int> nullQueue = new Queue<int>();
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i] == null)
                {
                    nullQueue.Enqueue(i);
                }
                else if (nullQueue.Count > 0)
                {
                    int nullIdx = nullQueue.Dequeue();
                    itemArray[nullIdx] = itemArray[i];
                    itemArray[i] = null;
                    nullQueue.Enqueue(i);
                }
            }
        }

        private void sort()
        {
            ItemBase[] itemArray = this.SelectedTab.Items;
            Array.Sort<ItemBase>(itemArray, (a, b) =>
            {
                if (a == null) return 1;
                if (b == null) return -1;
                return a.ItemID - b.ItemID;
            });
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return this.vScroll;
                yield return this.btnFullMax;
                yield return this.btnFull;
                yield return this.btnSmall;
                yield return this.btnSmallMax;
                yield return this.btnCoin3;
                yield return this.btnCoin4;
                //yield return this.btnPoint;
                //yield return this.btnGather;
                yield return this.btnSort;
                yield return this.btnSortFull;
                yield return this.btnSortFullMax;
                yield return this.btnSlotLock;
                yield return this.btnSlotActive;
                yield return this.btnSlotLockFull;
                yield return this.btnSlotActiveFull;
                yield return this.btnSlotLockFullMax;
                yield return this.btnSlotActiveFullMax;
                //yield return this.btnItemLock;
                //yield return this.btnItemActive;
                //yield return this.btnItemLockFull;
                //yield return this.btnItemActiveFull;
                //yield return this.btnItemLockFullMax;
                //yield return this.btnItemActiveFullMax;
                //yield return this.btnDisassemble3;
                //yield return this.btnDisassemble4;
                yield return this.btnExtract3;
                yield return this.btnExtract4;
                //yield return this.btnAppraise3;
                //yield return this.btnAppraise4;
                yield return this.btnBits3;
                yield return this.btnBits4;
                yield return this.btnPot3;
                yield return this.btnPot4;
                yield return this.btnUpgrade3;
                yield return this.btnUpgrade4;
                yield return this.btnToad3;
                yield return this.btnToad4;
                //yield return this.btnCashshop;
                yield return this.btnClose;
                yield return this.btnHelp;
                yield return this.btnEquip;
                yield return this.btnShop;
                yield return this.btnBag;
                yield return this.btnFilter;
                yield return this.btnFilterApply;
                yield return this.btnFilterFull;
                yield return this.btnFilterFullApply;
                yield return this.btnFilterFullMax;
                yield return this.btnFilterFullMaxApply;
                yield return this.btnEquipFull;
                yield return this.btnShopFull;
                yield return this.btnBagFull;
                yield return this.btnSearchFull;
            }
        }

        public ItemTab[] ItemTabs
        {
            get { return this.itemTabs; }
        }

        /// <summary>
        /// 获取或设置一个bool值，它表示是否当前背包显示为小背包模式。
        /// </summary>
        public bool SmallMode
        {
            get { return smallMode; }
            set { smallMode = value; }
        }

        /// <summary>
        /// 获取或设置一个bool值，它表示是否当前背包显示为大背包模式。
        /// </summary>
        /// 
        public bool FullMode
        {
            get { return fullMode; }
            set { fullMode = value; }
        }

        /// <summary>
        /// 获取或设置一个bool值，它表示是否当前背包显示为最大背包模式。
        /// </summary>
        public bool FullMaxMode
        {
            get { return fullmaxMode; }
            set { fullmaxMode = value; }
        }

        /// <summary>
        /// 获取或设置正在选中的背包选项卡的索引。
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                for (int i = 0; i < this.itemTabs.Length; i++)
                {
                    if (this.itemTabs[i].Selected)
                        return i;
                }
                this.itemTabs[0].Selected = true;
                return 0;
            }
            set
            {
                value = Math.Min(Math.Max(value, 0), this.itemTabs.Length - 1);
                this.selectedIndexChanging = true;
                for (int i = 0; i < this.itemTabs.Length; i++)
                {
                    this.itemTabs[i].Selected = (i == value);
                }
                this.selectedIndexChanging = false;
            }
        }

        public bool AddItem(ItemBase item)
        {
            if (item == null)
                return false;

            int idx;
            switch (item.Type)
            {
                case ItemBaseType.Equip:
                    idx = 0;
                    if (item is Gear gear && gear.Cash) // Ensure the item is a Gear instance before accessing the Cash property
                        idx = 5;
                    break;
                case ItemBaseType.Consume: idx = 1; break;
                case ItemBaseType.Etc: idx = 2; break;
                case ItemBaseType.Install: idx = 3; break;
                case ItemBaseType.Cash: idx = 4; break;
                case ItemBaseType.Deco: idx = 5; break;
                default: return false;
            }

            ItemBase[] itemArray = this.itemTabs[idx].Items;

            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i] == null)
                {
                    itemArray[i] = (ItemBase)item.Clone();
                    this.SelectedIndex = idx;
                    this.itemTabs[idx].ScrollValue = (i - 28) / 4;
                    this.Refresh();
                    return true;
                }
            }
            return false;
        }

        public void RemoveItem(int index)
        {
            ItemBase[] itemArray = this.SelectedTab.Items;
            if (index >= 0 && index < itemArray.Length && itemArray[index] != null)
            {
                itemArray[index] = null;
                this.Refresh();
            }
        }

        /// <summary>
        /// 获取或设置正在选中的背包选项卡。
        /// </summary>
        public ItemTab SelectedTab
        {
            get { return this.itemTabs[this.SelectedIndex]; }
            set { this.SelectedIndex = Array.IndexOf(this.itemTabs, value); }
        }

        public class ItemTab
        {
            public ItemTab(AfrmItem owner)
            {
                this.owner = owner;
                this.items = new ItemBase[ItemCount];
            }

            public const int ItemCount = 128;
            private AfrmItem owner;
            private ItemBase[] items;
            private bool selected;
            private BitmapOrigin tabEnabled;
            private BitmapOrigin tabDisabled;
            private int scrollValue;

            public ItemBase[] Items
            {
                get { return this.items; }
            }

            public bool Selected
            {
                get { return selected; }
                set
                {
                    if (selected != value)
                    {
                        if (!this.owner.selectedIndexChanging)
                            this.owner.SelectedIndex = Array.IndexOf(this.owner.itemTabs, this);
                        this.selected = value;
                    }
                }
            }

            public bool SetItemSource(ItemBase[] items)
            {
                if (items == null || items.Length != ItemCount)
                {
                    return false;
                }
                else
                {
                    this.items = items;
                    return true;
                }
            }

            public void ClearItems()
            {
                for (int i = 0; i < this.items.Length; i++)
                {
                    this.items[i] = null;
                }
            }

            public BitmapOrigin TabEnabled
            {
                get { return tabEnabled; }
                set { tabEnabled = value; }
            }

            public BitmapOrigin TabDisabled
            {
                get { return tabDisabled; }
                set { tabDisabled = value; }
            }

            public int ScrollMaxValue
            {
                get { return this.items.Length / 4; }
            }

            public int ScrollValue
            {
                get { return scrollValue; }
                set
                {
                    value = Math.Min(Math.Max(0, value), ScrollMaxValue);
                    scrollValue = value;
                }
            }
        }
    }
}