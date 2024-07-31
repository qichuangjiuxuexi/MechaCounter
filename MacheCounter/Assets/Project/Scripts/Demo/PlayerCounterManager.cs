using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WordGame.Utils;

namespace Project.Scripts
{
    public class PlayerCounterManager : IProcess
    {
        public List<PlayerCounter> PlayersData = new();

        public int RoundPlayerIndex;
        public void Init(List<IPlayerData> playersData)
        {
            InitPlayer(playersData);
            OnPlayerRoundIndexSet();
        }
        
        /// <summary>
        /// 初始化生成局内玩家信息
        /// </summary>
        /// <param name="playersData"></param>
        private void InitPlayer(List<IPlayerData> playersData)
        {
            for (var i = 0; i < playersData.Count; i++)
            {
                //todo：临时阵营
                PlayersData.Add(new PlayerCounter(i, i / 2, playersData[i]));
            }
        }

        /// <summary>
        /// 校验回合顺序链表
        /// </summary>
        private void OnPlayerRoundIndexSet()
        {
            PlayersData.Sort((a, b) => -a.PlayerData.Speed.CompareTo(b.PlayerData.Speed));
        }

        /// <summary>
        /// 角色角度是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsOver()
        {
            return PlayersData.Count(p => p.Hp > 0) == 1;
        }

        public void GetEffect(List<IEffect> effects)
        {
            if (effects is not { Count: > 0 }) return;
            //拿到目标是各自的效果并挂载到自身
            foreach (var playerCounter in PlayersData)
            {
                var terrainIndex = playerCounter.TerrainIndex;
                playerCounter.GetEffect(effects
                    .Where(e => e.Target == ETarget.TargetPlayer && e.TerrainIndex == terrainIndex).ToList());
            }
        }

        public void BeforeGameStart()
        {
            RoundPlayerIndex = 0;
        }

        public void GameOver()
        {
            
        }

        public void RoundBefore()
        {
            
            //计算当前回合行动的角色
            var playerData = PlayersData[RoundPlayerIndex];
            playerData.RoundBefore();
        }

        public void RoundStart()
        {
            var playerData = PlayersData[RoundPlayerIndex];
            //生成本回合攻击目标
            var activePlayer = PlayersData.Where(p => p.Hp > 0 && p.Camp != playerData.Camp).ToList();
            var random = Random.Range(0, activePlayer.Count);
            var targetPlayer = activePlayer[random];
            var targetPlayerId = targetPlayer.Id;
            //获得生成效果
            var effects= playerData.RoundStart(targetPlayerId);
            
            //todo: 被选为目标后执行当前获得效果
            var targetPlayerEffect = effects.Where(e => e.Target is ETarget.TargetPlayer).ToList();
            targetPlayer.TargetRound(targetPlayerEffect);
            
            //todo: 地形为目标后挂载效果
            //todo: 校验是否全局结束
            
        }

        public void RoundAfter()
        {
            var playerData = PlayersData[RoundPlayerIndex];
            playerData.RoundAfter();
            //todo: 校验是否全局结束
        }

        public void RoundOver()
        {
            while (PlayersData[RoundPlayerIndex].Hp < 0)
            {
                RoundPlayerIndex = (RoundPlayerIndex + 1) % PlayersData.Count;
            }
        }
    }
    
    
    /// <summary>
    /// 回合内控制
    /// </summary>
    public class PlayerCounter
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// 阵营
        /// </summary>
        public int Camp { get; private set; }
        
        /// <summary>
        /// 地形坐标
        /// </summary>
        public int TerrainIndex { get; private set; }
        public IPlayerData PlayerData { get; private set; }
        
        /// <summary>
        /// 每秒聚气进度
        /// </summary>
        public int Speed { get; }
        
        /// <summary>
        /// 聚气进度
        /// </summary>
        public int EnergyNum { get; private set; }
        
        /// <summary>
        /// 当前血量
        /// </summary>
        public long Hp { get; private set; }
        
        /// <summary>
        /// 当前蓝量
        /// </summary>
        public long Mp { get; private set; }

        /// <summary>
        /// 自身挂载效果
        /// </summary>
        public List<IEffect> SelfEffects = new();

        /// <summary>
        /// 主伤害技能
        /// </summary>
        public IDamageSkill[] AtkSkills;
        
        /// <summary>
        /// 运气技能
        /// </summary>
        public ISpeedSkill[] SpeedSkills;
        
        /// <summary>
        /// 内功技能
        /// </summary>
        public IBuffSkill[] BuffSkills;

        /// <summary>
        /// 是否配置了伤害技能
        /// </summary>
        public bool HasSkills => AtkSkills is { Length: > 0 };

        /// <summary>
        /// 下次释放技能的下标
        /// </summary>
        public int SkillIndex;
        
        /// <summary>
        /// 当前执行技能
        /// </summary>
        public IDamageSkill AtkSill;

        public PlayerCounter(int id, int camp, IPlayerData playerData)
        {
            TerrainIndex = 4;
            Id = id;
            Camp = camp;
            PlayerData = playerData;
            Hp = PlayerData.Hp;
            Mp = PlayerData.Mp;
            InitSkills();
        }

        private void InitSkills()
        {
            if (PlayerData.Skills is { Count: > 0 })
            {
                var atkSkills = PlayerData.Skills.Where(s => s.SkillType == ESkillType.DamageSkill).ToList();
                if (atkSkills is { Count: > 0 })
                {
                    AtkSkills = new IDamageSkill[atkSkills.Count];
                    for (var i = 0; i < atkSkills.Count; i++)
                    {
                        AtkSkills[i] = atkSkills[i] as IDamageSkill;
                    }
                }

                var speedSkills = PlayerData.Skills.Where(s => s.SkillType == ESkillType.Speed).ToList();
                if (speedSkills is { Count: > 0 })
                {
                    SpeedSkills = new ISpeedSkill[speedSkills.Count];
                    for (var i = 0; i < speedSkills.Count; i++)
                    {
                        SpeedSkills[i] = speedSkills[i] as ISpeedSkill;
                    }
                }

                var buffSkills = PlayerData.Skills.Where(s => s.SkillType == ESkillType.Buff).ToList();
                if (buffSkills is { Count: > 0 })
                {
                    BuffSkills = new IBuffSkill[buffSkills.Count];
                    for (var i = 0; i < buffSkills.Count; i++)
                    {
                        BuffSkills[i] = buffSkills[i] as IBuffSkill;
                    }
                }
            }
        }

        public List<IEffect> GetEffects()
        {
            var atkEffects = new List<IEffect>();
            return atkEffects;
        }

        /// <summary>
        /// 前置流程：计算Dot，计算回合前生效的Effect
        /// </summary>
        public void RoundBefore()
        {
            //前置效果生效
            var beforeEffect = SelfEffects.Where(e => e.TriggerTime == ETriggerTime.BeforeRound);
            beforeEffect.ForEach(e => e.TakeEffect(this));

            if (Hp <= 0)
            {
                //todo：发送阵亡事件
                return;
            }
            
            //设置当前
            AtkSill = AtkSkills[SkillIndex];
            //todo: 主动位移
            //位移至地形下标
            var startTerrainIndex = AtkSill.StartTerrainIndex;
        }        
        
        /// <summary>
        /// 回合种生效的Effect，计算当前回合触发的招式
        /// </summary>
        public List<IEffect> RoundStart(int targetPlayerId)
        {

            return GetEffects().Where(e => e.Target != ETarget.Self).ToList();
        }        
        
        /// <summary>
        /// 结束后流程
        /// </summary>
        public void RoundAfter()
        {
            //todo: 技能释放造成位移
            
            //更新下次要释放的技能
            SkillIndex = (SkillIndex + 1) % AtkSkills.Length;
        }

        /// <summary>
        /// 被选定目标流程
        /// </summary>
        /// <param name="effects"></param>
        public void TargetRound(List<IEffect> effects)
        {
            
        }

        /// <summary>
        /// 伤害计算
        /// </summary>
        public void OnDamageCalculate()
        {
            
        }

        public void GetEffect(List<IEffect> effects)
        {
            effects.ForEach(e => e.Target = ETarget.Self);
            SelfEffects.AddRange(effects);
        }
    }
}