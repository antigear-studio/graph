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
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/Edit"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/Copy"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/Delete"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/BringToFront"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/SendToBack"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/SaveShape"), null),
                new OptionData(LocalizationManager
                    .GetTranslation("Selection/MakeFreeform"), null),
            };

            return data;
        }

        protected override void OnMenuSelect(int itemIndex) {
            base.OnMenuSelect(itemIndex);

            if (itemIndex == 0) {
                OnEdit();
            } else if (itemIndex == 1) {
                OnCopy();
            } else if (itemIndex == 2) {
                OnDelete();
            } else if (itemIndex == 3) {
                OnBringToFront();
            } else if (itemIndex == 4) {
                OnSendToBack();
            } else if (itemIndex == 5) {
                Debug.Log("TODO: Save shape.");
            } else if (itemIndex == 6) {
                Debug.Log("TODO: Make freeform.");

            }
        }
    }
}