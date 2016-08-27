/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// <summary>
/// 描述：Lua脚本创建编辑器
/// <para>创建时间：2016-08-09</para>
/// </summary>
public class LuaScriptEditor : EditorWindow
{
    private  List<string> templetes = new List<string>(); 
    private Dictionary<string , string>  nameAndPath = new Dictionary<string, string>();

    private int selectTempleteIndex;
    private string tableName = "";
    private string selectDir = "";

    [MenuItem("GameObject/Custom/CreateLua")]
    [MenuItem("AssetTools/Lua %L" , false , 50)]
    public static void CreateLuaScript()
    {
        LuaScriptEditor editor = EditorWindow.GetWindow<LuaScriptEditor>("Create Lua");
        editor.Initizalier();
        editor.Show();
    }

    public void Initizalier()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Editor/Script", "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileName = fileName.Replace("Templete", "");
            templetes.Add(fileName);

            nameAndPath.Add(fileName , file);
        }

        string log = getCreateLog;
        if (!string.IsNullOrEmpty(log))
        {
            string[] logInfos = log.Split(';');
            selectTempleteIndex = Convert.ToInt32(logInfos[0]);
            selectDir = logInfos[1];
        }
    }


    public void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.MinHeight(100));
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Templete:" , GUILayout.MaxWidth(100));
        selectTempleteIndex = EditorGUILayout.Popup(selectTempleteIndex, templetes.ToArray());
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Table Name:", GUILayout.MaxWidth(100));
        tableName = EditorGUILayout.TextField(tableName);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.LabelField("Path", GUILayout.MaxWidth(100));
        GUILayout.BeginHorizontal();
        selectDir = selectDir.Replace(Application.dataPath, "");
        GUILayout.TextField(string.IsNullOrEmpty(selectDir) ? "[Warn]No Path!!" : selectDir);
        if (GUILayout.Button("..." , GUILayout.MaxWidth(30)))
        {
            selectDir = EditorUtility.OpenFolderPanel("Save Folder", Application.dataPath + "/GameScript", "");
            selectDir = selectDir.Replace(Application.dataPath, "Assets");
        }
        GUILayout.EndHorizontal();



        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space();

            if (GUILayout.Button("Create" , GUILayout.Width(80)))
            {
                string fileText = File.ReadAllText(nameAndPath[templetes[selectTempleteIndex]]);
                fileText = fileText.Replace("tableName", tableName);

                string filePath = Path.Combine(selectDir, tableName);
                if (!filePath.EndsWith(".lua")) filePath += ".lua";

                filePath = filePath.Replace("\\", "/");
                File.WriteAllText(filePath , fileText);

                SaveLog(); //保存记录
                
                AssetDatabase.ImportAsset(filePath);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);

                AssetDatabase.Refresh();
            }
        }


        GUILayout.EndVertical();
    }

    private string key
    {
        get { return Application.dataPath + typeof (LuaScriptEditor); }
    }

    private string getCreateLog
    {
        get { return EditorPrefs.GetString(key); }

    }

    private void SaveLog()
    {
        EditorPrefs.SetString(key, selectTempleteIndex + ";" + selectDir);
    }
}
