using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Maintains metadata for each component of the graph and provides common
    /// methods such as translation and rotation.
    /// </summary>
    [Serializable]
    public abstract class Drawable {
        public string name;

        /// <summary>
        /// The time this object was last selected in seconds since app is
        /// running.
        /// </summary>
        [JsonIgnore]
        public float timeLastSelected;

        /// <summary>
        /// Flag used at runtime to render view in a different manner when being
        /// selected.
        /// </summary>
        [JsonIgnore]
        public bool isSelected;

        /// <summary>
        /// Flag used at runtime to render view in a different manner when in
        /// editing mode.
        /// </summary>
        [JsonIgnore]
        public bool isEditing;

        public Vector2 rotationPivot;

        public virtual bool Selectible() {
            return Time.time - timeLastSelected > 
                PlayerPrefs.GetFloat(PlayerPrefKey.SelectionBlockTime);
        }

        /// <summary>
        /// Returns a copy of this object.
        /// </summary>
        public virtual Drawable Copy() {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.TypeNameHandling = TypeNameHandling.All;
            string output = JsonConvert.SerializeObject(this, settings);

            return (Drawable)JsonConvert.DeserializeObject(output, settings);
        }

        /// <summary>
        /// Offsets this drawable's position by the specified amount.
        /// </summary>
        /// <param name="amount">Amount.</param>
        public virtual void Offset(Vector2 amount) {}
    }
}
