using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace ASimpleRoguelike {
    public class VolumeController : MonoBehaviour
    {
        public AudioMixer mixerGroup;
        public string group;

        public void Start()
        {
            Slider slider = GetComponent<Slider>();
            slider.value = mixerGroup.GetFloat(group, out float value) ? Mathf.Pow(10, value / 20f) : 0;
        }

        public void SetLevel(float sliderValue) {
            if (sliderValue == 0)
            {
                mixerGroup.SetFloat(group, -100);
                return;
            }
            
            mixerGroup.SetFloat(group, Mathf.Log10(sliderValue) * 20);
        }
    }
}