﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
<<<<<<< HEAD
using System.Text.RegularExpressions;
=======
<<<<<<< HEAD
using System.Text.RegularExpressions;
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
using WzComparerR2.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
using DevComponents.DotNetBar;
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
using System.Globalization;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
namespace WzComparerR2.CharaSim
{
    public class Translator
    {
        // L2C stands for Language to Currency
        private static Dictionary<string, string> dictL2C = new Dictionary<string, string>()
        {
            { "ja", "jpy" },
            { "ko", "krw" },
            { "zh-CN", "cny" },
            { "en", "usd" },
            { "zh-TW", "twd" }
        };

        private static Dictionary<string, string> dictC2L = new Dictionary<string, string>()
        {
            { "jpy", "ja" },
            { "krw", "ko" },
            { "cny", "zh-CN" },
            { "usd", "en" },
            { "twd", "zh-TW" },
            { "sgd", "en" }
        };

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        // Language Model Expression
        private static Dictionary<string, string> dictL2LM = new Dictionary<string, string>()
        {
            { "ja", "日语" },
            { "ko", "韩语" },
            { "zh-CN", "简体中文" },
            { "en", "英语" },
            { "zh-TW", "繁体中文" },
            { "yue", "粤语" }
        };

        private static Dictionary<string, string> dictCurrencyName = new Dictionary<string, string>()
        {
            { "jpy", "日元" },
            { "krw", "韩元" },
            { "cny", "人民币" },
            { "usd", "美元" },
            { "twd", "台币" },
            { "hkd", "港币" },
            { "mop", "澳门元" },
            { "sgd", "新加坡元" },
            { "eur", "欧元" },
            { "cad", "加元" },
            { "aud", "澳元" },
            { "myr", "马来西亚元" },
<<<<<<< HEAD
=======
=======
        private static Dictionary<string, string> dictCurrencyName = new Dictionary<string, string>()
        {
            { "jpy", "��" },
            { "krw", "������" },
            { "cny", "Ԫ" },
            { "usd", "�ɥ�" },
            { "twd", "̨��ɥ�" },
            { "hkd", "��ۥɥ�" },
            { "mop", "�ޥ����ѥ���" },
            { "sgd", "���󥬥ݩ`��ɥ�" },
            { "eur", "��`��" },
            { "cad", "���ʥ��ɥ�" },
            { "aud", "���`���ȥ�ꥢ�ɥ�" },
            { "myr", "�ޥ�`������󥮥å�" },
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        };

        private static string GTranslateBaseURL = "https://translate.googleapis.com/translate_a/t";
        private static string NTranslateBaseURL = "https://naveropenapi.apigw.ntruss.com";
<<<<<<< HEAD
        public static string OAITranslateBaseURL { get; set; }
=======
<<<<<<< HEAD
        public static string OAITranslateBaseURL { get; set; }
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4

        private static List<string> CurrencyBaseURL = new List<string>()
        {
            "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/",
            "https://latest.currency-api.pages.dev/v1/currencies/",
            "https://registry.npmmirror.com/@fawazahmed0/currency-api/latest/files/v1/currencies/"
        };

        private static JArray GTranslate(string text, string desiredLanguage)
        {
            var request = (HttpWebRequest)WebRequest.Create(GTranslateBaseURL + "?client=gtx&format=text&sl=auto&tl=" + desiredLanguage);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var postData = "q=" + Uri.EscapeDataString(text);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JArray.Parse(responseString);
            }
            catch
            {
                return JArray.Parse($"[[\"{text}\",\"{desiredLanguage}\"]]");
            }
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        private static string OAITranslate(string text, string desiredLanguage, bool singleLine = false)
        {
            if (string.IsNullOrEmpty(OAITranslateBaseURL)) OAITranslateBaseURL = "https://api.openai.com/v1";
            var request = (HttpWebRequest)WebRequest.Create(OAITranslateBaseURL + "/chat/completions");
            request.Method = "POST";
            request.ContentType = "application/json";
            if (!string.IsNullOrEmpty(DefaultTranslateAPIKey))
            {
                JObject reqHeaders = JObject.Parse(DefaultTranslateAPIKey);
                foreach (var property in reqHeaders.Properties()) request.Headers.Add(property.Name, property.Value.ToString());
            }
            var postData = new JObject(
                new JProperty("model", DefaultLanguageModel),
                new JProperty("messages", new JArray(
                    new JObject(
                        new JProperty("role", "system"),
                        new JProperty("content", "You are an automated translator for a community game engine.")
                    ),
                    new JObject(
                        new JProperty("role", "user"),
                        new JProperty("content", "Please translate following in-game content into " + dictL2LM[desiredLanguage] + ": " + text)
                    )
                )),
                new JProperty("stream", false)
            );
            if (IsExtraParamEnabled)
            {
                postData.Add(new JProperty("temperature", DefaultLMTemperature));
                postData.Add(new JProperty("max_tokens", DefaultMaximumToken));
            }
            var byteArray = System.Text.Encoding.UTF8.GetBytes(postData.ToString());
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jrResponse = JObject.Parse(responseString);
                string responseResult = jrResponse.SelectToken("choices[0].message.content").ToString();
                if (responseResult.Contains("</think>"))
                {
                    responseResult = responseResult.Split(new String[] { "</think>\n\n" }, StringSplitOptions.None)[1];
                }
                if (responseResult.Contains("：\n\n") || responseResult.Contains(":\n\n") || responseResult.Contains(": \n\n"))
                {
                    // TO DO: Put extra output to NetworkLogger.
                    responseResult = responseResult.Split(new String[] { "\n\n" }, StringSplitOptions.None)[1];
                    if (responseResult.Contains("\r")) responseResult = responseResult.Split(new String[] { "\r" }, StringSplitOptions.None)[0];
                    if (responseResult.Contains("\n")) responseResult = responseResult.Split(new String[] { "\n" }, StringSplitOptions.None)[0];
                }
                if (singleLine)
                {
                    return responseResult.Replace("\r\n", " ").Replace("\n", "").Replace("  ", " ");
                }
                else
                {
                    return responseResult;
                }

            }
            catch (Exception e)
            {
                return text;
            }
        }

<<<<<<< HEAD
=======
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public static bool IsKoreanStringPresent(string checkString)
        {
            if (checkString == null) return false;
            return checkString.Any(c => (c >= '\uAC00' && c <= '\uD7A3'));
        }

        private static string GetKeyValue(string jsonDictKey)
        {
            try
            {
                return JObject.Parse(DefaultTranslateAPIKey).SelectToken(jsonDictKey).ToString();
            }
            catch
            {
                return "";
            }

        }
        private static JObject MTranslate(string text, string engine, string sourceLanguage, string desiredLanguage)
        {
            var request = (HttpWebRequest)WebRequest.Create(DefaultMozhiBackend + "/api/translate?engine=" + engine + "&from=" + sourceLanguage + "&to=" + desiredLanguage + "&text=" + Uri.EscapeDataString(text));
            request.Accept = "application/json";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JObject.Parse(responseString);
            }
            catch
            {
                return JObject.Parse("{\"translated-text\": \"" + text + "\"}");
            }
        }

        private static JObject NTranslate(string text, string desiredLanguage)
        {
            var request = (HttpWebRequest)WebRequest.Create(NTranslateBaseURL + "/nmt/v1");
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["X-NCP-APIGW-API-KEY-ID"] = GetKeyValue("X-NCP-APIGW-API-KEY-ID");
            request.Headers["X-NCP-APIGW-API-KEY"] = GetKeyValue("X-NCP-APIGW-API-KEY-ID");
            var postData = "source=auto&target=" + desiredLanguage + "text=" + Uri.EscapeDataString(text);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JObject.Parse(responseString);
            }
            catch
            {
<<<<<<< HEAD
                return JObject.Parse("{\"message\": {\"result\": {\"translatedText\": \"无效Naver API\"}}}");
=======
<<<<<<< HEAD
                return JObject.Parse("{\"message\": {\"result\": {\"translatedText\": \"无效Naver API\"}}}");
=======
                return JObject.Parse("{\"message\": {\"result\": {\"translatedText\": \"�o����Naver API���`\"}}}");
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            }
        }

        public static string MergeString(string text1, string text2, int newLineCounts = 0, bool oneLineSeparatorRequired = false, bool bracketRequiredForText2 = false)
        {
            if (text1 == text2)
            {
                return text1;
            }
            else if (!string.IsNullOrEmpty(text1) && !string.IsNullOrEmpty(text2))
            {
                string resultStr;
                switch (DefaultPreferredLayout)
                {
                    case 1:
                        resultStr = text2;
                        if (newLineCounts == 0 && oneLineSeparatorRequired) resultStr += " / ";
                        if (newLineCounts == 0 && bracketRequiredForText2) resultStr += " ";
                        while (newLineCounts > 0)
                        {
                            resultStr += Environment.NewLine;
                            newLineCounts--;
                        }
                        if (bracketRequiredForText2) resultStr += "(";
                        resultStr += text1;
                        if (bracketRequiredForText2) resultStr += ")";
                        break;
                    case 2:
                        resultStr = text1;
                        if (newLineCounts == 0 && oneLineSeparatorRequired) resultStr += " / ";
                        if (newLineCounts == 0 && bracketRequiredForText2) resultStr += " ";
                        while (newLineCounts > 0)
                        {
                            resultStr += Environment.NewLine;
                            newLineCounts--;
                        }
                        if (bracketRequiredForText2) resultStr += "(";
                        resultStr += text2;
                        if (bracketRequiredForText2) resultStr += ")";
                        break;
                    case 3:
                        resultStr = text2;
                        break;
                    default:
                        resultStr = text1;
                        break;
                }
                return resultStr;
            }
            else
            {
                return text1;
            }
        }

        public static string TranslateString(string orgText, bool titleCase = false)
        {
            if (string.IsNullOrEmpty(orgText) || orgText == "(null)") return orgText;
            bool isMozhiUsed = false;
            string mozhiEngine = "";
            string translatedText = "";
            string sourceLanguage = "auto";
            string targetLanguage = DefaultDesiredLanguage;
            switch (DefaultPreferredTranslateEngine)
            {
                //0: Google (Non-Mozhi)
                default:
                case 0:
<<<<<<< HEAD
                    JArray responseArr = GTranslate(ConvHashTagToHTMLTag(orgText), Translator.DefaultDesiredLanguage);
                    translatedText = responseArr[0][0].ToString().Replace("＃", "#");
=======
<<<<<<< HEAD
                    JArray responseArr = GTranslate(ConvHashTagToHTMLTag(orgText), Translator.DefaultDesiredLanguage);
                    translatedText = responseArr[0][0].ToString().Replace("＃", "#");
=======
                    JArray responseArr = GTranslate(orgText.Replace("\\n", "\r\n"), Translator.DefaultDesiredLanguage);
                    translatedText = responseArr[0][0].ToString().Replace("\r\n", "\\n").Replace("��", "#");
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
                    break;
                //1: Google (Mozhi)
                case 1:
                    isMozhiUsed = true;
                    mozhiEngine = "google";
                    break;
                //2: DeepL (Mozhi)
                case 2:
                    isMozhiUsed = true;
                    sourceLanguage = "en";
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "deepl";
                    break;
                //3: DuckDuckGo / Bing (Mozhi)
                case 3:
                    isMozhiUsed = true;
                    if (targetLanguage == "zh-CN") targetLanguage = "zh";
                    mozhiEngine = "duckduckgo";
                    break;
                //4: MyMemory (Mozhi)
                case 4:
                    isMozhiUsed = true;
                    sourceLanguage = "Autodetect";
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "mymemory";
                    break;
                //5: Yandex (Mozhi)
                case 5:
                    isMozhiUsed = true;
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "yandex";
                    break;
                //6: Naver Papago (Non-Mozhi)
                case 6:
                    if (targetLanguage == "yue") targetLanguage = "zh-TW";
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
                    JObject responseObj = NTranslate(ConvHashTagToHTMLTag(orgText), Translator.DefaultDesiredLanguage);
                    translatedText = responseObj.SelectToken("message.result.translatedText").ToString();
                    break;
                //9: OpenAI Compatible
                case 9:
                    if (titleCase && targetLanguage == "en")
                    {
                        translatedText = OAITranslate(ConvHashTagToHTMLTag(orgText), Translator.DefaultDesiredLanguage, true);
                    }
                    else
                    {
                        translatedText = OAITranslate(ConvHashTagToHTMLTag(orgText), Translator.DefaultDesiredLanguage);
                    }
                    break;
            }
            if (isMozhiUsed)
            {
                translatedText = MTranslate(ConvHashTagToHTMLTag(orgText), mozhiEngine, sourceLanguage, targetLanguage).SelectToken("translated-text").ToString().Replace("＃", "#");
<<<<<<< HEAD
=======
=======
                    JObject responseObj = NTranslate(orgText.Replace("\\n", "\r\n"), Translator.DefaultDesiredLanguage);
                    translatedText = responseObj.SelectToken("message.result.translatedText").ToString();
                    break;
                    //7: iFlyTek (Non-Mozhi)
            }
            if (isMozhiUsed)
            {
                translatedText = MTranslate(orgText.Replace("\\n", "\r\n"), mozhiEngine, sourceLanguage, targetLanguage).SelectToken("translated-text").ToString().Replace("\r\n", "\\n").Replace("��", "#");
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            }
            if (titleCase && targetLanguage == "en")
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                translatedText = textInfo.ToTitleCase(translatedText);
            }
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            translatedText = ConvHTMLTagToHashTag(translatedText);
            return translatedText;
        }

        public static string ConvHashTagToHTMLTag(string orgText)
        {
            if (!string.IsNullOrEmpty(orgText))
            {
                return orgText.Replace("#c", "<CHL>").Replace("#", "</CHL>").Replace("\\r\\n", "<BR/>").Replace("\\n", "<BR/>");
            }
            else
            {
                return orgText;
            }
        }

        public static string ConvHTMLTagToHashTag(string orgText)
        {
            if (!string.IsNullOrEmpty(orgText))
            {
                return Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                orgText.Replace("< ", "<").Replace(" >", ">"),
                                "<CHL>", "#c", RegexOptions.IgnoreCase),
                            "</CHL>", "#", RegexOptions.IgnoreCase),
                        "<BR/>", "\r\n", RegexOptions.IgnoreCase),
                    "CHL>", "#c", RegexOptions.IgnoreCase);
            }
            else
            {
                return orgText;
            }
        }

<<<<<<< HEAD
=======
=======
            return translatedText;
        }

>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public static string GetLanguage(string orgText)
        {
            if (string.IsNullOrEmpty(orgText) || orgText == "(null)") return "ja";
            bool isMozhiUsed = false;
            string mozhiEngine = "";
            string orgLanguage = "";
            string sourceLanguage = "auto";
            string targetLanguage = DefaultDesiredLanguage;
            switch (DefaultPreferredTranslateEngine)
            {
                //0: Google (Non-Mozhi)
                default:
                case 0:
<<<<<<< HEAD
                    JArray responseArr = GTranslate(orgText, Translator.DefaultDesiredLanguage);
=======
<<<<<<< HEAD
                    JArray responseArr = GTranslate(orgText, Translator.DefaultDesiredLanguage);
=======
                    JArray responseArr = GTranslate(orgText.Replace("\\n", "\r\n"), Translator.DefaultDesiredLanguage);
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
                    orgLanguage = responseArr[0][1].ToString();
                    break;
                //1: Google (Mozhi)
                case 1:
                    isMozhiUsed = true;
                    mozhiEngine = "google";
                    break;
                //2: DeepL (Mozhi)
                case 2:
                    isMozhiUsed = true;
                    sourceLanguage = "en";
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "deepl";
                    break;
                //3: DuckDuckGo / Bing (Mozhi)
                case 3:
                    isMozhiUsed = true;
                    if (targetLanguage == "zh-CN") targetLanguage = "zh";
                    mozhiEngine = "duckduckgo";
                    break;
                //4: MyMemory (Mozhi)
                case 4:
                    isMozhiUsed = true;
                    sourceLanguage = "Autodetect";
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "mymemory";
                    break;
                //5: Yandex (Mozhi)
                case 5:
                    isMozhiUsed = true;
                    if (targetLanguage.Contains("zh") || targetLanguage == "yue") targetLanguage = "zh";
                    mozhiEngine = "yandex";
                    break;
                //6: Naver Papago (Non-Mozhi)
                case 6:
                    if (targetLanguage == "yue") targetLanguage = "zh-TW";
<<<<<<< HEAD
                    JObject responseObj = NTranslate(orgText, Translator.DefaultDesiredLanguage);
=======
<<<<<<< HEAD
                    JObject responseObj = NTranslate(orgText, Translator.DefaultDesiredLanguage);
=======
                    JObject responseObj = NTranslate(orgText.Replace("\\n", "\r\n"), Translator.DefaultDesiredLanguage);
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
                    orgLanguage = responseObj.SelectToken("message.result.srcLangType").ToString();
                    break;
                    //7: iFlyTek (Non-Mozhi)
            }
            if (isMozhiUsed)
            {
<<<<<<< HEAD
                orgLanguage = MTranslate(orgText, mozhiEngine, sourceLanguage, targetLanguage).SelectToken("detected").ToString();
=======
<<<<<<< HEAD
                orgLanguage = MTranslate(orgText, mozhiEngine, sourceLanguage, targetLanguage).SelectToken("detected").ToString();
=======
                orgLanguage = MTranslate(orgText.Replace("\\n", "\r\n"), mozhiEngine, sourceLanguage, targetLanguage).SelectToken("detected").ToString();
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
            }
            return orgLanguage;
        }

        public static string GetConvertedCurrency(int pointValue, string sourceLanguage)
        {
            if (DefaultDesiredCurrency == "none")
            {
                return null;
            }
            UpdateExchangeTable();
            if (String.IsNullOrEmpty(ExchangeTable))
            {
                return null;
            }
            double irlPrice;
            string sourceCurrency;
            switch (sourceLanguage)
            {
                case "zh-CN":
<<<<<<< HEAD
                    irlPrice = pointValue / 100.00; break; // CMSでは100ポイントあたり1元
                case "en":
                    irlPrice = pointValue / 1000.00; break; // GMSでは1000ポイントあたり1ドル
=======
<<<<<<< HEAD
                    irlPrice = pointValue / 100.00; break; // CMSでは100ポイントあたり1元
                case "en":
                    irlPrice = pointValue / 1000.00; break; // GMSでは1000ポイントあたり1ドル
=======
                    irlPrice = pointValue / 100.00 * 0.98; break; // CMS�Ǥ�100�ݥ���Ȥ�����0.98Ԫ
                case "en":
                    irlPrice = pointValue / 1000.00; break; // GMS�Ǥ�1000�ݥ���Ȥ�����1�ɥ�
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
                default:
                    irlPrice = pointValue; break;
            }
            JObject exTable = JObject.Parse(ExchangeTable);
            if (DefaultDetectCurrency == "auto")
            {
                sourceCurrency = dictL2C[sourceLanguage];
            }
            else
            {
                sourceCurrency = DefaultDetectCurrency;
            }
            double exchangeMultipler = 1;
            double.TryParse(exTable.SelectToken(DefaultDesiredCurrency + "." + sourceCurrency).ToString(), out exchangeMultipler);
            double convertedPrice = irlPrice / exchangeMultipler;
<<<<<<< HEAD
            return "約" + convertedPrice.ToString("0.##") + dictCurrencyName[DefaultDesiredCurrency];
=======
<<<<<<< HEAD
            return "約" + convertedPrice.ToString("0.##") + dictCurrencyName[DefaultDesiredCurrency];
=======
            return "�s" + convertedPrice.ToString("0.##") + dictCurrencyName[DefaultDesiredCurrency];
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        }

        public static string ConvertCurrencyToLang(string currency)
        {
            try
            {
                return dictC2L[currency];
            }
            catch
            {
                return null;
            }
        }

        public static bool IsDesiredLanguage(string orgText)
        {
            if (string.IsNullOrEmpty(orgText)) return true;
            JArray response = GTranslate(orgText, Translator.DefaultDesiredLanguage);
            return (response[0][1].ToString() == DefaultDesiredLanguage);
        }

        public static void UpdateExchangeTable()
        {
            if (String.IsNullOrEmpty(ExchangeTable))
            {
                foreach (string bURL in CurrencyBaseURL)
                {
                    string fetchURL = bURL + DefaultDesiredCurrency + ".min.json";
                    var request = (HttpWebRequest)WebRequest.Create(fetchURL);
                    request.Accept = "application/json";
                    try
                    {
                        var response = (HttpWebResponse)request.GetResponse();
                        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        ExchangeTable = responseString;
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        #region Global Settings
        public static string ExchangeTable { get; set; }
        public static string DefaultDesiredLanguage { get; set; }
        public static string DefaultMozhiBackend { get; set; }
<<<<<<< HEAD
        public static string DefaultLanguageModel { get; set; }
=======
<<<<<<< HEAD
        public static string DefaultLanguageModel { get; set; }
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        public static string DefaultTranslateAPIKey { get; set; }
        public static int DefaultPreferredLayout { get; set; }
        public static int DefaultPreferredTranslateEngine { get; set; }
        public static bool IsTranslateEnabled { get; set; }
        public static string DefaultDetectCurrency { get; set; }
        public static string DefaultDesiredCurrency { get; set; }
<<<<<<< HEAD
        public static double DefaultLMTemperature { get; set; }
        public static int DefaultMaximumToken { get; set; }
        public static bool IsExtraParamEnabled { get; set; }
=======
<<<<<<< HEAD
        public static double DefaultLMTemperature { get; set; }
        public static int DefaultMaximumToken { get; set; }
        public static bool IsExtraParamEnabled { get; set; }
=======
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
>>>>>>> a85b27c1e063b5817109d5f7fd2c91dbb8ed93b4
        #endregion
    }

}