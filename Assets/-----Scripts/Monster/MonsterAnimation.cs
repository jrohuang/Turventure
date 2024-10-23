using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LittleTurtle.Enemy {

    public class MonsterAnimation : MonoBehaviour {

        public Action Action_DieAnimationEnd;
        public Animator animator;

        public void _DieAnimationEnd() {
            Action_DieAnimationEnd?.Invoke();
            Action_DieAnimationEnd = null;
        }

        public void SwitchAnimationType(MonsterAnimatorType type) {
            switch (type) {
                case MonsterAnimatorType.idel:
                    animator.SetTrigger("Idel");
                    break;
                case MonsterAnimatorType.attack:
                    animator.SetTrigger("Attack");
                    break;
                case MonsterAnimatorType.move:
                    animator.SetTrigger("Move");
                    break;
                case MonsterAnimatorType.Die:
                    animator.Play("Die");
                    break;
            }
        }

    }

    public enum MonsterAnimatorType {
        idel,
        move,
        attack,
        Die
    }

}