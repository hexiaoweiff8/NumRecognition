//using UnityEngine;
//using System.Collections;

//public class UIControl : MonoBehaviour
//{

//    public GameObject BuildingList;


//#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
//    public MapEditor MapEditorHolder;


//    private bool isShowBuildingList = false;


//    void Start () {
	
//        BuildingList.SetActive(false);
//    }

//    /// <summary>
//    /// 选择level1
//    /// </summary>
//    public void ChooseLevel1()
//    {
//        MapEditorHolder.ChangeLevel1();
//    }

//    /// <summary>
//    /// 选择level2
//    /// </summary>
//    public void ChooseLevel2()
//    {
//        if (!isShowBuildingList)
//        {
//            MapEditorHolder.ChangeLevel2();
//        }
//    }

//    /// <summary>
//    /// 选择level3
//    /// </summary>
//    public void ChooseLevel3()
//    {
//        MapEditorHolder.ChangeLevel3();
//    }


//    /// <summary>
//    /// 选择建筑
//    /// </summary>
//    public void ShowOrCloseItemSelect()
//    {
//        // 显示/隐藏UI
//        isShowBuildingList = !isShowBuildingList;
//        BuildingList.SetActive(isShowBuildingList);
//    }


//    /// <summary>
//    /// 绘制己方基地位置
//    /// </summary>
//    public void SelectMyBase()
//    {
//        MapEditorHolder.SelectMyBase();
//    }

//    /// <summary>
//    /// 绘制己方炮塔位置
//    /// </summary>
//    public void SelectMyTurret()
//    {
//        MapEditorHolder.SelectMyTurret();
//    }

//    /// <summary>
//    /// 绘制敌方基地位置
//    /// </summary>
//    public void SelectEnemyBase()
//    {
//        MapEditorHolder.SelectEnemyBase();
//    }

//    /// <summary>
//    /// 绘制敌方炮塔位置
//    /// </summary>
//    public void SelectEnemyTurret()
//    {
//        MapEditorHolder.SelectEnemyTurret();
//    }

//    /// <summary>
//    /// 弹出加载框
//    /// </summary>
//    public void Load()
//    {
//        MapEditorHolder.LoadConfig();
//    }

//    /// <summary>
//    /// 弹出保存Window
//    /// </summary>
//    public void Save()
//    {
//        MapEditorHolder.ConsoleMap();
//    }

//    /// <summary>
//    /// 清空地图
//    /// </summary>
//    public void Clear()
//    {
//        MapEditorHolder.Init();
//    }

//#endif
//}
