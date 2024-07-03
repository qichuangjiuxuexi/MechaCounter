namespace Project.Scripts
{
    public enum EEffectBaseType
    {
        BanPlayer = 1,
        BanActive = 1 << 2,
        BanBack = 1 << 3,
        BanEnhance = 1 << 4,
        BanActiveAndBack = BanActive | BanBack,
        BanWork = 1 << 5,
        Control = BanPlayer | BanActive | BanBack | BanEnhance | BanActiveAndBack | BanWork,
        AttributePromotion = 1 << 6,
        AddHp = 1 << 7,
        AddMp = 1 << 8,
        Block = 1 << 9,
        Buff = AttributePromotion | AddHp | AddMp | Block,
        AttributeReduce = 1 << 10,
        Poisoning = 1 << 11,
        Bleed = 1 << 12,
        Firing = 1 << 13,
        Frostbite = 1 << 14,
        LoseMp = 1 << 15,
        Break = 1 << 16,
        DeBuff = AttributeReduce | Poisoning | Bleed | Firing | Frostbite | LoseMp | Break,
        Aot = Poisoning | Bleed | Firing | Frostbite,
        DoubleHit = 1 << 17
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
        public int EffectTime { get; }

        /// <summary>
        /// 效果生效
        /// </summary>
        public void TakeEffect();
    }
    
    public class Effect
    {
        
    }
}