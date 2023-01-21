using UnityEngine;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        [SerializeField] private Player.Player _clientPrefab;

        private void Start() {
            var player = Instantiate(_clientPrefab);
            player.Initialize(new Player.Player.Configuration {
                PlayerId = 0,
                Chassis = "testMech",
                Weapon = "testWeapon"
            });
        }
    }
}