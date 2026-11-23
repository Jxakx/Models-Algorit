using System.Collections;
using UnityEngine;
using ArenaSurvivor.Core.Combat;

namespace ArenaSurvivor.Fx
{
    /// Flashea el sprite en blanco un instante al recibir daño
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.08f;

        private Health _health;
        private SpriteRenderer _spriteRenderer;
        private float _lastKnownHealth;
        private Color _colorBeforeFlash;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _health.HealthChanged += HandleHealthChanged;
        }

        private void Start()
        {
            // Start corre después de TODOS los Awake de la escena
            _lastKnownHealth = _health.CurrentHealth;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= HandleHealthChanged;

            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                _flashRoutine = null;

                // El golpe que MATA dispara el flash y a continuación el pool desactiva al enemigo
                _spriteRenderer.color = _colorBeforeFlash;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (current < _lastKnownHealth)
            {
                if (_flashRoutine != null)
                {
                    StopCoroutine(_flashRoutine);
                    _spriteRenderer.color = _colorBeforeFlash;
                }

                _flashRoutine = StartCoroutine(FlashRoutine());
            }

            _lastKnownHealth = current;
        }

        private IEnumerator FlashRoutine()
        {
            // Se guarda el color justo antes de flashear
            _colorBeforeFlash = _spriteRenderer.color;
            _spriteRenderer.color = flashColor;

            yield return new WaitForSeconds(flashDuration);

            _spriteRenderer.color = _colorBeforeFlash;
            _flashRoutine = null;
        }
    }
}
