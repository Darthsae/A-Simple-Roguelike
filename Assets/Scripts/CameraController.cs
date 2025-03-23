using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace ASimpleRoguelike{
    public class CameraController : MonoBehaviour {
        [Tooltip("The target object to follow")]
        public Transform target;
        [Tooltip("The smoothness factor of the camera movement")]
        public float smoothSpeed = 0.125f;
        [Tooltip("The offset of the camera from the target object")]
        public Vector3 offset;
        [Tooltip("The z-position of the camera in the cutscene")]
        public float cutsceneZPosition = -10f;
        [Tooltip("The list of cutscenes")]
        public CameraCutscene[] cutscenes;
        [Tooltip("The quad to play the cutscene on")]
        public GameObject cutsceneQuad;
        [Tooltip("In cutscene")]
        public static bool inCutscene = false;
        [Tooltip("UI GameObject")]
        public GameObject ui;
        [Tooltip("Current Boss Notification")]
        public BossNotification bossNotification;
        [Tooltip("Boss Health Bar")]
        public GameObject bossHealthBar;
        public RectTransform bossHealthBarMask;
        [Tooltip("Music Audio Source")]
        public AudioSource musicAudioSourceCurrent;
        public AudioSource musicAudioSourceNew;
        public bool onCurrent = true;
        public float fadeTime = 2f;
        private Action callbackHolder;
        [Tooltip("Music Tracks")]
        public MusicTrack[] musicTracks;
        [Tooltip("Current Music Track")]
        public int currentMusicTrack = 0;

        public bool snap = false;

        void Start() {
            StartMusic(0, null);
        }

        public void SetSnap(bool snapNew) {
            snap = snapNew;
        }

        private void FixedUpdate() {
            if (inCutscene) {
                transform.position = new Vector3(transform.position.x, transform.position.y, cutsceneZPosition);
                return;
            }
            
            if (target != null) {
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = snap ? desiredPosition : Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }

        #region Cutscenes
        public void StartCutscene(int cutsceneIndex, Action callback) {
            StartCoroutine(CutsceneCoroutine(cutsceneIndex, callback));
        }

        public void StartCutscene(string cutsceneName, Action callback) {
            for (int i = 0; i < cutscenes.Length; i++) {
                if (cutscenes[i].name == cutsceneName) {
                    StartCoroutine(CutsceneCoroutine(i, callback));
                    return;
                }
            }

            Debug.LogError("Cutscene not found: " + cutsceneName);
        }

        IEnumerator CutsceneCoroutine(int cutsceneIndex, Action callback) {
            inCutscene = true;
            GlobalGameData.AddPauseReason("Cutscene");
            cutsceneQuad.SetActive(true);
            cutsceneQuad.GetComponent<VideoPlayer>().clip = cutscenes[cutsceneIndex].videoClip;
            cutsceneQuad.GetComponent<VideoPlayer>().Play();
            ui.SetActive(false);
            yield return new WaitForSeconds((float)cutscenes[cutsceneIndex].Duration);
            ui.SetActive(true);
            cutsceneQuad.SetActive(false);
            GlobalGameData.RemovePauseReason("Cutscene");
            inCutscene = false;
            callback?.Invoke();
        }
        #endregion

        #region Music
        public void StartMusic(int musicIndex, Action callback) {
            StartCoroutine(MusicCoroutine(musicIndex, callback));
        }

        public void StartMusic(string musicName, Action callback) {
            for (int i = 0; i < musicTracks.Length; i++) {
                if (musicTracks[i].name == musicName) {
                    StartCoroutine(MusicCoroutine(i, callback));
                    return;
                }
            }

            Debug.LogError("Cutscene not found: " + musicName);
        }

        public IEnumerator MusicCoroutine(int musicIndex, Action callback) {
            switch (onCurrent) {
                case true:
                    musicAudioSourceNew.clip = musicTracks[musicIndex].audioClip;
                    musicAudioSourceNew.Play();
                    break;
                case false:
                    musicAudioSourceCurrent.clip = musicTracks[musicIndex].audioClip;
                    musicAudioSourceCurrent.Play();
                    break;
            }

            float fadeProgress = 0f;

            while (fadeProgress < 1f) {
                fadeProgress += Time.deltaTime / fadeTime;

                switch (onCurrent) {
                    case true:
                        musicAudioSourceCurrent.volume = Mathf.Lerp(1f, 0f, fadeProgress) * musicTracks[currentMusicTrack].idealVolume;
                        musicAudioSourceNew.volume = Mathf.Lerp(0f, 1f, fadeProgress) * musicTracks[musicIndex].idealVolume;
                        break;
                    case false:
                        musicAudioSourceCurrent.volume = Mathf.Lerp(0f, 1f, fadeProgress) * musicTracks[musicIndex].idealVolume;
                        musicAudioSourceNew.volume = Mathf.Lerp(1f, 0f, fadeProgress) * musicTracks[currentMusicTrack].idealVolume;
                        break;
                }

                //Debug.Log(fadeProgress);

                yield return null;
            }

            switch (onCurrent) {
                case true:
                    musicAudioSourceCurrent.volume = 0f;
                    musicAudioSourceCurrent.Stop();
                    musicAudioSourceNew.volume = musicTracks[musicIndex].idealVolume;
                    break;
                case false:
                    musicAudioSourceCurrent.volume = musicTracks[musicIndex].idealVolume;
                    musicAudioSourceNew.volume = 0f;
                    musicAudioSourceNew.Stop();
                    break;
            }

            onCurrent = !onCurrent;
            currentMusicTrack = musicIndex;

            callbackHolder?.Invoke();
            callbackHolder = callback;
        }
        #endregion


        public void SetBoss() {
            bossHealthBar.SetActive(true);
            bossNotification.health.OnHealthChanged += (int healthChange) => { 
                float percent = (float)bossNotification.health.health / bossNotification.health.maxHealth;
                bossHealthBarMask.sizeDelta = new Vector2(bossHealthBarMask.sizeDelta.x, percent * 980); 
            };
            float percent = (float)bossNotification.health.health / bossNotification.health.maxHealth;
            bossHealthBarMask.sizeDelta = new Vector2(bossHealthBarMask.sizeDelta.x, percent * 980);
            //Debug.Log(bossNotification.bossName + " is the boss!");
            //Debug.Log("The boss theme is " + bossNotification.bossTheme + ".");
            StartMusic(bossNotification.bossTheme, null);
        }

        public void BossKilled(string newTrack) {
            //Debug.Log(bossNotification.bossName + " was killed!");
            bossHealthBar.SetActive(false);
            StartMusic(newTrack, null);
        }
    }

    [Serializable]
    public class CameraCutscene {
        public string name;
        public VideoClip videoClip;
        public double Duration => videoClip.length;
    }

    [Serializable]
    public class MusicTrack {
        public string name;
        public AudioClip audioClip;
        public double Duration => audioClip.length;
        public float idealVolume = 1f;
    }
}