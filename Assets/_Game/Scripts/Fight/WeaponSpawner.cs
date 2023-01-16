using System;
using GeneralUtils;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Fight {
    public class WeaponSpawner : NetworkBehaviour {
        [SerializeField] private NetworkObject _weaponPrefab;
        private NetworkObject _spawnedWeapon;

        private readonly Action<Weapon> _onWeaponSpawned;
        public readonly Event<Weapon> OnWeaponSpawned;

        public WeaponSpawner() {
            OnWeaponSpawned = new Event<Weapon>(out _onWeaponSpawned);
        }

        public override void OnNetworkSpawn() {
            if (!IsServer) {
                return;
            }

            _spawnedWeapon = Instantiate(_weaponPrefab);
            _spawnedWeapon.Spawn();
            _spawnedWeapon.TrySetParent(NetworkObject, false);
        }

        public void RegisterWeaponSpawned(Weapon weapon) => _onWeaponSpawned(weapon);
        
        public override void OnNetworkDespawn()
        {
            if (IsServer && _spawnedWeapon != null && _spawnedWeapon.IsSpawned)
            {
                _spawnedWeapon.Despawn();
            }
        }
    }
}