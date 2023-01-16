using _Game.Scripts.Fight;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class PlayerLogic : NetworkBehaviour {
        [SerializeField] private DamageProxy _damageProxy;
        [SerializeField] private float _initialHealth;

        private readonly NetworkVariable<float> _health = new NetworkVariable<float>();

        private void Awake() {
            _damageProxy.OnDamage.Subscribe(OnDamage);
        }

        public override void OnNetworkSpawn() {
            if (IsServer) {
                _health.Value = _initialHealth;
            }

            if (IsClient) {
                _health.OnValueChanged += OnHealthChanged;
            }
        }

        private void OnDamage(float damage) {
            if (IsServer) {
                _health.Value -= damage;
            }
        }

        private void OnHealthChanged(float previousvalue, float newvalue) {
            Debug.Log($"Health changed: {previousvalue} -> {newvalue}");
        }
    }
}