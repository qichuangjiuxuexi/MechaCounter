using System;
using System.Collections.Generic;
using UnityEngine;

#region Architecture

public interface IArchitecture
{
    void RegisterSystem<T>(T system) where T : ISystem;

    void RegisterModel<T>(T model) where T : IModel;

    void RegisterUtility<T>(T utility);

    T GetSystem<T>() where T : class, ISystem;

    T GetModel<T>() where T : class, IModel;

    T GetUtility<T>() where T : class, IUtility;

    void SendCommand<T>() where T : ICommand, new();

    void SendCommand<T>(T command) where T : ICommand;

    TResult SendQuery<TResult>(IQuery<TResult> query);

    void SendEvent<T>() where T : new();

    void SendEvent<T>(T e);

    IUnRegister RegisterEvent<T>(Action<T> onEvent);

    void UnRegisterEvent<T>(Action<T> onEvent);
}

public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
{
    /// <summary>
    /// 是否初始话完成
    /// </summary>
    private bool mInited = false;

    public static Action<T> OnRegisterPatch = architecture => { };

    private List<ISystem> mSystems = new List<ISystem>();

    private List<IModel> mModels = new List<IModel>();

    private static T mArchitecture;

    public static IArchitecture Interface
    {
        get
        {
            if (mArchitecture == null)
                MakeSureArchitecture();
            return mArchitecture;
        }
    }

    static void MakeSureArchitecture()
    {
        if (mArchitecture == null)
        {
            mArchitecture = new T();
            mArchitecture.Init();

            OnRegisterPatch?.Invoke(mArchitecture);

            foreach (var architectureModel in mArchitecture.mModels)
            {
                architectureModel.Init();
            }

            mArchitecture.mModels.Clear();

            foreach (var architectureSystem in mArchitecture.mSystems)
            {
                architectureSystem.Init();
            }

            mArchitecture.mSystems.Clear();

            mArchitecture.mInited = true;
        }
    }

    private IOCContainer mContainer = new IOCContainer();

    protected abstract void Init();

    public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
    {
        system.SetArchitecture(this);
        mArchitecture.mContainer.Register<TSystem>(system);
        if (!mInited)
        {
            mSystems.Add(system);
        }
        else
        {
            system.Init();
        }
    }

    public void RegisterModel<TModel>(TModel model) where TModel : IModel
    {
        model.SetArchitecture(this);
        mArchitecture.mContainer.Register<TModel>(model);
        if (!mInited)
        {
            mModels.Add(model);
        }
        else
        {
            model.Init();
        }
    }

    public void RegisterUtility<TUtility>(TUtility utility)
    {
        mContainer.Register<TUtility>(utility);
    }

    public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
    {
        return mContainer.Get<TSystem>();
    }

    public TModel GetModel<TModel>() where TModel : class, IModel
    {
        return mContainer.Get<TModel>();
    }

    public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
    {
        return mContainer.Get<TUtility>();
    }

    public void SendCommand<TCommand>() where TCommand : ICommand, new()
    {
        var command = new TCommand();
        command.SetArchitecture(this);
        command.Execute();
    }

    public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
    {
        command.SetArchitecture(this);
        command.Execute();
    }

    public TResult SendQuery<TResult>(IQuery<TResult> query)
    {
        query.SetArchitecture(this);
        return query.Do();
    }

    private TypeEventSystem mTypeEventSystem = new TypeEventSystem();

    public void SendEvent<TEvent>() where TEvent : new()
    {
        mTypeEventSystem.Send<TEvent>();
    }

    public void SendEvent<TEvent>(TEvent e)
    {
        mTypeEventSystem.Send<TEvent>(e);
    }

    public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent)
    {
        return mTypeEventSystem.Register<TEvent>(onEvent);
    }

    public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent)
    {
        mTypeEventSystem.UnRegister<TEvent>(onEvent);
    }
}

#endregion

#region Controller

public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetModel, ICanGetSystem,
    ICanRegisterEvent, ICanSendQuery
{

}

#endregion

#region System

public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility, ICanSendEvent,
    ICanRegisterEvent, ICanGetSystem
{
    void Init();
}

public abstract class AbstractSystem : ISystem
{
    protected string TAG => GetType().Name;
    private IArchitecture mArchitecture;

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return mArchitecture;
    }

    void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
    {
        mArchitecture = architecture;
    }

    void ISystem.Init()
    {
        OnInit();
    }

    protected abstract void OnInit();
}

#endregion

#region Model

public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent, ICanSendQuery
{
    void Init();
}

public abstract class AbstractModel : IModel
{
    private IArchitecture mArchitecture;

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return mArchitecture;
    }

    void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
    {
        mArchitecture = architecture;
    }

    void IModel.Init()
    {
        OnInit();
    }

    protected abstract void OnInit();
}

#endregion

#region Utility

public interface IUtility
{

}

#endregion

#region Query

public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem,
    ICanSendQuery
{
    TResult Do();
}

public abstract class AbstractQuery<T> : IQuery<T>
{
    public T Do()
    {
        return OnDO();
    }

    protected abstract T OnDO();

    private IArchitecture mArchitecture;

    public IArchitecture GetArchitecture()
    {
        return mArchitecture;
    }

    public void SetArchitecture(IArchitecture architecture)
    {
        mArchitecture = architecture;
    }
}

#endregion

#region Command

public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem, ICanGetUtility,
    ICanSendEvent, ICanSendCommand, ICanSendQuery
{
    public void Execute();
}

public abstract class AbstractCommand : ICommand
{
    private IArchitecture mArchitecture;

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return mArchitecture;
    }

    void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
    {
        mArchitecture = architecture;
    }

    void ICommand.Execute()
    {
        OnExecute();
    }

    protected abstract void OnExecute();
}

#endregion

#region Rule

public interface IBelongToArchitecture
{
    IArchitecture GetArchitecture();
}

public interface ICanSetArchitecture
{
    void SetArchitecture(IArchitecture architecture);
}

public interface ICanGetModel : IBelongToArchitecture
{

}

public static class CanGetModelExtension
{
    public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
    {
        return self.GetArchitecture().GetModel<T>();
    }
}

public interface ICanGetSystem : IBelongToArchitecture
{

}

public static class CanGetSystemExtension
{
    public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem
    {
        return self.GetArchitecture().GetSystem<T>();
    }
}

public interface ICanGetUtility : IBelongToArchitecture
{

}

public static class CanGetUtilityExtension
{
    public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
    {
        return self.GetArchitecture().GetUtility<T>();
    }
}

public interface ICanRegisterEvent : IBelongToArchitecture
{

}

public static class CanRegisterEvent
{
    public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
    {
        return self.GetArchitecture().RegisterEvent(onEvent);
    }

    public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)
    {
        self.GetArchitecture().UnRegisterEvent(onEvent);
    }
}

public interface ICanSendCommand : IBelongToArchitecture
{

}

public static class CanSendCommandExtension
{
    public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new()
    {
        self.GetArchitecture().SendCommand<T>();
    }

    public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand
    {
        self.GetArchitecture().SendCommand<T>(command);
    }
}

public interface ICanSendEvent : IBelongToArchitecture
{

}

public static class CanSendEvent
{
    public static void SendEvent<T>(this ICanSendEvent self) where T : new()
    {
        self.GetArchitecture().SendEvent<T>();
    }

    public static void SendEvent<T>(this ICanSendEvent self, T onEvent)
    {
        self.GetArchitecture().SendEvent<T>(onEvent);
    }
}

public interface ICanSendQuery : IBelongToArchitecture
{

}

public static class CanSendQueryExtension
{
    public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query)
    {
        return self.GetArchitecture().SendQuery(query);
    }
}

#endregion

#region TypeEventSystem

public interface ITypeEventSystem
{
    void Send<T>() where T : new();
    void Send<T>(T e);
    IUnRegister Register<T>(Action<T> onEvent);
    void UnRegister<T>(Action<T> onEvent);
}

public interface IUnRegister
{
    void UnRegister();
}

public struct TypeEventSystemUnRegister<T> : IUnRegister
{
    public ITypeEventSystem TypeEventSystem;
    public Action<T> OnEvent;

    public void UnRegister()
    {
        TypeEventSystem.UnRegister<T>(OnEvent);

        TypeEventSystem = null;

        OnEvent = null;
    }
}

public class UnRegisterOnDestroyTrigger : MonoBehaviour
{
    private HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

    public void AddUnRegister(IUnRegister unRegister)
    {
        mUnRegisters.Add(unRegister);
    }

    private void OnDestroy()
    {
        foreach (var unRegister in mUnRegisters)
        {
            unRegister.UnRegister();
        }

        mUnRegisters.Clear();
    }
}

public static class UnRegisterExtension
{
    public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
    {
        var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

        if (!trigger)
        {
            trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
        }

        trigger.AddUnRegister(unRegister);
    }
}

public class TypeEventSystem : ITypeEventSystem
{
    public interface IRegistrations
    {

    }

    public class Registrations<T> : IRegistrations
    {
        public Action<T> OnEvent = e => { };
    }

    private Dictionary<Type, IRegistrations> mEventRegistration = new Dictionary<Type, IRegistrations>();

    public void Send<T>() where T : new()
    {
        var e = new T();
        Send<T>(e);
    }

    public void Send<T>(T e)
    {
        var type = typeof(T);
        IRegistrations registrations;

        if (mEventRegistration.TryGetValue(type, out registrations))
        {
            (registrations as Registrations<T>).OnEvent(e);
        }
    }

    public IUnRegister Register<T>(Action<T> onEvent)
    {
        var type = typeof(T);
        IRegistrations registrations;

        if (mEventRegistration.TryGetValue(type, out registrations))
        {

        }
        else
        {
            registrations = new Registrations<T>();
            mEventRegistration.Add(type, registrations);
        }

        (registrations as Registrations<T>).OnEvent += onEvent;

        return new TypeEventSystemUnRegister<T>()
        {
            OnEvent = onEvent,
            TypeEventSystem = this
        };
    }

    public void UnRegister<T>(Action<T> onEvent)
    {
        var type = typeof(T);
        IRegistrations registrations;

        if (mEventRegistration.TryGetValue(type, out registrations))
        {
            (registrations as Registrations<T>).OnEvent -= onEvent;
        }
    }
}

#endregion

#region IOC

public class IOCContainer
{
    private Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

    public void Register<T>(T instance)
    {
        var key = typeof(T);
        if (mInstances.ContainsKey(key))
        {
            mInstances[key] = instance;
        }
        else
        {
            mInstances.Add(key, instance);
        }
    }

    public T Get<T>() where T : class
    {
        var key = typeof(T);
        if (mInstances.TryGetValue(key, out var resInstance))
        {
            return resInstance as T;
        }

        return null;
    }
}

#endregion

#region BindableProperty

public class BindableProperty<T>
{
    public BindableProperty(T defaultValue = default)
    {
        mValue = defaultValue;
    }

    private T mValue = default(T);

    public T Value
    {
        get => mValue;
        set
        {
            if (value == null && mValue == null) return;
            if (value != null && value.Equals(mValue)) return;
            mValue = value;
            mOnValueChanged?.Invoke(mValue);
        }
    }

    private Action<T> mOnValueChanged = (v) => { };

    public IUnRegister Register(Action<T> onValueChanged)
    {
        mOnValueChanged += onValueChanged;
        return new BindablePropertyUnRegister<T>()
        {
            BindableProperty = this,
            OnValueChanged = mOnValueChanged
        };
    }

    public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
    {
        onValueChanged(mValue);
        return Register(onValueChanged);
    }

    public static implicit operator T(BindableProperty<T> property)
    {
        return property.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public void UnRegister(Action<T> onValueChange)
    {
        mOnValueChanged -= onValueChange;
    }
}

public class BindablePropertyUnRegister<T> : IUnRegister
{
    public BindableProperty<T> BindableProperty { get; set; }

    public Action<T> OnValueChanged { get; set; }

    public void UnRegister()
    {
        BindableProperty.UnRegister(OnValueChanged);

        BindableProperty = null;
        OnValueChanged = null;
    }
}

#endregion
