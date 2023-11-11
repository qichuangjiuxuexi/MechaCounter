using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class AddressableUtil
{
    public const string AddressableRootGroupPath = "Assets/Project/AddressableRes";
    public const string KeyAddressScriptPath = "Assets/Project/AddressableRes/Common/Scripts/Definition/AAConst.cs";

    public const string SinglePackPrefix = "s_";
    public const string MultiPackPrefix = "m_";
    public const string DynamicLoadSuffix = "_dl";
    public const string IgnoreBuildTag = "__i";

    public const string TemplateAAConst = @"using System.Collections.Generic;
public class AAConst
{{
{0}
}}
";

    public static readonly HashSet<string> IgnoreExtentions = new ()
    {
        ".meta",
        ".cs",
        ".dll",
        ".DS_Store",
        ".keep",
    };

    [MenuItem("Tools/Addressable/Build Addressable Contents")]
    public static void BuildAddressableContents()
    {
        if (Application.isBatchMode)
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        }
        CheckAddressableSettings();
        RemoveAllEntities();
        ReGroupAllEntities();
        SpriteAtlasUtil.RemoveAllAtlasTextures();
        RemoveAllEmptyGroups();
        if (!Application.isBatchMode)
            EditorUtility.DisplayDialog("更新Addressable配置完成", "", "OK");
    }

    private static void CheckAddressableSettings()
    {
        var path = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        var settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(path);
        if (settings == null)
        {
            throw new Exception("Addressable settings not found: " + path);
        }
        AddressableAssetSettingsDefaultObject.Settings = settings;
    }

    /// <summary>
    /// 删除所有Groups，DefaultLocalGroup除外
    /// </summary>
    public static void RemoveAllGroups()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var groups = new List<AddressableAssetGroup>(settings.groups);
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            if (group?.name == "Built In Data") return;
            if (group?.Default == true)
            {
                RemoveEntitiesInGroup(group, settings);
            }
            else
            {
                try
                {
                    settings.RemoveGroup(group);
                }
                catch
                {
                    settings.groups.Remove(group);
                }
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, null, true, true);
            }
        }
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 清空Groups内的所有Entities，但不要删掉Groups
    /// </summary>
    public static void RemoveAllEntities()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.groups.ToList().ForEach(group => RemoveEntitiesInGroup(group, settings));
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 删除所有的空Groups，DefaultLocalGroup除外
    /// </summary>
    public static void RemoveAllEmptyGroups()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var groups = new List<AddressableAssetGroup>(settings.groups);
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            if (group?.name == "Built In Data") continue;
            if (group?.Default == true) continue;
            try
            {
                if (group != null && group.entries != null && group.entries.Count > 0) continue;
                settings.RemoveGroup(group);
            }
            catch
            {
                settings.groups.Remove(group);
            }
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, null, true, true);
        }
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RemoveEntitiesInGroup(AddressableAssetGroup group, AddressableAssetSettings settings)
    {
        if (group?.name == "Built In Data") return;
        try
        {
            var entries = new List<AddressableAssetEntry>(group.entries);
            entries.ForEach(x =>
            {
                try
                {
                    group.RemoveAssetEntry(x);
                }
                catch
                {
                    group.entries.Remove(x);
                }
            });
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, entries, true, true);
        }
        catch
        {
            try
            {
                settings.RemoveGroup(group);
            }
            catch
            {
                settings.groups.Remove(group);
            }
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupRemoved, null, true, true);
        }
    }

    private static void ReGroupAllEntities()
    {
        var keyAddressMap = new Dictionary<string, string>();
        var uiKeys = new List<string>();
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        string rootDir = Path.Combine(Environment.CurrentDirectory, AddressableRootGroupPath);
        string[] directories = Directory.GetDirectories(rootDir, "?_*", SearchOption.AllDirectories);
        Array.Sort(directories);
        for (int i = 0; i < directories.Length; i++)
        {
            string dirPath = directories[i];
            string dirName = Path.GetFileName(dirPath);
            if (!dirName.StartsWith(SinglePackPrefix) && !dirName.StartsWith(MultiPackPrefix) || dirName.Contains(IgnoreBuildTag))
            {
                continue;
            }
            
            if (!Application.isBatchMode)
            {
                float progress = i / ((float)directories.Length - 1);
                EditorUtility.DisplayProgressBar("分组中", $"正在分组{dirPath}，请稍后……", progress);
            }

            bool isSingle = dirName.StartsWith(SinglePackPrefix);
            bool isDynamic = dirName.EndsWith(DynamicLoadSuffix);
            string groupName = dirName;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            group ??= settings.CreateGroup(groupName,
                false,
                false,
                true,
                new List<AddressableAssetGroupSchema>(),
                typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema)
            );

            string[] files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
            Array.Sort(files);
            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].Contains(IgnoreBuildTag)) continue;
                var fileExt = Path.GetExtension(files[j]);
                if (string.IsNullOrEmpty(fileExt) || IgnoreExtentions.Contains(fileExt)) continue;
                string assetPath = files[j].Substring(Environment.CurrentDirectory.Length + 1);
                
                //所属分组的文件夹下的祖先文件夹中有任何一个以_dl结尾，也算dynamic
                bool fileWithDynamic = isDynamic || IsAncestorDirDynamic(files[j],dirPath.Length);

                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                string address = $"{Directory.GetParent(files[j]).Name}.{Path.GetFileName(files[j])}";
                AddressableAssetEntry entry = group.GetAssetEntry(guid);
                entry ??= settings.CreateOrMoveEntry(guid, group, false, false);

                if (entry == null)
                {
                    Debug.LogError(assetPath);
                    continue;
                }

                entry.SetAddress(address);
                if (fileWithDynamic)
                {
                    string key = Path.GetFileNameWithoutExtension(files[j]);
                    keyAddressMap[key] = address;
                    uiKeys.Add(key);
                }
            }

            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>() ?? group.AddSchema<BundledAssetGroupSchema>();
            schema.BundleMode = isSingle
                ? BundledAssetGroupSchema.BundlePackingMode.PackSeparately
                : BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.GUID;

            var updateSchema = group.GetSchema<ContentUpdateGroupSchema>() ?? group.AddSchema<ContentUpdateGroupSchema>();
            updateSchema.StaticContent = true;
        }
        EditorUtility.SetDirty(settings);
        SaveToAAConst(keyAddressMap, uiKeys);
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayProgressBar("分组中", "保存地址映射关系……", 1);
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SaveToAAConst(Dictionary<string, string> keyAddressMap, List<string> uiKeys)
    {
        uiKeys = uiKeys.Distinct().OrderBy(x => x).ToList();
        string destPath = Path.Combine(Environment.CurrentDirectory, KeyAddressScriptPath);
        StringBuilder builder = new StringBuilder();
        foreach (var item in uiKeys)
        {
            builder.Append("\t");
            builder.AppendFormat("public static string {0} = \"{1}\";\n", item, keyAddressMap[item]);
        }

        builder.Append("\n\t");
        builder.Append("public static Dictionary<string,string> keyAddressDict = new Dictionary<string,string>()\n");
        builder.Append("\t{\n");

        for (int i = 0; i < uiKeys.Count; i++)
        {
            builder.Append("\t\t{");
            builder.AppendFormat("\"{0}\" , {1}", uiKeys[i], uiKeys[i]);
            builder.Append("},\n");
        }
        builder.Append("\t};\n");

        builder.Append("\n\t");
        builder.Append("public static string GetAddress(string key)\n");
        builder.Append("\t{\n");
        builder.Append("\t\tif(keyAddressDict.TryGetValue(key, out var address))\n\t\t{\n\t\t\treturn address;\n\t\t}\n");
        builder.Append("\t\treturn \"\";\n");
        builder.Append("\t}\n");
        string finalContent = string.Format(TemplateAAConst, builder);
        File.WriteAllText(destPath, finalContent);
    }
    
    static bool IsAncestorDirDynamic(string file,int length)
    {
        var parent = Directory.GetParent(file);
        while (parent != null)
        {
            string fullName = parent.FullName;
            if (fullName.Length >= length)
            {
                if (fullName.EndsWith(DynamicLoadSuffix)) 
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

            parent = parent.Parent;
        }

        return false;
    }
}