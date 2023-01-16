using System;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Fight {
    public class DamageProxy : MonoBehaviour, IDamageable {
        private readonly Action<float> _onDamage;
        public readonly Event<float> OnDamage;

        public DamageProxy() {
            OnDamage = new Event<float>(out _onDamage);
        }

        public void ApplyDamage(float damage) {
            _onDamage(damage);
        }
    }
}