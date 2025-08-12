using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Resource = CharaSimResource.Resource;
using WzComparerR2.Common;
using WzComparerR2.CharaSim;
using WzComparerR2.WzLib;
using WzComparerR2.Comparer;
using System.Text.RegularExpressions;

namespace WzComparerR2.CharaSimControl
{
    public class SkillTooltipRender2 : TooltipRender
    {
        public SkillTooltipRender2()
        {
        }

        public Skill Skill { get; set; }

        public override object TargetItem
        {
            get { return this.Skill; }
            set { this.Skill = value as Skill; }
        }

        public bool ShowProperties { get; set; } = true;
        public bool ShowDelay { get; set; }
        public bool ShowReqSkill { get; set; } = true;
        public bool DisplayCooltimeMSAsSec { get; set; } = true;
        public bool DisplayPermyriadAsPercent { get; set; } = true;
        public bool IgnoreEvalError { get; set; } = false;
        public bool IsWideMode { get; set; } = true;
        public bool DoSetDiffColor { get; set; } = false;
        public Dictionary<string, List<string>> diffSkillTags = new Dictionary<string, List<string>>();
        public Wz_Node wzNode { get; set; } = null;

        public TooltipRender LinkRidingGearRender { get; set; }

        public override Bitmap Render()
        {
            if (this.Skill == null)
            {
                return null;
            }

            CanvasRegion region = this.IsWideMode ? CanvasRegion.Wide : CanvasRegion.Original;

            int picHeight;
            List<int> splitterH;
            Bitmap originBmp = RenderSkill(region, out picHeight, out splitterH);
            Bitmap ridingGearBmp = null;

            int vehicleID = Skill.VehicleID;
            if (vehicleID == 0)
            {
                vehicleID = PluginBase.PluginManager.FindWz(string.Format(@"Skill\RidingSkillInfo.img\{0:D7}\vehicleID", Skill.SkillID)).GetValueEx<int>(0);
            }
            if (vehicleID != 0)
            {
                Wz_Node imgNode = PluginBase.PluginManager.FindWz(string.Format(@"Character\TamingMob\{0:D8}.img", vehicleID));
                if (imgNode != null)
                {
                    Gear gear = Gear.CreateFromNode(imgNode, path => PluginBase.PluginManager.FindWz(path));
                    if (gear != null)
                    {
                        ridingGearBmp = RenderLinkRidingGear(gear);
                    }
                }
            }

            Size totalSize = new Size(originBmp.Width, picHeight);
            Point ridingGearOrigin = Point.Empty;

            if (ridingGearBmp != null)
            {
                totalSize.Width += ridingGearBmp.Width;
                totalSize.Height = Math.Max(picHeight, ridingGearBmp.Height);
                ridingGearOrigin.X = originBmp.Width;
            }

            Bitmap tooltip = new Bitmap(totalSize.Width, totalSize.Height);
            Graphics g = Graphics.FromImage(tooltip);

            //绘制背景区域
            GearGraphics.DrawNewTooltipBack(g, 0, 0, originBmp.Width, picHeight);
            if (splitterH != null && splitterH.Count > 0)
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                foreach (var y in splitterH)
                {
                    DrawV6SkillDotline(g, region.SplitterX1, region.SplitterX2, y);
                }
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            }

            //复制图像
            g.DrawImage(originBmp, 0, 0, new Rectangle(0, 0, originBmp.Width, picHeight), GraphicsUnit.Pixel);

            //左上角
            g.DrawImage(Resource.UIToolTip_img_Skill_Frame_cover, 3, 3);

            if (this.ShowObjectID)
            {
                GearGraphics.DrawGearDetailNumber(g, 3, 3, Skill.SkillID.ToString("d7"), true);
            }

            if (ridingGearBmp != null)
            {
                g.DrawImage(ridingGearBmp, ridingGearOrigin.X, ridingGearOrigin.Y,
                    new Rectangle(Point.Empty, ridingGearBmp.Size), GraphicsUnit.Pixel);
            }

            if (originBmp != null)
                originBmp.Dispose();
            if (ridingGearBmp != null)
                ridingGearBmp.Dispose();

            g.Dispose();
            return tooltip;
        }

        private Bitmap RenderSkill(CanvasRegion region, out int picH, out List<int> splitterH)
        {
            Bitmap bitmap = new Bitmap(region.Width, DefaultPicHeight);
            Graphics g = Graphics.FromImage(bitmap);
            StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone();
            var v6SkillSummaryFontColorTable = new Dictionary<string, Color>()
            {
                { "c", GearGraphics.SkillSummaryOrangeTextColor },
            };

            picH = 0;
            splitterH = new List<int>();

            //获取文字
            StringResult sr;
            if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(Skill.SkillID, out sr))
            {
                sr = new StringResultSkill();
                sr.Name = "(null)";
            }

            //绘制技能名称
            format.Alignment = StringAlignment.Center;
            if (IsKoreanStringPresent(sr.Name))
            {
                TextRenderer.DrawText(g, sr.Name, GearGraphics.KMSItemNameFont, new Point(bitmap.Width, 10), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix);
            }
            else
            {
                TextRenderer.DrawText(g, sr.Name, GearGraphics.ItemNameFont2, new Point(bitmap.Width, 10), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix);
            }
            //绘制图标
            if (Skill.Icon.Bitmap != null)
            {
                picH = 33;
                g.DrawImage(Resource.UIToolTip_img_Skill_Frame_iconBackgrnd, 13, picH - 2);
                g.DrawImage(GearGraphics.EnlargeBitmap(Skill.Icon.Bitmap),
                15 + (1 - Skill.Icon.Origin.X) * 2,
                picH + (33 - Skill.Icon.Bitmap.Height) * 2);
            }

            // for 6th job skills
            if (Skill.Origin)
            {
                g.DrawImage(Resource.UIWindow2_img_Skill_skillTypeIcon_origin, 16, 11);
            }

            //绘制desc
            picH = 35;
            if (Skill.HyperStat)
                GearGraphics.DrawString(g, "[最高等级 : " + Skill.MaxLevel + "]", GearGraphics.ItemDetailFont2, region.LevelDescLeft, region.TextRight, ref picH, 16);
            else if (!Skill.PreBBSkill)
                GearGraphics.DrawString(g, "[最高等级 : " + Skill.MaxLevel + "]", GearGraphics.ItemDetailFont2, region.SkillDescLeft, region.TextRight, ref picH, 16);

            if (sr.Desc != null)
            {
                string hdesc = SummaryParser.GetSkillSummary(sr.Desc, Skill.Level, Skill.Common, SummaryParams.Default);
                //string hStr = SummaryParser.GetSkillSummary(skill, skill.Level, sr, SummaryParams.Default);
                if (IsKoreanStringPresent(hdesc))
                {
                    GearGraphics.DrawString(g, hdesc, GearGraphics.KMSItemDetailFont, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
                }
                else
                {
                    GearGraphics.DrawString(g, hdesc, GearGraphics.ItemDetailFont, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
                }
            }
            if (Skill.TimeLimited)
            {
                DateTime time = DateTime.Now.AddDays(7d);
                string expireStr = time.ToString("有效时间 : yyyy年 M月 d日 HH时 mm分");
                GearGraphics.DrawString(g, "#c" + expireStr + "#", GearGraphics.ItemDetailFont2, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
            }
            if (Skill.RelationSkill != null)
            {
                StringResult sr2 = null;
                if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(Skill.RelationSkill.Item1, out sr2))
                {
                    sr2 = new StringResultSkill();
                    sr2.Name = "(null)";
                }
                DateTime time = DateTime.Now.AddMinutes(Skill.RelationSkill.Item2);
                string expireStr = time.ToString("有效时间 : yyyy年 M月 d日 H时 m分");
                GearGraphics.DrawString(g, "#c" + sr2.Name + "的 " + expireStr + "#", GearGraphics.ItemDetailFont2, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
            }
            if (Skill.IsPetAutoBuff)
            {
                GearGraphics.DrawString(g, "#c可登记宠物自动增益技能#", GearGraphics.ItemDetailFont2, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
            }
            if (Skill.reqGuildLv > 0)
            {
                GearGraphics.DrawString(g, "#c要求: 公会" + Skill.reqGuildLv + "级以上#", GearGraphics.ItemDetailFont2, v6SkillSummaryFontColorTable, Skill.Icon.Bitmap == null ? region.LevelDescLeft : region.SkillDescLeft, region.TextRight, ref picH, 16);
            }
            /*if (Skill.ReqLevel > 0)
            {
                GearGraphics.DrawString(g, "#c[要求等级：" + Skill.ReqLevel.ToString() + "]#", GearGraphics.ItemDetailFont2, region.SkillDescLeft, region.TextRight, ref picH, 16);
            }
            if (Skill.ReqAmount > 0)
            {
                GearGraphics.DrawString(g, "#c" + ItemStringHelper.GetSkillReqAmount(Skill.SkillID, Skill.ReqAmount) + "#", GearGraphics.ItemDetailFont2, region.SkillDescLeft, region.TextRight, ref picH, 16);
            }*/
            picH += 13;

            //delay rendering v6 splitter
            picH = Math.Max(picH, 114);
            splitterH.Add(picH);
            picH += 15;

            if (Skill.Level > 0)
            {
                string hStr = null; ;
                // 스킬 변경점에 초록색 칠하기
                if (DoSetDiffColor)
                {
                    //code from SummaryParser
                    string h = null;
                    if (Skill.PreBBSkill) //用level声明的技能
                    {
                        string hs;
                        if (Skill.Common.TryGetValue("hs", out hs))
                        {
                            h = sr[hs];
                        }
                        else if (sr.SkillH.Count >= Skill.Level)
                        {
                            h = sr.SkillH[Skill.Level - 1];
                        }
                    }
                    else
                    {
                        if (sr.SkillH.Count > 0)
                        {
                            h = sr.SkillH[0];
                        }
                    }
                    if (diffSkillTags.ContainsKey(Skill.SkillID.ToString()))
                    {
                        foreach (var tags in diffSkillTags[Skill.SkillID.ToString()])
                        {
                            h = (h == null ? null : Regex.Replace(h, "#" + tags + @"([^a-zA-Z0-9])", "#g#" + tags + "#$1"));
                        }
                    }
                    if (Skill.SkillID / 100000 == 4000)
                    {
                        if (Skill.VSkillValue == 2) Skill.Level = 60;
                        if (Skill.VSkillValue == 1) Skill.Level = 30;
                    }
                    hStr = SummaryParser.GetSkillSummary(h, Skill.Level, Skill.Common, SummaryParams.Default, new SkillSummaryOptions
                    {
                        ConvertCooltimeMS = this.DisplayCooltimeMSAsSec,
                        ConvertPerM = this.DisplayPermyriadAsPercent,
                        IgnoreEvalError = this.IgnoreEvalError,
                    });
                }
                else
                {
                    hStr = SummaryParser.GetSkillSummary(Skill, Skill.Level, sr, SummaryParams.Default, new SkillSummaryOptions
                    {
                        ConvertCooltimeMS = this.DisplayCooltimeMSAsSec,
                        ConvertPerM = this.DisplayPermyriadAsPercent,
                        IgnoreEvalError = this.IgnoreEvalError,
                    });
                }
                GearGraphics.DrawString(g, "[现在等级 " + Skill.Level + "]", GearGraphics.ItemDetailFont, region.LevelDescLeft, region.TextRight, ref picH, 16);
                if (Skill.SkillID / 10000 / 1000 == 10 && Skill.Level == 1 && Skill.ReqLevel > 0)
                {
                    GearGraphics.DrawPlainText(g, "[要求等级: " + Skill.ReqLevel.ToString() + "级以上]", GearGraphics.ItemDetailFont2, GearGraphics.skillYellowColor, region.LevelDescLeft, region.TextRight, ref picH, 16);
                }
                if (hStr != null)
                {
                    if (IsKoreanStringPresent(hStr))
                    {
                        GearGraphics.DrawString(g, hStr, GearGraphics.KMSItemDetailFont, v6SkillSummaryFontColorTable, region.LevelDescLeft, region.TextRight, ref picH, 16);
                    }
                    else
                    {
                        GearGraphics.DrawString(g, hStr, GearGraphics.ItemDetailFont, v6SkillSummaryFontColorTable, region.LevelDescLeft, region.TextRight, ref picH, 16);
                    }
                }
            }

            if (Skill.Level < Skill.MaxLevel && !Skill.DisableNextLevelInfo)
            {
                string hStr = SummaryParser.GetSkillSummary(Skill, Skill.Level + 1, sr, SummaryParams.Default, new SkillSummaryOptions
                {
                    ConvertCooltimeMS = this.DisplayCooltimeMSAsSec,
                    ConvertPerM = this.DisplayPermyriadAsPercent,
                    IgnoreEvalError = this.IgnoreEvalError,
                });
                GearGraphics.DrawString(g, "[下次等级 " + (Skill.Level + 1) + "]", GearGraphics.ItemDetailFont, region.LevelDescLeft, region.TextRight, ref picH, 16);
                if (Skill.SkillID / 10000 / 1000 == 10 && (Skill.Level + 1) == 1 && Skill.ReqLevel > 0)
                {
                    GearGraphics.DrawPlainText(g, "[要求等级: " + Skill.ReqLevel.ToString() + "级以上]", GearGraphics.ItemDetailFont2, GearGraphics.skillYellowColor, region.LevelDescLeft, region.TextRight, ref picH, 16);
                }
                if (hStr != null)
                {
                    if (IsKoreanStringPresent(hStr))
                    {
                        GearGraphics.DrawString(g, hStr, GearGraphics.KMSItemDetailFont, v6SkillSummaryFontColorTable, region.LevelDescLeft, region.TextRight, ref picH, 16);
                    }
                    else
                    {
                        GearGraphics.DrawString(g, hStr, GearGraphics.ItemDetailFont, v6SkillSummaryFontColorTable, region.LevelDescLeft, region.TextRight, ref picH, 16);
                    }
                }
            }
            picH += 3;

            if (Skill.AddAttackToolTipDescSkill != 0)
            {
                //delay rendering v6 splitter
                splitterH.Add(picH);
                picH += 15;
                GearGraphics.DrawPlainText(g, "[组合技能]", GearGraphics.ItemDetailFont, Color.FromArgb(119, 204, 255), region.LevelDescLeft, region.TextRight, ref picH, 16);
                picH += 4;
                BitmapOrigin icon = new BitmapOrigin();
                Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", Skill.AddAttackToolTipDescSkill / 10000, Skill.AddAttackToolTipDescSkill));
                if (skillNode != null)
                {
                    Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz);
                    icon = skill.Icon;
                }
                if (icon.Bitmap != null)
                {
                    g.DrawImage(icon.Bitmap, 13 - icon.Origin.X, picH + 32 - icon.Origin.Y);
                }
                string skillName;
                if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(Skill.AddAttackToolTipDescSkill, out sr))
                {
                    skillName = sr.Name;
                }
                else
                {
                    skillName = Skill.AddAttackToolTipDescSkill.ToString();
                }
                picH += 10;
                GearGraphics.DrawString(g, skillName, GearGraphics.ItemDetailFont, region.LinkedSkillNameLeft, region.TextRight, ref picH, 16);
                picH += 6;
                picH += 13;
            }

            if (Skill.AssistSkillLink != 0)
            {
                //delay rendering v6 splitter
                splitterH.Add(picH);
                picH += 15;
                GearGraphics.DrawPlainText(g, "[助攻技能]", GearGraphics.ItemDetailFont, GearGraphics.SkillSummaryOrangeTextColor, region.LevelDescLeft, region.TextRight, ref picH, 16);
                picH += 4;
                BitmapOrigin icon = new BitmapOrigin();
                Wz_Node skillNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\{0}.img\skill\{1}", Skill.AssistSkillLink / 10000, Skill.AssistSkillLink));
                if (skillNode != null)
                {
                    Skill skill = Skill.CreateFromNode(skillNode, PluginBase.PluginManager.FindWz);
                    icon = skill.Icon;
                }
                if (icon.Bitmap != null)
                {
                    g.DrawImage(icon.Bitmap, 13 - icon.Origin.X, picH + 32 - icon.Origin.Y);
                }
                string skillName;
                if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(Skill.AssistSkillLink, out sr))
                {
                    skillName = sr.Name;
                }
                else
                {
                    skillName = Skill.AssistSkillLink.ToString();
                }
                picH += 10;
                GearGraphics.DrawString(g, skillName, GearGraphics.ItemDetailFont, region.LinkedSkillNameLeft, region.TextRight, ref picH, 16);
                picH += 6;
                picH += 13;
            }

            List<string> skillDescEx = new List<string>();
            if (ShowProperties)
            {
                List<string> attr = new List<string>();
                if (Skill.Invisible || Skill.invisible_tw)
                {
                    attr.Add("隐藏技能");
                }
                if (Skill.applyHyper)
                {
                    attr.Add("应用超级技能");
                }
                if (Skill.areaAttack)
                {
                    attr.Add("区域攻击");
                }
                if (Skill.chainAttack)
                {
                    attr.Add("连锁攻击");
                }
                if (Skill.isFieldMovingLimit)
                {
                    attr.Add("地区移动限制");
                }
                if (Skill.CombatOrders)
                {
                    attr.Add("应用战斗命令");
                }
                if (Skill.microCooltime)
                {
                    attr.Add("小型冷却时间");
                }
                if (Skill.disabledDuringAction)
                {
                    attr.Add("动作期间无效");
                }
                if (Skill.ignoreActionWhenAddAttackProc)
                {
                    attr.Add("攻击保护时无视动作");
                }
                if (Skill.notExtend)
                {
                    attr.Add("无法延长");
                }
                if (Skill.NotRemoved)
                {
                    attr.Add("无法移除");
                }
                if (Skill.notCanceled)
                {
                    attr.Add("无法取消");
                }
                if (Skill.cannotCancelByRButton)
                {
                    attr.Add("右击按钮无法取消");
                }
                if (Skill.ignoreSpecialCore)
                {
                    attr.Add("无视特殊核心");
                }
                if (Skill.blockBattleStats)
                {
                    attr.Add("禁用战斗属性");
                }
                if (Skill.applyPassiveLvUp)
                {
                    attr.Add("应用被动升级");
                }
                if (Skill.preservedOnDead)
                {
                    attr.Add("死亡时保留");
                }
                if (Skill.notIncBuffDuration)
                {
                    attr.Add("无法增加增益时间");
                }
                if (Skill.notCooltimeReset)
                {
                    attr.Add("无法重置冷却时间");
                }
                if (Skill.notCooltimeReduce)
                {
                    attr.Add("无法减少冷却时间");
                }
                if (Skill.showStackCooltimeNum)
                {
                    attr.Add("显示叠加冷却时间");
                }
                if (Skill.canJobRidingUse)
                {
                    attr.Add("可职业骑乘使用");
                }
                if (Skill.canRidingUse)
                {
                    attr.Add("可骑乘使用");
                }
                if (Skill.canNotStealableSkill)
                {
                    attr.Add("无法复制");
                }
                if (Skill.preloadEff)
                {
                    attr.Add("预载效果");
                }
                if (Skill.fixEffectCanceled)
                {
                    attr.Add("可取消固定效果");
                }
                if (Skill.ignoreCounter)
                {
                    attr.Add("无视反制");
                }
                if (Skill.ignoreUserAttackSuccess)
                {
                    attr.Add("无视使用者攻击");
                }
                if (Skill.footholdAffectedArea)
                {
                    attr.Add("区域受平台影响");
                }
                if (Skill.footholdInstallSummoned)
                {
                    attr.Add("平台设置召唤");
                }
                if (Skill.addAttackCoolTime)
                {
                    attr.Add("增加攻击冷却时间");
                }
                if (Skill.notRemoveEffectByCancelSkill)
                {
                    attr.Add("取消技能保留效果");
                }
                if (Skill.RemoveBuffByRemoveSummon)
                {
                    attr.Add("取消召唤保留效果");
                }
                if (Skill.maintainSplitSummoned)
                {
                    attr.Add("维持分离召唤");
                }
                if (Skill.isCancelableBuff)
                {
                    attr.Add("可取消增益");
                }
                if (Skill.cancelBuffByForbiddenField)
                {
                    attr.Add("禁区取消增益");
                } 
                if (Skill.notResetDarkSight)
                {
                    attr.Add("无法重置隐身术");
                }
                if (Skill.resetDarkSightOnShoot)
                {
                    attr.Add("重置隐身术");
                }
                if (Skill.ignoreResetDarkSightOnAttack)
                {
                    attr.Add("无视重置隐身术");
                }
                if (Skill.canSummonedAttackOnDarkSight)
                {
                    attr.Add("隐身术可召唤");
                }
                if (Skill.showSummonedBuffIcon)
                {
                    attr.Add("显示召唤增益图标");
                }
                if (Skill.jobShield)
                {
                    attr.Add("职业盾牌");
                }
                if (Skill.rectBasedOnTarget)
                {
                    attr.Add("基于目标校正");
                }
                if (Skill.normalAttackAtBulletEmpty)
                {
                    attr.Add("无发射体时普通攻击");
                }
                if (Skill.hideBullet)
                {
                    attr.Add("隐藏发射体");
                }
                if (Skill.noBulletApply)
                {
                    attr.Add("不应用发射体");
                }
                if (Skill.isUsableSylphdia)
                {
                    attr.Add("希比迪亚可用");
                }
                if (Skill.canOverlapOnMegasmasher)
                {
                    attr.Add("超能光束炮可叠加");
                }
                if (Skill.canOverlapOnFullBurst)
                {
                    attr.Add("全弹发射可叠加");
                }
                if (Skill.isUsableKaiserDragon)
                {
                    attr.Add("终极变形可用");
                }
                if (Skill.canGloryWingUse)
                {
                    attr.Add("荣耀之翼可用");
                }
                if (Skill.isUsableExpressionRiver)
                {
                    attr.Add("河流之地可用");
                }
                if (Skill.makeEventPointByMobDead)
                {
                    attr.Add("获得活动点数");
                }
                if (Skill.teleportVehicle)
                {
                    attr.Add("传送骑宠");
                }
                if (Skill.ableTeleportWorldmap)
                {
                    attr.Add("传送世界地图");
                }
                if (Skill.petPassive)
                {
                    attr.Add("宠物被动增益");
                }
                if (Skill.partyPassive)
                {
                    attr.Add("组队被动增益");
                }
                if (Skill.repeatDance)
                {
                    attr.Add("舞蹈动作");
                }
                if (Skill.infiniteTime)
                {
                    attr.Add("无限时间");
                }
                if (Skill.collabo)
                {
                    attr.Add("联名技能");
                }
                if (attr.Count > 0)
                {
                    skillDescEx.Add("#c" + string.Join("、", attr.ToArray()) + "#");
                }
            }

            if (ShowDelay && Skill.Action.Count > 0)
            {
                foreach (string action in Skill.Action)
                {
                    skillDescEx.Add("#c[技能延时] " + action + ": " + CharaSimLoader.GetActionDelay(action, this.wzNode) + " ms#");
                }
            }
            
            if (Skill.alertTime > 0)
            {
                skillDescEx.Add("#c[提示时间] " + Skill.alertTime + " ms#");
            }

            if (Skill.activeDelay > 0)
            {
                skillDescEx.Add("#c[启动延时] " + Skill.activeDelay + " ms#");
            }

            if (Skill.MasterLevel > 0 && Skill.MasterLevel < Skill.MaxLevel)
            {
                skillDescEx.Add("#c[初始掌握] " + Skill.MasterLevel + "级#");
            }

            if (ShowReqSkill && Skill.ReqSkill.Count > 0)
            {
                foreach (var kv in Skill.ReqSkill)
                {
                    string skillName;
                    if (this.StringLinker != null && this.StringLinker.StringSkill.TryGetValue(kv.Key, out sr))
                    {
                        skillName = sr.Name;
                    }
                    else
                    {
                        skillName = kv.Key.ToString();
                    }
                    skillDescEx.Add("#c[需要技能] " + skillName + ": " + kv.Value + "级以上#");
                }
            }

            if (Skill.LT.X != 0)
            {
                skillDescEx.Add("#c[技能范围] LT(左上): (" + Skill.LT.X + "," + Skill.LT.Y + ")" + " / " + "RB(右下): (" + Skill.RB.X + "," + Skill.RB.Y + ")");
                int LT = Math.Abs(Skill.LT.X) + Skill.RB.X;
                int RB = Math.Abs(Skill.LT.Y) + Skill.RB.Y;
                skillDescEx.Add("#c[范围] " + LT + " x " + RB);

            }

            if (Skill.makeMesoByMobDead > 0)
            {
                skillDescEx.Add("#c[掉落] " + Skill.makeMesoByMobDead + "金币#");
            }

            if (Skill.makeMesoByMobDead_reboot > 0)
            {
                skillDescEx.Add("#c[掉落] " + Skill.makeMesoByMobDead_reboot + "金币#");
            }

            if (skillDescEx.Count > 0)
            {
                //delay rendering v6 splitter
                splitterH.Add(picH);
                picH += 9;
                foreach (var descEx in skillDescEx)
                {
                    GearGraphics.DrawString(g, descEx, GearGraphics.ItemDetailFont, region.LevelDescLeft, region.TextRight, ref picH, 16);
                }
                picH += 3;
            }

            picH += 6;

            format.Dispose();
            g.Dispose();
            return bitmap;
        }
        private bool IsKoreanStringPresent(string checkString)
        {
            return checkString.Any(c => (c >= '\uAC00' && c <= '\uD7A3'));
        }
        private void DrawV6SkillDotline(Graphics g, int x1, int x2, int y)
        {
            // here's a trick that we won't draw left and right part because it looks the same as background border.
            var picCenter = Resource.UIToolTip_img_Skill_Frame_dotline_c;
            using (var brush = new TextureBrush(picCenter))
            {
                brush.TranslateTransform(x1, y);
                g.FillRectangle(brush, new Rectangle(x1, y, x2 - x1, picCenter.Height));
            }
        }

        private Bitmap RenderLinkRidingGear(Gear gear)
        {
            TooltipRender renderer = this.LinkRidingGearRender;
            if (renderer == null)
            {
                GearTooltipRender2 defaultRenderer = new GearTooltipRender2();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = false;
                renderer = defaultRenderer;
            }

            renderer.TargetItem = gear;
            return renderer.Render();
        }

        private class CanvasRegion
        {
            public int Width { get; private set; }
            public int TitleCenterX { get; private set; }
            public int SplitterX1 { get; private set; }
            public int SplitterX2 { get; private set; }
            public int SkillDescLeft { get; private set; }
            public int LinkedSkillNameLeft { get; private set; }
            public int LevelDescLeft { get; private set; }
            public int TextRight { get; private set; }

            public static CanvasRegion Original { get; } = new CanvasRegion()
            {
                Width = 290,
                TitleCenterX = 144,
                SplitterX1 = 4,
                SplitterX2 = 284,
                SkillDescLeft = 90,
                LinkedSkillNameLeft = 46,
                LevelDescLeft = 8,
                TextRight = 272,
            };

            public static CanvasRegion Wide { get; } = new CanvasRegion()
            {
                Width = 430,
                TitleCenterX = 215,
                SplitterX1 = 4,
                SplitterX2 = 424,
                SkillDescLeft = 92,
                LinkedSkillNameLeft = 49,
                LevelDescLeft = 13,
                TextRight = 411,
            };
        }
    }
}
