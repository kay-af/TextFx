using System;

using UnityEngine;

namespace TextFx
{
    /// <summary>
    /// A special type of curve that starts rising from 0 to 1 following a profile (Rising profile),
    /// is 1 for a certain range, and then falls back to 0 following another profile (Falling profile).
    /// <para>
    /// The values used are fractional i.e, 0 is considered the starting point and 1, the ending point no matter
    /// the actual width of the range it is applied to.
    /// </para>
    /// </summary>
    [Serializable]
    public class PlateauCurve
    {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Fractional value [0-1] indicating when the curve should reach 1 starting from 0.")]
        private float _rise = 0.25f;

        /// <summary>
        /// Fractional value [0-1] indicating when the curve should reach 1 starting from 0.
        /// <para>
        /// The assigned value is clamped to the range [0,1].
        /// </para>
        /// </summary>
        public float Rise
        {
            get => _rise;
            set => _rise = Mathf.Clamp01(value);
        }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Fractional value [0-1] indicating when the curve should start falling from 1 to reach 0.")]
        private float _fall = 0.75f;

        /// <summary>
        /// Fractional value [0-1] indicating when the curve should start falling from 1 to reach 0.
        /// <para>
        /// The assigned value is clamped to the range [0,1].
        /// </para>
        /// </summary>
        public float Fall
        {
            get => _fall;
            set => _fall = Mathf.Clamp01(value);
        }

        [SerializeField]
        [Tooltip("Should the curve use separate rising and falling profiles?")]
        private bool _separateProfiles = false;

        /// <summary>
        /// Should the curve use separate rising and falling profiles?
        /// </summary>
        public bool SeparateProfiles
        {
            get => _separateProfiles;
            set => _separateProfiles = value;
        }

        [SerializeField]
        private AnimationCurve _risingProfile = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// Smoothly trasitions the rise using the curve.
        /// <para>
        /// Note: If separate profiles is disabled, this will be used as the falling profile as well where falling profile is evaluated right to left (x = 1 to x = 0).
        /// </para>
        /// </summary>
        public AnimationCurve RisingProfile
        {
            get => _risingProfile;
            set => _risingProfile = value ?? _risingProfile;
        }

        [SerializeField]
        [Tooltip("Smoothly transitions the fall using the curve. Evaluated right to left (x = 1 to x = 0).")]
        private AnimationCurve _fallingProfile = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// Smoothly transitions the fall using the curve. Evaluated right to left (x = 1 to x = 0).
        /// <para>
        /// Note: Only used if separateProfiles is true.
        /// </para>
        /// </summary>
        public AnimationCurve FallingProfile
        {
            get => _fallingProfile;
            set => _fallingProfile = value ?? _fallingProfile;
        }

        /// <summary>
        /// The effective falling profile based on separateProfiles value.
        /// </summary>
        private AnimationCurve EffectiveFallingProfile
        {
            get
            {
                if (_separateProfiles)
                {
                    return _fallingProfile;
                }

                return _risingProfile;
            }
        }

        /// <summary>
        /// Evaluates the curve at a given time.
        /// </summary>
        /// <param name="time">The time to evaluate the curve at.</param>
        /// <returns>The value of the curve at the given time.</returns>
        public float Evaluate(float time)
        {
            // If the time is out of the range [0,1], the value is 0.
            if (time < 0 || time > 1)
            {
                return 0f;
            }

            // Use the rising profile when time <= Rise.
            if (time <= Rise)
            {
                var factor = Mathf.InverseLerp(0, Rise, time);
                return _risingProfile.Evaluate(factor);
            }

            // Use the falling profile when time >= Fall.
            if (time >= Fall)
            {
                var factor = Mathf.InverseLerp(1, Fall, time);
                return EffectiveFallingProfile.Evaluate(factor);
            }

            // When time is between rise and fall, the value is 1.
            return 1f;
        }
    }
}