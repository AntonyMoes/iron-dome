using _Game.Scripts.Fight;
using _Game.Scripts.Network;
using _Game.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Units {
    public abstract class Chassis : NetworkBehaviour, ISpawner {
        [SerializeField] private CameraController _cameraController;
        protected CameraController CameraController => _cameraController;

        protected Weapon Weapon { get; private set; }
        private bool _isPlayer;
        private Rigidbody _player;
        private bool _spawned;

        public void Setup(Weapon weaponPrefab, Rigidbody player, bool isPlayer) {
            _player = player;
            _isPlayer = isPlayer;
            _cameraController.SetActive(isPlayer);

            if (!IsServer) {
                return;
            }

            var mount = SpawnWeaponMount();
            var weapon = Instantiate(weaponPrefab);
            weapon.transform.position = mount.ChildPosition;
            weapon.NetworkObject.Spawn();
            weapon.NetworkObject.TrySetParent(mount.NetworkObject);

            _spawned = true;
        }

        protected abstract NestedObject SpawnWeaponMount();

        public void ApplyInput(Vector2 moveInput, Vector2 lookInput, bool fire) {
            PerformApplyInput(_player, Time.deltaTime, moveInput, lookInput, fire);
        }

        protected abstract void PerformApplyInput(Rigidbody player, float deltaTime, Vector2 moveInput, Vector2 lookInput, bool fire);

        public override void OnNetworkDespawn() {
            if (IsServer && _spawned) {
                DespawnObjects();
            }
        }

        protected virtual void DespawnObjects() {
            Weapon.NetworkObject.Despawn();
        }

        public virtual void RegisterSpawned(SpawnedObject spawned) {
            if (spawned.TryGetComponent<Weapon>(out var weapon)) {
                Weapon = weapon;
            }
        }
    }
}