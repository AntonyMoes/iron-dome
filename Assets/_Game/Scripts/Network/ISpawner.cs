using Unity.Netcode;

namespace _Game.Scripts.Network {
    public interface ISpawner {
        public void RegisterSpawned(SpawnedObject spawned);
    }
}