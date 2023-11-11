/*
using System;
using System.Collections.Generic;
using UnityEngine;
using WordGame.Utils;
using AppBase.Module;
using AppBase.Resource;

namespace Project.DictData
{
    /// <summary>
    /// 配置文件获取 控制器
    /// </summary>
    public class ConfigManager : ModuleBase
    {
        /// <summary>
        /// Scriptable文件数据
        /// </summary>
        private Dictionary<string, ScriptableObject> configAssets = new Dictionary<string, ScriptableObject>();

        public void LoadConfig(string keyName, Action finishCallback)
        {
            var address = AAConst.GetAddress(keyName);
            Game.Resource.LoadAsset<ScriptableObject>(address, this.GetResourceReference(), asset =>
            {
                if (asset != null)
                {
                    configAssets[address] = asset;
                }
                finishCallback?.Invoke();
            });
        }

        /// <summary>
        /// 获取配置文件信息（数组）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>配置数组</returns>
        public List<T> GetConfigList<T>(string configPath) where T : BaseConfig
        {
            if (configAssets.TryGetValue(configPath, out var sc))
            {
                BaseConfigDataList<T> config = sc as BaseConfigDataList<T>;
                if (config == null)
                {
                    Debugger.LogDError($"configPath data is NULL:{config}");
                    return null;
                }
                return config.dataList;
            }
            return new List<T>();
        }
        
        /// <summary>
        /// 获取配置文件信息（字典）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="K">Key类型</typeparam>
        /// <param name="configPath">配置文件路径</param>
        /// <returns>配置字典</returns>
        public Dictionary<K, T> GetConfigMap<K, T>(string configPath) where T : BaseConfig
        {
            if (configAssets.TryGetValue(configPath, out var sc))
            {
                var config = sc as BaseConfigDataDictionary<K, T>;
                if (config == null)
                {
                    Debugger.LogDError($"configPath data is NULL:{configPath}");
                    return null;
                }
                return config.DataMap;
            }
            return null;
        }

        /// <summary>
        /// 根据字典Key查询配置条目
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="key">字典Key</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="K">Key类型</typeparam>
        /// <returns>配置条目</returns>
        public T GetConfigByKey<K, T>(string configPath, K key) where T : BaseConfig
        {
            var map = GetConfigMap<K, T>(configPath);
            if (map == null || !map.TryGetValue(key, out T result)) return default;
            return result;
        }

        /// <summary>
        /// 根据List的Index查询配置条目
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="index">List的Index，从0开始</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>配置条目</returns>
        public T GetConfigByIndex<T>(string configPath, int index) where T : BaseConfig
        {
            var list = GetConfigList<T>(configPath);
            if (list == null || index < 0 || index >= list.Count) return default;
            return list[index];
        }
    }
}
*/
