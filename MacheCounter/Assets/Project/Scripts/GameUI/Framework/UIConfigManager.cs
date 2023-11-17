using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ViewId
{
    None,
    LogoUI = 10,//Logo
    LoginUI = 20,//登录 
    LobbyUI = 20,//大厅 
    FightUI = 80,//战斗ui    
    SettingUI = 90,//设置
    Max,
}

public class UIConfigManager : MonoBehaviour
{
    public const int None = -1;
    public const int System = 200;
    public const int Menu = 300;
    public const int Dialog = 400;
    public const int Loading = 900;
    public const int Max = 1000;
    
    private const string Login_Path = "GameUi/LoginUi/prefab/LoginUi";

    private static Dictionary<int, UIConfig> uiDictionary = new Dictionary<int, UIConfig>()
    {
        { (int)ViewId.LoginUI,new UIConfig((int)ViewId.LoginUI,Login_Path,System)},
    };
    
    public static UIConfig GetConfig(int viewId)
    {
        if (uiDictionary.ContainsKey(viewId))
        {
            return uiDictionary[viewId];
        }

        return null;
    }

    public static void AddConfig(UIConfig uiConfig)
    {
        if (uiConfig != null && !uiDictionary.ContainsKey(uiConfig.viewId))
        {
            uiDictionary.Add(uiConfig.viewId, uiConfig);
        }
    }

    public static void RemoveConfig(int viewId)
    {
        if (uiDictionary.ContainsKey(viewId))
        {
            uiDictionary.Remove(viewId);
        }
    }
}
