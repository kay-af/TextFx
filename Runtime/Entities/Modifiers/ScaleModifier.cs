using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Controls the local scale of characters around the configured pivot.
    /// </summary>
    [Serializable]
    public class ScaleModifier : BaseModifier
    {
        [SerializeField]
        [Tooltip("The local scale to apply around the configured pivot.")]
        private Vector3 _scale = Vector3.one;

        /// <summary>
        /// The local scale to apply around the configured pivot.
        /// <para>
        /// The pivot is controlled by the group encapsulating this modifier.
        /// </para>
        /// </summary>
        public Vector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public override void Apply(ref ModificationContext context)
        {
            var effectiveScale = Vector3.Lerp(Vector3.one, _scale, context.Strength);

            context.Transformation =
                Matrix4x4.Translate(context.Pivot) *
                Matrix4x4.Scale(effectiveScale) *
                Matrix4x4.Translate(-context.Pivot) *
                context.Transformation;
        }
    }
}