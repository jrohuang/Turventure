using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using LittleTurtle.Enemy;
using LittleTurtle.UI;
using LittleTurtle.Inventory;

namespace LittleTurtle.Player.Weapon {
    public class PlayerWeapon : MonoBehaviour {

        public Equipment_Weapon data;
        public Animator animator;

        [Header("Animation")]
        public WeaponAimingArrow arrow;
        public GameObject HitEffect;
        public Transform EffefctPos;

        private bool hasHitEffect;
        protected bool isAttacking = false;
        protected PlayerCtrl playerCtrl;

        public virtual void Start() {
            hasHitEffect = (HitEffect != null && EffefctPos != null) ? true : false;
            playerCtrl = PlayerCtrl.Instance;
        }

        public virtual void Attack() {
            animator.SetTrigger("Attack");
        }

        public virtual void HitMonster(Monster monster) {
            playerCtrl.audioSource.PlayOneShot(data.audio_Hit);
            monster.HealthChange(-data.Damage);

            // hit effects
            if (hasHitEffect) {
                Instantiate(HitEffect, EffefctPos.position, Quaternion.identity);
            }
        }
        #region --Animation--
        public void _AttackStart() {
            isAttacking = true;
            playerCtrl.audioSource.PlayOneShot(data.audio_Attack);
        }
        public virtual void _AttackEnd() {
            isAttacking = false;
            monsterColliders.Clear();
            playerCtrl.SetIsAiming(false);
        }

        public void ShowAimingArrow(bool show) {
            arrow.ShowArrow(show);
        }

        #endregion


        private List<Collider2D> monsterColliders = new List<Collider2D>();  // The record of monsters that have been hit
        private void OnTriggerStay2D(Collider2D collision) {
            if (isAttacking) {
                if (collision.transform.tag == "Monster") {

                    if (monsterColliders.Contains(collision)) {
                        return;
                    }
                    HitMonster(collision.transform.GetComponent<Monster>());
                    monsterColliders.Add(collision);
                }
            }
        }
    }
}