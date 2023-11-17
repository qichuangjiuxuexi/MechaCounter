using System.Collections.Generic;
using UnityEngine;

public class UILayer : MonoBehaviour
{
    public int layerType;

    private List<UIView> mViewList = new List<UIView>();

    public void AddView(UIView uiView)
    {
        if (uiView != null)
        {
            if (!mViewList.Contains(uiView))
            {
                mViewList.Add(uiView);
            }
        }
    }

    public UIView GetView(int viewId)
    {
        if (mViewList.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < mViewList.Count; i++)
        {
            UIView view = mViewList[i];
            if (view != null && view.GetViewId() == viewId)
            {
                return view;
            }
        }

        return null;
    }
    
    public void RemoveView(int viewId)
    {
        UIView uiView = GetView(viewId);
        if (uiView != null)
        {
            RemoveView(uiView);
        }
    }

    public void RemoveView(UIView view)
    {
        if (mViewList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < mViewList.Count; i++)
        {
            UIView uiView = mViewList[i];
            if (uiView != null && uiView == view)
            {
                Destroy(view.gameObject);
                mViewList.RemoveAt(i);
                break;
            }
        }
    }
    
    public bool HasView(int viewId)
    {
        for (int i = 0; i < mViewList.Count; i++)
        {
            if (mViewList[i] != null && mViewList[i].GetViewId() == viewId)
            {
                return true;
            }
        }

        return false;
    }
}

