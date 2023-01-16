using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class NetworkPlayer : NetworkBehaviour {
        [SerializeField] private PlayerController _playerController;
        public readonly NetworkVariable<float> Health = new NetworkVariable<float>();

        private readonly Dictionary<string, Action<NetworkObject>> _callbacks = new Dictionary<string, Action<NetworkObject>>();

        public override void OnNetworkSpawn() {
            _playerController.Setup(IsOwner);
        }
    }
}