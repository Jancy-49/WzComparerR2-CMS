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
using WzComparerR2.WzLib;
using System.Security.Cryptography;
using CsvHelper;

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
        private ACtrlButton btnException;
        private ACtrlButton btnEnhanceWeapon;
        private ACtrlButton btnCube;
        private ACtrlButton btnexOption;
        private ACtrlButton btnInheritance;
        private ACtrlButton btnNPC;
        private ACtrlButton btnPotential;
        private ACtrlButton btnTitle;
        private ACtrlButton btnTotem;
        private ACtrlButton btnSymbol;
        private ACtrlButton btnArc;
        private ACtrlButton btnAut;
        private ACtrlButton btnPreset1;
        private ACtrlButton btnPreset2;
        private ACtrlButton btnPreset3;
        private ACtrlButton btnPresetApply;
        private ACtrlButton btnEffectSetting;
        private ACtrlButton btnDragonOpen;
        private ACtrlButton btnDragonClose;
        private ACtrlButton btnMechanicOpen;
        private ACtrlButton btnMechanicClose;
        private ACtrlButton btnBeautyRoom;
        private ACtrlButton btnCoordiPreset;
        private ACtrlButton btnAndroidShop;
        private ACtrlButton btnDressUpTab;
        private ACtrlButton btnAndroidTab;
        private ACtrlButton btnDamageSkinTab;
        private ACtrlButton btnHairTab;
        private ACtrlButton btnFaceTab;
        private ACtrlButton btnSkinTab;
        private ACtrlButton btnHelp;
        private ACtrlButton btnClose;

        private bool CMSMode = false;
        private bool KMSMode = false;
        private bool evanMode = false;
        private bool mechanicMode = false;
        private bool zeroMode = false;
        private bool showSpec = false;
        private bool equipVisible = true;
        private bool equipMode = true;
        private bool petMode = false;
        private bool enchanceMode = false;
        private bool dressupMode = false;
        private bool androidMode = false;
        private bool damageSkinMode = false;
        private bool decoVisible = false;
        private bool DragonVisible = false;
        private bool MechanicVisible = false;
        private bool TitleMedalVisble = false;
        private bool BeautyRoomVisble = false;
        private bool CoordiPresetVisble = false;
        private bool GrandAutMode = false;
        private bool HairMode = true;
        private bool FaceMode = false;
        private bool SkinMode = false;
        private int ArcAut = 1;
        private int currentPreset = 1;
        private int checkPreset = 1;
        public int jobID;

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
                    new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/dragonEquip/canvas:dragon"), PluginBase.PluginManager.FindWz).Bitmap.Width - 1,
                    baseOffset.Y + 54),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/dragonEquip/canvas:dragon"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private Rectangle MechanicRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/mechanicEquip/canvas:mechanic"), PluginBase.PluginManager.FindWz).Bitmap.Width - 1,
                    baseOffset.Y + 54),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/mechanicEquip/canvas:mechanic"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private Rectangle TotemRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X,
                    baseOffset.Y + BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Height + 1),
                    Resource.UIInventory_img_Equip_EquipTab_totemEquip_canvastotem.Size);
            }
        }

        private Rectangle SymbolRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X + BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1,
                        baseOffset.Y + 53),
                    Resource.UIInventory_img_Equip_Symbol_backgrnd.Size);
            }
        }

        private Rectangle TitleRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow4.img/Equip/titleSkin/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width - 2,
                    baseOffset.Y + 53),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow4.img/Equip/titleSkin/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private Rectangle BeautyRoomRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/BeautyRoom/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width - 1,
                    baseOffset.Y),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/BeautyRoom/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private Rectangle CooridiPresetRect
        {
            get
            {
                return new Rectangle(
                    new Point(baseOffset.X + BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1,
                    baseOffset.Y + BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Height - BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiPreset/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Height),
                    BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiPreset/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size);
            }
        }

        private void initCtrl()
        {
            this.btnDeco = new ACtrlButton();
            this.btnDeco.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:DecoUI/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnDeco.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:DecoUI/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnDeco.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:DecoUI/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnDeco.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:DecoUI/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnDeco.Location = new Point(251, 72);
            this.btnDeco.Size = new Size(89, 22);
            this.btnDeco.Visible = true;
            this.btnDeco.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDeco.MouseClick += new System.Windows.Forms.MouseEventHandler(btnDeco_MouseClick);

            this.btnEquip = new ACtrlButton();
            this.btnEquip.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:EquipUI/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnEquip.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:EquipUI/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnEquip.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:EquipUI/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnEquip.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:EquipUI/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnEquip.Location = new Point(251, 72);
            this.btnEquip.Size = new Size(89, 22);
            this.btnEquip.Visible = false;
            this.btnEquip.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquip.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEquip_MouseClick);

            this.btnEquipTab = new ACtrlButton();
            this.btnEquipTab.Location = new Point(11, 31);
            this.btnEquipTab.Visible = true;
            this.btnEquipTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEquipTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEquipTab_MouseClick);

            this.btnPetTab = new ACtrlButton();
            this.btnPetTab.Visible = true;
            this.btnPetTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPetTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPetTab_MouseClick);

            this.btnException = new ACtrlButton();
            this.btnException.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/PetTab/button:Exception/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnException.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/PetTab/button:Exception/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnException.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/PetTab/button:Exception/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnException.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/PetTab/button:Exception/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnException.Location = new Point(27, 421);
            this.btnException.Size = new Size(190, 27);
            this.btnException.Visible = false;
            this.btnException.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnEnhanceWeapon = new ACtrlButton();
            this.btnEnhanceWeapon.Location = new Point(244, 31);
            this.btnEnhanceWeapon.Size = new Size(113, 22);
            this.btnEnhanceWeapon.Visible = true;
            this.btnEnhanceWeapon.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEnhanceWeapon.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEnhanceWeapon_MouseClick);

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
            this.btnHairTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnHairTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnHairTab_MouseClick);

            this.btnFaceTab = new ACtrlButton();
            this.btnFaceTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnFaceTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnFaceTab_MouseClick);

            this.btnSkinTab = new ACtrlButton();
            this.btnSkinTab.Location = new Point(278, 31);
            this.btnSkinTab.Size = new Size(132, 23);
            this.btnSkinTab.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSkinTab.MouseClick += new System.Windows.Forms.MouseEventHandler(btnSkinTab_MouseClick);

            this.btnArc = new ACtrlButton();
            this.btnArc.Location = new Point(388, 71);
            this.btnArc.Size = new Size(92, 20);
            this.btnArc.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnArc.MouseClick += new System.Windows.Forms.MouseEventHandler(btnArc_MouseClick);

            this.btnAut = new ACtrlButton();
            this.btnAut.Location = new Point(480, 71);
            this.btnAut.Size = new Size(92, 20);
            this.btnAut.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnAut.MouseClick += new System.Windows.Forms.MouseEventHandler(btnAut_MouseClick);

            this.btnCube = new ACtrlButton();
            this.btnCube.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Cube/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnCube.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Cube/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnCube.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Cube/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnCube.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Cube/disbaled/0"), PluginBase.PluginManager.FindWz);
            this.btnCube.Size = new Size(84, 24);
            this.btnCube.Location = new Point(84, 383);
            this.btnCube.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnexOption = new ACtrlButton();
            this.btnexOption.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:exOption/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnexOption.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:exOption/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnexOption.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:exOption/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnexOption.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:exOption/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnexOption.Size = new Size(84, 24);
            this.btnexOption.Location = new Point(0, 383);
            this.btnexOption.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnInheritance = new ACtrlButton();
            this.btnInheritance.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Inheritance/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnInheritance.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Inheritance/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnInheritance.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Inheritance/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnInheritance.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:Inheritance/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnInheritance.Size = new Size(83, 24);
            this.btnInheritance.Location = new Point(127, 383);
            this.btnInheritance.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnNPC = new ACtrlButton();
            this.btnNPC.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:npc/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnNPC.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:npc/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnNPC.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:npc/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnNPC.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:npc/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnNPC.Size = new Size(112, 24);
            this.btnNPC.Location = new Point(243, 383);
            this.btnNPC.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnPotential = new ACtrlButton();
            this.btnPotential.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:potential/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnPotential.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:potential/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnPotential.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:potential/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnPotential.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:potential/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnPotential.Size = new Size(111, 24);
            this.btnPotential.Location = new Point(12, 383);
            this.btnPotential.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);

            this.btnTitle = new ACtrlButton();
            this.btnTitle.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:titleSkin/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:titleSkin/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:titleSkin/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:titleSkin/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnTitle.Size = new Size(83, 24);
            this.btnTitle.Visible = true;
            this.btnTitle.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTitle.MouseClick += new System.Windows.Forms.MouseEventHandler(btnTitle_MouseClick);


            this.btnTotem = new ACtrlButton();
            this.btnTotem.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_normal_0);
            this.btnTotem.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_pressed_0);
            this.btnTotem.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_mouseOver_0);
            this.btnTotem.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttontotem_disabled_0);
            this.btnTotem.Location = new Point(98, 443);
            this.btnTotem.Size = new Size(84, 23);
            this.btnTotem.Visible = true;
            this.btnTotem.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnTotem.MouseClick += new System.Windows.Forms.MouseEventHandler(btnTotem_MouseClick);

            this.btnSymbol = new ACtrlButton();
            this.btnSymbol.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:symbol/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnSymbol.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:symbol/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnSymbol.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:symbol/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnSymbol.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:symbol/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnSymbol.Size = new Size(84, 23);
            this.btnSymbol.Visible = true;
            this.btnSymbol.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnSymbol.MouseClick += new System.Windows.Forms.MouseEventHandler(btnSymbol_MouseClick);

            this.btnEffectSetting = new ACtrlButton();
            this.btnEffectSetting.Normal = new BitmapOrigin(Resource.UIEquip_img_Equip_EquipTab_buttonEffectSetting_normal_0);
            this.btnEffectSetting.Pressed = new BitmapOrigin(Resource.UIEquip_img_Equip_EquipTab_buttonEffectSetting_pressed_0);
            this.btnEffectSetting.MouseOver = new BitmapOrigin(Resource.UIEquip_img_Equip_EquipTab_buttonEffectSetting_mouseOver_0);
            this.btnEffectSetting.Disabled = new BitmapOrigin(Resource.UIEquip_img_Equip_EquipTab_buttonEffectSetting_disabled_0);
            this.btnEffectSetting.Location = new Point(248, 376);
            this.btnEffectSetting.Size = new Size(92, 25);
            this.btnEffectSetting.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnEffectSetting.MouseClick += new System.Windows.Forms.MouseEventHandler(btnEffectSetting_MouseClick);

            this.btnPresetApply = new ACtrlButton();
            this.btnPresetApply.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_normal_0);
            this.btnPresetApply.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_pressed_0);
            this.btnPresetApply.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_mouseOver_0);
            this.btnPresetApply.Disabled = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpresetApplication_disabled_0);
            this.btnPresetApply.Size = new Size(58, 25);
            this.btnPresetApply.Visible = false;
            this.btnPresetApply.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPresetApply.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPresetApply_MouseClick);

            this.btnPreset1 = new ACtrlButton();
            this.btnPreset1.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_normal_0);
            this.btnPreset1.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_pressed_0);
            this.btnPreset1.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset1_mouseOver_0);
            this.btnPreset1.Size = new Size(18, 18);
            this.btnPreset1.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset1.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset1_MouseClick);

            this.btnPreset2 = new ACtrlButton();
            this.btnPreset2.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_normal_0);
            this.btnPreset2.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_pressed_0);
            this.btnPreset2.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset2_mouseOver_0);
            this.btnPreset2.Size = new Size(18, 18);
            this.btnPreset2.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset2.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset2_MouseClick);

            this.btnPreset3 = new ACtrlButton();
            this.btnPreset3.Normal = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_normal_0);
            this.btnPreset3.Pressed = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_pressed_0);
            this.btnPreset3.MouseOver = new BitmapOrigin(Resource.UIInventory_img_Equip_EquipTab_buttonpreset3_mouseOver_0);
            this.btnPreset3.Size = new Size(18, 18);
            this.btnPreset3.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnPreset3.MouseClick += new System.Windows.Forms.MouseEventHandler(btnPreset3_MouseClick);

            this.btnDragonOpen = new ACtrlButton();
            this.btnDragonOpen.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipOpen/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonOpen.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipOpen/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonOpen.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipOpen/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonOpen.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipOpen/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonOpen.Location = new Point(26, 376);
            this.btnDragonOpen.Size = new Size(92, 25);
            this.btnDragonOpen.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDragonOpen.MouseClick += new MouseEventHandler(btnDragonOpen_MouseClick);

            this.btnDragonClose = new ACtrlButton();
            this.btnDragonClose.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipClose/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonClose.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipClose/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonClose.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipClose/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonClose.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:dragonEquipClose/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnDragonClose.Location = new Point(26, 376);
            this.btnDragonClose.Size = new Size(92, 25);
            this.btnDragonClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnDragonClose.MouseClick += new MouseEventHandler(btnDragonClose_MouseClick);

            this.btnMechanicOpen = new ACtrlButton();
            this.btnMechanicOpen.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipOpen/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicOpen.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipOpen/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicOpen.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipOpen/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicOpen.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipOpen/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicOpen.Location = new Point(26, 376);
            this.btnMechanicOpen.Size = new Size(92, 25);
            this.btnMechanicOpen.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnMechanicOpen.MouseClick += new MouseEventHandler(btnMechanicOpen_MouseClick);

            this.btnMechanicClose = new ACtrlButton();
            this.btnMechanicClose.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipClose/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicClose.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipClose/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicClose.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipClose/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicClose.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/button:mechanicEquipClose/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnMechanicClose.Location = new Point(26, 376);
            this.btnMechanicClose.Size = new Size(92, 25);
            this.btnMechanicClose.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnMechanicClose.MouseClick += new MouseEventHandler(btnMechanicClose_MouseClick);

            this.btnBeautyRoom = new ACtrlButton();
            this.btnBeautyRoom.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:BeautyRoom/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnBeautyRoom.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:BeautyRoom/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnBeautyRoom.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:BeautyRoom/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnBeautyRoom.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:BeautyRoom/disabled/0"), PluginBase.PluginManager.FindWz);
            this.btnBeautyRoom.Location = new Point(11, 414);
            this.btnBeautyRoom.Size = new Size(101, 23);
            this.btnBeautyRoom.Visible = false;
            this.btnBeautyRoom.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
            this.btnBeautyRoom.MouseClick += new MouseEventHandler(btnBeautyRoom_MouseClick);

            this.btnCoordiPreset = new ACtrlButton();
            this.btnCoordiPreset.Normal = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:CoordiPreset/normal/0"), PluginBase.PluginManager.FindWz);
            this.btnCoordiPreset.Pressed = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:CoordiPreset/pressed/0"), PluginBase.PluginManager.FindWz);
            this.btnCoordiPreset.MouseOver = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:CoordiPreset/mouseOver/0"), PluginBase.PluginManager.FindWz);
            this.btnCoordiPreset.Disabled = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiTab/button:CoordiPreset/disabled/0"), PluginBase.PluginManager.FindWz);
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

        private void LoadButton(ACtrlButton aCtrlButton, string path, int offset_x = 0, int offset_y = 0)
        {
            foreach (Wz_Node node in PluginBase.PluginManager.FindWz(path).Nodes)
            {
                string _outlink = PluginBase.PluginManager.FindWz($"path/{node.Text}/0/_outlink").GetValue<string>();
                Wz_Node imgnode = PluginBase.PluginManager.FindWz(_outlink);
                BitmapOrigin image = BitmapOrigin.CreateFromNode(node, PluginBase.PluginManager.FindWz);
                switch (node.Text)
                {
                    case "normal":
                        aCtrlButton.Normal = image;
                        Wz_Vector vector = imgnode.GetValueEx<Wz_Vector>(null);
                        aCtrlButton.Location = new Point((int)vector.X * (-1) + offset_x, (int)vector.Y * (-1) + offset_y);
                        aCtrlButton.Size = image.Bitmap.Size;
                        break;
                    case "pressed":
                        aCtrlButton.Pressed = image;
                        break;
                    case "mouseOver":
                        aCtrlButton.MouseOver = image;
                        break;
                    case "disabled":
                        aCtrlButton.Disabled = image;
                        break;
                }
            }
            aCtrlButton.ButtonStateChanged += new EventHandler(aCtrl_RefreshCall);
        }

        public override void Refresh()
        {
            this.preRender();
            this.SetBitmap(this.Bitmap);
            this.CaptionRectangle = new Rectangle(this.baseOffset, new Size(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width, 24));
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
            //处理按钮可见
            //setControlState();

            //计算图像大小
            Size size = BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/main/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Size;
            CMSMode = size.Height > 444;
            if (!CMSMode) KMSMode = get_KMSmode();
            control_event();
            Point baseOffsetnew = calcRenderBaseOffset();
            size.Width += baseOffsetnew.X;
            if (this.equipVisible)
            {
                if (this.TotemVisible && !KMSMode)
                    size.Height += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/totemEquip/canvas:totem"), PluginBase.PluginManager.FindWz).Bitmap.Height + 1;
                if (this.SymbolVisible)
                    size.Width += Resource.UIInventory_img_Equip_Symbol_backgrnd.Width + 1;
                if (this.TitleMedalVisble)
                    size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow4.img/Equip/titleSkin/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 2;
                if (this.DragonVisible)
                    size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/dragonEquip/canvas:dragon"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
                if (this.MechanicVisible)
                    size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/mechanicEquip/canvas:mechanic"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
            }
            if (decoVisible)
            {
                if (this.BeautyRoomVisble)
                    size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/BeautyRoom/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
                if (this.CoordiPresetVisble)
                    size.Width += BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/CoordiPreset/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1;
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
            if (this.TotemVisible && !KMSMode) renderTotem(g);
            if (this.DragonVisible) renderDragon(g);
            if (this.MechanicVisible) renderMechanic(g);

            g.Dispose();
            this.Bitmap = bitmap;
        }

        private Point calcRenderBaseOffset()
        {
            if (this.TitleMedalVisble)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow4.img/Equip/titleSkin/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1, 0);
            if (this.SymbolVisible)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/Symbol/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width, 0);
            if (this.BeautyRoomVisble)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Deco/BeautyRoom/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1, 0);
            if (this.DragonVisible)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/dragonEquip/canvas:dragon"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1, 0);
            else if (this.MechanicVisible)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/mechanicEquip/canvas:mechanic"), PluginBase.PluginManager.FindWz).Bitmap.Width + 1, 0);
            else if (this.TotemVisible && !KMSMode)
                return new Point(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIEquip.img/Equip/EquipTab/totemEquip/canvas:totem"), PluginBase.PluginManager.FindWz).Bitmap.Width, 0);
            else
                return new Point(0, 0);
        }

        private void control_event()
        {
            switch (this.jobID)
            {
                case 2200: this.evanMode = true; this.mechanicMode = false; this.zeroMode = false; break;
                case 3500: this.evanMode = false; this.mechanicMode = true; this.zeroMode = false; break;
                case 10100: this.evanMode = false; this.mechanicMode = false; this.zeroMode = true; break;
                default: this.evanMode = false; this.mechanicMode = false; this.zeroMode = false; break;
            }
            if (this.equipVisible)
            {
                if (equipMode)
                {
                    this.btnDeco.Visible = true;
                    this.btnEquip.Visible = false;
                    this.btnTotem.Visible = true;
                    this.btnTitle.Visible = true;
                    this.btnSymbol.Visible = true;
                    this.btnPreset1.Visible = true;
                    this.btnPreset2.Visible = true;
                    this.btnPreset3.Visible = true;
                    this.btnPreset1.Location = CMSMode ? new Point(184, 407) : new Point(184, 379);
                    this.btnPreset2.Location = CMSMode ? new Point(214, 407) : new Point(214, 379);
                    this.btnPreset3.Location = CMSMode ? new Point(244, 407) : new Point(244, 379);
                    this.btnTotem.Location = CMSMode ? new Point(98, 443) : new Point(98, 414);
                    this.btnTitle.Location = CMSMode ? new Point(14, 443) : new Point(11, 414);
                    this.btnSymbol.Location = CMSMode ? new Point(272, 443) : new Point(272, 414);
                    this.btnPresetApply.Location = CMSMode ? new Point(282, 404) : new Point(282, 376);
                    this.btnEffectSetting.Visible = CMSMode;
                    if (this.currentPreset == this.checkPreset)
                        this.btnPresetApply.Visible = false;
                    else
                        this.btnPresetApply.Visible = true;
                    if (evanMode || mechanicMode)
                        this.showSpec = true;
                    if (evanMode && !CMSMode)
                    {
                        if (!DragonVisible)
                        {
                            this.btnDragonOpen.Visible = true;
                            this.btnDragonClose.Visible = false;
                        }
                        else
                        {
                            this.btnDragonOpen.Visible = false;
                            this.btnDragonClose.Visible = true;
                        }
                        this.btnMechanicOpen.Visible = false;
                        this.btnMechanicClose.Visible = false;
                    }
                    else if (mechanicMode && !CMSMode && !MechanicVisible)
                    {
                        if (!MechanicVisible)
                        {
                            this.btnMechanicOpen.Visible = true;
                            this.btnMechanicClose.Visible = false;
                        }
                        else
                        {
                            this.btnMechanicOpen.Visible = false;
                            this.btnMechanicClose.Visible = true;
                        }
                        this.btnDragonOpen.Visible = false;
                        this.btnDragonClose.Visible = false;
                    }
                    else if (CMSMode)
                    {
                        this.btnDragonOpen.Visible = false;
                        this.btnDragonClose.Visible = false;
                        this.btnMechanicOpen.Visible = false;
                        this.btnMechanicClose.Visible = false;
                    }
                    this.btnCube.Visible = false;
                    this.btnexOption.Visible = false;
                    this.btnInheritance.Visible = false;
                    this.btnNPC.Visible = false;
                    this.btnPotential.Visible = false;
                    this.btnException.Visible = false;
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
                    this.btnEffectSetting.Visible = false;
                    this.btnDragonOpen.Visible = false;
                    this.btnDragonClose.Visible = false;
                    this.btnMechanicOpen.Visible = false;
                    this.btnMechanicClose.Visible = false;
                    this.btnCube.Visible = false;
                    this.btnexOption.Visible = false;
                    this.btnInheritance.Visible = false;
                    this.btnNPC.Visible = false;
                    this.btnPotential.Visible = false;
                    if (CMSMode)
                        this.btnException.Visible = true;
                }
                else if (enchanceMode)
                {
                    this.btnCube.Visible = true;
                    this.btnexOption.Visible = true;
                    this.btnInheritance.Visible = true;
                    this.btnPotential.Visible = true;
                    this.btnNPC.Visible = true;
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnTitle.Visible = false;
                    this.btnTotem.Visible = false;
                    this.btnSymbol.Visible = false;
                    this.btnPreset1.Visible = false;
                    this.btnPreset2.Visible = false;
                    this.btnPreset3.Visible = false;
                    this.btnPresetApply.Visible = false;
                    this.btnEffectSetting.Visible = false;
                    this.btnDragonOpen.Visible = false;
                    this.btnDragonClose.Visible = false;
                    this.btnMechanicOpen.Visible = false;
                    this.btnMechanicClose.Visible = false;
                    this.btnException.Visible = false;
                }
                this.btnEquipTab.Visible = true;
                this.btnPetTab.Visible = true;
                if (!zeroMode)
                {
                    this.btnEquipTab.Size = new Size(171, 22);
                    this.btnPetTab.Location = new Point(184, 31);
                    this.btnPetTab.Size = new Size(171, 22);
                    this.btnEnhanceWeapon.Visible = false;
                }
                else
                {
                    this.btnEquipTab.Size = new Size(113, 22);
                    this.btnPetTab.Location = new Point(127, 31);
                    this.btnPetTab.Size = new Size(114, 22);
                    this.btnEnhanceWeapon.Visible = true;
                }
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
                    this.btnHelp.Visible = false;
                }
                else if (this.androidMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnBeautyRoom.Visible = false;
                    this.btnCoordiPreset.Visible = false;
                    this.btnAndroidShop.Visible = true;
                    this.btnHelp.Visible = false;
                }
                else if (this.damageSkinMode)
                {
                    this.btnDeco.Visible = false;
                    this.btnEquip.Visible = false;
                    this.btnBeautyRoom.Visible = false;
                    this.btnCoordiPreset.Visible = false;
                    this.btnAndroidShop.Visible = false;
                    this.btnHelp.Visible = true;
                }
                if (this.BeautyRoomVisble)
                {
                    this.btnFaceTab.Visible = true;
                    this.btnHairTab.Visible = true;
                    if (CMSMode)
                    {
                        this.btnHairTab.Size = new Size(199, 23);
                        this.btnFaceTab.Location = new Point(211, 31);
                        this.btnFaceTab.Size = new Size(199, 23);
                        this.btnSkinTab.Visible = false;
                    }
                    else
                    {
                        this.btnHairTab.Size = new Size(132, 23);
                        this.btnFaceTab.Location = new Point(144, 31);
                        this.btnFaceTab.Size = new Size(132, 23);
                        this.btnSkinTab.Visible = true;
                    }
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
                this.btnTotem.Visible = false;
                this.btnEffectSetting.Visible = false;
                this.TitleMedalVisble = false;
                this.SymbolVisible = false;
                this.btnDragonOpen.Visible = false;
                this.btnDragonClose.Visible = false;
                this.btnMechanicOpen.Visible = false;
                this.btnMechanicClose.Visible = false;
                this.btnCube.Visible = false;
                this.btnexOption.Visible = false;
                this.btnInheritance.Visible = false;
                this.btnNPC.Visible = false;
                this.btnPotential.Visible = false;
                this.btnException.Visible = false;
            }
            if (KMSMode)
            {
                this.btnTotem.Visible = false;
                this.btnSymbol.Location = new Point(255, 414);
            }
        }

        private void renderEquip(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/backgrnd", 0, 0);
            if (equipMode) //装备模式
            {
                if (!zeroMode)
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab/selected/0", 11, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab/normal/1", 184, 31);
                }
                else
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/selected/0", 11, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/1", 127, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/2", 244, 31);

                }
                if (!showSpec)
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/canvas:equip", 12, 61);
                else if (KMSMode && zeroMode)
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/canvas:equip3", 12, 61);
                else
                {
                    string _outlink = PluginBase.PluginManager.FindWz("UI/UIEquip.img/Equip/EquipTab/canvas:equip2/_outlink").GetValue<string>(null);
                    render_bitmap(g, _outlink, 12, 61);
                }

                if (this.checkPreset == this.currentPreset)
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/button:presetApplication/disabled/0", 282, CMSMode ? 404 : 376);
                switch (checkPreset)
                {
                    case 1: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 182, CMSMode ? 392 : 364); break;
                    case 2: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 212, CMSMode ? 392 : 364); break;
                    case 3: g.DrawImage(Resource.UIInventory_img_Equip_EquipTab_presetSelected_0, 242, CMSMode ? 392 : 364); break;
                }
                render_slot(g, "UI/UIEquip.img/Equip/EquipTab/SlotName");
                //if (this.character != null) //绘制装备
                //{
                //    for (int i = 0; i < 30; i++)
                //    {
                //        Gear gear = this.character.Equip.GearSlots[i];
                //        if (gear != null)
                //        {
                //            int dx = 10 + i % 5 * 33, dy = 27 + i / 5 * 33;
                //            drawGearIcon(gear, g, dx, dy);
                //        }
                //    }
                //}
            }
            else if (petMode) //宠物模式
            {
                if (!zeroMode)
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab/normal/0", 11, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab/selected/1", 184, 31);

                }
                else
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/0", 11, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/selected/1", 127, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/2", 244, 31);
                }
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/PetTab/canvas:pet", 12, 61);
                render_slot(g, "UI/UIEquip.img/Equip/PetTab/SlotName");
                if (CMSMode)
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/PetTab/AutoSkill/1", 24, 331);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/PetTab/petPotion/1", 24, 353);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/PetTab/PetFood/1", 24, 375);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/PetTab/ItemRoot/1", 24, 397);
                }
            }
            else if (enchanceMode && zeroMode)//神之子武器强化模式
            {
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/ZeroTab/canvas:Zero", 12, 61);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/0", 11, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/normal/1", 127, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/main/tab:detailTab2/selected/2", 244, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/ZeroTab/Npc_R/normal/0", 283, 89, true);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/ZeroTab/Npc_L/normal/0", 12, 87, true);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:ok/disabled/0", 144, 163);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/ZeroTab/button:cancel/disabled/0", 144, 187);
            }
            //if (this.character != null
            //    && (this.character.Status.Job / 100 == 23 || this.character.Status.Job == 2002))
            //{
            //    g.DrawImage(Resource.Equip_character_magicArrow, 142, 126);
            //}

            foreach (AControl aCtrl in this.aControls)
            {
                aCtrl.Draw(g);
            }

            g.ResetTransform();
        }

        private void renderDeco(Graphics g)
        {
            g.TranslateTransform(baseOffset.X, baseOffset.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/backgrnd", 0, 0);
            if (this.dressupMode)
            {
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/selected/0", 11, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/1", 127, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/2", 244, 31);
                if (!zeroMode)
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/CoordiTab/canvas:coordi", 12, 61);
                else
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/CoordiTab/canvas:zero", 12, 61);
                render_slot(g, "UI/UIEquip.img/Deco/CoordiTab/SlotName");
            }
            else if (androidMode)
            {
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/0", 11, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/selected/1", 127, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/2", 244, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/AndroidTab/canvas:and", 12, 61);
                render_slot(g, "UI/UIEquip.img/Deco/AndroidTab/SlotName");
            }
            else if (damageSkinMode)
            {
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/0", 11, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/normal/1", 127, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/main/tab:detailTab/selected/2", 244, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/DamageSkinTab/canvas:and", 12, 61);
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
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/backgrnd", 0, 0);
            if (this.HairMode)
            {
                if (CMSMode)
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/selected/0", 10, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/1", 211, 31);
                }
                else
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/selected/0", 10, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/1", 144, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/2", 278, 31);
                }
            }
            else if (this.FaceMode)
            {
                if (CMSMode)
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/0", 10, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/selected/1", 211, 31);
                }
                else
                {
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/0", 10, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/selected/1", 144, 31);
                    render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/2", 278, 31);
                }
            }
            else if (this.SkinMode && !CMSMode)
            {
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/0", 10, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/normal/1", 144, 31);
                render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/BeautyRoom/BeautyRoom/tab:Tab/selected/2", 278, 31);
            }

            for (int i = 0; i < 6; i++)
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
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Deco/CoordiPreset/backgrnd", 0, 0);
            render_slot(g, "UI/UIEquip.img/Deco/CoordiPreset/SlotName");
            g.ResetTransform();
        }

        private void renderDragon(Graphics g)
        {
            Rectangle rect = this.DragonRect;
            g.TranslateTransform(rect.X, rect.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/dragonEquip/canvas:dragon", 0, 0);
            render_slot(g, "UI/UIEquip.img/Equip/EquipTab/dragonEquip/SlotName");

            //if (this.character != null)
            //{
            //    for (int i = 35; i < 39; i++)
            //    {
            //        Gear gear = this.character.Equip.GearSlots[i];
            //        if (gear != null)
            //        {
            //            int dx = 10 + (i - 35) * 33, dy = 22 + (((i - 1) % 2) + 1) * 33;
            //            drawGearIcon(gear, g, dx, dy);
            //        }
            //    }
            //}
            g.ResetTransform();
        }

        private void renderMechanic(Graphics g)
        {
            Rectangle rect = this.MechanicRect;
            g.TranslateTransform(rect.X, rect.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/mechanicEquip/canvas:mechanic", 0, 0);
            render_slot(g, "UI/UIEquip.img/Equip/EquipTab/mechanicEquip/SlotName");

            //if (this.character != null)
            //{
            //    int dx, dy;
            //    for (int i = 39; i < 44; i++)
            //    {
            //        Gear gear = this.character.Equip.GearSlots[i];
            //        if (gear != null)
            //        {
            //            switch(i)
            //            {
            //                case 39: dx = 1; dy = 1; break;
            //                case 40: dx = 1; dy = 2; break;
            //                case 41: dx = 2; dy = 2; break;
            //                case 42: dx = 0; dy = 3; break;
            //                case 43: dx = 1; dy = 3; break;
            //                default: continue;
            //            }
            //            dx = 10 + dx * 33;
            //            dy = 22 + dy * 33;
            //            drawGearIcon(gear, g, dx, dy);
            //        }
            //    }
            //}
            g.ResetTransform();
        }

        private void renderTotem(Graphics g)
        {
            Rectangle rect = this.TotemRect;
            g.TranslateTransform(rect.X, rect.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/EquipTab/totemEquip/canvas:totem", 0, 0);
            render_slot(g, "UI/UIEquip.img/Equip/EquipTab/totemEquip/SlotName");
            g.ResetTransform();
        }

        private void renderSymbol(Graphics g)
        {
            Rectangle rect = this.SymbolRect;
            g.TranslateTransform(rect.X, rect.Y);
            render_bitmap(g, "UI/_Canvas/UIEquip.img/Equip/Symbol/backgrnd", 0, 0);
            if (ArcAut == 1)
            {
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/tab:symbolTab/selected/0", 22, 18);
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/tab:symbolTab/normal/1", 114, 18);
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/ArcEquip/backgrnd", 22, 45);
                foreach (Wz_Node Node in PluginBase.PluginManager.FindWz("UI/UIEquip.img/Equip/Symbol/ArcEquip/slotPos").Nodes)
                {
                    Wz_Vector Vector = Node.GetValueEx<Wz_Vector>(null);
                    render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/ArcEquip/slotVector1/canvas:slot", Vector.X + 22, Vector.Y + 46);
                }
            }
            else if (ArcAut == 2)
            {
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/tab:symbolTab/normal/0", 22, 18);
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/tab:symbolTab/selected/1", 114, 18);
                render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/AutEquip/backgrnd", 22, 45);
                foreach (Wz_Node Node in PluginBase.PluginManager.FindWz("UI/UIEquip.img/Equip/Symbol/AutEquip/slotPos").Nodes)
                {
                    Wz_Vector Vector = Node.GetValueEx<Wz_Vector>(null);
                    render_bitmap(g, "UI/_Canvas/UICharacterInfo.img/remote/detailEquip/AutEquip/slotVector1/canvas:slot", Vector.X + 22, Vector.Y + 46);
                }
            }

            foreach (AControl aCtrl in this.SymbolControls)
            {
                aCtrl.Draw(g);
            }
            g.ResetTransform();
        }

        private void renderTitleMedal(Graphics g)
        {
            Rectangle rect = this.TitleRect;
            g.TranslateTransform(rect.X, rect.Y);
            g.DrawImage(BitmapOrigin.CreateFromNode(PluginBase.PluginManager.FindWz("UI/_Canvas/UIWindow4.img/Equip/titleSkin/backgrnd"), PluginBase.PluginManager.FindWz).Bitmap, 1, 0);
            g.ResetTransform();
        }

        private void render_bitmap(Graphics g, string nodepath, int x, int y, bool reverse = false)
        {
            Wz_Node Node = PluginBase.PluginManager.FindWz(nodepath);
            Bitmap image = BitmapOrigin.CreateFromNode(Node, PluginBase.PluginManager.FindWz).Bitmap;
            if (reverse)
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            g.DrawImage(image, x, y);
        }

        private void render_slot(Graphics g, string nodepath)
        {
            foreach (Wz_Node Wz_Node in PluginBase.PluginManager.FindWz(nodepath).Nodes)
            {
                string _outlink = Wz_Node.FindNodeByPath("_outlink").GetValue<string>(null);
                Wz_Node imgnode = PluginBase.PluginManager.FindWz(_outlink);
                Bitmap slotimg = BitmapOrigin.CreateFromNode(imgnode, PluginBase.PluginManager.FindWz).Bitmap;
                Wz_Vector Vector = Wz_Node.FindNodeByPath("origin").GetValueEx<Wz_Vector>(null);
                g.DrawImage(slotimg, (int)Vector.X * (-1), (int)Vector.Y * (-1));
            }
        }

        //private void drawGearIcon(Gear gear, Graphics g, int x, int y)
        //{
        //    if (gear == null || g == null)
        //        return;
        //    if (gear.State == GearState.disable)
        //        g.DrawImage(Resource.Equip_character_disabled, x, y);
        //    Pen pen = GearGraphics.GetGearItemBorderPen(gear.Grade);
        //    if (pen != null)
        //    {
        //        Point[] path = GearGraphics.GetIconBorderPath(x, y);
        //        g.DrawLines(pen, path);
        //    }
        //    g.DrawImage(gear.Icon.Bitmap,
        //        x - gear.Icon.Origin.X,
        //        y + 32 - gear.Icon.Origin.Y);
        //}

        private bool get_KMSmode()
        {
            foreach (Wz_Node node in PluginBase.PluginManager.FindWz("UI/UIEquip.img/Equip/EquipTab").Nodes)
            {
                if (node.Text == "totemEquip")
                    return false;
            }
            return true;
        }

        private IEnumerable<AControl> aControls
        {
            get
            {
                yield return btnDeco;
                yield return btnEquip;
                if (zeroMode)
                {
                    yield return btnEnhanceWeapon;
                    yield return btnCube;
                    yield return btnexOption;
                    yield return btnInheritance;
                    yield return btnNPC;
                    yield return btnPotential;
                }
                yield return btnPreset1;
                yield return btnPreset2;
                yield return btnPreset3;
                yield return btnPresetApply;
                if (!CMSMode && evanMode)
                {
                    if (!DragonVisible) yield return btnDragonOpen;
                    if (DragonVisible) yield return btnDragonClose;
                }
                else if (!CMSMode && mechanicMode)
                {
                    if (!MechanicVisible) yield return btnMechanicOpen;
                    if (MechanicVisible) yield return btnMechanicClose;
                }
                else if (CMSMode)
                {
                    yield return btnEffectSetting;
                    yield return btnException;
                }
                yield return btnTitle;
                yield return btnTotem;
                yield return btnSymbol;
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
                if (!CMSMode)
                    yield return btnSkinTab;
            }
        }

        private IEnumerable<AControl> SymbolControls
        {
            get
            {
                yield return btnArc;
                yield return btnAut;
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
            this.enchanceMode = false;
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
            this.ArcAut = 1;
            this.GrandAutMode = false;
            this.waitForRefresh = true;
        }

        private void btnTitle_MouseClick(object sender, MouseEventArgs e)
        {
            this.TitleMedalVisble = !this.TitleMedalVisble;
            this.waitForRefresh = true;
        }

        private void btnEffectSetting_MouseClick(Object sender, MouseEventArgs e)
        {
            if (this.jobID == 2200)
            {
                this.DragonVisible = !this.DragonVisible;
            }
            else if (this.jobID == 3500)
            {
                this.MechanicVisible = !this.MechanicVisible;
            }
            this.waitForRefresh = true;
        }

        private void btnDragonOpen_MouseClick(object sender, MouseEventArgs e)
        {
            this.DragonVisible = true;
            this.waitForRefresh = true;
        }

        private void btnDragonClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.DragonVisible = false;
            this.waitForRefresh = true;
        }

        private void btnMechanicOpen_MouseClick(object sender, MouseEventArgs e)
        {
            this.MechanicVisible = true;
            this.waitForRefresh = true;
        }

        private void btnMechanicClose_MouseClick(object sender, MouseEventArgs e)
        {
            this.MechanicVisible = false;
            this.waitForRefresh = true;
        }

        private void btnArc_MouseClick(object sender, MouseEventArgs e)
        {
            this.ArcAut = 1;
            this.waitForRefresh = true;
        }

        private void btnAut_MouseClick(object sender, MouseEventArgs e)
        {
            this.ArcAut = 2;
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
            this.SkinMode = false;
            this.waitForRefresh = true;
        }

        private void btnFaceTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.HairMode = false;
            this.FaceMode = true;
            this.SkinMode = false;
            this.waitForRefresh = true;
        }

        private void btnSkinTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.HairMode = false;
            this.FaceMode = false;
            this.SkinMode = true;
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
            this.enchanceMode = false;
            this.waitForRefresh = true;
        }

        private void btnPetTab_MouseClick(object sender, MouseEventArgs e)
        {
            this.equipMode = false;
            this.petMode = true;
            this.enchanceMode = false;
            this.waitForRefresh = true;
        }

        private void btnEnhanceWeapon_MouseClick(object sender, MouseEventArgs e)
        {
            this.equipMode = false;
            this.petMode = false;
            this.enchanceMode = true;
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

            MouseEventArgs BeautyChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BeautyRoomRect.X, e.Y - BeautyRoomRect.Y, e.Delta);

            foreach (AControl ctrl in this.BeautyControls)
            {
                ctrl.OnMouseMove(BeautyChildArgs);
            }

            foreach (AControl ctrl in this.SymbolControls)
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

            MouseEventArgs BeautyChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BeautyRoomRect.X, e.Y - BeautyRoomRect.Y, e.Delta);

            foreach (AControl ctrl in this.BeautyControls)
            {
                ctrl.OnMouseDown(BeautyChildArgs);
            }

            foreach (AControl ctrl in this.SymbolControls)
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

            MouseEventArgs BeautyChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BeautyRoomRect.X, e.Y - BeautyRoomRect.Y, e.Delta);

            foreach (AControl ctrl in this.BeautyControls)
            {
                ctrl.OnMouseUp(BeautyChildArgs);
            }

            foreach (AControl ctrl in this.SymbolControls)
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

            MouseEventArgs BeautyChildArgs = new MouseEventArgs(e.Button, e.Clicks, e.X - BeautyRoomRect.X, e.Y - BeautyRoomRect.Y, e.Delta);

            foreach (AControl ctrl in this.BeautyControls)
            {
                ctrl.OnMouseClick(BeautyChildArgs);
            }

            foreach (AControl ctrl in this.SymbolControls)
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
