using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antigear.Graph {
    public class GraphNavDrawer : NavigationDrawer {
        public ScrollRect scrollRect;

        bool hasCurrentDrag;

        void Start() {
            scrollRect.SetDragListener(this);


        }

        public override void OnBeginDrag(PointerEventData data) {
            hasCurrentDrag = Mathf.Abs(data.delta.x) > Mathf.Abs(data.delta.y);

            if (hasCurrentDrag) {
                base.OnBeginDrag(data);
                scrollRect.hasCurrentDrag = false;
            }
        }

        public override void OnDrag(PointerEventData data) {
            if (hasCurrentDrag)
                base.OnDrag(data);
        }


        public override void OnEndDrag(PointerEventData data) {
            if (hasCurrentDrag)
                base.OnEndDrag(data);
        }

    }
}