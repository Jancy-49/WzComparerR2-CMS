using System;
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
using WzComparerR2.AvatarCommon;
using DevComponents.DotNetBar;
using Newtonsoft.Json.Linq;

namespace WzComparerR2.CharaSimControl
{
    public class ItemTooltipRender2 : TooltipRender
    {
        static ItemTooltipRender2()
        {
            res = new Dictionary<string, TextureBrush>();
            res["line"] = new TextureBrush(Resource.UIToolTipNew_img_Item_Common_frame_fixed_line, WrapMode.Clamp);
        }
        private static Dictionary<string, TextureBrush> res;

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
        public bool ShowLinkedTamingMob { get; set; }
        public bool CompareMode { get; set; } = false;
        public int CosmeticHairColor { get; set; }
        public int CosmeticFaceColor { get; set; }
        public bool ShowCashPurchasePrice { get; set; }
        public bool Enable22AniStyle { get; set; }
        public int LoadedCommoditiesSlot { get; set; } = 0;
        public bool ShowDamageSkin { get; set; }
        public bool ShowDamageSkinID { get; set; }
        public bool UseMiniSizeDamageSkin { get; set; }
        public bool AlwaysUseMseaFormatDamageSkin { get; set; }
        public bool AllowFamiliarOutOfBounds { get; set; }
        public bool UseCTFamiliarRender { get; set; }
        public bool ShowApplicablePetEquip { get; set; }
        public long DamageSkinNumber { get; set; }
        private bool WillDrawNickTag { get; set; }
        private Wz_Node NickResNode { get; set; }
        private Bitmap ItemSample { get; set; }

        public CashPackage CashPackage { get; set; }

        public TooltipRender LinkRecipeInfoRender { get; set; }
        public TooltipRender LinkRecipeGearRender { get; set; }
        public TooltipRender LinkRecipeItemRender { get; set; }
        public TooltipRender LinkDamageSkinRender { get; set; }
        public TooltipRender SetItemRender { get; set; }
        public TooltipRender CashPackageRender { get; set; }
        public TooltipRender FamiliarRender { get; set; }
        public TooltipRender MorphRender { get; set; }
        private AvatarCanvasManager avatar { get; set; }
        private bool isCurrencyConversionEnabled = (Translator.DefaultDesiredCurrency != "none");
        private string titleLanguage = "";

        private bool isMsnClient;
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
            List<Bitmap> recipeItemBmps = new();
            List<Bitmap> recipeInfoBmps = new();
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
                                    long tuc, tucCnt;
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
                                                    gear.Upgrade(infoNode, (int)tucCnt);

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (ShowLinkedTamingMob) recipeItemBmps.Add(RenderLinkRecipeGear(gear));
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
            if (this.item.Recipes.Count > 0)
            {
                foreach (int recipeID in this.item.Recipes)
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
                            recipeInfoBmps.Add(RenderLinkRecipeInfo(recipe));
                        }

                        if (this.LinkRecipeItem)
                        {
                            int itemID = recipe.MainTargetItemID;
                            AppendGearOrItem(itemID);
                        }
                    }
                }
            }

            long value;
            if (this.item.Props.TryGetValue(ItemPropType.dressUpgrade, out value))
            {
                long itemID = value;
                AppendGearOrItem((int)itemID);
            }
            if (this.item.Props.TryGetValue(ItemPropType.tamingMob, out value))
            {
                long itemID = value;
                AppendGearOrItem((int)itemID);
            }

            if (this.item.AddTooltips.Count > 0)
            {
                foreach (int itemID in item.AddTooltips)
                {
                    AppendGearOrItem(itemID);
                }
            }

            if (this.item.Props.TryGetValue(ItemPropType.setItemID, out long setID))
            {
                SetItem setItem;
                if (CompareMode)
                {
                    setItem = CharaSimLoader.LoadSetItem((int)setID, this.SourceWzFile);
                    if (setItem != null)
                        setItemBmp = RenderSetItem(setItem);
                }
                else if (CharaSimLoader.LoadedSetItems.TryGetValue((int)setID, out setItem))
                {
                    setItemBmp = RenderSetItem(setItem);
                }
            }

            if (this.item.DamageSkinID != null && ShowDamageSkin)
            {
                DamageSkin damageSkin = DamageSkin.CreateFromNode(PluginManager.FindWz($@"Etc\DamageSkin.img\{item.DamageSkinID}", this.SourceWzFile), PluginManager.FindWz);
                if (damageSkin != null)
                {
                    setItemBmp = RenderDamageSkin(damageSkin);
                }
            }

            if (this.item.FamiliarID != null)
            {
                Familiar familiar = Familiar.CreateFromNode(PluginManager.FindWz($@"Character\Familiar\{item.FamiliarID}.img", this.SourceWzFile), PluginManager.FindWz);
                if (familiar != null)
                {
                    return UseCTFamiliarRender ? RenderCTFamiliar(familiar) : RenderGJFamiliar(familiar);
                }
            }

            if (this.item.IsPet && ShowApplicablePetEquip)
            {
                List<int> petEquipList = GetApplicablePetEquip();
                foreach (int itemID in petEquipList)
                {
                    AppendGearOrItem(itemID);
                }
            }

            if (this.item.Specs.TryGetValue(ItemSpecType.morph, out long morphID) && morphID > 0)
            {
                Morph morph = Morph.CreateFromNode(PluginManager.FindWz($@"Morph\{morphID:D4}.img", this.SourceWzFile), PluginManager.FindWz, PluginManager.FindWz, this.SourceWzFile);
                if (morph != null)
                {
                    setItemBmp = RenderMorph(morph);
                    morph.Dispose();
                }
            }

            //计算布局
            Size totalSize = new Size(itemBmp.Width, picHeight);
            Point recipeInfoOrigin = Point.Empty;
            Point recipeItemOrigin = Point.Empty;
            Point setItemOrigin = Point.Empty;
            Point levelOrigin = Point.Empty;

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
            if (recipeItemBmps.Count > 0)
            {
                // layout:
                //   item        |  recipeItem
                //   recipeInfo  |
                recipeItemOrigin.X = totalSize.Width;
                totalSize.Width += recipeItemBmps.Max(bmp => bmp.Width);

                if (recipeInfoBmps.Count > 0)
                {
                    recipeInfoOrigin.X = itemBmp.Width - recipeInfoBmps.Max(bmp => bmp.Width);
                    recipeInfoOrigin.Y = picHeight;
                    totalSize.Height = Math.Max(totalSize.Height, Math.Max(picHeight + recipeInfoBmps.Sum(bmp => bmp.Height), recipeItemBmps.Sum(bmp => bmp.Height)));
                }
                else
                {
                    totalSize.Height = Math.Max(totalSize.Height, Math.Max(picHeight, recipeItemBmps.Sum(bmp => bmp.Height)));
                }
            }
            else if (recipeInfoBmps.Count > 0)
            {
                // layout:
                //   item  |  recipeInfo
                totalSize.Width += recipeInfoBmps.Max(bmp => bmp.Width);
                totalSize.Height = Math.Max(picHeight, recipeInfoBmps.Sum(bmp => bmp.Height));
                recipeInfoOrigin.X = itemBmp.Width;
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
                if (!Enable22AniStyle) g.DrawImage(Resource.UIToolTip_img_Item_Frame2_cover, 3, 3);

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

            //绘制套装
            if (setItemBmp != null)
            {
                g.DrawImage(setItemBmp, setItemOrigin.X, setItemOrigin.Y,
                    new Rectangle(Point.Empty, setItemBmp.Size), GraphicsUnit.Pixel);
            }

            //绘制产出道具
            if (recipeItemBmps.Count > 0)
            {
                for (int i = 0, y = recipeItemOrigin.Y; i < recipeItemBmps.Count; i++)
                {
                    g.DrawImage(recipeItemBmps[i], recipeItemOrigin.X, y,
                        new Rectangle(Point.Empty, recipeItemBmps[i].Size), GraphicsUnit.Pixel);
                    y += recipeItemBmps[i].Height;
                }
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
                recipeItemBmps.ForEach(bmp => bmp.Dispose());
            if (setItemBmp != null)
                setItemBmp.Dispose();
            if (levelBmp != null)
                levelBmp.Dispose();

            g.Dispose();
            return tooltip;
        }


        private Bitmap RenderItem(out int picH)
        {
            isMsnClient = StringLinker.StringEqp.TryGetValue(1006514, out _);
            bool isTranslateRequired = Translator.IsTranslateEnabled;
            Bitmap tooltip = new Bitmap(290, DefaultPicHeight);
            Graphics g = Graphics.FromImage(tooltip);
            StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone();
            long value;
            int intvalue;

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
            if (isCurrencyConversionEnabled)
            {
                if (Translator.DefaultDetectCurrency == "auto")
                {
                    titleLanguage = Translator.GetLanguage(itemName);
                }
                else
                {
                    titleLanguage = Translator.ConvertCurrencyToLang(Translator.DefaultDetectCurrency);
                }
            }
            if (isTranslateRequired)
            {
                string translatedItemName = Translator.MergeString(itemName, Translator.TranslateString(itemName, true), 0, false, true);
                if (translatedItemName == itemName)
                {
                    // isTranslateRequired = false;
                }
                else
                {
                    itemName = translatedItemName;
                }
            }
            //SizeF titleSize = TextRenderer.MeasureText(g, sr.Name.Replace(Environment.NewLine, ""), GearGraphics.ItemNameFont2, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix);
            SizeF titleSize;
            if (Translator.IsKoreanStringPresent(itemName))
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
            if (Translator.IsKoreanStringPresent(sr.Name))
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
            if (this.Enable22AniStyle)
            {
                g.DrawImage(Resource.UIToolTipNew_img_Item_Common_ItemIcon_base, iconX, picH);
            }
            else
            {
                g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_base, iconX, picH);
            }
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
            if (!this.Enable22AniStyle)
            {
                g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_new, iconX + 7, picH + 7);
                g.DrawImage(Resource.UIToolTip_img_Item_ItemIcon_cover, iconX + 4, picH + 4); //绘制左上角cover
            }

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
                    if (item.Props.TryGetValue(ItemPropType.setItemID, out long setID))
                    {
                        SetItem setItem;
                        if ((!CompareMode && CharaSimLoader.LoadedSetItems.TryGetValue((int)setID, out setItem)) || (CompareMode && (setItem = CharaSimLoader.LoadSetItem((int)setID, this.SourceWzFile)) != null))
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
                desc += isMsnClient ? "\n#c技能：NESO拾取" : "\n#c技能:金币拾取";
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
                if (Translator.IsKoreanStringPresent(desc))
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
                if (Translator.IsKoreanStringPresent(sr.AutoDesc))
                {
                    GearGraphics.DrawString(g, sr.AutoDesc, GearGraphics.KMSItemDetailFont, 100, right, ref picH, 16);
                }
                else
                {
                    GearGraphics.DrawString(g, sr.AutoDesc, GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                }
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
                GearGraphics.DrawString(g, "(可制作时间 : " + value + "天)", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
            }
            if (item.Specs.TryGetValue(ItemSpecType.recipeUseCount, out value) && value > 0)
            {
                GearGraphics.DrawString(g, "(可制作次数 : " + value + "次)", GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
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
                    string text = l0 + "级以上 : " + string.Join(", ", commandLev.Where(i => i.Value == l0).Select(i => i.Key).OrderBy(s => s));
                    if (Translator.IsKoreanStringPresent(text))
                    {
                        GearGraphics.DrawString(g, text, GearGraphics.KMSItemDetailFont, 100, right, ref picH, 16);
                    }
                    else
                    {
                        GearGraphics.DrawString(g, text, GearGraphics.ItemDetailFont, 100, right, ref picH, 16);
                    }
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
            long minLev = 0, maxLev = 0;
            bool willDrawExp = item.Props.TryGetValue(ItemPropType.exp_minLev, out minLev) && item.Props.TryGetValue(ItemPropType.exp_maxLev, out maxLev);

            if (!string.IsNullOrEmpty(descLeftAlign) || item.CoreSpecs.Count > 0 || item.Sample.Bitmap != null || item.SamplePath != null || item.ShowCosmetic || willDrawNickTag || willDrawExp)
            {
                if (picH < iconY + 84)
                {
                    picH = iconY + 84;
                }
                if (!string.IsNullOrEmpty(descLeftAlign))
                {
                    picH += 12;
                    if (Translator.IsKoreanStringPresent(descLeftAlign))
                    {
                        GearGraphics.DrawString(g, ReplaceDescTags(descLeftAlign), GearGraphics.KMSItemDetailFont, 14, right, ref picH, 16);
                    }
                    else
                    {
                        GearGraphics.DrawString(g, ReplaceDescTags(descLeftAlign), GearGraphics.ItemDetailFont, 14, right, ref picH, 16);
                    }
                }
                if (item.CoreSpecs.Count > 0)
                {
                    g.DrawLine(Pens.White, 6, picH - 1, tooltip.Width - 7, picH - 1);
                    picH += 9;
                    foreach (KeyValuePair<ItemCoreSpecType, Wz_Node> p in item.CoreSpecs)
                    {
                        string coreSpec;
                        intvalue = 0;
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
                                    if (int.TryParse(addMobNode.Text, out intvalue))
                                    {
                                        break;
                                    }
                                }
                                coreSpec = ItemStringHelper.GetItemCoreSpecString(ItemCoreSpecType.Ctrl_addMob, intvalue, srMob.Name);
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
                if (this.item.Specs.TryGetValue(ItemSpecType.cosmetic, out value) && value > 0)
                {
                    if (this.avatar == null)
                    {
                        this.avatar = new AvatarCanvasManager();
                    }

                    this.avatar.SetCosmeticColor(this.CosmeticHairColor, this.CosmeticFaceColor);

                    if (value < 1000)
                    {
                        this.avatar.AddBodyFromSkin3((int)value);
                    }
                    else
                    {
                        this.avatar.AddBodyFromSkin4(2015);
                        this.avatar.AddHairOrFace((int)value);
                    }

                    this.avatar.AddGears([1042194, 1062153]);

                    var frame = this.avatar.GetBitmapOrigin();
                    if (frame.Bitmap != null)
                    {
                        g.DrawImage(frame.Bitmap, (tooltip.Width - frame.Bitmap.Width) / 2, picH);
                        picH += frame.Bitmap.Height;
                        picH += 2;
                    }

                    this.avatar.ClearCanvas();
                }
                if (item.SamplePath != null)
                {
                    Wz_Node sampleNode = PluginManager.FindWz(item.SamplePath);
                    int sampleW = 15;
                    if (sampleNode == null && item.SamplePath.Contains("ChatEmoticon.img"))
                    {
                        sampleNode = PluginManager.FindWz(item.SamplePath.Replace("ChatEmoticon.img/", "ChatEmoticon.img/Emoticon/"));
                    }
                    if (sampleNode != null)
                    {
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
                }
                if (nickResNode != null)
                {
                    //获取称号名称
                    string nickName = GearGraphics.GetNameTagString(sr);
                    GearGraphics.DrawNameTag(g, nickResNode, nickName, tooltip.Width, ref picH);
                    picH += 4; // value is either 4 or 14 (it was 14 in previous iteration)
                }
                if (minLev > 0 && maxLev > 0)
                {
                    long totalExp = 0;

                    for (int i = (int)minLev; i < maxLev; i++)
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
                long reqSkill, reqSkillLevel;
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
                if (StringLinker == null || !StringLinker.StringSkill.TryGetValue((int)reqSkill, out sr))
                {
                    sr = new StringResult();
                    sr.Name = "- (null)";
                }
                switch (sr.Name)
                {
                    case "장비제작": sr.Name = "装备制作"; break;
                    case "장신구제작": sr.Name = "饰品制作"; break;
                }
                TextRenderer.DrawText(g, string.Format("- {0} Lv {1}", sr.Name, reqSkillLevel), GearGraphics.ItemDetailFont, new Point(13, picH), ((SolidBrush)GearGraphics.SetItemNameBrush).Color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                picH += 16;
                picH += 6;
            }

            if (ShowCashPurchasePrice && !isMsnClient && item.Cash)
            {
                HashSet<string> priceList = new HashSet<string>();
                if (CharaSimLoader.LoadedCommodityPricesByItemId[LoadedCommoditiesSlot].ContainsKey(item.ItemID))
                {
                    bool containRebootOnlyPrice = false;
                    foreach (var i in CharaSimLoader.LoadedCommodityPricesByItemId[LoadedCommoditiesSlot][item.ItemID])
                    {
                        containRebootOnlyPrice = containRebootOnlyPrice || i.Meso;
                    }
                    foreach (var i in CharaSimLoader.LoadedCommodityPricesByItemId[LoadedCommoditiesSlot][item.ItemID])
                    {
                        if (i.Price == 0) continue;
                        string currency = i.Meso ? "金币" : "冒险券";
                        string rebootWorld = i.Reboot ? " (重启世界)" : (containRebootOnlyPrice ? " (普通世界)" : "");
                        if (containRebootOnlyPrice && !i.Meso && i.Reboot) continue;
                        string approxPrice = "";
                        if (Translator.DefaultDesiredCurrency != "none" && !i.Meso)
                        {
                            approxPrice = $" ({Translator.GetConvertedCurrency(i.Price, titleLanguage)})";
                            if (i.Reboot) rebootWorld += approxPrice;
                        }
                        priceList.Add(string.Format("#$S - {0}个 {1} {2}{3}#", i.Count, ItemStringHelper.ToCJKNumberExpr(i.Price), currency, rebootWorld));
                    }
                }
                if (priceList.Count > 0)
                {
                    var itemPriceColorTable = new Dictionary<string, Color>()
                    {
                        { "$S", ((SolidBrush)GearGraphics.ItemPriceBrush).Color },
                    };
                    picH += 29;
                    switch (priceList.Count)
                    {
                        /*
                        case 1:
                            GearGraphics.DrawString(g, " · 购买价格： " + priceList[0].Replace(" · 1个 ", "").Replace(" · ", ""), GearGraphics.EquipDetailFont, 100, right, ref picH, 16);
                            break;
                        */
                        default:
                            GearGraphics.DrawString(g, "#$S购买价格：#", GearGraphics.ItemDetailFont, itemPriceColorTable, 100, right, ref picH, 16);
                            foreach (var i in priceList)
                                GearGraphics.DrawString(g, i, GearGraphics.ItemDetailFont, itemPriceColorTable, 100, right, ref picH, 16);
                            break;
                    }
                }
            }

            picH = Math.Max(iconY + 94, picH + 6);
            return tooltip;
        }

        private List<string> GetItemAttributeString()
        {
            long value, value2;
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
            if (item.Props.TryGetValue(ItemPropType.mintable, out value))
            {
                tags.Add(ItemStringHelper.GetItemPropString(ItemPropType.mintable, value));
            }

            return tags;
        }
        private Bitmap RenderDamageSkin(DamageSkin damageSkin)
        {
            TooltipRender renderer = this.LinkDamageSkinRender;
            if (renderer == null)
            {
                DamageSkinTooltipRenderer defaultRenderer = new DamageSkinTooltipRenderer();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = this.ShowDamageSkinID;
                defaultRenderer.UseMiniSize = this.UseMiniSizeDamageSkin;
                defaultRenderer.AlwaysUseMseaFormat = this.AlwaysUseMseaFormatDamageSkin;
                defaultRenderer.DamageSkinNumber = this.DamageSkinNumber;
                renderer = defaultRenderer;
                defaultRenderer.DamageSkin = damageSkin;
                item.DamageSkinSampleNonCriticalBitmap = defaultRenderer.GetCustomSample(this.DamageSkinNumber, this.UseMiniSizeDamageSkin, false);
                item.DamageSkinSampleCriticalBitmap = defaultRenderer.GetCustomSample(this.DamageSkinNumber, this.UseMiniSizeDamageSkin, true);
                item.DamageSkinExtraBitmap = defaultRenderer.GetExtraEffect();
            }
            renderer.TargetItem = damageSkin;
            return renderer.Render();
        }

        private Bitmap RenderCTFamiliar(Familiar familiar)
        {
            TooltipRender renderer = this.FamiliarRender;
            if (renderer == null)
            {
                FamiliarTooltipRender defaultRenderer = new FamiliarTooltipRender();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = this.ShowObjectID;
                defaultRenderer.AllowOutOfBounds = this.AllowFamiliarOutOfBounds;
                defaultRenderer.ItemID = this.item.ItemID;
                renderer = defaultRenderer;
            }
            renderer.TargetItem = familiar;
            return renderer.Render();
        }

        private Bitmap RenderGJFamiliar(Familiar familiar)
        {
            TooltipRender renderer = this.FamiliarRender;
            if (renderer == null)
            {
                FamiliarTooltipRender2 defaultRenderer = new FamiliarTooltipRender2();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = this.ShowObjectID;
                defaultRenderer.AllowOutOfBounds = this.AllowFamiliarOutOfBounds;
                defaultRenderer.ItemID = this.item.ItemID;
                defaultRenderer.UseAssembleUI = false;
                renderer = defaultRenderer;
            }
            renderer.TargetItem = familiar;
            return renderer.Render();
        }

        private Bitmap RenderMorph(Morph morph)
        {
            TooltipRender renderer = this.MorphRender;
            if (renderer == null)
            {
                MorphTooltipRenderer defaultRenderer = new MorphTooltipRenderer();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = this.ShowObjectID;
                renderer = defaultRenderer;
            }
            renderer.TargetItem = morph;
            return renderer.Render();
        }

        private Bitmap RenderLinkRecipeInfo(Recipe recipe)
        {
            TooltipRender renderer = this.LinkRecipeInfoRender;
            if (renderer == null)
            {
                RecipeTooltipRender defaultRenderer = new RecipeTooltipRender();
                defaultRenderer.StringLinker = this.StringLinker;
                defaultRenderer.ShowObjectID = true;
                defaultRenderer.Enable22AniStyle = this.Enable22AniStyle;
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
                if (this.Enable22AniStyle)
                {
                    GearTooltipRender22 defaultRenderer = new GearTooltipRender22();
                    defaultRenderer.StringLinker = this.StringLinker;
                    defaultRenderer.ShowObjectID = false;
                    renderer = defaultRenderer;
                }
                else
                {
                    GearTooltipRender2 defaultRenderer = new GearTooltipRender2();
                    defaultRenderer.StringLinker = this.StringLinker;
                    defaultRenderer.ShowObjectID = false;
                    renderer = defaultRenderer;
                }
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
                defaultRenderer.Enable22AniStyle = this.Enable22AniStyle;
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
                if (this.Enable22AniStyle)
                {
                    var defaultRenderer = new SetItemTooltipRender22();
                    defaultRenderer.StringLinker = this.StringLinker;
                    defaultRenderer.ShowObjectID = false;
                    renderer = defaultRenderer;
                }
                else
                {
                    var defaultRenderer = new SetItemTooltipRender();
                    defaultRenderer.StringLinker = this.StringLinker;
                    defaultRenderer.ShowObjectID = false;
                    renderer = defaultRenderer;
                }
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

        private List<int> GetApplicablePetEquip()
        {
            List<int> petEquipList = new List<int>();
            foreach (var i in CharaSimLoader.LoadedPetEquipInfo)
            {
                if (i.Value.Contains(item.ItemID)) petEquipList.Add(i.Key);
            }
            return petEquipList;
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

        private bool TryGetNickResource(long nickTag, out Wz_Node resNode)
        {
            resNode = PluginBase.PluginManager.FindWz("UI/NameTag.img/nick/" + nickTag);
            return resNode != null;
        }

        private string ReplaceDescTags(string text)
        {
            if (text.Contains("#cosmetic_EULO#"))
            {
                this.Item.Specs.TryGetValue(ItemSpecType.cosmetic, out long cosmeticID);
                var gender = Gear.GetGender((int)cosmeticID);
                var name = "";
                if (StringLinker == null || !StringLinker.StringEqp.TryGetValue((int)cosmeticID, out var sr))
                {
                    sr = new StringResult();
                    sr.Name = "(null)";
                }
                name = $"#c{sr.Name}{(gender == 0 ? "(男)" : (gender == 1 ? "(女)" : ""))}#";

                int last = (name.LastOrDefault(c => c >= '가' && c <= '힣') - '가') % 28;
                name += ((last == 0 || last == 8 ? "" : "으") + "로");

                foreach (var color in AvatarCanvas.HairColor)
                {
                    name = name.Replace($"{color} ", "");
                }

                text = text.Replace("#cosmetic_EULO#", name);
            }

            text = Regex.Replace(text, @$"#(t)\s*(\d{{1,9}}).*?#", match => // id should be less than 1,000,000,000
            {
                string tag = match.Groups[1].Value;
                if (!int.TryParse(match.Groups[2].Value, out int id)) id = -1;
                StringResult sr;
                var name = "";
                switch (tag)
                {
                    case "t":
                        StringLinker.StringItem.TryGetValue(id, out sr);
                        if (sr == null)
                        {
                            StringLinker.StringEqp.TryGetValue(id, out sr);
                        }
                        name = sr?.Name ?? id.ToString();
                        return $"{name}";

                    default:
                        return id.ToString();
                }
            });

            return text;
        }

        public bool HasSamples()
        {
            return this.Item.Sample.Bitmap != null || (this.NickResNode != null && this.ShowNickTag) || this.ItemSample != null;
        }

        public Bitmap GetSampleBitmap()
        {
            if (this.WillDrawNickTag && this.ShowNickTag)
            {
                Rectangle rect = new Rectangle();
                int block = 300;
                using Bitmap tempBitmap = new Bitmap(block, block);
                using Graphics tempG = Graphics.FromImage(tempBitmap);
                tempG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                int h = block / 2;

                StringResult sr;
                if (StringLinker == null || !StringLinker.StringItem.TryGetValue(item.ItemID, out sr))
                {
                    sr = new StringResult();
                    sr.Name = "(null)";
                }

                string nickName = GearGraphics.GetNameTagString(sr);
                GearGraphics.DrawNameTag(tempG, this.NickResNode, nickName, tempBitmap.Width, out rect, ref h);

                Bitmap resBitmap = new Bitmap(rect.Width, rect.Height);
                using Graphics g = Graphics.FromImage(resBitmap);
                g.DrawImage(tempBitmap, 0, 0, rect, GraphicsUnit.Pixel);

                return resBitmap;
            }
            else if (this.Item.Sample.Bitmap != null)
            {
                return new Bitmap(this.Item.Sample.Bitmap);
            }
            else if (this.ItemSample != null)
            {
                return new Bitmap(this.ItemSample);
            }

            return null;
        }
    }
}