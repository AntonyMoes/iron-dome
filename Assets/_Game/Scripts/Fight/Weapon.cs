using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Fight {
    public class Weapon : NetworkBehaviour {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _spawnPoint;

        public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {
            if (parentNetworkObject != null && parentNetworkObject.TryGetComponent<WeaponSpawner>(out var registry)) {
                registry.RegisterWeaponSpawned(this);
            }
        }

        public void Fire() {
            SpawnProjectileServerRPC();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnProjectileServerRPC() {
            var projectile = Instantiate(_projectile);
            projectile.transform.position = _spawnPoint.position;
            projectile.transform.forward = _spawnPoint.forward;
            projectile.NetworkObject.Spawn();
        }
    }
}