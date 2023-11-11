using System;
using System.Collections.Generic;

/// <summary>
/// 浮点数值配置
/// </summary>
[Serializable]
public class FloatValueConfig : BaseConfig
{
    /// <summary>
    /// 索引
    /// </summary>
    public string Key;

    /// <summary>
    /// 值
    /// </summary>
    public float Value;
}
