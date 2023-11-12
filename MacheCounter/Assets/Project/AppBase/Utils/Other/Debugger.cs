/**********************************************

Copyright(c) 2020
All right reserved

Author : Terrence Rao 
Date : 2020-07-18 19:30:13
Ver : 1.0.0
Description : 模拟Me2zen.Util中的 Debugger, 来保持代码中的一致
ChangeLog :
**********************************************/

using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace WordGame.Utils
{
    /// <summary>
    /// Log输出时的tag
    /// </summary>
    public enum LogTag
    {
        /// <summary>
        /// 默认,
        /// 1. 用于项目业务
        /// </summary>
        Default = 0,

        /// <summary>
        /// App
        /// </summary>
        App,

        /// <summary>
        /// Facebook
        /// 登录, 分享, 日志
        /// </summary>
        Facebook,

        /// <summary>
        /// Adjust日志
        /// </summary>
        Adjust,

        /// <summary>
        /// 内购
        /// </summary>
        Iap,

        /// <summary>
        /// 广告
        /// </summary>
        Ads,

        /// <summary>
        /// Bi逻辑
        /// </summary>
        BILog,

        /// <summary>
        /// 词典
        /// </summary>
        Dictionary,

        /// <summary>
        /// 本地通知
        /// </summary>
        Notification,

        /// <summary>
        /// helpshift
        /// </summary>
        Helpshift,

        /// <summary>
        /// 临时标签, 方便过滤, 用完删除
        /// </summary>
        Temp,
        Thinking,
        /// <summary>
        /// 所有Log
        /// 
        /// </summary>
        All,
    }

    /// <summary>
    /// 模拟Me2zen.Util中的 Debugger, 来保持代码中的一致
    /// </summary>
    public class Debugger
    {
        private static bool logMessageReceived = true;
        private static string unityLogClassName = "com.unity3d.nativelog.NativeLog";
        private static AndroidJavaClass unityLogClass = (AndroidJavaClass) null;
        private static List<string> logEnableTags = new List<string>();
        private static string internalStoragePath;
        public const string logTagDefault = "default";
        private static bool logAlreadyOn;


        //private const string logAllEnableTag = "all";
        private const string CONFIG_SAVE_FILE = "debug_config.txt";

        static Debugger()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            //读取存档
            Debugger.ReadDebugConfig();
        }

        public static void SetLogEnable(string logTag, bool isEnable)
        {
            if (isEnable && !Debugger.logEnableTags.Contains(logTag))
                Debugger.logEnableTags.Add(logTag);
            else if (!isEnable && Debugger.logEnableTags.Contains(logTag))
                Debugger.logEnableTags.Remove(logTag);
            Debugger.WriteDebugConfig();
        }

        public static void SetLogEnable(LogTag logTag, bool isEnable)
        {
            Debugger.SetLogEnable(logTag.ToString().ToLower(), isEnable);
        }

        public static bool GetLogEnable(string logTag)
        {
            return Debugger.logEnableTags.Contains("all") || Debugger.logEnableTags.Contains(logTag);
        }

        public static void SetLogEnable(string[] logTags, bool isEnable)
        {
            if (isEnable)
            {
                foreach (string logTag in logTags)
                {
                    if (!Debugger.logEnableTags.Contains(logTag))
                        Debugger.logEnableTags.Add(logTag);
                }
            }
            else
            {
                foreach (string logTag in logTags)
                {
                    if (Debugger.logEnableTags.Contains(logTag))
                        Debugger.logEnableTags.Remove(logTag);
                }
            }

            Debugger.WriteDebugConfig();
        }

        public static void SetLogMessageReceived(bool isReceived)
        {
            Debugger.logMessageReceived = isReceived;
            Debugger.ShowDebugLog(Debugger.logEnableTags.Count > 0 && Debugger.logMessageReceived);
        }

        public static void LogD(object message)
        {
            Debugger.Log(logTagDefault, message);
        }

        public static void LogD(object message, Object context)
        {
            Debugger.Log(logTagDefault, message, context);
        }

        public static void LogDFormat(string format, params object[] args)
        {
            Debugger.LogFormat(logTagDefault, format, args);
        }

        public static void LogDFormat(Object context, string format, params object[] args)
        {
            Debugger.LogFormat(logTagDefault, context, format, args);
        }

        public static void LogDError(object message)
        {
            Debugger.LogError(logTagDefault, message);
        }

        public static void LogDError(object message, Object context)
        {
            Debugger.LogError(logTagDefault, message, context);
        }

        public static void LogDErrorFormat(string format, params object[] args)
        {
            Debugger.LogErrorFormat(logTagDefault, format, args);
        }

        public static void LogDErrorFormat(Object context, string format, params object[] args)
        {
            Debugger.LogErrorFormat(logTagDefault, context, format, args);
        }

        public static void LogDWarning(object message)
        {
            Debugger.LogWarning(logTagDefault, message);
        }

        public static void LogDWarning(object message, Object context)
        {
            Debugger.LogWarning(logTagDefault, message, context);
        }

        public static void LogDWarningFormat(string format, params object[] args)
        {
            Debugger.LogWarningFormat(logTagDefault, format, args);
        }

        public static void LogDWarningFormat(Object context, string format, params object[] args)
        {
            Debugger.LogWarningFormat(logTagDefault, context, format, args);
        }

        public static void Log(string logTag, object message)
        {
            Debugger.Log(logTag, message, (Object) null);
        }

        public static void Log(LogTag logTag, object message)
        {
            Debugger.Log(logTag.ToString().ToLower(), message, (Object) null);
        }

        public static void Log(string logTag, object message, Object context)
        {
            if (!GetLogEnable(logTag))
                return;

            //Android 有最大输出限制, 需要特殊处理
#if UNITY_ANDROID && !UNITY_EDITOR
            string strContent = message.ToString();
            const int MAX_LENGTH = 1000;
            int strLen = strContent.Length;
            if (strLen <= MAX_LENGTH)
            {
                Debug.Log((object) string.Format("[{0}]:{1}", (object) logTag, message), context);
            }
            else
            {
                int index = 0;
                while (index < strContent.Length)
                {
                    string strFinal;
                    if (strLen <= index + MAX_LENGTH)
                    {
                        strFinal = strContent.Substring(index);
                    }
                    else
                    {
                        strFinal = strContent.Substring(index, MAX_LENGTH);
                    }

                    index += MAX_LENGTH;
                    Debug.Log((object) string.Format("[{0}]: {1}", (object) logTag, strFinal), context);
                }
            }
#else
            Debug.Log((object) string.Format("[{0}]: {1}", (object) logTag, message), context);
#endif
        }

        public static void LogFormat(string logTag, string format, params object[] args)
        {
            Debugger.Log(logTag, (object) string.Format(format, args));
        }

        public static void LogFormat(LogTag logTag, string format, params object[] args)
        {
            Debugger.Log(logTag.ToString().ToLower(), (object) string.Format(format, args));
        }

        public static void LogFormat(
            string logTag,
            Object context,
            string format,
            params object[] args)
        {
            Debugger.Log(logTag, (object) string.Format(format, args), context);
        }

        public static void LogError(string logTag, object message)
        {
            Debugger.LogError(logTag, message, (Object) null);
        }

        public static void LogError(LogTag logTag, object message)
        {
            Debugger.LogError(logTag.ToString().ToLower(), message, (Object) null);
        }

        public static void LogError(string logTag, object message, Object context)
        {
            Debug.LogError((object) string.Format("[{0}]: {1}", (object) logTag, message), context);
        }

        public static void LogErrorFormat(string logTag, string format, params object[] args)
        {
            Debugger.LogError(logTag, (object) string.Format(format, args));
        }

        public static void LogErrorFormat(LogTag logTag, string format, params object[] args)
        {
            Debugger.LogError(logTag.ToString().ToLower(), (object) string.Format(format, args));
        }

        public static void LogErrorFormat(
            string logTag,
            Object context,
            string format,
            params object[] args)
        {
            Debugger.LogError(logTag, (object) string.Format(format, args), context);
        }

        public static void LogWarning(string logTag, object message)
        {
            Debugger.LogWarning(logTag, message, (Object) null);
        }

        public static void LogWarning(LogTag logTag, object message)
        {
            Debugger.LogWarning(logTag.ToString().ToLower(), message, null);
        }

        public static void LogWarning(string logTag, object message, Object context)
        {
            Debug.LogWarning((object) string.Format("[{0}]: {1}", (object) logTag, message), context);
        }


        public static void LogWarningFormat(string logTag, string format, params object[] args)
        {
            Debugger.LogWarning(logTag, (object) string.Format(format, args));
        }

        public static void LogWarningFormat(LogTag logTag, string format, params object[] args)
        {
            Debugger.LogWarning(logTag.ToString().ToLower(), (object) string.Format(format, args));
        }

        public static void LogWarningFormat(
            string logTag,
            Object context,
            string format,
            params object[] args)
        {
            Debugger.LogWarning(logTag, (object) string.Format(format, args), context);
        }

        private static void ReadDebugConfig()
        {
            string path = Path.Combine(Debugger.GetInternalStoragePath(), CONFIG_SAVE_FILE);
            if (File.Exists(path))
            {
                string strContent = File.ReadAllText(path);
                logEnableTags = ToolString.SplitStringToStringList(strContent, ",");
                Debugger.EnableLogForiOS();
            }
        }

        private static void WriteDebugConfig()
        {
            File.WriteAllText(Path.Combine(Debugger.GetInternalStoragePath(), CONFIG_SAVE_FILE),
                string.Join(",", Debugger.logEnableTags.ToArray()));
            Debugger.EnableLogForiOS();
        }

        private static void EnableLogForiOS()
        {
            //Debugger.ShowDebugLog(Debugger.logEnableTags.Count > 0 && Debugger.logMessageReceived);
        }

        private static string GetInternalStoragePath()
        {
            if (!string.IsNullOrEmpty(Debugger.internalStoragePath))
                return Debugger.internalStoragePath;
            if (Application.platform == RuntimePlatform.Android)
                Debugger.internalStoragePath = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity")
                    .Call<AndroidJavaObject>("getApplicationContext", new object[0])
                    .Call<AndroidJavaObject>("getFilesDir", new object[0])
                    .Call<string>("getAbsolutePath", new object[0]);
            if (string.IsNullOrEmpty(Debugger.internalStoragePath))
                Debugger.internalStoragePath = Application.persistentDataPath;
            return Debugger.internalStoragePath;
        }

        private static void LogClassCallStatic(string methodName, params object[] args)
        {
            if (Debugger.unityLogClass == null && (Application.platform != RuntimePlatform.OSXEditor &&
                                                   Application.platform != RuntimePlatform.WindowsEditor))
                Debugger.unityLogClass = new AndroidJavaClass(Debugger.unityLogClassName);
            if (Debugger.unityLogClass == null)
                return;
            Debugger.unityLogClass.CallStatic(methodName, args);
        }

        public static void ShowDebugLog(bool isOpen)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
                return;
            Application.LogCallback logCallback = (Application.LogCallback) ((condition, stackTrace, logType) =>
                string.Format("[unity]{0}", (object) condition));
            if (isOpen && !Debugger.logAlreadyOn)
            {
                Application.logMessageReceived += logCallback;
                Debugger.logAlreadyOn = true;
            }
            else if (!isOpen && Debugger.logAlreadyOn)
            {
                Application.logMessageReceived -= logCallback;
                Debugger.logAlreadyOn = false;
            }
        }
    }
}