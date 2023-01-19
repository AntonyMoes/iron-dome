using UnityEngine;
using NetworkPlayer = _Game.Scripts.Player.NetworkPlayer;

namespace _Game.Scripts.Units {
    public class CarChassis : Chassis {
        [Header("Wheels")]
        [SerializeField] private WheelCollider[] _wheels;
        [SerializeField] private WheelCollider[] _drivingWheels;
        [SerializeField] private WheelCollider[] _steeringWheels;

        [Header("Settings")]
        [SerializeField] private float _steerAngle;
        [SerializeField] private float _motorForce;
        [SerializeField] private float _breakForce;

        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _weaponMount;

        private float TurretRotation {
            get => _turret.transform.localRotation.eulerAngles.y;
            set {
                var previousRotation = _turret.transform.localRotation.eulerAngles;
                _turret.transform.localRotation = Quaternion.Euler(previousRotation.x, value, previousRotation.z);
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
            State.OnValueChanged += (_, state) => {
                TurretRotation = state.Value1;
                MountRotation = state.Value2;
            };
        }

        protected override void PerformApplyInputs(Rigidbody player, float deltaTime, Vector2 moveInput, Quaternion lookRotation) {
            ApplyDrivingInput(moveInput);

            State.Value = new NetworkPlayer.SynchronizedState {
                Value1 = lookRotation.eulerAngles.y,
                Value2 = lookRotation.eulerAngles.x
            };
        }

        private void ApplyDrivingInput(Vector2 moveInput) {
            // TODO implement at least somewhat satisfying car controls; add breaking
            foreach (var drivingWheel in _drivingWheels) {
                drivingWheel.motorTorque = moveInput.y * _motorForce;
            }

            foreach (var steeringWheel in _steeringWheels) {
                steeringWheel.steerAngle = moveInput.x * _steerAngle;
            }
        }
    }
}