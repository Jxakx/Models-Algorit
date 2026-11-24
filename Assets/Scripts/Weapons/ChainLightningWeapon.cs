using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Fx;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    // Cadena eléctrica: pega en el enemigo que elija su estrategia de targeting y salta al próximo dentro de un radio corto
    public class ChainLightningWeapon : TargetedWeapon
    {
        [SerializeField] private float range = 9f;
        [SerializeField] private int maxChainJumps = 3;
        [SerializeField] private float chainRange = 4f;
        [SerializeField] private LineRenderer chainVisual;
        [SerializeField] private float visualDuration = 0.2f;

        private readonly List<Enemy> _hitEnemies = new List<Enemy>();
        private readonly List<Vector3> _visualPoints = new List<Vector3>();
        private Coroutine _visualRoutine;

        protected override void Fire()
        {
            var first = FindTarget(transform.position, range);
            if (first == null)
            {
                return;
            }

            EventManager.Publish(new WeaponFiredEvent(Data.FireSound));

            _hitEnemies.Clear();
            _visualPoints.Clear();
            _visualPoints.Add(transform.position);

            var damage = CurrentDamage;
            var current = first;

            for (var jump = 0; jump < maxChainJumps && current != null; jump++)
            {
                _hitEnemies.Add(current);

                var hitPosition = (Vector2)current.transform.position;
                _visualPoints.Add(hitPosition);

                var direction = (hitPosition - (Vector2)transform.position).normalized;
                HitEffect.Apply(new HitContext(current, hitPosition, direction, damage));

                current = FindTarget(hitPosition, chainRange, _hitEnemies);
            }

            ShowVisual();
        }

        private void ShowVisual()
        {
            if (chainVisual == null)
            {
                return;
            }

            chainVisual.positionCount = _visualPoints.Count;
            chainVisual.SetPositions(_visualPoints.ToArray());

            if (_visualRoutine != null)
            {
                StopCoroutine(_visualRoutine);
            }

            _visualRoutine = StartCoroutine(VisualRoutine());
        }

        private IEnumerator VisualRoutine()
        {
            chainVisual.enabled = true;
            yield return new WaitForSeconds(visualDuration);
            chainVisual.enabled = false;
            _visualRoutine = null;
        }
    }
}
