using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace AppBase.Event
{
    /// <summary>
    /// 事件注册代理，用于在MonoBehaviour中注册事件，自动在OnDestroy时解注册
    /// </summary>
    public class EventBehaviour : MonoBehaviour, IEventObserver
    {
        protected List<IEventListener> listeners = new();

        /// <summary>
        /// 注册同步事件
        /// <param name="callback">事件通知回调</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// </summary>
        public IEventObserver Subscribe<T>(Action<T> callback, int priority = 0) where T : IEvent
        {
            var listener = MacheCounter.Interface.GetSystem<IEventSystem>().Subscribe<T>(callback, priority);
            if (listener != null)
            {
                listeners.Add(listener);
            }
            return this;
        }

        /// <summary>
        /// 注册异步事件，等callback回调被执行才调用下一个监听者
        /// <param name="callback">异步的事件通知回调，Action被执行才调用下一个监听者</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// </summary>
        public IEventObserver Subscribe<T>(Action<T, Action> callback, int priority = 0) where T : IEvent
        {
            var listener = MacheCounter.Interface.GetSystem<IEventSystem>().Subscribe<T>(callback, priority);
            if (listener != null)
            {
                listeners.Add(listener);
            }
            return this;
        }

        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// </summary>
        public bool Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var listener = MacheCounter.Interface.GetSystem<IEventSystem>().Unsubscribe(callback);
            if (listener != null)
            {
                listeners.Remove(listener);
            }
            return listener != null;
        }

        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// </summary>
        public bool Unsubscribe<T>(Action<T, Action> callback) where T : IEvent
        {
            var listener = MacheCounter.Interface.GetSystem<IEventSystem>().Unsubscribe(callback);
            if (listener != null)
            {
                listeners.Remove(listener);
            }
            return listener != null;
        }
    
        /// <summary>
        /// 移除所有监听器
        /// </summary>
        public void UnregisterEvents()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                MacheCounter.Interface.GetSystem<IEventSystem>().Unsubscribe(listeners[i]);
            }
            listeners.Clear();
        }
    
        protected void OnDestroy()
        {
            UnregisterEvents();
        }
    }
}
