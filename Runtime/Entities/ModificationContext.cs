using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Context used internally to compute and pass modified character information
    /// across all groups and their modifiers sequentially.
    /// Each modifier applies its properties to this context, which accumulates
    /// the final transformation, color, and opacity for each character.
    /// </summary>
    public struct ModificationContext
    {
        /// <summary>
        /// The strength multiplier for the current modifier effect [0-1].
        /// </summary>
        public float Strength { get; set; }

        /// <summary>
        /// The pivot that should be used for rotation and scaling effects.
        /// </summary>
        public Vector3 Pivot { get; set; }

        /// <summary>
        /// The transformation computed so far.
        /// </summary>
        public Matrix4x4 Transformation { get; set; }

        /// <summary>
        /// The color computed so far.
        /// </summary>
        public Color32 Color { get; set; }

        /// <summary>
        /// The opacity computed so far.
        /// </summary>
        public float Opacity { get; set; }
    }
}
