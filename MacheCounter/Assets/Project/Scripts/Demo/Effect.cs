namespace Project.Scripts
{
    public enum EEffectBaseType
    {
        /// <summary>
        /// 眩晕
        /// </summary>
        BanPlayer = 1,
        /// <summary>
        /// 禁止先手
        /// </summary>
        BanActive = 1 << 2,
        /// <summary>
        /// 禁止后手
        /// </summary>
        BanBack = 1 << 3,
        /// <summary>
        /// 禁止运功
        /// </summary>
        BanEnhance = 1 << 4,
        /// <summary>
        /// 禁止先手后手
        /// </summary>
        BanActiveAndBack = BanActive & BanBack,
        /// <summary>
        /// 禁止精进
        /// </summary>
        BanWork = 1 << 5,
        Control = BanPlayer | BanActive | BanBack | BanEnhance | BanActiveAndBack | BanWork,
        /// <summary>
        /// 属性提升
        /// </summary>
        AttributePromotion = 1 << 6,
        /// <summary>
        /// 回血
        /// </summary>
        AddHp = 1 << 7,
        /// <summary>
        /// 回蓝
        /// </summary>
        AddMp = 1 << 8,
        /// <summary>
        /// 格挡
        /// </summary>
        Block = 1 << 9,
        Buff = AttributePromotion | AddHp | AddMp | Block,
        /// <summary>
        /// 属性减少
        /// </summary>
        AttributeReduce = 1 << 10,
        /// <summary>
        /// 中毒
        /// </summary>
        Poisoning = 1 << 11,
        /// <summary>
        /// 流血
        /// </summary>
        Bleed = 1 << 12,
        /// <summary>
        /// 燃烧
        /// </summary>
        Firing = 1 << 13,
        /// <summary>
        /// 霜寒
        /// </summary>
        Frostbite = 1 << 14,
        /// <summary>
        /// 失去蓝内伤
        /// </summary>
        LoseMp = 1 << 15,
        /// <summary>
        /// 破绽
        /// </summary>
        Break = 1 << 16,
        DeBuff = AttributeReduce | Poisoning | Bleed | Firing | Frostbite | LoseMp | Break,
        /// <summary>
        /// 冰冻
        /// </summary>
        Frozen = 1 << 17,
        /// <summary>
        /// 持续伤害
        /// </summary>
        Aot = Poisoning | Bleed | Firing | Frostbite | Frozen,
        /// <summary>
        /// 连击
        /// </summary>
        DoubleHit = 1 << 18
    }

    /// <summary>
    /// 触发时机
    /// </summary>
    public enum ETriggerTime
    {
        /// <summary>
        /// 立即触发
        /// </summary>
        Now,
        /// <summary>
        /// 回合开始前
        /// </summary>
        BeforeRound,
        /// <summary>
        /// 回合结束
        /// </summary>
        AfterRound
    }

    /// <summary>
    /// 效果
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// 效果类型
        /// </summary>
        public int Type { get; }
        
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// 作用对象
        /// </summary>
        public int Target { get; }
        
        /// <summary>
        /// 效果等级
        /// </summary>
        public int Level { get; }
        
        /// <summary>
        /// 持续回合数
        /// </summary>
        public int Round { get; }
        
        /// <summary>
        /// 触发时机
        /// </summary>
        public ETriggerTime TriggerTime { get; }

        /// <summary>
        /// 效果生效
        /// </summary>
        public void TakeEffect();
    }
    
    public class Effect
    {
        
    }
}