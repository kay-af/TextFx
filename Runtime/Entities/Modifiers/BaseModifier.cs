using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// The base class of all modifiers responsible for applying an effect on a character.
    /// </summary>
    [Serializable]
    public abstract class BaseModifier
    {
        [SerializeField]
        [Tooltip("Is this modifier enabled?")]
        private bool _enabled;

        /// <summary>
        /// Is this modifier enabled?
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        [SerializeField]
        [Tooltip("Should the effect be inverted? (Strength is flipped respecting the group mask).")]
        private bool _invert;

        /// <summary>
        /// Should the effect be inverted? (Strength is flipped respecting the group mask).
        /// </summary>
        public bool Invert
        {
            get => _invert;
            set => _invert = value;
        }

        /// <summary>
        /// Updates the context values with the modifier applied.
        /// </summary>
        /// <param name="context">The context to update.</param>
        public abstract void Apply(ref ModificationContext context);
    }
}
