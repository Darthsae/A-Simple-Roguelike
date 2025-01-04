using System;
using TMPro;
using UnityEngine;

namespace ASimpleRoguelike {
    public class TimerHandler : MonoBehaviour
    {
        private int phases = 0;
        public double time = 0f;
        public float timeScale = 1f;

        public TMP_Text timer_text;
        public PhaseManager phaseManager;

        public int GetPhase() => phases;

        void Start()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().OnDie += () => {GlobalGameData.longestTime = Math.Max(GlobalGameData.longestTime, time); GlobalGameData.highestPhase = Math.Max(GlobalGameData.highestPhase, phases);};
        }

        void Update()
        {
            if (GlobalGameData.isPaused) return;
            
            time += Time.deltaTime * timeScale;
            Format();
            switch (phases) {
                case 0:
                    if (time > 360) {
                        phases = 1;
                        phaseManager.PhaseOne();
                    }
                    break;
                case 1:
                    if (time > 900) {
                        phases = 2;
                        phaseManager.PhaseTwo();
                    }
                    break;
                case 2:
                    if (time > 1500) {
                        phases = 3;
                        phaseManager.PhaseThree();
                    }
                    break;
            }
        }

        public void Format() {
            int days = (int)(time / 86400d);
            double remainingTime = time % 86400d;
            int hours = (int)(remainingTime / 3600d);
            remainingTime %= 3600d;
            int minutes = (int)(remainingTime / 60d);
            int seconds = (int)(remainingTime % 60d);

            // Using formatted string to avoid string concatenation overhead
            string text = $"<mspace=28px>{days:D3}</mspace>:<mspace=28px>{hours:D2}</mspace>:<mspace=28px>{minutes:D2}</mspace>:<mspace=28px>{seconds:D2}</mspace>";
            timer_text.text = text;
        }

    }
}