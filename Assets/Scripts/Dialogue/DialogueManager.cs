using ASimpleRoguelike.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ASimpleRoguelike.Dialogue {
    public class DialogueManager : MonoBehaviour {
        public static DialogueManager Instance;
        public GlobalGameData globalGameData;
        public DialogueScene activeScene;
        public float time;
        public int index = -1;
        public TMP_Text text;
        public Image portraitImage;

        public PackedDisplay display;
        public GameObject template;
        public GameObject content;

        public DialogueScene quick;
        public bool cacheMouseShow;
        public CursorLockMode cacheMouseLocked;
        public bool inside = false;

        void Awake() {
            Instance = this;
        }

        //void Start() {
        //    ChangeScene(quick);
        //}

        void Update() {
            if (index != -1) {
                time -= Time.deltaTime;
                if (time <= 0) {
                    Progress();
                } else {
                    ContinueLine();
                }
            }
        }

        void Progress() {
            EndLine();
            if (activeScene.lines.Count > ++index) {
                BeginLine();
            } else {
                activeScene.response.Call(globalGameData);
            }
        }

        void BeginLine() {
            if (!globalGameData.player.dialogueUI.activeInHierarchy) {
                globalGameData.player.dialogueUI.SetActive(true);
            }

            DialogueLine dialogueLine = activeScene.lines[index];

            display.Clear();
            for (int i = 0; i < activeScene.lines[index].responses.Count; i++) {
                int temp = index;
                IDialogueOption option = activeScene.lines[index].responses[i];
                GameObject gameObject = Instantiate(template, content.transform);
                display.AddItemNoCalculate(gameObject);
                gameObject.GetComponentInChildren<TMP_Text>().SetText(activeScene.lines[index].responses[i].name);
                gameObject.GetComponent<Button>().onClick.AddListener(() => { activeScene.lines[temp].responses[i].Call(globalGameData); });
            }
            display.Recalculate();

            dialogueLine.portrait.Apply(ref portraitImage, dialogueLine.context);

            if (dialogueLine.pauseMusic) {
                globalGameData.cameraController.PauseMusic();
            } else {
                globalGameData.cameraController.UnPauseMusic();
            }

            if (dialogueLine.pauses) {
                GlobalGameData.AddPauseReason("Dialogue");
                globalGameData.player.generalUI.SetActive(false);
            } else if (GlobalGameData.pauseReasons.Contains("Dialogue")) {
                GlobalGameData.RemovePauseReason("Dialogue");
                globalGameData.player.generalUI.SetActive(true);
            }

            time = activeScene.lines[index].responseTime + activeScene.lines[index].speechSpeed;

            // And here we apply the options.
        }

        void ContinueLine() {
            DialogueLine dialogueLine = activeScene.lines[index];

            if (time > dialogueLine.responseTime) {
                if (!dialogueLine.unskipable && Input.GetKeyDown(KeyCode.Space)) {
                    time = dialogueLine.responseTime;
                }
                text.SetText(dialogueLine.speechText[..(int)(uint)(dialogueLine.speechText.Length * (1.0 - (time - dialogueLine.responseTime) / dialogueLine.speechSpeed))]);
            } else {
                if (!dialogueLine.unskipable && Input.GetKeyDown(KeyCode.Space)) {
                    time = 0;
                }
                text.SetText(dialogueLine.speechText);
            }
        }

        void EndLine() {
            //DialogueLine dialogueLine = activeScene.lines[index];

            display.Clear();
        }

        public void ChangeScene(DialogueScene a_dialogueScene) {
            if (!inside) {
                inside = true;
                cacheMouseLocked = Cursor.lockState;
                cacheMouseShow = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            index = 0;
            activeScene = a_dialogueScene;
            BeginLine();
        }

        public void EndScene() {
            index = -1;
            display.Clear();
            if (globalGameData.cameraController.musicPaused) {
                globalGameData.cameraController.UnPauseMusic();
            }

            if (GlobalGameData.pauseReasons.Contains("Dialogue")) {
                GlobalGameData.RemovePauseReason("Dialogue");
                globalGameData.player.generalUI.SetActive(true);
            }

            globalGameData.player.dialogueUI.SetActive(false);
            inside = false;
            Cursor.lockState = cacheMouseLocked;
            Cursor.visible = cacheMouseShow;
        }
    }
}