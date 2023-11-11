using System;
using System.Collections.Generic;

/// <summary>
/// 整数数组配置
/// </summary>
[Serializable]
public class IntListValueConfig : BaseConfig
{
    /// <summary>
    /// 索引
    /// </summary>
    public string Key;

    /// <summary>
    /// 值
    /// </summary>
    public List<int> Value;
}
