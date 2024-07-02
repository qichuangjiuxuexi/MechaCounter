namespace Project.Scripts
{
    /// <summary>
    /// 属性类型
    /// </summary>
    public enum EPropertyType
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
        Blood = 3,
        /// <summary>
        /// 能源
        /// </summary>
        Energy = 4,
        /// <summary>
        /// 聚能
        /// </summary>
        GetEnergy = 5,
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
}