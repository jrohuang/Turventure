using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor;
using LittleTurtle.System_;
using LittleTurtle.Inventory;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

namespace LittleTurtle.UI {

    public class Canvas_Game : MonoBehaviour {

        public static Canvas_Game Instance;

        #region = Parameters =

        [Header("Instances")]
        public MonsterDataPanel monsterDataPanel;
        public ItemInfoPanel itemInfoPanel;
        public Bag bag;
        public TurtleInfoPanel turtleInfoPanel;

        [Header("Joy stick")]
        public RectTransform RT_AttackJoyStick;
        public Joystick_Movement joystick_Movement;
        public Joystick_Attack joystick_Attack;

        [Header("HP bar")]
        public Image healthBar;
        public TMP_Text healthBarText;

        [Header("Other")]
        public RectTransform RT_SettingButton;
        public GameObject settingPanel;
        public RectTransform RT_settingPanel;
        public Image turtleDirArrowIcon;
        public Image BlackPanel;

        [Header("Activate before game start")]
        public GameObject[] gameObjectsNeedToActivate;

        private MyGameManager mGM;
        private static PlayerCtrl playerCtrl;
        private bool DetectingTouch = false;
        private int touchFingerID;
        private CurveLerping _lerping;
        #endregion

        private void Awake() {
            if (Instance == null) Instance = this;
            BlackPanel.gameObject.SetActive(true);
        }

        private void Start() {
            mGM = MyGameManager.Instance;

            _lerping = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping.SetAdjustVaule(healthBar);

            playerCtrl = PlayerCtrl.Instance;
            playerCtrl.InitizlizePlayerHealth();

            foreach (var item in gameObjectsNeedToActivate) {
                item.SetActive(true);
                item.SetActive(false);
            }

            BlackPanelBrighten();
        }

        private async void BlackPanelBrighten() {
            while (BlackPanel.color.a > 0) {
                BlackPanel.color = new Color(0, 0, 0, BlackPanel.color.a - 0.03f);
                await Task.Delay(10);
            }
            BlackPanel.gameObject.SetActive(false);
        }

        private void Update() {

            #region == JoySticks ==

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {

                if (Input.GetMouseButtonDown(0)) {
                    Vector2 pos = Input.mousePosition;

                    // move => if the pointer is down within the range
                    if (RectTransformUtility.RectangleContainsScreenPoint(joystick_Movement.ShiftRange, pos)) {
                        if (!EventSystem.current.IsPointerOverGameObject()) {
                            joystick_Movement.PointerDown(false);
                        }
                    }

                    //attack
                    else if (RectTransformUtility.RectangleContainsScreenPoint(joystick_Attack.ShiftRange, pos)) {
                        if (!EventSystem.current.IsPointerOverGameObject()) {
                            if (!joystick_Attack.isCooldowning & !RectTransformUtility.RectangleContainsScreenPoint(RT_AttackJoyStick, pos)) {
                                joystick_Attack.PointerDown(false);
                            }
                            else {
                                joystick_Attack.PointerDown(true);
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0)) {
                    if (joystick_Movement.IsDetectingJoystick) {
                        joystick_Movement.PointerUp();
                    }
                    // attack
                    if (joystick_Attack.IsDetectingJoystick) {
                        joystick_Attack.PointerUp();
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                // the same logic as the editor platform
                if (Input.touchCount > 0) {
                    foreach (var t in Input.touches) {
                        if (t.phase == TouchPhase.Began) {
                            if (RectTransformUtility.RectangleContainsScreenPoint(joystick_Movement.ShiftRange, t.position)) {
                                if (!EventSystem.current.IsPointerOverGameObject(t.fingerId)) {
                                    joystick_Movement.PointerDown(t, false);
                                }
                            }
                            else if (RectTransformUtility.RectangleContainsScreenPoint(joystick_Attack.ShiftRange, t.position)) {
                                if (!joystick_Attack.isCooldowning &! RectTransformUtility.RectangleContainsScreenPoint(RT_AttackJoyStick, t.position)) {
                                    if (!EventSystem.current.IsPointerOverGameObject(t.fingerId)) {
                                        joystick_Attack.PointerDown(t, false);
                                    }
                                }
                                else {
                                    if (!EventSystem.current.IsPointerOverGameObject(t.fingerId)) {
                                        joystick_Attack.PointerDown(t, true);
                                    }
                                }
                            }
                        }
                        else if (t.phase == TouchPhase.Moved) {
                            if (joystick_Movement.IsDetectingJoystick) {
                                joystick_Movement.UpdatePos(t);
                            }
                            if (joystick_Attack.IsDetectingJoystick) {
                                joystick_Attack.UpdatePos(t);
                            }
                        }
                        else if (t.phase == TouchPhase.Ended) {
                            if (joystick_Movement.IsDetectingJoystick) {
                                joystick_Movement.PointerUp(t);
                            }
                            if (joystick_Attack.IsDetectingJoystick) {
                                joystick_Attack.PointerUp(t);
                            }
                        }
                    }
                }
            }

            #endregion

            #region == Setting Panel Detect ==
            if (DetectingTouch) {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                    if (Input.GetMouseButton(0) &&
                            !RectTransformUtility.RectangleContainsScreenPoint(RT_settingPanel, Input.mousePosition)) {
                        _CloseSettingPanel();
                    }
                }
                else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                    foreach (var t in Input.touches) {
                        if (t.phase == TouchPhase.Began) {
                            touchFingerID = t.fingerId;
                        }
                        if (t.phase == TouchPhase.Ended && t.fingerId == touchFingerID) {
                            if (!RectTransformUtility.RectangleContainsScreenPoint(RT_settingPanel, t.position)) {
                                _CloseSettingPanel();
                                touchFingerID = -1;
                            }
                        }

                    }
                }
            }
            #endregion

            _lerping.Update();
        }

        public void HealthBarUpdate() {
            if (playerCtrl == null) playerCtrl = PlayerCtrl.Instance;

            _lerping.SetTarget((float)playerCtrl.Health / playerCtrl.MaxHealth, CurveLerping.CurveType.EaseOut_Fast);
            healthBarText.text = playerCtrl.Health + "/" + playerCtrl.MaxHealth;
        }

        #region --Button--

        public void _ClosePanel(GameObject panel) {
            panel.SetActive(false);
        }
        public void _ToggleTurtlePanel() {
            turtleInfoPanel.gameObject.SetActive(!turtleInfoPanel.gameObject.activeSelf);
        }

        public void _ToggleBag() {
            bag.gameObject.SetActive(!bag.gameObject.activeSelf);
        }
        public void _SwitchLanguage() {
            //            Language nextLanguage;
            //            Language l;

            //#if UNITY_EDITOR
            //            Enum.TryParse(EditorPrefs.GetString("Language"), out l);
            //            nextLanguage = (Language)(((int)l + 1) % Enum.GetValues(typeof(Language)).Length);
            //            LanguageMgr.currentLanguage = nextLanguage;
            //#endif

            //            if (Application.platform == RuntimePlatform.Android) {
            //                Enum.TryParse(PlayerPrefs.GetString("Language"), out l);
            //                nextLanguage = (Language)(((int)l + 1) % Enum.GetValues(typeof(Language)).Length);
            //                LanguageMgr.currentLanguage = nextLanguage;
            //            }

            LanguageMgr.Instance.ChangeLanguage();
        }

        public void _OpenSettingPanel() {
            settingPanel.SetActive(true);
            joystick_Attack.IsDetectingJoystick = false;
            joystick_Movement.IsDetectingJoystick = false;
            joystick_Movement.IsMoveing = false;

            mGM.StopGame();
            StartCoroutine(WaitforSecond(0.1f));
        }
        public void _CloseSettingPanel() {
            joystick_Attack.IsDetectingJoystick = false;
            joystick_Movement.IsDetectingJoystick = false;

            settingPanel.SetActive(false);
            DetectingTouch = false;
            mGM.ResumeGame();
        }

        IEnumerator WaitforSecond(float time) {
            float startTime = Time.realtimeSinceStartup;
            float elapsedTime = 0;

            while (elapsedTime < time) {
                elapsedTime = Time.realtimeSinceStartup - startTime;
                yield return null;
            }
            DetectingTouch = true;
        }
        
        public void _SaveGame() {
            MyGameManager.Instance.SaveGame();
            mGM.ExitGame();
        }
        #endregion


    }
}
