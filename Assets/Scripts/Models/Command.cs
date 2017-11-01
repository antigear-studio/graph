using System;

namespace Antigear.Graph {
    /// <summary>
    /// Represents a discrete action user performs. This records a change in the
    /// graph state.
    /// </summary>
    [Serializable]
    public class Command {
        /// <summary>
        /// Type of action user committed.
        /// </summary>
        public enum Type {
            NoOp,
            CreateDrawable,
            UpdateDrawable,
            DeleteDrawable,
            CreateLayer,
            UpdateLayer,
            DeleteLayer,
        }

        public Type type;

        // Depending on the type above, the information below are different. For
        // layer commands, only layer attributes will be non-null. For drawable
        // commands, only drawable info and layer index will be non-null.
        public Drawable previousDrawable;
        public Drawable currentDrawable;

        public Layer previousLayer;
        public Layer currentLayer;

        // These will always be available.
        public int layerIndex = -1;
        public int drawableIndex = -1;
    }
}
