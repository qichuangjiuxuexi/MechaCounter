using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    protected UIConfig mUiConfig;
    protected RectTransform mViewRectTransform;
    
    public virtual void Awake()
    {
        Initialize();
    }


    //#region 数据相关
    protected void Initialize()
    {
        mViewRectTransform = gameObject.GetComponent<RectTransform>();
    }
    
    
    //view 放在哪个层级
    public int LayerType
    {
        get
        {
            if (mUiConfig != null)
            {
                return mUiConfig.layerType;
            }

            return UIConfigManager.None;
        }
    }

    public void SetUIConfig(UIConfig uiConfig)
    {
        mUiConfig = uiConfig;
    }
    
    
    public int GetViewId()
    {
        if (mUiConfig != null)
        {
            return mUiConfig.viewId;
        }

        return 0;
    }
    //#endregion
    
    
    //#region ui相关
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);

        if (active)
        {
            OnShow();
        }
        else
        {
            OnHide();
        }
    }
    
    public virtual void OnShow()
    {

    }

    public virtual void OnHide()
    {

    }
    //#endregion
}
