using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Fight {
    public class Projectile : NetworkBehaviour {
        [SerializeField] private float _damage;
        [SerializeField] private float _initialVelocity;
        [SerializeField] private Rigidbody _rigidbody;

        private const float DespawnAfter = 10f;
        private Coroutine _despawnCoroutine;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            _rigidbody.velocity = transform.forward * _initialVelocity;

            if (IsServer) {
                _despawnCoroutine = StartCoroutine(DespawnByTimer());
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (!IsServer) {
                return;
            }

            if (collision.body != null && collision.body.TryGetComponent<IDamageable>(out var damageable)) {
                damageable.ApplyDamage(_damage);
            }

            StopCoroutine(_despawnCoroutine);
            NetworkObject.Despawn();
        }

        private IEnumerator DespawnByTimer() {
            yield return new WaitForSeconds(DespawnAfter);
            NetworkObject.Despawn();
        }
    }
}