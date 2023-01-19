using _Game.Scripts.Fight;
using _Game.Scripts.Player;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = _Game.Scripts.Player.NetworkPlayer;

namespace _Game.Scripts.Units {
    public abstract class Chassis : MonoBehaviour {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Transform _weaponParent;

        private Weapon _weapon;
        private bool _isPlayer;
        private Rigidbody _player;

        protected NetworkVariable<NetworkPlayer.SynchronizedState> State { get; private set; }

        public void Setup(Weapon weaponPrefab, Rigidbody player, bool isPlayer, NetworkVariable<NetworkPlayer.SynchronizedState> state) {
            _player = player;
            _isPlayer = isPlayer;
            _cameraController.SetActive(isPlayer);

            _weapon = Instantiate(weaponPrefab, _weaponParent);
            State = state;

            PerformSetup();
        }

        protected virtual void PerformSetup() { }

        public void ApplyInputs(bool isServer, PlayerController.Inputs inputs) {
            // TODO: add layer of indirection
            _cameraController.ApplyInput(inputs.LookInput);

            if (isServer) {
                PerformApplyInputs(_player, Time.deltaTime, inputs.MoveInput, _cameraController.LookRotation);
            }

            if (inputs.Fire) {
                _weapon.Fire(isServer);
            }
        }

        protected abstract void PerformApplyInputs(Rigidbody player, float deltaTime, Vector2 moveInput, Quaternion lookRotation);
    }
}