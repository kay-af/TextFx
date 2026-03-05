using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Controls the opacity of characters.
    /// </summary>
    [Serializable]
    public class OpacityModifier : BaseModifier
    {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The opacity to apply.")]
        private float _opacity = 1f;

        /// <summary>
        /// The opacity to apply.
        /// <para>
        /// The value must be in range [0,1]. The assigned value is clamped to this range.
        /// </para>
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set => _opacity = Mathf.Clamp01(value);
        }

        public override void Apply(ref ModificationContext context)
        {
            context.Opacity = Mathf.Clamp01(context.Strength * _opacity);
        }
    }
}