using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class EquinoxUIHandler : MonoBehaviour {
        public GameObject content;
        public GameObject panelPrefab;
        public List<EquinoxUIPanelHandler> panels;

        void Start() {
            float y = 0;
            for (int i = 0; i < Equinox.EquinoxCount; i++) {
                if (GlobalGameData.unlockedEquinoxes[i]) {
                    GameObject panel = Instantiate(panelPrefab, content.transform);
                    panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);

                    EquinoxUIPanelHandler panelHandler = panel.GetComponent<EquinoxUIPanelHandler>();
                    panelHandler.Init(Equinox.equinoxes[i]);
                    panelHandler.Resize();
                    y -= panelHandler.GetComponent<RectTransform>().sizeDelta.y + 50;

                    panels.Add(panelHandler);
                }
            }
        }
    }
}