using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WzComparerR2.WzLib;

namespace WzComparerR2.CharaSim
{
    public class Achievement
    {
        public Achievement()
        {
            this.ID = -1;
            this.PriorIDs = new List<int>();
            this.Missions = new List<string>();
            this.Rewards = new List<AchievementReward>();
        }

        private string _mainCategory { get; set; }
        private string _subCategory { get; set; }
        public int ID { get; set; }
        public int Score { get; set; }
        public string MainCategory
        {
            get { return GetMainCategoryStr(); }
        }
        public string SubCategory
        {
            get { return GetSubCategoryStr(); }
        }
        public string Difficulty { get; set; }
        public string UiForm { get; set; }
        public string Block { get; set; }
        public string PriorCondition { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public List<int> PriorIDs { get; set; }
        public List<string> Missions { get; set; }
        public List<AchievementReward> Rewards { get; set; }

        public bool ShowMissions
        {
            get
            {
                return (this.UiForm == "mission" || this.UiForm == "all")
                    && this.Missions.Count > 0;
            }
        }
        public bool HasRewards
        {
            get { return this.Rewards.Count > 0; }
        }
        public bool Hide
        {
            get { return this.Block == "hide"; }
        }

        public static Achievement CreateFromNode(
            Wz_Node node,
            GlobalFindNodeFunction findNode,
            GlobalFindNodeFunction2 findNode2,
            Wz_File wzf = null
        )
        {
            if (node == null)
                return null;

            Match m = Regex.Match(node.Text, @"^(\d+)\.img$");
            if (!(m.Success && Int32.TryParse(m.Result("$1"), out int achievementID)))
            {
                return null;
            }

            Achievement achievement = new Achievement();
            achievement.ID = achievementID;
            Wz_Node infoNode = node.FindNodeByPath("info").ResolveUol();
            if (infoNode != null)
            {
                foreach (var propNode in infoNode.Nodes)
                {
                    switch (propNode.Text)
                    {
                        case "score":
                            achievement.Score = propNode.GetValueEx<int>(0);
                            break;
                        case "mainCategory":
                            achievement._mainCategory = propNode.GetValueEx<string>(null);
                            break;
                        case "subCategory":
                            achievement._subCategory = propNode.GetValueEx<string>(null);
                            break;
                        case "difficulty":
                            achievement.Difficulty = propNode.GetValueEx<string>("normal");
                            break;
                        case "prior":
                            var prior = propNode.FindNodeByPath("achievement_id");
                            if (prior != null)
                                achievement.PriorIDs.Add(prior.GetValueEx<int>(-1));
                            else
                            {
                                var valueNode = propNode.FindNodeByPath("values");
                                foreach (
                                    var value in valueNode?.Nodes
                                        ?? new Wz_Node.WzNodeCollection(null)
                                )
                                {
                                    prior = value.FindNodeByPath("achievement_id");
                                    var priorID = prior.GetValueEx<int>(-1);
                                    if (priorID > -1)
                                    {
                                        achievement.PriorIDs.Add(prior.GetValueEx<int>(priorID));
                                    }
                                }
                            }
                            achievement.PriorCondition = propNode
                                .FindNodeByPath("condition")
                                .GetValueEx<string>(null);
                            break;
                        case "uiType":
                            achievement.UiForm = propNode
                                .FindNodeByPath("uiForm")
                                .GetValueEx<string>("basic");
                            break;
                        case "block":
                            achievement.Block = propNode.GetValueEx<string>("none");
                            break;
                        case "period":
                            achievement.Start = propNode
                                .FindNodeByPath("start")
                                .GetValueEx<string>(null);
                            achievement.End = propNode
                                .FindNodeByPath("end")
                                .GetValueEx<string>(null);
                            break;
                    }
                }
            }

            Wz_Node missionNode = node.FindNodeByPath("mission").ResolveUol();
            if (missionNode != null)
            {
                foreach (var mission in missionNode.Nodes)
                {
                    var missionName = mission.FindNodeByPath("name").GetValueEx<string>(null);
                    if (!string.IsNullOrEmpty(missionName))
                    {
                        achievement.Missions.Add(missionName);
                    }
                }
            }

            Wz_Node rewardNode = node.FindNodeByPath("reward").ResolveUol();
            if (rewardNode != null)
            {
                foreach (var reward in rewardNode.Nodes)
                {
                    var id = reward.FindNodeByPath("id").GetValueEx<int>(-1);
                    var desc = reward.FindNodeByPath("desc").GetValueEx<string>(null);
                    if (id >= 0 && !string.IsNullOrEmpty(desc))
                    {
                        achievement.Rewards.Add(new AchievementReward() { ID = id, Desc = desc });
                    }
                }
            }

            return achievement;
        }

        private string GetMainCategoryStr()
        {
            // Etc/Achievement/AchievementInfo.img/Category
            switch (this._mainCategory)
            {
                case "general":
                    return "普通";
                case "growth":
                    return "养成";
                case "job":
                    return "职业";
                case "item":
                    return "道具";
                case "adventure":
                    return "冒险";
                case "battle":
                    return "战斗";
                case "social":
                    return "社交";
                case "event":
                    return "活动";
                case "memory":
                    return "回忆";

                default:
                    return this._mainCategory;
            }
        }

        private string GetSubCategoryStr()
        {
            // Etc/Achievement/AchievementInfo.img/Category
            switch (this._mainCategory)
            {
                case "general":
                case "event":
                case "memory":
                    return null;
            }

            switch (this._subCategory)
            {
                case "level":
                    return "等级";
                case "stat":
                    return "能力值";
                case "personality":
                    return "倾向";
                case "makingSkill":
                    return "专业技术";
                case "union":
                    return "联盟";

                case "story":
                    return "剧情";
                case "jobChange":
                    return "转职";
                case "skill":
                    return "技能";
                case "vMatrix":
                    return "V矩阵";
                case "linkSkill":
                    return "链接技能";

                case "collection":
                    return "收集";
                case "enchantment":
                    return "强化";
                case "equip":
                    return "穿戴";

                case "exploration":
                    return "探险";
                case "quest":
                    return "任务";
                case "cooperation":
                    return "协作";
                case "special":
                    return "特殊";

                case "field":
                    return "区域";
                case "boss":
                    return "首领怪";
                case "loot":
                    return "战利品";

                case "party":
                    return "组队";
                case "guild":
                    return "家族";
                case "trade":
                    return "交易";
                case "etc":
                    return "其他";

                case "progress":
                    return "正在进行的活动";
                case "complete":
                    return "上一个活动";

                default:
                    return this._subCategory;
            }
        }

        public struct AchievementReward
        {
            public int ID;
            public string Desc;
        }
    }
}
