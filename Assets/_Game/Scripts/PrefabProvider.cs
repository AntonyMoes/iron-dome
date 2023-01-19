using System;
using System.Linq;
using _Game.Scripts.Fight;
using _Game.Scripts.Units;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts {
    public class PrefabProvider : SingletonBehaviour<PrefabProvider> {
        [SerializeField] private NamedPrefab<Chassis>[] _chassis;
        [SerializeField] private NamedPrefab<Weapon>[] _weapons;

        public Chassis GetChassis(string name) => _chassis.FirstOrDefault(np => np.name == name)?.prefab;
        public Weapon GetWeapon(string name) => _weapons.FirstOrDefault(np => np.name == name)?.prefab;

        [Serializable]
        private class NamedPrefab<T> where T : MonoBehaviour {
            public string name;
            public T prefab;
        }
    }
}