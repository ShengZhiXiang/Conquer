using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class ShowMapInfo : EditorWindow
{

    [MenuItem("Tool/Window/GridCordinate")]
    public static void ShowWindow()
    {
        var window = GetWindow<ShowMapInfo>();
        window.Show();
    }
    private void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        Repaint();
    }
    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        //获取TileMap组件
        Tilemap map = FindObjectOfType<Tilemap>();
        if (map == null)
        {
            return;
        }
        //获取鼠标位置，当前是Scene视图坐标
        Vector2 mousePos = Event.current.mousePosition;
        //获取Scene窗体的Camera
        Camera cam = sceneView.camera;
        //Scene窗体中的缩放应当被考虑进去
        float mult = EditorGUIUtility.pixelsPerPoint;
        //将Scene视图的坐标转换成屏幕坐标
        mousePos.y = cam.pixelHeight - mousePos.y * mult;
        mousePos.x = mousePos.x * mult;
        //将屏幕坐标转成世界坐标
        mousePos = sceneView.camera.ScreenToWorldPoint(mousePos);
        Vector3Int vec3 = map.WorldToCell(mousePos);
        
        string str = vec3.ToString();
        int campID = -1;
        int battleUnit = 0;
        string terrianName = "";
        int leftPopulation = 0;
        int increasePopulation = 0;
        bool hasCannon = false;
        bool isInCool = false;
        int lastBomb = 0;
        bool opp = false;
        string cardFunc = "";
        BattleMap tmb = FindObjectOfType<BattleMap>();
        if (tmb != null)
        {
            if (tmb.Map != null)
            {
                opp = true;
                int x = tmb.Convert2ArrayIndex(vec3)[0];
                int y = tmb.Convert2ArrayIndex(vec3)[1];
                if (x >= 0 && x < tmb.Map.GetLength(0) && y >= 0 && y < tmb.Map.GetLength(1))
                {
                    campID = tmb.Map[x, y].CampID;
                    battleUnit = tmb.Map[x, y].BattleUnit;
                    terrianName = tmb.Map[x, y].CurTerrian.name;
                    leftPopulation = tmb.Map[x, y].leftPopulation;
                    increasePopulation = tmb.Map[x, y].increasePopulation;
                    hasCannon = tmb.Map[x, y].cannon.isOwned;
                    isInCool = tmb.Map[x, y].cannon.isInCool;
                    lastBomb = tmb.Map[x, y].cannon.lastFire;
                    cardFunc = tmb.Map[x, y].BattleCard != null? "有卡牌":"没有卡牌";
                }

            }
        }
        //绘制Label
        Handles.BeginGUI();
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.textColor = Color.green;   //设置字体颜色
        fontStyle.fontSize = 18;       //字体大小
        GUI.Label(new Rect(0f, 60f, 500f, 100f), "当前地块坐标:" + str, fontStyle);      
        GUI.Label(new Rect(0f, 90f, 500f, 100f), "当前地块地形:" + terrianName, fontStyle);
        GUI.Label(new Rect(0f, 120f, 500f, 100f), "当前地块剩余人口:" + leftPopulation, fontStyle);
        GUI.Label(new Rect(0f, 150f, 500f, 100f), "当前地块每回合增长人口:" + increasePopulation, fontStyle);
        GUI.Label(new Rect(0f, 180f, 500f, 100f), "地图是否为空:" + opp , fontStyle);
        GUI.Label(new Rect(0f, 210f, 500f, 100f), "**********************");
        //作战信息
        GUI.Label(new Rect(0f, 240f, 500f, 100f), "当前地块阵营:" + campID.ToString(), fontStyle);
        GUI.Label(new Rect(0f, 270f, 500f, 100f), "当前地块作战单位:" + battleUnit.ToString(), fontStyle);
        GUI.Label(new Rect(0f, 300f, 500f, 100f), "当前地块是否有炮:" + hasCannon.ToString(), fontStyle);
        GUI.Label(new Rect(0f, 330f, 500f, 100f), "当前地块的炮是否处于冷却:" + isInCool.ToString(), fontStyle);
        GUI.Label(new Rect(0f, 360f, 500f, 100f), "当前地块的炮上一次开火回合:" + lastBomb.ToString(), fontStyle);
        GUI.Label(new Rect(0f, 390f, 500f, 100f), "当前地块的卡牌:" + cardFunc, fontStyle);
        Handles.EndGUI();

        //刷新界面，保证坐标实时跟随鼠标
        sceneView.Repaint();
        HandleUtility.Repaint();

    }

}
