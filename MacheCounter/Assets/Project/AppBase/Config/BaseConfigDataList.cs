using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 配置列表
/// </summary>
[Serializable]
public abstract class BaseConfigDataList<T> : ScriptableObject where T : BaseConfig
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> dataList;
}

/// <summary>
/// 配置列表（可通过主键检索）
/// </summary>
[Serializable]
public abstract class BaseConfigDataDictionary<K, T> : BaseConfigDataList<T> where T : BaseConfig
{
    [SerializeField]
    protected List<K> keys;
    
    [NonSerialized]
    private Dictionary<K, T> _map;

    /// <summary>
    /// 配置字典，根据配置列表产生，方便快速检索
    /// </summary>
    [JsonIgnore] [XmlIgnore]
    public Dictionary<K, T> DataMap
    {
        get
        {
            if (_map != null) return _map;
            if (dataList == null || keys == null || dataList.Count != keys.Count)
            {
                Debug.LogError($"Create DataMap Failed: Type: {GetType().Name}, ListCount: {dataList?.Count}, KeyCount: {keys?.Count}");
                return null;
            }
            _map = new Dictionary<K, T>();
            for (int i = 0; i < dataList.Count; i++)
            {
                var key = keys[i];
                var val = dataList[i];
                if (!_map.TryAdd(key, val))
                {
                    Debug.LogWarning($"Create DataMap Duplicated: Type: {val.GetType().Name}, Key: {key}, Index: {i}");
                }
            }
            return _map;
        }
    }
}

/// <summary>
/// 配置基类
/// </summary>
public interface BaseConfig
{
}
