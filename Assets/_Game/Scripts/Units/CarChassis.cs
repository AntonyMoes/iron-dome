using _Game.Scripts.Network;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Units {
    public class CarChassis : Chassis {
        [SerializeField] private NestedObject _turretPrefab;
        [SerializeField] private NestedObject _weaponMountPrefab;
        [SerializeField] private Transform _turretPosition;

        [Header("Wheels")]
        [SerializeField] private WheelCollider[] _wheels;
        [SerializeField] private WheelCollider[] _drivingWheels;
        [SerializeField] private WheelCollider[] _steeringWheels;

        [Header("Settings")]
        [SerializeField] private float _steerAngle;
        [SerializeField] private float _motorForce;
        [SerializeField] private float _breakForce;

        private readonly NetworkVariable<float> _turretRotation = new NetworkVariable<float>();
        private readonly NetworkVariable<float> _mountRotation = new NetworkVariable<float>();

        private NetworkObject _spawnedTurret;
        private NetworkObject _spawnedMount;

        private float TurretRotation {
            get => _spawnedTurret != null ? _spawnedTurret.transform.localRotation.eulerAngles.y : 0f;
            set {
                if (_spawnedTurret == null) {
                    return;
                }

                var previousRotation = _spawnedTurret.transform.localRotation.eulerAngles;
                _spawnedTurret.transform.localRotation = Quaternion.Euler(previousRotation.x, value, previousRotation.z);
            }
        }

        private float MountRotation {
            get => _spawnedMount != null ? _spawnedMount.transform.localRotation.eulerAngles.x : 0f;
            set {
                if (_spawnedMount == null) {
                    return;
                }

                var previousRotation = _spawnedMount.transform.localRotation.eulerAngles;
                _spawnedMount.transform.localRotation = Quaternion.Euler(value, previousRotation.y, previousRotation.z);
            }
        }

        public CarChassis() {
            _turretRotation.OnValueChanged += (_, value) => TurretRotation = value;
            _mountRotation.OnValueChanged += (_, value) => MountRotation = value;
        }

        protected override NestedObject SpawnWeaponMount() {
            var turret = Instantiate(_turretPrefab);
            var spawnedTurret = turret.NetworkObject;
            spawnedTurret.transform.position = _turretPosition.position;
            spawnedTurret.Spawn();
            spawnedTurret.TrySetParent(NetworkObject);

            var mount = Instantiate(_weaponMountPrefab);
            var spawnedMount = mount.NetworkObject;
            spawnedMount.transform.position = turret.ChildPosition;
            spawnedMount.Spawn();
            spawnedMount.TrySetParent(_spawnedTurret);

            return mount;
        }

        protected override void PerformApplyInput(Rigidbody player, float deltaTime, Vector2 moveInput, Vector2 lookInput, bool fire) {
            ApplyDrivingInput(moveInput);

            CameraController.ApplyInput(lookInput);
            var lookRotation = CameraController.LookRotation.eulerAngles;
            UpdateRotationsServerRPC(lookRotation.y, lookRotation.x);
            
            // Debug.Log($"Look rotation: {lookRotation}");

            if (fire && Weapon != null) {
                Weapon.Fire();
            }
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

        [ServerRpc(RequireOwnership = false)]
        private void UpdateRotationsServerRPC(float turretRotation, float mountRotation) {
            _turretRotation.Value = turretRotation;
            _mountRotation.Value = mountRotation;
        }

        public override void RegisterSpawned(SpawnedObject spawned) {
            if (spawned.Type == _turretPrefab.GetComponent<SpawnedObject>().Type) {
                _spawnedTurret = spawned.NetworkObject;
            } else if (spawned.Type == _weaponMountPrefab.GetComponent<SpawnedObject>().Type) {
                _spawnedMount = spawned.NetworkObject;
            }

            base.RegisterSpawned(spawned);
        }

        protected override void DespawnObjects() {
            _spawnedTurret.Despawn();
            _spawnedMount.Despawn();

            base.DespawnObjects();
        }
    }
}