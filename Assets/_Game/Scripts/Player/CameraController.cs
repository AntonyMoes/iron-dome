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

        public Vector2 LookRotation =>
            new Vector2(_horizontalCameraTransform.localRotation.eulerAngles.y,
                -_verticalCameraTransform.localRotation.eulerAngles.z);

        public void SetActive(bool active) {
            _cameraTransform.gameObject.SetActive(active);
        }

        public Vector2 CalculateRotation(Vector2 lookInput) {
            var horizontalDelta = lookInput.x * _mouseSensitivity;
            // _horizontalCameraTransform.Rotate(Vector3.up, horizontalDelta);

            var verticalDelta = -lookInput.y * _mouseSensitivity;
            var currentRotation = _verticalCameraTransform.localRotation.eulerAngles;
            var currentVerticalRotation = currentRotation.z > 180f ? currentRotation.z - 360f : currentRotation.z;
            var newVerticalRotation = Mathf.Clamp(currentVerticalRotation + verticalDelta, _minVerticalAngle, _maxVerticalAngle);
            var clampedVerticalDelta = newVerticalRotation - currentVerticalRotation;
            // _verticalCameraTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newVerticalRotation);
            return new Vector2(horizontalDelta, clampedVerticalDelta);
        }

        public void ApplyRotation(Vector2 rotation, bool applyHorizontal = true, bool applyVertical = true) {
            if (applyHorizontal) {
                _horizontalCameraTransform.Rotate(Vector3.up, rotation.x);
            }

            if (applyVertical) {
                var currentRotation = _verticalCameraTransform.localRotation.eulerAngles;
                _verticalCameraTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y,
                    currentRotation.z + rotation.y);
            }
        }
    }
}