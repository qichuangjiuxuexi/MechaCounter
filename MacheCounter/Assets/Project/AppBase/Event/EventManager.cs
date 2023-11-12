using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using WordGame.Utils;

namespace AppBase.Event
{
    public interface IEventSystem : ISystem
    {
        /// <summary>
        /// 注册同步事件
        /// </summary>
        /// <param name="callback">事件通知回调</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件监听器</returns>
        IEventListener Subscribe<T>(Action<T> callback, int priority = 0) where T : IEvent;

        /// <summary>
        /// 注册异步事件，等callback回调被执行才调用下一个监听者
        /// </summary>
        /// <param name="callback">异步回调，Action参数回调被执行才调用下一个监听者</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件监听器</returns>
        IEventListener Subscribe<T>(Action<T, Action> callback, int priority = 0) where T : IEvent;

        /// <summary>
        /// 解注册事件
        /// <param name="listener">消息监听器</param>
        /// </summary>
        void Unsubscribe(IEventListener listener);
        
        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// <returns>被解注册的消息监听器</returns>
        /// </summary>
        IEventListener Unsubscribe<T>(Action<T> callback) where T : IEvent;
        
        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// <returns>被解注册的消息监听器</returns>
        /// </summary>
        IEventListener Unsubscribe<T>(Action<T, Action> callback) where T : IEvent;

        /// <summary>
        /// 分发事件
        /// <param name="eventData">事件消息</param>
        /// <param name="callback">全部分发完成后的回调</param>
        /// </summary>
        void Broadcast<T>(T eventData = default, Action callback = null) where T : IEvent;
    }
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : AbstractSystem, IEventSystem
    {
        private Dictionary<Type, List<IEventListener>> m_observerMap = new();
        
        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 注册同步事件
        /// </summary>
        /// <param name="callback">事件通知回调</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件监听器</returns>
        public IEventListener Subscribe<T>(Action<T> callback, int priority = 0) where T : IEvent
        {
            Type type = typeof(T);
            var listener = new EventListener<T>(callback, priority);
            if (m_observerMap.TryGetValue(type, out var list))
            {
                var oldListener = list.Find(x => ((EventListener<T>)x).callback == callback);
                if (oldListener == null)
                {
                    var index = list.FindIndex(x => x.priority <= priority);
                    list.Insert(Math.Max(index, 0), listener);
                }
                else
                {
                    Debugger.LogWarning(TAG, "EventManager.Subscribe callback already exist: " + callback.Method.Name);
                    return null;
                }
            }
            else
            {
                list = new List<IEventListener> { listener };
                m_observerMap.Add(type, list);
            }
            return listener;
        }

        /// <summary>
        /// 注册异步事件，等callback回调被执行才调用下一个监听者
        /// </summary>
        /// <param name="callback">异步回调，Action参数回调被执行才调用下一个监听者</param>
        /// <param name="priority">优先级，越小越优先</param>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>事件监听器</returns>
        public IEventListener Subscribe<T>(Action<T, Action> callback, int priority = 0) where T : IEvent
        {
            Type type = typeof(T);
            var listener = new EventListener<T>(callback, priority);
            if (m_observerMap.TryGetValue(type, out var list))
            {
                var oldListener = list.Find(x => ((EventListener<T>)x).trainedCallback == callback);
                if (oldListener == null)
                {
                    var index = list.FindIndex(x => x.priority <= priority);
                    list.Insert(Math.Max(index, 0), listener);
                }
                else
                {
                    Debugger.LogWarning(TAG, "EventManager.Subscribe callback already exist: " + callback.Method.Name);
                    return null;
                }
            }
            else
            {
                list = new List<IEventListener> { listener };
                m_observerMap.Add(type, list);
            }
            return listener;
        }

        /// <summary>
        /// 解注册事件
        /// <param name="listener">消息监听器</param>
        /// </summary>
        public void Unsubscribe(IEventListener listener)
        {
            if (listener == null) return;
            if (m_observerMap.TryGetValue(listener.eventType, out var list))
            {
                list.Remove(listener);
                if (list.Count == 0)
                {
                    m_observerMap.Remove(listener.eventType);
                }
            }
        }

        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// <returns>被解注册的消息监听器</returns>
        /// </summary>
        public IEventListener Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);
            IEventListener listener = null;
            if (m_observerMap.TryGetValue(type, out var list))
            {
                listener = list.Find(x => ((EventListener<T>)x).callback == callback);
                if (listener != null)
                {
                    list.Remove(listener);
                    if (list.Count == 0)
                    {
                        m_observerMap.Remove(type);
                    }
                }
            }
            return listener;
        }

        /// <summary>
        /// 解注册事件
        /// <param name="callback">消息监听回调</param>
        /// <returns>被解注册的消息监听器</returns>
        /// </summary>
        public IEventListener Unsubscribe<T>(Action<T, Action> callback) where T : IEvent
        {
            var type = typeof(T);
            IEventListener listener = null;
            if (m_observerMap.TryGetValue(type, out var list))
            {
                listener = list.Find(x => ((EventListener<T>)x).trainedCallback == callback);
                if (listener != null)
                {
                    list.Remove(listener);
                    if (list.Count == 0)
                    {
                        m_observerMap.Remove(type);
                    }
                }
            }
            return listener;
        }

        /// <summary>
        /// 分发事件
        /// <param name="eventData">事件消息</param>
        /// <param name="callback">全部分发完成后的回调</param>
        /// </summary>
        public void Broadcast<T>(T eventData = default, Action callback = null) where T : IEvent
        {
            var type = typeof(T);
            if (m_observerMap.TryGetValue(type, out var list))
            {
                InvokeListener(list, eventData, callback, list.Count - 1);
            }
        }

        /// <summary>
        /// 调用分发事件
        /// </summary>
        private void InvokeListener<T>(List<IEventListener> list, T eventData, Action callback, int index) where T : IEvent
        {
            if (list == null || index < 0 || index >= list.Count)
            {
                callback?.Invoke();
                return;
            }
            var listener = (EventListener<T>)list[index];
            if (listener.trainedCallback != null)
            {
                listener.trainedCallback.Invoke(eventData, () =>
                {
                    InvokeListener(list, eventData, callback, index - 1);
                });
            }
            else
            {
                listener.callback?.Invoke(eventData);
                InvokeListener(list, eventData, callback, index - 1);
            }
        }
    }
}