using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ASimpleRoguelike.Movement {
    public class MoveChoice : MonoBehaviour {
        public TMP_Dropdown dropdown;
        public Player player;
        public List<MovementController> movementControllers;

        // Start is called before the first frame update
        void Start()
        {
            dropdown.ClearOptions();
            for (int i = 0; i < movementControllers.Count; i++) {
                dropdown.options.Add(new TMP_Dropdown.OptionData(movementControllers[i].displayName));
            }
            if (dropdown.options.Count == 0) {
                gameObject.SetActive(false);
                return;
            }
            dropdown.onValueChanged.AddListener(HandleDropdownChange); // Add listener to the onValueChanged events
            HandleDropdownChange(0);
            dropdown.RefreshShownValue();
        }

        void HandleDropdownChange(int newIndex)
        {
            player.SetMovementController(movementControllers[newIndex], movementControllers[newIndex]);
        }
    }
}