using System;

namespace AppBase.Event
{
    /// <summary>
    /// 事件监听器
    /// </summary>
    public class EventListener<T> : IEventListener where T : IEvent
    {
        public Action<T> callback;
        public Action<T, Action> trainedCallback;

        public EventListener(Action<T> inCallback, int inPriority)
        {
            callback = inCallback;
            priority = inPriority;
            eventType = typeof(T);
        }

        public EventListener(Action<T, Action> inCallback, int inPriority)
        {
            trainedCallback = inCallback;
            priority = inPriority;
            eventType = typeof(T);
        }
    }
}