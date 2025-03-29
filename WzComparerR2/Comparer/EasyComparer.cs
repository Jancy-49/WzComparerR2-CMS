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
        private Wz_File mobWzNew { get; set; }
        private Wz_File npcWzNew { get; set; }
        private Wz_File eqpWzNew { get; set; }
        private Wz_File etcWzNew { get; set; }
        private Wz_File stringWzOld { get; set; }
        private Wz_File itemWzOld { get; set; }
        private Wz_File mobWzOld { get; set; }
        private Wz_File npcWzOld { get; set; }
        private Wz_File eqpWzOld { get; set; }
        private Wz_File etcWzOld { get; set; }
        private List<string> TooltipInfo = new List<string>();
        private List<string> itemTooltipInfo = new List<string>();
        private List<string> eqpTooltipInfo = new List<string>();
        private List<string> mobTooltipInfo = new List<string>();
        private List<string> npcTooltipInfo = new List<string>();
        private List<string> cashTooltipInfo = new List<string>();
        private Dictionary<string, List<string>> diffSkillTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffItemTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffEqpTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffMobTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffNpcTags = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> diffCashTags = new Dictionary<string, List<string>>();
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
        public bool HashPngFileName { get; set; }

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
            string mobTooltipPath = Path.Combine(outputDir, "MobTooltip");
            string npcTooltipPath = Path.Combine(outputDir, "NpcTooltip");

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
                }.Where(p => p != null)));
                sw.WriteLine("</table>");
                sw.WriteLine("</p>");

                //输出目录
                StringBuilder[] sb = { new StringBuilder(), new StringBuilder(), new StringBuilder() };
                int[] count = new int[6];
                string[] diffStr = { "变更", "新增", "删除" };
                foreach (CompareDifference diff in diffLst)
                {
                    int idx = -1;
                    string detail = null;
                    switch (diff.DifferenceType)
                    {
                        case DifferenceType.Changed:
                            idx = 0;
                            detail = string.Format("<a name=\"m_{1}_{2}\" href=\"#a_{1}_{2}\">{0}</a>", diff.NodeNew.FullPathToFile, idx, count[idx]);
                            break;
                        case DifferenceType.Append:
                            idx = 1;
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
            if (saveSkillTooltip && type.ToString() == "String" && TooltipInfo != null)
            {
                if (!Directory.Exists(skillTooltipPath))
                {
                    Directory.CreateDirectory(skillTooltipPath);
                }
                saveTooltip(skillTooltipPath);
            }
            if (saveItemTooltip && type.ToString() == "String" && itemTooltipInfo != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                saveTooltip2(itemTooltipPath);
            }
            if (saveEqpTooltip && type.ToString() == "String" && eqpTooltipInfo != null)
            {
                if (!Directory.Exists(eqpTooltipPath))
                {
                    Directory.CreateDirectory(eqpTooltipPath);
                }
                saveTooltip3(eqpTooltipPath);
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
            if (saveCashTooltip && type.ToString() == "String" && cashTooltipInfo != null)
            {
                if (!Directory.Exists(itemTooltipPath))
                {
                    Directory.CreateDirectory(itemTooltipPath);
                }
                saveTooltip6(itemTooltipPath);
            }
        }

        // 变更技能Tooltip输出
        private void saveTooltip(string skillTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            SkillTooltipRender2 skillRenderNew = new SkillTooltipRender2();
            SkillTooltipRender2 skillRenderOld = new SkillTooltipRender2();
            int count = 0;
            int allCount = TooltipInfo.Count;
            var skillTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            skillRenderNew.StringLinker = slNew;
            skillRenderOld.StringLinker = slOld;
            skillRenderNew.ShowObjectID = true;
            skillRenderOld.ShowObjectID = true;
            skillRenderNew.ShowDelay = true;
            skillRenderOld.ShowDelay = true;
            skillRenderNew.wzNode = wzNew;
            skillRenderOld.wzNode = wzOld;
            skillRenderNew.DiffSkillTags = this.diffSkillTags;
            skillRenderOld.DiffSkillTags = this.diffSkillTags;
            skillRenderNew.IgnoreEvalError = true;
            skillRenderOld.IgnoreEvalError = true;

            foreach (var skillID in TooltipInfo)
            {
                count++;
                StateInfo = string.Format("{0}/{1} 技能: {2}", count, allCount, skillID);
                StateDetail = "正在以Tooltip图像处理技能变更点...";

                Bitmap skillImageNew = null;
                Bitmap skillImageOld = null;
                string skillType = "删除";
                string skillNodePath = int.Parse(skillID) / 10000000 == 8 ? String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 100, skillID) : String.Format(@"\{0:D}.img\skill\{1:D}", int.Parse(skillID) / 10000, skillID);
                if (int.Parse(skillID) / 10000 == 0) skillNodePath = String.Format(@"\000.img\skill\{0:D7}", skillID);
                int heightNew = 0, heightOld = 0;
                int width = 0;

                // 变更后Tooltip图像生成
                Skill skillNew = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, wzNew.GetNodeWzFile()), PluginManager.FindWz, wzNew?.GetNodeWzFile());
                if (skillNew != null)
                {
                    skillNew.Level = skillNew.MaxLevel;
                    skillRenderNew.Skill = skillNew;
                    skillImageNew = skillRenderNew.Render();
                    width += skillImageNew.Width;
                    heightNew = skillImageNew.Height;
                }
                // 变更前Tooltip图像生成
                Skill skillOld = Skill.CreateFromNode(PluginManager.FindWz("Skill" + skillNodePath, wzOld.GetNodeWzFile()), PluginManager.FindWz, wzOld?.GetNodeWzFile());
                if (skillOld != null)
                {
                    skillOld.Level = skillOld.MaxLevel;
                    skillRenderOld.Skill = skillOld;
                    skillImageOld = skillRenderOld.Render();
                    width += skillImageOld.Width;
                    heightOld = skillImageOld.Height;
                }
                if (width == 0) continue;
                // Tooltip图像合成
                Bitmap resultImage = new Bitmap(width, Math.Max(heightNew, heightOld));
                Graphics g = Graphics.FromImage(resultImage);

                if (skillImageOld != null)
                {
                    if (skillImageNew != null)
                    {
                        g.DrawImage(skillImageNew, skillImageOld.Width, 0);
                        skillImageNew.Dispose();
                        skillType = "变更";
                    }
                    g.DrawImage(skillImageOld, 0, 0);
                    skillImageOld.Dispose();
                }
                else
                {
                    g.DrawImage(skillImageNew, 0, 0);
                    skillImageNew.Dispose();
                    skillType = "新增";
                }

                var skillTypeTextInfo = g.MeasureString(skillType, GearGraphics.ItemDetailFont2);
                int picH = 13;
                GearGraphics.DrawPlainText(g, skillType, skillTypeFont, Color.FromArgb(255, 255, 255), 2, (int)Math.Ceiling(skillTypeTextInfo.Width) + 2, ref picH, 10);

                string imageName = Path.Combine(skillTooltipPath, "Skill_" + skillID + '[' + (ItemStringHelper.GetJobName(int.Parse(skillID) / 10000) ?? "其它") + "]_" + skillType + ".png");
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            TooltipInfo.Clear();
            diffSkillTags.Clear();
        }

        // 变更道具Tooltip输出
        private void saveTooltip2(string itemTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            ItemTooltipRender2 itemRenderNew = new ItemTooltipRender2();
            ItemTooltipRender2 itemRenderOld = new ItemTooltipRender2();
            CashPackageTooltipRender cashRenderNew = new CashPackageTooltipRender();
            CashPackageTooltipRender cashRenderOld = new CashPackageTooltipRender();
            int count2 = 0;
            int allCount2 = itemTooltipInfo.Count;
            var itemTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            itemRenderNew.StringLinker = slNew;
            itemRenderOld.StringLinker = slOld;
            cashRenderNew.StringLinker = slNew;
            cashRenderOld.StringLinker = slOld;
            itemRenderNew.ShowObjectID = true;
            itemRenderOld.ShowObjectID = true;
            cashRenderNew.ShowObjectID = true;
            cashRenderOld.ShowObjectID = true;

            foreach (var itemID in itemTooltipInfo)
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
                else if (itemID.StartsWith("05")) // 判断第1位是否是02
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
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            itemTooltipInfo.Clear();
            diffItemTags.Clear();
        }

        private void saveTooltip3(string eqpTooltipPath)
        {
            StringLinker slNew = new StringLinker();
            StringLinker slOld = new StringLinker();
            GearTooltipRender2 eqpRenderNew = new GearTooltipRender2();
            GearTooltipRender2 eqpRenderOld = new GearTooltipRender2();
            int count3 = 0;
            int allCount3 = eqpTooltipInfo.Count;
            var eqpTypeFont = new Font("宋体", 11f, GraphicsUnit.Pixel);

            this.stringWzNew = wzNew?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzNew = wzNew?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzNew = wzNew?.FindNodeByPath("Etc").GetNodeWzFile();
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            eqpRenderNew.StringLinker = slNew;
            eqpRenderOld.StringLinker = slOld;
            eqpRenderNew.ShowObjectID = true;
            eqpRenderOld.ShowObjectID = true;
            foreach (var eqpID in eqpTooltipInfo)
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
                else if (Regex.IsMatch(eqpID, "^012[1-9]|^013|^014|^015|^0160|^0169|^0170")) // 判断开头是否是012~015、0160或0169-0179
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
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            eqpTooltipInfo.Clear();
            diffEqpTags.Clear();
        }

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
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            mobRenderNew.StringLinker = slNew;
            mobRenderOld.StringLinker = slOld;
            mobRenderNew.ShowObjectID = true;
            mobRenderOld.ShowObjectID = true;
            foreach (var mobID in mobTooltipInfo)
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
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            mobTooltipInfo.Clear();
            diffMobTags.Clear();
        }

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
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();

            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            npcRenderNew.StringLinker = slNew;
            npcRenderOld.StringLinker = slOld;
            npcRenderNew.ShowObjectID = true;
            npcRenderOld.ShowObjectID = true;
            foreach (var npcID in npcTooltipInfo)
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
                Npc npcNew = Npc.CreateFromNode(PluginManager.FindWz(npcNodePath, wzNew?.GetNodeWzFile()), PluginManager.FindWz);
                if (npcNew != null)
                {
                    npcRenderNew.NpcInfo = npcNew;
                    npcImageNew = npcRenderNew.Render();
                    width += npcImageNew.Width;
                    heightNew = npcImageNew.Height;
                }
                if (width == 0) continue;
                // 变更前Tooltip图像生成
                Npc npcOld = Npc.CreateFromNode(PluginManager.FindWz(npcNodePath, wzOld?.GetNodeWzFile()), PluginManager.FindWz);
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
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            npcTooltipInfo.Clear();
            diffNpcTags.Clear();
        }

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
            this.stringWzOld = wzOld?.FindNodeByPath("String").GetNodeWzFile();
            this.itemWzOld = wzOld?.FindNodeByPath("Item").GetNodeWzFile();
            this.etcWzOld = wzOld?.FindNodeByPath("Etc").GetNodeWzFile();
            slNew.Load(stringWzNew, itemWzNew, etcWzNew);
            slOld.Load(stringWzOld, itemWzOld, etcWzOld);
            cashRenderNew.StringLinker = slNew;
            cashRenderOld.StringLinker = slOld;
            cashRenderNew.ShowObjectID = true;
            cashRenderOld.ShowObjectID = true;
            foreach (var itemID in cashTooltipInfo)
            {
                count6++;
                StateInfo = string.Format("{0}/{1} 礼包: {2}", count6, allCount6, itemID);
                StateDetail = "正在以Tooltip图像处理道具变更点...";

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
<<<<<<< HEAD
                CashPackage itemOld = CashPackage.CreateFromNode(PluginManager.FindWz(itemNodePath, wzOld?.GetNodeWzFile()), PluginBase.PluginManager.FindWz(string.Format(@"Etc\CashPackage.img\{0}", itemID)), PluginManager.FindWz);
=======
                CashPackage itemOld = CashPackage.CreateFromNode(PluginManager.FindWz(itemNodePath, wzNew?.GetNodeWzFile()), PluginBase.PluginManager.FindWz(string.Format(@"Etc\CashPackage.img\{0}", itemID)), PluginManager.FindWz);
>>>>>>> 7e9cc6786fcad07de1db367547c62c87f3fd5fe4
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
                if (!File.Exists(imageName))
                {
                    resultImage.Save(imageName, System.Drawing.Imaging.ImageFormat.Png);
                }
                resultImage.Dispose();
                g.Dispose();
            }
            cashTooltipInfo.Clear();
            diffItemTags.Clear();
        }

        // 从Skill不同节点获取SkillID
        private void getIDFromSkill(Wz_Node node)
        {
            var tag = node.Text;
            Match match = Regex.Match(node.FullPathToFile, @"^Skill\d*\\\d+.img\\skill\\(\d+)\\(common|masterLevel|combatOrders|action|isPetAutoBuff|BGM).*");
            if (match.Success)
            {
                string skillID = match.Groups[1].ToString();
                if (!TooltipInfo.Contains(skillID) && skillID != null)
                {
                    TooltipInfo.Add(skillID);
                    diffSkillTags[skillID] = new List<string>();
                    diffSkillTags[skillID].Add(tag);
                }
                else if (TooltipInfo.Contains(skillID) && skillID != null)
                {
                    if (!diffSkillTags[skillID].Contains(tag))
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

        // 从String不同节点获取SkillID
        private void getIDFromString(Wz_Node node)
        {
            Match match = Regex.Match(node.FullPathToFile, @"^String\\Skill.img\\(\d+).*");
            if (match.Success)
            {
                string skillID = match.Groups[1].ToString();
                if (!TooltipInfo.Contains(skillID) && skillID != null)
                {
                    TooltipInfo.Add(skillID);
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
                if (saveSkillTooltip && outputDir.Contains("Skill"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromSkill(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromSkill(diff.NodeOld);
                    }
                }
                if (saveSkillTooltip && outputDir.Contains("String"))
                {
                    if (diff.NodeNew != null)
                    {
                        getIDFromString(diff.NodeNew);
                    }
                    if (diff.NodeOld != null)
                    {
                        getIDFromString(diff.NodeOld);
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

                    if (saveSkillTooltip && outputDir.Contains("Skill")) // 变更技能Tooltip处理
                    {
                        getIDFromSkill(node);
                    }
                    if (saveItemTooltip && outputDir.Contains("Item")) // 变更道具Tooltip处理
                    {
                        getIDFromItem(node);
                    }
                    if (saveEqpTooltip && outputDir.Contains("Character")) // 变更装备Tooltip处理
                    {
                        getIDFromChar(node);
                    }
                    if (saveEqpTooltip && outputDir.Contains("Mob")) // 变更装备Tooltip处理
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

                        if (this.HashPngFileName)
                        {
                            fileName = ToHexString(MD5Hash(fileName));
                            // TODO: save file name mapping to another file? 
                        }
                        else
                        {
                            for (int i = 0; i < invalidChars.Length; i++)
                            {
                                fileName = fileName.Replace(invalidChars[i], '_');
                            }
                        }

                        fileName = fileName + "_" + colName + ".png";
                        using (Bitmap bmp = png.ExtractPng())
                        {
                            bmp.Save(Path.Combine(outputDir, fileName), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        return string.Format("<img src=\"{0}/{1}\" />", new DirectoryInfo(outputDir).Name, WebUtility.UrlEncode(fileName));
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

                        byte[] mp3 = sound.ExtractSound();
                        if (mp3 != null)
                        {
                            FileStream fileStream = new FileStream(Path.Combine(outputDir, filePath), FileMode.Create, FileAccess.Write);
                            fileStream.Write(mp3, 0, mp3.Length);
                            fileStream.Close();
                        }
                        return string.Format("<audio controls src=\"{0}\" type=\"audio/mpeg\">Audio {1}ms\n</audio>", Path.Combine(new DirectoryInfo(outputDir).Name, filePath), sound.Ms);
                    }
                    else
                    {
                        return string.Format("Audio {0}ms", sound.Ms);
                    }

                case Wz_Image _:
                    return "(img)";
            }

            return WebUtility.HtmlEncode(Convert.ToString(value.Value));
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
    }
}
