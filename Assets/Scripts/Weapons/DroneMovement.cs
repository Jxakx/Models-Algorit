using UnityEngine;

namespace ArenaSurvivor.Weapons
{
    /// <summary>
    /// Mantiene al dron siempre a la misma distancia del jugador (su "límite"), girando
    /// despacio para mirar hacia el enemigo que le indique la estrategia de targeting de
    /// su arma cuando hay uno cerca, y volviendo solo a su ángulo de reposo cuando no hay
    /// ninguno — nunca se aleja más del límite, solo cambia de qué lado del jugador se para.
    /// </summary>
    public class DroneMovement : MonoBehaviour
    {
        [SerializeField] private float leashRadius = 1.4f;
        [SerializeField] private float restAngleDegrees = 45f;
        [SerializeField] private float detectionRange = 7f;
        [SerializeField] private float turnSpeedDegreesPerSecond = 90f;
        [SerializeField] private float bobAmplitude = 0.1f;
        [SerializeField] private float bobSpeed = 2f;

        private Transform _player;
        private TargetedWeapon _owner;
        private float _currentAngleDegrees;

        /// <summary>El arma dueña del dron: de ella sale la estrategia que decide hacia quién mira.</summary>
        public void SetOwner(TargetedWeapon owner)
        {
            _owner = owner;
        }

        private void Awake()
        {
            _player = transform.parent;
            _currentAngleDegrees = restAngleDegrees;
        }

        private void Update()
        {
            var targetAngle = restAngleDegrees;
            var target = _owner != null ? _owner.FindTarget(_player.position, detectionRange) : null;

            if (target != null)
            {
                var toEnemy = (Vector2)target.transform.position - (Vector2)_player.position;
                targetAngle = Mathf.Atan2(toEnemy.y, toEnemy.x) * Mathf.Rad2Deg;
            }

            _currentAngleDegrees = Mathf.MoveTowardsAngle(_currentAngleDegrees, targetAngle,
                turnSpeedDegreesPerSecond * Time.deltaTime);

            var radians = _currentAngleDegrees * Mathf.Deg2Rad;
            var bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            var orbitOffset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * leashRadius;

            transform.localPosition = orbitOffset + Vector3.up * bob;
        }
    }
}
