using _Game.Scripts.Fight;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _cameraTransform;

        [Header("Settings")]
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _minVerticalAngle;
        [SerializeField] private float _maxVerticalAngle;
        [SerializeField] private float _mouseSensitivity;

        [Header("Combat test")]
        [SerializeField] private WeaponSpawner _weaponSpawner;

        private Vector3 _inputVelocity;
        private Weapon _weapon;

        private void Awake() {
            _weaponSpawner.OnWeaponSpawned.Subscribe(OnWeaponSpawned);
        }

        private void OnWeaponSpawned(Weapon weapon) {
            _weapon = weapon;
        }

        public void Setup(bool isActive) {
            gameObject.SetActive(isActive);
            Cursor.visible = !isActive;
            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Update() {
            var deltaTime = Time.deltaTime;

            UpdateInputs(deltaTime);
        }

        private void FixedUpdate() {
            var deltaTime = Time.fixedDeltaTime;

            UpdateMovement(deltaTime);
        }

        private void UpdateInputs(float deltaTime) {
            var currentDirection = Quaternion.FromToRotation(Vector3.forward, _transform.forward);
        
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            var movementSpeed = new Vector3(horizontalInput, 0f, verticalInput).normalized * _movementSpeed;
            var movementSpeedRotated = currentDirection * movementSpeed;

            _inputVelocity = movementSpeedRotated;

            var horizontalMouseInput = Input.GetAxisRaw("Mouse X");
            var verticalMouseInput = Input.GetAxisRaw("Mouse Y");

            var horizontalRotation = horizontalMouseInput * _mouseSensitivity;
            _transform.Rotate(Vector3.up, horizontalRotation);

            var verticalRotation = -verticalMouseInput * _mouseSensitivity;
            var currentRotation = _cameraTransform.rotation.eulerAngles;
            var currentVerticalRotation = currentRotation.x > 180f ? currentRotation.x - 360f : currentRotation.x;
            var newVerticalRotation = Mathf.Clamp(currentVerticalRotation + verticalRotation, _minVerticalAngle, _maxVerticalAngle);
            _cameraTransform.rotation = Quaternion.Euler(newVerticalRotation ,currentRotation.y, currentRotation.z);

            if (Input.GetButtonDown("Fire1") && _weapon != null) {
                _weapon.Fire();
            }
        }

        private void UpdateMovement(float deltaTime) {
            var currentVelocity = _rigidbody.velocity;
            var inputVelocity = _inputVelocity;
            inputVelocity.y = currentVelocity.y;
            _rigidbody.velocity = inputVelocity;
        }
    }
}