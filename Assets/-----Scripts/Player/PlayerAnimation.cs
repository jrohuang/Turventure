using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Player{
    public class PlayerAnimation : MonoBehaviour{

        public PlayerCtrl playerCtrl;

        public void _PunchStart() {
            playerCtrl._PunchStart();
        }
        public void _PunchEnd() {
            playerCtrl._PunchEnd();
        }
    }
}
