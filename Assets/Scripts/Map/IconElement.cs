using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ASimpleRoguelike.Map {
    public class IconElement : MonoBehaviour, IPointerClickHandler {
        public MapDisplay mapDisplay;
        public MapScene map;
        public IconData iconData;
        public Image frame;
        public Image icon;

        public bool used = false;
        public bool unusable = false;

        public void Init(MapScene mapScene, MapDisplay mapDisplay) {
            map = mapScene;
            iconData = map.icon;
            frame.sprite = iconData.frame;
            icon.sprite = iconData.icon;
            this.mapDisplay = mapDisplay;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!used && !unusable) {
                GlobalGameData.RemovePauseReason("Map");
                used = true;
                mapDisplay.StartMap(map);
                Cursor.visible = false;
            }
        }

        public void MakeUnusable() {
            unusable = true;
            icon.color = Color.red;
        }
    }
}