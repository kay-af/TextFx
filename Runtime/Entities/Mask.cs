using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// A simple fractional mask that defines a range within the [0-1] space.
    /// <para>
    /// Used to determine which characters should be affected by a group based on their
    /// position within the text. Values are fractional [0-1] and does not depend on the actual width.
    /// </para>
    /// </summary>
    [Serializable]
    public class Mask
    {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Indicates the start of the mask.")]
        private float _start = 0f;

        /// <summary>
        /// Indicates the start of the mask.
        /// <para>
        /// The assigned value is clamped to the range [0,1].
        /// </para>
        /// </summary>
        public float Start
        {
            get => _start;
            set => _start = Mathf.Clamp01(value);
        }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Indicates the end of the mask.")]
        private float _end = 1f;

        /// <summary>
        /// Indicates the end of the mask.
        /// <para>
        /// The assigned value is clamped to the range [0,1].
        /// </para>
        /// </summary>
        public float End
        {
            get => _end;
            set => _end = Mathf.Clamp01(value);
        }
    }
}