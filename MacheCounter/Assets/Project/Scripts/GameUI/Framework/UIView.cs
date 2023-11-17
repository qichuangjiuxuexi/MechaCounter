using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    protected UIConfig mUiConfig;


    public int GetViewId()
    {
        if (mUiConfig != null)
        {
            return mUiConfig.viewId;
        }

        return 0;
    }
}
