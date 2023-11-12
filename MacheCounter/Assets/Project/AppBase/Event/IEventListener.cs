using System;

namespace AppBase.Event
{
    /// <summary>
    /// 事件监听器基类
    /// </summary>
    public abstract class IEventListener
    {
        public int priority; //越小越先执行
        public Type eventType;
    }
}
