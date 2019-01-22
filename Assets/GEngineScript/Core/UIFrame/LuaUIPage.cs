/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using LuaInterface;
using UnityEngine.UI;

/// <summary>
/// 描述：Lua层UIPage生命期中转
/// <para>创建时间：2016-08-08</para>
/// </summary>
public class LuaUIPage : ABaseUIPage
{
    private Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();

      
    #region -----------Page 生命周期-----------------
    public override void OnAwake(GameObject gObj)
    {
        base.OnAwake(gObj);

        BaseLuaManager luaMgr = AppFacade.Instance.GetManager<BaseLuaManager>();
        luaMgr.LuaMachine[Name + ".gameObject"] = gObj;
        luaMgr.LuaMachine[Name + ".transform"] = gObj.transform;
        luaMgr.LuaMachine[Name + ".ui"] = this;
        //添加事件
        Util.CallMethod("EventManager", "AddEvent", Name);

        Util.CallMethod(Name, "OnAwake" , this);
    }

    /// <summary>
    /// 每次显示时调用
    /// </summary>
    public override void OnShow(object param)
    {
        base.OnShow(param);
        Util.CallMethod(Name, "OnShow" , param);
    }


    public override void OnHide()
    {
        base.OnHide();

        Util.CallMethod(Name, "OnHide");
        //注销事件
        Util.CallMethod("EventManager", "ClearEvent", Name);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        
        this.ClearClick();

        Util.CallMethod(Name, "OnDestroy");
    }

    #endregion

    #region ------------按钮点击---------------------
    /// <summary>
    /// 添加单击事件
    /// </summary>
    public void AddClick(GameObject go, LuaFunction luafunc)
    {
        if (go == null || luafunc == null) return;
        buttons.Add(go.name, luafunc);
        go.GetComponent<Button>().onClick.AddListener(
            delegate () {
                luafunc.Call(go);
            }
        );
    }

    /// <summary>
    /// 添加结点按钮点击事件
    /// </summary>
    /// <param name="goHierarchy">层次结点</param>
    /// <param name="luafunc"></param>
    public void AddClick(string goHierarchy, LuaFunction luafunc)
    {
        Transform btnTrans = CacheTrans.Find(goHierarchy);
        if (btnTrans == null)
        {
            Debugger.LogWarning("<<LuaUIPage, AddClildClick>> Cant find transform ! hierarchy:" + goHierarchy);
            return;
        }
        AddClick(btnTrans.gameObject, luafunc);
    }


    /// <summary>
    /// 删除单击事件
    /// </summary>
    /// <param name="go"></param>
    public void RemoveClick(GameObject go)
    {
        if (go == null) return;
        LuaFunction luafunc = null;
        if (buttons.TryGetValue(go.name, out luafunc))
        {
            luafunc.Dispose();
            luafunc = null;
            buttons.Remove(go.name);
        }
    }

    public void RemoveClick(string goHierarchy)
    {
        Transform btnTrans = CacheTrans.Find(goHierarchy);
        if (btnTrans == null)
        {
            Debugger.LogWarning("<<LuaUIPage, RemoveClick>> Cant find transform ! hierarchy:" + goHierarchy);
            return;
        }
        RemoveClick(btnTrans.gameObject);
    }

    /// <summary>
    /// 清除单击事件
    /// </summary>
    public void ClearClick()
    {
        foreach (var de in buttons)
        {
            if (de.Value != null)
            {
                de.Value.Dispose();
            }
        }
        buttons.Clear();
    }
    #endregion
}
