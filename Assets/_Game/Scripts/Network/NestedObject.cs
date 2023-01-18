using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Network {
    public class NestedObject : NetworkBehaviour {
        [SerializeField] private Transform _childPosition;
        public Vector3 ChildPosition => _childPosition.position;
    }
}