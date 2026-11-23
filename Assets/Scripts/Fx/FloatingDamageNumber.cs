using System;
using UnityEngine;
using ArenaSurvivor.Core.Flyweight;
using ArenaSurvivor.Core.Pooling;

namespace ArenaSurvivor.Fx
{
    // Un número de daño flotante pooleado: sube, se desvanece y se libera solo 
    [RequireComponent(typeof(TextMesh))]
    public class FloatingDamageNumber : MonoBehaviour, IPoolable
    {
        private TextMesh _textMesh;
        private Action<FloatingDamageNumber> _releaseToPool;
        private DamageNumberStyle _style;
        private bool _flyweightAcquired;
        private float _timer;

        private void Awake()
        {
            _textMesh = GetComponent<TextMesh>();
        }

        public void Show(float amount, DamageNumberStyle style, Action<FloatingDamageNumber> releaseToPool)
        {
            _releaseToPool = releaseToPool;

            ReleaseFlyweight();
            _style = FlyweightFactory<DamageNumberStyle>.Acquire(style);
            _flyweightAcquired = _style != null;

            _textMesh.text = Mathf.CeilToInt(amount).ToString();
            _textMesh.color = _style.Color;
        }

        public void OnSpawn()
        {
            _timer = 0f;
        }

        public void OnDespawn()
        {
        }

        private void OnDisable()
        {
            ReleaseFlyweight();
        }

        private void ReleaseFlyweight()
        {
            if (!_flyweightAcquired)
            {
                return;
            }

            FlyweightFactory<DamageNumberStyle>.Release(_style);
            _flyweightAcquired = false;
        }

        private void Update()
        {
            if (_style == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            transform.position += Vector3.up * (_style.MoveSpeed * Time.deltaTime);

            var t = Mathf.Clamp01(_timer / _style.Lifetime);
            var fadedColor = _style.Color;
            fadedColor.a = 1f - t;
            _textMesh.color = fadedColor;

            if (_timer >= _style.Lifetime)
            {
                _releaseToPool?.Invoke(this);
            }
        }
    }
}
