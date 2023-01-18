using _Game.Scripts.Fight;
using _Game.Scripts.Network;
using _Game.Scripts.Units;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class NetworkPlayer : NetworkBehaviour, ISpawner {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Temp place for prefabs")]
        [SerializeField] private Weapon _weaponPrefab;
        [SerializeField] private Chassis _chassisPrefab;

        private readonly NetworkVariable<ulong> _playerId = new NetworkVariable<ulong>(ulong.MaxValue);

        private bool _idSet;
        private Chassis _chassis;

        public NetworkPlayer() {
            _playerId.OnValueChanged += OnPlayerIdSet;
        }

        public void Initialize(ulong id) {
            if (!IsServer) {
                return;
            }

            _playerId.Value = id;

            var chassis = Instantiate(_chassisPrefab);
            chassis.NetworkObject.Spawn();
            chassis.NetworkObject.TrySetParent(NetworkObject, false);
        }

        private void OnPlayerIdSet(ulong _, ulong __) {
            _idSet = true;

            TrySetup();
        }

        public void RegisterSpawned(SpawnedObject spawned) {
            if (spawned.TryGetComponent<Chassis>(out var chassis)) {
                _chassis = chassis;

                TrySetup();
            }
        }

        private void TrySetup() {
            if (!_idSet || _chassis == null) {
                return;
            }

            var isPlayer = _playerId.Value == NetworkObject.NetworkManager.LocalClientId;
            _chassis.Setup(_weaponPrefab, _rigidbody, isPlayer);
            _playerController.Setup(isPlayer, _chassis);
        }

        public override void OnNetworkDespawn() {
            if (IsServer && _chassis != null) {
                _chassis.NetworkObject.Despawn();
            }
        }
    }
}