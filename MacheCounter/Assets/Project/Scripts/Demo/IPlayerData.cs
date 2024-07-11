using System.Collections.Generic;
using WordGame.Utils;

namespace Project.Scripts
{
    /// <summary>
    /// 数据参数
    /// </summary>
    public interface IPlayerData : IBaseAttribute
    {
        /// <summary>
        /// 枪械攻击力
        /// </summary>
        public int GunAtk { get; }
        
        /// <summary>
        /// 拳攻击力
        /// </summary>
        public int BoxingAtk { get; }
        
        /// <summary>
        /// 炮攻击力
        /// </summary>
        public int CannonAtk { get; }
        
        /// <summary>
        /// 刀剑攻击力
        /// </summary>
        public int SwordAtk { get; }
        
        /// <summary>
        /// 枪械减伤
        /// </summary>
        public int GunReduce { get; }
        
        /// <summary>
        /// 拳头减伤
        /// </summary>
        public int BoxingReduce { get; }
        
        /// <summary>
        /// 炮减伤
        /// </summary>
        public int CannonReduce { get; }
        
        /// <summary>
        /// 光剑减伤
        /// </summary>
        public int SwordReduce { get; }
        
        /// <summary>
        /// 枪械增伤
        /// </summary>
        public int GunIncreased  { get; }
        
        /// <summary>
        /// 拳头增伤
        /// </summary>
        public int BoxingIncreased { get; }
        
        /// <summary>
        /// 炮增伤
        /// </summary>
        public int CannonIncreased { get; }
        
        /// <summary>
        /// 光剑增伤
        /// </summary>
        public int SwordIncreased { get; }
    }

    public interface IBaseAttribute
    {
        /// <summary>
        /// 攻击力（普攻和招式）
        /// </summary>
        public int Atk { get; }
        
        /// <summary>
        /// 防御力
        /// </summary>
        public int Def { get; }
        
        /// <summary>
        /// 装甲耐久
        /// </summary>
        public long Hp { get; }
        
        /// <summary>
        /// 能量
        /// </summary>
        public long Mp { get; }
        
        /// <summary>
        /// 聚能
        /// </summary>
        public long Speed { get; }

        /// <summary>
        /// 技能列表
        /// </summary>
        public HashSet<ISkill> Skills { get; }
    }
    
    /// <summary>
    /// 属性类型
    /// </summary>
    public enum EAttributeType
    {
        /// <summary>
        /// 攻击
        /// </summary>
        Atk = 1,
        /// <summary>
        /// 防御
        /// </summary>
        Dfs = 2,
        /// <summary>
        /// 气血
        /// </summary>
        Hp = 3,
        /// <summary>
        /// 魔力值
        /// </summary>
        Mp = 4,
        /// <summary>
        /// 聚能
        /// </summary>
        Speed = 5,
        /// <summary>
        /// 武器系列攻击
        /// </summary>
        FourAtk = 6,
        /// <summary>
        /// 枪械系攻击
        /// </summary>
        GunAtk = 7,
        /// <summary>
        /// 炮系攻击
        /// </summary>
        CannonAtk = 8,
        /// <summary>
        /// 拳系攻击
        /// </summary>
        BoxingAtk = 9,
        /// <summary>
        /// 刀剑系攻击
        /// </summary>
        SwordAtk = 10,
        /// <summary>
        /// 枪械系减伤
        /// </summary>
        GunReduce = 11,
        /// <summary>
        /// 拳系减伤
        /// </summary>
        BoxingReduce = 12,
        /// <summary>
        /// 炮系减伤
        /// </summary>
        CannonReduce = 13,
        /// <summary>
        /// 刀剑系减伤
        /// </summary>
        SwordReduce = 14,
        /// <summary>
        /// 枪械系增伤
        /// </summary>
        GunIncreased = 15,
        /// <summary>
        /// 拳系增伤
        /// </summary>
        BoxingIncreased = 16,
        /// <summary>
        /// 炮系增伤
        /// </summary>
        CannonIncreased = 17,
        /// <summary>
        /// 刀剑系增伤
        /// </summary>
        SwordIncreased = 18,
    }

    public interface IPropertyItem
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public EAttributeType Type { get; }
        
        /// <summary>
        /// 属性参数
        /// </summary>
        public long Num { get; }
    }

    public class PropertyItem : IPropertyItem
    {
        public EAttributeType Type { get; }
        public long Num { get; }

        public PropertyItem(int type, long num)
        {
            Type = (EAttributeType)type;
            Num = num;
        }
    }

    public class PlayerCounter
    {
        public IPlayerData PlayerData { get; private set; }
        
        /// <summary>
        /// 每秒聚气进度
        /// </summary>
        public float Speed { get; }
        
        /// <summary>
        /// 聚气进度
        /// </summary>
        public float EnergyNum { get; private set; }
        
        /// <summary>
        /// 当前血量
        /// </summary>
        public long Hp { get; private set; }
        
        /// <summary>
        /// 当前蓝量
        /// </summary>
        public long Mp { get; private set; }

        public List<IEffect> SelfEffects;

        public PlayerCounter(IPlayerData playerData)
        {
            PlayerData = playerData;
            Hp = PlayerData.Hp;
            Mp = PlayerData.Mp;
        }

        public List<IEffect> GetAtkEffects()
        {
            var atkEffects = new List<IEffect>();
            return atkEffects;
        }
    }
}