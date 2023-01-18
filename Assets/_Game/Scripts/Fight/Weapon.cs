using _Game.Scripts.Units;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Fight {
    public class Weapon : NetworkBehaviour {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _spawnPoint;

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