using System.Collections.Generic;
using UnityEngine;

namespace LuaDataBind
{
    /// <summary>
    ///   Command which is invoked when a onClick event occured on an event trigger.
    /// </summary>
    [AddComponentMenu("Data Bind/Commands/[NGUI] Event Trigger Click Command")]
    public class EventTriggerClickCommand : NguiEventCommand<UIEventTrigger>
    {

        protected override List<EventDelegate> GetEvent(UIEventTrigger target)
        {
            return target.onClick;
        }
        
    }
}