using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using WzComparerR2.Patcher;

namespace WzComparerR2.Config
{
    [SectionName("WcR2")]
    public sealed class WcR2Config : ConfigSectionBase<WcR2Config>
    {
        public WcR2Config()
        {
            this.MainStyle = DevComponents.DotNetBar.eStyle.Office2016;
            this.MainStyleColor = Color.DimGray;
            this.SortWzOnOpened = true;
            this.AutoDetectExtFiles = true;
            this.AutoDetectUpdate = true;
            this.NoPatcherPrompt = false;
            this.PreferredLayout = 0;
            this.DesiredLanguage = "ja";
            this.MozhiBackend = "https://mozhi.aryak.me";
            this.DetectCurrency = "auto";
            this.DesiredCurrency = "none";
            this.OpenAIExtraOption = false;
            this.EnableAutoUpdate = true;
            this.LMTemperature = 0.7;
            this.MaximumToken = -1;
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
        /// Preferred Client Region Configuration
        /// </summary>
        [ConfigurationProperty("PreferredClientRegion")]
        public ConfigItem<int> PreferredClientRegion
        {
            get { return (ConfigItem<int>)this["PreferredClientRegion"]; }
            set { this["PreferredClientRegion"] = value; }
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
        /// Automatic Update Configuration
        /// </summary>
        [ConfigurationProperty("EnableAutoUpdate")]
        public ConfigItem<bool> EnableAutoUpdate
        {
            get { return (ConfigItem<bool>)this["EnableAutoUpdate"]; }
            set { this["EnableAutoUpdate"] = value; }
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

        [ConfigurationProperty("autoDetectUpdate")]
        public ConfigItem<bool> AutoDetectUpdate
        {
            get { return (ConfigItem<bool>)this["autoDetectUpdate"]; }
            set { this["autoDetectUpdate"] = value; }
        }

        [ConfigurationProperty("bucket")]
        public ConfigItem<string> Bucket
        {
            get { return (ConfigItem<string>)this["bucket"]; }
            set { this["bucket"] = value; }
        }

        [ConfigurationProperty("region")]
        public ConfigItem<string> Region
        {
            get { return (ConfigItem<string>)this["region"]; }
            set { this["region"] = value; }
        }

        [ConfigurationProperty("secretid")]
        public ConfigItem<string> SecretID
        {
            get { return (ConfigItem<string>)this["secretid"]; }
            set { this["secretid"] = value; }
        }

        [ConfigurationProperty("secretkey")]
        public ConfigItem<string> SecretKey
        {
            get { return (ConfigItem<string>)this["secretkey"]; }
            set { this["secretkey"] = value; }
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
        /// <summary>
        /// 获取或设置一个值，指示读取wz是否跳过img检测。
        /// </summary>
        [ConfigurationProperty("imgCheckDisabled")]
        public ConfigItem<bool> ImgCheckDisabled
        {
            get { return (ConfigItem<bool>)this["imgCheckDisabled"]; }
            set { this["imgCheckDisabled"] = value; }
        }

        [ConfigurationProperty("patcherSettings")]
        [ConfigurationCollection(typeof(PatcherSetting), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public PatcherSettingCollection PatcherSettings
        {
            get { return (PatcherSettingCollection)this["patcherSettings"]; }
        }

        private static readonly HashSet<string> obsoleteElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "wzVersionVerifyMode",
        };

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (obsoleteElements.Contains(elementName))
            {
                reader.Skip();
                return true;
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        [ConfigurationProperty("nexonOpenAPIKey")]
        public ConfigItem<string> NexonOpenAPIKey
        {
            get { return (ConfigItem<string>)this["nexonOpenAPIKey"]; }
            set { this["nexonOpenAPIKey"] = value; }
        }
    }
}