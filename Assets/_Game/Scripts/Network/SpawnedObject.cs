using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Network {
    public class SpawnedObject : NetworkBehaviour {
        [SerializeField] private string _type;
        public string Type => _type;

        public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) {
            if (parentNetworkObject == null) {
                return;
            }

            var spawners = parentNetworkObject.GetComponentsInParent<ISpawner>();
            foreach (var spawner in spawners) {
                spawner.RegisterSpawned(this);
            }
        }
    }
}