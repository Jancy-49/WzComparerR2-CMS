using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using WzComparerR2.WzLib;
using WzComparerR2.Common;
using WzComparerR2.PluginBase;
using WzComparerR2.CharaSimControl;
using WzComparerR2.CharaSim;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Imaging;
using SharpDX.Direct3D11;
using WzComparerR2.Config;

namespace WzComparerR2.Comparer
{
    public class EasyComparer
    {
        public EasyComparer()
        {
            this.Comparer = new WzFileComparer();
        }
        private Wz_Node wzNew { get; set; }
        private Wz_Node wzOld { get; set; }
        private Wz_File stringWzNew { get; set; }
        private Wz_File itemWzNew { get; set; }
        private Wz_File etcWzNew { get; set; }
        private Wz_File questWzNew { get; set; }
        private Wz_File stringWzOld { get; set; }
        private Wz_File itemWzOld { get; set; }
        private Wz_File etcWzOld { get; set; }
        private Wz_File questWzOld { get; set; }
        private Wz_Node[] WzNewOld { get; set; } = new Wz_Node[2];
        private Wz_File[] WzFileNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] StringWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] ItemWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] EtcWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] QuestWzNewOld { get; set; } = new Wz_File[2];
        private HashSet<string> OutputSkillTooltipIDs { get; set; } = new HashSet<string>();
        private HashSet<string> PerJobSkillTooltipInfo { get; set; } = new HashSet<string>();
        private List<string> skillTooltipInfo = new List<string>();
        private List<string> itemTooltipInfo = new List<string>();
        private List<string> eqpTooltipInfo = new List<string>();
        private List<string> mobTooltipInfo = new List<string>();
        private List<string> npcTooltipInfo = new List<string>();
        private List<string> mapTooltipInfo = new List<string>();
        private List<string> cashTooltipInfo = new List<string>();
        private List<string> questTooltipInfo = new List<string>();
        private List<string> achievementTooltipInfo = new List<string>();
        private Dictionary<string, Dictionary<string, List<string>>> diffHtml = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> diffPerJobSkillTags { get; set; } = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffSkillTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffItemTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffEqpTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffMobTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffNpcTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffCashTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffMapTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffAchvTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<int>> KMSContentID = new Dictionary<string, List<int>>();
        private Dictionary<string, List<string>> KMSComponentDict = new Dictionary<string, List<string>>();
        private Dictionary<int, List<int>> FifthJobSkillToJobID = new Dictionary<int, List<int>>();
        public Dictionary<string, string> FailToExportNodes = new Dictionary<string, string>();
        public Dictionary<string, string> FailToExportTooltips { get; private set; } = new Dictionary<string, string>();
        private Dictionary<string, HashSet<int>> ChangedActions { get; set; } = new Dictionary<string, HashSet<int>>();
        private SortedSet<int> OutputMapTooltipIDs { get; set; } = new SortedSet<int>();

        public WzFileComparer Comparer { get; protected set; }
        private string stateInfo;
        private string stateDetail;
        public bool OutputPng { get; set; }
        public bool OutputAddedImg { get; set; }
        public bool OutputRemovedImg { get; set; }
        public bool EnableDarkMode { get; set; }
        public bool saveSkillTooltip { get; set; }
        public bool saveItemTooltip { get; set; }
        public bool saveCashTooltip { get; set; }
        public bool saveEqpTooltip { get; set; }
        public bool saveMobTooltip { get; set; }
        public bool saveNpcTooltip { get; set; }
        public bool saveQuestTooltip { get; set; }
        public bool saveAchievementTooltip { get; set; }
        public bool saveMapTooltip { get; set; }
        public bool HashPngFileName { get; set; }
        public bool Enable22AniStyle { get; set; }
        public bool ShowObjectID { get; set; }
        public bool ShowChangeType { get; set; }
        public bool ShowLinkedTamingMob { get; set; }
        public bool SkipKMSContent { get; set; }
        public bool DownloadKMSContentDB { get; set; }
        public bool SkipGodChangseopDuplicatedNodes { get; set; }
        public bool EnableAssembleTooltip { get; set; }
        public bool AllowFamiliarOutOfBounds { get; set; }
        public bool UseCTFamiliarUI { get; set; }
        public bool EnableWorldArchive { get; set; }
        public bool EnableMonsterBook { get; set; }
        public bool ShowNpcQuotes { get; set; }
        public bool LocatePetEquip { get; set; }
        public int QuestState { get; set; }

        public string StateInfo
        {
            get { return stateInfo; }
            set
            {
                stateInfo = value;
                this.OnStateInfoChanged(EventArgs.Empty);
            }
        }

        public string StateDetail
        {
            get { return stateDetail; }
            set
            {
                stateDetail = value;
                this.OnStateDetailChanged(EventArgs.Empty);
            }
        }

        public event EventHandler StateInfoChanged;
        public event EventHandler StateDetailChanged;
        public event EventHandler<Patcher.PatchingEventArgs> PatchingStateChanged;

        protected virtual void OnStateInfoChanged(EventArgs e)
        {
            if (this.StateInfoChanged != null)
                this.StateInfoChanged(this, e);
        }

        protected virtual void OnStateDetailChanged(EventArgs e)
        {
            if (this.StateDetailChanged != null)
                this.StateDetailChanged(this, e);
        }

        protected virtual void OnPatchingStateChanged(Patcher.PatchingEventArgs e)
        {
            if (this.PatchingStateChanged != null)
                this.PatchingStateChanged(this, e);
        }

        public void EasyCompareWzFiles(Wz_File fileNew, Wz_File fileOld, string outputDir, StreamWriter index = null)
        {
            StateInfo = "正在对比Wz...";

            if ((fileNew.Type == Wz_Type.Base || fileOld.Type == Wz_Type.Base) && index == null) //至少有一个base 拆分对比
            {
                var virtualNodeNew = RebuildWzFile(fileNew);
                var virtualNodeOld = RebuildWzFile(fileOld);
                WzFileComparer comparer = new WzFileComparer();
                comparer.IgnoreWzFile = true;

                if (saveCashTooltip || saveEqpTooltip || saveItemTooltip || saveMapTooltip || saveMobTooltip || saveNpcTooltip || saveSkillTooltip || saveQuestTooltip || saveAchievementTooltip || SkipKMSContent)
                {
                    this.WzNewOld[0] = fileNew.Node;
                    this.WzNewOld[1] = fileOld.Node;
                    this.WzFileNewOld[0] = fileNew.Node.GetNodeWzFile();
                    this.WzFileNewOld[1] = fileOld.Node.GetNodeWzFile();

                    StateInfo = "正在初始化5转技能应用职业代码...";
                    for (int i = 0; i < 2; i++)
                    {
                        Wz_Node vCoreData = PluginManager.FindWz("Etc\\VcoreNew.img\\vSkill\\CoreData", WzFileNewOld[i]);
                        if (vCoreData == null || vCoreData.FullPath == "Base.wz") vCoreData = PluginManager.FindWz("Etc\\VCore.img\\CoreData", WzFileNewOld[i]);
                        if (vCoreData == null || vCoreData.FullPath == "Base.wz") break;

                        foreach (Wz_Node data in vCoreData.Nodes)
                        {
                            Wz_Node connectSkill = data.FindNodeByPath("connectSkill").ResolveUol();
                            Wz_Node jobIDValue = data.FindNodeByPath("job").ResolveUol();
                            List<int> applicableJobID = new List<int>();
                            foreach (Wz_Node jobID in jobIDValue.Nodes)
                            {
                                applicableJobID.Add(jobID.GetValueEx<int>(0));
                            }
                            if (connectSkill == null)
                            {
                                int skillIDValue = data.FindNodeByPath("spCoreOption\\effect\\skill_id").ResolveUol().GetValueEx<int>(0);
                                if (!FifthJobSkillToJobID.ContainsKey(skillIDValue)) FifthJobSkillToJobID.Add(skillIDValue, [0]);
                            }
                            else
                            {
                                foreach (Wz_Node skillID in connectSkill.Nodes)
                                {
                                    int skillIDValue = skillID.GetValueEx<int>(0);
                                    if (skillIDValue > 0 && !FifthJobSkillToJobID.ContainsKey(skillIDValue))
                                    {
                                        FifthJobSkillToJobID.Add(skillIDValue, applicableJobID);
                                    }
                                }
                            }
                        }
                    }

                    if (SkipKMSContent)
                    {
                        KMSContentID["Skill"] = new List<int>();
                        if (DownloadKMSContentDB)
                        {
                            foreach (string item in new string[] { "Item", "Map", "Mob", "Npc", "Skill", "Achievement" })
                            {
                                StateInfo = string.Format("正在导出{0}对应的KMS数据...", item);
                                var request = (HttpWebRequest)WebRequest.Create(string.Format("https://raw.githubusercontent.com/HikariCalyx/KMSContent/refs/heads/main/{0}ID.txt", item));
                                request.Method = "GET";
                                request.UserAgent = "WzComparerR2-JMS/1.0";
                                request.Timeout = 15000;
                                try
                                {
                                    var response = (HttpWebResponse)request.GetResponse();
                                    var responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                                    foreach (string line in responseString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (line.StartsWith("#")) continue;
                                        string[] parts = line.Split(new[] { ' ' }, 2);
                                        if (parts.Length > 1) continue;
                                        string id = parts[0];
                                        if (int.TryParse(id, out int parsedID))
                                        {
                                            if (!KMSContentID.ContainsKey(item))
                                            {
                                                KMSContentID[item] = new List<int>();
                                            }
                                            if (!KMSContentID[item].Contains(parsedID))
                                            {
                                                KMSContentID[item].Add(parsedID);
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    if (!KMSContentID.ContainsKey(item))
                                    {
                                        KMSContentID[item] = new List<int>();
                                    }
                                }
                            }
                            foreach (string item in new string[] { "Effect", "MapBack", "MapObj", "MapTile", "MapWorldMap", "MobBossPattern" })
                            {
                                StateInfo = string.Format("正在导出{0}对应的KMS数据...", item);
                                var request = (HttpWebRequest)WebRequest.Create(string.Format("https://raw.githubusercontent.com/HikariCalyx/KMSContent/refs/heads/main/{0}ImgList.txt", item));
                                request.Method = "GET";
                                request.UserAgent = "WzComparerR2-JMS/1.0";
                                request.Timeout = 15000;
                                try
                                {
                                    var response = (HttpWebResponse)request.GetResponse();
                                    var responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                                    foreach (string line in responseString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        if (line.StartsWith("#")) continue;
                                        string[] parts = line.Split(new[] { ' ' }, 2);
                                        if (parts.Length > 1) continue;
                                        string img = parts[0];
                                        if (!KMSComponentDict.ContainsKey(item))
                                        {
                                            KMSComponentDict[item] = new List<string>();
                                        }
                                        if (!KMSComponentDict[item].Contains(img))
                                        {
                                            KMSComponentDict[item].Add(img);
                                        }
                                    }
                                }
                                catch
                                {
                                    if (!KMSComponentDict.ContainsKey(item))
                                    {
                                        KMSComponentDict[item] = new List<string>();
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (string item in new string[] { "Item", "Map", "Mob", "Npc", "Skill", "Achievement" })
                            {
                                if (!KMSContentID.ContainsKey(item))
                                {
                                    KMSContentID[item] = new List<int>();
                                }
                            }
                            foreach (string item in new string[] { "Effect", "MapBack", "MapObj", "MapTile", "MapWorldMap" })
                            {
                                if (!KMSComponentDict.ContainsKey(item))
                                {
                                    KMSComponentDict[item] = new List<string>();

                                }
                            }
                        }
                    }
                }
                if (saveItemTooltip || saveEqpTooltip) // Check commodity differences
                {
                    StateInfo = "正在整理现金道具";
                    CharaSimLoader.ClearAll();
                    CharaSimLoader.LoadSetItemsIfEmpty(fileNew);
                    CharaSimLoader.LoadAstraSubWeaponsIfEmpty(fileNew);
                    CharaSimLoader.LoadExclusiveEquipsIfEmpty(fileNew);
                    CharaSimLoader.LoadMsnMintableItemListIfEmpty(fileNew);
                    if (this.LocatePetEquip) CharaSimLoader.LoadPetEquipInfoIfEmpty(fileNew);
                    CharaSimLoader.LoadCommodities(fileOld, slotIdx: 1);
                    CharaSimLoader.LoadCommodities(fileNew, slotIdx: 0);
                    CompareCommodities();
                    StateInfo = "现金道具整理完毕";
                }

                this.wzNew = fileNew.Node;
                this.wzOld = fileOld.Node;

                var dictNew = SplitVirtualNode(virtualNodeNew);
                var dictOld = SplitVirtualNode(virtualNodeOld);

                //寻找共同wzType
                var wzTypeList = dictNew.Select(kv => kv.Key)
                    .Where(wzType => dictOld.ContainsKey(wzType));

                CreateStyleSheet(outputDir);

                string htmlFilePath = Path.Combine(outputDir, "index.html");

                FileStream htmlFile = null;
                StreamWriter sw = null;
                StateInfo = "Index档案制作中...";
                StateDetail = "档案构成生成中";
                try
                {
                    htmlFile = new FileStream(htmlFilePath, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(htmlFile, Encoding.UTF8);
                    sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                    sw.WriteLine("<html>");
                    sw.WriteLine("<head>");
                    sw.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\">");
                    sw.WriteLine("<title>Index {0}←{1}</title>", fileNew.Header.WzVersion, fileOld.Header.WzVersion);
                    sw.WriteLine("<link type=\"text/css\" rel=\"stylesheet\" href=\"style.css\" />");
                    sw.WriteLine("</head>");
                    sw.WriteLine("<body>");
                    //输出概况
                    sw.WriteLine("<p class=\"wzf\">");
                    sw.WriteLine("<table>");
                    sw.WriteLine("<tr><th>文件名</th><th>新版本大小</th><th>旧版本大小</th><th>变更</th><th>新增</th><th>删除</th></tr>");
                    foreach (var wzType in wzTypeList)
                    {
                        var vNodeNew = dictNew[wzType];
                        var vNodeOld = dictOld[wzType];
                        var cmp = comparer.Compare(vNodeNew, vNodeOld);
                        OutputFile(vNodeNew.LinkNodes.Select(node => node.Value).OfType<Wz_File>().ToList(),
                            vNodeOld.LinkNodes.Select(node => node.Value).OfType<Wz_File>().ToList(),
                            wzType,
                            cmp.ToList(),
                            outputDir,
                            sw);
                    }
                    sw.WriteLine("</table>");
                    sw.WriteLine("</p>");

                    //html结束
                    sw.WriteLine("</body>");
                    sw.WriteLine("</html>");
                }
                finally
                {
                    try
                    {
                        if (sw != null)
                        {
                            sw.Flush();
                            sw.Close();
                        }
                        if (saveCashTooltip || saveEqpTooltip || saveItemTooltip || saveMapTooltip || saveMobTooltip || saveNpcTooltip || saveSkillTooltip || saveQuestTooltip || saveAchievementTooltip)
                        {
                            saveTooltipHtml(outputDir);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else //执行传统对比
            {
                WzFileComparer comparer = new WzFileComparer();
                comparer.IgnoreWzFile = false;
                var cmp = comparer.Compare(fileNew.Node, fileOld.Node);
                CreateStyleSheet(outputDir);
                OutputFile(fileNew, fileOld, fileNew.Type, cmp.ToList(), outputDir, index);
            }

            GC.Collect();
        }

        public void EasyCompareWzStructures(Wz_Structure structureNew, Wz_Structure structureOld, string outputDir, StreamWriter index)
        {
            var virtualNodeNew = RebuildWzStructure(structureNew);
            var virtualNodeOld = RebuildWzStructure(structureOld);
            WzFileComparer comparer = new WzFileComparer();
            comparer.IgnoreWzFile = true;

            var dictNew = SplitVirtualNode(virtualNodeNew);
            var dictOld = SplitVirtualNode(virtualNodeOld);

            //寻找共同wzType
            var wzTypeList = dictNew.Select(kv => kv.Key)
                .Where(wzType => dictOld.ContainsKey(wzType));

            CreateStyleSheet(outputDir);

            foreach (var wzType in wzTypeList)
            {
                var vNodeNew = dictNew[wzType];
                var vNodeOld = dictOld[wzType];
                var cmp = comparer.Compare(vNodeNew, vNodeOld);
                OutputFile(vNodeNew.LinkNodes.Select(node => node.Value).OfType<Wz_File>().ToList(),
                    vNodeOld.LinkNodes.Select(node => node.Value).OfType<Wz_File>().ToList(),
                    wzType,
                    cmp.ToList(),
                    outputDir,
                    index);
            }
        }

        public void EasyCompareWzStructuresToWzFiles(Wz_File fileNew, Wz_Structure structureOld, string outputDir, StreamWriter index)
        {
            var virtualNodeOld = RebuildWzStructure(structureOld);
            WzFileComparer comparer = new WzFileComparer();
            comparer.IgnoreWzFile = true;

            var dictOld = SplitVirtualNode(virtualNodeOld);

            //寻找共同wzType
            var wzTypeList = dictOld.Select(kv => kv.Key)
                .Where(wzType => dictOld.ContainsKey(wzType));

            CreateStyleSheet(outputDir);

            foreach (var wzType in wzTypeList)
            {
                var vNodeOld = dictOld[wzType];
                var cmp = comparer.Compare(fileNew.Node, vNodeOld);
                OutputFile(new List<Wz_File>() { fileNew },
                    vNodeOld.LinkNodes.Select(node => node.Value).OfType<Wz_File>().ToList(),
                    wzType,
                    cmp.ToList(),
                    outputDir,
                    index);
            }
        }

        private WzVirtualNode RebuildWzFile(Wz_File wzFile)
        {
            //分组
            List<Wz_File> subFiles = new List<Wz_File>();
            WzVirtualNode topNode = new WzVirtualNode(wzFile.Node);

            foreach (var childNode in wzFile.Node.Nodes)
            {
                var subFile = childNode.GetValue<Wz_File>();
                if (subFile != null && !subFile.IsSubDir) //wz子文件
                {
                    subFiles.Add(subFile);
                }
                else //其他
                {
                    topNode.AddChild(childNode, true);
                }
            }

            if (wzFile.Type == Wz_Type.Base)
            {
                foreach (var grp in subFiles.GroupBy(f => f.Type))
                {
                    WzVirtualNode fileNode = new WzVirtualNode();
                    fileNode.Name = grp.Key.ToString();
                    foreach (var file in grp)
                    {
                        fileNode.Combine(file.Node);
                    }
                    topNode.AddChild(fileNode);
                }
            }
            return topNode;
        }

        private WzVirtualNode RebuildWzStructure(Wz_Structure wzStructure)
        {
            //分组
            List<Wz_File> subFiles = wzStructure.wz_files.Where(wz_file => wz_file != null).ToList();
            WzVirtualNode topNode = new WzVirtualNode();

            foreach (var grp in subFiles.GroupBy(f => f.Type))
            {
                WzVirtualNode fileNode = new WzVirtualNode();
                fileNode.Name = grp.Key.ToString();
                foreach (var file in grp)
                {
                    fileNode.Combine(file.Node);
                }
                topNode.AddChild(fileNode);
            }
            return topNode;
        }

        private Dictionary<Wz_Type, WzVirtualNode> SplitVirtualNode(WzVirtualNode node)
        {
            var dict = new Dictionary<Wz_Type, WzVirtualNode>();
            Wz_File wzFile = null;
            if (node.LinkNodes.Count > 0)
            {
                wzFile = node.LinkNodes[0].Value as Wz_File;
                dict[wzFile.Type] = node;
            }

            if (wzFile?.Type == Wz_Type.Base || node.LinkNodes.Count == 0) //额外处理
            {
                var wzFileList = node.ChildNodes
                    .Select(child => new { Node = child, WzFile = child.LinkNodes[0].Value as Wz_File })
                    .Where(item => item.WzFile != null);

                foreach (var item in wzFileList)
                {
                    dict[item.WzFile.Type] = item.Node;
                }
            }

            return dict;
        }

        private IEnumerable<string> GetFileInfo(Wz_File wzf, Func<Wz_File, string> extractor)
        {
            IEnumerable<string> result = new[] { extractor.Invoke(wzf) }
                .Concat(wzf.MergedWzFiles.Select(extractor.Invoke));

            if (wzf.Type != Wz_Type.Base)
            {
                result = result.Concat(wzf.Node.Nodes.Where(n => n.Value is Wz_File).SelectMany(nwzf => GetFileInfo((Wz_File)nwzf.Value, extractor)));
            }

            return result;
        }

        private void OutputFile(Wz_File fileNew, Wz_File fileOld, Wz_Type type, List<CompareDifference> diffLst, string outputDir, StreamWriter index)
        {
            OutputFile(new List<Wz_File>() { fileNew },
                new List<Wz_File>() { fileOld },
                type,
                diffLst,
                outputDir,
                index);
        }
        private void OutputFile(List<Wz_File> fileNew, List<Wz_File> fileOld, Wz_Type type, List<CompareDifference> diffLst, string outputDir, StreamWriter index = null)
        {
            string htmlFilePath = Path.Combine(outputDir, type.ToString() + ".html");
            for (int i = 1; File.Exists(htmlFilePath); i++)
            {
                htmlFilePath = Path.Combine(outputDir, string.Format("{0}_{1}.html", type, i));
            }
            string srcDirPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(htmlFilePath) + "_files");
            if (OutputPng && !Directory.Exists(srcDirPath))
            {
                Directory.CreateDirectory(srcDirPath);
            }
            string skillTooltipPath = Path.Combine(outputDir, "SkillTooltip");
            string itemTooltipPath = Path.Combine(outputDir, "ItemTooltip");
            string eqpTooltipPath = Path.Combine(outputDir, "EqpTooltip");
            string mapTooltipPath = Path.Combine(outputDir, "MapTooltip");
            string mobTooltipPath = Path.Combine(outputDir, "MobTooltip");
            string npcTooltipPath = Path.Combine(outputDir, "NpcTooltip");
            string questTooltipPath = Path.Combine(outputDir, "QuestTooltip");
            string achvTooltipPath = Path.Combine(outputDir, "AchievementTooltip");

            FileStream htmlFile = null;
            StreamWriter sw = null;
            StateInfo = type + " 档案制作中...";
            StateDetail = "档案构成生成中";
            try
            {
                htmlFile = new FileStream(htmlFilePath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(htmlFile, Encoding.UTF8);
                sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\">");
                sw.WriteLine("<title>{0} {1}←{2}</title>", type, fileNew[0].GetMergedVersion(), fileOld[0].GetMergedVersion());
                sw.WriteLine("<link type=\"text/css\" rel=\"stylesheet\" href=\"style.css\" />");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                //输出概况
                sw.WriteLine("<p class=\"wzf\">");
                sw.WriteLine("<table>");
                sw.WriteLine("<tr><th>&nbsp;</th><th>文件名</th><th>大小</th><th>版本</th></tr>");
                sw.WriteLine("<tr><td>新版本</td><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                    string.Join("<br/>", fileNew.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileName))),
                    string.Join("<br/>", fileNew.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileSize.ToString("N0")))),
                    string.Join("<br/>", fileNew.Select(wzf => wzf.GetMergedVersion()))
                    );
                sw.WriteLine("<tr><td>旧版本</td><td>{0}</td><td>{1}</td><td>{2}</td></tr>",
                    string.Join("<br/>", fileOld.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileName))),
                    string.Join("<br/>", fileOld.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileSize.ToString("N0")))),
                    string.Join("<br/>", fileOld.Select(wzf => wzf.GetMergedVersion()))
                    );
                sw.WriteLine("<tr><td>当前时间</td><td colspan='3'>{0:yyyy-MM-dd HH:mm:ss.fff}</td></tr>", DateTime.Now);
                sw.WriteLine("<tr><td>选项</td><td colspan='3'>{0}</td></tr>", string.Join("<br/>", new[] {
                    this.OutputPng ? "-OutputPng" : null,
                    this.OutputAddedImg ? "-OutputAddedImg" : null,
                    this.OutputRemovedImg ? "-OutputRemovedImg" : null,
                    this.EnableDarkMode ? "-EnableDarkMode" : null,
                    "-PngComparison " + this.Comparer.PngComparison,
                    this.Comparer.ResolvePngLink ? "-ResolvePngLink" : null,
                    this.SkipKMSContent ? "-SkipKMSContent" : null,
                    this.SkipGodChangseopDuplicatedNodes ? "-SkipGodChangseopDuplicatedNodes" : null,
                }.Where(p => p != null)));
                sw.WriteLine("</table>");
                sw.WriteLine("</p>");

                //输出目录
                StringBuilder[] sb = { new StringBuilder(), new StringBuilder(), new StringBuilder() };
                int[] count = new int[6];
                List<CompareDifference> kmsContent = new List<CompareDifference> { };
                List<CompareDifference> godChangseopNode = new List<CompareDifference> { };
                string[] diffStr = { "变更", "新增", "删除" };
                foreach (CompareDifference diff in diffLst)
                {
                    int idx = -1;
                    string detail = null;
                    switch (diff.DifferenceType)
                    {
                        case DifferenceType.Changed:
                            idx = 0;
                            if (SkipKMSContent && (isKMSNode(diff.NodeNew) || isKMSNode(diff.NodeOld)))
                            {
                                kmsContent.Add(diff);
                                continue;
                            }
                            if (SkipGodChangseopDuplicatedNodes && (isGodChangseopNode(diff.NodeNew) || isGodChangseopNode(diff.NodeOld)))
                            {
                                godChangseopNode.Add(diff);
                                continue;
                            }
                            detail = string.Format("<a name=\"m_{1}_{2}\" href=\"#a_{1}_{2}\">{0}</a>", diff.NodeNew.FullPathToFile, idx, count[idx]);
                            break;
                        case DifferenceType.Append:
                            idx = 1;
                            if (SkipKMSContent && isKMSNode(diff.NodeNew))
                            {
                                kmsContent.Add(diff);
                                continue;
                            }
                            if (SkipGodChangseopDuplicatedNodes && isGodChangseopNode(diff.NodeNew))
                            {
                                godChangseopNode.Add(diff);
                                continue;
                            }
                            if (this.OutputAddedImg)
                            {
                                detail = string.Format("<a name=\"m_{1}_{2}\" href=\"#a_{1}_{2}\">{0}</a>", diff.NodeNew.FullPathToFile, idx, count[idx]);
                            }
                            else
                            {
                                detail = diff.NodeNew.FullPathToFile;
                            }
                            break;
                        case DifferenceType.Remove:
                            idx = 2;
                            if (SkipKMSContent && isKMSNode(diff.NodeOld))
                            {
                                kmsContent.Add(diff);
                                continue;
                            }
                            if (SkipGodChangseopDuplicatedNodes && isGodChangseopNode(diff.NodeOld))
                            {
                                godChangseopNode.Add(diff);
                                continue;
                            }
                            if (this.OutputRemovedImg)
                            {
                                detail = string.Format("<a name=\"m_{1}_{2}\" href=\"#a_{1}_{2}\">{0}</a>", diff.NodeOld.FullPathToFile, idx, count[idx]);
                            }
                            else
                            {
                                detail = diff.NodeOld.FullPathToFile;
                            }
                            break;
                        default:
                            continue;
                    }
                    sb[idx].Append("<tr><td>");
                    sb[idx].Append(detail);
                    sb[idx].AppendLine("</td></tr>");
                    count[idx]++;
                }
                StateDetail = "目录处理中";
                Array.Copy(count, 0, count, 3, 3);
                for (int i = 0; i < sb.Length; i++)
                {
                    sw.WriteLine("<table class=\"lst{0}\">", i);
                    sw.WriteLine("<tr><th><a name=\"m_{0}\">{1}:{2}</a></th></tr>", i, diffStr[i], count[i]);
                    sw.Write(sb[i].ToString());
                    sw.WriteLine("</table>");
                    sb[i] = null;
                    count[i] = 0;
                }

                Patcher.PatchPartContext part = new Patcher.PatchPartContext("", 0, 0);
                part.NewFileLength = count[3] + (this.OutputAddedImg ? count[4] : 0) + (this.OutputRemovedImg ? count[5] : 0);

                OnPatchingStateChanged(new Patcher.PatchingEventArgs(part, Patcher.PatchingState.CompareStarted));

                foreach (CompareDifference diff in diffLst)
                {
                    if (kmsContent.Contains(diff))
                    {
                        StateInfo = string.Format("{0}/{1} 变更: {2}", count[0], count[3], "KMS内容");
                        count[0]++;
                        continue;
                    }
                    if (godChangseopNode.Contains(diff))
                    {
                        StateInfo = string.Format("{0}/{1} 变更: {2}", count[0], count[3], "神昌燮重复节点");
                        count[0]++;
                        continue;
                    }
                    OnPatchingStateChanged(new Patcher.PatchingEventArgs(part, Patcher.PatchingState.TempFileBuildProcessChanged, count[0] + count[1] + count[2]));
                    switch (diff.DifferenceType)
                    {
                        case DifferenceType.Changed:
                            {
                                StateInfo = string.Format("{0}/{1} 变更: {2}", count[0], count[3], diff.NodeNew.FullPath);
                                Wz_Image imgNew, imgOld;
                                if ((imgNew = diff.ValueNew as Wz_Image) != null
                                    && ((imgOld = diff.ValueOld as Wz_Image) != null))
                                {
                                    string anchorName = "a_0_" + count[0];
                                    string menuAnchorName = "m_0_" + count[0];
                                    CompareImg(imgNew, imgOld, diff.NodeNew.FullPathToFile, anchorName, menuAnchorName, srcDirPath, sw);
                                }
                                count[0]++;
                            }
                            break;

                        case DifferenceType.Append:
                            if (this.OutputAddedImg)
                            {
                                StateInfo = string.Format("{0}/{1} 新增: {2}", count[1], count[4], diff.NodeNew.FullPath);
                                Wz_Image imgNew = diff.ValueNew as Wz_Image;
                                if (imgNew != null)
                                {
                                    string anchorName = "a_1_" + count[1];
                                    string menuAnchorName = "m_1_" + count[1];
                                    OutputImg(imgNew, diff.DifferenceType, diff.NodeNew.FullPathToFile, anchorName, menuAnchorName, srcDirPath, sw);
                                }
                                count[1]++;
                            }
                            break;

                        case DifferenceType.Remove:
                            if (this.OutputRemovedImg)
                            {
                                StateInfo = string.Format("{0}/{1} 删除: {2}", count[2], count[5], diff.NodeOld.FullPath);
                                Wz_Image imgOld = diff.ValueOld as Wz_Image;
                                if (imgOld != null)
                                {
                                    string anchorName = "a_2_" + count[2];
                                    string menuAnchorName = "m_2_" + count[2];
                                    OutputImg(imgOld, diff.DifferenceType, diff.NodeOld.FullPathToFile, anchorName, menuAnchorName, srcDirPath, sw);
                                }
                                count[2]++;
                            }
                            break;

                        case DifferenceType.NotChanged:
                            break;
                    }

                }
                //html结束
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");

                if (index != null)
                {
                    index.WriteLine("<tr><td><a href=\"{0}.html\">{0}.wz</a></td><td>{1}</td><td>{2}</td><td><a href=\"{0}.html#m_0\">{3}</a></td><td><a href=\"{0}.html#m_1\">{4}</a></td><td><a href=\"{0}.html#m_2\">{5}</a></td></tr>",
                        type.ToString(),
                        string.Join("<br/>", fileNew.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileSize.ToString("N0")))),
                        string.Join("<br/>", fileOld.SelectMany(wzf => GetFileInfo(wzf, ewzf => ewzf.Header.FileSize.ToString("N0")))),
                        count[3],
                        count[4],
                        count[5]
                        );
                    index.Flush();
                }
            }
            finally
            {
                try
                {
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch
                {
                }
                OnPatchingStateChanged(new Patcher.PatchingEventArgs(null, Patcher.PatchingState.CompareFinished));
            }
            if (saveSkillTooltip && type.ToString() == "String" && skillTooltipInfo != null)
            {
                if (!Directory.Exists(skillTooltipPath))
                {
                    Directory.CreateDirectory(skillTooltipPath);
                }
                saveTooltip(skillTooltipPath);
            }
            if (saveSkillTooltip && type.ToString() == "String" && PerJobSkillTooltipInfo != null)
            {
                if (!Directory.Exists(skillTooltipPath))
                {
                    Directory.CreateDirectory(skillTooltipPath);
                }
                savePerJobSkillTooltip(skillTooltipPath);
            }
            if (saveItemTooltip && type.ToString() == "String" && itemTooltipInfo != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                if (this.EnableAssembleTooltip)
                {
                    saveTooltip22(itemTooltipPath);
                }
                else
                {
                    saveTooltip2(itemTooltipPath);
                }
            }
            if (saveEqpTooltip && type.ToString() == "String" && eqpTooltipInfo != null)
            {
                if (!Directory.Exists(eqpTooltipPath))
                {
                    Directory.CreateDirectory(eqpTooltipPath);
                }
                saveTooltip3(eqpTooltipPath);
            }
            if (saveMapTooltip && type.ToString() == "String" && mapTooltipInfo != null)
            {
                if (!Directory.Exists(mapTooltipPath))
                {
                    Directory.CreateDirectory(mapTooltipPath);
                }
                saveTooltip7(mapTooltipPath);
            }
            if (saveMobTooltip && type.ToString() == "String" && mobTooltipInfo != null)
            {
                if (!Directory.Exists(mobTooltipPath))
                {
                    Directory.CreateDirectory(mobTooltipPath);
                }
                saveTooltip4(mobTooltipPath);
            }
            if (saveNpcTooltip && type.ToString() == "String" && npcTooltipInfo != null)
            {
                if (!Directory.Exists(npcTooltipPath))
                {
                    Directory.CreateDirectory(npcTooltipPath);
                }
                saveTooltip5(npcTooltipPath);
            }
            if (saveQuestTooltip && type.ToString() == "String" && questTooltipInfo != null)
            {
                if (!Directory.Exists(questTooltipPath))
                {
                    Directory.CreateDirectory(questTooltipPath);
                }
                saveTooltip8(questTooltipPath);
            }
            if (saveAchievementTooltip && type.ToString() == "String" && achievementTooltipInfo != null)
            {
                if (!Directory.Exists(achvTooltipPath))
                {
                    Directory.CreateDirectory(achvTooltipPath);
                }
                saveTooltip9(achvTooltipPath);
            }
            if (saveCashTooltip && type.ToString() == "String" && cashTooltipInfo != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                saveTooltip6(itemTooltipPath);
            }
        }

        //将Tooltip输出为HTML格式
        private void saveTooltipHtml(string outputDir)
        {
            FileStream htmlFile = null;
            StreamWriter sw = null;
            string htmlTooltipPath = Path.Combine(outputDir, "Tooltip.html");
            try
            {
                htmlFile = new FileStream(htmlTooltipPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(htmlFile, Encoding.UTF8);
                sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                sw.WriteLine("<html>");
                sw.WriteLine("<head>");
                sw.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\">");
                sw.WriteLine("<title>Tooltip</title>");
                sw.WriteLine("<link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css?family=Noto+Sans+SC:100,300,400,500,700,900\">");
                sw.WriteLine("<link type=\"text/css\" rel=\"stylesheet\" href=\"https://jancy-1256059393.cos-website.ap-guangzhou.myqcloud.com/Compare/Compare.css\" />");
                sw.WriteLine("</head>");
                sw.WriteLine("<body>");
                foreach (var category in diffHtml)
                {
                    string categoryName = category.Key;
                    Dictionary<string, List<string>> changes = category.Value;
                    if (changes.Values.All(list => list.Count == 0)) continue;
                    string TooltipPath = Path.Combine(outputDir, categoryName + "Tooltip");
                    sw.Write("<p class=\"sections\" section=\"{0}\">{0}</p></br>", categoryName);
                    foreach (var changeType in changes)
                    {
                        string changeName = changeType.Key;
                        List<string> itemList = changeType.Value;
                        sw.WriteLine("<h3 class=\"compare\">{0}</h3></br>", changeName);
                        sw.WriteLine("<ul class=\"{0}\" style=\"font-family: \"Noto Sans SC\"\";>", categoryName);
                        foreach (string item in itemList)
                        {
                            sw.WriteLine("<span><img src=\"{0}/{1}Tooltip/{2}\"></img></span></br>", outputDir, categoryName, item);
                            sw.WriteLine("<li>{0}</li>", item.Split(new[] { '_' + categoryName }, StringSplitOptions.None).Last());
                        }
                        sw.WriteLine("</ul>");
                    }
                }
                sw.WriteLine("</body>");
                sw.WriteLine("</html>");
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                diffHtml.Clear();
            }
        }

        // 变更技能Tooltip处理
        private void UpdateActionChanges()
        {
            if (ChangedActions.Count <= 0) return;

            StateInfo = $"正在整理{ChangedActions.Count}个延迟变更点...";
            StateDetail = "正在以Tooltip图像处理技能变更点...";

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                var skill_wz = PluginManager.FindWz(Wz_Type.Skill, WzFileNewOld[i]);
                foreach (var skill_img in skill_wz?.Nodes ?? new Wz_Node.WzNodeCollection(null))
                {
                    if (!Regex.Match(skill_img.Text, @"^\d+[.]img$").Success) continue;

                    var skill_node = skill_img.FindNodeByPath("skill", true);
                    foreach (var skill in skill_node?.Nodes ?? new Wz_Node.WzNodeCollection(null))
                    {
                        if (!int.TryParse(skill.Text, out int skill_id)) continue;

                        var action_node = skill.FindNodeByPath("action");
                        foreach (var action in action_node?.Nodes ?? new Wz_Node.WzNodeCollection(null))
                        {
                            var action_str = action.GetValueEx<string>(null);
                            if (string.IsNullOrEmpty(action_str)) continue;
                            if (ChangedActions.ContainsKey(action_str))
                            {
                                ChangedActions[action_str].Add(skill_id);
                            }
                        }
                    }
                }
            }
            foreach (var kv in ChangedActions)
            {
                var action = kv.Key;
                var ids = kv.Value;
                foreach (var id in ids)
                {
                    if (!OutputSkillTooltipIDs.Contains(id.ToString()))
                    {
                        OutputSkillTooltipIDs.Add(id.ToString());
                        diffSkillTags[id.ToString()] = new List<string>();
                    }

                    if (!diffSkillTags[id.ToString()].Contains(action))
                    {
                        diffSkillTags[id.ToString()].Add(action);
                    }
                }
            }
            ChangedActions.Clear();
        }

        // 变更技能Tooltip输出
        private void saveTooltip(string skillTooltipPath)
        {
            UpdateActionChanges();
            SkillTooltipRender2[] skillRenderNewOld = new SkillTooltipRender2[2];
            int count = 0;
            int allCount = skillTooltipInfo.Count;
            var skillTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                this.StringWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("String").GetNodeWzFile();
                this.ItemWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Item").GetNodeWzFile();
                this.EtcWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Etc").GetNodeWzFile();
                this.QuestWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Quest").GetNodeWzFile();

                skillRenderNewOld[i] = new SkillTooltipRender2();
                skillRenderNewOld[i].StringLinker = new StringLinker();
                skillRenderNewOld[i].StringLinker.Load(StringWzNewOld[i], ItemWzNewOld[i], EtcWzNewOld[i], QuestWzNewOld[i]);
                skillRenderNewOld[i].ShowObjectID = this.ShowObjectID;
                skillRenderNewOld[i].ShowDelay = true;
                skillRenderNewOld[i].wzNode = WzNewOld[i];
                skillRenderNewOld[i].DiffSkillTags = this.diffSkillTags;
                skillRenderNewOld[i].IgnoreEvalError = true;
                skillRenderNewOld[i].Enable22AniStyle = this.Enable22AniStyle;
                skillRenderNewOld[i].ShowParameters = CharaSimConfig.Default.Skill.ShowParameters;
            }

            diffHtml["Skill"] = new Dictionary<string, List<string>>() { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };
            foreach (var skillID in skillTooltipInfo)
            {
                try
                {
                    count++;
                    StateInfo = string.Format("{0}/{1} 技能: {2}", count, allCount, skillID);
                    StateDetail = "正在以Tooltip图像处理技能变更点...";

                    bool[] isSkillNull = new bool[2] { false, false };

                    if (SkipKMSContent && isKMSSkillID(Int32.Parse(skillID))) continue;

                    string skillType = "";
                    string skillNodePath = int.Parse(skillID) / 10000000 == 8 ? String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 100, skillID) : String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 10000, skillID);
                    if (int.Parse(skillID) / 10000 == 0) skillNodePath = String.Format(@"\000.img\skill\{0:D7}", skillID);
                    int nullSkillIdx = 0;

                    // 绘制变更前技能Tooltip
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Skill skill = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            (Skill.CreateFromNode(PluginManager.FindWz("Skill001" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            (Skill.CreateFromNode(PluginManager.FindWz("Skill002" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            Skill.CreateFromNode(PluginManager.FindWz("Skill003" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i])));

                        if (skill != null)
                        {
                            skill.Level = skill.MaxLevel;
                            skillRenderNewOld[i].Skill = skill;
                        }
                        else
                        {
                            isSkillNull[i] = true;
                            nullSkillIdx = i + 1;
                        }
                    }

                    // 合成Tooltip图片
                    Bitmap resultImage = null;
                    Graphics g = null;

                    switch (nullSkillIdx)
                    {
                        case 0: // change
                            skillType = "变更";

                            Bitmap ImageNew = skillRenderNewOld[0].Render(true);
                            Bitmap ImageOld = skillRenderNewOld[1].Render(true);
                            if (ShowChangeType)
                            {
                                int picHchange = ShowObjectID ? 13 : 1;
                                Graphics[] gNewOld = new Graphics[] { Graphics.FromImage(ImageNew), Graphics.FromImage(ImageOld) };
                                GearGraphics.DrawPlainText(gNewOld[1], "变更前", skillTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                picHchange = ShowObjectID ? 13 : 1;
                                GearGraphics.DrawPlainText(gNewOld[0], "变更后", skillTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                            }

                            resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                            g = Graphics.FromImage(resultImage);

                            g.DrawImage(ImageOld, 0, 0);
                            g.DrawImage(ImageNew, ImageOld.Width, 0);
                            break;

                        case 1: // delete
                            skillType = "删除";
                            if (isSkillNull[1]) continue;
                            resultImage = skillRenderNewOld[1].Render();
                            g = Graphics.FromImage(resultImage);
                            break;

                        case 2: // add
                            skillType = "新增";
                            if (isSkillNull[0]) continue;
                            resultImage = skillRenderNewOld[0].Render();
                            g = Graphics.FromImage(resultImage);
                            break;

                        default:
                            break;
                    }

                    if (resultImage == null || g == null)
                    {
                        continue;
                    }

                    var skillTypeTextInfo = g.MeasureString(skillType, GearGraphics.ItemDetailFont);
                    int picH = ShowObjectID ? 13 : 1;
                    if (ShowChangeType && nullSkillIdx != 0) GearGraphics.DrawPlainText(g, skillType, skillTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(skillTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(skillTooltipPath, "Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(int.Parse(skillID) / 10000) ?? "其它") + "]_" + skillType + ".png");
                    diffHtml["Skill"][skillType].Add("Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(int.Parse(skillID) / 10000) ?? "其它") + "]_" + skillType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Skill Tooltip: " + skillID, ex.Message);
                }
            }
            skillTooltipInfo.Clear();
            diffSkillTags.Clear();
        }

        private void savePerJobSkillTooltip(string skillTooltipPath)
        {
            UpdateActionChanges();
            SkillTooltipRender2[] skillRenderNewOld = new SkillTooltipRender2[2];
            int count = 0;
            int allCount = skillTooltipInfo.Count;
            var skillTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                this.StringWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("String").GetNodeWzFile();
                this.ItemWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Item").GetNodeWzFile();
                this.EtcWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Etc").GetNodeWzFile();
                this.QuestWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Quest").GetNodeWzFile();

                skillRenderNewOld[i] = new SkillTooltipRender2();
                skillRenderNewOld[i].StringLinker = new StringLinker();
                skillRenderNewOld[i].StringLinker.Load(StringWzNewOld[i], ItemWzNewOld[i], EtcWzNewOld[i], QuestWzNewOld[i]);
                skillRenderNewOld[i].ShowObjectID = this.ShowObjectID;
                skillRenderNewOld[i].ShowDelay = true;
                skillRenderNewOld[i].wzNode = WzNewOld[i];
                skillRenderNewOld[i].DiffSkillTags = this.diffPerJobSkillTags;
                skillRenderNewOld[i].IgnoreEvalError = true;
                skillRenderNewOld[i].Enable22AniStyle = this.Enable22AniStyle;
                skillRenderNewOld[i].ShowParameters = CharaSimConfig.Default.Skill.ShowParameters;
            }
            diffHtml["Skill"] = new Dictionary<string, List<string>>() { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };
            foreach (var skillID in PerJobSkillTooltipInfo)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 技能: {2}", ++count, allCount, skillID);
                    StateDetail = "正在以Tooltip图像处理分职业技能变更点...";

                    bool[] isSkillNull = new bool[2] { false, false };

                    if (SkipKMSContent && isKMSSkillID(Int32.Parse(skillID))) continue;

                    string skillType = "";
                    string skillNodePath = int.Parse(skillID) / 10000000 == 8 ? String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 100, skillID) : String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 10000, skillID);
                    if (int.Parse(skillID) / 10000 == 0) skillNodePath = String.Format(@"\000.img\skill\{0:D7}", skillID);
                    int nullSkillIdx = 0;

                    int maxSkillIndex = 0;
                    bool isSixthJobSkill = int.Parse(skillID) / 100000000 == 5;

                    // 变更前后Tooltip图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Skill skill = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            (Skill.CreateFromNode(PluginManager.FindWz("Skill001" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            (Skill.CreateFromNode(PluginManager.FindWz("Skill002" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]) ??
                            Skill.CreateFromNode(PluginManager.FindWz("Skill003" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i])));

                        if (skill != null)
                        {
                            skill.Level = skill.MaxLevel;
                            skillRenderNewOld[i].Skill = skill;
                            maxSkillIndex = skill.PerJobAttackInfo.Count;
                        }
                        else
                        {
                            isSkillNull[i] = true;
                            nullSkillIdx = i + 1;
                        }
                    }

                    for (int jobIndex = 0; jobIndex < maxSkillIndex; jobIndex++)
                    {
                        // 绘制Tooltip图像
                        Bitmap resultImage = null;
                        Graphics g = null;

                        int targetJobId = 0;

                        switch (nullSkillIdx)
                        {
                            case 0: // change
                                skillType = "变更";
                                skillRenderNewOld[0].Skill.PerJobIndex = jobIndex;
                                skillRenderNewOld[1].Skill.PerJobIndex = jobIndex;
                                targetJobId = skillRenderNewOld[0].Skill.PerJobAttackInfo.Keys.ToList()[jobIndex];
                                Bitmap ImageNew = skillRenderNewOld[0].Render(true);
                                Bitmap ImageOld = skillRenderNewOld[1].Render(true);
                                if (ShowChangeType)
                                {
                                    int picHchange = ShowObjectID ? 13 : 1;
                                    Graphics[] gNewOld = new Graphics[] { Graphics.FromImage(ImageNew), Graphics.FromImage(ImageOld) };
                                    GearGraphics.DrawPlainText(gNewOld[1], "变更前", skillTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                    picHchange = ShowObjectID ? 13 : 1;
                                    GearGraphics.DrawPlainText(gNewOld[0], "变更后", skillTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                }

                                resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                                g = Graphics.FromImage(resultImage);

                                g.DrawImage(ImageOld, 0, 0);
                                g.DrawImage(ImageNew, ImageOld.Width, 0);
                                break;

                            case 1: // delete
                                skillType = "删除";
                                if (isSkillNull[1]) continue;
                                skillRenderNewOld[1].Skill.PerJobIndex = jobIndex;
                                targetJobId = skillRenderNewOld[1].Skill.PerJobAttackInfo.Keys.ToList()[jobIndex];
                                resultImage = skillRenderNewOld[1].Render();
                                g = Graphics.FromImage(resultImage);
                                break;

                            case 2: // add
                                skillType = "新增";
                                if (isSkillNull[0]) continue;
                                skillRenderNewOld[0].Skill.PerJobIndex = jobIndex;
                                targetJobId = skillRenderNewOld[0].Skill.PerJobAttackInfo.Keys.ToList()[jobIndex];
                                resultImage = skillRenderNewOld[0].Render();
                                g = Graphics.FromImage(resultImage);
                                break;

                            default:
                                break;
                        }

                        if (resultImage == null || g == null)
                        {
                            continue;
                        }

                        var skillTypeTextInfo = g.MeasureString(skillType, GearGraphics.ItemDetailFont);
                        int picH = ShowObjectID ? 13 : 1;
                        if (ShowChangeType && nullSkillIdx != 0) GearGraphics.DrawPlainText(g, skillType, skillTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(skillTypeTextInfo.Width) + 2, ref picH, 10);

                        string categoryPath = (ItemStringHelper.GetJobName(isSixthJobSkill ? targetJobId + 2 : targetJobId) ?? "其他");
                        if (!Directory.Exists(Path.Combine(skillTooltipPath, categoryPath)))
                        {
                            Directory.CreateDirectory(Path.Combine(skillTooltipPath, categoryPath));
                        }

                        string imageName = Path.Combine(skillTooltipPath, categoryPath, "Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(int.Parse(skillID) / 10000) ?? "其它") + "]_" + skillType + ".png");
                        diffHtml["Skill"][skillType].Add("Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(int.Parse(skillID) / 10000) ?? "其它") + "]_" + skillType + ".png");
                        if (!File.Exists(imageName))
                        {
                            resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        resultImage.Dispose();
                        g.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Skill Tooltip: " + skillID, ex.Message);
                }
            }
            PerJobSkillTooltipInfo.Clear();
            diffPerJobSkillTags.Clear();
        }

        // 变更道具Tooltip输出
        private void saveTooltip2(string itemTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            ItemTooltipRender2 itemRenderNew = new ItemTooltipRender2();
            ItemTooltipRender2 itemRenderOld = new ItemTooltipRender2();
            int count2 = 0;
            int allCount2 = itemTooltipInfo.Count;
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            itemRenderNew.StringLinker = slNew;
            itemRenderOld.StringLinker = slOld;
            itemRenderNew.ShowObjectID = true;
            itemRenderOld.ShowObjectID = true;
            itemRenderNew.ShowLinkedTamingMob = this.ShowLinkedTamingMob;
            itemRenderOld.ShowLinkedTamingMob = this.ShowLinkedTamingMob;
            itemRenderNew.CompareMode = true;
            itemRenderOld.CompareMode = true;
            itemRenderNew.ShowApplicablePetEquip = this.LocatePetEquip;
            itemRenderOld.ShowApplicablePetEquip = this.LocatePetEquip;
            itemRenderNew.Enable22AniStyle = CharaSimConfig.Default.Enable22AniStyle;
            itemRenderOld.Enable22AniStyle = CharaSimConfig.Default.Enable22AniStyle;
            diffHtml["Item"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var itemID in itemTooltipInfo)
            {
                try
                {
                    count2++;
                    StateInfo = string.Format("{0}/{1} 道具: {2}", count2, allCount2, itemID);
                    StateDetail = "正在以Tooltip图像处理道具变更点...";

                    Bitmap itemImageNew = null;
                    Bitmap itemImageOld = null;
                    string itemType = "删除";
                    string itemNodePath = null;
                    if (itemID.StartsWith("03015")) // 判断开头是否是03015
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 100, itemID);
                    }
                    else if (itemID.StartsWith("0301")) // 判断开头是否是0301
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 1000, itemID);
                    }
                    else if (itemID.StartsWith("500")) // 判断开头是否是0500
                    {
                        itemNodePath = String.Format(@"Item\Pet\{0:D}.img", itemID);
                    }
                    else if (itemID.StartsWith("02")) // 判断第1位是否是02
                    {
                        itemNodePath = String.Format(@"Item\Consume\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("03")) // 判断第1位是否是03
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("04")) // 判断第1位是否是04
                    {
                        itemNodePath = String.Format(@"Item\Etc\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("05")) // 判断第1位是否是05
                    {
                        itemNodePath = String.Format(@"Item\Cash\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    Item itemNew = Item.CreateFromNode(PluginManager.FindWz(itemNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz);
                    if (itemNew != null)
                    {
                        itemRenderNew.Item = itemNew;  // 使用 itemRenderNew 渲染 Item 类型
                        itemImageNew = itemRenderNew.Render(); // 渲染图像
                        width += itemImageNew.Width;
                        heightNew = itemImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    Item itemOld = Item.CreateFromNode(PluginManager.FindWz(itemNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz);
                    if (itemOld != null)
                    {
                        itemRenderOld.Item = itemOld;  // 使用 itemRenderNew 渲染 Item 类型
                        itemImageOld = itemRenderOld.Render(); // 渲染图像
                        width += itemImageOld.Width;
                        heightOld = itemImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (itemImageOld != null)
                    {
                        if (itemImageNew != null)
                        {
                            g.DrawImage(itemImageNew, itemImageOld.Width, 0);
                            itemImageNew.Dispose();
                            itemType = "变更";
                        }
                        g.DrawImage(itemImageOld, 0, 0);
                        itemImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(itemImageNew, 0, 0);
                        itemImageNew.Dispose();
                        itemType = "新增";
                    }
                    var itemTypeTextInfo = g.MeasureString(itemType, GearGraphics.ItemDetailFont2);
                    int picH = 13;
                    GearGraphics.DrawPlainText(g, itemType, itemTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(itemTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(itemTooltipPath, "Item_" + itemID + "_" + itemType + ".png");
                    diffHtml["Item"][itemType].Add("Item_" + itemID + "_" + itemType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Item Tooltip: " + itemID, ex.Message);
                }
            }
            itemTooltipInfo.Clear();
            diffItemTags.Clear();
        }

        private void saveTooltip22(string itemTooltipPath)
        {
            ItemTooltipRender3[] itemRenderNewOld = new ItemTooltipRender3[2];
            int count = 0;
            int allCount = itemTooltipInfo.Count;
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                this.StringWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("String").GetNodeWzFile();
                this.ItemWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Item").GetNodeWzFile();
                this.EtcWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Etc").GetNodeWzFile();
                this.QuestWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Quest").GetNodeWzFile();

                itemRenderNewOld[i] = new ItemTooltipRender3();
                itemRenderNewOld[i].StringLinker = new StringLinker();
                itemRenderNewOld[i].StringLinker.Load(StringWzNewOld[i], ItemWzNewOld[i], EtcWzNewOld[i], QuestWzNewOld[i]);
                itemRenderNewOld[i].ShowObjectID = this.ShowObjectID;
                itemRenderNewOld[i].ShowLinkedTamingMob = this.ShowLinkedTamingMob;
                itemRenderNewOld[i].AllowFamiliarOutOfBounds = this.AllowFamiliarOutOfBounds;
                itemRenderNewOld[i].UseCTFamiliarRender = this.UseCTFamiliarUI;
                itemRenderNewOld[i].ShowApplicablePetEquip = this.LocatePetEquip;
                itemRenderNewOld[i].CompareMode = true;
            }
            diffHtml["Item"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var itemID in itemTooltipInfo)
            {
                try
                {

                    StateInfo = string.Format("{0}/{1} 道具: {2}", ++count, allCount, itemID);
                    StateDetail = "正在以Tooltip图像处理道具变更点...";
                    bool[] isItemNull = new bool[2] { false, false };
                    string itemType = "";
                    string itemNodePath = null;
                    string categoryPath = "";

                    if (!int.TryParse(itemID, out _)) continue;
                    if (SkipKMSContent && KMSContentID["Item"].Contains((Int32.Parse(itemID)))) continue;

                    if (itemID.StartsWith("03015")) // 判断开头是否是03015
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 100, itemID);
                    }
                    else if (itemID.StartsWith("0301")) // 判断开头是否是0301
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 1000, itemID);
                    }
                    else if (itemID.StartsWith("500")) // 判断开头是否是0500
                    {
                        itemNodePath = String.Format(@"Item\Pet\{0:D}.img", itemID);
                    }
                    else if (itemID.StartsWith("02")) // 判断第1位是否是02
                    {
                        itemNodePath = String.Format(@"Item\Consume\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("03")) // 判断第1位是否是03
                    {
                        itemNodePath = String.Format(@"Item\Install\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("04")) // 判断第1位是否是04
                    {
                        itemNodePath = String.Format(@"Item\Etc\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    else if (itemID.StartsWith("05")) // 判断第1位是否是05
                    {
                        itemNodePath = String.Format(@"Item\Cash\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }

                    StringResult sr;
                    string ItemName;
                    if (itemRenderNewOld[1].StringLinker == null || !itemRenderNewOld[1].StringLinker.StringItem.TryGetValue(int.Parse(itemID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知道具";
                    }
                    ItemName = sr.Name;
                    if (itemRenderNewOld[0].StringLinker == null || !itemRenderNewOld[0].StringLinker.StringItem.TryGetValue(int.Parse(itemID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知道具";
                    }
                    if (ItemName != sr.Name && ItemName != "未知道具" && sr.Name != "未知道具")
                    {
                        ItemName += "_" + sr.Name;
                    }
                    else if (ItemName == "未知道具")
                    {
                        ItemName = sr.Name;
                    }
                    if (String.IsNullOrEmpty(ItemName)) ItemName = "未知道具";
                    ItemName = RemoveInvalidFileNameChars(ItemName);
                    int nullItemIdx = 0;

                    // 変更前後のツールチップ画像の作成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Item item = Item.CreateFromNode(PluginManager.FindWz(itemNodePath, WzFileNewOld[i]), PluginManager.FindWz);

                        if (item != null)
                        {
                            itemRenderNewOld[i].Item = item;
                        }
                        else
                        {
                            isItemNull[i] = true;
                            nullItemIdx = i + 1;
                        }
                    }

                    // 合成Tooltip图像
                    Bitmap resultImage = null;
                    Graphics g = null;

                    switch (nullItemIdx)
                    {
                        case 0: // change
                            itemType = "变更";


                            Bitmap ImageNew = itemRenderNewOld[0].Render();
                            Bitmap ImageOld = itemRenderNewOld[1].Render();
                            if (GetBitmapHash(ImageNew) == GetBitmapHash(ImageOld)) continue;
                            if (ShowChangeType)
                            {
                                int picHchange = ShowObjectID ? 13 : 1;
                                Graphics[] gNewOld = new Graphics[] { Graphics.FromImage(ImageNew), Graphics.FromImage(ImageOld) };
                                GearGraphics.DrawPlainText(gNewOld[1], "变更前", itemTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                picHchange = ShowObjectID ? 13 : 1;
                                GearGraphics.DrawPlainText(gNewOld[0], "变更后", itemTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                            }
                            resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                            g = Graphics.FromImage(resultImage);

                            g.DrawImage(ImageOld, 0, 0);
                            g.DrawImage(ImageNew, ImageOld.Width, 0);
                            break;

                        case 1: // delete
                            itemType = "删除";
                            if (isItemNull[1]) continue;
                            resultImage = itemRenderNewOld[1].Render();
                            g = Graphics.FromImage(resultImage);
                            break;

                        case 2: // add
                            itemType = "新增";
                            if (isItemNull[0]) continue;
                            resultImage = itemRenderNewOld[0].Render();
                            g = Graphics.FromImage(resultImage);
                            break;

                        default:
                            break;
                    }

                    if (resultImage == null || g == null)
                    {
                        continue;
                    }

                    if (!Directory.Exists(Path.Combine(itemTooltipPath, categoryPath)))
                    {
                        Directory.CreateDirectory(Path.Combine(itemTooltipPath, categoryPath));
                    }

                    var itemTypeTextInfo = g.MeasureString(itemType, GearGraphics.ItemDetailFont);
                    int picH = ShowObjectID ? 13 : 1;
                    if (ShowChangeType && nullItemIdx != 0) GearGraphics.DrawPlainText(g, itemType, itemTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(itemTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(itemTooltipPath, categoryPath, "Item_" + itemID + "_" + itemType + ".png");
                    diffHtml["Item"][itemType].Add("Item_" + itemID + "_" + itemType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Item Tooltip 3: " + itemID, ex.Message);
                }
            }
            itemTooltipInfo.Clear();
            diffItemTags.Clear();
        }

        // 变更装备Tooltip输出
        private void saveTooltip3(string eqpTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            GearTooltipRender22 eqpRenderNew = new GearTooltipRender22();
            GearTooltipRender22 eqpRenderOld = new GearTooltipRender22();
            int count3 = 0;
            int allCount3 = eqpTooltipInfo.Count;
            var eqpTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            eqpRenderNew.StringLinker = slNew;
            eqpRenderOld.StringLinker = slOld;
            eqpRenderNew.ShowObjectID = true;
            eqpRenderOld.ShowObjectID = true;
            eqpRenderNew.ShowApplicablePet = this.LocatePetEquip;
            eqpRenderOld.ShowApplicablePet = this.LocatePetEquip;
            diffHtml["Eqp"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var eqpID in eqpTooltipInfo)
            {
                try
                {
                    count3++;
                    StateInfo = string.Format("{0}/{1} 装备: {2}", count3, allCount3, eqpID);
                    StateDetail = "正在以Tooltip图像处理装备变更点...";
                    Bitmap eqpImageNew = null;
                    Bitmap eqpImageOld = null;
                    string eqpType = "删除";
                    string eqpNodePath = null;
                    if (Regex.IsMatch(eqpID, "^0101|^0102|^0103|^0112|^0113|^0114|^0115|^0116|^0118|^0119")) // 判断开头是否是0101~0103或0112~0116-0118~0119
                    {
                        eqpNodePath = String.Format(@"Character\Accessory\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0100")) // 判断开头是否是0100
                    {
                        eqpNodePath = String.Format(@"Character\Cap\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0104")) // 判断开头是否是0104
                    {
                        eqpNodePath = String.Format(@"Character\Coat\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0105")) // 判断开头是否是0104
                    {
                        eqpNodePath = String.Format(@"Character\Longcoat\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0106")) // 判断开头是否是0106
                    {
                        eqpNodePath = String.Format(@"Character\Pants\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0107")) // 判断开头是否是0107
                    {
                        eqpNodePath = String.Format(@"Character\Shoes\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0108")) // 判断开头是否是0108
                    {
                        eqpNodePath = String.Format(@"Character\Glove\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0109")) // 判断开头是否是0109
                    {
                        eqpNodePath = String.Format(@"Character\Shield\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0110")) // 判断开头是否是0110
                    {
                        eqpNodePath = String.Format(@"Character\Cape\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0111")) // 判断开头是否是0111
                    {
                        eqpNodePath = String.Format(@"Character\Ring\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0120") || eqpID.StartsWith("120")) // 判断开头是否是0120
                    {
                        eqpNodePath = String.Format(@"Character\Totem\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^012[1-9]|^013|^014|^015|^0160|^0169|^0170|^0172")) // 判断开头是否是012~015、0160或0169-0179
                    {
                        eqpNodePath = String.Format(@"Character\Weapon\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0161|^0162|^0163|^0164|^0165"))// 判断开头是否是0161~0165
                    {
                        eqpNodePath = String.Format(@"Character\Mechanic\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0166|^0167")) // 判断开头是否是0166或0167
                    {
                        eqpNodePath = String.Format(@"Character\Android\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("0168")) // 判断开头是否是0168
                    {
                        eqpNodePath = String.Format(@"Character\Bits\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("01712")) // 判断开头是否是01712
                    {
                        eqpNodePath = String.Format(@"Character\ArcaneForce\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^01713|^01714")) // 判断开头是否是01713或01714
                    {
                        eqpNodePath = String.Format(@"Character\AuthenticForce\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0179"))  // 判断开头是否是0179
                    {
                        eqpNodePath = String.Format(@"Character\NT_Beauty\{0:D}.img", eqpID);
                    }
                    else if (eqpID.StartsWith("018")) // 判断开头是否是018
                    {
                        eqpNodePath = String.Format(@"Character\PetEquip\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0194|^0195|^0196|^0197")) // 判断开头是否是0194~0197
                    {
                        eqpNodePath = String.Format(@"Character\Dragon\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0190|^0191|^0192|^0193|^0198")) // 判断开头是否是0190~0193或0198
                    {
                        eqpNodePath = String.Format(@"Character\TamingMob\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0002|^0005")) // 判断开头是否是0002或0005
                    {
                        eqpNodePath = String.Format(@"Character\Face\{0:D}.img", eqpID);
                    }
                    else if (Regex.IsMatch(eqpID, "^0003|^0004|^0006")) // 判断开头是否是0003、0004或0006
                    {
                        eqpNodePath = String.Format(@"Character\Hair\{0:D}.img", eqpID);
                    }
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    Gear eqpNew = Gear.CreateFromNode(PluginManager.FindWz(eqpNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz);
                    if (eqpNew != null)
                    {
                        eqpRenderNew.Gear = eqpNew;
                        eqpImageNew = eqpRenderNew.Render();
                        width += eqpImageNew.Width;
                        heightNew = eqpImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    Gear eqpOld = Gear.CreateFromNode(PluginManager.FindWz(eqpNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz);
                    if (eqpOld != null)
                    {
                        eqpRenderOld.Gear = eqpOld;
                        eqpImageOld = eqpRenderOld.Render();
                        width += eqpImageOld.Width;
                        heightOld = eqpImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (eqpImageOld != null)
                    {
                        if (eqpImageNew != null)
                        {
                            g.DrawImage(eqpImageNew, eqpImageOld.Width, 0);
                            eqpImageNew.Dispose();
                            eqpType = "变更";
                        }
                        g.DrawImage(eqpImageOld, 0, 0);
                        eqpImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(eqpImageNew, 0, 0);
                        eqpImageNew.Dispose();
                        eqpType = "新增";
                    }
                    var eqpTypeTextInfo = g.MeasureString(eqpType, GearGraphics.EquipDetailFont2);
                    int picH = 13;
                    GearGraphics.DrawPlainText(g, eqpType, eqpTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(eqpTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(eqpTooltipPath, "Eqp_" + eqpID + "_" + eqpType + ".png");
                    diffHtml["Eqp"][eqpType].Add("Eqp_" + eqpID + "_" + eqpType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Gear Tooltip: " + eqpID, ex.Message);
                }
            }
            eqpTooltipInfo.Clear();
            diffEqpTags.Clear();
        }

        // 变更怪物Tooltip输出
        private void saveTooltip4(string mobTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            MobTooltipRenderer mobRenderNew = new MobTooltipRenderer();
            MobTooltipRenderer mobRenderOld = new MobTooltipRenderer();
            int count4 = 0;
            int allCount4 = mobTooltipInfo.Count;
            var mobTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            mobRenderNew.StringLinker = slNew;
            mobRenderOld.StringLinker = slOld;
            mobRenderNew.ShowObjectID = true;
            mobRenderOld.ShowObjectID = true;
            diffHtml["Mob"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var mobID in mobTooltipInfo)
            {
                try
                {
                    count4++;
                    StateInfo = string.Format("{0}/{1} 怪物: {2}", count4, allCount4, mobID);
                    StateDetail = "正在以Tooltip图像处理怪物变更点...";

                    Bitmap mobImageNew = null;
                    Bitmap mobImageOld = null;
                    string mobType = "删除";
                    string mobNodePath = String.Format(@"Mob\{0:D}.img", mobID);
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    Mob mobNew = Mob.CreateFromNode(PluginManager.FindWz(mobNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz);
                    if (mobNew != null)
                    {
                        mobRenderNew.MobInfo = mobNew;
                        mobImageNew = mobRenderNew.Render();
                        width += mobImageNew.Width;
                        heightNew = mobImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    Mob mobOld = Mob.CreateFromNode(PluginManager.FindWz(mobNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz);
                    if (mobOld != null)
                    {
                        mobRenderOld.MobInfo = mobOld;
                        mobImageOld = mobRenderOld.Render();
                        width += mobImageOld.Width;
                        heightOld = mobImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (mobImageOld != null)
                    {
                        if (mobImageNew != null)
                        {
                            g.DrawImage(mobImageNew, mobImageOld.Width, 0);
                            mobImageNew.Dispose();
                            mobType = "变更";
                        }
                        g.DrawImage(mobImageOld, 0, 0);
                        mobImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(mobImageNew, 0, 0);
                        mobImageNew.Dispose();
                        mobType = "新增";
                    }
                    var mobTypeTextInfo = g.MeasureString(mobType, GearGraphics.EquipDetailFont2);
                    //int picH = 13;
                    //GearGraphics.DrawPlainText(g, mobType, mobTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(mobTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(mobTooltipPath, "Mob_" + mobID + "_" + mobType + ".png");
                    diffHtml["Mob"][mobType].Add("Mob_" + mobID + "_" + mobType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Mob Tooltip: " + mobID, ex.Message);
                }
            }
            mobTooltipInfo.Clear();
            diffMobTags.Clear();
        }

        // 变更NPC Tooltip输出
        private void saveTooltip5(string npcTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            NpcTooltipRenderer npcRenderNew = new NpcTooltipRenderer();
            NpcTooltipRenderer npcRenderOld = new NpcTooltipRenderer();
            int count5 = 0;
            int allCount5 = npcTooltipInfo.Count;
            var npcTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            npcRenderNew.StringLinker = slNew;
            npcRenderOld.StringLinker = slOld;
            npcRenderNew.ShowAllIllustAtOnce = true;
            npcRenderOld.ShowAllIllustAtOnce = true;
            npcRenderNew.EnableWorldArchive = true;
            npcRenderOld.EnableWorldArchive = true;
            npcRenderNew.ShowNpcQuotes = true;
            npcRenderOld.ShowNpcQuotes = true;
            npcRenderNew.ShowObjectID = true;
            npcRenderOld.ShowObjectID = true;
            diffHtml["Npc"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var npcID in npcTooltipInfo)
            {
                try
                {
                    count5++;
                    StateInfo = string.Format("{0}/{1} NPC: {2}", count5, allCount5, npcID);
                    StateDetail = "正在以Tooltip图像处理NPC变更点...";

                    Bitmap npcImageNew = null;
                    Bitmap npcImageOld = null;
                    string npcType = "删除";
                    string npcNodePath = String.Format(@"Npc\{0:D}.img", npcID);
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    Npc npcNew = Npc.CreateFromNode(PluginManager.FindWz(npcNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz, PluginManager.FindWz);
                    if (npcNew != null)
                    {
                        npcRenderNew.NpcInfo = npcNew;
                        npcImageNew = npcRenderNew.Render();
                        width += npcImageNew.Width;
                        heightNew = npcImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    Npc npcOld = Npc.CreateFromNode(PluginManager.FindWz(npcNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz, PluginManager.FindWz);
                    if (npcOld != null)
                    {
                        npcRenderOld.NpcInfo = npcOld;
                        npcImageOld = npcRenderOld.Render();
                        width += npcImageOld.Width;
                        heightOld = npcImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (npcImageOld != null)
                    {
                        if (npcImageNew != null)
                        {
                            g.DrawImage(npcImageNew, npcImageOld.Width, 0);
                            npcImageNew.Dispose();
                            npcType = "变更";
                        }
                        g.DrawImage(npcImageOld, 0, 0);
                        npcImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(npcImageNew, 0, 0);
                        npcImageNew.Dispose();
                        npcType = "新增";
                    }
                    var npcTypeTextInfo = g.MeasureString(npcType, GearGraphics.EquipDetailFont2);
                    //int picH = 13;
                    //GearGraphics.DrawPlainText(g, npcType, npcTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(npcTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(npcTooltipPath, "Npc_" + npcID + "_" + npcType + ".png");
                    diffHtml["Npc"][npcType].Add("Npc_" + npcID + "_" + npcType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Npc Tooltip: " + npcID, ex.Message);
                }
            }
            npcTooltipInfo.Clear();
            diffNpcTags.Clear();
        }

        // 变更礼包Tooltip输出
        private void saveTooltip6(string itemTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            CashPackageTooltipRender cashRenderNew = new CashPackageTooltipRender();
            CashPackageTooltipRender cashRenderOld = new CashPackageTooltipRender();
            int count6 = 0;
            int allCount6 = cashTooltipInfo.Count;
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            cashRenderNew.StringLinker = slNew;
            cashRenderOld.StringLinker = slOld;
            cashRenderNew.ShowObjectID = true;
            cashRenderOld.ShowObjectID = true;
            diffHtml["Item"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var itemID in cashTooltipInfo)
            {
                try
                {
                    count6++;
                    StateInfo = string.Format("{0}/{1} 礼包: {2}", count6, allCount6, itemID);
                    StateDetail = "正在以Tooltip图像处理礼包变更点...";

                    Bitmap itemImageNew = null;
                    Bitmap itemImageOld = null;
                    string itemType = "删除";
                    string itemNodePath = null;
                    if (itemID.StartsWith("9")) // 判断第1位是否是09
                    {
                        itemNodePath = String.Format(@"Item\Special\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                    }
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    CashPackage itemNew = CashPackage.CreateFromNode(PluginManager.FindWz(itemNodePath, wzNew?.GetNodeWzFile()), PluginBase.PluginManager.FindWz(string.Format(@"Etc\CashPackage.img\{0}", itemID)), PluginManager.FindWz);
                    if (itemNew != null)
                    {
                        cashRenderNew.CashPackage = itemNew;
                        itemImageNew = cashRenderNew.Render();
                        width += itemImageNew.Width;
                        heightNew = itemImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    CashPackage itemOld = CashPackage.CreateFromNode(PluginManager.FindWz(itemNodePath, wzOld?.GetNodeWzFile()), PluginBase.PluginManager.FindWz(string.Format(@"Etc\CashPackage.img\{0}", itemID)), PluginManager.FindWz);
                    if (itemOld != null)
                    {
                        cashRenderOld.CashPackage = itemOld;
                        itemImageOld = cashRenderOld.Render();
                        width += itemImageOld.Width;
                        heightOld = itemImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (itemImageOld != null)
                    {
                        if (itemImageNew != null)
                        {
                            g.DrawImage(itemImageNew, itemImageOld.Width, 0);
                            itemImageNew.Dispose();
                            itemType = "变更";
                        }
                        g.DrawImage(itemImageOld, 0, 0);
                        itemImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(itemImageNew, 0, 0);
                        itemImageNew.Dispose();
                        itemType = "新增";
                    }
                    var itemTypeTextInfo = g.MeasureString(itemType, GearGraphics.ItemDetailFont2);
                    int picH = 13;
                    GearGraphics.DrawPlainText(g, itemType, itemTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(itemTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(itemTooltipPath, "Item_" + itemID + "_" + itemType + ".png");
                    diffHtml["Item"][itemType].Add("Item_" + itemID + "_" + itemType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("CashPackage Tooltip: " + itemID, ex.Message);
                }
            }
            cashTooltipInfo.Clear();
            diffCashTags.Clear();
        }

        // 变更地图Tooltip输出
        private void saveTooltip7(string mapTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            MapTooltipRenderer mapRenderNew = new MapTooltipRenderer();
            MapTooltipRenderer mapRenderOld = new MapTooltipRenderer();
            int count7 = 0;
            int allCount7 = cashTooltipInfo.Count;
            var mapTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzNew = wzNew?.FindNodeByPath("Quest").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            this.questWzOld = wzOld?.FindNodeByPath("Quest").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew, questWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld, questWzOld);
            mapRenderNew.StringLinker = slNew;
            mapRenderOld.StringLinker = slOld;
            mapRenderNew.ShowObjectID = true;
            mapRenderOld.ShowObjectID = true;
            mapRenderNew.ShowMiniMap = true;
            mapRenderOld.ShowMiniMap = true;
            mapRenderNew.ShowMiniMapMob = true;
            mapRenderOld.ShowMiniMapMob = true;
            mapRenderNew.ShowMiniMapNpc = true;
            mapRenderOld.ShowMiniMapNpc = true;
            mapRenderNew.ShowMiniMapPortal = true;
            mapRenderOld.ShowMiniMapPortal = true;
            mapRenderNew.ShowBgmName = true;
            mapRenderOld.ShowBgmName = true;
            mapRenderNew.ShowMobNpcObjectID = true;
            mapRenderOld.ShowMobNpcObjectID = true;
            diffHtml["Map"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var mapID in mapTooltipInfo)
            {
                try
                {
                    count7++;
                    StateInfo = string.Format("{0}/{1} 地图: {2}", count7, allCount7, mapID);
                    StateDetail = "正在以Tooltip图像处理地图变更点...";

                    Bitmap mapImageNew = null;
                    Bitmap mapImageOld = null;
                    string mapType = "删除";
                    string mapNodePath = string.Format(@"Map\Map\Map{0}\{1:D9}.img", int.Parse(mapID) / 100000000, int.Parse(mapID));
                    int heightNew = 0, heightOld = 0;
                    int width = 0;
                    // 变更后Tooltip图像生成
                    Map mapNew = Map.CreateFromNode(PluginManager.FindWz(mapNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz);
                    if (mapNew != null)
                    {
                        mapRenderNew.Map = mapNew;
                        mapImageNew = mapRenderNew.Render();
                        width += mapImageNew.Width;
                        heightNew = mapImageNew.Height;
                    }
                    if (width == 0) continue;
                    // 变更前Tooltip图像生成
                    Map mapOld = Map.CreateFromNode(PluginManager.FindWz(mapNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz);
                    if (mapOld != null)
                    {
                        mapRenderOld.Map = mapOld;
                        mapImageOld = mapRenderOld.Render();
                        width += mapImageOld.Width;
                        heightOld = mapImageOld.Height;
                    }
                    if (width == 0) continue;
                    // Tooltip图像合成
                    Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                    Graphics g = Graphics.FromImage(resultImage);
                    if (mapImageOld != null)
                    {
                        if (mapImageNew != null)
                        {
                            g.DrawImage(mapImageNew, mapImageOld.Width, 0);
                            mapImageNew.Dispose();
                            mapType = "变更";
                        }
                        g.DrawImage(mapImageOld, 0, 0);
                        mapImageOld.Dispose();
                    }
                    else
                    {
                        g.DrawImage(mapImageNew, 0, 0);
                        mapImageNew.Dispose();
                        mapType = "新增";
                    }
                    var npcTypeTextInfo = g.MeasureString(mapType, GearGraphics.EquipDetailFont2);
                    //int picH = 13;
                    //GearGraphics.DrawPlainText(g, mapType, mapTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(mapTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(mapTooltipPath, "Map_" + mapID + "_" + mapType + ".png");
                    diffHtml["Map"][mapType].Add("Map_" + mapID + "_" + mapType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Map Tooltip: " + mapID, ex.Message);
                }
            }
            mapTooltipInfo.Clear();
            diffMapTags.Clear();
        }

        // 变更任务Tooltip输出
        private void saveTooltip8(string questTooltipPath)
        {
            QuestTooltipRenderer[] questRenderNewOld = new QuestTooltipRenderer[2];
            bool[] isQuestNull = new bool[2] { false, false };
            int count = 0;
            int allCount = questTooltipInfo.Count;
            var questTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                this.StringWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("String").GetNodeWzFile();
                this.ItemWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Item").GetNodeWzFile();
                this.EtcWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Etc").GetNodeWzFile();
                this.QuestWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Quest").GetNodeWzFile();

                questRenderNewOld[i] = new QuestTooltipRenderer();
                questRenderNewOld[i].StringLinker = new StringLinker();
                questRenderNewOld[i].StringLinker.Load(StringWzNewOld[i], ItemWzNewOld[i], EtcWzNewOld[i], QuestWzNewOld[i]);
                questRenderNewOld[i].ShowObjectID = this.ShowObjectID;
                questRenderNewOld[i].DefaultState = this.QuestState;
                questRenderNewOld[i].CompareMode = true;
                questRenderNewOld[i].ShowAllStates = true;
            }
            diffHtml["Quest"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var questID in questTooltipInfo)
            {
                try
                {
                    if (!int.TryParse(questID, out _)) continue;
                    StateInfo = string.Format("{0}/{1} 任务: {2}", ++count, allCount, questID);
                    StateDetail = "正在以Tooltip图像处理任务变更点...";
                    string questType = "";
                    string questNodePath = String.Format(@"Quest\QuestData\{0:D}.img", questID);
                    string questNodePathLegacy = String.Format(@"Quest\QuestInfo.img\{0:D}", questID);

                    StringResult sr;
                    string QuestName;
                    if (questRenderNewOld[1].StringLinker == null || !questRenderNewOld[1].StringLinker.StringQuest.TryGetValue(int.Parse(questID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知任务";
                    }
                    QuestName = sr.Name;
                    if (questRenderNewOld[0].StringLinker == null || !questRenderNewOld[0].StringLinker.StringQuest.TryGetValue(int.Parse(questID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知任务";
                    }
                    if (QuestName != sr.Name && QuestName != "未知任务" && sr.Name != "未知任务")
                    {
                        QuestName += "_" + sr.Name;
                    }
                    else if (QuestName == "未知任务")
                    {
                        QuestName = sr.Name;
                    }
                    if (String.IsNullOrEmpty(QuestName)) QuestName = "未知任务";
                    QuestName = RemoveInvalidFileNameChars(QuestName);
                    int nullQuestIdx = 0;

                    // 变更前后Tooltip图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Quest quest = Quest.CreateFromNode(PluginManager.FindWz(questNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz) ?? Quest.CreateFromNode(PluginManager.FindWz(questNodePathLegacy, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, fromInfoNode: int.Parse(questID));

                        if (quest != null)
                        {
                            questRenderNewOld[i].Quest = quest;
                        }
                        else
                        {
                            quest = Quest.CreateFromNode(PluginManager.FindWz(questNodePathLegacy, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz);
                            if (quest == null)
                            {
                                isQuestNull[i] = true;
                                nullQuestIdx = i + 1;
                            }
                            else
                            {
                                questRenderNewOld[i].Quest = quest;
                            }
                        }
                    }

                    // Tooltip图像合成
                    Bitmap resultImage = null;
                    Graphics g = null;

                    switch (nullQuestIdx)
                    {
                        case 0: // change
                            questType = "变更";

                            Bitmap ImageNew = questRenderNewOld[0].Render();
                            Bitmap ImageOld = questRenderNewOld[1].Render();
                            if (GetBitmapHash(ImageNew) == GetBitmapHash(ImageOld)) continue;
                            if (ShowChangeType)
                            {
                                int picHchange = ShowObjectID ? 25 : 1;
                                Graphics[] gNewOld = new Graphics[] { Graphics.FromImage(ImageNew), Graphics.FromImage(ImageOld) };
                                picHchange += questRenderNewOld[1].Margin_top;
                                GearGraphics.DrawPlainText(gNewOld[1], "变更前", questTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                picHchange = ShowObjectID ? 25 : 1;
                                picHchange += questRenderNewOld[0].Margin_top;
                                GearGraphics.DrawPlainText(gNewOld[0], "变更后", questTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                            }
                            resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                            g = Graphics.FromImage(resultImage);

                            g.DrawImage(ImageOld, 0, 0);
                            g.DrawImage(ImageNew, ImageOld.Width, 0);
                            break;

                        case 1: // delete
                            questType = "删除";
                            if (isQuestNull[1]) continue;
                            resultImage = questRenderNewOld[1].Render();
                            if (resultImage == null) continue;
                            g = Graphics.FromImage(resultImage);
                            break;

                        case 2: // add
                            questType = "新增";
                            if (isQuestNull[0]) continue;
                            resultImage = questRenderNewOld[0].Render();
                            if (resultImage == null) continue;
                            g = Graphics.FromImage(resultImage);
                            break;

                        default:
                            break;
                    }

                    if (resultImage == null || g == null)
                    {
                        continue;
                    }

                    var questTypeTextInfo = g.MeasureString(questType, GearGraphics.ItemDetailFont);
                    int picH = ShowObjectID ? 25 : 1;
                    switch (nullQuestIdx)
                    {
                        case 1:
                            picH += questRenderNewOld[1].Margin_top;
                            break;
                        case 2:
                            picH += questRenderNewOld[0].Margin_top;
                            break;
                        default:
                            break;
                    }
                    if (ShowChangeType && nullQuestIdx != 0) GearGraphics.DrawPlainText(g, questType, questTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(questTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(questTooltipPath, "Quest_" + questID + "_" + questType + ".png");
                    diffHtml["Quest"][questType].Add("Quest_" + questID + "_" + questType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Quest Tooltip: " + questID, ex.Message);
                }
            }
            questTooltipInfo.Clear();
        }

        // 变更成就Tooltip输出
        private void saveTooltip9(string achvTooltipPath)
        {
            AchievementTooltipRenderer[] achvRenderNewOld = new AchievementTooltipRenderer[2];
            int count = 0;
            int allCount = achievementTooltipInfo.Count;
            var achvTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                this.StringWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("String").GetNodeWzFile();
                this.ItemWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Item").GetNodeWzFile();
                this.EtcWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Etc").GetNodeWzFile();
                this.QuestWzNewOld[i] = WzNewOld[i]?.FindNodeByPath("Quest").GetNodeWzFile();

                achvRenderNewOld[i] = new AchievementTooltipRenderer();
                achvRenderNewOld[i].StringLinker = new StringLinker();
                achvRenderNewOld[i].StringLinker.Load(StringWzNewOld[i], ItemWzNewOld[i], EtcWzNewOld[i], QuestWzNewOld[i]);
                achvRenderNewOld[i].ShowObjectID = this.ShowObjectID;
                achvRenderNewOld[i].CompareMode = true;
            }
            diffHtml["Achievement"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var achvID in achievementTooltipInfo)
            {
                try
                {
                    if (!int.TryParse(achvID, out _)) continue;
                    StateInfo = string.Format("{0}/{1} 成就: {2}", ++count, allCount, achvID);
                    StateDetail = "正在以Tooltip图像处理成就变更点...";
                    bool[] isAchievementNull = new bool[2] { false, false };

                    if (SkipKMSContent && KMSContentID["Achievement"].Contains((Int32.Parse(achvID)))) continue;

                    string achvType = "";
                    string achvNodePath = String.Format(@"Achievement\AchievementData\{0:D}.img", achvID);

                    StringResult sr;
                    string AchievementName;
                    if (achvRenderNewOld[1].StringLinker == null || !achvRenderNewOld[1].StringLinker.StringAchievement.TryGetValue(int.Parse(achvID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知成就";
                    }
                    AchievementName = sr.Name;
                    if (achvRenderNewOld[0].StringLinker == null || !achvRenderNewOld[0].StringLinker.StringAchievement.TryGetValue(int.Parse(achvID), out sr))
                    {
                        sr = new StringResult();
                        sr.Name = "未知成就";
                    }
                    if (AchievementName != sr.Name && AchievementName != "未知成就" && sr.Name != "未知成就")
                    {
                        AchievementName += "_" + sr.Name;
                    }
                    else if (AchievementName == "未知成就")
                    {
                        AchievementName = sr.Name;
                    }
                    if (String.IsNullOrEmpty(AchievementName)) AchievementName = "未知成就";
                    AchievementName = RemoveInvalidFileNameChars(AchievementName);
                    int nullAchievementIdx = 0;

                    // 変更前後のツールチップ画像の作成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Achievement achv = Achievement.CreateFromNode(PluginManager.FindWz($@"Etc\Achievement\AchievementData\{achvID}.img", WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz);

                        if (achv == null)
                        {
                            isAchievementNull[i] = true;
                            nullAchievementIdx = i + 1;
                        }
                        else
                        {
                            achvRenderNewOld[i].Achievement = achv;
                        }
                    }

                    // ツールチップ画像を合わせる
                    Bitmap resultImage = null;
                    Graphics g = null;

                    switch (nullAchievementIdx)
                    {
                        case 0: // change
                            achvType = "变更";

                            Bitmap ImageNew = achvRenderNewOld[0].Render();
                            Bitmap ImageOld = achvRenderNewOld[1].Render();
                            if (GetBitmapHash(ImageNew) == GetBitmapHash(ImageOld)) continue;
                            if (ShowChangeType)
                            {
                                int picHchange = ShowObjectID ? 13 : 1;
                                Graphics[] gNewOld = new Graphics[] { Graphics.FromImage(ImageNew), Graphics.FromImage(ImageOld) };
                                GearGraphics.DrawPlainText(gNewOld[1], "变更前", achvTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                                picHchange = ShowObjectID ? 13 : 1;
                                GearGraphics.DrawPlainText(gNewOld[0], "变更后", achvTypeFont, Color.FromArgb(255, 255, 255), 2, 64, ref picHchange, 10);
                            }
                            resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                            g = Graphics.FromImage(resultImage);

                            g.DrawImage(ImageOld, 0, 0);
                            g.DrawImage(ImageNew, ImageOld.Width, 0);
                            break;

                        case 1: // delete
                            achvType = "删除";
                            if (isAchievementNull[1]) continue;
                            resultImage = achvRenderNewOld[1].Render();
                            if (resultImage == null) continue;
                            g = Graphics.FromImage(resultImage);
                            break;

                        case 2: // add
                            achvType = "新增";
                            if (isAchievementNull[0]) continue;
                            resultImage = achvRenderNewOld[0].Render();
                            if (resultImage == null) continue;
                            g = Graphics.FromImage(resultImage);
                            break;

                        default:
                            break;
                    }

                    if (resultImage == null || g == null)
                    {
                        continue;
                    }

                    var achvTypeTextInfo = g.MeasureString(achvType, GearGraphics.ItemDetailFont);
                    int picH = ShowObjectID ? 13 : 1;
                    if (ShowChangeType && nullAchievementIdx != 0) GearGraphics.DrawPlainText(g, achvType, achvTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(achvTypeTextInfo.Width) + 2, ref picH, 10);

                    string imageName = Path.Combine(achvTooltipPath, "Achievement_" + achvID + "_" + achvType + ".png");
                    diffHtml["Achievement"][achvType].Add("Achievement_" + achvID + "_" + achvType + ".png");
                    if (!File.Exists(imageName))
                    {
                        resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    resultImage.Dispose();
                    g.Dispose();
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Achievement Tooltip: " + achvID, ex.Message);
                }
            }
            achievementTooltipInfo.Clear();
            diffAchvTags.Clear();
        }

        //获取动作变更点
        private void GetActionChanges(Wz_Node node, bool change)
        {
            if (node == null) return;

            Match match = Regex.Match(node.FullPathToFile, @"^Character\\00002000.img\\([^\\]+)\\\d+\\delay");
            if (match.Success)
            {
                string action = match.Groups[1].ToString();

                if (!string.IsNullOrEmpty(action))
                {
                    if (!ChangedActions.ContainsKey(action))
                    {
                        ChangedActions[action] = new HashSet<int>();
                    }
                }
            }
        }

        // 从Skill不同节点获取SkillID
        private void getIDFromSkill(Wz_Node node, bool change)
        {
            if (node == null) return;
            Match match = Regex.Match(node.FullPathToFile, @"^String\\Skill.img\\(\d+).*");
            string tag = null;
            if (!match.Success)
            {
                tag = node.Text;
                match = Regex.Match(node.FullPathToFile, @"^Skill\d*\\\d+.img\\skill\\(\d+)\\(common|masterLevel|combatOrders|action|isPetAutoBuff|isSequenceOn|BGM).*"); // 변경점 중 스킬 툴팁 출력할 것들
                if (change && !match.Success)
                {
                    match = Regex.Match(node.FullPathToFile, @"^Skill\\_Canvas\\\d+.img\\skill\\(\d+)\\(icon)$"); // 스킬 아이콘 변경 체크
                }
            }

            if (match.Success)
            {
                string skillID = match.Groups[1].ToString();
                if (skillID != null)
                {
                    if (node.FindNodeByPath("common\\attackInfo") != null)
                    {
                        if (!PerJobSkillTooltipInfo.Contains(skillID))
                        {
                            PerJobSkillTooltipInfo.Add(skillID);
                            diffPerJobSkillTags[skillID] = new List<string>();
                        }
                    }
                    else
                    {
                        if (!skillTooltipInfo.Contains(skillID))
                        {
                            skillTooltipInfo.Add(skillID);
                            diffSkillTags[skillID] = new List<string>();
                        }
                    }

                    if (tag != null && !diffSkillTags[skillID].Contains(tag))
                    {
                        diffSkillTags[skillID].Add(tag);
                    }
                }
            }
        }

        //从Item不同节点获取ItemID
        private void getIDFromItem(Wz_Node node)
        {
            var tag = node.Text;
            Match match = Regex.Match(node.FullPathToFile, @"^Item\\(Cash|Consume|Etc|Install)\\\d+.img\\(\d+)\\.*");
            if (match.Success)
            {
                string itemID = match.Groups[2].ToString();
                if (!itemTooltipInfo.Contains(itemID) && itemID != null)
                {
                    itemTooltipInfo.Add(itemID);
                    diffItemTags[itemID] = new List<string>();
                    diffItemTags[itemID].Add(tag);
                }
                else if (itemTooltipInfo.Contains(itemID) && itemID != null)
                {
                    if (!diffItemTags[itemID].Contains(tag))
                    {
                        diffItemTags[itemID].Add(tag);
                    }
                }
            }
        }

        //从Item/Special不同节点获取CashID
        private void getIDFromCash(Wz_Node node)
        {
            var tag = node.Text;
            Match match = Regex.Match(node.FullPathToFile, @"^Item\\Special\\\d+.img\\(\d+)\\.*");
            if (match.Success)
            {
                string cashID = match.Groups[1].ToString();
                if (!cashTooltipInfo.Contains(cashID) && cashID != null)
                {
                    cashTooltipInfo.Add(cashID);
                    diffCashTags[cashID] = new List<string>();
                    diffCashTags[cashID].Add(tag);
                }
                else if (cashTooltipInfo.Contains(cashID) && cashID != null)
                {
                    if (!diffCashTags[cashID].Contains(tag))
                    {
                        diffCashTags[cashID].Add(tag);
                    }
                }
            }
        }

        //从Character不同节点找到EqpID
        private void getIDFromChar(Wz_Node node)
        {
            var tag = node.Text;
            Match match3 = Regex.Match(node.FullPathToFile, @"^Character\\\w+\\(\d+).img\\.*");
            if (match3.Success)
            {
                string eqpID = match3.Groups[1].ToString();
                if (!eqpTooltipInfo.Contains(eqpID) && eqpID != null)
                {
                    eqpTooltipInfo.Add(eqpID);
                    diffEqpTags[eqpID] = new List<string>();
                    diffEqpTags[eqpID].Add(tag);
                }
                else if (eqpTooltipInfo.Contains(eqpID) && eqpID != null)
                {
                    if (!diffEqpTags[eqpID].Contains(tag))
                    {
                        diffEqpTags[eqpID].Add(tag);
                    }
                }
            }
        }

        //从Mob不同节点找到MobID
        private void getIDFromMob(Wz_Node node)
        {
            var tag = node.Text;
            Match match4 = Regex.Match(node.FullPathToFile, @"^Mob\\(\d+).img\\.*");
            if (match4.Success)
            {
                string mobID = match4.Groups[1].ToString();
                if (!mobTooltipInfo.Contains(mobID) && mobID != null)
                {
                    mobTooltipInfo.Add(mobID);
                    diffMobTags[mobID] = new List<string>();
                    diffMobTags[mobID].Add(tag);
                }
                else if (mobTooltipInfo.Contains(mobID) && mobID != null)
                {
                    if (!diffMobTags[mobID].Contains(tag))
                    {
                        diffMobTags[mobID].Add(tag);
                    }
                }
            }
        }

        //从NPC不同节点找到NpcID
        private void getIDFromNpc(Wz_Node node)
        {
            var tag = node.Text;
            Match match5 = Regex.Match(node.FullPathToFile, @"^Npc\\(\d+).img\\.*");
            if (match5.Success)
            {
                string npcID = match5.Groups[1].ToString();
                if (!npcTooltipInfo.Contains(npcID) && npcID != null)
                {
                    npcTooltipInfo.Add(npcID);
                    diffNpcTags[npcID] = new List<string>();
                    diffNpcTags[npcID].Add(tag);
                }
                else if (npcTooltipInfo.Contains(npcID) && npcID != null)
                {
                    if (!diffNpcTags[npcID].Contains(tag))
                    {
                        diffNpcTags[npcID].Add(tag);
                    }
                }
            }
        }

        // 노드에서 맵 ID 얻기
        private void GetIDFromMap(Wz_Node node, bool change)
        {
            if (node == null) return;

            Match match = Regex.Match(node.FullPathToFile, @"^String\\Map.img\\.+?\\(\d+)\\(streetName|mapName).*"); // 스트링 확인

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Map\\Map\\Map\d\\(\d+).img\\info\\(barrier|barrierArc|barrierAut).*"); // 변경점 중 툴팁 출력할 것들
            }

            if (change && !match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Map\\Map\\Map\d\\_Canvas\\(\d+).img\\miniMap\\(canvas)$"); // 아이콘 변경 체크
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Map\\Map\\Map\d\\(\d+).img$"); // 추가/삭제 확인
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\MapObjectInfo.img\\(\d+)\\.*"); // 위치 확인
            }

            if (match.Success)
            {
                string mapID = match.Groups[1].ToString();

                if (mapID != null)
                {
                    var id = int.Parse(mapID);
                    if (!OutputMapTooltipIDs.Contains(id))
                    {
                        OutputMapTooltipIDs.Add(id);
                    }
                }
            }
        }

        // 从String不同节点获取ItemID
        private void getIDFromString2(Wz_Node node)
        {
            Match match = Regex.Match(node.FullPathToFile, @"^String\\(Cash.img|Consume.img|Etc.img\\Etc|Ins.img|Pet.img)\\(\d+).*");
            if (match.Success)
            {
                string ItemID = match.Groups[2].ToString();
                if (!ItemID.StartsWith("500"))
                {
                    ItemID = ItemID.PadLeft(8, '0'); // 如果不是以500或910开头，则补齐8位数
                }
                if (!itemTooltipInfo.Contains(ItemID) && ItemID != null)
                {
                    itemTooltipInfo.Add(ItemID);
                }
            }
        }

        // 从String不同节点获取EqpID
        private void getIDFromString3(Wz_Node node)
        {
            Match match3 = Regex.Match(node.FullPathToFile, @"^String\\Eqp.img\\Eqp\\\w+\\(\d+).*");
            if (match3.Success)
            {
                string EqpID = match3.Groups[1].ToString().PadLeft(8, '0');
                if (!eqpTooltipInfo.Contains(EqpID) && EqpID != null)
                {
                    eqpTooltipInfo.Add(EqpID);
                }
            }
        }

        // 从String不同节点获取MobID
        private void getIDFromString4(Wz_Node node)
        {
            Match match4 = Regex.Match(node.FullPathToFile, @"^String\\Mob.img\\(\d+).*");
            if (match4.Success)
            {
                string MobID = match4.Groups[1].ToString().PadLeft(7, '0');
                if (!mobTooltipInfo.Contains(MobID) && MobID != null)
                {
                    mobTooltipInfo.Add(MobID);
                }
            }
        }

        // 从String不同节点获取NpcID
        private void getIDFromString5(Wz_Node node)
        {
            Match match5 = Regex.Match(node.FullPathToFile, @"^String\\Npc.img\\(\d+).*");
            if (match5.Success)
            {
                string NpcID = match5.Groups[1].ToString().PadLeft(7, '0');
                if (!npcTooltipInfo.Contains(NpcID) && NpcID != null)
                {
                    npcTooltipInfo.Add(NpcID);
                }
            }
        }

        // 从String不同节点获取NpcID
        private void getIDFromString6(Wz_Node node)
        {
            Match match6 = Regex.Match(node.FullPathToFile, @"^Item\\Special.img\\0910.img\\(\d+)\\name");
            if (match6.Success)
            {
                string cashID = match6.Groups[1].ToString();
                if (!cashTooltipInfo.Contains(cashID) && cashID != null)
                {
                    cashTooltipInfo.Add(cashID);
                }
            }
        }

        //从String不同节点获取MapID
        private void getIDFromString7(Wz_Node node)
        {
            Match match7 = Regex.Match(node.FullPathToFile, @"^String\\Map.img\\.*\\(\d+)\\(streetName|mapName|mapDesc|help\d*)");
            if (match7.Success)
            {
                string mapID = match7.Groups[1].ToString();
                if (!mapTooltipInfo.Contains(mapID) && mapID != null)
                {
                    mapTooltipInfo.Add(mapID);
                }
            }
        }

        private void getIDFromString8(Wz_Node node)
        {
            if (node == null) return; // 변경은 확인하지 않음 // 추가,삭제만 확인
            Match match8 = Regex.Match(node.FullPathToFile, @"^Quest\\QuestInfo.img\\(\d+).*\\.*");
            if (!match8.Success)
            {
                match8 = Regex.Match(node.FullPathToFile, @"^Quest\\QuestInfo.img\\(\d+)$");
            }

            if (!match8.Success)
            {
                match8 = Regex.Match(node.FullPathToFile, @"^Quest\\QuestData\\(\d+).img\\(QuestInfo|Check|Act)\\.*");
            }

            if (!match8.Success)
            {
                match8 = Regex.Match(node.FullPathToFile, @"^Quest\\QuestData\\(\d+).img$");
            }
            if (match8.Success)
            {
                var questID = match8.Groups[1].Value;

                if (questID != null)
                {
                    if (!questTooltipInfo.Contains(questID))
                    {
                        questTooltipInfo.Add(questID);
                    }
                }
            }
        }

        private void getIDFromString9(Wz_Node node)
        {
            if (node == null) return;

            Match match = Regex.Match(node.FullPathToFile, @"^Etc\\Achievement\\AchievementData\\(\d+).img$");

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\Achievement\\AchievementData\\(\d+).img\\info\\(name|desc|difficulty|score|mainCategory).*$");
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\Achievement\\AchievementData\\(\d+).img\\mission\\\d+\\(name).*$");
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\Achievement\\AchievementData\\(\d+).img\\reward\\0\\(desc).*$");
            }

            if (match.Success)
            {
                var achvID = match.Groups[1].Value;

                if (achvID != null)
                {
                    if (!achievementTooltipInfo.Contains(achvID))
                    {
                        achievementTooltipInfo.Add(achvID);
                    }
                }
            }
        }

        // 确认现金道具价格变更
        private void CompareCommodities()
        {
            var commodities_new = CharaSimLoader.LoadedCommodityPricesByItemId[0];
            var commodities_old = CharaSimLoader.LoadedCommodityPricesByItemId[1];

            var key_new = commodities_new.Keys;
            var key_old = commodities_old.Keys;

            var added = key_new.Except(key_old).ToList();
            var removed = key_old.Except(key_new).ToList();
            List<int> ids = new List<int>();
            ids.AddRange(added);
            ids.AddRange(removed);

            var common = key_new.Intersect(key_old);
            foreach (var id in common)
            {
                if (commodities_new.TryGetValue(id, out var commodity_new) && commodities_old.TryGetValue(id, out var commodity_old) && CommodityPriceChanged(commodity_new, commodity_old))
                {
                    ids.Add(id);
                }
            }

            foreach (var id in ids)
            {
                if (id >= 2000000 && saveItemTooltip) // item
                {
                    if (!itemTooltipInfo.Contains(id.ToString()))
                    {
                        itemTooltipInfo.Add(id.ToString());
                    }
                }
                else if (saveEqpTooltip) // eqp
                {
                    if (!eqpTooltipInfo.Contains(id.ToString()))
                    {
                        eqpTooltipInfo.Add(id.ToString());
                    }
                }
            }
        }

        private bool CommodityPriceChanged(IReadOnlyList<CommodityPriceInfo> commodity_new, IReadOnlyList<CommodityPriceInfo> commodity_old)
        {
            if (commodity_new.Count != commodity_old.Count)
                return true;

            for (int i = 0; i < commodity_new.Count; i++) // Sorted in CharaSimLoader
            {
                if (commodity_new[i] != commodity_old[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void CompareImg(Wz_Image imgNew, Wz_Image imgOld, string imgName, string anchorName, string menuAnchorName, string outputDir, StreamWriter sw)
        {
            StateDetail = "img构成分析中";
            if (!imgNew.TryExtract() || !imgOld.TryExtract())
                return;
            StateDetail = "正在比较img";
            List<CompareDifference> diffList = new List<CompareDifference>(Comparer.Compare(imgNew.Node, imgOld.Node));
            StringBuilder sb = new StringBuilder();
            int[] count = new int[3];
            StateDetail = "总共发现" + diffList.Count + "个变更事项，合算中";
            foreach (var diff in diffList)
            {
                int idx = -1;
                string col0 = null;
                switch (diff.DifferenceType)
                {
                    case DifferenceType.Changed:
                        idx = 0;
                        col0 = diff.NodeNew.FullPath;
                        break;
                    case DifferenceType.Append:
                        idx = 1;
                        col0 = diff.NodeNew.FullPath;
                        break;
                    case DifferenceType.Remove:
                        idx = 2;
                        col0 = diff.NodeOld.FullPath;
                        break;
                }
                sb.AppendFormat("<tr class=\"r{0}\">", idx);
                sb.AppendFormat("<td>{0}</td>", col0 ?? " ");
                sb.AppendFormat("<td>{0}</td>", OutputNodeValue(col0, diff.NodeNew, 0, outputDir) ?? " ");
                sb.AppendFormat("<td>{0}</td>", OutputNodeValue(col0, diff.NodeOld, 1, outputDir) ?? " ");
                sb.AppendLine("</tr>");
                count[idx]++;

                // 变更的技能Tooltip处理
                if (saveSkillTooltip)
                {
                    if (imgName.StartsWith("Skill") || imgName.StartsWith("String"))
                    {
                        getIDFromSkill(diff.NodeNew, idx == 0 ? true : false);
                        getIDFromSkill(diff.NodeOld, idx == 0 ? true : false);
                    }
                    if (imgName.StartsWith("Character\\00002000.img"))
                    {
                        GetActionChanges(diff.NodeNew, idx == 0 ? true : false);
                        GetActionChanges(diff.NodeOld, idx == 0 ? true : false);
                    }
                }
                // 变更的道具Tooltip处理
                if (saveItemTooltip && outputDir.Contains("Item"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromItem(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromItem(diff.NodeOld);
                    }
                }
                if (saveItemTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString2(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString2(diff.NodeOld);
                    }
                }
                // 变更的装备Tooltip处理
                if (saveEqpTooltip && outputDir.Contains("Character"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromChar(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromChar(diff.NodeOld);
                    }
                }
                if (saveEqpTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString3(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString3(diff.NodeOld);
                    }
                }
                //变更的地图Tooltip处理
                if (saveMapTooltip && outputDir.Contains("Map"))
                {
                    if (diff.NodeNew != null)
                    {
                        GetIDFromMap(diff.NodeNew, true);
                    }
                    if (diff.NodeOld != null)
                    {
                        GetIDFromMap(diff.NodeOld, false);
                    }
                }
                if (saveMapTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString7(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString7(diff.NodeOld);
                    }
                }
                // 变更的怪物Tooltip处理
                if (saveMobTooltip && outputDir.Contains("Mob"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromMob(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromMob(diff.NodeOld);
                    }
                }
                if (saveMobTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString4(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString4(diff.NodeOld);
                    }
                }
                // 变更的Npc Tooltip处理
                if (saveNpcTooltip && outputDir.Contains("Npc"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromNpc(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromNpc(diff.NodeOld);
                    }
                }
                if (saveNpcTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString5(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString5(diff.NodeOld);
                    }
                }
                // 变更的礼包Tooltip处理
                if (saveCashTooltip && outputDir.Contains("Item"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromCash(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromCash(diff.NodeOld);
                    }
                }
                if (saveCashTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString6(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString6(diff.NodeOld);
                    }
                }
                // 变更的任务Tooltip处理
                if (saveQuestTooltip && (outputDir.Contains("Quest")))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString8(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString8(diff.NodeOld);
                    }
                }
                // 变更的成就Tooltip处理
                if (saveAchievementTooltip && (outputDir.Contains("Etc") && imgName.Contains("Achievement") && !imgName.Contains("_Canvas")))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString9(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString9(diff.NodeOld);
                    }
                }
            }

            StateDetail = "正在处理档案";
            bool noChange = diffList.Count <= 0;
            sw.WriteLine("<table class=\"img{0}\">", noChange ? " noChange" : "");
            sw.WriteLine("<tr><th colspan=\"3\"><a name=\"{1}\">{0}</a> 变更:{2} 新增:{3} 删除:{4}</th></tr>",
                imgName, anchorName, count[0], count[1], count[2]);
            sw.WriteLine(sb.ToString());
            sw.WriteLine("<tr><td colspan=\"3\"><a href=\"#{1}\">{0}</a></td></tr>", "回到顶部", menuAnchorName);
            sw.WriteLine("</table>");
            imgNew.Unextract();
            imgOld.Unextract();
            sb = null;
        }

        private void OutputImg(Wz_Image img, DifferenceType diffType, string imgName, string anchorName, string menuAnchorName, string outputDir, StreamWriter sw)
        {
            StateDetail = "img构成分析中";
            if (!img.TryExtract())
                return;

            int idx = 0; ;
            switch (diffType)
            {
                case DifferenceType.Changed:
                    idx = 0;
                    break;
                case DifferenceType.Append:
                    idx = 1;
                    break;
                case DifferenceType.Remove:
                    idx = 2;
                    break;
            }
            Action<Wz_Node> fnOutput = null;
            fnOutput = node =>
            {
                if (node != null)
                {
                    string fullPath = node.FullPath;
                    sw.Write("<tr class=\"r{0}\">", idx);
                    sw.Write("<td>{0}</td>", fullPath ?? " ");
                    sw.Write("<td>{0}</td>", OutputNodeValue(fullPath, node, 0, outputDir) ?? " ");
                    sw.WriteLine("</tr>");

                    if (saveSkillTooltip) // 变更技能Tooltip处理
                    {
                        if (imgName.StartsWith("Skill") || imgName.StartsWith("String"))
                        {
                            getIDFromSkill(node, idx == 0 ? true : false);
                        }
                        if (imgName.StartsWith("Character\\00002000.img"))
                        {
                            GetActionChanges(node, idx == 0 ? true : false);
                        }
                    }
                    if (saveItemTooltip && outputDir.Contains("Item")) // 变更道具Tooltip处理
                    {
                        getIDFromItem(node);
                    }
                    if (saveEqpTooltip && outputDir.Contains("Character")) // 变更装备Tooltip处理
                    {
                        getIDFromChar(node);
                    }
                    if (saveMapTooltip && outputDir.Contains("Map")) // 变更地图Tooltip处理
                    {
                        GetIDFromMap(node, idx == 0 ? true : false);
                    }
                    if (saveMobTooltip && outputDir.Contains("Mob")) // 变更装备Tooltip处理
                    {
                        getIDFromMob(node);
                    }
                    if (saveNpcTooltip && outputDir.Contains("Npc")) // 变更Npc Tooltip处理
                    {
                        getIDFromNpc(node);
                    }
                    if (saveCashTooltip && outputDir.Contains("Item")) // 变更礼包Tooltip处理
                    {
                        getIDFromItem(node);
                    }

                    if (node.Nodes.Count > 0)
                    {
                        foreach (Wz_Node child in node.Nodes)
                        {
                            fnOutput(child);
                        }
                    }
                }
            };

            StateDetail = "正在处理img构成";
            sw.WriteLine("<table class=\"img\">");
            sw.WriteLine("<tr><th colspan=\"2\"><a name=\"{1}\">{0}</a></th></tr>", imgName, anchorName);
            fnOutput(img.Node);
            sw.WriteLine("<tr><td colspan=\"2\"><a href=\"#{1}\">{0}</a></td></tr>", "回到顶部", menuAnchorName);
            sw.WriteLine("</table>");
            img.Unextract();
        }

        protected virtual string OutputNodeValue(string fullPath, Wz_Node value, int col, string outputDir)
        {
            if (value == null)
                return null;

            Wz_Node linkNode;
            if ((linkNode = value.GetLinkedSourceNode(path => PluginBase.PluginManager.FindWz(path, value.GetNodeWzFile()))) != value)
            {
                return "(link) " + OutputNodeValue(fullPath, linkNode, col, outputDir);
            }

            switch (value.Value)
            {
                case Wz_Png png:
                    if (OutputPng)
                    {
                        char[] invalidChars = Path.GetInvalidFileNameChars();
                        string colName = col == 0 ? "new" : (col == 1 ? "old" : col.ToString());
                        string fileName = fullPath.Replace('\\', '.');
                        string suffix = "_" + colName + ".png";
                        string canvas = "_Canvas";

                        for (int i = 0; i < invalidChars.Length; i++)
                        {
                            fileName = fileName.Replace(invalidChars[i], '_');
                        }
                        if (outputDir.Length + fileName.Length > 240)
                        {
                            fileName = fileName.Substring(0, 40) + "_" + ToHexString(MD5Hash(fileName)).Substring(0, 8);
                        }

                        fileName = fileName + suffix;
                        string outputDirName = new DirectoryInfo(outputDir).Name;
                        bool isCanvas = fileName.Contains(canvas);
                        if (isCanvas)
                        {
                            if (this.Comparer.ResolvePngLink)
                            {
                                fileName = fileName.Replace(canvas + ".", string.Empty);
                            }
                            else
                            {
                                outputDir = Path.Combine(outputDir, canvas);
                                if (!Directory.Exists(outputDir))
                                {
                                    Directory.CreateDirectory(outputDir);
                                }
                            }
                        }
                        // Skip unparseable content
                        try
                        {
                            using (Bitmap bmp = png.ExtractPng())
                            {
                                bmp.Save(Path.Combine(outputDir, fileName), System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!FailToExportNodes.ContainsKey(colName + ": " + fullPath.Replace('\\', '/')))
                            {
                                FailToExportNodes.Add(colName + ": " + fullPath.Replace('\\', '/'), ex.Message);
                            }
                            else
                            {
                                FailToExportNodes[colName + ": " + fullPath.Replace('\\', '/')] = ex.Message;
                            }
                            return string.Format("无法解析的PNG数据 {0} bytes", png.DataLength);
                        }
                        return string.Format("<img src=\"{0}/{1}\" />", (isCanvas && !this.Comparer.ResolvePngLink) ? Path.Combine(outputDirName, canvas) : outputDirName, WebUtility.UrlEncode(fileName));
                    }
                    else
                    {
                        return string.Format("PNG {0}*{1} ({2}B)", png.Width, png.Height, png.DataLength);
                    }

                case Wz_Uol uol:
                    return "(uol) " + uol.Uol;

                case Wz_Vector vector:
                    return string.Format("({0}, {1})", vector.X, vector.Y);

                case Wz_Sound sound:
                    if (OutputPng)
                    {
                        char[] invalidChars = Path.GetInvalidFileNameChars();
                        string colName = col == 0 ? "new" : (col == 1 ? "old" : col.ToString());
                        string filePath = fullPath.Replace('\\', '.') + "_" + colName + ".mp3";

                        for (int i = 0; i < invalidChars.Length; i++)
                        {
                            filePath = filePath.Replace(invalidChars[i].ToString(), null);
                        }

                        try
                        {
                            byte[] mp3 = sound.ExtractSound();
                            if (mp3 != null)
                            {
                                FileStream fileStream = new FileStream(Path.Combine(outputDir, filePath), FileMode.Create, FileAccess.Write);
                                fileStream.Write(mp3, 0, mp3.Length);
                                fileStream.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!FailToExportNodes.ContainsKey(colName + ": " + fullPath.Replace('\\', '/')))
                            {
                                FailToExportNodes.Add(colName + ": " + fullPath.Replace('\\', '/'), ex.Message);
                            }
                            else
                            {
                                FailToExportNodes[colName + ": " + fullPath.Replace('\\', '/')] = ex.Message;
                            }
                            return string.Format("无法解析的音频数据 {0} bytes", sound.DataLength);
                        }
                        return string.Format("<audio controls src=\"{0}\" type=\"audio/mpeg\">audio {1} ms\n</audio>", Path.Combine(new DirectoryInfo(outputDir).Name, filePath), sound.Ms);
                    }
                    else
                    {
                        return string.Format("Audio {0}ms", sound.Ms);
                    }

                case Wz_Convex convex:
                    return string.Format("convex {0}", string.Join(" ", convex.Points.Select(vec => $"({vec.X},{vec.Y})")));

                case Wz_RawData rawData:
                    return string.Format("rawdata {0} bytes", rawData.Length);

                case Wz_Video video:
                    return string.Format("video {0} bytes", video.Length);

                case Wz_Image _:
                    return "{ img }";

                default:
                    return string.Format("<span title=\"{0}\">{1}</span>", value.GetType().Name, WebUtility.HtmlEncode(Convert.ToString(value.Value)));
            }
        }

        // 输出style.css
        public virtual void CreateStyleSheet(string outputDir)
        {
            string path = Path.Combine(outputDir, "style.css");
            if (File.Exists(path))
                return;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            if (EnableDarkMode)
            {

                sw.WriteLine("body { font-size:12px; background-color:black; color:white; }");
                sw.WriteLine("a { color:white; }");
                sw.WriteLine("p.wzf { }");
                sw.WriteLine("table, tr, th, td { border:1px solid #ff8000; border-collapse:collapse; }");
                sw.WriteLine("table { margin-bottom:16px; }");
                sw.WriteLine("th { text-align:left; }");
                sw.WriteLine("table.lst0 { }");
                sw.WriteLine("table.lst1 { }");
                sw.WriteLine("table.lst2 { }");
                sw.WriteLine("table.img { }");
                sw.WriteLine("table.img tr.r0 { background-color:#003049; }");
                sw.WriteLine("table.img tr.r1 { background-color:#000000; }");
                sw.WriteLine("table.img tr.r2 { background-color:#462306; }");
                sw.WriteLine("table.img.noChange { display:none; }");
            }
            else
            {
                sw.WriteLine("body { font-size:12px; }");
                sw.WriteLine("p.wzf { }");
                sw.WriteLine("table, tr, th, td { border:1px solid #ff8000; border-collapse:collapse; }");
                sw.WriteLine("table { margin-bottom:16px; }");
                sw.WriteLine("th { text-align:left; }");
                sw.WriteLine("table.lst0 { }");
                sw.WriteLine("table.lst1 { }");
                sw.WriteLine("table.lst2 { }");
                sw.WriteLine("table.img { }");
                sw.WriteLine("table.img tr.r0 { background-color:#fff4c4; }");
                sw.WriteLine("table.img tr.r1 { background-color:#ebf2f8; }");
                sw.WriteLine("table.img tr.r2 { background-color:#ffffff; }");
                sw.WriteLine("table.img.noChange { display:none; }");
            }
            sw.Flush();
            sw.Close();
        }

        private static byte[] MD5Hash(string text)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
        }

        private static string ToHexString(byte[] inArray)
        {
            StringBuilder hex = new StringBuilder(inArray.Length * 2);
            foreach (byte b in inArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private static string RemoveInvalidFileNameChars(string fileName)
        {
            string invalidChars = new string(System.IO.Path.GetInvalidFileNameChars());
            string regexPattern = $"[{Regex.Escape(invalidChars)}]";
            return Regex.Replace(fileName, regexPattern, "_");
        }

        private static string GetBitmapHash(Bitmap bitmap)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Lock bits for direct memory access
                BitmapData bmpData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

                try
                {
                    // Get the raw pixel data
                    int byteCount = Math.Abs(bmpData.Stride) * bitmap.Height;
                    byte[] pixelBuffer = new byte[byteCount];
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelBuffer, 0, byteCount);

                    // Compute the hash from pixel data
                    byte[] hashBytes = sha256.ComputeHash(pixelBuffer);

                    // Convert hash to string
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                }
                finally
                {
                    // Unlock bits
                    bitmap.UnlockBits(bmpData);
                }
            }
        }

        private bool isKMSNode(Wz_Node node)
        {
            if (node == null)
                return false;
            if (node.FullPathToFile.StartsWith("Character"))
            {
                string[] gearNodePath = node.FullPathToFile.Split('\\');
                string gearImgStr = gearNodePath.LastOrDefault(part => part.EndsWith(".img"));
                if (gearImgStr == null)
                {
                    return false;
                }
                else
                {
                    if (Int32.TryParse(gearImgStr.Replace(".img", ""), out int gearID))
                    {
                        if (KMSContentID.ContainsKey("Item"))
                        {
                            return KMSContentID["Item"].Contains(gearID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (node.FullPathToFile.StartsWith("Effect"))
            {
                string[] effectNodePath = node.FullPathToFile.Split('\\');
                string effectImgStr = effectNodePath.LastOrDefault(part => part.EndsWith(".img"));
                if (KMSComponentDict.ContainsKey("Effect"))
                {
                    return KMSComponentDict["Effect"].Contains(effectImgStr);
                }
                else
                {
                    return false;
                }
            }
            else if (node.FullPathToFile.StartsWith("Item"))
            {
                string[] itemNodePath = node.FullPathToFile.Split('\\');
                int itemBaseImgIndex = Array.FindIndex(itemNodePath, s => s.EndsWith(".img"));
                if (itemBaseImgIndex != -1 && itemBaseImgIndex < itemNodePath.Length - 1)
                {
                    if (Int32.TryParse(itemNodePath[itemBaseImgIndex + 1], out int itemID))
                    {
                        if (KMSContentID.ContainsKey("Item"))
                        {
                            return KMSContentID["Item"].Contains(itemID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (node.FullPathToFile.StartsWith("Map"))
            {
                string[] mapNodePath = node.FullPathToFile.Split('\\');
                string mapImgStr = mapNodePath.LastOrDefault(part => part.EndsWith(".img"));
                if (mapImgStr == null)
                {
                    return false;
                }
                else
                {
                    if (Int32.TryParse(mapImgStr.Replace(".img", ""), out int mapID))
                    {
                        if (KMSContentID.ContainsKey("Map"))
                        {
                            return KMSContentID["Map"].Contains(mapID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (mapNodePath.Length > 2)
                        {
                            switch (mapNodePath[1])
                            {
                                case "Back":
                                    if (KMSComponentDict.ContainsKey("MapBack"))
                                    {
                                        return KMSComponentDict["MapBack"].Contains(mapImgStr);
                                    }
                                    break;
                                case "Obj":
                                    if (KMSComponentDict.ContainsKey("MapObj"))
                                    {
                                        return KMSComponentDict["MapObj"].Contains(mapImgStr);
                                    }
                                    break;
                                case "Tile":
                                    if (KMSComponentDict.ContainsKey("MapTile"))
                                    {
                                        return KMSComponentDict["MapTile"].Contains(mapImgStr);
                                    }
                                    break;
                                case "WorldMap":
                                    if (KMSComponentDict.ContainsKey("MapWorldMap"))
                                    {
                                        return KMSComponentDict["MapWorldMap"].Contains(mapImgStr);
                                    }
                                    break;
                            }
                        }
                        return false;
                    }
                }
            }
            else if (node.FullPathToFile.StartsWith("Mob"))
            {
                string[] mobNodePath = node.FullPathToFile.Split('\\');
                if (mobNodePath.Contains("BossPattern"))
                {
                    string bossPatternImgStr = mobNodePath.LastOrDefault(part => part.EndsWith(".img"));
                    if (bossPatternImgStr == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (KMSComponentDict.ContainsKey("MobBossPattern"))
                        {
                            return KMSComponentDict["MobBossPattern"].Contains(bossPatternImgStr);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                string mobImgStr = mobNodePath.LastOrDefault(part => part.EndsWith(".img"));
                if (mobImgStr == null)
                {
                    return false;
                }
                else
                {
                    if (Int32.TryParse(mobImgStr.Replace(".img", ""), out int mobID))
                    {
                        if (KMSContentID.ContainsKey("Mob"))
                        {
                            return KMSContentID["Mob"].Contains(mobID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (node.FullPathToFile.StartsWith("Npc"))
            {
                string[] npcNodePath = node.FullPathToFile.Split('\\');
                string npcImgStr = npcNodePath.LastOrDefault(part => part.EndsWith(".img"));
                if (npcImgStr == null)
                {
                    return false;
                }
                else
                {
                    if (Int32.TryParse(npcImgStr.Replace(".img", ""), out int npcID))
                    {
                        if (KMSContentID.ContainsKey("Npc"))
                        {
                            return KMSContentID["Npc"].Contains(npcID);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (node.FullPathToFile.StartsWith("Skill"))
            {
                string skillNodePath = node.FullPathToFile.Replace("_Canvas\\", "");
                Match SkillMatch1 = Regex.Match(skillNodePath, @"^Skill\\\d+\\\d+\.img\\(\d+)$");
                if (SkillMatch1.Success)
                {
                    return false;
                }
                else
                {
                    string baseSkillID = skillNodePath.Split('\\')[1].Replace(".img", "");
                    if (Int32.TryParse(baseSkillID, out int baseSkillIDInt))
                    {
                        switch (baseSkillIDInt / 1000)
                        {
                            case 0:
                                return !(new int[] { 508, 570, 571, 572 }.Contains(baseSkillIDInt)); // ジェット
                            case 4: // 暁の陣
                            case 11: // ビーストテイマー
                            case 12: // アニメコラボ
                            case 17: // 江湖
                            case 18: // Shine
                                return false;
                            case 40: // 5次スキル
                            case 50: // 6次強化コア
                            case 800: // イベントスキル
                                if (skillNodePath.Split('\\').Length < 4)
                                {
                                    return false;
                                }
                                else
                                {
                                    if (Int32.TryParse(skillNodePath.Split('\\')[3], out int skillID))
                                    {
                                        return isKMSSkillID(skillID);
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            default:
                                return true;
                        }
                    }
                    else if (baseSkillID == "Dragon")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private bool isGodChangseopNode(Wz_Node node)
        {
            if (node == null)
                return false;
            string[] nodePath = node.FullPathToFile.Split('\\');
            string imgStr = nodePath.LastOrDefault(part => part.EndsWith(".img"));
            if (string.IsNullOrEmpty(imgStr)) return false;
            return imgStr.EndsWith("_.img");
        }
        private bool isKMSSkillID(int skillID)
        {
            switch (skillID / 10000000)
            {
                case 0:
                    return !(new int[] { 508, 570, 571, 572 }.Contains((int)skillID / 10000)); // ジェット
                case 4: // 晓之阵
                case 11: // ビーストテイマー
                case 12: // 动漫合作
                case 17: // 江湖
                case 18: // Shine
                    return false;
                case 40: // 5转技能
                case 50: // 6转强化核心
                    if (FifthJobSkillToJobID.ContainsKey(skillID))
                    {
                        bool KMSClassOnly = true;
                        foreach (int jobID in FifthJobSkillToJobID[skillID])
                        {
                            KMSClassOnly = KMSClassOnly &&
                                           !(jobID == 572 || jobID / 1000 == 4 || jobID / 1000 == 11 ||
                                            jobID / 1000 == 12 || jobID / 1000 == 17 || jobID / 1000 == 18);
                        }
                        return KMSClassOnly;
                    }
                    else
                    {
                        return KMSContentID["Skill"].Contains(skillID);
                    }
                case 8:
                    if (KMSContentID.ContainsKey("Skill"))
                    {
                        return KMSContentID["Skill"].Contains(skillID);
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return true;
            }
        }
    }
}
