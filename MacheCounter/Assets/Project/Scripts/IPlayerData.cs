using System.Collections.Generic;
using WordGame.Utils;

namespace Project.Scripts
{
    /// <summary>
    /// 数据参数
    /// </summary>
    public interface IPlayerData : IBaseProperty
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

    public interface IBaseProperty
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
        public long Blood { get; }
        
        /// <summary>
        /// 能量
        /// </summary>
        public long Energy { get; }
        
        /// <summary>
        /// 聚能
        /// </summary>
        public long GetEnergy { get; }
    }

    public interface IPropertyItem
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public EPropertyType Type { get; }
        
        /// <summary>
        /// 属性参数
        /// </summary>
        public long Num { get; }
    }

    public class PropertyItem : IPropertyItem
    {
        public EPropertyType Type { get; }
        public long Num { get; }

        public PropertyItem(int type, long num)
        {
            Type = (EPropertyType)type;
            Num = num;
        }
    }

    /// <summary>
    /// 装备属性
    /// </summary>
    public interface IEquipmentProperty
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
        public List<IPropertyItem> PropertyItems { get;}

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
    public class AEquipmentProperty : IEquipmentProperty
    {
        public EEquipmentType Type { get; private set; }
        
        public int Level { get; private set; }
        
        public List<IPropertyItem> PropertyItems => propertyItems;

        private readonly List<IPropertyItem> propertyItems = new();

        public void SetPropertyItems(int type, int level, params IPropertyItem[] items)
        {
            Type = (EEquipmentType)type;
            Level = level;
            if (items is not { Length: > 0 }) 
                items.ForEach(p => propertyItems.Add(new PropertyItem((int)p.Type, p.Num)));
        }
    }
}