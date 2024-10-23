using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LittleTurtle.UI {

    public class Joystick_Movement : MobileJoystick {

        public static Joystick_Movement Instance;
        private PlayerCtrl playerCtrl;

        private bool _isMoveing;
        public bool IsMoveing { get => _isMoveing; set { _isMoveing = value; ChangeIsMoveing(); }  }

        public void Awake() {
            if (Instance == null) Instance = this;
        }

        protected override void Start() {
            base.Start();

            playerCtrl = PlayerCtrl.Instance;
        }

        private void ChangeIsMoveing() {
            playerCtrl.SwitchIsMoveing();
        }

        public override void PointerDrag() {
            IsMoveing = true;
            PlayerCtrl.Instance.SetMovement(new Vector2(inputDirection.x, inputDirection.z));
        }

        public override void OnPointerUp() {
            base.OnPointerUp();
            IsMoveing = false;
        }

    }
}