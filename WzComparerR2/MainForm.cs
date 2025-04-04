﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Xml;
using Timer = System.Timers.Timer;
using System.Threading;
using System.Threading.Tasks;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.AdvTree;
using WzComparerR2.WzLib;
using WzComparerR2.Common;
using WzComparerR2.CharaSimControl;
using WzComparerR2.PluginBase;
using WzComparerR2.CharaSim;
using WzComparerR2.Comparer;
using WzComparerR2.Controls;
using WzComparerR2.Rendering;
using WzComparerR2.Config;
using WzComparerR2.Animation;
using static Microsoft.Xna.Framework.MathHelper;
using System.Net.Http;
using HtmlAgilityPack;
//using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WzComparerR2
{
    public partial class MainForm : Office2007RibbonForm, PluginContextProvider
    {
        public MainForm()
        {
            InitializeComponent();
#if NET6_0_OR_GREATER
            // https://learn.microsoft.com/en-us/dotnet/core/compatibility/fx-core#controldefaultfont-changed-to-segoe-ui-9pt
            this.Font = new Font(new FontFamily("宋体"), 9f);
#endif
            Form.CheckForIllegalCrossThreadCalls = false;
            this.MinimumSize = new Size(600, 450);
            advTree1.AfterNodeSelect += new AdvTreeNodeEventHandler(advTree1_AfterNodeSelect_2);
            advTree2.AfterNodeSelect += new AdvTreeNodeEventHandler(advTree2_AfterNodeSelect_2);
            //new ImageDragHandler(this.pictureBox1).AttachEvents();
            RegisterPluginEvents();
            createStyleItems();
            initFields();
            loadUIState();
            textBoxAPIKey.Text = Properties.Settings.Default.APIKey;
        }

        List<Wz_Structure> openedWz;
        StringLinker stringLinker;
        HistoryList<Node> historyNodeList;
        bool historySelecting;

        //soundPlayer
        BassSoundPlayer soundPlayer;
        Timer soundTimer;
        bool timerChangeValue;

        //charaSim
        AfrmTooltip tooltipQuickView;
        CharaSimControlGroup charaSimCtrl;
        AdvTree lastSelectedTree;
        DefaultLevel skillDefaultLevel = DefaultLevel.Level0;
        int skillInterval = 32;

        //compare
        Thread compareThread;

        private void initFields()
        {
            openedWz = new List<Wz_Structure>();
            stringLinker = new StringLinker();
            historyNodeList = new HistoryList<Node>();

            tooltipQuickView = new AfrmTooltip();
            tooltipQuickView.Visible = false;
            tooltipQuickView.StringLinker = this.stringLinker;
            tooltipQuickView.KeyDown += new KeyEventHandler(afrm_KeyDown);
            tooltipQuickView.ShowID = true;
            tooltipQuickView.ShowMenu = true;

            charaSimCtrl = new CharaSimControlGroup();
            charaSimCtrl.StringLinker = this.stringLinker;
            charaSimCtrl.Character = new Character();
            charaSimCtrl.Character.Name = "WzComparerR2";
            charaSimCtrl.UIItem.Visible = false;
            charaSimCtrl.UIItem.VisibleChanged += new EventHandler(afrm_VisibleChanged);
            charaSimCtrl.UIStat.Visible = false;
            charaSimCtrl.UIStat.VisibleChanged += new EventHandler(afrm_VisibleChanged);
            charaSimCtrl.UIEquip.Visible = false;
            charaSimCtrl.UIEquip.VisibleChanged += new EventHandler(afrm_VisibleChanged);
            charaSimCtrl.UIUnion.Visible = false;
            charaSimCtrl.UIUnion.VisibleChanged += new EventHandler(afrm_VisibleChanged);

            string[] images = new string[] { "dir", "mp3", "num", "png", "str", "uol", "vector", "img", "rawdata", "convex", "video" };
            foreach (string img in images)
            {
                imageList1.Images.Add(img, (Image)Properties.Resources.ResourceManager.GetObject(img));
            }

            soundPlayer = new BassSoundPlayer();
            if (!soundPlayer.Init())
            {
                ManagedBass.Errors error = soundPlayer.GetLastError();
                MessageBoxEx.Show("Bass音频播放器错误\r\n\r\nerrorCode : " + (int)error + "(" + error + ")", "错误");
            }
            soundTimer = new Timer(120d);
            soundTimer.Elapsed += new System.Timers.ElapsedEventHandler(soundTimer_Elapsed);
            soundTimer.Enabled = true;

            PluginBase.PluginManager.WzFileFinding += new FindWzEventHandler(CharaSimLoader_WzFileFinding);

            foreach (WzPngComparison comp in Enum.GetValues(typeof(WzPngComparison)))
            {
                cmbComparePng.Items.Add(comp);
            }
            cmbComparePng.SelectedItem = WzPngComparison.SizeAndDataLength;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            saveUIState();
            // 保存 APIKey 值
            Properties.Settings.Default.APIKey = textBoxAPIKey.Text;
            Properties.Settings.Default.Save();
        }

        private void saveUIState()
        {
            UIStateConfig.Default.WindowState = (int)this.WindowState;
            UIStateConfig.Default.WindowWidth = this.Size.Width;
            UIStateConfig.Default.WindowHeight = this.Size.Height;
            UIStateConfig.Default.RibbonExpanded = this.ribbonControl1.Expanded;
            UIStateConfig.Default.SelectedRibbonTabIndex = this.ribbonControl1.SelectedRibbonTabItem.Name.Last() - '0';
            UIStateConfig.Default.SplitterPosition1 = this.expandableSplitter1.SplitPosition;
            UIStateConfig.Default.SplitterPosition2 = this.expandableSplitter2.SplitPosition;
            UIStateConfig.Default.ColumnWidth3 = this.columnHeader3.Width.Absolute;
            UIStateConfig.Default.ColumnWidth4 = this.columnHeader4.Width.Absolute;
            UIStateConfig.Default.ColumnWidth5 = this.columnHeader5.Width.Absolute;
            UIStateConfig.Default.ColumnWidth6 = this.columnHeader6.Width;
            UIStateConfig.Default.ColumnWidth7 = this.columnHeader7.Width;
            UIStateConfig.Default.ColumnWidth8 = this.columnHeader8.Width;
            UIStateConfig.Default.ColumnWidth9 = this.columnHeader9.Width;
            UIStateConfig.Default.BarLayout = this.dotNetBarManager1.LayoutDefinition;
            ConfigManager.Save();
        }

        private void loadUIState()
        {
            try
            {
                this.WindowState = (FormWindowState)UIStateConfig.Default.WindowState.Value;
                this.Size = new Size(UIStateConfig.Default.WindowWidth, UIStateConfig.Default.WindowHeight);
                if (this.ribbonControl1.Expanded = UIStateConfig.Default.RibbonExpanded)
                {
                    switch (UIStateConfig.Default.SelectedRibbonTabIndex)
                    {
                        case 1: this.ribbonControl1.SelectedRibbonTabItem = this.ribbonTabItem1; break;
                        case 2: this.ribbonControl1.SelectedRibbonTabItem = this.ribbonTabItem2; break;
                        case 3: this.ribbonControl1.SelectedRibbonTabItem = this.ribbonTabItem3; break;
                        default: this.ribbonControl1.SelectFirstVisibleRibbonTab(); break;
                    }
                }
                this.expandableSplitter1.SplitPosition = UIStateConfig.Default.SplitterPosition1;
                this.expandableSplitter2.SplitPosition = UIStateConfig.Default.SplitterPosition2;
                this.columnHeader3.Width.Absolute = UIStateConfig.Default.ColumnWidth3;
                this.columnHeader4.Width.Absolute = UIStateConfig.Default.ColumnWidth4;
                this.columnHeader5.Width.Absolute = UIStateConfig.Default.ColumnWidth5;
                this.columnHeader6.Width = UIStateConfig.Default.ColumnWidth6;
                this.columnHeader7.Width = UIStateConfig.Default.ColumnWidth7;
                this.columnHeader8.Width = UIStateConfig.Default.ColumnWidth8;
                this.columnHeader9.Width = UIStateConfig.Default.ColumnWidth9;
                this.dotNetBarManager1.LayoutDefinition = UIStateConfig.Default.BarLayout;
            }
            catch (Exception ex)
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(1200, 800); // = new Size(766, 520);
                this.ribbonControl1.Expanded = false; // = false;
                this.expandableSplitter1.SplitPosition = 468; // = 233;
                this.expandableSplitter2.SplitPosition = 230; // = 255;
                this.columnHeader3.Width.Absolute = 150;
                this.columnHeader4.Width.Absolute = 150;
                this.columnHeader5.Width.Absolute = 150;
                this.columnHeader6.Width = 80;
                this.columnHeader7.Width = 200; // = 100
                this.columnHeader8.Width = 600; // = 350
                this.columnHeader9.Width = 250; // = 150
                this.dotNetBarManager1.LayoutDefinition = "<dotnetbarlayout version=\"6\" zorder=\"7,8,1,0\"><docksite size=\"0\" dockingside=\"Top\" originaldocksitesize=\"0\" /><docksite size=\"182\" dockingside=\"Bottom\" originaldocksitesize=\"0\"><dockcontainer orientation=\"1\" w=\"0\" h=\"0\"><barcontainer w=\"1184\" h=\"179\"><bar name=\"bar1\" dockline=\"0\" layout=\"2\" dockoffset=\"0\" state=\"2\" dockside=\"4\" visible=\"true\"><items><item name=\"dockContainerItem1\" origBar=\"\" origPos=\"-1\" pos=\"0\" /></items></bar></barcontainer></dockcontainer></docksite><docksite size=\"0\" dockingside=\"Left\" originaldocksitesize=\"0\" /><docksite size=\"0\" dockingside=\"Right\" originaldocksitesize=\"0\" /><bars /></dotnetbarlayout>";
            }
        }

        /// <summary>
        /// 插件加载时执行的方法，用于初始化配置文件。
        /// </summary>
        internal void PluginOnLoad()
        {
            ConfigManager.RegisterAllSection();
            var conf = ImageHandlerConfig.Default;
            //刷新最近打开文件列表
            refreshRecentDocItems();
            //读取CharaSim配置
            UpdateCharaSimSettings();
            //wz加载配置
            UpdateWzLoadingSettings();
            //Translator Configuration Load
            UpdateTranslateSettings();

            //杂项配置
            labelItemAutoSaveFolder.Text = ImageHandlerConfig.Default.AutoSavePictureFolder;
            buttonItemAutoSave.Checked = ImageHandlerConfig.Default.AutoSaveEnabled;
            comboBoxItemLanguage.SelectedIndex = Clamp(CharaSimConfig.Default.SelectedFontIndex, 0, comboBoxItemLanguage.Items.Count);


            //更新界面颜色
            styleManager1.ManagerStyle = WcR2Config.Default.MainStyle;
            UpdateButtonItemStyles();
            styleManager1.ManagerColorTint = WcR2Config.Default.MainStyleColor;
        }

        void UpdateCharaSimSettings()
        {
            var Setting = CharaSimConfig.Default;
            this.buttonItemAutoQuickView.Checked = Setting.AutoQuickView;
            tooltipQuickView.SkillRender.ShowProperties = Setting.Skill.ShowProperties;
            tooltipQuickView.SkillRender.ShowObjectID = Setting.Skill.ShowID;
            tooltipQuickView.SkillRender.ShowDelay = Setting.Skill.ShowDelay;
            tooltipQuickView.SkillRender.DisplayCooltimeMSAsSec = Setting.Skill.DisplayCooltimeMSAsSec;
            tooltipQuickView.SkillRender.DisplayPermyriadAsPercent = Setting.Skill.DisplayPermyriadAsPercent;
            tooltipQuickView.SkillRender.IgnoreEvalError = Setting.Skill.IgnoreEvalError;
            this.skillDefaultLevel = Setting.Skill.DefaultLevel;
            this.skillInterval = Setting.Skill.IntervalLevel;
            tooltipQuickView.GearRender.ShowObjectID = Setting.Gear.ShowID;
            tooltipQuickView.GearRender.ShowSpeed = Setting.Gear.ShowWeaponSpeed;
            tooltipQuickView.GearRender.ShowLevelOrSealed = Setting.Gear.ShowLevelOrSealed;
            tooltipQuickView.GearRender.ShowMedalTag = Setting.Gear.ShowMedalTag;
            tooltipQuickView.GearRender.AutoTitleWrap = Setting.Gear.AutoTitleWrap;
            tooltipQuickView.ItemRender.ShowObjectID = Setting.Item.ShowID;
            tooltipQuickView.ItemRender.LinkRecipeInfo = Setting.Item.LinkRecipeInfo;
            tooltipQuickView.ItemRender.LinkRecipeItem = Setting.Item.LinkRecipeItem;
            tooltipQuickView.ItemRender.ShowLevelOrSealed = Setting.Gear.ShowLevelOrSealed;
            tooltipQuickView.ItemRender.ShowNickTag = Setting.Item.ShowNickTag;
            tooltipQuickView.RecipeRender.ShowObjectID = Setting.Recipe.ShowID;
        }

        void UpdateWzLoadingSettings()
        {
            var config = WcR2Config.Default;
            Encoding enc;
            try
            {
                enc = Encoding.GetEncoding(config.WzEncoding);
            }
            catch
            {
                enc = null;
            }
            Wz_Structure.DefaultEncoding = enc;
            Wz_Structure.DefaultAutoDetectExtFiles = config.AutoDetectExtFiles;
            Wz_Structure.DefaultImgCheckDisabled = config.ImgCheckDisabled;
            Wz_Structure.DefaultWzVersionVerifyMode = config.WzVersionVerifyMode;
        }

        void UpdateTranslateSettings()
        {
            var config = WcR2Config.Default;
            Translator.DefaultDesiredLanguage = config.DesiredLanguage;
            Translator.DefaultMozhiBackend = config.MozhiBackend;
<<<<<<< HEAD
            Translator.DefaultLanguageModel = config.LanguageModel;
            Translator.OAITranslateBaseURL = config.OpenAIBackend;
=======
<<<<<<< HEAD
            Translator.DefaultLanguageModel = config.LanguageModel;
            Translator.OAITranslateBaseURL = config.OpenAIBackend;
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            Translator.DefaultPreferredTranslateEngine = config.PreferredTranslateEngine;
            Translator.DefaultTranslateAPIKey = config.NxSecretKey;
            Translator.DefaultPreferredLayout = config.PreferredLayout;
            Translator.IsTranslateEnabled = (config.PreferredLayout > 0);
            Translator.DefaultDetectCurrency = config.DetectCurrency;
            Translator.DefaultDesiredCurrency = config.DesiredCurrency;
<<<<<<< HEAD
            Translator.DefaultLMTemperature = config.LMTemperature;
            Translator.DefaultMaximumToken = config.MaximumToken;
            Translator.IsExtraParamEnabled = config.OpenAIExtraOption;
=======
<<<<<<< HEAD
            Translator.DefaultLMTemperature = config.LMTemperature;
            Translator.DefaultMaximumToken = config.MaximumToken;
            Translator.IsExtraParamEnabled = config.OpenAIExtraOption;
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            Translator.ExchangeTable = null;
        }

        void CharaSimLoader_WzFileFinding(object sender, FindWzEventArgs e)
        {
            string[] fullPath = null;
            if (!string.IsNullOrEmpty(e.FullPath)) //用fullpath作为输入参数
            {
                fullPath = e.FullPath.Split('/', '\\');
                e.WzType = Enum.TryParse<Wz_Type>(fullPath[0], true, out var wzType) ? wzType : Wz_Type.Unknown;
            }

            List<Wz_Node> preSearch = new List<Wz_Node>();
            if (e.WzType != Wz_Type.Unknown) //用wztype作为输入参数
            {
                IEnumerable<Wz_Structure> preSearchWz = e.WzFile?.WzStructure != null ?
                    Enumerable.Repeat(e.WzFile.WzStructure, 1) :
                    this.openedWz;
                foreach (var wzs in preSearchWz)
                {
                    Wz_File baseWz = null;
                    bool find = false;
                    foreach (Wz_File wz_f in wzs.wz_files)
                    {
                        if (wz_f.Type == e.WzType)
                        {
                            if (e.HasChildNodes && wz_f.Node.Nodes.Count <= 0)
                            {
                                continue;
                            }
                            preSearch.Add(wz_f.Node);
                            find = true;
                            //e.WzFile = wz_f;
                        }
                        if (wz_f.Type == Wz_Type.Base)
                        {
                            baseWz = wz_f;
                        }
                    }

                    // detect data.wz
                    if (baseWz != null && !find)
                    {
                        string key = e.WzType.ToString();
                        foreach (Wz_Node node in baseWz.Node.Nodes)
                        {
                            if (node.Text == key && node.Nodes.Count > 0)
                            {
                                preSearch.Add(node);
                            }
                        }
                    }
                }
            }

            if (fullPath == null || fullPath.Length <= 1)
            {
                if (e.WzType != Wz_Type.Unknown && preSearch.Count > 0) //返回wzFile
                {
                    e.WzNode = preSearch[0];
                    e.WzFile = preSearch[0].Value as Wz_File;
                }
                return;
            }

            if (preSearch.Count <= 0)
            {
                return;
            }

            foreach (var wzFileNode in preSearch)
            {
                var searchNode = wzFileNode;
                for (int i = 1; i < fullPath.Length && searchNode != null; i++)
                {
                    searchNode = searchNode.Nodes[fullPath[i]];
                    var img = searchNode.GetValueEx<Wz_Image>(null);
                    if (img != null)
                    {
                        searchNode = img.TryExtract() ? img.Node : null;
                    }
                }

                if (searchNode != null)
                {
                    e.WzNode = searchNode;
                    e.WzFile = wzFileNode.Value as Wz_File;
                    return;
                }
            }
            //寻找失败
            e.WzNode = null;
        }

        #region 界面主题配置
        private void createStyleItems()
        {
            //添加菜单
            foreach (eStyle style in Enum.GetValues(typeof(eStyle)).OfType<eStyle>().Distinct())
            {
                var buttonItemStyle = new ButtonItem() { Tag = style, Text = style.ToString(), Checked = (styleManager1.ManagerStyle == style) };
                buttonItemStyle.Click += new EventHandler(buttonItemStyle_Click);
                this.buttonItemStyle.SubItems.Add(buttonItemStyle);
            }

            var styleColorPicker = new ColorPickerDropDown() { Text = "StyleColorTint", BeginGroup = true, SelectedColor = styleManager1.ManagerColorTint };
            styleColorPicker.SelectedColorChanged += new EventHandler(styleColorPicker_SelectedColorChanged);
            buttonItemStyle.SubItems.Add(styleColorPicker);
        }

        private void buttonItemStyle_Click(object sender, EventArgs e)
        {
            var style = (eStyle)((sender as ButtonItem).Tag);
            styleManager1.ManagerStyle = style;
            UpdateButtonItemStyles();
            ConfigManager.Reload();
            WcR2Config.Default.MainStyle = style;
            ConfigManager.Save();
        }

        private void UpdateButtonItemStyles()
        {
            foreach (BaseItem item in buttonItemStyle.SubItems)
            {
                ButtonItem buttonItem = item as ButtonItem;
                if (buttonItem != null)
                {
                    buttonItem.Checked = (buttonItem.Tag as eStyle?) == styleManager1.ManagerStyle;
                }
            }
        }

        private void styleColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            var color = (sender as ColorPickerDropDown).SelectedColor;
            styleManager1.ManagerColorTint = color;
            ConfigManager.Reload();
            WcR2Config.Default.MainStyleColor = color;
            ConfigManager.Save();
        }
        #endregion

        #region 读取wz相关方法
        private Node createNode(Wz_Node wzNode)
        {
            if (wzNode == null)
                return null;

            Node parentNode = new Node(wzNode.Text) { Tag = new WeakReference(wzNode) };
            foreach (Wz_Node subNode in wzNode.Nodes)
            {
                Node subTreeNode = createNode(subNode);
                if (subTreeNode != null)
                    parentNode.Nodes.Add(subTreeNode);
            }
            return parentNode;
        }

        private void sortWzNode(Wz_Node wzNode)
        {
            this.sortWzNode(wzNode, WcR2Config.Default.SortWzByImgID);
        }

        private void sortWzNode(Wz_Node wzNode, bool sortByImgID)
        {
            if (wzNode.Nodes.Count > 1)
            {
                if (sortByImgID)
                {
                    wzNode.Nodes.SortByImgID();
                }
                else
                {
                    wzNode.Nodes.Sort();
                }
            }
            foreach (Wz_Node subNode in wzNode.Nodes)
            {
                sortWzNode(subNode, sortByImgID);
            }
        }
        #endregion

        #region wz提取右侧
        private void cmbItemAniNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbItemAniNames.SelectedIndex > -1 && this.pictureBoxEx1.Items.Count > 0)
            {
                if (this.pictureBoxEx1.Items[0] is ISpineAnimator aniItem)
                {
                    string aniName = this.cmbItemAniNames.SelectedItem as string;
                    aniItem.SelectedAnimationName = aniName;
                    this.cmbItemAniNames.Tooltip = aniName;
                }
                var aniItem2 = this.pictureBoxEx1.Items[0] as Animation.MultiFrameAnimator;
                if (aniItem2 != null)
                {
                    string aniName = this.cmbItemAniNames.SelectedItem as string;
                    aniItem2.SelectedAnimationName = aniName;
                    this.cmbItemAniNames.Tooltip = aniName;
                }
            }
        }

        private void cmbItemSkins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbItemSkins.SelectedIndex > -1 && this.pictureBoxEx1.Items.Count > 0)
            {
                if (this.pictureBoxEx1.Items[0] is ISpineAnimator aniItem)
                {
                    string skinName = this.cmbItemSkins.SelectedItem as string;
                    aniItem.SelectedSkin = skinName;
                    this.cmbItemSkins.Tooltip = skinName;
                }
            }
        }

        private void buttonItemSaveImage_Click(object sender, EventArgs e)
        {
            this.OnSaveImage(false);
        }

        private void buttonItemSaveWithOptions_Click(object sender, EventArgs e)
        {
            this.OnSaveImage(true);
        }

        private Node handleUol(Node currentNode, string uolString)
        {
            if (currentNode == null || currentNode.Parent == null || string.IsNullOrEmpty(uolString))
                return null;
            string[] dirs = uolString.Split('/');
            currentNode = currentNode.Parent;

            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                if (dir == "..")
                {
                    currentNode = currentNode.Parent;
                }
                else
                {
                    bool find = false;
                    foreach (Node child in currentNode.Nodes)
                    {
                        if (child.Text == dir)
                        {
                            currentNode = child;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                        currentNode = null;
                }
                if (currentNode == null)
                    return null;
            }
            return currentNode;
        }

        private void labelItemAutoSaveFolder_Click(object sender, EventArgs e)
        {
            string dir = ImageHandlerConfig.Default.AutoSavePictureFolder;
            if (!string.IsNullOrEmpty(dir))
            {
                System.Diagnostics.Process.Start("explorer.exe", dir);
            }
        }

        private void buttonItemGif_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;

            Wz_Node node = advTree3.SelectedNode.AsWzNode();
            string aniName = GetSelectedNodeImageName();

            //添加到动画控件
            var spineDetectResult = SpineLoader.Detect(node);
            if (spineDetectResult.Success)
            {
                var spineData = this.pictureBoxEx1.LoadSpineAnimation(spineDetectResult);

                if (spineData != null)
                {
                    this.pictureBoxEx1.ShowAnimation(spineData);
                    var aniItem = this.pictureBoxEx1.Items[0] as ISpineAnimator;

                    this.cmbItemAniNames.Items.Clear();
                    this.cmbItemAniNames.Items.Add("");
                    this.cmbItemAniNames.Items.AddRange(aniItem.Animations.ToArray());
                    this.cmbItemAniNames.SelectedIndex = 0;

                    this.cmbItemSkins.Visible = true;
                    this.cmbItemSkins.Items.Clear();
                    this.cmbItemSkins.Items.AddRange(aniItem.Skins.ToArray());
                    this.cmbItemSkins.SelectedIndex = aniItem.Skins.IndexOf(aniItem.SelectedSkin);
                }
            }
            else
            {
                var options = (sender == this.buttonItemExtractGifEx) ? FrameAnimationCreatingOptions.ScanAllChildrenFrames: default;
                var frameData = this.pictureBoxEx1.LoadFrameAnimation(node, options);

                if (frameData != null)
                {
                    this.pictureBoxEx1.ShowAnimation(frameData);
                    this.cmbItemAniNames.Items.Clear();
                    this.cmbItemSkins.Visible = false;
                }
                else
                {
                    var multiData = this.pictureBoxEx1.LoadMultiFrameAnimation(node);

                    if (multiData != null)
                    {
                        this.pictureBoxEx1.ShowAnimation(multiData);
                        var aniItem = this.pictureBoxEx1.Items[0] as Animation.MultiFrameAnimator;

                        this.cmbItemAniNames.Items.Clear();
                        this.cmbItemAniNames.Items.AddRange(aniItem.Animations.ToArray());
                        this.cmbItemAniNames.SelectedIndex = 0;
                    }
                }
            }
            this.pictureBoxEx1.PictureName = aniName;
        }

        private void buttonItemGif2_Click(object sender, EventArgs e)
        {
            // code from buttonItemGif_Click()
            if (advTree3.SelectedNode == null)
                return;

            Wz_Node node = advTree3.SelectedNode.AsWzNode();
            string aniName = "嵌套_" + GetSelectedNodeImageName();

            if (node.Value is Wz_Png)
            {
                var pngFrameData = this.pictureBoxEx1.LoadPngFrameAnimation(node);

                if (pngFrameData != null)
                {
                    this.pictureBoxEx1.ShowOverlayAnimation(pngFrameData, true);
                    this.cmbItemAniNames.Items.Clear();
                    this.cmbItemSkins.Visible = false;
                    this.pictureBoxEx1.PictureName = aniName;
                }

                return;
            }

            //添加到动画控件
            if (node.Text.EndsWith(".atlas", StringComparison.OrdinalIgnoreCase))
            {
                /*
                var spineData = this.pictureBoxEx1.LoadSpineAnimation(node);
                if (spineData != null)
                {
                    this.pictureBoxEx1.ShowAnimation(spineData);
                    var aniItem = this.pictureBoxEx1.Items[0] as Animation.SpineAnimator;
                    this.cmbItemAniNames.Items.Clear();
                    this.cmbItemAniNames.Items.Add("");
                    this.cmbItemAniNames.Items.AddRange(aniItem.Animations.ToArray());
                    this.cmbItemAniNames.SelectedIndex = 0;
                    this.cmbItemSkins.Visible = true;
                    this.cmbItemSkins.Items.Clear();
                    this.cmbItemSkins.Items.AddRange(aniItem.Skins.ToArray());
                    this.cmbItemSkins.SelectedIndex = aniItem.Skins.IndexOf(aniItem.SelectedSkin);
                }
                */
                MessageBoxEx.Show("Spine动画不可重合。", "未支持");
                return;
            }
            else
            {
                var frameData = this.pictureBoxEx1.LoadFrameAnimation(node);

                if (frameData != null)
                {
                    this.pictureBoxEx1.ShowOverlayAnimation(frameData);
                    this.cmbItemAniNames.Items.Clear();
                    this.cmbItemSkins.Visible = false;
                    this.pictureBoxEx1.PictureName = aniName;
                }
                else
                {
                    var multiData = this.pictureBoxEx1.LoadMultiFrameAnimation(node);

                    if (multiData != null)
                    {
                        /*
                        this.pictureBoxEx1.ShowAnimation(multiData);
                        var aniItem = this.pictureBoxEx1.Items[0] as Animation.MultiFrameAnimator;
                        this.cmbItemAniNames.Items.Clear();
                        this.cmbItemAniNames.Items.AddRange(aniItem.Animations.ToArray());
                        this.cmbItemAniNames.SelectedIndex = 0;
                        */
                        MessageBoxEx.Show("Multi帧动画无法重合。", "未支持");
                        return;
                    }
                }
            }
            //this.pictureBoxEx1.PictureName = aniName;
        }

        private string GetSelectedNodeImageName()
        {
            Wz_Node node = advTree3.SelectedNode.AsWzNode();

            string aniName;
            switch (ImageHandlerConfig.Default.ImageNameMethod.Value)
            {
                default:
                case ImageNameMethod.Default:
                    advTree3.PathSeparator = ".";
                    aniName = advTree3.SelectedNode.FullPath;
                    break;

                case ImageNameMethod.PathToImage:
                    aniName = node.FullPath.Replace('\\', '.');
                    break;

                case ImageNameMethod.PathToWz:
                    aniName = node.FullPathToFile.Replace('\\', '.');
                    break;
            }

            return aniName;
        }

        private void buttonItemGifSetting_Click(object sender, EventArgs e)
        {
            FrmGifSetting frm = new FrmGifSetting();
            frm.Load(ImageHandlerConfig.Default);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ConfigManager.Reload();
                frm.Save(ImageHandlerConfig.Default);
                ConfigManager.Save();
            }
        }

        private void buttonDisableOverlayAni_Click(object sender, EventArgs e)
        {
            if (this.pictureBoxEx1.ShowOverlayAni)
            {
                this.pictureBoxEx1.ShowOverlayAni = false;
                this.pictureBoxEx1.Items.Clear();
            }
        }

        private void buttonOverlayRect_Click(object sender, EventArgs e)
        {
            if (this.pictureBoxEx1.ShowOverlayAni)
            {
                this.pictureBoxEx1.AddOverlayRect();
            }
        }

        private void buttonItemAutoSave_Click(object sender, EventArgs e)
        {
            ConfigManager.Reload();
            ImageHandlerConfig.Default.AutoSaveEnabled = buttonItemAutoSave.Checked;
            ConfigManager.Save();
        }

        private void buttonItemAutoSaveFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "请选择自动导出动画的文件夹。";
                dlg.SelectedPath = ImageHandlerConfig.Default.AutoSavePictureFolder;
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    labelItemAutoSaveFolder.Text = dlg.SelectedPath;
                    ConfigManager.Reload();
                    ImageHandlerConfig.Default.AutoSavePictureFolder = dlg.SelectedPath;
                    ConfigManager.Save();
                }
            }
        }

        private void OnSaveImage(bool options)
        {
            if (this.pictureBoxEx1.Items.Count <= 0)
            {
                return;
            }

            var aniItem = this.pictureBoxEx1.Items[0];
            var frameData = (aniItem as FrameAnimator)?.Data;
            if (frameData != null && frameData.Frames.Count == 1 
                && frameData.Frames[0].A0 == 255 && frameData.Frames[0].A1 == 255 && frameData.Frames[0].Delay == 0 || pictureBoxEx1.ShowOverlayAni)
            {
                // save still picture as png
                this.OnSavePngFile(frameData.Frames[0]);
            }
            else
            {
                // save as gif/apng
                this.OnSaveGifFile(aniItem, options);
            }
        }

        private void OnSavePngFile(Frame frame)
        {
            if (frame.Png != null)
            {
                var config = ImageHandlerConfig.Default;
                string pngFileName = pictureBoxEx1.PictureName + ".png";

                if (config.AutoSaveEnabled)
                {
                    pngFileName = Path.Combine(config.AutoSavePictureFolder, string.Join("_", pngFileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.None)));
                }
                else
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filter = "PNG (*.png)|*.png|全部文件 (*.*)|*.*";
                    dlg.FileName = pngFileName;
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    pngFileName = dlg.FileName;
                }

                using (var bmp = frame.Png.ExtractPng())
                {
                    bmp.Save(pngFileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                labelItemStatus.Text = "导出动画完毕: " + pngFileName;
            }
            else if (pictureBoxEx1.ShowOverlayAni && frame.Texture != null) // 嵌套动画
            {
                var config = ImageHandlerConfig.Default;
                string pngFileName = pictureBoxEx1.PictureName + ".png";

                if (config.AutoSaveEnabled)
                {
                    pngFileName = Path.Combine(config.AutoSavePictureFolder, string.Join("_", pngFileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.None)));
                }
                else
                {
                    var dlg = new SaveFileDialog();
                    dlg.Filter = "PNG (*.png)|*.png|全部文件 (*.*)|*.*";
                    dlg.FileName = pngFileName;
                    if (dlg.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    pngFileName = dlg.FileName;
                }

                byte[] frameData = new byte[frame.Texture.Width * frame.Texture.Height * 4];
                frame.Texture.GetData(frameData);
                var targetSize = new Point(frame.Texture.Width, frame.Texture.Height);
                unsafe
                {
                    fixed (byte* pFrameBuffer = frameData)
                    {
                        using (var bmp = new System.Drawing.Bitmap(targetSize.X, targetSize.Y, targetSize.X * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, new IntPtr(pFrameBuffer)))
                        {
                            bmp.Save(pngFileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                labelItemStatus.Text = "导出动画完毕: " + pngFileName;
            }
            else
            {
                labelItemStatus.Text = "导出动画失败。";
            }
        }

        private void OnSaveGifFile(AnimationItem aniItem, bool options)
        {
            var config = ImageHandlerConfig.Default;
            var encParams = AnimateEncoderFactory.GetEncoderParams(config.GifEncoder.Value);

            string aniName = this.cmbItemAniNames.SelectedItem as string;
            string aniFileName = pictureBoxEx1.PictureName
                    + (string.IsNullOrEmpty(aniName) ? "" : ("." + aniName))
                    + encParams.FileExtension;

            if (config.AutoSaveEnabled)
            {
                var fullFileName = Path.Combine(config.AutoSavePictureFolder, string.Join("_", aniFileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.None)));
                int i = 1;
                while (File.Exists(fullFileName))
                {
                    fullFileName = Path.Combine(config.AutoSavePictureFolder, string.Format("{0}({1}){2}",
                        Path.GetFileNameWithoutExtension(aniFileName), i, Path.GetExtension(aniFileName)));
                    i++;
                }
                aniFileName = fullFileName;
            }
            else
            {
                var dlg = new SaveFileDialog();

                dlg.Filter = string.Format("{0} (*{1})|*{1}|全部文件 (*.*)|*.*", encParams.FileDescription, encParams.FileExtension);
                dlg.FileName = aniFileName;

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                aniFileName = dlg.FileName;
            }

            var clonedAniItem = (AnimationItem)aniItem.Clone();
            if (this.pictureBoxEx1.SaveAsGif(clonedAniItem, aniFileName, config, options))
            {
                labelItemStatus.Text = "导出动画完毕: " + aniFileName;
            }
        }
        #endregion

        #region File菜单的事件
        private void btnItemOpenWz_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "打开Wz";
                dlg.Filter = "Base.wz|*.wz";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    openWz(dlg.FileName);
                }
            }
        }

        private void openWz(string wzFilePath)
        {
            foreach (Wz_Structure wzs in openedWz)
            {
                foreach (Wz_File wz_f in wzs.wz_files)
                {
                    if (string.Compare(wz_f.Header.FileName, wzFilePath, true) == 0)
                    {
                        MessageBoxEx.Show("已经打开的wz文件。", "错误");
                        return;
                    }
                }
            }

            Wz_Structure wz = new Wz_Structure();
            QueryPerformance.Start();
            advTree1.BeginUpdate();
            try
            {
                if (wz.IsKMST1125WzFormat(wzFilePath))
                {
                    wz.LoadKMST1125DataWz(wzFilePath);
                    string packsDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(wzFilePath)), "Packs");
                    if (Directory.Exists(packsDir))
                    {
                        foreach (var msFile in Directory.GetFiles(packsDir, "*.ms"))
                        {
                            wz.LoadMsFile(msFile);
                        }
                    }
                }
                else
                {
                    wz.Load(wzFilePath, true);
                }

                if (WcR2Config.Default.SortWzOnOpened)
                {
                    sortWzNode(wz.WzNode);
                }
                Node node = createNode(wz.WzNode);
                node.Expand();
                advTree1.Nodes.Add(node);
                this.openedWz.Add(wz);
                OnWzOpened(new WzStructureEventArgs(wz)); //触发事件
                QueryPerformance.End();
                labelItemStatus.Text = "Wz打开完毕: 用时 " + (Math.Round(QueryPerformance.GetLastInterval(), 4) * 1000) + "ms, " + wz.img_number + "个img";

                ConfigManager.Reload();
                WcR2Config.Default.RecentDocuments.Remove(wzFilePath);
                WcR2Config.Default.RecentDocuments.Insert(0, wzFilePath);
                ConfigManager.Save();
                refreshRecentDocItems();
            }
            catch (FileNotFoundException)
            {
                MessageBoxEx.Show("无法查找文件。", "错误");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.ToString(), "错误");
                wz.Clear();
            }
            finally
            {
                advTree1.EndUpdate();
            }
        }

        private void btnItemOpenImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "打开Img...";
                dlg.Filter = "*.img|*.img|*.wz|*.wz";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    openImg(dlg.FileName);
                }
            }
        }

        private void openImg(string imgFileName)
        {
            foreach (Wz_Structure wzs in openedWz)
            {
                foreach (Wz_File wz_f in wzs.wz_files)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(wz_f.Header.FileName, imgFileName))
                    {
                        MessageBoxEx.Show("已经打开的wz文件。", "错误");
                        return;
                    }
                }
            }

            Wz_Structure wz = new Wz_Structure();
            var sw = Stopwatch.StartNew();
            advTree1.BeginUpdate();
            try
            {
                wz.LoadImg(imgFileName);

                Node node = createNode(wz.WzNode);
                node.Expand();
                advTree1.Nodes.Add(node);
                this.openedWz.Add(wz);
                OnWzOpened(new WzStructureEventArgs(wz)); //触发事件
                sw.Stop();
                labelItemStatus.Text = $"打开Img完毕: 用时 {sw.ElapsedMilliseconds}ms";
                refreshRecentDocItems();
            }
            catch (FileNotFoundException)
            {
                MessageBoxEx.Show("无法查找文件。", "错误");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.ToString(), "错误");
                wz.Clear();
            }
            finally
            {
                advTree1.EndUpdate();
            }
        }

        private void buttonItemClose_Click(object sender, EventArgs e)
        {
            if (advTree1.SelectedNode == null)
            {
                MessageBoxEx.Show("不存在要关闭的wz文件。", "错误");
                return;
            }
            Node baseWzNode = advTree1.SelectedNode;
            while (baseWzNode.Parent != null)
                baseWzNode = baseWzNode.Parent;
            if (baseWzNode.Text.ToLower() == "list.wz")
            {
                advTree1.Nodes.Remove(baseWzNode);
                labelItemStatus.Text = "List.wz无法使用。";
                return;
            }

            Wz_File wz_f = advTree1.SelectedNode.AsWzNode()?.GetNodeWzFile();
            if (wz_f == null)
            {
                MessageBoxEx.Show("请选择正确的wz文件。", "错误");
                return;
            }
            Wz_Structure wz = wz_f.WzStructure;

            advTree1.Nodes.Remove(baseWzNode);

            listViewExWzDetail.Items.Clear();

            Wz_Image image = null;
            if (advTree2.Nodes.Count > 0
                && (image = advTree2.Nodes[0].AsWzNode()?.GetValue<Wz_Image>()) != null
                && image.WzFile.WzStructure == wz)
            {
                advTree2.Nodes.Clear();
            }

            if (advTree3.Nodes.Count > 0
                && (image = advTree3.Nodes[0].AsWzNode()?.GetNodeWzImage()) != null
                && image.WzFile.WzStructure == wz)
            {
                advTree3.Nodes.Clear();
            }

            OnWzClosing(new WzStructureEventArgs(wz));
            wz.Clear();
            if (this.openedWz.Remove(wz))
                labelItemStatus.Text = "Wz关闭完毕";
            else
                labelItemStatus.Text = "Wz关闭失败: 发生未知错误";
        }

        private void buttonItemCloseAll_Click(object sender, EventArgs e)
        {
            advTree1.ClearAndDisposeAllNodes();
            advTree2.ClearAndDisposeAllNodes();
            advTree3.ClearAndDisposeAllNodes();
            foreach (Wz_Structure wz in openedWz)
            {
                OnWzClosing(new WzStructureEventArgs(wz));
                wz.Clear();
            }
            openedWz.Clear();
            CharaSimLoader.ClearAll();
            stringLinker.Clear();
            labelItemStatus.Text = "全部关闭完毕";
            GC.Collect();
        }

        private void refreshRecentDocItems()
        {
            List<BaseItem> items = new List<BaseItem>();
            foreach (BaseItem item in galleryContainerRecent.SubItems)
            {
                if (item is ButtonItem)
                {
                    items.Add(item);
                }
            }
            galleryContainerRecent.SubItems.RemoveRange(items.ToArray());
            items.Clear();

            foreach (var doc in WcR2Config.Default.RecentDocuments)
            {
                ButtonItem item = new ButtonItem() { Text = "&" + (items.Count + 1) + ". " + Path.GetFileName(doc), Tooltip = doc, Tag = doc };
                item.Click += new EventHandler(buttonItemRecentDocument_Click);
                items.Add(item);
            }
            galleryContainerRecent.SubItems.AddRange(items.ToArray());
        }

        void buttonItemRecentDocument_Click(object sender, EventArgs e)
        {
            ButtonItem btnItem = sender as ButtonItem;
            string path;
            if (btnItem == null || (path = btnItem.Tag as string) == null)
                return;
            openWz(path);
        }
        #endregion

        #region wzView和提取的事件和方法
        private void advTree1_DragEnter(object sender, DragEventArgs e)
        {
            string[] types = e.Data.GetFormats();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) != ".wz")
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void advTree1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    openWz(file);
                }
            }
        }

        private void advTree1_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
        {
            Wz_Node selectedNode = e.Node.AsWzNode();

            if (selectedNode == null)
            {
                return;
            }

            listViewExWzDetail.BeginUpdate();
            listViewExWzDetail.Items.Clear();

            if (selectedNode.Value == null)
            {
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "目录名称", Path.GetFileName(e.Node.Text) }));
                autoResizeColumns(listViewExWzDetail);
            }
            else if (selectedNode.Value is Wz_File wzFile)
            {
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "文件名", wzFile.Header.FileName }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "文件大小", wzFile.Header.FileSize + " 字节" }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "版权", wzFile.Header.Copyright }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "版本", wzFile.GetMergedVersion().ToString() }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "Wz类型", wzFile.IsSubDir ? "SubDir" : wzFile.Type.ToString() }));

                foreach (Wz_File subFile in wzFile.MergedWzFiles)
                {
                    listViewExWzDetail.Items.Add(" ");
                    listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "文件名", subFile.Header.FileName }));
                    listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "文件大小", subFile.Header.FileSize + " 字节" }));
                    listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "版权", subFile.Header.Copyright }));
                    listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "版本", subFile.Header.WzVersion.ToString() }));
                }

                autoResizeColumns(listViewExWzDetail);
            }
            else if (selectedNode.Value is Wz_Image wzImage)
            {
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "图像名称", wzImage.Name }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "图像大小", wzImage.Size + " 字节" }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "图像偏移", wzImage.Offset + " 字节" }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "路径", wzImage.Node.FullPathToFile }));
                listViewExWzDetail.Items.Add(new ListViewItem(new string[] { "校验和", wzImage.Checksum.ToString() }));
                autoResizeColumns(listViewExWzDetail);

                advTree2.ClearAndDisposeAllNodes();
                //advTree2.Nodes.Clear();

                QueryPerformance.Start();
                try
                {
                    Exception ex;
                    if (wzImage.TryExtract(out ex))
                    {
                        advTree2.Nodes.Add(createNode(wzImage.Node));
                        advTree2.Nodes[0].Expand();
                        QueryPerformance.End();
                        double ms = (Math.Round(QueryPerformance.GetLastInterval(), 4) * 1000);

                        labelItemStatus.Text = "加载完成: 用时 " + ms + "ms";
                    }
                    else
                    {

                        labelItemStatus.Text = "加载失败：" + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    labelItemStatus.Text = "加载失败：" + ex.Message;
                }
            }
            listViewExWzDetail.EndUpdate();
        }

        private void autoResizeColumns(ListViewEx listView)
        {
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (System.Windows.Forms.ColumnHeader column in listView.Columns)
            {
                column.Width += (int)(listView.Font.Size * 2);
            }
        }

        private void advTree2_NodeDoubleClick(object sender, TreeNodeMouseEventArgs e)
        {
            if (e.Node == null || e.Button != MouseButtons.Left)
                return;
            historyNodeList.Clear();
            advTree3.Nodes.Clear();

            var selectedNode = e.Node.AsWzNode();
            if (selectedNode != null)
            {
                advTree3.BeginUpdate();
                try
                {
                    var node = createNodeDetail(e.Node);
                    node.ExpandAll();
                    advTree3.Nodes.Add(node);
                    advTree3.SelectedNode = node;
                }
                finally
                {
                    advTree3.EndUpdate();
                }
            }
        }

        private Node createNodeDetail(Node parentNode)
        {
            Node newNode = new Node(parentNode.Text);
            newNode.Tag = parentNode.Tag;
            Wz_Node wzNode = newNode.AsWzNode();
            if (wzNode != null)
            {
                newNode.Cells.Add(new Cell(wzNode.Value == null ? "<" + parentNode.Nodes.Count + ">" : getValueString(wzNode.Value)));
                newNode.Cells.Add(new Cell(wzNode.Value == null ? null : wzNode.Value.GetType().Name));
                newNode.ImageKey = wzNode.Value == null ? "dir" : (getValueImageKey(wzNode.Value) ?? "num");
            }
            foreach (Node subNode in parentNode.Nodes)
            {
                newNode.Nodes.Add(createNodeDetail(subNode));
            }
            return newNode;
        }

        private string getValueString(object value)
        {
            switch (value)
            {
                case Wz_Png png:
                    return $"png {png.Width}*{png.Height} ({png.Form})";

                case Wz_Vector vector:
                    return $"({vector.X}, {vector.Y})";

                case Wz_Uol uol:
                    return uol.Uol;

                case Wz_Sound sound:
                    return $"音频 {sound.Ms}ms";

                case Wz_Image img:
                    return $"<{img.Node.Nodes.Count}>";

                case Wz_RawData rawData:
                    return $"rawdata {rawData.Length}";

                case Wz_Convex convex:
                    return $"convex [{convex.Points.Length}]";

                case Wz_Video video:
                    return $"video {video.Length}";

                default:
                    string cellVal = Convert.ToString(value);
                    if (cellVal != null && cellVal.Length > 50)
                    {
                        cellVal = cellVal.Substring(0, 50);
                    }
                    return cellVal;
            }
        }

        private string getValueImageKey(object value)
        {
            return value switch
            {
                string => "str",
                short or int or long or float or double=> "num",
                Wz_Png => "png",
                Wz_Vector => "vector",
                Wz_Uol => "uol",
                Wz_Sound sound => sound.SoundType == Wz_SoundType.Binary ? "rawdata" : "mp3",
                Wz_Image => "img",
                Wz_RawData => "rawdata",
                Wz_Convex => "convex",
                Wz_Video => "video",
                _ => null
            };
        }

        private void advTree3_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
        {
            if (e.Node == null)
                return;

            if (!historySelecting && (historyNodeList.Count == 0 || e.Node != historyNodeList.Current))
            {
                historyNodeList.Add(e.Node);
            }
            else
            {
                historySelecting = false;
            }

            Wz_Node selectedNode = e.Node.AsWzNode();
            if (selectedNode == null)
                return;

            switch (selectedNode.Value)
            {
                case Wz_Png png:
                    pictureBoxEx1.PictureName = GetSelectedNodeImageName();
                    pictureBoxEx1.ShowImage(png);
                    this.cmbItemAniNames.Items.Clear();
                    advTree3.PathSeparator = ".";
                    textBoxX1.Text = "dataLength: " + png.DataLength + " bytes\r\n" +
                        "offset: " + png.Offset + "\r\n" +
                        "size: " + png.Width + "*" + png.Height + "\r\n" +
                        "png format: " + png.Form;

                    var sourceNode = selectedNode.GetLinkedSourceNode(PluginManager.FindWz);
                    if (sourceNode != selectedNode)
                    {
                        png = sourceNode.GetValueEx<Wz_Png>(null);
                        if (png != null)
                        {
                            string linkStr = Convert.ToString((selectedNode.Nodes["source"] ?? selectedNode.Nodes["_inlink"] ?? selectedNode.Nodes["_outlink"])?.Value);
                            if (linkStr != null && linkStr.Contains("\n") && !linkStr.Contains("\r\n"))
                            {
                                linkStr = linkStr.Replace("\n", "\r\n");
                            }
                            textBoxX1.AppendText("\r\n\r\n" + Convert.ToString(linkStr));

                            pictureBoxEx1.PictureName = GetSelectedNodeImageName();
                            pictureBoxEx1.ShowImage(png);
                            this.cmbItemAniNames.Items.Clear();
                            advTree3.PathSeparator = ".";
                            textBoxX1.AppendText("\r\n\r\ndataLength: " + png.DataLength + " bytes\r\n" +
                                "offset: " + png.Offset + "\r\n" +
                                "size: " + png.Width + "*" + png.Height + "\r\n" +
                                "png format: " + png.Form);
                        }
                    }
                    break;

                case Wz_Vector vector:
                    textBoxX1.Text = "x: " + vector.X + " px\r\n" +
                        "y: " + vector.Y + " px";
                    break;

                case Wz_Convex convex:
                    var sb = new StringBuilder();
                    for (int i = 0; i < convex.Points.Length; i++)
                    {
                        if (i > 0) sb.AppendLine();
                        sb.AppendFormat("({0}, {1})", convex.Points[i].X, convex.Points[i].Y);
                    }
                    textBoxX1.Text = sb.ToString();
                    break;

                case Wz_Uol uol:
                    textBoxX1.Text = "uolPath: " + uol.Uol;
                    break;

                case Wz_Sound sound:
                    preLoadSound(sound, selectedNode.Text);
                    textBoxX1.Text = "dataLength: " + sound.DataLength + " bytes\r\n" +
                        "offset: " + sound.Offset + "\r\n" +
                        "duration: " + sound.Ms + " ms\r\n" +
                        "channels: " + sound.Channels + "\r\n" +
                        "freq: " + sound.Frequency + " Hz\r\n" +
                        "type: " + sound.SoundType.ToString();
                    break;

                case Wz_Image:
                    //do nothing;
                    break;

                case Wz_RawData rawData:
                    textBoxX1.Text = "dataLength: " + rawData.Length + " bytes\r\n" +
                        "offset: " + rawData.Offset;
                    break;

                case Wz_Video video:
                    textBoxX1.Text = "dataLength: " + video.Length + " bytes\r\n" +
                        "offset: " + video.Offset;
                    break;

                default:
                    string valueStr = Convert.ToString(selectedNode.Value);
                    if (valueStr != null && valueStr.Contains("\n") && !valueStr.Contains("\r\n"))
                    {
                        valueStr = valueStr.Replace("\n", "\r\n");
                    }
                    textBoxX1.Text = Convert.ToString(valueStr);

                    switch (selectedNode.Text)
                    {
                        case "source":
                        case "_inlink":
                        case "_outlink":
                            {
                                var parentNode = selectedNode.ParentNode;
                                if (parentNode != null && parentNode.Value is Wz_Png)
                                {
                                    var linkNode = parentNode.GetLinkedSourceNode(PluginManager.FindWz);
                                    var png = linkNode.GetValueEx<Wz_Png>(null);

                                    if (png != null)
                                    {
                                        pictureBoxEx1.PictureName = GetSelectedNodeImageName();
                                        pictureBoxEx1.ShowImage(png);
                                        this.cmbItemAniNames.Items.Clear();
                                        advTree3.PathSeparator = ".";
                                        textBoxX1.AppendText("\r\n\r\ndataLength: " + png.DataLength + " bytes\r\n" +
                                        "offset: " + png.Offset + "\r\n" +
                                        "size: " + png.Width + "*" + png.Height + "\r\n" +
                                            "png format: " + png.Form);
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*
            if (pictureBox1.Image != null && e.Button == MouseButtons.Left)
            {
                string tempFile = Path.Combine(Path.GetTempPath(), Convert.ToString(pictureBox1.Tag));
                switch (Path.GetExtension(tempFile))
                {
                    case ".png":
                        pictureBox1.Image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Png);
                        System.Diagnostics.Process.Start(tempFile);
                        break;
                    case ".gif":
                        pictureBox1.Image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Gif);
                        System.Diagnostics.Process.Start(tempFile);
                        break;
                    default:
                        MessageBoxEx.Show("不识别的文件名：" + tempFile, "喵~");
                        break;
                }
            }*/
        }

        private void listViewExString_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.listViewExStringFind();
            }
        }

        private void listViewExString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.listViewExStringFind();
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                this.listViewExStringCopy();
            }
        }

        private void listViewExStringFind()
        {
            if (listViewExString.SelectedItems.Count == 0 || advTree1.Nodes.Count == 0)
            {
                return;
            }
            string id = listViewExString.SelectedItems[0].Text;
            string nodePath = listViewExString.SelectedItems[0].SubItems[3].Text;
            List<string[]> objPathList = detectObjPathByStringPath(id, nodePath);

            //分离wz路径和img路径
            foreach (string[] fullPath in objPathList)
            {
                //寻找所有可能的wzfile
                List<Wz_Node> allWzFile = new List<Wz_Node>();
                Wz_Type wzType = ParseType(fullPath[0]);
                foreach (var wzs in this.openedWz)
                {
                    foreach (var wzf in wzs.wz_files)
                    {
                        if (wzf.Type == wzType && wzf.OwnerWzFile == null)
                        {
                            allWzFile.Add(wzf.Node);
                        }
                    }
                }

                //开始搜索
                foreach (var wzFileNode in allWzFile)
                {
                    Wz_Node node = SearchNode(wzFileNode, fullPath, 1);
                    if (node != null)
                    {
                        OnSelectedWzNode(node); //遇到第一个 选中 返回
                        return;
                    }
                }
            }

            //失败
            string path;
            if (objPathList.Count == 1)
            {
                path = string.Join("\\", objPathList[0]);
            }
            else
            {
                path = "(" + objPathList.Count + ")个节点";
            }
            labelItemStatus.Text = "imageNode加载失败: " + path;
        }

        private Wz_Node SearchNode(Wz_Node parent, string[] path, int startIndex)
        {
            if (startIndex >= path.Length)
            {
                return null;
            }
            if (parent.Value is Wz_Image)
            {
                Wz_Image img = parent.GetValue<Wz_Image>();
                if (!img.TryExtract())
                {
                    return null;
                }
                parent = img.Node;
            }
            string nodeName = path[startIndex];
            if (!string.IsNullOrEmpty(nodeName))
            {
                Wz_Node child = parent.FindNodeByPath(false, true, nodeName);
                if (child != null)
                {
                    return (startIndex == path.Length - 1) ? child : SearchNode(child, path, startIndex + 1);
                }
            }
            else //遍历全部
            {
                foreach (Wz_Node child in parent.Nodes)
                {
                    if (child.Nodes.Count == 0) //只过滤文件夹 未来有需求再改
                    {
                        continue;
                    }
                    Wz_Node find = SearchNode(child, path, startIndex + 1);
                    if (find != null)
                    {
                        return (startIndex == path.Length - 1) ? null : find;
                    }

                }
            }

            return null;
        }

        private bool OnSelectedWzNode(Wz_Node wzNode)
        {
            Wz_File wzFile = wzNode.GetNodeWzFile();
            string[] path = wzNode.FullPathToFile.Split('\\');
            if (wzFile == null)
            {
                return false;
            }

            Node treeNode = findWzFileTreeNode(wzFile);
            if (treeNode == null)
            {
                return false;
            }

            for (int i = 1; i < path.Length; i++)
            {
                Node find = null;
                foreach (Node child in treeNode.Nodes)
                {
                    if (child.Text == path[i])
                    {
                        find = child;
                        break;
                    }
                }
                if (find == null)
                {
                    return false;
                }

                if (find.AsWzNode()?.Value is Wz_Image)
                {
                    advTree1.SelectedNode = find;
                    if (advTree2.Nodes.Count > 0)
                    {
                        treeNode = advTree2.Nodes[0];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    treeNode = find;
                }
            }

            advTree2.SelectedNode = treeNode;
            return true;
        }

        private void listViewExStringCopy()
        {
            if (listViewExString.SelectedItems.Count == 0 || advTree1.Nodes.Count == 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem.ListViewSubItem item in listViewExString.SelectedItems[0].SubItems)
            {
                sb.Append(item.Text).Append(" ");
            }
            sb.Remove(sb.Length - 1, 1);
            Clipboard.SetText(sb.ToString(), TextDataFormat.UnicodeText);
            labelItemStatus.Text = "已从剪切板复制。";
        }

        private List<string[]> detectObjPathByStringPath(string id, string stringNodePath)
        {
            List<string[]> pathList = new List<string[]>();

            List<string> wzPath = new List<string>();
            List<string> imagePath = new List<string>();

            Action addPath = () =>
            {
                List<string> fullPath = new List<string>(wzPath.Count + imagePath.Count);
                fullPath.AddRange(wzPath);
                fullPath.AddRange(imagePath);
                pathList.Add(fullPath.ToArray());
            };

            string[] pathArray = stringNodePath.Split('\\');
            switch (pathArray[0])
            {
                case "Cash.img":
                case "Consume.img":
                case "Etc.img":
                case "Pet.img":
                    wzPath.Add("Item");
                    wzPath.Add(pathArray[0].Substring(0, pathArray[0].IndexOf(".img")));
                    if (pathArray[0] == "Pet.img")
                    {
                        wzPath.Add(id.TrimStart('0') + ".img");
                    }
                    else
                    {
                        id = id.PadLeft(8, '0');
                        wzPath.Add(id.Substring(0, 4) + ".img");
                        imagePath.Add(id);
                    }
                    addPath();
                    break;

                case "Ins.img": //KMST1066
                    wzPath.Add("Item");
                    wzPath.Add("Install");
                    wzPath.Add("");
                    id = id.PadLeft(8, '0');
                    imagePath.Add(id);
                    for (int len = 4; len <= 6; len++)
                    {
                        wzPath[2] = id.Substring(0, len) + ".img";
                        addPath();
                    }
                    break;

                case "Eqp.img":
                    wzPath.Add("Character");
                    if (pathArray[2] == "Taming")
                    {
                        wzPath.Add("TamingMob");
                    }
                    else if (pathArray[2] != "Skin")
                    {
                        wzPath.Add(pathArray[2]);
                    }
                    wzPath.Add(id.PadLeft(8, '0') + ".img");
                    addPath();
                    //往往这个不靠谱。。 加一个任意门备用
                    wzPath[1] = "";
                    addPath();
                    break;

                case "Map.img":
                    id = id.PadLeft(9, '0');
                    wzPath.AddRange(new string[] { "Map", "Map", "Map" + id[0], id + ".img" });
                    addPath();
                    break;

                case "Mob.img":
                    wzPath.Add("Mob");
                    wzPath.Add(id.PadLeft(7, '0') + ".img");
                    addPath();
                    break;

                case "Npc.img":
                    wzPath.Add("Npc");
                    wzPath.Add(id.PadLeft(7, '0') + ".img");
                    addPath();
                    break;

                case "Skill.img":
                    id = id.PadLeft(7, '0');
                    wzPath.Add("Skill");
                    //old skill
                    wzPath.Add(id.Substring(0, id.Length - 4) + ".img");
                    imagePath.Add("skill");
                    imagePath.Add(id);
                    addPath();
                    if (Regex.IsMatch(id, @"80\d{6}")) //kmst new skill
                    {
                        wzPath[1] = id.Substring(0, 6) + ".img";
                        addPath();
                    }
                    break;

                case "0910.img":
                    wzPath.Add("Item");
                    wzPath.Add("Special");
                    wzPath.Add("0910.img");
                    imagePath.Add(id);
                    addPath();
                    break;

                case "SetItemInfo.img":
                    wzPath.Add("Etc");
                    wzPath.Add("SetItemInfo.img");
                    imagePath.Add(id);
                    addPath();
                    break;
                default:
                    break;
            }

            return pathList;
        }

        /// <summary>
        /// 通过给定的wz名称，在advTree1中寻找第一个对应的wz_file节点。
        /// </summary>
        /// <param Name="wzName">要寻找的wz名称，不包含".wz"后缀。</param>
        /// <returns></returns>
        private Node findWzFileTreeNode(string wzName)
        {
            Wz_Type type = ParseType(wzName);
            if (type == Wz_Type.Unknown)
            {
                return null;
            }

            foreach (var wzs in this.openedWz)
            {
                foreach (var wzf in wzs.wz_files)
                {
                    if (wzf.Type == type)
                    {
                        Node node = findWzFileTreeNode(wzf);
                        if (node != null)
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        private Wz_Type ParseType(string wzName)
        {
            Wz_Type type;
            try
            {
                type = (Wz_Type)Enum.Parse(typeof(Wz_Type), wzName, true);
            }
            catch
            {
                type = Wz_Type.Unknown;
            }

            return type;
        }

        private Node findWzFileTreeNode(Wz_File wzFile)
        {
            foreach (Node baseNode in advTree1.Nodes)
            {
                Wz_File wz_f = baseNode.AsWzNode()?.Value as Wz_File;
                if (wz_f != null)
                {
                    if (wz_f == wzFile)
                    {
                        return baseNode;
                    }
                    else if (wz_f.Type == Wz_Type.Base)
                    {
                        foreach (Node wzNode in baseNode.Nodes)
                        {
                            if ((wz_f = wzNode.AsWzNode()?.Value as Wz_File) != null && wz_f == wzFile)
                            {
                                return wzNode;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Node findChildTreeNode(Node parent, string[] path)
        {
            if (parent == null || path == null)
                return null;
            for (int i = 0; i < path.Length; i++)
            {
                bool find = false;
                foreach (Node subNode in parent.Nodes)
                {
                    if (subNode.Text == path[i])
                    {
                        parent = subNode;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    return null;
                }
            }
            return parent;
        }
        #endregion

        #region contextMenuStrip1
        private void tsmi1Sort_Click(object sender, EventArgs e)
        {
            if (openedWz.Count > 0)
            {
                var sw = Stopwatch.StartNew();
                advTree1.BeginUpdate();
                try
                {
                    advTree1.ClearAndDisposeAllNodes();
                    foreach (Wz_Structure wz in openedWz)
                    {
                        sortWzNode(wz.WzNode);
                        Node node = createNode(wz.WzNode);
                        node.Expand();
                        advTree1.Nodes.Add(node);
                    }
                }
                finally
                {
                    advTree1.EndUpdate();
                    sw.Stop();
                }
                GC.Collect();
                labelItemStatus.Text = $"整理完毕: 用时 {sw.ElapsedMilliseconds}ms";
            }
            else
            {
                labelItemStatus.Text = "整理失败: 打开的wz文件不存在";
            }
        }

        private void tsmi1Export_Click(object sender, EventArgs e)
        {
            Wz_Image img = advTree1.SelectedNode?.AsWzNode()?.GetValue<Wz_Image>();
            if (img == null)
            {
                MessageBoxEx.Show("请选择导出的img。");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".img";
            dlg.FileName = img.Name;
            dlg.Filter = "IMG (*.img)|*.img";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write);
                    Stream fsWz = img.WzFile.FileStream;
                    fsWz.Seek(img.Offset, SeekOrigin.Begin);
                    byte[] buffer = new byte[2048];
                    int count, size = img.Size;
                    while (size > 0 &&
                        (count = fsWz.Read(buffer, 0, Math.Min(size, buffer.Length))) > 0)
                    {
                        fs.Write(buffer, 0, count);
                        size -= count;
                    }
                    labelItemStatus.Text = "导出完毕: " + img.Name;
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show(ex.ToString(), "错误");
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
        }

        private void tsmi1DumpAsXml_Click(object sender, EventArgs e)
        {
            Wz_Image img = advTree1.SelectedNode?.AsWzNode()?.GetValue<Wz_Image>();
            if (img == null)
            {
                MessageBoxEx.Show("请选择导出为XML的img。");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML (*.xml)|*.xml";
            dlg.FileName = img.Node.FullPathToFile.Replace('\\', '.') + ".xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write);
                    var xsetting = new XmlWriterSettings()
                    {
                        CloseOutput = false,
                        Indent = true,
                        Encoding = Encoding.UTF8,
                        CheckCharacters = true,
                        NewLineChars = Environment.NewLine,
                        NewLineOnAttributes = false,
                    };
                    var writer = XmlWriter.Create(fs, xsetting);
                    writer.WriteStartDocument(true);
                    img.Node.DumpAsXml(writer);
                    writer.WriteEndDocument();
                    writer.Close();

                    labelItemStatus.Text = "导出为XML完毕: " + img.Name;
                }
                catch (Exception ex)
                {
                    MessageBoxEx.Show(ex.ToString(), "错误");
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
        }
        #endregion

        #region Tools菜单事件和方法
        private void buttonItemSearchWz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxItemSearchWz.Text))
                return;
            if (comboBoxItem1.SelectedIndex == -1)
            {
                comboBoxItem1.SelectedIndex = 0;
            }

            switch (comboBoxItem1.SelectedIndex)
            {
                case 0:
                    searchAdvTree(advTree1, 0, textBoxItemSearchWz.Text, checkBoxItemExact1.Checked, checkBoxItemRegex1.Checked);
                    break;
                case 1:
                    searchAdvTree(advTree2, 0, textBoxItemSearchWz.Text, checkBoxItemExact1.Checked, checkBoxItemRegex1.Checked);
                    break;
                case 2:
                    searchAdvTree(advTree3, 1, textBoxItemSearchWz.Text, checkBoxItemExact1.Checked, checkBoxItemRegex1.Checked);
                    break;
            }
        }

        private void searchAdvTree(AdvTree advTree, int cellIndex, string searchText, bool exact, bool regex)
        {
            if (string.IsNullOrEmpty(searchText))
                return;

            try
            {
                Node searchNode = searchAdvTree(advTree, cellIndex, searchText, exact, regex, true);
                advTree.SelectedNode = searchNode;
                if (searchNode == null)
                    MessageBoxEx.Show("搜索结果不存在。", "错误");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(this, ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Node searchAdvTree(AdvTree advTree, int cellIndex, string searchText, bool exact, bool isRegex, bool ignoreCase)
        {
            if (advTree.Nodes.Count == 0)
                return null;

            if (isRegex)
            {
                Regex r = new Regex(searchText, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                foreach (var node in findNextNode(advTree))
                {
                    if (node != null && node.Cells.Count > cellIndex && r.IsMatch(node.Cells[cellIndex].Text))
                    {
                        return node;
                    }
                }
            }
            else
            {
                string[] pattern = searchText.Split('\\');
                foreach (var node in findNextNode(advTree))
                {
                    if (checkSearchNodeText(node, cellIndex, pattern, exact, ignoreCase))
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        private IEnumerable<Node> findNextNode(AdvTree advTree)
        {
            var node = advTree.SelectedNode;
            if (node == null)
            {
                node = advTree.Nodes[0];
                yield return node;
            }

            var levelStack = new Stack<int>();
            int index = node.Index + 1;

            while (true)
            {
                if (node.Nodes.Count > 0)
                {
                    levelStack.Push(index);
                    index = 0;
                    yield return node = node.Nodes[index++];
                    continue;
                }

                NodeCollection owner;

                while (index >= (owner = (node.Parent?.Nodes ?? advTree.Nodes)).Count)
                {
                    node = node.Parent;
                    if (node == null)
                    {
                        yield break;
                    }
                    if (levelStack.Count > 0)
                    {
                        index = levelStack.Pop();
                    }
                    else
                    {
                        index = node.Index + 1;
                    }
                }

                yield return node = owner[index++];
            }
        }

        private bool checkSearchNodeText(Node node, int cellIndex, string[] searchTextArray, bool exact, bool ignoreCase)
        {
            if (node == null || searchTextArray == null || searchTextArray.Length == 0)
                return false;
            for (int i = searchTextArray.Length - 1; i >= 0; i--)
            {
                if (node == null || node.Cells.Count <= cellIndex)
                    return false;
                if (exact)
                {
                    if (string.Compare(node.Cells[cellIndex].Text, searchTextArray[i], ignoreCase) != 0)
                        return false;
                }
                else
                {
                    if (ignoreCase ? node.Cells[cellIndex].Text.IndexOf(searchTextArray[i], StringComparison.CurrentCultureIgnoreCase) < 0 :
                        !node.Cells[cellIndex].Text.Contains(searchTextArray[i]))
                        return false;
                }

                node = node.Parent;
            }
            return true;
        }

        private void textBoxItemSearchWz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonItemSearchWz_Click(buttonItemSearchWz, EventArgs.Empty);
            }
        }

        private void buttonItemSearchString_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxItemSearchString.Text))
                return;
            QueryPerformance.Start();
            if (!this.stringLinker.HasValues)
            {
                if (!this.stringLinker.Load(findStringWz(), findItemWz(), findEtcWz()))
                {
                    MessageBoxEx.Show("请优先打开Base.wz。", "错误");
                    return;
                }
                QueryPerformance.End();
                double ms = (Math.Round(QueryPerformance.GetLastInterval(), 4) * 1000);
                labelItemStatus.Text = "StringLinker初始化完毕: 用时 " + ms + "ms";
            }
            if (comboBoxItem2.SelectedIndex < 0)
                comboBoxItem2.SelectedIndex = 0;

            List<Dictionary<int, StringResult>> dicts = new List<Dictionary<int, StringResult>>();
            switch (comboBoxItem2.SelectedIndex)
            {
                case 0:
                    dicts.Add(stringLinker.StringEqp);
                    dicts.Add(stringLinker.StringItem);
                    dicts.Add(stringLinker.StringMap);
                    dicts.Add(stringLinker.StringMob);
                    dicts.Add(stringLinker.StringNpc);
                    dicts.Add(stringLinker.StringSkill);
                    dicts.Add(stringLinker.StringSetItem);
                    break;
                case 1:
                    dicts.Add(stringLinker.StringEqp);
                    break;
                case 2:
                    dicts.Add(stringLinker.StringItem);
                    break;
                case 3:
                    dicts.Add(stringLinker.StringMap);
                    break;
                case 4:
                    dicts.Add(stringLinker.StringMob);
                    break;
                case 5:
                    dicts.Add(stringLinker.StringNpc);
                    break;
                case 6:
                    dicts.Add(stringLinker.StringSkill);
                    break;
                case 7:
                    dicts.Add(stringLinker.StringSetItem);
                    break;
            }

            listViewExString.BeginUpdate();
            try
            {
                listViewExString.Items.Clear();
                IEnumerable<KeyValuePair<int, StringResult>> results = searchStringLinker(dicts, textBoxItemSearchString.Text, checkBoxItemExact2.Checked, checkBoxItemRegex2.Checked);
                foreach (KeyValuePair<int, StringResult> kv in results)
                {
                    string[] item = new string[] { kv.Key.ToString(), kv.Value.Name, kv.Value.Desc, kv.Value.FullPath };
                    listViewExString.Items.Add(new ListViewItem(item));
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                listViewExString.EndUpdate();
            }
        }

        private Wz_File findStringWz()
        {
            foreach (Wz_Structure wz in openedWz)
            {
                foreach (Wz_File file in wz.wz_files)
                {
                    if (file.Type == Wz_Type.String && file.Node.Nodes.Count > 0)
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        private Wz_File findItemWz()
        {
            foreach (Wz_Structure wz in openedWz)
            {
                foreach (Wz_File file in wz.wz_files)
                {
                    if (file.Type == Wz_Type.Item && file.Node.Nodes.Count > 0)
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        private Wz_File findEtcWz()
        {
            foreach (Wz_Structure wz in openedWz)
            {
                foreach (Wz_File file in wz.wz_files)
                {
                    if (file.Type == Wz_Type.Etc && file.Node.Nodes.Count > 0)
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        private IEnumerable<KeyValuePair<int, StringResult>> searchStringLinker(IEnumerable<Dictionary<int, StringResult>> dicts, string key, bool exact, bool isRegex)
        {
            string[] match = key.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Regex re = null;
            if (isRegex)
            {
                re = new Regex(key, RegexOptions.IgnoreCase);
            }

            foreach (Dictionary<int, StringResult> dict in dicts)
            {
                foreach (KeyValuePair<int, StringResult> kv in dict)
                {
                    if (exact)
                    {
                        if (kv.Key.ToString() == key || kv.Value.Name == key)
                            yield return kv;
                    }
                    else if (isRegex)
                    {
                        if (re.IsMatch(kv.Key.ToString()) || (!string.IsNullOrEmpty(kv.Value.Name) && re.IsMatch(kv.Value.Name)))
                        {
                            yield return kv;
                        }
                    }
                    else
                    {
                        string id = kv.Key.ToString();
                        bool r = true;
                        foreach (string str in match)
                        {
                            if (!(id.Contains(str) || (!string.IsNullOrEmpty(kv.Value.Name) && kv.Value.Name.Contains(str))))
                            {
                                r = false;
                                break;
                            }
                        }
                        if (r)
                        {
                            yield return kv;
                        }
                    }
                }
            }
        }

        private void textBoxItemSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonItemSearchString_Click(buttonItemSearchString, EventArgs.Empty);
            }
        }

        private void buttonItemSelectStringWz_Click(object sender, EventArgs e)
        {
            Wz_File stringWzFile = advTree1.SelectedNode?.AsWzNode()?.FindNodeByPath("String").GetNodeWzFile();
            Wz_File itemWzFile = advTree1.SelectedNode?.AsWzNode()?.FindNodeByPath("Item").GetNodeWzFile();
            Wz_File etcWzFile = advTree1.SelectedNode?.AsWzNode()?.FindNodeByPath("Etc").GetNodeWzFile();
            if (stringWzFile == null || itemWzFile == null || etcWzFile == null)
            {
                MessageBoxEx.Show("请选择Base.wz。", "错误");
                return;
            }
            QueryPerformance.Start();
            bool r = stringLinker.Load(stringWzFile, itemWzFile, etcWzFile);
            QueryPerformance.End();
            if (r)
            {
                double ms = (Math.Round(QueryPerformance.GetLastInterval(), 4) * 1000);
                labelItemStatus.Text = "StringLinker初始化完毕: 用时 " + ms + "ms";
            }
            else
            {
                MessageBoxEx.Show("StringLinker初始化失败。", "错误");
            }
        }

        private void buttonItemClearStringWz_Click(object sender, EventArgs e)
        {
            stringLinker.Clear();
            labelItemStatus.Text = "StringLinker整理完毕";
        }

        private void buttonItemPatcher_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is FrmPatcher && !form.IsDisposed)
                {
                    form.Show();
                    form.BringToFront();
                    return;
                }
            }
            FrmPatcher patcher = new FrmPatcher();
            var config = WcR2Config.Default;
            var defaultEnc = config?.WzEncoding?.Value ?? 0;
            if (defaultEnc != 0)
            {
                patcher.PatcherNoticeEncoding = Encoding.GetEncoding(defaultEnc);
            }
            patcher.Owner = this;
            patcher.Show();
        }

        private void buttonInstallGame_Click(object sender, EventArgs e)
        {
            #if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "ngm://launch/ -mode:install -game:'16785939@bb01'",
            });
            #else
            Process.Start("ngm://launch/ -mode:install -game:'16785939@bb01'");
            #endif
        }

        private void buttonGameStart_Click(object sender, EventArgs e)
        {
            #if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "ngm://launch/ -mode:launch -game:'16785939@bb01'",
            });
            #else
            Process.Start("ngm://launch/ -mode:launch -game:'16785939@bb01'");
            #endif
        }
        #endregion

        #region soundPlayer相关事件
        private void preLoadSound(Wz_Sound sound, string soundName)
        {
            byte[] data = sound.ExtractSound();
            if (data == null || data.Length <= 0)
            {
                return;
            }
            soundPlayer.PreLoad(data);
            labelItemSoundTitle.Text = soundName;

            switch (sound.SoundType)
            {
                case Wz_SoundType.Mp3: soundName += ".mp3"; break;
                case Wz_SoundType.Pcm: soundName += ".wav"; break;
            }
            soundPlayer.PlayingSoundName = soundName;
            labelItemSoundTitle.Tooltip = soundName;
        }

        private void sliderItemSoundTime_ValueChanged(object sender, EventArgs e)
        {
            if (!timerChangeValue)
                soundPlayer.SoundPosition = sliderItemSoundTime.Value;
        }

        private void sliderItemSoundVol_ValueChanged(object sender, EventArgs e)
        {
            soundPlayer.Volume = sliderItemSoundVol.Value;
        }

        private void buttonItemLoadSound_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                List<string> supportExt = new List<string>();
                supportExt.Add("音频文件 (*.mp3;*.ogg;*.wav)|*.mp3;*.ogg;*.wav");
                foreach (string ext in this.soundPlayer.GetPluginSupportedExt())
                {
                    supportExt.Add(ext);
                }
                supportExt.Add("全部文件 (*.*)|*.*");

                dlg.Title = "打开音频文件";
                dlg.Filter = string.Join("|", supportExt.ToArray());
                dlg.Multiselect = false;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    loadCostumSoundFile(dlg.FileName);
                }
            }
        }

        private void buttonItemSoundPlay_Click(object sender, EventArgs e)
        {
            if (soundPlayer.State == PlayState.Playing)
            {
                soundPlayer.Pause();
                buttonItemSoundPlay.Image = WzComparerR2.Properties.Resources.Play;
                //buttonItemSoundPlay.Text = " Play";
            }
            else if (soundPlayer.State == PlayState.Paused)
            {
                soundPlayer.Resume();
                //buttonItemSoundPlay.Text = "Pause";
                buttonItemSoundPlay.Image = WzComparerR2.Properties.Resources.Pause;
            }
            else
            {
                soundPlayer.Play();
                //buttonItemSoundPlay.Text = "Pause";
                buttonItemSoundPlay.Image = WzComparerR2.Properties.Resources.Pause;
            }
        }

        private void buttonItemSoundStop_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
            //buttonItemSoundPlay.Text = " Play";
            buttonItemSoundPlay.Image = WzComparerR2.Properties.Resources.Play;
        }

        private void buttonItemSoundSave_Click(object sender, EventArgs e)
        {
            byte[] data = soundPlayer.Data;
            if (data == null)
                return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.AddExtension = true;
                dlg.Title = "选择导出的文件夹";
                dlg.Filter = "全部文件 (*.*)|*.*";
                dlg.AddExtension = false;
                dlg.FileName = soundPlayer.PlayingSoundName;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(dlg.FileName, FileMode.Create);
                        fs.Write(data, 0, data.Length);

                        MessageBoxEx.Show("导出成功。");
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show("导出失败\r\n\r\n" + ex.ToString(), "错误");
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
            }
        }

        private void checkBoxItemSoundLoop_CheckedChanged(object sender, CheckBoxChangeEventArgs e)
        {
            soundPlayer.Loop = checkBoxItemSoundLoop.Checked;
        }

        private void soundTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan currentTime = TimeSpan.FromSeconds(soundPlayer.SoundPosition);
            TimeSpan totalTime = TimeSpan.FromSeconds(soundPlayer.SoundLength);
            labelItemSoundTime.Text = string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d3} / {4:d2}:{5:d2}:{6:d2}.{7:d3}",
                currentTime.Hours, currentTime.Minutes, currentTime.Seconds, currentTime.Milliseconds,
                totalTime.Hours, totalTime.Minutes, totalTime.Seconds, totalTime.Milliseconds);
            timerChangeValue = true;
            sliderItemSoundTime.Maximum = (int)totalTime.TotalSeconds;
            sliderItemSoundTime.Value = (int)currentTime.TotalSeconds;
            timerChangeValue = false;
        }

        private void ribbonBar3_DragEnter(object sender, DragEventArgs e)
        {
            string[] types = e.Data.GetFormats();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ribbonBar3_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                loadCostumSoundFile(files[0]);
            }
        }

        private void loadCostumSoundFile(string fileName)
        {
            CustomSoundFile soundFile = new CustomSoundFile(fileName, 0, (int)(new FileInfo(fileName).Length));
            soundPlayer.PreLoad(soundFile);
            soundPlayer.PlayingSoundName = Path.GetFileName(fileName);
            labelItemSoundTitle.Text = "(外部文件) " + soundPlayer.PlayingSoundName;
            labelItemSoundTitle.Tooltip = fileName;
        }
        #endregion

        #region contextMenuStrip2
        private void tsmi2SaveAs_Click(object sender, EventArgs e)
        {
            object item = advTree3.SelectedNode?.AsWzNode()?.Value;

            if (item == null)
                return;

            if (item is string str)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = advTree3.SelectedNode.Text;
                if (!dlg.FileName.Contains("."))
                {
                    dlg.FileName += ".txt";
                }
                dlg.Filter = "全部文件 (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dlg.FileName, str);
                        this.labelItemStatus.Text = "导出文档完毕";
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show("导出文档失败\r\n" + ex.ToString(), "错误");
                    }
                }
            }
            else if (item is Wz_Sound wzSound)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = advTree3.SelectedNode.Text;
                if (!dlg.FileName.Contains("."))
                {
                    switch (wzSound.SoundType)
                    {
                        case Wz_SoundType.Mp3: dlg.FileName += ".mp3"; break;
                        case Wz_SoundType.Pcm: dlg.FileName += ".pcm"; break;
                    }
                }
                dlg.Filter = "全部文件 (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var f = File.Create(dlg.FileName))
                        {
                            wzSound.WzFile.FileStream.Seek(wzSound.Offset, SeekOrigin.Begin);
                            byte[] buffer = new byte[4096];
                            int bytes = wzSound.DataLength;
                            while (bytes > 0)
                            {
                                int count = wzSound.WzFile.FileStream.Read(buffer, 0, Math.Min(buffer.Length, bytes));
                                if (count > 0)
                                {
                                    f.Write(buffer, 0, count);
                                    bytes -= count;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        this.labelItemStatus.Text = "导出音频完毕";
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show("导出音频失败\r\n" + ex.ToString(), "错误");
                    }
                }
            }
            else if (item is Wz_RawData rawData)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = advTree3.SelectedNode.Text;
                dlg.Filter = "全部文件 (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var f = File.Create(dlg.FileName))
                        {
                            rawData.WzFile.FileStream.Seek(rawData.Offset, SeekOrigin.Begin);
                            byte[] buffer = new byte[4096];
                            int bytes = rawData.Length;
                            while (bytes > 0)
                            {
                                int count = rawData.WzFile.FileStream.Read(buffer, 0, Math.Min(buffer.Length, bytes));
                                if (count > 0)
                                {
                                    f.Write(buffer, 0, count);
                                    bytes -= count;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        this.labelItemStatus.Text = "文件导出完毕";
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show("文件导出失败\r\n" + ex.ToString(), "错误");
                    }
                }
            }
        }

        private void tsmi2HandleUol_Click(object sender, EventArgs e)
        {
            Wz_Uol uol = advTree3.SelectedNode?.AsWzNode()?.Value as Wz_Uol;
            if (uol == null)
            {
                labelItemStatus.Text = "未选择Uol节点。";
                return;
            }

            Node uolNode = handleUol(advTree3.SelectedNode, uol.Uol);
            if (uolNode == null)
            {
                labelItemStatus.Text = "无法查找Uol对象节点。";
                return;
            }
            else
            {
                advTree3.SelectedNode = uolNode;
            }
        }

        private void tsmi2ExpandAll_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;
            advTree3.BeginUpdate();
            advTree3.SelectedNode.ExpandAll();
            advTree3.SelectedNode.Expand();
            advTree3.EndUpdate();
        }

        private void tsmi2CollapseAll_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;
            advTree3.BeginUpdate();
            advTree3.SelectedNode.Collapse();
            advTree3.SelectedNode.CollapseAll();
            advTree3.EndUpdate();
        }

        private void tsmi2ExpandLevel_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;

            advTree3.BeginUpdate();
            foreach (Node node in getEqualLevelNode(advTree3.SelectedNode))
            {
                node.Expand();
            }
            advTree3.EndUpdate();
        }

        private void tsmi2CollapseLevel_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;

            advTree3.BeginUpdate();
            foreach (Node node in getEqualLevelNode(advTree3.SelectedNode))
            {
                node.Collapse();
            }
            advTree3.EndUpdate();
        }

        private IEnumerable<Node> getEqualLevelNode(Node currentNode)
        {
            if (currentNode == null)
                yield break;
            int level = currentNode.Level;
            Node parent = currentNode;
            while (parent != null && parent.Parent != null)
            {
                parent = parent.Parent;
            }
            Queue<Node> nodeList = new Queue<Node>();
            nodeList.Enqueue(parent);
            for (int i = 0; i < level; i++)
            {
                int count = nodeList.Count;
                for (int j = 0; j < count; j++)
                {
                    Node node = nodeList.Dequeue();
                    foreach (Node child in node.Nodes)
                        nodeList.Enqueue(child);
                }
            }

            while (nodeList.Count > 0)
            {
                yield return nodeList.Dequeue();
            }
        }

        private void tsmi2ExpandType_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;

            advTree3.BeginUpdate();
            foreach (Node node in getEqualTypeNode(advTree3.SelectedNode))
            {
                node.Expand();
            }
            advTree3.EndUpdate();
        }

        private void tsmi2CollapseType_Click(object sender, EventArgs e)
        {
            if (advTree3.SelectedNode == null)
                return;

            advTree3.BeginUpdate();
            foreach (Node node in getEqualTypeNode(advTree3.SelectedNode))
            {
                node.Collapse();
            }
            advTree3.EndUpdate();
        }

        private IEnumerable<Node> getEqualTypeNode(Node currentNode)
        {
            if (currentNode == null)
                yield break;
            Type type = currentNode.AsWzNode()?.Value?.GetType();
            Node parent = currentNode;
            while (parent != null && parent.Parent != null)
            {
                parent = parent.Parent;
            }
            Queue<Node> nodeList = new Queue<Node>();
            nodeList.Enqueue(parent);
            while (nodeList.Count > 0)
            {
                int count = nodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    Node node = nodeList.Dequeue();
                    if (node.AsWzNode()?.Value?.GetType() == type)
                    {
                        yield return node;
                    }
                    foreach (Node child in node.Nodes)
                        nodeList.Enqueue(child);
                }
            }
        }

        private void tsmi2Prev_Click(object sender, EventArgs e)
        {
            if (historyNodeList.PrevCount > 0)
            {
                historySelecting = true;
                advTree3.SelectedNode = historyNodeList.MovePrev();
            }
        }

        private void tsmi2Next_Click(object sender, EventArgs e)
        {
            if (historyNodeList.NextCount > 0)
            {
                historySelecting = true;
                advTree3.SelectedNode = historyNodeList.MoveNext();
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            var node = advTree3.SelectedNode.AsWzNode();
            tsmi2SaveAs.Visible = false;
            tsmi2HandleUol.Visible = false;
            if (node != null)
            {
                if (node.Value is Wz_Sound || node.Value is Wz_Png || node.Value is string || node.Value is Wz_RawData || node.Value is Wz_Video)
                {
                    tsmi2SaveAs.Visible = true;
                    tsmi2SaveAs.Enabled = true;
                }
                else if (node.Value is Wz_Uol)
                {
                    tsmi2HandleUol.Visible = true;
                }
                else
                {
                    tsmi2SaveAs.Visible = true;
                    tsmi2SaveAs.Enabled = false;
                }
            }
        }
        #endregion

        #region charaSim相关
        private void buttonItemQuickView_Click(object sender, EventArgs e)
        {
            quickView();
        }

        private void advTree1_AfterNodeSelect_2(object sender, AdvTreeNodeEventArgs e)
        {
            lastSelectedTree = advTree1;
            if (buttonItemAutoQuickView.Checked)
            {
                quickView(advTree1.SelectedNode);
            }
        }

        private void advTree2_AfterNodeSelect_2(object sender, AdvTreeNodeEventArgs e)
        {
            lastSelectedTree = advTree2;
            if (buttonItemAutoQuickView.Checked)
            {
                quickView(advTree2.SelectedNode);
            }
        }

        private void quickView()
        {
            if (lastSelectedTree != null)
            {
                quickView(lastSelectedTree.SelectedNode);
            }
        }

        private void quickView(Node node)
        {
            Wz_Node selectedNode = node.AsWzNode();
            if (selectedNode == null)
            {
                return;
            }

            Wz_Image image;

            Wz_File wzf = selectedNode.GetNodeWzFile();
            if (wzf == null)
            {
                labelItemStatus.Text = "Wz文件的节点不可加载。";
                return;
            }

            if (!this.stringLinker.HasValues)
            {
                this.stringLinker.Load(findStringWz(), findItemWz(), findEtcWz());
            }

            object obj = null;
            string fileName = null;
            switch (wzf.Type)
            {
                case Wz_Type.Character:
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    CharaSimLoader.LoadSetItemsIfEmpty();
                    CharaSimLoader.LoadExclusiveEquipsIfEmpty();
                    CharaSimLoader.LoadCommoditiesIfEmpty();
                    var gear = Gear.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = gear;
                    if (gear != null)
                    {
                        fileName = gear.ItemID + ".png";
                    }
                    break;
                case Wz_Type.Item:
                    CharaSimLoader.LoadCommoditiesIfEmpty();
                    Wz_Node itemNode = selectedNode;
                    if (Regex.IsMatch(itemNode.FullPathToFile, @"^Item\\(Cash|Consume|Etc|Install|Cash)\\\d{4,6}.img\\\d+$") || Regex.IsMatch(itemNode.FullPathToFile, @"^Item\\Special\\0910.img\\\d+$"))
                    {
                        var item = Item.CreateFromNode(itemNode, PluginManager.FindWz);
                        obj = item;
                        if (item != null)
                        {
                            fileName = item.ItemID + ".png";
                        }
                    }
                    else if (Regex.IsMatch(itemNode.FullPathToFile, @"^Item\\Pet\\\d{7}.img"))
                    {
                        if (CharaSimLoader.LoadedSetItems.Count == 0) //宠物 预读套装
                        {
                            CharaSimLoader.LoadSetItemsIfEmpty();
                        }
                        if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                            return;
                        var item = Item.CreateFromNode(image.Node, PluginManager.FindWz);
                        obj = item;
                        if (item != null)
                        {
                            fileName = item.ItemID + ".png";
                        }
                    }

                    break;
                case Wz_Type.Skill:
                    Wz_Node skillNode = selectedNode;
                    //模式路径分析
                    if (Regex.IsMatch(skillNode.FullPathToFile, @"^Skill\d*\\Recipe_\d+.img\\\d+$"))
                    {
                        Recipe recipe = Recipe.CreateFromNode(skillNode);
                        obj = recipe;
                        if (recipe != null)
                        {
                            fileName = "recipe_" + recipe.RecipeID + ".png";
                        }
                    }
                    else if (Regex.IsMatch(skillNode.FullPathToFile, @"^Skill\d*\\\d+.img\\skill\\\d+$"))
                    {
                        Skill skill = Skill.CreateFromNode(skillNode, PluginManager.FindWz);
                        if (skill != null)
                        {
                            switch (this.skillDefaultLevel)
                            {
                                case DefaultLevel.Level0: skill.Level = 0; break;
                                case DefaultLevel.Level1: skill.Level = 1; break;
                                case DefaultLevel.LevelMax: skill.Level = skill.MaxLevel; break;
                                case DefaultLevel.LevelMaxWithCO: skill.Level = skill.MaxLevel + 2; break;
                            }
                            obj = skill;
                            fileName = "skill_" + skill.SkillID + ".png";
                        }
                    }
                    break;

                case Wz_Type.Mob:
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    var mob = Mob.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = mob;
                    if (mob != null)
                    {
                        fileName = mob.ID + ".png";
                    }
                    break;

                case Wz_Type.Npc:
                    if ((image = selectedNode.GetValue<Wz_Image>()) == null || !image.TryExtract())
                        return;
                    var npc = Npc.CreateFromNode(image.Node, PluginManager.FindWz);
                    obj = npc;
                    if (npc != null)
                    {
                        fileName = npc.ID + ".png";
                    }
                    break;

                case Wz_Type.Etc:
                    CharaSimLoader.LoadSetItemsIfEmpty();
                    Wz_Node setItemNode = selectedNode;
                    if (Regex.IsMatch(setItemNode.FullPathToFile, @"^Etc\\SetItemInfo.img\\-?\d+$"))
                    {
                        SetItem setItem;
                        if (!CharaSimLoader.LoadedSetItems.TryGetValue(Convert.ToInt32(selectedNode.Text), out setItem))
                            return;
                        obj = setItem;
                        if (setItem != null)
                        {
                            fileName = setItem.SetItemID + ".png";
                        }
                    }
                    break;
            }
            if (obj != null)
            {
                tooltipQuickView.TargetItem = obj;
                tooltipQuickView.ImageFileName = fileName;
                tooltipQuickView.Refresh();
                tooltipQuickView.HideOnHover = false;
                tooltipQuickView.Show();
            }
        }

        private void comboBoxItemLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevComponents.Editors.ComboItem item = comboBoxItemLanguage.SelectedItem as DevComponents.Editors.ComboItem;

            if (item != null)
            {
                GearGraphics.SetFontFamily("宋体");
                ConfigManager.Reload();
                CharaSimConfig.Default.SelectedFontIndex = comboBoxItemLanguage.SelectedIndex;
                ConfigManager.Save();
            }
        }

        private void buttonItemClearSetItems_Click(object sender, EventArgs e)
        {
            int count = CharaSimLoader.LoadedSetItems.Count;
            CharaSimLoader.LoadedSetItems.Clear();
            labelItemStatus.Text = "套装道具" + count + "个整理完毕";
        }

        private void buttonItemClearExclusiveEquips_Click(object sender, EventArgs e)
        {
            int count = CharaSimLoader.LoadedExclusiveEquips.Count;
            CharaSimLoader.LoadedExclusiveEquips.Clear();
            labelItemStatus.Text = "不可重复佩戴道具" + count + "个整理完毕";
        }

        private void buttonItemClearCommodities_Click(object sender, EventArgs e)
        {
            int count = CharaSimLoader.LoadedCommoditiesBySN.Count;
            CharaSimLoader.LoadedCommoditiesBySN.Clear();
            CharaSimLoader.LoadedCommoditiesByItemId.Clear();
            labelItemStatus.Text = "现金道具" + count + "个整理完毕";
        }

        private void buttonItemCharItem_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonItemCharItem.Checked)
                this.charaSimCtrl.UIItem.Refresh();
            this.charaSimCtrl.UIItem.Visible = buttonItemCharItem.Checked;
        }

        private void buttonItemAddItem_Click(object sender, EventArgs e)
        {
            bool success;

            success = this.charaSimCtrl.UIItem.AddItem(this.tooltipQuickView.TargetItem as ItemBase);
            if (!success)
            {
                labelItemStatus.Text = "选择的道具不存在或再也不能放入。";
            }
        }

        private void afrm_KeyDown(object sender, KeyEventArgs e)
        {
            AfrmTooltip frm = sender as AfrmTooltip;
            if (frm == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    frm.Hide();
                    break;
                case Keys.Up:
                    frm.Top -= 1;
                    break;
                case Keys.Down:
                    frm.Top += 1;
                    break;
                case Keys.Left:
                    frm.Left -= 1;
                    break;
                case Keys.Right:
                    frm.Left += 1;
                    break;
            }

            Skill skill = frm.TargetItem as Skill;
            if (skill != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Oemplus:
                    case Keys.Add:
                        skill.Level += 1;
                        break;

                    case Keys.OemMinus:
                    case Keys.Subtract:
                        skill.Level -= 1;
                        break;

                    case Keys.OemOpenBrackets:
                        skill.Level -= this.skillInterval;
                        break;
                    case Keys.OemCloseBrackets:
                        skill.Level += this.skillInterval;
                        break;
                }
                frm.Refresh();
            }
        }

        private void afrm_VisibleChanged(object sender, EventArgs e)
        {
            if (sender is AfrmItem)
            {
                buttonItemCharItem.Checked = ((AfrmItem)sender).Visible;
            }
            else if (sender is AfrmStat)
            {
                buttonItemCharaStat.Checked = ((AfrmStat)sender).Visible;
            }
            else if (sender is AfrmUnion)
            {
                btnMapleUnion.Checked = ((AfrmUnion)sender).Visible;
            }
        }

        private void btnPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (btnPreview.Checked)
            {
                this.charaSimCtrl.UIStat.Refresh();
            }
            this.charaSimCtrl.UIStat.Visible = btnPreview.Checked;
        }

        private void buttonItemCharaStat_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonItemCharaStat.Checked)
            {
                this.charaSimCtrl.UIStat.Refresh();
            }
            this.charaSimCtrl.UIStat.Visible = buttonItemCharaStat.Checked;
        }


        private void buttonItemQuickViewSetting_Click(object sender, EventArgs e)
        {
            using (FrmQuickViewSetting frm = new FrmQuickViewSetting())
            {
                frm.Load(CharaSimConfig.Default);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ConfigManager.Reload();
                    frm.Save(CharaSimConfig.Default);
                    ConfigManager.Save();
                    UpdateCharaSimSettings();
                }
            }
        }

        private void btnMapleUnion_CheckedChanged(object sender, EventArgs e)
        {
            if (btnMapleUnion.Checked)
            {
                this.charaSimCtrl.UIUnion.Refresh();
            }
            this.charaSimCtrl.UIUnion.Visible = btnMapleUnion.Checked;
        }
        #endregion

        #region 实现插件接口
        Office2007RibbonForm PluginContextProvider.MainForm
        {
            get { return this; }
        }

        DotNetBarManager PluginContextProvider.DotNetBarManager
        {
            get { return this.dotNetBarManager1; }
        }

        IList<Wz_Structure> PluginContextProvider.LoadedWz
        {
            get { return new System.Collections.ObjectModel.ReadOnlyCollection<Wz_Structure>(this.openedWz); }
        }

        Wz_Node PluginContextProvider.SelectedNode1
        {
            get { return advTree1.SelectedNode.AsWzNode(); }
        }

        Wz_Node PluginContextProvider.SelectedNode2
        {
            get { return advTree2.SelectedNode.AsWzNode(); }
        }

        Wz_Node PluginContextProvider.SelectedNode3
        {
            get { return advTree3.SelectedNode.AsWzNode(); }
        }

        private EventHandler<WzNodeEventArgs> selectedNode1Changed;
        private EventHandler<WzNodeEventArgs> selectedNode2Changed;
        private EventHandler<WzNodeEventArgs> selectedNode3Changed;
        private EventHandler<WzStructureEventArgs> wzOpened;
        private EventHandler<WzStructureEventArgs> wzClosing;

        event EventHandler<WzNodeEventArgs> PluginContextProvider.SelectedNode1Changed
        {
            add { selectedNode1Changed += value; }
            remove { selectedNode1Changed -= value; }
        }

        event EventHandler<WzNodeEventArgs> PluginContextProvider.SelectedNode2Changed
        {
            add { selectedNode2Changed += value; }
            remove { selectedNode2Changed -= value; }
        }

        event EventHandler<WzNodeEventArgs> PluginContextProvider.SelectedNode3Changed
        {
            add { selectedNode3Changed += value; }
            remove { selectedNode3Changed -= value; }
        }

        event EventHandler<WzStructureEventArgs> PluginContextProvider.WzOpened
        {
            add { wzOpened += value; }
            remove { wzOpened -= value; }
        }

        event EventHandler<WzStructureEventArgs> PluginContextProvider.WzClosing
        {
            add { wzClosing += value; }
            remove { wzClosing -= value; }
        }

        StringLinker PluginContextProvider.DefaultStringLinker
        {
            get { return this.stringLinker; }
        }

        AlphaForm PluginContextProvider.DefaultTooltipWindow
        {
            get { return this.tooltipQuickView; }
        }

        private void RegisterPluginEvents()
        {
            advTree1.AfterNodeSelect += advTree1_AfterNodeSelect_Plugin;
            advTree2.AfterNodeSelect += advTree2_AfterNodeSelect_Plugin;
            advTree3.AfterNodeSelect += advTree3_AfterNodeSelect_Plugin;
        }

        private void advTree1_AfterNodeSelect_Plugin(object sender, AdvTreeNodeEventArgs e)
        {
            if (selectedNode1Changed != null)
            {
                var wzNode = ((PluginContextProvider)(this)).SelectedNode1;
                var args = new WzNodeEventArgs(wzNode);
                selectedNode1Changed(this, args);
            }
        }

        private void advTree2_AfterNodeSelect_Plugin(object sender, AdvTreeNodeEventArgs e)
        {
            if (selectedNode2Changed != null)
            {
                var wzNode = ((PluginContextProvider)(this)).SelectedNode2;
                var args = new WzNodeEventArgs(wzNode);
                selectedNode2Changed(this, args);
            }
        }

        private void advTree3_AfterNodeSelect_Plugin(object sender, AdvTreeNodeEventArgs e)
        {
            if (selectedNode3Changed != null)
            {
                var wzNode = ((PluginContextProvider)(this)).SelectedNode3;
                var args = new WzNodeEventArgs(wzNode);
                selectedNode3Changed(this, args);
            }
        }

        protected virtual void OnWzOpened(WzStructureEventArgs e)
        {
            if (wzOpened != null)
            {
                wzOpened(this, e);
            }
        }

        protected virtual void OnWzClosing(WzStructureEventArgs e)
        {
            if (wzClosing != null)
            {
                wzClosing(this, e);
            }
        }
        #endregion

        private void btnEasyCompare_Click(object sender, EventArgs e)
        {
            if (compareThread != null)
            {
                compareThread.Suspend();
                if (DialogResult.Yes == MessageBoxEx.Show("正在进行对比。是否进行中断？", "提示", MessageBoxButtons.YesNoCancel))
                {
                    compareThread.Resume();
                    compareThread.Interrupt();
                }
                else
                {
                    compareThread.Resume();
                }
                return;
            }

            if (openedWz.Count < 2)
            {
                MessageBoxEx.Show("请选择对比的两个以上的Wz文件。", "错误");
                return;
            }

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择导出的文件夹。";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                compareThread = new Thread(() =>
                {
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    EasyComparer comparer = new EasyComparer();
                    comparer.Comparer.PngComparison = (WzPngComparison)cmbComparePng.SelectedItem;
                    comparer.Comparer.ResolvePngLink = chkResolvePngLink.Checked;
                    comparer.OutputPng = chkOutputPng.Checked;
                    comparer.OutputAddedImg = chkOutputAddedImg.Checked;
                    comparer.OutputRemovedImg = chkOutputRemovedImg.Checked;
                    comparer.EnableDarkMode = chkEnableDarkMode.Checked;
                    comparer.saveSkillTooltip = chkOutputSkillTooltip.Checked;
                    comparer.saveItemTooltip = chkSaveItemTooltip.Checked;
                    comparer.saveEqpTooltip = chkSaveEqpTooltip.Checked;
                    comparer.saveMobTooltip = chkSaveMobTooltip.Checked;
                    comparer.saveNpcTooltip = chkSaveNpcTooltip.Checked;
                    comparer.saveCashTooltip = chkSaveCashTooltip.Checked;
                    comparer.HashPngFileName = chkHashPngFileName.Checked;
                    comparer.StateInfoChanged += new EventHandler(comparer_StateInfoChanged);
                    comparer.StateDetailChanged += new EventHandler(comparer_StateDetailChanged);
                    try
                    {
                        Wz_File fileNew = openedWz[0].wz_files[0];
                        Wz_File fileOld = openedWz[1].wz_files[0];

                        while (true)
                        {
                            string txt = string.Format("Wz文件 :\r\n\r\n  新版本 : {0} (V{1})\r\n  旧版本 : {2} (V{3})\r\n\r\n按下Yes时开始对比，按下No时可对调新版本和旧版本。",
                                fileNew.Header.FileName,
                                fileNew.GetMergedVersion(),
                                fileOld.Header.FileName,
                                fileOld.GetMergedVersion()
                                );
                            switch (MessageBoxEx.Show(txt, "WZ Compare", MessageBoxButtons.YesNoCancel))
                            {
                                case DialogResult.Yes:
                                    comparer.EasyCompareWzFiles(fileNew, fileOld, dlg.SelectedPath);
                                    return;

                                case DialogResult.No:
                                    Wz_File tmp = fileNew;
                                    fileNew = fileOld;
                                    fileOld = tmp;
                                    break;

                                case DialogResult.Cancel:
                                default:
                                    return;
                            }
                        }

                    }
                    catch (ThreadAbortException)
                    {
                        MessageBoxEx.Show(this, "对比已中断。", "错误");
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show(this, "对比已中断。" + ex.ToString(), "错误");
                    }
                    finally
                    {
                        sw.Stop();
                        compareThread = null;
                        labelXComp1.Text = "Wz对比结束: 用时 " + sw.Elapsed.ToString();
                        labelXComp2.Text = "";
                    }
                });
                compareThread.Priority = ThreadPriority.Highest;
                compareThread.Start();
            }
        }

        void comparer_StateDetailChanged(object sender, EventArgs e)
        {
            EasyComparer comp = sender as EasyComparer;
            if (comp != null)
            {
                labelXComp1.Text = comp.StateInfo;
            }
        }

        void comparer_StateInfoChanged(object sender, EventArgs e)
        {
            EasyComparer comp = sender as EasyComparer;
            if (comp != null)
            {
                labelXComp2.Text = comp.StateDetail;
            }
        }

        private void buttonItemAbout_Click(object sender, EventArgs e)
        {
            new FrmAbout().ShowDialog();
        }

        private void btnExportSkill_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择导出的文件夹。";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!this.stringLinker.HasValues)
                    this.stringLinker.Load(findStringWz(), findItemWz(), findEtcWz());

                DBConnection conn = new DBConnection(this.stringLinker);
                DataSet ds = conn.GenerateSkillTable();
                foreach (DataTable dt in ds.Tables)
                {
                    FileStream fs = new FileStream(Path.Combine(dlg.SelectedPath, dt.TableName + ".csv"), FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    conn.OutputCsv(sw, dt);
                    sw.Close();
                    fs.Dispose();
                }
                MessageBoxEx.Show("导出完成。");
            }
        }

        private void btnExportSkillOption_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择导出的文件夹。";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!this.stringLinker.HasValues)
                    this.stringLinker.Load(findStringWz(), findItemWz(), findEtcWz());

                DBConnection conn = new DBConnection(this.stringLinker);
                conn.ExportSkillOption(dlg.SelectedPath);
                MessageBoxEx.Show("导出完成。");
            }
        }

        private async void btnLoadNotice_Click(object sender, EventArgs e)
        {
            string url = "https://jancy-49.github.io/Papulatus-Daily/index.html";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var html = await httpClient.GetStringAsync(url);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var newsDivs = doc.DocumentNode.SelectNodes("//div[@class='news']");
                    if (newsDivs != null)
                    {
                        listViewExNotice.Items.Clear();
                        foreach (var newsDiv in newsDivs)
                        {
                            string server = newsDiv.SelectSingleNode(".//p[@class='tag']").InnerText;
                            string title = newsDiv.SelectSingleNode(".//p[@class='title']").InnerText;
                            string taskid = newsDiv.GetAttributeValue("id", string.Empty);
                            string detecttime = newsDiv.SelectSingleNode(".//p[@class='time']").InnerText;
                            string link = newsDiv.SelectSingleNode(".//p/a").GetAttributeValue("href", string.Empty);
                            var listViewItem = new ListViewItem(new[] { server, title, taskid, detecttime});
                            listViewItem.Tag = link;
                            listViewExNotice.Items.Add(listViewItem);
                        }
                        labelItemStatus.Text = "公告导入完毕";
                    }
                    else
                    {
                        labelItemStatus.Text = "未查询到公告";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("加载公告失败\r\n" + ex.ToString(), "错误");
            }
        }

        private void listViewExNotice_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewExNotice.SelectedItems.Count > 0)
            {
                var selectedItem = listViewExNotice.SelectedItems[0];
                string link = selectedItem.Tag as string; 
                if (!string.IsNullOrEmpty(link))
                {
                    string message = $"是否要访问该公告？";
                    DialogResult result = MessageBoxEx.Show(message, "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = link,
                            UseShellExecute = true
                        });
                    }
                }
            }
        }

        private void listViewResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewResult.SelectedItems.Count > 0) 
            {
                var selectedItem = listViewResult.SelectedItems[0];
                if (cmbSearchContent.Text == "排名" && cmbCharInfo.Text == "综合排名")
                {
                    this.cmbSearchContent.Text = "角色";
                    this.textBoxIGN.Text = selectedItem.SubItems[3].Text;
                    this.btnIGN.PerformClick();
                }
                else if (cmbSearchContent.Text == "排名" && cmbCharInfo.Text == "联盟排名")
                {
                    this.textBoxDate.Text = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                    this.cmbSearchContent.Text = "角色";
                    this.textBoxIGN.Text = selectedItem.SubItems[2].Text;
                    this.btnIGN.PerformClick();
                    this.cmbCharInfo.Text = "联盟";
                    this.btnExecute.PerformClick();
                    this.cmbCharInfo.Text = "联盟突袭者";
                    this.btnExecute.PerformClick();
                    this.cmbCharInfo.Text = "联盟神器";
                    this.btnExecute.PerformClick();
                    AfrmUnion afrmUnion = this.charaSimCtrl.UIUnion;
                    afrmUnion.union_attackpower = selectedItem.SubItems[7].Text;
                    this.btnMapleUnion.Checked = true;
                }
                else if (cmbSearchContent.Text == "排名" && Regex.IsMatch(cmbCharInfo.Text, "武陵道场排名|起源之塔排名|成就排名"))
                {
                    this.cmbSearchContent.Text = "角色";
                    this.textBoxIGN.Text = selectedItem.SubItems[4].Text;
                    this.btnIGN.PerformClick();
                }
                else if (cmbSearchContent.Text == "公告" && Regex.IsMatch(cmbCharInfo.Text, "公告|更新公告|活动公告|现金商城公告"))
                {
                    this.cmbCharInfo.Text = cmbCharInfo.Text + "详情";
                    this.cmbNoticeid.Text = selectedItem.SubItems[2].Text;
                    this.btnExecute.PerformClick();
                }
            }
        }

        private void buttonItemAutoQuickView_Click(object sender, EventArgs e)
        {
            ConfigManager.Reload();
            CharaSimConfig.Default.AutoQuickView = buttonItemAutoQuickView.Checked;
            ConfigManager.Save();
        }

        private void panelExLeft_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                if (panelExLeft.Tag is int)
                {
                    int oldHeight = (int)panelExLeft.Tag;
                    advTree1.Height = (int)(1.0 * advTree1.Height / oldHeight * panelExLeft.Height);
                }
                panelExLeft.Tag = panelExLeft.Height;
            }
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
        }

        private void labelItemStatus_TextChanged(object sender, EventArgs e)
        {
            ribbonBar2.RecalcLayout();
        }

        private void btnNodeBack_Click(object sender, EventArgs e)
        {

        }

        private void btnNodeForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonItemUpdate_Click(object sender, EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/Jancy-49/WzComparerR2-CMS",
            });
#else
            Process.Start("https://github.com/Jancy-49/WzComparerR2-CMS");
#endif
        }
        
        private void buttonPapulatus_Click(object sender, EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/index.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/index.html");
#endif
        }

        private void btnItemOptions_Click(object sender, System.EventArgs e)
        {
            var frm = new FrmOptions();
            frm.Load(WcR2Config.Default);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ConfigManager.Reload();
                frm.Save(WcR2Config.Default);
                ConfigManager.Save();
                UpdateWzLoadingSettings();
                UpdateTranslateSettings();
            }
        }

        private void btnSkillName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Skill.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Skill.html");
#endif
        }

        private void btnQuestName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Quest.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Quest.html");
#endif
        }

        private void btnAchievementName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Achievement.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Achievement.html");
#endif
        }

        private void btnMapName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Map.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Map.html");
#endif
        }

        private void btnMobName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Mob.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Mob.html");
#endif
        }

        private void btnNpcName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/NPC.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/NPC.html");
#endif
        }
        private void btnEqpName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Eqp.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Eqp.html");
#endif
        }

        private void btnConsumeName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Consume.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Consume.html");
#endif
        }

        private void btnEtcName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Etc.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Etc.html");
#endif
        }

        private void btnInstallName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Ins.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Ins.html");
#endif
        }

        private void btnCashName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Cash.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Cash.html");
#endif
        }

        private void btnPetName_Click(object sender, System.EventArgs e)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://jancy-49.github.io/Papulatus-Daily/translate/Pet.html",
            });
#else
            Process.Start("https://jancy-49.github.io/Papulatus-Daily/translate/Pet.html");
#endif
        }

        private async void btnSearchString_Click(object sender, System.EventArgs e)
        {
            string searchType = cmbSearchItem.Text;
            string searchValue = cmbSearchValue.Text;
            string searchString = textBoxX2.Text;
            string url = $"https://jancy-49.github.io/Papulatus-Daily/translate/{searchType}.html";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var html = await httpClient.GetStringAsync(url);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    var dataDivs = doc.DocumentNode.SelectNodes("//div[@class='data']");
                    if (dataDivs != null)
                    {
                        bool found = false;
                        string finalResult = string.Empty;

                        foreach (var dataDiv in dataDivs)
                        {
                            var targetDiv = dataDiv.SelectSingleNode($"./div[@class='{searchValue}']");
                            if (targetDiv != null && targetDiv.InnerText.Contains(searchString))
                            {
                                found = true;
                                finalResult = $"";
                                var classNames = new[] { "code", "KMS", "MSEA", "GMS", "JMS", "TMS", "CMS", "MSN" };
                                foreach (var className in classNames)
                                {
                                    var div = dataDiv.SelectSingleNode($"./div[@class='{className}']");
                                    if (div != null)
                                    {
                                        finalResult += $"[{className}] {div.InnerText}\n";
                                    }
                                }
                                break;
                            }
                        }

                        if (found)
                        {
                            labelX2.Text = finalResult;
                        }
                        else
                        {
                            labelX2.Text = "未查询到文本";
                        }
                    }
                    else
                    {
                        labelX2.Text = "未查询到文本";
                    }
                }
            }
            catch (Exception ex)
            {
                labelX2.Text = $"网络请求失败: {ex.Message}";
            }
        }

        private void btnIGN_Click(object sender, System.EventArgs e)
        {
            string x_nxopen_api_key = textBoxAPIKey.Text;
            string charName = textBoxIGN.Text;
            string url = $"https://open.api.nexon.com/maplestory/v1/id?character_name={charName}";
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add("x-nxopen-api-key", x_nxopen_api_key);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    // 将WebResponse转换为HttpWebResponse
                    HttpWebResponse httpResponse = response as HttpWebResponse;
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseText = reader.ReadToEnd();
                            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                            string ocid = jsonResponse["ocid"];
                            textBoxocid.Text = ocid;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "错误");
            }
        }

        private void btnExecute_Click(object sender, System.EventArgs e)
        {
            string x_nxopen_api_key = textBoxAPIKey.Text;
            string ocid = textBoxocid.Text;
            string count = textBoxcount.Text;
            string Date = textBoxDate.Text;
            string Category = cmbSearchContent.Text;
            string InfoType = cmbCharInfo.Text;
            string SkillGrade = cmbSkillGrade.Text;
            string GuildName = textBoxGuildName.Text;
            string WorldName = cmbWorldName.Text;
            string oguild_id = textBoxGuildid.Text;
            string notice_id = cmbNoticeid.Text;
            string ranking_type = cmbRankingType.Text;
            string difficulty = cmbDifficulty.Text;
            string page = textBoxPage.Text;
            Dictionary<string, string> urls = new Dictionary<string, string>
            {
                { "基础信息", $"https://open.api.nexon.com/maplestory/v1/character/basic?ocid={ocid}&date={Date}" },
                { "角色列表", $"https://open.api.nexon.com/maplestory/v1/character/list?ocid={ocid}" },
                { "人气度", $"https://open.api.nexon.com/maplestory/v1/character/popularity?ocid={ocid}&date={Date}" },
                { "角色属性", $"https://open.api.nexon.com/maplestory/v1/character/stat?ocid={ocid}&date={Date}" },
                { "超级属性", $"https://open.api.nexon.com/maplestory/v1/character/hyper-stat?ocid={ocid}&date={Date}" },
                { "倾向值", $"https://open.api.nexon.com/maplestory/v1/character/propensity?ocid={ocid}&date={Date}" },
                { "内在能力", $"https://open.api.nexon.com/maplestory/v1/character/ability?ocid={ocid}&date={Date}" },
                { "道具装备", $"https://open.api.nexon.com/maplestory/v1/character/item-equipment?ocid={ocid}&date={Date}" },
                { "现金装备", $"https://open.api.nexon.com/maplestory/v1/character/cashitem-equipment?ocid={ocid}&date={Date}" },
                { "徽章装备", $"https://open.api.nexon.com/maplestory/v1/character/symbol-equipment?ocid={ocid}&date={Date}" },
                { "套装效果", $"https://open.api.nexon.com/maplestory/v1/character/set-effect?ocid={ocid}&date={Date}" },
                { "美容装备", $"https://open.api.nexon.com/maplestory/v1/character/beauty-equipment?ocid={ocid}&date={Date}" },
                { "机器人装备", $"https://open.api.nexon.com/maplestory/v1/character/android-equipment?ocid={ocid}&date={Date}" },
                { "宠物装备", $"https://open.api.nexon.com/maplestory/v1/character/pet-equipment?ocid={ocid}&date={Date}" },
                { "技能", $"https://open.api.nexon.com/maplestory/v1/character/skill?ocid={ocid}&date={Date}&character_skill_grade={SkillGrade}" },
                { "链接技能", $"https://open.api.nexon.com/maplestory/v1/character/link-skill?ocid={ocid}&date={Date}" },
                { "V矩阵", $"https://open.api.nexon.com/maplestory/v1/character/vmatrix?ocid={ocid}&date={Date}" },
                { "HEXA矩阵", $"https://open.api.nexon.com/maplestory/v1/character/hexamatrix?ocid={ocid}&date={Date}" },
                { "HEXA属性", $"https://open.api.nexon.com/maplestory/v1/character/hexamatrix-stat?ocid={ocid}&date={Date}" },
                { "武陵道场", $"https://open.api.nexon.com/maplestory/v1/character/dojang?ocid={ocid}&date={Date}" },
                { "联盟", $"https://open.api.nexon.com/maplestory/v1/user/union?ocid={ocid}&date={Date}" },
                { "联盟突袭者", $"https://open.api.nexon.com/maplestory/v1/user/union-raider?ocid={ocid}&date={Date}" },
                { "联盟神器", $"https://open.api.nexon.com/maplestory/v1/user/union-artifact?ocid={ocid}&date={Date}" },
                { "联盟冠军", $"https://open.api.nexon.com/maplestory/v1/user/union-champion?ocid={ocid}&date={Date}" },
                { "OUID", $"https://open.api.nexon.com/maplestory/v1/ouid" },
                { "星之力", $"https://open.api.nexon.com/maplestory/v1/history/starforce?count={count}" },
                { "潜在能力", $"https://open.api.nexon.com/maplestory/v1/history/potential?count={count}" },
                { "魔方", $"https://open.api.nexon.com/maplestory/v1/history/cube?count={count}" },
                { "公会ID", $"https://open.api.nexon.com/maplestory/v1/guild/id?guild_name={GuildName}&world_name={WorldName}" },
                { "公会信息", $"https://open.api.nexon.com/maplestory/v1/guild/basic?oguild_id={oguild_id}" },
                { "综合排名", $"https://open.api.nexon.com/maplestory/v1/ranking/overall?date={Date}" },
                { "联盟排名", $"https://open.api.nexon.com/maplestory/v1/ranking/union?date={Date}" },
                { "公会排名", $"https://open.api.nexon.com/maplestory/v1/ranking/guild?date={Date}&ranking_type={ranking_type}" },
                { "武陵道场排名", $"https://open.api.nexon.com/maplestory/v1/ranking/dojang?date={Date}&difficulty={difficulty}&page={page}" },
                { "起源之塔排名", $"https://open.api.nexon.com/maplestory/v1/ranking/theseed?date={Date}" },
                { "成就排名", $"https://open.api.nexon.com/maplestory/v1/ranking/achievement?date={Date}" },
                { "公告", $"https://open.api.nexon.com/maplestory/v1/notice" },
                { "公告详情", $"https://open.api.nexon.com/maplestory/v1/notice/detail?notice_id={notice_id}" },
                { "更新公告", $"https://open.api.nexon.com/maplestory/v1/notice-update" },
                { "更新公告详情", $"https://open.api.nexon.com/maplestory/v1/notice-update/detail?notice_id={notice_id}" },
                { "活动公告", $"https://open.api.nexon.com/maplestory/v1/notice-event" },
                { "活动公告详情", $"https://open.api.nexon.com/maplestory/v1/notice-event/detail?notice_id={notice_id}" },
                { "现金商城公告", $"https://open.api.nexon.com/maplestory/v1/notice-cashshop" },
                { "现金商城公告详情", $"https://open.api.nexon.com/maplestory/v1/notice-cashshop/detail?notice_id={notice_id}" },
            };
            string url = urls[InfoType];
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add("x-nxopen-api-key", x_nxopen_api_key);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    // 将WebResponse转换为HttpWebResponse
                    HttpWebResponse httpResponse = response as HttpWebResponse;
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseText = reader.ReadToEnd();
                            if (InfoType == "OUID")
                            {
                                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                                textBoxouid.Text = json["ouid"];
                            }
                            else if (InfoType == "公会ID")
                            {
                                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                                textBoxGuildid.Text = json["oguild_id"];
                            }
                            else if (InfoType == "公告" || InfoType == "更新公告" || InfoType == "活动公告" || InfoType == "现金商城公告")
                            {
                                textBoxResult.Text = responseText;
                                JObject jsonResponse = JObject.Parse(responseText);
                                JArray notices = null;
                                JArray resultArray = null;
                                switch (InfoType)
                                {
                                    case "公告": resultArray = (JArray)jsonResponse["notice"]; notices = jsonResponse["notice"].Value<JArray>(); break;
                                    case "更新公告": resultArray = (JArray)jsonResponse["update_notice"]; notices = jsonResponse["update_notice"].Value<JArray>(); break;
                                    case "活动公告": resultArray = (JArray)jsonResponse["event_notice"]; notices = jsonResponse["event_notice"].Value<JArray>(); break;
                                    case "现金商城公告": resultArray = (JArray)jsonResponse["cashshop_notice"]; notices = jsonResponse["cashshop_notice"].Value<JArray>(); break;
                                    default: break;
                                }
                                cmbNoticeid.Items.Clear();
                                foreach (JObject notice in notices)
                                {
                                    string noticeId = notice.Value<string>("notice_id");
                                    cmbNoticeid.Items.Add(noticeId);
                                }
                                listViewResult.Items.Clear();
                                listViewResult.Columns.Clear();
                                if (resultArray.Count > 0)
                                {
                                    var keys = resultArray[0].ToObject<Dictionary<string, object>>().Keys;
                                    foreach (var key in keys)
                                    {
                                        listViewResult.Columns.Add(key);
                                    }
                                    foreach (var item in resultArray)
                                    {
                                        var values = item.ToObject<Dictionary<string, object>>().Values.Select(v => v?.ToString() ?? "").ToArray();
                                        ListViewItem listViewItem = new ListViewItem(values.First().ToString());
                                        listViewItem.SubItems.AddRange(values.Skip(1).Select(v => v.ToString()).ToArray());
                                        listViewResult.Items.Add(listViewItem);
                                    }
                                }
                            }
                            else if (InfoType == "联盟" || InfoType == "联盟突袭者" || InfoType == "联盟神器" || InfoType == "联盟冠军")
                            {
                                textBoxResult.Text = responseText;
                                AfrmUnion afrmUnion = this.charaSimCtrl.UIUnion;
                                switch (InfoType)
                                {
                                    case "联盟":
                                        var union = JsonConvert.DeserializeObject<JObject>(responseText);
                                        afrmUnion.union_level = union["union_level"].ToString(); 
                                        afrmUnion.union_grade = union["union_grade"].ToString(); 
                                        afrmUnion.union_artifact_level = union["union_artifact_level"].Value<int>(); 
                                        afrmUnion.union_artifact_exp = union["union_artifact_exp"].Value<int>(); 
                                        afrmUnion.union_artifact_point = union["union_artifact_point"].ToString(); 
                                        break;
                                    case "联盟突袭者":
                                        JObject jsonAttacker = JObject.Parse(responseText);
                                        string use_preset_no = jsonAttacker["use_preset_no"].ToString();
                                        afrmUnion.resultJson = jsonAttacker;
                                        afrmUnion.union_preset = jsonAttacker["use_preset_no"].Value<int>();
                                        break;
                                    case "联盟神器":
                                        JObject jsonArtifact = JObject.Parse(responseText);
                                        afrmUnion.artifact_ap = jsonArtifact["union_artifact_remain_ap"].ToString();
                                        break;
                                    case "联盟冠军":
                                        JObject jsonChampion = JObject.Parse(responseText);
                                        break;
                                    default: break;
                                }
                            }
                            else if (Category == "排名")
                            {
                                JObject jsonResponse = JObject.Parse(responseText);
                                JArray resultArray = (JArray)jsonResponse["ranking"];
                                listViewResult.Items.Clear();
                                listViewResult.Columns.Clear();
                                if (resultArray.Count > 0)
                                {
                                    var keys = resultArray[0].ToObject<Dictionary<string, object>>().Keys;
                                    foreach (var key in keys)
                                    {
                                        listViewResult.Columns.Add(key);
                                    }
                                    foreach (var item in resultArray)
                                    {
                                        var values = item.ToObject<Dictionary<string, object>>().Values.Select(v => v?.ToString() ?? "").ToArray();
                                        ListViewItem listViewItem = new ListViewItem(values.First().ToString());
                                        listViewItem.SubItems.AddRange(values.Skip(1).Select(v => v.ToString()).ToArray());
                                        listViewResult.Items.Add(listViewItem);
                                    }
                                }
                            }
                            else
                            {
                                textBoxResult.Text = responseText;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "错误");
            }
        }

        private void cmbCharInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchContent.SelectedItem.ToString() == "角色" && this.cmbCharInfo.SelectedItem.ToString() == "技能")
            {
                this.labelSkillGrade.Visible = true;
                this.cmbSkillGrade.Visible = true;
                this.btnExecute.Location = new System.Drawing.Point(546, 53);
                this.btnPreview.Location = new System.Drawing.Point(606, 53);
            }
            else if (cmbSearchContent.SelectedItem.ToString() == "排名" && this.cmbCharInfo.SelectedItem.ToString() == "公会排名")
            {
                this.labelRankingType.Visible = true;
                this.cmbRankingType.Visible = true;
                this.labelDifficulty.Visible = false;
                this.cmbDifficulty.Visible = false;
                this.labelPage.Visible = false;
                this.textBoxPage.Visible = false;
            }
            else if (cmbSearchContent.SelectedItem.ToString() == "排名" && this.cmbCharInfo.SelectedItem.ToString() == "武陵道场排名")
            {
                this.labelRankingType.Visible = false;
                this.cmbRankingType.Visible = false;
                this.labelDifficulty.Visible = true;
                this.cmbDifficulty.Visible = true;
                this.labelPage.Visible = true;
                this.textBoxPage.Visible = true;
            }
            else
            {
                this.labelRankingType.Visible = false;
                this.cmbRankingType.Visible = false;
                this.labelDifficulty.Visible = false;
                this.cmbDifficulty.Visible = false;
                this.labelPage.Visible = false;
                this.textBoxPage.Visible = false;
                this.labelSkillGrade.Visible = false;
                this.cmbSkillGrade.Visible = false;
                this.btnExecute.Location = new System.Drawing.Point(366, 53);
                this.btnPreview.Location = new System.Drawing.Point(426, 53);
            }
        }

        private void cmbSearchContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmbCharInfo.Items.Clear();
            switch (cmbSearchContent.SelectedItem)
            {
                case "角色":
                    this.cmbCharInfo.Items.Add("基础信息");
                    this.cmbCharInfo.Items.Add("人气度");
                    this.cmbCharInfo.Items.Add("角色属性");
                    this.cmbCharInfo.Items.Add("超级属性");
                    this.cmbCharInfo.Items.Add("倾向值");
                    this.cmbCharInfo.Items.Add("内在能力");
                    this.cmbCharInfo.Items.Add("道具装备");
                    this.cmbCharInfo.Items.Add("现金装备");
                    this.cmbCharInfo.Items.Add("徽章装备");
                    this.cmbCharInfo.Items.Add("套装效果");
                    this.cmbCharInfo.Items.Add("美容装备");
                    this.cmbCharInfo.Items.Add("机器人装备");
                    this.cmbCharInfo.Items.Add("宠物装备");
                    this.cmbCharInfo.Items.Add("宠物装备");
                    this.cmbCharInfo.Items.Add("技能");
                    this.cmbCharInfo.Items.Add("链接技能");
                    this.cmbCharInfo.Items.Add("V矩阵");
                    this.cmbCharInfo.Items.Add("HEXA矩阵");
                    this.cmbCharInfo.Items.Add("HEXA属性");
                    this.cmbCharInfo.Items.Add("武陵道场");
                    this.cmbCharInfo.Items.Add("联盟");
                    this.cmbCharInfo.Items.Add("联盟突袭者");
                    this.cmbCharInfo.Items.Add("联盟神器");
                    this.cmbCharInfo.Items.Add("联盟冠军");
                    this.labelSearchContent.Visible = true;
                    this.cmbCharInfo.Visible = true;
                    this.btnExecute.Visible = true;
                    this.btnPreview.Visible = true;
                    this.labelIGN.Visible = true;
                    this.btnIGN.Visible = true;
                    this.textBoxIGN.Visible = true;
                    this.labelocid.Visible = true;
                    this.textBoxocid.Visible = true;
                    this.labelDate.Location = new System.Drawing.Point(516, 96);
                    this.labelDate.Visible = true;
                    this.textBoxDate.Location = new System.Drawing.Point(576, 96);
                    this.textBoxDate.Visible = true;
                    this.labelouid.Visible = false;
                    this.textBoxouid.Visible = false;
                    this.labelcount.Visible = false;
                    this.textBoxcount.Visible = false;
                    this.textBoxGuildid.Visible = false;
                    this.labelGuildid.Visible = false;
                    this.textBoxGuildName.Visible = false;
                    this.labelGuildName.Visible = false;
                    this.labelWorldName.Visible = false;
                    this.cmbWorldName.Visible = false;
                    this.labelRankingType.Visible = false;
                    this.cmbRankingType.Visible = false;
                    this.labelPage.Visible = false;
                    this.cmbDifficulty.Visible = false;
                    this.labelnoticeid.Visible = false;
                    this.cmbNoticeid.Visible = false;
                    this.textBoxResult.Visible = true;
                    break;
                case "个人":
                    this.cmbCharInfo.Items.Add("角色列表");
                    this.cmbCharInfo.Items.Add("OUID");
                    this.cmbCharInfo.Items.Add("星之力");
                    this.cmbCharInfo.Items.Add("潜在能力");
                    this.cmbCharInfo.Items.Add("魔方");
                    this.labelSearchContent.Visible = true;
                    this.cmbCharInfo.Visible = true;
                    this.btnExecute.Visible = true;
                    this.btnPreview.Visible = false;
                    this.labelouid.Visible = true;
                    this.labelcount.Visible = true;
                    this.textBoxcount.Visible = true;
                    this.textBoxouid.Visible = true;
                    this.labelIGN.Visible = false;
                    this.btnIGN.Visible = false;
                    this.textBoxIGN.Visible = false;
                    this.labelocid.Visible = false;
                    this.textBoxocid.Visible = false;
                    this.labelDate.Visible = false;
                    this.textBoxDate.Visible = false;
                    this.textBoxGuildid.Visible = false;
                    this.labelGuildid.Visible = false;
                    this.textBoxGuildName.Visible = false;
                    this.labelGuildName.Visible = false;
                    this.labelWorldName.Visible = false;
                    this.cmbWorldName.Visible = false;
                    this.labelRankingType.Visible = false;
                    this.cmbRankingType.Visible = false;
                    this.labelPage.Visible = false;
                    this.textBoxPage.Visible = false;
                    this.cmbDifficulty.Visible = false;
                    this.labelnoticeid.Visible = false;
                    this.cmbNoticeid.Visible = false;
                    this.textBoxResult.Visible = true;
                    break;
                case "公会":
                    this.cmbCharInfo.Items.Add("公会ID");
                    this.cmbCharInfo.Items.Add("公会信息");
                    this.labelSearchContent.Visible = true;
                    this.cmbCharInfo.Visible = true;
                    this.btnExecute.Visible = true;
                    this.btnPreview.Visible = false;
                    this.textBoxGuildid.Visible = true;
                    this.labelGuildid.Visible = true;
                    this.textBoxGuildName.Visible = true;
                    this.labelGuildName.Visible = true;
                    this.labelWorldName.Visible = true;
                    this.cmbWorldName.Visible = true;
                    this.labelIGN.Visible = false;
                    this.btnIGN.Visible = false;
                    this.textBoxIGN.Visible = false;
                    this.labelocid.Visible = false;
                    this.textBoxocid.Visible = false;
                    this.labelDate.Visible = false;
                    this.textBoxDate.Visible = false;
                    this.labelouid.Visible = false;
                    this.textBoxouid.Visible = false;
                    this.labelcount.Visible = false;
                    this.textBoxcount.Visible = false;
                    this.labelRankingType.Visible = false;
                    this.cmbRankingType.Visible = false;
                    this.labelPage.Visible = false;
                    this.textBoxPage.Visible = false;
                    this.cmbDifficulty.Visible = false;
                    this.labelnoticeid.Visible = false;
                    this.cmbNoticeid.Visible = false;
                    this.textBoxResult.Visible = true;
                    break;
                case "排名":
                    this.cmbCharInfo.Items.Add("综合排名");
                    this.cmbCharInfo.Items.Add("联盟排名");
                    this.cmbCharInfo.Items.Add("公会排名");
                    this.cmbCharInfo.Items.Add("武陵道场排名");
                    this.cmbCharInfo.Items.Add("起源之塔排名");
                    this.cmbCharInfo.Items.Add("成就排名");
                    this.labelDate.Location = new System.Drawing.Point(426, 53);
                    this.labelDate.Visible = true;
                    this.textBoxDate.Location = new System.Drawing.Point(476, 53);
                    this.textBoxDate.Visible = true;
                    this.labelSearchContent.Visible = true;
                    this.cmbCharInfo.Visible = true;
                    this.btnExecute.Visible = true;
                    this.btnPreview.Visible = false;
                    this.labelIGN.Visible = false;
                    this.btnIGN.Visible = false;
                    this.textBoxIGN.Visible = false;
                    this.labelocid.Visible = false;
                    this.textBoxocid.Visible = false;
                    this.labelouid.Visible = false;
                    this.textBoxouid.Visible = false;
                    this.labelcount.Visible = false;
                    this.textBoxcount.Visible = false;
                    this.textBoxGuildid.Visible = false;
                    this.labelGuildid.Visible = false;
                    this.textBoxGuildName.Visible = false;
                    this.labelGuildName.Visible = false;
                    this.labelWorldName.Visible = false;
                    this.cmbWorldName.Visible = false;
                    this.labelnoticeid.Visible = false;
                    this.cmbNoticeid.Visible = false;
                    this.textBoxResult.Visible = false;
                    break;
                case "公告":
                    this.cmbCharInfo.Items.Add("公告");
                    this.cmbCharInfo.Items.Add("公告详情");
                    this.cmbCharInfo.Items.Add("更新公告");
                    this.cmbCharInfo.Items.Add("更新公告详情");
                    this.cmbCharInfo.Items.Add("活动公告");
                    this.cmbCharInfo.Items.Add("活动公告详情");
                    this.cmbCharInfo.Items.Add("现金商城公告");
                    this.cmbCharInfo.Items.Add("现金商城公告详情");
                    this.labelSearchContent.Visible = true;
                    this.cmbCharInfo.Visible = true;
                    this.btnExecute.Visible = true;
                    this.labelnoticeid.Visible = true;
                    this.cmbNoticeid.Visible = true;
                    this.btnPreview.Visible = false;
                    this.labelIGN.Visible = false;
                    this.btnIGN.Visible = false;
                    this.textBoxIGN.Visible = false;
                    this.labelocid.Visible = false;
                    this.textBoxocid.Visible = false;
                    this.labelDate.Visible = false;
                    this.textBoxDate.Visible = false;
                    this.labelouid.Visible = false;
                    this.textBoxouid.Visible = false;
                    this.labelcount.Visible = false;
                    this.textBoxcount.Visible = false;
                    this.textBoxGuildid.Visible = false;
                    this.labelGuildid.Visible = false;
                    this.textBoxGuildName.Visible = false;
                    this.labelGuildName.Visible = false;
                    this.labelWorldName.Visible = false;
                    this.cmbWorldName.Visible = false;
                    this.labelRankingType.Visible = false;
                    this.cmbRankingType.Visible = false;
                    this.textBoxGuildName.Visible = false;
                    this.labelGuildName.Visible = false;
                    this.labelPage.Visible = false;
                    this.textBoxPage.Visible = false;
                    this.cmbDifficulty.Visible = false;
                    this.textBoxResult.Visible = true;
                    break;
            }
        }

        private async void btnLoadDojo_Click(object sender, EventArgs e)
        {
            string url = "https://jancy-49.github.io/Papulatus-Daily/Dojo/bestrecord.html";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var html = await httpClient.GetStringAsync(url);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var dojoRecords = doc.DocumentNode.SelectNodes("//div[@class='dojorecord']");
                    if (dojoRecords != null)
                    {
                        listViewResult.Columns.Clear();
                        listViewResult.Columns.Add("排名", 50);
                        listViewResult.Columns.Add("职业", 150); 
                        listViewResult.Columns.Add("角色名", 100); 
                        listViewResult.Columns.Add("层数", 50); 
                        listViewResult.Columns.Add("用时", 100); 
                        listViewResult.Columns.Add("等级", 50);
                        listViewResult.Columns.Add("记录时间", 100);
                        listViewResult.Items.Clear();
                        foreach (var info in dojoRecords)
                        {
                            string order = info.SelectSingleNode(".//div[@class='order']").InnerText;
                            string Job = info.SelectSingleNode(".//div[@class='Job']").InnerText;
                            string charname = info.SelectSingleNode(".//div[@class='charname']").InnerText;
                            string Floor = info.SelectSingleNode(".//div[@class='Floor']").InnerText;
                            string Dojotime = info.SelectSingleNode(".//div[@class='Dojotime']").InnerText;
                            string Level = info.SelectSingleNode(".//div[@class='Level']").InnerText;
                            string recorddate = info.SelectSingleNode(".//div[@class='recorddate']").InnerText;
                            var listViewItem = new ListViewItem(new[] { order, Job, charname, Floor, Dojotime, Level, recorddate });
                            listViewResult.Items.Add(listViewItem);
                        }
                        labelItemStatus.Text = "武陵道场最佳记录加载完毕";
                    }
                }
            }
            catch (Exception ex)
            {
                labelItemStatus.Text = "武陵道场最佳记录加载失败";
                MessageBoxEx.Show("加载武陵道场最佳记录失败\r\n" + ex.ToString(), "错误");
            }
        }

        private void btnLoadDaily_Click(object sender, EventArgs e)
        {
            if (dailyReportForm.ShowDialog() == DialogResult.OK)
            {
                DateTime selectedDate = dateTimePickerDailyReport.Value;
                LoadDailyReport(selectedDate);
            }
        }

        private void btnConfirmDailyReport_Click(object sender, EventArgs e)
        {
            dailyReportForm.DialogResult = DialogResult.OK;
            dailyReportForm.Close();
        }

        private void LoadDailyReport(DateTime date)
        {
#if NET6_0_OR_GREATER
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = $"https://jancy-49.github.io/Papulatus-Daily/Daily/Papulatus_Daily({date:yyyy-MM-dd}).html",
            });
#else
            Process.Start($"https://jancy-49.github.io/Papulatus-Daily/Daily/Papulatus_Daily({date:yyyy-MM-dd}).html");
#endif
        }
    }

    #region 内部用扩展方法
    internal static partial class Ext
    {
        public static Wz_Node AsWzNode(this Node node)
        {
            return (node?.Tag as WeakReference)?.Target as Wz_Node;
        }
    }
    #endregion
}
