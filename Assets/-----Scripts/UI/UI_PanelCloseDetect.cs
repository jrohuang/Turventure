using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LittleTurtle.UI {
    public class UI_PanelCloseDetect : MonoBehaviour {

        public static List<GameObject> PanelsActive = new List<GameObject>();
        public static bool isClosing = false;

        private RectTransform panel;
        private float deltaTime;
        private int touchID = -1;
        private void OnEnable() {
            PanelsActive.Add(gameObject);
        }
        private void OnDisable() {
            //PanelsActive.Remove(gameObject);
        }
        private void Awake() {
            panel = GetComponent<RectTransform>();
        }
        void Update(){

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {

                if (Input.GetMouseButtonDown(0)) {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition)) {
                        deltaTime = 0;
                    }
                    deltaTime = 0;
                }
                if (Input.GetMouseButton(0)) {
                    deltaTime += Time.deltaTime;
                }
                if (Input.GetMouseButtonUp(0)) {
                    if (deltaTime < 0.08f) {
                        if (!RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition)
                            &! EventSystem.current.IsPointerOverGameObject() &! isClosing) {
                            ClosePanel();
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape) & !isClosing) {
                    ClosePanel();
                }
            }
            else if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {

                foreach (Touch touch in Input.touches) {

                    if (touch.phase == TouchPhase.Began) {
                        if (!RectTransformUtility.RectangleContainsScreenPoint(panel, touch.position)) {
                            deltaTime = 0;
                        }

                        touchID = touch.fingerId;
                        deltaTime = 0;
                    }

                    if (touch.phase == TouchPhase.Moved && touch.fingerId == touchID) {
                        deltaTime += Time.deltaTime;
                    }

                    if (touch.phase == TouchPhase.Ended && touch.fingerId == touchID) {
                        if (deltaTime < 0.08f) {

                            if (!RectTransformUtility.RectangleContainsScreenPoint(panel, touch.position)) {
                                PointerEventData data = new PointerEventData(EventSystem.current);
                                data.position = touch.position;

                                List<RaycastResult> result = new List<RaycastResult>();
                                EventSystem.current.RaycastAll(data, result);

                                if (result.Count == 0 &! isClosing) {
                                    ClosePanel();
                                }
                            }
                        }
                    }

                }
            }

        }

        private void ClosePanel() {
            isClosing = true;
            StartCoroutine(SetIsClosingFalse());
        }

        private IEnumerator SetIsClosingFalse() {
            yield return null;

            foreach (var item in PanelsActive) {
                if (item) {
                    item.SetActive(false);
                }
            }
            PanelsActive.Clear();
            isClosing = false;
        }

    }
}
