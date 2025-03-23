using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class EquinoxSelectionUI : MonoBehaviour {
        public TMP_Text description;
        public TMP_Dropdown dropdown;
        public TMP_Text attunement;

        public List<int> info;

        // Start is called before the first frame update
        void Start() {
            dropdown.ClearOptions();
            for (int i = 0; i < Equinox.equinoxes.Count; i++) {
                if (!GlobalGameData.unlockedEquinoxes[i]) continue;
                dropdown.options.Add(new TMP_Dropdown.OptionData(Equinox.equinoxes[i].name, Equinox.equinoxes[i].sprite));
                info.Add(i);
            }
            if (dropdown.options.Count == 0) {
                gameObject.SetActive(false);
                return;
            }
            dropdown.onValueChanged.AddListener(HandleDropdownChange); // Add listener to the onValueChanged events
            
            if (EquinoxHandler.currentEquinox == -1) {
                HandleDropdownChange(0);
            } else {
                HandleDropdownChange(EquinoxHandler.currentEquinox);
            }

            dropdown.RefreshShownValue();
        }

        void HandleDropdownChange(int newIndex) {
            EquinoxHandler.currentEquinox = info[newIndex];
            description.text = Equinox.equinoxes[EquinoxHandler.currentEquinox].description;
            attunement.text = Equinox.equinoxes[EquinoxHandler.currentEquinox].attunement.ToString();
            //Debug.Log("Selected option index: " + newIndex + " Equinox: " + EquinoxHandler.currentEquinox);
        }
    }
}