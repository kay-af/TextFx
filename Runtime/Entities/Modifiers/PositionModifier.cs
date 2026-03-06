using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Controls the local translation of characters.
    /// </summary>
    [Serializable]
    public class PositionModifier : BaseModifier
    {
        [SerializeField]
        [Tooltip("The local translation to apply.")]
        private Vector3 _position = new(0f, 0f, 0f);

        /// <summary>
        /// The local translation to apply.
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public override void Apply(ref ModificationContext context)
        {
            var effectivePosition = context.Strength * _position;

            context.Transformation = Matrix4x4.Translate(effectivePosition) * context.Transformation;

            // Translation shifts the pivot for future groups.
            context.Pivot += effectivePosition;
        }
    }
}