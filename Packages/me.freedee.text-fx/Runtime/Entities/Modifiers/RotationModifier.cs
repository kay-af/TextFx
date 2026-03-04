using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Controls the local rotation of characters around the configured pivot.
    /// </summary>
    [Serializable]
    public class RotationModifier : BaseModifier
    {
        [SerializeField]
        [Tooltip("The local euler rotation to apply around the configured pivot.")]
        private Vector3 _rotation = Vector3.zero;

        /// <summary>
        /// The local euler rotation to apply around the configured pivot.
        /// <para>
        /// The pivot is controlled by the group encapsulating this modifier.
        /// </para>
        /// </summary>
        public Vector3 Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public override void Apply(ref ModificationContext context)
        {
            var effectiveRotation = Quaternion.Euler(context.Strength * _rotation);

            // Rotate around the pivot.
            context.Transformation =
                Matrix4x4.Translate(context.Pivot) *
                Matrix4x4.Rotate(effectiveRotation) *
                Matrix4x4.Translate(-context.Pivot) *
                context.Transformation;
        }
    }
}