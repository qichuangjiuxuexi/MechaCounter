using System.Collections.Generic;

namespace Project.Scripts
{
    /// <summary>
    /// 招式类型
    /// </summary>
    public enum ETrackType
    {
        /// <summary>
        /// 出手
        /// </summary>
        Active = 1,

        /// <summary>
        /// 反击
        /// </summary>
        Back = 1 << 1,

        /// <summary>
        /// 提升
        /// </summary>
        Enhance = 1 << 2,

        /// <summary>
        /// 伤害技能招式
        /// </summary>
        DamageSkillTrack = Active | Back | Enhance,

        /// <summary>
        /// 核心转换
        /// </summary>
        ChangeEnergy = 1 << 3,

        /// <summary>
        /// 气势
        /// </summary>
        Momentum = 1 << 4,

        /// <summary>
        /// 附体
        /// </summary>
        Possessed = 1 << 5,
        
        /// <summary>
        /// 运功
        /// </summary>
        Work = 1 << 6,

        /// <summary>
        /// 核心技能招式
        /// </summary>
        EnergyCoreTrack = ChangeEnergy | Momentum | Possessed | Work
    }

    /// <summary>
    /// 招式
    /// </summary>
    public interface ITrack
    {
        /// <summary>
        /// 招式类型
        /// </summary>
        public ETrackType TrackType { get; }
        
        /// <summary>
        /// 触发概率
        /// </summary>
        public float Probability { get; }

        /// <summary>
        /// 获取产生效果
        /// </summary>
        public List<IEffect> TakeTrack();
    }
    public class Track
    {
        
    }
}