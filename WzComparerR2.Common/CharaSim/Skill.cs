using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WzComparerR2.WzLib;

namespace WzComparerR2.CharaSim
{
    public class Skill
    {
        public Skill()
        {
            this.level = 0;
            this.levelCommon = new List<Dictionary<string, string>>();
            this.common = new Dictionary<string, string>();
            this.PVPcommon = new Dictionary<string, string>();
            this.RelationSkill = null;
            this.ReqSkill = new Dictionary<int, int>();
            this.Action = new List<string>();
        }

        private int level;
        internal List<Dictionary<string, string>> levelCommon;
        internal Dictionary<string, string> common;

        public Dictionary<string, string> Common
        {
            get
            {
                if (PreBBSkill && this.level > 0 && this.level <= levelCommon.Count)
                    return levelCommon[this.level - 1];
                else
                    return common;
            }
        }

        public Dictionary<string, string> PVPcommon { get; private set; }
        public int SkillID { get; set; }
        public BitmapOrigin Icon { get; set; }
        public BitmapOrigin IconMouseOver { get; set; }
        public BitmapOrigin IconDisabled { get; set; }

        public HyperSkillType Hyper { get; set; }
        public bool HyperStat { get; set; }
        public bool applyHyper { get; set; }

        public int Level
        {
            get { return level; }
            set
            {
                bool canBreakLevel = this.CombatOrders || this.VSkill
                    || this.SkillID / 100000 == 4000; //fix for evan
                int maxLevel = canBreakLevel ? 100 : this.MaxLevel;
                level = Math.Max(0, Math.Min(value, maxLevel));
            }
        }

        public int ReqLevel { get; set; }
        public int ReqAmount { get; set; }
        public bool PreBBSkill { get; set; }
        public bool Invisible { get; set; }
        public bool invisible_tw { get; set; }
        public bool disable { get; set; }
        public bool areaAttack { get; set; }
        public bool chainAttack { get; set; }
        public bool isFieldMovingLimit { get; set; }
        public bool ignoreActionWhenAddAttackProc { get; set; }
        public bool CombatOrders { get; set; }
        public bool microCooltime { get; set; }
        public bool NotRemoved { get; set; }
        public bool notExtend { get; set; }
        public bool notCanceled { get; set; }
        public bool cannotCancelByRButton { get; set; }
        public bool disabledDuringAction { get; set; }
        public bool notIncBuffDuration { get; set; }
        public bool notCooltimeReset { get; set; }
        public bool notCooltimeReduce { get; set; }
        public bool showStackCooltimeNum { get; set; }
        public bool ignoreSpecialCore { get; set; }
        public bool blockBattleStats { get; set; }
        public bool applyPassiveLvUp { get; set; }
        public bool preloadEff { get; set; }
        public bool VSkill { get; set; }
        public bool canJobRidingUse { get; set; }
        public bool canNotStealableSkill { get; set; }
        public int VSkillValue { get; set; }
        public int alertTime { get; set; }
        public bool Origin { get; set; }
        public bool TimeLimited { get; set; }
        public bool isCancelableBuff { get; set; }
        public bool fixEffectCanceled { get; set; }
        public bool ignoreCounter { get; set; }
        public bool ignoreUserAttackSuccess { get; set; }
        public bool cancelBuffByForbiddenField { get; set; }
        public bool notRemoveEffectByCancelSkill { get; set; }
        public bool RemoveBuffByRemoveSummon { get; set; }
        public bool notResetDarkSight { get; set; }
        public bool resetDarkSightOnShoot { get; set; }
        public bool ignoreResetDarkSightOnAttack { get; set; }
        public bool canSummonedAttackOnDarkSight { get; set; }
        public bool maintainSplitSummoned { get; set; }
        public bool showSummonedBuffIcon { get; set; }
        public bool jobShield { get; set; }
        public bool rectBasedOnTarget { get; set; }
        public bool normalAttackAtBulletEmpty { get; set; }
        public bool hideBullet { get; set; }
        public bool noBulletApply { get; set; }
        public bool isUsableSylphdia { get; set; }
        public bool canOverlapOnMegasmasher { get; set; }
        public bool canOverlapOnFullBurst { get; set; }
        public bool isUsableKaiserDragon { get; set; }
        public bool canGloryWingUse { get; set; }
        public bool isUsableExpressionRiver { get; set; }
        public bool canRidingUse { get; set; }
        public bool teleportVehicle { get; set; }
        public bool ableTeleportWorldmap { get; set; }
        public bool makeEventPointByMobDead { get; set; }
        public bool footholdAffectedArea { get; set; }
        public bool footholdInstallSummoned { get; set; }
        public bool addAttackCoolTime { get; set; }
        public Tuple<int, int> RelationSkill { get; set; }
        public bool IsPetAutoBuff { get; set; }
        public bool DisableNextLevelInfo { get; set; }
        public bool petPassive { get; set; }
        public bool partyPassive { get; set; }
        public bool repeatDance { get; set; }
        public bool preservedOnDead { get; set; }
        public bool infiniteTime { get; set; }
        public int MasterLevel { get; set; }
        public int activeDelay { get; set; }
        public Dictionary<int, int> ReqSkill { get; private set; }
        public List<string> Action { get; private set; }
        public int AddAttackToolTipDescSkill { get; set; }
        public int AssistSkillLink { get; set; }
        public int VehicleID { get; set; }
        public int makeMesoByMobDead { get; set; }
        public int makeMesoByMobDead_reboot { get; set; }
        public int screenPoolCount { get; set; }
        public bool bossCoinWorthR { get; set; }
        public bool bossRewardDropR { get; set; }
        public bool collabo { get; set; }
        public int reqGuildLv { get; set; }
        public Point LT { get; set; }
        public Point RB { get; set; }
        public int MaxLevel
        {
            get
            {
                string v;
                if (this.PreBBSkill)
                    return levelCommon.Count;
                else if (common.TryGetValue("maxLevel", out v))
                    return Convert.ToInt32(v);
                return 0;
            }
        }

        public static Skill CreateFromNode(Wz_Node node, GlobalFindNodeFunction findNode, Wz_File wzf = null)
        {
            Skill skill = new Skill();
            int skillID;
            if (!Int32.TryParse(node.Text, out skillID))
                return null;
            skill.SkillID = skillID;

            foreach (Wz_Node childNode in node.Nodes)
            {
                switch (childNode.Text)
                {
                    case "icon":
                        skill.Icon = BitmapOrigin.CreateFromNode(childNode, findNode, wzf);
                        break;
                    case "iconMouseOver":
                        skill.IconMouseOver = BitmapOrigin.CreateFromNode(childNode, findNode, wzf);
                        break;
                    case "iconDisabled":
                        skill.IconDisabled = BitmapOrigin.CreateFromNode(childNode, findNode, wzf);
                        break;
                    case "common":
                        foreach (Wz_Node commonNode in childNode.Nodes)
                        {
                            if (commonNode.Value != null && !(commonNode.Value is Wz_Vector))
                            {
                                skill.common[commonNode.Text] = commonNode.Value.ToString();
                            }
                            else if (commonNode.Value != null && commonNode.Value is Wz_Vector)
                            {
                                Wz_Vector cNode = commonNode.Value as Wz_Vector;
                                if (commonNode.Text == "lt")
                                {
                                    skill.LT = new Point(cNode.X, cNode.Y);
                                }
                                else if (commonNode.Text == "rb")
                                {
                                    skill.RB = new Point(cNode.X, cNode.Y);
                                }
                            }
                        }
                        break;
                    case "PVPcommon":
                        foreach (Wz_Node commonNode in childNode.Nodes)
                        {
                            if (commonNode.Value != null && !(commonNode.Value is Wz_Vector))
                            {
                                skill.PVPcommon[commonNode.Text] = commonNode.Value.ToString();
                            }
                        }
                        break;
                    case "level":
                        for (int i = 1; ; i++)
                        {
                            Wz_Node levelNode = childNode.FindNodeByPath(i.ToString());
                            if (levelNode == null)
                                break;
                            Dictionary<string, string> levelInfo = new Dictionary<string, string>();

                            foreach (Wz_Node commonNode in levelNode.Nodes)
                            {
                                if (commonNode.Value != null && !(commonNode.Value is Wz_Vector))
                                {
                                    levelInfo[commonNode.Text] = commonNode.Value.ToString();
                                }
                            }

                            skill.levelCommon.Add(levelInfo);
                        }
                        break;
                    case "hyper":
                        skill.Hyper = (HyperSkillType)childNode.GetValue<int>();
                        break;
                    case "hyperStat":
                        skill.HyperStat = childNode.GetValue<int>() != 0;
                        break;
                    case "applyHyper":
                        skill.applyHyper = childNode.GetValue<int>() != 0;
                        break;
                    case "invisible":
                        skill.Invisible = childNode.GetValue<int>() != 0;
                        break;
                    case "invisible_tw":
                        skill.invisible_tw = childNode.GetValue<int>() != 0;
                        break;
                    case "disable":
                        skill.disable = childNode.GetValue<int>() != 0;
                        break;
                    case "areaAttack":
                        skill.areaAttack = childNode.GetValue<int>() != 0;
                        break;
                    case "chainAttack":
                        skill.chainAttack = childNode.GetValue<int>() != 0;
                        break;
                    case "isFieldMovingLimit":
                        skill.isFieldMovingLimit = childNode.GetValue<int>() != 0;
                        break;
                    case "combatOrders":
                        skill.CombatOrders = childNode.GetValue<int>() != 0;
                        break;
                    case "microCooltime":
                        skill.microCooltime = childNode.GetValue<int>() != 0;
                        break;
                    case "disabledDuringAction":
                        skill.disabledDuringAction = childNode.GetValue<int>() != 0;
                        break;
                    case "ignoreActionWhenAddAttackProc":
                        skill.ignoreActionWhenAddAttackProc = childNode.GetValue<int>() != 0;
                        break;
                    case "notExtend":
                        skill.notExtend = childNode.GetValue<int>() != 0;
                        break;
                    case "notRemoved":
                        skill.NotRemoved = childNode.GetValue<int>() != 0;
                        break;
                    case "notCanceled":
                        skill.notCanceled = childNode.GetValue<int>() != 0;
                        break;
                    case "notIncBuffDuration":
                        skill.notIncBuffDuration = childNode.GetValue<int>() != 0;
                        break;
                    case "notCooltimeReset":
                        skill.notCooltimeReset = childNode.GetValue<int>() != 0;
                        break;
                    case "notCooltimeReduce":
                        skill.notCooltimeReduce = childNode.GetValue<int>() != 0;
                        break;
                    case "showStackCooltimeNum":
                        skill.showStackCooltimeNum = childNode.GetValue<int>() != 0;
                        break;
                    case "ignoreSpecialCore":
                        skill.ignoreSpecialCore = childNode.GetValue<int>() != 0;
                        break;
                    case "blockBattleStats":
                        skill.blockBattleStats = childNode.GetValue<int>() != 0;
                        break;
                    case "applyPassiveLvUp":
                        skill.applyPassiveLvUp = childNode.GetValue<int>() != 0;
                        break;
                    case "preservedOnDead":
                        skill.preservedOnDead = childNode.GetValue<int>() != 0;
                        break;
                    case "vSkill":
                        skill.VSkill = childNode.GetValue<int>() != 0;
                        skill.VSkillValue = childNode.GetValue<int>();
                        break;
                    case "preloadEff":
                        skill.preloadEff = childNode.GetValue<int>() != 0;
                        break;
                    case "footholdAffectedArea":
                        skill.footholdAffectedArea = childNode.GetValue<int>() != 0;
                        break;
                    case "footholdInstallSummoned":
                        skill.footholdInstallSummoned = childNode.GetValue<int>() != 0;
                        break;
                    case "addAttackCoolTime":
                        skill.addAttackCoolTime = childNode.GetValue<int>() != 0;
                        break;
                    case "origin":
                        skill.Origin = childNode.GetValue<int>() != 0;
                        break;
                    case "timeLimited":
                        skill.TimeLimited = childNode.GetValue<int>() != 0;
                        break;
                    case "relationSkill":
                        skill.RelationSkill = Tuple.Create(childNode.Nodes["skillID"].GetValueEx<int>(0), childNode.Nodes["periodMin"].GetValueEx<int>(0));
                        break;
                    case "isPetAutoBuff":
                        skill.IsPetAutoBuff = childNode.GetValue<int>() != 0;
                        break;
                    case "petPassive":
                        skill.petPassive = childNode.GetValue<int>() != 0;
                        break;
                    case "partyPassive":
                        skill.partyPassive = childNode.GetValue<int>() != 0;
                        break;
                    case "disableNextLevelInfo":
                        skill.DisableNextLevelInfo = childNode.GetValue<int>() != 0;
                        break;
                    case "masterLevel":
                        skill.MasterLevel = childNode.GetValue<int>();
                        break;
                    case "activeDelay":
                        skill.activeDelay = childNode.GetValue<int>();
                        break;
                    case "reqGuildLv":
                        skill.reqGuildLv = childNode.GetValue<int>();
                        break;
                    case "reqLev":
                        skill.ReqLevel = childNode.GetValue<int>();
                        break;
                    case "req":
                        foreach (Wz_Node reqNode in childNode.Nodes)
                        {
                            if (reqNode.Text == "level")
                            {
                                skill.ReqLevel = reqNode.GetValue<int>();
                            }
                            else if (reqNode.Text == "reqAmount")
                            {
                                skill.ReqAmount = reqNode.GetValue<int>();
                            }
                            else
                            {
                                int reqSkill;
                                if (Int32.TryParse(reqNode.Text, out reqSkill))
                                {
                                    skill.ReqSkill[reqSkill] = reqNode.GetValue<int>();
                                }
                            }
                        }
                        break;
                    case "action":
                        for (int i = 0; ; i++)
                        {
                            Wz_Node idxNode = childNode.FindNodeByPath(i.ToString());
                            if (idxNode == null)
                                break;
                            skill.Action.Add(idxNode.GetValue<string>());
                        }
                        break;
                    case "addAttack":
                        Wz_Node toolTipDescNode = childNode.FindNodeByPath("toolTipDesc");
                        if (toolTipDescNode != null && toolTipDescNode.GetValue<int>() != 0)
                        {
                            skill.AddAttackToolTipDescSkill = childNode.FindNodeByPath("toolTipDescSkill").GetValue<int>();
                        }
                        break;
                    case "assistSkillLink":
                        skill.AssistSkillLink = childNode.FindNodeByPath("skill").GetValue<int>();
                        break;
                    case "vehicleID":
                        skill.VehicleID = childNode.GetValue<int>();
                        break;
                    case "makeMesoByMobDead":
                        skill.makeMesoByMobDead = childNode.GetValue<int>();
                        break;
                    case "makeMesoByMobDead_reboot":
                        skill.makeMesoByMobDead_reboot = childNode.GetValue<int>();
                        break;
                    case "screenPoolCount":
                        skill.screenPoolCount = childNode.GetValue<int>();
                        break;
                    case "alertTime":
                        skill.alertTime = childNode.GetValue<int>();
                        break;
                    case "canJobRidingUse":
                        skill.canJobRidingUse = childNode.GetValue<int>() != 0;
                        break;
                    case "canNotStealableSkill":
                        skill.canNotStealableSkill = childNode.GetValue<int>() != 0;
                        break;
                    case "isCancelableBuff":
                        skill.isCancelableBuff = childNode.GetValue<int>() != 0;
                        break;
                    case "fixEffectCanceled":
                        skill.fixEffectCanceled = childNode.GetValue<int>() != 0;
                        break;
                    case "ignoreCounter":
                        skill.ignoreCounter = childNode.GetValue<int>() != 0;
                        break;
                    case "ignoreUserAttackSuccess":
                        skill.ignoreUserAttackSuccess = childNode.GetValue<int>() != 0;
                        break;
                    case "cancelBuffByForbiddenField":
                        skill.cancelBuffByForbiddenField = childNode.GetValue<int>() != 0;
                        break;
                    case "notRemoveEffectByCancelSkill":
                        skill.notRemoveEffectByCancelSkill = childNode.GetValue<int>() != 0;
                        break;
                    case "RemoveBuffByRemoveSummon":
                        skill.RemoveBuffByRemoveSummon = childNode.GetValue<int>() != 0;
                        break;
                    case "maintainSplitSummoned":
                        skill.maintainSplitSummoned = childNode.GetValue<int>() != 0;
                        break;
                    case "notResetDarkSight":
                        skill.notResetDarkSight = childNode.GetValue<int>() != 0;
                        break;
                    case "resetDarkSightOnShoot":
                        skill.resetDarkSightOnShoot = childNode.GetValue<int>() != 0;
                        break;
                    case "ignoreResetDarkSightOnAttack":
                        skill.ignoreResetDarkSightOnAttack = childNode.GetValue<int>() != 0;
                        break;
                    case "canSummonedAttackOnDarkSight":
                        skill.canSummonedAttackOnDarkSight = childNode.GetValue<int>() != 0;
                        break;
                    case "isUsableKaiserDragon":
                        skill.isUsableKaiserDragon = childNode.GetValue<int>() != 0;
                        break;
                    case "showSummonedBuffIcon":
                        skill.showSummonedBuffIcon = childNode.GetValue<int>() != 0;
                        break;
                    case "jobShield":
                        skill.jobShield = childNode.GetValue<int>() != 0;
                        break;
                    case "rectBasedOnTarget":
                        skill.rectBasedOnTarget = childNode.GetValue<int>() != 0;
                        break;
                    case "normalAttackAtBulletEmpty":
                        skill.normalAttackAtBulletEmpty = childNode.GetValue<int>() != 0;
                        break;
                    case "hideBullet":
                        skill.hideBullet = childNode.GetValue<int>() != 0;
                        break;
                    case "noBulletApply":
                        skill.noBulletApply = childNode.GetValue<int>() != 0;
                        break;
                    case "isUsableSylphdia":
                        skill.isUsableSylphdia = childNode.GetValue<int>() != 0;
                        break;
                    case "canOverlapOnMegasmasher":
                        skill.canOverlapOnMegasmasher = childNode.GetValue<int>() != 0;
                        break;
                    case "canOverlapOnFullBurst":
                        skill.canOverlapOnFullBurst = childNode.GetValue<int>() != 0;
                        break;
                    case "canGloryWingUse":
                        skill.canGloryWingUse = childNode.GetValue<int>() != 0;
                        break;
                    case "isUsableExpressionRiver":
                        skill.isUsableExpressionRiver = childNode.GetValue<int>() != 0;
                        break;
                    case "teleportVehicle":
                        skill.teleportVehicle = childNode.GetValue<int>() != 0;
                        break;
                    case "ableTeleportWorldmap":
                        skill.ableTeleportWorldmap = childNode.GetValue<int>() != 0;
                        break;
                    case "makeEventPointByMobDead":
                        skill.makeEventPointByMobDead = childNode.GetValue<int>() != 0;
                        break;
                    case "bossCoinWorthR":
                        skill.bossCoinWorthR = childNode.GetValue<int>() != 0;
                        break;
                    case "bossRewardDropR":
                        skill.bossRewardDropR = childNode.GetValue<int>() != 0;
                        break;
                    case "repeatDance":
                        skill.repeatDance = childNode.GetValue<int>() != 0;
                        break;
                    case "infiniteTime":
                        skill.infiniteTime = childNode.GetValue<int>() != 0;
                        break;
                    case "collabo":
                        skill.collabo = childNode.GetValue<int>() != 0;
                        break;
                }
            }

            if ((skill.common.ContainsKey("forceCon") || (skill.levelCommon.Count > 0 && skill.levelCommon[0].ContainsKey("forceCon"))) && skill.Hyper == HyperSkillType.None)
            {
                Wz_Node forceNode = null;
                if (skill.SkillID / 10000 == 3001 || skill.SkillID / 10000 == 3100 || skill.SkillID / 10000 == 3110 || skill.SkillID / 10000 == 3111 || skill.SkillID / 10000 == 3112)
                {
                    forceNode = findNode.Invoke(string.Format("UI\\UIWindow2.img\\Skill\\main\\Force\\{0}", (Int32.Parse(skill.common["forceCon"]) - 1) / 30));
                }
                else if (skill.SkillID / 10000 / 1000 == 10)
                {
                    forceNode = findNode.Invoke(string.Format("UI\\UIWindow2.img\\SkillZero\\main\\Alpha\\{0}", skill.SkillID / 1000 % 10));
                }
                if (forceNode != null)
                {
                    BitmapOrigin force = BitmapOrigin.CreateFromNode(forceNode, findNode, wzf);
                    using (Graphics graphics = Graphics.FromImage(skill.Icon.Bitmap))
                    {
                        graphics.DrawImage(force.Bitmap, new Point(0, 0));
                    }
                    using (Graphics graphics = Graphics.FromImage(skill.IconMouseOver.Bitmap))
                    {
                        graphics.DrawImage(force.Bitmap, new Point(0, 0));
                    }
                    using (Graphics graphics = Graphics.FromImage(skill.IconDisabled.Bitmap))
                    {
                        graphics.DrawImage(force.Bitmap, new Point(0, 0));
                    }
                }
            }

            //判定技能声明版本
            skill.PreBBSkill = false;
            if (skill.levelCommon.Count > 0)
            {
                if (skill.common.Count <= 0 || skill.common.ContainsKey("maxLevel"))
                {
                    skill.PreBBSkill = true;
                }
            }

            return skill;
        }
    }
}
