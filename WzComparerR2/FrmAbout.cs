using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using DevComponents.AdvTree;

namespace WzComparerR2
{
    public partial class FrmAbout : DevComponents.DotNetBar.Office2007Form
    {
        public FrmAbout()
        {
            InitializeComponent();
#if NET6_0_OR_GREATER
            // https://learn.microsoft.com/en-us/dotnet/core/compatibility/fx-core#controldefaultfont-changed-to-segoe-ui-9pt
            this.Font = new Font(new FontFamily("宋体"), 9f);
#endif

            this.lblClrVer.Text = string.Format("{0} ({1})", Environment.Version, Environment.Is64BitProcess ? "x64" : "x86");
            this.lblAsmVer.Text = GetAsmVersion().ToString();
            this.lblFileVer.Text = GetFileVersion().ToString();
            this.lblCopyright.Text = GetAsmCopyright().ToString();
            GetPluginInfo();
        }

        private Version GetAsmVersion()
        {
            return this.GetType().Assembly.GetName().Version;
        }

        private string GetFileVersion()
        {
            return this.GetAsmAttr<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? this.GetAsmAttr<AssemblyFileVersionAttribute>()?.Version;
        }

        private string GetAsmCopyright()
        {
            return this.GetAsmAttr<AssemblyCopyrightAttribute>()?.Copyright;
        }

        private void GetPluginInfo()
        {
            this.advTree1.Nodes.Clear();

            this.advTree1.Nodes.Add(new Node("CMS <font color=\"#808080\">v5.5.0</font>"));
            this.advTree1.Nodes.Add(new Node("[CMS] 中文版<font color=\"#808080\">Jancy</font>"));
            foreach (var contribution in new[]
{
                Tuple.Create("[KMS] 新增各种功能，最终翻译", "朴贤民"),
                Tuple.Create("[KMS] 短语翻译", "舒林猫"),
                Tuple.Create("[KMS] 短语错误报告", "인소야닷컴 실버"),
                Tuple.Create("[KMS] 短语错误报告", "jusir_@naver.com"),
                Tuple.Create("[KMS] 装备提示框错误报告", "@Sunaries"),
                Tuple.Create("[KMS] 不可重复佩戴短语错误报告", "인소야닷컴 진류"),
                Tuple.Create("[KMS] 新增纸娃娃导出功能", "@craftingmod"),
                Tuple.Create("[KMS] 纸娃娃加载错误报告", "인소야닷컴 일감"),
                Tuple.Create("[KMS] 导出文件时名称规则错误报告", "@mabooky"),
                Tuple.Create("[KMS] 纸娃娃高等翼人耳朵错误报告", "메이플인벤 누리신드롬"),
                Tuple.Create("[KMS] 各种错误报告, 提供GMS信息", "@Sunaries"),
                Tuple.Create("[KMS] 可使用职业短语错误提供", "@tanyoucai"),
                Tuple.Create("[KMS] 任务状态粒子未应用错误提供", "메이플인벤 펄더"),
                Tuple.Create("[KMS] 纸娃娃错误提供", "@giraffebin"),
                Tuple.Create("[KMS] 短语、提示框位置错误修复及提供，新增保存窗口大小功能、炼狱黑客支援", "@OniOniOn-"),
                Tuple.Create("[KMS] 与补丁一起对比时错误报告", "@lowrt"),
                Tuple.Create("[KMS] 纸娃娃全部导出错误报告", "@pid011"),
                Tuple.Create("[KMS] 新增提示框相关功能，错误修复及报告", "@sh-cho"),
                Tuple.Create("[KMS] 新增勋章预览错误报告，脚本连接地图功能", "@seotbeo"),
                Tuple.Create("[KMS] 实现纸娃娃混合颜色组合方法", "snlt7d"),
            })
            {
                string nodeTxt = string.Format("{0} <font color=\"#808080\">{1}</font>",
                        contribution.Item1,
                        contribution.Item2);
                Node node = new Node(nodeTxt);
                this.advTree1.Nodes.Add(node);
            }

            if (PluginBase.PluginManager.LoadedPlugins.Count > 0)
            {
                foreach (var plugin in PluginBase.PluginManager.LoadedPlugins)
                {
                    string nodeTxt = string.Format("{0} <font color=\"#808080\">{1} ({2})</font>",
                        plugin.Instance.Name,
                        plugin.Instance.Version,
                        plugin.Instance.FileVersion);
                    Node node = new Node(nodeTxt);
                    this.advTree1.Nodes.Add(node);
                }
            }
            else
            {
                string nodeTxt = "<font color=\"#808080\">连接的插件不存在</font>";
                Node node = new Node(nodeTxt);
                this.advTree1.Nodes.Add(node);
            }
        }

        private T GetAsmAttr<T>()
        {
            object[] attr = this.GetType().Assembly.GetCustomAttributes(typeof(T), true);
            if (attr != null && attr.Length > 0)
            {
                return (T)attr[0];
            }
            return default(T);
        }
    }
}