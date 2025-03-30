﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using WzComparerR2.Patcher;

namespace WzComparerR2.Config
{
    [SectionName("WcR2")]
    public sealed class WcR2Config : ConfigSectionBase<WcR2Config>
    {
        public WcR2Config()
        {
<<<<<<< HEAD
            this.MainStyle = DevComponents.DotNetBar.eStyle.Office2016;
=======
<<<<<<< HEAD
            this.MainStyle = DevComponents.DotNetBar.eStyle.Office2016;
=======
            this.MainStyle = DevComponents.DotNetBar.eStyle.Office2007VistaGlass;
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            this.MainStyleColor = Color.DimGray;
            this.SortWzOnOpened = true;
            this.AutoDetectExtFiles = true;
            this.NoPatcherPrompt = false;
            this.WzVersionVerifyMode = WzLib.WzVersionVerifyMode.Fast;
            this.PreferredLayout = 0;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            this.DesiredLanguage = "ja";
            this.MozhiBackend = "https://mozhi.aryak.me";
            this.DetectCurrency = "auto";
            this.DesiredCurrency = "none";
            this.OpenAIExtraOption = false;
            this.LMTemperature = 0.7;
            this.MaximumToken = -1;
<<<<<<< HEAD
=======
=======
            this.DesiredLanguage = "zh-CN";
            this.MozhiBackend = "https://mozhi.aryak.me";
            this.DetectCurrency = "auto";
            this.DesiredCurrency = "none";
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        }

        /// <summary>
        /// 获取最近打开的文档列表。
        /// </summary>
        [ConfigurationProperty("recentDocuments")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigArrayList<string> RecentDocuments
        {
            get { return (ConfigArrayList<string>)this["recentDocuments"]; }
        }

        /// <summary>
        /// 获取或设置主窗体界面样式。
        /// </summary>
        [ConfigurationProperty("mainStyle")]
        public ConfigItem<DevComponents.DotNetBar.eStyle> MainStyle
        {
            get { return (ConfigItem<DevComponents.DotNetBar.eStyle>)this["mainStyle"]; }
            set { this["mainStyle"] = value; }
        }

        /// <summary>
        /// 获取或设置主窗体界面主题色。
        /// </summary>
        [ConfigurationProperty("mainStyleColor")]
        public ConfigItem<Color> MainStyleColor
        {
            get { return (ConfigItem<Color>)this["mainStyleColor"]; }
            set { this["mainStyleColor"] = value; }
        }

        /// <summary>
        /// NXOpenAPI Configuration
        /// </summary>
        [ConfigurationProperty("nxOpenAPIKey")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> NxOpenAPIKey
        {
            get { return (ConfigItem<string>)this["nxOpenAPIKey"]; }
            set { this["nxOpenAPIKey"] = value; }
        }

        /// <summary>
        /// Mozhi Backend Configuration
        /// </summary>
        [ConfigurationProperty("MozhiBackend")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> MozhiBackend
        {
            get { return (ConfigItem<string>)this["MozhiBackend"]; }
            set { this["MozhiBackend"] = value; }
        }

        /// <summary>
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        /// Language Model Configuration
        /// </summary>
        [ConfigurationProperty("LanguageModel")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> LanguageModel
        {
            get { return (ConfigItem<string>)this["LanguageModel"]; }
            set { this["LanguageModel"] = value; }
        }

        /// <summary>
        /// OpenAI Backend Configuration
        /// </summary>
        [ConfigurationProperty("OpenAIBackend")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> OpenAIBackend
        {
            get { return (ConfigItem<string>)this["OpenAIBackend"]; }
            set { this["OpenAIBackend"] = value; }
        }

        /// <summary>
        /// Enable Open AI Extra Option Configuration
        /// </summary>
        [ConfigurationProperty("OpenAIExtraOption")]
        public ConfigItem<bool> OpenAIExtraOption
        {
            get { return (ConfigItem<bool>)this["OpenAIExtraOption"]; }
            set { this["OpenAIExtraOption"] = value; }
        }

        /// <summary>
        /// Language Model Temperature Configuration
        /// </summary>
        [ConfigurationProperty("LMTemperature")]
        [ConfigurationCollection(typeof(ConfigArrayList<double>.ItemElement))]
        public ConfigItem<double> LMTemperature
        {
            get { return (ConfigItem<double>)this["LMTemperature"]; }
            set { this["LMTemperature"] = value; }
        }

        /// <summary>
        /// Language Model Maximum Token Configuration
        /// </summary>
        [ConfigurationProperty("MaximumToken")]
        [ConfigurationCollection(typeof(ConfigArrayList<int>.ItemElement))]
        public ConfigItem<int> MaximumToken
        {
            get { return (ConfigItem<int>)this["MaximumToken"]; }
            set { this["MaximumToken"] = value; }
        }

        /// <summary>
<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        /// Desired Language Configuration
        /// </summary>
        [ConfigurationProperty("DesiredLanguage")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> DesiredLanguage
        {
            get { return (ConfigItem<string>)this["DesiredLanguage"]; }
            set { this["DesiredLanguage"] = value; }
        }

        /// <summary>
        /// Preferred Translate Engine Configuration
        /// </summary>
        [ConfigurationProperty("PreferredTranslateEngine")]
        public ConfigItem<int> PreferredTranslateEngine
        {
            get { return (ConfigItem<int>)this["PreferredTranslateEngine"]; }
            set { this["PreferredTranslateEngine"] = value; }
        }

        /// <summary>
        /// Preferred Layout Configuration
        /// </summary>
        [ConfigurationProperty("PreferredLayout")]
        public ConfigItem<int> PreferredLayout
        {
            get { return (ConfigItem<int>)this["PreferredLayout"]; }
            set { this["PreferredLayout"] = value; }
        }

        /// <summary>
        /// Detect Currency Configuration
        /// </summary>
        [ConfigurationProperty("DetectCurrency")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> DetectCurrency
        {
            get { return (ConfigItem<string>)this["DetectCurrency"]; }
            set { this["DetectCurrency"] = value; }
        }

        /// <summary>
        /// Desired Currency Configuration
        /// </summary>
        [ConfigurationProperty("DesiredCurrency")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> DesiredCurrency
        {
            get { return (ConfigItem<string>)this["DesiredCurrency"]; }
            set { this["DesiredCurrency"] = value; }
        }

        /// <summary>
        /// NXSecretKey Configuration
        /// </summary>
        [ConfigurationProperty("nxSecretKey")]
        [ConfigurationCollection(typeof(ConfigArrayList<string>.ItemElement))]
        public ConfigItem<string> NxSecretKey
        {
            get { return (ConfigItem<string>)this["nxSecretKey"]; }
            set { this["nxSecretKey"] = value; }
        }

        /// <summary>
        /// 获取或设置Wz对比报告默认输出文件夹。
        /// </summary>
        [ConfigurationProperty("comparerOutputFolder")]
        public ConfigItem<string> ComparerOutputFolder
        {
            get { return (ConfigItem<string>)this["comparerOutputFolder"]; }
            set { this["comparerOutputFolder"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示Wz文件加载后是否自动排序。
        /// </summary>
        [ConfigurationProperty("sortWzOnOpened")]
        public ConfigItem<bool> SortWzOnOpened
        {
            get { return (ConfigItem<bool>)this["sortWzOnOpened"]; }
            set { this["sortWzOnOpened"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示Wz文件加载后是否自动排序。
        /// </summary>
        [ConfigurationProperty("sortWzByImgID")]
        public ConfigItem<bool> SortWzByImgID
        {
            get { return (ConfigItem<bool>)this["sortWzByImgID"]; }
            set { this["sortWzByImgID"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示Wz加载中对于ansi字符串的编码。
        /// </summary>
        [ConfigurationProperty("wzEncoding")]
        public ConfigItem<int> WzEncoding
        {
            get { return (ConfigItem<int>)this["wzEncoding"]; }
            set { this["wzEncoding"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示加载Base.wz时是否自动检测扩展wz文件（如Map2、Mob2）。
        /// </summary>
        [ConfigurationProperty("autoDetectExtFiles")]
        public ConfigItem<bool> AutoDetectExtFiles
        {
            get { return (ConfigItem<bool>)this["autoDetectExtFiles"]; }
            set { this["autoDetectExtFiles"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否不再提示游戏更新器通知。
        /// </summary>
        [ConfigurationProperty("noPatcherPrompt")]
        public ConfigItem<bool> NoPatcherPrompt
        {
            get { return (ConfigItem<bool>)this["noPatcherPrompt"]; }
            set { this["noPatcherPrompt"] = value; }
        }
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======

>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        /// <summary>
        /// 获取或设置一个值，指示读取wz是否跳过img检测。
        /// </summary>
        [ConfigurationProperty("imgCheckDisabled")]
        public ConfigItem<bool> ImgCheckDisabled
        {
            get { return (ConfigItem<bool>)this["imgCheckDisabled"]; }
            set { this["imgCheckDisabled"] = value; }
        }

        /// <summary>
        /// 获取或设置一个值，指示读取wz是否跳过img检测。
        /// </summary>
        [ConfigurationProperty("wzVersionVerifyMode")]
        public ConfigItem<WzLib.WzVersionVerifyMode> WzVersionVerifyMode
        {
            get { return (ConfigItem<WzLib.WzVersionVerifyMode>)this["wzVersionVerifyMode"]; }
            set { this["wzVersionVerifyMode"] = value; }
        }

        [ConfigurationProperty("patcherSettings")]
        [ConfigurationCollection(typeof(PatcherSetting), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public PatcherSettingCollection PatcherSettings
        {
            get { return (PatcherSettingCollection)this["patcherSettings"]; }
        }
    }
}