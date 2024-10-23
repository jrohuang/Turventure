using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Enemy {

    public class Monster_1_HugeRoach : Monster {

        public GameObject weapon;

        public override void Attack() {
            Attack_General(weapon);
        }

    }
}