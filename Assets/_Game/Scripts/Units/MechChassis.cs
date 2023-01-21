using UnityEngine;

namespace _Game.Scripts.Units {
    public class MechChassis : Chassis {
        [Header("Settings")]
        [SerializeField] private float _speed;

        [Header("Joints")]
        [SerializeField] private Transform _legs;
        [SerializeField] private Transform _weaponMount;

        protected override bool RotateCameraHorizontal => false;

        private float LegsRotation {
            get => _legs.transform.localRotation.eulerAngles.y;
            set {
                var previousRotation = _legs.transform.localRotation.eulerAngles;
                _legs.transform.localRotation = Quaternion.Euler(previousRotation.x, value, previousRotation.z);
            }
        }

        private float MountRotation {
            get => _weaponMount.transform.localRotation.eulerAngles.x;
            set {
                var previousRotation = _weaponMount.transform.localRotation.eulerAngles;
                _weaponMount.transform.localRotation = Quaternion.Euler(value, previousRotation.y, previousRotation.z);
            }
        }

        protected override void PerformSetup() {
            State.Subscribe(state => {
                LegsRotation = state.Value1;
                MountRotation = state.Value2;
            });
        }

        protected override void PerformApplyInputs(Rigidbody player, float deltaTime, Vector2 moveInput, Vector2 lookRotation) {
            player.transform.Rotate(Vector3.up, lookRotation.x);

            ApplyMoveInput(player, moveInput, out var legsRotation);

            State.Value = new Player.Player.SynchronizedState {
                Value1 = legsRotation,
                Value2 = LookRotation.y
            };
        }

        private void ApplyMoveInput(Rigidbody player, Vector2 moveInput, out float legsRotation) {
            var pTransform = player.transform;
            var direction = pTransform.forward * moveInput.y + pTransform.right * moveInput.x;

            // TODO: something with mech controls
            // player.AddForce(direction * _speed * 0.001f, ForceMode.VelocityChange);
            player.velocity = direction * _speed;

            legsRotation = Vector3.SignedAngle(pTransform.forward, direction, pTransform.up);
        }
    }
}