using System;
using GeneralUtils;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Player {
    public class LocalPlayerSynchronizer : MonoBehaviour, IPlayerSynchronizer {
        public UpdatedValue<Player.Configuration> Configuration { get; }
        public UpdatedValue<Player.SynchronizedState> State { get; }

        public bool IsPlayer(ulong id) => true;

        public bool Client => true;
        public bool Server => true;

        private readonly Action _onSpawn;
        public Event OnSpawn { get; }

        private readonly Action<PlayerController.Inputs> _onInputsReceived;
        public Event<PlayerController.Inputs> OnInputsReceived { get; }

        public LocalPlayerSynchronizer() {
            Configuration = new UpdatedValue<Player.Configuration>();
            State = new UpdatedValue<Player.SynchronizedState>(new Player.SynchronizedState());

            OnSpawn = new Event(out _onSpawn);

            OnInputsReceived = new Event<PlayerController.Inputs>(out _onInputsReceived);
        }

        private void Start() {
            _onSpawn();
        }

        public void SendInputs(PlayerController.Inputs inputs) {
            _onInputsReceived(inputs);
        }
    }
}