using Newtonsoft.Json;
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

        /// <summary>
        /// Returns a copy of this object.
        /// </summary>
        public virtual Layer Copy() {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.TypeNameHandling = TypeNameHandling.All;
            string output = JsonConvert.SerializeObject(this, settings);

            return (Layer)JsonConvert.DeserializeObject(output, settings);
        }
    }
}
