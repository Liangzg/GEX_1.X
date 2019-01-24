using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static ClickEventListener Get(GameObject obj)
    {
        ClickEventListener listener = obj.GetComponent<ClickEventListener>();
        if (listener == null)
        {
            listener = obj.AddComponent<ClickEventListener>();
        }
        return listener;
    }

    Action<GameObject, PointerEventData> mClickedHandler = null;
    Action<GameObject, PointerEventData> mDoubleClickedHandler = null;
    Action<GameObject, PointerEventData> mOnPointerDownHandler = null;
    Action<GameObject, PointerEventData> mOnPointerUpHandler = null;
    Action<GameObject, PointerEventData> mOnPointerEnterHandler = null;
    Action<GameObject, PointerEventData> mOnPointerExitHandler = null;

    bool mIsPressed = false;
    bool mForbidEvent = false;//意义 禁用当前自己的事件

    public bool IsPressd
    {
        get { return mIsPressed; }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!mForbidEvent)
        {
            if (eventData.clickCount == 2)
            {
                if (mDoubleClickedHandler != null)
                {
                    mDoubleClickedHandler(gameObject, eventData);
                }
            }
            else
            {
                if (mClickedHandler != null)
                {
                    mClickedHandler(gameObject, eventData);
                }
            }
        }

    }
    public void SetClickEventHandler(Action<GameObject, PointerEventData> handler)
    {
        mClickedHandler += handler;
    }

    public void SetDoubleClickEventHandler(Action<GameObject, PointerEventData> handler)
    {
        mDoubleClickedHandler += handler;
    }

    public void SetPointerDownHandler(Action<GameObject, PointerEventData> handler)
    {
        mOnPointerDownHandler += handler;
    }

    public void SetPointerUpHandler(Action<GameObject, PointerEventData> handler)
    {
        mOnPointerUpHandler += handler;
    }

    public void SetPointerEnterHandler(Action<GameObject, PointerEventData> handler)
    {
        mOnPointerEnterHandler += handler;
    }

    public void SetPointerExitHandler(Action<GameObject, PointerEventData> handler)
    {
        mOnPointerExitHandler += handler;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mIsPressed = true;
        if (mOnPointerDownHandler != null)
        {
            mOnPointerDownHandler(gameObject, eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mIsPressed = false;
        if (mOnPointerUpHandler != null)
        {
            mOnPointerUpHandler(gameObject, eventData);
        }
        mForbidEvent = false;
        if (eventData.pointerCurrentRaycast.gameObject != null &&
        (eventData.pointerCurrentRaycast.gameObject.layer == LayerMask.NameToLayer("TipsPenetrate")))
        {
            //再往下传
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            GameObject current = eventData.pointerCurrentRaycast.gameObject;

            for (int i = 0; i < results.Count; i++)
            {
                var nextObject = results[i].gameObject;
                if (current != nextObject)
                {
                    bool canClick = ExecuteEvents.CanHandleEvent<IPointerClickHandler>(nextObject);
                    if (canClick && nextObject.tag == "ItemCell")
                    {
                        mForbidEvent = true;
                        ExecuteEvents.Execute(nextObject, eventData, ExecuteEvents.pointerClickHandler);
                        break;
                    }

                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mOnPointerEnterHandler != null)
        {
            mOnPointerEnterHandler(gameObject, eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mOnPointerExitHandler != null)
        {
            mOnPointerExitHandler(gameObject, eventData);
        }
    }

    private void OnDestroy()
    {
        mClickedHandler = null;
        mDoubleClickedHandler = null;
        mOnPointerDownHandler = null;
        mOnPointerUpHandler = null;
        mOnPointerEnterHandler = null;
        mOnPointerExitHandler = null;
    }
}


