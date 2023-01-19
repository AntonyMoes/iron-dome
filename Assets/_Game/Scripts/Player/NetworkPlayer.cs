using _Game.Scripts.Units;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class NetworkPlayer : NetworkBehaviour {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Rigidbody _rigidbody;

        private readonly NetworkVariable<Configuration> _configuration = new NetworkVariable<Configuration>(new Configuration());

        private readonly NetworkVariable<SynchronizedState> _state = new NetworkVariable<SynchronizedState>();

        private bool _isPlayer;
        private bool _configured;
        private Chassis _chassis;

        public NetworkPlayer() {
            _configuration.OnValueChanged += OnConfigurationSet;
        }

        public void Initialize(Configuration configuration) {
            if (!IsServer) {
                return;
            }

            _configuration.Value = configuration;
        }

        public override void OnNetworkSpawn() {
            if (_configuration.Value.Empty || _configured) {
                return;
            }

            OnConfigurationSet(null, _configuration.Value);
        }

        private void OnConfigurationSet(Configuration _, Configuration configuration) {
            _configured = true;

            _isPlayer = NetworkManager.LocalClientId == configuration.PlayerId;

            var provider = PrefabProvider.Instance;
            var chassisPrefab = provider.GetChassis(configuration.Chassis);
            _chassis = Instantiate(chassisPrefab, transform);

            var weaponPrefab = provider.GetWeapon(configuration.Weapon);
            _chassis.Setup(weaponPrefab, _rigidbody, _isPlayer, _state);

            _playerController.Setup(_isPlayer);
        }

        private void Update() {
            if (!_configured) {
                return;
            }

            if (IsClient && _isPlayer) {
                var inputs = _playerController.GetInputs();
                _chassis.ApplyInputs(false, inputs);
                ApplyInputsServerRPC(inputs);
            }
        }

        [ServerRpc]
        private void ApplyInputsServerRPC(PlayerController.Inputs inputs) {
            _chassis.ApplyInputs(true, inputs);
        }

        public class Configuration : INetworkSerializable {
            public bool Empty => PlayerId == 0 && string.IsNullOrEmpty(Chassis) && string.IsNullOrEmpty(Weapon);
            public ulong PlayerId;
            public string Chassis;
            public string Weapon;

            public Configuration() {
                PlayerId = 0;
                Chassis = "";
                Weapon = "";
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
                serializer.SerializeValue(ref PlayerId);
                serializer.SerializeValue(ref Chassis);
                serializer.SerializeValue(ref Weapon);
            }
        }
        
        public struct SynchronizedState : INetworkSerializable {
            public float Value1;
            public float Value2;
            public float Value3;
            public float Value4;
            public float Value5;
            public float Value6;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
                serializer.SerializeValue(ref Value1);
                serializer.SerializeValue(ref Value2);
                serializer.SerializeValue(ref Value3);
                serializer.SerializeValue(ref Value4);
                serializer.SerializeValue(ref Value5);
                serializer.SerializeValue(ref Value6);
            }
        }
    }
}