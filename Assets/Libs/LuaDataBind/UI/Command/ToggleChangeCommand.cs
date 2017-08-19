using System.Collections.Generic;
using UnityEngine;

namespace LuaDataBind
{
    /// <summary>
    ///   Command which is triggered when the selected value of a toggle changed.
    /// </summary>
    [AddComponentMenu("Data Bind/Commands/[NGUI] Toggle Change Command")]
    public class ToggleChangeCommand : NguiEventCommand<UIToggle>
    {
        /// <summary>
        ///   Storing toggle value for comparison, the onChange event is triggered
        ///   if the same value is selected again, too.
        /// </summary>
        private bool value;
        /// <summary>
        /// Only toggle value is true when the onChange event is triggered
        /// </summary>
        public bool EnableOnly = false;

        protected override List<EventDelegate> GetEvent(UIToggle target)
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

            this.value = newValue;
            if (EnableOnly && !newValue)
                return;

            base.OnEvent();
        }
    }
}