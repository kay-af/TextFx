using System;
using System.Collections.Generic;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// A group encapsulating characters and provides an interface to control various modifiers
    /// and settings related to the characters in the group.
    /// </summary>
    [Serializable]
    public class ModifierGroup
    {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The strength multiplier for the group.")]
        private float _strength = 1f;

        /// <summary>
        /// The strength multiplier for the group.
        /// <para>
        /// The value must be in range [0,1]. The assigned value is clamped to this range.
        /// </para>
        /// </summary>
        public float Strength
        {
            get => _strength;
            set => _strength = Mathf.Clamp01(value);
        }

        [SerializeField]
        [Tooltip("The fractional offset of the group.")]
        private float _offset = 0f;

        /// <summary>
        /// The fractional offset of the group.
        /// <para>
        /// The offset is the starting fractional position [0-1] of the range affected by the effects.
        /// Value of 0 corresponds to the left bound of the first contributing character.
        /// Value of1 corresponds to the right bound of the last contributing character.
        /// Paired with extent, it defines the range for which the group modifiers are applied.
        /// </para>
        /// </summary>
        public float Offset
        {
            get => _offset;
            set => _offset = value;
        }

        [SerializeField]
        [Min(0f)]
        [Tooltip("The fractional extent of the range starting at offset that is affected by the effects.")]
        private float _extent = 1f;

        /// <summary>
        /// The fractional extent of the range starting at offset that is affected by the effects.
        /// <para>
        /// Value of 1 corresponds to the sum of all the widths of the contributing character bounds.
        /// The assigned value must be non-negative.
        /// If the assigned value is negative, it will be transformed to 0.
        /// </para>
        /// </summary>
        public float Extent
        {
            get => _extent;
            set => _extent = Mathf.Max(0f, value);
        }

        [SerializeField]
        [Tooltip("Shifts the pivot of the chatacters affected by the group.")]
        private Vector3 _pivot = Vector3.zero;

        /// <summary>
        /// Shifts the pivot of the chatacters affected by the group.
        /// <para>
        /// Default pivot lies on the baseline at the center of each character.
        /// </para>
        /// </summary>
        public Vector3 Pivot
        {
            get => _pivot;
            set => _pivot = value;
        }

        [SerializeField]
        [Tooltip("The curve that defines the strength of the effects over the range.")]
        private PlateauCurve _curve = new();

        /// <summary>
        /// The curve that defines the strength of the effects over the range.
        /// </summary>
        public PlateauCurve Curve => _curve;

        [SerializeField]
        [Tooltip("Fractional mask controlling the characters affected in this group.")]
        private Mask _mask = new();

        /// <summary>
        /// Fractional mask controlling the characters affected in this group.
        /// </summary>
        public Mask Mask => _mask;

        #region Modifiers

        [SerializeField]
        [Tooltip("Controls the local translation of characters.")]
        private PositionModifier _positionModifier = new();

        /// <summary>
        /// Controls the local translation of characters.
        /// </summary>
        public PositionModifier PositionModifier => _positionModifier;

        [SerializeField]
        [Tooltip("Controls the local rotation of characters around the configured pivot.")]
        private RotationModifier _rotationModifier = new();

        /// <summary>
        /// Controls the local rotation of characters around the configured pivot.
        /// </summary>
        public RotationModifier RotationModifier => _rotationModifier;

        [SerializeField]
        [Tooltip("Controls the local scale of characters around the configured pivot.")]
        private ScaleModifier _scaleModifier = new();

        /// <summary>
        /// Controls the local scale of characters around the configured pivot.
        /// </summary>
        public ScaleModifier ScaleModifier => _scaleModifier;

        [SerializeField]
        [Tooltip("Controls the color of characters.")]
        private ColorModifier _colorModifier = new();

        /// <summary>
        /// Controls the color of characters.
        /// </summary>
        public ColorModifier ColorModifier => _colorModifier;

        [SerializeField]
        [Tooltip("Controls the opacity of characters.")]
        private OpacityModifier _opacityModifier = new();

        /// <summary>
        /// Controls the opacity of characters.
        /// </summary>
        public OpacityModifier OpacityModifier => _opacityModifier;

        // A cache to lazily handle ordered modifiers list creation.
        private List<BaseModifier> _orderedModifiersCache;

        /// <summary>
        /// The order in which the modifiers should be applied to make the transformations intuitive.
        /// </summary>
        public IReadOnlyList<BaseModifier> OrderedModifiers => _orderedModifiersCache ??= new()
        {
            _scaleModifier,
            _rotationModifier,
            _positionModifier,
            _colorModifier,
            _opacityModifier,
        };

        #endregion
    }
}