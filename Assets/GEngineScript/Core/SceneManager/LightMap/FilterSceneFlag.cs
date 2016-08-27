/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：地景过滤组件标识，生成Prefab实体时，被脚本绑定的结点，将不会被生成到目标Prefab内
/// <para>创建时间：2016-06-21</para>
/// </summary>
public class FilterSceneFlag : MonoBehaviour {

    public void SetActive(bool flag)
    {
        this.gameObject.SetActive(flag);
    }
    /// <summary>
    /// 生成预设时删除自己
    /// </summary>
    public void ClearSelf()
    {
        DestroyImmediate(this.gameObject);
    }

}
