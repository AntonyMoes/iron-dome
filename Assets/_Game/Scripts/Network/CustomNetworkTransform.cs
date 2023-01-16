using Unity.Netcode.Components;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Unity.Netcode.Editor;
#endif

namespace _Game.Scripts.Network {
    public class CustomNetworkTransform : NetworkTransform {
        [SerializeField] private bool _serverAuthoritative;

        protected override bool OnIsServerAuthoritative() => _serverAuthoritative;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomNetworkTransform))]
    [CanEditMultipleObjects]
    class CustomNetworkTransformEditor : NetworkTransformEditor {
        public override void OnInspectorGUI() {
            var serverAuthoritative = serializedObject.FindProperty("_serverAuthoritative");

            EditorGUILayout.PropertyField(serverAuthoritative);
            base.OnInspectorGUI();
        }
    }
#endif
}