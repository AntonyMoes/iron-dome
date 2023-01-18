using UnityEngine;

namespace _Game.Scripts.Player {
    public class CameraController : MonoBehaviour {
        [SerializeField] private Transform _horizontalCameraTransform;
        [SerializeField] private Transform _verticalCameraTransform;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _forward;

        [Header("Settings")]
        [SerializeField] private float _minVerticalAngle;
        [SerializeField] private float _maxVerticalAngle;
        [SerializeField] private float _mouseSensitivity;

        public Quaternion LookRotation =>
            Quaternion.Euler(-_verticalCameraTransform.localRotation.eulerAngles.z,
                _horizontalCameraTransform.localRotation.eulerAngles.y, 0);

        public void SetActive(bool active) {
            _cameraTransform.gameObject.SetActive(active);
        }

        public void ApplyInput(Vector2 lookInput) {
            var horizontalRotation = lookInput.x * _mouseSensitivity;
            _horizontalCameraTransform.Rotate(Vector3.up, horizontalRotation);

            var verticalRotation = -lookInput.y * _mouseSensitivity;
            var currentRotation = _verticalCameraTransform.localRotation.eulerAngles;
            var currentVerticalRotation = currentRotation.z > 180f ? currentRotation.z - 360f : currentRotation.z;
            var newVerticalRotation = Mathf.Clamp(currentVerticalRotation + verticalRotation, _minVerticalAngle, _maxVerticalAngle);
            _verticalCameraTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newVerticalRotation);
        }
    }
}