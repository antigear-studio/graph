using System;
using System.Collections.Generic;

namespace Antigear.Graph {
    /// <summary>
    /// A layer in the graph, with additional attributes such as transparency,
    /// visibility, interactiveness, and so on.
    /// </summary>
    [Serializable]
    public class Layer : List<Drawable> {
        public float alpha = 1;
        public bool visible = true;
        public bool interactive = true;
        public string name = "Layer";
    }
}
