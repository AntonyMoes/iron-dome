using System;
using GeneralUtils;
using Unity.Netcode;

namespace _Game.Scripts.Player {
    public class NetworkPlayerSynchronizer : NetworkBehaviour, IPlayerSynchronizer {
        private readonly NetworkVariable<Player.Configuration> _configuration =
            new NetworkVariable<Player.Configuration>();
        public UpdatedValue<Player.Configuration> Configuration { get; }

        private readonly NetworkVariable<Player.SynchronizedState> _state =
            new NetworkVariable<Player.SynchronizedState>();
        public UpdatedValue<Player.SynchronizedState> State { get; }

        public bool IsPlayer(ulong id) => id == NetworkObject.NetworkManager.LocalClientId;

        public bool Client => IsClient;
        public bool Server => IsServer;

        private readonly Action _onSpawn;
        public Event OnSpawn { get; }

        private readonly Action<PlayerController.Inputs> _onInputsReceived;
        public Event<PlayerController.Inputs> OnInputsReceived { get; }

        public NetworkPlayerSynchronizer() {
            OnSpawn = new Event(out _onSpawn);

            Configuration = new UpdatedValue<Player.Configuration>();
            Configuration.Subscribe(newValue => {
                if (_configuration.Value != newValue) {
                    _configuration.Value = newValue;
                }
            });
            _configuration.OnValueChanged = (_, newValue) => {
                if (Configuration.Value != newValue) {
                    Configuration.Value = newValue;
                }
            };

            State = new UpdatedValue<Player.SynchronizedState>();
            State.Subscribe(newValue => {
                if (_state.Value != newValue) {
                    _state.Value = newValue;
                }
            });
            _state.OnValueChanged = (_, newValue) => {
                if (State.Value != newValue) {
                    State.Value = newValue;
                }
            };
            State.Value = new Player.SynchronizedState();

            OnInputsReceived = new Event<PlayerController.Inputs>(out _onInputsReceived);
        }

        public override void OnNetworkSpawn() {
            _onSpawn();
        }

        public void SendInputs(PlayerController.Inputs inputs) {
            SendInputsServerRPC(inputs);
        }

        [ServerRpc]
        private void SendInputsServerRPC(PlayerController.Inputs inputs) {
            _onInputsReceived(inputs);
        }
    }
}