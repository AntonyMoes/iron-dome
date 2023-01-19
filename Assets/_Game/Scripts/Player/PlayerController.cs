using Unity.Netcode;
using UnityEngine;

namespace _Game.Scripts.Player {
    public class PlayerController : MonoBehaviour {
        public void Setup(bool isActive) {
            if (isActive) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public Inputs GetInputs() {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            var moveInput = new Vector2(horizontalInput, verticalInput);

            var horizontalMouseInput = Input.GetAxisRaw("Mouse X");
            var verticalMouseInput = Input.GetAxisRaw("Mouse Y");
            var lookInput = new Vector2(horizontalMouseInput, verticalMouseInput);

            var fire = Input.GetButtonDown("Fire1");

            return new Inputs {
                MoveInput = moveInput,
                LookInput = lookInput,
                Fire = fire
            };
        }

        public struct Inputs : INetworkSerializable {
            public Vector2 MoveInput;
            public Vector2 LookInput;
            public bool Fire;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
                serializer.SerializeValue(ref MoveInput);
                serializer.SerializeValue(ref LookInput);
                serializer.SerializeValue(ref Fire);
            }
        }
    }
}