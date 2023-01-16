﻿using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Game.Scripts {
    public class NetworkApp: MonoBehaviour {
        [SerializeField] private NetworkManager _netManager;
        [SerializeField] private NetworkObject _clientPrefab;

        private readonly Dictionary<ulong, NetworkObject> _clients = new Dictionary<ulong, NetworkObject>();

        public void Start() {
            _netManager.OnClientConnectedCallback += OnClientConnected;
            _netManager.OnClientDisconnectCallback += OnClientDisconnected;

            if (Application.isEditor) return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mode", out var mode)) {
                switch (mode)
                {
                    case "server":
                        _netManager.StartServer();
                        break;
                    case "host":
                        _netManager.StartHost();
                        break;
                    case "client":
                        _netManager.StartClient();
                        break;
                    default:
                        Debug.Log("INCORRECT MODE");
                        break;
                }
            } else {
                // _netManager.GetComponent<UnityTransport>().SetConnectionData();
                Debug.Log("default clint mode");
            }
        }

        private void OnClientConnected(ulong id) {
            Debug.Log($"Client connected: {id}");

            if (_netManager.IsServer) {
                var networkClient = Instantiate(_clientPrefab);
                networkClient.SpawnWithOwnership(id);
                _clients.Add(id, networkClient);
            }
        }

        private void OnClientDisconnected(ulong id) {
            Debug.Log($"Client disconnected: {id}");

            if (_netManager.IsServer) {
                _clients[id].Despawn();
                _clients.Remove(id);
            }
        }

        private static Dictionary<string, string> GetCommandlineArgs()
        {
            var argDictionary = new Dictionary<string, string>();
            var args = System.Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length; ++i) {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-")) {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = value?.StartsWith("-") ?? false ? null : value;

                    argDictionary.Add(arg, value);
                }
            }

            return argDictionary;
        }
    }
}