/********************************************************************************
** Author： LiangZG
** Email :  game.liangzg@foxmail.com
*********************************************************************************/

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace GOE.Scene
{
    /// <summary>
    /// 描述：常规加载器
    /// <para>创建时间：2016-06-15</para>
    /// </summary>
    public sealed class SceneNomalLoader : ASceneLoader {


        protected override IEnumerator onLoadLevel(string sceneName)
        {
            return AssetLoader.LoadScene(sceneName , (obj)=> OnFinish() , progress);
        }

    }

}

