using System.Collections.Generic;

using UnityEngine;

namespace LuaDataBind
{
    /// <summary>
    ///   Command to invoke when a button was clicked.
    /// </summary>
    [AddComponentMenu("Data Bind/Commands/[NGUI] Button Click Command")]
    public class ButtonClickCommand : NguiEventCommand<UIButton>
    {

        protected override List<EventDelegate> GetEvent(UIButton target)
        {
            return target.onClick;
        }
    }
}