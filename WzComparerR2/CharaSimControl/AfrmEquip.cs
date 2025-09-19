using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using CharaSimResource;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.Controls;
using WzComparerR2.CharaSimControl;
using System.Security.Cryptography;

namespace WzComparerR2.CharaSimControl
{
    public class AfrmEquip : AlphaForm
    {
        public AfrmEquip()
        {
            sec = new int[6];
            for (int i = 0; i < sec.Length; i++)
                sec[i] = 1 << i;

            initCtrl();
            this.AllowDrop = true;
            this.TotemVisible = false;
            this.SymbolVisible = false;
        }

        private BitVector32 partVisible;
        private int[] sec;

        private Point baseOffset;
        private Point newLocation;
        private bool waitForRefresh;
        private Character character;

        private ACtrlButton btnDeco;
        private ACtrlButton btnEquip;
        private ACtrlButton btnEquipTab;
        private ACtrlButton btnPetTab;
        private ACtrlButton btnTitle;
        private ACtrlButton btnTotem;
        private ACtrlButton btnSymbol;
        private ACtrlButton btnPreset1;
        private ACtrlButton btnPreset2;
        private ACtrlButton btnPreset3;
        private ACtrlButton btnPresetApply;
        private ACtrlButton btnPet;
        private ACtrlButton btnDragon;
        private ACtrlButton btnMechanic;
        private ACtrlButton btnAndroid;
        private ACtrlButton btnBeautyRoom;
        private ACtrlButton btnCoordiPreset;
        private ACtrlButton btnAndroidShop;
        private ACtrlButton btnDressUpTab;
        private ACtrlButton btnAndroidTab;
        private ACtrlButton btnDamageSkinTab;
        private ACtrlButton btnHairTab;
        private ACtrlButton btnFaceTab;
        private ACtrlButton btnHelp;
        private ACtrlButton btnClose;

        private bool showSpec = false;
        private bool equipVisible = true;
        private bool equipMode = true;
        private bool petMode = false;
        private bool dressupMode = false;
        private bool androidMode = false;
        private bool damageSkinMode = false;
        private bool decoVisible = false;
        private bool TitleMedalVisble = false;
        private bool BeautyRoomVisble = false;
        private bool CoordiPresetVisble = false;
        private bool ArcMode = false;
        private bool AutMode = false;
        private bool GrandAutMode = false;
        private bool HairMode = false;
        private bool FaceMode = false;
        private int currentPreset = 1;
        private int checkPreset = 1;
        private int jobID;

        public Character Character
        {
            get { return character; }
            set { character = value; }
        }

        public bool PetVisible
        {
            get { return partVisible[sec[0]]; }
            private set { partVisible[sec[0]] = value; }
        }

        public bool DragonVisible
        {
            get { return partVisible[sec[1]]; }
            private set
            {
                partVisible[sec[1]] = value;
                if (value)
                {
                    partVisible[sec[2]] = false;
                    partVisible[sec[3]] = false;
                }
            }
        }

        public bool MechanicVisible
        {
            get { return partVisible[sec[2]]; }
            private set
            {
                partVisible[sec[2]] = value;
                if (value)
                {
                    partVisible[sec[1]] = false;
                    partVisible[sec[3]] = false;
                }
            }
        }

        public bool AndroidVisible
        {
            get { return partVisible[sec[3]]; }
            private set
            {
                partVisible[sec[3]] = value;
                if (value)
                {
                    partVisible[sec[1]] = false;
                    partVisible[sec[2]] = false;
                }
            }
        }

        public bool TotemVisible
        {
            get { return partVisible[sec[4]]; }
            private set { partVisible[sec[4]] = value; }
        }

        public bool SymbolVisible
        {
            get { return partVisible[sec[5]]; }
            private set { partVisible[sec[5]] = value; }
        }

        private Rectangle DragonRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.Equip_dragon_backgrnd.Width, baseOffset.Y),
                    Resource.Equip_dragon_backgrnd.Size);
            }
        }

        private Rectangle MechanicRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.Equip_mechanic_backgrnd.Width, baseOffset.Y),
                    Resource.Equip_mechanic_backgrnd.Size);
            }
        }

        private Rectangle AndroidRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.Equip_Android_backgrnd.Width, baseOffset.Y),
                    Resource.Equip_Android_backgrnd.Size);
            }
        }

        private Rectangle TotemRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X,
                        baseOffset.Y + Resource.UIInventory_img_Equip_main_backgrnd.Height + 1),
                    Resource.UIInventory_img_Equip_EquipTab_totemEquip_canvastotem.Size);
            }
        }

        private Rectangle SymbolRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X + Resource.UIInventory_img_Equip_main_backgrnd.Width + 1,
                        baseOffset.Y + 53),
                    Resource.UIInventory_img_Equip_Symbol_backgrnd.Size);
            }
        }

        private Rectangle TitleRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.UIWindow4_img_Equip_titleSkin_backgrnd.Width - 1,
                    baseOffset.Y + 53),
                    Resource.UIWindow4_img_Equip_titleSkin_backgrnd.Size);
            }
        }

        private Rectangle BeautyRoomRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - Resource.UIInventory_img_Deco_BeautyRoom_backgrnd.Width - 1,
                    baseOffset.Y),
                    Resource.UIInventory_img_Deco_BeautyRoom_backgrnd.Size);
            }
        }

        private Rectangle CooridiPresetRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X + Resource.UIInventory_img_Equip_main_backgrnd.Width + 1,
                    baseOffset.Y + Resource.UIInventory_img_Equip_main_backgrnd.Height - Resource.UIInventory_img_Deco_CoordiPreset_backgrnd.Height),
                    Resource.UIInventory_img_Deco_CoordiPreset_backgrnd.Size);
            }
        }

        private void initCtrl()
        {
            this.btnDeco = new ACtrlButton();
            this.btnDeco.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonDecoUI_normal_0);
            this.btnDeco.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonDecoUI_pressed_0);
            this.btnDeco.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonDecoUI_mouseOver_0);
            this.btnDeco.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonDecoUI_disabled_0);
            this.btnDeco.Location = new Point(251, 72);
            this.btnDeco.Size = new Size(89, 22);
            this.btnDeco.Visible = true;
            this.btnDeco.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDeco.MouseClick += new System.Windows.Forms.MouseEventHandler(btnDeco_MouseClick);

            this.btnEquip = new ACtrlButton();
            this.btnEquip.Normal = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonEquipUI_normal_0);
            this.btnEquip.Pressed = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonEquipUI_pressed_0);
            this.btnEquip.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonEquipUI_mouseOver_0);
            this.btnEquip.Disabled = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonEquipUI_disabled_0);
            this.btnEquip.Location = new Point(251, 72);
            this.btnEquip.Size = new Size(89, 22);
            this.btnEquip.Visible = false;
            this.btnEquip.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquip.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEquip_MouseClick);

            this.btnEquipTab = new ACtrlButton();
            this.btnEquipTab.Location = new Point(11, 31);
            this.btnEquipTab.Size = new Size(171, 22);
            this.btnEquipTab.Visible = true;
            this.btnEquipTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquipTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEquipTab_MouseClick);

            this.btnPetTab = new ACtrlButton();
            this.btnPetTab.Location = new Point(184, 31);
            this.btnPetTab.Size = new Size(171, 22);
            this.btnPetTab.Visible = true;
            this.btnPetTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPetTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPetTab_MouseClick);

            this.btnDressUpTab = new ACtrlButton();
            this.btnDressUpTab.Location = new Point(12, 31);
            this.btnDressUpTab.Size = new Size(113, 22);
            this.btnDressUpTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDressUpTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnDressUpTab_MouseClick);

            this.btnAndroidTab = new ACtrlButton();
            this.btnAndroidTab.Location = new Point(127, 31);
            this.btnAndroidTab.Size = new Size(113, 22);
            this.btnAndroidTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnAndroidTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnAndroidTab_MouseClick);

            this.btnDamageSkinTab = new ACtrlButton();
            this.btnDamageSkinTab.Location = new Point(243, 31);
            this.btnDamageSkinTab.Size = new Size(113, 22);
            this.btnDamageSkinTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDamageSkinTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnDamageSkinTab_MouseClick);

            this.btnHairTab = new ACtrlButton();
            this.btnHairTab.Location = new Point(10, 31);
            this.btnHairTab.Size = new Size(199, 23);
            this.btnHairTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHairTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnHairTab_MouseClick);

            this.btnFaceTab = new ACtrlButton();
            this.btnFaceTab.Location = new Point(211, 31);
            this.btnFaceTab.Size = new Size(199, 23);
            this.btnFaceTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFaceTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnFaceTab_MouseClick);

            this.btnTitle = new ACtrlButton();
            this.btnTitle.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontitleSkin_normal_0);
            this.btnTitle.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontitleSkin_pressed_0);
            this.btnTitle.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontitleSkin_mouseOver_0);
            this.btnTitle.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontitleSkin_disabled_0);
            this.btnTitle.Location = new Point(11, 414);
            this.btnTitle.Size = new Size(84, 23);
            this.btnTitle.Visible = true;
            this.btnTitle.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(btnTitle_MouseClick);

            this.btnTotem = new ACtrlButton();
            this.btnTotem.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_normal_0);
            this.btnTotem.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_pressed_0);
            this.btnTotem.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_mouseOver_0);
            this.btnTotem.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_disabled_0);
            this.btnTotem.Location = new Point(98, 414);
            this.btnTotem.Size = new Size(84, 23);
            this.btnTotem.Visible = true;
            this.btnTotem.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTotem.MouseClick += new System.Windows.Forms.MouseEventHandler(btnTotem_MouseClick);

            this.btnSymbol = new ACtrlButton();
            this.btnSymbol.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonsymbol_normal_0);
            this.btnSymbol.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonsymbol_pressed_0);
            this.btnSymbol.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonsymbol_mouseOver_0);
            this.btnSymbol.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonsymbol_disabled_0);
            this.btnSymbol.Location = new Point(272, 414);
            this.btnSymbol.Size = new Size(84, 23);
            this.btnSymbol.Visible = true;
            this.btnSymbol.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSymbol.MouseClick += new System.Windows.Forms.MouseEventHandler(btnSymbol_MouseClick); ;

            this.btnPet = new ACtrlButton();
            this.btnPet.Normal = new BitmapOrigin(Resource.Equip_character_BtPet_normal_0);
            this.btnPet.Pressed = new BitmapOrigin(Resource.Equip_character_BtPet_pressed_0);
            this.btnPet.MouseOver = new BitmapOrigin(Resource.Equip_character_BtPet_mouseOver_0);
            this.btnPet.Disabled = new BitmapOrigin(Resource.Equip_character_BtPet_disabled_0);
            this.btnPet.Location = new Point(142, 266);
            this.btnPet.Size = new Size(32, 12);
            this.btnPet.Visible = false;
            this.btnPet.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPet.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPet_MouseClick);

            this.btnPresetApply = new ACtrlButton();
            this.btnPresetApply.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_normal_0);
            this.btnPresetApply.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_pressed_0);
            this.btnPresetApply.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_mouseOver_0);
            this.btnPresetApply.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_disabled_0);
            this.btnPresetApply.Location = new Point(282, 376);
            this.btnPresetApply.Size = new Size(58, 25);
            this.btnPresetApply.Visible = false;
            this.btnPresetApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetApply.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPresetApply_MouseClick);

            this.btnPreset1 = new ACtrlButton();
            this.btnPreset1.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_normal_0);
            this.btnPreset1.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_pressed_0);
            this.btnPreset1.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_mouseOver_0);
            this.btnPreset1.Location = new Point(184, 379);
            this.btnPreset1.Size = new Size(18, 18);
            this.btnPreset1.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset1.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset1_MouseClick);

            this.btnPreset2 = new ACtrlButton();
            this.btnPreset2.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_normal_0);
            this.btnPreset2.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_pressed_0);
            this.btnPreset2.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_mouseOver_0);
            this.btnPreset2.Location = new Point(214, 379);
            this.btnPreset2.Size = new Size(18, 18);
            this.btnPreset2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset2.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset2_MouseClick);

            this.btnPreset3 = new ACtrlButton();
            this.btnPreset3.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_normal_0);
            this.btnPreset3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_pressed_0);
            this.btnPreset3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_mouseOver_0);
            this.btnPreset3.Location = new Point(244, 379);
            this.btnPreset3.Size = new Size(18, 18);
            this.btnPreset3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset3.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset3_MouseClick);

            this.btnDragon = new ACtrlButton();
            this.btnDragon.Normal = new BitmapOrigin(Resource.Equip_character_BtDragon_normal_0);
            this.btnDragon.Pressed = new BitmapOrigin(Resource.Equip_character_BtDragon_pressed_0);
            this.btnDragon.MouseOver = new BitmapOrigin(Resource.Equip_character_BtDragon_mouseOver_0);
            this.btnDragon.Disabled = new BitmapOrigin(Resource.Equip_character_BtDragon_disabled_0);
            this.btnDragon.Location = new Point(10, 266);
            this.btnDragon.Size = new Size(43, 12);
            this.btnDragon.Visible = false;
            this.btnDragon.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDragon.MouseClick += new MouseEventHandler(btnDragon_MouseClick);

            this.btnMechanic = new ACtrlButton();
            this.btnMechanic.Normal = new BitmapOrigin(Resource.Equip_character_BtMechanic_normal_0);
            this.btnMechanic.Pressed = new BitmapOrigin(Resource.Equip_character_BtMechanic_pressed_0);
            this.btnMechanic.MouseOver = new BitmapOrigin(Resource.Equip_character_BtMechanic_mouseOver_0);
            this.btnMechanic.Disabled = new BitmapOrigin(Resource.Equip_character_BtMechanic_disabled_0);
            this.btnMechanic.Location = new Point(10, 266);
            this.btnMechanic.Size = new Size(43, 12);
            this.btnMechanic.Visible = false;
            this.btnMechanic.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnMechanic.MouseClick += new MouseEventHandler(btnMechanic_MouseClick);

            this.btnAndroid = new ACtrlButton();
            this.btnAndroid.Normal = new BitmapOrigin(Resource.Equip_character_BtAndroid_normal_0);
            this.btnAndroid.Pressed = new BitmapOrigin(Resource.Equip_character_BtAndroid_pressed_0);
            this.btnAndroid.MouseOver = new BitmapOrigin(Resource.Equip_character_BtAndroid_mouseOver_0);
            this.btnAndroid.Disabled = new BitmapOrigin(Resource.Equip_character_BtAndroid_disabled_0);
            this.btnAndroid.Location = new Point(65, 266);
            this.btnAndroid.Size = new Size(25, 12);
            this.btnAndroid.Visible = false;
            this.btnAndroid.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnAndroid.MouseClick += new MouseEventHandler(btnAndroid_MouseClick);

            this.btnBeautyRoom = new ACtrlButton();
            this.btnBeautyRoom.Normal = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_BeautyRoom_normal_0);
            this.btnBeautyRoom.Pressed = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_BeautyRoom_pressed_0);
            this.btnBeautyRoom.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_BeautyRoom_mouseOver_0);
            this.btnBeautyRoom.Disabled = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_BeautyRoom_disabled_0);
            this.btnBeautyRoom.Location = new Point(11, 414);
            this.btnBeautyRoom.Size = new Size(101, 23);
            this.btnBeautyRoom.Visible = false;
            this.btnBeautyRoom.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnBeautyRoom.MouseClick += new MouseEventHandler(btnBeautyRoom_MouseClick);

            this.btnCoordiPreset = new ACtrlButton();
            this.btnCoordiPreset.Normal = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonCoordiPreset_normal_0);
            this.btnCoordiPreset.Pressed = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonCoordiPreset_pressed_0);
            this.btnCoordiPreset.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonCoordiPreset_mouseOver_0);
            this.btnCoordiPreset.Disabled = new BitmapOrigin(Resource.UIInventory_img_Deco_CoordiTab_buttonCoordiPreset_disabled_0);
            this.btnCoordiPreset.Location = new Point(255, 414);
            this.btnCoordiPreset.Size = new Size(101, 23);
            this.btnCoordiPreset.Visible = false;
            this.btnCoordiPreset.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnCoordiPreset.MouseClick += new MouseEventHandler(btnCoordiPreset_MouseClick);

            this.btnAndroidShop = new ACtrlButton();
            this.btnAndroidShop.Normal = new BitmapOrigin(Resource.UIInventory_img_Deco_AndroidTab_buttonShop_normal_0);
            this.btnAndroidShop.Pressed = new BitmapOrigin(Resource.UIInventory_img_Deco_AndroidTab_buttonShop_pressed_0);
            this.btnAndroidShop.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Deco_AndroidTab_buttonShop_mouseOver_0);
            this.btnAndroidShop.Disabled = new BitmapOrigin(Resource.UIInventory_img_Deco_AndroidTab_buttonShop_disabled_0);
            this.btnAndroidShop.Location = new Point(255, 414);
            this.btnAndroidShop.Size = new Size(101, 23);
            this.btnAndroidShop.Visible = false;
            this.btnAndroidShop.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnHelp = new ACtrlButton();
            this.btnHelp.Normal = new BitmapOrigin(Resource.UIInventory_img_Deco_BeautyRoom_buttonhelp_normal_0);
            this.btnHelp.Pressed = new BitmapOrigin(Resource.UIInventory_img_Deco_BeautyRoom_buttonhelp_pressed_0);
            this.btnHelp.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Deco_BeautyRoom_buttonhelp_mouseOver_0);
            this.btnHelp.Disabled = new BitmapOrigin(Resource.UIInventory_img_Deco_BeautyRoom_buttonhelp_disabled_0);
            this.btnHelp.Location = new Point(11, 414);
            this.btnHelp.Size = new Size(36, 23);
            this.btnHelp.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnClose = new ACtrlButton();
            this.btnClose.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_main_buttonclose_normal_0);
            this.btnClose.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_main_buttonclose_pressed_0);
            this.btnClose.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_main_buttonclose_mouseOver_0);
            this.btnClose.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_main_buttonclose_disabled_0);
            this.btnClose.Location = new Point(343, 12);
            this.btnClose.Size = new Size(11, 11);
            this.btnClose.Visible = true;
            this.btnClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnClose.MouseClick += new MouseEventHandler(btnClose_MouseClick);
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(Resource.UIInventory_img_Equip_main_backgrnd.Width, 24));
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
            control_event();
            //处理按钮可见
            //setControlState();

            //计算图像大小
            Point baseOffsetnew = calcRenderBaseOffset();
            Size size = Resource.UIInventory_img_Equip_main_backgrnd.Size;
            size.Width += baseOffsetnew.X;
            if (this.equipVisible)
            {
                if (this.TotemVisible)
                    size.Height += Resource.UIInventory_img_Equip_EquipTab_totemEquip_canvastotem.Height + 1;
                if (this.SymbolVisible)
                    size.Width += Resource.UIInventory_img_Equip_Symbol_backgrnd.Width + 1;
                if (this.TitleMedalVisble)
                    size.Width += Resource.UIWindow4_img_Equip_titleSkin_backgrnd.Width + 1;
            }
            if (decoVisible)
            {
                if (this.BeautyRoomVisble)
                    size.Width += Resource.UIInventory_img_Deco_BeautyRoom_backgrnd.Width + 1;
                if (this.CoordiPresetVisble)
                    size.Width += Resource.UIInventory_img_Deco_CoordiPreset_backgrnd.Width + 1;
            }


            //处理偏移
            this.newLocation = new Point(this.Location.X + this.baseOffset.X - baseOffsetnew.X,
                this.Location.Y + this.baseOffset.Y - baseOffsetnew.Y);
            this.baseOffset = baseOffsetnew;

            //绘制图像
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bitmap);

            if (this.equipVisible)
            {
                renderEquip(g);

                if (this.SymbolVisible)
                    renderSymbol(g);
                if (this.TitleMedalVisble)
                    renderTitleMedal(g);
            }
            if (this.decoVisible)
            {
                renderDeco(g);
                if (this.dressupMode)
                {
                    if (this.BeautyRoomVisble)
                        renderBeautyRoom(g);
                    if (this.CoordiPresetVisble)
                        renderCoordiPreset(g);
                }
            }
            if (this.TotemVisible) renderTotem(g);
            if (this.DragonVisible) renderDragon(g);
            else if (this.MechanicVisible) renderMechanic(g);
            else if (this.AndroidVisible) renderAndroid(g);

            g.Dispose();
            this.Bitmap = bitmap;
        }

        private Point calcRenderBaseOffset()
        {
            if (this.TitleMedalVisble)
                return new Point(Resource.UIWindow4_img_Equip_titleSkin_backgrnd.Width, 0);
            if (this.BeautyRoomVisble)
                return new Point(Resource.UIInventory_img_Deco_BeautyRoom_backgrnd.Width, 0);
            if (this.DragonVisible)
                return new Point(Resource.Equip_dragon_backgrnd.Width, 0);
            else if (this.MechanicVisible)
                return new Point(Resource.Equip_mechanic_backgrnd.Width, 0);
            else if (this.AndroidVisible)
                return new Point(Resource.Equip_Android_backgrnd.Width, 0);
            else if (this.TotemVisible)
                return new Point(Resource.Equip_totem_backgrnd.Width, 0);
            else
                return new Point(0, 0);
        }



        private void control_event()
        {
            if (this.equipVisible)
            {
                if (equipMode)
                {
                    this.btnDeco.Visible = true;
                    this.btnEquip.Visible = false;
                    this.btnTotem.Location = new Point(98, 414);
                    this.btnTotem.Visible = true;
                    this.btnTitle.Visible = true;
                    this.btnSymbol.Visible = true;
                    this.btnPreset1.Visible = true;
                    this.btnPreset2.Visible = true;
                    this.btnPreset3.Visible = true;
                    if (this.currentPreset == this.checkPreset)
                        this.btnPresetApply.Visible = false;
                    else
                        this.btnPresetApply.Visible = true;
                    if (this.jobID == 2200 || this.jobID == 3500)
                        this.showSpec = true;
                    if (this.jobID == 2200)
                        this.DragonVisible = true;
                    else if (this.jobID == 3500)
                        this.MechanicVisible = true;
                    else
                    {
                        this.DragonVisible = false;
                        this.MechanicVisible = false;
                    }
                }
                else if (petMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnTitle.Visible = false;
                    this.btnTotem.Visible = false;
                    this.btnSymbol.Visible = false;
                    this.btnPreset1.Visible = false;
                    this.btnPreset2.Visible = false;
                    this.btnPreset3.Visible = false;
                    this.btnPresetApply.Visible = false;
                }
                this.btnEquipTab.Visible = true;
                this.btnPetTab.Visible = true;
                this.btnDressUpTab.Visible = false;
                this.btnAndroidTab.Visible = false;
                this.btnDamageSkinTab.Visible = false;
                this.btnBeautyRoom.Visible = false;
                this.btnCoordiPreset.Visible = false;
                this.btnAndroidShop.Visible = false;
                this.btnFaceTab.Visible = false;
                this.btnHairTab.Visible = false;
                this.btnHelp.Visible = false;
            }
            else if (this.decoVisible)
            {
                if (this.dressupMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = true;
                    this.btnBeautyRoom.Visible = true;
                    this.btnCoordiPreset.Visible = true;
                    this.btnAndroidShop.Visible = false;
                    this.btnTotem.Location = new Point(143, 414);
                    this.btnTotem.Visible = true;
                    this.btnHelp.Visible = false;
                }
                else if (this.androidMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnBeautyRoom.Visible = false;
                    this.btnCoordiPreset.Visible = false;
                    this.btnAndroidShop.Visible = true;
                    this.btnTotem.Visible = false;
                    this.btnHelp.Visible = false;
                }
                else if (this.damageSkinMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnBeautyRoom.Visible = false;
                    this.btnCoordiPreset.Visible = false;
                    this.btnAndroidShop.Visible = false;
                    this.btnTotem.Visible = false;
                    this.btnHelp.Visible = true;
                }
                if (this.BeautyRoomVisble)
                {
                    this.btnFaceTab.Visible = true;
                    this.btnHairTab.Visible = true;
                }
                this.btnDressUpTab.Visible = true;
                this.btnAndroidTab.Visible = true;
                this.btnDamageSkinTab.Visible = true;
                this.btnTitle.Visible = false;
                this.btnSymbol.Visible = false;
                this.btnPreset1.Visible = false;
                this.btnPreset2.Visible = false;
                this.btnPreset3.Visible = false;
                this.btnPresetApply.Visible = false;
                this.TitleMedalVisble = false;
                this.SymbolVisible = false;
            }
        }

        private void setControlState()
        {
            if (this.character == null)
            {
                this.btnDragon.Visible = false;
                this.btnMechanic.Visible = false;
                this.DragonVisible = false;
                this.MechanicVisible = false;
            }
            else
            {
                if (this.character.Status.Job / 100 == 22) //龙神
                {
                    this.btnDragon.Visible = true;
                }
                else
                {
                    this.btnDragon.Visible = false;
                    this.DragonVisible = false;
                }

                if (this.character.Status.Job / 100 == 35) //机械
                {
                    this.btnMechanic.Visible = true;
                }
                else
                {
                    this.btnMechanic.Visible = false;
                    this.MechanicVisible = false;
                }
            }
        }

        private void renderEquip(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UIInventory_img_Equip_main_backgrnd, 0, 0);
            if (equipMode) //装备模式
            {
                g.DrawImage(Resource.UIInventory_img_Equip_main_tabdetailTab_selected_0, 11, 31);
                g.DrawImage(Resource.UIInventory_img_Equip_main_tabdetailTab_normal_1, 184, 31);
                if (!showSpec)
                    g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_canvasequip, 12, 61);
                else
                    g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_canvasequip2, 12, 61);

                if (this.checkPreset == this.currentPreset)
                    g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_disabled_0, 282, 376);
                switch (checkPreset)
                {
                    case 1: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 182, 379 - 15); break;
                    case 2: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 212, 379 - 15); break;
                    case 3: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 242, 379 - 15); break;
                }
                if (this.character != null) //绘制装备
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Gear gear = this.character.Equip.GearSlots[i];
                        if (gear != null)
                        {
                            int dx = 10 + i % 5 * 33, dy = 27 + i / 5 * 33;
                            drawGearIcon(gear, g, dx, dy);
                        }
                    }
                }
            }
            else if (petMode) //宠物模式
            {
                g.DrawImage(Resource.UIInventory_img_Equip_main_tabdetailTab_normal_0, 12, 31);
                g.DrawImage(Resource.UIInventory_img_Equip_main_tabdetailTab_selected_1, 184, 31);
                g.DrawImage(Resource.UIInventory_img_Equip_PetTab_canvaspet, 12, 61);
            }
            //g.DrawImage(Resource.Equip_character_backgrnd3, 10, 27);
            //g.DrawImage(Resource.Equip_character_cashPendant, 76, 93);
            //g.DrawImage(Resource.Equip_character_charmPocket, 10, 93);
            if (this.character != null
                && (this.character.Status.Job / 100 == 23 || this.character.Status.Job == 2002))
            {
                g.DrawImage(Resource.Equip_character_magicArrow, 142, 126);
            }

            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }

            g.ResetTransform();
        }

        private void renderDeco(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            g.DrawImage(Resource.UIInventory_img_Deco_main_backgrnd, 0, 0);
            if (this.dressupMode)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_selected_0, 11, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_1, 127, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_2, 244, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_CoordiTab_canvascoordi, 12, 61);
            }
            else if (androidMode)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_0, 11, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_selected_1, 127, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_2, 244, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_AndroidTab_canvasand, 12, 61);
            }
            else if (damageSkinMode)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_0, 11, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_normal_1, 127, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_main_tabdetailTab_selected_2, 244, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_DamageSkinTab_canvasand, 12, 61);
            }

            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }

            g.ResetTransform();

        }

        private void renderBeautyRoom(Graphics g)
        {
            Rectangle rect = this.BeautyRoomRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_backgrnd, 0, 0);
            if (this.HairMode)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_BeautyRoom_tabTab_selected_0, 10, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_BeautyRoom_tabTab_normal_1, 211, 31);
            }
            else if (this.FaceMode)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_BeautyRoom_tabTab_normal_0, 10, 31);
                g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_BeautyRoom_tabTab_selected_1, 211, 31);
            }

            for (int i = 0; i< 6; i++)
            {
                g.DrawImage(Resource.UIInventory_img_Deco_BeautyRoom_BeautyRoom_Slot_layerunavailableSlot, 10 + 134 * (i % 3), 61 + 178 * (i / 3));
            }
            foreach (AControl aCtrl in this.BeautyControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void renderCoordiPreset(Graphics g)
        {
            Rectangle rect = this.CooridiPresetRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UIInventory_img_Deco_CoordiPreset_backgrnd, 0, 0);
            g.ResetTransform();
        }

        private void renderDragon(Graphics g)
        {
            Rectangle rect = this.DragonRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.Equip_dragon_backgrnd, 0, 0);
            g.DrawImage(Resource.Equip_dragon_backgrnd2, 6, 22);
            g.DrawImage(Resource.Equip_dragon_backgrnd3, 10, 29);

            if (this.character != null)
            {
                for (int i = 35; i < 39; i++)
                {
                    Gear gear = this.character.Equip.GearSlots[i];
                    if (gear != null)
                    {
                        int dx = 10 + (i - 35) * 33, dy = 22 + (((i - 1) % 2) + 1) * 33;
                        drawGearIcon(gear, g, dx, dy);
                    }
                }
            }
            g.ResetTransform();
        }

        private void renderMechanic(Graphics g)
        {
            Rectangle rect = this.MechanicRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.Equip_mechanic_backgrnd, 0, 0);
            g.DrawImage(Resource.Equip_mechanic_backgrnd2, 6, 22);
            g.DrawImage(Resource.Equip_mechanic_backgrnd3, 12, 35);

            if (this.character != null)
            {
                int dx, dy;
                for (int i = 39; i < 44; i++)
                {
                    Gear gear = this.character.Equip.GearSlots[i];
                    if (gear != null)
                    {
                        switch(i)
                        {
                            case 39: dx = 1; dy = 1; break;
                            case 40: dx = 1; dy = 2; break;
                            case 41: dx = 2; dy = 2; break;
                            case 42: dx = 0; dy = 3; break;
                            case 43: dx = 1; dy = 3; break;
                            default: continue;
                        }
                        dx = 10 + dx * 33;
                        dy = 22 + dy * 33;
                        drawGearIcon(gear, g, dx, dy);
                    }
                }
            }
            g.ResetTransform();
        }

        private void renderAndroid(Graphics g)
        {
            Rectangle rect = this.AndroidRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.Equip_Android_backgrnd, 0, 0);
            g.DrawImage(Resource.Equip_Android_backgrnd2, 6, 24);
            g.DrawImage(Resource.Equip_Android_backgrnd3, 12, 28);
            g.ResetTransform();
        }

        private void renderTotem(Graphics g)
        {
            Rectangle rect = this.TotemRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_totemEquip_canvastotem, 0, 0);
            g.ResetTransform();
        }

        private void renderSymbol(Graphics g)
        {
            Rectangle rect = this.SymbolRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UIInventory_img_Equip_Symbol_backgrnd, 0, 0);
            if (ArcMode)
            {
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_tabcategoryTab_selected_0, 22, 18);
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_tabcategoryTab_normal_1, 114, 18);
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_ArcEquip_backgrnd, 22, 45);
            }
            else if (AutMode)
            {
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_tabcategoryTab_normal_0, 22, 18);
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_tabcategoryTab_selected_1, 114, 18);
                g.DrawImage(Resource.UIInventory_img_Equip_Symbol_ArcEquip_backgrnd, 22, 45);
            }
            g.ResetTransform();
        }

        private void renderTitleMedal(Graphics g)
        {
            Rectangle rect = this.TitleRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(Resource.UIWindow4_img_Equip_titleSkin_backgrnd, 0, 0);
            g.ResetTransform();
        }

        private void drawGearIcon(Gear gear, Graphics g, int x, int y)
        {
            if (gear == null || g == null)
                return;
            if (gear.State == GearState.disable)
                g.DrawImage(Resource.Equip_character_disabled, x, y);
            Pen pen = GearGraphics.GetGearItemBorderPen(gear.Grade);
            if (pen != null)
            {
                Point[] path = GearGraphics.GetIconBorderPath(x, y);
                g.DrawLines(pen, path);
            }
            g.DrawImage(gear.Icon.Bitmap,
                x - gear.Icon.Origin.X,
                y + 32 - gear.Icon.Origin.Y);
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return btnDeco;
                yield return btnEquip;
                yield return btnPreset1;
                yield return btnPreset2;
                yield return btnPreset3;
                yield return btnPresetApply;
                yield return btnDragon;
                yield return btnMechanic;
                yield return btnTitle;
                yield return btnTotem;
                yield return btnSymbol;
                yield return btnPet;
                yield return btnAndroid;
                yield return btnEquipTab;
                yield return btnPetTab;
                yield return btnDressUpTab;
                yield return btnAndroidTab;
                yield return btnDamageSkinTab;
                yield return btnBeautyRoom;
                yield return btnCoordiPreset;
                yield return btnAndroidShop;
                yield return btnHelp;
                yield return btnClose;
            }
        }

        private IEnumerable<AControl> BeautyControls
        {
            get
            {
                yield return btnHairTab;
                yield return btnFaceTab;
            }
        }

        private void aCtrl_RefreshCall(object sender, EventArgs e)
        {
            this.waitForRefresh = true;
        }

        private void btnDeco_MouseClick(object sender, MouseEventArgs e)
        {
            this.decoVisible = true;
            this.equipVisible = false;
            this.dressupMode = true;
            this.androidMode = false;
            this.damageSkinMode = false;
            this.waitForRefresh = true;
        }

        private void btnEquip_MouseClick(object sender, MouseEventArgs e)
        {
            this.decoVisible = false;
            this.equipVisible = true;
            this.equipMode = true;
            this.petMode = false;
            this.waitForRefresh = true;
        }

        private void btnPresetApply_MouseClick(object sender, MouseEventArgs e)
        {
            this.currentPreset = this.checkPreset;
            this.btnPresetApply.Visible = false;
            this.waitForRefresh = true;
        }

        private void btnPreset1_MouseClick(object sender, MouseEventArgs e)
        {
            this.checkPreset = 1;
            if (this.checkPreset != this.currentPreset)
                this.btnPresetApply.Visible = true;
            this.waitForRefresh = true;
        }
        private void btnPreset2_MouseClick(object sender, MouseEventArgs e)
        {
            this.checkPreset = 2;
            if (this.checkPreset != this.currentPreset)
                this.btnPresetApply.Visible = true;
            this.waitForRefresh = true;
        }
        private void btnPreset3_MouseClick(object sender, MouseEventArgs e)
        {
            this.checkPreset = 3;
            if (this.checkPreset != this.currentPreset)
                this.btnPresetApply.Visible = true;
            this.waitForRefresh = true;
        }

        private void btnTotem_MouseClick(object sender, MouseEventArgs e)
        {
            this.TotemVisible = !this.TotemVisible;
            this.waitForRefresh = true;
        }

        private void btnSymbol_MouseClick(object sender, MouseEventArgs e)
        {
            this.SymbolVisible = !this.SymbolVisible;
            this.ArcMode = true;
            this.AutMode = false;
            this.GrandAutMode = false;
            this.waitForRefresh = true;
        }

        private void btnTitle_MouseClick(object sender, MouseEventArgs e)
        {
            this.TitleMedalVisble = !this.TitleMedalVisble;
            this.waitForRefresh = true;
        }

        private void btnPet_MouseClick(object sender, MouseEventArgs e)
        {
            this.PetVisible = !this.PetVisible;
            this.waitForRefresh = true;
        }

        private void btnDragon_MouseClick(object sender, MouseEventArgs e)
        {
            this.DragonVisible = !this.DragonVisible;
            this.waitForRefresh = true;
        }

        private void btnMechanic_MouseClick(object sender, MouseEventArgs e)
        {
            this.MechanicVisible = !this.MechanicVisible;
            this.waitForRefresh = true;
        }

        private void btnAndroid_MouseClick(object sender, MouseEventArgs e)
        {
            this.AndroidVisible = !this.AndroidVisible;
            this.waitForRefresh = true;
        }

        private void btnBeautyRoom_MouseClick(object sender, MouseEventArgs e)
        {
            this.BeautyRoomVisble = !this.BeautyRoomVisble;
            this.HairMode = true;
            this.FaceMode = false;
            this.waitForRefresh = true;
        }

        private void btnHairTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.HairMode = true;
            this.FaceMode = false;
            this.waitForRefresh = true;
        }

        private void btnFaceTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.HairMode = false;
            this.FaceMode = true;
            this.waitForRefresh = true;
        }

        private void btnCoordiPreset_MouseClick(object sender, MouseEventArgs e)
        {
            this.CoordiPresetVisble = !this.CoordiPresetVisble;
            this.waitForRefresh = true;
        }

        private void btnEquipTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.equipMode = true;
            this.petMode = false;
            this.waitForRefresh = true;
        }

        private void btnPetTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.equipMode = false;
            this.petMode = true;
            this.waitForRefresh = true;
        }
        private void btnDressUpTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.btnEquip.Visible = true;
            this.dressupMode = true;
            this.androidMode = false;
            this.damageSkinMode = false;
            this.waitForRefresh = true;
        }

        private void btnAndroidTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.dressupMode = false;
            this.androidMode = true;
            this.damageSkinMode = false;
            this.waitForRefresh = true;
        }

        private void btnDamageSkinTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.dressupMode = false;
            this.androidMode = false;
            this.damageSkinMode = true;
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
    }
}
