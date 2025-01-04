using TMPro;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class EquinoxUIPanelHandler : MonoBehaviour {
        public EquinoxData data;
        public TMP_Text title;
        public TMP_Text attunement;
        public TMP_Text description;

        public void Init(EquinoxData data) {
            this.data = data;
            title.text = data.name;
            attunement.text = data.attunement.ToString();
            description.text = data.description;
            Debug.Log(description.textInfo.lineCount);
        }

        public void Resize() {
            description.GetComponent<RectTransform>().sizeDelta = new Vector2(description.GetComponent<RectTransform>().sizeDelta.x, description.textInfo.lineCount * 30);
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 50 + description.textInfo.lineCount * 30);
        }
    }
}