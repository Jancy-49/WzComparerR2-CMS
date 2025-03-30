using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.Editors;
using WzComparerR2.Config;
using System.Security.Policy;
using System.IO;
using Spine;
using Newtonsoft.Json.Linq;

namespace WzComparerR2
{
    public partial class FrmOptions : DevComponents.DotNetBar.Office2007Form
    {
        public FrmOptions()
        {
            InitializeComponent();
#if NET6_0_OR_GREATER
            // https://learn.microsoft.com/en-us/dotnet/core/compatibility/fx-core#controldefaultfont-changed-to-segoe-ui-9pt
            this.Font = new Font(new FontFamily("MS PGothic"), 9f);
#endif

            cmbWzEncoding.Items.AddRange(new[]
            {
                new ComboItem("基础"){ Value = 0 },
                new ComboItem("Shift-JIS (JMS)"){ Value = 932 },
                new ComboItem("GB2312 (CMS)"){ Value = 936 },
                new ComboItem("EUC-KR (KMS)"){ Value = 949 },
                new ComboItem("Big5 (TMS)"){ Value = 950 },
                new ComboItem("ISO-8859-1 (GMS)"){ Value = 1252 },
                new ComboItem("ASCII"){ Value = -1 },
            });

            cmbWzVersionVerifyMode.Items.AddRange(new[]
            {
                new ComboItem("基础"){ Value = WzLib.WzVersionVerifyMode.Default },
                new ComboItem("快速"){ Value = WzLib.WzVersionVerifyMode.Fast },
            });

            cmbDesiredLanguage.Items.AddRange(new[]
{
                new ComboItem("英语 (GMS/MSEA)"){ Value = "en" },
                new ComboItem("韩语 (KMS)"){ Value = "ko" },
                new ComboItem("粤语 (HKMS)"){ Value = "yue" },
                new ComboItem("简体中文 (CMS)"){ Value = "zh-CN" },
                new ComboItem("日语 (JMS)"){ Value = "ja" },
                new ComboItem("繁体中文 (TMS)"){ Value = "zh-TW" },
            });

            cmbMozhiBackend.Items.AddRange(new[]
            {
                new ComboItem("mozhi.aryak.me"){ Value = "https://mozhi.aryak.me" },
                new ComboItem("translate.bus-hit.me"){ Value = "https://translate.bus-hit.me" },
                new ComboItem("nyc1.mz.ggtyler.dev"){ Value = "https://nyc1.mz.ggtyler.dev" },
                new ComboItem("translate.projectsegfau.lt"){ Value = "https://translate.projectsegfau.lt" },
                new ComboItem("translate.nerdvpn.de"){ Value = "https://translate.nerdvpn.de" },
                new ComboItem("mozhi.ducks.party"){ Value = "https://mozhi.ducks.party" },
                new ComboItem("mozhi.frontendfriendly.xyz"){ Value = "https://mozhi.frontendfriendly.xyz" },
                new ComboItem("mozhi.pussthecat.org"){ Value = "https://mozhi.pussthecat.org" },
                new ComboItem("mo.zorby.top"){ Value = "https://mo.zorby.top" },
                new ComboItem("mozhi.adminforge.de"){ Value = "https://mozhi.adminforge.de" },
                new ComboItem("translate.privacyredirect.com"){ Value = "https://translate.privacyredirect.com" },
                new ComboItem("mozhi.canine.tools"){ Value = "https://mozhi.canine.tools" },
                new ComboItem("mozhi.gitro.xyz"){ Value = "https://mozhi.gitro.xyz" },
                new ComboItem("api.hikaricalyx.com"){ Value = "https://api.hikaricalyx.com/mozhi" },
            });

            cmbPreferredTranslateEngine.Items.AddRange(new[]
            {
                new ComboItem("Google (非Mozhi)"){ Value = 0 },
                new ComboItem("Google"){ Value = 1 },
                new ComboItem("DeepL"){ Value = 2 },
                new ComboItem("DuckDuckGo / Bing"){ Value = 3 },
                new ComboItem("MyMemory"){ Value = 4 },
                new ComboItem("Yandex"){ Value = 5 },
                new ComboItem("Naver Papago (非Mozhi)"){ Value = 6 },
<<<<<<< HEAD
                //new ComboItem("ディープシークAPI"){ Value = 7 },
                //new ComboItem("オラマLLM (ローカル)"){ Value = 8 },
                new ComboItem("OpenAI互换"){ Value = 9 },
=======
<<<<<<< HEAD
                //new ComboItem("ディープシークAPI"){ Value = 7 },
                //new ComboItem("オラマLLM (ローカル)"){ Value = 8 },
                new ComboItem("OpenAI互换"){ Value = 9 },
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            });

            cmbPreferredLayout.Items.AddRange(new[]
{
                new ComboItem("不翻译"){ Value = 0 },
                new ComboItem("先译文后原文"){ Value = 1 },
                new ComboItem("先原文后译文"){ Value = 2 },
                new ComboItem("仅译文"){ Value = 3 },
            });

            cmbDetectCurrency.Items.AddRange(new[]
            {
                new ComboItem("自动"){ Value = "auto" },
                new ComboItem("韩元 (KRW)"){ Value = "krw" },
                new ComboItem("新加坡元 (SGD)"){ Value = "sgd" },
                new ComboItem("新台币 (NTD)"){ Value = "twd" },
                new ComboItem("人民币 (CNY)"){ Value = "cny" },
                new ComboItem("日元 (JPY)"){ Value = "jpy" },
                new ComboItem("美元 (USD)"){ Value = "usd" },
            });

            cmbDesiredCurrency.Items.AddRange(new[]
            {
                new ComboItem("不转换"){ Value = "none" },
                new ComboItem("加元 (CAD)"){ Value = "cad" },
                new ComboItem("澳大利亚元 (AUD)"){ Value = "aud" },
                new ComboItem("韩元 (KRW)"){ Value = "krw" },
                new ComboItem("新加坡元 (SGD)"){ Value = "sgd" },
                new ComboItem("新台币 (NTD)"){ Value = "twd" },
                new ComboItem("人民币 (CNY)"){ Value = "cny" },
                new ComboItem("日元 (JPY)"){ Value = "jpy" },
                new ComboItem("美元 (USD)"){ Value = "usd" },
                new ComboItem("港元 (HKD)"){ Value = "hkd" },
                new ComboItem("澳门币 (MOP)"){ Value = "mop" },
                new ComboItem("马来西亚林吉特 (MYR)"){ Value = "myr" },
                new ComboItem("欧元 (EUR)"){ Value = "eur" },
            });
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            cmbLanguageModel.Items.AddRange(new[]
{
                new ComboItem("不变化"){ Value = "none" },
            });
<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        }

        public bool SortWzOnOpened
        {
            get { return chkWzAutoSort.Checked; }
            set { chkWzAutoSort.Checked = value; }
        }

        public bool SortWzByImgID
        {
            get { return chkWzSortByImgID.Checked; }
            set { chkWzSortByImgID.Checked = value; }
        }

        public int DefaultWzCodePage
        {
            get
            {
                return ((cmbWzEncoding.SelectedItem as ComboItem)?.Value as int?) ?? 0;
            }
            set
            {
                var items = cmbWzEncoding.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as int? == value)
                    ?? items.Last();
                item.Value = value;
                cmbWzEncoding.SelectedItem = item;
            }
        }

        public bool AutoDetectExtFiles
        {
            get { return chkAutoCheckExtFiles.Checked; }
            set { chkAutoCheckExtFiles.Checked = value; }
        }

        public bool ImgCheckDisabled
        {
            get { return chkImgCheckDisabled.Checked; }
            set { chkImgCheckDisabled.Checked = value; }
        }

        public string NxOpenAPIKey
        {
            get { return txtAPIkey.Text; }
            set { txtAPIkey.Text = value; }
        }
        public string GCloudAPIKey
        {
            get { return txtGCloudTranslateAPIkey.Text; }
            set { txtGCloudTranslateAPIkey.Text = value; }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public string OpenAIBackend
        {
            get { return txtOpenAIBackend.Text; }
            set { txtOpenAIBackend.Text = value; }
        }
<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public string NxSecretKey
        {
            get { return txtSecretkey.Text; }
            set { txtSecretkey.Text = value;}
        }
        public int PreferredLayout
        {
            get
            {
                return ((cmbPreferredLayout.SelectedItem as ComboItem)?.Value as int?) ?? 0;
            }
            set
            {
                var items = cmbPreferredLayout.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as int? == value)
                    ?? items.Last();
                item.Value = value;
                cmbPreferredLayout.SelectedItem = item;
            }
        }

        public string MozhiBackend
        {
            get
            {
                return ((cmbMozhiBackend.SelectedItem as ComboItem)?.Value as string) ?? "https://mozhi.aryak.me";
            }
            set
            {
                var items = cmbMozhiBackend.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as string == value)
                    ?? items.Last();
                item.Value = value;
                cmbMozhiBackend.SelectedItem = item;
            }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public string LanguageModel
        {
            get
            {
                return ((cmbLanguageModel.SelectedItem as ComboItem)?.Value as string) ?? "";
            }
            set
            {
                var items = cmbLanguageModel.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as string == value)
                    ?? items.Last();
                item.Value = value;
                cmbLanguageModel.SelectedItem = item;
            }
        }

        public bool OpenAIExtraOption
        {
            get { return chkOpenAIExtraOption.Checked; }
            set { chkOpenAIExtraOption.Checked = value; }
        }

        public double LMTemperature
        {
            get
            {
                return Double.Parse(txtLMTemperature.Text);
            }
            set
            {
                txtLMTemperature.Text = value.ToString();
            }
        }
        public int MaximumToken
        {
            get
            {
                return int.Parse(txtMaximumToken.Text);
            }
            set
            {
                txtMaximumToken.Text = value.ToString();
            }
        }

<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public string DetectCurrency
        {
            get
            {
                return ((cmbDetectCurrency.SelectedItem as ComboItem)?.Value as string) ?? "auto";
            }
            set
            {
                var items = cmbDetectCurrency.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as string == value)
                    ?? items.Last();
                item.Value = value;
                cmbDetectCurrency.SelectedItem = item;
            }
        }

        public string DesiredCurrency
        {
            get
            {
                return ((cmbDesiredCurrency.SelectedItem as ComboItem)?.Value as string) ?? "jpy";
            }
            set
            {
                var items = cmbDesiredCurrency.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as string == value)
                    ?? items.Last();
                item.Value = value;
                cmbDesiredCurrency.SelectedItem = item;
            }
        }

        public int PreferredTranslateEngine
        {
            get
            {
                return ((cmbPreferredTranslateEngine.SelectedItem as ComboItem)?.Value as int?) ?? 0;
            }
            set
            {
                var items = cmbPreferredTranslateEngine.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as int? == value)
                    ?? items.Last();
                item.Value = value;
                cmbPreferredTranslateEngine.SelectedItem = item;
            }
        }

        public string DesiredLanguage
        {
            get
            {
                return ((cmbDesiredLanguage.SelectedItem as ComboItem)?.Value as string) ?? "zh-CN";
            }
            set
            {
                var items = cmbDesiredLanguage.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as string == value)
                    ?? items.Last();
                item.Value = value;
                cmbDesiredLanguage.SelectedItem = item;
            }
        }
        private void buttonXCheck_Click(object sender, EventArgs e)
        {
            string respText;
            var req = WebRequest.Create(Program.NxAPIBaseURL + "/maplestory/v1/character/list") as HttpWebRequest;
            req.Timeout = 15000;
            req.Headers.Add("Accept", "application/json");
            req.Headers.Add("x-nxopen-api-key", txtAPIkey.Text);
            try
            {
                string respJson = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
                Clipboard.SetText(respJson);
                respText = "API 密钥有效。与" + Environment.NewLine + "API 密钥关联的字符已以 JSON 格式复制到您的剪贴板。";
            }
            catch (WebException ex)
            {
                string respJson = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                respText = "API 密钥无效。" + Environment.NewLine + respJson;
            }
            catch (Exception ex)
            {
                respText = "发生未知错误：" + ex;
            }
            MessageBoxEx.Show(respText);
        }

        private void buttonXCheck2_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            ComboItem selectedItem = (ComboItem)cmbPreferredTranslateEngine.SelectedItem;
            string respText;
            HttpWebRequest req;
            string backendAddress;
            if (string.IsNullOrEmpty(OpenAIBackend))
            {
                backendAddress = txtOpenAIBackend.WatermarkText;
            }
            else
            {
                backendAddress = OpenAIBackend;
            }
            switch ((int)selectedItem.Value)
            {
                case 8:
                case 9:
                    req = WebRequest.Create(backendAddress + "/models") as HttpWebRequest;
                    req.Timeout = 15000;
                    if (!string.IsNullOrEmpty(NxSecretKey))
                    {
                        JObject reqHeaders = JObject.Parse(NxSecretKey);
                        foreach (var property in reqHeaders.Properties()) req.Headers.Add(property.Name, property.Value.ToString());
                    }
                    try
                    {
                        string respJson = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        JObject jsonResp = JObject.Parse(respJson);
                        JArray dataArray = (JArray)jsonResp["data"];
                        StringBuilder sb = new StringBuilder();
                        cmbLanguageModel.Items.Clear();
                        foreach (JObject dataItem in dataArray)
                        {
                            sb.AppendLine(dataItem["id"].ToString());
                            cmbLanguageModel.Items.Add(new ComboItem(dataItem["id"].ToString()) { Value = dataItem["id"].ToString() });
                        }
                        cmbLanguageModel.SelectedIndex = 0;
                        respText = sb.ToString();
                    }
                    catch
                    {
                        respText = "该API未启用";
                    }
                    break;
                default:
                    req = WebRequest.Create((cmbMozhiBackend.SelectedItem as ComboItem)?.Value + "/api/engines") as HttpWebRequest;
                    req.Timeout = 15000;
                    try
                    {
                        string respJson = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        if (respJson.Contains("All Engines"))
                        {
                            respText = "Mozhi服务器有效。";
                        }
                        else
                        {
                            respText = "Mozhi服务器无效。";
                        }
                    }
                    catch (WebException ex)
                    {
                        string respJson = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        respText = "Mozhi服务器无效。";
                    }
                    catch (Exception ex)
                    {
                        respText = "未知错误：" + ex;
                    }
                    break;
            }
            MessageBoxEx.Show(respText);
        }

        private void buttonXCheck3_Click(object sender, EventArgs e)
        {
            string respText;
            JObject testJObject;
            try
            {
                testJObject = JObject.Parse(txtSecretkey.Text);
                respText = "JSON有效。";
            }
            catch
            {
                respText = "JSON无效。";
            }
            MessageBoxEx.Show(respText);
        }

        private void cmbPreferredTranslateEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboItem selectedItem = (ComboItem)cmbPreferredTranslateEngine.SelectedItem;
            switch ((int)selectedItem.Value)
            {
                case 0:
                case 6:
                    labelX5.Visible = true;
                    labelX5.Enabled = false;
                    labelX11.Visible = false;
                    labelX12.Enabled = false;
                    labelX13.Enabled = false;
                    labelX14.Enabled = false;
                    txtOpenAIBackend.Enabled = false;
                    txtLMTemperature.Enabled = false;
                    txtMaximumToken.Enabled = false;
                    cmbMozhiBackend.Visible = true;
                    cmbMozhiBackend.Enabled = false;
                    cmbLanguageModel.Visible = false;
                    chkOpenAIExtraOption.Enabled = false;
                    buttonXCheck2.Enabled = false;
                    break;
                case 8:
                case 9:
                    labelX5.Visible = false;
                    labelX11.Visible = true;
                    labelX12.Enabled = true;
                    labelX13.Enabled = chkOpenAIExtraOption.Checked;
                    labelX14.Enabled = chkOpenAIExtraOption.Checked;
                    txtOpenAIBackend.Enabled = true;
                    txtLMTemperature.Enabled = chkOpenAIExtraOption.Checked;
                    txtMaximumToken.Enabled = chkOpenAIExtraOption.Checked;
                    cmbMozhiBackend.Visible = false;
                    cmbLanguageModel.Visible = true;
                    chkOpenAIExtraOption.Enabled = true;
                    buttonXCheck2.Enabled = true;
                    break;
                default:
                    labelX5.Visible = true;
                    labelX5.Enabled = true;
                    labelX11.Visible = false;
                    labelX12.Enabled = false;
                    labelX13.Enabled = false;
                    labelX14.Enabled = false;
                    txtOpenAIBackend.Enabled = false;
                    txtLMTemperature.Enabled = false;
                    txtMaximumToken.Enabled = false;
                    cmbMozhiBackend.Visible = true;
                    cmbMozhiBackend.Enabled = true;
                    cmbLanguageModel.Visible = false;
                    chkOpenAIExtraOption.Enabled = false;
                    buttonXCheck2.Enabled = true;
                    break;
            }
        }
        private void chkOpenAIExtraOption_CheckedChanged(object sender, EventArgs e)
        {
            txtLMTemperature.Enabled = chkOpenAIExtraOption.Checked;
            txtMaximumToken.Enabled = chkOpenAIExtraOption.Checked;
        }
<<<<<<< HEAD
=======
=======
            string respText;
            var req = WebRequest.Create((cmbMozhiBackend.SelectedItem as ComboItem)?.Value + "/api/engines") as HttpWebRequest;
            req.Timeout = 15000;
            try
            {
                string respJson = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
                if (respJson.Contains("All Engines"))
                {
                    respText = "Mozhi服务有效。";
                }
                else
                {
                    respText = "Mozhi服务无效。";
                }
            }
            catch (WebException ex)
            {
                string respJson = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                respText = "Mozhi服务无效。";
            }
            catch (Exception ex)
            {
                respText = "发生未知错误：" + ex;
            }
            MessageBoxEx.Show(respText);
        }
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public WzLib.WzVersionVerifyMode WzVersionVerifyMode
        {
            get { return ((cmbWzVersionVerifyMode.SelectedItem as ComboItem)?.Value as WzLib.WzVersionVerifyMode?) ?? default; }
            set
            {
                var items = cmbWzVersionVerifyMode.Items.Cast<ComboItem>();
                var item = items.FirstOrDefault(_item => _item.Value as WzLib.WzVersionVerifyMode? == value)
                    ?? items.First();
                cmbWzVersionVerifyMode.SelectedItem = item;
            }
        }

        public void Load(WcR2Config config)
        {
            this.SortWzOnOpened = config.SortWzOnOpened;
            this.SortWzByImgID = config.SortWzByImgID;
            this.DefaultWzCodePage = config.WzEncoding;
            this.AutoDetectExtFiles = config.AutoDetectExtFiles;
            this.ImgCheckDisabled = config.ImgCheckDisabled;
            this.WzVersionVerifyMode = config.WzVersionVerifyMode;
            this.NxOpenAPIKey = config.NxOpenAPIKey;
            this.NxSecretKey = config.NxSecretKey;
            this.MozhiBackend = config.MozhiBackend;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            this.LanguageModel = config.LanguageModel;
            this.OpenAIBackend = config.OpenAIBackend;
            this.OpenAIExtraOption = config.OpenAIExtraOption;
            this.LMTemperature = config.LMTemperature;
            this.MaximumToken = config.MaximumToken;
<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            this.PreferredTranslateEngine = config.PreferredTranslateEngine;
            this.DesiredLanguage = config.DesiredLanguage;
            this.PreferredLayout = config.PreferredLayout;
            this.DetectCurrency = config.DetectCurrency;
            this.DesiredCurrency = config.DesiredCurrency;
        }

        public void Save(WcR2Config config)
        {
            config.SortWzOnOpened = this.SortWzOnOpened;
            config.SortWzByImgID = this.SortWzByImgID;
            config.WzEncoding = this.DefaultWzCodePage;
            config.AutoDetectExtFiles = this.AutoDetectExtFiles;
            config.ImgCheckDisabled = this.ImgCheckDisabled;
            config.WzVersionVerifyMode = this.WzVersionVerifyMode;
            config.NxOpenAPIKey = this.NxOpenAPIKey;
            config.NxSecretKey = this.NxSecretKey;
            config.MozhiBackend = this.MozhiBackend;
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            if (this.LanguageModel != "none") config.LanguageModel = this.LanguageModel;
            config.OpenAIBackend = this.OpenAIBackend;
            config.OpenAIExtraOption = this.OpenAIExtraOption;
            config.LMTemperature = this.LMTemperature;
            config.MaximumToken = this.MaximumToken;
<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            config.PreferredTranslateEngine = this.PreferredTranslateEngine;
            config.DesiredLanguage = this.DesiredLanguage;
            config.PreferredLayout = this.PreferredLayout;
            config.DetectCurrency = this.DetectCurrency;
            config.DesiredCurrency = this.DesiredCurrency;
        }
    }
}