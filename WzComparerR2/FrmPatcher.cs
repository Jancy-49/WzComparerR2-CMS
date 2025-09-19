using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using DevComponents.Editors;
using WzComparerR2.Comparer;
using WzComparerR2.Config;
using WzComparerR2.Patcher;
using WzComparerR2.WzLib;

namespace WzComparerR2
{
    public partial class FrmPatcher : DevComponents.DotNetBar.Office2007Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
#if NET6_0_OR_GREATER
            // https://learn.microsoft.com/en-us/dotnet/core/compatibility/fx-core#controldefaultfont-changed-to-segoe-ui-9pt
            this.Font = new Font(new FontFamily("MS PGothic"), 9f);
#endif
            panelEx1.AutoScroll = true;

            var settings = WcR2Config.Default.PatcherSettings;
            if (settings.Count <= 0)
            {
                settings.Add(new PatcherSetting("KMST", "http://maplestory.dn.nexoncdn.co.kr/PatchT/{1:d5}/{0:d5}to{1:d5}.patch", 2));
                settings.Add(new PatcherSetting("KMST-Minor", "http://maplestory.dn.nexoncdn.co.kr/PatchT/{0:d5}/Minor/{1:d2}to{2:d2}.patch", 3));
                settings.Add(new PatcherSetting("KMS", "http://maplestory.dn.nexoncdn.co.kr/Patch/{1:d5}/{0:d5}to{1:d5}.patch", 2));
                settings.Add(new PatcherSetting("KMS-Minor", "http://maplestory.dn.nexoncdn.co.kr/Patch/{0:d5}/Minor/{1:d2}to{2:d2}.patch", 3));
                settings.Add(new PatcherSetting("JMS", "http://webdown2.nexon.co.jp/maple/patch/patchdir/{1:d5}/{0:d5}to{1:d5}.patch", 2));
                settings.Add(new PatcherSetting("GMS", "http://download2.nexon.net/Game/MapleStory/patch/patchdir/{1:d5}/CustomPatch{0}to{1}.exe", 2));
                settings.Add(new PatcherSetting("TMS", "http://tw.cdnpatch.maplestory.beanfun.com/maplestory/patch/patchdir/{1:d5}/{0:d5}to{1:d5}.patch", 2));
                settings.Add(new PatcherSetting("MSEA", "http://patch.maplesea.com/sea/patch/patchdir/{1:d5}/{0:d5}to{1:d5}.patch", 2));
                settings.Add(new PatcherSetting("CMS", "http://mxd.clientdown.sdo.com/maplestory/patch/patchdir/{1:d5}/{0:d5}to{1:d5}.patch", 2));
            }

            foreach (PatcherSetting p in settings)
            {
                this.MigrateSetting(p);
                comboBoxEx1.Items.Add(p);
            }
            if (comboBoxEx1.Items.Count > 0)
                comboBoxEx1.SelectedIndex = 0;

            foreach (WzPngComparison comp in Enum.GetValues(typeof(WzPngComparison)))
            {
                cmbComparePng.Items.Add(comp);
            }
            cmbComparePng.SelectedItem = WzPngComparison.SizeAndDataLength;
        }
        
        public Encoding PatcherNoticeEncoding { get; set; }

        Thread patchThread;
        EventWaitHandle waitHandle;
        bool waiting;
        string loggingFileName;
        bool isUpdating;

        private PatcherSetting SelectedPatcherSetting => comboBoxEx1.SelectedItem as PatcherSetting;

        private void MigrateSetting(PatcherSetting patcherSetting)
        {
            if (patcherSetting.MaxVersion == 0 && patcherSetting.Versions == null)
            {
                patcherSetting.MaxVersion = 2;
                patcherSetting.Versions = new[] { patcherSetting.Version0 ?? 0, patcherSetting.Version1 ?? 0 };
                patcherSetting.Version0 = null;
                patcherSetting.Version1 = null;
            }
            if (patcherSetting.Versions != null && patcherSetting.Versions.Length < patcherSetting.MaxVersion)
            {
                var newVersions = new int[patcherSetting.MaxVersion];
                Array.Copy(patcherSetting.Versions, newVersions, patcherSetting.Versions.Length);
                patcherSetting.Versions = newVersions;
            }
        }

        private void ApplySetting(PatcherSetting p)
        {
            if (isUpdating)
            {
                return;
            }
            isUpdating = true;
            try
            {
                if (this.flowLayoutPanel1.Controls.Count < p.MaxVersion)
                {
                    var inputTemplate = this.integerInput1;
                    var preAddedControls = Enumerable.Range(0, p.MaxVersion - this.flowLayoutPanel1.Controls.Count)
                        .Select(_ =>
                        {
                            var input = new IntegerInput()
                            {
                                AllowEmptyState = inputTemplate.AllowEmptyState,
                                Size = inputTemplate.Size,
                                Value = 0,
                                MinValue = inputTemplate.MinValue,
                                MaxValue = inputTemplate.MaxValue,
                                DisplayFormat = inputTemplate.DisplayFormat,
                                ShowUpDown = inputTemplate.ShowUpDown,
                            };
                            input.BackgroundStyle.ApplyStyle(inputTemplate.BackgroundStyle);
                            input.ValueChanged += this.integerInput_ValueChanged;
                            return input;
                        }).ToArray();
                    this.flowLayoutPanel1.Controls.AddRange(preAddedControls);
                }
                for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
                {
                    var input = (IntegerInput)this.flowLayoutPanel1.Controls[i];
                    if (i < p.MaxVersion)
                    {
                        input.Show();
                        input.Value = (p.Versions != null && i < p.Versions.Length) ? p.Versions[i] : 0;
                    }
                    else
                    {
                        input.Hide();
                        input.Value = 0;
                    }
                }
                this.txtUrl.Text = p.Url;
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void combineUrl()
        {
            if (this.SelectedPatcherSetting is var p)
            {
                txtUrl.Text = p.Url;
            }
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedPatcherSetting is var p)
            {
                this.ApplySetting(p);
            }
        }

        private void integerInput_ValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedPatcherSetting is var p && sender is IntegerInput input)
            {
                var i = this.flowLayoutPanel1.Controls.IndexOf(input);
                if (i > -1 && i < p.MaxVersion)
                {
                    if (p.Versions == null)
                    {
                        p.Versions = new int[p.MaxVersion];
                    }
                    p.Versions[i] = input.Value;
                }
                this.ApplySetting(p);
            }
        }

        private void buttonXCheck_Click(object sender, EventArgs e)
        {
            DownloadingItem item = new DownloadingItem(txtUrl.Text, null);
            try
            {
                item.GetFileLength();
                if (item.FileLength > 0)
                {
                    switch (MessageBoxEx.Show(string.Format("大小: {0:N0} Bytes\r\n更新时间: {1:yyyy年M月d日 HH:mm:ss}\r\n是否立即开始下载文件？", item.FileLength, item.LastModified), "確認", MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                        #if NET6_0_OR_GREATER
                            Process.Start(new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                FileName = txtUrl.Text,
                            });
                        #else
                            Process.Start(txtUrl.Text);
                        #endif
                            return;

                        case DialogResult.No:
                            return;
                    }
                }
                else
                {
                    MessageBoxEx.Show("文件不存在。");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("错误：" + ex.Message);
            }
        }

        private void FrmPatcher_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (patchThread != null && patchThread.IsAlive)
            {
                patchThread.Interrupt();
            }
            ConfigManager.Reload();
            WcR2Config.Default.PatcherSettings.Clear();
            foreach (PatcherSetting item in comboBoxEx1.Items)
            {
                WcR2Config.Default.PatcherSettings.Add(item);
            }
            ConfigManager.Save();
        }

        private void NewFile(BinaryReader reader, string fileName, string patchDir)
        {
            string tmpFile = Path.Combine(patchDir, fileName);
            string dir = Path.GetDirectoryName(tmpFile);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void buttonXOpen1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "打开补丁文件";
            dlg.Filter = "打开补丁 (*.patch;*.exe)|*.patch;*.exe";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtPatchFile.Text = dlg.FileName;
            }
        }

        private void buttonXOpen2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择冒险岛文件夹。";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtMSFolder.Text = dlg.SelectedPath;
            }
        }

        private void buttonXPatch_Click(object sender, EventArgs e)
        {
            if (patchThread != null)
            {
                if (waiting)
                {
                    waitHandle.Set();
                    waiting = false;
                    return;
                }
                else
                {
                    MessageBoxEx.Show("补丁已经进行中。");
                    return;
                }
            }
            compareFolder = null;
            if (chkCompare.Checked)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "请选择导出对比结果的文件夹。";
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                compareFolder = dlg.SelectedPath;
            }

            patchFile = txtPatchFile.Text;
            msFolder = txtMSFolder.Text;
            prePatch = chkPrePatch.Checked;
            deadPatch = chkDeadPatch.Checked;

            patchThread = new Thread(() => ExecutePatch(patchFile, msFolder, prePatch));
            patchThread.Priority = ThreadPriority.Highest;
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            waiting = false;
            patchThread.Start();
            panelEx2.Visible = true;
            expandablePanel2.Height = 340;
        }

        string patchFile;
        string msFolder;
        string compareFolder;
        bool prePatch;
        bool deadPatch;
        string htmlFilePath;
        FileStream htmlFile;
        StreamWriter sw;
        Dictionary<Wz_Type, List<PatchPartContext>> typedParts;

        private void ExecutePatch(string patchFile, string msFolder, bool prePatch)
        {
            WzPatcher patcher = null;
            advTreePatchFiles.Nodes.Clear();
            txtNotice.Clear();
            txtPatchState.Clear();
            this.loggingFileName = Path.Combine(msFolder, $"wcpatcher_{DateTime.Now:yyyyMMdd_HHmmssfff}.log");
            try
            {
                patcher = new WzPatcher(patchFile);
                patcher.NoticeEncoding = this.PatcherNoticeEncoding ?? Encoding.Default;
                patcher.PatchingStateChanged += new EventHandler<PatchingEventArgs>(patcher_PatchingStateChanged);
                AppendStateText($"补丁文件名: {patchFile}\r\n");
                AppendStateText("补丁确认中...");
                patcher.OpenDecompress();
                AppendStateText("完成\r\n");
                if (prePatch)
                {
                    AppendStateText("补丁准备中... \r\n");
                    long decompressedSize = patcher.PrePatch();
                    if (patcher.IsKMST1125Format.Value)
                    {
                        AppendStateText("补丁类型: KMST1125\r\n");
                        if (patcher.OldFileHash != null)
                        {
                            AppendStateText($"补丁前确认校验和的文件个数: {patcher.OldFileHash.Count}\r\n");
                        }
                    }
                    AppendStateText(string.Format("文件大小: {0:N0} 字节...\r\n", decompressedSize));
                    AppendStateText(string.Format("补丁的文件个数: {0}...\r\n",
                        patcher.PatchParts == null ? -1 : patcher.PatchParts.Count));
                    txtNotice.Text = patcher.NoticeText;
                    foreach (PatchPartContext part in patcher.PatchParts)
                    {
                        advTreePatchFiles.Nodes.Add(CreateFileNode(part));
                    }
                    advTreePatchFiles.Enabled = true;
                    AppendStateText("请在选择补丁的文件后按下补丁按钮。\r\n");
                    waiting = true;
                    waitHandle.WaitOne();
                    advTreePatchFiles.Enabled = false;
                    patcher.PatchParts.Clear();
                    for (int i = 0, j = advTreePatchFiles.Nodes.Count; i < j; i++)
                    {
                        if (advTreePatchFiles.Nodes[i].Checked)
                        {
                            patcher.PatchParts.Add(advTreePatchFiles.Nodes[i].Tag as PatchPartContext);
                        }
                        advTreePatchFiles.Nodes[i].Enabled = false;
                    }
                    patcher.PatchParts.Sort((part1, part2) => part1.Offset.CompareTo(part2.Offset));
                }
                AppendStateText("更新中...\r\n");
                DateTime time = DateTime.Now;
                patcher.Patch(msFolder);
                if (sw != null)
                {
                    sw.WriteLine("</table>");
                    sw.WriteLine("</p>");

                    //html结束
                    sw.WriteLine("</body>");
                    sw.WriteLine("</html>");

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
                AppendStateText("完成\r\n");
                TimeSpan interval = DateTime.Now - time;
                MessageBoxEx.Show(this, "更新完成: 用时 " + interval.ToString(), "Patcher");
            }
            catch (ThreadAbortException)
            {
                MessageBoxEx.Show("补丁已中断。", "Patcher");
            }
            catch (ThreadInterruptedException)
            {
                MessageBoxEx.Show("补丁已中断。", "Patcher");
            }
            catch (UnauthorizedAccessException ex)
            {
                // File IO permission error
                MessageBoxEx.Show(this, ex.ToString(), "Patcher");
            }
            catch (Exception ex)
            {
                AppendStateText(ex.ToString());
                MessageBoxEx.Show(this, ex.ToString(), "Patcher"); 
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
                try
                {
                    if (htmlFile != null)
                    {
                        htmlFile.Flush();
                        htmlFile.Close();
                    }
                }
                catch
                {
                }
                htmlFilePath = null;

                if (patcher != null)
                {
                    patcher.Close();
                    patcher = null;
                }
                patchThread = null;
                waitHandle = null;
                GC.Collect();

                panelEx2.Visible = false;
                expandablePanel2.Height = 157;
            }
        }

        private void patcher_PatchingStateChanged(object sender, PatchingEventArgs e)
        {
            switch (e.State)
            {
                case PatchingState.PatchStart:
                    AppendStateText("[" + e.Part.FileName + "] 更新中\r\n");
                    break;
                case PatchingState.VerifyOldChecksumBegin:
                    AppendStateText("  补丁前确认校验和...");
                    progressBarX1.Maximum = (int)e.Part.OldFileLength;
                    break;
                case PatchingState.VerifyOldChecksumEnd:
                    AppendStateText("  完成\r\n");
                    break;
                case PatchingState.VerifyNewChecksumBegin:
                    AppendStateText("  补丁后确认校验和...");
                    break;
                case PatchingState.VerifyNewChecksumEnd:
                    AppendStateText("  完成\r\n");
                    break;
                case PatchingState.TempFileCreated:
                    AppendStateText("  开始写入临时文件...\r\n");
                    progressBarX1.Maximum = e.Part.NewFileLength;
                    break;
                case PatchingState.TempFileBuildProcessChanged:
                    progressBarX1.Value = (int)e.CurrentFileLength;
                    progressBarX1.Text = string.Format("{0:N0}/{1:N0}", e.CurrentFileLength, e.Part.NewFileLength);
                    break;
                case PatchingState.TempFileClosed:
                    AppendStateText("  临时文件写入完成。\r\n");
                    progressBarX1.Value = 0;
                    progressBarX1.Maximum = 0;
                    progressBarX1.Text = string.Empty;

                    if (!string.IsNullOrEmpty(this.compareFolder)
                        && e.Part.Type == 1
                        && Path.GetExtension(e.Part.FileName).Equals(".wz", StringComparison.OrdinalIgnoreCase)
                        && !Path.GetFileName(e.Part.FileName).Equals("list.wz", StringComparison.OrdinalIgnoreCase))
                    {
                        Wz_Structure wznew = new Wz_Structure();
                        Wz_Structure wzold = new Wz_Structure();
                        try
                        {
                            AppendStateText("  文件对比...\r\n");
                            EasyComparer comparer = new EasyComparer();
                            comparer.OutputPng = chkOutputPng.Checked;
                            comparer.OutputAddedImg = chkOutputAddedImg.Checked;
                            comparer.OutputRemovedImg = chkOutputRemovedImg.Checked;
                            comparer.EnableDarkMode = chkEnableDarkMode.Checked;
                            comparer.saveSkillTooltip = chkSaveSkillTooltip.Checked;
                            comparer.saveItemTooltip = chkSaveItemTooltip.Checked;
                            comparer.saveEqpTooltip = chkSaveEqpTooltip.Checked;
                            comparer.saveMobTooltip = chkSaveMobTooltip.Checked;
                            comparer.saveNpcTooltip = chkSaveNpcTooltip.Checked;
                            comparer.Comparer.PngComparison = (WzPngComparison)cmbComparePng.SelectedItem;
                            comparer.Comparer.ResolvePngLink = chkResolvePngLink.Checked;
                            comparer.PatchingStateChanged += new EventHandler<PatchingEventArgs>(patcher_PatchingStateChanged);
                            //wznew.Load(e.Part.TempFilePath, false);
                            //wzold.Load(e.Part.OldFilePath, false);
                            //comparer.EasyCompareWzFiles(wznew.wz_files[0], wzold.wz_files[0], this.compareFolder);
                            string tempDir = e.Part.TempFilePath;
                            while (Path.GetDirectoryName(tempDir) != msFolder)
                            {
                                tempDir = Path.GetDirectoryName(tempDir);
                            }
                            wznew.Load(e.Part.TempFilePath, false);
                            wzold.Load(e.Part.OldFilePath, false);
                            comparer.EasyCompareWzFiles(wznew.wz_files[0], wzold.wz_files[0], this.compareFolder);
                        }
                        catch (Exception ex)
                        {
                            txtPatchState.AppendText(ex.ToString());
                        }
                        finally
                        {
                            wznew.Clear();
                            wzold.Clear();
                            GC.Collect();
                        }

                        if (this.deadPatch && typedParts[e.Part.WzType].Count == ((WzPatcher)sender).PatchParts.Where(part => part.WzType == e.Part.WzType).Count())
                        {
                            foreach (PatchPartContext part in typedParts[e.Part.WzType].Where(part => part.Type == 1))
                            {
                                ((WzPatcher)sender).SafeMove(part.TempFilePath, part.OldFilePath);
                            }
                            AppendStateText("  文件应用...\r\n");
                        }
                    }

                    if (string.IsNullOrEmpty(this.compareFolder) && this.deadPatch && e.Part.Type == 1 && sender is WzPatcher patcher)
                    {
                        if (patcher.IsKMST1125Format.Value)
                        {
                            // TODO: we should build the file dependency tree to make sure all old files could be overridden safely.
                            AppendStateText("  (即时补丁) 连接文件应用...\r\n");
                        }
                        else
                        {
                            patcher.SafeMove(e.Part.TempFilePath, e.Part.OldFilePath);
                            AppendStateText("  (即时补丁) 文件应用...\r\n");
                        }
                    }
                    break;
                case PatchingState.CompareStarted:
                    progressBarX1.Maximum = e.Part.NewFileLength;
                    break;
                case PatchingState.CompareProcessChanged:
                    progressBarX1.Value = (int)e.CurrentFileLength;
                    progressBarX1.Text = string.Format("{0:N0}/{1:N0}", e.CurrentFileLength, e.Part.NewFileLength);
                    break;
                case PatchingState.CompareFinished:
                    progressBarX1.Value = 0;
                    progressBarX1.Maximum = 0;
                    progressBarX1.Text = string.Empty;
                    break;
                case PatchingState.PrepareVerifyOldChecksumBegin:
                    AppendStateText($"确认更新前校验和: {e.Part.FileName}");
                    break;
                case PatchingState.PrepareVerifyOldChecksumEnd:
                    AppendStateText(" 完成\r\n");
                    break;
                case PatchingState.ApplyFile:
                    AppendStateText($"文件应用: {e.Part.FileName}\r\n");
                    break;
            }
        }

        private void AppendStateText(string text)
        {
            try
            {
                this.Invoke((Action<string>)(t => { this.txtPatchState.AppendText(t); }), text);
            }
            catch (Exception ex)
            {
                ;
            }
            if (this.loggingFileName != null)
            {
                File.AppendAllText(this.loggingFileName, text, Encoding.UTF8);
            }
        }

        private Node CreateFileNode(PatchPartContext part)
        {
            Node node = new Node(part.FileName) { CheckBoxVisible = true, Checked = true };
            ElementStyle style = new ElementStyle();
            style.TextAlignment = eStyleTextAlignment.Far;
            switch (part.Type)
            {
                case 0: node.Cells.Add(new Cell("新增", style)); break;
                case 1: node.Cells.Add(new Cell("变更", style)); break;
                case 2: node.Cells.Add(new Cell("删除", style)); break;
                default: node.Cells.Add(new Cell(part.Type.ToString(), style)); break;
            }
            node.Cells.Add(new Cell(part.NewFileLength.ToString("n0"), style));
            node.Cells.Add(new Cell(part.NewChecksum.ToString("x8"), style));
            node.Cells.Add(new Cell(part.OldChecksum?.ToString("x8"), style));
            if (part.Type == 1)
            {
                string text = string.Format("{0}|{1}|{2}|{3}", part.Action0, part.Action1, part.Action2, part.DependencyFiles.Count);
                node.Cells.Add(new Cell(text, style));
            }
            node.Tag = part;
            return node;
        }

        private void buttonXOpen3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "打开补丁文件";
            dlg.Filter = "补丁文件 (*.patch;*.exe)|*.patch;*.exe";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtPatchFile2.Text = dlg.FileName;
            }
        }

        private void buttonXOpen4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "请选择冒险岛文件夹。";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtMSFolder2.Text = dlg.SelectedPath;
            }
        }

        private void buttonXCreate_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show(@"> 这是一个测试功能...
> 还没完成 所以请选择patch文件  exe补丁暂时懒得分离
> 没有检查原客户端版本 为了正确执行请预先确认
> 暂时不提供文件块的筛选或文件缺失提示
> 没优化 于是可能生成文件体积较大 但是几乎可以保证完整性
> 对于KMST1125后无法正常工作", "声明");

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "补丁文件 (*.patch)|*.patch";
            dlg.Title = "导出补丁文件";
            dlg.CheckFileExists = false;
            dlg.InitialDirectory = Path.GetDirectoryName(txtPatchFile2.Text);
            dlg.FileName = Path.GetFileNameWithoutExtension(txtPatchFile2.Text) + "_reverse.patch";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ReversePatcherBuilder builder = new ReversePatcherBuilder();
                    builder.msDir = txtMSFolder2.Text;
                    builder.patchFileName = txtPatchFile2.Text;
                    builder.outputFileName = dlg.FileName;
                    builder.Build();
                }
                catch(Exception ex)
                {
                }
            }
        }
    }
}