using System;
using System.Collections.Generic;
using UnityEngine;
using WordGame.Utils;
using Object = UnityEngine.Object;

namespace AppBase.Resource
{
    /// <summary>
    /// 资源引用模块，当模块被析构时，会归还资源引用
    /// </summary>
    public interface IResourceReference
    {
        public List<ResourceHandler> Handlers { get; }
    }

    /// <summary>
    /// 资源引用模块，普通类中使用，需要手动调用Dispose来归还资源引用
    /// </summary>
    public class ResourceReference : IResourceReference, IDisposable
    {
        public bool IsDisposed { get; protected set; }
        public List<ResourceHandler> Handlers { get; } = new();
        public void Dispose()
        {
            this.ReleaseAllHandlers();
            IsDisposed = true;
        }
    }

    /// <summary>
    /// 资源引用模块，子模块中使用，当模块被析构时自动归还资源引用
    /// </summary>
    public class ResourceReferenceModule : ModuleBase, IResourceReference
    {
        public List<ResourceHandler> Handlers { get; } = new();
        protected override void OnDestroy() => this.ReleaseAllHandlers();
    }

    /// <summary>
    /// 资源引用模块，MonoBehaviour中使用，当Destroy时自动归还资源引用
    /// </summary>
    public class ResourceReferenceBehaviour : MonoBehaviour, IResourceReference
    {
        public List<ResourceHandler> Handlers { get; } = new();
        protected void OnDestroy() => this.ReleaseAllHandlers();
    }
    
    public static class ResourceReferenceExtension
    {
        /// <summary>
        /// 增加资源引用
        /// </summary>
        public static void AddHandler(this IResourceReference reference, ResourceHandler handler)
        {
            if (handler != null && !reference.Handlers.Contains(handler))
            {
                handler.Retain();
                reference.Handlers.Add(handler);
            }
        }
        
        /// <summary>
        /// 移除资源引用
        /// </summary>
        public static bool ReleaseHandler(this IResourceReference reference, ResourceHandler handler)
        {
            if (handler != null && reference.Handlers.Remove(handler))
            {
                handler.Release();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 根据具体的资源，移除对应的资源引用
        /// </summary>
        public static ResourceHandler ReleaseAsset(this IResourceReference reference, Object asset)
        {
            if (asset == null) return null;
            var handler = reference.Handlers.Find(h => h.GetAsset() == asset);
            if (handler == null) return null;
            handler.Release();
            reference.Handlers.Remove(handler);
            return handler;
        }

        /// <summary>
        /// 移除所有资源引用
        /// </summary>
        public static void ReleaseAllHandlers(this IResourceReference reference)
        {
            foreach (var handler in reference.Handlers)
            {
                handler.Release();
            }
            reference.Handlers.Clear();
        }

        /// <summary>
        /// 获取或创建一个资源引用模块
        /// </summary>
        public static IResourceReference GetResourceReference(this GameObject gameObject)
        {
            return gameObject?.GetOrAddComponent<ResourceReferenceBehaviour>();
        }

        /// <summary>
        /// 获取或创建一个资源引用模块
        /// </summary>
        public static IResourceReference GetResourceReference(this Component component)
        {
            return component?.GetOrAddComponent<ResourceReferenceBehaviour>();
        }

        /// <summary>
        /// 获取或创建一个资源引用模块
        /// </summary>
        public static IResourceReference GetResourceReference(this ModuleBase module)
        {
            if (module is ResourceReferenceModule refModule) return refModule;
            return module?.AddModule<ResourceReferenceModule>();
        }
        
        /// <summary>
        /// 判定资源引用是否仍有效
        /// </summary>
        public static bool IsValid(this IResourceReference reference)
        {
            switch (reference)
            {
                case ResourceReferenceBehaviour behaviour:
                    return behaviour.gameObject != null;
                case ResourceReferenceModule module:
                    return module.IsModuleInited;
                case ResourceReference refer:
                    return !refer.IsDisposed;
                default:
                    return false;
            }
        }
    }
}
