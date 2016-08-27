/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// 描述：应用主框架入口
/// <para>创建时间：2016-08-03</para>
/// </summary>
public class AppMain : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GameObject.DontDestroyOnLoad(this.gameObject);

        AppFacade.Instance.StartUp();   //启动游戏
    }
}
