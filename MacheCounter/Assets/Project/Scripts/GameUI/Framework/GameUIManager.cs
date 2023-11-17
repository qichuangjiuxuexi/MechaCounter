using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public void ShowView(int viewId)
    {
        UIConfig config = UIConfigManager.GetConfig(viewId);
        //TODO: 1、获取到perfab 2、创建
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
