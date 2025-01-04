using System;
using UnityEngine;

namespace ASimpleRoguelike {
    public class Stamina : MonoBehaviour
    {
        public int maxStamina = 100;
        public int stamina;
        public event Action OnStaminaZero;
        public event Action OnStaminaChanged;
        public event Action OnStaminaMax;

        void Start()
        {
            stamina = maxStamina;
        }

        public void ChangeStamina(int stamina) {
            this.stamina += stamina;

            this.stamina = Math.Clamp(this.stamina, 0, maxStamina);

            OnStaminaChanged?.Invoke();

            if (this.stamina == 0) {
                OnStaminaZero?.Invoke();
            }
            else if (this.stamina == maxStamina) {
                OnStaminaMax?.Invoke();
            }
        }
    }
}