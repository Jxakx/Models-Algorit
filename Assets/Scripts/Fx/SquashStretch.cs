using System.Collections;
using UnityEngine;

namespace ArenaSurvivor.Fx
{
    /// <summary>
    /// "Pop" de aparición: crece desde 0 hasta su escala real pasándose un poco (efecto
    /// resorte). Se dispara solo al activarse (sirve tal cual para la gema de XP, cuya
    /// escala nunca cambia) o se puede llamar a Play(escala) a mano cuando la escala final
    /// se decide después de activarse (caso Enemy, que la fija en Initialize según su tipo).
    ///
    /// La escala "de reposo" se guarda en Awake (corre una sola vez en toda la vida del
    /// objeto pooleado) y no se vuelve a leer de transform.localScale en cada OnEnable: si
    /// un gema se recicla justo a mitad del pop (por ejemplo, la agarrás apenas aparece),
    /// releerla dejaba grabada esa escala a medio crecer como si fuera la "normal", y el
    /// pool la iba arrastrando cada vez más chica o más grande en usos sucesivos.
    /// </summary>
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
