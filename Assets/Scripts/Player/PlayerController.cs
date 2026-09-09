using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaSurvivor.Player
{
    /// <summary>
    /// Responsable únicamente del movimiento del jugador: lee el input y mueve el Rigidbody2D.
    /// No sabe nada de vida, armas ni nivel — esos viven en sus propios componentes (SRP).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D _rigidbody;
        private PlayerStats _stats;
        private InputSystem_Actions _inputActions;
        private Vector2 _moveInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _stats = GetComponent<PlayerStats>();
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            // Defensivo: si Unity recompila mientras el juego está en Play, hace un domain
            // reload que resetea campos no serializados como este sin volver a llamar a
            // Awake() — sin este chequeo, _inputActions quedaba null y tiraba
            // NullReferenceException acá.
            _inputActions ??= new InputSystem_Actions();

            _inputActions.Player.Enable();
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            if (_inputActions == null)
            {
                return;
            }

            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
            var speedMultiplier = _stats != null ? _stats.MoveSpeedMultiplier : 1f;
            _rigidbody.linearVelocity = _moveInput * moveSpeed * speedMultiplier;
        }
    }
}
