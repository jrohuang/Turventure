using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LittleTurtle.UI {

    public class Joystick_Attack : MobileJoystick {

        public static Joystick_Attack Instance;

        public Image backgroundImage;
        public RectTransform CancelAttackRange;
        private static PlayerCtrl playerCtrl;
        private float deltaTime = 0;

        public bool isCooldowning = false;
        public float CooldownTime;


        private void Awake() {
            Instance = this;
        }

        protected override void Start() {
            base.Start();

            isCooldowning = false;
            backgroundImage.fillAmount = 1;
            playerCtrl = PlayerCtrl.Instance;
        }

        public override void Update() {
            base.Update();

            if (isCooldowning) {
                deltaTime += Time.deltaTime;
                backgroundImage.fillAmount = deltaTime / CooldownTime;
            }
        }

        public override void PointerDrag() {
            playerCtrl.SetIsAiming(true);
            playerCtrl.SetRotation(new Vector2(inputDirection.x, inputDirection.z));

            if (RectTransformUtility.RectangleContainsScreenPoint(CancelAttackRange, Pos)) {
                IsDetectingJoystick = false;
            }
        }

        public override void OnPointerDown() {
            base.OnPointerDown();
            if (!isCooldowning) {
                playerCtrl.currentWeapon.ShowAimingArrow(true);
            }
        }

        public override void OnPointerUp() {
            base.OnPointerUp();
            if (!isCooldowning) {
                playerCtrl.Attack(inputDirection);
                CooldownWeapon();
            }
            else {
                playerCtrl.SetIsAiming(false);
            }
        }

        protected override void OnIsDetectingChange(bool value) {
            base.OnIsDetectingChange(value);
            if (!value) {
                playerCtrl.currentWeapon.ShowAimingArrow(false);
            }
        }

        private Coroutine coroutine_cooldown;
        public void CooldownWeapon() {
            StopAllCoroutines();
            StartCoroutine(_cooldown());
        }

        public IEnumerator _cooldown() {
            isCooldowning = true;
            deltaTime = 0;

            yield return new WaitForSeconds(CooldownTime);
            isCooldowning = false;
            if (playerCtrl.aiming) playerCtrl.currentWeapon.ShowAimingArrow(true);
        }
    }
}