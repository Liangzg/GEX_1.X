using System.Collections.Generic;
using UnityEngine;

namespace LuaDataBind
{
    /// <summary>
    ///   Command which is triggered when the selected value of a popup list changed.
    /// </summary>
    [AddComponentMenu("Data Bind/Commands/[NGUI] Popup List Change Command")]
    public class PopupListChangeCommand : NguiEventCommand<UIPopupList>
    {
        /// <summary>
        ///   Storing popup value for comparison, the onChange event is triggered
        ///   if the same value is selected again, too.
        /// </summary>
        private string value;


        protected override List<EventDelegate> GetEvent(UIPopupList target)
        {
            return target.onChange;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.Target != null)
            {
                this.value = this.Target.value;
            }
        }

        protected override void OnEvent()
        {
            var newValue = this.Target.value;
            if (newValue == this.value)
            {
                return;
            }

            base.OnEvent();
            this.value = newValue;
        }
    }
}