using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WzComparerR2.CharaSim
{
    public static class ItemStringHelper
    {
        /// <summary>
        /// 获取怪物category属性对应的类型说明。
        /// </summary>
        /// <param Name="category">怪物的category属性的值。</param>
        /// <returns></returns>
        public static string GetMobCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "无形态";
                case 1: return "动物型";
                case 2: return "植物型";
                case 3: return "鱼类型";
                case 4: return "爬虫类型";
                case 5: return "精灵型";
                case 6: return "恶魔型";
                case 7: return "不死型";
                case 8: return "无机物型";
                default: return null;
            }
        }

        public static string GetGearPropString(GearPropType propType, long value)
        {
            return GetGearPropString(propType, value, 0);
        }

        /// <summary>
        /// 获取GearPropType所对应的文字说明。
        /// </summary>
        /// <param Name="propType">表示装备属性枚举GearPropType。</param>
        /// <param Name="Value">表示propType属性所对应的值。</param>
        /// <returns></returns>
        public static string GetGearPropString(GearPropType propType, long value, int signFlag)
        {

            string sign;
            switch (signFlag)
            {
                default:
                case 0: //默认处理符号
                    sign = value > 0 ? "+" : null;
                    break;

                case 1: //固定加号
                    sign = "+";
                    break;

                case 2: //无特别符号
                    sign = "";
                    break;
            }
            switch (propType)
            {
                case GearPropType.incSTR: return "力量 : " + sign + value;
                case GearPropType.incSTRr: return "力量 : " + sign + value + "%";
                case GearPropType.incDEX: return "敏捷 : " + sign + value;
                case GearPropType.incDEXr: return "敏捷 : " + sign + value + "%";
                case GearPropType.incINT: return "智力 : " + sign + value;
                case GearPropType.incINTr: return "智力 : " + sign + value + "%";
                case GearPropType.incLUK: return "运气 : " + sign + value;
                case GearPropType.incLUKr: return "运气 : " + sign + value + "%";
                case GearPropType.incAllStat: return "全属性 : " + sign + value;
                case GearPropType.statR: return "全属性 : " + sign + value + "%";
                case GearPropType.incMHP: return "最大血量 : " + sign + value;
                case GearPropType.incMHPr: return "最大血量 : " + sign + value + "%";
                case GearPropType.incMMP: return "最大魔量 : " + sign + value;
                case GearPropType.incMMPr: return "最大魔量 : " + sign + value + "%";
                case GearPropType.incMDF: return "MaxDF : " + sign + value;
                case GearPropType.incPAD: return "攻击力 : " + sign + value;
                case GearPropType.incPADr: return "攻击力 : " + sign + value + "%";
                case GearPropType.incMAD: return "魔法攻击力 : " + sign + value;
                case GearPropType.incMADr: return "魔法攻击力 : " + sign + value + "%";
                case GearPropType.incPDD: return "防御力 : " + sign + value;
                case GearPropType.incPDDr: return "防御力 : " + sign + value + "%";
                //case GearPropType.incMDD: return "MAGIC DEF. : " + sign + value;
                //case GearPropType.incMDDr: return "MAGIC DEF. : " + sign + value + "%";
                //case GearPropType.incACC: return "ACCURACY : " + sign + value;
                //case GearPropType.incACCr: return "ACCURACY : " + sign + value + "%";
                //case GearPropType.incEVA: return "AVOIDABILITY : " + sign + value;
                //case GearPropType.incEVAr: return "AVOIDABILITY : " + sign + value + "%";
                case GearPropType.incSpeed: return "移动速度 : " + sign + value;
                case GearPropType.incJump: return "跳跃力 : " + sign + value;
                case GearPropType.incCraft: return "手技 : " + sign + value;
                case GearPropType.damR:
                case GearPropType.incDAMr: return "伤害 : " + sign + value + "%";
                case GearPropType.incCr: return "爆击率 : " + sign + value + "%";
                case GearPropType.incCDr: return "暴击伤害 : " + sign + value + "%";
                case GearPropType.knockback: return "直接攻击时" + value + "的比率发生后退现象。";
                case GearPropType.incPVPDamage: return "大乱斗时追加攻击力 " + sign + value;
                case GearPropType.incPQEXPr: return "组队任务经验值 " + value + "% 증가";
                case GearPropType.incEXPr: return "经验值增加" + value + "%";
                case GearPropType.incBDR:
                case GearPropType.bdR: return "首领怪攻击力 +" + value + "%";
                case GearPropType.incIMDR:
                case GearPropType.imdR: return "无视防御力 : +" + value + "%";
                case GearPropType.limitBreak: return "伤害上限突破至" + ToChineseNumberExpr(value) + "。";
                case GearPropType.reduceReq: return "减少佩戴等级 : - " + value;
                case GearPropType.nbdR: return "普通怪物伤害 : +" + value + "%";

                case GearPropType.only: return value == 0 ? null : "固有道具";
                case GearPropType.tradeBlock: return value == 0 ? null : "不可交换";
                case GearPropType.equipTradeBlock: return value == 0 ? null : "佩戴后不可交换";
                case GearPropType.accountSharable: return value == 0 ? null : "只有世界内我的角色间可移动";
                case GearPropType.sharableOnce: return value == 0 ? null : "世界内我的角色间可移动1次\n(移动后不可交换)";
                case GearPropType.onlyEquip: return value == 0 ? null : "固有装备物品";
                case GearPropType.notExtend: return value == 0 ? null : "有效时间不可延长";
                case GearPropType.accountSharableAfterExchange: return value == 0 ? null : "可交易1次\n(交易后只能在世界内我的角色之间移动)";
                case GearPropType.mintable: return value == 0 ? null : "可铸造";
                case GearPropType.tradeAvailable:
                    switch (value)
                    {
                        case 1: return " #c使用宿命剪刀，可以使物品交易1次。#";
                        case 2: return " #c使用白金宿命剪刀，可以使物品交易1次。#";
                        default: return null;
                    }
                case GearPropType.accountShareTag:
                    switch (value)
                    {
                        case 1: return " #c使用物品共享牌，可以在同一账号内的角色间移动1次。#";
                        default: return null;
                    }
                case GearPropType.noPotential: return value == 0 ? null : "不可设置潜能";
                case GearPropType.fixedPotential: return value == 0 ? null : "不可重置潜能";
                case GearPropType.superiorEqp: return value == 0 ? null : "道具强化成功时，可以获得更高的效果。";
                case GearPropType.nActivatedSocket: return value == 0 ? null : "#c可以镶嵌星岩#";
                case GearPropType.jokerToSetItem: return value == 0 ? null : " #c当前装备3个以上的所有套装道具中包含的幸运物品！#";
                //case GearPropType.plusToSetItem: return value == 0 ? null : "#c装備すると、アイテムセットは2つ装備したものとしてカウントされます。#";
                case GearPropType.abilityTimeLimited: return value == 0 ? null : "限期能力值";
                case GearPropType.blockGoldHammer: return value == 0 ? null : "无法使用黄金锤";
                case GearPropType.colorvar: return value == 0 ? null : "#c该装备可通过染色颜料来变更颜色.#";
                case GearPropType.cantRepair: return value == 0 ? null : "无法修复";
                case GearPropType.noLookChange: return value == 0 ? null : "不可使用勋章神秘铁砧";

                case GearPropType.incAllStat_incMHP25: return "全属性: " + sign + value + ", 最大血量 : " + sign + (value * 25);
                case GearPropType.incAllStat_incMHP50_incMMP50: return "全属性: " + sign + value + ", 最大血量/最大魔量 : " + sign + (value * 50);
                case GearPropType.incMHP_incMMP: return "最大血量/最大魔量 : " + sign + value;
                case GearPropType.incMHPr_incMMPr: return "最大血量/最大魔量 : " + sign + value + "%";
                case GearPropType.incPAD_incMAD:
                case GearPropType.incAD: return "攻击力/魔法攻击力 : " + sign + value;
                case GearPropType.incPDD_incMDD: return "防御力 : " + sign + value;
                //case GearPropType.incACC_incEVA: return "ACC/AVO :" + sign + value;

                case GearPropType.incARC: return "神秘之力 : " + sign + value;
                case GearPropType.incAUT: return "原初之力 : " + sign + value;

                case GearPropType.Etuc: return "可进行卓越强化。(最多 : " + value + "次)";
                case GearPropType.CuttableCount: return "可使用剪刀次数 : " + value + "次";
                default: return null;
            }
        }


        public static string GetGearPropDiffString(GearPropType propType, int value, int standardValue)
        {
            var propStr = GetGearPropString(propType, value);
            if (value > standardValue)
            {
                string subfix = null;
                string openAPISubfix = "";
                switch (propType)
                {
                    case GearPropType.incSTR:
                    case GearPropType.incDEX:
                    case GearPropType.incINT:
                    case GearPropType.incLUK:
                    case GearPropType.incMHP:
                    case GearPropType.incMMP:
                    case GearPropType.incMDF:
                    case GearPropType.incARC:
                    case GearPropType.incAUT:
                    case GearPropType.incPAD:
                    case GearPropType.incMAD:
                    case GearPropType.incPDD:
                    case GearPropType.incMDD:
                    case GearPropType.incSpeed:
                    case GearPropType.incJump:
                        subfix = $"({standardValue} #$e+{value - standardValue}#)"; break;
                    case GearPropType.bdR:
                    case GearPropType.incBDR:
                    case GearPropType.imdR:
                    case GearPropType.incIMDR:
                    case GearPropType.damR:
                    case GearPropType.incDAMr:
                    case GearPropType.statR:
                        subfix = $"({standardValue}% #$y+{value - standardValue}%#)"; break;

                    case GearPropType.addSTR:
                    case GearPropType.addDEX:
                    case GearPropType.addINT:
                    case GearPropType.addLUK:
                    case GearPropType.addMHP:
                    case GearPropType.addMMP:
                    case GearPropType.addPAD:
                    case GearPropType.addMAD:
                    case GearPropType.addDEF:
                    case GearPropType.addSpeed:
                    case GearPropType.addJump:
                    case GearPropType.addLvlDec:
                        openAPISubfix += $"#$g+{value - standardValue}#"; break;


                    case GearPropType.addBDR:
                    case GearPropType.addDamR:
                    case GearPropType.addAllStatR:
                        openAPISubfix += $"#$g+{value - standardValue}%#"; break;

                    case GearPropType.scrollSTR:
                    case GearPropType.scrollDEX:
                    case GearPropType.scrollINT:
                    case GearPropType.scrollLUK:
                    case GearPropType.scrollMHP:
                    case GearPropType.scrollMMP:
                    case GearPropType.scrollPAD:
                    case GearPropType.scrollMAD:
                    case GearPropType.scrollDEF:
                    case GearPropType.scrollSpeed:
                    case GearPropType.scrollJump:
                        openAPISubfix += $" #$e+{value - standardValue}#"; break;

                    case GearPropType.starSTR:
                    case GearPropType.starDEX:
                    case GearPropType.starINT:
                    case GearPropType.starLUK:
                    case GearPropType.starMHP:
                    case GearPropType.starMMP:
                    case GearPropType.starPAD:
                    case GearPropType.starMAD:
                    case GearPropType.starDEF:
                    case GearPropType.starSpeed:
                    case GearPropType.starJump:
                        openAPISubfix += $" #c+{value - standardValue}#"; break;

                }
                if (openAPISubfix.Length > 0 )
                {
                    openAPISubfix = $"({standardValue}" + openAPISubfix + ")";
                }
                propStr = "#$y" + propStr + "# " + subfix + openAPISubfix;
            }
            return propStr;
        }

        /// <summary>
        /// 获取gearGrade所对应的字符串。
        /// </summary>
        /// <param Name="rank">表示装备的潜能等级GearGrade。</param>
        /// <returns></returns>
        public static string GetGearGradeString(GearGrade rank)
        {
            switch (rank)
            {
                //case GearGrade.C: return "C级(一般物品)";
                case GearGrade.B: return "(B级道具)";
                case GearGrade.A: return "(A级道具)";
                case GearGrade.S: return "(S级道具)";
                case GearGrade.SS: return "(SS级道具)";
                case GearGrade.Special: return "(特殊道具)";
                default: return null;
            }
        }

        /// <summary>
        /// 获取gearType所对应的字符串。
        /// </summary>
        /// <param Name="Type">表示装备类型GearType。</param>
        /// <returns></returns>
        public static string GetGearTypeString(GearType type)
        {
            switch (type)
            {
                //case GearType.body: return "Avatar (Body)";
                case GearType.head: return "皮肤";
                case GearType.face:
                case GearType.face2: return "脸型";
                case GearType.hair:
                case GearType.hair2:
                case GearType.hair3: return "发型";
                case GearType.faceAccessory: return "脸饰";
                case GearType.eyeAccessory: return "眼饰";
                case GearType.earrings: return "耳环";
                case GearType.pendant: return "吊坠";
                case GearType.belt: return "腰带";
                case GearType.medal: return "勋章";
                case GearType.shoulderPad: return "肩饰";
                case GearType.cap: return "帽子";
                case GearType.cape: return "披风";
                case GearType.coat: return "上衣";
                case GearType.dragonMask: return "龙神帽子";
                case GearType.dragonPendant: return "龙神吊坠";
                case GearType.dragonWings: return "龙神翅膀";
                case GearType.dragonTail: return "龙神尾巴";
                case GearType.glove: return "手套";
                case GearType.longcoat: return "套服";
                case GearType.machineEngine: return "机甲引擎";
                case GearType.machineArms: return "机甲机械臂";
                case GearType.machineLegs: return "机甲机械腿";
                case GearType.machineBody: return "机甲机身材质";
                case GearType.machineTransistors: return "机甲晶体管";
                case GearType.pants: return "裙/裤";
                case GearType.ring: return "戒指";
                case GearType.shield: return "盾牌";
                case GearType.shoes: return "鞋子";
                case GearType.shiningRod: return "双头杖";
                case GearType.soulShooter: return "灵魂手铳";
                case GearType.ohSword: return "单手剑";
                case GearType.ohAxe: return "单手斧";
                case GearType.ohBlunt: return "单手钝器";
                case GearType.dagger: return "短刀";
                case GearType.katara: return "刀";
                case GearType.magicArrow: return "魔法箭矢";
                case GearType.card: return "卡牌";
                case GearType.box: return "宝盒";
                case GearType.orb: return "宝珠";
                case GearType.novaMarrow: return "龙之精髓";
                case GearType.soulBangle: return "灵魂手镯";
                case GearType.mailin: return "麦林";
                case GearType.cane: return "手杖";
                case GearType.wand: return "短杖";
                case GearType.staff: return "长杖";
                case GearType.thSword: return "双手剑";
                case GearType.thAxe: return "双手斧";
                case GearType.thBlunt: return "双手钝器";
                case GearType.spear: return "枪";
                case GearType.polearm: return "矛";
                case GearType.bow: return "弓";
                case GearType.crossbow: return "弩";
                case GearType.throwingGlove: return "拳套";
                case GearType.knuckle: return "指节";
                case GearType.gun: return "短枪";
                case GearType.android: return "智能机器人";
                case GearType.machineHeart: return "机械心脏";
                case GearType.pickaxe: return "采矿工具";
                case GearType.shovel: return "采药工具";
                case GearType.pocket: return "口袋道具";
                case GearType.dualBow: return "双刀";
                case GearType.handCannon: return "手炮";
                case GearType.badge: return "徽章";
                case GearType.emblem: return "纹章";
                case GearType.soulShield: return "灵魂盾";
                case GearType.demonShield: return "精气盾";
                case GearType.totem: return "图腾";
                case GearType.petEquip: return "宠物装备";
                case GearType.taming:
                case GearType.taming2:
                case GearType.taming3: return "骑宠";
                case GearType.tamingChair: return "鞍部";
                case GearType.saddle: return "鞍子";
                case GearType.katana: return "武士刀";
                case GearType.fan: return "折扇";
                case GearType.swordZB: return "大剑";
                case GearType.swordZL: return "太刀";
                case GearType.weapon: return "武器";
                case GearType.subWeapon: return "辅助武器";
                case GearType.heroMedal: return "勋章";
                case GearType.rosario: return "念珠";
                case GearType.chain: return "铁链";
                case GearType.book1:
                case GearType.book2:
                case GearType.book3: return "魔道书";
                case GearType.bowMasterFeather: return "箭羽";
                case GearType.crossBowThimble: return "扳指";
                case GearType.shadowerSheath: return "短剑剑鞘";
                case GearType.nightLordPoutch: return "护身符";
                case GearType.viperWristband: return "手腕护带";
                case GearType.captainSight: return "瞄准器";
                case GearType.cannonGunPowder:
                case GearType.cannonGunPowder2: return "火药桶";
                case GearType.aranPendulum: return "砝码";
                case GearType.evanPaper: return "文件";
                case GearType.battlemageBall: return "魔法球";
                case GearType.wildHunterArrowHead: return "箭轴";
                case GearType.cygnusGem: return "宝石";
                case GearType.controller: return "控制器";
                case GearType.foxPearl: return "狐狸珠";
                case GearType.chess: return "棋子";
                case GearType.powerSource: return "能源";

                case GearType.energySword: return "能量剑";
                case GearType.desperado: return "亡命剑";
                case GearType.magicStick: return "记忆长杖";
                case GearType.leaf: return "飞越";
                case GearType.boxingClaw: return "拳爪";
                case GearType.kodachi2: return "小太刀";
                case GearType.espLimiter: return "ESP限制器";

                case GearType.GauntletBuster: return "机甲手枪";
                case GearType.ExplosivePill: return "装弹";

                case GearType.chain2: return "锁链";
                case GearType.magicGauntlet: return "魔力手套";
                case GearType.transmitter: return "武器传送装置";
                case GearType.magicWing: return "魔法之翼";
                case GearType.pathOfAbyss: return "深渊精气珠";

                case GearType.relic: return "遗物";
                case GearType.ancientBow: return "远古弓";

                case GearType.handFan: return "扇子";
                case GearType.fanTassel: return "扇坠";

                case GearType.tuner: return "调谐器";
                case GearType.bracelet: return "手链";

                case GearType.boxingCannon: return "拳封";
                case GearType.boxingSky: return "拳天";
                case GearType.jewel: return "宝玉";

                case GearType.breathShooter: return "龙息臂箭";
                case GearType.weaponBelt: return "武器腰带";

                case GearType.ornament: return "饰品";

                case GearType.chakram: return "环刃";
                case GearType.hexSeeker: return "索魂器";
                default: return null;
            }
        }

        /// <summary>
        /// 获取武器攻击速度所对应的字符串。
        /// </summary>
        /// <param Name="attackSpeed">表示武器的攻击速度，通常为2~9的数字。</param>
        /// <returns></returns>
        public static string GetAttackSpeedString(int attackSpeed)
        {
            switch (attackSpeed)
            {
                case 2:
                case 3: return "极快";
                case 4:
                case 5: return "快";
                case 6: return "普通";
                case 7:
                case 8: return "缓慢";
                case 9: return "较慢";
                default:
                    return attackSpeed.ToString();
            }
        }

        /// <summary>
        /// 获取套装装备类型的字符串。
        /// </summary>
        /// <param Name="Type">表示套装装备类型的GearType。</param>
        /// <returns></returns>
        public static string GetSetItemGearTypeString(GearType type)
        {
            return GetGearTypeString(type);
        }

        /// <summary>
        /// 获取装备额外职业要求说明的字符串。
        /// </summary>
        /// <param Name="Type">表示装备类型的GearType。</param>
        /// <returns></returns>
        public static string GetExtraJobReqString(GearType type)
        {
            switch (type)
            {
                //0xxx
                case GearType.heroMedal: return "英雄职业群可佩戴";
                case GearType.rosario: return "圣骑士职业群可佩戴";
                case GearType.chain: return "黑骑士职业群可佩戴";
                case GearType.book1: return "火/毒系列魔法师可佩戴";
                case GearType.book2: return "冰/雷系列魔法师可佩戴";
                case GearType.book3: return "主教系列魔法师可佩戴";
                case GearType.bowMasterFeather: return "神射手职业群可佩戴";
                case GearType.crossBowThimble: return "箭神职业群可佩戴";
                case GearType.shadowerSheath: return "侠盗职业群可佩戴";
                case GearType.nightLordPoutch: return "隐士职业群可佩戴";
                case GearType.katara: return "暗影双刀职业群可佩戴";
                case GearType.viperWristband: return "冲锋队长职业群可佩戴";
                case GearType.captainSight: return "船长职业群可佩戴";
                case GearType.cannonGunPowder:
                case GearType.cannonGunPowder2: return "火炮手职业群可佩戴";
                case GearType.box:
                case GearType.boxingClaw: return "杰特可佩戴";
                case GearType.relic: return "古迹猎人职业群可佩戴";


                //1xxx
                case GearType.cygnusGem: return "希纳斯骑士团可佩戴";

                //2xxx
                case GearType.aranPendulum: return GetExtraJobReqString(21);
                case GearType.dragonMask:
                case GearType.dragonPendant:
                case GearType.dragonWings:
                case GearType.dragonTail:
                case GearType.evanPaper: return GetExtraJobReqString(22);
                case GearType.magicArrow: return GetExtraJobReqString(23);
                case GearType.card: return GetExtraJobReqString(24);
                case GearType.foxPearl: return GetExtraJobReqString(25);
                case GearType.orb:
                case GearType.shiningRod: return GetExtraJobReqString(27);

                //3xxx
                case GearType.demonShield: return GetExtraJobReqString(31);
                case GearType.desperado: return "恶魔复仇者可佩戴";
                case GearType.battlemageBall: return "唤灵斗师可佩戴";
                case GearType.wildHunterArrowHead: return "豹弩游侠可佩戴";
                case GearType.machineEngine:
                case GearType.machineArms:
                case GearType.machineLegs:
                case GearType.machineBody:
                case GearType.machineTransistors:
                case GearType.mailin: return "机械师可佩戴";
                case GearType.controller:
                case GearType.powerSource:
                case GearType.energySword: return GetExtraJobReqString(36);
                case GearType.GauntletBuster:
                case GearType.ExplosivePill: return GetExtraJobReqString(37);

                //4xxx
                case GearType.katana:
                case GearType.kodachi:
                case GearType.kodachi2: return GetExtraJobReqString(41);
                case GearType.fan: return "阴阳师可佩戴";

                //5xxx
                case GearType.soulShield: return "米哈尔可佩戴";

                //6xxx
                case GearType.novaMarrow: return GetExtraJobReqString(61);
                case GearType.weaponBelt:
                case GearType.breathShooter: return GetExtraJobReqString(63);
                case GearType.chain2:
                case GearType.transmitter: return GetExtraJobReqString(64);
                case GearType.soulBangle:
                case GearType.soulShooter: return GetExtraJobReqString(65);

                //10xxx
                case GearType.swordZB:
                case GearType.swordZL: return GetExtraJobReqString(101);

                case GearType.magicStick: return GetExtraJobReqString(112);
                case GearType.leaf:
                case GearType.leaf2:
                case GearType.memorialStaff: return GetExtraJobReqString(172);

                case GearType.espLimiter:
                case GearType.chess: return GetExtraJobReqString(142);

                case GearType.magicGauntlet:
                case GearType.magicWing: return GetExtraJobReqString(152);

                case GearType.pathOfAbyss: return GetExtraJobReqString(155);
                case GearType.handFan:
                case GearType.fanTassel: return GetExtraJobReqString(164);

                case GearType.tuner:
                case GearType.bracelet: return GetExtraJobReqString(151);

                case GearType.boxingCannon:
                case GearType.boxingSky: return GetExtraJobReqString(175);

                case GearType.ornament: return GetExtraJobReqString(162);
                default: return null;
            }
        }

        /// <summary>
        /// 获取装备额外职业要求说明的字符串。
        /// </summary>
        /// <param Name="specJob">表示装备属性的reqSpecJob的值。</param>
        /// <returns></returns>
        public static string GetExtraJobReqString(int specJob)
        {
            switch (specJob)
            {
                case 2: return "冰雷魔导师、火毒魔导师、主教、龙神、\r\n炎术士、唤灵斗师、森林小主可佩戴";
                case 21: return "战神可佩戴";
                case 22: return "龙神可佩戴";
                case 23: return "双弩精灵可佩戴";
                case 24: return "幻影可佩戴";
                case 25: return "隐月可佩戴";
                case 27: return "夜光法师可佩戴";
                case 31: return "恶魔职业群可佩戴";
                case 36: return "尖兵可佩戴";
                case 37: return "爆破手可佩戴";
                case 41: return "剑豪可佩戴";
                case 42: return "阴阳师可佩戴";
                case 51: return "米哈尔可佩戴";
                case 61: return "狂龙战士可佩戴";
                case 63: return "炼狱黑客可佩戴";
                case 64: return "魔链影士可佩戴";
                case 65: return "爆莉萌天使可佩戴";
                case 99: return "小白";
                case 101: return "神之子可佩戴";
                case 112: return "林之灵可佩戴";
                case 142: return "超能力者可佩戴";
                case 151: return "御剑骑士可佩戴";
                case 152: return "圣晶使徒可佩戴";
                case 154: return "飞刃沙士可佩戴";
                case 155: return "影魂异人可佩戴";
                case 162: return "元素师可佩戴";
                case 164: return "虎影可佩戴";
                case 172: return "森林小主可佩戴";
                case 175: return "墨玄可佩戴";

                default: return null;
            }
        }

        public static string GetReqSpecJobMultipleString(int specJob)
        {
            switch (specJob)
            {
                case 1: return "英雄、圣骑士、";
                case 2: return "冰雷魔导师、火毒魔导师、主教、\r\n";
                case 4: return "侠盗、";
                case 11: return "魂骑士、";
                case 12: return "炎术士、";
                case 22: return "龙神、";
                case 32: return "唤灵斗师、";
                case 172: return "森林小主、";

                default: return null;
            }
        }

        public static string GetItemPropString(ItemPropType propType, long value)
        {
            switch (propType)
            {
                case ItemPropType.tradeBlock:
                    return GetGearPropString(GearPropType.tradeBlock, value);
                case ItemPropType.useTradeBlock:
                    return value == 0 ? null : "使用后不可交换";
                case ItemPropType.tradeAvailable:
                    return GetGearPropString(GearPropType.tradeAvailable, value);
                case ItemPropType.only:
                    return GetGearPropString(GearPropType.only, value);
                case ItemPropType.accountSharable:
                    return GetGearPropString(GearPropType.accountSharable, value);
                case ItemPropType.sharableOnce:
                    return GetGearPropString(GearPropType.sharableOnce, value);
                case ItemPropType.accountSharableAfterExchange:
                    return GetGearPropString(GearPropType.accountSharableAfterExchange, value);
                case ItemPropType.exchangeableOnce:
                    return value == 0 ? null : "可交换1次 (使用或交易后不可交换)";
                case ItemPropType.quest:
                    return value == 0 ? null : "任务道具";
                case ItemPropType.pquest:
                    return value == 0 ? null : "组队任务道具";
                case ItemPropType.multiPet:
                    return value == 0 ? "普通宠物 (不可与其它普通宠物重复使用)" : "多重宠物 (最多可与其它3个宠物重复使用)";
                case ItemPropType.permanent:
                    return value == 0 ? null : "可以一直使用魔法的神奇宠物。";
                case ItemPropType.mintable:
                    return GetGearPropString(GearPropType.mintable, value);
                default:
                    return null;
            }
        }

        public static string GetItemCoreSpecString(ItemCoreSpecType coreSpecType, int value, string desc)
        {
            bool hasCoda = false;
            if (desc?.Length > 0)
            {
                char lastCharacter = desc.Last();
                hasCoda = lastCharacter >= '가' && lastCharacter <= '힣' && (lastCharacter - '가') % 28 != 0;
            }
            switch (coreSpecType)
            {
                case ItemCoreSpecType.Ctrl_mobLv:
                    return value == 0 ? null : "Monster Level " + "+" + value;
                case ItemCoreSpecType.Ctrl_mobHPRate:
                    return value == 0 ? null : "Monster HP " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_mobRate:
                    return value == 0 ? null : "Monster Population " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_mobRateSpecial:
                    return value == 0 ? null : "Monster Population " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_change_Mob:
                    return desc == null ? null : "Change monster skins for " + desc;
                case ItemCoreSpecType.Ctrl_change_BGM:
                    return desc == null ? null : "Change music for " + desc;
                case ItemCoreSpecType.Ctrl_change_BackGrnd:
                    return desc == null ? null : "Change background image for " + desc;
                case ItemCoreSpecType.Ctrl_partyExp:
                    return value == 0 ? null : "Party EXP " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_partyExpSpecial:
                    return value == 0 ? null : "Party EXP " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_addMob:
                    return value == 0 || desc == null ? null : desc + ", Link " + value + " added to area";
                case ItemCoreSpecType.Ctrl_dropRate:
                    return value == 0 ? null : "Drop Rate " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_dropRateSpecial:
                    return value == 0 ? null : "Drop Rate " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_dropRate_Herb:
                    return value == 0 ? null : "Herb Drop Rate " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_dropRate_Mineral:
                    return value == 0 ? null : "Mineral Drop Rate " + "+" + value + "%";
                case ItemCoreSpecType.Ctrl_dropRareEquip:
                    return value == 0 ? null : "Rare Equipment Drop";
                case ItemCoreSpecType.Ctrl_reward:
                case ItemCoreSpecType.Ctrl_addMission:
                    return desc;
                default:
                    return null;
            }
        }

        public static string GetSkillReqAmount(int skillID, int reqAmount)
        {
            switch (skillID / 10000)
            {
                case 11200: return "[需要巨熊技能点: " + reqAmount + "]";
                case 11210: return "[需要雪豹技能点: " + reqAmount + "]";
                case 11211: return "[需要猛禽技能点: " + reqAmount + "]";
                case 11212: return "[需要猫咪技能点: " + reqAmount + "]";
                default: return "[需要？？技能点: " + reqAmount + "]";
            }
        }

        public static string GetJobName(int jobCode)
        {
            switch (jobCode)
            {
                case 0: return "新手";
                case 100: return "战士";
                case 110: return "斗士";
                case 111: return "勇士";
                case 112: return "英雄";
                case 114: return "英雄(6转)";
                case 120: return "准骑士";
                case 121: return "骑士";
                case 122: return "圣骑士";
                case 124: return "圣骑士(6转)";
                case 130: return "枪战士";
                case 131: return "龙骑士";
                case 132: return "黑骑士";
                case 134: return "黑骑士(6转)";
                case 200: return "魔法师";
                case 210: return "法师(火毒)";
                case 211: return "巫师(火毒)";
                case 212: return "魔导师（火毒）";
                case 214: return "魔导师（火毒）(6转)";
                case 220: return "法师(冰雷)";
                case 221: return "巫师(冰雷)";
                case 222: return "魔导师（冰雷）";
                case 224: return "魔导师（冰雷）(6转)";
                case 230: return "牧师";
                case 231: return "祭司";
                case 232: return "主教";
                case 234: return "主教(6转)";
                case 300: return "弓箭手";
                case 301: return "弓箭手";
                case 310: return "猎手";
                case 311: return "游侠";
                case 312: return "神射手";
                case 314: return "神射手(6转)";
                case 320: return "弩弓手";
                case 321: return "游侠";
                case 322: return "箭神";
                case 324: return "箭神(6转)";
                case 330: return "古迹猎人";
                case 331: return "古迹猎人";
                case 332: return "古迹猎人";
                case 334: return "古迹猎人(6转)";
                case 400: return "飞侠";
                case 410: return "刺客";
                case 411: return "无影人";
                case 412: return "隐士";
                case 414: return "隐士(6转)";
                case 420: return "飞侠";
                case 421: return "独行客";
                case 422: return "侠盗";
                case 424: return "侠盗(6转)";
                case 430: return "见习刀客";
                case 431: return "双刀客";
                case 432: return "双刀侠";
                case 433: return "血刀";
                case 434: return "暗影双刀";
                case 436: return "暗影双刀(6转)";
                case 500: return "海盗";
                case 501: return "海盗";
                case 510: return "拳手";
                case 511: return "斗士";
                case 512: return "冲锋队长";
                case 514: return "冲锋队长(6转)";
                case 520: return "火枪手";
                case 521: return "大副";
                case 522: return "船长";
                case 524: return "船长(6转)";
                case 530: return "火炮手";
                case 531: return "毁灭炮手";
                case 532: return "神炮王";
                case 534: return "神炮王(6转)";

                case 800: return "管理员";
                case 900: return "管理员";


                case 1000: return "初心者";
                case 1100: return "魂骑士(1转)";
                case 1110: return "魂骑士(2转)";
                case 1111: return "魂骑士(3转)";
                case 1112: return "魂骑士(4转)";
                case 1114: return "魂骑士(6转)";
                case 1200: return "炎术士(1转)";
                case 1210: return "炎术士(2转)";
                case 1211: return "炎术士(3转)";
                case 1212: return "炎术士(4转)";
                case 1214: return "炎术士(6转)";
                case 1300: return "风灵使者(1转)";
                case 1310: return "风灵使者(2转)";
                case 1311: return "风灵使者(3转)";
                case 1312: return "风灵使者(4转)";
                case 1314: return "风灵使者(6转)";
                case 1400: return "夜行者(1转)";
                case 1410: return "夜行者(2转)";
                case 1411: return "夜行者(3转)";
                case 1412: return "夜行者(4转)";
                case 1414: return "夜行者(6转)";
                case 1500: return "奇袭者(1转)";
                case 1510: return "奇袭者(2转)";
                case 1511: return "奇袭者(3转)";
                case 1512: return "奇袭者(4转)";
                case 1514: return "奇袭者(6转)";

                case 2000: return "战童";
                case 2001: return "小不点";
                case 2002: return "双弩精灵";
                case 2003: return "幻影";
                case 2004: return "夜光法师";
                case 2005: return "隐月";
                case 2100: return "战神(1转)";
                case 2110: return "战神(2转)";
                case 2111: return "战神(3转)";
                case 2112: return "战神(4转)";
                case 2114: return "战神(6转)";
                case 2200:
                case 2210: return "龙神(1转)";
                case 2211:
                case 2212:
                case 2213: return "龙神(2转)";
                case 2214:
                case 2215:
                case 2216: return "龙神(3转)";
                case 2217:
                case 2218: return "龙神(4转)";
                case 2220: return "龙神(6转)";
                case 2300: return "双弩精灵(1转)";
                case 2310: return "双弩精灵(2转)";
                case 2311: return "双弩精灵(3转)";
                case 2312: return "双弩精灵(4转)";
                case 2314: return "双弩精灵(6转)";
                case 2400: return "幻影(1转)";
                case 2410: return "幻影(2转)";
                case 2411: return "幻影(3转)";
                case 2412: return "幻影(4转)";
                case 2414: return "幻影(6转)";
                case 2500: return "隐月(1转)";
                case 2510: return "隐月(2转)";
                case 2511: return "隐月(3转)";
                case 2512: return "隐月(4转)";
                case 2514: return "隐月(6转)";
                case 2700: return "夜光法师(1转)";
                case 2710: return "夜光法师(2转)";
                case 2711: return "夜光法师(3转)";
                case 2712: return "夜光法师(4转)";
                case 2714: return "夜光法师(6转)";


                case 3000: return "市民";
                case 3001: return "恶魔";
                case 3100: return "恶魔猎手(1转)";
                case 3110: return "恶魔猎手(2转)";
                case 3111: return "恶魔猎手(3转)";
                case 3112: return "恶魔猎手(4转)";
                case 3114: return "恶魔猎手(6转)";
                case 3101: return "恶魔复仇者(1转)";
                case 3120: return "恶魔复仇者(2转)";
                case 3121: return "恶魔复仇者(3转)";
                case 3122: return "恶魔复仇者(4转)";
                case 3124: return "恶魔复仇者(6转)";
                case 3200: return "唤灵斗师(1转)";
                case 3210: return "唤灵斗师(2转)";
                case 3211: return "唤灵斗师(3转)";
                case 3212: return "唤灵斗师(4转)";
                case 3214: return "唤灵斗师(6转)";
                case 3300: return "豹弩游侠(1转)";
                case 3310: return "豹弩游侠(2转)";
                case 3311: return "豹弩游侠(3转)";
                case 3312: return "豹弩游侠(4转)";
                case 3314: return "豹弩游侠(6转)";
                case 3500: return "机械师(1转)";
                case 3510: return "机械师(2转)";
                case 3511: return "机械师(3转)";
                case 3512: return "机械师(4转)";
                case 3514: return "机械师(6转)";
                case 3002: return "尖兵";
                case 3600: return "尖兵(1转)";
                case 3610: return "尖兵(2转)";
                case 3611: return "尖兵(3转)";
                case 3612: return "尖兵(4转)";
                case 3614: return "尖兵(6转)";
                case 3700: return "爆破手";
                case 3710: return "爆破手(2转)";
                case 3711: return "爆破手(3转)";
                case 3712: return "爆破手(4转)";
                case 3714: return "爆破手(6转)";

                case 4001: return "阴阳师";
                case 4002: return "阴阳师";
                case 4100: return "剑豪(1转)";
                case 4110: return "剑豪(2转)";
                case 4111: return "剑豪(3转)";
                case 4112: return "剑豪(4转)";
                case 4114: return "剑豪(6转)";
                case 4200: return "阴阳师(1转)";
                case 4210: return "阴阳师(2转)";
                case 4211: return "阴阳师(3转)";
                case 4212: return "阴阳师(4转)";
                case 4214: return "阴阳师(6转)";


                case 5000: return "米哈尔";
                case 5100: return "米哈尔(1转)";
                case 5110: return "米哈尔(2转)";
                case 5111: return "米哈尔(3转)";
                case 5112: return "米哈尔(4转)";
                case 5114: return "米哈尔(6转)";


                case 6000: return "狂龙战士";
                case 6001: return "爆莉萌天使";
                case 6002: return "魔链影士";
                case 6003: return "炼狱黑客";
                case 6100: return "狂龙战士(1转)";
                case 6110: return "狂龙战士(2转)";
                case 6111: return "狂龙战士(3转)";
                case 6112: return "狂龙战士(4转)";
                case 6114: return "狂龙战士(6转)";
                case 6300: return "炼狱黑客(1转)";
                case 6310: return "炼狱黑客(2转)";
                case 6311: return "炼狱黑客(3转)";
                case 6312: return "炼狱黑客(4转)";
                case 6314: return "炼狱黑客(6转)";
                case 6400: return "魔链影士(1转)";
                case 6410: return "魔链影士(2转)";
                case 6411: return "魔链影士(3转)";
                case 6412: return "魔链影士(4转)";
                case 6414: return "魔链影士(6转)";
                case 6500: return "爆莉萌天使(1转)";
                case 6510: return "爆莉萌天使(2转)";
                case 6511: return "爆莉萌天使(3转)";
                case 6512: return "爆莉萌天使(4转)";
                case 6514: return "爆莉萌天使(6转)";


                case 7000: return "内在能力";
                case 7100: return "联盟";
                case 7200: return "怪物农庄";


                case 9100: return "公会";
                case 9200: return "专业技术";
                case 9201: return "专业技术";
                case 9202: return "专业技术";
                case 9203: return "专业技术";
                case 9204: return "专业技术";


                case 10000: return "神之子";
                case 10100: return "神之子";
                case 10110: return "神之子";
                case 10111: return "神之子";
                case 10112: return "神之子";
                case 10114: return "神之子(6转)";

                case 11000: return "林之灵";
                case 11200: return "林之灵(1转)";
                case 11210: return "林之灵(2转)";
                case 11211: return "林之灵(3转)";
                case 11212: return "林之灵(4转)";
                case 11214: return "林之灵(6转)";

                case 12000: return "灶门炭治郎";
                case 12100: return "灶门炭治郎";
                case 12110: return "灶门炭治郎";
                case 12111: return "灶门炭治郎";
                case 12112: return "灶门炭治郎";

                case 13000: return "品克缤";
                case 13001: return "白雪人";
                case 13100: return "品克缤";
                case 13500: return "白雪人";

                case 14000: return "超能力者";
                case 14200: return "超能力者(1转)";
                case 14210: return "超能力者(2转)";
                case 14211: return "超能力者(3转)";
                case 14212: return "超能力者(4转)";
                case 14214: return "超能力者(6转)";

                case 15000: return "圣晶使徒";
                case 15001: return "影魂异人";
                case 15002: return "御剑骑士";
                case 15003: return "飞刃沙士";
                case 15100: return "御剑骑士(1转)";
                case 15110: return "御剑骑士(2转)";
                case 15111: return "御剑骑士(3转)";
                case 15112: return "御剑骑士(4转)";
                case 15114: return "御剑骑士(6转)";
                case 15200: return "圣晶使徒(1转)";
                case 15210: return "圣晶使徒(2转)";
                case 15211: return "圣晶使徒(3转)";
                case 15212: return "圣晶使徒(4转)";
                case 15214: return "圣晶使徒(6转)";
                case 15400: return "飞刃沙士(1转)";
                case 15410: return "飞刃沙士(2转)";
                case 15411: return "飞刃沙士(3转)";
                case 15412: return "飞刃沙士(4转)";
                case 15414: return "飞刃沙士(6转)";
                case 15500: return "影魂异人(1转)";
                case 15510: return "影魂异人(2转)";
                case 15511: return "影魂异人(3转)";
                case 15512: return "影魂异人(4转)";
                case 15514: return "影魂异人(6转)";

                case 16000: return "虎影";
                case 16001: return "元素师";
                case 16200: return "元素师(1转)";
                case 16210: return "元素师(2转)";
                case 16211: return "元素师(3转)";
                case 16212: return "元素师(4转)";
                case 16214: return "元素师(6转)";
                case 16400: return "虎影(1转)";
                case 16410: return "虎影(2转)";
                case 16411: return "虎影(3转)";
                case 16412: return "虎影(4转)";
                case 16414: return "虎影(6转)";

                case 17000: return "墨玄";
                case 17001: return "琳恩";
                case 17200: return "琳恩(1转)";
                case 17210: return "琳恩(2转)";
                case 17211: return "琳恩(3转)";
                case 17212: return "琳恩(4转)";
                case 17214: return "琳恩(5转)";
                case 17500: return "墨玄(1转)";
                case 17510: return "墨玄(2转)";
                case 17511: return "墨玄(3转)";
                case 17512: return "墨玄(4转)";
                case 17514: return "墨玄(6转)";


                case 40000: return "5转";
                case 40001: return "5转(战士)";
                case 40002: return "5转(魔法师)";
                case 40003: return "5转(弓箭手)";
                case 40004: return "5转(飞侠)";
                case 40005: return "5转(海盗)";
                case 50000: return "6转";
                case 50006: return "6转";
                case 50007: return "6转";
            }
            return null;
        }

        private static string ToChineseNumberExpr(long value)
        {
            var sb = new StringBuilder(16);
            bool firstPart = true;
            if (value < 0)
            {
                sb.Append("-");
                value = -value; // just ignore the exception -2147483648
            }
            if (value >= 1_0000_0000)
            {
                long part = value / 1_0000_0000;
                sb.AppendFormat("{0}亿", part);
                value -= part * 1_0000_0000;
                firstPart = false;
            }
            if (value >= 1_0000)
            {
                long part = value / 1_0000;
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}万", part);
                value -= part * 1_0000;
                firstPart = false;
            }
            if (value > 0)
            {
                sb.Append(firstPart ? null : " ");
                sb.AppendFormat("{0}", value);
            }

            return sb.Length > 0 ? sb.ToString() : "0";
        }
    }
}
