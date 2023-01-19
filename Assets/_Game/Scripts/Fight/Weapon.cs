using UnityEngine;

namespace _Game.Scripts.Fight {
    public class Weapon : MonoBehaviour {
        [SerializeField] private Projectile _projectile;
        [SerializeField] private Transform _spawnPoint;

        public void Fire(bool isServer) {
            if (isServer) {
                SpawnProjectile();
            }
        }

        private void SpawnProjectile() {
            var projectile = Instantiate(_projectile);
            projectile.transform.position = _spawnPoint.position;
            projectile.transform.forward = _spawnPoint.forward;
            projectile.NetworkObject.Spawn();
        }
    }
}