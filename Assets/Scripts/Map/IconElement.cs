using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ASimpleRoguelike.Map {
    public class IconElement : MonoBehaviour, IPointerClickHandler {
        public PhaseManager phaseManager;
        public MapScene map;
        public IconData iconData;
        public Image frame;
        public Image icon;

        public bool used = false;

        public void Init(MapScene mapScene, PhaseManager phaseManager) {
            map = mapScene;
            iconData = map.icon;
            frame.sprite = iconData.frame;
            icon.sprite = iconData.icon;
            this.phaseManager = phaseManager;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!used) {
                GlobalGameData.RemovePauseReason("Map");
                phaseManager.StartMap(map);
                Cursor.visible = false;
                used = true;
            }
        }
    }
}