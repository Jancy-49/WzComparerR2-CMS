﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;
using WzComparerR2.Common;
using WzComparerR2.Config;
using WzComparerR2.PluginBase;
using WzComparerR2.Network.Contracts;
using System.Security.Cryptography;
using DevComponents.DotNetBar;
using WzComparerR2.CharaSim;
using System.Net;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.IO;


namespace WzComparerR2.Network
{
    public class Entry : PluginEntry
    {
        static Entry()
        {
            DefaultServer = Encoding.UTF8.GetString(Convert.FromBase64String("d2Mua2FnYW1pYS5jb20="));
        }

        public static readonly string DefaultServer;

        public Entry(PluginContext context)
         : base(context)
        {
            this.handlers = new Dictionary<Type, Action<object>>();
            this.RegisterAllHandlers();
        }

        public WcClient Client { get; private set; }

        private Dictionary<Type, Action<object>> handlers;
        private Session session;
        private LoggerForm.LogPrinter logger;
        private string AIBaseURL = "https://api.openai.com/v1";
        private string selectedLM = "gpt-4o-mini";
        private string systemMessage = "";
        private string APIKeyJSON = "";
        private double LMTemperature = 0.2;
        private int MaximumToken = -1;
        private bool ExtraParamEnabled = false;
        private bool AIChatEnabled = false;

        private JObject AIChatJson = new JObject();

        protected override void OnLoad()
        {
            WzComparerR2.Config.ConfigManager.RegisterAllSection();
            CheckConfig();
            var config = NetworkConfig.Default;

            var form1 = new LoggerForm();
            var dockSite = this.Context.DotNetBarManager.BottomDockSite;
            form1.AttachDockBar(dockSite);
            form1.OnCommand += Form1_OnCommand;

            this.logger = form1.GetLogger();
            logger.Level = config.LogLevel;
            Log.Loggers.Add(logger);

            //TODO: use config file, multi server selection.
            this.Client = new WcClient();
            this.Client.Host = DefaultServer;
            this.Client.Port = 2100;
            this.Client.AutoReconnect = true;
            this.Client.Connected += Client_Connected;
            this.Client.Disconnected += Client_Disconnected;
            this.Client.OnPackReceived += Client_OnPackReceived;
            var task = this.Client.Connect();

            if (!string.IsNullOrEmpty(Translator.OAITranslateBaseURL)) AIBaseURL = Translator.OAITranslateBaseURL;
            if (!string.IsNullOrEmpty(Translator.DefaultLanguageModel)) selectedLM = Translator.DefaultLanguageModel;
            if (!string.IsNullOrEmpty(Translator.DefaultTranslateAPIKey)) APIKeyJSON = Translator.DefaultTranslateAPIKey;

            AIChatJson = InitiateChatCompletion(selectedLM, false);
        }

        private void CheckConfig()
        {
            var config = NetworkConfig.Default;

            Guid guid;
            bool needSave = false;
            if (!Guid.TryParse(config.WcID, out guid))
            {
                guid = Guid.NewGuid();
                needSave = true;
            }

            string nickName = config.NickName;
            if (string.IsNullOrWhiteSpace(nickName))
            {
                nickName = "No Name #" + new Random().Next(10000);
                needSave = true;
            }

            string servers = config.Servers;
            if (string.IsNullOrEmpty(servers))
            {
                servers = ":2100;:2101;:2102;:2103;:2104";
                needSave = true;
            }

            if (needSave)
            {
                ConfigManager.Reload();
                config = NetworkConfig.Default;
                config.WcID = guid.ToString();
                config.NickName = nickName;
                config.Servers = servers;
                ConfigManager.Save();
            }
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            this.session = new Session();
            //开始加密
            this.CryptoRequest();
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            this.session = null;
        }

        private void Client_OnPackReceived(object sender, PackEventArgs e)
        {
            var type = e.Pack.GetType();
            Action<object> handler;
            if (this.handlers.TryGetValue(type, out handler))
            {
                handler?.Invoke(e.Pack);
            }
        }

        private async void Form1_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.Command.StartsWith("/"))
            {
                string[] args = e.Command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (args[0].ToLower())
                {
                    case "/users":
                        var sb = new StringBuilder();
                        lock (this.session.Users)
                        {
                            sb.AppendFormat("Users Online: {0}", this.session.Users.Count);
                            var time = DateTime.UtcNow;
                            foreach (var user in this.session.Users)
                            {
                                var loginTime = time - this.session.LocalTimeOffset - user.LoginTimeUTC;
                                sb.AppendLine().AppendFormat("  {0}, online for {1} minutes.", user.NickName, (int)loginTime.TotalMinutes);
                            }
                        }
                        Log.Info(sb.ToString());
                        break;

                    case "/name":
                        if (Client.IsConnected)
                        {
                            string newName = e.Command.Substring(5).Trim();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                ConfigManager.Reload();
                                NetworkConfig.Default.NickName = newName;
                                ConfigManager.Save();
                                var req = new PackUserProfileUpdateReq()
                                {
                                    NickName = newName
                                };
                                Client.Send(req);
                            }
                        }
                        break;

                    case "/aichat":
                        var sbAi = new StringBuilder();
                        string aiExtraParam = e.Command.Substring(7).Trim();
                        if (aiExtraParam == "on")
                        {
                            AIChatEnabled = true;
                            sbAi.Append("AI ​​chat feature is enabled. You will not be able to chat with other users until you disable it.");
                        }
                        else if (aiExtraParam == "off")
                        {
                            AIChatEnabled = false;
                            sbAi.Append("AI chat feature is disabled. You can now chat with other users.");
                        }
                        else if (AIChatEnabled)
                        {
                            sbAi.Append("AI chat function is enabled.");
                        }
                        else
                        {
                            sbAi.Append("AI chat function is disabled.");
                        }
                        Log.Info(sbAi.ToString());
                        break;

                    case "/new":
                        var sbNewMsg = new StringBuilder();
                        if (AIChatEnabled)
                        {
                            AIChatJson = InitiateChatCompletion(selectedLM, false);
                            sbNewMsg.AppendFormat("AI chat has been initialized. Previous chats will not be sent to the AI.");
                            Log.Info(sbNewMsg.ToString());
                        }
                        break;

                    case "/sysmsg":
                        var sbSysMsg = new StringBuilder();
                        systemMessage = e.Command.Substring(7).Trim();
                        if (!string.IsNullOrEmpty(systemMessage))
                        {
                            AIChatJson = InitiateChatCompletion(selectedLM, false);
                            ((JArray)AIChatJson["messages"]).Add(new JObject(
                                new JProperty("role", "system"),
                                new JProperty("content", systemMessage)
                            ));
                            sbSysMsg.AppendFormat("Current system message to AI is \"{0}\". AI chat has been initialized.", systemMessage);
                            Log.Info(sbSysMsg.ToString());
                        }
                        else
                        {
                            AIChatJson = InitiateChatCompletion(selectedLM, false);
                            sbSysMsg.AppendFormat("Any current system messages to the AI ​​will be cleared. AI chat has been initialized.");
                            Log.Info(sbSysMsg.ToString());
                        }
                        break;
                    case "/help":
                        var sbHelp = new StringBuilder();
                        sbHelp.AppendFormat("Guidance to use the Network Logger command\r\n");
                        sbHelp.AppendFormat("/users : Lists online users.\r\n");
                        sbHelp.AppendFormat("/name [name]: Changes the username to the specified name.\r\n");
                        sbHelp.AppendFormat("/aichat [on|off] : Toggle AI chat function.\r\n");
                        sbHelp.AppendFormat("/new : Reinitialize AI chat.\r\n");
                        sbHelp.AppendFormat("/sysmsg [message] : Specifies the system message to the AI ​​chat.\r\n");
                        sbHelp.AppendFormat("/help : Show this help.");
                        Log.Info(sbHelp.ToString());
                        break;
                }
            }
            else
            {
                if (AIChatEnabled)
                {
                    Log.Warn(e.Command);
                    Log.Info("Waiting for AI response...");
                    await Task.Run(() => ChatToAI(e.Command));
                    return;
                }
                if (Client.IsConnected)
                {
                    var pack = new PackSendChat()
                    {
                        Group = ChatGroup.Public,
                        Message = e.Command
                    };
                    Client.Send(pack);
                }
                else
                {
                    Log.Warn("Command failed: Not connected to the server.");
                }
            }
        }

        private async void ChatToAI(string message)
        {
            var request = (HttpWebRequest)WebRequest.Create(AIBaseURL + "/chat/completions");
            request.Method = "POST";
            request.ContentType = "application/json";
            if (!string.IsNullOrEmpty(APIKeyJSON))
            {
                JObject reqHeaders = JObject.Parse(APIKeyJSON);
                foreach (var property in reqHeaders.Properties()) request.Headers.Add(property.Name, property.Value.ToString());
            }
            ((JArray)AIChatJson["messages"]).Add(new JObject(
                new JProperty("role", "user"),
                new JProperty("content", message)
            ));

            var postData = AIChatJson;

            if (ExtraParamEnabled)
            {
                postData.Add(new JProperty("temperature", LMTemperature));
                postData.Add(new JProperty("max_tokens", MaximumToken));
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
                ((JArray)AIChatJson["messages"]).Add(new JObject(
                    new JProperty("role", "assistant"),
                    new JProperty("content", responseResult)
                ));
                Log.Info(responseResult);
            }
            catch
            {
                Log.Warn("Failed to chat with AI.");
            }
        }

        private JObject InitiateChatCompletion(string languageModel, bool isStreamEnabled)
        {
            return new JObject(
                new JProperty("model", languageModel),
                new JProperty("messages", new JArray()),
                new JProperty("stream", isStreamEnabled)
            );
        }

        private void RegisterAllHandlers()
        {
            var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "OnPackReceived" && m.ReturnParameter.ParameterType == typeof(void));
            foreach (var method in methods)
            {
                var p = method.GetParameters();
                if (p.Length == 1)
                {
                    var type = p[0].ParameterType;
                    var funcType = typeof(Action<>).MakeGenericType(type);
                    var handler = method.CreateDelegate(funcType, this);
                    RegisterHandler(type, o => handler.DynamicInvoke(o));
                }
            }
        }

        private void RegisterHandler<T>(Action<T> handler)
        {
            RegisterHandler(typeof(T), obj =>
            {
                if (obj is T)
                {
                    handler((T)obj);
                }
            });
        }

        private void RegisterHandler(Type packType, Action<object> handler)
        {
            this.handlers[packType] = handler;
        }

        #region PackHandlers
        private void CryptoRequest()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            this.session.RSA = rsa;

            var rsaParams = rsa.ExportParameters(false);
            var req = new PackCryptReq()
            {
                Exponent = rsaParams.Exponent,
                Modulus = rsaParams.Modulus
            };
            this.Client.Send(req);
        }

        private void ServerInfoRequest()
        {
            var req = new PackGetServerInfoReq();
            this.Client.Send(req);
        }

        private void UserListRequest()
        {
            var req = new PackGetAllUsersReq();
            this.Client.Send(req);
        }

        private void LoginRequest()
        {
            CheckConfig();
            var config = NetworkConfig.Default;

            var req = new PackLoginReq()
            {
                WcID = config.WcID,
                NickName = config.NickName
            };
            this.Client.Send(req);
        }

        private void OnPackReceived(PackHeartBeat pack)
        {
            Client.Send(pack);
        }

        private void OnPackReceived(PackCryptResp pack)
        {
            var rc4S2C = RC4.Create();
            rc4S2C.Key = this.session.RSA.Decrypt(pack.KeyEncryptedS2C, false);
            var rc4C2S = RC4.Create();
            rc4C2S.Key = this.session.RSA.Decrypt(pack.KeyEncryptedC2S, false);
            this.Client.BeginCrypto(rc4S2C.CreateDecryptor(), rc4C2S.CreateEncryptor());
            this.session.RSA.Dispose();
            this.session.RSA = null;

            //获取服务器状态
            ServerInfoRequest();
            //开始登录协议
            LoginRequest();
        }

        private void OnPackReceived(PackGetServerInfoResp pack)
        {
            this.session.LocalTimeOffset = DateTime.UtcNow - pack.CurrentTimeUTC;

            Log.Info("Server version: {0} - Time: {1:yyyy-MM-dd HH:mm:ss}, {2:%d\\d\\ h\\h\\ m\\m\\ s\\s} elapsed - {3} user(s) online.",
                pack.Version,
                pack.CurrentTimeUTC.ToLocalTime(),
                pack.CurrentTimeUTC - pack.StartTimeUTC,
                pack.UserCount);
        }

        private void OnPackReceived(PackLoginResp pack)
        {
            Log.Info("You have successfully logged in.");
            this.session.SID = pack.SessionID;

            //获取在线列表
            UserListRequest();
        }

        private void OnPackReceived(PackOnChat pack)
        {
            //聊天到达
            string nickName;
            lock (this.session.Users)
            {
                nickName = this.session.Users.FirstOrDefault(u => u.UID == pack.FromID).NickName ?? pack.FromID;
            }
            Log.Write(LogLevel.None, "[{0}] {1}", nickName, pack.Message);
            if (!this.Context.MainForm.ContainsFocus)
            {
                NativeMethods.FlashWindowEx(this.Context.MainForm);
            }
        }

        private void OnPackReceived(PackGetAllUsersResp pack)
        {
            Log.Info("{0} user(s) online.", pack.Users.Count);
            lock (this.session.Users)
            {
                this.session.Users.Clear();
                this.session.Users.AddRange(pack.Users);
            }
        }

        /// <summary>
        /// 服务器公告或错误。
        /// </summary>
        private void OnPackReceived(PackOnServerMessage pack)
        {
            switch (pack.Type)
            {
                case MessageType.Normal:
                    Log.Info("(Notice) {0}", pack.Message);
                    break;

                case MessageType.Error:
                    Log.Error("(ServerError) {0}", pack.Message);
                    break;
            }
        }

        /// <summary>
        /// 用户列表更新。
        /// </summary>
        /// <param name="pack"></param>
        private void OnPackReceived(PackOnUserUpdate pack)
        {
            if (AIChatEnabled) return;
            lock (this.session.Users)
            {
                var idx = this.session.Users.FindIndex(u => u.UID == pack.UserInfo.UID && u.SID == pack.UserInfo.SID);

                switch (pack.UpdateReason)
                {
                    case UserUpdateReason.Online:
                        this.session.Users.Add(pack.UserInfo);
                        Log.Info("[{0}] is online.", pack.UserInfo.NickName);
                        break;

                    case UserUpdateReason.Offline:
                        if (idx > -1)
                        {
                            var oldUser = this.session.Users[idx];
                            this.session.Users.RemoveAt(idx);
                            Log.Info("[{0}] is offline.", oldUser.NickName);
                        }
                        break;

                    case UserUpdateReason.InfoChanged:
                        if (idx > -1)
                        {
                            var oldUser = this.session.Users[idx];
                            this.session.Users[idx] = pack.UserInfo;
                            Log.Info("[{0}] changed its name to [{1}].", oldUser.NickName, pack.UserInfo.NickName);
                        }
                        else
                        {
                            this.session.Users.Add(pack.UserInfo);
                            Log.Info("[{0}] is online.", pack.UserInfo.NickName);
                        }

                        break;
                }
            }

        }
        #endregion

        class Session
        {
            public RSACryptoServiceProvider RSA;
            public string SID;
            public TimeSpan LocalTimeOffset;
            public List<UserInfo> Users = new List<UserInfo>();
        }
    }
}