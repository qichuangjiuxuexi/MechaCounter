using System;
using System.Collections.Generic;
using UnityEngine;

public interface IModule
{
    /// <summary>
    /// 初始化模块
    /// </summary>
    void Init();

    /// <summary>
    /// 所有子模块都初始化完毕后，调用此方法
    /// </summary>
    void AfterInit();

    void Dispose();

    /// <summary>
    /// 添加模块，如果模块类型已存在，则返回已存在的模块
    /// </summary>
    /// <param name="moduleData">模块初始化数据</param>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    T AddModule<T>(object moduleData = null) where T : ModuleBase, new();

    /// <summary>
    /// 添加模块，如果模块类型已存在，则会重复添加
    /// </summary>
    /// <param name="module">模块引用</param>
    /// <param name="moduleData">模块初始化数据</param>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    T AddModule<T>(T module, object moduleData = null) where T : ModuleBase;

    /// <summary>
    /// 获取模块
    /// </summary>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块类型</returns>
    T GetModule<T>() where T : ModuleBase;

    /// <summary>
    /// 移除模块
    /// </summary>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    T RemoveModule<T>() where T : ModuleBase;

    /// <summary>
    /// 移除模块
    /// </summary>
    /// <param name="module">模块引用</param>
    /// <returns>是否存在</returns>
    bool RemoveModule(ModuleBase module);

    /// <summary>
    /// 移除所有模块
    /// </summary>
    void RemoveAllModules();
}

/// <summary>
/// 模块基类
/// </summary>
public abstract class ModuleBase : IModule, IDisposable
{
    protected string TAG => GetType().Name;
    private List<ModuleBase> moduleList;
    private Dictionary<Type, ModuleBase> moduleDict;
    private bool isModuleInited;
    public bool IsModuleInited => isModuleInited;

    /// <summary>
    /// 模块数据
    /// </summary>
    protected object moduleData;

    /// <summary>
    /// 父组件
    /// </summary>
    public ModuleBase ParentModule => parentModule;

    private ModuleBase parentModule;

    /// <summary>
    /// 初始化模块时调用，用于在登录成功后，在里面加载数据、注册事件等，在其中添加子模块会立即Init
    /// </summary>
    protected virtual void OnInit()
    {
    }

    /// <summary>
    /// 析构模块时调用，用于当切换账号时，会先调用此方法，在里面清除数据、注销事件等
    /// </summary>
    protected virtual void OnDestroy()
    {
    }

    /// <summary>
    /// 初始化模块前调用，在其中添加子模块不会立即Init
    /// </summary>
    protected virtual void OnBeforeInit()
    {
    }

    /// <summary>
    /// 初始化模块后调用，此时所有子模块都Init完毕
    /// </summary>
    protected virtual void OnAfterInit()
    {
    }

    /// <summary>
    /// 初始化模块
    /// </summary>
    public void Init()
    {
        if (isModuleInited) return;
        OnBeforeInit();
        isModuleInited = true;
        OnInit();
        moduleList?.ForEach(m => m.Init());
        if (ParentModule == null)
        {
            AfterInit();
        }
    }

    /// <summary>
    /// 所有子模块都初始化完毕后，调用此方法
    /// </summary>
    public void AfterInit()
    {
        if (!isModuleInited) return;
        OnAfterInit();
        moduleList?.ForEach(m => m.AfterInit());
    }

    public void Dispose()
    {
        RemoveAllModules();
        if (isModuleInited)
        {
            OnDestroy();
        }

        parentModule = null;
        moduleData = null;
        isModuleInited = false;
    }

    /// <summary>
    /// 添加模块，如果模块类型已存在，则返回已存在的模块
    /// </summary>
    /// <param name="moduleData">模块初始化数据</param>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    public T AddModule<T>(object moduleData = null) where T : ModuleBase, new()
    {
        var type = typeof(T);
        if (moduleDict != null && moduleDict.TryGetValue(type, out var m))
        {
            return (T) m;
        }

        var module = new T();
        moduleList ??= new List<ModuleBase>();
        moduleDict ??= new Dictionary<Type, ModuleBase>();
        moduleList.Add(module);
        moduleDict.Add(type, module);
        module.parentModule = this;
        module.moduleData = moduleData;
        //子模块在父模块初始化后再初始化
        if (isModuleInited)
        {
            module.Init();
        }

        return module;
    }

    /// <summary>
    /// 添加模块，如果模块类型已存在，则会重复添加
    /// </summary>
    /// <param name="module">模块引用</param>
    /// <param name="moduleData">模块初始化数据</param>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    public T AddModule<T>(T module, object moduleData = null) where T : ModuleBase
    {
        if (module == null) return null;
        moduleList ??= new List<ModuleBase>();
        moduleDict ??= new Dictionary<Type, ModuleBase>();
        moduleList.Add(module);
        moduleDict.TryAdd(module.GetType(), module);
        module.parentModule = this;
        module.moduleData = moduleData;
        //子模块在父模块初始化后再初始化
        if (isModuleInited)
        {
            module.Init();
        }

        return module;
    }

    /// <summary>
    /// 获取模块
    /// </summary>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块类型</returns>
    public T GetModule<T>() where T : ModuleBase
    {
        if (moduleDict != null && moduleDict.TryGetValue(typeof(T), out var m))
        {
            return (T) m;
        }

        return null;
    }

    /// <summary>
    /// 移除模块
    /// </summary>
    /// <typeparam name="T">模块类型</typeparam>
    /// <returns>模块引用</returns>
    public T RemoveModule<T>() where T : ModuleBase
    {
        var type = typeof(T);
        if (moduleDict != null && moduleDict.TryGetValue(type, out var m))
        {
            moduleDict.Remove(type);
            moduleList.Remove(m);
            m.Dispose();
            return (T) m;
        }

        return null;
    }

    /// <summary>
    /// 移除模块
    /// </summary>
    /// <param name="module">模块引用</param>
    /// <returns>是否存在</returns>
    public bool RemoveModule(ModuleBase module)
    {
        if (module == null || moduleList == null) return false;
        if (moduleList.Remove(module))
        {
            var type = module.GetType();
            if (moduleDict.TryGetValue(type, out var m) && m == module)
            {
                moduleDict.Remove(type);
            }

            module.Dispose();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 移除所有模块
    /// </summary>
    public void RemoveAllModules()
    {
        if (moduleList != null)
        {
            moduleList.ForEach(m => m.Dispose());
            moduleList.Clear();
            moduleList = null;
        }

        if (moduleDict != null)
        {
            moduleDict.Clear();
            moduleDict = null;
        }
    }
}

public abstract class MonoModule : ModuleBase
{
    private string gameObjectPath;
    private GameObject gameObject;

    /// <summary>
    /// 路径
    /// </summary>
    public virtual string GameObjectPath
    {
        get { return gameObjectPath ?? $"_MonoModulesRoot/{GetType().Name}"; }
        set { gameObjectPath = value; }
    }

    /// <summary>
    /// GameObject
    /// </summary>
    public GameObject GameObject
    {
        get
        {
            if (gameObject != null) return gameObject;
            gameObject = CreateGameObject(GameObjectPath);
            return gameObject;
        }
    }

    /// <summary>
    /// Transform
    /// </summary>
    public Transform Transform
    {
        get { return GameObject?.transform; }
    }

    private GameObject CreateGameObject(string path)
    {
        return CreateGameObject(null, path.Split('/'), 0);
    }

    private GameObject CreateGameObject(Transform parent, string[] path, int index)
    {
        var go = parent == null ? GameObject.Find(path[index]) : parent.Find(path[index])?.gameObject;
        if (go == null)
        {
            go = new GameObject(path[index]);
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    GameObject.DontDestroyOnLoad(go);
                }
            }
        }

        if (index < path.Length - 1)
        {
            return CreateGameObject(go.transform, path, index + 1);
        }

        return go;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (gameObject != null && gameObject.transform.root?.gameObject.scene.name == "DontDestroyOnLoad")
        {
            GameObject.Destroy(gameObject);
            gameObject = null;
        }
    }
}