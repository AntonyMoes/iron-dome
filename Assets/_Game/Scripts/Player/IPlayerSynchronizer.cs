using GeneralUtils;

namespace _Game.Scripts.Player {
    public interface IPlayerSynchronizer {
        public UpdatedValue<Player.Configuration> Configuration { get; }
        public UpdatedValue<Player.SynchronizedState> State { get; }
        
        public bool IsPlayer(ulong id);

        public bool Client { get; }
        public bool Server { get; }
        public Event OnSpawn { get; }

        public void SendInputs(PlayerController.Inputs inputs);
        public Event<PlayerController.Inputs> OnInputsReceived { get; }
    }
}