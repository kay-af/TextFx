using System.Collections;

using UnityEngine;

namespace TextFx.Samples
{
    [RequireComponent(typeof(TextFxController))]
    public class TextFxWave : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        [Tooltip("Time to wait before starting / looping the animation.")]
        private float _delay = 1f;

        [SerializeField]
        [Min(0.01f)]
        [Tooltip("Time taken to complete the animation.")]
        private float _time = 1f;

        private TextFxController _controller;

        private void Awake() => _controller = GetComponent<TextFxController>();

        private IEnumerator Start()
        {
            _controller.enabled = true;

            var waitForDelaySeconds = new WaitForSeconds(_delay);

            while (true)
            {
                yield return waitForDelaySeconds;

                float elapsed = 0f;
                while (elapsed < _time)
                {
                    var t = elapsed / _time;

                    _controller.Offset = Mathf.Lerp(-1f, 1f, t);
                    _controller.UpdateMesh();

                    elapsed += Time.deltaTime;

                    yield return null;
                }
            }
        }
    }
}