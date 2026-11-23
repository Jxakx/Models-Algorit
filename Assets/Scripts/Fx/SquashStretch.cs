using System.Collections;
using UnityEngine;

namespace ArenaSurvivor.Fx
{
    public class SquashStretch : MonoBehaviour
    {
        [SerializeField] private float duration = 0.18f;

        private Vector3 _restingScale;
        private Coroutine _routine;

        private void Awake()
        {
            _restingScale = transform.localScale;
        }

        private void OnEnable()
        {
            Play(_restingScale);
        }

        public void Play(Vector3 targetScale)
        {
            _restingScale = targetScale;

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(PopRoutine(targetScale));
        }

        private IEnumerator PopRoutine(Vector3 targetScale)
        {
            transform.localScale = Vector3.zero;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                transform.localScale = targetScale * EaseOutBack(t);
                yield return null;
            }

            transform.localScale = targetScale;
            _routine = null;
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            var x = t - 1f;
            return 1f + c3 * x * x * x + c1 * x * x;
        }
    }
}
