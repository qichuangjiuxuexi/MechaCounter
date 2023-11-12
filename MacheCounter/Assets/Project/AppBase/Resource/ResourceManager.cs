using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

namespace AppBase.Resource
{    
    public interface IResourceSystem : ISystem
    {
        /// <summary>
        /// 当加载器引用计数为0时，自动释放资源
        /// </summary>
        public bool OnHandlerDestroy(ResourceHandler handler);
    }

    public abstract class AbstractResource : AbstractSystem, IResourceSystem
    {
        public bool OnHandlerDestroy(ResourceHandler handler)
        {
            return false;
        }
    }
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager : AbstractResource
    {
        /// <summary>
        /// 资源缓存池
        /// </summary>
        protected Dictionary<string, ResourceHandler> assetsPool = new();

        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="reference">标记生命周期的引用，当引用被释放时，资源也一起被释放</param>
        /// <param name="successCallback">加载成功时的回调</param>
        /// <param name="failureCallback">加载失败时的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载器</returns>
        public ResourceHandler LoadAsset<T>(string address, IResourceReference reference, Action<T> successCallback, Action failureCallback = null) where T : Object
        {
            if (!assetsPool.TryGetValue(address, out var handler))
            {
                handler = new ResourceHandler(address);
                assetsPool[address] = handler;
            }
            handler.LoadAsset<T>(h =>
            {
                if (h.IsSuccess && reference.IsValid())
                {
                    var obj = h.GetAsset<T>();
                    if (obj != null)
                    {
                        reference?.AddHandler(h);
                        successCallback?.Invoke(obj);
                        return;
                    }
                }
                h.CheckRetainCount();
                failureCallback?.Invoke();
            });
            return handler;
        }
        
        /// <summary>
        /// 加载资源handler，注意需要手动调用Release来管理生命周期，否则会造成内存泄露
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="successCallback">加载成功时的回调</param>
        /// <param name="failureCallback">加载失败时的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载器</returns>
        public ResourceHandler LoadAssetHandler<T>(string address, Action<ResourceHandler> successCallback, Action failureCallback = null) where T : Object
        {
            if (!assetsPool.TryGetValue(address, out var handler))
            {
                handler = new ResourceHandler(address);
                assetsPool[address] = handler;
            }
            handler.LoadAsset<T>(h =>
            {
                if (h.IsSuccess)
                {
                    h.Retain();
                    successCallback?.Invoke(h);
                }
                else
                {
                    h.CheckRetainCount();
                    failureCallback?.Invoke();
                }
            });
            return handler;
        }
        
        /// <summary>
        /// 实例化游戏对象，资源生命周期跟随实例化的游戏对象
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="instantParams">实例化参数</param>
        /// <param name="successCallback">加载成功时的回调</param>
        /// <param name="failureCallback">加载失败时的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载器</returns>
        public ResourceHandler InstantGameObject(string address, InstantiationParameters instantParams, Action<GameObject> successCallback, Action failureCallback = null)
        {
            var handler = new ResourceHandler(address);
            handler.LoadInstantiation(instantParams, h =>
            {
                if (h.IsSuccess)
                {
                    var obj = h.GetAsset<GameObject>();
                    if (obj != null)
                    {
                        obj.name = obj.name.Replace("(Clone)", "");
                        obj.GetResourceReference().AddHandler(h);
                        successCallback?.Invoke(obj);
                        return;
                    }
                }
                h.CheckRetainCount();
                failureCallback?.Invoke();
            });
            return handler;
        }

        /// <summary>
        /// 实例化游戏对象，资源生命周期跟随实例化的游戏对象
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="parent">实例化父节点</param>
        /// <param name="successCallback">加载成功时的回调</param>
        /// <param name="failureCallback">加载失败时的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载器</returns>
        public ResourceHandler InstantGameObject(string address, Transform parent, Action<GameObject> successCallback, Action failureCallback = null)
        {
            return InstantGameObject(address, new InstantiationParameters(parent , false), successCallback, failureCallback);
        }

        /// <summary>
        /// 当加载器引用计数为0时，自动释放资源
        /// </summary>
        public bool OnHandlerDestroy(ResourceHandler handler)
        {
            if (handler != null && handler.RetainCount <= 0 && assetsPool.TryGetValue(handler.Address, out var oldHandler) && oldHandler == handler)
            {
                return assetsPool.Remove(handler.Address);
            }
            return false;
        }

        /// <summary>
        /// 检查地址是否存在
        /// </summary>
        /// <param name="address">地址</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>地址是否存在</returns>
        public bool IsAddressExist<T>(string address) where T : Object
        {
            return Addressables.ResourceLocators.Any(r => r.Locate(address, typeof(T), out _));
        }
    }
}