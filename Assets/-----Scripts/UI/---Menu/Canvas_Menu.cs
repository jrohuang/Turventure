using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using LittleTurtle.System_;

namespace LittleTurtle.UI.Menu {
    public class Canvas_Menu : MonoBehaviour {

        public TMP_Text progress_Text;
        public Button LoadSave_Btn;

        [Header("Dialogue Panel")]
        public GameObject DialoguePanel;
        public TMP_Text DialogueText_Content, DialogueText_LeftButton, DialogueText_RightButton;

        //
        private MyGameManager myGameManager;
        private bool isLoadingScene;
        private Animator DialoguePanelAnimator;

        private void Start() {
            myGameManager = MyGameManager.Instance;
            isLoadingScene = false;
            DialoguePanelAnimator = DialoguePanel.GetComponent<Animator>();

            // check saved
            DataFileHandler fileHandler = new DataFileHandler("GameData");
            if (fileHandler.CheckFileExists()) {
                LoadSave_Btn.interactable = true;
            }
            else {
                LoadSave_Btn.interactable = false;
            }
        }

        #region = Button =
        public void _LoadGame() {
            myGameManager.LoadScene("Game", true);
        }
        public void _NewGame() {
            if (LoadSave_Btn.interactable) {
                DialoguePanel.SetActive(true);
                DialoguePanelAnimator.SetTrigger("Show");
                action_dialogueConfirm += LoadGameScene;
            }
            else {
                action_dialogueConfirm = null;
                myGameManager.LoadScene("Game", false);
            }
        }
        public void LoadGameScene() {
            myGameManager.LoadScene("Game", false);
        }
        public void _DialogueLeftButton() {
            DialoguePanelAnimator.SetTrigger("Hide");
            action_dialogueConfirm = null;
            CloseDialoguePanel();
        }
        public void _DialogueRightButton() {
            DialoguePanelAnimator.SetTrigger("Hide");
            CloseDialoguePanel();
        }
        private Action action_dialogueConfirm;
        private async void CloseDialoguePanel() {
            await System.Threading.Tasks.Task.Delay(100);
            DialoguePanel.SetActive(false);
            action_dialogueConfirm?.Invoke();
            action_dialogueConfirm = null;
        }

        public void _SwitchLanguage() {
            LanguageMgr.Instance.ChangeLanguage();
        }

        #endregion
    }
}
