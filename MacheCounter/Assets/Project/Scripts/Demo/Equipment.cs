using System.Collections.Generic;
using WordGame.Utils;

namespace Project.Scripts
{
    /// <summary>
    /// 装备类型
    /// </summary>
    public enum EEquipmentType
    {
        /// <summary>
        /// 头甲
        /// </summary>
        Head = 1,
        /// <summary>
        /// 左臂
        /// </summary>
        LeftArm = 2,
        /// <summary>
        /// 左腿
        /// </summary>
        LeftLeg = 3,
        /// <summary>
        /// 武器
        /// </summary>
        Weapon = 4,
        /// <summary>
        /// 右臂
        /// </summary>
        RightArm = 5,
        /// <summary>
        /// 右腿
        /// </summary>
        RightLeg = 6,
        /// <summary>
        /// 机甲回合材料
        /// </summary>
        Mechanism = 7,
        /// <summary>
        /// 能源
        /// </summary>
        EnergyResource = 8,
    }
    
    /// <summary>
    /// 装备属性
    /// </summary>
    public interface IEquipmentAttribute
    {
        /// <summary>
        /// 类型: 装备位置
        /// </summary>
        public EEquipmentType Type { get;}
        
        /// <summary>
        /// 装备等级
        /// </summary>
        public int Level { get;}

        /// <summary>
        /// 属性
        /// </summary>
        public HashSet<IPropertyItem> PropertyItems { get;}

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="level">等级</param>
        /// <param name="item">属性</param>
        public void SetPropertyItems(int type, int level, params IPropertyItem[] item);
    }

    /// <summary>
    /// 单个装备
    /// </summary>
    public class Equipment : IEquipmentAttribute
    {
        public EEquipmentType Type { get; private set; }
        
        public int Level { get; private set; }
        
        public HashSet<IPropertyItem> PropertyItems => propertyItems;

        private readonly HashSet<IPropertyItem> propertyItems = new();

        public void SetPropertyItems(int type, int level, params IPropertyItem[] items)
        {
            Type = (EEquipmentType)type;
            Level = level;
            if (items is not { Length: > 0 }) 
                items.ForEach(p => propertyItems.Add(new PropertyItem((int)p.Type, p.Num)));
        }
    }
}