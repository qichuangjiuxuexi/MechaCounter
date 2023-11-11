/*using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using WordGame.Utils;
using Object = UnityEngine.Object;

namespace AppBase.Resource
{
    /// <summary>
    /// 资源加载器，可以用来缓存资源
    /// </summary>
    public class ResourceHandler : Retainable, IEnumerator
    {
        protected const string TAG = "ResourceHandler";
        protected event Action<ResourceHandler> callback;

        /// <summary>
        /// 资源地址
        /// </summary>
        protected string address;
        public string Address => address;
        protected AsyncOperationHandle handler;
        public AsyncOperationHandle Handler => handler;
        
        /// <summary>
        /// 是否加载成功
        /// </summary>
        public bool IsSuccess => handler.IsValid() && handler.Status == AsyncOperationStatus.Succeeded;
        protected bool IsLoading;

        public ResourceHandler(string address)
        {
            this.address = address;
        }
        
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="callback">加载完成回调，无论成功失败都会回调，需要调用IsSuccess自行判定是否加载成功</param>
        public void LoadAsset<T>(Action<ResourceHandler> callback) where T : Object
        {
            if (IsSuccess)
            {
                callback?.Invoke(this);
                return;
            }
            this.callback += callback;
            if (!IsLoading)
            {
                IsLoading = true;
                handler = Addressables.LoadAssetAsync<T>(address);
                handler.Completed += OnLoadCompleted;
            }
        }

        /// <summary>
        /// 加载资源并实例化
        /// </summary>
        /// <param name="instantParams">实例化参数</param>
        /// <param name="callback">加载完成回调，无论成功失败都会回调，需要调用IsSuccess自行判定是否加载成功</param>
        public void LoadInstantiation(InstantiationParameters instantParams, Action<ResourceHandler> callback)
        {
            if (IsSuccess)
            {
                callback?.Invoke(this);
                return;
            }
            this.callback += callback;
            if (!IsLoading)
            {
                IsLoading = true;
                handler = Addressables.InstantiateAsync(address, instantParams);
                handler.Completed += OnLoadCompleted;
            }
        }

        protected void OnLoadCompleted(AsyncOperationHandle inHandler)
        {
            IsLoading = false;
            inHandler.Completed -= OnLoadCompleted;
            if (inHandler.Status != AsyncOperationStatus.Succeeded)
            {
                Debugger.LogError(TAG, $"OnLoadAsset Failed: {address}");
            }
            callback?.Invoke(this);
            callback = null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void OnDestroy()
        {
            Debugger.Log(TAG, $"OnDestroy: {address}");
            IsLoading = false;
            callback = null;
            if (handler.IsValid())
            {
                handler.Completed -= OnLoadCompleted;
                Addressables.Release(handler);
            }
            Game.Resource.OnHandlerDestroy(this);
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        public Object GetAsset()
        {
            if (!IsSuccess) return null;
            return (Object)handler.Result;
        }

        /// <summary>
        /// 获取资源T
        /// </summary>
        public T GetAsset<T>() where T : Object
        {
            if (!IsSuccess) return null;
            return handler.Result as T;
        }

        /// <summary>
        /// 实例化资源
        /// </summary>
        public GameObject GetInstantiation(InstantiationParameters instantParams)
        {
            if (!IsSuccess) return null;
            var newGo = handler.Result as GameObject;
            if (newGo == null) return null;
            return instantParams.SetPositionRotation ?
                GameObject.Instantiate(newGo, instantParams.Position, instantParams.Rotation, instantParams.Parent) :
                GameObject.Instantiate(newGo, instantParams.Parent, instantParams.InstantiateInWorldPosition);
        }

        /// <summary>
        /// 使用UniTask等待加载完成
        /// </summary>
        public async UniTask<ResourceHandler> Await()
        {
            if (IsLoading && handler.IsValid())
            {
                await handler;
            }
            return this;
        }

        #region IEnumerator相关
        public bool MoveNext() => !handler.IsDone;
        public object Current => handler.Result;
        public void Reset() {}
        #endregion
    }
}*/