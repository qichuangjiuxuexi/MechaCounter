using System.Reflection;
using UnityEngine;

public class AOTLaunch : MonoBehaviour
{
    protected void Start()
    {
        var assembly = Assembly.Load("HotfixAsm");
        var type = assembly.GetType("LaunchLoadingControl");
        var method = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        method.Invoke(null, new object[] { });
    }
}
