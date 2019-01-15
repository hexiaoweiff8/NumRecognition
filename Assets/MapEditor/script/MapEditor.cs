using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
//#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 地图编辑器
/// </summary>
public class MapEditor : MonoBehaviour
{

    // -----------------------外置属性-----------------------
    /// <summary>
    /// 地面
    /// </summary>
    public BoxCollider Plane;

    /// <summary>
    /// 障碍物对象, 如果该对象为空则创建cube
    /// </summary>
    public GameObject Obstacler;

    /// <summary>
    /// 障碍物父级
    /// </summary>
    public GameObject ObstaclerList;

    /// <summary>
    /// 列表按钮
    /// </summary>
    public Button MenuButton;

    /// <summary>
    /// 主相机
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// 地图宽度
    /// </summary>
    public int MapWidth = 100;

    /// <summary>
    /// 地图高度
    /// </summary>
    public int MapHeight = 100;

    /// <summary>
    /// 单位宽度
    /// </summary>
    public int UnitWidth = 1;

    /// <summary>
    /// 鼠标敏感度
    /// </summary>
    public int MouseSensitivity = 10;

    /// <summary>
    /// 网格线颜色
    /// </summary>
    public Color LineColor = Color.red;

    /// <summary>
    /// level1层元素
    /// </summary>
    public List<KeyValuePair<int, string>> Level1ItemList = new List<KeyValuePair<int, string>>()
    {
        new KeyValuePair<int, string>(101, "类型1"),
    };

    /// <summary>
    /// level2层元素
    /// </summary>
    public List<KeyValuePair<int, string>> Level2ItemList = new List<KeyValuePair<int, string>>()
    {
        new KeyValuePair<int, string>(200, "障碍1"),
        new KeyValuePair<int, string>(201, "障碍2"),
    };

    /// <summary>
    /// level3层元素
    /// </summary>
    public List<KeyValuePair<int, string>> Level3ItemList = new List<KeyValuePair<int, string>>()
    {
        new KeyValuePair<int, string>(301, "出口"),
        new KeyValuePair<int, string>(302, "入口"),
        new KeyValuePair<int, string>(401, "塔基"),
    };

    //--------------------常量---------------------------

    /// <summary>
    /// 当前位置可设置
    /// </summary>
    public const int CouldSet = 0;

    /// <summary>
    /// 当前位置已设置
    /// </summary>
    public const int SetYet = 1;
    


    //--------------------私有属性-----------------------

    /// <summary>
    /// 网格
    /// </summary>
    private int[][] array = null;

    /// <summary>
    /// 鼠标点击状态
    /// 0: 当前位置无障碍
    /// 1: 当前位置有障碍
    /// </summary>
    private int mapControlState = CouldSet;

    /// <summary>
    /// 是否正在操作地图
    /// </summary>
    private bool isInControl = false;

    /// <summary>
    /// 地图状态
    /// </summary>
    //private Dictionary<long, GameObject> mapStateDic = new Dictionary<long, GameObject>();

    /// <summary>
    /// 地图状态
    /// </summary>
    private Dictionary<int, GameObject[,]> mapStateDic = new Dictionary<int, GameObject[,]>();

    /// <summary>
    /// 当前level列表
    /// </summary>
    private List<KeyValuePair<int, string>> nowLevelItemList = null;

    /// <summary>
    /// 按钮列表
    /// </summary>
    private List<Button> buttonList = new List<Button>();


    // -----------------------------障碍曾ID---------------------------------

    /// <summary>
    /// 无障碍物
    /// </summary>
    private const int AccessibilityId = 0;



    /// <summary>
    /// 障碍物颜色
    /// </summary>
    private Dictionary<int, Color> obstaclerColor = new Dictionary<int, Color>()
    {
        {0, Color.white},
        {1, Color.black},
    };


    /// <summary>
    /// 当前类型ID
    /// </summary>
    private int NowTypeId
    {
        get { return nowTypeId; }
        set { nowTypeId = value; }
    }

    private int nowTypeId = 0;

    //-------------------计算优化属性---------------------
    /// <summary>
    /// 半地图宽度
    /// </summary>
    private float halfMapWidth;

    /// <summary>
    /// 半地图长度
    /// </summary>
    private float halfMapHight;

    /// <summary>
    /// 地图四角位置
    /// 初始化时计算
    /// </summary>
    private Vector3 leftup = Vector3.zero;
    private Vector3 leftdown = Vector3.zero;
    private Vector3 rightup = Vector3.zero;
    private Vector3 rightdown = Vector3.zero;


    //---------------------定义结束-----------------------
    void Start()
    {
        Init();
    }

    void Update()
    {
        // 控制
        Control();
        // Plane上画网格
        DrawLine();
        // 刷新地图状态
        RefreshMap();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {

        if (Plane != null)
        {
            // 设置地图大小
            // 设置地缩放
            Plane.transform.localScale = new Vector3(MapWidth, 1, MapHeight);
            mapStateDic[1] = new GameObject[MapHeight, MapWidth];

            // 初始化障碍物列表
            foreach (var mapData in mapStateDic)
            {
                foreach (var item in mapData.Value)
                {
                    if (item != null)
                    {
                        Destroy(item);
                    }
                }
            }

            // 创建对应大小的map数据
            array = new int[MapHeight][];


            for (var row = 0; row < MapHeight; row++)
            {
                array[row] = new int[MapWidth];
                for (var col = 0; col < MapWidth; col++)
                {
                    array[row][col] = 1;
                }
            }
            // 初始化优化数据
            halfMapWidth = MapWidth / 2.0f;
            halfMapHight = MapHeight / 2.0f;

            // 获得起始点
            Vector3 startPosition = Plane.transform.position;
            // 初始化四角点
            leftup = new Vector3(-halfMapWidth + startPosition.x, (Plane.size.y * Plane.transform.localScale.y) / 2 + startPosition.y, halfMapHight + startPosition.z);
            leftdown = new Vector3(-halfMapWidth + startPosition.x, (Plane.size.y * Plane.transform.localScale.y) / 2 + startPosition.y, -halfMapHight + startPosition.z);
            rightup = new Vector3(halfMapWidth + startPosition.x, (Plane.size.y * Plane.transform.localScale.y) / 2 + startPosition.y, halfMapHight + startPosition.z);
            rightdown = new Vector3(halfMapWidth + startPosition.x, (Plane.size.y * Plane.transform.localScale.y) / 2 + startPosition.y, -halfMapHight + startPosition.z);

        }
    }

    /// <summary>
    /// 弹出加载框
    /// </summary>
    public void LoadConfig()
    {
        // 创建窗口是否加载文件或新建文件
        var loadMapData = EditorWindow.GetWindow<LoadOrCreateWindow>(typeof(LoadOrCreateWindow));
        loadMapData.Start();
        loadMapData.LoadMapData = (mapDataArray) =>
        {
            array = StringMapData2IntArray(mapDataArray[0]);
        };
    }


    /// <summary>
    /// 控制
    /// 上下左右控制相机x,z轴移动
    /// pageup pagedown 控制相机y轴移动
    /// 鼠标控制相机方向
    /// 回车保存地图
    /// </summary>
    private void Control()
    {
        // 上下左右位置移动
        // 移动x,z轴
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            MainCamera.transform.localPosition += new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            MainCamera.transform.localPosition -= new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            MainCamera.transform.localPosition += Quaternion.Euler(0, -90, 0) * new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            MainCamera.transform.localPosition += Quaternion.Euler(0, 90, 0) * new Vector3(MainCamera.transform.forward.x, 0, MainCamera.transform.forward.z);
        }
        // 滚轮拉近拉远
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            MainCamera.transform.localPosition -= MainCamera.transform.forward;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            MainCamera.transform.localPosition += MainCamera.transform.forward;
        }

        // 移动y轴
        if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.E))
        {
            MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y + 1, MainCamera.transform.localPosition.z);
        }
        if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Q))
        {
            MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y - 1, MainCamera.transform.localPosition.z);
        }

        // 方向移动
        if (Input.GetMouseButton(1))
        {
            float rotateX = MainCamera.transform.localEulerAngles.x - Input.GetAxis("Mouse Y");
            float rotateY = MainCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X");
            MainCamera.transform.localEulerAngles = new Vector3(rotateX, rotateY, 0);
        }

        // 输出地图数据 回车
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {
            ConsoleMap();
        }

        // 清空地图 esc
        if (Input.GetKey(KeyCode.Escape))
        {
            Init();
            // 加载文件
            LoadConfig();
        }

        // 创建障碍物
        CreateObstacle();
    }

    /// <summary>
    /// 在地图上画出网格
    /// </summary>
    private void DrawLine()
    {
        // 在底板上画出格子
        // 画四边
        Debug.DrawLine(leftup, rightup, LineColor);
        Debug.DrawLine(leftup, leftdown, LineColor);
        Debug.DrawLine(rightdown, rightup, LineColor);
        Debug.DrawLine(rightdown, leftdown, LineColor);

        // 获得格数
        var xCount = MapWidth / UnitWidth;
        var yCount = MapHeight / UnitWidth;

        for (var i = 1; i <= xCount; i++)
        {
            Debug.DrawLine(leftup + new Vector3(i * UnitWidth, 0, 0), leftdown + new Vector3(i * UnitWidth, 0, 0), LineColor);
        }
        for (var i = 1; i <= yCount; i++)
        {
            Debug.DrawLine(leftdown + new Vector3(0, 0, i * UnitWidth), rightdown + new Vector3(0, 0, i * UnitWidth), LineColor);
        }
    }

    /// <summary>
    /// 创建障碍物
    /// 鼠标点击创建障碍物
    /// </summary>
    private void CreateObstacle()
    {
        if (MainCamera == null)
        {
            Debug.Log("主相机为空.");
            return;
        }

        if (Input.GetMouseButton(0))
        {
            var ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name.Equals(Plane.name))
                {
                    // 识别位置
                    var posOnMap = Utils.PositionToNum(Plane.transform.position, hit.point, UnitWidth, MapWidth, MapHeight);
                    // Debug.Log(posOnMap[0] + ":" + posOnMap[1]);
                    // 若该位置没有障碍则记录并创建障碍, 若该位置有障碍则消除该位置的障碍
                    if (!isInControl)
                    {
                        mapControlState = array[posOnMap[1]][posOnMap[0]] != AccessibilityId ? SetYet : CouldSet;
                        isInControl = true;
                    }
                    else
                    {
                        switch (mapControlState)
                        {
                            case SetYet:
                                // 如果该位置已有障碍, 则将障碍清除, 反之不处理
                                array[posOnMap[1]][posOnMap[0]] = AccessibilityId;
                                break;
                            case CouldSet:
                                // 如果该位置未有物体, 则将障碍设置, 反之不处理
                                array[posOnMap[1]][posOnMap[0]] = NowTypeId;
                                break;
                        }
                    }

                }
            }
        }

        // 释放状态
        if (Input.GetMouseButtonUp(0))
        {
            isInControl = false;
        }
    }

    /// <summary>
    /// 将map状态刷到plane上
    /// </summary>
    private void RefreshMap()
    {
        RefreshMap(array, 1);
    }

    private void RefreshMap(int[][] map, int layer)
    {
        var mapState = mapStateDic[layer];
        for (long row = 0; row < map.Length; row++)
        {
            var oneRow = map[row];
            for (long col = 0; col < oneRow.Length; col++)
            {
                var val = oneRow[col];
                if (AccessibilityId != val)
                {
                    var newObstacler = mapState[row, col];
                    if (newObstacler == null)
                    {
                        // 创建该位置标志
                        newObstacler = CreateObstacler(ObstaclerList == null ? null : ObstaclerList.transform,
                            new Vector3(UnitWidth, UnitWidth, UnitWidth),
                            Utils.NumToPositionV(Plane.transform.position + Vector3.up * UnitWidth, // + new Vector3(UnitWidth * 0.5f, 0, UnitWidth * 0.5f),
                            new Vector2(col, row),
                            UnitWidth,
                            MapWidth,
                            MapHeight),
                            layer);
                        mapState[row, col] = newObstacler;
                    }
                    // 变更不同层级单位的颜色
                    var meshRander = newObstacler.GetComponent<MeshRenderer>();
                    //print(val);
                    meshRander.material.color = obstaclerColor[val];
                    newObstacler.layer = layer;
                }
                else
                {
                    if (val == AccessibilityId)
                    {
                        if (mapState[row, col] != null)
                        {
                            Destroy(mapState[row, col]);
                            mapState[row, col] = null;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建障碍物对象
    /// 如果障碍物引用为空则创建cube
    /// </summary>
    /// <returns>障碍物引用</returns>
    private GameObject CreateObstacler(Transform parent, Vector3 scale, Vector3 pos, int layer)
    {
        if (Obstacler == null)
        {
            Obstacler = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(Obstacler.GetComponent<BoxCollider>());
            Obstacler.name = "Obstacler";
            Obstacler.transform.localPosition = leftup;
            // 显示层级为当前控制层级
            Obstacler.layer = 1;
        }
        var result = Instantiate(Obstacler);

        result.transform.parent = parent;
        result.transform.localScale = scale;
        result.transform.position = pos;
        result.layer = layer;

        return result;
    }

    


    /// <summary>
    /// 将map数据输出
    /// </summary>
    public void ConsoleMap()
    {
        // TODO 弹出框强制输入文件名称
        var window = EditorWindow.GetWindow<EditMapNameWindow>(true, "地图保存");
        window.MapData.Push(IntArrayArrayToString(array));
        window.Start();
    }


    private string IntArrayArrayToString(int[][] map)
    {
        var strResult = new StringBuilder();
        for (var row = 0; row < map.Length; row++)
        {
            var oneRow = map[row];
            for (var col = 0; col < oneRow.Length; col++)
            {
                var cell = oneRow[col];
                strResult.Append(cell + ((col == oneRow.Length - 1) ? "" : ","));
            }
            strResult.Append((row == array.Length - 1) ? "" : "\n");
        }
        return strResult.ToString();
    }


    private int[][] StringMapData2IntArray(string mapDataStr)
    {
        int[][] mapData = null;
        var rowCount = 0;
        var colCount = 0;
        if (!string.IsNullOrEmpty(mapDataStr))
        {
            // 解析字符串
            var rows = mapDataStr.Split('\n');
            rowCount = rows.Length;
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                var cells = row.Split(',');
                if (colCount == 0)
                {
                    colCount = cells.Length;
                    mapData = new int[rowCount][];
                }
                mapData[i] = new int[colCount];
                for (var j = 0; j < cells.Length; j++)
                {
                    var cell = cells[j];
                    try
                    {
                        mapData[i][j] = Convert.ToInt32(cell);
                    }
                    catch
                    {
                        int s = 1;
                    }
                }
            }
        }
        return mapData;
    }

    // -----------------------------外部事件-----------------------------
    

    /// <summary>
    /// 刷新列表
    /// </summary>
    private void RefreshList()
    {
        Debug.Log("刷新列表");
        MenuButton.gameObject.SetActive(false);
        // 清空列表
        if (buttonList.Count > 0)
        {
            // 销毁所有按钮
            while (buttonList.Count > 0)
            {
                var button = buttonList[0];
                Destroy(button.gameObject);
                buttonList.RemoveAt(0);
            }
        }

        // 遍历当前level列表
        if (nowLevelItemList != null)
        {
            foreach (var kv in nowLevelItemList)
            {
                var id = kv.Key;
                var name = kv.Value;
                // 创建新按钮
                var newButton = Instantiate(MenuButton);
                buttonList.Add(newButton);
                newButton.transform.parent = MenuButton.transform.parent;
                newButton.gameObject.SetActive(true);
                var btnText = newButton.GetComponentInChildren<Text>();
                btnText.text = name;

                // 添加事件
                newButton.onClick = new Button.ButtonClickedEvent();
                newButton.onClick.AddListener(() =>
                {
                    Debug.Log("切换:" + name);
                    // 切换id
                    NowTypeId = id;

                });
            }
        }
        

        // 创建按钮并添加事件

    }
}


//#endif