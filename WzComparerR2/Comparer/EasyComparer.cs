using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using DevComponents.DotNetBar;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using WzComparerR2.CharaSim;
using WzComparerR2.CharaSimControl;
using WzComparerR2.Common;
using WzComparerR2.Config;
using WzComparerR2.PluginBase;
using WzComparerR2.WzLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WzComparerR2.Comparer
{
    public class EasyComparer
    {
        public EasyComparer()
        {
            this.Comparer = new WzFileComparer();
        }
        private Wz_Node[] WzNewOld { get; set; } = new Wz_Node[2];
        private Wz_File[] WzFileNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] StringWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] ItemWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] EtcWzNewOld { get; set; } = new Wz_File[2];
        private Wz_File[] QuestWzNewOld { get; set; } = new Wz_File[2];
        private StringLinker[] StringLinkerNewOld { get; set; } = new StringLinker[2];
        private SortedSet<int> OutputGearTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputItemTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputMapTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputMobTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputNpcTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputQuestTooltipIDs { get; set; } = new SortedSet<int>();
        private SortedSet<int> OutputAchvTooltipIDs { get; set; } = new SortedSet<int>();
        private HashSet<string> OutputSkillTooltipIDs { get; set; } = new HashSet<string>();
        private HashSet<string> OutputPerJobSkillTooltipIDs { get; set; } = new HashSet<string>();
        private List<string> cashTooltipInfo = new List<string>();
        private Dictionary<string, Dictionary<string, List<string>>> diffHtml = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> diffPerJobSkillTags { get; set; } = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffSkillTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffCashTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffMapTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffAchvTags = new Dictionary<string, List<string>>();
        private Dictionary<int, HashSet<string>> DiffMobTags { get; set; } = new Dictionary<int, HashSet<string>>();
        private Dictionary<string, List<int>> KMSContentID = new Dictionary<string, List<int>>();
        private Dictionary<string, List<string>> KMSComponentDict = new Dictionary<string, List<string>>();
        private Dictionary<int, List<int>> FifthJobSkillToJobID = new Dictionary<int, List<int>>();
        public Dictionary<string, string> FailToExportNodes = new Dictionary<string, string>();
        public Dictionary<string, string> FailToExportTooltips { get; private set; } = new Dictionary<string, string>();
        private Dictionary<string, HashSet<int>> ChangedActions { get; set; } = new Dictionary<string, HashSet<int>>();

        public WzFileComparer Comparer { get; protected set; }
        private string stateInfo;
        private string stateDetail;
        public bool OutputPng { get; set; }
        public bool OutputAddedImg { get; set; }
        public bool OutputRemovedImg { get; set; }
        public List<Color> ColorTable { get; set; }
        public bool saveSkillTooltip { get; set; }
        public bool saveItemTooltip { get; set; }
        public bool saveCashTooltip { get; set; }
        public bool saveEqpTooltip { get; set; }
        public bool saveMobTooltip { get; set; }
        public bool saveNpcTooltip { get; set; }
        public bool saveQuestTooltip { get; set; }
        public bool saveAchievementTooltip { get; set; }
        public bool saveMapTooltip { get; set; }
        public bool OutputWorldArchives { get; set; }
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
        public bool ShowAllIllustAtOnce { get; set; }
        public bool LocatePetEquip { get; set; }
        public bool EnableBucket { get; set; }
        public string bucketPath { get; set; }
        public int QuestState { get; set; }
        public Dictionary<string, bool> SelectedNodes { get; set; }

        public bool OutputTooltips
        {
            get
            {
                return saveSkillTooltip || saveItemTooltip || saveEqpTooltip || saveMapTooltip || saveMobTooltip || saveNpcTooltip || saveQuestTooltip || saveAchievementTooltip || saveCashTooltip;
            }
            set
            {
                saveSkillTooltip = saveItemTooltip = saveEqpTooltip = saveMapTooltip = saveMobTooltip = saveNpcTooltip = saveQuestTooltip = saveAchievementTooltip = saveCashTooltip = value;
            }
        }

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

        public string StateUpload
        {
            get { return StateUpload; }
            set
            {
                StateUpload = value;
                this.OnStateUploadChanged(EventArgs.Empty);
            }
        }
        public event EventHandler StateInfoChanged;
        public event EventHandler StateDetailChanged;
        public event EventHandler StateUploadChanged;
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

        protected virtual void OnStateUploadChanged(EventArgs e)
        {
            if (this.StateUploadChanged != null)
                this.StateUploadChanged(this, e);
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

                if (SelectedNodes.TryGetValue("String", out bool s) && !s)
                {
                    OutputTooltips = false;
                }
                if (OutputTooltips || SkipKMSContent)
                {
                    this.WzNewOld[0] = fileNew.Node;
                    this.WzNewOld[1] = fileOld.Node;
                    this.WzFileNewOld[0] = fileNew.Node.GetNodeWzFile();
                    this.WzFileNewOld[1] = fileOld.Node.GetNodeWzFile();

                    StateInfo = "正在导入新旧版本StringLinker...";
                    for (int i = 0; i < 2; i++)
                    {
                        this.StringLinkerNewOld[i] = new StringLinker();
                        this.StringLinkerNewOld[i].Load(WzNewOld[i].FindNodeByPath("String").GetNodeWzFile(),
                            WzNewOld[i].FindNodeByPath("Item").GetNodeWzFile(),
                            WzNewOld[i].FindNodeByPath("Etc").GetNodeWzFile(),
                            WzNewOld[i].FindNodeByPath("Quest").GetNodeWzFile());
                    }
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
                                    CompareImg(imgNew, imgOld, diff.NodeNew.FullPathToFile, anchorName, menuAnchorName, srcDirPath, sw, fileOld[0].GetMergedVersion(), fileNew[0].GetMergedVersion());
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
            if (saveSkillTooltip && type.ToString() == "String" && OutputSkillTooltipIDs != null)
            {
                if (!Directory.Exists(skillTooltipPath))
                {
                    Directory.CreateDirectory(skillTooltipPath);
                }
                SaveSkillTooltip(skillTooltipPath);
            }
            if (saveSkillTooltip && type.ToString() == "String" && OutputPerJobSkillTooltipIDs != null)
            {
                if (!Directory.Exists(skillTooltipPath))
                {
                    Directory.CreateDirectory(skillTooltipPath);
                }
                savePerJobSkillTooltip(skillTooltipPath);
            }
            if (saveItemTooltip && type.ToString() == "String" && OutputItemTooltipIDs != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                SaveItemTooltip(itemTooltipPath);
            }
            if (saveEqpTooltip && type.ToString() == "String" && OutputGearTooltipIDs != null)
            {
                if (!Directory.Exists(eqpTooltipPath))
                {
                    Directory.CreateDirectory(eqpTooltipPath);
                }
                SaveGearTooltip(eqpTooltipPath);
            }
            if (saveMapTooltip && type.ToString() == "String" && OutputMapTooltipIDs != null)
            {
                if (!Directory.Exists(mapTooltipPath))
                {
                    Directory.CreateDirectory(mapTooltipPath);
                }
                SaveMapTooltip(mapTooltipPath);
            }
            if (saveMobTooltip && type.ToString() == "String" && OutputMobTooltipIDs != null)
            {
                if (!Directory.Exists(mobTooltipPath))
                {
                    Directory.CreateDirectory(mobTooltipPath);
                }
                SaveMobTooltip(mobTooltipPath);
            }
            if (saveNpcTooltip && type.ToString() == "String" && OutputNpcTooltipIDs != null)
            {
                if (!Directory.Exists(npcTooltipPath))
                {
                    Directory.CreateDirectory(npcTooltipPath);
                }
                SaveNPCTooltip(npcTooltipPath);
            }
            if (saveQuestTooltip && type.ToString() == "String" && OutputQuestTooltipIDs != null)
            {
                if (!Directory.Exists(questTooltipPath))
                {
                    Directory.CreateDirectory(questTooltipPath);
                }
                SaveQuestTooltip(questTooltipPath);
            }
            if (saveAchievementTooltip && type.ToString() == "String" && OutputAchvTooltipIDs != null)
            {
                if (!Directory.Exists(achvTooltipPath))
                {
                    Directory.CreateDirectory(achvTooltipPath);
                }
                SaveAchvTooltip(achvTooltipPath);
            }
            if (saveCashTooltip && type.ToString() == "String" && cashTooltipInfo != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                SaveCashTooltip(itemTooltipPath);
            }
            //for (var i = 0; i < 2; i++)
            //{
            //    this.WzNewOld[i] = null;
            //    this.WzFileNewOld[i] = null;
            //    this.StringLinkerNewOld[i] = null;
            //}
            if (EnableBucket && (saveCashTooltip || saveEqpTooltip || saveItemTooltip || saveMapTooltip || saveMobTooltip || saveNpcTooltip || saveSkillTooltip || saveQuestTooltip || saveAchievementTooltip))
            {
                uploadToBucket(outputDir);
            }
        }

        private void uploadToBucket(string outputDir)
        {
            UploadObject.UploadObject obj = new UploadObject.UploadObject();
            foreach (var category in diffHtml)
            {
                string categoryName = category.Key;
                Dictionary<string, List<string>> changes = category.Value;
                if (changes.Values.All(list => list.Count == 0)) continue;
                string TooltipPath = Path.Combine(outputDir, categoryName + "Tooltip");
                string objectDir = $"{categoryName}Tooltip/{bucketPath}";
                if (!obj.DoesObjectExist(objectDir))
                {
                    StateDetail = $"正在创建对象路径：{objectDir}...";
                    obj.CreateDir(objectDir);
                }
                foreach (var changeType in changes)
                {
                    List<string> itemList = changeType.Value;
                    foreach (string item in itemList)
                    {
                        StateDetail = $"正在上传对象：{categoryName}Tooltip/{bucketPath}{item}";
                        string localFilePath = Path.Combine(TooltipPath, item);
                        string objectKey = $"{categoryName}Tooltip/{bucketPath}{item}";
                        if (File.Exists(localFilePath))
                        {
                            obj.PutObject(objectKey, localFilePath);
                        }
                    }
                }
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
                try
                {
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
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Action: " + action, ex.Message);
                }
            }
            ChangedActions.Clear();
        }

        // 变更技能Tooltip输出
        private void SaveSkillTooltip(string skillTooltipPath)
        {
            UpdateActionChanges();
            SkillTooltipRender2[] skillRenderNewOld = new SkillTooltipRender2[2];
            int count = 0;
            int allCount = OutputSkillTooltipIDs.Count;
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
            foreach (var skillID in OutputSkillTooltipIDs)
            {
                count++;
                StateInfo = string.Format("{0}/{1} 技能: {2}", count, allCount, skillID);
                StateDetail = "正在以Tooltip图像处理技能变更点...";

                bool[] isSkillNull = new bool[2] { false, false };

                if (SkipKMSContent && isKMSSkillID(Int32.Parse(skillID))) continue;

                string skillNodePath = int.Parse(skillID) / 10000000 == 8 ? String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 100, skillID) : String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 10000, skillID);
                if (int.Parse(skillID) / 10000 == 0) skillNodePath = String.Format(@"\000.img\skill\{0:D7}", skillID);
                int nullSkillIdx = 0;
                bool isPerJobVariableSkill = false;
                try
                {
                    // 绘制变更前技能Tooltip
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Skill skill = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]);

                        if (skill != null)
                        {
                            skill.Level = skill.MaxLevel;
                            isPerJobVariableSkill = skill.PerJobAttackInfo.Count > 0;
                            isSkillNull[i] = false; // 明确标记存在
                        }
                        else
                        {
                            isSkillNull[i] = true;
                            nullSkillIdx |= i + 1;
                        }
                        skillRenderNewOld[i].Skill = skill;
                    }
                    SaveTooltip(skillRenderNewOld[0], skillRenderNewOld[1], nullSkillIdx, skillTooltipPath, skillID, "Skill");
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Skill Tooltip: " + skillNodePath, ex.Message);
                }
            }
            OutputSkillTooltipIDs.Clear();
            diffSkillTags.Clear();
        }

        private void savePerJobSkillTooltip(string skillTooltipPath)
        {
            UpdateActionChanges();
            SkillTooltipRender2[] skillRenderNewOld = new SkillTooltipRender2[2];
            int count = 0;
            int allCount = OutputPerJobSkillTooltipIDs.Count;
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
            foreach (var skillID in OutputPerJobSkillTooltipIDs)
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

                    bool isPerJobVariableSkill = true;

                    // 变更前后Tooltip图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Skill skill = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]);

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

                        string imageName = Path.Combine(skillTooltipPath, categoryPath, "Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(targetJobId / 10000) ?? "其它") + "]_" + skillType + ".png");
                        diffHtml["Skill"][skillType].Add("Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(targetJobId) ?? "其它") + "]_" + skillType + ".png");
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
                    FailToExportTooltips.Add("Skill Tooltip(Per Job): " + skillID, ex.Message);
                }
            }
            OutputPerJobSkillTooltipIDs.Clear();
            diffPerJobSkillTags.Clear();
        }

        //  输出变更道具提示框
        private void SaveItemTooltip(string itemTooltipPath)
        {
            TooltipRender[] tooltipRenderNewOld = new TooltipRender[2];
            int count = 0;
            int allCount = OutputItemTooltipIDs.Count;
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                if (CharaSimConfig.Default.Misc.Enable22AniStyle)
                {
                    tooltipRenderNewOld[i] = new ItemTooltipRender3();
                    tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                    tooltipRenderNewOld[i].ShowObjectID = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).LinkRecipeInfo = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).LinkRecipeItem = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).ShowLevelOrSealed = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).CompareMode = true;
                    //tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).ShowLinkedTamingMob = this.ShowLinkedTamingMob;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).AllowFamiliarOutOfBounds = this.AllowFamiliarOutOfBounds;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).UseCTFamiliarRender = this.UseCTFamiliarUI;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).ShowApplicablePetEquip = this.LocatePetEquip;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).ShowCashPurchasePrice = CharaSimConfig.Default.Item.ShowPurchasePrice;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).LoadedCommoditiesSlot = i;
                    (tooltipRenderNewOld[i] as ItemTooltipRender3).CompareMode = true;
                }
                else
                {
                    tooltipRenderNewOld[i] = new ItemTooltipRender2();
                    tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                    tooltipRenderNewOld[i].ShowObjectID = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).LinkRecipeInfo = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).LinkRecipeItem = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).ShowLevelOrSealed = true;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).CompareMode = true;
                    //tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).ShowLinkedTamingMob = this.ShowLinkedTamingMob;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).AllowFamiliarOutOfBounds = this.AllowFamiliarOutOfBounds;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).UseCTFamiliarRender = this.UseCTFamiliarUI;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).ShowApplicablePetEquip = this.LocatePetEquip;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).ShowCashPurchasePrice = CharaSimConfig.Default.Item.ShowPurchasePrice;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).LoadedCommoditiesSlot = i;
                    (tooltipRenderNewOld[i] as ItemTooltipRender2).CompareMode = true;
                }
            }
            diffHtml["Item"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var itemID in OutputItemTooltipIDs)
            {
                StateInfo = string.Format("{0}/{1} 道具: {2}", ++count, allCount, itemID);
                StateDetail = "正在以Tooltip图像处理道具变更点...";
                string itemType = Item.GetItemType(itemID).ToString();
                string nodePath = (itemID / 10000 == 500) ? $@"{itemID:D7}.img"
                    : (itemID / 1000 == 3015) ? $@"{(itemID / 100):D6}.img\{itemID:D8}"
                    : (itemID / 10000 == 301) ? $@"{(itemID / 1000):D5}.img\{itemID:D8}"
                    : $@"{(itemID / 10000):D4}.img\{itemID:D8}";
                try
                {
                    if (SkipKMSContent && KMSContentID["Item"].Contains(itemID)) continue;
                    int nullItemIdx = 0;

                    // 变更前后提示框图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Item item = Item.CreateFromNode(PluginManager.FindWz($@"Item\{itemType}\{nodePath}", WzFileNewOld[i]), PluginManager.FindWz, WzFileNewOld[i]);

                        if (item == null)
                        {
                            nullItemIdx |= i + 1;
                        }
                        if (tooltipRenderNewOld[i] is ItemTooltipRender3)
                            (tooltipRenderNewOld[i] as ItemTooltipRender3).Item = item;
                        else
                            (tooltipRenderNewOld[i] as ItemTooltipRender2).Item = item;
                    }
                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullItemIdx, itemTooltipPath, itemID.ToString(), "Item");
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Item Tooltip 3: " + $@"Item\{itemType}\{nodePath}", ex.Message);
                }
            }
            OutputItemTooltipIDs.Clear();
        }

        // 变更装备Tooltip输出
        private void SaveGearTooltip(string eqpTooltipPath)
        {
            TooltipRender[] tooltipRenderNewOld = new TooltipRender[2];
            Wz_Node[] CharaWzNodeNewOld = new Wz_Node[2];
            int count3 = 0;
            int allCount3 = OutputGearTooltipIDs.Count;

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                if (CharaSimConfig.Default.Misc.Enable22AniStyle)
                {
                    tooltipRenderNewOld[i] = new GearTooltipRender22();
                    tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                    tooltipRenderNewOld[i].ShowObjectID = true;
                    tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                    (tooltipRenderNewOld[i] as GearTooltipRender22).ShowLevelOrSealed = true;
                    (tooltipRenderNewOld[i] as GearTooltipRender22).CompareMode = true;
                    (tooltipRenderNewOld[i] as GearTooltipRender22).MaxStar25 = CharaSimConfig.Default.Gear.MaxStar25;
                    (tooltipRenderNewOld[i] as GearTooltipRender22).ShowCosmetic = CharaSimConfig.Default.Gear.ShowCosmetic;
                    (tooltipRenderNewOld[i] as GearTooltipRender22).ShowCashPurchasePrice = CharaSimConfig.Default.Gear.ShowPurchasePrice;
                    (tooltipRenderNewOld[i] as GearTooltipRender22).LoadedCommoditiesSlot = i;
                }
                else
                {
                    tooltipRenderNewOld[i] = new GearTooltipRender2();
                    tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                    tooltipRenderNewOld[i].ShowObjectID = true;
                    tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                    (tooltipRenderNewOld[i] as GearTooltipRender2).ShowLevelOrSealed = true;
                    (tooltipRenderNewOld[i] as GearTooltipRender2).CompareMode = true;
                    (tooltipRenderNewOld[i] as GearTooltipRender2).MaxStar25 = CharaSimConfig.Default.Gear.MaxStar25;
                    (tooltipRenderNewOld[i] as GearTooltipRender2).ShowCosmetic = CharaSimConfig.Default.Gear.ShowCosmetic;
                    (tooltipRenderNewOld[i] as GearTooltipRender2).ShowCashPurchasePrice = CharaSimConfig.Default.Gear.ShowPurchasePrice;
                    (tooltipRenderNewOld[i] as GearTooltipRender2).LoadedCommoditiesSlot = i;
                }
                CharaWzNodeNewOld[i] = PluginManager.FindWz(Wz_Type.Character, WzFileNewOld[i]);
            }

            diffHtml["Eqp"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var eqpID in OutputGearTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 装备: {2}", ++count3, allCount3, eqpID);
                    StateDetail = "正在以Tooltip图像处理装备变更点...";

                    string nodePath = $@"{eqpID:D8}.img";
                    int nullIdx = 0;

                    // 生成变更前后提示框图片
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Gear gear = null;
                        foreach (var category in CharaWzNodeNewOld[i]?.Nodes ?? Enumerable.Empty<Wz_Node>())
                        {
                            if (category.Text.ToLower().Contains("canvas")) continue;

                            Wz_Node gearNode = null;
                            if (category.Text.Contains(".img") && category.Text == nodePath)
                            {
                                var img = category.GetValueEx<Wz_Image>(null);
                                if (img != null)
                                {
                                    gearNode = img.TryExtract() ? img.Node : null;
                                }

                                gear = Gear.CreateFromNode(gearNode, PluginManager.FindWz, WzFileNewOld[i]);
                                break;
                            }

                            gearNode = category.FindNodeByPath(nodePath);
                            if (gearNode != null)
                            {
                                var img = gearNode.GetValueEx<Wz_Image>(null);
                                if (img != null)
                                {
                                    gearNode = img.TryExtract() ? img.Node : null;
                                }

                                gear = Gear.CreateFromNode(gearNode, PluginManager.FindWz, WzFileNewOld[i]);
                                break;
                            }
                        }

                        if (gear == null)
                        {
                            nullIdx |= i + 1;
                        }
                        if (tooltipRenderNewOld[i] is GearTooltipRender22)
                            (tooltipRenderNewOld[i] as GearTooltipRender22).Gear = gear;
                        else
                            (tooltipRenderNewOld[i] as GearTooltipRender2).Gear = gear;
                    }
                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullIdx, eqpTooltipPath, eqpID.ToString(), "Eqp");
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Gear Tooltip: " + eqpID, ex.Message);
                }
            }
            OutputGearTooltipIDs.Clear();
        }

        // 变更怪物Tooltip输出
        private void SaveMobTooltip(string mobTooltipPath)
        {
            MobTooltipRenderer[] tooltipRenderNewOld = new MobTooltipRenderer[2];
            int count = 0;
            int allCount = OutputMobTooltipIDs.Count;

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                tooltipRenderNewOld[i] = new MobTooltipRenderer();
                tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                tooltipRenderNewOld[i].ShowObjectID = true;
                tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                tooltipRenderNewOld[i].DiffMobTags = this.DiffMobTags;
                tooltipRenderNewOld[i].EnableWorldArchive = this.OutputWorldArchives;
            }
            diffHtml["Mob"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var mobID in OutputMobTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 怪物: {2}", ++count, allCount, mobID);
                    StateDetail = "正在以Tooltip图像处理怪物变更点...";

                    string nodePath = $@"{mobID:D7}.img";
                    int nullIdx = 0;

                    // 生成变更前后提示框图像
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Mob mob = Mob.CreateFromNode(PluginManager.FindWz($@"Mob\{nodePath}", WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]);

                        if (mob == null)
                        {
                            nullIdx |= i + 1;
                        }
                        tooltipRenderNewOld[i].MobInfo = mob;
                    }

                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullIdx, mobTooltipPath, mobID.ToString(), "Mob", typePicH: 3);
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Mob Tooltip: " + mobID, ex.Message);
                }
            }
            OutputMobTooltipIDs.Clear();
            DiffMobTags.Clear();
        }

        // 变更NPC Tooltip输出
        private void SaveNPCTooltip(string npcTooltipPath)
        {
            NpcTooltipRenderer[] tooltipRenderNewOld = new NpcTooltipRenderer[2];
            int count = 0;
            int allCount = OutputNpcTooltipIDs.Count;

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                tooltipRenderNewOld[i] = new NpcTooltipRenderer();
                tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                tooltipRenderNewOld[i].ShowObjectID = true;
                tooltipRenderNewOld[i].ShowAllIllustAtOnce = this.ShowAllIllustAtOnce;
                tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                tooltipRenderNewOld[i].EnableWorldArchive = this.OutputWorldArchives;
                tooltipRenderNewOld[i].ShowNpcQuotes = this.ShowNpcQuotes;
            }
            diffHtml["Npc"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var npcID in OutputNpcTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} NPC: {2}", ++count, allCount, npcID);
                    StateDetail = "正在以Tooltip图像处理NPC变更点...";

                    string nodePath = $@"{npcID:D7}.img";
                    int nullIdx = 0;

                    // 生成变更前后提示框图像
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Npc npc = Npc.CreateFromNode(PluginManager.FindWz($@"Npc\{nodePath}", WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, WzFileNewOld[i]);

                        if (npc == null)
                        {
                            nullIdx |= i + 1;
                        }
                        tooltipRenderNewOld[i].NpcInfo = npc;
                    }

                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullIdx, npcTooltipPath, npcID.ToString(), "Npc", typePicH: 3);
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Npc Tooltip: " + npcID, ex.Message);
                }
            }
            OutputNpcTooltipIDs.Clear();
        }

        // 变更礼包Tooltip输出
        private void SaveCashTooltip(string itemTooltipPath)
        {
            CashPackageTooltipRender[] tooltipRenderNewOld = new CashPackageTooltipRender[2];
            int count = 0;
            int allCount = cashTooltipInfo.Count;

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                tooltipRenderNewOld[i] = new CashPackageTooltipRender();
                tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
                tooltipRenderNewOld[i].ShowObjectID = true;
            }
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            diffHtml["Item"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var itemID in cashTooltipInfo)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 礼包: {2}", ++count, allCount, itemID);
                    StateDetail = "正在以Tooltip图像处理礼包变更点...";
                    string itemNodePath = null;
                    string cashNodePath = null;
                    if (itemID.StartsWith("9")) // 判断第1位是否是09
                    {
                        itemNodePath = String.Format(@"Item\Special\0{0:D}.img\{1:D}", int.Parse(itemID) / 10000, itemID);
                        cashNodePath = string.Format(@"Etc\CashPackage.img\{0}", itemID);
                    }
                    int nullIdx = 0;

                    // 生成变更前后提示框图像
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        CharaSimLoader.LoadCommodities(WzFileNewOld[i], i);
                        CashPackage cash = CashPackage.CreateFromNode(PluginManager.FindWz(itemNodePath, WzFileNewOld[i]), PluginManager.FindWz(cashNodePath, WzFileNewOld[i]), PluginManager.FindWz, WzFileNewOld[i]);

                        if (cash == null)
                        {
                            nullIdx |= i + 1;
                        }
                        tooltipRenderNewOld[i].CashPackage = cash;
                    }
                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullIdx, itemTooltipPath, itemID.ToString(), "Item", typePicH: 15);
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
        private void SaveMapTooltip(string mapTooltipPath)
        {
            MapTooltipRenderer[] tooltipRenderNewOld = new MapTooltipRenderer[2];
            int count = 0;
            int allCount = OutputMapTooltipIDs.Count;

            for (int i = 0; i < 2; i++) // 0: New, 1: Old
            {
                tooltipRenderNewOld[i] = new MapTooltipRenderer();
                tooltipRenderNewOld[i].StringLinker = this.StringLinkerNewOld[i];
                tooltipRenderNewOld[i].ShowObjectID = true;
                tooltipRenderNewOld[i].ShowMiniMap = true;
                tooltipRenderNewOld[i].ShowMiniMapMob = CharaSimConfig.Default.Map.ShowMiniMapMob;
                tooltipRenderNewOld[i].ShowMiniMapNpc = true;
                tooltipRenderNewOld[i].ShowMiniMapPortal = true;
                tooltipRenderNewOld[i].ShowBgmName = true;
                tooltipRenderNewOld[i].ShowMobNpcObjectID = true;
                tooltipRenderNewOld[i].SourceWzFile = WzFileNewOld[i];
            }
            diffHtml["Map"] = new Dictionary<string, List<string>> { { "变更", new List<string>() }, { "新增", new List<string>() }, { "删除", new List<string>() } };

            foreach (var mapID in OutputMapTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 地图: {2}", ++count, allCount, mapID);
                    StateDetail = "正在以Tooltip图像处理地图变更点...";

                    string nodePath = $@"{mapID:D9}.img";
                    int nullIdx = 0;

                    // 生成变更前后提示框图像
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Map map = Map.CreateFromNode(PluginManager.FindWz($@"Map\Map\Map{mapID / 100000000}\{nodePath}", WzFileNewOld[i]), PluginManager.FindWz, WzFileNewOld[i]);

                        if (map == null)
                        {
                            nullIdx |= i + 1;
                        }
                        tooltipRenderNewOld[i].Map = map;
                    }

                    SaveTooltip(tooltipRenderNewOld[0], tooltipRenderNewOld[1], nullIdx, mapTooltipPath, mapID.ToString(), "Map", typePicH: 1);
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Map Tooltip: " + mapID, ex.Message);
                }
            }
            OutputMapTooltipIDs.Clear();
        }

        // 变更任务Tooltip输出
        private void SaveQuestTooltip(string questTooltipPath)
        {
            QuestTooltipRenderer[] questRenderNewOld = new QuestTooltipRenderer[2];
            int count = 0;
            int allCount = OutputQuestTooltipIDs.Count;
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

            foreach (var questID in OutputQuestTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 任务: {2}", ++count, allCount, questID);
                    StateDetail = "正在以Tooltip图像处理任务变更点...";
                    string questNodePath = String.Format(@"Quest\QuestData\{0:D}.img", questID);
                    string questNodePathLegacy = String.Format(@"Quest\QuestInfo.img\{0:D}", questID);
                    int nullQuestIdx = 0;

                    // 变更前后Tooltip图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Quest quest = Quest.CreateFromNode(PluginManager.FindWz(questNodePath, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz) ?? Quest.CreateFromNode(PluginManager.FindWz(questNodePathLegacy, WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz, fromInfoNode: questID);

                        if (quest == null)
                        {
                            nullQuestIdx |= i + 1;
                        }
                        questRenderNewOld[i].Quest = quest;
                    }

                    SaveTooltip(questRenderNewOld[0], questRenderNewOld[1], nullQuestIdx, questTooltipPath, questID.ToString(), "Quest");
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Quest Tooltip: " + questID, ex.Message);
                }
            }
            OutputQuestTooltipIDs.Clear();
        }

        // 变更成就Tooltip输出
        private void SaveAchvTooltip(string achvTooltipPath)
        {
            AchievementTooltipRenderer[] achvRenderNewOld = new AchievementTooltipRenderer[2];
            int count = 0;
            int allCount = OutputAchvTooltipIDs.Count;
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

            foreach (var achvID in OutputAchvTooltipIDs)
            {
                try
                {
                    StateInfo = string.Format("{0}/{1} 成就: {2}", ++count, allCount, achvID);
                    StateDetail = "正在以Tooltip图像处理成就变更点...";
                    bool[] isAchievementNull = new bool[2] { false, false };

                    if (SkipKMSContent && KMSContentID["Achievement"].Contains(achvID)) continue;

                    string achvNodePath = String.Format(@"Achievement\AchievementData\{0:D}.img", achvID);
                    int nullAchievementIdx = 0;

                    // 变更前后提示框图像生成
                    for (int i = 0; i < 2; i++) // 0: New, 1: Old
                    {
                        Achievement achv = Achievement.CreateFromNode(PluginManager.FindWz($@"Etc\Achievement\AchievementData\{achvID}.img", WzFileNewOld[i]), PluginManager.FindWz, PluginManager.FindWz);

                        if (achv == null)
                        {
                            isAchievementNull[i] = true;
                            nullAchievementIdx |= i + 1;
                        }
                        achvRenderNewOld[i].Achievement = achv;
                    }

                    SaveTooltip(achvRenderNewOld[0], achvRenderNewOld[1], nullAchievementIdx, achvTooltipPath, achvID.ToString(), "Achievement");
                }
                catch (Exception ex)
                {
                    FailToExportTooltips.Add("Achievement Tooltip: " + achvID, ex.Message);
                }
            }
            OutputAchvTooltipIDs.Clear();
            diffAchvTags.Clear();
        }

        // 提示框图像合成
        private void SaveTooltip(TooltipRender RenderNew, TooltipRender RenderOld, int nullIdx, string tooltipPath, string ID, string tooltipType, string infoText = null, int typePicH = 13)
        {
            Bitmap resultImage = null;
            Graphics g = null;
            string type = "";

            switch (nullIdx)
            {
                case 0: // change
                    type = "变更";
                    Bitmap ImageNew = null;
                    Bitmap ImageOld = null;
                    if (RenderNew is SkillTooltipRender2)
                    {
                        ImageNew = (RenderNew as SkillTooltipRender2).Render(true);
                        ImageOld = (RenderOld as SkillTooltipRender2).Render(true);
                    }
                    else if (RenderNew is MobTooltipRenderer)
                    {
                        ImageNew = (RenderNew as MobTooltipRenderer).Render();
                        ImageOld = (RenderOld as MobTooltipRenderer).Render();
                    }
                    else
                    {
                        ImageNew = RenderNew.Render();
                        ImageOld = RenderOld.Render();
                    }
                    resultImage = new Bitmap(ImageNew.Width + ImageOld.Width, Math.Max(ImageNew.Height, ImageOld.Height));
                    g = Graphics.FromImage(resultImage);

                    g.DrawImage(ImageOld, 0, 0);
                    g.DrawImage(ImageNew, ImageOld.Width, 0);
                    ImageNew.Dispose();
                    ImageOld.Dispose();
                    break;

                case 1: // delete
                    type = "删除";

                    resultImage = RenderOld.Render();
                    g = Graphics.FromImage(resultImage);
                    break;

                case 2: // add
                    type = "新增";

                    resultImage = RenderNew.Render();
                    g = Graphics.FromImage(resultImage);
                    break;

                default:
                    break;
            }

            if (resultImage == null || g == null)
            {
                return;
            }

            int picH = typePicH;
            if (RenderNew is QuestTooltipRenderer)
            {
                picH = Math.Max(picH, typePicH + (RenderNew as QuestTooltipRenderer).Margin_top);
            }
            if (RenderOld is QuestTooltipRenderer)
            {
                picH = Math.Max(picH, typePicH + (RenderOld as QuestTooltipRenderer).Margin_top);
            }
            if (RenderNew is MapTooltipRenderer || RenderOld is MapTooltipRenderer)
            {
                picH = 15;
            }
            GearGraphics.DrawPlainText(g, type, GearGraphics.ItemReqLevelFont, Color.FromArgb(255, 255, 255), 2, 100, ref picH, 100);

            if (infoText == null)
            {
                infoText = tooltipType == "Skill" ? $"[{(ItemStringHelper.GetJobName(Int32.Parse(ID) / 10000) ?? "其它")}]" : "";
            }
            string imageName = Path.Combine(tooltipPath, $"{tooltipType}_{ID}{infoText}_{type}.png");
            diffHtml[tooltipType][type].Add($"{tooltipType}_{ID}{infoText}_{type}.png");
            if (!File.Exists(imageName))
            {
                resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
            }
            resultImage.Dispose();
            g.Dispose();
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
                    if (node.FindNodeByPath("common")?.FindNodeByPath("attackInfo") != null)
                    {
                        if (!OutputPerJobSkillTooltipIDs.Contains(skillID))
                        {
                            OutputPerJobSkillTooltipIDs.Add(skillID);
                            diffPerJobSkillTags[skillID] = new List<string>();
                        }

                        if (tag != null && !diffPerJobSkillTags[skillID].Contains(tag))
                        {
                            diffPerJobSkillTags[skillID].Add(tag);
                        }
                    }
                    else
                    {
                        if (!OutputSkillTooltipIDs.Contains(skillID))
                        {
                            OutputSkillTooltipIDs.Add(skillID);
                            diffSkillTags[skillID] = new List<string>();
                        }

                        if (tag != null && !diffSkillTags[skillID].Contains(tag))
                        {
                            diffSkillTags[skillID].Add(tag);
                        }
                    }
                }
            }
        }

        //从Item不同节点获取ItemID
        private void getIDFromItem(Wz_Node node, bool change)
        {
            if (node == null) return;
            Match match = Regex.Match(node.FullPathToFile, @"^String\\(?:Cash|Consume|Etc|Ins|Pet).img\\(?:.+?\\)?(\d+).*"); // 스트링 확인

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Item\\(?:Cash|Consume|Etc|Install|Pet)\\\d+.img\\(\d+)$"); // 추가/삭제 확인
            }

            if (change && !match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Item\\(?:Cash|Consume|Etc|Install|Pet)\\_Canvas\\\d+.img\\(\d+)\\info\\(icon)$"); // 아이콘 변경 체크
            }

            if (match.Success)
            {
                string itemID = match.Groups[1].ToString();

                if (itemID != null && int.TryParse(itemID, out var id))
                {
                    if (!OutputItemTooltipIDs.Contains(id))
                    {
                        OutputItemTooltipIDs.Add(id);
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
        private void getIDFromGear(Wz_Node node, bool change)
        {
            if (node == null) return;
            var tag = node.Text;
            Match match = Regex.Match(node.FullPathToFile, @"^String\\Eqp.img\\Eqp\\(?:.+?\\)?(\d+).*"); // 스트링 확인

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Character\\.+?\\(\d+).img$"); // 추가/삭제 확인
            }

            if (change && !match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Character\\.+?\\_Canvas\\(\d+).img\\info\\(icon)$"); // 아이콘 변경 체크
            }
            if (match.Success)
            {
                string eqpID = match.Groups[1].ToString();
                if (eqpID != null && int.TryParse(eqpID, out var id))
                {
                    if (!OutputGearTooltipIDs.Contains(id))
                    {
                        OutputGearTooltipIDs.Add(id);
                    }
                }

            }
        }

        //从Mob不同节点找到MobID
        private void getIDFromMob(Wz_Node node)
        {
            if (node == null) return;

            Match match = Regex.Match(node.FullPathToFile, @"^String\\Mob.img\\(\d+)\\(name).*"); // 스트링 확인
            string tag = null;

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Mob\\(\d+).img\\info\\(level|maxHP|PDRate|MDRate|boss|exp).*"); // 변경점 중 툴팁 출력할 것들
                tag = node.Text;
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Mob\\(\d+).img$"); // 추가/삭제 확인
                tag = null;
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\MobLocation.img\\(\d+)\\.*"); // 위치 확인
                tag = null;
            }

            if (match.Success)
            {
                string mobID = match.Groups[1].ToString();

                if (mobID != null && int.TryParse(mobID, out var id))
                {
                    if (!OutputMobTooltipIDs.Contains(id))
                    {
                        OutputMobTooltipIDs.Add(id);
                        DiffMobTags[id] = new HashSet<string>();
                    }

                    if (tag != null && !DiffMobTags[id].Contains(tag))
                    {
                        DiffMobTags[id].Add(tag);
                    }
                }
            }

            if (!match.Success && this.OutputWorldArchives)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\worldArchive.img\\collectionInfo\\\d+\\\d+\\mob\\\d+\\desc$"); // 确认世界档案馆
                if (match.Success)
                {
                    IEnumerable<int> ids = (node.ParentNode?.FindNodeByPath("id")?.Nodes ?? new Wz_Node.WzNodeCollection(null)).Select(node => node.GetValueEx<int>(0)).Distinct();
                    foreach (var id in ids)
                    {
                        if (!OutputMobTooltipIDs.Contains(id))
                        {
                            OutputMobTooltipIDs.Add(id);
                        }
                    }
                }
            }
        }

        //从NPC不同节点找到NpcID
        private void getIDFromNpc(Wz_Node node)
        {
            if (node == null) return;

            Match match = Regex.Match(node.FullPathToFile, @"^String\\Npc.img\\(\d+)\\(name).*"); // 스트링 확인

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Npc\\(\d+).img$"); // 추가/삭제 확인
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\NpcLocation.img\\(\d+)\\.*[^\\]$"); // 위치 확인
            }

            if (match.Success)
            {
                string npcID = match.Groups[1].ToString();

                if (npcID != null && int.TryParse(npcID, out var id))
                {
                    if (!OutputNpcTooltipIDs.Contains(id))
                    {
                        OutputNpcTooltipIDs.Add(id);
                    }
                }
            }

            if (!match.Success && this.OutputWorldArchives)
            {
                match = Regex.Match(node.FullPathToFile, @"^Etc\\worldArchive.img\\collectionInfo\\\d+\\\d+\\npc\\\d+\\desc$"); // 确认世界档案馆
                if (match.Success)
                {
                    IEnumerable<int> ids = (node.ParentNode?.FindNodeByPath("id")?.Nodes ?? new Wz_Node.WzNodeCollection(null)).Select(node => node.GetValueEx<int>(0)).Distinct();
                    foreach (var id in ids)
                    {
                        if (!OutputNpcTooltipIDs.Contains(id))
                        {
                            OutputNpcTooltipIDs.Add(id);
                        }
                    }
                }
            }
        }

        // 从节点获取MapID
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

        private void GetQuestID(Wz_Node node)
        {
            if (node == null ) return;
            Match match = Regex.Match(node.FullPathToFile, @"^Quest\\QuestInfo.img\\(\d+).*\\.*");
            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Quest\\QuestInfo.img\\(\d+)$");
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Quest\\QuestData\\(\d+).img\\(QuestInfo|Check|Act)\\.*");
            }

            if (!match.Success)
            {
                match = Regex.Match(node.FullPathToFile, @"^Quest\\QuestData\\(\d+).img$");
            }
            if (match.Success)
            {
                var questID = match.Groups[1].Value;

                if (questID != null && int.TryParse(questID, out var id))
                {
                    if (!OutputQuestTooltipIDs.Contains(id))
                    {
                        OutputQuestTooltipIDs.Add(id);
                    }
                }
            }
        }

        private void GetAchvID(Wz_Node node)
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

                if (achvID != null && int.TryParse(achvID, out var id))
                {
                    if (!OutputAchvTooltipIDs.Contains(id))
                    {
                        OutputAchvTooltipIDs.Add(id);
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
                    if (!OutputItemTooltipIDs.Contains(id))
                    {
                        OutputItemTooltipIDs.Add(id);
                    }
                }
                else if (saveEqpTooltip) // gear
                {
                    if (!OutputGearTooltipIDs.Contains(id))
                    {
                        OutputGearTooltipIDs.Add(id);
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

        private void CompareImg(Wz_Image imgNew, Wz_Image imgOld, string imgName, string anchorName, string menuAnchorName, string outputDir, StreamWriter sw, int newNumber = 0, int oldNumber = 0)
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
                if (saveItemTooltip && outputDir.Contains("Item") || imgName.StartsWith("String"))
                {
                    getIDFromItem(diff.NodeNew, idx == 0 ? true : false);
                    getIDFromItem(diff.NodeOld, idx == 0 ? true : false);
                }
                // 变更的装备Tooltip处理
                if (saveEqpTooltip && outputDir.Contains("Character") || imgName.StartsWith("String"))
                {
                    getIDFromGear(diff.NodeNew, idx == 0 ? true : false);
                    getIDFromGear(diff.NodeOld, idx == 0 ? true : false);
                }
                //变更的地图Tooltip处理
                if (saveMapTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Map") || imgName.StartsWith("String")))
                {
                    GetIDFromMap(diff.NodeNew, idx == 0 ? true : false);
                    GetIDFromMap(diff.NodeOld, idx == 0 ? true : false);
                }
                // 变更的怪物Tooltip处理
                if (saveMobTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Mob") || imgName.StartsWith("String")))
                {
                    getIDFromMob(diff.NodeNew);
                    getIDFromMob(diff.NodeOld);
                }
                // 变更的Npc Tooltip处理
                if (saveNpcTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Npc") || imgName.StartsWith("String")))
                {
                    getIDFromNpc(diff.NodeNew);
                    getIDFromNpc(diff.NodeOld);
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
                if (saveQuestTooltip && (imgName.Contains("QuestInfo") || imgName.Contains("QuestData")))
                {
                    GetQuestID(diff.NodeNew);
                    GetQuestID(diff.NodeOld);
                }
                // 变更的成就Tooltip处理
                if (saveAchievementTooltip && (outputDir.Contains("Etc") && imgName.Contains("Achievement") && !imgName.Contains("_Canvas")))
                {
                    GetAchvID(diff.NodeNew);
                    GetAchvID(diff.NodeOld);
                }
            }

            StateDetail = "正在处理档案";
            bool noChange = diffList.Count <= 0;
            sw.WriteLine("<table class=\"img{0}\">", noChange ? " noChange" : "");
            sw.WriteLine("<tr><th colspan=\"3\"><a name=\"{1}\">{0}</a> 变更:{2} 新增:{3} 删除:{4}</th></tr>",
                imgName, anchorName, count[0], count[1], count[2]);
            sw.WriteLine(String.Format(@"<tr><th>路径</th><th>新版本(v{0})</th><th>旧版本(v{1})</th></tr>", newNumber, oldNumber));
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
                    if (saveItemTooltip && (imgName.StartsWith("Item") || imgName.StartsWith("String"))) // 变更道具Tooltip处理
                    {
                        getIDFromItem(node, idx == 0 ? true : false);
                    }
                    if (saveEqpTooltip && (imgName.StartsWith("Character") || imgName.StartsWith("String"))) // 变更装备Tooltip处理
                    {
                        getIDFromGear(node, idx == 0 ? true : false);
                    }
                    if (saveMapTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Map") || imgName.StartsWith("String"))) // 变更地图Tooltip处理
                    {
                        GetIDFromMap(node, idx == 0 ? true : false);
                    }
                    if (saveMobTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Mob") || imgName.StartsWith("String"))) // 变更装备Tooltip处理
                    {
                        getIDFromMob(node);
                    }
                    if (saveNpcTooltip && (imgName.StartsWith("Etc") || imgName.StartsWith("Npc") || imgName.StartsWith("String"))) // 变更Npc Tooltip处理
                    {
                        getIDFromNpc(node);
                    }
                    if (saveQuestTooltip && (imgName.Contains("QuestInfo") || imgName.Contains("QuestData"))) //变更任务 Tooltip处理
                    {
                        GetQuestID(node);
                    }
                    if (saveAchievementTooltip && (imgName.StartsWith("Etc") && imgName.Contains("Achievement") && !imgName.Contains("_Canvas"))) //变更成就 Tooltip处理
                    {
                        GetAchvID(node);
                    }
                    if (saveCashTooltip && (imgName.StartsWith("Item") || imgName.StartsWith("String"))) // 变更礼包Tooltip处理
                    {
                        getIDFromItem(node, idx == 0 ? true : false);
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
            StringBuilder css = new StringBuilder();
            css.AppendLine($"body {{ font-size:12px; background-color:{ColorToHex(ColorTable[0])}; color:{ColorToHex(ColorTable[1])}; }}");
            css.AppendLine($"a {{ color:{ColorToHex(ColorTable[8])}; }}");
            css.AppendLine($"p.wzf {{ }}");
            css.AppendLine($"table, tr, th, td {{ border:1px solid #ff8000; border-collapse:collapse; }}");
            css.AppendLine($"table {{ margin-bottom:16px; }}");
            css.AppendLine($"th {{ text-align:left; }}");
            css.AppendLine($"table.lst0 {{ }}");
            css.AppendLine($"table.lst1 {{ }}");
            css.AppendLine($"table.lst2 {{ }}");
            css.AppendLine($"table.img {{ }}");
            css.AppendLine($"table.img tr.r0 {{ background-color:{ColorToHex(ColorTable[2])}; color:{ColorToHex(ColorTable[5])}; }}");
            css.AppendLine($"table.img tr.r1 {{ background-color:{ColorToHex(ColorTable[3])}; color:{ColorToHex(ColorTable[6])}; }}");
            css.AppendLine($"table.img tr.r2 {{ background-color:{ColorToHex(ColorTable[4])}; color:{ColorToHex(ColorTable[7])}; }}");
            css.AppendLine($"table.img.noChange {{ display:none; }}");
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(css.ToString());
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

        private static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
