/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPageRoot : Singleton<UIPageRoot>
{

    private static Vector2 screenResolution = new Vector2(1280 , 720);

    public Transform root;
    public Transform fixedRoot;
    public Transform normalRoot;
    public Transform popupRoot;
    public Camera uiCamera;

    private int offsetOrder = 50;  //界面深度偏移值

    private Vector2 normalRange = new Vector2(500, 1500);
    private int normalOrder;
    
    private Vector2 popupRange = new Vector2(1600 , 2000);
    private int popupOrder;
    
    protected override void Awake()
    {
        base.Awake();

        initRoot();
    }

    private void initRoot()
    {
        GameObject go = new GameObject("UIRoot");
        go.layer = LayerMask.NameToLayer("UI");
        go.AddComponent<RectTransform>();

        GameObject camObj = new GameObject("UICamera");
        camObj.layer = LayerMask.NameToLayer("UI");
        camObj.transform.parent = go.transform;
        camObj.transform.localPosition = new Vector3(0,0,-100f);

        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Depth;
        cam.orthographic = true;
        cam.farClipPlane = 200f;
        
        uiCamera = cam;
        cam.cullingMask = 1<<5;
        cam.nearClipPlane = -50f;
        cam.farClipPlane = 50f;

//        Canvas can = go.AddComponent<Canvas>();
//        can.renderMode = RenderMode.ScreenSpaceCamera;
//        can.pixelPerfect = true;
//        can.worldCamera = cam;

        //add audio listener
        camObj.AddComponent<AudioListener>();
        camObj.AddComponent<GUILayer>();

//        CanvasScaler cs = go.AddComponent<CanvasScaler>();
//        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//        cs.referenceResolution = screenResolution;
//        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        GameObject subRoot = CreateSubCanvasForRoot(go.transform,250);
        subRoot.name = "FixedRoot";
        fixedRoot = subRoot.transform;

        subRoot = CreateSubCanvasForRoot(go.transform,0);
        subRoot.name = "NormalRoot";
        normalRoot = subRoot.transform;

        subRoot = CreateSubCanvasForRoot(go.transform,500);
        subRoot.name = "PopupRoot";
        popupRoot = subRoot.transform;

        //add Event System
        GameObject eventObj = GameObject.Find("EventSystem");
        if(eventObj == null)
        {
            eventObj = new GameObject("EventSystem");
            eventObj.AddComponent<EventSystem>();
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        eventObj.layer = LayerMask.NameToLayer("UI");
        eventObj.transform.SetParent(go.transform);

        //初始化排序
        normalOrder = (int)normalRange.x;
    }

    static GameObject CreateSubCanvasForRoot(Transform root,int sort)
    {
        GameObject go = new GameObject("canvas");
        go.transform.parent = root;
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;

//        Canvas can = go.AddComponent<Canvas>();
//        RectTransform rect = go.GetComponent<RectTransform>();
//        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
//        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
//        rect.anchorMin = Vector2.zero;
//        rect.anchorMax = Vector2.one;
//
//        can.overrideSorting = true;
//        can.sortingOrder = sort;
//
//        go.AddComponent<GraphicRaycaster>();

        return go;
    }

    /// <summary>
    /// 获得对应Page类型的Root
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Transform GetRoot(EPageType type)
    {
        if(type == EPageType.Fixed)     return fixedRoot;
        if(type == EPageType.Normal)    return normalRoot;
        if(type == EPageType.PopUp)     return popupRoot;

        return root;
    }

    /// <summary>
    /// 增加Normal类型的界面显示序号
    /// </summary>
    /// <returns></returns>
    public int AddOrder(EPageType pageType)
    {
        switch (pageType)
        {
                case EPageType.Normal:
                normalOrder = (int)Mathf.Clamp(normalOrder + offsetOrder, normalRange.x, normalRange.y);
                return normalOrder;
                case EPageType.PopUp:
                popupOrder = (int)Mathf.Clamp(popupOrder + offsetOrder, popupRange.x, popupRange.y);
                return popupOrder;

        }

        Debug.LogError("固定类型的页界，不进行深度自动排序");
        return 0;
    }
    /// <summary>
    /// 减小Normal类型的界面显示序号
    /// </summary>
    /// <returns></returns>
    public int SubOrder(EPageType pageType)
    {
        switch (pageType)
        {
            case EPageType.Normal:
                normalOrder = (int)Mathf.Clamp(normalOrder - offsetOrder, normalRange.x, normalRange.y);
                return normalOrder;
            case EPageType.PopUp:
                popupOrder = (int)Mathf.Clamp(popupOrder - offsetOrder, popupRange.x, popupRange.y);
                return popupOrder;

        }

        Debug.LogError("固定类型的页界，不进行深度自动排序");
        return 0;
    }
}
