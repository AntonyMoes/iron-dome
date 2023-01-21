using _Game.Scripts.Fight;
using _Game.Scripts.Player;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Units {
    public abstract class Chassis : MonoBehaviour {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private Transform _weaponParent;

        private Weapon _weapon;
        private Rigidbody _player;

        protected UpdatedValue<Player.Player.SynchronizedState> State { get; private set; }

        protected virtual bool RotateCameraHorizontal => true;
        protected virtual bool RotateCameraVertical => true;

        protected Vector2 LookRotation => _cameraController.LookRotation;

        public void Setup(Weapon weaponPrefab, Rigidbody player, bool isPlayer, UpdatedValue<Player.Player.SynchronizedState> state) {
            _player = player;
            _cameraController.SetActive(isPlayer);

            _weapon = Instantiate(weaponPrefab, _weaponParent);
            State = state;

            PerformSetup();
        }

        protected virtual void PerformSetup() { }

        public void ApplyInputs(bool isServer, PlayerController.Inputs inputs) {
            // TODO: add a layer of indirection

            var rotation = _cameraController.CalculateRotation(inputs.LookInput);
            _cameraController.ApplyRotation(rotation, RotateCameraHorizontal, RotateCameraVertical);

            if (isServer) {
                PerformApplyInputs(_player, Time.deltaTime, inputs.MoveInput, rotation);
            }

            if (inputs.Fire) {
                _weapon.Fire(isServer);
            }
        }

        protected abstract void PerformApplyInputs(Rigidbody player, float deltaTime, Vector2 moveInput, Vector2 lookRotation);
    }
}