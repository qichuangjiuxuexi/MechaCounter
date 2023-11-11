using System;
using System.Collections.Generic;

/// <summary>
/// 整数数值配置
/// </summary>
[Serializable]
public class IntValueConfig : BaseConfig
{
    /// <summary>
    /// 索引
    /// </summary>
    public string Key;

    /// <summary>
    /// 值
    /// </summary>
    public int Value;
}
