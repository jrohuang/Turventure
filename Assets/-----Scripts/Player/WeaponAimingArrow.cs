using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Player.Weapon{

    public class WeaponAimingArrow : MonoBehaviour{

        public Animator animator;
        private Vector3 initalScale;

        private void Awake() {
            animator = GetComponent<Animator>();
            initalScale = transform.localScale;
        }
        private void LateUpdate() {
            transform.localScale = initalScale;
        }
        public void ShowArrow(bool show) {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            animator.Play(show ? "Show" : "Hide");
        }
    }
}
