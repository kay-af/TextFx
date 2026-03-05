using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// Controls the color of characters.
    /// </summary>
    [Serializable]
    public class ColorModifier : BaseModifier
    {
        [SerializeField]
        [Tooltip("The color to apply. The final color is computed as a linear interpolation between the original color and this color based on the strength.")]
        private Color32 _color = UnityEngine.Color.gray;

        /// <summary>
        /// The color to apply. The final color is computed as a linear interpolation between the original color and this color based on the strength.
        /// </summary>
        public Color32 Color
        {
            get => _color;
            set => _color = value;
        }

        public override void Apply(ref ModificationContext context)
        {
            context.Color = Color32.Lerp(context.Color, _color, context.Strength);
        }
    }
}