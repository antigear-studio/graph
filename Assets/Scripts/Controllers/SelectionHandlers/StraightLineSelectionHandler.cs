using I2.Loc;
using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Straight line selection handler.
    /// </summary>
    public class StraightLineSelectionHandler : LineSelectionHandler {
        protected override OptionData[] GetMenuItems() {
            OptionData[] data = {
                new OptionData(ScriptLocalization.Get("Selection/Edit"), null),
                new OptionData(ScriptLocalization.Get("Selection/Delete"), null)
            };

            return data;
        }
    }
}