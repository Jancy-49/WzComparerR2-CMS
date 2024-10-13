﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Resource = CharaSimResource.Resource;
using WzComparerR2.PluginBase;
using WzComparerR2.WzLib;
using WzComparerR2.Common;
using WzComparerR2.CharaSim;

namespace WzComparerR2.CharaSimControl
{
    public class ItemTooltipRender2 : TooltipRender
    {
        public ItemTooltipRender2()
        {
        }

        private Item item;

        public Item Item
        {
            get { return item; }
            set { item = value; }
        }

        public override object TargetItem
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value as Item;
            }
        }


        public bool LinkRecipeInfo { get; set; }
        public bool LinkRecipeItem { get; set; }
        public bool ShowLevelOrSealed { get; set; }
        public bool ShowNickTag { get; set; }

        public TooltipRender LinkRecipeInfoRender { get; set; }
        public TooltipRender LinkRecipeGearRender { get; set; }
        public TooltipRender LinkRecipeItemRender { get; set; }
        public TooltipRender SetItemRender { get; set; }
        public TooltipRender CashPackageRender { get; set; }

        public override Bitmap Render()
        {
            if (this.item == null)
            {
                return null;
            }
            //绘制道具
            int picHeight;
            Bitmap itemBmp = RenderItem(out picHeight);
            Bitmap recipeInfoBmp = null;
            List<Bitmap> recipeItemBmps = new List<Bitmap>();
            Bitmap setItemBmp = null;
            Bitmap levelBmp = null;
            int levelHeight = 0;
            if (this.ShowLevelOrSealed)
            {
                levelBmp = RenderLevel(out levelHeight);
            }

            if (this.item.ItemID / 10000 == 910)
            {
                Wz_Node itemNode = PluginBase.PluginManager.FindWz(string.Format(@"Item\Special\{0:D4}.img\{1}", this.item.ItemID / 10000, this.item.ItemID));
                Wz_Node cashPackageNode = PluginBase.PluginManager.FindWz(string.Format(@"Etc\CashPackage.img\{0}", this.item.ItemID));
                CashPackage cashPackage = CashPackage.CreateFromNode(itemNode, cashPackageNode, PluginBase.PluginManager.FindWz);
                return RenderCashPackage(cashPackage);
            }

            Action<int> AppendGearOrItem = (int itemID) =>
            {
                int itemIDClass = itemID / 1000000;
                if (itemIDClass == 1) //通过ID寻找装备
                {
                    Wz_Node charaWz = PluginManager.FindWz(Wz_Type.Character);
                    if (charaWz != null)
                    {
                        string imgName = itemID.ToString("d8") + ".img";
                        foreach (Wz_Node node0 in charaWz.Nodes)
                        {
                            Wz_Node imgNode = node0.FindNodeByPath(imgName, true);
                            if (imgNode != null)
                            {
                                Gear gear = Gear.CreateFromNode(imgNode, path => PluginManager.FindWz(path));
                                if (gear != null)
                                {
                                    gear.Props[GearPropType.timeLimited] = 0;
                                    int tuc, tucCnt;
                                    if (Item.Props.TryGetValue(ItemPropType.addTooltip_tuc, out tuc) && Item.Props.TryGetValue(ItemPropType.addTooltip_tucCnt, out tucCnt))
                                    {
                                        Wz_Node itemWz = PluginManager.FindWz(Wz_Type.Item);
                                        if (itemWz != null)
                                        {
                                            string imgClass = (tuc / 10000).ToString("d4") + ".img\\" + tuc.ToString("d8") + "\\info";
                                            foreach (Wz_Node node1 in itemWz.Nodes)
                                            {
                                                Wz_Node infoNode = node1.FindNodeByPath(imgClass, true);
                                                if (infoNode != null)
                                                {
                                                    gear.Upgrade(infoNode, tucCnt);

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    recipeItemBmps.Add(RenderLinkRecipeGear(gear));
                                }

                                break;
                            }
                        }
                    }
                }
                else if (itemIDClass >= 2 && itemIDClass <= 5) //通过ID寻找道具
                {
                    Wz_Node itemWz = PluginManager.FindWz(Wz_Type.Item);
                    if (itemWz != null)
                    {
                        string imgClass = (itemID / 10000).ToString("d4") + ".img\\" + itemID.ToString("d8");
                        foreach (Wz_Node node0 in itemWz.Nodes)
                        {
                            Wz_Node imgNode = node0.FindNodeByPath(imgClass, true);
                            if (imgNode != null)
                            {
                                Item item = Item.CreateFromNode(imgNode, PluginManager.FindWz);
                                item.Props[ItemPropType.timeLimited] = 0;
                                if (item != null)
                                {
                                    recipeItemBmps.Add(RenderLinkRecipeItem(item));
                                }

                                break;
                            }
                        }
                    }
                }
            };

            //图纸相关
            int recipeID;
            if (this.item.Specs.TryGetValue(ItemSpecType.recipe, out recipeID))
            {
                int recipeSkillID = recipeID / 10000;
                Recipe recipe = null;
                //寻找配方
                Wz_Node recipeNode = PluginBase.PluginManager.FindWz(string.Format(@"Skill\Recipe_{0}.img\{1}", recipeSkillID, recipeID));
                if (recipeNode != null)
                {
                    recipe = Recipe.CreateFromNode(recipeNode);
                }
                //生成配方图像
                if (recipe != null)
                {
                    if (this.LinkRecipeInfo)
                    {
                        recipeInfoBmp = RenderLinkRecipeInfo(recipe);
                    }

                    if (this.LinkRecipeItem)
                    {
                        int itemID = recipe.MainTargetItemID;
                        AppendGearOrItem(itemID);
                    }
                }
            }

            int value;
            if (this.item.Props.TryGetValue(ItemPropType.dressUpgrade, out value))
            {
                int itemID = value;
                AppendGearOrItem(itemID);
            }
            if (this.item.Props.TryGetValue(ItemPropType.tamingMob, out value))
            {
                int itemID = value;
                AppendGearOrItem(itemID);
            }

            if (this.item.AddTooltips.Count > 0)
            {
                foreach (int itemID in item.AddTooltips)
                {
                    AppendGearOrItem(itemID);
                }
            }

            int setID;
            if (this.item.Props.TryGetValue(ItemPropType.setItemID, out setID))
            {
                SetItem setItem;
                if (CharaSimLoader.LoadedSetItems.TryGetValue(setID, out setItem))
                {
                    setItemBmp = RenderSetItem(setItem);
                }
            }

            //计算布局
            Size totalSize = new Size(itemBmp.Width, picHeight);
            Point recipeInfoOrigin = Point.Empty;
            List<Point> recipeItemOrigins = new List<Point>();
            Point setItemOrigin = Point.Empty;
            Point levelOrigin = Point.Empty;

            if (recipeItemBmps.Count > 0)
            {
                if (recipeInfoBmp != null)
                {
                    recipeItemOrigins.Add(new Point(totalSize.Width, 0));
                    recipeInfoOrigin.X = itemBmp.Width - recipeInfoBmp.Width;
                    recipeInfoOrigin.Y = picHeight;
                    totalSize.Width += recipeItemBmps[0].Width;
                    totalSize.Height = Math.Max(picHeight + recipeInfoBmp.Height, recipeItemBmps[0].Height);
                }
                else
                {
                    int itemCnt = recipeItemBmps.Count;
                    for (int i = 0; i < itemCnt; ++i)
                    {
                        recipeItemOrigins.Add(new Point(totalSize.Width, 0));
                        totalSize.Width += recipeItemBmps[i].Width;
                        totalSize.Height = Math.Max(picHeight, recipeItemBmps[i].Height);
                    }
                }
            }
            else if (recipeInfoBmp != null)
            {
                totalSize.Width += recipeInfoBmp.Width;
                totalSize.Height = Math.Max(picHeight, recipeInfoBmp.Height);
                recipeInfoOrigin.X = itemBmp.Width;
            }
            if (setItemBmp != null)
            {
                setItemOrigin = new Point(totalSize.Width, 0);
                totalSize.Width += setItemBmp.Width;
                totalSize.Height = Math.Max(totalSize.Height, setItemBmp.Height);
            }
            if (levelBmp != null)
            {
                levelOrigin = new Point(totalSize.Width, 0);
                totalSize.Width += levelBmp.Width;
                totalSize.Height = Math.Max(totalSize.Height, levelHeight);
            }

            //开始绘制
            Bitmap tooltip = new Bitmap(totalSize.Width, totalSize.Height);
            Graphics g = Graphics.FromImage(tooltip);

            if (itemBmp != null)
            {
                //绘制背景区域
                GearGraphics.DrawNewTooltipBack(g, 0, 0, itemBmp.Width, picHeight);
                //复制图像
                g.DrawImage(itemBmp, 0, 0, new Rectangle(0, 0, itemBmp.Width, picHeight), GraphicsUnit.Pixel);
                //左上角
                g.DrawImage(Resource.UIToolTip_img_Item_Frame2_cover, 3, 3);

                if (this.ShowObjectID)
                {
                    GearGraphics.DrawGearDetailNumber(g, 3, 3, item.ItemID.ToString("d8"), true);
                }
            }

            //绘制配方
            if (recipeInfoBmp != null)
            {
                g.DrawImage(recipeInfoBmp, recipeInfoOrigin.X, recipeInfoOrigin.Y,
                    new Rectangle(Point.Empty, recipeInfoBmp.Size), GraphicsUnit.Pixel);
            }

            //绘制产出道具
            if (recipeItemBmps.Count > 0)
            {
                int itemCnt = recipeItemBmps.Count;
                for (int i = 0; i < itemCnt; ++i)
                {
                    g.DrawImage(recipeItemBmps[i], recipeItemOrigins[i].X, recipeItemOrigins[i].Y,
                        new Rectangle(Point.Empty, recipeItemBmps[i].Size), GraphicsUnit.Pixel);
                }
            }

            //绘制套装
            if (setItemBmp != null)
            {
                g.DrawImage(setItemBmp, setItemOrigin.X, setItemOrigin.Y,
                    new Rectangle(Point.Empty, setItemBmp.Size), GraphicsUnit.Pixel);
            }

            if (levelBmp != null)
            {
                //绘制背景区域
                GearGraphics.DrawNewTooltipBack(g, levelOrigin.X, levelOrigin.Y, levelBmp.Width, levelHeight);
                //复制图像
                g.DrawImage(levelBmp, levelOrigin.X, levelOrigin.Y, new Rectangle(0, 0, levelBmp.Width, levelHeight), GraphicsUnit.Pixel);
            }

            if (itemBmp != null)
                itemBmp.Dispose();
            if (recipeInfoBmp != null)
                recipeInfoBmp.Dispose();
            if (recipeItemBmps.Count > 0)
                foreach (Bitmap recipeItemBmp in recipeItemBmps)
                    recipeItemBmp.Dispose();
            if (setItemBmp != null)
                setItemBmp.Dispose();
            if (levelBmp != null)
                levelBmp.Dispose();

            g.Dispose();
            return tooltip;
        }


        private Bitmap RenderItem(out int picH)
        {
            Bitmap tooltip = new Bitmap(290, DefaultPicHeight);
            Graphics g = Graphics.FromImage(tooltip);
            StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone();
            int value;

            picH = 10;
            //物品标题
            StringResult sr;
            if (StringLinker == null || !StringLinker.StringItem.TryGetValue(item.ItemID, out sr))
            {
                sr = new StringResult();
                sr.Name = "(null)";
            }
            string itemName = sr.Name.Replace(Environment.NewLine, "");
            string nameAdd = item.ItemID / 10000 == 313 || item.ItemID / 10000 == 501 ? "OFF" : null;
            if (!string.IsNullOrEmpty(nameAdd))
            {
                itemName += " (" + nameAdd + ")";
            }

            //SizeF titleSize = TextRenderer.MeasureText(g, sr.Name.Replace(Environment.NewLine, ""), GearGraphics.ItemNameFont2, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix);
            SizeF titleSize;
            if (IsKoreanStringPresent(itemName))
            {
                titleSize = TextRenderer.MeasureText(g, itemName, GearGraphics.KMSItemNameFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix);
            }
            else
            {
                titleSize = TextRenderer.MeasureText(g, itemName, GearGraphics.ItemNameFont2, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix);
            }
            titleSize.Width += 9 * 2;//9 was 12
            if (titleSize.Width > 290)
            {
                //重构大小
                g.Dispose();
                tooltip.Dispose();

                tooltip = new Bitmap((int)Math.Ceiling(titleSize.Width), DefaultPicHeight);
                g = Graphics.FromImage(tooltip);
                picH = 21;
            }
            if (sr["fixWidth"] != null)
            {
                //重构大小
                g.Dispose();
                tooltip.Dispose();

                tooltip = new Bitmap(Int32.Parse(sr["fixWidth"]), DefaultPicHeight);
                g = Graphics.FromImage(tooltip);
                picH = 10;
            }

            //绘制标题
            bool hasPart2 = false;
            format.Alignment = StringAlignment.Center;
            if (IsKoreanStringPresent(sr.Name))
            {
                g.DrawString(sr.Name, GearGraphics.KMSItemNameFont, Brushes.White, tooltip.Width / 2, picH, format);
            }
            else
            {
                g.DrawString(sr.Name, GearGraphics.ItemNameFont2, Brushes.White, tooltip.Width / 2, picH, format);
            }
            picH += 22;

            if (Item.Props.TryGetValue(ItemPropType.wonderGrade, out value) && value > 0)
            {
                switch (value)
                {
                    case 1:
                        TextRenderer.DrawText(g, "奇趣黑", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), ((SolidBrush)GearGraphics.OrangeBrush3).Color, TextFormatFlags.HorizontalCenter);
                        break;
                    case 4:
                        TextRenderer.DrawText(g, "月之甜蜜", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), GearGraphics.itemPinkColor, TextFormatFlags.HorizontalCenter);
                        break;
                    case 5:
                        TextRenderer.DrawText(g, "月之酣梦", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), ((SolidBrush)GearGraphics.BlueBrush).Color, TextFormatFlags.HorizontalCenter);
                        break;
                    case 6:
                        TextRenderer.DrawText(g, "月光宝宝", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), GearGraphics.itemPurpleColor, TextFormatFlags.HorizontalCenter);
                        break;
                    default:
                        picH -= 15;
                        break;
                }
                picH += 15;
            }
            else if (Item.Props.TryGetValue(ItemPropType.BTSLabel, out value) && value > 0)
            {
                TextRenderer.DrawText(g, "BTS标签", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), Color.FromArgb(187, 102, 238), TextFormatFlags.HorizontalCenter);
                picH += 15;
            }
            else if (Item.Props.TryGetValue(ItemPropType.BLACKPINKLabel, out value) && value > 0)
            {
                TextRenderer.DrawText(g, "BLACKPINK标签", GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), Color.FromArgb(255, 136, 170), TextFormatFlags.HorizontalCenter);
                picH += 15;
            }

            //额外特性
            var attrList = GetItemAttributeString();
            if (attrList.Count > 0)
            {
                var font = GearGraphics.ItemDetailFont;
                string attrStr = null;
                for (int i = 0; i < attrList.Count; i++)
                {
                    var newStr = (attrStr != null ? (attrStr + ", ") : null) + attrList[i];
                    if (TextRenderer.MeasureText(g, newStr, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width > tooltip.Width - 7 || (attrList[i].Contains('\n') && attrStr != null))
                    {
                        TextRenderer.DrawText(g, attrStr, GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), ((SolidBrush)GearGraphics.OrangeBrush4).Color, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
                        picH += 20;
                        attrStr = attrList[i];
                    }
                    else
                    {
                        attrStr = newStr;
                    }
                }
                if (!string.IsNullOrEmpty(attrStr))
                {
                    foreach (string attrLine in attrStr.Split('\n'))
                    {
                        TextRenderer.DrawText(g, attrLine, GearGraphics.ItemDetailFont, new Point(tooltip.Width, picH), ((SolidBrush)GearGraphics.OrangeBrush4).Color, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
                        picH += 20;
                    }
                    picH -= 3;
                }
                hasPart2 = true;
            }

            string expireTime = null;
            if (item.TimeLimited)
            {
                DateTime time = DateTime.Now.AddDays(7d);
                if (!item.Cash)
                {
                    expireTime = time.ToString("yyyy年 M月 d日 HH时 mm分前可使用");
                }
                else
                {
                    expireTime = time.ToString("yyyy年 M月 d日 HH时前可使用");
                }
            }
            else if (item.ConsumableFrom != null || item.EndUseDate != null)
            {
                expireTime = "";
                if (item.ConsumableFrom != null)
                {
                    expireTime += string.Format("\n{0}年 {1}月 {2}日 {3:D2}时 {4:D2}分起可使用", Convert.ToInt32(item.ConsumableFrom.Substring(0, 4)), Convert.ToInt32(item.ConsumableFrom.Substring(4, 2)), Convert.ToInt32(item.ConsumableFrom.Substring(6, 2)), Convert.ToInt32(item.ConsumableFrom.Substring(8, 2)), Convert.ToInt32(item.ConsumableFrom.Substring(10, 2)));
                }
                if (item.EndUseDate != null)
                {
                    expireTime += string.Format("\n{0}年 {1}月 {2}日 {3:D2}时 {4:D2}分前可使用", Convert.ToInt32(item.EndUseDate.Substring(0, 4)), Convert.ToInt32(item.EndUseDate.Substring(4, 2)), Convert.ToInt32(item.EndUseDate.Substring(6, 2)), Convert.ToInt32(item.EndUseDate.Substring(8, 2)), Convert.ToInt32(item.EndUseDate.Substring(10, 2)));
                }
            }
            else if ((item.Props.TryGetValue(ItemPropType.permanent, out value) && value != 0) || (item.ItemID / 10000 == 500 && item.Props.TryGetValue(ItemPropType.life, out value) && value == 0))
            {
                picH -= 3;
                if (value == 0)
                {
                    value = 1;
                }
                expireTime = ItemStringHelper.GetItemPropString(ItemPropType.permanent, value);
            }
            else if (item.ItemID / 10000 == 500 && item.Props.TryGetValue(ItemPropType.limitedLife, out value) && value > 0)
            {
                picH -= 3;
                expireTime = string.Format("魔法时间: {0}时 {1}分", value / 3600, (value % 3600) / 60);
            }
            else if (item.ItemID / 10000 == 500 && item.Props.TryGetValue(ItemPropType.life, out value) && value > 0)
            {
                picH -= 3;
                DateTime time = DateTime.Now.AddDays(value);
                expireTime = time.ToString("魔法时间: 到yyyy年 M月 d日 HH时");
            }
            if (!string.IsNullOrEmpty(expireTime))
            {
                if (attrList.Count > 0)
                {
                    picH += 3;
                }
                //picH += 3;
                //TextRenderer.DrawText(g, expireTime, GearGraphics.ItemDetailFont, new Point(tooltip.Width / 25, picH), Color.White, TextFormatFlags.Left);
                //picH += 16;
                //hasPart2 = true;
                foreach (string expireTimeLine in expireTime.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    g.DrawString(expireTime, GearGraphics.ItemDetailFont, Brushes.White, tooltip.Width / 2, picH, format);
                    picH += 16;
                }
                if (expireTime.Contains('\n'))
                {
                    picH += 4;
                }
                hasPart2 = true;
            }

            if (hasPart2)
            {
                picH += 4;
            }

            //绘制图标
            int iconY = picH;
            int iconX = 10;
            g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_base, iconX, picH);
            if (item.Icon.Bitmap != null)
            {
                g.DrawImage(GearGraphics.EnlargeBitmap(item.Icon.Bitmap),
                iconX + 6 + (1 - item.Icon.Origin.X) * 2,
                picH + 6 + (33 - item.Icon.Origin.Y) * 2);
                //picH + 8 + (33 - item.Icon.Bitmap.Height) * 2);
            }
            if (item.Cash)
            {
                Bitmap cashImg = null;
                Point cashOrigin = new Point(12, 12);

                if (item.Props.TryGetValue(ItemPropType.wonderGrade, out value) && value > 0)
                {
                    string resKey = $"CashShop_img_CashItem_label_{value + 3}";
                    cashImg = Resource.ResourceManager.GetObject(resKey) as Bitmap;
                }
                else if (Item.Props.TryGetValue(ItemPropType.BTSLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_10;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                else if (Item.Props.TryGetValue(ItemPropType.BLACKPINKLabel, out value) && value > 0)
                {
                    cashImg = Resource.CashShop_img_CashItem_label_11;
                    cashOrigin = new Point(cashImg.Width, cashImg.Height);
                }
                if (cashImg == null) //default cashImg
                {
                    cashImg = Resource.CashItem_0;
                }

                g.DrawImage(GearGraphics.EnlargeBitmap(cashImg),
                    iconX + 6 + 68 - cashOrigin.X * 2 - 2,
                    picH + 6 + 68 - cashOrigin.Y * 2 - 2);
            }
            g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_new, iconX + 7, picH + 7);
            g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_cover, iconX + 4, picH + 4); //绘制左上角cover

            value = 0;
            if (item.Props.TryGetValue(ItemPropType.reqLevel, out value) || item.ItemID / 10000 == 301 || item.ItemID / 1000 == 5204)
            {
                picH += 4;//default value is 4
                g.DrawImage(Resource.ToolTip_Equip_Can_reqLEV, 100, picH);
                GearGraphics.DrawGearDetailNumber(g, 150, picH, value.ToString(), true);
                picH += 15;
            }
            else
            {
                picH += 3;
            }

            int right = tooltip.Width - 18;

            string desc = null;
            if (item.Level > 0)
            {
                desc += $"[LV.{item.Level}] ";
            }
            desc += sr.Desc;
            if (item.ItemID / 10000 == 500)
            {
                if (item.Props.TryGetValue(ItemPropType.wonderGrade, out value) && value > 0)
                {
                    int setID;
                    if (item.Props.TryGetValue(ItemPropType.setItemID, out setID))
                    {
                        SetItem setItem;
                        if (CharaSimLoader.LoadedSetItems.TryGetValue(setID, out setItem))
                        {
                            string wonderGradeString = null;
                            string setItemName = setItem.SetItemName;
                            string setSkillName = "";
                            switch (value)
                            {
                                case 1:
                                    wonderGradeString = "奇趣黑";
                                    foreach (KeyValuePair<GearPropType, object> prop in setItem.Effects.Values.SelectMany(f => f.PropsV5))
                                    {
                                        if (prop.Key == GearPropType.activeSkill)
                                        {
                                            SetItemActiveSkill p = ((List<SetItemActiveSkill>)prop.Value)[0];
                                            StringResult sr2;
                                            if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(p.SkillID, out sr2))
                                            {
                                                sr2 = new StringResult();
                                                sr2.Name = p.SkillID.ToString();
                                            }
                                            setSkillName = Regex.Replace(sr2.Name, " Lv.\\d", "");
                                            break;
                                        }
                                    }
                                    break;
                                case 4:
                                    wonderGradeString = "月之甜蜜";
                                    setSkillName = "月之甜蜜";
                                    break;
                                case 5:
                                    wonderGradeString = "月之酣梦";
                                    setSkillName = "月之酣梦";
                                    break;
                            }
                            if (wonderGradeString != null)
                            {
                                desc += $"\n佩戴#c{wonderGradeString}#等级的#c{setItemName}#宠物时获得#c{setSkillName}#套装效果。(最高3阶段)\n套装效果根据佩戴的#c{setItemName}#宠物种类强化至3套。";
                            }
                        }
                    }
                }
                desc += "\n#c技能:金币拾取";
                if (item.Props.TryGetValue(ItemPropType.pickupItem, out value) && value > 0)
                {
                    desc += ", 道具拾取";
                }
                if (item.Props.TryGetValue(ItemPropType.longRange, out value) && value > 0)
                {
                    desc += ", 移动半径扩展";
                }
                if (item.Props.TryGetValue(ItemPropType.sweepForDrop, out value) && value > 0)
                {
                    desc += ", 自动拾取";
                }
                if (item.Props.TryGetValue(ItemPropType.pickupAll, out value) && value > 0)
                {
                    desc += ", 无所有权道具&金币拾取";
                }
                if (item.Props.TryGetValue(ItemPropType.consumeHP, out value) && value > 0)
                {
                    desc += ", 血量药水补充";
                }
                if (item.Props.TryGetValue(ItemPropType.consumeMP, out value) && value > 0)
                {
                    desc += ", 魔量药水补充";
                }
                if (item.Props.TryGetValue(ItemPropType.autoBuff, out value) && value > 0)
                {
                    desc += ", 自动使用增益技能";
                }
                if (item.Props.TryGetValue(ItemPropType.giantPet, out value) && value > 0)
                {
                    desc += ", 宠物巨大化技能";
                }
                if (item.Props.TryGetValue(ItemPropType.consumeCure, out value) && value > 0)
                {
                    desc += ", 自动使用万能治疗药";
                }
                desc += "#";
            }
            if (!string.IsNullOrEmpty(desc))
            {
                if (IsKoreanStringPresent(desc))
                {
                    GearGraphics.DrawString(g, desc, GearGraphics.KMSItemDetailFont, 100, right, ref picH, 16);
                }
                else
                {
                    GearGraphics.DrawString(g, desc, GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                }
            }
            if (!string.IsNullOrEmpty(sr.AutoDesc))
            {
                GearGraphics.DrawString(g, sr.AutoDesc, GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Props.TryGetValue(ItemPropType.tradeAvailable, out value) && value > 0)
            {
                string attr = ItemStringHelper.GetItemPropString(ItemPropType.tradeAvailable, value);
                if (!string.IsNullOrEmpty(attr))
                    GearGraphics.DrawString(g, "#c" + attr + "#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Props.TryGetValue(ItemPropType.pointCost, out value) && value > 0)
            {
                picH += 16;
                GearGraphics.DrawString(g, "- " + value + " Point(s)", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Specs.TryGetValue(ItemSpecType.recipeValidDay, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "( 可制作时间 : " + value + "日 )", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Specs.TryGetValue(ItemSpecType.recipeUseCount, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "( 可制作次数 : " + value + "次 )", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.ItemID / 1000 == 5533)
            {
                GearGraphics.DrawString(g, "\n#c双击时在预览中每3秒可按顺序确认箱子中的道具。\n\n在现金保管箱中可双击使用，箱子可交换。\n从箱子获得的奖品不可与他人交换。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Cash)
            {
                if (item.Props.TryGetValue(ItemPropType.noMoveToLocker, out value) && value > 0)
                {
                    GearGraphics.DrawString(g, "\n#c不可移动至现金保管箱的道具。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                }
                else if (item.Props.TryGetValue(ItemPropType.onlyCash, out value) && value > 0)
                {
                    GearGraphics.DrawString(g, "\n#c只能用冒险券购买。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                }
                else if ((!item.Props.TryGetValue(ItemPropType.tradeBlock, out value) || value == 0) && item.ItemID / 10000 != 501 && item.ItemID / 10000 != 502 && item.ItemID / 10000 != 516)
                {
                    /*GearGraphics.DrawString(g, "\n#cThis item cannot be traded once it has been used.#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);*/ //GMS - Enable when GMS uses this line.
                }
            }
            if (item.Props.TryGetValue(ItemPropType.flatRate, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "\n#c期限定额制道具。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Props.TryGetValue(ItemPropType.noScroll, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "#c不可使用宠物技能卷轴和取宠物名。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Props.TryGetValue(ItemPropType.noRevive, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "#c不可使用生命之水。#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }

            if (item.ItemID / 10000 == 500)
            {
                Wz_Node petDialog = PluginManager.FindWz("String\\PetDialog.img\\" + item.ItemID);
                Dictionary<string, int> commandLev = new Dictionary<string, int>();
                foreach (Wz_Node commandNode in PluginManager.FindWz("Item\\Pet\\" + item.ItemID + ".img\\interact").Nodes)
                {
                    foreach (string command in petDialog?.Nodes[commandNode.Nodes["command"].GetValue<string>()].GetValueEx<string>(null)?.Split('|') ?? Enumerable.Empty<string>())
                    {
                        int l0;
                        if (!commandLev.TryGetValue(command, out l0))
                        {
                            commandLev.Add(command, commandNode.Nodes["l0"].GetValue<int>());
                        }
                        else
                        {
                            commandLev[command] = Math.Min(l0, commandNode.Nodes["l0"].GetValue<int>());
                        }
                    }
                }

                GearGraphics.DrawString(g, "[可使用命令]", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                foreach (int l0 in commandLev.Values.OrderBy(i => i).Distinct())
                {
                    GearGraphics.DrawString(g, l0 + "级以上 : " + string.Join(", ", commandLev.Where(i => i.Value == l0).Select(i => i.Key).OrderBy(s => s)), GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                }
                GearGraphics.DrawString(g, "Tip. 宠物达到15级时可特定说话。", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                GearGraphics.DrawString(g, "#c例) /宠物 [说话]#", GearGraphics.ItemDetailFont, new Dictionary<string, Color>() { { "c", ((SolidBrush)GearGraphics.OrangeBrush4).Color } }, 100, right, ref picH, 16);
            }
            
            string incline = null;
            ItemPropType[] inclineTypes = new ItemPropType[]{
                    ItemPropType.charismaEXP,
                    ItemPropType.insightEXP,
                    ItemPropType.willEXP,
                    ItemPropType.craftEXP,
                    ItemPropType.senseEXP,
                    ItemPropType.charmEXP };

            string[] inclineString = new string[]{
                    "领导力","感性","洞察力","意志","手技","魅力"};

            for (int i = 0; i < inclineTypes.Length; i++)
            {
                if (item.Props.TryGetValue(inclineTypes[i], out value) && value > 0)
                {
                    incline += ", " + inclineString[i] + value;
                }
            }

            if (!string.IsNullOrEmpty(incline))
            {
                GearGraphics.DrawString(g, "#c装备时限1次获得" + incline.Substring(2) + "的经验值.(每天限制,超过最大值时除外)#", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }

            picH += 3;

            Wz_Node nickResNode = null;
            bool willDrawNickTag = this.ShowNickTag
                && this.Item.Props.TryGetValue(ItemPropType.nickTag, out value)
                && this.TryGetNickResource(value, out nickResNode);
            string descLeftAlign = sr["desc_leftalign"];
            int minLev = 0, maxLev = 0;
            bool willDrawExp = item.Props.TryGetValue(ItemPropType.exp_minLev, out minLev) && item.Props.TryGetValue(ItemPropType.exp_maxLev, out maxLev);

            if (!string.IsNullOrEmpty(descLeftAlign) || item.CoreSpecs.Count > 0 || item.Sample.Bitmap != null || item.SamplePath != null || willDrawNickTag || willDrawExp)
            {
                if (picH < iconY + 84)
                {
                    picH = iconY + 84;
                }
                if (!string.IsNullOrEmpty(descLeftAlign))
                {
                    picH += 12;
                    if (IsKoreanStringPresent(descLeftAlign))
                    {
                        GearGraphics.DrawString(g, descLeftAlign, GearGraphics.KMSItemDetailFont, 14, right, ref picH, 16);
                    }
                    else
                    {
                        GearGraphics.DrawString(g, descLeftAlign, GearGraphics.ItemDetailFont, 14, right, ref picH, 16);
                    }
                }
                if (item.CoreSpecs.Count > 0)
                {
                    g.DrawLine(Pens.White, 6, picH - 1, tooltip.Width - 7, picH - 1);
                    picH += 9;
                    foreach (KeyValuePair<ItemCoreSpecType, Wz_Node> p in item.CoreSpecs)
                    {
                        string coreSpec;
                        switch (p.Key)
                        {
                            case ItemCoreSpecType.Ctrl_addMob:
                                StringResult srMob;
                                if (StringLinker == null || !StringLinker.StringMob.TryGetValue(Convert.ToInt32(p.Value.Nodes["mobID"].Value), out srMob))
                                {
                                    srMob = new StringResult();
                                    srMob.Name = "(null)";
                                }
                                foreach (Wz_Node addMobNode in p.Value.Nodes)
                                {
                                    if (int.TryParse(addMobNode.Text, out value))
                                    {
                                        break;
                                    }
                                }
                                coreSpec = ItemStringHelper.GetItemCoreSpecString(ItemCoreSpecType.Ctrl_addMob, value, srMob.Name);
                                break;

                            default:
                                try
                                {
                                    coreSpec = ItemStringHelper.GetItemCoreSpecString(p.Key, Convert.ToInt32(p.Value.Value), Convert.ToString(p.Value.Nodes["desc"]?.Value));
                                }
                                finally
                                {
                                }
                                break;
                        }
                        GearGraphics.DrawString(g, "* " + coreSpec, GearGraphics.ItemDetailFont, 14, right, ref picH, 16);
                    }
                }
                if (item.Sample.Bitmap != null)
                {
                    g.DrawImage(item.Sample.Bitmap, (tooltip.Width - item.Sample.Bitmap.Width) / 2, picH);
                    picH += item.Sample.Bitmap.Height;
                    picH += 2;
                }
                if (item.SamplePath != null)
                {
                    Wz_Node sampleNode = PluginManager.FindWz(item.SamplePath);
                    int sampleW = 15;
                    for (int i = 1; ; i++)
                    {
                        Wz_Node effectNode = sampleNode.FindNodeByPath(string.Format("{0}{1:D4}\\effect\\0", sampleNode.Text, i));
                        if (effectNode == null)
                        {
                            break;
                        }

                        BitmapOrigin effect = BitmapOrigin.CreateFromNode(effectNode, PluginManager.FindWz);
                        if (sampleW + 87 >= tooltip.Width)
                        {
                            picH += 62;
                            sampleW = 15;
                        }
                        g.DrawImage(effect.Bitmap, sampleW + (85 - effect.Bitmap.Width - 1) / 2, picH + (62 - effect.Bitmap.Height - 1) / 2);
                        sampleW += 87;
                    }
                    picH += 62;
                }
                if (nickResNode != null)
                {
                    //获取称号名称
                    string nickName;
                    string nickWithQR = sr["nickWithQR"];
                    if (nickWithQR != null)
                    {
                        string qrDefault = sr["qrDefault"] ?? string.Empty;
                        nickName = Regex.Replace(nickWithQR, "#qr.*?#", qrDefault);
                    }
                    else
                    {
                        nickName = sr.Name;
                    }
                    GearGraphics.DrawNameTag(g, nickResNode, nickName, tooltip.Width, ref picH);
                    picH += 4; // value is either 4 or 14 (it was 14 in previous iteration)
                }
                if (minLev > 0 && maxLev > 0)
                {
                    long totalExp = 0;

                    for (int i = minLev; i < maxLev; i++)
                        totalExp += Character.ExpToNextLevel(i);

                    g.DrawLine(Pens.White, 6, picH, tooltip.Width - 7, picH);
                    picH += 8;

                    TextRenderer.DrawText(g, "总经验值量 :" + totalExp, GearGraphics.ItemDetailFont, new Point(10, picH), ((SolidBrush)GearGraphics.OrangeBrush4).Color, TextFormatFlags.NoPadding);
                    picH += 16;

                    TextRenderer.DrawText(g, "剩余经验值量:" + totalExp, GearGraphics.ItemDetailFont, new Point(10, picH), Color.Red, TextFormatFlags.NoPadding);
                    picH += 16;

                    string cantAccountSharable = null;
                    Wz_Node itemWz = PluginManager.FindWz(Wz_Type.Item);
                    if (itemWz != null)
                    {
                        string imgClass = (item.ItemID / 10000).ToString("d4") + ".img\\" + item.ItemID.ToString("d8");
                        foreach (Wz_Node node0 in itemWz.Nodes)
                        {
                            Wz_Node imgNode = node0.FindNodeByPath(imgClass, true);
                            if (imgNode != null)
                            {
                                cantAccountSharable = imgNode.FindNodeByPath("info\\cantAccountSharable\\tooltip").GetValueEx<string>(null);
                                break;
                            }
                        }
                    }

                    if (cantAccountSharable != null)
                    {
                        TextRenderer.DrawText(g, cantAccountSharable, GearGraphics.ItemDetailFont, new Point(10, picH), ((SolidBrush)GearGraphics.SetItemNameBrush).Color, TextFormatFlags.NoPadding);
                        picH += 16;
                        picH += 16;
                    }
                }
            }


            //绘制配方需求
            if (item.Specs.TryGetValue(ItemSpecType.recipe, out value))
            {
                int reqSkill, reqSkillLevel;
                if (!item.Specs.TryGetValue(ItemSpecType.reqSkill, out reqSkill))
                {
                    reqSkill = value / 10000 * 10000;
                }

                if (!item.Specs.TryGetValue(ItemSpecType.reqSkillLevel, out reqSkillLevel))
                {
                    reqSkillLevel = 1;
                }

                picH = Math.Max(picH, iconY + 107);
                g.DrawLine(Pens.White, 6, picH, 283, picH);//分割线
                picH += 10;
                TextRenderer.DrawText(g, "< 使用制限条件 >", GearGraphics.ItemDetailFont, new Point(8, picH), ((SolidBrush)GearGraphics.SetItemNameBrush).Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                picH += 17;

                //技能标题
                if (StringLinker == null || !StringLinker.StringSkill.TryGetValue(reqSkill, out sr))
                {
                    sr = new StringResult();
                    sr.Name = "- (null)";
                }
                switch (sr.Name)
                {
                    case "装备制作": sr.Name = "装备制作"; break;
                    case "饰品制作": sr.Name = "饰品制作"; break;
                }
                TextRenderer.DrawText(g, string.Format("- {0} Lv {1}", sr.Name, reqSkillLevel), GearGraphics.ItemDetailFont, new Point(13, picH), ((SolidBrush)GearGraphics.SetItemNameBrush).Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                picH += 16;
                picH += 6;
            }

            picH = Math.Max(iconY + 94, picH + 6);
            return tooltip;
        }

        private List<string> GetItemAttributeString()
        {
            int value, value2;
            List<string> tags = new List<string>();

            if (item.Props.TryGetValue(ItemPropType.quest, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.quest, value));
            }
            if (item.Props.TryGetValue(ItemPropType.pquest, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.pquest, value));
            }
            if (item.Props.TryGetValue(ItemPropType.only, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.only, value));
            }
            if (item.Props.TryGetValue(ItemPropType.tradeBlock, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.tradeBlock, value));
            }
            if (item.Props.TryGetValue(ItemPropType.useTradeBlock, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.useTradeBlock, value));
            }
            else if (item.ItemID / 10000 == 501 || item.ItemID / 10000 == 502 || item.ItemID / 10000 == 516)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.tradeBlock, 1));
            }
            if (item.Props.TryGetValue(ItemPropType.accountSharable, out value) && value != 0)
            {
                if (item.Props.TryGetValue(ItemPropType.exp_minLev, out value2) && value2 != 0)
                {
                    tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.useTradeBlock, 1));
                }
                if (item.Props.TryGetValue(ItemPropType.sharableOnce, out value2) && value2 != 0)
                {
                    tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.sharableOnce, value2));
                }
                else
                {
                    tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.accountSharable, value));
                }
            }
            if (item.Props.TryGetValue(ItemPropType.accountSharableAfterExchange, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.accountSharableAfterExchange, value));
            }
            if (item.Props.TryGetValue(ItemPropType.exchangeableOnce, out value) && value != 0)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.exchangeableOnce, value));
            }
            if (item.Props.TryGetValue(ItemPropType.multiPet, out value))
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.multiPet, value));
            }
            else if (item.ItemID / 10000 == 500)
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.multiPet, 0));
            }

            return tags;
        }

        private Bitmap RenderLinkRecipeInfo(Recipe recipe)
        {
            TooltipRender renderer = this.LinkRecipeInfoRender;
            if (renderer == null)
            {
                RecipeTooltipRender defaultRenderer = new RecipeTooltipRender();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = false;
                renderer = defaultRenderer;
            }

            renderer.TargetItem = recipe;
            return renderer.Render();
        }

        private Bitmap RenderLinkRecipeGear(Gear gear)
        {
            TooltipRender renderer = this.LinkRecipeGearRender;
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

        private Bitmap RenderLinkRecipeItem(Item item)
        {
            TooltipRender renderer = this.LinkRecipeItemRender;
            if (renderer == null)
            {
                ItemTooltipRender2 defaultRenderer = new ItemTooltipRender2();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = false;
                renderer = defaultRenderer;
            }

            renderer.TargetItem = item;
            return renderer.Render();
        }

        private Bitmap RenderSetItem(SetItem setItem)
        {
            TooltipRender renderer = this.SetItemRender;
            if (renderer == null)
            {
                var defaultRenderer = new SetItemTooltipRender();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = false;
                renderer = defaultRenderer;
            }

            renderer.TargetItem = setItem;
            return renderer.Render();
        }

        private Bitmap RenderCashPackage(CashPackage cashPackage)
        {
            TooltipRender renderer = this.CashPackageRender;
            if (renderer == null)
            {
                var defaultRenderer = new CashPackageTooltipRender();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = this.ShowObjectID;
                renderer = defaultRenderer;
            }

            renderer.TargetItem = cashPackage;
            return renderer.Render();
        }

        private Bitmap RenderLevel(out int picHeight)
        {
            Bitmap level = null;
            Graphics g = null;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            picHeight = 0;
            if (Item.Levels != null)
            {
                if (level == null)
                {
                    level = new Bitmap(261, DefaultPicHeight);
                    g = Graphics.FromImage(level);
                }
                picHeight += 13;
                TextRenderer.DrawText(g, "等级信息", GearGraphics.ItemDetailFont, new Point(261, picHeight), ((SolidBrush)GearGraphics.GreenBrush2).Color, TextFormatFlags.HorizontalCenter);
                picHeight += 15;

                for (int i = 0; i < Item.Levels.Count; i++)
                {
                    var info = Item.Levels[i];
                    TextRenderer.DrawText(g, "等级 " + info.Level + (i >= Item.Levels.Count - 1 ? " (MAX)" : null), GearGraphics.ItemDetailFont, new Point(10, picHeight), ((SolidBrush)GearGraphics.GreenBrush2).Color, TextFormatFlags.NoPadding);
                    picHeight += 15;
                    foreach (var kv in info.BonusProps)
                    {
                        GearLevelInfo.Range range = kv.Value;

                        string propString = ItemStringHelper.GetGearPropString(kv.Key, kv.Value.Min);
                        if (propString != null)
                        {
                            if (range.Max != range.Min)
                            {
                                propString += " ~ " + kv.Value.Max + (propString.EndsWith("%") ? "%" : null);
                            }
                            TextRenderer.DrawText(g, propString, GearGraphics.ItemDetailFont, new Point(10, picHeight), Color.White, TextFormatFlags.NoPadding);
                            picHeight += 15;
                        }
                    }
                    if (info.Skills.Count > 0)
                    {
                        string title = string.Format("有 {2:P2}({0}/{1}) 的几率获得技能 :", info.Prob, info.ProbTotal, info.Prob * 1.0 / info.ProbTotal);
                        TextRenderer.DrawText(g, title, GearGraphics.ItemDetailFont, new Point(10, picHeight), Color.White, TextFormatFlags.NoPadding);
                        picHeight += 15;
                        foreach (var kv in info.Skills)
                        {
                            StringResult sr = null;
                            if (this.StringLinker != null)
                            {
                                this.StringLinker.StringSkill.TryGetValue(kv.Key, out sr);
                            }
                            string text = string.Format(" +{2} {0}", sr == null ? null : sr.Name, kv.Key, kv.Value);
                            TextRenderer.DrawText(g, text, GearGraphics.ItemDetailFont, new Point(10, picHeight), ((SolidBrush)GearGraphics.OrangeBrush).Color, TextFormatFlags.NoPadding);
                            picHeight += 15;
                        }
                    }
                    if (info.EquipmentSkills.Count > 0)
                    {
                        string title;
                        if (info.Prob < info.ProbTotal)
                        {
                            title = string.Format("有 {2:P2}({0}/{1}) 的几率装备时获得技能 :", info.Prob, info.ProbTotal, info.Prob * 1.0 / info.ProbTotal);
                        }
                        else
                        {
                            title = "可使用技能 :";
                        }
                        TextRenderer.DrawText(g, title, GearGraphics.ItemDetailFont, new Point(10, picHeight), Color.White, TextFormatFlags.NoPadding);
                        picHeight += 15;
                        foreach (var kv in info.EquipmentSkills)
                        {
                            StringResult sr = null;
                            if (this.StringLinker != null)
                            {
                                this.StringLinker.StringSkill.TryGetValue(kv.Key, out sr);
                            }
                            string text = string.Format("{0}({1}) Lv.{2}", sr == null ? null : sr.Name, kv.Key, kv.Value);
                            TextRenderer.DrawText(g, text, GearGraphics.ItemDetailFont, new Point(10, picHeight), ((SolidBrush)GearGraphics.OrangeBrush).Color, TextFormatFlags.NoPadding);
                            picHeight += 15;
                        }
                    }
                    if (info.Exp > 0)
                    {
                        TextRenderer.DrawText(g, "单位经验值 : " + info.Exp + "%", GearGraphics.ItemDetailFont, new Point(10, picHeight), Color.White, TextFormatFlags.NoPadding);
                        picHeight += 15;
                    }

                    picHeight += 2;
                }
            }


            format.Dispose();
            if (g != null)
            {
                g.Dispose();
                picHeight += 13;
            }
            return level;
        }
        private bool IsKoreanStringPresent(string checkString)
        {
            return checkString.Any(c => (c >= '\uAC00' && c <= '\uD7A3'));
        }
        private bool TryGetNickResource(int nickTag, out Wz_Node resNode)
        {
            resNode = PluginBase.PluginManager.FindWz("UI/NameTag.img/nick/" + nickTag);
            return resNode != null;
        }
    }
}