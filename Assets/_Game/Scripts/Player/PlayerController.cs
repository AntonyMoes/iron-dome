using _Game.Scripts.Units;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        private Chassis _chassis;
        private bool _isActive;

        public void Setup(bool isActive, Chassis chassis) {
            _isActive = isActive;
            Cursor.visible = !isActive;
            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;

            _chassis = chassis;
        }

        private void Update() {
            if (!_isActive) {
                return;
            }

            UpdateInputs();
        }

        private void UpdateInputs() {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            var moveInput = new Vector2(horizontalInput, verticalInput);

            var horizontalMouseInput = Input.GetAxisRaw("Mouse X");
            var verticalMouseInput = Input.GetAxisRaw("Mouse Y");
            var lookInput = new Vector2(horizontalMouseInput, verticalMouseInput);

            var fire = Input.GetButtonDown("Fire1");

            if (_chassis != null) {
                _chassis.ApplyInput(moveInput, lookInput, fire);
            }
        }
    }
}