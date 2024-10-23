using UnityEngine;
using UnityEngine.EventSystems;

namespace LittleTurtle.UI {

    public abstract class MobileJoystick : MonoBehaviour {

        public float DragRange;

        public RectTransform joystickHandle;
        public RectTransform joystickBackground;
        
        protected Vector2 originalPos;
        protected Vector3 inputDirection;

        [HideInInspector]
        public RectTransform ShiftRange;

        private float DragRangeMin;
        private int fingerId = -1;
        private Vector2 pointerDownPos;

        protected Vector2 Pos;  // current input pos

        private bool isDetectingJoystick = false;
        public bool IsDetectingJoystick { get { return isDetectingJoystick; } set { OnIsDetectingChange(value); } }
        protected virtual void OnIsDetectingChange(bool value) {
            isDetectingJoystick = value;
            if (value == false) {
                inputDirection = Vector3.zero;
                joystickHandle.anchoredPosition = Vector3.zero;
                joystickBackground.position = originalPos;
            }
        }

        protected virtual void Start() {
            DragRangeMin = (joystickBackground.rect.width + joystickBackground.rect.height) / 10f;

            ShiftRange = GetComponent<RectTransform>();
            originalPos = joystickBackground.position;
        }


        #region == Pointer Up & Down ==
        public virtual void PointerDown(bool freezePosWhenPressOnTheImage) {
            
            IsDetectingJoystick = true;
            pointerDownPos = Input.mousePosition;

            if (!freezePosWhenPressOnTheImage) {
                joystickBackground.position = Input.mousePosition;
            }

            OnPointerDown();
        }
        public virtual void PointerDown(Touch touch, bool freezePosWhenPressOnTheImage) {
            IsDetectingJoystick = true;
            fingerId = touch.fingerId;
            pointerDownPos = touch.position;
            Pos = touch.position;

            if (!freezePosWhenPressOnTheImage) {
                joystickBackground.position = touch.position;
            }

            OnPointerDown();
        }

        public virtual void OnPointerDown() {}


        public void PointerUp() {
            OnPointerUp();
        }
        public void PointerUp(Touch touch) {
            if (touch.fingerId != fingerId) return;
            OnPointerUp();
            fingerId = -1;
        }
        public virtual void OnPointerUp() {
            IsDetectingJoystick = false;
        }
        #endregion

        public void UpdatePos(Touch touch) {
            if (touch.fingerId == fingerId) {
                Pos = touch.position;
            }
        }

        public virtual void Update() {
            if (!isDetectingJoystick) { return; }
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                Pos = Input.mousePosition;
            }

            if (Vector2.Distance(Pos, pointerDownPos) < DragRangeMin) {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, Pos, null, out Vector2 position);
            position.x = (position.x / joystickBackground.rect.size.x);
            position.y = (position.y / joystickBackground.rect.size.y);
            
            inputDirection = new Vector3(position.x, 0, position.y);

            inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;
            joystickHandle.anchoredPosition = new Vector3(inputDirection.x * (joystickBackground.rect.size.x / DragRange),
                                                            inputDirection.z * (joystickBackground.rect.size.y / DragRange));
            PointerDrag();
        }
        public abstract void PointerDrag();
    }
}
