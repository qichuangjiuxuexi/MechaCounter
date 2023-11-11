using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

public class UpdateConfigUtil
{
    private const string configScriptPath = "Assets/Project/AddressableRes/m_Configs_dl/Scripts/";
    private const string configNamesPath = configScriptPath + "ConfigNames.cs";
    private const string configAssetPath = "Assets/Project/AddressableRes/m_Configs_dl/Data/";
    private static string[] assemblyList2SearchType = new [] {"HotfixAsm"};

    /// <summary>
    /// 一维数组分隔符
    /// </summary>
    private static readonly char[] ArraySeparatorChars = {';', ','};
    
    /// <summary>
    /// 二维数组分隔符
    /// </summary>
    private static readonly char[] Array2dSeparatorChars = {'|'};
    
    private static readonly Dictionary<string, TypeParser> typeParsers = new()
    {
        {"int", new TypeParser("int", typeof(int), s => ParseInt(s))},
        {"long", new TypeParser("long", typeof(long), s => ParseLong(s))},
        {"float", new TypeParser("float", typeof(float), s => ParseFloat(s))},
        {"bool", new TypeParser("bool", typeof(bool), s => ParseBool(s))},
        {"string", new TypeParser("string", typeof(string), s => ParseString(s))},
        
        {"int[]", new TypeParser("List<int>", typeof(List<int>), s => ParseArray(s, ParseInt))},
        {"long[]", new TypeParser("List<long>", typeof(List<long>), s => ParseArray(s, ParseLong))},
        {"float[]", new TypeParser("List<float>", typeof(List<float>), s => ParseArray(s, ParseFloat))},
        {"bool[]", new TypeParser("List<bool>", typeof(List<bool>), s => ParseArray(s, ParseBool))},
        {"string[]", new TypeParser("List<string>", typeof(List<string>), s => ParseArray(s, ParseString))},
        
        {"int[][]", new TypeParser("List<List<int>>", typeof(List<List<int>>), s => ParseArray2d(s, ParseInt))},
        {"long[][]", new TypeParser("List<List<long>>", typeof(List<List<long>>), s => ParseArray2d(s, ParseLong))},
        {"float[][]", new TypeParser("List<List<float>>", typeof(List<List<float>>), s => ParseArray2d(s, ParseFloat))},
        {"bool[][]", new TypeParser("List<List<bool>>", typeof(List<List<bool>>), s => ParseArray2d(s, ParseBool))},
        {"string[][]", new TypeParser("List<List<string>>", typeof(List<List<string>>), s => ParseArray2d(s, ParseString))},
        
        {"int{}", new TypeParser("int", typeof(int), s => ParseInt(s))},
        {"string{}", new TypeParser("string", typeof(string), s => ParseString(s))},
    };
    
    public static string excelSourcePath
    {
        get
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Environment.CurrentDirectory)!);
            var path = dir.EnumerateDirectories("*Config", SearchOption.TopDirectoryOnly).First().FullName;
            return path;
        }
    }
    
    private const string templateConfig = @"using System;
using System.Collections.Generic;

/// <summary>
/// {1}
/// </summary>
[Serializable]
public class {0} : BaseConfig
{{{2}}}
";
    
    private const string templateConfigList = @"using System;

[Serializable]
public class {0}List : {1}
{{
}}
";
    
    private const string templateConfigProperty = @"
    /// <summary>
    /// {0}
    /// </summary>
    public {1} {2};
";

    private const string templateConfigKeys = @"
/// <summary>
/// {0}
/// </summary>
public static class {1}Keys
{{{2}}}
";
    
    private const string templateConfigKeysProperty = @"
    /// <summary>
    /// {0}
    /// </summary>
    public const string {1} = ""{2}"";
";
    
    private const string templateConfigNames = @"using System.Collections.Generic;
public static class ConfigNames
{{
    public static List<string> configs = new List<string>()
    {{
        ""{0}""
    }};
}}
";

    public const int skipEmptyCell = 10; //当连续出现10行或10列空白时，扫描结束，避免无限扫描
    private const string waitingForUpdateConfigData = "isWaitingForUpdateConfigData";
    
    [MenuItem("Tools/Config/Update Configs")]
    public static void UpdateConfigs()
    {
        EditorPrefs.SetBool(waitingForUpdateConfigData, true);
        UpdateConfigScripts();
        if (!EditorApplication.isCompiling)
            WaitForCompile();
    }
    
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void WaitForCompile()
    {
        if (!EditorPrefs.GetBool(waitingForUpdateConfigData)) return;
        EditorPrefs.DeleteKey(waitingForUpdateConfigData);
        UpdateConfigData();
    }
    
    [MenuItem("Tools/Config/Steps/1. Update Config Scripts")]
    public static void UpdateConfigScripts()
    {
        if (Application.isBatchMode)
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        }
        Debug.Log("UpdateConfigScripts");
        var excelDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, excelSourcePath));
        if (!excelDir.Exists) throw new Exception($"Excel source path not found: {excelDir.FullName}");
        var excelFiles = excelDir.GetFiles("*.xlsx", SearchOption.AllDirectories);
        var list = new List<string>();
        foreach (var excelFile in excelFiles)
        {
            if (IsNameInvalid(excelFile.Name) || excelFile.Attributes.HasFlag(FileAttributes.Hidden)) continue;
            using var excel = new ExcelPackage(excelFile);
            var count = excel.Workbook.Worksheets.Count;
            for (int i = 1; i <= count; i++)
            {
                var sheet = excel.Workbook.Worksheets[i];
                Debug.Log($"{excelFile.Name} {sheet.Name}");
                if (IsNameInvalid(sheet.Name) || sheet.Hidden != eWorkSheetHidden.Visible) continue;
                UpdateConfigScript(sheet);
                var title = sheet.Cells[1, 2].Text?.Trim();
                if (!string.IsNullOrEmpty(title)) list.Add(title);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("更新配置脚本完成", string.Join(", ", list), "OK");
    }

    [MenuItem("Tools/Config/Steps/2. Update Config Data")]
    public static void UpdateConfigData()
    {
        if (Application.isBatchMode)
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        }
        Debug.Log("UpdateConfigData");
        var excelDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, excelSourcePath));
        if (!excelDir.Exists) throw new Exception($"Excel source path not found: {excelDir.FullName}");
        var excelFiles = excelDir.GetFiles("*.xlsx", SearchOption.AllDirectories);
        var list = new List<string>();
        foreach (var excelFile in excelFiles)
        {
            if (IsNameInvalid(excelFile.Name) || excelFile.Attributes.HasFlag(FileAttributes.Hidden)) continue;
            using var excel = new ExcelPackage(excelFile);
            var count = excel.Workbook.Worksheets.Count;
            for (int i = 1; i <= count; i++)
            {
                var sheet = excel.Workbook.Worksheets[i];
                Debug.Log($"{excelFile.Name} {sheet.Name}");
                if (IsNameInvalid(sheet.Name) || sheet.Hidden != eWorkSheetHidden.Visible) continue;
                UpdateConfigData(sheet);
                var title = sheet.Cells[1, 2].Text?.Trim();
                if (!string.IsNullOrEmpty(title)) list.Add(title);
            }
        }
        UpdateConfigNames(list);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("更新配置数据完成", string.Join(", ", list), "OK");
    }

    /// <summary>
    /// 更新配置脚本
    /// </summary>
    private static void UpdateConfigScript(ExcelWorksheet sheet)
    {
        var configName = sheet.Cells[1, 2].Text?.Trim();
        if (string.IsNullOrEmpty(configName))
        {
            Debug.LogWarning($"Ignore Sheet: {sheet.Name}");
            return;
        }
        
        var className = sheet.Cells[1, 4].Text?.Trim();
        if (string.IsNullOrEmpty(className))
        {
            className = configName;
        }

        var sheetComment = sheet.Cells[1, 3].Text;
        if (string.IsNullOrEmpty(sheetComment))
        {
            sheetComment = className;
        }

        var properties = new StringBuilder();
        var emptyColumn = 0;
        for (int i = 2; i <= sheet.Dimension.End.Column; i++)
        {
            var header = sheet.Cells[4, i].Text;
            var comment = sheet.Cells[2, i].Text;
            if (string.IsNullOrEmpty(comment)) comment = header;
            var type = sheet.Cells[3, i].Text.Trim();
            if (string.IsNullOrEmpty(type) || type.ToLower() == "null")
            {
                if (++emptyColumn >= skipEmptyCell) break;
                continue;
            }
            emptyColumn = 0;
            properties.AppendFormat(templateConfigProperty, ParseCommend(comment), ParseTypeName(type), header);
        }
        
        var code = string.Format(templateConfig, className, ParseCommend(sheetComment), properties);
        var assetPath = configScriptPath + className + ".cs";
        var path = Path.Combine(Environment.CurrentDirectory, assetPath);
        File.WriteAllText(path, code, Encoding.UTF8);

        var keyType = sheet.Cells[3, 2].Text.Trim();
        var listType = keyType.EndsWith("{}") ? $"BaseConfigDataDictionary<{ParseTypeName(keyType)}, {className}>" : $"BaseConfigDataList<{className}>";
        code = string.Format(templateConfigList, className, listType);
        assetPath = configScriptPath + className + "List.cs";
        path = Path.Combine(Environment.CurrentDirectory, assetPath);
        File.WriteAllText(path, code, Encoding.UTF8);
    }
    
    private static void UpdateConfigNames(List<string> list)
    {
        list.Sort();
        var code = string.Format(templateConfigNames, string.Join("\",\n        \"", list));
        var path = Path.Combine(Environment.CurrentDirectory, configNamesPath);
        File.WriteAllText(path, code, Encoding.UTF8);
    }
    
    /// <summary>
    /// 更新配置数据
    /// </summary>
    private static void UpdateConfigData(ExcelWorksheet sheet)
    {
        var configName = sheet.Cells[1, 2].Text?.Trim();
        if (string.IsNullOrEmpty(configName))
        {
            Debug.LogWarning($"Ignore Sheet: {sheet.Name}");
            return;
        }
        
        var className = sheet.Cells[1, 4].Text?.Trim();
        if (string.IsNullOrEmpty(className))
        {
            className = configName;
        }

        // Parse type
        GetDataTypeFromAsms(configName,className, out var configType, out var assetType);
        var keyTypeName = sheet.Cells[3, 2].Text.Trim().ToLower();
        var isKeyMapType = keyTypeName.EndsWith("{}");
        var listField = isKeyMapType ? assetType.BaseType?.BaseType?.GetField("dataList") : assetType.BaseType?.GetField("dataList");
        if (listField == null) throw new Exception(configName);
        var listType = typeof(List<>).MakeGenericType(configType);
        var listObj = Activator.CreateInstance(listType) as IList;
        if (listObj == null) throw new Exception(configName);

        // Parse Map type
        var keysType = isKeyMapType ? typeof(List<>).MakeGenericType(ParseType(keyTypeName)) : null;
        var keysField = isKeyMapType ? assetType.BaseType?.GetField("keys", BindingFlags.Instance | BindingFlags.NonPublic) : null;
        if (isKeyMapType && keysField == null) throw new Exception(configName);
        var keysObj = isKeyMapType ? Activator.CreateInstance(keysType) as IList : null;
        if (isKeyMapType && keysObj == null) throw new Exception(configName);
        var keysComments = isKeyMapType ? new List<string>() : null;

        var emptyRow = 0;
        // each row
        for (int r = 5; r <= sheet.Dimension.End.Row; r++)
        {
            // skip row with empty key 
            var rowObj = Activator.CreateInstance(configType);
            if (rowObj == null) throw new Exception(configName);
            if (string.IsNullOrWhiteSpace(sheet.Cells[r, 2].Text))
            {
                if (++emptyRow >= skipEmptyCell) break;
                continue;
            }
            emptyRow = 0;
            var emptyColumn = 0;
            for (int c = 2; c <= sheet.Dimension.End.Column; c++)
            {
                var typeName = sheet.Cells[3, c].Text?.Trim();
                if (string.IsNullOrEmpty(typeName) || typeName.ToLower() == "null")
                {
                    if (++emptyColumn >= skipEmptyCell) break;
                    continue;
                }
                emptyColumn = 0;
                var columnName = sheet.Cells[4, c].Text;
                var columnData = sheet.Cells[r, c].Text ?? "";
                ParseField(rowObj, columnName, typeName, columnData);
                if (isKeyMapType && c == 2)
                {
                    keysObj.Add(ParseValue(keyTypeName, columnData));
                    keysComments.Add(ParseCommend(sheet.Cells[r, 1].Text));
                }
            }
            listObj.Add(rowObj);
        }

        // Save resources
        var path = configAssetPath + configName + ".asset";
        bool isNew = false;
        var assetObj = AssetDatabase.LoadAssetAtPath(path, assetType);
        if (assetObj == null)
        {
            assetObj = ScriptableObject.CreateInstance(assetType);
            isNew = true;
        }
        if (assetObj == null) throw new Exception(configName);
        listField.SetValue(assetObj, listObj);
        if (isKeyMapType) keysField.SetValue(assetObj, keysObj);
        EditorUtility.SetDirty(assetObj);
        if (isNew)
        {
            AssetDatabase.CreateAsset(assetObj, path);
        }
        
        // save keys enum
        if (isKeyMapType && keyTypeName == "string{}")
        {
            var sheetComment = sheet.Cells[1, 3].Text;
            if (string.IsNullOrEmpty(sheetComment)) sheetComment = className;
            var keyProperties = keysObj.Cast<string>()
                .Select((s, i) => string.Format(templateConfigKeysProperty, keysComments[i] ?? ParseCommend(s), ParseKey(s), s));
            var keysCode = string.Format(templateConfigKeys, ParseCommend(sheetComment), configName, string.Join("", keyProperties));
            path = configScriptPath + configName + "Keys.cs";
            File.WriteAllText(path, keysCode, Encoding.UTF8);
        }
    }

    private static void GetDataTypeFromAsms(string configName, string className, out Type configType,
        out Type assetType)
    {
        configType = null;
        assetType = null;
        for (int i = 0; i < assemblyList2SearchType.Length; i++)
        {
            var assembly = Assembly.Load(assemblyList2SearchType[i]);
            configType = assembly.GetType(className);
            assetType = assembly.GetType(className + "List");
            if (assetType != null && configType != null)
            {
                break;
            }
        }
        if (assetType == null || configType == null)
            throw new Exception($"{configName}表里的类型：{className}在所有配置的程序集中找不到！");
    }

    public static string ParseTypeName(string typeName)
    {
        typeName = typeName.Trim().ToLower();
        if (!typeParsers.TryGetValue(typeName, out var parser)) throw new Exception(typeName);
        return parser.name;
    }
    
    private static Type ParseType(string typeName)
    {
        typeName = typeName.Trim().ToLower();
        if (!typeParsers.TryGetValue(typeName, out var parser)) throw new Exception(typeName);
        return parser.type;
    }

    private static object ParseValue(string typeName, string str)
    {
        typeName = typeName.Trim().ToLower();
        if (!typeParsers.TryGetValue(typeName, out var parser)) throw new Exception(typeName);
        return parser.Parse(str);
    }

    private static void ParseField(object obj, string fieldName, string typeName, string str)
    {
        var fieldInfo = obj.GetType().GetField(fieldName);
        if (fieldInfo == null) throw new Exception(fieldName);
        fieldInfo.SetValue(obj, ParseValue(typeName, str));
    }

    private static string ParseCommend(string commend)
    {
        return commend.Replace("\\n", "\n").Replace("\n", "\n    /// ");
    }

    private static int ParseInt(string str)
    {
        return !int.TryParse(str.Trim(), out int result) ? 0 : result;
    }

    private static long ParseLong(string str)
    {
        return !long.TryParse(str.Trim(), out long result) ? 0 : result;
    }
    
    private static float ParseFloat(string str)
    {
        return !float.TryParse(str.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? 0 : result;
    }
    
    private static bool ParseBool(string str)
    {
        return str.Trim().ToLower() is "1" or "true";
    }

    private static string ParseString(string str)
    {
        return str.Trim().Replace("\\n", "\n");
    }

    private static List<T> ParseArray<T>(string str, Func<string, T> selector)
    {
        if (string.IsNullOrWhiteSpace(str)) return new List<T>();
        return str.Trim().Split(ArraySeparatorChars).Select(selector).ToList();
    }

    private static List<List<T>> ParseArray2d<T>(string str, Func<string, T> selector)
    {
        if (string.IsNullOrWhiteSpace(str)) return new List<List<T>>();
        return str.Trim().Split(Array2dSeparatorChars).Select(x => ParseArray(x, selector)).ToList();
    }

    private static string ParseKey(string str)
    {
        str = str.Trim().Replace(" ", "_").Replace(".", "_");
        if (str.Length > 0 && char.IsDigit(str[0])) str = '_' + str;
        return str;
    }

    public static bool IsNameInvalid(string name)
    {
        return string.IsNullOrEmpty(name) || name.StartsWith('~') || name.StartsWith('$') || name.StartsWith('.');
    }
    
    private class TypeParser
    {
        public string name;
        public Type type;
        public Func<string, object> Parse;
        public TypeParser(string name, Type type, Func<string, object> parser)
        {
            this.name = name;
            this.type = type;
            this.Parse = parser;
        }
    }
}