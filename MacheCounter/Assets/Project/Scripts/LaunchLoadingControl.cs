using UnityEngine;

public class LaunchLoadingControl : MonoBehaviour
{
    public static void Create()
    {
        Debug.Log("加载开始界面！！");
        /*var go = new GameObject("LaunchLoadingControl");
        DontDestroyOnLoad(go);*/
        //go.AddComponent<LaunchLoadingControl>();
    }
}
