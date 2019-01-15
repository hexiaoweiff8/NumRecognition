using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


/// <summary>
/// 地图名称编辑窗口
/// </summary>
public class EditMapNameWindow : EditorWindow
{
    /// <summary>
    /// 地图文件名称
    /// </summary>
    public string MapFileId = "0001";

    /// <summary>
    /// 地图数据
    /// </summary>
    public Stack<string> MapData = new Stack<string>();

    /// <summary>
    /// 文件保存位置
    /// </summary>
    public string MapDataFilePath = "";

    public void Start()
    {
        MapDataFilePath = Application.dataPath + @"\StreamingAssets\config\mapDatas";
        var dInfo = new DirectoryInfo(MapDataFilePath);
        if (!dInfo.Exists)
        {
            dInfo.Create();
        }
        Debug.Log("开始编辑地图名称, 文件保存位置:" + MapDataFilePath);
    }


    void OnGUI()
    {
        MapFileId = EditorGUILayout.TextField("地图文件名称:", MapFileId);
        EditorGUILayout.TextField("文件保存位置:", MapDataFilePath);

        if (GUILayout.Button("保存"))
        {
            SaveMapData();
        }
    }


    private void SaveMapData()
    {
        // 验证数据是否有效
        if (string.IsNullOrEmpty(MapFileId) || string.IsNullOrEmpty(MapDataFilePath))
        {
            Debug.LogError("数据错误: MapData:" + MapData);
            Debug.LogError("数据错误: MapFileName:" + MapFileId);
            Debug.LogError("数据错误: MapDataFilePath:" + MapDataFilePath);
        }
        var level = 1;
        while (MapData.Count > 0)
        {
            Utils.CreateOrOpenFile(MapDataFilePath, Utils.GetMapFileNameById(MapFileId, level), MapData.Pop());
            level++;
        }
        Debug.Log("地图文件保存成功:" + MapFileId);
    }




}


/// <summary>
/// 地图名称编辑窗口
/// </summary>
public class LoadOrCreateWindow : EditorWindow
{
    /// <summary>
    /// 地图文件名称
    /// </summary>
    public string MapFileName = "MapInfo0001";

    /// <summary>
    /// 文件保存位置
    /// </summary>
    public string MapDataFilePath = "";

    /// <summary>
    /// 回调调用文件
    /// </summary>
    public Action<string[]> LoadMapData = null;

    public void Start()
    {
        MapDataFilePath = Application.dataPath + @"\StreamingAssets\config\mapDatas\";
        Debug.Log("开始编辑地图名称, 文件保存位置:" + MapDataFilePath);
    }


    void OnGUI()
    {
        MapFileName = EditorGUILayout.TextField("地图文件名称:", MapFileName);
        if (GUILayout.Button("加载"))
        {
            var mapDataLevel1 = Utils.LoadFileInfo(MapDataFilePath + MapFileName + "_Level1");
            var mapDataLevel2 = Utils.LoadFileInfo(MapDataFilePath + MapFileName + "_Level2");
            var mapDataLevel3 = Utils.LoadFileInfo(MapDataFilePath + MapFileName + "_Level3");
            if (mapDataLevel1 == null)
            {
                Debug.LogError("文件不存在:" + MapDataFilePath + MapFileName);
                return;
            }
            LoadMapData(new[] { mapDataLevel1, mapDataLevel2, mapDataLevel3});
        }

        if (GUILayout.Button("新建"))
        {
            this.Close();
        }
    }
    


}

//#endif