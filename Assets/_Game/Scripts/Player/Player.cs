using _Game.Scripts.Units;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class Player : MonoBehaviour {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Rigidbody _rigidbody;

        private IPlayerSynchronizer _synchronizer;
        private IPlayerSynchronizer Synchronizer => _synchronizer ??= GetComponent<IPlayerSynchronizer>();

        private bool _configured;
        private bool _isPlayer;
        private Chassis _chassis;

        private void Awake() {
            Synchronizer.Configuration.Subscribe(OnConfigurationSet);
            Synchronizer.OnSpawn.Subscribe(OnSpawn);
            Synchronizer.OnInputsReceived.Subscribe(OnInputsReceived);
        }

        public void Initialize(Configuration configuration) {
            if (!Synchronizer.Server) {
                return;
            }

            Synchronizer.Configuration.Value = configuration;
        }

        private void OnSpawn() {
            if ((Synchronizer.Configuration.Value?.Empty ?? true) || _configured) {
                return;
            }

            OnConfigurationSet(Synchronizer.Configuration.Value);
        }

        private void OnConfigurationSet(Configuration configuration) {
            _configured = true;

            _isPlayer = Synchronizer.IsPlayer(configuration.PlayerId);

            var provider = PrefabProvider.Instance;
            var chassisPrefab = provider.GetChassis(configuration.Chassis);
            _chassis = Instantiate(chassisPrefab, transform);

            var weaponPrefab = provider.GetWeapon(configuration.Weapon);
            _chassis.Setup(weaponPrefab, _rigidbody, _isPlayer, Synchronizer.State);

            _playerController.Setup(_isPlayer);
        }

        private void Update() {
            if (!_configured) {
                return;
            }

            if (Synchronizer.Client && _isPlayer) {
                var inputs = _playerController.GetInputs();
                _chassis.ApplyInputs(false, inputs);
                OnInputsReceived(inputs);
            }
        }

        private void OnInputsReceived(PlayerController.Inputs inputs) {
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
        
        public class SynchronizedState : INetworkSerializable {
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