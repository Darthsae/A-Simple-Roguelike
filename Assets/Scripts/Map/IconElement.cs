using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ASimpleRoguelike.Map {
    public class IconElement : MonoBehaviour, IPointerClickHandler {
        public MapScene map;
        public IconData iconData;
        public Image frame;
        public Image icon;

        public void Init(MapScene mapScene) {
            map = mapScene;
            iconData = map.icon;
            frame.sprite = iconData.frame;
            icon.sprite = iconData.icon;
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log(map.name);
        }
    }
}