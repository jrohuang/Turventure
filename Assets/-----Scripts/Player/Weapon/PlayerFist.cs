using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Player.Weapon {

    public class PlayerFist : PlayerWeapon {

        public override void Start() {
            base.Start();
            animator = playerCtrl.animator_player;
        }
        public override void Attack() {
            base.Attack();
            isAttacking = true;
        }
    }

}