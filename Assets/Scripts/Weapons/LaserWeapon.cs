using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArenaSurvivor.Core.Events;
using ArenaSurvivor.Enemies;
using ArenaSurvivor.Fx;
using ArenaSurvivor.Weapons.HitEffects;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Haz perforante: dispara en línea recta hacia el enemigo que elija su estrategia de
    /// targeting y atraviesa a todos los que estén en el camino (no se detiene en el
    /// primero, a diferencia de la bala). Es instantáneo (hit-scan, sin proyectil viajando)
    /// — el LineRenderer es solo el feedback visual del disparo, no participa en la
    /// detección de impacto.
    /// </summary>
    public class LaserWeapon : TargetedWeapon
    {
        [SerializeField] private float range = 12f;

        [Tooltip("Ancho del pasillo alrededor de la línea de disparo que cuenta como 'en el haz'.")]
        [SerializeField] private float beamWidth = 0.6f;

        [SerializeField] private LineRenderer beamVisual;
        [SerializeField] private float beamVisualDuration = 0.15f;

        // Se juntan primero los enemigos del haz y recién después se les aplica el golpe:
        // un golpe que mata saca al enemigo de EnemyRegistry en el acto, y modificar la lista
        // mientras se la recorre salteaba enemigos del haz.
        private readonly List<Enemy> _beamHits = new List<Enemy>();
        private Coroutine _visualRoutine;

        protected override void Fire()
        {
            var origin = (Vector2)transform.position;
            var aimTarget = FindTarget(origin, range);
            if (aimTarget == null)
            {
                return;
            }

            var direction = ((Vector2)aimTarget.transform.position - origin).normalized;
            var damage = CurrentDamage;

            EventManager.Publish(new WeaponFiredEvent(Data.FireSound));

            CollectEnemiesInBeam(origin, direction);

            for (var i = 0; i < _beamHits.Count; i++)
            {
                var enemy = _beamHits[i];
                HitEffect.Apply(new HitContext(enemy, enemy.transform.position, direction, damage));
            }

            ShowBeam(origin, origin + direction * range);
        }

        private void CollectEnemiesInBeam(Vector2 origin, Vector2 direction)
        {
            _beamHits.Clear();
            var rangeSquared = range * range;

            var enemies = EnemyRegistry.ActiveEnemies;
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var toEnemy = (Vector2)enemy.transform.position - origin;
                var forwardDistance = Vector2.Dot(toEnemy, direction);

                if (forwardDistance < 0f || forwardDistance * forwardDistance > rangeSquared)
                {
                    continue;
                }

                var perpendicular = toEnemy - direction * forwardDistance;
                if (perpendicular.sqrMagnitude <= beamWidth * beamWidth)
                {
                    _beamHits.Add(enemy);
                }
            }
        }

        private void ShowBeam(Vector2 from, Vector2 to)
        {
            if (beamVisual == null)
            {
                return;
            }

            beamVisual.positionCount = 2;
            beamVisual.SetPosition(0, from);
            beamVisual.SetPosition(1, to);

            if (_visualRoutine != null)
            {
                StopCoroutine(_visualRoutine);
            }

            _visualRoutine = StartCoroutine(BeamVisualRoutine());
        }

        private IEnumerator BeamVisualRoutine()
        {
            beamVisual.enabled = true;
            yield return new WaitForSeconds(beamVisualDuration);
            beamVisual.enabled = false;
            _visualRoutine = null;
        }
    }
}
